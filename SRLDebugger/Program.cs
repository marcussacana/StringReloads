using StringReloads.Engine.String;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SRLDebugger
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            CString DataA = "You got Caw (x645115454)";
            CString DataB = "You got Bone (x16545615)";
            string OutputA = (CString)StringReloads.EntryPoint.Process(DataA);
            string OutputB = (CString)StringReloads.EntryPoint.Process(DataB);
            System.Threading.Thread.Sleep(10000);
        }
    }
}
