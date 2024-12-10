using System.Drawing;

namespace DesktopPet.Display.Resource.Models;

public class InteractionResource
{
    public string RequiredState { get; set; } = string.Empty;
    // -1 represents that this limit does not exists
    public short RequiredMinimumFavorability { get; set; } = -1;
    // -1 represents that this limit does not exists
    public short RequiredMaximumFavorability { get; set; } = -1;
    // public string[] Tags { get; set; }
    public short ExperienceChange { get; set; } = 0;
    public short StaminaChange { get; set; } = 0;
    public short HealthChange { get; set; } = 0;
    public short MoodChange { get; set; } = 0;
    public short FavorabilityChange { get; set; } = 0;

    public short CurrencyChange { get; set; } = 0;
    // public Action<MainWindow> OnInteraction { get; set; }
    // public AnimationResource Animation { get; set; } // FIXME
    public Point Position { get; set; }
    public Size Size { get; set; }
}