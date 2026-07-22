using DaniDojo.Assets.Audio;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Hooks
{
    internal class TestingHooks
    {
        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcToResult))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoGameManager_ProcToResult_Prefix(EnsoGameManager __instance)
        //{
        //    ModLogger.Log("ProcToResult", 1);

        //    if (__instance.graphicManager.IsEnsoFadeBlackEnd())
        //    {
        //        ModLogger.Log("IsEnsoFadeBlackEnd() == true", 1);
        //    }

        //    return true;
        //}

        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcResult))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoGameManager_ProcResult_Prefix(EnsoGameManager __instance)
        //{

        //    ModLogger.Log("ProcResult", 1);

        //    ModLogger.Log("this.ensoParam.IsResultEnd: " + __instance.ensoParam.IsResultEnd);

        //    return true;
        //}

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_ProcExecMain_Prefix(EnsoGameManager __instance)
        {
            List<string> data = new List<string>()
            {
                "__instance.ensoParam.TotalTime: " + __instance.ensoParam.TotalTime,
                "__instance.totalTime: " + __instance.totalTime,
                "__instance.ensoSound.GetSongPosition(): " + __instance.ensoSound.GetSongPosition(),
                "__instance.adjustCounter: " + __instance.adjustCounter,
                "__instance.adjustSubTime: " + __instance.adjustSubTime,
                "__instance.adjustTime: " + __instance.adjustTime,
            };

            //ModLogger.Log(data, 1);

            return true;
        }

        //[HarmonyPatch(typeof(EnsoSound))]
        //[HarmonyPatch(nameof(EnsoSound.LoadStart))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoSound_LoadStart_Prefix(EnsoSound __instance)
        //{
        //    //List<string> data = new List<string>()
        //    //{
        //    //    "__instance.ensoParam.TotalTime: " + __instance.ensoParam.TotalTime,
        //    //    "__instance.totalTime: " + __instance.totalTime,
        //    //    "__instance.ensoSound.GetSongPosition(): " + __instance.ensoSound.GetSongPosition(),
        //    //    "__instance.adjustCounter: " + __instance.adjustCounter,
        //    //    "__instance.adjustSubTime: " + __instance.adjustSubTime,
        //    //    "__instance.adjustTime: " + __instance.adjustTime,
        //    //};

        //    //ModLogger.Log(data, 1);

        //    return true;
        //}

        [HarmonyPatch(typeof(SoundManager))]
        [HarmonyPatch(nameof(SoundManager.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static void SoundManager_Start_Prefix(SoundManager __instance)
        {
            //ModLogger.Log("SoundManager_Start_Prefix");
            //__instance.gameObject.AddComponent<SoundTester>();
        }


        class SoundTester : MonoBehaviour
        {
            Dictionary<string, bool> loadedCues;
            SoundManager soundManager;

            private readonly Dictionary<KeyCode, DaniDojoAudio> testAudioMappings = new Dictionary<KeyCode, DaniDojoAudio>
            {
                { KeyCode.Alpha1, DaniDojoAudio.BgmDaniOdaiPrimalLoop },
                { KeyCode.Alpha2, DaniDojoAudio.BgmDaniResultPrimalLoop },
                { KeyCode.Alpha3, DaniDojoAudio.SeDaniOdaiIntro },
                { KeyCode.Alpha4, DaniDojoAudio.SeDaniPlayDisqualify },
                { KeyCode.Alpha5, DaniDojoAudio.SeDaniPlayFusumaClose },
                { KeyCode.Alpha6, DaniDojoAudio.SeDaniPlayFusumaOpen },
                { KeyCode.Alpha7, DaniDojoAudio.SeDaniResultPartialPlate },
                { KeyCode.Alpha8, DaniDojoAudio.VoiceDaniOdaiDecide },
                { KeyCode.Alpha9, DaniDojoAudio.VoiceDaniResultAdvance },
            };

            private static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

            void Awake()
            {
                soundManager = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager;
                loadedCues = new Dictionary<string, bool>();
            }


            void Update()
            {
                // If press 1, start audio x
                // If press 2, start audio y
                // etc for whatever audio I want to try playing
                foreach (var mapping in testAudioMappings)
                {
                    if (Input.GetKeyDown(mapping.Key))
                    {
                        PlayAudio2(mapping.Value);
                    }
                }
            }

            void PlayAudio(string audioFileName)
            {
                ModLogger.Log("PlayAudio: " + audioFileName);
                string fullPath = Path.Combine(AssetFilePath, "Sound", audioFileName);
                var audioCue = Path.GetFileNameWithoutExtension(audioFileName);
                if (!loadedCues.ContainsKey(audioCue))
                {
                    soundManager.cueName.Add(audioFileName);
                    CriAtom.AddCueSheet(audioCue, File.ReadAllBytes(fullPath), null, null);
                    loadedCues.Add(audioCue, true);
                }

                soundManager.CommonSePlay(audioCue, true, false);
            }

            void PlayAudio2(DaniDojoAudio sound)
            {
                string audioFileName = sound.GetFileName();
                ModLogger.Log("PlayAudio: " + audioFileName);
                if (audioFileName.Contains("bgm"))
                {
                    DaniSoundManager.PlayBgm(sound);
                }
                else
                {
                    DaniSoundManager.PlaySound(sound);
                }
            }
        }
    }
}
