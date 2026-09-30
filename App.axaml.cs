using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using JaqueAndo.Services;
using JaqueAndo.ViewModels;
using System;
using System.Threading.Tasks;

namespace JaqueAndo;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();

        // Carga/Siembra de la base de datos en segundo plano para evitar congelar la UI
        Task.Run(() =>
        {
            try
            {
                ExcelImporterService.SeedJugadoresDesdeExcel();
                Console.WriteLine(">>> Base de datos SQLite sincronizada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Error al sembrar datos: {ex.Message}");
            }
        });
    }
}