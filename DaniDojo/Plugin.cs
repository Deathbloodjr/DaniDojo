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
using System.Linq;
using DaniDojo.Managers;
using DaniDojo.Hooks;

#if TAIKO_IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
#endif

namespace DaniDojo
{
    public enum LogType
    {
        Info,
        Warning,
        Error,
        Fatal,
        Message,
        Debug
    }

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


        public ConfigEntry<string> ConfigSongTitleLanguage;

        public ConfigEntry<bool> ConfigNamePlateDanRankEnabled;

        public ConfigEntry<bool> ConfigLoggingEnabled;
        public ConfigEntry<int> ConfigLoggingDetailLevelEnabled;


        //public static List<DaniSeriesData> AllDaniData;
        //public static List<DaniDojoCurrentPlay> AllDaniScores;

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

            //InitializeDaniData();
            CourseDataManager.LoadCourseData();
            //InitializeDaniRecords();
            SaveDataManager.LoadSaveData();

            SetupHarmony();
        }

        private void SetupConfig()
        {
            string dataFolder = Path.Combine("BepInEx", "data", "DaniDojo");

            ConfigEnabled = Config.Bind("General",
                "Enabled",
                true,
                "Enables the mod.");

            ConfigDaniDojoDataLocation = Config.Bind("Data",
                "DaniDojoDataLocation",
                Path.Combine(dataFolder, "Courses"),
                "The file location for all dani dojo course data.");

            ConfigDaniDojoAssetLocation = Config.Bind("Data",
                "DaniDojoAssetLocation",
                Path.Combine(dataFolder, "Assets"),
                "The file location for all dani dojo asset data.");

            ConfigDaniDojoSaveLocation = Config.Bind("Data",
                "DaniDojoSaveLocation",
                Path.Combine(dataFolder, "Save"),
                "The file location for dani dojo save data.");

            ConfigSongTitleLanguage = Config.Bind("General",
                "SongTitleLanguage",
                "Eng",
                "The language for any song titles that could not be found. (Eng or Jp)");

            ConfigNamePlateDanRankEnabled = Config.Bind("NamePlate",
                "DanRankEnabled",
                true,
                "Enables the Dan Rank icon to the left of your name on the nameplate.");

            ConfigLoggingEnabled = Config.Bind("Debug",
                "LoggingEnabled",
                true,
                "Enables logs to be sent to the console.");

            ConfigLoggingDetailLevelEnabled = Config.Bind("Debug",
                "LoggingDetailLevelEnabled",
                0,
                "Enables more detailed logs to be sent to the console. The higher the number, the more logs will be displayed. Mostly for my own debugging.");
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

        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

            if (ConfigEnabled.Value)
            {
                // This stuff shouldn't be done here
                // It should be in their respective managers
                // And each of these will have their own managers
                if (!Directory.Exists(ConfigDaniDojoAssetLocation.Value))
                {
                    Directory.CreateDirectory(ConfigDaniDojoAssetLocation.Value);
                }
                if (!Directory.Exists(ConfigDaniDojoSaveLocation.Value))
                {
                    Directory.CreateDirectory(ConfigDaniDojoSaveLocation.Value);
                }



                _harmony.PatchAll(typeof(DaniDojoTempEnso));
                //_harmony.PatchAll(typeof(DaniDojoTempSelect));
                _harmony.PatchAll(typeof(DaniDojoSongSelect));
                _harmony.PatchAll(typeof(PlayerNameDaniRank));

                _harmony.PatchAll(typeof(ScoreUpdateHook));
                _harmony.PatchAll(typeof(EnsoPauseHook));

                _harmony.PatchAll(typeof(ResultsHook));
                _harmony.PatchAll(typeof(HitResultHook));
                _harmony.PatchAll(typeof(LoadingScreenHook));



                _harmony.PatchAll(typeof(TestingHooks));
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

        //public void LogInfoInstance(string value, int detailLevel = 0)
        //{
        //    // Only print if Detailed Enabled is true, or if DetailedEnabled is false and isDetailed is false
        //    if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
        //    {
        //        Log.LogInfo("[" + detailLevel + "] " + value);
        //    }
        //}
        //public static void LogInfo(string value, int detailLevel = 0)
        //{
        //    Instance.LogInfoInstance(value, detailLevel);
        //}

        public void LogInfoInstance(LogType type, string value, int detailLevel = 0)
        {
            // Only print if Detailed Enabled is true, or if DetailedEnabled is false and isDetailed is false
            if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
            {
                switch (type)
                {
                    case LogType.Info:
                        Log.LogInfo("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Warning:
                        Log.LogWarning("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Error:
                        Log.LogError("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Fatal:
                        Log.LogFatal("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Message:
                        Log.LogMessage("[" + detailLevel + "] " + value);
                        break;
                    case LogType.Debug:
                        Log.LogDebug("[" + detailLevel + "] " + value);
                        break;
                    default:
                        break;
                }
            }
        }
        public static void LogInfo(LogType type, string value, int detailLevel = 0)
        {
            Instance.LogInfoInstance(type, value, detailLevel);
        }
        public static void LogInfo(LogType type, List<string> value, int detailLevel = 0)
        {
            if (value.Count == 0)
            {
                return;
            }
            string sendValue = value[0];
            for (int i = 1; i < value.Count; i++)
            {
                sendValue += "\n" + value[i];
            }
            Instance.LogInfoInstance(type, sendValue, detailLevel);
        }

        //public void LogWarningInstance(string value, int detailLevel = 0)
        //{
        //    if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
        //    {
        //        Log.LogWarning("[" + detailLevel + "] " + value);
        //    }
        //}
        //public static void LogWarning(string value, int detailLevel = 0)
        //{
        //    Instance.LogWarningInstance(value, detailLevel);
        //}


        //public void LogErrorInstance(string value, int detailLevel = 0)
        //{
        //    if (ConfigLoggingEnabled.Value && (ConfigLoggingDetailLevelEnabled.Value >= detailLevel))
        //    {
        //        Log.LogError("[" + detailLevel + "] " + value);
        //    }
        //}
        //public static void LogError(string value, int detailLevel = 0)
        //{
        //    Instance.LogErrorInstance(value, detailLevel);
        //}

    }
}