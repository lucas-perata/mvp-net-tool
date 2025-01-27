using System;
using System.Diagnostics;

namespace MVPTool.UI.Services;

public class ProjectGenerator
{
    public void CreateBaseProject(string projectName, string outputPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo 
            {
                FileName="dotnet",
                Arguments = $"new webapi -n {projectName} -o {outputPath}", 
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
    }
}
