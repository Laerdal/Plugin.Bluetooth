# Permissions Guide

This guide provides comprehensive information about Bluetooth permission handling across iOS, Android, and Windows platforms.

## Overview

Plugin.Bluetooth provides a unified permission model that adapts to platform-specific requirements:

- **Permission Request Strategies**: Choose when and how permissions are requested
- **Platform-Specific Behavior**: Understand how each platform handles permissions
- **Runtime Permission Management**: Check and request permissions programmatically

## Permission Request Strategies

Plugin.Bluetooth offers three strategies for permission management, available for both scanning and connection operations:

### RequestAutomatically (Default)

The library automatically requests permissions before performing operations.

```csharp
// Permissions requested automatically
await scanner.StartScanningAsync();

// Or explicitly specify (same as default)
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
};
await scanner.StartScanningAsync(options);
```

**When to use:**
- Most applications (recommended default)
- Simple apps without complex permission flows
- When you want the library to handle permission dialogs

**Platform behavior:**
- **Android**: Shows permission dialog automatically before scanning/connecting
- **iOS/macOS**: No-op (CoreBluetooth shows system dialog automatically)
- **Windows**: Checks adapter state and requests radio access if needed

### ThrowIfNotGranted

Take explicit control over permission requests. The library throws `BluetoothPermissionException` if permissions are not granted.

```csharp
// Check permissions
if (!await scanner.HasScannerPermissionsAsync())
{
    try
    {
        // Request permissions explicitly
        await scanner.RequestScannerPermissionsAsync();
    }
    catch (BluetoothPermissionException ex)
    {
        // Handle permission denial
        await ShowPermissionDeniedDialog();
        return;
    }
}

// Start operation with explicit strategy
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted
};

try
{
    await scanner.StartScanningAsync(options);
}
catch (BluetoothPermissionException)
{
    // This shouldn't happen if you checked above, but handle just in case
}
```

**When to use:**
- Apps with custom permission request flows
- When you want to show custom UI before system dialogs
- Multi-step wizard-style permission flows
- When you need fine-grained error handling

**Advantages:**
- Complete control over permission timing
- Can show custom explanations before system dialogs
- Better user experience for permission-sensitive features

### AssumeGranted

Skip all permission checks. Use only when you've already verified permissions elsewhere.

```csharp
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.AssumeGranted
};

await scanner.StartScanningAsync(options);
```

**When to use:**
- Testing and development
- When using external permission management libraries
- Apps with custom permission handling outside the library

> **Warning**: Using `AssumeGranted` without actually having permissions will cause runtime errors:
> - **Android**: `SecurityException` from native Bluetooth APIs
> - **iOS**: System will still show permission dialog (cannot be prevented)
> - **Windows**: Operations may fail when accessing Bluetooth APIs

## Permission APIs

### Checking Permissions

Check if permissions are currently granted without triggering a request:

```csharp
using Bluetooth.Abstractions.Scanning;

// Check scanner permissions
bool hasScannerPermissions = await scanner.HasScannerPermissionsAsync();

if (!hasScannerPermissions)
{
    // Show explanation or request permissions
}
```

**Platform behavior:**
- **Android**: Checks `BLUETOOTH_SCAN` (API 31+) or location permissions (older)
- **iOS/macOS**: Checks Bluetooth authorization status
- **Windows**: Checks adapter availability and radio state

### Requesting Permissions

Explicitly request permissions from the user:

```csharp
try
{
    // Request scanner permissions
    await scanner.RequestScannerPermissionsAsync();

    // Optional: Request background location (Android API 29-30 only)
    await scanner.RequestScannerPermissionsAsync(
        requireBackgroundLocation: true
    );
}
catch (BluetoothPermissionException ex)
{
    // Permission denied by user
    Console.WriteLine($"Permission denied: {ex.Message}");

    // Check inner exception for platform-specific details
    if (ex.InnerException is SecurityException secEx)
    {
        // Android-specific handling
    }
}
```

**Platform behavior:**
- **Android**: Shows system permission dialog; can be called multiple times
- **iOS/macOS**: Shows permission dialog on first call; subsequent denials require Settings app
- **Windows**: Checks adapter state and requests radio access if needed

### Using Cancellation Tokens

Support cancellation for long-running permission requests:

```csharp
private CancellationTokenSource _cts = new();

public async Task RequestPermissionsAsync()
{
    try
    {
        await scanner.RequestScannerPermissionsAsync(
            requireBackgroundLocation: false,
            cancellationToken: _cts.Token
        );
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Permission request cancelled");
    }
}

public void CancelPermissionRequest()
{
    _cts.Cancel();
}
```

## Platform-Specific Permission Behavior

### iOS & MacCatalyst

#### Permission Model

iOS uses a simple authorized/not-authorized model managed entirely by CoreBluetooth:

```csharp
// Check current authorization status
bool isAuthorized = await scanner.HasScannerPermissionsAsync();

// Request authorization (shows dialog on first call)
await scanner.RequestScannerPermissionsAsync();
```

#### Authorization States

On iOS, Bluetooth authorization can be in several states:

1. **Not Determined**: User hasn't been asked yet
2. **Denied**: User explicitly denied permission
3. **Authorized**: Permission granted
4. **Restricted**: Permission restricted by device policy

#### First-Time Request

When requesting Bluetooth permission for the first time:

```
┌─────────────────────────────────────────┐
│  "MyApp" Would Like to Use Bluetooth    │
├─────────────────────────────────────────┤
│  This app uses Bluetooth to connect     │
│  to nearby devices                       │
├─────────────────────────────────────────┤
│            Don't Allow    │    OK        │
└─────────────────────────────────────────┘
```

The message comes from `NSBluetoothAlwaysUsageDescription` in `Info.plist`.

#### Subsequent Requests

If the user denies permission:
- **First denial**: System dialog appears again on next request
- **Repeated denials**: iOS shows "Go to Settings" dialog

```csharp
// User denied permission previously
try
{
    await scanner.RequestScannerPermissionsAsync();
}
catch (BluetoothPermissionException)
{
    // Show dialog guiding user to Settings
    await ShowOpenSettingsDialog(
        "Bluetooth access is required. Please enable it in Settings > MyApp > Bluetooth"
    );
}
```

#### Opening Settings

Guide users to Settings to re-enable permissions:

```csharp
// iOS-specific: Open app settings
#if IOS || MACCATALYST
if (UIKit.UIApplication.SharedApplication.CanOpenUrl(
    new Foundation.NSUrl("app-settings:")))
{
    await UIKit.UIApplication.SharedApplication.OpenUrlAsync(
        new Foundation.NSUrl("app-settings:"),
        new UIKit.UIApplicationOpenUrlOptions()
    );
}
#endif
```

#### Key Characteristics

- Permission dialog managed by system (CoreBluetooth)
- Cannot customize dialog appearance
- `RequestAutomatically` is effectively a no-op (system always controls dialog)
- Background scanning requires additional setup (see [Platform Setup](./Platform-Setup.md))

### Android

Android's permission model is the most complex, varying significantly across API levels.

#### API Level Differences

| API Level | Required Permissions | Notes |
|-----------|---------------------|-------|
| 23-28 (Android 6-8.1) | `BLUETOOTH`, `BLUETOOTH_ADMIN`, `ACCESS_FINE_LOCATION` | Location required for scanning |
| 29-30 (Android 10-11) | Above + `ACCESS_BACKGROUND_LOCATION` | Additional permission for background |
| 31+ (Android 12+) | `BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT` | Location no longer required |

#### Modern Android (API 31+)

On Android 12 and above, Bluetooth permissions are separate from location:

```csharp
// Check permissions
bool hasPermissions = await scanner.HasScannerPermissionsAsync();

// Request permissions (shows Bluetooth permission dialog)
await scanner.RequestScannerPermissionsAsync();
```

**Permission Dialog:**
```
┌─────────────────────────────────────────┐
│  Allow MyApp to find, connect to, and   │
│  determine the relative position of      │
│  nearby devices?                         │
├─────────────────────────────────────────┤
│  While using the app                     │
│  Only this time                          │
│  Don't allow                             │
└─────────────────────────────────────────┘
```

#### Legacy Android (API 23-30)

On Android 6-11, location permission is required for Bluetooth scanning:

```csharp
// Check permissions (includes location)
bool hasPermissions = await scanner.HasScannerPermissionsAsync();

// Request permissions (shows location permission dialog)
await scanner.RequestScannerPermissionsAsync();
```

**Why Location?** Bluetooth device MAC addresses can be used to infer physical location, so Android requires location permission for scanning on these API levels.

**Permission Dialog:**
```
┌─────────────────────────────────────────┐
│  Allow MyApp to access this device's    │
│  location?                               │
├─────────────────────────────────────────┤
│  While using the app                     │
│  Only this time                          │
│  Don't allow                             │
└─────────────────────────────────────────┘
```

#### Background Location (API 29-30)

For background scanning on Android 10-11, request background location:

```csharp
var options = new ScanningOptions
{
    RequireBackgroundLocation = true
};

await scanner.StartScanningAsync(options);

// Or request explicitly
await scanner.RequestScannerPermissionsAsync(
    requireBackgroundLocation: true
);
```

**Two-Step Process:**
1. First, user grants foreground location
2. Then, system shows background location dialog

```
┌─────────────────────────────────────────┐
│  Allow MyApp to access this device's    │
│  location all the time?                  │
├─────────────────────────────────────────┤
│  Allow all the time                      │
│  Allow only while using the app          │
│  Don't allow                             │
└─────────────────────────────────────────┘
```

#### "Don't Ask Again" Handling

Android shows a "Don't ask again" checkbox after repeated denials:

```csharp
try
{
    await scanner.RequestScannerPermissionsAsync();
}
catch (BluetoothPermissionException ex)
{
    // Check if user selected "Don't ask again"
    // On Android, subsequent requests will fail immediately

    // Guide user to app settings
    await ShowOpenSettingsDialog(
        "Bluetooth permission is required. Please enable it in Settings > Apps > MyApp > Permissions"
    );
}
```

Opening app settings on Android:

```csharp
#if ANDROID
var intent = new Android.Content.Intent(
    Android.Provider.Settings.ActionApplicationDetailsSettings
);
intent.SetData(Android.Net.Uri.Parse(
    $"package:{Android.App.Application.Context.PackageName}"
));
Android.App.Application.Context.StartActivity(intent);
#endif
```

#### Connection Permissions (API 31+)

Connecting to devices on Android 12+ requires `BLUETOOTH_CONNECT`:

```csharp
// Connection options with permission strategy
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
};

try
{
    await device.ConnectAsync(options);
}
catch (BluetoothPermissionException)
{
    // Handle BLUETOOTH_CONNECT denial
}
```

#### Location Services Requirement (API 23-30)

On Android 6-11, location services must be enabled on the device:

```csharp
// Check if location services are enabled (Android-specific)
#if ANDROID
var locationManager = Android.App.Application.Context.GetSystemService(
    Android.Content.Context.LocationService
) as Android.Locations.LocationManager;

bool locationEnabled = locationManager?.IsLocationEnabled ?? false;

if (!locationEnabled)
{
    // Guide user to enable location services
    await ShowEnableLocationDialog(
        "Please enable Location Services in device settings to scan for Bluetooth devices"
    );
}
#endif
```

### Windows

Windows has the simplest permission model for desktop applications.

#### Permission Model

Windows doesn't show permission dialogs for Bluetooth access in desktop apps:

```csharp
// Check if Bluetooth adapter is available and enabled
bool isAvailable = await scanner.HasScannerPermissionsAsync();

if (!isAvailable)
{
    // Adapter is not available or Bluetooth is off
    await ShowEnableBluetoothDialog(
        "Please enable Bluetooth in Windows Settings to scan for devices"
    );
}
```

#### Adapter States

The Bluetooth adapter can be in several states:

1. **Powered On**: Ready to use
2. **Powered Off**: Bluetooth radio is disabled
3. **Not Available**: No Bluetooth hardware or drivers not installed
4. **Unauthorized**: Capability not declared in manifest

#### Requesting Radio Access

Plugin.Bluetooth can request radio access if Bluetooth is turned off:

```csharp
try
{
    await scanner.RequestScannerPermissionsAsync();
    // This may prompt user to turn on Bluetooth radio
}
catch (BluetoothPermissionException ex)
{
    // Adapter not available or user declined to enable radio
    Console.WriteLine($"Bluetooth not available: {ex.Message}");
}
```

#### Opening Settings

Guide users to Windows Settings to enable Bluetooth:

```csharp
#if WINDOWS
// Open Bluetooth settings
await Windows.System.Launcher.LaunchUriAsync(
    new Uri("ms-settings:bluetooth")
);
#endif
```

#### Key Characteristics

- No runtime permission dialogs for desktop apps
- Must declare `bluetooth` capability in `Package.appxmanifest`
- Adapter must be physically present and drivers installed
- Can request radio access but cannot install Bluetooth adapter in code

## Permission Scenarios

### Scenario 1: First-Time App Launch

Recommended flow for apps using Bluetooth on first launch:

```csharp
public async Task InitializeBluetoothAsync()
{
    // Check if permissions are already granted
    bool hasPermissions = await scanner.HasScannerPermissionsAsync();

    if (!hasPermissions)
    {
        // Show educational UI explaining why Bluetooth is needed
        await ShowBluetoothExplanationAsync();

        try
        {
            // Request permissions
            await scanner.RequestScannerPermissionsAsync();
        }
        catch (BluetoothPermissionException ex)
        {
            // User denied permission
            await ShowPermissionDeniedDialogAsync();
            return;
        }
    }

    // Permissions granted - start scanning
    await scanner.StartScanningAsync();
}

private async Task ShowBluetoothExplanationAsync()
{
    await DisplayAlert(
        "Bluetooth Access",
        "This app uses Bluetooth to connect to nearby fitness devices and track your activities.",
        "OK"
    );
}
```

### Scenario 2: Permission Denied

Handle gracefully when users deny permission:

```csharp
public async Task HandlePermissionDeniedAsync()
{
    bool shouldRetry = await DisplayAlert(
        "Permission Required",
        "Bluetooth access is required to connect to your devices. Without this permission, the app cannot function properly.",
        "Go to Settings",
        "Cancel"
    );

    if (shouldRetry)
    {
        // Open platform-specific settings
        await OpenAppSettingsAsync();
    }
}

private async Task OpenAppSettingsAsync()
{
#if IOS || MACCATALYST
    await OpenIOSSettingsAsync();
#elif ANDROID
    OpenAndroidSettings();
#elif WINDOWS
    await OpenWindowsBluetoothSettingsAsync();
#endif
}
```

### Scenario 3: Runtime Permission Revocation

Monitor permission changes while app is running:

```csharp
public class BluetoothService
{
    private readonly IBluetoothScanner _scanner;
    private Timer _permissionCheckTimer;

    public event EventHandler<bool> PermissionStatusChanged;

    public BluetoothService(IBluetoothScanner scanner)
    {
        _scanner = scanner;

        // Check permissions periodically
        _permissionCheckTimer = new Timer(CheckPermissions, null,
            TimeSpan.Zero, TimeSpan.FromSeconds(5));
    }

    private async void CheckPermissions(object state)
    {
        bool hasPermissions = await _scanner.HasScannerPermissionsAsync();

        PermissionStatusChanged?.Invoke(this, hasPermissions);

        if (!hasPermissions && _scanner.IsRunning)
        {
            // Permissions were revoked while scanning
            await _scanner.StopScanningAsync();
            await ShowPermissionRevokedDialogAsync();
        }
    }
}
```

### Scenario 4: Background Scanning (Android)

Request background location permission for background scanning:

```csharp
public async Task EnableBackgroundScanningAsync()
{
    // First, ensure foreground permissions are granted
    if (!await scanner.HasScannerPermissionsAsync())
    {
        await scanner.RequestScannerPermissionsAsync();
    }

#if ANDROID
    // On Android 10-11, request background location
    if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Q &&
        Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.S)
    {
        var options = new ScanningOptions
        {
            RequireBackgroundLocation = true
        };

        try
        {
            await scanner.StartScanningAsync(options);
        }
        catch (BluetoothPermissionException)
        {
            await DisplayAlert(
                "Background Permission Required",
                "To scan for devices in the background, please allow 'All the time' location access.",
                "OK"
            );
        }
    }
#endif
}
```

## Best Practices

### 1. Request Permissions Just-In-Time

Don't request permissions until the user performs an action that requires them:

```csharp
// ❌ Bad: Request on app launch
public override async void OnAppearing()
{
    await scanner.RequestScannerPermissionsAsync();
}

// ✅ Good: Request when user taps "Scan" button
private async void OnScanButtonClicked(object sender, EventArgs e)
{
    try
    {
        await scanner.StartScanningAsync(); // Uses RequestAutomatically
    }
    catch (BluetoothPermissionException)
    {
        await ShowPermissionRequiredDialog();
    }
}
```

### 2. Provide Context Before Requesting

Show explanation before triggering permission dialogs:

```csharp
private async Task ScanForDevicesAsync()
{
    if (!await scanner.HasScannerPermissionsAsync())
    {
        // Explain why permission is needed
        bool accepted = await DisplayAlert(
            "Bluetooth Access Needed",
            "To find your fitness tracker, we need permission to scan for nearby Bluetooth devices.",
            "Continue",
            "Cancel"
        );

        if (!accepted) return;

        // Now request permission
        try
        {
            await scanner.RequestScannerPermissionsAsync();
        }
        catch (BluetoothPermissionException)
        {
            await ShowPermissionDeniedDialog();
            return;
        }
    }

    await scanner.StartScanningAsync();
}
```

### 3. Handle Graceful Degradation

Provide alternative functionality when permissions are denied:

```csharp
private async Task InitializeAsync()
{
    if (!await scanner.HasScannerPermissionsAsync())
    {
        // Show UI explaining limited functionality
        ShowLimitedModeUI();
        return;
    }

    // Full functionality available
    ShowFullFeatureUI();
    await scanner.StartScanningAsync();
}

private void ShowLimitedModeUI()
{
    // Show read-only mode, manual device entry, etc.
}
```

### 4. Test Permission Flows

Test all permission scenarios:

- First-time request
- Permission granted
- Permission denied
- Permission denied with "Don't ask again" (Android)
- Permission revoked while app is running
- Permission re-granted after denial

### 5. Platform-Specific Handling

Use conditional compilation for platform-specific behavior:

```csharp
private async Task HandlePermissionDenialAsync()
{
#if ANDROID
    // Android: Check for "Don't ask again"
    // Guide to settings if needed
#elif IOS || MACCATALYST
    // iOS: Always guide to Settings after first denial
    await OpenIOSSettingsAsync();
#elif WINDOWS
    // Windows: Check if adapter is available
    await OpenBluetoothSettingsAsync();
#endif
}
```

## Troubleshooting

### Permission Requests Not Showing

**iOS:**
- Verify `NSBluetoothAlwaysUsageDescription` is in `Info.plist`
- Clean and rebuild project
- Check that description string is not empty

**Android:**
- Verify permissions are declared in `AndroidManifest.xml`
- Check target SDK version matches permission requirements
- Ensure location services are enabled (API 23-30)

**Windows:**
- Verify `bluetooth` capability in `Package.appxmanifest`
- Check that Bluetooth adapter is available

### Permission Requests Failing Silently

Enable detailed exception logging:

```csharp
try
{
    await scanner.RequestScannerPermissionsAsync();
}
catch (BluetoothPermissionException ex)
{
    // Log full exception details
    Console.WriteLine($"Permission exception: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
```

### Permissions Granted But Operations Fail

**Android API 23-30:**
- Check if location services are enabled on device
- Verify GPS/location is turned on in device settings

**Windows:**
- Check if Bluetooth radio is powered on
- Verify Bluetooth drivers are installed

## Next Steps

- [Getting Started Guide](./README.md) - Basic usage and code examples
- [Platform Setup Guide](./Platform-Setup.md) - Platform-specific manifest configuration
- [Architecture Guidelines](../ARCHITECTURE_GUIDELINES.md) - Understanding library design

## Additional Resources

- [iOS Bluetooth Authorization](https://developer.apple.com/documentation/corebluetooth/cbmanagerauthorization)
- [Android Bluetooth Permissions](https://developer.android.com/guide/topics/connectivity/bluetooth/permissions)
- [Android Runtime Permissions](https://developer.android.com/training/permissions/requesting)
- [Windows Bluetooth Capabilities](https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth)
