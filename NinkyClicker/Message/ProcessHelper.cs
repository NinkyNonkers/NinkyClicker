using System.Diagnostics;
using NinkyNonk.Shared.Environment;

namespace NinkyClicker.Message;

public static class ProcessHelper
{
    public static Process? SearchProcesses(string search)
    {
        Project.LoggingProxy.LogInfo($"Searching programs for '{search}'...");
        Process[] programs = Process.GetProcesses().Where(p => p.Id != Project.CurrentProcess.Id && (p.ProcessName.ToLower().Contains(search) || p.MainWindowTitle.ToLower().Contains(search))).ToArray();

        if (programs.Length < 1)
        {
            Project.LoggingProxy.LogError($"Could not find any processes under '{search}'");
            return null;
        }
        
        for (int i = 0; i < programs.Length && i < 5; i++)
            Project.LoggingProxy.Log($"{i + 1}. {programs[i].ProcessName} {programs[i].Id} {programs[i].MainWindowTitle}");
        
        int index = Convert.ToInt32(Project.LoggingProxy.AskInput("Enter program index (1-5): "));
        return programs[index - 1];
    }
}