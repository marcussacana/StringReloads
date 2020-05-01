namespace StringReloads.Engine.Interface
{
    public interface IAutoInstall
    {
        public string Name { get; }

        public bool IsCompatible();
        public void Install();
        public void Uninstall();
    }
}
