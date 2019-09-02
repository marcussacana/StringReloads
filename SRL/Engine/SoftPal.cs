using System;
using System.Runtime.InteropServices;

namespace SRL.Engine
{
    class SoftPal : IEngine
    {
        public string Name => "SoftPAL";

        DrawText HookDelegate;
        DrawText RealDelegate;

        UnmanagedHook<DrawText> HookManager;

        public bool IsCompatible()
        {
            foreach (var Import in UnmanagedHook.GetImports())
            {
                if (Import.Module.ToLower() == "pal.dll" && Import.Function?.ToLower() == "drawtext")
                {
                    RealDelegate = (DrawText)Marshal.GetDelegateForFunctionPointer(Import.FunctionAddress, typeof(DrawText));

                    HookDelegate = DrawTextHook;

                    HookManager = new UnmanagedHook<DrawText>(Import, HookDelegate);

                    return true;
                }
            }

            return false;
        }

        public void InstallStrHook() => HookManager.Install();

        public void UninstallStrHook() => HookManager.Uninstall();

        public void DrawTextHook(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23)
        {
            Text = StringReloader.ProcessReal(Text);
            RealDelegate(Text, a2, a3, a4, a5, a6, a7, a8, a9, a10, a11, a12, a13, a14, a15, a16, a17, a18, a19, a20, a21, a22, a23);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        delegate void DrawText(IntPtr Text, IntPtr a2, IntPtr a3, IntPtr a4, IntPtr a5, IntPtr a6, IntPtr a7, IntPtr a8, IntPtr a9, IntPtr a10, IntPtr a11, IntPtr a12, IntPtr a13, IntPtr a14, IntPtr a15, IntPtr a16, IntPtr a17, IntPtr a18, IntPtr a19, IntPtr a20, IntPtr a21, IntPtr a22, IntPtr a23);

    }
}
