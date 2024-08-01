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
        static bool IsEndOfCourse = false;
        static bool IsEndOfSong = false;

        [HarmonyPatch(typeof(EnsoGameManager))]
        [HarmonyPatch(nameof(EnsoGameManager.CheckEnsoEnd))]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPrefix]
        public static bool EnsoGameManager_CheckEnsoEnd_Prefix(EnsoGameManager __instance)
        {
            if (DaniPlayManager.CheckIsInDan())
            {
                TaikoCoreFrameResults frameResults = __instance.taikoCorePlayer.GetFrameResults();

                if (__instance.ensoParam.EnsoEndType == EnsoPlayingParameter.EnsoEndTypes.None && frameResults.isPastLastOnpuJustTime)
                {
                    if (DaniPlayManager.IsFinalSong() && !IsEndOfCourse)
                    {
                        IsEndOfCourse = true;
                        // Make any bads/oks become rainbow if they're gold reqs
                        DaniDojoAssets.EnsoAssets.UpdateAllRequirementBars(0, endOfCourse: IsEndOfCourse);
                    }
                    else if (!IsEndOfSong)
                    {
                        IsEndOfSong = true;
                        DaniDojoAssets.EnsoAssets.UpdateAllRequirementBars(0, endOfCourse: IsEndOfCourse, endOfSong: IsEndOfSong);
                    }

                    // 5 seconds after the final note of the song
                    // Definitely not perfect, but better than what happened with magical parfait
                    if (frameResults.totalTime < frameResults.fumenLength + 1000)
                    {
                        return false;
                    }

                    if (DaniPlayManager.AdvanceSong())
                    {
                        var courseData = DaniPlayManager.GetCurrentCourse();
                        var songIndex = DaniPlayManager.GetCurrentSongNumber();
                        AdvanceSong(courseData, songIndex);
                        IsEndOfSong = false;
                        IsEndOfCourse = false;
                    }
                    else
                    {
                        CreateAssets = true;
                    }
                }
            }

            return true;
        }


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
#if DEBUG
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.Auto;
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
#else
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
#endif
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
#if DEBUG
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.Auto;
            //TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
#else
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.EnsoData.ensoSettings.ensoPlayerSettings[0].special = DataConst.SpecialTypes.None;
#endif
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
                DaniDojoParent.layer = LayerMask.NameToLayer("UI");

                var CanvasBg = GameObject.Find("CanvasBg");

                DaniDojoParent.name = daniDojoRequirementsParentName;
                DaniDojoParent.transform.parent = __instance.transform.parent.parent;


                AssetUtility.AddCanvasComponent(DaniDojoParent);

                var daniDojoCanvas = DaniDojoParent.GetComponent<Canvas>();
                var baseCanvas = CanvasBg.GetComponent<Canvas>();
                daniDojoCanvas.renderMode = baseCanvas.renderMode;
                daniDojoCanvas.worldCamera = baseCanvas.worldCamera;

                daniDojoCanvas.planeDistance = 1000;

                


                DaniDojoAssetUtility.ChangeSprite("hideLeft", Path.Combine(baseImageFilePath, "Enso", "lane_left_3.png"));

                DaniDojoAssets.EnsoAssets.CreateBottomAssets(DaniDojoParent);

               
                DaniDojoAssets.EnsoAssets.ChangeCourseIcon(null);


                var bgDon = GameObject.FindObjectOfType<BGDon>();
                if (bgDon != null)
                {
                    bgDon.gameObject.SetActive(false);
                }
                var dancer = GameObject.FindObjectOfType<DancerPlayer>();
                if (dancer != null)
                {
                    dancer.gameObject.SetActive(false);
                }
                var bgDai = GameObject.FindObjectOfType<BgDai>();
                if (bgDai != null)
                {
                    var bgDaiDai = bgDai.transform.Find("main").Find("bg_fever_mc").Find("dai");
                    if (bgDaiDai != null)
                    {
                        bgDaiDai.gameObject.SetActive(false);
                    }
                }
                var bgDancer = GameObject.FindObjectOfType<BGDancer>();
                if (bgDancer != null)
                {
                    bgDancer.gameObject.SetActive(false);
                }
                var bgFever = GameObject.FindObjectOfType<BGFever>();
                if (bgFever != null)
                {
                    bgFever.gameObject.SetActive(false);
                }


                #region AddRequirementAssets

                CommonAssets.CreateDaniCourse(DaniDojoParent, new Vector2(1548, 5), DaniPlayManager.GetCurrentCourse());

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
