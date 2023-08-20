using DaniDojo.Data;
using DaniDojo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DaniDojo.Managers
{
    internal class CourseDataManager
    {
        public static List<DaniSeries> AllSeriesData = new List<DaniSeries>();

        public static void LoadCourseData()
        {
            Plugin.LogInfo("LoadCourseData Start", true);
            AllSeriesData = new List<DaniSeries>();
            AllSeriesData = LoadCourseData(Plugin.Instance.ConfigDaniDojoDataLocation.Value);
            Plugin.LogInfo("LoadCourseData Finished", true);
        }

        static List<DaniSeries> LoadCourseData(string jsonFolderLocation)
        {
            var allSeriesData = new List<DaniSeries>();
            string folderPath = jsonFolderLocation;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            var files = dirInfo.GetFiles("*.json", SearchOption.AllDirectories).ToList();

            for (int i = 0; i < files.Count; i++)
            {
                Plugin.LogInfo("Loading File \"" + files[i].Name + "\"", true);
                var text = File.ReadAllText(files[i].FullName);
                JsonNode node = JsonNode.Parse(text);
                var series = LoadSeries(node);
                allSeriesData.Add(series);
                Plugin.LogInfo("Loading File \"" + files[i].Name + "\" complete", true);
            }

            allSeriesData.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            return allSeriesData;
        }

        static DaniSeries LoadSeries(JsonNode node)
        {
            Plugin.LogInfo("Loading Series start", true);
            if (node["danSeriesTitle"] != null)
            {
                var oldSeriesData = LoadOldSeries(node);
                return oldSeriesData;
            }
            DaniSeries seriesData = new DaniSeries();
            seriesData.Id = node["SeriesId"].GetValue<string>();
            seriesData.Title = node["SeriesTitle"].GetValue<string>();
            seriesData.IsActive = node["IsActive"].GetValue<bool>();
            seriesData.Order = node["Order"].GetValue<int>();
            var courses = node["Courses"].AsArray();
            for (int i = 0; i < courses.Count; i++)
            {
                var course = LoadCourse(courses[i]);
                seriesData.Courses.Add(course);
            }

            seriesData.Courses.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            Plugin.LogInfo("Loading Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadCourse(JsonNode node)
        {
            Plugin.LogInfo("Loading Course start", true);
            DaniCourse course = new DaniCourse();
            course.Id = node["Id"].GetValue<string>();
            course.Title = node["Title"].GetValue<string>();
            course.Order = node["Order"].GetValue<int>();
            course.IsLocked = node["IsLocked"].GetValue<bool>();
            var background = node["Background"].GetValue<string>();
            switch (background)
            {
                case "Tan": course.Background = CourseBackground.Tan; break;
                case "Wood": course.Background = CourseBackground.Wood; break;
                case "Blue": course.Background = CourseBackground.Blue; break;
                case "Red": course.Background = CourseBackground.Red; break;
                case "Silver": course.Background = CourseBackground.Silver; break;
                case "Gold": course.Background = CourseBackground.Gold; break;
                default: course.Background = CourseBackground.Tan; break;
            }

            var songs = node["Songs"].AsArray();
            for (int i = 0; i < songs.Count; i++)
            {
                var song = LoadSongData(songs[i]);
                course.Songs.Add(song);
            }

            var borders = node["Borders"].AsArray();
            for (int i = 0; i < borders.Count; i++)
            {
                var border = LoadBorder(borders[i]);
                course.Borders.Add(border);
            }

            Plugin.LogInfo("Loading Course finish", true);
            return course;
        }

        static DaniSongData LoadSongData(JsonNode node)
        {
            Plugin.LogInfo("Loading Song Data start", true);
            DaniSongData song = new DaniSongData();
            song.SongId = node["SongId"].GetValue<string>();
            song.TitleEng = node["TitleEng"].GetValue<string>();
            song.TitleJp = node["TitleJp"].GetValue<string>();
            song.Level = (EnsoData.EnsoLevelType)(node["Level"]!.GetValue<int>() - 1);
            song.IsHidden = node["IsHidden"].GetValue<bool>();
            Plugin.LogInfo("Loading Song Data finish", true);
            return song;
        }

        static DaniBorder LoadBorder(JsonNode node)
        {
            Plugin.LogInfo("Loading Border start", true);
            DaniBorder border = new DaniBorder();
            border.BorderType = (BorderType)node["BorderType"]!.GetValue<int>();
            var redBorder = node["RedBorder"];
            if (redBorder is JsonArray)
            {
                var redBorderArray = redBorder.AsArray();
                var goldBorderArray = node["GoldBorder"].AsArray();
                for (int i = 0; i < redBorderArray.Count; i++)
                {
                    border.RedReqs.Add(redBorderArray[i].GetValue<int>());
                    border.GoldReqs.Add(goldBorderArray[i].GetValue<int>());
                }
            }
            else
            {
                border.RedReqs.Add(node["RedBorder"].GetValue<int>());
                border.GoldReqs.Add(node["GoldBorder"].GetValue<int>());
            }
            Plugin.LogInfo("Loading Border finish", true);
            return border;
        }


        static DaniSeries LoadOldSeries(JsonNode node)
        {
            Plugin.LogInfo("Loading Old Series start", true);
            DaniSeries seriesData = new DaniSeries();
            seriesData.Title = node["danSeriesTitle"]!.GetValue<string>();
            seriesData.Id = node["danSeriesId"]!.GetValue<string>();
            seriesData.IsActive = node["isActiveDan"]!.GetValue<bool>();
            seriesData.Order = node["order"]!.GetValue<int>();
            var courses = node["courses"].AsArray();
            for (int j = 0; j < courses.Count; j++)
            {
                var course = LoadOldCourse(courses[j], seriesData);

                seriesData.Courses.Add(course);
            }
            seriesData.Courses.Sort((x, y) => x.Order > y.Order ? 1 : -1);
            Plugin.LogInfo("Loading Old Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadOldCourse(JsonNode node, DaniSeries parent)
        {
            Plugin.LogInfo("Loading Old Course start", true);
            DaniCourse course = new DaniCourse();

            course.Order = node["danId"]!.GetValue<int>();
            course.Id = course.Order.ToString();
            course.Title = node["title"]!.GetValue<string>();

            // Check to see if "locked" is in the json for this course
            if (node["locked"] != null)
            {
                course.IsLocked = node["locked"]!.GetValue<bool>();
            }


            for (int i = 0; i < node["aryOdaiSong"]!.AsArray().Count; i++)
            {
                var songNode = node["aryOdaiSong"]![i]!;

                DaniSongData song = new DaniSongData();

                song.SongId = songNode["songNo"]!.GetValue<string>();
                song.Level = (EnsoData.EnsoLevelType)(songNode["level"]!.GetValue<int>() - 1);
                song.IsHidden = songNode["isHiddenSongName"]!.GetValue<bool>();

                course.Songs.Add(song);
            }

            for (int i = 0; i < node["aryOdaiBorder"]!.AsArray().Count; i++)
            {
                var borderNode = node["aryOdaiBorder"]![i]!;
                DaniBorder border = new DaniBorder();

                border.BorderType = (BorderType)borderNode["odaiType"]!.GetValue<int>();
                border.IsTotal = borderNode["borderType"]!.GetValue<int>() == 1;

                if (borderNode["redBorderTotal"] != null)
                {
                    border.RedReqs.Add(borderNode["redBorderTotal"]!.GetValue<int>());
                }
                else
                {
                    border.RedReqs.Add(borderNode["redBorder_1"]!.GetValue<int>());
                    border.RedReqs.Add(borderNode["redBorder_2"]!.GetValue<int>());
                    border.RedReqs.Add(borderNode["redBorder_3"]!.GetValue<int>());
                }

                if (borderNode["goldBorderTotal"] != null)
                {
                    border.GoldReqs.Add(borderNode["goldBorderTotal"]!.GetValue<int>());
                }
                else
                {
                    border.GoldReqs.Add(borderNode["goldBorder_1"]!.GetValue<int>());
                    border.GoldReqs.Add(borderNode["goldBorder_2"]!.GetValue<int>());
                    border.GoldReqs.Add(borderNode["goldBorder_3"]!.GetValue<int>());
                }

                course.Borders.Add(border);
            }

            course.Parent = parent;
            course.Hash = GetHashFromCourse(course);

            Plugin.LogInfo("Loading Old Course finish", true);
            return course;
        }

        static DaniCourse GetCourseFromHash(uint hash)
        {
            for (int i = 0; i < AllSeriesData.Count; i++)
            {
                for (int j = 0; j < AllSeriesData[i].Courses.Count; j++)
                {
                    if (AllSeriesData[i].Courses[j].Hash == hash)
                    {
                        return AllSeriesData[i].Courses[j];
                    }
                }
            }
            Plugin.LogError("Could not find corresponding Course from Hash");
            return null;
        }

        static uint GetHashFromCourse(DaniCourse course)
        {
            if (course.Hash == 0)
            {
                string hashString = course.Parent.Id;
                hashString += course.Id.ToString();
                for (int i = 0; i < course.Songs.Count; i++)
                {
                    hashString += course.Songs[i].SongId;
                    hashString += course.Songs[i].Level;
                }

                course.Hash = MurmurHash2.Hash(hashString);
            }

            return course.Hash;
        }


    }
}
