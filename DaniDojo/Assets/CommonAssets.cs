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

            if (songTitleInfo != null)
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

        static public GameObject CreateDaniCourse(GameObject parent, Vector2 position, DaniCourse course)
        {
            string bgImageFile = course.Background switch
            {
                CourseBackground.Tan => bgImageFile = "TanBg.png",
                CourseBackground.Wood => bgImageFile = "WoodBg.png",
                CourseBackground.Blue => bgImageFile = "BlueBg.png",
                CourseBackground.Red => bgImageFile = "RedBg.png",
                CourseBackground.Silver => bgImageFile = "SilverBg.png",
                CourseBackground.Gold => bgImageFile = "GoldBg.png",
                _ => bgImageFile = "TanBg.png",
            };

            var courseId = course.Id;
            if (int.TryParse(courseId, out int _))
            {
                courseId = course.Title;
            }

            string textImageBg = courseId switch
            {
                "5kyuu" or "五級 5th Kyu" => textImageBg = "kyuu5.png",
                "4kyuu" or "四級 4th Kyu" => textImageBg = "kyuu4.png",
                "3kyuu" or "三級 3rd Kyu" => textImageBg = "kyuu3.png",
                "2kyuu" or "二級 2nd Kyu" => textImageBg = "kyuu2.png",
                "1kyuu" or "一級 1st Kyu" => textImageBg = "kyuu1.png",
                "1dan" or "初段 1st Dan" => textImageBg = "dan1.png",
                "2dan" or "二段 2nd Dan" => textImageBg = "dan2.png",
                "3dan" or "三段 3rd Dan" => textImageBg = "dan3.png",
                "4dan" or "四段 4th Dan" => textImageBg = "dan4.png",
                "5dan" or "五段 5th Dan" => textImageBg = "dan5.png",
                "6dan" or "六段 6th Dan" => textImageBg = "dan6.png",
                "7dan" or "七段 7th Dan" => textImageBg = "dan7.png",
                "8dan" or "八段 8th Dan" => textImageBg = "dan8.png",
                "9dan" or "九段 9th Dan" => textImageBg = "dan9.png",
                "10dan" or "十段 10th Dan" => textImageBg = "dan10.png",
                "11dan" or "玄人 Kuroto" => textImageBg = "kuroto.png",
                "12dan" or "名人 Meijin" => textImageBg = "meijin.png",
                "13dan" or "超人 Chojin" => textImageBg = "chojin.png",
                "14dan" or "達人 Tatsujin" => textImageBg = "tatsujin.png",
                _ => textImageBg = "gaiden.png",
            };

            var daniCourseParent = AssetUtility.CreateEmptyObject(parent, "DaniCourse", position);
            AssetUtility.CreateImageChild(daniCourseParent, "Background", new Vector2(0, 0), Path.Combine(AssetFilePath, "Course", "DaniCourseIcons", bgImageFile));
            AssetUtility.CreateImageChild(daniCourseParent, "Text", new Vector2(52, 124), Path.Combine(AssetFilePath, "Course", "DaniCourseIcons", textImageBg));

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
