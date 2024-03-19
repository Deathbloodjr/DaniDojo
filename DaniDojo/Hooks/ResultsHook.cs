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
        [HarmonyPatch(typeof(ActionResultPlayer))]
        [HarmonyPatch(nameof(ActionResultPlayer.Update))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool ActionResultPlayer_Update_Prefix(ActionResultPlayer __instance)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                Plugin.LogInfo(LogType.Info, "ActionResultPlayer_Update");
                if (!__instance.isEnable)
                {
                    return false;
                }
                if (__instance.ensoParam == null)
                {
                    return false;
                }
                if (__instance.ensoParam.GetFrameResults() == null)
                {
                    return false;
                }
                EnsoData.PlayerResult playerResult = __instance.ensoParam.GetPlayerResult(__instance.playerNo);
                if (playerResult == null)
                {
                    return false;
                }
                if (!__instance.resultFlag && __instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.Normal && __instance.ensoParam.IsFixResult)
                {
                    var currentPlay = DaniPlayManager.GetCurrentPlay();
                    int goods = currentPlay.SongPlayData.Sum((x) => x.Goods);
                    int oks = currentPlay.SongPlayData.Sum((x) => x.Oks);
                    int bads = currentPlay.SongPlayData.Sum((x) => x.Bads);
                    if (DaniPlayManager.HasFailed())
                    {
                        __instance.gameObject.transform.Find("Fail").gameObject.GetComponent<AnimClips>().AnimClipsAPI_PlayClip(1, 1f, 0f, 0f);
                        __instance.setLanguageType(1);
                        __instance.ensoParam.Sound.KeyOnGame("fin_ng");
                    }
                    else if (oks + bads == 0)
                    {
                        __instance.gameObject.transform.Find("DondaFullCombo").gameObject.GetComponent<StandardAnimClips>().StandardAnimClipsAPI_PlayStart(1f, 0f, null, 0);
                        __instance.setResultAnimation(ActionResultPlayer.ResultAnimationSts.DondaFullCombo);
                        __instance.ensoParam.Sound.KeyOnGameDonVoicePlayer(__instance.playerNo, "v_game_dondafullcombo");
                        __instance.ensoParam.Sound.KeyOnGame("fin_donderful");
                    }
                    else if (bads == 0)
                    {
                        __instance.gameObject.transform.Find("FullCombo").gameObject.GetComponent<StandardAnimClips>().StandardAnimClipsAPI_PlayStart(1f, 0f, null, 0);
                        __instance.setResultAnimation(ActionResultPlayer.ResultAnimationSts.FullCombo);
                        __instance.ensoParam.Sound.KeyOnGameDonVoicePlayer(__instance.playerNo, "v_game_fullcombo");
                        __instance.ensoParam.Sound.KeyOnGame("fin_full");
                    }
                    else
                    {
                        __instance.gameObject.transform.Find("Success").gameObject.GetComponent<StandardAnimClips>().StandardAnimClipsAPI_PlayStart(1f, 0f, null, 0);
                        __instance.setResultAnimation(ActionResultPlayer.ResultAnimationSts.Success);
                        __instance.ensoParam.Sound.KeyOnGame("fin_clear");
                    }
                    __instance.resultFlag = true;
                }
                return false;
            }
            return true;
        }

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
