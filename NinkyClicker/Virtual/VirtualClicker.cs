using NinkyClicker.Keys;

namespace NinkyClicker.Virtual;

public class VirtualClicker : Clicker
{
    public VirtualClicker(ushort cps) : base(cps)
    {
    }

    public VirtualClicker(ushort cps, ClickerConfiguration configuration) : base(cps, configuration)
    {
    }
    
    protected override void ModifyThread()
    {
        while (Running)
        {
            bool shouldClick = !VirtualKeyHelper.IsHeld(VirtualKey.LControl);
            if (ShouldClick == 1 && !shouldClick)
                Interlocked.Decrement(ref ShouldClick);
            else if (ShouldClick == 0 && shouldClick)
                Interlocked.Increment(ref ShouldClick);
            
            IsLeftClick = VirtualKeyHelper.IsHeld(VirtualKey.LButton);
            IsRightClick = VirtualKeyHelper.IsHeld(VirtualKey.RButton);
        }
    }
}