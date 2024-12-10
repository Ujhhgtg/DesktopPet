namespace DesktopPet.Display.Resource.Models;

// FIXME: do better class initialization
public class ConsumableResource
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ConsumableResourceType Type { get; set; }
    public short ExperienceChange { get; set; }
    // 'Strength'
    public short StaminaChange { get; set; }
    // 'StrengthFood'
    public short SaturationChange { get; set; }
    // 'StrengthDrink'
    public short ThirstChange { get; set; }
    public short HealthChange { get; set; }
    public short MoodChange { get; set; }
    // 'Likability'
    public short FavorabilityChange { get; set; }
    public ushort Price { get; set; }
    public string IconResolver { get; set; }
    public ImageResource? Icon { get; set; }
}