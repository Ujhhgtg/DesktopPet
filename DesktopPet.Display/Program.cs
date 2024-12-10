using System;
using Avalonia;

namespace DesktopPet.Display;

internal static class Program
{
    public static string[] Arguments { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        Arguments = args;
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}