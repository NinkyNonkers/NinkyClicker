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
        _proc.Send(PushType.Down, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(SleepTime / 2 + 5);
        _proc.Send(PushType.Up, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(SleepTime / 2);
    }

    protected override void RightClick()
    {
        CheckProcess();
        _proc.Send(PushType.Down, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(SleepTime / 2 + 5);
        _proc.Send(PushType.Up, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(SleepTime / 2);
    }

    private void CheckProcess()
    {
        if (_proc is { HasExited: false } || !Running)
            return;
        
        Project.LoggingProxy.LogFatal("Target process has closed.");
        Dispose();
    }
}