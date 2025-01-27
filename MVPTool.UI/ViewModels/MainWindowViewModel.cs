using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MsBox.Avalonia;
using MVPTool.UI.Models;
using MVPTool.UI.Services;
using ReactiveUI;

namespace MVPTool.UI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool _useFastEndpoints = true;
    public bool UseFastEndpoints
    {
        get => _useFastEndpoints;
        set => this.RaiseAndSetIfChanged(ref _useFastEndpoints, value);
    }

    private bool _useIdentity;
    public bool UseIdentity
    {
        get => _useIdentity;
        set => this.RaiseAndSetIfChanged(ref _useIdentity, value);
    }

    private bool _useDocker;
    public bool UseDocker
    {
        get => _useDocker;
        set => this.RaiseAndSetIfChanged(ref _useDocker, value);
    }

    private bool _useEfCore;
    public bool UseEfCore
    {
        get => _useEfCore;
        set => this.RaiseAndSetIfChanged(ref _useEfCore, value);
    }

    private readonly ProjectGeneratorService _generator = new ProjectGeneratorService();


    private async Task GenerateProjectAsync(IProgress<ProgressReport> progress)
    {
        var options = new ProjectOptions
        {
            UseJwtAuth = _useIdentity,
            UseEfCore = _useEfCore,
            UseIdentity = _useIdentity,
            UseDocker = _useDocker,
            UseFastEndpoints = _useFastEndpoints
        };

        var generator = new ProjectGeneratorService(options); 

        try
        {
            IsProcessing = true;
            var report = new ProgressReport();
            report.Description = "Creando estructura base del proyecto...";
            report.Percentage = 25;
            progress.Report(report);
            await Task.Delay(1000);

            string fullPath = Path.Combine(OutputPath, ProjectName);
            generator.CreateBaseProject(ProjectName, OutputPath);

            report.Description = "Agregando paquetes NuGet...";
            report.Percentage = 50;
            progress.Report(report);
            await Task.Delay(1000);

            string projectPath = Path.Combine(fullPath, $"{ProjectName}.csproj");
            await AddPackagesToProjectAsync(progress);

            report.Description = "Generando endpoints...";
            report.Percentage = 75;
            progress.Report(report);
            await Task.Delay(1000);
            // TODO Modify Program.cs - Add Endpoints, etc 

            report.Description = "Proyecto completado...";
            report.Percentage = 100;
            progress.Report(report);
            await Task.Delay(1000);

        }
        catch (Exception ex)
        {
            await ShowDialogAsync($"Error: {ex.Message}");
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private async Task AddPackagesToProjectAsync(IProgress<ProgressReport> progress)
    {
        var installer = new PackageInstallerService(_generator);

        await installer.InstallPackagesAsync(OutputPath,
        _useFastEndpoints,
        _useEfCore,
        _useIdentity,
        progress);
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
        GenerateCommand = ReactiveCommand.CreateFromTask(() =>
    {
        var progress = new Progress<ProgressReport>(report =>
    {
        ProgressValue = report.Percentage;
        ProgressDescription = report.Description;
    });

        return GenerateProjectAsync(progress);
    }, canGenerate);
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

    private double _progressValue;
    public double ProgressValue
    {
        get => _progressValue;
        private set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    private string _progressDescription = string.Empty;
    public string ProgressDescription
    {
        get => _progressDescription;
        private set => this.RaiseAndSetIfChanged(ref _progressDescription, value);
    }

    private bool _isProcessing;
    public bool IsProcessing
    {
        get => _isProcessing;
        private set => this.RaiseAndSetIfChanged(ref _isProcessing, value);
    }

}