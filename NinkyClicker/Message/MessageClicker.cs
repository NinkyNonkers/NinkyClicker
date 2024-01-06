using System.Diagnostics;
using NinkyClicker.Keys;

namespace NinkyClicker.Message;

public class MessageClicker : Clicker
{
    private readonly Process _proc;
    
    public MessageClicker(ushort cps, Process proc) : base(cps)
    {
        _proc = proc;
    }

    protected override void LeftClick()
    {
        _proc.Send(MessageType.LButtonDown, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(5);
        _proc.Send(MessageType.LButtonUp, MessageParameter.LButton, VirtualKey.LButton);
    }

    protected override void RightClick()
    {
        _proc.Send(MessageType.RButtonDown, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(5);
        _proc.Send(MessageType.RButtonUp, MessageParameter.RButton, VirtualKey.RButton);
    }
}