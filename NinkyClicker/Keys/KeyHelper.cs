using System.Runtime.InteropServices;

namespace NinkyClicker.Keys;

public static class KeyHelper
{
    [DllImport("User32.dll")]
    private static extern short GetAsyncKeyState(int vKey);

    public static bool IsHeld(VirtualKey key)
    {
        short state = GetState(key);
        return (state & 0x8000) != 0;
    }

    private static short GetState(VirtualKey k)
    {
        short state = GetAsyncKeyState((int) k);
        return state;
    }
}