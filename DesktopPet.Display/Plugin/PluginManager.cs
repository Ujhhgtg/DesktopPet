using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Serilog;
using Ujhhgtg.Library;

namespace DesktopPet.Display.Plugin;

public partial class PluginManager(MainWindow window) : ChildOf<MainWindow>(window)
{
    private readonly PathObject _pluginsDirectory = "./plugins";
    public Dictionary<string, Plugin> Plugins { get; } = [];
    public bool PluginsLoaded { get; private set; } = false;
    public bool PluginsAllEnabled { get; private set; } = false;

    public const int ApiVersion = 1;

    public void LoadPlugins()
    {
        if (PluginsLoaded)
        {
            Log.Error("[plugin-manager] Plugins are already loaded and cannot be loaded twice");
            return;
        }

        if (!_pluginsDirectory.IsDirectory)
        {
            _pluginsDirectory.Delete();
            _pluginsDirectory.CreateDirectory();
        }

        PluginsLoaded = true;

        var pluginFiles = _pluginsDirectory.AsDirectory().EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);

        // TODO: dependency check
        foreach (var pluginFile in pluginFiles)
        {
            var pluginAssembly = Assembly.LoadFile(pluginFile.FullName);
            foreach (var type in pluginAssembly.GetTypes())
                if (typeof(Plugin).IsAssignableFrom(type))
                {
                    var plugin = (Plugin)Activator.CreateInstance(type)!;
                    var pluginAttr = plugin.GetPluginAttribute();

                    // validation
                    if (!IsPluginNameValid(pluginAttr.Name)) continue; // name
                    if (plugin.ApiVersion != ApiVersion) // api
                        Log.Warning(
                            "[plugin-manager] Plugin {Name} API version {Version1} does not match DesktopPet API version {Version2}, plugin may not work correctly",
                            plugin.ApiVersion,
                            pluginAttr.Name,
                            ApiVersion);

                    plugin.Assembly = pluginAssembly;
                    Plugins.Add(pluginAttr.Name, plugin);
                    try
                    {
                        Log.Information("[plugin-manager] Loading plugin {Name}...", pluginAttr.Name);
                        plugin.OnLoad(Parent);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "[plugin-manager] Exception while loading plugin {Name}", pluginAttr.Name);
                        Plugins.Remove(pluginAttr.Name);
                    }
                }
        }
    }

    public bool EnablePlugin(Plugin plugin)
    {
        var pluginName = plugin.GetPluginAttribute().Name;
        try
        {
            Log.Warning("[plugin-manager] Enabling plugin {Name}...", pluginName);
            plugin.OnEnable(Parent);
            plugin.Enabled = true;
            return true;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[plugin-manager] Exception while enabling plugin {Name}!", pluginName);
            return false;
        }
    }

    public bool DisablePlugin(Plugin plugin)
    {
        var pluginName = plugin.GetPluginAttribute().Name;
        try
        {
            Log.Warning("[plugin-manager] Disabling plugin {Name}...", pluginName);
            plugin.OnDisable(Parent);
            plugin.Enabled = false;
            return true;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "[plugin-manager] Exception while disabling plugin {Name}", pluginName);
            return false;
        }
    }

    public void EnableAllPlugins()
    {
        PluginsAllEnabled = true;
        foreach (var plugin in Plugins)
            EnablePlugin(plugin.Value);
    }

    public void DisableAllPlugins()
    {
        PluginsAllEnabled = false;
        foreach (var plugin in Plugins)
            DisablePlugin(plugin.Value);
    }

    public void ReloadAllPlugins()
    {
        DisableAllPlugins();
        EnableAllPlugins();
    }

    private bool IsPluginNameValid(string name)
    {
        var regex = ValidPluginNameRegex();
        return regex.IsMatch(name);
    }

    [GeneratedRegex("^[a-zA-Z0-9_]+$")]
    private partial Regex ValidPluginNameRegex();
}