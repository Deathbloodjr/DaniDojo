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
        //    Plugin.LogInfo("ProcToResult", 1);

        //    if (__instance.graphicManager.IsEnsoFadeBlackEnd())
        //    {
        //        Plugin.LogInfo("IsEnsoFadeBlackEnd() == true", 1);
        //    }

        //    return true;
        //}

        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcResult))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoGameManager_ProcResult_Prefix(EnsoGameManager __instance)
        //{

        //    Plugin.LogInfo("ProcResult", 1);

        //    Plugin.LogInfo("this.ensoParam.IsResultEnd: " + __instance.ensoParam.IsResultEnd);

        //    return true;
        //}

        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoGameManager_ProcExecMain_Prefix(EnsoGameManager __instance)
        //{
        //    Plugin.LogInfo("__instance.ensoParam.TotalTime: " + __instance.ensoParam.TotalTime + "\n" +
        //                   "__instance.totalTime: " + __instance.totalTime + "\n" +
        //                   "__instance.ensoSound.GetSongPosition(): " + __instance.ensoSound.GetSongPosition() + "\n" +
        //                   "__instance.adjustCounter: " + __instance.adjustCounter + "\n" +
        //                   "__instance.adjustSubTime: " + __instance.adjustSubTime + "\n" +
        //                   "__instance.adjustTime: " + __instance.adjustTime + "\n", 1);

        //    return true;
        //}
    }
}
