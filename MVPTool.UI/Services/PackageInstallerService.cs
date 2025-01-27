using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVPTool.UI.Models;
namespace MVPTool.UI.Services;

public class PackageInstallerService
{
    private readonly ProjectGeneratorService _generator;

    public PackageInstallerService(ProjectGeneratorService generator)
    {
       _generator = generator; 
    }

    public async Task InstallPackagesAsync(string outputhPath, bool UseFastEndpoints, bool useEfCore, bool useIdentity, IProgress<ProgressReport> progress)
    {
        var actions = new List<ActionContext>
        {
            new(UseFastEndpoints, "FastEndpoints", "Instalando FastEndpoints..."),
            new(UseFastEndpoints, "FastEndpoints.Swagger", "Instalando FastEndpoints Swagger..."),
            new(useEfCore, "Microsoft.EntityFrameworkCore", "Instalando Entity Framework Core..."),
            new(useEfCore, "Microsoft.EntityFrameworkCore.Design", "Instalando EntityFrameworkCore Design..."),
            new(useIdentity, "Microsoft.AspNetCore.Identity.EntityFrameworkCore", "Instalando Identity Framework..."),
            new(useIdentity, "Microsoft.AspNetCore.Authentication.JwtBearer", "Instalando JWT Bearer...")
        };

        foreach(var action in actions.Where(a => a.Shouldexecute))
        {
            progress.Report(new ProgressReport
            {
                Description = action.Description,
                Percentage = 75 
            });

            _generator.AddPackage(outputhPath, action.PackageName);
            await Task.Delay(100);
        }
    }

    private record ActionContext(bool Shouldexecute, string PackageName, string Description);
}
