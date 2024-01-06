using System.Diagnostics;
using NinkyClicker.Keys;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker.Message;

public class MessageClicker : Clicker
{
    private readonly Process _proc;
    
    public MessageClicker(ushort cps, Process proc) : base(cps)
    {
        _proc = proc;
        _proc.Exited += (_, _) => CheckProcess();
    }

    protected override void LeftClick()
    {
        CheckProcess();
        _proc.Send(MessageType.LButtonDown, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(5);
        _proc.Send(MessageType.LButtonUp, MessageParameter.LButton, VirtualKey.LButton);
    }

    protected override void RightClick()
    {
        CheckProcess();
        _proc.Send(MessageType.RButtonDown, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(5);
        _proc.Send(MessageType.RButtonUp, MessageParameter.RButton, VirtualKey.RButton);
    }

    private void CheckProcess()
    {
        if (_proc is { HasExited: false } || !Running)
            return;
        
        Project.LoggingProxy.LogFatal("Target process has closed.");
        Dispose();
    }
}