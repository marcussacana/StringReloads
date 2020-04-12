namespace StringReloads.Engine.Interface
{
    public interface IMatch
    {
        public char? ResolveRemap(char Char);
        public bool HasMatch(string String);
        public bool HasValue(string String);
        public LSTEntry? MatchString(string String);
        public FontRemap? ResolveRemap(string Facename, int Width, int Height, uint Charset);
    }
}
