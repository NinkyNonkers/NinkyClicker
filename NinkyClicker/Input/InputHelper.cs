using System.Runtime.InteropServices;
using NinkyClicker.Input.Flags;

namespace NinkyClicker.Input;

public static class InputHelper
{
    [DllImport("User32.dll", SetLastError = true)]
    private static extern uint SendInput(uint number, GlobalInput[] pInputs, int size);

    public static unsafe void SendMouseEvent(MouseEvent m)
    {
        GlobalInput[] input = { new() { Type = (uint)InputType.Mouse, MouseInput = { Flags = (uint) m, ExtraInfo = 0, Time = 0 } } };
        uint res = SendInput(1, input, sizeof(GlobalInput));
        switch (res)
        {
            case 0:
                throw new Exception("Error sending mouse click: " + Marshal.GetLastWin32Error());
        }    
    }
}