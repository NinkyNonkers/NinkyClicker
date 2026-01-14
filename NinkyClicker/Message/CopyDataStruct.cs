namespace NinkyClicker.Message;

[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
public struct CopyDataStruct
{
    public uint dwData;
    public int cbData;
    public IntPtr lpData;
}