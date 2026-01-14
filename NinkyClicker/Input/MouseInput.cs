using System.Runtime.InteropServices;

namespace NinkyClicker.Input;

[StructLayout(LayoutKind.Sequential)]
public struct MouseInput
{
        public int X;
        public int Y;
        public uint MouseData;
        public uint Flags;
        public uint Time; 
        public IntPtr ExtraInfo;
}