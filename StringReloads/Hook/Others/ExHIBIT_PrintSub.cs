using StringReloads.Engine.String;
using StringReloads.Hook.Base;

namespace StringReloads.Hook.Others
{
    unsafe class ExHIBIT_PrintSub3 : Hook<ExHIBIT_PrintSub3Delegate>
    {
        public override string Library => "resident.dll";

        public override string Export => "?printSub@RetouchPrintManager@@AAE_NPBDAAVUxPrintData@@K@Z";

        public override void Initialize()
        {
            HookDelegate = hExHIBIT_PrintSub3;
            Compile();
        }

        void hExHIBIT_PrintSub3(void* This, void* Text, void* a2, void * a3) {
            Text = EntryPoint.SRL.ProcessString((CString)Text);
            Bypass(This, Text, a2, a3);
        }
    }
}
