// ReSharper disable RedundantUsingDirective.Global
global using System;
global using System.Collections.ObjectModel;
global using System.Globalization;
global using System.Reflection;
global using System.Diagnostics;
global using System.Runtime.Versioning;

global using Bluetooth.Core;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Enums;
#if IOS || MACCATALYST
global using Bluetooth.Maui.Platforms.Apple;
#elif ANDROID
global using Bluetooth.Maui.Platforms.Droid;
#elif WINDOWS
global using Bluetooth.Maui.Platforms.Windows;
#else
global using Bluetooth.Maui.Platforms.DotNetCore;
#endif