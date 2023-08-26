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

        #region LoadCourses
        public static void LoadCourseData()
        {
            //Plugin.LogInfo("LoadCourseData Start", true);
            AllSeriesData = new List<DaniSeries>(); // I'm not sure if this line is actually needed, or even detrimental
            AllSeriesData = LoadCourseData(Plugin.Instance.ConfigDaniDojoDataLocation.Value);
            //Plugin.LogInfo("LoadCourseData Finished", true);
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
                Plugin.LogInfo("Loading File \"" + files[i].Name + "\"");
                var text = File.ReadAllText(files[i].FullName);
                JsonNode node = JsonNode.Parse(text);
                var series = LoadSeries(node);
                allSeriesData.Add(series);
                Plugin.LogInfo("Loading File \"" + files[i].Name + "\" complete");
            }

            allSeriesData.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            return allSeriesData;
        }

        static DaniSeries LoadSeries(JsonNode node)
        {
            //Plugin.LogInfo("Loading Series start", true);
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
                var course = LoadCourse(courses[i], seriesData);
                seriesData.Courses.Add(course);
            }

            seriesData.Courses.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            //Plugin.LogInfo("Loading Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadCourse(JsonNode node, DaniSeries parent)
        {
            //Plugin.LogInfo("Loading Course start", true);
            DaniCourse course = new DaniCourse();
            course.Id = node["Id"].GetValue<string>();
            course.Title = node["Title"].GetValue<string>();
            course.Order = node["Order"].GetValue<int>();

            course.IsLocked = node["IsLocked"].GetValue<bool>();

            if (node["Background"] != null)
            {
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
            }
            else
            {
                switch (course.Id.ToLower().Trim())
                {
                    case "5kyuu":
                    case "4kyuu":
                    case "3kyuu":
                    case "2kyuu":
                    case "1kyuu": course.Background = CourseBackground.Wood; break;
                    case "1dan":
                    case "2dan":
                    case "3dan":
                    case "4dan":
                    case "5dan": course.Background = CourseBackground.Blue; break;
                    case "6dan":
                    case "7dan":
                    case "8dan":
                    case "9dan":
                    case "10dan": course.Background = CourseBackground.Red; break;
                    case "11dan":
                    case "12dan":
                    case "13dan": course.Background = CourseBackground.Silver; break;
                    case "14dan": course.Background = CourseBackground.Gold; break;
                    default: course.Background = CourseBackground.Tan; break;
                }
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

            course.Parent = parent;
            course.Hash = GetHashFromCourse(course);

            //Plugin.LogInfo("Loading Course finish", true);
            return course;
        }

        static DaniSongData LoadSongData(JsonNode node)
        {
            //Plugin.LogInfo("Loading Song Data start", true);
            DaniSongData song = new DaniSongData();
            song.SongId = node["SongId"].GetValue<string>();
            song.TitleEng = node["TitleEng"].GetValue<string>();
            song.TitleJp = node["TitleJp"].GetValue<string>();
            //song.Level = (EnsoData.EnsoLevelType)(node["Level"]!.GetValue<int>() - 1);

            string levelString = string.Empty;
            try
            {
                var levelInt = node["Level"].GetValue<int>();
                levelString = levelInt.ToString();
                Plugin.LogInfo("Level int = " + levelInt, 2);
            }
            catch { }
            try
            {
                levelString = node["Level"].GetValue<string>();
                Plugin.LogInfo("Level string = " + levelString, 2);
            }
            catch { }
            try
            {
                var levelInt = node["Course"].GetValue<int>();
                levelString = levelInt.ToString();
                Plugin.LogInfo("Course int = " + levelInt, 2);
            }
            catch { }
            try
            {
                levelString = node["Course"].GetValue<string>();
                Plugin.LogInfo("Course string = " + levelString, 2);
            }
            catch { }

            switch (levelString.ToLower().Trim())
            {
                case "1":
                case "easy":
                case "かんたん":
                case "kantan":
                    song.Level = EnsoData.EnsoLevelType.Easy;
                    break;
                case "2":
                case "normal":
                case "ふつう":
                case "futsu":
                case "futsuu":
                    song.Level = EnsoData.EnsoLevelType.Normal;
                    break;
                case "3":
                case "hard":
                case "muzukashi":
                case "muzukashii":
                case "むずかしい":
                    song.Level = EnsoData.EnsoLevelType.Hard;
                    break;
                case "4":
                case "oni":
                case "おに":
                case "extreme":
                case "mania":
                    song.Level = EnsoData.EnsoLevelType.Mania;
                    break;
                case "5":
                case "edit":
                case "extraextreme":
                case "extra extreme":
                case "exex":
                case "ura":
                case "uraoni":
                case "えでぃと":
                    song.Level = EnsoData.EnsoLevelType.Ura;
                    break;
                default:
                    song.Level = EnsoData.EnsoLevelType.Mania;
                    Plugin.LogError("Error reading Song Level of " + levelString + ", defaulted to Oni");
                    break;
            }

            song.IsHidden = node["IsHidden"].GetValue<bool>();
            //Plugin.LogInfo("Loading Song Data finish", true);
            return song;
        }

        static DaniBorder LoadBorder(JsonNode node)
        {
            //Plugin.LogInfo("Loading Border start", true);
            DaniBorder border = new DaniBorder();
            //Plugin.LogInfo("Loading BorderType", true);
            string borderTypeString = string.Empty;
            try
            {
                var borderTypeInt = node["BorderType"]!.GetValue<int>();
                borderTypeString = borderTypeInt.ToString();
                //Plugin.LogInfo("BorderType int = " + borderTypeInt, true);
            }
            catch { }
            try
            {
                borderTypeString = node["BorderType"]!.GetValue<string>();
                //Plugin.LogInfo("BorderType string = " + borderTypeString, true);
            }
            catch { }

            switch (borderTypeString.ToLower())
            {
                case "1":
                case "soul gauge":
                case "soulgauge":
                    border.BorderType = BorderType.SoulGauge;
                    break;
                case "2":
                case "good":
                case "goods":
                    border.BorderType = BorderType.Goods;
                    break;
                case "3":
                case "ok":
                case "oks":
                    border.BorderType = BorderType.Oks;
                    break;
                case "4":
                case "bad":
                case "bads":
                    border.BorderType = BorderType.Bads;
                    break;
                case "5":
                case "combo":
                    border.BorderType = BorderType.Combo;
                    break;
                case "6":
                case "drumroll":
                case "renda":
                    border.BorderType = BorderType.Drumroll;
                    break;
                case "7":
                case "score":
                    border.BorderType = BorderType.Score;
                    break;
                case "8":
                case "totalhits":
                case "total hits":
                case "totalhit":
                case "total hit":
                    border.BorderType = BorderType.TotalHits;
                    break;
                default:
                    border.BorderType = BorderType.TotalHits;
                    Plugin.LogError("Error reading BorderType of " + borderTypeString + ", defaulted to TotalHits");
                    break;
            }

            //Plugin.LogInfo("BorderType Loaded", true);
            var redBorder = node["RedBorder"];
            if (redBorder is JsonArray)
            {
                Plugin.LogInfo("Border is Array", 2);
                var redBorderArray = redBorder.AsArray();
                var goldBorderArray = node["GoldBorder"].AsArray();
                for (int i = 0; i < redBorderArray.Count; i++)
                {
                    border.RedReqs.Add(redBorderArray[i].GetValue<int>());
                    border.GoldReqs.Add(goldBorderArray[i].GetValue<int>());
                }

                // If there's only 1 item, IsTotal = true
                border.IsTotal = redBorderArray.Count == 1;
            }
            else
            {
                //Plugin.LogInfo("Border is not Array", true);
                border.RedReqs.Add(node["RedBorder"].GetValue<int>());
                border.GoldReqs.Add(node["GoldBorder"].GetValue<int>());
                border.IsTotal = true;
            }
            //Plugin.LogInfo("Loading Border finish", true);
            return border;
        }


        static DaniSeries LoadOldSeries(JsonNode node)
        {
            //Plugin.LogInfo("Loading Old Series start", true);
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
            //Plugin.LogInfo("Loading Old Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadOldCourse(JsonNode node, DaniSeries parent)
        {
            //Plugin.LogInfo("Loading Old Course start", true);
            DaniCourse course = new DaniCourse();

            course.Order = node["danId"]!.GetValue<int>();
            course.Id = course.Order.ToString();
            course.Title = node["title"]!.GetValue<string>();
            if (node["Background"] != null)
            {
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
            }
            else
            {
                switch (course.Title)
                {
                    case "5kyuu":
                    case "五級 5th Kyu":
                    case "4kyuu":
                    case "四級 4th Kyu":
                    case "3kyuu":
                    case "三級 3rd Kyu":
                    case "2kyuu":
                    case "二級 2nd Kyu":
                    case "1kyuu":
                    case "一級 1st Kyu":
                        course.Background = CourseBackground.Wood;
                        break;
                    case "1dan":
                    case "初段 1st Dan":
                    case "2dan":
                    case "二段 2nd Dan":
                    case "3dan":
                    case "三段 3rd Dan":
                    case "4dan":
                    case "四段 4th Dan":
                    case "5dan":
                    case "五段 5th Dan":
                        course.Background = CourseBackground.Blue;
                        break;
                    case "6dan":
                    case "六段 6th Dan":
                    case "7dan":
                    case "七段 7th Dan":
                    case "8dan":
                    case "八段 8th Dan":
                    case "9dan":
                    case "九段 9th Dan":
                    case "10dan":
                    case "十段 10th Dan":
                        course.Background = CourseBackground.Red;
                        break;
                    case "11dan":
                    case "玄人 Kuroto":
                    case "12dan":
                    case "名人 Meijin":
                    case "13dan":
                    case "超人 Chojin":
                        course.Background = CourseBackground.Silver;
                        break;
                    case "14dan":
                    case "達人 Tatsujin":
                        course.Background = CourseBackground.Gold;
                        break;
                    default:
                        course.Background = CourseBackground.Tan;
                        break;
                }
                

            }

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

            //Plugin.LogInfo("Loading Old Course finish", true);
            return course;
        }

        #endregion

        static public DaniCourse GetCourseFromHash(uint hash)
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
            // The hash consists of:
            //   SeriesId
            //   CourseId
            //     SongId
            //     SongLevel
            //     SongId
            //     SongLevel, etc.
            //Plugin.LogInfo("course.Hash = " + course.Hash, true);
            if (course.Hash == 0)
            {
                string hashString = course.Parent.Id;
                hashString += course.Id;
                for (int i = 0; i < course.Songs.Count; i++)
                {
                    hashString += course.Songs[i].SongId;
                    hashString += course.Songs[i].Level;
                }

                course.Hash = MurmurHash2.Hash(hashString);
            }
            //Plugin.LogInfo("course.Hash = " + course.Hash, true);

            return course.Hash;
        }

        static public int GetCourseIndex(DaniSeries series, DaniCourse course)
        {
            //Plugin.LogInfo("GetCourseIndex: Hash: " + course.Hash, true);
            return series.Courses.FindIndex((x) => x.Hash == course.Hash);
        }

        /// <summary>
        /// Gets the next course, whether it's locked or not.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="course"></param>
        /// <returns></returns>
        static public DaniCourse GetNextCourse(DaniSeries series, DaniCourse course)
        {
            var index = GetCourseIndex(series, course) + 1;
            if (index >= series.Courses.Count)
            {
                index = 0;
            }
            return series.Courses[index];
        }

        static public DaniCourse GetNextUnlockedCourse(DaniSeries series, DaniCourse course)
        {
            int numAttempts = series.Courses.Count * 2;
            DaniCourse nextCourse = course;
            do
            {
                nextCourse = GetNextCourse(series, nextCourse);
                //Plugin.LogInfo("GetNextUnlockedCourse: Checking " + nextCourse.Id, true);

                numAttempts--;
            } while (SaveDataManager.IsCourseLocked(series, nextCourse) && numAttempts > 0);
            if (numAttempts == 0)
            {
                nextCourse = series.Courses[0];
            }
            //Plugin.LogInfo("GetNextUnlockedCourse: Returning " + nextCourse.Id, true);
            return nextCourse;
        }

        /// <summary>
        /// Gets the previous course, whether it's locked or not.
        /// </summary>
        /// <param name="series"></param>
        /// <param name="course"></param>
        /// <returns></returns>
        static public DaniCourse GetPreviousCourse(DaniSeries series, DaniCourse course)
        {
            var index = GetCourseIndex(series, course) - 1;
            if (index < 0)
            {
                index = series.Courses.Count - 1;
            }
            return series.Courses[index];
        }

        static public DaniCourse GetPreviousUnlockedCourse(DaniSeries series, DaniCourse course)
        {
            int numAttempts = series.Courses.Count * 2;
            DaniCourse prevCourse = course;
            do
            {
                prevCourse = GetPreviousCourse(series, prevCourse);
                numAttempts--;
            } while (SaveDataManager.IsCourseLocked(series, prevCourse) && numAttempts > 0);
            if (numAttempts == 0)
            {
                prevCourse = series.Courses[0];
            }
            return prevCourse;
        }

    }
}
