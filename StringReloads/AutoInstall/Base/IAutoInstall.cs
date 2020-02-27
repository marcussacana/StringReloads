namespace StringReloads.AutoInstall.Base
{
    public interface IAutoInstall
    {
        public string Name { get; }

        public bool IsCompatible();
        public void Install();
        public void Uninstall();
    }
}
