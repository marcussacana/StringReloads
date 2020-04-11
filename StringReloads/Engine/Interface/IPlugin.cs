using StringReloads.AutoInstall.Base;
using StringReloads.Mods.Base;
using StringReloads.StringModifier;

namespace StringReloads.Engine.Interface
{
    public interface IPlugin
    {
        public string Name { get; }
        public void Initialize(Main Engine);
        public Hook.Base.Hook[] GetHooks();
        public IAutoInstall[] GetAutoInstallers();
        public IMatch[] GetMatchs();
        public IMod[] GetMods();
        public IStringModifier[] GetModifiers();
    }
}
