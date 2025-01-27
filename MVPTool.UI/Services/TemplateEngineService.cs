using System;
using System.Collections.Generic;
using System.IO;

namespace MVPTool.UI.Services;

public class TemplateEngineService
{

    public string ProcessTemplate(string templateContent, Dictionary<string, string> replacements)
    {
        foreach(var key in replacements.Keys)
        {
            templateContent = templateContent.Replace($"{{{{{key}}}}}", replacements[key]);
        }

        return templateContent;
    }

    public string GetTemplate(string templateName)
    {
        var templatePath = Path.Combine("Templates", templateName);

        return File.ReadAllText(templatePath);
    }

}
