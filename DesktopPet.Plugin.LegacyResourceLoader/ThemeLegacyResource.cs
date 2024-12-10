using System.Drawing;
using DesktopPet.Display.Resource.Models;

namespace DesktopPet.Plugin.LegacyResourceLoader;

public class ThemeLegacyResource
{
    public string Name { get; set; }
    public string IconResolver { get; set; }
    public ImageResource Icon { get; set; }
    public Color PrimaryColor { get; set; }
    public Color PrimaryLightColor { get; set; }
    public Color PrimaryThinColor { get; set; }
    public Color PrimaryDarkColor { get; set; }
    public Color PrimaryBlackColor { get; set; }
    public Color PrimaryTextColor { get; set; }
    public Color SecondaryColor { get; set; }
    public Color SecondaryLightColor { get; set; }
    public Color SecondaryThinColor { get; set; }
    public Color SecondaryDarkColor { get; set; }
    public Color SecondaryBlackColor { get; set; }
    public Color SecondaryTextColor { get; set; }
    public Color DarkPrimaryColor { get; set; }
    public Color DarkPrimaryLightColor { get; set; }
    public Color DarkPrimaryThinColor { get; set; }
    public Color DarkPrimaryDarkColor { get; set; }
    public Color DarkPrimaryBlackColor { get; set; }
    public Color DarkPrimaryTextColor { get; set; }
    public Color ShadowColor { get; set; }
}