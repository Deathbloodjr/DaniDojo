using Blittables;
using DaniDojo.Data;
using DaniDojo.Managers;
using DaniDojo.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaikoCoreTypes;

namespace DaniDojo.Hooks
{
    internal class HitResultHook
    {
        #region Note Counting
        //[HarmonyPatch(typeof(HitEffect))]
        //[HarmonyPatch(nameof(HitEffect.switchPlayAnimationOnpuTypes))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPostfix]
        //public static void HitEffect_switchPlayAnimationOnpuTypes_Postfix(HitEffect __instance, HitResultInfo info)
        //{
        //    if (DaniPlayManager.CheckIsInDan())
        //    {
        //        DaniPlayManager.AddHitResult(info);
        //        int hitResult = info.hitResult;
        //        if (info.onpuType == (int)OnpuTypes.Don || info.onpuType == (int)OnpuTypes.Do || info.onpuType == (int)OnpuTypes.Ko || info.onpuType == (int)OnpuTypes.Katsu || info.onpuType == (int)OnpuTypes.Ka
        //            || info.onpuType == (int)OnpuTypes.DaiDon || info.onpuType == (int)OnpuTypes.DaiKatsu
        //            || info.onpuType == (int)OnpuTypes.WDon || info.onpuType == (int)OnpuTypes.WKatsu)
        //        {
        //            if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
        //            {
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Bads);
        //            }
        //            else if (hitResult == (int)HitResultTypes.Ka)
        //            {
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Oks);
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
        //            }
        //            else if (hitResult == (int)HitResultTypes.Ryo)
        //            {
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Goods);
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
        //            }
        //        }
        //        else if (info.onpuType == (int)OnpuTypes.Renda || info.onpuType == (int)OnpuTypes.DaiRenda || info.onpuType == (int)OnpuTypes.Imo || info.onpuType == (int)OnpuTypes.GekiRenda)
        //        {
        //            if (hitResult == (int)HitResultTypes.Ryo)
        //            {
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
        //                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Drumroll);
        //            }
        //        }
        //    }
        //}


        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix_GetNoteResults(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                var frameResult = __instance.ensoParam.GetFrameResults();
                EachPlayer eachPlayer = frameResult.GetEachPlayer(0);
                DaniPlayManager.AddHitResultFromEachPlayer(eachPlayer);
                SoulGaugeManager.AddSoulGaugeValue(frameResult);
                return;
                Plugin.LogInfo(LogType.Info, "eachPlayer.countFuka: " + eachPlayer.countFuka);
                for (int i = 0; i < frameResult.hitResultInfoNum - 1; i++)
                {
                    if (frameResult.hitResultInfo[i].player == 0)
                    {
                        var info = frameResult.hitResultInfo[i];

                        DaniPlayManager.AddHitResult(info);
                        int hitResult = info.hitResult;
                        if (info.onpuType == (int)OnpuTypes.Don || info.onpuType == (int)OnpuTypes.Do || info.onpuType == (int)OnpuTypes.Ko || info.onpuType == (int)OnpuTypes.Katsu || info.onpuType == (int)OnpuTypes.Ka
                            || info.onpuType == (int)OnpuTypes.DaiDon || info.onpuType == (int)OnpuTypes.DaiKatsu
                            || info.onpuType == (int)OnpuTypes.WDon || info.onpuType == (int)OnpuTypes.WKatsu)
                        {
                            if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
                            {
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Bads);
                            }
                            else if (hitResult == (int)HitResultTypes.Ka)
                            {
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Oks);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                            }
                            else if (hitResult == (int)HitResultTypes.Ryo)
                            {
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Goods);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                            }
                        }
                        else if (info.onpuType == (int)OnpuTypes.Renda || info.onpuType == (int)OnpuTypes.DaiRenda)
                        {
                            if (hitResult == (int)HitResultTypes.Ryo)
                            {
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Drumroll);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
