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
    }
}
