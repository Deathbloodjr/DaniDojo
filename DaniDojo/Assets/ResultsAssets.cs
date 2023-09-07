using DaniDojo.Data;
using DaniDojo.Managers;
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
    internal class ResultsAssets
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        static public GameObject CreateBg(GameObject parent)
        {
            return AssetUtility.CreateImageChild(parent, "DaniResultBg", new Vector2(0, 0), Path.Combine(AssetFilePath, "Results", "Background.png"));
        }

        static public GameObject CreateCourseIcon(GameObject parent, DaniCourse course)
        {
            return CommonAssets.CreateDaniCourse(parent, new Vector2(50, 400 + 1920), course);
        }

        static public GameObject CreateSongPanel(GameObject parent)
        {
            var songBg = AssetUtility.CreateImageChild(parent, "SongMainBg", new Vector2(352, 69), Path.Combine(AssetFilePath, "Results", "SongsWoodBackground.png"));
            //CreateEachSongBg(songBg);
            return songBg;
        }

        static public void CreateEachSongBg(GameObject parent, DaniCourse course, PlayData play, SaveCourse save)
        {
            for (int i = 0; i < Math.Min(course.Songs.Count, 3); i++)
            {
                int x = 28;
                int y = 607 - (i * 276);
                var songBg = AssetUtility.CreateImageChild(parent, "SongBg", new Vector2(x, y), Path.Combine(AssetFilePath, "Results", "SongBg.png"));
                var songPanel = AssetUtility.CreateImageChild(songBg, "SongPanel" + i, new Vector2(38, 119), Path.Combine(AssetFilePath, "Results", "SongPanel.png"));

                AssetUtility.CreateImageChild(songPanel, "SongIndicator", new Vector2(10, 10), Path.Combine(AssetFilePath, "Results", "SongIndicator" + (i + 1) + ".png"));

                CommonAssets.CreateSongCourseChild(songPanel, new Vector2(112, 42), course.Songs[i]);
                CommonAssets.CreateSongLevelChild(songPanel, new Vector2(121, 15), course.Songs[i]);

                var songTitle = CommonAssets.CreateSongTitleChild(songPanel, new Vector2(220, 28), course.Songs[i], Math.Max(play.SongReached, save.SongReached) <= i);
                var songDetail = CommonAssets.CreateSongDetailChild(songPanel, new Vector2(220, 70), course.Songs[i], Math.Max(play.SongReached, save.SongReached) <= i);

                int valuesX = 136;
                int valuesY = 37;
                int valuesInterval = 317;
                var songGoodsPanel = AssetUtility.CreateImageChild(songBg, "SongGoodsPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongGoodsBg.png"));
                valuesX += valuesInterval;
                var songOksPanel = AssetUtility.CreateImageChild(songBg, "SongOksPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongOksBg.png"));
                valuesX += valuesInterval;
                var songBadsPanel = AssetUtility.CreateImageChild(songBg, "SongBadsPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongBadsBg.png"));
                valuesX += valuesInterval;
                var songDrumrollsPanel = AssetUtility.CreateImageChild(songBg, "SongDrumrollPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongDrumrollBg.png"));

                var goods = play.SongPlayData[i].Goods.ToString();
                var oks = play.SongPlayData[i].Oks.ToString();
                var bads = play.SongPlayData[i].Bads.ToString();
                var drumroll = play.SongPlayData[i].Drumroll.ToString();

                Vector2 basePosition = new Vector2(279, 13);
                for (int j = 0; j < goods.Length; j++)
                {
                    Vector2 digitPosition = basePosition + new Vector2(-28 * (goods.Length - (j + 1)), 0);
                    CommonAssets.CreateDigit(songGoodsPanel, "Goods" + (goods.Length - (j + 1)), digitPosition, DigitType.ResultsBlack, goods[j]);
                }
                for (int j = 0; j < oks.Length; j++)
                {
                    Vector2 digitPosition = basePosition + new Vector2(-28 * (oks.Length - (j + 1)), 0);
                    CommonAssets.CreateDigit(songOksPanel, "Oks" + (oks.Length - (j + 1)), digitPosition, DigitType.ResultsBlack, oks[j]);
                }
                for (int j = 0; j < bads.Length; j++)
                {
                    Vector2 digitPosition = basePosition + new Vector2(-28 * (bads.Length - (j + 1)), 0);
                    CommonAssets.CreateDigit(songBadsPanel, "Bads" + (bads.Length - (j + 1)), digitPosition, DigitType.ResultsBlack, bads[j]);
                }
                for (int j = 0; j < drumroll.Length; j++)
                {
                    Vector2 digitPosition = basePosition + new Vector2(-28 * (drumroll.Length - (j + 1)), 0);
                    CommonAssets.CreateDigit(songDrumrollsPanel, "Drumroll" + (drumroll.Length - (j + 1)), digitPosition, DigitType.ResultsBlack, drumroll[j]);
                }
            }
        }

        static public GameObject CreatePlayRecordBg(GameObject parent)
        {
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecord", new Vector2(337 + 1920, 44), Path.Combine(AssetFilePath, "Results", "PlayResultsBackground.png"));
            return playRecordBg;
        }

        static public GameObject CreatePlayRecordScoreBg(GameObject parent, PlayData play)
        {
            var scoreBg = AssetUtility.CreateImageChild(parent, "ScoreBg", new Vector2(128, 752), Path.Combine(AssetFilePath, "Results", "PlayScoreBg.png"));

            var textObject = AssetUtility.CreateTextChild(scoreBg, "ScoreBgText", new Rect(300, 200, 300, 50), "Score");
            AssetUtility.SetTextAlignment(textObject, HorizontalAlignmentOptions.Center);

            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
            TMP_FontAsset scoreFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
            Material scoreFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);
            AssetUtility.SetTextFontAndMaterial(textObject, scoreFont, scoreFontMaterial);


            // Slightly difficult, looks like the score is centered
            var score = play.SongPlayData.Sum((x) => x.Score).ToString();
            int bgWidth = 434;
            int digitWidth = 47;
            int TotalDigitWidth = score.Length * digitWidth;

            Vector2 basePosition = new Vector2((bgWidth - TotalDigitWidth) / 2, 19);
            for (int i = 0; i < score.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(digitWidth * i, 0);
                var digitObject = AssetUtility.CreateEmptyObject(scoreBg, "Score" + (score.Length - (i + 1)), digitPosition);
                CommonAssets.CreateDigit(digitObject, "Shadow", Vector2.zero, DigitType.ResultsScoreShadow, score[i]);
                CommonAssets.CreateDigit(digitObject, "Fill", Vector2.zero, DigitType.ResultsScore, score[i]);
            }

            return scoreBg;
        }

        static public GameObject CreatePlayRecordGoodOkBad(GameObject parent, PlayData play)
        {
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecordBg1", new Vector2(571, 696), Path.Combine(AssetFilePath, "Results", "PlayRecord1.png"));

            var goods = play.SongPlayData.Sum((x) => x.Goods).ToString();
            var oks = play.SongPlayData.Sum((x) => x.Oks).ToString();
            var bads = play.SongPlayData.Sum((x) => x.Bads).ToString();

            Vector2 basePosition = new Vector2(327, 143);
            for (int i = 0; i < goods.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (goods.Length - (i + 1)), -62 * 0);
                CommonAssets.CreateDigit(playRecordBg, "Goods" + (goods.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, goods[i]);
            }
            for (int i = 0; i < oks.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (oks.Length - (i + 1)), -62 * 1);
                CommonAssets.CreateDigit(playRecordBg, "Oks" + (oks.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, oks[i]);
            }
            for (int i = 0; i < bads.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (bads.Length - (i + 1)), -62 * 2);
                CommonAssets.CreateDigit(playRecordBg, "Bads" + (bads.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, bads[i]);
            }

            return playRecordBg;
        }

        static public GameObject CreatePlayRecordDrumrollComboTotalHits(GameObject parent, PlayData play)
        {
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecordBg2", new Vector2(955, 696), Path.Combine(AssetFilePath, "Results", "PlayRecord2.png"));

            var drumroll = play.SongPlayData.Sum((x) => x.Drumroll).ToString();
            var combo = play.MaxCombo.ToString();
            var totalHits = play.SongPlayData.Sum((x) => x.Goods + x.Oks + x.Drumroll).ToString();

            Vector2 basePosition = new Vector2(397, 143);
            for (int i = 0; i < drumroll.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (drumroll.Length - (i + 1)), -62 * 0);
                CommonAssets.CreateDigit(playRecordBg, "Drumroll" + (drumroll.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, drumroll[i]);
            }
            for (int i = 0; i < combo.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (combo.Length - (i + 1)), -62 * 1);
                CommonAssets.CreateDigit(playRecordBg, "Combo" + (combo.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, combo[i]);
            }
            for (int i = 0; i < totalHits.Length; i++)
            {
                Vector2 digitPosition = basePosition + new Vector2(-28 * (totalHits.Length - (i + 1)), -62 * 2);
                CommonAssets.CreateDigit(playRecordBg, "TotalHits" + (totalHits.Length - (i + 1)), digitPosition, DigitType.ResultsBlack, totalHits[i]);
            }

            return playRecordBg;
        }

        static public GameObject CreateDanResult(GameObject parent, PlayData play)
        {
            var danResultParent = AssetUtility.CreateEmptyObject(parent, "DanResult", new Vector2(20, 300));

            string comboAsset;
            switch (play.RankCombo.Combo)
            {
                case DaniCombo.None: return danResultParent;
                case DaniCombo.Silver: comboAsset = "SilverBg.png"; break;
                case DaniCombo.Gold: comboAsset = "GoldBg.png"; break;
                case DaniCombo.Rainbow: comboAsset = "RainbowBg.png"; break;
                default: return danResultParent;
            }

            string resultAsset;
            switch (play.RankCombo.Rank)
            {
                case DaniRank.None: return danResultParent;
                case DaniRank.RedClear: resultAsset = "RedClear.png"; break;
                case DaniRank.GoldClear: resultAsset = "GoldClear.png"; break;
                default: return danResultParent;
            }

            AssetUtility.CreateImageChild(danResultParent, "ComboBg", new Vector2(0, 0), Path.Combine(AssetFilePath, "Results", comboAsset));
            AssetUtility.CreateImageChild(danResultParent, "Rank", new Vector2(0, 0), Path.Combine(AssetFilePath, "Results", resultAsset));
            return danResultParent;
        }
    }
}
