using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Utility
{
    internal class BepInExUtility
    {
        public static string GetConfigString(string modGuid, string section, string key)
        {
            string configFilePath = Path.Combine("BepInEx", "config", modGuid + ".cfg");
            if (File.Exists(configFilePath))
            {
                var lines = File.ReadAllLines(configFilePath).ToList();
                string currentSection = string.Empty;
                for (int i = 0; i < lines.Count; i++)
                {
                    var trimLine = lines[i].Trim();
                    if (trimLine.StartsWith("[") && trimLine.EndsWith("]"))
                    {
                        currentSection = trimLine.Remove(0, 1).Remove(trimLine.Length - 2);
                    }
                    if (currentSection == section)
                    {
                        // Example line looks like this:
                        // SongDirectory = ../Taiko no Tatsujin PC/BepInEx/data/TakoTako/customSongs
                        var splitLine = lines[i].Split('=');
                        if (splitLine[0].Trim() == key)
                        {
                            return splitLine[1].Trim();
                        }
                    }
                }
            }

            return string.Empty;
        }

    }
}
