using DaniDojo.Data;
using DaniDojo.Managers;
using DaniDojo.Patches;
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
        static public GameObject CreateBg(GameObject parent)
        {
            return AssetUtility.CreateImageChild(parent, "DaniResultBg", new Vector2(0, 0), Path.Combine("Results", "Background.png"));
        }

        static public GameObject CreateCourseIcon(GameObject parent, DaniCourse course)
        {
            // TODO: Animate this by having it start 1080 y higher, then moving it down after initialization
            return CommonAssets.CreateDaniCourse(parent, new Vector2(77, 472), course);
        }

        static public GameObject CreateCourseTitle(GameObject parent, DaniCourse course)
        {
            return CommonAssets.CreateCourseTitleBar(parent, new Vector2(347, 954), course);
        }


        static public GameObject CreateSongPanel(GameObject parent)
        {
            var songBg = AssetUtility.CreateImageChild(parent, "SongMainBg", new Vector2(352, 69), Path.Combine("Results", "SongsWoodBackground.png"));
            //CreateEachSongBg(songBg);
            return songBg;
        }

        static public void CreateEachSongBg(GameObject parent, DaniCourse course, PlayData play, SaveCourse save)
        {
            for (int i = 0; i < Math.Min(course.Songs.Count, 3); i++)
            {
                int x = 28;
                int y = 607 - (i * 276);
                var songBg = AssetUtility.CreateImageChild(parent, "SongBg", new Vector2(x, y), Path.Combine("Results", "SongBg.png"));
                var songPanel = AssetUtility.CreateImageChild(songBg, "SongPanel" + i, new Vector2(38, 119), Path.Combine("Results", "SongPanel.png"));

                AssetUtility.CreateImageChild(songPanel, "SongIndicator", new Vector2(10, 10), Path.Combine("Results", "SongIndicator" + (i + 1) + ".png"));

                CommonAssets.CreateSongCourseChild(songPanel, new Vector2(112, 42), course.Songs[i]);
                CommonAssets.CreateSongLevelChild(songPanel, new Vector2(121, 15), course.Songs[i]);

                var songTitle = CommonAssets.CreateSongTitleChild(songPanel, new Vector2(218, 30), course.Songs[i], Math.Max(play.SongReached, save.SongReached) <= i && course.Songs[i].IsHidden);
                var songDetail = CommonAssets.CreateSongDetailChild(songPanel, new Vector2(218, 70), course.Songs[i], Math.Max(play.SongReached, save.SongReached) <= i && course.Songs[i].IsHidden);

                int valuesX = 136;
                int valuesY = 37;
                int valuesInterval = 317;
                var songGoodsPanel = AssetUtility.CreateImageChild(songBg, "SongGoodsPanel", new Vector2(valuesX, valuesY), Path.Combine("Results", "SongGoodsBg.png"));
                valuesX += valuesInterval;
                var songOksPanel = AssetUtility.CreateImageChild(songBg, "SongOksPanel", new Vector2(valuesX, valuesY), Path.Combine("Results", "SongOksBg.png"));
                valuesX += valuesInterval;
                var songBadsPanel = AssetUtility.CreateImageChild(songBg, "SongBadsPanel", new Vector2(valuesX, valuesY), Path.Combine("Results", "SongBadsBg.png"));
                valuesX += valuesInterval;
                var songDrumrollsPanel = AssetUtility.CreateImageChild(songBg, "SongDrumrollPanel", new Vector2(valuesX, valuesY), Path.Combine("Results", "SongDrumrollBg.png"));

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
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecord", new Vector2(337 + 1920, 44), Path.Combine("Results", "PlayResultsBackground.png"));
            return playRecordBg;
        }

        static public GameObject CreatePlayRecordScoreBg(GameObject parent, PlayData play)
        {
            var scoreBg = AssetUtility.CreateImageChild(parent, "ScoreBg", new Vector2(128, 752), Path.Combine("Results", "PlayScoreBg.png"));

            var textObject = AssetUtility.CreateTextChild(scoreBg, "ScoreBgText", new Rect(74, 102, 300, 40), "Score");
            AssetUtility.SetTextAlignment(textObject, HorizontalAlignmentOptions.Center);

            FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;
            TMP_FontAsset scoreFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
            Material scoreFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.KanbanPops);
            AssetUtility.SetTextFontAndMaterial(textObject, scoreFont, scoreFontMaterial);


            // Slightly difficult, looks like the score is centered
            // UPDATE: Score is not fully centered
            // It is centered as if the score is 7 digits, even if it is not 7 digits
            // I will update this later if I don't forget which I probably will unless I don't
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
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecordBg1", new Vector2(571, 696), Path.Combine("Results", "PlayRecord1.png"));

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
            var playRecordBg = AssetUtility.CreateImageChild(parent, "PlayRecordBg2", new Vector2(955, 696), Path.Combine("Results", "PlayRecord2.png"));

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
            var danResultParent = AssetUtility.CreateEmptyObject(parent, "DanResult", new Vector2(7, 350));


            string comboAsset = "";
            bool isFailed = false;
            switch (play.RankCombo.Combo)
            {
                case DaniCombo.None: isFailed = true; break;
                case DaniCombo.Silver: comboAsset = "ClearBg.png"; break;
                case DaniCombo.Gold: comboAsset = "FcBg.png"; break;
                case DaniCombo.Rainbow: comboAsset = "DfcBg.png"; break;
                default: isFailed = true; break;
            }

            string resultAsset = "";
            switch (play.RankCombo.Rank)
            {
                case DaniRank.None: isFailed = true; break;
                case DaniRank.RedClear: resultAsset = "RedClearText.png"; break;
                case DaniRank.GoldClear: resultAsset = "GoldClearText.png"; break;
                default: isFailed = true; break;
            }

            if (isFailed)
            {
                AssetUtility.CreateImageChild(danResultParent, "FailShadow1", new Vector2(3, 55), Path.Combine("Results", "DaniResult", "ResultFailShadow1.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailShadow2", new Vector2(109, -14), Path.Combine("Results", "DaniResult", "ResultFailShadow2.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailShadow3", new Vector2(232, 21), Path.Combine("Results", "DaniResult", "ResultFailShadow3.png"));

                AssetUtility.CreateImageChild(danResultParent, "FailBg1", new Vector2(15, 78), Path.Combine("Results", "DaniResult", "ResultFailBackground1.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailBg2", new Vector2(123, 9), Path.Combine("Results", "DaniResult", "ResultFailBackground2.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailBg3", new Vector2(245, 43), Path.Combine("Results", "DaniResult", "ResultFailBackground3.png"));

                AssetUtility.CreateImageChild(danResultParent, "FailOutline1", new Vector2(19, 76), Path.Combine("Results", "DaniResult", "ResultFailOutline1.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailOutline2", new Vector2(127, 7), Path.Combine("Results", "DaniResult", "ResultFailOutline2.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailOutline3", new Vector2(248, 41), Path.Combine("Results", "DaniResult", "ResultFailOutline3.png"));

                AssetUtility.CreateImageChild(danResultParent, "FailText1", new Vector2(32, 93), Path.Combine("Results", "DaniResult", "ResultFailText1.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailText2", new Vector2(140, 26), Path.Combine("Results", "DaniResult", "ResultFailText2.png"));
                AssetUtility.CreateImageChild(danResultParent, "FailText3", new Vector2(261, 60), Path.Combine("Results", "DaniResult", "ResultFailText3.png"));
            }
            else
            {
                AssetUtility.CreateImageChild(danResultParent, "ComboBg", new Vector2(0, 0), Path.Combine("Results", "DaniResult", comboAsset));
                AssetUtility.CreateImageChild(danResultParent, "Rank", new Vector2(21, 39), Path.Combine("Results", "DaniResult", resultAsset));
            }
            return danResultParent;
        }

        static public GameObject CreateBorderPanels(GameObject parent, DaniCourse course, PlayData play)
        {
            var borderPanels = AssetUtility.CreateEmptyObject(parent, "BorderPanels", new Vector2(0, 0));

            for (int i = 0; i < course.Borders.Count; i++)
            {
                CreateBorderPanel(borderPanels, "Panel" + (i + 1), new Vector2(59, 534 - (i * 154)), course.Borders[i], play);
            }

            return borderPanels;
        }

        static public GameObject CreateBorderPanel(GameObject parent, string name, Vector2 position, DaniBorder border, PlayData play)
        {
            var borderPanel = AssetUtility.CreateEmptyObject(parent, name, position);

            var borderPanelBg = AssetUtility.CreateImageChild(borderPanel, "BorderBg", new Vector2(0, 0), Path.Combine("Results", "BorderResultsPanel.png"));

            var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();
            TMP_FontAsset reqTypefont = fontManager.GetDefaultFontAsset(DataConst.FontType.EFIGS);
            Material reqTypeFontMaterial = fontManager.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);
            string requirementText = string.Empty;
            switch (border.BorderType)
            {
                case BorderType.SoulGauge:
                    requirementText = "Soul Gauge";
                    break;
                case BorderType.Goods:
                    requirementText = "GOODs";
                    break;
                case BorderType.Oks:
                    requirementText = "OKs";
                    break;
                case BorderType.Bads:
                    requirementText = "BADs";
                    break;
                case BorderType.Combo:
                    requirementText = "Combo";
                    break;
                case BorderType.Drumroll:
                    requirementText = "Drumroll";
                    break;
                case BorderType.Score:
                    requirementText = "Score";
                    break;
                case BorderType.TotalHits:
                    requirementText = "Total Hits";
                    break;
                default:
                    break;
            }
            var requirementTypeText = AssetUtility.CreateTextChild(borderPanel, "BorderReqText", new Rect(24, 109, 334, 36), requirementText);
            AssetUtility.SetTextFontAndMaterial(requirementTypeText, reqTypefont, reqTypeFontMaterial);
            AssetUtility.SetTextAlignment(requirementTypeText, HorizontalAlignmentOptions.Center);





            //var borderReqText = AssetUtility.CreateTextChild(borderPanel, "BorderReqText", new Rect(30, 50, 300, 50), border.BorderType.ToString());

            if (border.BorderType == BorderType.SoulGauge)
            {
                AssetUtility.CreateImageChild(borderPanel, "SongNumIndicator", new Vector2(28, 30), Path.Combine("Enso", "CurSongIndicatorTotal.png"));

                var soulGauge = AssetUtility.CreateEmptyObject(borderPanel, "SoulGauge", new Vector2(50, 20));
                var soulGaugeBg = AssetUtility.CreateImageChild(soulGauge, "SoulGaugeBg", new Vector2(75, 8), Path.Combine("SoulGauge", "ResultsSoulGaugeBg.png"));
                var soulGaugeSeparators = AssetUtility.CreateImageChild(soulGauge, "SoulGaugeSeparators", new Vector2(131, 21), Path.Combine("SoulGauge", "SoulGaugeBarSeparators.png"));
            }
            else if (border.IsTotal)
            {
                CreateTotalRequirementPanel(borderPanel, border, play);
            }
            else
            {
                CreatePerSongRequirementPanel(borderPanel, border, play);
            }


            return borderPanel;
        }

        static public GameObject CreateTotalRequirementPanel(GameObject parent, DaniBorder border, PlayData play)
        {
            // Exit if not the correct type of border
            if (!border.IsTotal)
            {
                Plugin.LogInfo("Error creating TotalRequirementPanel: Not TotalRequirement Border");
                return null;
            }

            // Create SongNumIndicator (Always Total for this panel)
            AssetUtility.CreateImageChild(parent, "SongNumIndicator", new Vector2(28, 30), Path.Combine("Enso", "CurSongIndicatorTotal.png"));


            // Create the requirement value string
            var requirementValueString = string.Empty;
            if (border.BorderType == BorderType.Oks || border.BorderType == BorderType.Bads)
            {
                requirementValueString = "Less than " + border.RedReqs[0];
            }
            else
            {
                requirementValueString = border.RedReqs[0] + " or more";
            }

            var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();

            TMP_FontAsset reqValuefont = fontManager.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
            Material reqValueFontMaterial = fontManager.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

            var requirementTypeText = AssetUtility.CreateTextChild(parent, "RequirementValue", new Rect(28, 36, 334, 46), requirementValueString);
            AssetUtility.SetTextFontAndMaterial(requirementTypeText, reqValuefont, reqValueFontMaterial);
            AssetUtility.SetTextAlignment(requirementTypeText, HorizontalAlignmentOptions.Right);



            // Create the requirement bars
            var barData = DaniPlayManager.GetBorderBarData(border, play, remainingNotes: 0);
            var bar = AssetUtility.CreateEmptyObject(parent, "RequirementBar", new Vector2(13, -4));

            var curReqBarImagePath = Path.Combine("Enso", "Bars", "RequirementBarTotal.png");
            var curReqBarBorderImagePath = Path.Combine("Enso", "Bars", "RequirementBarBorderTotal.png");

            Vector2 barPositions = new Vector2(389, 20);
            AssetUtility.CreateImageChild(bar, "CurReqBar", barPositions, Path.Combine(curReqBarImagePath));

            Rect fillBarRect = new Rect(396, 36, 966, 80);
            Rect emptyBarRect = new Rect(396 + 966, 36, 966, 80);


            Plugin.LogInfo("barData.FillRatio: " + barData.FillRatio);

            var fillBar = AssetUtility.CreateImageChild(bar, "CurReqBarFill", fillBarRect, barData.Color);
            var colorLerp = fillBar.AddComponent<ColorLerp>();
            var emptyBar = AssetUtility.CreateImageChild(bar, "CurReqBarEmpty", emptyBarRect, BorderBarColors.BorderBarColor[BorderBarState.Grey]);
            AssetUtility.CreateImageChild(bar, "CurReqBarBorder", barPositions, Path.Combine(curReqBarBorderImagePath));

            var fillBarImage = AssetUtility.GetOrAddImageComponent(fillBar);

            var newScale = emptyBar.transform.localScale;
            newScale.x = barData.FillRatio / 100f;

            // Probably don't need this clamp anymore, but it shouldn't hurt to have it.
            newScale.x = Math.Max(newScale.x, 0);
            newScale.x = Math.Min(newScale.x, 1);

            // Previously was resizing the filled area
            // Currently resizing the empty area
            // Hopefully this math is correct
            newScale.x -= 1;
            emptyBar.transform.localScale = newScale;

            fillBarImage.color = barData.Color;

            colorLerp.UpdateState(barData, true, true);


            // Create the Requirement Value Digits
            var barState = DigitAssets.GetRequirementBarState(barData, border);
            DigitAssets.CreateRequirementBarNumber(bar, new Vector2(403, 44), barData.PlayValue, RequirementBarType.Large, barState);





            return parent;
        }

        static public GameObject CreatePerSongRequirementPanel(GameObject parent, DaniBorder border, PlayData play)
        {
            if (border.IsTotal)
            {
                Plugin.LogInfo("Error creating PerSongRequirementPanel: Not PerSongRequirement Border");
                return null;
            }

            for (int i = 0; i < Math.Min(3, play.SongReached); i++)
            {
                // Create SongNumIndicator
                var songPanel = AssetUtility.CreateEmptyObject(parent, "Song" + (i + 1), new Vector2(0 + (i * 465), 0));
                AssetUtility.CreateImageChild(songPanel, "SongNumIndicator", new Vector2(28, 30), Path.Combine("Enso", "CurSongIndicator" + (i + 1) + ".png"));







                // Create the requirement bars

                var barData = DaniPlayManager.GetBorderBarData(border, play, songNumber: i, remainingNotes: 0);
                var bar = AssetUtility.CreateEmptyObject(songPanel, "RequirementBar", new Vector2(-264, 13));

                Vector2 barPosition = new Vector2(389, 15);
                AssetUtility.CreateImageChild(bar, "CurReqBar", barPosition, Path.Combine("Results", "ResultsBorderFillSmall.png"));

                Rect fillBarRect = new Rect(393, 24, 322, 41);
                var fillBar = AssetUtility.CreateImageChild(bar, "CurReqBarFill", fillBarRect, barData.Color);
                var colorLerp = fillBar.AddComponent<ColorLerp>();

                Rect emptyBarRect = new Rect(394 + 322, 24, 322, 41);
                var emptyBar = AssetUtility.CreateImageChild(bar, "CurReqBarEmpty", emptyBarRect, BorderBarColors.BorderBarColor[BorderBarState.Grey]);

                Vector2 borderBarRect = new Vector2(389, 22);
                AssetUtility.CreateImageChild(bar, "CurReqBarBorder", borderBarRect, Path.Combine("Results", "ResultsBorderSmall.png"));


                Plugin.LogInfo("barData.FillRatio: " + barData.FillRatio);

                var fillBarImage = AssetUtility.GetOrAddImageComponent(fillBar);

                var newScale = emptyBar.transform.localScale;
                newScale.x = barData.FillRatio / 100f;

                // Probably don't need this clamp anymore, but it shouldn't hurt to have it.
                newScale.x = Math.Max(newScale.x, 0);
                newScale.x = Math.Min(newScale.x, 1);

                // Previously was resizing the filled area
                // Currently resizing the empty area
                // Hopefully this math is correct
                newScale.x -= 1;
                emptyBar.transform.localScale = newScale;

                fillBarImage.color = barData.Color;

                colorLerp.UpdateState(barData, false, true);

                var barState = DigitAssets.GetRequirementBarState(barData, border);
                DigitAssets.CreateRequirementBarNumber(bar, new Vector2(397, 29), barData.PlayValue, RequirementBarType.Medium, barState);


                // Create the requirement value string
                var requirementValueString = string.Empty;
                if (border.BorderType == BorderType.Oks || border.BorderType == BorderType.Bads)
                {
                    requirementValueString = "Less than " + border.RedReqs[i];
                }
                else
                {
                    requirementValueString = border.RedReqs[i] + " or more";
                }

                var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();

                TMP_FontAsset reqValuefont = fontManager.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                Material reqValueFontMaterial = fontManager.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                var requirementTypeText = AssetUtility.CreateTextChild(songPanel, "RequirementValue", new Rect(111, 20, 334, 26), requirementValueString);
                AssetUtility.SetTextFontAndMaterial(requirementTypeText, reqValuefont, reqValueFontMaterial);
                AssetUtility.SetTextAlignment(requirementTypeText, HorizontalAlignmentOptions.Right);

            }

            return parent;
        }

    }
}
