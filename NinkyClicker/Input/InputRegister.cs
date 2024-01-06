using System.Runtime.InteropServices;

namespace NinkyClicker.Input;

[StructLayout(LayoutKind.Explicit)]
public struct InputRegister
{
    [FieldOffset(0)]
    public MouseInput Mouse;

    /// <summary>
    /// The <see cref="KEYBDINPUT"/> definition.
    /// </summary>
    [FieldOffset(0)]
    public KeyboardInput Keyboard;

    /// <summary>
    /// The <see cref="HARDWAREINPUT"/> definition.
    /// </summary>
    [FieldOffset(0)]
    public HardwareInput Hardware;
}