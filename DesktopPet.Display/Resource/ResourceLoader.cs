using System.Collections.Generic;
using DesktopPet.Display.Resource.Models;
using Ujhhgtg.Library;

namespace DesktopPet.Display.Resource;

// ReSharper disable once InconsistentNaming
public interface ResourceLoader
{
    public List<PetResource> LoadPets(PathObject path);
    public PetResource LoadPet(PathObject path);
}
