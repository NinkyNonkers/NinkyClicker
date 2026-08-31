using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker.Hook;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public class HookClicker : Clicker
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, MouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
    
    [DllImport("user32.dll")]
    private static extern bool GetMessage(out HookMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll")]
    private static extern bool TranslateMessage(ref HookMessage lpMsg);

    [DllImport("user32.dll")]
    private static extern IntPtr DispatchMessage(ref HookMessage lpMsg);
    
    private delegate IntPtr MouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    private static IntPtr _hookId;
    
    public HookClicker(ushort cps) : base(cps)
    {
    }

    public HookClicker(ushort cps, ClickerConfiguration configuration) : base(cps, configuration)
    {
    }

    private static bool TryGetMessage(out HookMessage? msg)
    {
        try
        {
            return GetMessage(out msg, IntPtr.Zero, 0, 0);
        }
        catch (Exception e)
        {
            Project.LoggingProxy.LogError(e.ToString());
        }
        msg = null;
        return false;
    }
    
    protected override void Initialise()
    {
        Thread t = new Thread(HookThread);
        t.SetApartmentState(ApartmentState.STA);
        t.IsBackground = true;
        t.Start();
    }

    private void HookThread()
    {
        Project.LoggingProxy.LogInfo("Adding hooks...");
        using ProcessModule curModule = Process.GetCurrentProcess().MainModule!;
        _hookId = SetWindowsHookEx(14, HookCallback, GetModuleHandle(curModule.ModuleName), 0);
        
        while (Running)
        {
            if (!TryGetMessage(out HookMessage? msg) || msg == null)
                continue;
            try
            {
                TranslateMessage(ref msg);
                DispatchMessage(ref msg);
            }
            catch (Exception e)
            {
                Project.LoggingProxy.LogError(e.ToString());
            }
        }
        
        Project.LoggingProxy.LogInfo("Removing hooks...");
        UnhookWindowsHookEx(_hookId);
        _hookId = IntPtr.Zero;
        
        if (Running)
            Dispose();
    }
    
    
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            if (nCode < 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            if ((hookStruct.flags & 0x00000001) != 0) //if Clicker-generated
                return CallNextHookEx(_hookId, nCode, wParam, lParam); //TODO: m
        
            switch ((HookMouseEvent) wParam)
            {
                case HookMouseEvent.WmLbuttonup:
                    IsLeftClick = false;
                    break;
                case HookMouseEvent.WmLbuttondown:
                    IsLeftClick = true;
                    break;
                case HookMouseEvent.WmRbuttonup:
                    IsRightClick = false;
                    break;
                case HookMouseEvent.WmRbuttondown:
                    IsRightClick = true;
                    break;
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
        catch (Exception e)
        {
            Project.LoggingProxy.LogError(e.ToString());
        }
        
        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
}