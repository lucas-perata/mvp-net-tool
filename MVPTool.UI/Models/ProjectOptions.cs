using System;

namespace MVPTool.UI.Models;

public class ProjectOptions
{
    public bool UseJwtAuth { get; set; } = true; 
    public bool UseEfCore { get; set; } = false;
    public bool UseIdentity { get; set; } = false;
    public bool UseDocker { get; set; } = false;
    public bool UseFastEndpoints { get; set; } = false;

    public static ProjectOptions Default => new ProjectOptions();
}