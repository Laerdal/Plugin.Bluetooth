// ReSharper disable RedundantUsingDirective.Global
global using System;
global using System.Collections.ObjectModel;
global using System.Globalization;
global using System.Reflection;
global using System.Diagnostics;
global using System.Runtime.Versioning;

#if WINDOWS
global using System.Runtime.InteropServices.WindowsRuntime;
global using Windows.Devices.Bluetooth;
global using Windows.Devices.Bluetooth.Advertisement;
global using Windows.Devices.Bluetooth.GenericAttributeProfile;
global using Windows.Devices.Enumeration;
global using Windows.Devices.Radios;
global using Windows.Security.Authorization.AppCapabilityAccess;
global using Bluetooth.Maui.Platforms.Windows;
global using Bluetooth.Maui.Platforms.Windows.Scanning;
global using Bluetooth.Maui.Platforms.Windows.Broadcasting;
global using Bluetooth.Maui.Platforms.Windows.Permissions;
global using BluetoothAdapter = Bluetooth.Maui.Platforms.Windows.BluetoothAdapter;
#elif ANDROID
global using Android.Bluetooth;
global using Android.Bluetooth.LE;
global using Android.Content;
global using Android.OS;

global using Java.Lang.Reflect;
global using Java.Util;
global using Bluetooth.Maui.Platforms.Droid;
global using Bluetooth.Maui.Platforms.Droid.Scanning;
global using Bluetooth.Maui.Platforms.Droid.Broadcasting;
global using Bluetooth.Maui.Platforms.Droid.Permissions;
global using BluetoothAdapter = Bluetooth.Maui.Platforms.Droid.BluetoothAdapter;
#elif IOS || MACCATALYST
global using CoreBluetooth;
global using CoreFoundation;
global using Foundation;
global using Bluetooth.Maui.Platforms.Apple;
global using Bluetooth.Maui.Platforms.Apple.Scanning;
global using Bluetooth.Maui.Platforms.Apple.Broadcasting;
global using Bluetooth.Maui.Platforms.Apple.Permissions;
#else
global using Bluetooth.Maui.Platforms.DotNetCore;
global using Bluetooth.Maui.Platforms.DotNetCore.Scanning;
global using Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;
global using Bluetooth.Maui.Platforms.DotNetCore.Permissions;
#endif

global using Bluetooth.Core;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Scanning.Exceptions;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Core.Broadcasting.Exceptions;
global using Bluetooth.Core.Exceptions;

global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.AccessService;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Enums;
