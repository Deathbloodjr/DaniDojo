using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
