using Microsoft.Win32;

namespace Word.Helpers
{
    /// <summary>
    /// Helper for managing Windows Registry values.
    /// </summary>
    internal static class RegistryHelper
    {
        internal static void SaveValue(string path, string name, bool value) => SaveValue(path, name, value ? 1 : 0);

        internal static bool LoadValue(string path, string name, bool defaultValue = false) => LoadValue(path, name, defaultValue ? 1 : 0) != 0;

        internal static void SaveValue(string path, string name, int value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(path))
            {
                key?.SetValue(name, value, RegistryValueKind.DWord);
            }
        }

        internal static int LoadValue(string path, string name, int defaultValue = 0)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(path))
            {
                return key?.GetValue(name) is int intVal ? intVal : defaultValue;
            }
        }

        internal static void SaveValue(string path, string name, float value)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(path))
            {
                // Store as string to preserve decimal precision
                key?.SetValue(name, value.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    RegistryValueKind.String);
            }
        }

        internal static float LoadValue(string path, string name, float defaultValue = 0f)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(path))
            {
                if (key?.GetValue(name) is string s && float.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var f))
                    return f;

                return defaultValue;
            }
        }
    }
}
