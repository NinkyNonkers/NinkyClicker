using System.Diagnostics;
using System.Runtime.InteropServices;
using NinkyClicker.Keys;

namespace NinkyClicker.Message;

public static class MessageHelper
{
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern long SendMessage(IntPtr hwnd, uint msg, int wParam, uint lParam); 
    
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessageTimeout(IntPtr hwnd, uint msg, int wParam, uint lParam, uint fuFlags, uint timeout);
    
    //[DllImport("User32.dll", CharSet = CharSet.Auto)]
    //private static extern IntPtr PostMessage(IntPtr hwnd, uint msg, int wParam, uint lParam); 
    
    [DllImport("User32.dll", CharSet = CharSet.Auto)]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);
    
    public static void Send(this Process proc, PushType type, MessageParameter parameter, VirtualKey code)
    {
        uint lParam = GetParam(type, code);
        long result = SendMessage(proc.MainWindowHandle, (uint) GetType(type, code), (int) parameter, lParam);
    }

    public static void SendTimeout(this Process proc, PushType type, MessageParameter p, VirtualKey code)
    {
        uint lParam = GetParam(type, code);
        long result = SendMessageTimeout(proc.MainWindowHandle, (uint) GetType(type, code), (int)p, lParam, 0x0000, 500);
    }

    private static MessageType GetType(PushType type, VirtualKey code)
    {
        switch (code)
        {
            case VirtualKey.RButton:
                return type is PushType.Up ? MessageType.RButtonUp : MessageType.RButtonDown;
            case VirtualKey.LButton:
                return type is PushType.Up ? MessageType.LButtonUp : MessageType.LButtonDown;
            default:
                return MessageType.RButtonUp;
        }
    }

    private static uint GetParam(PushType type, VirtualKey code)
    {
        uint scanCode = MapVirtualKey((uint)code, 0);
        uint lParam = 0x00000001 | (scanCode << 16);
        if (type is PushType.Up)
            lParam |= 0xC0000000;
        return lParam;
    }
}