using NinkyClicker.Keys;

namespace NinkyClicker;

public class ClickerConfiguration
{
    public VirtualKey LeftClickToggleHold { get; } = VirtualKey.LButton;
    public VirtualKey RightClickToggleHold { get; } = VirtualKey.RButton;
    public VirtualKey DisableHold { get; } = VirtualKey.LControl;
    public string InputTargetWindow { get; } = string.Empty;
}