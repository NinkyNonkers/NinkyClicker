using System.Diagnostics;
using System.Runtime.InteropServices;
using NinkyClicker.Keys;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker.Message;

public class MessageClickExecutor : ClickExecutor
{
    private readonly Process _proc;
    private readonly Clicker _parent;
    
    public MessageClickExecutor(Process proc, Clicker parent)
    {
        _proc = proc;
        _parent = parent;
        _proc.Exited += (_, _) => CheckProcess();
    }

    public override void LeftClick()
    {
        CheckProcess();
        _proc.Send(PushType.Down, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(_parent.SleepTime / 2 + 5);
        _proc.Send(PushType.Up, MessageParameter.LButton, VirtualKey.LButton);
        Thread.Sleep(_parent.SleepTime / 2);
    }

    public override void RightClick()
    {
        CheckProcess();
        _proc.Send(PushType.Down, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(_parent.SleepTime / 2 + 5);
        _proc.Send(PushType.Up, MessageParameter.RButton, VirtualKey.RButton);
        Thread.Sleep(_parent.SleepTime / 2);
    }

    private void CheckProcess()
    {
        if (_proc is { HasExited: false } || !_parent.Running)
            return;
        
        Project.LoggingProxy.LogFatal("Target process has closed.");
        _parent.Dispose();
    }
}