using System;
using System.Reflection;

namespace DesktopPet.Display.Plugin;

public static class PluginExtensionMethods
{
    public static PluginAttribute GetPluginAttribute(this Type type)
    {
        return type.GetCustomAttribute<PluginAttribute>()!;
    }

    public static PluginAttribute GetPluginAttribute(this Plugin plugin)
    {
        return plugin.GetType().GetPluginAttribute();
    }
}