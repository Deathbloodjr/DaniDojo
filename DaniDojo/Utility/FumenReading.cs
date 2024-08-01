using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Utility
{
    internal class FumenReading
    {
        /// <summary>
        /// Gets the solo fumen path for a song added through TakoTako.
        /// </summary>
        /// <param name="songId"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetCustomFumenPath(string songId, EnsoData.EnsoLevelType level)
        {
            var customSongDir = BepInExUtility.GetConfigString("com.fluto.takotako", "CustomSongs", "SongDirectory");
            DirectoryInfo dirInfo = new DirectoryInfo(customSongDir);
            var songDirs = dirInfo.GetDirectories(songId, SearchOption.AllDirectories).ToList();
            for (int i = 0; i < songDirs.Count; i++)
            {
                // search for data.json -> parse it to make sure songId matches
                // I guess the holding folder doesn't need to have the songId, maybe if this fails i search all data.json files, which could take awhile... fuck
                var dataFiles = songDirs[i].GetFiles("data.json", SearchOption.AllDirectories).ToList();
                for (int j = 0; j < dataFiles.Count; j++)
                {
                    var node = LWJson.Parse(File.ReadAllText(dataFiles[j].FullName));
                    if (node != null)
                    {
                        if (node["id"].AsValue().AsString() == songId)
                        {
                            var filePath = Path.Combine(dataFiles[j].Directory.FullName, songId);
                            switch (level)
                            {
                                case EnsoData.EnsoLevelType.Easy: filePath += "_e.bin"; break;
                                case EnsoData.EnsoLevelType.Normal: filePath += "_n.bin"; break;
                                case EnsoData.EnsoLevelType.Hard: filePath += "_h.bin"; break;
                                case EnsoData.EnsoLevelType.Mania: filePath += "_m.bin"; break;
                                case EnsoData.EnsoLevelType.Ura: filePath += "_x.bin"; break;
                            }
                            if (File.Exists(filePath))
                            {
                                return filePath;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                }
            }

            var allDataJsons = dirInfo.GetFiles("data.json", SearchOption.AllDirectories).ToList();
            for (int i = 0; i < allDataJsons.Count; i++)
            {
                var node = LWJson.Parse(File.ReadAllText(allDataJsons[i].FullName));
                if (node != null)
                {
                    if (node["id"].AsValue().AsString() == songId)
                    {
                        var filePath = Path.Combine(allDataJsons[i].Directory.FullName, songId);
                        switch (level)
                        {
                            case EnsoData.EnsoLevelType.Easy: filePath += "_e.bin"; break;
                            case EnsoData.EnsoLevelType.Normal: filePath += "_n.bin"; break;
                            case EnsoData.EnsoLevelType.Hard: filePath += "_h.bin"; break;
                            case EnsoData.EnsoLevelType.Mania: filePath += "_m.bin"; break;
                            case EnsoData.EnsoLevelType.Ura: filePath += "_x.bin"; break;
                        }
                        if (File.Exists(filePath))
                        {
                            return filePath;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            return string.Empty;
        }


    }
}
