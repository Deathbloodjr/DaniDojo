using DaniDojo.Assets;
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
        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcResult))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_ProcResult_Prefix(EnsoGameManager __instance)
        {
            if (__instance.stateTimer == 1)
            {
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
    }
}
