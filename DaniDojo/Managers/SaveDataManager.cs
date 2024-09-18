using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DaniDojo.Data;
using LightWeightJsonParser;

namespace DaniDojo.Managers
{
    internal class SaveDataManager
    {
#if DEBUG
        const string SaveFileName = "DebugDaniSave.json";
        const string TmpSaveFileName = "DebugTmpSave.json";
#else
        const string SaveFileName = "DaniSave.json";
        const string TmpSaveFileName = "TmpSave.json";
        const string OldSaveFileName = "dansave.json";
#endif

        static DaniSaveData SaveData { get; set; }

        #region Loading
        public static void LoadSaveData()
        {
            Plugin.LogInfo(LogType.Info, "LoadSaveData Start", 1);
            SaveData = new DaniSaveData(); // I'm not sure if this line is actually needed, or even detrimental
            SaveData = LoadSaveData(Plugin.Instance.ConfigDaniDojoSaveLocation.Value);
            Plugin.LogInfo(LogType.Info, "LoadSaveData Finished", 1);
        }

        static DaniSaveData LoadSaveData(string folderLocation)
        {
            DaniSaveData data = new DaniSaveData();
            // Check for old save first
            // If it is old save, save it as the new save type, and delete the old save?
            // This is assuming there won't be a new save and old save
            if (File.Exists(Path.Combine(folderLocation, SaveFileName)))
            {
                var node = LWJson.Parse(File.ReadAllText(Path.Combine(folderLocation, SaveFileName)));
                data = LoadSaveFile(node);
            }
            else if (File.Exists(Path.Combine(folderLocation, TmpSaveFileName)))
            {
                var node = LWJson.Parse(File.ReadAllText(Path.Combine(folderLocation, TmpSaveFileName)));
                data = LoadSaveFile(node);
                SaveDaniSaveData(data);
            }
#if RELEASE
            else if (File.Exists(Path.Combine(folderLocation, OldSaveFileName)))
            {
                var node = LWJson.Parse(File.ReadAllText(Path.Combine(folderLocation, OldSaveFileName)));
                data = LoadOldSaveFile(node);
                SaveDaniSaveData(data);
                return LoadSaveData(folderLocation);
            }
#endif

            return data;
        }

        static DaniSaveData LoadSaveFile(LWJson node)
        {
            DaniSaveData data = new DaniSaveData();
            data.Courses = new List<SaveCourse>();

            var courses = node["Courses"].AsArray();
            for (int i = 0; i < courses.Count; i++)
            {
                data.Courses.Add(LoadCourseObject(courses[i]));
            }

            return data;
        }

        static SaveCourse LoadCourseObject(LWJson node)
        {
            SaveCourse course = new SaveCourse();

            course.Hash = (uint)node["Hash"].AsDouble();

            var playDataObject = node["PlayData"].AsArray();
            for (int i = 0; i < playDataObject.Count; i++)
            {
                var playData = LoadPlayDataObject(playDataObject[i]);
                var rank = DaniPlayManager.CalculateRankBorders(CourseDataManager.GetCourseFromHash(course.Hash), playData);
                var combo = DaniPlayManager.CalculateComboRank(playData);
                playData.RankCombo = new DaniRankCombo(rank, combo);
                course.PlayData.Add(playData);
            }

            course.SongReached = course.PlayData.Max((x) => x.SongReached);
            course.RankCombo = course.PlayData.Max((x) => x.RankCombo);
            return course;
        }

        static PlayData LoadPlayDataObject(LWJson node)
        {
            PlayData playData = new PlayData();

            playData.PlayDateTime = DateTime.Parse(node["DateTime"].AsString());
            var modifiers = node["Modifiers"];
            playData.Modifiers = new PlayModifiers(
                (DataConst.SpeedTypes)modifiers["Speed"].AsInteger(),
                (DataConst.OptionOnOff)modifiers["Vanish"].AsInteger(),
                (DataConst.OptionOnOff)modifiers["Inverse"].AsInteger(),
                (DataConst.RandomLevel)modifiers["Random"].AsInteger(),
                (DataConst.SpecialTypes)modifiers["Special"].AsInteger());
            playData.MaxCombo = node["MaxCombo"].AsInteger();
            playData.SoulGauge = node["SoulGauge"].AsInteger();

            var songDataObject = node["Songs"].AsArray();
            for (int i = 0; i < songDataObject.Count; i++)
            {
                playData.SongPlayData.Add(LoadSongDataObject(songDataObject[i]));
            }

            for (int i = 0; i < playData.SongPlayData.Count; i++)
            {
                if (playData.SongPlayData[i].Score == 0 &&
                    playData.SongPlayData[i].Goods == 0 &&
                    playData.SongPlayData[i].Oks == 0 &&
                    playData.SongPlayData[i].Bads == 0 &&
                    playData.SongPlayData[i].Drumroll == 0 &&
                    playData.SongPlayData[i].Combo == 0)
                {
                    playData.SongReached = i;
                    break;
                }
                playData.SongReached = i + 1;
            }

            // To make up for me storing all SoulGauge values as 0 while I couldn't calculate them
            if (playData.SoulGauge == 0)
            {
                if ((playData.SongReached == playData.SongPlayData.Count) &&
                    (playData.SongPlayData[playData.SongPlayData.Count - 1].Goods != 0))
                {
                    playData.SoulGauge = 100;
                }
            }

            return playData;
        }

        static SongPlayData LoadSongDataObject(LWJson node)
        {
            SongPlayData songPlayData = new SongPlayData();

            songPlayData.Score = node["Score"].AsInteger();
            songPlayData.Goods = node["Goods"].AsInteger();
            songPlayData.Oks = node["Oks"].AsInteger();
            songPlayData.Bads = node["Bads"].AsInteger();
            songPlayData.Drumroll = node["Drumroll"].AsInteger();
            songPlayData.Combo = node["Combo"].AsInteger();

            return songPlayData;
        }

        static DaniSaveData LoadOldSaveFile(LWJson node)
        {
            DaniSaveData data = new DaniSaveData();

            data.Courses = new List<SaveCourse>();

            var courses = node["courses"].AsArray();
            for (int i = 0; i < courses.Count; i++)
            {
                data.Courses.Add(LoadOldCourseObject(courses[i]));
            }

            return data;
        }

        static SaveCourse LoadOldCourseObject(LWJson node)
        {
            SaveCourse course = new SaveCourse();

            course.Hash = (uint)node["danHash"].AsDouble();

            PlayData play = new PlayData();
            play.SoulGauge = node["totalSoulGauge"].AsInteger();
            play.MaxCombo = node["totalCombo"].AsInteger();

            var songsObject = node["songScores"].AsArray();
            for (int i = 0; i < songsObject.Count; i++)
            {
                play.SongPlayData.Add(LoadOldSongObject(songsObject[i]));
            }

            var rank = DaniPlayManager.CalculateRankBorders(CourseDataManager.GetCourseFromHash(course.Hash), play);
            var combo = DaniPlayManager.CalculateComboRank(play);
            play.RankCombo = new DaniRankCombo(rank, combo);

            course.PlayData.Add(play);

            return course;
        }

        static SongPlayData LoadOldSongObject(LWJson node)
        {
            SongPlayData songPlayData = new SongPlayData();

            songPlayData.Score = node["score"].AsInteger();
            songPlayData.Goods = node["goods"].AsInteger();
            songPlayData.Oks = node["oks"].AsInteger();
            songPlayData.Bads = node["bads"].AsInteger();
            songPlayData.Drumroll = node["drumroll"].AsInteger();
            songPlayData.Combo = node["combo"].AsInteger();

            return songPlayData;
        }

        #endregion


        #region Saving

        public static void SaveDaniSaveData()
        {
            SaveDaniSaveData(SaveData);
        }

        static void SaveDaniSaveData(DaniSaveData saveData)
        {
            Plugin.LogInfo(LogType.Info, "Saving Dani Data", 1);
            var saveJsonObject = new LWJsonObject()
            {
                ["Courses"] = new LWJsonArray(),
            };

            for (int i = 0; i < saveData.Courses.Count; i++)
            {
                if (saveData.Courses[i].PlayData.Count == 0)
                {
                    continue;
                }
                var courseObject = SaveCourseObject(saveData.Courses[i]);
                saveJsonObject["Courses"].AsArray().Add(courseObject);
            }

            var folderLocation = Plugin.Instance.ConfigDaniDojoSaveLocation.Value;

            // Write the json file to the tmp file path
            var tmpFilePath = Path.Combine(folderLocation, TmpSaveFileName);
            //JsonSerializerOptions options = new JsonSerializerOptions();
            //options.WriteIndented = false; // To save space hopefully
            //var jsonString = saveJsonObject.ToJsonString(options);
            var jsonString = saveJsonObject.ToString();
            File.WriteAllText(tmpFilePath, jsonString);

            // Attempt to read the tmp file path
            var writtenText = File.ReadAllText(tmpFilePath);
            var loadedFile = LoadSaveFile(LWJson.Parse(writtenText));
            if (loadedFile != null)
            {
                // I hope this overwrites the current SaveFile
                // Otherwise I'd need to delete the current SaveFile, then move it
                File.Delete(Path.Combine(folderLocation, SaveFileName));
                File.Move(Path.Combine(folderLocation, TmpSaveFileName), Path.Combine(folderLocation, SaveFileName));
            }

            Plugin.LogInfo(LogType.Info, "Saving Dani Data Complete", 1);
        }

        static LWJsonObject SaveCourseObject(SaveCourse course)
        {
            var courseJsonObject = new LWJsonObject()
                .Add("Hash", course.Hash)
                .Add("PlayData", new LWJsonArray());

            for (int i = 0; i < course.PlayData.Count; i++)
            {
                var playDataObject = SavePlayDataObject(course.PlayData[i]);
                courseJsonObject["PlayData"].AsArray().Add(playDataObject);
            }

            return courseJsonObject;
        }

        static LWJsonObject SavePlayDataObject(PlayData playData)
        {
            var playDataJsonObject = new LWJsonObject()
                .Add("DateTime", playData.PlayDateTime.ToString("s"))
                .Add("Modifiers", new LWJsonObject()
                    .Add("Speed", (int)playData.Modifiers.Speed)
                    .Add("Vanish", (int)playData.Modifiers.Vanish)
                    .Add("Inverse", (int)playData.Modifiers.Inverse)
                    .Add("Random", (int)playData.Modifiers.Random)
                    .Add("Special", (int)playData.Modifiers.Special))
                .Add("MaxCombo", playData.MaxCombo)
                .Add("SoulGauge", playData.SoulGauge)
                .Add("Songs", new LWJsonArray());
          
            for (int i = 0; i < playData.SongPlayData.Count; i++)
            {
                var songDataObject = SaveSongDataObject(playData.SongPlayData[i]);
                playDataJsonObject["Songs"].AsArray().Add(songDataObject);
            }
            return playDataJsonObject;
        }

        static LWJsonObject SaveSongDataObject(SongPlayData song)
        {
            var songJsonObject = new LWJsonObject()
                .Add("Score", song.Score)
                .Add("Goods", song.Goods)
                .Add("Oks", song.Oks)
                .Add("Bads", song.Bads)
                .Add("Drumroll", song.Drumroll)
                .Add("Combo", song.Combo);
            return songJsonObject;
        }

        #endregion

        static public void AddPlayData(uint hash, PlayData play)
        {
            Plugin.LogInfo(LogType.Info, "AddPlayData", 1);
            for (int i = 0; i < SaveData.Courses.Count; i++)
            {
                if (SaveData.Courses[i].Hash == hash)
                {
                    SaveData.Courses[i].PlayData.Add(play);
                    return;
                }
            }
            // Do I return null?
            // Or do I return an empty SaveCourse?
            var course = new SaveCourse(hash);
            course.PlayData.Add(play);
            SaveData.Courses.Add(course);
        }

        static public SaveCourse GetCourseRecord(uint hash)
        {
            for (int i = 0; i < SaveData.Courses.Count; i++)
            {
                if (SaveData.Courses[i].Hash == hash)
                {
                    return SaveData.Courses[i];
                }
            }
            // Do I return null?
            // Or do I return an empty SaveCourse?
            var course = new SaveCourse(hash);
            SaveData.Courses.Add(course);
            return SaveData.Courses[SaveData.Courses.Count - 1];
        }

        /// <summary>
        /// This returns the course after the highest cleared course, or the highest course if the highest has been cleared.
        /// </summary>
        /// <param name="series">The series to search for the course.</param>
        /// <returns>The next highest course.</returns>
        static public DaniCourse GetDefaultCourse(DaniSeries series)
        {
            Plugin.LogInfo(LogType.Info, "GetDefaultCourse Start", 2);
            // First find the first dan, which is generally the starting point
            // Then move up from there to find the highest cleared dan
            int highestClearedIndex = 0;
            for (int i = 0; i < series.Courses.Count; i++)
            {
                if (series.Courses[i].Id == "1dan")
                {
                    highestClearedIndex = i;
                    break;
                }
            }

            Plugin.LogInfo(LogType.Info, "GetDefaultCourse Middle", 2);

            for (int i = highestClearedIndex; i < series.Courses.Count - 1; i++)
            {
                var saveCourse = GetCourseRecord(series.Courses[i].Hash);
                if (saveCourse.RankCombo.Rank >= DaniRank.RedClear)
                {
                    highestClearedIndex = i + 1;
                }
            }

            Plugin.LogInfo(LogType.Info, "GetDefaultCourse End", 2);
            return series.Courses[highestClearedIndex];
        }

        static public bool IsCourseLocked(DaniSeries series, DaniCourse course)
        {
            if (!course.IsLocked)
            {
                return false;
            }
            var previousCourse = CourseDataManager.GetPreviousCourse(series, course);
            var saveData = GetCourseRecord(previousCourse.Hash);
            // If the previous rank is red or higher, this course is not locked
            // This course is locked if it is not red or higher
            return !(saveData.RankCombo.Rank >= DaniRank.RedClear);
        }

        static public SaveCourse GetHighestActiveClear()
        {
            var series = CourseDataManager.GetActiveSeries();
            SaveCourse highestCourseRecord = null;
            if (series != null)
            {
                for (int i = 0; i < series.Courses.Count; i++)
                {
                    var saveCourse = GetCourseRecord(series.Courses[i].Hash);
                    if (saveCourse.RankCombo.Rank >= DaniRank.RedClear)
                    {
                        highestCourseRecord = saveCourse;
                    }
                }
            }
            return highestCourseRecord;
        }
    }
}
