using System;
using Microsoft.Win32;

namespace Snappo.Settings;

internal static class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "Snappo";

    public static bool IsEnabled()
    {
        using RegistryKey? runKey = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        return runKey?.GetValue(ValueName) is string;
    }

    public static void SetEnabled(bool enabled)
    {
        using RegistryKey runKey = Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true)
            ?? throw new InvalidOperationException("Could not open the Windows startup registry key.");

        if (enabled)
        {
            string exePath = Environment.ProcessPath
                ?? throw new InvalidOperationException("Could not determine Snappo's executable path.");

            runKey.SetValue(ValueName, $"\"{exePath}\"");
        }
        else
        {
            runKey.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }
}
