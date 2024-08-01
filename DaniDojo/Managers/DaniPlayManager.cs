using Blittables;
using DaniDojo.Assets;
using DaniDojo.Data;
using DaniDojo.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TaikoCoreTypes;

namespace DaniDojo.Managers
{
    internal class DaniPlayManager
    {
        static CurrentPlayData currentPlay;
        static DaniCourse currentCourse;
        static bool IsInDan;
        static bool StartResult;

        static public bool HasFailed()
        {
            // Various border checks here, still not sure how I want to do that
            //var currentRank = CalculateRankBorders(currentCourse, currentPlay.PlayData);

            var songNum = GetCurrentSongNumber();
            //Plugin.LogInfo(LogType.Info, "HasFailed: songZNum " + songNum, 2);

            for (int i = 0; i < currentCourse.Borders.Count; i++)
            {
                var border = currentCourse.Borders[i];
                //Plugin.LogInfo(LogType.Info, "HasFailed: Border " + border.BorderType.ToString(), 2);
                var values = GetBorderPlayResults(border, currentPlay.PlayData);
                if (border.IsTotal)
                {
                    if (border.BorderType == BorderType.Oks ||
                        border.BorderType == BorderType.Bads)
                    {
                        //Plugin.LogInfo(LogType.Info, "HasFailed: ReqReqs.Sum(): " + border.RedReqs.Sum(), 2);
                        //Plugin.LogInfo(LogType.Info, "HasFailed: Value.Sum(): " + values.Sum(), 2);
                        if (values.Sum() >= border.RedReqs.Sum())
                        {
                            return true;
                        }
                    }
                }
                else // Per song requirements
                {
                    //Plugin.LogInfo(LogType.Info, "HasFailed: RedReqs[songNum]: " + border.RedReqs[songNum], 2);
                    //Plugin.LogInfo(LogType.Info, "HasFailed: values[songNum]: " + values[songNum], 2);
                    if (border.BorderType == BorderType.Oks ||
                        border.BorderType == BorderType.Bads)
                    {
                        if (values[songNum] >= border.RedReqs[songNum])
                        {
                            return true;
                        }
                    }
                    else // goods, drumroll, totalhit, etc
                    {
                        if (values[songNum] < border.RedReqs[songNum])
                        {
                            return true;
                        }
                    }
                    // Only want to check through the current song
                    //for (int j = 0; j < songNum + 1; j++)
                    //{
                    //    if (border.BorderType == BorderType.Oks ||
                    //        border.BorderType == BorderType.Bads)
                    //    {
                    //        if (values[i] >= border.RedReqs[i])
                    //        {
                    //            return true;
                    //        }
                    //    }
                    //    else // goods, drumroll, totalhit, etc
                    //    {
                    //        if (values[i] < border.RedReqs[i])
                    //        {
                    //            return true;
                    //        }
                    //    }
                    //}
                }
            }

            return false;
        }

        internal static bool CheckStartResult()
        {
            return StartResult;
        }

        static public void SetStartResult(bool value)
        {
            StartResult = value;
        }

        static public bool CheckIsInDan()
        {
            //Plugin.LogInfo(LogType.Info, "CheckIsInDan: " + IsInDan, 5);
            return IsInDan;
        }

        static public void StartDanPlay(DaniCourse course)
        {
            Plugin.LogInfo(LogType.Info, "Start Course " + course.Parent.Title + " - " + course.Title, 0);
            IsInDan = true;
            StartResult = false;

            currentCourse = course;
            currentPlay = new CurrentPlayData(course);

            SoulGaugeManager.InitializeSoulGaugeData(course);

            EnsoAssets.SetRainbowStartTime();

            // I'm not sure if I'd want to actually begin the song here
            // Probably wouldn't want to do that
        }

        static public void RestartDanPlay()
        {
            if (IsInDan)
            {
                //Plugin.LogInfo(LogType.Info, "Restart Course", 1);
                currentPlay = new CurrentPlayData(currentCourse);
                SoulGaugeManager.InitializeSoulGaugeData(currentCourse);
            }
        }

        static public void LeaveDanPlay()
        {
            if (IsInDan)
            {
                //Plugin.LogInfo(LogType.Info, "Leave Course", 1);
                IsInDan = false;
                StartResult = false;
                // I don't know if I need to reset anything for currentPlay
                DaniDojoDaniCourseSelect.ChangeSceneDaniDojo();
            }
        }

        static public void EndDanPlay()
        {
            //Plugin.LogInfo(LogType.Info, "End Course", 1);
            IsInDan = false;
            StartResult = true;
            currentPlay.PlayData.SoulGauge = currentPlay.CurrentSoulGauge;
            currentPlay.PlayData.RankCombo = new DaniRankCombo(CalculateRankBorders(currentCourse, currentPlay.PlayData), CalculateComboRank(currentPlay.PlayData));
            SaveDataManager.AddPlayData(currentCourse.Hash, currentPlay.PlayData);
            SaveDataManager.SaveDaniSaveData();
            SaveDataManager.LoadSaveData();
        }

        static public DaniSongData GetSongData()
        {
            return currentCourse.Songs[currentPlay.CurrentSongIndex];
        }

        /// <summary>
        /// Advances the song if able, returns bool on if it is able to advance.
        /// </summary>
        /// <returns>Whether it advanced or not.</returns>
        static public bool AdvanceSong()
        {
            //Plugin.LogInfo(LogType.Info, "Advance Song", 1);
            currentPlay.PlayData.SongReached = currentPlay.CurrentSongIndex + 1;
            if (HasFailed() || IsFinalSong())
            {
                EndDanPlay();
                return false;
            }
            else
            {
                currentPlay.CurrentSongIndex++;
                currentPlay.CurrentSongCombo = 0;
                return true;
            }
        }

        internal static bool IsFinalSong()
        {
            return currentPlay.CurrentSongIndex == currentCourse.Songs.Count - 1;
        }

        static public int GetCurrentCombo()
        {
            return currentPlay.CurrentCombo;
        }

        /// <summary>
        /// Gets the current song number.
        /// </summary>
        /// <returns>The current song number, 0 indexed</returns>
        static public int GetCurrentSongNumber()
        {
            return currentPlay.CurrentSongIndex;
        }

        static public DaniCourse GetCurrentCourse()
        {
            return currentCourse;
        }

        static public PlayData GetCurrentPlay()
        {
            return currentPlay.PlayData;
        }

        static public List<DaniBorder> GetCurrentBorderOfType(BorderType borderType)
        {
            List<DaniBorder> borders = new List<DaniBorder>();
            var indexes = GetIndexexOfBorder(borderType);
            for (int i = 0; i < indexes.Count; i++)
            {
                borders.Add(currentCourse.Borders[indexes[i]]);
            }
            return borders;
        }

        static public List<DaniBorder> GetCurrentCourseBorders()
        {
            return currentCourse.Borders;
        }


        static public List<int> GetIndexexOfBorder(BorderType borderType)
        {
            List<int> indexes = new List<int>();
            for (int i = 0; i < currentCourse.Borders.Count; i++)
            {
                if (currentCourse.Borders[i].BorderType == borderType)
                {
                    indexes.Add(i);
                }
            }
            return indexes;
        }

        static public List<int> GetCurrentBorderValue(BorderType borderType)
        {
            return GetCurrentBorderValue(GetCurrentBorderOfType(borderType));
        }
        static public List<int> GetCurrentBorderValue(List<DaniBorder> borders)
        {
            List<int> results = new List<int>();
            for (int i = 0; i < borders.Count; i++)
            {
                var allResults = GetBorderPlayResults(borders[i], currentPlay.PlayData);
                if (borders[i].IsTotal && allResults.Count > 0)
                {
                    results.Add(allResults[0]);
                }
                else if (allResults.Count > currentPlay.CurrentSongIndex)
                {
                    results.Add(allResults[currentPlay.CurrentSongIndex]);
                }
                else
                {
                    results.Add(0);
                }
            }
            return results;
        }

        static public int GetCurrentBorderRequirement(DaniBorder border)
        {
            if (border.IsTotal)
            {
                return border.RedReqs[0];
            }
            else
            {
                return border.RedReqs[currentPlay.CurrentSongIndex];
            }
        }

        static public int GetCurrentGoldBorderRequirement(DaniBorder border)
        {
            if (border.IsTotal)
            {
                return border.GoldReqs[0];
            }
            else
            {
                return border.GoldReqs[currentPlay.CurrentSongIndex];
            }
        }

        #region UpdatePlayData
        static public void AddHitResult(HitResultInfo info)
        {
            int hitResult = info.hitResult;
            if (info.onpuType == (int)OnpuTypes.Don || info.onpuType == (int)OnpuTypes.Do || info.onpuType == (int)OnpuTypes.Ko || info.onpuType == (int)OnpuTypes.Katsu || info.onpuType == (int)OnpuTypes.Ka
                || info.onpuType == (int)OnpuTypes.DaiDon || info.onpuType == (int)OnpuTypes.DaiKatsu
                || info.onpuType == (int)OnpuTypes.WDon || info.onpuType == (int)OnpuTypes.WKatsu)
            {
                if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
                {
                    AddBad();
                }
                else if (hitResult == (int)HitResultTypes.Ka)
                {
                    AddOk();
                }
                else if (hitResult == (int)HitResultTypes.Ryo)
                {
                    AddGood();
                }
            }
            else if (info.onpuType == (int)OnpuTypes.Renda || info.onpuType == (int)OnpuTypes.DaiRenda || info.onpuType == (int)OnpuTypes.Imo || info.onpuType == (int)OnpuTypes.GekiRenda)
            {
                if (hitResult == (int)HitResultTypes.Ryo)
                {
                    AddDrumroll();
                }
            }
        }

        static public void AddHitResultFromEachPlayer(EachPlayer player)
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            if (songPlayData.Goods != (int)player.countRyo)
            {
                songPlayData.Goods = (int)player.countRyo;
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Goods);
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                AddCombo();
            }
            if (songPlayData.Oks != (int)player.countKa)
            {
                songPlayData.Oks = (int)player.countKa;
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Oks);
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
                AddCombo();
            }
            if (songPlayData.Bads != (int)player.countFuka)
            {
                songPlayData.Bads = (int)player.countFuka;
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Bads);
                currentPlay.CurrentSongCombo = 0;
                currentPlay.CurrentCombo = 0;
            }
            if (songPlayData.Drumroll != (int)player.countRenda)
            {
                songPlayData.Drumroll = (int)player.countRenda;
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Drumroll);
                DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.TotalHits);
            }
        }

        static void AddGood()
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            songPlayData.Goods++;
            AddCombo();
            //AddScore(100); // I need to figure out how to store score data like this, or how to even find it.
        }

        static void AddOk()
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            songPlayData.Oks++;
            AddCombo();
            //AddScore(100 / 2); // I need to figure out how to store score data like this, or how to even find it.
        }

        static void AddCombo()
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            currentPlay.CurrentSongCombo++;
            currentPlay.CurrentCombo++;
            currentPlay.PlayData.MaxCombo = Math.Max(currentPlay.PlayData.MaxCombo, currentPlay.CurrentCombo);
            songPlayData.Combo = Math.Max(currentPlay.CurrentSongCombo, songPlayData.Combo);
            DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Combo);
        }

        static public void AddScore(int points)
        {
            //Plugin.LogInfo(LogType.Info, "Score added: " + points, 2);

            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            songPlayData.Score += points;
            DaniDojoAssets.EnsoAssets.UpdateRequirementBar(BorderType.Score);
        }

        static void AddBad()
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            songPlayData.Bads++;
            currentPlay.CurrentSongCombo = 0;
            currentPlay.CurrentCombo = 0;

        }

        static void AddDrumroll()
        {
            var songPlayData = currentPlay.PlayData.SongPlayData[currentPlay.CurrentSongIndex];
            songPlayData.Drumroll++;
            //AddScore(100); // This won't change
        }



        #endregion


        #region BorderChecks

        // I might need some actual song data to go in here as well
        // To tell how many notes are remaining in the song/course, that can be a way to fail
        static public DaniRank CalculateRankBorders(DaniCourse course, PlayData play)
        {
            //Plugin.LogInfo(LogType.Info, "CalculateRankBorders Start", 2);
            if (course == null)
            {
                return DaniRank.None;
            }
            //Plugin.LogInfo(LogType.Info, "CalculateRankBorders: Series: " + course.Parent.Title, 2);
            //Plugin.LogInfo(LogType.Info, "CalculateRankBorders: Course: " + course.Title, 2);
            DaniRank minRank = DaniRank.GoldClear;
            bool isCheckTotal = play.SongReached == course.Songs.Count;
            for (int i = 0; i < course.Borders.Count; i++)
            {
                // Skip SoulGauge
                // Skip if the play hasn't reached the final song yet, and the border IsTotal
                if (course.Borders[i].IsTotal && !isCheckTotal)
                {
                    continue;
                }
                minRank = (DaniRank)Math.Min((int)minRank, (int)CalculateBorder(course.Borders[i], play));
                //Plugin.LogInfo(LogType.Info, "CalculateRankBorders: Border " + i + " (" + course.Borders[i].BorderType.ToString() + "): " + minRank.ToString(), 2);
                if (minRank == DaniRank.None)
                {
                    return DaniRank.None;
                }
            }
            return minRank;
        }

        static public DaniRank CalculateBorderAtIndex(int index)
        {
            if (index > currentCourse.Borders.Count)
            {
                return DaniRank.None;
            }
            var border = currentCourse.Borders[index];
            return CalculateBorder(border, currentPlay.PlayData);
        }

        static public DaniRank CalculateBorderMidSong(DaniBorder border)
        {
            var playValues = GetBorderPlayResults(border, currentPlay.PlayData);
            if (playValues.Count > border.RedReqs.Count ||
                playValues.Count > border.GoldReqs.Count)
            {
                //Plugin.LogInfo(LogType.Error, "CalculateBorder Error: Too many values in PlayValues compared to Border Requirements.");
                return DaniRank.None;
            }

            DaniRank tmpRank = DaniRank.GoldClear;

            int songNum = GetCurrentSongNumber();

            if (border.IsTotal)
            {
                songNum = 0;
            }

            if (border.BorderType == BorderType.Oks ||
                border.BorderType == BorderType.Bads)
            {
                if (playValues[songNum] < border.GoldReqs[songNum])
                {
                    tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.GoldClear);
                }
                else if (playValues[songNum] < border.RedReqs[songNum])
                {
                    tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.RedClear);
                }
                else
                {
                    return DaniRank.None;
                    //tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.None);
                }
            }
            else
            {
                if (playValues[songNum] >= border.GoldReqs[songNum])
                {
                    tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.GoldClear);
                }
                else if (playValues[songNum] >= border.RedReqs[songNum])
                {
                    tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.RedClear);
                }
                else
                {
                    return DaniRank.None;
                    //tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.None);
                }
            }
            return tmpRank;
        }

        static public DaniRank CalculateBorder(DaniBorder border)
        {
            return CalculateBorder(border, currentPlay.PlayData);
        }

        static DaniRank CalculateBorder(DaniBorder border, PlayData play)
        {
            var playValues = GetBorderPlayResults(border, play);
            if (playValues.Count > border.RedReqs.Count ||
                playValues.Count > border.GoldReqs.Count)
            {
                //Plugin.LogInfo(LogType.Error, "CalculateBorder Error: Too many values in PlayValues compared to Border Requirements.");
                return DaniRank.None;
            }

            DaniRank tmpRank = DaniRank.GoldClear;

            //Plugin.LogInfo(LogType.Info, "CalculateBorder: BorderType: " + border.BorderType.ToString(), 2);
            for (int i = 0; i < playValues.Count; i++)
            {
                //Plugin.LogInfo(LogType.Info, "CalculateBorder: playValues[" + i + "]: " + playValues[i].ToString(), 2);
                //Plugin.LogInfo(LogType.Info, "CalculateBorder: border.RedReqs[" + i + "]: " + border.RedReqs[i].ToString(), 2);
                //Plugin.LogInfo(LogType.Info, "CalculateBorder: border.GoldReqs[" + i + "]: " + border.GoldReqs[i].ToString(), 2);
                if (border.BorderType == BorderType.Oks ||
                    border.BorderType == BorderType.Bads)
                {
                    if (playValues[i] < border.GoldReqs[i])
                    {
                        tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.GoldClear);
                    }
                    else if (playValues[i] < border.RedReqs[i])
                    {
                        tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.RedClear);
                    }
                    else
                    {
                        return DaniRank.None;
                    }
                }
                else
                {
                    if (playValues[i] >= border.GoldReqs[i])
                    {
                        tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.GoldClear);
                    }
                    else if (playValues[i] >= border.RedReqs[i])
                    {
                        tmpRank = (DaniRank)Math.Min((int)tmpRank, (int)DaniRank.RedClear);
                    }
                    else
                    {
                        return DaniRank.None;
                    }
                }
            }
            return tmpRank;
        }

        static public List<int> GetBorderPlayResults(DaniBorder border)
        {
            return GetBorderPlayResults(border, currentPlay.PlayData);
        }

        static public List<int> GetBorderPlayResults(DaniBorder border, PlayData play)
        {
            List<int> playResults = new List<int>();
            if (border.IsTotal)
            {
                switch (border.BorderType)
                {
                    case BorderType.SoulGauge: playResults.Add(play.SoulGauge); break; // Soul Gauge will take some time to figure out properly
                    case BorderType.Goods: playResults.Add(play.SongPlayData.Sum((x) => x.Goods)); break;
                    case BorderType.Oks: playResults.Add(play.SongPlayData.Sum((x) => x.Oks)); break;
                    case BorderType.Bads: playResults.Add(play.SongPlayData.Sum((x) => x.Bads)); break;
                    case BorderType.Combo: playResults.Add(play.MaxCombo); break;
                    case BorderType.Drumroll: playResults.Add(play.SongPlayData.Sum((x) => x.Drumroll)); break;
                    case BorderType.Score: playResults.Add(play.SongPlayData.Sum((x) => x.Score)); break;
                    case BorderType.TotalHits: playResults.Add(play.SongPlayData.Sum((x) => x.Goods + x.Oks + x.Drumroll)); break;
                }
            }
            else
            {
                for (int i = 0; i < play.SongPlayData.Count; i++)
                {
                    switch (border.BorderType)
                    {
                        case BorderType.Goods: playResults.Add(play.SongPlayData[i].Goods); break;
                        case BorderType.Oks: playResults.Add(play.SongPlayData[i].Oks); break;
                        case BorderType.Bads: playResults.Add(play.SongPlayData[i].Bads); break;
                        case BorderType.Combo: playResults.Add(play.SongPlayData[i].Combo); break;
                        case BorderType.Drumroll: playResults.Add(play.SongPlayData[i].Drumroll); break;
                        case BorderType.Score: playResults.Add(play.SongPlayData[i].Score); break;
                        case BorderType.TotalHits: playResults.Add(play.SongPlayData[i].Goods + play.SongPlayData[i].Oks + play.SongPlayData[i].Drumroll); break;
                    }
                }
            }
            return playResults;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="border"></param>
        /// <param name="play"></param>
        /// <param name="songNumber">The song number, 0 indexed</param>
        /// <param name="remainingNotes"></param>
        /// <param name="remainingDrumrollTime"></param>
        /// <returns></returns>
        static public BorderBarData GetBorderBarData(DaniBorder border, PlayData play, int songNumber = -1, int remainingNotes = -1, float remainingDrumrollTime = -1f, bool endOfSong = false, bool endOfCourse = false)
        {
            if (border.BorderType == BorderType.SoulGauge)
            {
                return null;
            }

            if (!border.IsTotal && songNumber == -1)
            {
                return null;
            }

            BorderBarData data = new BorderBarData();

            int redReq = (border.IsTotal ? border.RedReqs[0] : border.RedReqs[songNumber]);
            int goldReq = (border.IsTotal ? border.GoldReqs[0] : border.GoldReqs[songNumber]);
            var playValues = GetBorderPlayResults(border, play);
            int playValue = (border.IsTotal ? playValues[0] : playValues[songNumber]);

            float ratio = (float)playValue / (float)redReq;
            float ratioMinusOne = (float)(playValue - 1) / (float)redReq;
            float goldRatio = 0;
            float goldRatioMinusOne = 0;
            if (ratio > 1)
            {
                goldRatio = (playValue - redReq) / (float)(goldReq - redReq);
                goldRatioMinusOne = ((playValue - 1) - redReq) / (float)(goldReq - redReq);
            }

            data.State = BorderBarState.Grey;

            if (border.BorderType != BorderType.Oks && border.BorderType != BorderType.Bads)
            {
                data.FillRatio = (int)(ratio * 100);
                if (data.FillRatio == 0 && playValue > 0)
                {
                    data.FillRatio = 1;
                }

                data.PlayValue = playValue;

                List<(float threshold, BorderBarState state, BorderBarFlashState flashState)> GoldThresholds = new List<(float threshold, BorderBarState state, BorderBarFlashState flashState)>()
                {
                    (1f, BorderBarState.Rainbow, BorderBarFlashState.None),
                    (0.66f, BorderBarState.Pink, BorderBarFlashState.WhiteFast),
                    (0.33f, BorderBarState.Pink, BorderBarFlashState.WhiteSlow),
                    (0f, BorderBarState.Pink, BorderBarFlashState.None),
                };

                List<(float threshold, BorderBarState state, BorderBarFlashState flashState)> Thresholds = new List<(float threshold, BorderBarState state, BorderBarFlashState flashState)>()
                {
                    (1f, BorderBarState.Pink, BorderBarFlashState.None),
                    (0.95f, BorderBarState.Yellow, BorderBarFlashState.WhiteSlow),
                    (0.5f, BorderBarState.Yellow, BorderBarFlashState.None),
                    (0.00001f, BorderBarState.DarkYellow, BorderBarFlashState.None) // Anything greater than 0, but these checks are for greater than or equal to, so that sucks
                };

                if (goldRatio > 0)
                {
                    for (int i = 0; i < GoldThresholds.Count; i++)
                    {
                        if (goldRatio >= GoldThresholds[i].threshold)
                        {
                            data.State = GoldThresholds[i].state;
                            data.FlashState = GoldThresholds[i].flashState;
                            if (goldRatioMinusOne < GoldThresholds[i].threshold)
                            {
                                data.StateChanged = true;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Thresholds.Count; i++)
                    {
                        if (ratio >= Thresholds[i].threshold)
                        {
                            data.State = Thresholds[i].state;
                            data.FlashState = Thresholds[i].flashState;
                            if (ratioMinusOne < Thresholds[i].threshold)
                            {
                                data.StateChanged = true;
                            }
                            break;
                        }
                    }
                }
                if ((border.IsTotal && endOfCourse) ||
                    (!border.IsTotal && (endOfSong || endOfCourse)))
                {
                    if (data.PlayValue < redReq)
                    {
                        data.Failed = true;
                    }
                }
            }
            else // OKs or Bad requirements
            {
                data.FillRatio = (int)((1 - ratio) * 100);
                if (data.FillRatio == 0 && playValue > 0)
                {
                    data.FillRatio = 1;
                }

                data.PlayValue = Math.Max((redReq - playValue), 0); // Don't want it to go negative

                List<(float threshold, BorderBarState state, BorderBarFlashState flashState)> Thresholds = new List<(float threshold, BorderBarState state, BorderBarFlashState flashState)>()
                {
                    (1f, BorderBarState.Grey, BorderBarFlashState.None),
                    (0.8f, BorderBarState.Orange, BorderBarFlashState.BlackSlow),
                    (0.7f, BorderBarState.Orange, BorderBarFlashState.None),
                    (0.5f, BorderBarState.Yellow, BorderBarFlashState.None),
                    (0f, BorderBarState.Pink, BorderBarFlashState.None),
                };

                for (int i = 0; i < Thresholds.Count; i++)
                {
                    if (ratio >= Thresholds[i].threshold)
                    {
                        data.State = Thresholds[i].state;
                        data.FlashState = Thresholds[i].flashState;
                        if (ratioMinusOne < Thresholds[i].threshold)
                        {
                            data.StateChanged = true;
                        }
                        break;
                    }
                }

                if (playValue < goldReq)
                {
                    if ((border.IsTotal && endOfCourse) ||
                        (!border.IsTotal && (endOfSong || endOfCourse)))
                    {
                        data.State = BorderBarState.Rainbow;
                        data.FlashState = BorderBarFlashState.None;
                    }
                }

                if (playValue >= redReq)
                {
                    data.Failed = true;
                }
            }
            return data;
        }

        static public DaniCombo CalculateComboRank(PlayData play)
        {
            //Plugin.LogInfo(LogType.Info, "CalculateComboRank Start", 2);
            if (play.SongReached != play.SongPlayData.Count)
            {
                //Plugin.LogInfo(LogType.Info, "CalculateComboRank DaniCombo.None", 2);
                return DaniCombo.None;
            }
            int numOks = 0;
            int numBads = 0;
            for (int i = 0; i < play.SongPlayData.Count; i++)
            {
                numOks += play.SongPlayData[i].Oks;
                numBads += play.SongPlayData[i].Bads;
            }

            if (numOks == 0 && numBads == 0)
            {
                //Plugin.LogInfo(LogType.Info, "CalculateComboRank DaniCombo.Rainbow", 2);
                return DaniCombo.Rainbow;
            }
            else if (numBads == 0)
            {
                //Plugin.LogInfo(LogType.Info, "CalculateComboRank DaniCombo.Gold", 2);
                return DaniCombo.Gold;
            }
            else
            {
                //Plugin.LogInfo(LogType.Info, "CalculateComboRank DaniCombo.Silver", 2);
                return DaniCombo.Silver;
            }
        }
    }
}
