using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using DynamicData;
using MVPTool.UI.Models;

namespace MVPTool.UI.Services;

public class ProjectGeneratorService
{
    private readonly TemplateEngineService _templateEngine = new TemplateEngineService();
    private readonly ProjectOptions _options;

    public ProjectGeneratorService(ProjectOptions options = null)
    {
        _options = options ?? ProjectOptions.Default;
    }
    public void CreateBaseProject(string projectName, string outputPath)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"new webapi -n {projectName} -o {outputPath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        ReplaceProgramCs(projectName, outputPath);
    }

    public void AddPackage(string projectPath, string packageName)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"add {projectPath} package {packageName}",
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
    }
    private void ReplaceProgramCs(string projectName, string outputPath)
    {
        var template = _templateEngine.GetTemplate("RootFiles/Program.cs.txt");
        var replacements = new Dictionary<string, string>
        {
            { "ProjectName", projectName },
            { "AddServices", GetServicesConfiguration() },
            {"AddNamespaces", GetNamespacesConfiguration()},
            {"AddAppUsing", GetAppUsingConfiguration()}
        };

        var finalContent = _templateEngine.ProcessTemplate(template, replacements);
        File.WriteAllText(Path.Combine(outputPath, "Program.cs"), finalContent);
    }

    private string GetServicesConfiguration()
    {
        var services = new StringBuilder();

        if (_options.UseJwtAuth)
        {
            services.AppendLine("builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)");
            // TODO JWT CONFIG
            services.AppendLine("    .AddJwtBearer(options => { /* Configuración JWT */ });");
            services.AppendLine("builder.Services.AddAuthorization()");
        }

        if (_options.UseEfCore)
        {
            services.AppendLine("builder.Services.AddDbContext<AppDbContext>();");
        }

        if (_options.UseIdentity)
        {
            services.AppendLine("builder.Services.AddIdentity<IdentityUser, IdentityRole>()");
            services.AppendLine("    .AddEntityFrameworkStores<AppDbContext>();");
        }

        return services.ToString();
    }

    private string GetNamespacesConfiguration()
    {
        var namespaces = new StringBuilder();

        if (_options.UseJwtAuth)
        {
            namespaces.AppendLine("using Microsoft.AspNetCore.Authentication.JwtBearer;");
        }

        if (_options.UseEfCore)
        {
            namespaces.AppendLine("using Microsoft.EntityFrameworkCore;");
        }

        if (_options.UseIdentity)
        {
            namespaces.AppendLine("using Microsoft.AspNetCore.Identity");


            return namespaces.ToString();
        }

        return namespaces.ToString();
    }

    private string GetAppUsingConfiguration()
    {
        var appUsing = new StringBuilder();

        if(_options.UseIdentity)
        {
            appUsing.AppendLine("app.UseAuthentication();");
            appUsing.AppendLine("app.UseAuthorization();");
        }

        return appUsing.ToString();
    }
}
