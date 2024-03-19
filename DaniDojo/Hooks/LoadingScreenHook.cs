using DaniDojo.Assets;
using DaniDojo.Managers;
using DaniDojo.Patches;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Hooks
{
    internal class LoadingScreenHook
    {
        [HarmonyPatch(typeof(LoadingScript))]
        [HarmonyPatch(nameof(LoadingScript.SetLoadingCanvasIn))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool LoadingScript_SetLoadingCanvasIn_Prefix(LoadingScript __instance)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                __instance.isDisplaying = true;
                //__instance.setAlphaCanvasGroup(true, true, false);
                //__instance.setLoadingCanvasType(LoadingScript.LoadingTypeName.LoadingSong);
                TaikoSingletonMonoBehaviour<InputGuide>.Instance.DisableGuide();
                __instance.setManager();
                //__instance.animPlayClip(LoadingScript.LoadingAnimType.LoadingIcon, 2, null);
                __instance.StartCoroutine(LoadDaniEnso(__instance));

                __instance.isAnimPlayed = true;
                return false;
            }
            else
            {
                var daniLoading = AssetUtility.GetChildByName(__instance.canvasGroups[0].gameObject, "DaniLoading");
                GameObject.Destroy(daniLoading);
            }
            return true;
        }

        static GameObject DaniLoadingCanvasGroup;
        public static IEnumerator LoadDaniEnso(LoadingScript __instance)
        {
            EndLoading = false;
            var daniParent = AssetUtility.CreateEmptyObject(__instance.canvasGroups[0].gameObject, "DaniLoading", Vector2.zero);
            var loadingBg = AssetUtility.CreateImageChild(daniParent, "DaniEnsoLoadingBg", new Vector2(0, 1080 - 1300), Path.Combine("Loading", "LoadingEnsoBg.png"));
            CommonAssets.CreateDaniCourse(daniParent, new Vector2(1570, 489), DaniPlayManager.GetCurrentCourse());
            yield return AssetUtility.MoveOverSeconds(loadingBg, Vector2.zero, 3);
            yield return new WaitForSeconds(1.25f);

            EndLoading = true;
        }



        [HarmonyPatch(typeof(LoadingScript))]
        [HarmonyPatch(nameof(LoadingScript.SetLoadingCanvasOut))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool LoadingScript_SetLoadingCanvasOut_Prefix(LoadingScript __instance)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                __instance.setLoadingCanvasOut(LoadingScript.LoadingTypeName.LoadingSong, delegate (bool result)
                {
                    if (result)
                    {
                        //__instance.setAlphaCanvasGroup(false, false, false);
                        __instance.isDisplaying = false;
                    }
                });
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LoadingScript))]
        [HarmonyPatch(nameof(LoadingScript.setLoadingCanvasOut))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool LoadingScript_setLoadingCanvasOut_Prefix(LoadingScript __instance, LoadingScript.BoolDelegate callback)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                __instance.setAlphaCanvasGroup(false, false, false);
                __instance.isAnimPlayed = false;

                __instance.StartCoroutine(WaitForSeconds(5, callback));

                return false;
            }
            return true;
        }

        public static bool EndLoading = false;

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcPreparing))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_ProcPreparing_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan() || DaniPlayManager.CheckStartResult())
            {
                return EndLoading;
            }
            return true;
        }

        internal static IEnumerator WaitForSeconds(float sec, LoadingScript.BoolDelegate callback)
        {
            yield return new WaitForSeconds(sec);
            if (callback != null)
            {
                callback(true);
            }
            yield break;
        }

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_ProcExecMain_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                return !DaniDojoTempEnso.EnsoPause;
            }
            return true;
        }

        [HarmonyPatch(typeof(EnsoSound))]
        [HarmonyPatch(nameof(EnsoSound.PlaySong))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoSound_PlaySong_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                return !DaniDojoTempEnso.EnsoPause;
            }
            return true;
        }
    }
}
