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
        Project.LoggingProxy.LogInfo("Obtaining handle for " + args[1]);
        program = Process.GetProcesses().First(p => p.ProcessName.ToLower().Contains(args[1]) || p.MainWindowTitle.ToLower().Contains(args[1]));
        break;
}

ushort cps = ushort.Parse(cpsEntry);

if (program == null && !Project.LoggingProxy.AskInputBool("Specific program?"))
    c = new InputClicker(cps);
else
{
    if (program == null)
    {
        string search = Project.LoggingProxy.AskInput("Search programs: ");
        Process[] programs = Process.GetProcesses().Where(p => p.ProcessName.ToLower().Contains(search) || p.MainWindowTitle.ToLower().Contains(search)).ToArray();
        
        for (int i = 0; i < programs.Length && i < 5; i++)
            Project.LoggingProxy.Log($"{i + 1}. {programs[i].ProcessName} {programs[i].Id} {programs[i].MainWindowTitle}");
        
        int index = Convert.ToInt32(Project.LoggingProxy.AskInput("Enter program index (1-5): "));
        program = programs[index - 1];
    }

    c = new MessageClicker(cps, program);
}

c.StartLoop();
Project.LoggingProxy.Log("Clicker has stopped, press any key to exit");
Console.ReadKey();