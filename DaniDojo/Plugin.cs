using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using BepInEx.Configuration;
using DaniDojo.Patches;
using System.IO;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Linq;
using System.Text.Json;

#if TAIKO_IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace DaniDojo
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, "Dani Dojo", PluginInfo.PLUGIN_VERSION)]
#if TAIKO_MONO
    public class Plugin : BaseUnityPlugin
#elif TAIKO_IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public static Plugin Instance;
        private Harmony _harmony;
        public static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<string> ConfigDaniDojoDataLocation;
        public ConfigEntry<string> ConfigDaniDojoAssetLocation;
        public ConfigEntry<string> ConfigDaniDojoSaveLocation;

        public ConfigEntry<bool> ConfigNamePlateDanRankEnabled;

        public static List<DaniSeriesData> AllDaniData;
        public static List<DaniDojoCurrentPlay> AllDaniScores;

#if TAIKO_MONO
        private void Awake()
#elif TAIKO_IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if TAIKO_MONO
            Log = Logger;
#elif TAIKO_IL2CPP
            Log = base.Log;
#endif

            SetupConfig();

            InitializeDaniDojoSceneAssetBundle();

            InitializeDaniData();
            InitializeDaniRecords();

            SetupHarmony();
        }

        private void SetupConfig()
        {
            // I never really used this
            // I'd rather just use a folder in BepInEx's folder for storing information
            var userFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            ConfigDaniDojoDataLocation = Config.Bind("DaniDojo",
                "DaniDojoDataLocation",
                "BepInEx\\data\\DaniDojo",
                "The file location for all dani dojo course data.");

            ConfigDaniDojoAssetLocation = Config.Bind("DaniDojo",
                "DaniDojoAssetLocation",
                "BepInEx\\data\\DaniDojoAssets",
                "The file location for all dani dojo asset data.");

            ConfigDaniDojoSaveLocation = Config.Bind("DaniDojo",
                "DaniDojoSaveLocation",
                "BepInEx\\data\\DaniDojoSaves",
                "The file location for dani dojo save data.");

            ConfigNamePlateDanRankEnabled = Config.Bind("NamePlate",
                "DanRankEnabled",
                true,
                "Enables the Dan Rank icon to the left of your name on the nameplate.");
        }

        private const string ASSETBUNDLE_NAME = "danidojo.scene";
        public static AssetBundle Assets;
        private void InitializeDaniDojoSceneAssetBundle()
        {
            Plugin.Log.LogInfo("danidojo scene load start");
            string assetBundlePath = Path.Combine(ConfigDaniDojoAssetLocation.Value, ASSETBUNDLE_NAME);
            if (!File.Exists(assetBundlePath))
            {
                Assets = null;
                return;
            }
            Assets = AssetBundle.LoadFromFile(assetBundlePath);
            Plugin.Log.LogInfo("danidojo scene loaded?");
        }

        private void InitializeDaniData()
        {
            AllDaniData = new List<DaniSeriesData>();
            string folderPath = Plugin.Instance.ConfigDaniDojoDataLocation.Value;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles("*.json", SearchOption.AllDirectories).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Log.LogInfo("Initializing " + files[i].Name);
                var text = File.ReadAllText(files[i].FullName);
                JsonNode node = JsonNode.Parse(text)!;
                DaniSeriesData seriesData = new DaniSeriesData();
                seriesData.seriesTitle = node["danSeriesTitle"]!.GetValue<string>();
                seriesData.seriesId = node["danSeriesId"]!.GetValue<string>();
                seriesData.isActiveDan = node["isActiveDan"]!.GetValue<bool>();
                seriesData.order = node["order"]!.GetValue<int>();
                var courses = node["courses"].AsArray();
                for (int j = 0; j < courses.Count; j++)
                {
                    DaniData course = new DaniData(courses[j]!, seriesData);

                    seriesData.courseData.Add(course);
                }
                seriesData.courseData.Sort((x, y) => x.danId > y.danId ? 1 : -1);

                AllDaniData.Add(seriesData);
            }
            AllDaniData.Sort((x, y) => x.order > y.order ? 1 : -1);
            Log.LogInfo("AllDaniData Initialized!");
        }

        private void InitializeDaniRecords()
        {
            Plugin.Log.LogInfo("InitializeDaniRecords");
            AllDaniScores = new List<DaniDojoCurrentPlay>();
            string folderPath = Plugin.Instance.ConfigDaniDojoSaveLocation.Value;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            Plugin.Log.LogInfo("1");
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles("dansave.json").ToList();
            Plugin.Log.LogInfo("2");
            if (files.Count != 0)
            {
                var text = File.ReadAllText(files[0].FullName);
                JsonNode node = JsonNode.Parse(text)!;
                Plugin.Log.LogInfo("3");

                //List<uint> allHashes = new List<uint>();
                var courses = node["courses"].AsArray();
                Plugin.Log.LogInfo("4");
                for (int i = 0; i < courses.Count; i++)
                {
                    //allHashes.Add(courses[i]!["danHash"].GetValue<uint>());
                    Plugin.Log.LogInfo("courses[i]!.GetValue<uint>(): " + courses[i]!["danHash"].GetValue<uint>());
                    DaniDojoCurrentPlay record = new DaniDojoCurrentPlay(courses[i]);

                    Log.LogInfo("1");
                    
                    AllDaniScores.Add(record);
                }
                Plugin.Log.LogInfo("5");



                for (int i = 0; i < AllDaniData.Count; i++)
                {
                    for (int j = 0; j < AllDaniData[i].courseData.Count; j++)
                    {
                        var course = AllDaniData[i].courseData[j];
                        for (int k = 0; k < AllDaniScores.Count; k++)
                        {
                            if (course.hash == AllDaniScores[k].hash)
                            {
                                Log.LogInfo("Started reading in dan hash: " + AllDaniScores[k].hash);
                                AllDaniScores[k].course = course;
                                AllDaniScores[k].danResult = AllDaniScores[k].CalculateRequirements();

                                break;
                            }
                        }
                    }
                }
                Plugin.Log.LogInfo("6");

            }
            Plugin.Log.LogInfo("InitializeDaniRecords Finished!");
        }

        public void SaveDaniRecords()
        {
            Plugin.Log.LogInfo("SaveDaniRecords Start");
            var scoresJsonObject = new JsonObject()
            {
                ["courses"] = new JsonArray(),
            };

            for (int i = 0; i < AllDaniScores.Count; i++)
            {
                Plugin.Log.LogInfo("Starting course " + (i + 1));
                var course = new JsonObject
                {
                    ["danHash"] = AllDaniScores[i].hash,
                    ["danResult"] = (int)AllDaniScores[i].danResult,
                    ["danComboResult"] = (int)AllDaniScores[i].comboResult,
                    ["playCount"] = AllDaniScores[i].playCount,
                    ["songReached"] = AllDaniScores[i].songReached,
                    ["totalSoulGauge"] = 100,
                    ["totalCombo"] = AllDaniScores[i].combo,
                    ["songScores"] = new JsonArray(),
                };

                for (int j = 0; j < AllDaniScores[i].songResults.Count; j++)
                {
                    var song = AllDaniScores[i].songResults[j];
                    var json = new JsonObject()
                    {
                        ["score"] = song.score,
                        ["goods"] = song.goods,
                        ["oks"] = song.oks,
                        ["bads"] = song.bads,
                        ["combo"] = song.songCombo,
                        ["drumroll"] = song.renda,
                        ["totalhits"] = song.goods + song.oks + song.renda,
                    };
                    course["songScores"].AsArray().Add(json);
                }
                scoresJsonObject["courses"].AsArray().Add(course);
                Plugin.Log.LogInfo("Added course " + (i + 1));
            }
            Plugin.Log.LogInfo("SaveDaniRecords 10");

            var filePath = Path.Combine(Plugin.Instance.ConfigDaniDojoSaveLocation.Value, "dansave.json");
            Plugin.Log.LogInfo("FilePath: " + filePath);

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;
                var jsonString = scoresJsonObject.ToJsonString(options);
                //Plugin.Log.LogInfo("JsonString:" + jsonString);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception e)
            {
                Log.LogInfo("Error creating JsonString: " + e.Message);
                throw;
            }


            Plugin.Log.LogInfo("SaveDaniRecords Finished");
        }

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                if (!Directory.Exists(ConfigDaniDojoDataLocation.Value))
                {
                    Directory.CreateDirectory(ConfigDaniDojoDataLocation.Value);
                }
                if (!Directory.Exists(ConfigDaniDojoAssetLocation.Value))
                {
                    Directory.CreateDirectory(ConfigDaniDojoAssetLocation.Value);
                }



                _harmony.PatchAll(typeof(DaniDojoTempEnso));
                _harmony.PatchAll(typeof(DaniDojoTempSelect));
                _harmony.PatchAll(typeof(DaniDojoSongSelect));
                _harmony.PatchAll(typeof(PlayerNameDaniRank));
                //_harmony.PatchAll(typeof(testing));
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Log.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        // I never used these, but they may come in handy at some point
        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCustomCoroutine(IEnumerator enumerator)
        {
#if TAIKO_MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif TAIKO_IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }

    }
}