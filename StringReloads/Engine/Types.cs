using System.ComponentModel;

namespace StringReloads.Engine
{
    public static class Types
    {
        public delegate void FlagTrigger(LSTFlag Entry, CancelEventArgs Args);
    }

    public struct FontRemap {
        public string From;
        public string To;
        public int Width;
        public int Height;
        public uint Charset;
    }


    public struct Filter
    {
        public string DenyList;
        public string IgnoreList;
        public string QuoteList;
        public int Sensitivity;
        public bool UseDB;
        public bool FromAsian;
    }
    public struct Quote
    {
        public char Start;
        public char End;
    }
}
