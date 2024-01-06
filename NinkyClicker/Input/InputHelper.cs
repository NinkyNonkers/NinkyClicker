using System.Runtime.InteropServices;
using NinkyClicker.Input.Flags;

namespace NinkyClicker.Input;

public static class InputHelper
{
    [DllImport("User32.dll")]
    private static extern uint SendInput(uint number, InputPayload[] pInputs, int size);

    public static void SendMouseEvent(MouseEvent m)
    {
        InputPayload input = new() { Type = (uint) InputType.Mouse, Data = new InputRegister { Mouse = new MouseInput {Flags = (uint) m}}};
        Send(input);
    }
    
    private static unsafe void Send(InputPayload input)
    {
        InputPayload[] i = { input };
        uint res = SendInput(1, i, sizeof(InputPayload));
        HandleStatusResponse(res);
    }

    private static void HandleStatusResponse(uint resp)
    {
        switch (resp)
        {
            case 0:
                throw new Exception("Error sending mouse click");
        }
    }
    
}