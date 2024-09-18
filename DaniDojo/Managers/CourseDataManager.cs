using DaniDojo.Data;
using DaniDojo.Utility;
using LightWeightJsonParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Managers
{
    internal class CourseDataManager
    {
        public static List<DaniSeries> AllSeriesData = new List<DaniSeries>();

        #region LoadCourses
        public static void LoadCourseData()
        {
            //Plugin.LogInfo(LogType.Info, "LoadCourseData Start", true);
            AllSeriesData = new List<DaniSeries>(); // I'm not sure if this line is actually needed, or even detrimental
            AllSeriesData = LoadCourseData(Plugin.Instance.ConfigDaniDojoDataLocation.Value);
            //Plugin.LogInfo(LogType.Info, "LoadCourseData Finished", true);
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
                Plugin.LogInfo(LogType.Info, "Loading File \"" + files[i].Name + "\"");
                var text = File.ReadAllText(files[i].FullName);
                LWJson node = LWJson.Parse(text);
                var series = LoadSeries(node);
                allSeriesData.Add(series);
                Plugin.LogInfo(LogType.Info, "Loading File \"" + files[i].Name + "\" complete");
            }

            allSeriesData.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            return allSeriesData;
        }

        static DaniSeries LoadSeries(LWJson node)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Series start", true);
            if (node["danSeriesTitle"] != null)
            {
                var oldSeriesData = LoadOldSeries(node);
                return oldSeriesData;
            }
            DaniSeries seriesData = new DaniSeries();
            seriesData.Id = node["SeriesId"].AsString();
            seriesData.Title = node["SeriesTitle"].AsString();
            seriesData.IsActive = node["IsActive"].AsBoolean();
            seriesData.Order = node["Order"].AsInteger();
            var courses = node["Courses"].AsArray();
            for (int i = 0; i < courses.Count; i++)
            {
                var course = LoadCourse(courses[i], seriesData);
                seriesData.Courses.Add(course);
            }

            seriesData.Courses.Sort((x, y) => x.Order > y.Order ? 1 : -1);

            //Plugin.LogInfo(LogType.Info, "Loading Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadCourse(LWJson node, DaniSeries parent)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Course start", true);
            DaniCourse course = new DaniCourse();
            course.Id = node["Id"].AsString();
            course.Title = course.Id;
            course.JpTitle = string.Empty;
            course.EngTitle = string.Empty;
            if (node["Title"] != null)
            {
                course.Title = node["Title"].AsString();
            }
            if (node["JpTitle"] != null)
            {
                course.JpTitle = node["JpTitle"].AsString();
            }
            if (node["EngTitle"] != null)
            {
                course.EngTitle = node["EngTitle"].AsString();
            }
            course.Order = node["Order"].AsInteger();

            course.IsLocked = node["IsLocked"].AsBoolean();
            course.CourseLevel = GetCourseLevelFromTitle(course.Id);

            if (node["Background"] != null)
            {
                var background = node["Background"].AsString();
                switch (background)
                {
                    case "Tan": course.Background = CourseBackground.Tan; break;
                    case "Wood": course.Background = CourseBackground.Wood; break;
                    case "Blue": course.Background = CourseBackground.Blue; break;
                    case "Red": course.Background = CourseBackground.Red; break;
                    case "Silver": course.Background = CourseBackground.Silver; break;
                    case "Gold": course.Background = CourseBackground.Gold; break;
                    case "Gaiden": course.Background = CourseBackground.Gaiden; break;
                    default: course.Background = CourseBackground.Sousaku; break;
                }
                if (course.Background == CourseBackground.Gaiden)
                {
                    course.CourseLevel = DaniCourseLevel.gaiden;
                }
                else if (course.Background == CourseBackground.Sousaku)
                {
                    course.CourseLevel = DaniCourseLevel.sousaku;
                }
            }
            else
            {
                switch (course.CourseLevel)
                {
                    case DaniCourseLevel.kyuuFirst:
                    case DaniCourseLevel.kyuu10:
                    case DaniCourseLevel.kyuu9:
                    case DaniCourseLevel.kyuu8:
                    case DaniCourseLevel.kyuu7:
                    case DaniCourseLevel.kyuu6: course.Background = CourseBackground.Tan; break;
                    case DaniCourseLevel.kyuu5:
                    case DaniCourseLevel.kyuu4:
                    case DaniCourseLevel.kyuu3:
                    case DaniCourseLevel.kyuu2:
                    case DaniCourseLevel.kyuu1: course.Background = CourseBackground.Wood; break;
                    case DaniCourseLevel.dan1:
                    case DaniCourseLevel.dan2:
                    case DaniCourseLevel.dan3:
                    case DaniCourseLevel.dan4:
                    case DaniCourseLevel.dan5: course.Background = CourseBackground.Blue; break;
                    case DaniCourseLevel.dan6:
                    case DaniCourseLevel.dan7:
                    case DaniCourseLevel.dan8:
                    case DaniCourseLevel.dan9:
                    case DaniCourseLevel.dan10: course.Background = CourseBackground.Red; break;
                    case DaniCourseLevel.kuroto:
                    case DaniCourseLevel.meijin:
                    case DaniCourseLevel.chojin: course.Background = CourseBackground.Silver; break;
                    case DaniCourseLevel.tatsujin: course.Background = CourseBackground.Gold; break;
                    case DaniCourseLevel.gaiden: course.Background = CourseBackground.Gaiden; break;
                    case DaniCourseLevel.sousaku: course.Background = CourseBackground.Sousaku; break;
                    default: course.Background = CourseBackground.Sousaku; break;
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

            //Plugin.LogInfo(LogType.Info, "Loading Course finish", true);
            return course;
        }

        static DaniSongData LoadSongData(LWJson node)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Song Data start", true);
            DaniSongData song = new DaniSongData();
            song.SongId = node["SongId"].AsString();
            song.TitleEng = node["TitleEng"].AsString();
            song.TitleJp = node["TitleJp"].AsString();
            //song.Level = (EnsoData.EnsoLevelType)(node["Level"]!.AsInteger() - 1);

            string levelString = string.Empty;
            try
            {
                var levelInt = node["Level"].AsInteger();
                levelString = levelInt.ToString();
                //Plugin.LogInfo(LogType.Info, "Level int = " + levelInt, 2);
            }
            catch { }
            try
            {
                levelString = node["Level"].AsString();
                //Plugin.LogInfo(LogType.Info, "Level string = " + levelString, 2);
            }
            catch { }
            try
            {
                var levelInt = node["Course"].AsInteger();
                levelString = levelInt.ToString();
                //Plugin.LogInfo(LogType.Info, "Course int = " + levelInt, 2);
            }
            catch { }
            try
            {
                levelString = node["Course"].AsString();
                //Plugin.LogInfo(LogType.Info, "Course string = " + levelString, 2);
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
                    Plugin.LogInfo(LogType.Error, "Error reading Song Level of " + levelString + ", defaulted to Oni");
                    break;
            }

            song.IsHidden = node["IsHidden"].AsBoolean();
            //Plugin.LogInfo(LogType.Info, "Loading Song Data finish", true);
            return song;
        }

        static DaniBorder LoadBorder(LWJson node)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Border start", true);
            DaniBorder border = new DaniBorder();
            //Plugin.LogInfo(LogType.Info, "Loading BorderType", true);
            string borderTypeString = string.Empty;
            try
            {
                var borderTypeInt = node["BorderType"]!.AsInteger();
                borderTypeString = borderTypeInt.ToString();
                //Plugin.LogInfo(LogType.Info, "BorderType int = " + borderTypeInt, true);
            }
            catch { }
            try
            {
                borderTypeString = node["BorderType"]!.AsString();
                //Plugin.LogInfo(LogType.Info, "BorderType string = " + borderTypeString, true);
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
                    Plugin.LogInfo(LogType.Error, "Error reading BorderType of " + borderTypeString + ", defaulted to TotalHits");
                    break;
            }

            //Plugin.LogInfo(LogType.Info, "BorderType Loaded", true);
            var redBorder = node["RedBorder"];
            if (redBorder is LWJsonArray)
            {
                //Plugin.LogInfo(LogType.Info, "Border is Array", 2);
                var redBorderArray = redBorder.AsArray();
                var goldBorderArray = node["GoldBorder"].AsArray();
                for (int i = 0; i < redBorderArray.Count; i++)
                {
                    border.RedReqs.Add(redBorderArray[i].AsInteger());
                    border.GoldReqs.Add(goldBorderArray[i].AsInteger());
                }

                // If there's only 1 item, IsTotal = true
                border.IsTotal = redBorderArray.Count == 1;
            }
            else
            {
                //Plugin.LogInfo(LogType.Info, "Border is not Array", true);
                border.RedReqs.Add(node["RedBorder"].AsInteger());
                border.GoldReqs.Add(node["GoldBorder"].AsInteger());
                border.IsTotal = true;
            }
            //Plugin.LogInfo(LogType.Info, "Loading Border finish", true);
            return border;
        }


        static DaniSeries LoadOldSeries(LWJson node)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Old Series start", true);
            DaniSeries seriesData = new DaniSeries();
            seriesData.Title = node["danSeriesTitle"]!.AsString();
            seriesData.Id = node["danSeriesId"]!.AsString();
            seriesData.IsActive = node["isActiveDan"]!.AsBoolean();
            seriesData.Order = node["order"]!.AsInteger();
            var courses = node["courses"].AsArray();
            for (int j = 0; j < courses.Count; j++)
            {
                var course = LoadOldCourse(courses[j], seriesData);

                seriesData.Courses.Add(course);
            }
            seriesData.Courses.Sort((x, y) => x.Order > y.Order ? 1 : -1);
            //Plugin.LogInfo(LogType.Info, "Loading Old Series finish", true);
            return seriesData;
        }

        static DaniCourse LoadOldCourse(LWJson node, DaniSeries parent)
        {
            //Plugin.LogInfo(LogType.Info, "Loading Old Course start", true);
            DaniCourse course = new DaniCourse();

            course.Order = node["danId"]!.AsInteger();
            course.Id = course.Order.ToString();
            course.Title = node["title"]!.AsString();
            if (node["Background"] != null)
            {
                var background = node["Background"].AsString();
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

            course.CourseLevel = GetCourseLevelFromTitle(course.Title);

            // Check to see if "locked" is in the json for this course
            if (node["locked"] != null)
            {
                course.IsLocked = node["locked"]!.AsBoolean();
            }


            for (int i = 0; i < node["aryOdaiSong"]!.AsArray().Count; i++)
            {
                var songNode = node["aryOdaiSong"]![i]!;

                DaniSongData song = new DaniSongData();

                song.SongId = songNode["songNo"]!.AsString();
                song.Level = (EnsoData.EnsoLevelType)(songNode["level"]!.AsInteger() - 1);
                song.IsHidden = songNode["isHiddenSongName"]!.AsBoolean();

                course.Songs.Add(song);
            }

            for (int i = 0; i < node["aryOdaiBorder"]!.AsArray().Count; i++)
            {
                var borderNode = node["aryOdaiBorder"]![i]!;
                DaniBorder border = new DaniBorder();

                border.BorderType = (BorderType)borderNode["odaiType"]!.AsInteger();
                border.IsTotal = borderNode["borderType"]!.AsInteger() == 1;

                if (borderNode["redBorderTotal"] != null)
                {
                    border.RedReqs.Add(borderNode["redBorderTotal"]!.AsInteger());
                }
                else
                {
                    border.RedReqs.Add(borderNode["redBorder_1"]!.AsInteger());
                    border.RedReqs.Add(borderNode["redBorder_2"]!.AsInteger());
                    border.RedReqs.Add(borderNode["redBorder_3"]!.AsInteger());
                }

                if (borderNode["goldBorderTotal"] != null)
                {
                    border.GoldReqs.Add(borderNode["goldBorderTotal"]!.AsInteger());
                }
                else
                {
                    border.GoldReqs.Add(borderNode["goldBorder_1"]!.AsInteger());
                    border.GoldReqs.Add(borderNode["goldBorder_2"]!.AsInteger());
                    border.GoldReqs.Add(borderNode["goldBorder_3"]!.AsInteger());
                }

                course.Borders.Add(border);
            }

            course.Parent = parent;
            course.Hash = GetHashFromCourse(course);

            //Plugin.LogInfo(LogType.Info, "Loading Old Course finish", true);
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
            //Plugin.LogInfo(LogType.Error, "Could not find corresponding Course from Hash");
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
            //Plugin.LogInfo(LogType.Info, "course.Hash = " + course.Hash, true);
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
            //Plugin.LogInfo(LogType.Info, "course.Hash = " + course.Hash, true);

            return course.Hash;
        }

        static public int GetCourseIndex(DaniSeries series, DaniCourse course)
        {
            //Plugin.LogInfo(LogType.Info, "GetCourseIndex: Hash: " + course.Hash, true);
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
                //Plugin.LogInfo(LogType.Info, "GetNextUnlockedCourse: Checking " + nextCourse.Id, true);

                numAttempts--;
            } while (SaveDataManager.IsCourseLocked(series, nextCourse) && numAttempts > 0);
            if (numAttempts == 0)
            {
                nextCourse = series.Courses[0];
            }
            //Plugin.LogInfo(LogType.Info, "GetNextUnlockedCourse: Returning " + nextCourse.Id, true);
            return nextCourse;
        }

        static public DaniSeries GetNextSeries(DaniSeries currentSeries)
        {
            var seriesIndex = AllSeriesData.FindIndex((x) => x == currentSeries);
            seriesIndex++;
            if (seriesIndex >= AllSeriesData.Count)
            {
                seriesIndex = 0;
            }

            return AllSeriesData[seriesIndex];
        }

        static public DaniSeries GetPreviousSeries(DaniSeries currentSeries)
        {
            var seriesIndex = AllSeriesData.FindIndex((x) => x == currentSeries);
            seriesIndex--;
            if (seriesIndex < 0)
            {
                seriesIndex = AllSeriesData.Count - 1;
            }

            return AllSeriesData[seriesIndex];
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

        static public DaniSeries GetActiveSeries()
        {
            if (AllSeriesData.Count == 0)
            {
                return null;
            }
            for (int i = 0; i < AllSeriesData.Count; i++)
            {
                if (AllSeriesData[i].IsActive)
                {
                    return AllSeriesData[i];
                }
            }
            return AllSeriesData[0];
        }

        static public DaniCourseLevel GetCourseLevelFromTitle(string title)
        {
            switch (title.ToLower())
            {
                case "1stkyu":
                case "1stkyuu":
                case "初級 first kyu":
                    return DaniCourseLevel.kyuuFirst;
                case "10kyu":
                case "10kyuu":
                case "十級 10th kyu":
                    return DaniCourseLevel.kyuu10;
                case "9kyu":
                case "9kyuu":
                case "九級 9th kyu":
                    return DaniCourseLevel.kyuu9;
                case "8kyu":
                case "8kyuu":
                case "八級 8th kyu":
                    return DaniCourseLevel.kyuu8;
                case "7kyu":
                case "7kyuu":
                case "七級 7th kyu":
                    return DaniCourseLevel.kyuu7;
                case "6kyu":
                case "6kyuu":
                case "六級 6th kyu":
                    return DaniCourseLevel.kyuu6;
                case "5kyu":
                case "5kyuu":
                case "五級 5th kyu":
                    return DaniCourseLevel.kyuu5;
                case "4kyu":
                case "4kyuu":
                case "四級 4th kyu":
                    return DaniCourseLevel.kyuu4;
                case "3kyu":
                case "3kyuu":
                case "三級 3rd kyu":
                    return DaniCourseLevel.kyuu3;
                case "2kyu":
                case "2kyuu":
                case "二級 2nd kyu":
                    return DaniCourseLevel.kyuu2;
                case "1kyu":
                case "1kyuu":
                case "一級 1st kyu":
                    return DaniCourseLevel.kyuu1;
                case "1dan":
                case "初段 1st dan":
                    return DaniCourseLevel.dan1;
                case "2dan":
                case "二段 2nd dan":
                    return DaniCourseLevel.dan2;
                case "3dan":
                case "三段 3rd dan":
                    return DaniCourseLevel.dan3;
                case "4dan":
                case "四段 4th dan":
                    return DaniCourseLevel.dan4;
                case "5dan":
                case "五段 5th dan":
                    return DaniCourseLevel.dan5;
                case "6dan":
                case "六段 6th dan":
                    return DaniCourseLevel.dan6;
                case "7dan":
                case "七段 7th dan":
                    return DaniCourseLevel.dan7;
                case "8dan":
                case "八段 8th dan":
                    return DaniCourseLevel.dan8;
                case "9dan":
                case "九段 9th dan":
                    return DaniCourseLevel.dan9;
                case "10dan":
                case "十段 10th dan":
                    return DaniCourseLevel.dan10;
                case "11dan":
                case "kuroto":
                case "玄人 kuroto":
                    return DaniCourseLevel.kuroto;
                case "12dan":
                case "meijin":
                case "名人 meijin":
                    return DaniCourseLevel.meijin;
                case "13dan":
                case "chojin":
                case "超人 chojin":
                    return DaniCourseLevel.chojin;
                case "14dan":
                case "tatsujin":
                case "達人 tatsujin":
                    return DaniCourseLevel.tatsujin;
                default:
                    return DaniCourseLevel.sousaku;
            }
        }

    }
}
