using DaniDojo.Managers;
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

            private readonly Dictionary<KeyCode, string> testAudioMappings = new Dictionary<KeyCode, string>
            {
                { KeyCode.Alpha1, "bgm_daniodai_primal_loop.bin" },
                { KeyCode.Alpha2, "bgm_daniresult_primal_loop.bin" },
                { KeyCode.Alpha3, "se_daniodai_intro.bin" },
                { KeyCode.Alpha4, "se_daniplay_disqualify.bin" },
                { KeyCode.Alpha5, "se_daniplay_fusuma_close.bin" },
                { KeyCode.Alpha6, "se_daniplay_fusuma_open.bin" },
                { KeyCode.Alpha7, "se_daniresult_partial_plate.bin" },
                { KeyCode.Alpha8, "voice_daniodai_gaiden.bin" },
                { KeyCode.Alpha9, "voice_daniodai_decide.bin" },
                { KeyCode.Alpha0, "voice_daniresult_advance.bin" },
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

            void PlayAudio2(string audioFileName)
            {
                ModLogger.Log("PlayAudio: " + audioFileName);
                if (audioFileName.Contains("bgm"))
                {
                    DaniSoundManager.PlayBgm(audioFileName);
                }
                else
                {
                    DaniSoundManager.PlaySound(audioFileName);
                }
            }
        }
    }
}
