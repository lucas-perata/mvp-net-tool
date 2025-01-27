using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MVPTool.UI.Services;
using ReactiveUI;

namespace MVPTool.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly ProjectGenerator _generator = new ProjectGenerator();

    private async Task GenerateProjectAsync()
    {
        try
        {
            string fullPath = Path.Combine(OutputPath, ProjectName);
            _generator.CreateBaseProject(ProjectName, OutputPath);

            string projectPath = Path.Combine(fullPath, $"{ProjectName}.csproj");
            // TODO Packages
            // TODO Modify Program.cs - Add Endpoints, etc 

            await ShowDialogAsync("Proyecto generado exitosamente");
        }
        catch (Exception ex)
        {
            await ShowDialogAsync($"Error: {ex.Message}");
        }
    }

    private async Task ShowDialogAsync(string message)
    {
        var dialog = MessageBoxManager.GetMessageBoxStandard("Caption", message);
        await dialog.ShowAsync();
    }
    private string _projectName = "";
    public string ProjectName
    {
        get => _projectName;
        set => this.RaiseAndSetIfChanged(ref _projectName, value);
    }

    private string _outputPath = "";
    public string OutputPath
    {
        get => _outputPath;
        set => this.RaiseAndSetIfChanged(ref _outputPath, value);
    }

    // Propiedades para los checkboxes
    private bool _useFastEndpoints = true;
    public bool UseFastEndpoints
    {
        get => _useFastEndpoints;
        set => this.RaiseAndSetIfChanged(ref _useFastEndpoints, value);
    }

    private bool _useJwtAuth;
    public bool UseJwtAuth
    {
        get => _useJwtAuth;
        set => this.RaiseAndSetIfChanged(ref _useJwtAuth, value);
    }

    private bool _useDocker;
    public bool UseDocker
    {
        get => _useDocker;
        set => this.RaiseAndSetIfChanged(ref _useDocker, value);
    }

    private bool _useEfCore = true;
    public bool UseEfCore
    {
        get => _useEfCore;
        set => this.RaiseAndSetIfChanged(ref _useEfCore, value);
    }

    // Comandos
    public ReactiveCommand<Unit, Unit> SelectOutputPathCommand { get; }
    public ReactiveCommand<Unit, Unit> GenerateCommand { get; }

    public MainWindowViewModel()
    {
        SelectOutputPathCommand = ReactiveCommand.CreateFromTask(SelectOutputPath);

        var canGenerate = this.WhenAnyValue(
            x => x.ProjectName,
            x => x.OutputPath,
            (name, path) => !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(path)
        );
        GenerateCommand = ReactiveCommand.CreateFromTask(GenerateProjectAsync, canGenerate);
    }

    private async Task SelectOutputPath()
    {
        var dialog = new OpenFolderDialog { Title = "Select Output Directory" };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            var result = await dialog.ShowAsync(mainWindow);
            if (!string.IsNullOrEmpty(result))
                OutputPath = result;
        }
    }


}