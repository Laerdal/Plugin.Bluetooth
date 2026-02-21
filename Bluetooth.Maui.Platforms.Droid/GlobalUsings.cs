// ReSharper disable RedundantUsingDirective.Global

global using System;
global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Globalization;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.Versioning;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Broadcasting.Enums;
global using Bluetooth.Abstractions.Broadcasting.EventArgs;
global using Bluetooth.Abstractions.Broadcasting.Exceptions;
global using Bluetooth.Abstractions.Broadcasting.Factories;
global using Bluetooth.Abstractions.Broadcasting.Options;
global using Bluetooth.Abstractions.Enums;
global using Bluetooth.Abstractions.EventArgs;
global using Bluetooth.Abstractions.Exceptions;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;
global using Bluetooth.Abstractions.Scanning.EventArgs;
global using Bluetooth.Abstractions.Scanning.Exceptions;
global using Bluetooth.Abstractions.Scanning.Factories;
global using Bluetooth.Abstractions.Scanning.Options;
global using Bluetooth.Core;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Core.Infrastructure.Scheduling;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Scanning.Converters;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;

global using Plugin.BaseTypeExtensions;
global using Plugin.ByteArrays;
global using Plugin.ExceptionListeners;

#if IOS || MACCATALYST
global using CoreBluetooth;
global using CoreFoundation;
global using Foundation;

global using Bluetooth.Maui.Platforms.Apple;
global using Bluetooth.Maui.Platforms.Apple.Tools;
global using Bluetooth.Maui.Platforms.Apple.NativeObjects;
global using Bluetooth.Maui.Platforms.Apple.Exceptions;
#elif ANDROID
global using Android.Bluetooth;
global using Android.Bluetooth.LE;
global using Android.Content;
global using Android.OS;

global using Java.Lang.Reflect;
global using Java.Util;

global using Bluetooth.Maui.Platforms.Droid;

#elif WINDOWS
global using System.Runtime.InteropServices.WindowsRuntime;
global using Windows.Devices.Bluetooth;
global using Windows.Devices.Bluetooth.Advertisement;
global using Windows.Devices.Bluetooth.GenericAttributeProfile;
global using Windows.Devices.Enumeration;
global using Windows.Devices.Radios;
global using Windows.Security.Authorization.AppCapabilityAccess;

global using Bluetooth.Maui.Platforms.Win;
#else
global using Bluetooth.Maui.Platforms.DotNetCore;
#endif