// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using NinkyClicker;
using NinkyClicker.Input;
using NinkyClicker.Message;
using NinkyNonk.Shared.Environment;

Project.LoggingProxy.LogProgramInfo();

Clicker c;
string cpsEntry;
Process? program = null;

switch (args.Length)
{
    default:
        cpsEntry = Project.LoggingProxy.AskInput("CPS: ");
        break;
    case 1:
        cpsEntry = args[0];
        break;
    case 2:
        cpsEntry = args[0];
        Project.LoggingProxy.LogInfo("Obtaining handle for " + args[1] + "...");
        program = Process.GetProcesses().First(p => p.Id != Project.CurrentProcess.Id && (p.ProcessName.ToLower().Contains(args[1]) || p.MainWindowTitle.ToLower().Contains(args[1])));
        break;
}

ushort cps = ushort.Parse(cpsEntry);
Project.LoggingProxy.LogUpdate("Specific program clicking does not support all applications; use global clicker instead where possible");
if (program == null && !Project.LoggingProxy.AskInputBool("Specific program?"))
    c = new InputClicker(cps);
else
{
    while (program == null)
        program = ProcessHelper.SearchProcesses(Project.LoggingProxy.AskInput("Search processes: "));
    c = new MessageClicker(cps, program);
}


c.Start();

while (c.Running)
    Thread.Sleep(100);

Project.LoggingProxy.Log("Clicker has stopped, press any key to exit");
Console.ReadKey();