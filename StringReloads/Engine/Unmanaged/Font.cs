using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StringReloads.Engine.Unmanaged
{
    internal unsafe static class FontUtility
    {
        internal static void LoadLocalFonts()
        {
            var RootDir = Path.Combine(EntryPoint.CurrentDll).TrimEnd(' ', '\\', '/');
            var RootDirB = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(' ', '\\', '/');
            var RootBEquals = RootDir.ToLowerInvariant().StartsWith(RootDirB.ToLowerInvariant());

            if (RootDirB.Length < RootDir.Length && RootBEquals)
                RootDir = RootDirB;

            var Exts = new string[] { "*.fon", "*.fnt", "*.ttf", "*.ttc", "*.fot", "*.otf", "*.mmm", "*.pfb", "*.pfm" };
            var Fonts = (from Ext in Exts
                         from Font in Directory.GetFiles(RootDir, Ext)
                         select Font);

            foreach (var Font in Fonts)
            {
                int Loaded = AddFontResourceExW(Font, FR_PRIVATE, null);
                
                if (Loaded > 0)
                    Log.Information("{0} Fonts Loaded From {1}", Loaded.ToString(), Path.GetFileName(Font));
            }
        }

        [DllImport("gdi32.dll")]
        static extern int AddFontResourceExW(string lpszFilename, uint fl, void* reserved);

        const uint FR_PRIVATE  = 0x10;
        const uint FR_NOT_ENUM = 0x20;
    }
}
