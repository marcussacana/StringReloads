using StringReloads.Hook.Base;

namespace StringReloads.Hook
{
    unsafe class ManagedInterceptor : Intercept
    {
        ManagedInterceptDelegate HookDelegate;
        public ManagedInterceptor(void* Address, ManagedInterceptDelegate Action)
        {
            HookDelegate = Action;
            Compile(Address);
        }

        public override ManagedInterceptDelegate ManagedHookFunction => HookDelegate;
    }
}
