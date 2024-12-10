namespace DesktopPet.Display.Resource.Models;

// FIXME: do better class initialization
public class PetResource
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Version { get; set; }
    public string[] Authors { get; set; }
    // FIXME: unused
    public string[] Tags { get; set; }
    public string RelativePath { get; set; }
    public ConsumableResource[] Consumables { get; set; }
    public InteractionResource[] Interactions { get; set; }
}