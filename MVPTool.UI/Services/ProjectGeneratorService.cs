using System;
using System.Diagnostics;
using System.IO;
using DynamicData;

namespace MVPTool.UI.Services;

public class ProjectGeneratorService
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

    public void AddPackage(string projectPath, string packageName)
    {
        var process = new Process 
        {
            StartInfo = new ProcessStartInfo 
            {
                FileName ="dotnet",
                Arguments=$"add {projectPath} package {packageName}",
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
    }

    public void CleanProject(string projectPath) 
    {
        // TODO
        var files = System.IO.Directory.GetFiles(projectPath, "Program.cs");
    }
}
