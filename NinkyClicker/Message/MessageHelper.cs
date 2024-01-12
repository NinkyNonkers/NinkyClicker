using System.Diagnostics;
using System.Runtime.InteropServices;
using NinkyClicker.Keys;

namespace NinkyClicker.Message;

public static class MessageHelper
{
    [DllImport("User32.dll")]
    private static extern IntPtr SendMessage(IntPtr hwnd, uint msg, int wParam, uint lParam); 
    
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);
    
    public static void Send(this Process proc, MessageType type, MessageParameter parameter, VirtualKey code)
    {
        uint scanCode = MapVirtualKey((uint)code, 0);
        uint lParam = 0x00000001 | (scanCode << 16);
        if (type is MessageType.LButtonUp or MessageType.RButtonUp)
            lParam |= 0xC0000000;
        SendMessage(proc.MainWindowHandle, (uint) type, (int) parameter, lParam);
    }
}