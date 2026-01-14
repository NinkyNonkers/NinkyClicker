using System.Runtime.InteropServices;

namespace NinkyClicker.Input;

[StructLayout(LayoutKind.Sequential)]
public struct GlobalInput
{
    public uint Type;
    public MouseInput MouseInput; 
}