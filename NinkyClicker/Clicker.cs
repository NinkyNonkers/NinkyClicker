using NinkyClicker.Keys;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker;

public abstract class Clicker
{
    private uint _shouldClick = 1;
    private bool _isLeftClick;
    private bool _isRightClick;
    private uint _clicks;

    private readonly int _sleepTime;


    protected Clicker(ushort cps)
    {
        _sleepTime = (int) Math.Round((float)(1000 / cps)) - 20;
    }

    public void Start()
    {
        Project.LoggingProxy.LogInfo("Starting...");
        ThreadPool.QueueUserWorkItem(_ => ModifyThread());
        ThreadPool.QueueUserWorkItem(_ => ClickThread());
        ThreadPool.QueueUserWorkItem(_ => LogThread());
        Project.LoggingProxy.LogSuccess($"Started a new {GetType().Name} successfully");
    }

    private void ModifyThread()
    {
        while (true)
        {
            bool shouldClick = !KeyHelper.IsHeld(VirtualKey.LControl);
            if (_shouldClick == 1 && !shouldClick)
                Interlocked.Decrement(ref _shouldClick);
            else if (_shouldClick == 0 && shouldClick)
                Interlocked.Increment(ref _shouldClick);
            
            _isLeftClick = KeyHelper.IsHeld(VirtualKey.LButton);
            _isRightClick = KeyHelper.IsHeld(VirtualKey.RButton);
        }
    }

    private void LogThread()
    {
        while (true)
        {
            Thread.Sleep(300000);
            Project.LoggingProxy.LogUpdate($"{_clicks} in the past 5 minutes.");
            Interlocked.Exchange(ref _clicks, 0);
        }
    }

    private void ClickThread()
    {
        while (true)
        {
            Thread.Sleep(_sleepTime);
            
            if (_shouldClick == 0 || (!_isLeftClick && !_isRightClick))
                continue;
            
            Interlocked.Increment(ref _clicks);
            
            if (_isLeftClick)
                LeftClick();
            if (_isRightClick)
                RightClick();
        }
    }

    protected abstract void LeftClick();
    protected abstract void RightClick();
}
