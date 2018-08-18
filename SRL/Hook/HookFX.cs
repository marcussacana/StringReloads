using System;
using System.Runtime.InteropServices;

public class FxHook : IDisposable {

    const int nBytes = 5;

    IntPtr addr;
    Protection old;
    byte[] src = new byte[5];
    byte[] dst = new byte[5];

    public FxHook(IntPtr source, IntPtr destination) {
        VirtualProtect(source, nBytes, Protection.PAGE_EXECUTE_READWRITE, out old);
        Marshal.Copy(source, src, 0, nBytes);
        dst[0] = 0xE9;
        var Result = (int)SRL.StringReloader.ParsePtr(destination) - (int)SRL.StringReloader.ParsePtr(source) - nBytes;
        var dx = BitConverter.GetBytes(Result);
        Array.Copy(dx, 0, dst, 1, nBytes - 1);
        addr = source;
    }
    public FxHook(IntPtr source, Delegate destination) : this(source, Marshal.GetFunctionPointerForDelegate(destination)) {
    }

    public FxHook(string library, string function, Delegate destination) : this(GetProcAddress(LoadLibrary(library), function), destination) {
    }

    public void Install() {
        Marshal.Copy(dst, 0, addr, nBytes);
    }

    public void Uninstall() {
        Marshal.Copy(src, 0, addr, nBytes);
    }

    public void Dispose() {
        Uninstall();
        Protection x;
        VirtualProtect(addr, nBytes, old, out x);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, Protection flNewProtect, out Protection lpflOldProtect);

    [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
    static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
    static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

    public enum Protection {
        PAGE_NOACCESS = 0x01,
        PAGE_READONLY = 0x02,
        PAGE_READWRITE = 0x04,
        PAGE_WRITECOPY = 0x08,
        PAGE_EXECUTE = 0x10,
        PAGE_EXECUTE_READ = 0x20,
        PAGE_EXECUTE_READWRITE = 0x40,
        PAGE_EXECUTE_WRITECOPY = 0x80,
        PAGE_GUARD = 0x100,
        PAGE_NOCACHE = 0x200,
        PAGE_WRITECOMBINE = 0x400
    }

}
