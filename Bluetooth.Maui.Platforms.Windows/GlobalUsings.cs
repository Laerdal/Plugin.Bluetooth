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
#endif

global using Bluetooth.Core;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;
global using Bluetooth.Abstractions.Enums;
