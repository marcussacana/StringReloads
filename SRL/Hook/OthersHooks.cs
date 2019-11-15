using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SRL
{
    static partial class StringReloader
    {
        public static void InstallCoInitializeHooks()
        {
            dCoInitialize = new CoInitializeDel(CoInitializeHook);
            dCoInitializeEx = new CoInitializeExDel(CoInitializeExHook);

            hCoInitialize = AutoHookCreator("Ole32.dll", "CoInitialize", dCoInitialize);
            hCoInitializeEx = AutoHookCreator("Ole32.dll", "CoInitializeEx", dCoInitializeEx);

            hCoInitialize.Install();
            hCoInitializeEx.Install();
        }

        public static IntPtr CoInitializeHook(IntPtr Reserved) => IntPtr.Zero;
        public static IntPtr CoInitializeExHook(IntPtr Reserved, IntPtr dwCoInit) => IntPtr.Zero;

        public static CoInitializeDel dCoInitialize;
        public static CoInitializeExDel dCoInitializeEx;

        public static UnmanagedHook hCoInitialize;
        public static UnmanagedHook hCoInitializeEx;


        public delegate IntPtr CoInitializeDel(IntPtr Reserved);
        public delegate IntPtr CoInitializeExDel(IntPtr Reserved, IntPtr dwCoInit);
    }
}
