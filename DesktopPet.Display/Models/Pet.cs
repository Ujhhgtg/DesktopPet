using DesktopPet.Display.Resource.Models;
using Ujhhgtg.Library;

namespace DesktopPet.Display.Models;

public class Pet(PetResource parent) : ChildOf<PetResource>(parent)
{
    public string Name { get; set; }
}