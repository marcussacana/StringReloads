using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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


            Log.Trace($"TextOutW X: {xStart} Y: {yStart}: {(string)InStr}");

            InStr = EntryPoint.ProcessW((WCString)(string)InStr);//Ensure Null-Terminated

            if (Config.Default.TextOutWUndoRemap)
                InStr = Remaper.Default.Restore(InStr);

            if (Config.Default.TextOutWRemapAlt)
                InStr = RemaperAlt.Default.Apply(InStr, null);

            return Bypass(dc, xStart, yStart, InStr, InStr.Count());
        }
    }
}
