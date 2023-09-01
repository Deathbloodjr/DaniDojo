using DaniDojo.Managers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Hooks
{
    internal class ScoreUpdateHook
    {
        [HarmonyPatch(typeof(ScorePlayer))]
        [HarmonyPatch(nameof(ScorePlayer.SetAddScorePool))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool ScorePlayer_SetAddScorePool_Prefix(ScorePlayer __instance, int score)
        {
            DaniPlayManager.AddScore(score);
            
            return true;
        }
    }
}
