using DaniDojo.Assets;
using DaniDojo.Managers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DaniDojo.Managers.ResultsManager;

namespace DaniDojo.Hooks
{
    internal class ResultsHook
    {
        // Go into the dani results screen instead of the normal results screen
        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcResult))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_ProcResult_Prefix(EnsoGameManager __instance)
        {
            if (__instance.stateTimer == 1 && (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult()))
            {
                //__instance.graphicManager.SetActiveStateFade();
                GameObject ResultsParent = GameObject.Find("DaniResults");
                if (ResultsParent == null)
                {
                    ResultsParent = AssetUtility.CreateEmptyObject(null, "DaniResults", Vector2.zero);
                    AssetUtility.AddCanvasComponent(ResultsParent);
                }
                DaniResultsPlayer resultsPlayer = ResultsParent.GetComponent<DaniResultsPlayer>();
                if (resultsPlayer == null)
                {
                    resultsPlayer = ResultsParent.AddComponent<DaniResultsPlayer>();
                }
                __instance.stateTimer++;
                return false;
            }

            return true;
        }

        // Don't save any results from this play
        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.SetResults))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_SetResults_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                return false;
            }
            return true;
        }

    }
}
