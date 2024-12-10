using System.Drawing;
using Newtonsoft.Json.Linq;
using Serilog;
using Ujhhgtg.Library;
using Ujhhgtg.Library.ExtensionMethods;
using DesktopPet.Display.Resource;
using DesktopPet.Display.Resource.Models;

namespace DesktopPet.Plugin.LegacyResourceLoader;

public class LegacyResourceLoader : ResourceLoader
{
    public List<PetResource> LoadPets(PathObject path)
    {
        // ALL RESOURCES: file, food (done), image, lang, pet, photo, text (partially done), theme, info.lps

        var consumables = new List<ConsumableResource>();
        var interactions = new List<InteractionResource>();

        #region Standard files
        
        var jsonArray = new JArray();
        
        foreach (var jsonPath in (path / "food").AsDirectory().EnumerateFiles(".json", SearchOption.AllDirectories).AsPaths())
        {
            var array = JArray.Parse(FileUtils.ReadText(jsonPath)!);
            jsonArray.Merge(array);
            // Log.Debug("[legacy-resource-loader] Current base array: {JArray}", jsonArray);
        }
        
        foreach (var jsonPath in (path / "text").AsDirectory().EnumerateFiles(".json", SearchOption.AllDirectories).AsPaths())
        {
            var array = JArray.Parse(FileUtils.ReadText(jsonPath)!);
            jsonArray.Merge(array);
            // Log.Debug("[legacy-resource-loader] Current base array: {JArray}", jsonArray);
        }
        
        // foreach (var jsonPath in (path / "photo").AsDirectory().EnumerateFiles(".json", SearchOption.AllDirectories).AsPaths())
        // {
        //     var array = JArray.Parse(FileUtils.ReadText(jsonPath)!);
        //     jsonArray.Merge(array);
        //     // Log.Debug("[legacy-resource-loader] Current base array: {JArray}", jsonArray);
        // }

        // FIXME: better nullability handling
#pragma warning disable CS8604
        foreach (var jsonToken in jsonArray)
        {
            var jsonObject = jsonToken.ToObject<JObject>()!;
            var objectBaseName = jsonObject.TryGetValue<string>("object_base_name");
            
            switch (objectBaseName)
            {
                case "food":
                    // since null value for short is 0, we do not have to deal with nonexistent (null) values
                    var consumable = new ConsumableResource
                    {
                        Name = jsonObject["name"].To<string>().Catch(),
                        Description = jsonObject["desc"].To<string>().Catch(),
                        Type = (jsonObject["type"].To<string>().Catch()) switch
                        {
                            "Drug" => ConsumableResourceType.Medicine,
                            "Drink" => ConsumableResourceType.Drink,
                            "Gift" => ConsumableResourceType.Present,
                            "Snack" => ConsumableResourceType.Snack,
                            "Meal" => ConsumableResourceType.Meal,
                            "Functional" => ConsumableResourceType.Functional,
                            _ => throw new ArgumentOutOfRangeException()
                        },
                        ExperienceChange = jsonObject["Exp"].To<short>(),
                        StaminaChange = jsonObject["Strength"].To<short>(),
                        SaturationChange = jsonObject["StrengthFood"].To<short>(),
                        ThirstChange = jsonObject["StrengthDrink"].To<short>(),
                        HealthChange = jsonObject["Health"].To<short>(),
                        MoodChange = jsonObject["Feeling"].To<short>(),
                        FavorabilityChange = jsonObject["Likability"].To<short>(),
                        Price = jsonObject["price"].To<ushort>(),
                        IconResolver = jsonObject["graph"].To<string>().Catch()
                    };
                    // FIXME: consumable.Icon =

                    consumables.Add(consumable);
                    break;
                
                case "clicktext":
                    var interaction = new DisplayTextInteractionResource
                    {
                        RequiredState = jsonObject["State"].To<string>() ?? string.Empty,
                        ExperienceChange = jsonObject["Exp"].To<short>(),
                        StaminaChange = jsonObject["Strength"].To<short>(),
                        HealthChange = jsonObject["Health"].To<short>(),
                        MoodChange = jsonObject["Feeling"].To<short>(),
                        FavorabilityChange = jsonObject["Likability"].To<short>(),
                        CurrencyChange = jsonObject["Money"].To<short>(),
                        Text = jsonObject["Text"].To<string>().Catch(),
                        Position = new Point(-1, -1),
                        Size = new Size(-1, -1)
                    };
                    var tagString = jsonObject["tag"].To<string>();
                    // interaction.Tags = tagString is not null ? tagString.Split(',') : [];

                    var rawMinFav = jsonObject["LikeMin"];
                    if (rawMinFav is not null)
                        interaction.RequiredMinimumFavorability = rawMinFav.To<short>();
                    else
                        interaction.RequiredMinimumFavorability = -1;
                    
                    var rawMaxFav = jsonObject["LikeMax"];
                    if (rawMaxFav is not null)
                        interaction.RequiredMaximumFavorability = rawMaxFav.To<short>();
                    else
                        interaction.RequiredMaximumFavorability = -1;
                    
                    interactions.Add(interaction);
                    break;
                
                default:
                    Log.Warning("[legacy-resource-loader] Found unhandled named object {Object}", jsonObject);
                    break;
            }
        }
        
        Log.Verbose("[legacy-resource-loader] Loaded {Consumables} consumables and {Interactions}", consumables.Count, interactions.Count);
        #endregion

        #region Theme files
        var themes = new List<ThemeLegacyResource>();
        foreach (var jsonPath in (path / "theme").AsDirectory().EnumerateFiles(".json", SearchOption.AllDirectories).AsPaths())
        {
            var array = JArray.Parse(FileUtils.ReadText(jsonPath)!);

            themes.Add(new ThemeLegacyResource
            {
                // i don't want to mess with reflection so i'm just going to set all of them one by one
                Name = array[0]["default"].To<string>().Catch(),
                IconResolver = array[0]["image"].To<string>().Catch(),
                PrimaryColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                PrimaryLightColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                PrimaryThinColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                PrimaryDarkColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                PrimaryBlackColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                PrimaryTextColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryLightColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryThinColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryDarkColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryBlackColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                SecondaryTextColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryLightColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryThinColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryDarkColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryBlackColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                DarkPrimaryTextColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
                ShadowColor = ColorTranslator.FromHtml("#" + array[1]["Primary"].To<string>().Catch()),
            });
        }
        #endregion

        var pets = new List<PetResource>();
        var activities = new List<ActivityResource>();
        
        foreach (var jsonPath in (path / "pet").AsDirectory().EnumerateFiles(".json").AsPaths())
        {
            var petJsonArray = JArray.Parse(FileUtils.ReadText(jsonPath)!);
            
            var pet = new PetResource
            {
                Id = petJsonArray[0]["pet"].To<string>().Catch(),
                Name = petJsonArray[0]["petname"].To<string>().Catch(),
                Description = petJsonArray[0]["intor"].To<string>().Catch(),
                RelativePath = petJsonArray[0]["path"].To<string>().Catch(),
                Tags = petJsonArray[1]["tag"].To<string>().Catch().Split(','),
                Consumables = consumables.ToArray()
            };

            foreach (var petJsonToken in petJsonArray)
            {
                var petJsonObject = petJsonToken.To<JObject>()!;

                switch (petJsonObject.TryGetValue<string>("object_base_name"))
                {
                    // ReSharper disable once StringLiteralTypo
                    case "touchhead":
                        interactions.Add(new DisplayAnimationInteractionResource
                        {
                            Name = "Head",
                            Position = new Point(petJsonObject["px"].To<short>(), petJsonObject["py"].To<short>()),
                            Size = new Size(petJsonObject["sw"].To<short>(), petJsonObject["sh"].To<short>())
                        });
                        break;
                    
                    case "touchbody":
                        interactions.Add(new DisplayAnimationInteractionResource
                        {
                            Name = "Body",
                            Position = new Point(petJsonObject["px"].To<short>(), petJsonObject["py"].To<short>()),
                            Size = new Size(petJsonObject["sw"].To<short>(), petJsonObject["sh"].To<short>())
                        });
                        break;
                    
                    case "touchraised":
                        // FIXME: moving pet
                        break;
                    
                    case "pinch":
                        interactions.Add(new DisplayAnimationInteractionResource
                        {
                            Name = "Face",
                            Position = new Point(petJsonObject["px"].To<short>(), petJsonObject["py"].To<short>()),
                            Size = new Size(petJsonObject["sw"].To<short>(), petJsonObject["sh"].To<short>())
                        });
                        break;
                    
                    case "work":
                        activities.Add(new ActivityResource
                        {
                            Name = petJsonObject["Name"].To<string>().Catch(),
                            Type = petJsonObject["Type"].To<string>().Catch(),
                            IconResolver = petJsonObject["Graph"].To<string>().Catch(),
                            CurrencyFactor = petJsonObject["MoneyBase"].To<short>(),
                            SaturationFactor = petJsonObject["StrengthFood"].To<short>(),
                            ThirstFactor = petJsonObject["StrengthDrink"].To<short>(),
                            MoodFactor = petJsonObject["Feeling"].To<short>(),
                            AwardFactor = petJsonObject["FinishBonus"].To<short>(),
                            Duration = TimeSpan.FromMinutes(petJsonObject["MoneyBase"].To<int>())
                        });
                        break;
                    
                    default:
                        // null can be shown here too
                        Log.Warning("[legacy-resource-loader] Found unhandled named object with name {Name}", petJsonObject["object_base_name"].To<string>());
                        break;
                }
            }

            pet.Interactions = interactions.ToArray();
        }
#pragma warning restore CS8604
        
        return pets;
    }

    public PetResource LoadPet(PathObject path)
    {
        throw new NotImplementedException();
    }
}