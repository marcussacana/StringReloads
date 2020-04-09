namespace StringReloads.StringModifier
{
    public interface IStringModifier
    {
        public string Name { get; }
        public bool CanRestore { get; }
        public string Apply(string String, string Original);
        public string Restore(string String);
    }
}
