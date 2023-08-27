using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using DaniDojo.Data;

namespace DaniDojo.Managers
{
    internal class SaveDataManager
    {
        const string SaveFileName = "DaniSave.json";
        const string TmpSaveFileName = "TmpSave.json";
        const string OldSaveFileName = "dansave.json";

        static DaniSaveData SaveData { get; set; }

        #region Loading
        public static void LoadSaveData()
        {
            Plugin.LogInfo("LoadSaveData Start", 1);
            SaveData = new DaniSaveData(); // I'm not sure if this line is actually needed, or even detrimental
            SaveData = LoadSaveData(Plugin.Instance.ConfigDaniDojoSaveLocation.Value);
            Plugin.LogInfo("LoadSaveData Finished", 1);
        }

        static DaniSaveData LoadSaveData(string folderLocation)
        {
            DaniSaveData data = new DaniSaveData();
            // Check for old save first
            // If it is old save, save it as the new save type, and delete the old save?
            // This is assuming there won't be a new save and old save
            if (File.Exists(Path.Combine(folderLocation, SaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, SaveFileName)));
                data = LoadSaveFile(node);
            }
            else if (File.Exists(Path.Combine(folderLocation, TmpSaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, TmpSaveFileName)));
                data = LoadSaveFile(node);
                SaveDaniSaveData(data);
            }
            else if (File.Exists(Path.Combine(folderLocation, OldSaveFileName)))
            {
                var node = JsonNode.Parse(File.ReadAllText(Path.Combine(folderLocation, OldSaveFileName)));
                data = LoadOldSaveFile(node);
                SaveDaniSaveData(data);
                return LoadSaveData(folderLocation);
            }


            return data;
        }

        static DaniSaveData LoadSaveFile(JsonNode node)
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

        static SaveCourse LoadCourseObject(JsonNode node)
        {
            SaveCourse course = new SaveCourse();

            course.Hash = node["Hash"].GetValue<uint>();

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

        static PlayData LoadPlayDataObject(JsonNode node)
        {
            PlayData playData = new PlayData();

            playData.PlayDateTime = node["DateTime"].GetValue<DateTime>();
            var modifiers = node["Modifiers"];
            playData.Modifiers = new PlayModifiers(
                (DataConst.SpeedTypes)modifiers["Speed"].GetValue<int>(),
                (DataConst.OptionOnOff)modifiers["Vanish"].GetValue<int>(),
                (DataConst.OptionOnOff)modifiers["Inverse"].GetValue<int>(),
                (DataConst.RandomLevel)modifiers["Random"].GetValue<int>(),
                (DataConst.SpecialTypes)modifiers["Special"].GetValue<int>());
            playData.MaxCombo = node["MaxCombo"].GetValue<int>();
            playData.SoulGauge = node["SoulGauge"].GetValue<int>();

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
            return playData;
        }

        static SongPlayData LoadSongDataObject(JsonNode node)
        {
            SongPlayData songPlayData = new SongPlayData();

            songPlayData.Score = node["Score"].GetValue<int>();
            songPlayData.Goods = node["Goods"].GetValue<int>();
            songPlayData.Oks = node["Oks"].GetValue<int>();
            songPlayData.Bads = node["Bads"].GetValue<int>();
            songPlayData.Drumroll = node["Drumroll"].GetValue<int>();
            songPlayData.Combo = node["Combo"].GetValue<int>();

            return songPlayData;
        }

        static DaniSaveData LoadOldSaveFile(JsonNode node)
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

        static SaveCourse LoadOldCourseObject(JsonNode node)
        {
            SaveCourse course = new SaveCourse();

            course.Hash = node["danHash"].GetValue<uint>();

            PlayData play = new PlayData();
            play.SoulGauge = node["totalSoulGauge"].GetValue<int>();
            play.MaxCombo = node["totalCombo"].GetValue<int>();

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

        static SongPlayData LoadOldSongObject(JsonNode node)
        {
            SongPlayData songPlayData = new SongPlayData();

            songPlayData.Score = node["score"].GetValue<int>();
            songPlayData.Goods = node["goods"].GetValue<int>();
            songPlayData.Oks = node["oks"].GetValue<int>();
            songPlayData.Bads = node["bads"].GetValue<int>();
            songPlayData.Drumroll = node["drumroll"].GetValue<int>();
            songPlayData.Combo = node["combo"].GetValue<int>();

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
            Plugin.LogInfo("Saving Dani Data", 1);
            var saveJsonObject = new JsonObject()
            {
                ["Courses"] = new JsonArray(),
            };

            for (int i = 0; i < saveData.Courses.Count; i++)
            {
                var courseObject = SaveCourseObject(saveData.Courses[i]);
                saveJsonObject["Courses"].AsArray().Add(courseObject);
            }

            var folderLocation = Plugin.Instance.ConfigDaniDojoSaveLocation.Value;

            // Write the json file to the tmp file path
            var tmpFilePath = Path.Combine(folderLocation, TmpSaveFileName);
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = false; // To save space hopefully
            var jsonString = saveJsonObject.ToJsonString(options);
            File.WriteAllText(tmpFilePath, jsonString);

            // Attempt to read the tmp file path
            var writtenText = File.ReadAllText(tmpFilePath);
            var loadedFile = LoadSaveFile(JsonNode.Parse(writtenText));
            if (loadedFile != null)
            {
                // I hope this overwrites the current SaveFile
                // Otherwise I'd need to delete the current SaveFile, then move it
                File.Move(Path.Combine(folderLocation, TmpSaveFileName), Path.Combine(folderLocation, SaveFileName));
            }

            Plugin.LogInfo("Saving Dani Data Complete", 1);
        }

        static JsonObject SaveCourseObject(SaveCourse course)
        {
            var courseJsonObject = new JsonObject()
            {
                ["Hash"] = course.Hash,
                ["PlayData"] = new JsonArray(),
            };

            for (int i = 0; i < course.PlayData.Count; i++)
            {
                var playDataObject = SavePlayDataObject(course.PlayData[i]);
                courseJsonObject["PlayData"].AsArray().Add(playDataObject);
            }

            return courseJsonObject;
        }

        static JsonObject SavePlayDataObject(PlayData playData)
        {
            var playDataJsonObject = new JsonObject()
            {
                ["DateTime"] = playData.PlayDateTime.ToString("s"),
                ["Modifiers"] = new JsonObject()
                {
                    ["Speed"] = (int)playData.Modifiers.Speed,
                    ["Vanish"] = (int)playData.Modifiers.Vanish,
                    ["Inverse"] = (int)playData.Modifiers.Inverse,
                    ["Random"] = (int)playData.Modifiers.Random,
                    ["Special"] = (int)playData.Modifiers.Special,
                },
                ["MaxCombo"] = playData.MaxCombo,
                ["SoulGauge"] = playData.SoulGauge,
                ["Songs"] = new JsonArray(),
            };

            for (int i = 0; i < playData.SongPlayData.Count; i++)
            {
                var songDataObject = SaveSongDataObject(playData.SongPlayData[i]);
                playDataJsonObject["Songs"].AsArray().Add(songDataObject);
            }
            return playDataJsonObject;
        }

        static JsonObject SaveSongDataObject(SongPlayData song)
        {
            var songJsonObject = new JsonObject()
            {
                ["Score"] = song.Score,
                ["Goods"] = song.Goods,
                ["Oks"] = song.Oks,
                ["Bads"] = song.Bads,
                ["Drumroll"] = song.Drumroll,
                ["Combo"] = song.Combo,
            };
            return songJsonObject;
        }

        #endregion

		static public void AddPlayData(uint hash, PlayData play)
        {
            var course = GetCourseRecord(hash);
            course.PlayData.Add(play);
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
            Plugin.LogInfo("GetDefaultCourse Start", 2);
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

            Plugin.LogInfo("GetDefaultCourse Middle", 2);

            for (int i = highestClearedIndex; i < series.Courses.Count - 1; i++)
            {
                var saveCourse = GetCourseRecord(series.Courses[i].Hash);
                if (saveCourse.RankCombo.Rank >= DaniRank.RedClear)
                {
                    highestClearedIndex = i + 1;
                }
            }

            Plugin.LogInfo("GetDefaultCourse End", 2);
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
    }
}
