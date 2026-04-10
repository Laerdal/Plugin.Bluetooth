namespace Bluetooth.Avalonia;

/// <summary>
///     Unified Bluetooth remote descriptor facade providing extension points for client customization.
/// </summary>
public class BluetoothRemoteDescriptor : IBluetoothRemoteDescriptor
{
    private readonly IBluetoothRemoteDescriptor _platformDescriptor;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothRemoteDescriptor" /> class.
    /// </summary>
    /// <param name="platformDescriptor">The platform descriptor to wrap.</param>
    /// <param name="characteristic">The wrapped parent characteristic.</param>
    public BluetoothRemoteDescriptor(IBluetoothRemoteDescriptor platformDescriptor, IBluetoothRemoteCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(platformDescriptor);
        ArgumentNullException.ThrowIfNull(characteristic);

        _platformDescriptor = platformDescriptor;
        Characteristic = characteristic;

        _platformDescriptor.ValueUpdated += (sender, args) => ValueUpdated?.Invoke(this, args);
        _platformDescriptor.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
    }

    /// <summary>
    ///     Gets the wrapped platform descriptor.
    /// </summary>
    public IBluetoothRemoteDescriptor PlatformDescriptor => _platformDescriptor;

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic Characteristic { get; }

    /// <inheritdoc />
    public Guid Id => _platformDescriptor.Id;

    /// <inheritdoc />
    public string Name => _platformDescriptor.Name;

    /// <inheritdoc />
    public bool CanRead => _platformDescriptor.CanRead;

    /// <inheritdoc />
    public bool IsReadingValue => _platformDescriptor.IsReadingValue;

    /// <inheritdoc />
    public bool CanWrite => _platformDescriptor.CanWrite;

    /// <inheritdoc />
    public bool IsWritingValue => _platformDescriptor.IsWritingValue;

    /// <inheritdoc />
    public ReadOnlySpan<byte> ValueSpan => _platformDescriptor.ValueSpan;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Value => _platformDescriptor.Value;

    /// <inheritdoc />
    public event EventHandler<ValueUpdatedEventArgs> ValueUpdated = delegate { };

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDescriptor.ReadValueAsync(skipIfPreviouslyRead, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async ValueTask WriteValueAsync(ReadOnlyMemory<byte> value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await OnBeforeWriteValueAsync(value, cancellationToken).ConfigureAwait(false);
        await _platformDescriptor.WriteValueAsync(value,
                                                  skipIfOldValueMatchesNewValue,
                                                  timeout,
                                                  cancellationToken)
                                 .ConfigureAwait(false);
        await OnAfterWriteValueAsync(value, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Called before writing descriptor value.
    /// </summary>
    protected virtual ValueTask OnBeforeWriteValueAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Called after writing descriptor value.
    /// </summary>
    protected virtual ValueTask OnAfterWriteValueAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(Func<ReadOnlyMemory<byte>, bool>? valueFilter = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformDescriptor.WaitForValueChangeAsync(valueFilter, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        return _platformDescriptor.DisposeAsync();
    }
}
