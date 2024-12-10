using System.IO;
using System.Reflection;

namespace DesktopPet.Display.Plugin;

public abstract class Plugin
{
    public required Assembly Assembly { get; set; }

    public readonly int ApiVersion = 1;
    public bool Enabled { get; set; } = false;

    public Stream? GetResource(string name)
    {
        return Assembly.GetManifestResourceStream(name);
    }

    #region Plugin events called by server

    /// <summary>
    /// Loads plugin.
    /// Warning: This API is unstable and is subject to change at any time.
    /// </summary>
    /// <param name="window">Display's main window.</param>
    public abstract void OnLoad(MainWindow window);
    
    /// <summary>
    /// Enables plugin.
    /// Warning: This API is unstable and is subject to change at any time.
    /// </summary>
    /// <param name="window">Display's main window.</param>
    public abstract void OnEnable(MainWindow window);
    
    /// <summary>
    /// Disables plugin.
    /// Warning: This API is unstable and is subject to change at any time.
    /// </summary>
    /// <param name="window">Display's main window.</param>
    public abstract void OnDisable(MainWindow window);

    #endregion
}
