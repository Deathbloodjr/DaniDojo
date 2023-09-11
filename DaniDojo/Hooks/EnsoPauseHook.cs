using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Hooks
{
    internal class EnsoPauseHook
    {
        static public bool IsPaused = false;

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.StartPause))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_StartPause_Postfix(EnsoGameManager __instance)
        {
            IsPaused = __instance.ensoParam.IsPause;
        }

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecPause))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecPause_Postfix(EnsoGameManager __instance)
        {
            IsPaused = __instance.ensoParam.IsPause;
        }
    }
}
