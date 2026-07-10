using DaniDojo.Assets;
using DaniDojo.Data;
using DaniDojo.Patches;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static DaniDojo.Patches.DaniDojoDaniCourseSelect;
#if IL2CPP
using Il2CppInterop.Runtime.Injection;
#endif

namespace DaniDojo.Managers
{
    internal class ResultsManager
    {
        internal class DaniResultsPlayer : MonoBehaviour
        {
#if IL2CPP
            static DaniResultsPlayer() => ClassInjector.RegisterTypeInIl2Cpp<DaniResultsPlayer>();
#endif
            enum DaniResultScreen
            {
                Songs,
                PlayResults,
                Advancement,
            }

            DaniResultScreen CurrentScreen;

            GameObject ResultsParent;
            GameObject PlayRecordParent;

            GameObject donCommon;
            GameObject playerName;

            DaniCourse currentCourse;

            private List<GameObject> songPanels = new List<GameObject>();
            private List<Coroutine> songPanelCoroutines = new List<Coroutine>();

            private Coroutine activeIntroCoroutine = null;
            private Coroutine autoAdvanceCoroutine = null;
            private Coroutine playResultMovementCoroutine = null;

            private Dictionary<DaniResultScreen, bool> introAnimationPlayed = new Dictionary<DaniResultScreen, bool>();

            private bool isSongsIntroAnimating = false;
            private bool isPlayResultsAnimating = false;
            private float autoAdvanceDelay = 10.0f;

            private bool autoScreenshotLoaded = false;
            private Dictionary<DaniResultScreen, bool> screenshotsTaken = new Dictionary<DaniResultScreen, bool>();



            public void Start()
            {
                ResultsParent = this.gameObject;
                currentCourse = DaniPlayManager.GetCurrentCourse();

                DaniSoundManager.SetupBgm("results_primal_loop.bin", true);
                DaniSoundManager.PlayBgm();

                foreach (DaniResultScreen screen in Enum.GetValues(typeof(DaniResultScreen)))
                {
                    introAnimationPlayed[screen] = false;
                }

                autoScreenshotLoaded = IsAutoScreenshotLoaded();
                if (autoScreenshotLoaded)
                {
                    foreach (DaniResultScreen screen in Enum.GetValues(typeof(DaniResultScreen)))
                    {
                        screenshotsTaken[screen] = false;
                    }
                }

                InitializeAssets();
                CurrentScreen = DaniResultScreen.Songs;
            }

            bool changingScene = false;

            public void Update()
            {
                if (!changingScene)
                {
                    GetInput();
                }
            }


            public void GetInput()
            {
                ControllerManager.Dir dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
                bool okDown = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1);
                bool cancelDown = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetCancelDown(ControllerManager.ControllerPlayerNo.Player1);

                if (CurrentScreen == DaniResultScreen.Songs && isSongsIntroAnimating)
                {
                    if (okDown)
                    {
                        ModLogger.Log("Stop Songs intro", LogType.Debug);

                        // Stop the intro sequence
                        if (activeIntroCoroutine != null) Plugin.Instance.StopCoroutine(activeIntroCoroutine);
                        activeIntroCoroutine = null;

                        // Instantly snap all panels to their finished positions
                        for (int i = 0; i < songPanelCoroutines.Count; i++)
                        {
                            if (songPanelCoroutines[i] != null)
                            {
                                Plugin.Instance.StopCoroutine(songPanelCoroutines[i]);
                            }
                        }
                        songPanelCoroutines.Clear();

                        for (int i = 0; i < songPanels.Count; i++)
                        {
                            var newPos = ResultsAssets.GetSongPanelPosition(i, false);

                            songPanels[i].transform.localPosition = newPos;
                        }

                        isSongsIntroAnimating = false;

                        TakeScreenshot(DaniResultScreen.Songs);

                        autoAdvanceCoroutine = Plugin.Instance.StartCoroutine(AutoAdvanceTimer());
                        return; // Consume input for this frame
                    }
                }
                else if (CurrentScreen == DaniResultScreen.PlayResults && isPlayResultsAnimating)
                {
                    if (okDown)
                    {
                        ModLogger.Log("Stop PlayResults intro", LogType.Debug);

                        if (activeIntroCoroutine != null) Plugin.Instance.StopCoroutine(activeIntroCoroutine);
                        activeIntroCoroutine = null;

                        if (playResultMovementCoroutine != null)
                        {
                            Plugin.Instance.StopCoroutine(playResultMovementCoroutine);
                        }
                        playResultMovementCoroutine = null;

                        PlayRecordParent.transform.position = new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0));

                        isPlayResultsAnimating = false;

                        TakeScreenshot(DaniResultScreen.PlayResults);
                        return; // Consume input for this frame
                    }
                }

                if (CurrentScreen == DaniResultScreen.Songs && autoAdvanceCoroutine != null)
                {
                    if (dir == ControllerManager.Dir.Right || okDown)
                    {
                        Plugin.Instance.StopCoroutine(autoAdvanceCoroutine);
                        autoAdvanceCoroutine = null;
                        TransitionToScreen(DaniResultScreen.PlayResults);
                        return; // Consume input for this frame
                    }
                }

                if ((dir == ControllerManager.Dir.Left || cancelDown) && CurrentScreen == DaniResultScreen.PlayResults)
                {
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                    TransitionToScreen(DaniResultScreen.Songs);
                }
                else if ((dir == ControllerManager.Dir.Right || okDown) && CurrentScreen == DaniResultScreen.Songs && !isSongsIntroAnimating)
                {
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                    TransitionToScreen(DaniResultScreen.PlayResults);
                }
                else if ((okDown) && CurrentScreen == DaniResultScreen.PlayResults)
                {
                    TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
                    DaniPlayManager.SetStartResult(false);
                    changingScene = true;
                    DaniDojoDaniCourseSelect.ChangeSceneDaniDojo();
                }
            }

            private void InitializeAssets()
            {
                var bg = ResultsAssets.CreateBg(ResultsParent);

                var songPanel = ResultsAssets.CreateSongPanel(bg);

                var currentPlay = DaniPlayManager.GetCurrentPlay();

                songPanels = ResultsAssets.CreateEachSongBg(songPanel, currentCourse, currentPlay, SaveDataManager.GetCourseRecord(currentCourse.Hash), true);

                PlayRecordParent = ResultsAssets.CreatePlayRecordBg(bg);
                var scoreBg = ResultsAssets.CreatePlayRecordScoreBg(PlayRecordParent, currentPlay);
                var playRecord1Bg = ResultsAssets.CreatePlayRecordGoodOkBad(PlayRecordParent, currentPlay);
                var playRecord2Bg = ResultsAssets.CreatePlayRecordDrumrollComboTotalHits(PlayRecordParent, currentPlay);
                var borders = ResultsAssets.CreateBorderPanels(PlayRecordParent, currentCourse, currentPlay);

                var danCourseIcon = ResultsAssets.CreateCourseIcon(bg, currentCourse);
                var danCourseTitle = ResultsAssets.CreateCourseTitle(bg, currentCourse);

                donCommon = Instantiate(DaniDojoSongSelect.donCommonObject);
                playerName = Instantiate(DaniDojoSongSelect.playerNameObject);
                donCommon.transform.SetParent(bg.transform);
                playerName.transform.SetParent(bg.transform);

                donCommon.transform.localPosition = new Vector3(202, 289, 0);
                playerName.transform.localPosition = new Vector3(194, 120, 0);

                var danResultAsset = ResultsAssets.CreateDanResult(bg, currentPlay);


                activeIntroCoroutine = Plugin.Instance.StartCoroutine(PlaySongPanelsIntro());
            }

            private void TransitionToScreen(DaniResultScreen targetScreen)
            {
                // Stop any overarching screen intro sequence currently running if they manual-swapped
                if (activeIntroCoroutine != null)
                {
                    Plugin.Instance.StopCoroutine(activeIntroCoroutine);
                    activeIntroCoroutine = null;
                }

                CurrentScreen = targetScreen;

                // Check if we've already run the setup/animations for this screen context
                if (!introAnimationPlayed[targetScreen])
                {
                    introAnimationPlayed[targetScreen] = true; // Mark as started instantly

                    // Route to the appropriate unique animation sequencer
                    switch (targetScreen)
                    {
                        case DaniResultScreen.Songs:
                            activeIntroCoroutine = Plugin.Instance.StartCoroutine(PlaySongPanelsIntro());
                            break;
                        case DaniResultScreen.PlayResults:
                            activeIntroCoroutine = Plugin.Instance.StartCoroutine(PlayResultsIntro());
                            break;
                        case DaniResultScreen.Advancement:
                            activeIntroCoroutine = Plugin.Instance.StartCoroutine(PlayAdvancementIntro());
                            break;
                    }
                }
                else
                {
                    switch (targetScreen)
                    {
                        case DaniResultScreen.Songs:
                            Plugin.Instance.StartCoroutine(MoveOverSeconds(PlayRecordParent, new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337 + 1920, 0)), 0.1f));
                            break;
                        case DaniResultScreen.PlayResults:
                            Plugin.Instance.StartCoroutine(MoveOverSeconds(PlayRecordParent, new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0)), 0.1f));
                            break;
                        case DaniResultScreen.Advancement:
                            break;
                        default:
                            break;
                    }
                }
            }

            public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds, bool deleteAfter = false)
            {
                float elapsedTime = 0;
                Vector3 startingPos = objectToMove.transform.position;

                while (elapsedTime < seconds)
                {
                    objectToMove.transform.position = Vector3.Lerp(startingPos, end, elapsedTime / seconds);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                objectToMove.transform.position = end;
                if (deleteAfter) GameObject.Destroy(objectToMove);
            }

            public IEnumerator MoveLocalOverSeconds(GameObject objectToMove, Vector3 end, float seconds, bool deleteAfter = false)
            {
                float elapsedTime = 0;
                Vector3 startingPos = objectToMove.transform.localPosition;

                while (elapsedTime < seconds)
                {
                    objectToMove.transform.localPosition = Vector3.Lerp(startingPos, end, elapsedTime / seconds);
                    elapsedTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                objectToMove.transform.localPosition = end;
                if (deleteAfter) GameObject.Destroy(objectToMove);
            }

            private IEnumerator PlaySongPanelsIntro()
            {
                ModLogger.Log("PlaySongPanelsIntro", LogType.Debug);
                isSongsIntroAnimating = true;

                yield return new WaitForSeconds(1);

                // Slide each panel in 1-by-1 with a slight delay between them
                float slideDuration = 0.3f;
                float delayBetweenPanels = 0.08f;

                for (int i = 0; i < songPanels.Count; i++)
                {
                    if (i != 0)
                    {
                        // Wait for the delay if it isn't the first panel
                        yield return new WaitForSeconds(delayBetweenPanels);
                    }

                    //ModLogger.Log("PlaySongPanelsIntro songPanel[" + i + "]", LogType.Debug);
                    // TODO: Play slide sound file
                    var newPos = ResultsAssets.GetSongPanelPosition(i, false);
                    yield return MoveSongPanel(songPanels[i], newPos);
                }

                isSongsIntroAnimating = false;
                activeIntroCoroutine = null;

                TakeScreenshot(DaniResultScreen.Songs);

                autoAdvanceCoroutine = Plugin.Instance.StartCoroutine(AutoAdvanceTimer());
            }

            private IEnumerator PlayResultsIntro()
            {
                ModLogger.Log("PlayResultsIntro", LogType.Debug);
                isPlayResultsAnimating = true;
                yield return MoveOverSeconds(PlayRecordParent, new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0)), 0.1f);
                isPlayResultsAnimating = false;

                TakeScreenshot(DaniResultScreen.PlayResults);
            }


            private IEnumerator PlayAdvancementIntro()
            {
                ModLogger.Log("PlayAdvancementIntro", LogType.Debug);
                isPlayResultsAnimating = true;
                yield return MoveOverSeconds(PlayRecordParent, new Vector2(0, 44) + AssetUtility.GetPositionFrom1080p(new Vector2(337, 0)), 0.1f);
                isPlayResultsAnimating = false;

                TakeScreenshot(DaniResultScreen.Advancement);
            }

            public IEnumerator MoveSongPanel(GameObject objectToMove, Vector3 endLocalPos, float easingFactor = 0.3f, float threshold = 0.5f)
            {
                // Make sure we use localPosition throughout
                Vector3 currentPos = objectToMove.transform.localPosition;

                // Loop until the panel is close enough to its final resting spot
                while (Vector3.Distance(currentPos, endLocalPos) > threshold)
                {
                    // Calculate the distance left to go
                    Vector3 distanceRemaining = endLocalPos - currentPos;

                    // Move a percentage of that remaining distance
                    // Frame-rate independent easing scales the factor by DeltaTime relative to a 60fps frame target
                    float frameScaledEase = easingFactor * (Time.deltaTime / 0.01666f);

                    // Clamp it so it never overshoots if the game hitches/stutters
                    frameScaledEase = Mathf.Clamp01(frameScaledEase);

                    currentPos += distanceRemaining * frameScaledEase;
                    objectToMove.transform.localPosition = currentPos;

                    yield return null; // Wait for the next frame
                }

                // Snap to the exact pixel destination when the loop finishes
                objectToMove.transform.localPosition = endLocalPos;
            }

            private IEnumerator AutoAdvanceTimer()
            {
                ModLogger.Log("AutoAdvanceTimer", LogType.Debug);
                yield return new WaitForSeconds(autoAdvanceDelay);

                // Double-check we are still on the Songs screen before moving
                if (CurrentScreen == DaniResultScreen.Songs)
                {
                    TransitionToScreen(DaniResultScreen.PlayResults);
                }
                autoAdvanceCoroutine = null;
            }

            private bool IsAutoScreenshotLoaded()
            {
                try
                {
                    Assembly loadedAssembly = Assembly.Load("com.DB.TDMX.AutoScreenshot");
                    return loadedAssembly != null;
                }
                catch
                {
                    return false;
                }
            }

            private void TakeScreenshot(DaniResultScreen currentScreen)
            {
                if (!autoScreenshotLoaded || screenshotsTaken[currentScreen])
                {
                    return;
                }

                // Call the isolated wrapper method
                Plugin.Instance.StartCoroutine(TakeScreenshotFunction(currentScreen));
            }

            [MethodImpl(MethodImplOptions.NoInlining)]
            private IEnumerator TakeScreenshotFunction(DaniResultScreen currentScreen)
            {
                // How do I want to name the image?
                // I know I want it in the "Dani Dojo" folder at least
                // That means I probably don't need to put "DaniDojo" in the image file itself I guess?
                // The images get the current datetime as a prefix
                // 2026-07-10 10-14-25_
                // I could do SeriesTitle_CourseTitle
                // There's a bit of an issue with language then, but whatever, I could also just put nothing (except result screen too)
                // 2026-07-10 10-14-25_Nijiiro 2026_10th dan_Songs

                yield return new WaitForEndOfFrame();

                AutoScreenshot.Screenshot.TakeScreenshot(currentCourse.Parent.Title + "_" + currentCourse.Title + "_" + (int)currentScreen + currentScreen, "DaniDojo");
                screenshotsTaken[currentScreen] = true;
            }
        }
    }
}
