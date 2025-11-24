// ReSharper disable RedundantUsingDirective.Global
global using System;
global using System.Collections.ObjectModel;
global using System.Globalization;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.Versioning;

global using Microsoft.Extensions.Logging;

#if WINDOWS

global using Windows.Devices.Bluetooth;
global using Windows.Devices.Bluetooth.Advertisement;
global using Windows.Devices.Bluetooth.GenericAttributeProfile;
global using Windows.Devices.Enumeration;
global using Windows.Devices.Radios;
global using Windows.Security.Authorization.AppCapabilityAccess;
global using System.Runtime.InteropServices.WindowsRuntime;

#endif

#if ANDROID

global using Android.Bluetooth;
global using Android.Bluetooth.LE;
global using Android.Content;
global using Android.OS;

global using Java.Lang.Reflect;
global using Java.Util;

#endif

#if IOS || MACCATALYST

global using CoreBluetooth;
global using CoreFoundation;
global using Foundation;

#endif

global using Plugin.Bluetooth;
global using Plugin.Bluetooth.Abstractions;
global using Plugin.Bluetooth.BaseClasses;
global using Plugin.Bluetooth.Enums;
global using Plugin.Bluetooth.Exceptions;
global using Plugin.Bluetooth.Maui.Exceptions;

