using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Word.Modules
{
    // Kept free of Word interop types so the unit tests can link just this file.
    internal static partial class Fonts
    {
        /// <summary>
        /// TODO: Review and refactor
        /// Gets the family names of a font file by loading it into a PrivateFontCollection.
        /// </summary>
        private static List<string> GetFontFamilyNamesFromFile(string fontFilePath)
        {
            // magick code
            // pray it works
            // seems to work fine for TTC also
            // need to test with other fonts than Cambria Math and Cambria

            var familyNames = new List<string>();

            if (!File.Exists(fontFilePath))
                return familyNames;

            try
            {
                using (var pfc = new PrivateFontCollection())
                {
                    pfc.AddFontFile(fontFilePath);
                    familyNames.AddRange(pfc.Families.Select(f => f.Name));
                }
            }
            catch
            {
            }

            return familyNames;
        }

        /// <summary>
        /// Builds a font family name -> font file path index from the Windows registry. Loads each font file once.
        /// </summary>
        internal static Dictionary<string, string> BuildFontPathIndex()
        {
            var index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var seenPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var fontsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            var registrySources = new[]
            {
                new { Hive = Registry.CurrentUser, Path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" },
                new { Hive = Registry.LocalMachine, Path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" }
            };

            foreach (var source in registrySources)
            {
                using (var fontsKey = source.Hive.OpenSubKey(source.Path))
                {
                    if (fontsKey == null)
                        continue;

                    foreach (var valueName in fontsKey.GetValueNames())
                    {
                        var regValue = fontsKey.GetValue(valueName) as string;
                        if (string.IsNullOrEmpty(regValue))
                            continue;

                        var fontPath = regValue;
                        if (!Path.IsPathRooted(fontPath))
                            fontPath = Path.Combine(fontsFolder, fontPath);

                        if (!File.Exists(fontPath) || !seenPaths.Add(fontPath))
                            continue;

                        foreach (var name in GetFontFamilyNamesFromFile(fontPath))
                            if (!index.ContainsKey(name))
                                index[name] = fontPath;
                    }
                }
            }

            return index;
        }
    }
}
