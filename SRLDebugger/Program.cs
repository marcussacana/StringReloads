using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SRLDebugger
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            byte[] Data = Encoding.UTF8.GetBytes("Hello World!\x0");
            fixed (void* pStr = &Data[0])
            {
                var NewPtr = StringReloads.EntryPoint.Process(pStr);
                for (int i = 0; i < 100000; i++)
                {
                    Console.Title = "DBG - " + i;
                    Console.WriteLine(System.IO.File.ReadAllText("SRLDebugger.exe.config"));
                }
            }   
        }
    }
}
