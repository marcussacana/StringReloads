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
            HookDelegate = new TextOutADelegate(hTextOut);
            Compile();
        }

        bool hTextOut(void* dc, int xStart, int yStart, byte* pStr, int strLen)
        {
            CString OriStr = pStr;
            OriStr.FixedLength = strLen;
            
            Log.Trace($"TextOutA X: {xStart} Y: {yStart}: {(string)OriStr}");

            WCString InStr = EntryPoint.ProcessW((WCString)(string)OriStr);

            if (Config.Default.TextOutAUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.TextOutARemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return TextOutW(dc, xStart, yStart, InStr, (int)InStr.LongCount());
        }

        [DllImport("gdi32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        extern static bool TextOutW(void* dc, int xStart, int yStart, byte* pStr, int strLen);
    }
}
