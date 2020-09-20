using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Hook.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Mods
{
    class CoInitializeFix : IMod
    {
        public string Name => "CoInitializeFix";

        public void Install()
        {
            EntryPoint.SRL.EnableHook(new CoInitialize());
        }

        public bool IsCompatible()
        {
            return true;
        }

        public void Uninstall() { }
    }
}
