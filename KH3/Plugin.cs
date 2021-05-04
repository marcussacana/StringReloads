using StringReloads.Engine;
using StringReloads.Engine.Interface;
using StringReloads.Hook.Base;

namespace KH3
{
    public class Plugin : IPlugin
    {
        SRL Engine;
        public string Name => "KH3";

        public IAutoInstall[] GetAutoInstallers()
        {
            return new IAutoInstall[] {
                new AutoInstall(Engine)
            };
        }

        public IEncoding[] GetEncodings() => null;

        public Hook[] GetHooks() => null;

        public IMatch[] GetMatchs() => null;

        public IStringModifier[] GetModifiers() => null;

        public IMod[] GetMods() => null;

        public IReloader[] GetReloaders() => null;

        public void Initialize(SRL Engine)
        {
            this.Engine = Engine;
        }
    }
}
