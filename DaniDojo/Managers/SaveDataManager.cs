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
            Plugin.LogInfo("LoadSaveData Start", true);
            SaveData = new DaniSaveData(); // I'm not sure if this line is actually needed, or even detrimental
            SaveData = LoadSaveData(Plugin.Instance.ConfigDaniDojoSaveLocation.Value);
            Plugin.LogInfo("LoadSaveData Finished", true);
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
                course.PlayData.Add(LoadPlayDataObject(playDataObject[i]));
            }

            return course;
        }

        static PlayData LoadPlayDataObject(JsonNode node)
        {
            PlayData playData = new PlayData();

            playData.PlayDateTime = node["DateTime"].GetValue<DateTime>();
            playData.Modifiers = new PlayModifiers(
                (DataConst.SpeedTypes)node["Speed"].GetValue<int>(),
                (DataConst.OptionOnOff)node["Vanish"].GetValue<int>(),
                (DataConst.OptionOnOff)node["Inverse"].GetValue<int>(),
                (DataConst.RandomLevel)node["Random"].GetValue<int>(),
                (DataConst.SpecialTypes)node["Special"].GetValue<int>());
            playData.TotalCombo = node["TotalCombo"].GetValue<int>();
            playData.SoulGauge = node["SoulGauge"].GetValue<int>();

            var songDataObject = node["Songs"].AsArray();
            for (int i = 0; i < songDataObject.Count; i++)
            {
                playData.SongPlayData.Add(LoadSongDataObject(songDataObject[i]));
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

            var courses = node["Courses"].AsArray();
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
            play.TotalCombo = node["totalCombo"].GetValue<int>();

            var songsObject = node["songScores"].AsArray();
            for (int i = 0; i < songsObject.Count; i++)
            {
                play.SongPlayData.Add(LoadOldSongObject(songsObject[i]));
            }

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
                ["DateTime"] = playData.PlayDateTime.ToString(),
                ["Modifiers"] = new JsonObject()
                {
                    ["Speed"] = (int)playData.Modifiers.Speed,
                    ["Vanish"] = (int)playData.Modifiers.Vanish,
                    ["Inverse"] = (int)playData.Modifiers.Inverse,
                    ["Random"] = (int)playData.Modifiers.Random,
                    ["Special"] = (int)playData.Modifiers.Special,
                },
                ["TotalCombo"] = playData.TotalCombo,
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

    }
}
