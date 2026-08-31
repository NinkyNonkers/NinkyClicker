using System.Runtime.InteropServices;

namespace NinkyClicker.Hook;

[StructLayout(LayoutKind.Sequential)]
public class HookMessage
{
    public IntPtr hwnd;
    public uint message;
    public IntPtr wParam;
    public IntPtr lParam;
    public uint time;
    public POINT pt;
}

[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
public struct MSLLHOOKSTRUCT
{
    public POINT pt;       
    public uint mouseData;  
    public uint flags;     
    public uint time;       
    public IntPtr dwExtraInfo;
}