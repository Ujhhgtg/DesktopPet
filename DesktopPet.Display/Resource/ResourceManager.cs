using System;
using System.Collections.Generic;
using System.Linq;
using DesktopPet.Display.Resource.Models;
using Serilog;
using Ujhhgtg.Library;

namespace DesktopPet.Display.Resource;

public class ResourceManager(MainWindow window) : ChildOf<MainWindow>(window)
{
    public List<PetResource> Pets { get; set; } = [];
    public Dictionary<Type, object> ResourceLoaders { get; set; } = new();

    public bool RegisterResourceLoader(Type resourceLoader)
    {
        if (!resourceLoader.IsAssignableTo(typeof(ResourceLoader)))
        {
            Log.Error("[resource-manager] Could not register resource loader {Type} because it is invalid", resourceLoader.FullName);
            return false;
        }

        if (ResourceLoaders.ContainsKey(resourceLoader))
        {
            Log.Error("[resource-manager] Could not register resource loader {Type} because it is already registered", resourceLoader.FullName);
        }
        
        ResourceLoaders.Add(resourceLoader, Activator.CreateInstance(resourceLoader)!);
        Log.Verbose("[resource-manager] Registered resource loader {Type}", resourceLoader.FullName);
        
        return true;
    }
    
    public bool UnregisterResourceLoader(Type resourceLoader)
    {
        ResourceLoaders.Remove(resourceLoader);
        return true;
    }

    public bool LoadAllResources(PathObject path)
    {
        if (!(path/"pets").IsDirectory)
            return false;
        
        foreach (var loader in ResourceLoaders.Values.Select(obj => (ResourceLoader)obj))
        {
            Pets.AddRange(loader.LoadPets(path / "pets"));
        }

        return true;
    }
}