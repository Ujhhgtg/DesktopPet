using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using DesktopPet.Display.Plugin;
using DesktopPet.Display.Resource;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Ujhhgtg.Library;
using Ujhhgtg.Library.ExtensionMethods;

namespace DesktopPet.Display;

public partial class MainWindow : Window
{
    public PluginManager PluginManager { get; }
    public ResourceManager ResourceManager { get; }
    public Config.Config Config { get; }
    
    public MainWindow()
    {
        #region Init
        
        #if DEBUG
        Log.Logger = LogUtils.SetupBasicLogger("./logs/display-debug.log", LogEventLevel.Debug, RollingInterval.Infinite);
        #else
        Log.Logger = LogUtils.SetupBasicLogger("./logs/display.log", LogEventLevel.Warning, RollingInterval.Day);
        #endif
        
        ProcessUtils.EnsureWorkingDirectory();
        PluginManager = new PluginManager(this);
        ResourceManager = new ResourceManager(this);

        // var sockEndpoint = new UnixDomainSocketEndPoint(KnownPaths.Temp / "desktop-pet.sock");
        // var sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
        // sock.Connect(sockEndpoint);
        
        Log.Information("[init] Starting DesktopPet display component...");
        Log.Information("[init] Operating system: {OS}", RuntimeInformation.RuntimeIdentifier);
        Log.Information("[init] Version: {Version}", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown");
        
        Log.Verbose("[init] Loading configuration and data...");
        var dataDir = KnownPaths.Data / "desktoppet";
        var configPath = dataDir / "config.json";
        var dataPath = dataDir / "data.json";
        if (!dataDir.IsDirectory || !configPath.IsFile || !dataPath.IsFile)
        {
            File.Delete(dataDir);
            Directory.CreateDirectory(dataDir);
            FileUtils.WriteText(configPath, "{}").Catch($"[init] Could not write to {configPath}");
            FileUtils.WriteText(dataPath, "{}").Catch($"[init] Could not write to {dataPath}");
        }
        Config = FileUtils.ReadJson<Config.Config>(configPath).Catch();
        
        // tiling window managers
        if (OperatingSystem.IsLinux() && (Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP") ?? "").ToLower() is "hyprland" or "i3" or "sway"
            or "bspwm" or "stumpwm" or "river" or "qtile" or "niri" or "notion" or "leftwm")
        {
            // i executed 'paru tiling' to find all tiling window managers
            Log.Warning("[init] Tiling window manager is detected; DesktopPet may not work as expected");
            Log.Warning("[init] Modify your window manager configuration to allow DesktopPet to work properly");

            if (Environment.GetEnvironmentVariable("XDG_CURRENT_DESKTOP")! == "Hyprland")
            {
                Log.Information("[init] Adding configuration for DesktopPet to your Hyprland configuration...");
                    
                var hyprConfigPath = KnownPaths.Config/ "hypr" / "hyprland.conf";
                if (hyprConfigPath.IsFile)
                {
                    var configLines = File.ReadAllLines(hyprConfigPath).ToList();
                    var isInConfigSection = false;
                    
                    foreach (var configLine in configLines.ToList())
                    {
                        if (configLine == "# BEGIN DesktopPet Configuration")
                            isInConfigSection = true;
                            
                        if (isInConfigSection)
                            configLines.Remove(configLine);
                            
                        if (configLine == "# END DesktopPet Configuration")
                            isInConfigSection = false;
                    }
                        
                    configLines.Add("# BEGIN DesktopPet Configuration");
                    configLines.Add("windowrule = float, title:^(DesktopPet Display)$");
                    configLines.Add("windowrule = noblur, title:^(DesktopPet Display)$");
                    configLines.Add("windowrule = noshadow, title:^(DesktopPet Display)$");
                    configLines.Add("windowrule = noinitialfocus, title:^(DesktopPet Display)$");
                    configLines.Add("windowrule = noborder, title:^(DesktopPet Display)$");
                    configLines.Add("# END DesktopPet Configuration");
                        
                    File.WriteAllLines(hyprConfigPath, configLines);
                }
            }
        }
        #endregion
        
        Log.Information("[ui] Initializing ui...");
        InitializeComponent();
        Position = new PixelPoint(0, 0);
        
        Log.Information("[ui] Loading plugins...");
        PluginManager.LoadPlugins();
        
        // // xorg mouse click-through
        // if (OperatingSystem.IsLinux())
        // {
        //     var display = Xorg.Xorg.GetDisplay();
        //     var window = GetTopLevel(this).Catch().TryGetPlatformHandle().Catch().Handle;
        //     Xorg.Xorg.MaskWindow(display, window);
        // }

        AnimateThread = new Thread(AnimateImage)
        {
            IsBackground = true
        };
        AnimateThread.Start();
        Running = true;
    }

    private void AnimateImage()
    {
        var files = "../../../Resources (Copy)/legacy/0000_core/pet/vup/".AsPath().AsDirectory().EnumerateFiles("*.png", SearchOption.AllDirectories).ToArray();
        var paths = files.Select(f => f.AsPath()).ToArray();
        
        var images = new Bitmap?[files.Length];
        var index = 0;
        
        while (Running)
        {
            // var watch = Stopwatch.StartNew();
            images[index] ??= new Bitmap(paths[index]);

            Dispatcher.UIThread.Invoke(() =>
            {
                // ReSharper disable once AccessToModifiedClosure
                PetImage.Source = images[index];
            });

            if (PerformImageGc)
            {
                Log.Warning("[animator] Garbage-collecting all images...");
                
                for (var i = 0; i < images.Length; i++)
                {
                    var image = images[i];
                    image?.Dispose();
                    images[i] = null;
                }
                
                Log.Warning("[animator] Garbage-collected all images");
                PerformImageGc = false;
            }
            
            if (index == paths.Length - 1)
                index = 0;
            else
                index++;
            // watch.Stop();
            
            Thread.Sleep(125/* - (short)watch.ElapsedMilliseconds*/);
        }
    }
    
    private bool Running { get; set; }
    private Thread AnimateThread { get; set; }
    private bool PerformImageGc { get; set; }

    #region Moving window
    // private  Image { get; }
    private bool IsMoving { get; set; }
    private PointerPoint OriginalPoint { get; set; }
    
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // var mousePoint = e.GetPosition(this);
        //
        // if (((Bitmap)PetImage.Source).)
        IsMoving = true;
        OriginalPoint = e.GetCurrentPoint(this);
    }
    
    private void InputElement_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsMoving) return;
    
        var currentPoint = e.GetCurrentPoint(this);
        Position = new PixelPoint(Position.X + (int)(currentPoint.Position.X - OriginalPoint.Position.X),
            Position.Y + (int)(currentPoint.Position.Y - OriginalPoint.Position.Y));
    }
    
    private void InputElement_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        IsMoving = false;
    }
    #endregion

    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        Running = false;
    }
}