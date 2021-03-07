using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System.Linq;

namespace StringReloads.Hook.Win32
{
    unsafe class TextOutW : Hook<TextOutWDelegate>
    {
        public override string Library => "gdi32.dll";

        public override string Export => "TextOutW";

        public override void Initialize()
        {
            HookDelegate = new TextOutWDelegate(TextOut);
            Compile();
        }

        bool TextOut(void* dc, int xStart, int yStart, byte* pStr, int strLen)
        {
            WCString InStr = pStr;
            InStr.FixedLength = (uint)strLen;

            InStr = EntryPoint.Process((WCString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.ExtTextOutWUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.ExtTextOutWRemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(dc, xStart, yStart, InStr, InStr.Count());
        }
    }
}
