using NinkyClicker.Keys;
using NinkyClicker.Virtual;
using NinkyNonk.Shared.Environment;
using NinkyNonk.Shared.Framework.Exception;

namespace NinkyClicker;

public abstract class Clicker : IDisposable
{
    protected uint ShouldClick = 1;
    protected bool IsLeftClick;
    protected bool IsRightClick;
    
    public int SleepTime { get; }
    public ClickExecutor Executor { get; set; }
    public bool Running { get; private set; }
    public ClickerConfiguration Configuration { get; }
    
    private uint _clicks;
    
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
        if (Executor == null)
            throw new FatalException("Executor is null");
        
        Project.LoggingProxy.LogInfo($"Initialising {GetType().Name} using {Executor.GetType().Name}...");
        Initialise();
        
        Project.LoggingProxy.LogInfo("Queuing worker threads...");
        ThreadPool.QueueUserWorkItem(_ => ClickThread());
        ThreadPool.QueueUserWorkItem(_ => LogThread());
        ThreadPool.QueueUserWorkItem(_ => ModifyThread());
        
        Project.LoggingProxy.LogSuccess($"Started a new {GetType().Name} successfully");
    }
    
    protected virtual void ModifyThread()
    {
        while (Running)
        {
            bool shouldClick = !VirtualKeyHelper.IsHeld(VirtualKey.LControl);
            if (ShouldClick == 1 && !shouldClick)
                Interlocked.Decrement(ref ShouldClick);
            else if (ShouldClick == 0 && shouldClick)
                Interlocked.Increment(ref ShouldClick);
        }
    }

    protected virtual void Initialise()
    {
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
            if (ShouldClick == 0 || (!IsLeftClick && !IsRightClick))
                continue;
            
            Interlocked.Increment(ref _clicks);

            try
            {
                if (IsLeftClick)
                    Executor.LeftClick();
            
                if (IsRightClick)
                    Executor.RightClick();
            }
            catch (Exception e)
            {
                Project.LoggingProxy.LogError(e.ToString());
            }
        }
    }
    
    public void Dispose()
    {
        Project.LoggingProxy.LogInfo("Stopping clicker...");
        Running = false;
        GC.SuppressFinalize(this);
    }
}
