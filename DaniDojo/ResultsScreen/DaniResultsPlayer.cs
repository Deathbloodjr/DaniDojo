using DaniDojo.Assets;
using DaniDojo.Data;
using DaniDojo.Managers;
using DaniDojo.Patches;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.ResultsScreen
{
    enum DaniResultScreen
    {
        Songs,
        PlayResults,
        Advancement,
    }

    internal class DaniResultsPlayer : MonoBehaviour
    {
#if IL2CPP
            static DaniResultsPlayer() => ClassInjector.RegisterTypeInIl2Cpp<DaniResultsPlayer>();
#endif
        DaniResultScreen CurrentScreen;

        GameObject ResultsParent;
        GameObject PlayResultsParent;

        GameObject donCommon;
        GameObject playerName;

        DaniCourse currentCourse;

        private SongsView songsView;
        private PlayResultsView playResultsView;
        private AdvancementView advancementView;

        private Dictionary<DaniResultScreen, bool> introAnimationPlayed = new Dictionary<DaniResultScreen, bool>();
        private Dictionary<DaniResultScreen, bool> screenshotsTaken = new Dictionary<DaniResultScreen, bool>();

        private bool autoScreenshotLoaded = false;
        private bool changingScene = false;


        float inputBuffer = 0.25f;
        float currentBuffer = 0;


        public void Start()
        {
            ResultsParent = this.gameObject;
            currentCourse = DaniPlayManager.GetCurrentCourse();

            DaniSoundManager.PlayBgm("bgm_daniresult_primal_loop.bin");

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
            songsView.StartIntro();
        }

        private void InitializeAssets()
        {
            var bg = ResultsAssets.CreateBg(ResultsParent);
            var currentPlay = DaniPlayManager.GetCurrentPlay();

            // Attach and set up the independent SongsView script layout
            songsView = bg.AddComponent<SongsView>();
            songsView.Initialize(this, bg, currentCourse, currentPlay);

            PlayResultsParent = ResultsAssets.CreatePlayRecordBg(bg);
            playResultsView = PlayResultsParent.AddComponent<PlayResultsView>();
            playResultsView.Initialize(this, PlayResultsParent, currentCourse, currentPlay);



            var danCourseIcon = ResultsAssets.CreateCourseIcon(bg, currentCourse);
            var danCourseTitle = ResultsAssets.CreateCourseTitle(bg, currentCourse);

            donCommon = Instantiate(DaniDojoSongSelect.donCommonObject);
            playerName = Instantiate(DaniDojoSongSelect.playerNameObject);
            donCommon.transform.SetParent(bg.transform);
            playerName.transform.SetParent(bg.transform);

            donCommon.transform.localPosition = new Vector3(202, 289, 0);
            playerName.transform.localPosition = new Vector3(194, 120, 0);

            var danResultAsset = ResultsAssets.CreateDanResult(bg, currentPlay);
        }

        private void TransitionToScreen(DaniResultScreen targetScreen)
        {
            // There should never be an animation playing when this is called

            // These are the possible actions, where Songs is 1, PlayResults is 2, and Advancement is 3
            // 1 -> 2: Move 2's GameObject over to the left
            // 2 -> 3: Fade in 3's GameObject
            // 3 -> 2: Fade out 3's GameObject
            // 2 -> 1: Move 2's GameObject over to the right

            if (CurrentScreen == DaniResultScreen.Songs &&
                targetScreen == DaniResultScreen.PlayResults)
            {
                playResultsView.ScreenEnter();
            }
            else if (CurrentScreen == DaniResultScreen.PlayResults &&
                     targetScreen == DaniResultScreen.Advancement)
            {
                advancementView.ScreenEnter();
            }
            else if (CurrentScreen == DaniResultScreen.Advancement &&
                     targetScreen == DaniResultScreen.PlayResults)
            {
                advancementView.ScreenExit();
            }
            else if (CurrentScreen == DaniResultScreen.PlayResults &&
                     targetScreen == DaniResultScreen.Songs)
            {
                playResultsView.ScreenExit();
            }
            else
            {
                // There shouldn't be anything else
                ModLogger.Log("Error changing screens: Target screen not found", LogType.Error);
            }

            CurrentScreen = targetScreen;
        }


        public void Update()
        {
            if (!changingScene)
            {
                GetInput();
            }
        }

        private void GetInput()
        {
            if (currentBuffer > 0)
            {
                currentBuffer -= Time.deltaTime;
                if (currentBuffer < 0)
                {
                    currentBuffer = 0;
                }
            }

            ControllerManager.Dir dir = GetInputDirection();
            bool okDown = GetOkDown();

            if (!okDown && dir == ControllerManager.Dir.None)
            {
                currentBuffer = 0f;
                return;
            }

            if (currentBuffer > 0)
            {
                return;
            }

            switch (CurrentScreen)
            {
                case DaniResultScreen.Songs:
                    HandleSongsInput(okDown, dir);
                    break;
                case DaniResultScreen.PlayResults:
                    HandlePlayResultsInput(okDown, dir);
                    break;
                case DaniResultScreen.Advancement:
                    HandleAdvancementInput(okDown, dir);
                    break;
            }
        }

        public void HandleAutoAdvance()
        {
            if (CurrentScreen == DaniResultScreen.Songs)
            {
                playResultsView.StartIntro();
                CurrentScreen = DaniResultScreen.PlayResults;
            }
            else if (CurrentScreen == DaniResultScreen.PlayResults)
            {
                advancementView.StartIntro();
                CurrentScreen = DaniResultScreen.Advancement;
            }
        }

        private void HandleSongsInput(bool okDown, ControllerManager.Dir dir)
        {
            if (songsView.IsInIntro() && okDown)
            {
                TriggerBuffer();
                songsView.StopIntro();
                return;
            }

            if (songsView.IsAutoAdvanceEnabled() && (okDown || dir == ControllerManager.Dir.Right))
            {
                TriggerBuffer();
                songsView.StopAutoAdvance();
                return;
            }

            if (dir == ControllerManager.Dir.Up ||
                dir == ControllerManager.Dir.Down)
            {
                TriggerBuffer();
                songsView.HandleUpDown(dir);
                return;
            }

            if (okDown || dir == ControllerManager.Dir.Right)
            {
                TriggerBuffer();
                TransitionToScreen(DaniResultScreen.PlayResults);
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                return;
            }
        }

        private void HandlePlayResultsInput(bool okDown, ControllerManager.Dir dir)
        {
            if (playResultsView.IsInIntro() && okDown)
            {
                TriggerBuffer();
                playResultsView.StopIntro();
                return;
            }

            if (playResultsView.IsAutoAdvanceEnabled() && (okDown || dir == ControllerManager.Dir.Right))
            {
                TriggerBuffer();
                playResultsView.StopAutoAdvance();
                return;
            }

            if (dir == ControllerManager.Dir.Left)
            {
                TriggerBuffer();
                TransitionToScreen(DaniResultScreen.Songs);
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("katsu", false, false);
                return;
            }

            // This is going to be slightly incorrect, as I'm skipping the advancement screen since it isn't implemented yet
            // There's going to be a check somewhere to state if there will be an advancement screen, as it's only there if the player reached a new clear on the given course
            // So for now, we can just say that it's always false
            bool isAdvancement = false;

            if (isAdvancement && okDown || dir == ControllerManager.Dir.Right)
            {
                TriggerBuffer();
                TransitionToScreen(DaniResultScreen.Advancement);
            }

            if (!isAdvancement && okDown)
            {
                ReturnToCourseSelectScene();
                return;
            }
        }

        private void HandleAdvancementInput(bool okDown, ControllerManager.Dir dir)
        {
            // I auto-piloted and added this here too
            // But I think I'll be making this unskippable maybe
            // It's a fairly quick animation, so it shouldn't be too big a deal
            //if (advancementView.IsInIntro() && okDown)
            //{
            //    TriggerBuffer();
            //    advancementView.StopIntro();
            //    return;
            //}

            if (dir == ControllerManager.Dir.Left)
            {
                TriggerBuffer();
                TransitionToScreen(DaniResultScreen.PlayResults);
                return;
            }

            if (okDown || dir == ControllerManager.Dir.Right)
            {
                TriggerBuffer();
                ReturnToCourseSelectScene();
                return;
            }
        }

        private void ReturnToCourseSelectScene()
        {
            TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MySoundManager.CommonSePlay("don", false, false);
            DaniPlayManager.SetStartResult(false);
            changingScene = true;
            DaniDojoDaniCourseSelect.ChangeSceneDaniDojo();
        }




        private void TriggerBuffer()
        {
            currentBuffer = inputBuffer;
        }

        private bool GetOkDown()
        {
            return TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetOkDown(ControllerManager.ControllerPlayerNo.Player1);
        }

        private ControllerManager.Dir GetInputDirection()
        {
            var dir = TaikoSingletonMonoBehaviour<ControllerManager>.Instance.GetDirectionButton(ControllerManager.ControllerPlayerNo.Player1, ControllerManager.Prio.None, false);
            return dir;
        }


        #region Screenshots
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

        public void TakeScreenshot(DaniResultScreen currentScreen)
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
        #endregion
    }
}
