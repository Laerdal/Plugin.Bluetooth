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
global using Bluetooth.Core;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Core.Infrastructure.Scheduling;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Logging.Abstractions;
global using Microsoft.Extensions.Options;

global using Plugin.BaseTypeExtensions;
global using Plugin.ByteArrays;
global using Plugin.ExceptionListeners;