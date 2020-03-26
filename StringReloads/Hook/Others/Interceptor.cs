using StringReloads.Hook.Base;

namespace StringReloads.Hook
{
    unsafe class Interceptor : Intercept
    {
        public Interceptor(void* Address, InterceptDelegate Action) {
            _HookFunc = Action;
            Compile(Address);
        }

        private InterceptDelegate _HookFunc;
        public override InterceptDelegate HookFunction => _HookFunc;
    }
}
