using System;

namespace DesktopPet.Display.Resource.Models;

public class ActivityResource : ICloneable
{
    public string Type { get; set; }
    public string Name { get; set; }
    public ImageResource Image { get; set; }
    public string IconResolver { get; set; }
    // earn currency
    public double CurrencyFactor  { get; set; }
    // lose saturation
    public double SaturationFactor { get; set; }
    // lose thirst
    public double ThirstFactor { get; set; }
    // lose mood
    public double MoodFactor { get; set; }
    public double AwardFactor { get; set; }
    public int RequiredMinimumLevel { get; set; }
    public TimeSpan Duration { get; set; }
    
    public object Clone()
    {
        return MemberwiseClone();
    }
}