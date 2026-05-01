global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.Globalization;
global using System.Linq;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;

global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Broadcasting.Enums;
global using Bluetooth.Abstractions.Broadcasting.EventArgs;
global using Bluetooth.Abstractions.Broadcasting.Exceptions;
global using Bluetooth.Abstractions.Broadcasting.Options;
global using Bluetooth.Abstractions.Enums;
global using Bluetooth.Abstractions.EventArgs;
global using Bluetooth.Abstractions.Exceptions;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;
global using Bluetooth.Abstractions.Scanning.EventArgs;
global using Bluetooth.Abstractions.Scanning.Exceptions;
global using Bluetooth.Abstractions.Scanning.Options;
global using Bluetooth.Core;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Core.Infrastructure.Scheduling;
global using Bluetooth.Core.Scanning;

global using Linux.Bluetooth;
global using Linux.Bluetooth.Extensions;

global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
