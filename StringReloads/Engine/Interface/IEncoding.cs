﻿using System.Text;

namespace StringReloads.Engine.Interface
{
    public interface IEncoding
    {
        public string Name { get; }
        public Encoding Encoding { get; }
    }
}
