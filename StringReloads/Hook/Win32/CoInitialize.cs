using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringReloads.Hook.Win32
{
    class CoInitialize : Base.Hook<CoInitializeDelegate>
    {
        public override string Name => "CoInitialize";
        public override string Library => "ole32.dll";

        public override string Export => "CoInitialize";

        public override void Initialize()
        {
            HookDelegate = hCoInitialize;
            Compile();
        }

        int hCoInitialize(IntPtr Reserved) {
            Bypass(Reserved);
            return 0;
        }
    }
}
