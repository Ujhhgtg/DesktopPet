using System.Diagnostics;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using Ujhhgtg.Library;
using Ujhhgtg.Library.ExtensionMethods;

namespace DesktopPet.Manager.Cli;

public static class Program
{
    private static void Main(string[] args)
    {
        #if DEBUG
        LogUtils.SetupBasicLogger("./logs/manager-cli-debug.log", LogEventLevel.Debug, RollingInterval.Infinite);
        #else
        LogUtils.SetupBasicLogger("./logs/manager-cli.log", LogEventLevel.Warning, RollingInterval.Day);
        #endif

        // var args = (Console.ReadLine() ?? "").Split(' ');

        if (args.Length == 0)
        {
            Log.Error("[manager-cli] Invalid arguments");
        }

        switch (args[0])
        {
            case "convert-lps-to-json":
                Debug.Assert(args.Length == 2);
                
                var inputPath = args[1];
                
                Log.Information("[manager-cli] Started converting lps in {Path}...", inputPath);
                
                #region Lps -> Json
                Log.Information("[manager-cli] Converting .lps in {Path}...", inputPath);

                var inputFiles = Directory.EnumerateFiles(inputPath, "*.lps", SearchOption.AllDirectories).AsPaths();
                
                foreach (var inputFile in inputFiles)
                {
                    var lines = FileUtils.ReadText(inputFile)!/*.Replace(":\n:", "").Replace(":\n|", "\n").Replace(@"\n", "").Replace("\n", "")*/.Split('\n');
                    
                    var root = new JArray();

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("//"))
                            continue;
                        
                        if (string.IsNullOrEmpty(line))
                            continue;
                        
                        var pairs = line.Split(":|").ToList();
                        
                        if (pairs.Count == 0)
                            continue;

                        // lps LINE ends with a SUB splitter (":|")
                        if (pairs[^1] == "")
                        {
                            pairs.RemoveAt(pairs.Count - 1);
                        }

                        var jsonObject = new JObject();
                        
                        foreach (var pair in pairs)
                        {
                            var splitPair = pair.Split('#');
                            var key = splitPair[0];
                            var value = splitPair.TryGetByIndex(1);

                            if (value is null)
                            {
                                jsonObject["object_base_name"] = key;
                            }
                            else
                            {
                                jsonObject[key] = value;
                            }
                        }
                        
                        root.Add(jsonObject);
                    }

                    var content = root.ToString(Formatting.Indented);
                    var path = inputFile.ToString().Replace(".lps", ".json");
                    File.WriteAllText(path, content);
                    
                    #if DEBUG
                    Log.Information("[manager-cli] Wrote to {Path} (content omitted)", path);
                    #endif
                }
                
                #endregion

                #region Zlps -> Directory
                Log.Information("[manager-cli] Converting .zlps in {Path}...", inputPath);

                inputFiles = Directory.EnumerateFiles(inputPath, "*.zlps", SearchOption.AllDirectories).AsPaths();
                
                foreach (var inputFile in inputFiles)
                {
                    var extractPath = inputFile.ToString().Replace(".zlps", "");
                    ZipFile.ExtractToDirectory(inputFile, extractPath, true);
                    Log.Information("[manager-cli] Extracted {ZipPath} to {DirPath}", inputFile, extractPath);
                }
                
                #endregion
                break;
        }
    }

    // private static class Guard
    // {
    //     public static void CompareInts(Comparison comparison, int target1, int target2, string message = "Ass")
    //     {
    //         switch (comparison)
    //         {
    //             case Comparison.Equals:
    //                 if (target1 != target2)
    //                     throw new ArgumentException(message);
    //                 break;
    //             case Comparison.BiggerThan:
    //                 if (target1 <= target2)
    //                     throw new ArgumentException(message);
    //                 break;
    //             case Comparison.SmallerThan:
    //                 if (target1 >= target2)
    //                     throw new ArgumentException(message);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null);
    //         }
    //     }
    //
    //     public enum Comparison
    //     {
    //         BiggerThan,
    //         Equals,
    //         SmallerThan
    //     }
    // }
}