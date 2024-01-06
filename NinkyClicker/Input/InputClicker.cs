using NinkyClicker.Input.Flags;

namespace NinkyClicker.Input;

public class InputClicker : Clicker
{
    protected override void LeftClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.LeftDown);
        Thread.Sleep(5);
        InputHelper.SendMouseEvent(MouseEvent.LeftUp);
    }

    protected override void RightClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.RightDown);
        Thread.Sleep(5);
        InputHelper.SendMouseEvent(MouseEvent.RightUp);
    }

    public InputClicker(ushort cps) : base(cps)
    {
    }
}