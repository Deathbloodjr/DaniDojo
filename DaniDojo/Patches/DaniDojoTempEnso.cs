using App;
using Blittables;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaniDojo.Patches.DaniDojoDaniCourseSelect;
using static TaikoCoreTypes;

namespace DaniDojo.Patches
{
    internal class DaniDojoTempEnso
    {
        public static DaniDojoCurrentPlay result;


        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.CheckEnsoEnd))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_CheckEnsoEnd_Prefix(EnsoGameManager __instance)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                TaikoCoreFrameResults frameResults = __instance.taikoCorePlayer.GetFrameResults();

                if (__instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.None && frameResults.isPastLastOnpuJustTime)
                {
                    if (frameResults.totalTime < frameResults.fumenLength || frameResults.totalTime < __instance.songTime)
                    {
                        //Plugin.Log.LogInfo("CheckEnsoEnd: totalTime less than fumen Length and song Time");
                        return false;
                    }

                    DaniDojoSelectManager.currentDanSongIndex++;
                    if (DaniDojoSelectManager.currentDanSongIndex < DaniDojoSelectManager.currentCourse.songs.Count)
                    {
                        if (result.HasFailed())
                        {
                            DaniDojoSelectManager.isInDan = false;
                            //DaniDojoSelectManager.currentCourse = null;
                            DaniDojoSelectManager.currentDanSongIndex = 0;

                            result.SaveResults();
                        }
                        else
                        {
                            BeginSong(DaniDojoSelectManager.currentCourse.songs[DaniDojoSelectManager.currentDanSongIndex].songId, DaniDojoSelectManager.currentCourse.songs[DaniDojoSelectManager.currentDanSongIndex].level);
                            result.AdvanceSong();
                            return false;
                        }
                    }
                    else
                    {
                        DaniDojoSelectManager.isInDan = false;
                        //DaniDojoSelectManager.currentCourse = null;
                        DaniDojoSelectManager.currentDanSongIndex = 0;

                        result.SaveResults();
                    }
                }
            }

            return true;
        }



        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix(EnsoGameManager __instance)
        {
            if (DaniDojoSelectManager.isInDan && result != null)
            {
                if (!result.HasSetConstPoints())
                {
                    Plugin.Log.LogInfo("TaikoCorePlayer_StartPlay_Postfix: Internal If Start");
                    TaikoCoreFrameResults frameResults = __instance.taikoCorePlayer.GetFrameResults();

                    int tamashiiMax = frameResults.eachPlayer[0].constTamashiiMax;
                    int[] tamashiiPoints = new int[3];

                    for (int i = 0; i < 3; i++)
                    {
                        tamashiiPoints[i] = (int)frameResults.eachPlayer[0].constTamashiiPoint[i];
                    }

                    int shinuchiScore = (int)frameResults.eachPlayer[0].constShinuchiScore;

                    for (int i = 0; i < frameResults.eachPlayer[0].constTamashiiPoint.Length; i++)
                    {
                        Plugin.Log.LogInfo("frameResults.eachPlayer[0].constTamashiiPoint[" + i + "]: " + frameResults.eachPlayer[0].constTamashiiPoint[i]);
                    }
                    Plugin.Log.LogInfo("frameResults.eachPlayer[0].constShinuchiScore: " + frameResults.eachPlayer[0].constShinuchiScore);

                    // This might always be 10000
                    Plugin.Log.LogInfo("frameResults.eachPlayer[0].constTamashiiMax: " + frameResults.eachPlayer[0].constTamashiiMax);

                    result.SetConstPoints(tamashiiPoints, shinuchiScore, tamashiiMax);
                    Plugin.Log.LogInfo("TaikoCorePlayer_StartPlay_Postfix: Internal If End");
                }
            }
        }

        public static void BeginSong(string songId, EnsoData.EnsoLevelType level)
        {
            Plugin.Log.LogInfo("BeginSong Start");
            MusicDataInterface.MusicInfoAccesser musicInfoAccesser = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers.Find((MusicDataInterface.MusicInfoAccesser info) => info.Id == songId);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicuid = musicInfoAccesser.Id;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicUniqueId = musicInfoAccesser.UniqueId;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].courseType = level;
            // To prevent highscores from showing up
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].hiScore = 2000000; 

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.genre = (EnsoData.SongGenre)musicInfoAccesser.GenreNo;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.songFilePath = musicInfoAccesser.SongFileName;

            SystemOption systemOption;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.GetSystemOption(out systemOption, false);
            int deviceTypeIndex = EnsoDataManager.GetDeviceTypeIndex(TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].inputDevice);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDispOffset = (int)systemOption.onpuDispLevels[deviceTypeIndex];
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDelay = (int)systemOption.onpuHitLevels[deviceTypeIndex];

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeRelayScene("Enso", false);

            Plugin.Log.LogInfo("BeginSong End");
        }

        #region Note Counting
        [HarmonyPatch(typeof(HitEffect))]
        [HarmonyPatch(nameof(HitEffect.switchPlayAnimationOnpuTypes))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void HitEffect_switchPlayAnimationOnpuTypes_Postfix(HitEffect __instance, HitResultInfo info)
        {
            if (DaniDojoSelectManager.isInDan && result != null)
            {
                int hitResult = info.hitResult;
                if (info.onpuType == (int)OnpuTypes.Don || info.onpuType == (int)OnpuTypes.Do || info.onpuType == (int)OnpuTypes.Ko || info.onpuType == (int)OnpuTypes.Katsu || info.onpuType == (int)OnpuTypes.Ka
                    || info.onpuType == (int)OnpuTypes.DaiDon || info.onpuType == (int)OnpuTypes.DaiKatsu
                    || info.onpuType == (int)OnpuTypes.WDon || info.onpuType == (int)OnpuTypes.WKatsu)
                {
                    if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
                    {
                        result.AddBad();
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Bads);
                    }
                    else if (hitResult == (int)HitResultTypes.Ka)
                    {
                        result.AddOk();
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Oks);
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                    }
                    else if (hitResult == (int)HitResultTypes.Ryo)
                    {
                        result.AddGood();
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Goods);
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                    }
                }
                else if (info.onpuType == (int)OnpuTypes.Renda || info.onpuType == (int)OnpuTypes.DaiRenda || info.onpuType == (int)OnpuTypes.Imo || info.onpuType == (int)OnpuTypes.GekiRenda)
                {
                    if (hitResult == (int)HitResultTypes.Ryo)
                    {
                        result.AddRenda();
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                        DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Drumroll);
                    }
                }
            }
        }


        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix_GetNoteResults(EnsoGameManager __instance)
        {
            if (DaniDojoSelectManager.isInDan && result != null)
            {
                var frameResult = __instance.ensoParam.GetFrameResults();
                for (int i = 0; i < frameResult.hitResultInfoNum - 1; i++)
                {
                    if (frameResult.hitResultInfo[i].player == 0)
                    {
                        var info = frameResult.hitResultInfo[i];

                        int hitResult = info.hitResult;
                        if (info.onpuType == (int)OnpuTypes.Don || info.onpuType == (int)OnpuTypes.Do || info.onpuType == (int)OnpuTypes.Ko || info.onpuType == (int)OnpuTypes.Katsu || info.onpuType == (int)OnpuTypes.Ka
                            || info.onpuType == (int)OnpuTypes.DaiDon || info.onpuType == (int)OnpuTypes.DaiKatsu
                            || info.onpuType == (int)OnpuTypes.WDon || info.onpuType == (int)OnpuTypes.WKatsu)
                        {
                            if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
                            {
                                result.AddBad();
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Bads);
                            }
                            else if (hitResult == (int)HitResultTypes.Ka)
                            {
                                result.AddOk();
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Oks);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                            }
                            else if (hitResult == (int)HitResultTypes.Ryo)
                            {
                                result.AddGood();
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Goods);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
                            }
                        }
                        else if (info.onpuType == (int)OnpuTypes.Renda || info.onpuType == (int)OnpuTypes.DaiRenda)
                        {
                            if (hitResult == (int)HitResultTypes.Ryo)
                            {
                                result.AddRenda();
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Drumroll);
                            }
                        }
                    }
                }
            }
        }



        #endregion


        static string baseImageFilePath = Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        [HarmonyPatch(typeof(EnsoGraphicManager))]
        [HarmonyPatch(nameof(EnsoGraphicManager.CreateParts))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGraphicManager_CreateParts_Postfix(EnsoGraphicManager __instance)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                Plugin.Log.LogInfo("Image Change Start");

                string daniDojoRequirementsParentName = "DaniDojo";

                var DaniDojoParent = new GameObject(daniDojoRequirementsParentName);
                Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Start");
                DaniDojoParent.layer = LayerMask.NameToLayer("UI");

                var CanvasBg = GameObject.Find("CanvasBg");

                DaniDojoParent.name = daniDojoRequirementsParentName;
                DaniDojoParent.transform.parent = __instance.transform.parent.parent;

                Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Parent set");

                var daniDojoCanvas = DaniDojoParent.AddComponent<Canvas>();
                var baseCanvas = CanvasBg.GetComponent<Canvas>();
                daniDojoCanvas.renderMode = baseCanvas.renderMode;
                daniDojoCanvas.worldCamera = baseCanvas.worldCamera;
                daniDojoCanvas.sortingLayerName = "EnsoBG";
                daniDojoCanvas.sortingOrder = 50;
                daniDojoCanvas.overrideSorting = true;

                Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Canvas set");

                var daniDojoCanvasScaler = DaniDojoParent.AddComponent<CanvasScaler>();
                var baseCanvasScaler = CanvasBg.GetComponent<CanvasScaler>();
                daniDojoCanvasScaler.uiScaleMode = baseCanvasScaler.uiScaleMode;
                daniDojoCanvasScaler.referenceResolution = baseCanvasScaler.referenceResolution;
                daniDojoCanvasScaler.screenMatchMode = baseCanvasScaler.screenMatchMode;
                daniDojoCanvasScaler.matchWidthOrHeight = baseCanvasScaler.matchWidthOrHeight;

                Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Canvas Scaler set");


                DaniDojoAssetUtility.ChangeSprite("hideLeft", Path.Combine(baseImageFilePath, "Enso", "lane_left_3.png"));

                DaniDojoAssets.EnsoAssets.CreateBottomAssets(DaniDojoParent);

                // Change this, it's bad
                //DaniDojoAssetUtility.ChangeSprite("DonBgA01P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));
                //DaniDojoAssetUtility.ChangeSprite("DonBgA02P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));
                //DaniDojoAssetUtility.ChangeSprite("DonBgA03P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));
                //DaniDojoAssetUtility.ChangeSprite("DonBgA04P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));
                //DaniDojoAssetUtility.ChangeSprite("DonBgA05P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));
                //DaniDojoAssetUtility.ChangeSprite("DonBgA06P1", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"));

                //DaniDojoAssetUtility.CreateImage("DaniTopBg", Path.Combine(baseImageFilePath, "Enso", "donbg_dojo_1p_bg.png"), new Vector2(0, 800), DaniDojoParent.transform);

                DaniDojoAssets.EnsoAssets.ChangeCourseIcon(null);
                //var courseIconObj = GameObject.Find("icon_course");
                //DaniDojoAssetUtility.ChangeSprite(courseIconObj, Path.Combine(baseImageFilePath, "Enso", "icon_course_danidojo.png"));
                //courseIconObj.GetComponentInChildren<TextMeshProUGUI>().text = "Dan-i dojo";

                Plugin.Log.LogInfo("Image Change 2");

                List<string> StuffToDisable = new List<string>()
                {
                    "bg_normal_01_light",
                    "bg_normal_02_light",
                    "bg05_04_001",

                    "dummy_01",
                    "dummy_02",
                    "dummy_03",
                    "dummy_04",
                    "dummy_05",

                    "dai",
                    "chochin_a",
                    "chochin_b",
                    "bg_normal_04_sakura",
                    "sakura",
                    "akari",
                    "fever_effect_01",
                    "fever_effect_02",
                    "fever_effect_namco",

                    "bg13_bg_001",
                    "bg13_bg_001_f",
                    "bg13_akari_001_l",
                    "bg13_akari_001_r",
                    "bg13_akari_002_l",
                    "bg13_akari_002_r",
                    "bg13_akari_001_l_1",
                    "bg13_akari_003_c_1",
                    "bg13_akari_002_c_1",
                    "bg13_akari_004_c_1",
                    "bg13_akari_001_r_1",
                    "bg13_akari_001_r_2",
                    "bg13_akari_003_c_2",
                    "bg13_akari_001_c_2",
                    "bg13_akari_004_c_2",
                    "bg13_akari_002_l_3",
                    "bg13_akari_001_c_3",
                    "bg13_akari_003_r_3",

                    "bg13_akari_002_r_a",
                    "bg13_akari_002_l_a",
                    "bg13_akari_002_r_b",
                    "bg13_akari_002_l_b",


                    "bg_normal_03_tatemono",
                    "bg_normal_03_denkyu",
                    "bg_normal_03_akari",

                    "bg_normal_03_kumade",
                    "bg_normal_03_kumade_a",

                    "bg_fever_a_01",
                    "bg_fever_a_02",
                    "bg_fever_a_03",
                };



                Plugin.Log.LogInfo("Image Change 3");

                for (int j = 0; j < StuffToDisable.Count; j++)
                {
                    GameObject disableThis = GameObject.Find(StuffToDisable[j]);
                    if (disableThis != null)
                    {
                        disableThis.SetActive(false);
                    }
                }

                Plugin.Log.LogInfo("Image Change 4");

                #region AddRequirementAssets


                //var daniDojoParentPosition = DaniDojoParent.transform.position;
                //daniDojoParentPosition.z = 10;
                //DaniDojoParent.transform.position = daniDojoParentPosition;





                //if (DaniDojoSelectManager.currentDan != null)
                //{
                //    int numPanels = 0;
                //    for (int j = 0; j < DaniDojoSelectManager.currentDan.borders.Count && numPanels < 3; j++)
                //    {
                //        if (DaniDojoSelectManager.currentDan.borders[j].borderType != BorderType.SoulGauge)
                //        {
                //            CreatePanel("Panel" + j, new Vector2(117, 353 - (159 * numPanels)), DaniDojoParent.transform, DaniDojoSelectManager.currentDan.borders[j]);
                //            numPanels++;
                //        }
                //    }

                //    numPanels = 0;
                //    for (int j = 0; j < DaniDojoSelectManager.currentDan.borders.Count && numPanels < 3; j++)
                //    {
                //        if (DaniDojoSelectManager.currentDan.borders[j].borderType != BorderType.SoulGauge)
                //        {
                //            DaniDojoAssets.EnsoAssets.UpdateRequirementBar(DaniDojoSelectManager.currentDan.borders[j].borderType);
                //            numPanels++;
                //        }
                //    }
                //}


                Plugin.Log.LogInfo("Panels created");


                if (DaniDojoSelectManager.currentCourse != null)
                {
                    var imageNames = DaniDojoSelectManager.currentCourse.GetDanCourseImagePaths();
                    string bgImageName = imageNames.bgImage;
                    string textImageName = imageNames.textImage;

                    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", bgImageName), new Vector2(1548, 5), DaniDojoParent.transform);
                    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", textImageName), new Vector2(1600, 129), DaniDojoParent.transform);
                }
                else
                {
                    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", "RedBg.png"), new Vector2(1548, 5), DaniDojoParent.transform);
                    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", "10dan.png"), new Vector2(1600, 129), DaniDojoParent.transform);
                }

                Plugin.Log.LogInfo("CurrentDanMarker created");



                Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Sprite Changed");


                #endregion

                Plugin.Log.LogInfo("Image Change End");
            }

        }

        [HarmonyPatch(typeof(EnsoDonAnimation))]
        [HarmonyPatch(nameof(EnsoDonAnimation.ProcessNormal))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoDonAnimation_ProcessNormal_Prefix(EnsoDonAnimation __instance)
        {
            return false;
        }

        [HarmonyPatch(typeof(IconCourse))]
        [HarmonyPatch(nameof(IconCourse.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void IconCourse_Start_Postfix(IconCourse __instance)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                DaniDojoAssets.EnsoAssets.ChangeCourseIcon(__instance.gameObject);
                //DaniDojoAssetUtility.ChangeSprite(__instance.gameObject, Path.Combine(baseImageFilePath, "Enso", "icon_course_danidojo.png"));
                //__instance.TextDiff.text = "Dan-i dojo";
            }
        }

        [HarmonyPatch(typeof(ComboNumbers.ComboNumber))]
        [HarmonyPatch(nameof(ComboNumbers.ComboNumber.setAnimationComboNumber))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool ComboNumber_setAnimationComboNumber_Prefix(ComboNumbers.ComboNumber __instance, ref uint cmbNum)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                cmbNum = (uint)result.currentCombo;
            }
            return true;
        }



        



        [HarmonyPatch(typeof(BGFever))]
        [HarmonyPatch(nameof(BGFever.IsPrepareFinished))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool BGFever_IsPrepareFinished_Prefix(BGFever __instance, ref bool __result)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(FeverEffect))]
        [HarmonyPatch(nameof(FeverEffect.Update))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool FeverEffect_Update_Prefix(FeverEffect __instance)
        {
            if (DaniDojoSelectManager.isInDan)
            {
                __instance.setup = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnRestartClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnRestartClicked_Prefix(EnsoPauseMenu __instance)
        {
            // Restart the dan
            if (DaniDojoSelectManager.isInDan)
            {
                DaniDojoSelectManager.isInDan = true;
                DaniDojoSelectManager.currentDanSongIndex = 0;

                result = new DaniDojoCurrentPlay(DaniDojoSelectManager.currentCourse);

                BeginSong(DaniDojoSelectManager.currentCourse.songs[0].songId, DaniDojoSelectManager.currentCourse.songs[0].level);
            }
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnReturnClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnReturnClicked_Postfix(EnsoPauseMenu __instance)
        {
            // Exit the dan
            DaniDojoSelectManager.isInDan = false;
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnButtonModeClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnButtonModeClicked_Postfix(EnsoPauseMenu __instance)
        {
            // Exit the dan
            DaniDojoSelectManager.isInDan = false;
        }

    }
}
