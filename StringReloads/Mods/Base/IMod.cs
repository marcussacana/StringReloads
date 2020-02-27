namespace StringReloads.Mods.Base
{
    public interface IMod
    {
        public string Name { get; }
        public void Install();
        public void Uninstall();
    }
}
