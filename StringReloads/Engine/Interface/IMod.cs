namespace StringReloads.Engine.Interface
{
    public interface IMod
    {
        public string Name { get; }
        public void Install();
        public void Uninstall();

        public bool IsCompatible();
    }
}
