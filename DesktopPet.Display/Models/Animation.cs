using Avalonia.Media;

namespace DesktopPet.Display.Models;

public class Animation
{
    public string[] ImageResolvers { get; set; }
    public IImage[] Images { get; set; }
}