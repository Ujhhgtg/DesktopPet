using System;

namespace DesktopPet.Display.Plugin;

[AttributeUsage(AttributeTargets.Class)]
public class PluginAttribute : Attribute
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public string[] Dependencies { get; init; } = [];
    public PluginFeatures Features { get; init; } = PluginFeatures.None;
}

[Flags]
public enum PluginFeatures
{
    None = 0
}
