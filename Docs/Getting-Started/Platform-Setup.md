# Platform Setup Guide

This guide covers platform-specific configuration required for Plugin.Bluetooth to work on iOS, Android, and Windows.

## Overview

Each platform requires specific permissions and capabilities to access Bluetooth hardware:

- **iOS/MacCatalyst**: Usage description strings in `Info.plist`
- **Android**: Runtime permissions in `AndroidManifest.xml` (varies by API level)
- **Windows**: Device capabilities in `Package.appxmanifest`

> Plugin.Bluetooth automatically handles runtime permission requests by default. This guide focuses on the required manifest/configuration file entries.

## iOS & MacCatalyst

### Required Configuration

Add Bluetooth usage descriptions to your `Info.plist` file located at:
- `Platforms/iOS/Info.plist`
- `Platforms/MacCatalyst/Info.plist`

```xml
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <!-- Other keys... -->

    <!-- Required: Bluetooth Always Usage Description -->
    <key>NSBluetoothAlwaysUsageDescription</key>
    <string>This app uses Bluetooth to connect to nearby devices</string>

    <!-- Legacy: Still recommended for older iOS versions -->
    <key>NSBluetoothPeripheralUsageDescription</key>
    <string>This app uses Bluetooth to connect to nearby devices</string>

</dict>
</plist>
```

### Usage Description Keys

| Key | iOS Version | Purpose | Required |
|-----|-------------|---------|----------|
| `NSBluetoothAlwaysUsageDescription` | iOS 13+ | Main Bluetooth permission | ✅ Yes |
| `NSBluetoothPeripheralUsageDescription` | iOS 6-12 | Legacy Bluetooth permission | ⚠️ Recommended |

### Customizing Permission Messages

Provide clear, user-friendly descriptions explaining why your app needs Bluetooth:

```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>We use Bluetooth to connect to your heart rate monitor and track your fitness activities</string>
```

> **Important**: Apps will crash at runtime if you attempt to use Bluetooth without these entries in `Info.plist`.

### Background Modes (Optional)

If your app needs to scan or communicate with Bluetooth devices while in the background, add background mode capabilities:

```xml
<key>UIBackgroundModes</key>
<array>
    <string>bluetooth-central</string>
    <!-- Add bluetooth-peripheral if acting as a peripheral -->
</array>
```

### Platform Behavior

- **Permission Dialog**: Shown automatically by iOS when first accessing Bluetooth (CoreBluetooth)
- **Auto-Request**: Plugin.Bluetooth's `RequestAutomatically` strategy is effectively a no-op on iOS
- **Re-Authorization**: Users must go to Settings to re-enable if denied; subsequent requests show "Go to Settings" dialog

## Android

Android Bluetooth permissions have evolved significantly across API levels. The required permissions depend on your target Android version.

### Permission Evolution Summary

| API Level | Android Version | Required Permissions |
|-----------|-----------------|----------------------|
| API 23-28 | Android 6-8.1 | `BLUETOOTH`, `BLUETOOTH_ADMIN`, `ACCESS_FINE_LOCATION` |
| API 29-30 | Android 10-11 | Above + `ACCESS_BACKGROUND_LOCATION` (for background) |
| API 31+ | Android 12+ | `BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT` (no location needed) |

### Basic Configuration (Targets API 31+)

For modern apps targeting Android 12 and above:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Bluetooth permissions for Android 12+ (API 31+) -->
    <uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                     android:usesPermissionFlags="neverForLocation" />
    <uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

    <!-- Optional: For broadcasting (peripheral mode) -->
    <uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />

    <application>
        <!-- Your app configuration -->
    </application>

</manifest>
```

> **Note**: The `android:usesPermissionFlags="neverForLocation"` attribute on `BLUETOOTH_SCAN` tells the system you don't need location data, simplifying the permission request for users.

### Backwards Compatible Configuration

For apps supporting older Android versions (API 23-30):

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Legacy Bluetooth permissions (API 23-30) -->
    <uses-permission android:name="android.permission.BLUETOOTH"
                     android:maxSdkVersion="30" />
    <uses-permission android:name="android.permission.BLUETOOTH_ADMIN"
                     android:maxSdkVersion="30" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"
                     android:maxSdkVersion="30" />

    <!-- Optional: Background location for scanning when app is in background (API 29-30) -->
    <uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION"
                     android:maxSdkVersion="30" />

    <!-- Modern Bluetooth permissions (API 31+) -->
    <uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                     android:usesPermissionFlags="neverForLocation" />
    <uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

    <!-- Optional: For broadcasting (peripheral mode) -->
    <uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />

    <application>
        <!-- Your app configuration -->
    </application>

</manifest>
```

### Detailed Permission Breakdown

#### API 23-28 (Android 6.0 - 8.1)

```xml
<!-- Basic Bluetooth access -->
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />

<!-- Required: Location permission for BLE scanning -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />

<!-- Alternative: Coarse location (less precise but sufficient for scanning) -->
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
```

**Why location?** On Android 6-11, BLE scanning requires location permission because device MAC addresses can be used to infer location.

#### API 29-30 (Android 10-11)

All permissions from API 23-28, plus:

```xml
<!-- For scanning when app is in background -->
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
```

#### API 31+ (Android 12+)

```xml
<!-- Scan for nearby Bluetooth devices -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />

<!-- Connect to paired/discovered devices -->
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

<!-- Optional: Advertise as a peripheral device -->
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />
```

### Background Scanning Permissions

If your app needs to scan for devices while in the background:

#### API 29-30
```xml
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
```

Request this permission through `ScanningOptions`:

```csharp
var options = new ScanningOptions
{
    RequireBackgroundLocation = true
};

await scanner.StartScanningAsync(options);
```

#### API 31+

Remove `neverForLocation` flag and request location permission:

```xml
<!-- Note: No neverForLocation flag -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

### Hardware Features

Declare Bluetooth as a required or optional hardware feature:

```xml
<!-- Optional: App works without Bluetooth -->
<uses-feature android:name="android.hardware.bluetooth_le"
              android:required="false" />

<!-- Required: App requires Bluetooth to function -->
<uses-feature android:name="android.hardware.bluetooth_le"
              android:required="true" />
```

Setting `android:required="false"` allows your app to be installed on devices without Bluetooth but requires runtime checks. Setting `true` prevents installation on incompatible devices.

### Platform Behavior

- **Runtime Requests**: On API 23+, Plugin.Bluetooth requests permissions at runtime automatically
- **Permission Dialogs**: Users see platform permission dialogs explaining what's being requested
- **Retry Handling**: If denied, subsequent requests show "Don't ask again" checkbox
- **Settings Override**: Users can grant/revoke permissions in Settings at any time

### Common Configuration Example

Complete `AndroidManifest.xml` for most apps:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">

    <!-- Declare app requires Bluetooth LE -->
    <uses-feature android:name="android.hardware.bluetooth_le"
                  android:required="false" />

    <!-- Legacy permissions (API 23-30) -->
    <uses-permission android:name="android.permission.BLUETOOTH"
                     android:maxSdkVersion="30" />
    <uses-permission android:name="android.permission.BLUETOOTH_ADMIN"
                     android:maxSdkVersion="30" />
    <uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"
                     android:maxSdkVersion="30" />

    <!-- Modern permissions (API 31+) -->
    <uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                     android:usesPermissionFlags="neverForLocation" />
    <uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

    <application
        android:allowBackup="true"
        android:icon="@mipmap/appicon"
        android:supportsRtl="true">
        <!-- Your activities -->
    </application>

</manifest>
```

## Windows

### Required Configuration

Add the Bluetooth device capability to your `Package.appxmanifest` file located at `Platforms/Windows/Package.appxmanifest`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">

  <!-- Package identity and properties -->
  <Identity Name="YourApp" Publisher="CN=YourCompany" Version="1.0.0.0" />

  <!-- Other configuration... -->

  <Capabilities>
    <rescap:Capability Name="runFullTrust" />

    <!-- Required: Bluetooth device capability -->
    <DeviceCapability Name="bluetooth" />

  </Capabilities>

</Package>
```

### Capability Details

| Capability | Purpose | Required |
|------------|---------|----------|
| `bluetooth` | Access to Bluetooth LE APIs | ✅ Yes |
| `runFullTrust` | Desktop app capability | ✅ Yes (for MAUI) |

### Platform Behavior

- **No Runtime Dialog**: Windows doesn't show a permission dialog for Bluetooth access
- **Adapter State**: Plugin.Bluetooth checks if Bluetooth adapter is available and powered on
- **Radio Access**: Automatically requests radio access if adapter is off
- **Settings Link**: If Bluetooth is disabled, users must enable it in Windows Settings

### Bluetooth Adapter Requirements

Your Windows device must have:
- Bluetooth 4.0+ adapter (for BLE support)
- Bluetooth drivers installed and enabled
- Bluetooth radio turned on

### Checking Adapter Availability

```csharp
var hasPermission = await scanner.HasScannerPermissionsAsync();
if (!hasPermission)
{
    // On Windows, this means adapter is not available or turned off
    // Guide user to turn on Bluetooth in Windows Settings
}
```

## Permission Request Strategies

Plugin.Bluetooth provides three strategies for handling permissions across all platforms:

### RequestAutomatically (Default)

Permissions are requested automatically when starting operations:

```csharp
// Default behavior - permissions requested automatically
await scanner.StartScanningAsync();
```

**Platform Behavior:**
- **Android**: Shows permission dialog before scanning starts
- **iOS**: No-op (CoreBluetooth shows dialog automatically)
- **Windows**: Checks adapter state and requests radio access

### ThrowIfNotGranted

Take explicit control over when permissions are requested:

```csharp
// Check permissions first
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
        ShowMessage("Bluetooth permission is required to scan for devices");
        return;
    }
}

// Now start scanning with explicit strategy
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted
};

await scanner.StartScanningAsync(options);
```

### AssumeGranted

Skip all permission checks (use with caution):

```csharp
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.AssumeGranted
};

await scanner.StartScanningAsync(options);
```

> **Warning**: If permissions are not actually granted, this will cause runtime errors:
> - **Android**: `SecurityException` from native code
> - **iOS**: Permission dialog will still appear (cannot be prevented)
> - **Windows**: May fail when accessing Bluetooth APIs

Use `AssumeGranted` only when you've already requested and verified permissions through another mechanism.

## Testing Permission Handling

### iOS Simulator
- Bluetooth is not available on iOS Simulator
- Test on physical devices only

### Android Emulator
- Bluetooth is typically not functional on emulators
- Test on physical devices for reliable results
- Some emulators support Bluetooth passthrough but with limitations

### Windows
- Requires physical Bluetooth adapter
- Can test on development machines with Bluetooth

## Troubleshooting

### iOS: App crashes when accessing Bluetooth

**Cause**: Missing `NSBluetoothAlwaysUsageDescription` in `Info.plist`

**Solution**: Add the required key with a description string to your `Info.plist`

### Android: "Permission denied" on devices running Android 12+

**Cause**: Missing `BLUETOOTH_SCAN` or `BLUETOOTH_CONNECT` permissions

**Solution**: Add Android 12+ permissions to `AndroidManifest.xml`:
```xml
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
```

### Android: Devices not discovered on Android 6-11

**Cause**: Missing location permission or location services disabled

**Solution**:
1. Add location permission to `AndroidManifest.xml`
2. Ensure location services are enabled in device settings
3. Request location permission at runtime

### Windows: Cannot find any Bluetooth devices

**Cause**: Bluetooth capability not declared or Bluetooth adapter is off

**Solution**:
1. Add `<DeviceCapability Name="bluetooth" />` to `Package.appxmanifest`
2. Ensure Bluetooth is enabled in Windows Settings
3. Check that Bluetooth adapter drivers are installed

### Windows: "Bluetooth adapter not available"

**Cause**: No Bluetooth hardware or drivers not installed

**Solution**: Verify device has Bluetooth 4.0+ hardware and drivers are properly installed

## Platform-Specific Notes

### iOS/MacCatalyst
- Permission dialog is shown automatically by CoreBluetooth
- Users cannot re-grant permission from within app; must use Settings
- Background scanning requires additional background mode configuration
- Mac Catalyst apps have same requirements as iOS

### Android
- Permission model varies drastically across API levels
- Location services must be enabled on device for API 23-30
- "Don't ask again" can permanently block permission requests
- Some manufacturers (Samsung, Xiaomi) have additional permission layers

### Windows
- No runtime permission dialog
- Adapter must be available and enabled
- Desktop apps (MAUI) require `runFullTrust` capability
- UWP apps have different manifest schema (not applicable to MAUI)

## Next Steps

Now that you've configured platform permissions, continue with:

- [Getting Started Guide](./README.md) - Basic usage and examples
- [Permissions Guide](./Permissions.md) - Detailed permission handling strategies
- [Architecture Guidelines](../ARCHITECTURE_GUIDELINES.md) - Understand the library design

## Additional Resources

- [iOS Bluetooth Permissions](https://developer.apple.com/documentation/corebluetooth)
- [Android Bluetooth Permissions](https://developer.android.com/guide/topics/connectivity/bluetooth/permissions)
- [Windows Bluetooth Capabilities](https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth)
