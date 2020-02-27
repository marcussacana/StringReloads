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
        public uint Width;
        public uint Height;
        public uint Charset;
    }
}
