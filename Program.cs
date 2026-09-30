using Avalonia;
using System;

namespace JaqueAndo;

internal class Program
{
    // El punto de entrada principal de la aplicación
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Configuración inicial de Avalonia UI
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}