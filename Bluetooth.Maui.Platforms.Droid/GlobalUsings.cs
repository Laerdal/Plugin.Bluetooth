// ReSharper disable RedundantUsingDirective.Global
global using System;
global using System.Collections.ObjectModel;
global using System.Globalization;
global using System.Reflection;
global using System.Diagnostics;
global using System.Runtime.Versioning;

#if ANDROID
global using Android.Bluetooth;
global using Android.Bluetooth.LE;
global using Android.Content;
global using Android.OS;

global using Java.Lang.Reflect;
global using Java.Util;
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
