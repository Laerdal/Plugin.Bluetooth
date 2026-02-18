// ReSharper disable RedundantUsingDirective.Global
global using System;
global using System.Collections.ObjectModel;
global using System.Globalization;
global using System.Reflection;
global using System.Diagnostics;
global using System.Runtime.Versioning;

#if IOS || MACCATALYST
global using CoreBluetooth;
global using CoreFoundation;
global using Foundation;
#endif

global using Bluetooth.Core;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Broadcasting.Enums;
global using Bluetooth.Abstractions.Broadcasting.Factories;
global using Bluetooth.Abstractions.Broadcasting.Options;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;
global using Bluetooth.Abstractions.Scanning.Factories;
global using Bluetooth.Abstractions.Scanning.Options;
global using Bluetooth.Abstractions.Enums;
