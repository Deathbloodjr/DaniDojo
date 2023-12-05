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
        //    Plugin.LogInfo(LogType.Info, "ProcToResult", 1);

        //    if (__instance.graphicManager.IsEnsoFadeBlackEnd())
        //    {
        //        Plugin.LogInfo(LogType.Info, "IsEnsoFadeBlackEnd() == true", 1);
        //    }

        //    return true;
        //}

        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcResult))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPrefix]
        //public static bool EnsoGameManager_ProcResult_Prefix(EnsoGameManager __instance)
        //{

        //    Plugin.LogInfo(LogType.Info, "ProcResult", 1);

        //    Plugin.LogInfo(LogType.Info, "this.ensoParam.IsResultEnd: " + __instance.ensoParam.IsResultEnd);

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

            //Plugin.LogInfo(LogType.Info, data, 1);

            return true;
        }
    }
}
