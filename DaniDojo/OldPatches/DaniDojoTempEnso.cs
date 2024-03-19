using App;
using Blittables;
using DaniDojo.Assets;
using DaniDojo.Data;
using DaniDojo.Hooks;
using DaniDojo.Managers;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TaikoCoreTypes;

namespace DaniDojo.Patches
{
    internal class DaniDojoTempEnso
    {
        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.CheckEnsoEnd))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_CheckEnsoEnd_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                TaikoCoreFrameResults frameResults = __instance.taikoCorePlayer.GetFrameResults();

                //Plugin.LogInfo(LogType.Info, "frameResults.isAllOnpuEnd: " + frameResults.isAllOnpuEnd, 1);
                //Plugin.LogInfo(LogType.Info, "frameResults.isPastLastOnpuJustTime: " + frameResults.isPastLastOnpuJustTime, 1);
                if (__instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.None && frameResults.isPastLastOnpuJustTime)
                {
                    //Plugin.LogInfo(LogType.Info, "frameResults.totalTime: " + frameResults.totalTime, 1);
                    //Plugin.LogInfo(LogType.Info, "__instance.songTime: " + __instance.songTime, 1);
                    //Plugin.LogInfo(LogType.Info, "frameResults.fumenLength: " + frameResults.fumenLength, 1);
                    //if (frameResults.totalTime < frameResults.fumenLength || frameResults.totalTime < __instance.songTime)
                    //{
                    //    //Plugin.Log.LogInfo("CheckEnsoEnd: totalTime less than fumen Length and song Time");
                    //    return false;
                    //}

                    // 5 seconds after the final note of the song
                    // Definitely not perfect, but better than what happened with magical parfait
                    if (frameResults.totalTime < frameResults.fumenLength + 1000)
                    {
                        //Plugin.Log.LogInfo("CheckEnsoEnd: totalTime less than fumen Length and song Time");
                        return false;
                    }

                    if (DaniPlayManager.AdvanceSong())
                    {
                        var courseData = DaniPlayManager.GetCurrentCourse();
                        var songIndex = DaniPlayManager.GetCurrentSongNumber();
                        AdvanceSong(courseData, songIndex);
                    }
                    else
                    {
                        CreateAssets = true;
                    }
                }
            }

            return true;
        }


        // This function can be useful
        // It is how I attempted to get SoulGauge and Score Points data previously

        //[HarmonyPatch(typeof(EnsoGameManager))]
        //[HarmonyPatch(nameof(EnsoGameManager.ProcExecMain))]
        //[HarmonyPatch(MethodType.Normal)]
        //[HarmonyPostfix]
        //public static void EnsoGameManager_ProcExecMain_Postfix(EnsoGameManager __instance)
        //{
        //    if (DaniDojoSelectManager.isInDan && result != null)
        //    {
        //        if (!result.HasSetConstPoints())
        //        {
        //            Plugin.Log.LogInfo("TaikoCorePlayer_StartPlay_Postfix: Internal If Start");
        //            TaikoCoreFrameResults frameResults = __instance.taikoCorePlayer.GetFrameResults();

        //            int tamashiiMax = frameResults.eachPlayer[0].constTamashiiMax;
        //            int[] tamashiiPoints = new int[3];

        //            for (int i = 0; i < 3; i++)
        //            {
        //                tamashiiPoints[i] = (int)frameResults.eachPlayer[0].constTamashiiPoint[i];
        //            }

        //            int shinuchiScore = (int)frameResults.eachPlayer[0].constShinuchiScore;

        //            for (int i = 0; i < frameResults.eachPlayer[0].constTamashiiPoint.Length; i++)
        //            {
        //                Plugin.Log.LogInfo("frameResults.eachPlayer[0].constTamashiiPoint[" + i + "]: " + frameResults.eachPlayer[0].constTamashiiPoint[i]);
        //            }
        //            Plugin.Log.LogInfo("frameResults.eachPlayer[0].constShinuchiScore: " + frameResults.eachPlayer[0].constShinuchiScore);

        //            // This might always be 10000
        //            Plugin.Log.LogInfo("frameResults.eachPlayer[0].constTamashiiMax: " + frameResults.eachPlayer[0].constTamashiiMax);

        //            result.SetConstPoints(tamashiiPoints, shinuchiScore, tamashiiMax);
        //            Plugin.Log.LogInfo("TaikoCorePlayer_StartPlay_Postfix: Internal If End");
        //        }
        //    }
        //}

        static bool CreateAssets = true;

        public static void BeginDan(DaniCourse course)
        {
            // Loading screen here would be awesome
            Plugin.Log.LogInfo("BeginSong Start");
            MusicDataInterface.MusicInfoAccesser musicInfoAccesser = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers.Find((MusicDataInterface.MusicInfoAccesser info) => info.Id == course.Songs[0].SongId);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoType = EnsoData.EnsoType.Normal;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.rankMatchType = EnsoData.RankMatchType.None;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicuid = musicInfoAccesser.Id;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicUniqueId = musicInfoAccesser.UniqueId;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.genre = (EnsoData.SongGenre)musicInfoAccesser.GenreNo;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum = 1;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].neiroId = 0;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].courseType = course.Songs[0].Level;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].speed = DataConst.SpeedTypes.Normal;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].dron = DataConst.OptionOnOff.Off;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].reverse = DataConst.OptionOnOff.Off;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].randomlv = DataConst.RandomLevel.None;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
            // To prevent highscores from showing up
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].hiScore = 2000000;

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.songFilePath = musicInfoAccesser.SongFileName;

            SystemOption systemOption;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.GetSystemOption(out systemOption, false);
            int deviceTypeIndex = EnsoDataManager.GetDeviceTypeIndex(TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].inputDevice);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDispOffset = (int)systemOption.onpuDispLevels[deviceTypeIndex];
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDelay = (int)systemOption.onpuHitLevels[deviceTypeIndex];

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.songVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.InGameSong);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.seVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Se);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.voiceVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Voice);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.bgmVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Bgm);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.neiroVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.InGameNeiro);


            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.SetSettings(ref TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings);

            CreateAssets = true;

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySceneManager.ChangeRelayScene("Enso", false);

            Plugin.Instance.StartCoroutine(CloseLaneCovers(true, course.Songs[0].SongId, (EnsoData.SongGenre)musicInfoAccesser.GenreNo));
        }

        static IEnumerator CloseLaneCovers(bool isInitialClose, string songId, EnsoData.SongGenre genreNo)
        {
            PauseEnso(true);
            while (!LoadingScreenHook.EndLoading)
            {
                yield return null;
            }
            GameObject canvasBack = null;
            do
            {
                canvasBack = GameObject.Find("CanvasBack");
                yield return null;
            } while (canvasBack == null);

            GameObject songInfoObject = null;
            do
            {
                songInfoObject = GameObject.Find("SongInfo");
                yield return null;
            } while (songInfoObject == null);
            var songInfoSongTitleObject = AssetUtility.GetChildByName(songInfoObject, "uiText_song_title_center");
            var songInfoLayout = songInfoObject.GetComponent<SongInfoLayOut>();
            var songInfoPlayer = songInfoObject.GetComponent<SongInfoPlayer>();

            var laneSongTitle = GameObject.Find("LaneSongTitle");
            var laneSongSubtitle = GameObject.Find("LaneSongSubtitle");
            TextMeshProUGUI subtitleText;

            var laneCoverLeft = GameObject.Find("LaneCoverLeft");
            var laneCoverRight = GameObject.Find("LaneCoverRight");

            Vector2 leftPos;
            Vector2 rightPos;

            if (isInitialClose)
            {
                laneSongTitle = GameObject.Instantiate(songInfoSongTitleObject, canvasBack.transform);
                laneSongTitle.name = "LaneSongTitle";
                laneSongTitle.transform.position = new Vector2(250, 140);
                laneSongSubtitle = GameObject.Instantiate(laneSongTitle, canvasBack.transform);
                laneSongSubtitle.name = "LaneSongSubtitle";
                laneSongSubtitle.transform.position = new Vector2(250, 85);
                subtitleText = laneSongSubtitle.GetComponent<TextMeshProUGUI>();
                subtitleText.fontSize = 34;

                int leftX = 1211;
                laneCoverLeft = AssetUtility.CreateImageChild(canvasBack, "LaneCoverLeft", new Vector2(leftX, 596), Path.Combine("Enso", "LaneCover.png"));
                laneCoverRight = AssetUtility.CreateImageChild(canvasBack, "LaneCoverRight", new Vector2(leftX, 596), Path.Combine("Enso", "LaneCover.png"));
                laneCoverLeft.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                leftPos = new Vector2(laneCoverLeft.transform.localPosition.x, laneCoverLeft.transform.localPosition.y);
                rightPos = new Vector2(laneCoverRight.transform.localPosition.x, laneCoverRight.transform.localPosition.y);

                Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(laneCoverLeft, leftPos + new Vector2(711, 0), 0.2f));
                Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(laneCoverRight, rightPos - new Vector2(711, 0), 0.2f));
                DaniSoundManager.PlaySound("lane_close.bin", false);
            }

            // The initial open will display the song title at the top right a bit sooner than the later songs

            yield return new WaitForSeconds(3);

            DaniDojoAssets.EnsoAssets.AdvanceSongPanel(DaniPlayManager.GetCurrentCourse(), DaniPlayManager.GetCurrentSongNumber());

            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(laneSongTitle, 0, true));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(laneSongSubtitle, 0, true));

            var wordDataManager = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr;
            var songTitle = wordDataManager.GetWordListInfo("song_" + songId).Text;
            var songSubtitle = wordDataManager.GetWordListInfo("song_sub_" + songId).Text;

            laneSongTitle.GetComponent<TextMeshProUGUI>().text = songTitle;
            subtitleText = laneSongSubtitle.GetComponent<TextMeshProUGUI>();
            subtitleText.text = songSubtitle;

            WordDataManager.WordListKeysInfo wordListInfo = wordDataManager.GetWordListInfo("song_sub_" + songId);
            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
            subtitleText.fontSharedMaterial = fontTMPMgr.GetDefaultFontMaterial(wordListInfo.FontType, DataConst.DefaultFontMaterialType.OutlineBlack);

            leftPos = new Vector2(laneCoverLeft.transform.localPosition.x, laneCoverLeft.transform.localPosition.y);
            rightPos = new Vector2(laneCoverRight.transform.localPosition.x, laneCoverRight.transform.localPosition.y);

            Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(laneCoverLeft, leftPos - new Vector2(711, 0), 0.2f));
            Plugin.Instance.StartCoroutine(AssetUtility.MoveOverSeconds(laneCoverRight, rightPos + new Vector2(711, 0), 0.2f));

            DaniSoundManager.PlaySound("lane_open.bin", false);


            yield return new WaitForSeconds(3);

            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(laneSongTitle, 2, false));
            Plugin.Instance.StartCoroutine(AssetUtility.ChangeTransparencyOverSeconds(laneSongSubtitle, 2, false));

            songInfoLayout.txt.text = songTitle;
            songInfoLayout.txtCenter.text = "";
            songInfoLayout.Update();
            songInfoPlayer.m_isSongInfoCenter = songInfoLayout.txtCenter.text != "";
            songInfoPlayer.m_Genre = genreNo;
            songInfoPlayer.m_songId = songId;
            songInfoPlayer.m_SongName = songTitle;
            songInfoPlayer.m_bExecute = true;


            yield return new WaitForSeconds(2);

            PauseEnso(false);

            yield break;
        }

        public static void AdvanceSong(DaniCourse course, int songIndex)
        {
            MusicDataInterface.MusicInfoAccesser musicInfoAccesser = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers.Find((MusicDataInterface.MusicInfoAccesser info) => info.Id == course.Songs[songIndex].SongId);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoType = EnsoData.EnsoType.Normal;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.rankMatchType = EnsoData.RankMatchType.None;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicuid = musicInfoAccesser.Id;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.musicUniqueId = musicInfoAccesser.UniqueId;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.genre = (EnsoData.SongGenre)musicInfoAccesser.GenreNo;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.playerNum = 1;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].neiroId = 0;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].courseType = course.Songs[songIndex].Level;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].speed = DataConst.SpeedTypes.Normal;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].dron = DataConst.OptionOnOff.Off;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].reverse = DataConst.OptionOnOff.Off;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].randomlv = DataConst.RandomLevel.None;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
            // To prevent highscores from showing up
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].hiScore = 2000000;

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.songFilePath = musicInfoAccesser.SongFileName;

            SystemOption systemOption;
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.PlayData.GetSystemOption(out systemOption, false);
            int deviceTypeIndex = EnsoDataManager.GetDeviceTypeIndex(TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].inputDevice);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDispOffset = (int)systemOption.onpuDispLevels[deviceTypeIndex];
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.noteDelay = (int)systemOption.onpuHitLevels[deviceTypeIndex];

            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.songVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.InGameSong);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.seVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Se);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.voiceVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Voice);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.bgmVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.Bgm);
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.neiroVolume = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.GetVolume(SoundManager.SoundType.InGameNeiro);


            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.SetSettings(ref TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings);


            var ensoSceneObject = GameObject.Find("SceneEnsoGame");

            var ensoPlayingParameterObj = AssetUtility.GetChildByName(ensoSceneObject, "EnsoPlayingParameter");
            var ensoGameManagerObj = AssetUtility.GetChildByName(ensoSceneObject, "EnsoGameManager");
            var fumenLoaderObj = AssetUtility.GetChildByName(ensoSceneObject, "FumenLoader");
            //var ensoGraphicManagerObj = AssetUtility.GetChildByName(ensoSceneObject, "EnsoGraphicManager");
            var ensoPlayingParameter = ensoPlayingParameterObj.GetComponent<EnsoPlayingParameter>();
            var ensoGameManager = ensoGameManagerObj.GetComponent<EnsoGameManager>();
            var fumenLoader = fumenLoaderObj.GetComponent<FumenLoader>();


            //var ensoGraphicManager = fumenLoaderObj.GetComponent<EnsoGraphicManager>();

            CreateAssets = false;

            ensoGameManager.taikoCorePlayer.EnsoFinailize();
            ensoGameManager.ensoSound.Dispose();

            fumenLoader.Awake();
            ensoGameManager.Awake();
            ensoPlayingParameter.Awake();
            fumenLoader.Start();
            ensoGameManager.Start();
            ensoPlayingParameter.TotalTime = 0;
            ensoGameManager.totalTime = 0;
            ensoGameManager.adjustCounter = 0;
            ensoGameManager.adjustSubTime = 0.0;
            ensoGameManager.adjustTime = 0.0;

            Plugin.Instance.StartCoroutine(CloseLaneCovers(false, course.Songs[songIndex].SongId, (EnsoData.SongGenre)musicInfoAccesser.GenreNo));
        }

        public static bool EnsoPause = false;

        static void PauseEnso(bool pause)
        {
            EnsoPause = pause;

            Plugin.Instance.StartCoroutine(PauseEnsoSong(pause));

            Plugin.LogInfo(LogType.Info, "EnsoPause: " + EnsoPause);

        }

        static IEnumerator PauseEnsoSong(bool pause)
        {
            GameObject ensoSceneObject;
            do
            {
                ensoSceneObject = GameObject.Find("SceneEnsoGame");
                yield return null;
            } while (ensoSceneObject == null);

            var ensoPlayingParameterObj = AssetUtility.GetChildByName(ensoSceneObject, "EnsoPlayingParameter");
            var ensoGameManagerObj = AssetUtility.GetChildByName(ensoSceneObject, "EnsoGameManager");

            var ensoPlayingParameter = ensoPlayingParameterObj.GetComponent<EnsoPlayingParameter>();
            var ensoGameManager = ensoGameManagerObj.GetComponent<EnsoGameManager>();


            ensoPlayingParameter.TotalTime = 0;
            ensoGameManager.totalTime = 0;

            if (pause)
            {
                Plugin.LogInfo(LogType.Info, "StopSong");
                ensoGameManager.ensoSound.StopSong();
                var laneHitEffects = GameObject.FindObjectOfType<LaneHitEffects>();
                laneHitEffects.Start();
                laneHitEffects.isStart = false;
                laneHitEffects.branchEffect.AnimClipsAPI_PlayClip(0);
            }
            if (!pause)
            {
                Plugin.LogInfo(LogType.Info, "PrepareSong");
                ensoGameManager.ensoSound.StopSong();
                ensoGameManager.ensoSound.PrepareSong(0);
                ensoGameManager.ensoSound.PlaySong();

                ensoGameManager.totalTime = 0;
                ensoGameManager.adjustCounter = 0;
                ensoGameManager.adjustSubTime = 0.0;
                ensoGameManager.adjustTime = 0.0;
            }
        }





        [HarmonyPatch(typeof(EnsoGraphicManager))]
        [HarmonyPatch(nameof(EnsoGraphicManager.CreateParts))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGraphicManager_CreateParts_Prefix(EnsoGraphicManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                return CreateAssets;
            }
            return true;
        }

        static string baseImageFilePath = Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        [HarmonyPatch(typeof(EnsoGraphicManager))]
        [HarmonyPatch(nameof(EnsoGraphicManager.CreateParts))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGraphicManager_CreateParts_Postfix(EnsoGraphicManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan() && CreateAssets)
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

                AssetUtility.AddCanvasComponent(DaniDojoParent);

                var daniDojoCanvas = DaniDojoParent.GetComponent<Canvas>();
                var baseCanvas = CanvasBg.GetComponent<Canvas>();
                daniDojoCanvas.renderMode = baseCanvas.renderMode;
                daniDojoCanvas.worldCamera = baseCanvas.worldCamera;

                daniDojoCanvas.planeDistance = 1000;

                //daniDojoCanvas.sortingLayerName = "EnsoBG";
                //daniDojoCanvas.sortingOrder = 50;
                //daniDojoCanvas.overrideSorting = true;

                //Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Canvas set");

                //var daniDojoCanvasScaler = DaniDojoParent.AddComponent<CanvasScaler>();
                //var baseCanvasScaler = CanvasBg.GetComponent<CanvasScaler>();
                //daniDojoCanvasScaler.uiScaleMode = baseCanvasScaler.uiScaleMode;
                //daniDojoCanvasScaler.referenceResolution = baseCanvasScaler.referenceResolution;
                //daniDojoCanvasScaler.screenMatchMode = baseCanvasScaler.screenMatchMode;
                //daniDojoCanvasScaler.matchWidthOrHeight = baseCanvasScaler.matchWidthOrHeight;

                //Plugin.Log.LogInfo("Image Change: DaniDojoRequirements: Canvas Scaler set");


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

                CommonAssets.CreateDaniCourse(DaniDojoParent, new Vector2(1548, 5), DaniPlayManager.GetCurrentCourse());

                //var currentCourse = DaniPlayManager.GetCurrentCourse();
                //if (currentCourse != null)
                //{
                //    var courseId = currentCourse.Id;
                //    if (int.TryParse(courseId, out int _))
                //    {
                //        courseId = currentCourse.Title;
                //    }

                //    (string bgImage, string textImage) imageNames;

                //    imageNames = courseId switch
                //    {
                //        "5kyuu" or "五級 5th Kyu" => ("WoodBg.png", "kyuu5.png"),
                //        "4kyuu" or "四級 4th Kyu" => ("WoodBg.png", "kyuu4.png"),
                //        "3kyuu" or "三級 3rd Kyu" => ("WoodBg.png", "kyuu3.png"),
                //        "2kyuu" or "二級 2nd Kyu" => ("WoodBg.png", "kyuu2.png"),
                //        "1kyuu" or "一級 1st Kyu" => ("WoodBg.png", "kyuu1.png"),
                //        "1dan" or "初段 1st Dan" => ("BlueBg.png", "dan1.png"),
                //        "2dan" or "二段 2nd Dan" => ("BlueBg.png", "dan2.png"),
                //        "3dan" or "三段 3rd Dan" => ("BlueBg.png", "dan3.png"),
                //        "4dan" or "四段 4th Dan" => ("BlueBg.png", "dan4.png"),
                //        "5dan" or "五段 5th Dan" => ("BlueBg.png", "dan5.png"),
                //        "6dan" or "六段 6th Dan" => ("RedBg.png", "dan6.png"),
                //        "7dan" or "七段 7th Dan" => ("RedBg.png", "dan7.png"),
                //        "8dan" or "八段 8th Dan" => ("RedBg.png", "dan8.png"),
                //        "9dan" or "九段 9th Dan" => ("RedBg.png", "dan9.png"),
                //        "10dan" or "十段 10th Dan" => ("RedBg.png", "dan10.png"),
                //        "11dan" or "玄人 Kuroto" => ("SilverBg.png", "kuroto.png"),
                //        "12dan" or "名人 Meijin" => ("SilverBg.png", "meijin.png"),
                //        "13dan" or "超人 Chojin" => ("SilverBg.png", "chojin.png"),
                //        "14dan" or "達人 Tatsujin" => ("GoldBg.png", "tatsujin.png"),
                //        _ => ("TanBg.png", "gaiden.png"),
                //    };

                //    string bgImageName = imageNames.bgImage;
                //    string textImageName = imageNames.textImage;

                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", bgImageName), new Vector2(1548, 5), DaniDojoParent.transform);
                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", textImageName), new Vector2(1600, 129), DaniDojoParent.transform);
                //}
                //else
                //{
                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", "RedBg.png"), new Vector2(1548, 5), DaniDojoParent.transform);
                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", Path.Combine(baseImageFilePath, "Course", "DaniCourseIcons", "10dan.png"), new Vector2(1600, 129), DaniDojoParent.transform);
                //}

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
            if (DaniPlayManager.CheckIsInDan())
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(IconCourse))]
        [HarmonyPatch(nameof(IconCourse.Start))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void IconCourse_Start_Postfix(IconCourse __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
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
            if (DaniPlayManager.CheckIsInDan())
            {
                cmbNum = (uint)DaniPlayManager.GetCurrentCombo();
            }
            return true;
        }







        [HarmonyPatch(typeof(BGFever))]
        [HarmonyPatch(nameof(BGFever.IsPrepareFinished))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool BGFever_IsPrepareFinished_Prefix(BGFever __instance, ref bool __result)
        {
            if (DaniPlayManager.CheckIsInDan())
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
            if (DaniPlayManager.CheckIsInDan())
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
            if (DaniPlayManager.CheckIsInDan())
            {
                DaniPlayManager.RestartDanPlay();

                BeginDan(DaniPlayManager.GetCurrentCourse());
            }
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnReturnClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnReturnClicked_Postfix(EnsoPauseMenu __instance)
        {
            // Exit the dan
            if (DaniPlayManager.CheckIsInDan())
            {
                DaniPlayManager.LeaveDanPlay();
            }
        }

        [HarmonyPatch(typeof(EnsoPauseMenu))]
        [HarmonyPatch(nameof(EnsoPauseMenu.OnButtonModeClicked))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoPauseMenu_OnButtonModeClicked_Postfix(EnsoPauseMenu __instance)
        {
            // Exit the dan
            if (DaniPlayManager.CheckIsInDan())
            {
                DaniPlayManager.LeaveDanPlay();
            }
        }

    }
}
