using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.StringModifier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringReloads.Hook
{
    public unsafe class WideCharToMultiByte : Base.Hook<WideCharToMultiByteDelegate>
    {
        List<Base.Hook> Parents = new List<Base.Hook>();
        void* Target = null;

        public WideCharToMultiByte()
        {
            if (Target == null && Config.Default.ImportHookEx)
            {
                foreach (var Module in Config.GameModules)
                    Parents.Add(new WideCharToMultiByte(Module.BaseAddress.ToPointer()));
            }
        }
        private WideCharToMultiByte(void* Target) {
            this.Target = Target;
            Install();
        }

        public override string Library => "Kernel32.dll";

        public override string Export => "WideCharToMultiByte";

        public new void Install()
        {
            base.Install();

            foreach (var Parent in Parents)
                Parent.Install();
        }

        public new void Uninstall()
        {
            base.Uninstall();

            foreach (var Parent in Parents)
                Parent.Install();
        }

        public override void Initialize()
        {
            if (Config.Default.ImportHook || Config.Default.ImportHookEx) {
                HookDelegate = new WideCharToMultiByteDelegate(PersistentWideCharToMultiByte);
                Compile(true, Target == null ? (IntPtr)null : new IntPtr(Target));
            }
            else { 
                HookDelegate = new WideCharToMultiByteDelegate(hWideCharToMultiByte);
                Compile();
            }
        }

        private int hWideCharToMultiByte(uint CodePage, uint dwFlags, byte* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, byte* lpDefaultChar, out bool lpUsedDefaultChar)
        {
            Uninstall();
            int Rst = PersistentWideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, lpMultiByteStr, cbMultiByte, lpDefaultChar, out lpUsedDefaultChar);
            Install();
            return Rst;
        }

        private int PersistentWideCharToMultiByte(uint CodePage, uint dwFlags, byte* lpWideCharStr, int cchWideChar, byte* lpMultiByteStr, int cbMultiByte, byte* lpDefaultChar, out bool lpUsedDefaultChar)
        {
            if (Config.Default.WideCharToMultiByteCodePage >= 0)
                CodePage = (uint)Config.Default.WideCharToMultiByteCodePage;

            Encoding WriteEncoding = null;
            if (CodePage != 0 && Config.Default.WideCharToMultiByteAutoEncoding) {
                WriteEncoding  = Config.Default.WriteEncoding;
                Config.Default.WriteEncoding = Encoding.GetEncoding((int)CodePage);
            }

            if (cchWideChar > 0)
            {
                byte[] Buffer = new byte[(cchWideChar + 1) * 2];
                for (int i = 0; i < cchWideChar * 2; i++)
                {
                    Buffer[i] = *(lpWideCharStr + i);
                }
                fixed (void* pBuffer = &Buffer[0])
                {

                    var NewStr = (byte*)EntryPoint.ProcessW((WCString)pBuffer);
                    if (NewStr != lpWideCharStr)
                    {
                        lpWideCharStr = NewStr;
                        cchWideChar = ((WCString)lpWideCharStr).Count() / 2;
                        CodePage = (uint)Config.Default.WriteEncoding.CodePage;
                    }
                }
            }
            else
            {
                var NewStr = (byte*)EntryPoint.ProcessW((WCString)lpWideCharStr);
                if (NewStr != lpWideCharStr)
                {
                    lpWideCharStr = NewStr;
                    CodePage = (uint)Config.Default.WriteEncoding.CodePage;
                }
            }

            if (Config.Default.WideCharToMultiByteUndoRemap)
                lpWideCharStr = (WCString)Remaper.Default.Restore((WCString)lpWideCharStr);

            if (Config.Default.WideCharToMultiByteRemapAlt)
                lpWideCharStr = (WCString)RemaperAlt.Default.Restore((WCString)lpWideCharStr);

            if (WriteEncoding != null)
                Config.Default.WriteEncoding = WriteEncoding;

            return Bypass(CodePage, dwFlags, lpWideCharStr, cchWideChar, lpMultiByteStr, cbMultiByte, lpDefaultChar, out lpUsedDefaultChar);
        }
    }
}
