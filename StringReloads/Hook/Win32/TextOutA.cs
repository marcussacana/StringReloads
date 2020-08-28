using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;
using System.Runtime.InteropServices;

namespace StringReloads.Hook.Win32
{
    unsafe class TextOutA : Hook<TextOutADelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "TextOutA";

        public override void Initialize()
        {
            HookDelegate = new TextOutADelegate(TextOut);
            Compile();
        }

        bool TextOut(void* dc, int xStart, int yStart, byte* pStr, int strLen)
        {
            CString OriStr = pStr;
            OriStr.FixedLength = strLen;

            WCString Str = EntryPoint.ProcessW((WCString)(string)OriStr);

            if (Config.Default.TextOutAUndoRemap)
                Str = Remaper.Default.Restore(Str);

            return TextOutW(dc, xStart, yStart, Str, (int)Str.LongCount());
        }

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        extern static bool TextOutW(void* dc, int xStart, int yStart, byte* pStr, int strLen);
    }
}
