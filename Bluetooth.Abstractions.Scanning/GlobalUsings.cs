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
global using Bluetooth.Abstractions.Enums;
global using Bluetooth.Abstractions.EventArgs;
global using Bluetooth.Abstractions.Exceptions;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;
global using Bluetooth.Abstractions.Scanning.EventArgs;
global using Bluetooth.Abstractions.Scanning.Exceptions;
global using Bluetooth.Abstractions.Scanning.Factories;
global using Bluetooth.Abstractions.Scanning.Options;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;

global using Plugin.BaseTypeExtensions;
global using Plugin.ByteArrays;
global using Plugin.ExceptionListeners;