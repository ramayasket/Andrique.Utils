using System.Runtime.InteropServices;

namespace Andrique.Shell
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LPARAM
    {
        [FieldOffset(0)]
        public LPARAMFIELDS fields;

        [FieldOffset(0)]
        public uint value;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LPARAMFIELDS
    {
        public ushort wRepeat;
        public byte bScan;
        public byte bUnused;
        public static int Size => Marshal.SizeOf(typeof(LPARAMFIELDS));
    }
}
