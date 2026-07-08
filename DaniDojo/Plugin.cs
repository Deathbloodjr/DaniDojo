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
using CustomGameModes.Patches;
using UnityEngine.Events;



#if IL2CPP
using BepInEx.Unity.IL2CPP.Utils;
using BepInEx.Unity.IL2CPP;
using Il2CppInterop.Runtime;
#endif

namespace DaniDojo
{

    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, ModName, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.DB.TDMX.CustomGameModes")]
#if MONO
    public class Plugin : BaseUnityPlugin
#elif IL2CPP
    public class Plugin : BasePlugin
#endif
    {
        public const string ModName = "DaniDojo";

        public static Plugin Instance;
        private Harmony _harmony;
        public static ManualLogSource Log;

        public ConfigEntry<bool> ConfigEnabled;
        public ConfigEntry<bool> ConfigDisplayDanSongsInSongSelect;
        public ConfigEntry<string> ConfigDaniDojoDataLocation;
        public ConfigEntry<string> ConfigDaniDojoAssetLocation;
        public ConfigEntry<string> ConfigDaniDojoSaveLocation;


        public ConfigEntry<string> ConfigSongTitleLanguage;

        public ConfigEntry<bool> ConfigNamePlateDanRankEnabled;


#if MONO
        private void Awake()
#elif IL2CPP
        public override void Load()
#endif
        {
            Instance = this;

#if MONO
            Log = Logger;
#elif IL2CPP
            Log = base.Log;
#endif

            SetupConfig(Config, Path.Combine("BepInEx", "data", ModName));

            // This has to be moved somewhere else
            // But not now
            CourseDataManager.LoadCourseData();
            SaveDataManager.LoadSaveData();

            SetupHarmony();
        }

        private void SetupConfig(ConfigFile config, string saveFolder, bool isSaveManager = false)
        {
            string dataFolder = Path.Combine("BepInEx", "data", ModName);

			if (!isSaveManager)
			{
				ConfigEnabled = config.Bind("General",
				   "Enabled",
				   true,
				   "Enables the mod.");
			}

			ConfigDisplayDanSongsInSongSelect = config.Bind("General",
                "DisplayDanSongsInSongSelect",
                true,
                "Will display an icon by songs that are in the active dan series.");

            ConfigDaniDojoDataLocation = config.Bind("Data",
                "DaniDojoDataLocation",
                Path.Combine(dataFolder, "Courses"),
                "The file location for all dani dojo course data.");

            ConfigDaniDojoAssetLocation = config.Bind("Data",
                "DaniDojoAssetLocation",
                Path.Combine(dataFolder, "Assets"),
                "The file location for all dani dojo asset data.");

            ConfigDaniDojoSaveLocation = config.Bind("Data",
                "DaniDojoSaveLocation",
                Path.Combine(saveFolder, "Save"),
                "The file location for dani dojo save data.");

            ConfigSongTitleLanguage = config.Bind("General",
                "SongTitleLanguage",
                "Eng",
                "The language for any song titles that could not be found. (Eng or Jp)");

            ConfigNamePlateDanRankEnabled = config.Bind("NamePlate",
                "DanRankEnabled",
                true,
                "Enables the Dan Rank icon to the left of your name on the nameplate.");
        }


        private void SetupHarmony()
        {
            // Patch methods
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);

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
                _harmony.PatchAll(typeof(DaniDojoSongSelect));
                _harmony.PatchAll(typeof(PlayerNameDaniRank));

                _harmony.PatchAll(typeof(ScoreUpdateHook));
                _harmony.PatchAll(typeof(EnsoPauseHook));

                _harmony.PatchAll(typeof(ResultsHook));
                _harmony.PatchAll(typeof(HitResultHook));
                _harmony.PatchAll(typeof(LoadingScreenHook));

                _harmony.PatchAll(typeof(TamasiiGaugeHooks));

                if (ConfigDisplayDanSongsInSongSelect.Value)
                {
                    _harmony.PatchAll(typeof(SongSelectHooks));
                    _harmony.PatchAll(typeof(SongCourseSelectHook));
                }

                _harmony.PatchAll(typeof(TestingHooks));

                try
                {
#if IL2CPP
                    CustomModeSelectApi.AddButton("DaniDojo", "Dan-i Dojo", "Enters the Dan-i Dojo mode!", new Color32(37, 101, 172, 255), DelegateSupport.ConvertDelegate<UnityAction>(() => DaniDojoDaniCourseSelect.ChangeSceneDaniDojo()));
#else
					CustomModeSelectApi.AddButton("DaniDojo", 
                        "Dan-i Dojo", 
                        "Enters the Dan-i Dojo mode!", 
                        new Color32(37, 101, 172, 255), 
                        DaniDojoDaniCourseSelect.ChangeSceneDaniDojo);
#endif
				}
                catch (Exception e)
                {
                    ModLogger.Log("Failed to add button for DaniDojo mode.", LogType.Error);
                    ModLogger.Log(e.Message, LogType.Error);
                }

                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
            }
            else
            {
                Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is disabled.");
            }
        }

        // I never used these, but they may come in handy at some point
        public static MonoBehaviour GetMonoBehaviour() => TaikoSingletonMonoBehaviour<CommonObjects>.Instance;

        public void StartCoroutine(IEnumerator enumerator)
        {
#if MONO
            GetMonoBehaviour().StartCoroutine(enumerator);
#elif IL2CPP
            GetMonoBehaviour().StartCoroutine(enumerator);
#endif
        }
    }
}