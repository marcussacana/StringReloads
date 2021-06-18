using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using System.Runtime.InteropServices;

namespace KH3
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    unsafe delegate void* DispText(void* A, void* Text, void* C, void* D);
    unsafe class DisplayTextHook : Hook<DispText>
    {
        SRL Engine;
        public DisplayTextHook(SRL Engine) {
            this.Engine = Engine;
        }
        public override string Library => null;

        public override string Export => null;

        public override void Initialize()
        {
            HookDelegate = new DispText(DisplayText);

            var FuncPos = (void*)((long)Config.Default.GameBaseAddress + 0x596B120L);
            //mov qword ptr ss:[rsp+8],rbx
            //mov qword ptr ss:[rsp+10],rsi
            //push rdi
            //sub rsp,20
            //mov rsi, rcx
            //mov rbx, r8
            //lea rcx, qword ptr ss:[rsp+48]
            //mov rdi, rdx
            //call kingdom hearts iii.7FF63E5FB240
            //mov r8,rbx
            //lea rcx,qword ptr ss:[rsp+48]
            //mov rdx, rdi
            //call kingdom hearts iii.7FF63E5FBB00
            //mov rdx,qword ptr ss:[rsp+48]
            //mov rax,7FFFFFFFFFFFFFFF

            Compile(FuncPos);
        }

        private void* DisplayText(void* A, void* Text, void* C, void* D)
        {
            string NewText = Engine.ProcessString((WCString)Text);
            if (NewText != null)
                Text = (WCString)NewText;

            return Bypass(A, Text, C, D);
        }
    }
}
