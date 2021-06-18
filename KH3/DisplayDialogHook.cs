using StringReloads.Engine;
using StringReloads.Engine.String;
using StringReloads.Hook.Base;
using System.Runtime.InteropServices;

namespace KH3
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    unsafe delegate void* DispDiag(void* A, void* B, void* C, void** Text, void* E, void* F);
    unsafe class DisplayDialogHook : Hook<DispDiag>
    {
        SRL Engine;
        public DisplayDialogHook(SRL Engine) {
            this.Engine = Engine;
        }
        public override string Library => null;

        public override string Export => null;

        public override void Initialize()
        {
            HookDelegate = new DispDiag(DisplayDiag);

            var FuncPos = (void*)((long)Config.Default.GameBaseAddress + 0x1A4BC90L);
            //push rbp
            //push rsi
            //push rdi
            //push r12
            //push r13
            //push r14
            //push r15
            //lea rbp, qword ptr ss:[rsp-1F0]
            //sub rsp,2F0
            //mov qword ptr ss:[rbp-10],FFFFFFFFFFFFFFFE
            //mov qword ptr ss:[rsp+338],rbx
            //mov rax,qword ptr ds:[7FF641A07140]
            //xor rax, rsp
            //mov qword ptr ss:[rbp+1E0],rax
            //mov rbx,r9
            //mov r13d,r8d
            //mov dword ptr ss:[rsp+30],r8d
            //mov edi,edx
            //mov dword ptr ss:[rsp+40],edx
            //mov r15,rcx
            //mov rax,qword ptr ss:[rbp+250]
            //mov qword ptr ss:[rbp-18],rax

            Compile(FuncPos);
        }

        private void* DisplayDiag(void* A, void* B, void* C, void** Text, void* E, void* F)
        {
            string NewText = Engine.ProcessString((WCString)(*Text));
            if (NewText != null)
                *Text = (WCString)NewText;

            return Bypass(A, B, C, Text, E, F);
        }
    }
}
