namespace Bluetooth.Avalonia.Platforms.Linux.Permissions;

/// <summary>
///     Helpers for checking Linux Bluetooth permissions.
/// </summary>
/// <remarks>
///     On Linux, BLE access requires either root privileges or membership in the
///     <c>bluetooth</c> group (plus <c>CAP_NET_ADMIN</c> for some operations).
///     This class checks group membership by reading <c>/proc/self/status</c>.
/// </remarks>
internal static class LinuxBluetoothPermissions
{
    private const string BluetoothGroup = "bluetooth";
    private const int RootUid = 0;

    /// <summary>
    ///     Returns <see langword="true" /> when the current process is running as root or is a
    ///     member of the <c>bluetooth</c> supplementary group.
    /// </summary>
    public static ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        try
        {
            return ValueTask.FromResult(IsRoot() || IsInBluetoothGroup());
        }
        catch
        {
            // If we cannot determine permissions, optimistically allow the call to proceed.
            // The D-Bus call will fail with a permission error if access is actually denied.
            return ValueTask.FromResult(true);
        }
    }

    /// <summary>
    ///     Returns <see langword="true" /> if the effective UID is 0 (root).
    /// </summary>
    private static bool IsRoot()
    {
        // Parse the effective UID from /proc/self/status
        // Line format: "Uid:\t<ruid>\t<euid>\t<suid>\t<fsuid>"
        if (!File.Exists("/proc/self/status"))
        {
            return false;
        }

        foreach (var line in File.ReadLines("/proc/self/status"))
        {
            if (!line.StartsWith("Uid:", StringComparison.Ordinal))
            {
                continue;
            }

            var parts = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);
            // parts[0] = "Uid:", parts[1] = ruid, parts[2] = euid, ...
            if (parts.Length >= 3 && int.TryParse(parts[2], out var euid))
            {
                return euid == RootUid;
            }
        }

        return false;
    }

    /// <summary>
    ///     Returns <see langword="true" /> if the current process belongs to the <c>bluetooth</c>
    ///     supplementary group as reported in <c>/proc/self/status</c>.
    /// </summary>
    private static bool IsInBluetoothGroup()
    {
        // Resolve the numeric group ID of "bluetooth"
        int? bluetoothGid = ResolveGroupId(BluetoothGroup);
        if (bluetoothGid == null)
        {
            return false;
        }

        // Parse supplementary group IDs from /proc/self/status
        // Line format: "Groups:\t<gid1> <gid2> ..."
        if (!File.Exists("/proc/self/status"))
        {
            return false;
        }

        foreach (var line in File.ReadLines("/proc/self/status"))
        {
            if (!line.StartsWith("Groups:", StringComparison.Ordinal))
            {
                continue;
            }

            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            // parts[0] = "Groups:"
            for (var i = 1; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out var gid) && gid == bluetoothGid.Value)
                {
                    return true;
                }
            }

            return false;
        }

        return false;
    }

    /// <summary>
    ///     Resolves the numeric GID for a group name by reading <c>/etc/group</c>.
    /// </summary>
    private static int? ResolveGroupId(string groupName)
    {
        const string groupFile = "/etc/group";
        if (!File.Exists(groupFile))
        {
            return null;
        }

        foreach (var line in File.ReadLines(groupFile))
        {
            // Format: group_name:password:GID:member_list
            var colonIdx = line.IndexOf(':');
            if (colonIdx < 0)
            {
                continue;
            }

            if (line.AsSpan(0, colonIdx).CompareTo(groupName.AsSpan(), StringComparison.Ordinal) != 0)
            {
                continue;
            }

            var secondColon = line.IndexOf(':', colonIdx + 1);
            if (secondColon < 0)
            {
                continue;
            }

            var thirdColon = line.IndexOf(':', secondColon + 1);
            if (thirdColon < 0)
            {
                continue;
            }

            var gidSpan = line.AsSpan(secondColon + 1, thirdColon - secondColon - 1);
            if (int.TryParse(gidSpan, out var gid))
            {
                return gid;
            }
        }

        return null;
    }
}
