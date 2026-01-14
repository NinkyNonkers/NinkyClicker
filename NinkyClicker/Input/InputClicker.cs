using NinkyClicker.Input.Flags;

namespace NinkyClicker.Input;

public class InputClicker : Clicker
{
    protected override void LeftClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.LeftDown);
        Thread.Sleep(SleepTime / 2);
        InputHelper.SendMouseEvent(MouseEvent.LeftUp);
        Thread.Sleep(SleepTime / 2);
    }

    protected override void RightClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.RightDown);
        Thread.Sleep(SleepTime / 2);
        InputHelper.SendMouseEvent(MouseEvent.RightUp);
        Thread.Sleep(SleepTime / 2);
    }

    public InputClicker(ushort cps) : base(cps)
    {
    }
}