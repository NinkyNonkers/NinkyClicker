using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NinkyClicker.Keys;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public abstract class Clicker : IDisposable
{
    private uint _shouldClick = 1;
    private bool _isLeftClick;
    private bool _isRightClick;
    private uint _clicks;
    
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

    protected int SleepTime { get; }
    public bool Running { get; private set; }
    public ClickerConfiguration Configuration { get; }
    
    protected Clicker(ushort cps)
    {
        SleepTime = (int) Math.Round((float)(1000 / cps)) - (int) Math.Round(cps * 1.2);
        Running = true;
        Configuration = new ClickerConfiguration();
    }
    
    protected Clicker(ushort cps, ClickerConfiguration configuration)
    {
        SleepTime = (int) Math.Round((float)(1000 / cps)) - (int) Math.Round(cps * 1.2);
        Running = true;
        Configuration = configuration;
    }

    public void Start()
    {
        Project.LoggingProxy.LogInfo($"Starting {GetType().Name}...");
        
        Thread t = new Thread(() =>
        {
            Project.LoggingProxy.LogInfo("Adding hooks...");
            Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            _hookId = SetWindowsHookEx(14, HookCallback, GetModuleHandle(curModule.ModuleName), 0);
            while (Running && GetMessage(out HookMessage msg, IntPtr.Zero, 0, 0))
            {
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
        });
        
        t.SetApartmentState(ApartmentState.STA);
        t.IsBackground = true;
        t.Start();
        
        Project.LoggingProxy.LogInfo("Queuing worker threads...");
        ThreadPool.QueueUserWorkItem(_ => ClickThread());
        ThreadPool.QueueUserWorkItem(_ => LogThread());
        ThreadPool.QueueUserWorkItem(_ => ModifyThread());
        
        Project.LoggingProxy.LogSuccess($"Started a new {GetType().Name} successfully");
    }
    
    private void ModifyThread()
    {
        while (Running)
        {
            bool shouldClick = !KeyHelper.IsHeld(VirtualKey.LControl);
            if (_shouldClick == 1 && !shouldClick)
                Interlocked.Decrement(ref _shouldClick);
            else if (_shouldClick == 0 && shouldClick)
                Interlocked.Increment(ref _shouldClick);
        }
    }
    
    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        try
        {
            if (nCode < 0)
                return CallNextHookEx(_hookId, nCode, wParam, lParam);

            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            if ((hookStruct.flags & 0x00000001) != 0) //if Clicker-generated
                return CallNextHookEx(_hookId, nCode, wParam, lParam);
        
            switch ((KeyMouseEvent) wParam)
            {
                case KeyMouseEvent.WmLbuttonup:
                    _isLeftClick = false;
                    break;
                case KeyMouseEvent.WmLbuttondown:
                    _isLeftClick = true;
                    break;
                case KeyMouseEvent.WmRbuttonup:
                    _isRightClick = false;
                    break;
                case KeyMouseEvent.WmRbuttondown:
                    _isRightClick = true;
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

    private void LogThread()
    {
        while (Running)
        {
            Thread.Sleep(300000);
            if (!Running)
                continue;
            Project.LoggingProxy.LogUpdate($"{_clicks} clicks in the past 5 minutes.");
            Interlocked.Exchange(ref _clicks, 0);
        }
    }

    private void ClickThread()
    {
        while (Running)
        {
            if (_shouldClick == 0 || (!_isLeftClick && !_isRightClick))
                continue;
            
            Interlocked.Increment(ref _clicks);

            try
            {
                if (_isLeftClick)
                    LeftClick();
            
                if (_isRightClick)
                    RightClick();
            }
            catch (Exception e)
            {
                Project.LoggingProxy.LogError(e.ToString());
            }
        }
    }

    protected abstract void LeftClick();
    protected abstract void RightClick();
    
    public void Dispose()
    {
        Project.LoggingProxy.LogInfo("Stopping clicker...");
        Running = false;
        GC.SuppressFinalize(this);
    }
}
