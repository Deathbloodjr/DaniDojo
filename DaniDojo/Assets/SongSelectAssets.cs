using DaniDojo.Data;
using DaniDojo.Managers;
using SongSelect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Assets
{
    internal class SongSelectAssets
    {
        #region SongSelect
        /// <summary>
        /// These are the songIds that are in the active Dani Dojo Course(s)
        /// If one of these is found, then search to see which course it's in
        /// If it's in multiple, show the highest (I guess, Re:End is the only song that's been in multiple, so I need to check to see how that worked)
        /// </summary>
        private static List<string> ActiveSongIds = new List<string>();


        public static void PopulateActiveSongIds()
        {
            ActiveSongIds.Clear();
            var activeSeries = CourseDataManager.GetActiveSeries();
            if (activeSeries != null)
            {
                for (int i = 0; i < activeSeries.Courses.Count; i++)
                {
                    var course = activeSeries.Courses[i];
                    var save = SaveDataManager.GetCourseRecord(course.Hash);
                    for (int j = 0; j < course.Songs.Count; j++)
                    {
                        var song = course.Songs[j];
                        if (!ActiveSongIds.Contains(song.SongId))
                        {
                            // Don't show the song if it's hidden and the player hasn't reached it yet
                            if (save.SongReached <= j && song.IsHidden)
                            {
                                continue;
                            }
                            ActiveSongIds.Add(song.SongId);
                        }
                    }
                }
            }
        }


        public static void UpdateSongSelectDaniDojoIcons(SongSelectKanban kanban, SongSelectManager.Song song)
        {
            if (kanban.name == "Kanban1")
            {
                UpdateSelectedKanbanIcons(kanban, song);
                return;
            }

            // Find/create the parent object
            var daniDojoIconTransform = kanban.transform.Find("DaniDojoUnselectedIcon");
            GameObject daniDojoObj = null;
            if (daniDojoIconTransform == null)
            {
                daniDojoObj = AssetUtility.CreateEmptyObject(kanban.gameObject, "DaniDojoUnselectedIcon", new Vector2(-70, 30));
                var scale = 0.751f;
                daniDojoObj.transform.localScale = new Vector2(scale, scale);
            }
            else
            {
                daniDojoObj = daniDojoIconTransform.gameObject;
            }

            // Set the image appropriately
            if (ActiveSongIds.Contains(song.Id))
            {
                daniDojoObj.SetActive(true);

                var course = GetHighestCourseLevelFromSongId(song.Id);
                var imageName = GetBackgroundFromCourseLevel(course.courseLevel);
                var diffName = GetDifficultyFromSongLevel(course.songLevel);
                var textName = GetTextImageNameFromCourseLevel(course.courseLevel);

                var bgImageObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoUnselectedBg", new Vector2(0, 0), Path.Combine("SongSelect", "Unselected", "Backgrounds", imageName));
                var diffObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoUnselectedDiff", new Vector2(19, 17), Path.Combine("SongSelect", "Unselected", "Difficulties", diffName));
                var textObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoUnselectedText", new Vector2(18, 55), Path.Combine("SongSelect", "Unselected", "Text", textName));
            }
            else
            {
                daniDojoObj.SetActive(false);
            }
        }

        private static void UpdateSelectedKanbanIcons(SongSelectKanban kanban, SongSelectManager.Song song)
        {
            if (kanban.name != "Kanban1")
            {
                return;
            }

            // Find/create the parent object
            var daniDojoIconTransform = kanban.transform.Find("DaniDojoSelectedIcon");
            GameObject daniDojoObj = null;
            if (daniDojoIconTransform == null)
            {
                daniDojoObj = AssetUtility.CreateEmptyObject(kanban.gameObject, "DaniDojoSelectedIcon", new Vector2(364, 190));
                var scale = 0.751f;
                daniDojoObj.transform.localScale = new Vector2(scale, scale);
            }
            else
            {
                daniDojoObj = daniDojoIconTransform.gameObject;
            }

            // Set the image appropriately
            if (ActiveSongIds.Contains(song.Id))
            {
                daniDojoObj.SetActive(true);

                var course = GetHighestCourseLevelFromSongId(song.Id);
                var imageName = GetBackgroundFromCourseLevel(course.courseLevel);
                var diffName = GetDifficultyFromSongLevel(course.songLevel);
                var textName = GetTextImageNameFromCourseLevel(course.courseLevel);

                var bgImageObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoSelectedBg", new Vector2(0, 0), Path.Combine("SongSelect", "Selected", "Backgrounds", imageName));
                var diffObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoSelectedDiff", new Vector2(146, 42), Path.Combine("SongSelect", "Selected", "Difficulties", diffName));
                var textObj = AssetUtility.GetOrCreateImageChild(daniDojoObj, "DaniDojoSelectedText", new Vector2(56, 46), Path.Combine("SongSelect", "Selected", "Text", textName));
            }
            else
            {
                daniDojoObj.SetActive(false);
            }
        }



        private static (DaniCourseLevel courseLevel, EnsoData.EnsoLevelType songLevel) GetHighestCourseLevelFromSongId(string songId)
        {
            if (!ActiveSongIds.Contains(songId))
            {
                return (DaniCourseLevel.None, EnsoData.EnsoLevelType.Mania);
            }

            var activeSeries = CourseDataManager.GetActiveSeries();
            for (int i = activeSeries.Courses.Count - 1; i >= 0; i--)
            {
                var course = activeSeries.Courses[i];
                var save = SaveDataManager.GetCourseRecord(course.Hash);
                for (int j = 0; j < course.Songs.Count; j++)
                {
                    var song = course.Songs[j];
                    if (songId == song.SongId)
                    {
                        // Don't show the song if it's hidden and the player hasn't reached it yet
                        if (save.SongReached <= j && song.IsHidden)
                        {
                            continue;
                        }
                        return (course.CourseLevel, song.Level);
                    }
                }
            }
            return (DaniCourseLevel.None, EnsoData.EnsoLevelType.Mania);
        }

        private static Dictionary<EnsoData.EnsoLevelType, DaniCourseLevel> GetAllCourseLevelsFromSongId(string songId)
        {
            var dict = new Dictionary<EnsoData.EnsoLevelType, DaniCourseLevel>();
            if (!ActiveSongIds.Contains(songId))
            {
                return dict;
            }

            var activeSeries = CourseDataManager.GetActiveSeries();
            for (int i = activeSeries.Courses.Count - 1; i >= 0; i--)
            {
                var course = activeSeries.Courses[i];
                var save = SaveDataManager.GetCourseRecord(course.Hash);
                for (int j = 0; j < course.Songs.Count; j++)
                {
                    var song = course.Songs[j];
                    if (songId == song.SongId)
                    {
                        if (save.SongReached <= j && song.IsHidden)
                        {
                            continue;
                        }
                        if (!dict.ContainsKey(song.Level))
                        {
                            dict.Add(song.Level, course.CourseLevel);
                        }
                    }
                }
            }
            return dict;
        }


        #endregion


        #region CourseSelect
        static MusicDataInterface.MusicInfoAccesser currentSong;

        static Dictionary<EnsoData.EnsoLevelType, GameObject> GetCourseButtonObjects()
        {
            var CourseButtonObjects = new Dictionary<EnsoData.EnsoLevelType, GameObject>();

            var courseSelectObject = GameObject.Find("CourseSelect");
            if (courseSelectObject != null)
            {
                var songSelectCourse = AssetUtility.GetChildByName(courseSelectObject, "SongSelectCourse");
                if (songSelectCourse != null)
                {
                    var kanban = AssetUtility.GetChildByName(songSelectCourse, "Kanban");
                    if (kanban != null)
                    {
                        var diffcourse = AssetUtility.GetChildByName(kanban, "DiffCourse");
                        if (diffcourse != null)
                        {
                            for (int i = 1; i < 6; i++)
                            {
                                var btnDiffCourse = AssetUtility.GetChildByName(diffcourse, "BtnDiffCourse" + i);
                                if (btnDiffCourse != null)
                                {
                                    var level = (EnsoData.EnsoLevelType)(i - 1);
                                    if (!CourseButtonObjects.ContainsKey(level))
                                    {
                                        CourseButtonObjects.Add(level, AssetUtility.GetChildByName(btnDiffCourse, "DiffCourse"));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return CourseButtonObjects;
        }

        public static void SetCurrentSong(MusicDataInterface.MusicInfoAccesser song)
        {
            currentSong = song;
        }

        public static void AddDaniDojoIconToCourseSelect()
        {
            var courseButtonObjects = GetCourseButtonObjects();
            var songCourseLevels = GetAllCourseLevelsFromSongId(currentSong.Id);

            foreach (var button in courseButtonObjects)
            {
                var daniDojoParent = AssetUtility.GetOrCreateEmptyChild(button.Value, "DaniDojoIcon", new Vector2(148, 169));
                var scale = 0.751f;
                daniDojoParent.transform.localScale = new Vector2(scale, scale);
                var icon = daniDojoParent.GetOrAddComponent<DaniDojoCourseSelectIcon>();
                var course = DaniCourseLevel.None;
                if (songCourseLevels.ContainsKey(button.Key))
                {
                    course = songCourseLevels[button.Key];
                }
                icon.Initialize(course);
            }

            //foreach (var courseLevel in songCourseLevels)
            //{
            //    var button = courseButtonObjects[courseLevel.Key];
            //    if (button != null)
            //    {
            //        var daniDojoParent = AssetUtility.GetOrCreateEmptyChild(button, "DaniDojoIcon", new Vector2(148, 169));
            //        var scale = 0.751f;
            //        daniDojoParent.transform.localScale = new Vector2(scale, scale);
            //        var icon = daniDojoParent.GetOrAddComponent<DaniDojoCourseSelectIcon>();
            //        icon.Initialize(courseLevel.Value);
            //    }
            //}
        }

        #endregion




        internal static string GetBackgroundFromCourseLevel(DaniCourseLevel level)
        {
            switch (level)
            {
                case DaniCourseLevel.None: return "";
                case DaniCourseLevel.kyuuFirst:
                case DaniCourseLevel.kyuu10:
                case DaniCourseLevel.kyuu9:
                case DaniCourseLevel.kyuu8:
                case DaniCourseLevel.kyuu7:
                case DaniCourseLevel.kyuu6:
                case DaniCourseLevel.kyuu5:
                case DaniCourseLevel.kyuu4:
                case DaniCourseLevel.kyuu3:
                case DaniCourseLevel.kyuu2:
                case DaniCourseLevel.kyuu1: return "KyuuBg.png"; 
                case DaniCourseLevel.dan1:
                case DaniCourseLevel.dan2:
                case DaniCourseLevel.dan3:
                case DaniCourseLevel.dan4:
                case DaniCourseLevel.dan5: return "BlueBg.png"; 
                case DaniCourseLevel.dan6:
                case DaniCourseLevel.dan7:
                case DaniCourseLevel.dan8:
                case DaniCourseLevel.dan9:
                case DaniCourseLevel.dan10: return "RedBg.png"; 
                case DaniCourseLevel.kuroto:
                case DaniCourseLevel.meijin:
                case DaniCourseLevel.chojin: return "SilverBg.png"; 
                case DaniCourseLevel.tatsujin: return "GoldBg.png"; 
                case DaniCourseLevel.gaiden:
                case DaniCourseLevel.sousaku:
                default: return "";
            }
        }

        private static string GetDifficultyFromSongLevel(EnsoData.EnsoLevelType songLevel)
        {
            switch (songLevel)
            {
                case EnsoData.EnsoLevelType.Easy: return "Easy.png";
                case EnsoData.EnsoLevelType.Normal: return "Normal.png";
                case EnsoData.EnsoLevelType.Hard: return "Hard.png";
                case EnsoData.EnsoLevelType.Mania: return "Oni.png";
                case EnsoData.EnsoLevelType.Ura: return "Ura.png";
                default: return "";
            }
        }

        internal static string GetTextImageNameFromCourseLevel(DaniCourseLevel level)
        {
            switch (level)
            {
                case DaniCourseLevel.None: return "";
                case DaniCourseLevel.kyuuFirst: return "";
                case DaniCourseLevel.kyuu10: return "";
                case DaniCourseLevel.kyuu9: return "";
                case DaniCourseLevel.kyuu8: return "";
                case DaniCourseLevel.kyuu7: return "";
                case DaniCourseLevel.kyuu6: return "";
                case DaniCourseLevel.kyuu5: return "5kyuu.png";
                case DaniCourseLevel.kyuu4: return "4kyuu.png";
                case DaniCourseLevel.kyuu3: return "3kyuu.png";
                case DaniCourseLevel.kyuu2: return "2kyuu.png";
                case DaniCourseLevel.kyuu1: return "1kyuu.png";
                case DaniCourseLevel.dan1: return "1dan.png";
                case DaniCourseLevel.dan2: return "2dan.png";
                case DaniCourseLevel.dan3: return "3dan.png";
                case DaniCourseLevel.dan4: return "4dan.png";
                case DaniCourseLevel.dan5: return "5dan.png";
                case DaniCourseLevel.dan6: return "6dan.png";
                case DaniCourseLevel.dan7: return "7dan.png";
                case DaniCourseLevel.dan8: return "8dan.png";
                case DaniCourseLevel.dan9: return "9dan.png";
                case DaniCourseLevel.dan10: return "10dan.png";
                case DaniCourseLevel.kuroto: return "kuroto.png";
                case DaniCourseLevel.meijin: return "meijin.png";
                case DaniCourseLevel.chojin: return "chojin.png";
                case DaniCourseLevel.tatsujin: return "tatsujin.png";
                case DaniCourseLevel.gaiden:
                case DaniCourseLevel.sousaku:
                default: return "";
            }
        }
    }
}
