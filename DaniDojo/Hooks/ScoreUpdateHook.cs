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
            if (DaniPlayManager.CheckIsInDan())
            {
                DaniPlayManager.AddScore(score);
            }

            return true;
        }

        [HarmonyPatch(typeof(ScorePlayer))]
        [HarmonyPatch(nameof(ScorePlayer.SetScore))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool ScorePlayer_SetScore_Prefix(ScorePlayer __instance, ref int score)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                score = DaniPlayManager.GetCurrentPlay().SongPlayData.Sum(x => x.Score);
            }
            return true;
        }
    }
}
