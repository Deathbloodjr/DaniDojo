using DaniDojo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace DaniDojo.Assets
{
    public enum DigitType
    {
        BigReqBorder,
        BigReqTransparent,
        BigReqFill,
        SmallReqBorder,
        SmallReqTransparent,
        SmallReqFill,
        ResultsBlack,
        ResultsScore,
        ResultsScoreShadow,
    }

    internal class CommonAssets
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        static public GameObject CreateSongTitleChild(GameObject parent, Vector2 position, DaniSongData song, bool isHidden = false)
        {
            var wordDataMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr;
            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

            // Using Hoshikuzu struck as a base, assuming it will have accurate font data
            // It might not have accurate font data for detail
            var titleFontType = wordDataMgr.GetWordListInfo("song_struck").FontType;
            var titleFont = fontTMPMgr.GetDefaultFontAsset(titleFontType);
            var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(titleFontType, DataConst.DefaultFontMaterialType.KanbanSelect);

            var songTitleInfo = wordDataMgr.GetWordListInfo("song_" + song.SongId);
            string songTitle;

            if (songTitleInfo != null && songTitleInfo.Text != "")
            {
                songTitle = songTitleInfo.Text;
            }
            else
            {
                songTitle = "Song not found: ";
                if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Jp")
                {
                    songTitle += song.TitleJp;
                }
                else if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Eng")
                {
                    songTitle += song.TitleEng;
                }
                else
                {
                    songTitle += song.SongId;
                }
            }

            if (isHidden)
            {
                songTitle = "? ? ?";
            }

            Rect rect = new Rect(position, new Vector2(1920, 40));

            var songTitleObject = AssetUtility.CreateTextChild(parent, "SongTitle", rect, songTitle);
            AssetUtility.SetTextFontAndMaterial(songTitleObject, titleFont, titleFontMaterial);
            AssetUtility.SetTextAlignment(songTitleObject, HorizontalAlignmentOptions.Left);

            return songTitleObject;
        }

        static public GameObject CreateSongDetailChild(GameObject parent, Vector2 position, DaniSongData song, bool isHidden = false)
        {
            var wordDataMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr;
            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

            // Using Hoshikuzu struck as a base, assuming it will have accurate font data
            // It might not have accurate font data for detail
            var detailFontType = wordDataMgr.GetWordListInfo("song_detail_struck").FontType;
            var detailFont = fontTMPMgr.GetDefaultFontAsset(detailFontType);
            var detailFontMaterial = fontTMPMgr.GetDefaultFontMaterial(detailFontType, DataConst.DefaultFontMaterialType.Plane);

            var songDetailInfo = wordDataMgr.GetWordListInfo("song_detail_" + song.SongId);
            string songDetail = "";

            if (songDetailInfo != null)
            {
                songDetail = songDetailInfo.Text;
            }
            if (isHidden)
            {
                songDetail = "";
            }

            Rect rect = new Rect(position, new Vector2(1920, 20));

            var songDetailObject = AssetUtility.CreateTextChild(parent, "SongDetail", rect, songDetail);
            AssetUtility.SetTextFontAndMaterial(songDetailObject, detailFont, detailFontMaterial);
            AssetUtility.SetTextAlignment(songDetailObject, HorizontalAlignmentOptions.Left);
            AssetUtility.SetTextColor(songDetailObject, Color.black);

            return songDetailObject;
        }

        static public GameObject CreateSongCourseChild(GameObject parent, Vector2 position, DaniSongData song)
        {
            string file = string.Empty;
            switch (song.Level)
            {
                case EnsoData.EnsoLevelType.Easy: file = "CourseEasy.png"; break;
                case EnsoData.EnsoLevelType.Normal: file = "CourseNormal.png"; break;
                case EnsoData.EnsoLevelType.Hard: file = "CourseHard.png"; break;
                case EnsoData.EnsoLevelType.Mania: file = "CourseOni.png"; break;
                case EnsoData.EnsoLevelType.Ura: file = "CourseUra.png"; break;
                default: file = "CourseOni.png"; break;
            }
            return AssetUtility.CreateImageChild(parent, "SongCourse", position, Path.Combine(AssetFilePath, "DifficultyAssets", file));
        }

        static public GameObject CreateSongLevelChild(GameObject parent, Vector2 position, DaniSongData song)
        {
            List<MusicDataInterface.MusicInfoAccesser> musicInfoAccessers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers;

            var musicInfo = musicInfoAccessers.Find((x) => x.Id == song.SongId);
            string file = string.Empty;
            if (musicInfo == null)
            {
                file = "Star10Plus.png";
            }
            else
            {
                switch (musicInfo.Stars[(int)song.Level])
                {
                    case 1: file = "Star1.png"; break;
                    case 2: file = "Star2.png"; break;
                    case 3: file = "Star3.png"; break;
                    case 4: file = "Star4.png"; break;
                    case 5: file = "Star5.png"; break;
                    case 6: file = "Star6.png"; break;
                    case 7: file = "Star7.png"; break;
                    case 8: file = "Star8.png"; break;
                    case 9: file = "Star9.png"; break;
                    case 10: file = "Star10.png"; break;
                    default: file = "Star10Plus.png"; break;
                }
            }
            return AssetUtility.CreateImageChild(parent, "SongLevel", position, Path.Combine(AssetFilePath, "DifficultyAssets", file));
        }

        static public GameObject CreateCourseTitleBar(GameObject parent, Vector2 position, DaniCourse course)
        {
            if (course.CourseLevel != DaniCourseLevel.gaiden &&
                course.CourseLevel != DaniCourseLevel.sousaku)
            {
                return null;
            }

            string bgImage = string.Empty;
            if (course.CourseLevel == DaniCourseLevel.gaiden)
            {
                bgImage = "GaidenTitleBg.png";
            }
            else if (course.CourseLevel == DaniCourseLevel.sousaku)
            {
                bgImage = "SousakuTitleBg.png";
            }

            var titleBarObject = AssetUtility.CreateImageChild(parent, "CourseTitleBar", position, Path.Combine("Course", "CourseSelect", bgImage));

            var wordDataMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr;
            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

            // Using Hoshikuzu struck as a base, assuming it will have accurate font data
            // It might not have accurate font data for detail
            var titleFontType = wordDataMgr.GetWordListInfo("song_struck").FontType;
            var titleFont = fontTMPMgr.GetDefaultFontAsset(titleFontType);
            var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(titleFontType, DataConst.DefaultFontMaterialType.KanbanSelect);

            string courseTitle = course.Title;
            if ((Plugin.Instance.ConfigSongTitleLanguage.Value == "Jp" || course.EngTitle == "") && course.JpTitle != "")
            {
                courseTitle = course.JpTitle;
            }
            else if ((Plugin.Instance.ConfigSongTitleLanguage.Value == "Eng" || course.JpTitle == "") && course.EngTitle != "")
            {
                courseTitle = course.EngTitle;
            }

            var titleText = AssetUtility.CreateTextChild(titleBarObject, "Title", new Rect(72, 6, 950, 70), courseTitle);
            AssetUtility.SetTextFontAndMaterial(titleText, titleFont, titleFontMaterial);
            AssetUtility.SetTextAlignment(titleText, HorizontalAlignmentOptions.Left);
            AssetUtility.SetTextFontSize(titleText, 41);

            return titleBarObject;
        }

        static public GameObject CreateDaniCourse(GameObject parent, Vector2 position, DaniCourse course)
        {
            string bgImageFile = course.Background switch
            {
                CourseBackground.Tan => "Tan.png",
                CourseBackground.Wood => "Wood.png",
                CourseBackground.Blue => "Blue.png",
                CourseBackground.Red => "Red.png",
                CourseBackground.Silver => "Silver.png",
                CourseBackground.Gold => "Gold.png",
                CourseBackground.Gaiden => "Gaiden.png",
                CourseBackground.Sousaku => "Sousaku.png",
                _ => "Sousaku.png",
            };

            string topJpText = string.Empty;
            string botJpText = string.Empty;
            string topEngText = string.Empty;
            string botEngText = string.Empty;

            switch (course.CourseLevel)
            {
                case DaniCourseLevel.kyuuFirst:
                case DaniCourseLevel.dan1:
                    topJpText = "Starting.png";
                    topEngText = "First.png";
                    break;
                case DaniCourseLevel.kyuu10:
                case DaniCourseLevel.dan10:
                    topJpText = "10th.png";
                    topEngText = "Tenth.png";
                    break;
                case DaniCourseLevel.kyuu9:
                case DaniCourseLevel.dan9:
                    topJpText = "9th.png";
                    topEngText = "Ninth.png";
                    break;
                case DaniCourseLevel.kyuu8:
                case DaniCourseLevel.dan8:
                    topJpText = "8th.png";
                    topEngText = "Eighth.png";
                    break;
                case DaniCourseLevel.kyuu7:
                case DaniCourseLevel.dan7:
                    topJpText = "7th.png";
                    topEngText = "Seventh.png";
                    break;
                case DaniCourseLevel.kyuu6:
                case DaniCourseLevel.dan6:
                    topJpText = "6th.png";
                    topEngText = "Sixth.png";
                    break;
                case DaniCourseLevel.kyuu5:
                case DaniCourseLevel.dan5:
                    topJpText = "5th.png";
                    topEngText = "Fifth.png";
                    break;
                case DaniCourseLevel.kyuu4:
                case DaniCourseLevel.dan4:
                    topJpText = "4th.png";
                    topEngText = "Fourth.png";
                    break;
                case DaniCourseLevel.kyuu3:
                case DaniCourseLevel.dan3:
                    topJpText = "3rd.png";
                    topEngText = "Third.png";
                    break;
                case DaniCourseLevel.kyuu2:
                case DaniCourseLevel.dan2:
                    topJpText = "2nd.png";
                    topEngText = "Second.png";
                    break;
                case DaniCourseLevel.kyuu1:
                    topJpText = "1st.png";
                    topEngText = "First.png";
                    break;
                case DaniCourseLevel.kuroto:
                    topJpText = "kuro.png";
                    topEngText = "Kuroto.png";
                    break;
                case DaniCourseLevel.meijin:
                    topJpText = "mei.png";
                    topEngText = "Meijin.png";
                    break;
                case DaniCourseLevel.chojin:
                    topJpText = "cho.png";
                    topEngText = "Chojin.png";
                    break;
                case DaniCourseLevel.tatsujin:
                    topJpText = "tatsu.png";
                    topEngText = "Tatsujin.png";
                    break;
                case DaniCourseLevel.gaiden:
                    topJpText = "gai.png";
                    botJpText = "den.png";
                    topEngText = "Gaiden.png";
                    break;
                default:
                    topJpText = "sou.png";
                    botJpText = "saku.png";
                    topEngText = "Sousaku.png";
                    break;
            }

            switch (course.CourseLevel)
            {
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
                case DaniCourseLevel.kyuu1:
                    botJpText = "kyuu.png";
                    botEngText = "Kyu.png";
                    break;
                case DaniCourseLevel.dan1:
                case DaniCourseLevel.dan2:
                case DaniCourseLevel.dan3:
                case DaniCourseLevel.dan4:
                case DaniCourseLevel.dan5:
                case DaniCourseLevel.dan6:
                case DaniCourseLevel.dan7:
                case DaniCourseLevel.dan8:
                case DaniCourseLevel.dan9:
                case DaniCourseLevel.dan10:
                    botJpText = "dan.png";
                    botEngText = "Dan.png";
                    break;
                case DaniCourseLevel.kuroto:
                case DaniCourseLevel.meijin:
                case DaniCourseLevel.chojin:
                case DaniCourseLevel.tatsujin:
                    botJpText = "jin.png";
                    break;
            }


            var daniCourseParent = AssetUtility.CreateEmptyObject(parent, "DaniCourse", position);
            AssetUtility.CreateImageChild(daniCourseParent, "Background", new Vector2(0, -1), Path.Combine("Course", "Main", "Bg", bgImageFile));
            var textParent = AssetUtility.GetOrCreateEmptyChild(daniCourseParent, "Text", new Vector2(52, 124));
            AssetUtility.CreateImageChild(textParent, "TopJpText", new Vector2(12, 225), Path.Combine("Course", "Main", "JpText", topJpText));
            AssetUtility.CreateImageChild(textParent, "BotJpText", new Vector2(12, 93), Path.Combine("Course", "Main", "JpText", botJpText));
            if (botEngText == string.Empty)
            {
                AssetUtility.CreateImageChild(textParent, "EngText", new Vector2(0, 16), Path.Combine("Course", "Main", "EngText", topEngText));
            }
            else
            {
                AssetUtility.CreateImageChild(textParent, "TopEngText", new Vector2(0, 30), Path.Combine("Course", "Main", "EngText", topEngText));
                AssetUtility.CreateImageChild(textParent, "BotEngText", new Vector2(0, 00), Path.Combine("Course", "Main", "EngText", botEngText));
            }

            return daniCourseParent;
        }


        static public GameObject CreateDigit(GameObject parent, string name, Vector2 position, DigitType type, char number)
        {
            string digitPath = GetDigitFilePath(type, number.ToString());
            return AssetUtility.CreateImageChild(parent, name, position, digitPath);
        }

        static public void ChangeDigit(GameObject gameObject, DigitType type, string number)
        {
            string digitPath = GetDigitFilePath(type, number);
            AssetUtility.ChangeImageSprite(gameObject, digitPath);
        }

        static private string GetDigitFilePath(DigitType type, string number)
        {
            // It might be more complicated than this
            // type.ToString() is just an oversimplified guess
            return Path.Combine(AssetFilePath, "Digits", type.ToString(), number + ".png");
        }

        // This function almost feels pointless
        static public void ChangeDigitColor(GameObject gameObject, Color color)
        {
            AssetUtility.ChangeImageColor(gameObject, color);
        }
    }
}
