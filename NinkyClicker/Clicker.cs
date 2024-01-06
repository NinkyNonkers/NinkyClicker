using NinkyClicker.Keys;

namespace NinkyClicker;

public abstract class Clicker
{
    private uint _shouldClick = 1;
    private bool _isLeftClick;
    private bool _isRightClick;

    private readonly int _sleepTime;

    private uint _clicks;

    protected Clicker(ushort cps)
    {
        _sleepTime = (int) Math.Round((float)(1000 / cps)) - 20;
    }

    public void Start()
    {
        ThreadPool.QueueUserWorkItem(_ => ModifyThread());
        ThreadPool.QueueUserWorkItem(_ => ClickThread());
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

    private void ClickThread()
    {
        while (true)
        {
            Thread.Sleep(_sleepTime);
            if (_shouldClick == 0)
                continue;
            if (_isLeftClick)
                LeftClick();
            if (_isRightClick)
                RightClick();
        }
    }

    protected abstract void LeftClick();
    protected abstract void RightClick();
}
