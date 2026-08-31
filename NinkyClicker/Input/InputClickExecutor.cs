using NinkyClicker.Input.Flags;

namespace NinkyClicker.Input;

public class InputClickExecutor : ClickExecutor
{
    private readonly Clicker _parent;

    public override void LeftClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.LeftDown);
        Thread.Sleep(_parent.SleepTime / 2);
        InputHelper.SendMouseEvent(MouseEvent.LeftUp);
        Thread.Sleep(_parent.SleepTime / 2);
    }

    public override void RightClick()
    {
        InputHelper.SendMouseEvent(MouseEvent.RightDown);
        Thread.Sleep(_parent.SleepTime / 2);
        InputHelper.SendMouseEvent(MouseEvent.RightUp);
        Thread.Sleep(_parent.SleepTime / 2);
    }

    public InputClickExecutor(Clicker parent)
    {
        _parent = parent;
    }
}