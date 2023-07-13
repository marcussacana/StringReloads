using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using System;

namespace StringReloads.Hook.Others
{
    unsafe class ExHIBIT_Question : Hook<ExHIBIT_QuestionDelegate>
    {
        public override string Library => "resident.dll";

        public const string ExportName = "?prepareQuestion@RetouchSystem@@QAEXHPBD@Z";

        public override string Export => ExportName;

        public override void Initialize()
        {
            HookDelegate = hExHIBIT_Question;
            Compile();
        }

        private void hExHIBIT_Question(void* This, void* a1, void* Text)
        {
            Text = EntryPoint.SRL.ProcessString((CString)Text);
            Bypass(This, a1, Text);
        }
    }
}
