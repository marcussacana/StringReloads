using StringReloads.Engine.String;
using StringReloads.Hook.Base;

namespace StringReloads.Hook.Others
{
    unsafe class ExHIBIT_Say11 : Hook<ExHIBIT_Say11Delegate>
    {
        public override string Library => "resident.dll";

                                        //?say@RetouchAdvCharacter@@QAEXHPBD0_NHHHHPAVRetouchPrintParam@@KPAVUxRuFukidashiData@@@Z
        public const string ExportName = "?say@RetouchAdvCharacter@@QAEXHPBD0_NHHHHPAVRetouchPrintParam@@KPAVUxRuFukidashiData@@@Z";

        public override string Export => ExportName;

        public override void Initialize()
        {
            HookDelegate = hExHIBIT_Say12;
            Compile();
        }

        void hExHIBIT_Say12(void* This, void* a1, void* a2, byte* Text, void* a4, void* a5, void* a6, void* a7, void* a8, void* a9, void* a10, void* a11) {
            Text = EntryPoint.SRL.ProcessString((CString)Text);
            Bypass(This, a1, a2, Text, a4, a5, a6, a7, a8, a9, a10, a11);
        }
    }
}
