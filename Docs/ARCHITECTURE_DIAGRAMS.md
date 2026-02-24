# Bluetooth Plugin - Architecture Diagrams

> **Note**: These Mermaid diagrams visualize the architecture patterns, dependencies, and flows in the Bluetooth Plugin solution. View this file in GitHub, GitLab, VS Code with Mermaid preview, or any Mermaid-compatible markdown viewer.

---

## 📦 Project Dependency Structure

```mermaid
graph TB
    subgraph "Tier 1: Abstractions (Interfaces)"
        ABS[Bluetooth.Abstractions]
        ABS_SCAN[Bluetooth.Abstractions.Scanning]
        ABS_BROAD[Bluetooth.Abstractions.Broadcasting]
    end

    subgraph "Tier 2: Core (Base Classes)"
        CORE[Bluetooth.Core]
        CORE_SCAN[Bluetooth.Core.Scanning]
        CORE_BROAD[Bluetooth.Core.Broadcasting]
    end

    subgraph "Tier 3: Platform Implementations"
        APPLE[Bluetooth.Maui.Platforms.Apple]
        ANDROID[Bluetooth.Maui.Platforms.Droid]
        WINDOWS[Bluetooth.Maui.Platforms.Win]
        DOTNET[Bluetooth.Maui.Platforms.DotNetCore]
    end

    subgraph "Tier 4: Composition"
        MAUI[Bluetooth.Maui]
    end

    SAMPLE[Bluetooth.Maui.Sample.Scanner]

    %% Dependencies
    CORE --> ABS
    CORE_SCAN --> ABS & ABS_SCAN & CORE
    CORE_BROAD --> ABS & ABS_BROAD & CORE

    APPLE --> ABS & ABS_SCAN & ABS_BROAD & CORE & CORE_SCAN & CORE_BROAD
    ANDROID --> ABS & ABS_SCAN & ABS_BROAD & CORE & CORE_SCAN & CORE_BROAD
    WINDOWS --> ABS & ABS_SCAN & ABS_BROAD & CORE & CORE_SCAN & CORE_BROAD
    DOTNET --> ABS & ABS_SCAN & ABS_BROAD & CORE & CORE_SCAN & CORE_BROAD

    MAUI --> APPLE & ANDROID & WINDOWS & DOTNET
    SAMPLE --> MAUI

    %% Styles - thick colored borders, transparent fills (works in light/dark mode)
    style ABS stroke:#2196F3,stroke-width:3px
    style ABS_SCAN stroke:#2196F3,stroke-width:3px
    style ABS_BROAD stroke:#2196F3,stroke-width:3px
    style CORE stroke:#FF9800,stroke-width:3px
    style CORE_SCAN stroke:#FF9800,stroke-width:3px
    style CORE_BROAD stroke:#FF9800,stroke-width:3px
    style APPLE stroke:#9C27B0,stroke-width:3px
    style ANDROID stroke:#9C27B0,stroke-width:3px
    style WINDOWS stroke:#9C27B0,stroke-width:3px
    style DOTNET stroke:#9C27B0,stroke-width:3px
    style MAUI stroke:#4CAF50,stroke-width:3px
    style SAMPLE stroke:#E91E63,stroke-width:3px
```

---

## 🏛️ Three-Tier Entity Pattern (Scanner Example)

```mermaid
classDiagram
    class IBluetoothScanner {
        <<interface>>
        +StartScanningAsync()
        +StopScanningAsync()
        +ClearDevicesAsync()
        +GetDevice()
        +GetDeviceOrDefault()
        +bool IsRunning
        +ObservableCollection Devices
        +event Starting
        +event Started
    }

    class BaseBluetoothScanner {
        <<abstract>>
        -TaskCompletionSource StartTcs
        -TaskCompletionSource StopTcs
        -SemaphoreSlim _operationSemaphore
        +StartScanningAsync()
        +StopScanningAsync()
        #abstract NativeStartAsync()
        #abstract NativeStopAsync()
        #OnStartSucceeded()
        #OnStartFailed()
        #LogScannerStarting()
    }

    class AppleBluetoothScanner {
        -CbCentralManagerWrapper _wrapper
        #NativeStartAsync()
        #NativeStopAsync()
        +DidDiscoverPeripheral()
    }

    class AndroidBluetoothScanner {
        -BluetoothLeScanner _scanner
        -ScanCallbackProxy _callback
        #NativeStartAsync()
        #NativeStopAsync()
        +OnScanResult()
    }

    class WindowsBluetoothScanner {
        -BluetoothLeAdvertisementWatcher _watcher
        #NativeStartAsync()
        #NativeStopAsync()
        +OnAdvertisementReceived()
    }

    IBluetoothScanner <|.. BaseBluetoothScanner : implements
    BaseBluetoothScanner <|-- AppleBluetoothScanner : extends
    BaseBluetoothScanner <|-- AndroidBluetoothScanner : extends
    BaseBluetoothScanner <|-- WindowsBluetoothScanner : extends
```

---

## 🏭 Factory Pattern with Nested Request Records

```mermaid
classDiagram
    class IBluetoothRemoteDeviceFactory {
        <<interface>>
        +CreateDevice(scanner, request)
    }

    class BluetoothRemoteDeviceFactoryRequest {
        <<record>>
        #BluetoothRemoteDeviceFactoryRequest(id, name)
        +string Id
        +string Name
    }

    class BaseBluetoothDeviceFactory {
        <<abstract>>
        #IBluetoothRemoteServiceFactory ServiceFactory
        #IBluetoothRssiConverter RssiConverter
        +abstract CreateDevice()
    }

    class AppleBluetoothRemoteDeviceFactoryRequest {
        <<record>>
        +AppleBluetoothRemoteDeviceFactoryRequest(id, name, peripheral)
        +CBPeripheral NativePeripheral
    }

    class AppleBluetoothDeviceFactory {
        -IBluetoothRemoteL2CapChannelFactory _l2CapFactory
        +CreateDevice()
    }

    class AndroidBluetoothRemoteDeviceFactoryRequest {
        <<record>>
        +AndroidBluetoothRemoteDeviceFactoryRequest(id, name, device)
        +BluetoothDevice NativeDevice
    }

    class AndroidBluetoothDeviceFactory {
        -IBluetoothRemoteL2CapChannelFactory _l2CapFactory
        +CreateDevice()
    }

    IBluetoothRemoteDeviceFactory o-- BluetoothRemoteDeviceFactoryRequest : nested
    IBluetoothRemoteDeviceFactory <|.. BaseBluetoothDeviceFactory : implements
    BaseBluetoothDeviceFactory <|-- AppleBluetoothDeviceFactory : extends
    BaseBluetoothDeviceFactory <|-- AndroidBluetoothDeviceFactory : extends
    BluetoothRemoteDeviceFactoryRequest <|-- AppleBluetoothRemoteDeviceFactoryRequest : extends
    BluetoothRemoteDeviceFactoryRequest <|-- AndroidBluetoothRemoteDeviceFactoryRequest : extends
```

---

## 🔄 TaskCompletionSource (TCS) Async Coordination Flow

```mermaid
sequenceDiagram
    participant User
    participant Base as BaseBluetoothScanner
    participant Platform as AppleBluetoothScanner
    participant Native as CBCentralManager
    participant TCS as TaskCompletionSource

    User->>Base: StartScanningAsync()

    Base->>Base: Check preconditions
    alt Already starting/started
        Base-->>Base: Merge concurrent call
        Base-->>User: Return existing task
    end

    Base->>TCS: Create new StartTcs
    Base->>Base: IsStarting = true
    Base->>Base: Invoke Starting event

    Base->>Platform: NativeStartAsync()
    Platform->>Native: ScanForPeripherals()
    Native-->>Platform: Returns immediately
    Platform-->>Base: Returns ValueTask.CompletedTask

    Base->>TCS: await StartTcs.Task.WaitBetterAsync()
    Note over Base,TCS: Waiting for native callback...

    Native->>Platform: DidUpdateState(PoweredOn)
    Platform->>Base: OnStartSucceeded()
    Base->>Base: IsRunning = true
    Base->>TCS: TrySetResult()

    TCS-->>Base: Task completes
    Base->>Base: Verify IsRunning == true
    Base->>Base: IsStarting = false
    Base->>Base: Invoke Started event
    Base-->>User: Success

    alt If native fails
        Native->>Platform: DidUpdateState(Error)
        Platform->>Base: OnStartFailed(exception)
        Base->>TCS: TrySetException()
        TCS-->>Base: Task throws
        Base-->>User: Exception propagated
    end
```

---

## 🧬 Complete Entity Inheritance Hierarchy

```mermaid
classDiagram
    %% Scanner Layer
    class IBluetoothScanner {
        <<interface>>
    }
    class BaseBluetoothScanner {
        <<abstract>>
    }
    IBluetoothScanner <|.. BaseBluetoothScanner
    BaseBluetoothScanner <|-- AppleBluetoothScanner
    BaseBluetoothScanner <|-- AndroidBluetoothScanner
    BaseBluetoothScanner <|-- WindowsBluetoothScanner

    %% Device Layer
    class IBluetoothRemoteDevice {
        <<interface>>
    }
    class BaseBluetoothRemoteDevice {
        <<abstract>>
    }
    IBluetoothRemoteDevice <|.. BaseBluetoothRemoteDevice
    BaseBluetoothRemoteDevice <|-- AppleBluetoothRemoteDevice
    BaseBluetoothRemoteDevice <|-- AndroidBluetoothRemoteDevice
    BaseBluetoothRemoteDevice <|-- WindowsBluetoothRemoteDevice

    %% Service Layer
    class IBluetoothRemoteService {
        <<interface>>
    }
    class BaseBluetoothRemoteService {
        <<abstract>>
    }
    IBluetoothRemoteService <|.. BaseBluetoothRemoteService
    BaseBluetoothRemoteService <|-- AppleBluetoothRemoteService
    BaseBluetoothRemoteService <|-- AndroidBluetoothRemoteService
    BaseBluetoothRemoteService <|-- WindowsBluetoothRemoteService

    %% Characteristic Layer
    class IBluetoothRemoteCharacteristic {
        <<interface>>
    }
    class BaseBluetoothRemoteCharacteristic {
        <<abstract>>
    }
    IBluetoothRemoteCharacteristic <|.. BaseBluetoothRemoteCharacteristic
    BaseBluetoothRemoteCharacteristic <|-- AppleBluetoothRemoteCharacteristic
    BaseBluetoothRemoteCharacteristic <|-- AndroidBluetoothRemoteCharacteristic
    BaseBluetoothRemoteCharacteristic <|-- WindowsBluetoothRemoteCharacteristic

    %% Descriptor Layer
    class IBluetoothRemoteDescriptor {
        <<interface>>
    }
    class BaseBluetoothRemoteDescriptor {
        <<abstract>>
    }
    IBluetoothRemoteDescriptor <|.. BaseBluetoothRemoteDescriptor
    BaseBluetoothRemoteDescriptor <|-- AppleBluetoothRemoteDescriptor
    BaseBluetoothRemoteDescriptor <|-- AndroidBluetoothRemoteDescriptor
    BaseBluetoothRemoteDescriptor <|-- WindowsBluetoothRemoteDescriptor

    %% L2CAP Layer
    class IBluetoothL2CapChannel {
        <<interface>>
    }
    class BaseBluetoothRemoteL2CapChannel {
        <<abstract>>
    }
    IBluetoothL2CapChannel <|.. BaseBluetoothRemoteL2CapChannel
    BaseBluetoothRemoteL2CapChannel <|-- AppleBluetoothRemoteL2CapChannel
    BaseBluetoothRemoteL2CapChannel <|-- AndroidBluetoothRemoteL2CapChannel

    %% Relationships
    IBluetoothScanner o-- IBluetoothRemoteDevice : devices
    IBluetoothRemoteDevice o-- IBluetoothRemoteService : services
    IBluetoothRemoteService o-- IBluetoothRemoteCharacteristic : characteristics
    IBluetoothRemoteCharacteristic o-- IBluetoothRemoteDescriptor : descriptors
    IBluetoothRemoteDevice o-- IBluetoothL2CapChannel : L2CAP channels
```

---

## 🔗 Scanner-to-Device-to-Service-to-Characteristic Flow

```mermaid
flowchart LR
    subgraph Scanner["IBluetoothScanner"]
        StartScanning[StartScanningAsync]
        Devices[ObservableCollection&lt;Device&gt;]
    end

    subgraph Device["IBluetoothRemoteDevice"]
        Connect[ConnectAsync]
        ExploreServices[ExploreServicesAsync]
        Services[ObservableCollection&lt;Service&gt;]
        OpenL2Cap[OpenL2CapChannelAsync]
    end

    subgraph Service["IBluetoothRemoteService"]
        ExploreChars[ExploreCharacteristicsAsync]
        Chars[ObservableCollection&lt;Char&gt;]
    end

    subgraph Char["IBluetoothRemoteCharacteristic"]
        Read[ReadValueAsync]
        Write[WriteValueAsync]
        Listen[StartListeningAsync]
        ExploreDesc[ExploreDescriptorsAsync]
    end

    subgraph L2Cap["IBluetoothL2CapChannel"]
        Open[OpenAsync]
        ReadData[ReadAsync]
        WriteData[WriteAsync]
        DataReceived[DataReceived Event]
    end

    StartScanning --> Devices
    Devices --> Connect
    Connect --> ExploreServices
    ExploreServices --> Services
    Services --> ExploreChars
    ExploreChars --> Chars
    Chars --> Read & Write & Listen & ExploreDesc

    Connect --> OpenL2Cap
    OpenL2Cap --> Open
    Open --> ReadData & WriteData
    WriteData --> DataReceived

    %% Styles - thick colored borders (works in light/dark mode)
    style Scanner stroke:#2196F3,stroke-width:3px
    style Device stroke:#FF9800,stroke-width:3px
    style Service stroke:#9C27B0,stroke-width:3px
    style Char stroke:#4CAF50,stroke-width:3px
    style L2Cap stroke:#E91E63,stroke-width:3px
```

---

## 🏗️ Dependency Injection Hierarchy

```mermaid
flowchart TD
    Start[IServiceCollection]

    Start --> AddBT[AddBluetoothServices]

    AddBT --> Core[AddBluetoothCoreServices]
    AddBT --> Platform{Platform Check}

    Core --> CoreScan[AddBluetoothCoreScanningServices]
    Core --> CoreBroad[AddBluetoothCoreBroadcastingServices]

    Platform -->|iOS/macOS| Apple[AddBluetoothMauiAppleServices]
    Platform -->|Android| Android[AddBluetoothMauiDroidServices]
    Platform -->|Windows| Windows[AddBluetoothMauiWinServices]
    Platform -->|Other| DotNet[AddBluetoothMauiDotNetCoreServices]

    Apple --> AppleScan[AddBluetoothMauiAppleScanningServices]
    Apple --> AppleBroad[AddBluetoothMauiAppleBroadcastingServices]

    Android --> AndroidScan[AddBluetoothMauiDroidScanningServices]
    Android --> AndroidBroad[AddBluetoothMauiDroidBroadcastingServices]

    Windows --> WinScan[AddBluetoothMauiWinScanningServices]
    Windows --> WinBroad[AddBluetoothMauiWinBroadcastingServices]

    DotNet --> DotNetScan[AddBluetoothMauiDotNetCoreScanningServices]
    DotNet --> DotNetBroad[AddBluetoothMauiDotNetCoreBroadcastingServices]

    AppleScan --> RegisterApple[Register Factories:<br/>AppleBluetoothDeviceFactory<br/>AppleBluetoothServiceFactory<br/>AppleBluetoothCharacteristicFactory<br/>AppleBluetoothDescriptorFactory<br/>AppleBluetoothRemoteL2CapChannelFactory]

    AndroidScan --> RegisterAndroid[Register Factories:<br/>AndroidBluetoothDeviceFactory<br/>AndroidBluetoothServiceFactory<br/>AndroidBluetoothCharacteristicFactory<br/>AndroidBluetoothDescriptorFactory<br/>AndroidBluetoothRemoteL2CapChannelFactory]

    %% Styles - thick colored borders (works in light/dark mode)
    style AddBT stroke:#4CAF50,stroke-width:3px
    style Core stroke:#FF9800,stroke-width:3px
    style Platform stroke:#E91E63,stroke-width:3px
    style Apple stroke:#9C27B0,stroke-width:3px
    style Android stroke:#9C27B0,stroke-width:3px
    style Windows stroke:#9C27B0,stroke-width:3px
    style DotNet stroke:#9C27B0,stroke-width:3px
```

---

## 🎯 Factory Creation Flow (Device Example)

```mermaid
sequenceDiagram
    participant Scanner
    participant DeviceFactory as IBluetoothRemoteDeviceFactory
    participant PlatformFactory as AppleBluetoothDeviceFactory
    participant ServiceFactory as IBluetoothRemoteServiceFactory
    participant L2CapFactory as IBluetoothRemoteL2CapChannelFactory
    participant Device as AppleBluetoothRemoteDevice

    Scanner->>Scanner: Discover CBPeripheral
    Scanner->>Scanner: Create AppleBluetoothRemoteDeviceFactoryRequest<br/>(id, name, CBPeripheral)

    Scanner->>DeviceFactory: CreateDevice(scanner, request)
    DeviceFactory->>PlatformFactory: CreateDevice(scanner, request)

    PlatformFactory->>PlatformFactory: Validate request type<br/>(must be AppleBluetoothRemoteDeviceFactoryRequest)

    PlatformFactory->>Device: new AppleBluetoothRemoteDevice(<br/>scanner,<br/>request,<br/>ServiceFactory,<br/>L2CapFactory,<br/>RssiConverter)

    Device->>Device: Store factories for later use

    Device-->>PlatformFactory: Return device instance
    PlatformFactory-->>DeviceFactory: Return device
    DeviceFactory-->>Scanner: Return device

    Scanner->>Scanner: Add to Devices collection

    Note over Scanner,Device: Device can now create services<br/>and L2CAP channels using injected factories
```

---

## 🧱 Native Wrapper Pattern (Apple Example)

```mermaid
classDiagram
    class CBCentralManager {
        <<iOS SDK>>
        +ScanForPeripherals()
        +ConnectPeripheral()
    }

    class CBCentralManagerDelegate {
        <<iOS Protocol>>
        +DidDiscoverPeripheral()
        +DidConnectPeripheral()
        +DidFailToConnect()
    }

    class CbCentralManagerWrapper {
        +CBCentralManager CbCentralManager
        -NativeDelegate _nativeDelegate
        -ICbCentralManagerDelegate _delegate
        +ScanForPeripherals()
        +StopScan()
    }

    class ICbCentralManagerDelegate {
        <<interface>>
        +DidDiscoverPeripheral()
        +DidConnectPeripheral()
        +DidFailToConnect()
    }

    class NativeDelegate {
        -ICbCentralManagerDelegate _delegate
        +override DidDiscoverPeripheral()
    }

    class AppleBluetoothScanner {
        -CbCentralManagerWrapper _wrapper
        +NativeStartAsync()
        +NativeStopAsync()
    }

    CBCentralManager <|-- NativeDelegate : implements
    CbCentralManagerWrapper o-- CBCentralManager : wraps
    CbCentralManagerWrapper *-- NativeDelegate : contains
    CbCentralManagerWrapper *-- ICbCentralManagerDelegate : defines
    NativeDelegate o-- ICbCentralManagerDelegate : uses
    AppleBluetoothScanner ..|> ICbCentralManagerDelegate : implements
    AppleBluetoothScanner o-- CbCentralManagerWrapper : uses
```

---

## 📊 Exception Hierarchy

```mermaid
classDiagram
    class Exception {
        <<.NET>>
    }

    class BluetoothException {
        <<abstract>>
    }

    class BluetoothScanningException {
        <<abstract>>
    }

    class BluetoothBroadcastingException {
        <<abstract>>
    }

    class BluetoothPermissionException

    class ScannerException
    class DeviceException
    class ServiceException
    class CharacteristicException
    class DescriptorException

    class ScannerFailedToStartException
    class ScannerIsAlreadyStartedException
    class DeviceFailedToConnectException
    class DeviceNotConnectedException
    class CharacteristicCantReadException
    class CharacteristicReadException

    class AppleNativeBluetoothException
    class AndroidNativeBluetoothException
    class WindowsNativeBluetoothException

    Exception <|-- BluetoothException
    BluetoothException <|-- BluetoothScanningException
    BluetoothException <|-- BluetoothBroadcastingException
    BluetoothException <|-- BluetoothPermissionException

    BluetoothScanningException <|-- ScannerException
    BluetoothScanningException <|-- DeviceException
    BluetoothScanningException <|-- ServiceException
    BluetoothScanningException <|-- CharacteristicException
    BluetoothScanningException <|-- DescriptorException

    ScannerException <|-- ScannerFailedToStartException
    ScannerException <|-- ScannerIsAlreadyStartedException
    DeviceException <|-- DeviceFailedToConnectException
    DeviceException <|-- DeviceNotConnectedException
    CharacteristicException <|-- CharacteristicCantReadException
    CharacteristicException <|-- CharacteristicReadException

    BluetoothException <|-- AppleNativeBluetoothException
    BluetoothException <|-- AndroidNativeBluetoothException
    BluetoothException <|-- WindowsNativeBluetoothException
```

---

## 🔍 Partial Class File Organization

```mermaid
graph TD
    subgraph "IBluetoothRemoteDevice (Interface)"
        I1[IBluetoothRemoteDevice.cs<br/>Core identity]
        I2[.Connection.cs<br/>Connect/Disconnect]
        I3[.ServiceList.cs<br/>Service exploration]
        I4[.SignalStrength.cs<br/>RSSI]
        I5[.Mtu.cs<br/>MTU requests]
        I6[.Phy.cs<br/>PHY selection]
        I7[.L2Cap.cs<br/>L2CAP channels]
    end

    subgraph "BaseBluetoothRemoteDevice (Base Class)"
        B1[BaseBluetoothRemoteDevice.cs<br/>Core logic]
        B2[.Connection.cs<br/>TCS coordination]
        B3[.ServiceList.cs<br/>Collection management]
        B4[.SignalStrength.cs<br/>RSSI history]
        B5[.Logging.cs<br/>LoggerMessage]
    end

    subgraph "AppleBluetoothRemoteDevice (Platform)"
        P1[AppleBluetoothRemoteDevice.cs<br/>Native implementation]
    end

    I1 -.-> B1
    I2 -.-> B2
    I3 -.-> B3
    I4 -.-> B4
    I5 -.-> B1
    I6 -.-> B1
    I7 -.-> B1

    B1 -.-> P1
    B2 -.-> P1
    B3 -.-> P1
    B4 -.-> P1
    B5 -.-> P1

    %% Styles - thick colored borders (works in light/dark mode)
    style I1 stroke:#2196F3,stroke-width:3px
    style I2 stroke:#2196F3,stroke-width:3px
    style I3 stroke:#2196F3,stroke-width:3px
    style I4 stroke:#2196F3,stroke-width:3px
    style I5 stroke:#2196F3,stroke-width:3px
    style I6 stroke:#2196F3,stroke-width:3px
    style I7 stroke:#2196F3,stroke-width:3px
    style B1 stroke:#FF9800,stroke-width:3px
    style B2 stroke:#FF9800,stroke-width:3px
    style B3 stroke:#FF9800,stroke-width:3px
    style B4 stroke:#FF9800,stroke-width:3px
    style B5 stroke:#FF9800,stroke-width:3px
    style P1 stroke:#9C27B0,stroke-width:3px
```

---

## 📈 Logging Architecture

```mermaid
flowchart TB
    subgraph Core["Core Layer Logging"]
        direction TB
        BaseClass[BaseBluetoothScanner.cs]
        LogFile[BaseBluetoothScanner.Logging.cs]

        BaseClass -.partial class.-> LogFile

        LogFile --> LogMethod["[LoggerMessage]<br/>partial void LogScannerStarting()"]
        LogMethod --> Field["Uses: private readonly ILogger _logger"]
    end

    subgraph Platform["Platform Layer Logging"]
        direction TB
        PlatformClass[AppleBluetoothScanner.cs]
        PlatformLogFile[AppleBluetoothLoggerMessages.cs]

        PlatformLogFile --> ExtMethod["[LoggerMessage]<br/>public static partial void<br/>LogScanStarting(this ILogger)"]
        PlatformClass --> Call["Logger?.LogScanStarting()"]
    end

    BaseClass -->|"LogScannerStarting()"| LogMethod
    PlatformClass -->|"Logger?.LogScanStarting()"| ExtMethod

    %% Styles - thick colored borders (works in light/dark mode)
    style Core stroke:#FF9800,stroke-width:3px
    style Platform stroke:#9C27B0,stroke-width:3px
    style BaseClass stroke:#4CAF50,stroke-width:3px
    style LogFile stroke:#4CAF50,stroke-width:3px
    style PlatformClass stroke:#E91E63,stroke-width:3px
    style PlatformLogFile stroke:#E91E63,stroke-width:3px
```

---

## 🎨 Key Architectural Concepts Summary

```mermaid
mindmap
  root((Bluetooth<br/>Plugin))
    Project Structure
      4 Tiers
        Abstractions
        Core
        Platforms
        Composition
      Strict Dependencies
        One-directional
        PrivateAssets=all
    Patterns
      Three-Tier Entities
        Interface
        Base Class
        Platform Impl
      Factory with Records
        Nested request
        Platform extends
      TCS Coordination
        Create TCS
        Native call
        Await callback
        Verify state
      BaseBindableObject
        Property store
        INotifyPropertyChanged
        WaitForProperty
    Coding Style
      Async Rules
        ConfigureAwait false
        ValueTask vs Task
      Logging
        LoggerMessage
        Partial methods
        Extension methods
      Disposal
        IAsyncDisposable
        DisposeAsyncCore
      Exceptions
        Hierarchy
        Static ThrowIf
```

---

**Generated:** 2026-02-24
**View in:** GitHub, VS Code (Markdown Preview Mermaid Support), or any Mermaid-compatible viewer
