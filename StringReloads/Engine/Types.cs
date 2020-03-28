using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace StringReloads.Engine
{
    public static class Types
    {
        internal delegate void FlagTrigger(LSTParser.LSTFlag Entry, CancelEventArgs Args);
    }

    struct FontRemap {
        public string From;
        public string To;
        public int Width;
        public int Height;
        public uint Charset;
    }


    struct Filter
    {
        public string DenyList;
        public string IgnoreList;
        public string QuoteList;
        public int Sensitivity;
        public bool UseDB;
        public bool FromAsian;
    }
    struct Quote
    {
        public char Start;
        public char End;
    }
}
