using static StringReloads.Hook.Base.Extensions;
using StringReloads.Engine;
using StringReloads.Engine.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StringReloads.Hook.Win32;
using StringReloads.Hook.Others;
using StringReloads.Engine.String;
using System.IO;

namespace StringReloads.AutoInstall
{
    unsafe class EntisGLS : IAutoInstall
    {
        public string Name => "EntisGLS";

        LoadResource LoadResourceHook;

        EntisGLS_eslHeapAllocate eslHeapAllocateHook;
        EntisGLS_eslHeapFree eslHeapFreeHook;

        public void Install()
        {
            if (LoadResourceHook == null)
            {
                LoadResourceHook = new LoadResource();
                eslHeapAllocateHook = new EntisGLS_eslHeapAllocate();
                eslHeapFreeHook = new EntisGLS_eslHeapFree();
            }

            LoadResourceHook.OnCalled += LoadResourceCalled;
            LoadResourceHook.Install();
        }

        private void LoadResourceCalled()
        {
            LoadResourceHook.Uninstall();

            Log.Debug("EntisGLS LoadResource Called");

            Heaps.Clear();

            eslHeapAllocateHook.OnHeapAllocated += HeapAllocated;
            eslHeapAllocateHook.Install();
        }

        List<IntPtr> Heaps = new List<IntPtr>();

        private void HeapAllocated(IntPtr Heap, int Size)
        {
            CheckHeaps();

            if (Size != 0x10000)
                return;

            if (!eslHeapFreeHook.Enabled)
            {
                eslHeapFreeHook.OnHeapDisposed += HeapDisposed;
                eslHeapFreeHook.Install();
            }

            Heaps.Add(Heap);
        }

        private void HeapDisposed(IntPtr Heap)
        {
            if (Heaps.Contains(Heap))
                Heaps.Remove(Heap);
            
            CheckHeaps();
        }

        void CheckHeaps()
        {
            foreach (var Heap in Heaps)
            {
                try
                {
                    CString HeapData = Heap;
                    string HeapString = HeapData;
                    if (HeapString.Contains("<?xml"))
                    {
                        eslHeapAllocateHook.Uninstall();
                        eslHeapFreeHook.Uninstall();

                        //LoadResourceHook.Install();

                        ProcessXml(HeapData);
                    }
                }
                catch { }
            }
        }

        string XMLPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EntisGLSConfig.xml");

        private void ProcessXml(CString XML)
        {
            if (!File.Exists(XMLPath))
            {
                Log.Debug("EntisGLS Engine Config Not Found: Dumping...");
                File.WriteAllBytes(XMLPath, XML.ToArray());
            }
            else
            {
                Log.Debug("EntisGLS Engine Config Not Found: Modding...");
                var DefaultRead = File.ReadAllText(XMLPath);

                Encoding Encoding = Encoding.GetEncoding(932);

                if (DefaultRead.Contains("\"UTF-8\"") || DefaultRead.Contains("\"UTF_8\""))
                    Encoding = Encoding.UTF8;

                XML.SetContent(File.ReadAllText(XMLPath, Encoding));
            }
        }

        public bool IsCompatible()
        {
            var MainModule = Config.GameBaseAddress;
            return GetProcAddress(MainModule, "eslHeapAllocate") != null;
        }

        public void Uninstall()
        {
            LoadResourceHook.Uninstall();
            eslHeapAllocateHook.Uninstall();
            eslHeapFreeHook.Uninstall();
        }
    }
}
