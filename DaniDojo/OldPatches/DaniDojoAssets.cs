using DaniDojo.Assets;
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
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace DaniDojo.Patches
{
    internal static class DaniDojoAssets
    {
        //static string BaseImageFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;


        //static Color32 InvisibleColor = new Color32(0, 0, 0, 0);

        static Color32 PinkBarColor = new Color32(254, 161, 183, 255);
        //static Color32 YellowBarColor = new Color32(249, 254, 55, 255);
        //static Color32 RedBarColor = new Color32(250, 124, 78, 255);
        static Color32 GreyBarColor = new Color32(69, 69, 69, 255);

        //static Color32 GoldReqTextBorderColor = new Color32(221, 89, 56, 255);
        //static Color32 GoldReqTextFillColor = new Color32(255, 93, 127, 255);
        //static Color32 GoldReqTextTransparentColor = new Color32(255, 244, 45, 255);

        //static Color32 NormalTextBorderColor = new Color32(177, 177, 177, 255);
        //static Color32 NormalTextFillColor = new Color32(255, 255, 255, 255);
        //static Color32 NormalTextTransparentColor = new Color32(0, 0, 0, 0);

        //static Color32 ZeroTextBorderColor = new Color32(177, 177, 177, 255);
        //static Color32 ZeroTextFillColor = new Color32(0, 0, 0, 0);
        //static Color32 ZeroTextTransparentColor = new Color32(0, 0, 0, 0);

        public static class EnsoAssets
        {
            public static void CreateBottomAssets(GameObject parent)
            {
                //DaniDojoAssetUtility.CreateImage("DaniBottomBg", Path.Combine(BaseImageFilePath, "Enso", "bottomBg.png"), new Vector2(0, 0), parent.transform);

                //DaniDojoAssetUtility.CreateImage("DaniTopBg", Path.Combine(BaseImageFilePath, "Enso", "topBg.png"), new Vector2(0, 800), parent.transform);
                var topBg = Assets.EnsoAssets.CreateTopBg(parent);
                Assets.EnsoAssets.CreateBottomBg(parent);

                Assets.EnsoAssets.CreateTopAnimatedParts(topBg);

                var borders = DaniPlayManager.GetCurrentCourseBorders();
                int numPanels = 0;
                for (int j = 0; j < borders.Count && numPanels < 3; j++)
                {
                    if (borders[j].BorderType != BorderType.SoulGauge)
                    {
                        CreatePanel("Panel" + j, new Vector2(117, 353 - (159 * numPanels)), parent, borders[j]);
                        numPanels++;
                    }
                }

                numPanels = 0;
                for (int j = 0; j < borders.Count && numPanels < 3; j++)
                {
                    if (borders[j].BorderType != BorderType.SoulGauge)
                    {
                        UpdateRequirementBar(borders[j].BorderType);
                        numPanels++;
                    }
                }
            }

            public static void ChangeCourseIcon(GameObject gameObject)
            {
                if (gameObject == null)
                {
                    gameObject = GameObject.Find("icon_course");
                }
                AssetUtility.ChangeImageSprite(gameObject, Path.Combine("Course", "DifficultyIcons", "DaniDojoResized.png"));
                //DaniDojoAssetUtility.ChangeSprite(gameObject, Path.Combine(BaseImageFilePath, "Course", "DifficultyIcons", "DaniDojoResized.png"));
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Dan-i dojo";
            }

            private static void CreatePanel(string name, Vector2 location, GameObject parent, DaniBorder border)
            {
                Plugin.Log.LogInfo("Create Panel");
                //var newPanel = DaniDojoAssetUtility.CreateImage(name, Path.Combine("Enso", "RequirementPanel.png"), location, parent);
                var newPanel = AssetUtility.CreateImageChild(parent, name, location, Path.Combine("Enso", "RequirementPanel.png"));

                string requirementText = "Goods";

                Plugin.Log.LogInfo("Initialize Requirement Text");

                var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();
                TMP_FontAsset reqTypefont = fontManager.GetDefaultFontAsset(DataConst.FontType.EFIGS);
                Material reqTypeFontMaterial = fontManager.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);
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

                //var requirementTypeText = DaniDojoAssetUtility.CreateText("RequirementTypeText", requirementText, new Rect(24, 109, 334, 36), reqTypefont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), newPanel.transform);
                var requirementTypeText = AssetUtility.CreateTextChild(newPanel, "RequirementTypeText", new Rect(24, 109, 334, 36), requirementText);
                AssetUtility.SetTextFontAndMaterial(requirementTypeText, reqTypefont, reqTypeFontMaterial);
                AssetUtility.SetTextAlignment(requirementTypeText, HorizontalAlignmentOptions.Center);

                Plugin.Log.LogInfo("Create SongIndicators");

                string songNumIndImagePath = "CurSongIndicatorTotal.png";

                if (!border.IsTotal)
                {
                    songNumIndImagePath = "CurSongIndicator" + (DaniPlayManager.GetCurrentSongNumber() + 1) + ".png";
                }

                //DaniDojoAssetUtility.CreateImage("SongNumIndicator", Path.Combine("Enso", songNumIndImagePath), new Vector2(20, 34), newPanel.transform);
                AssetUtility.CreateImageChild(newPanel, "SongNumIndicator", new Vector2(20, 34), Path.Combine("Enso", songNumIndImagePath));

                Plugin.Log.LogInfo("Create Requirement Value Text");

                var requirementValueString = string.Empty;
                if (border.BorderType == BorderType.Oks || border.BorderType == BorderType.Bads)
                {
                    if (border.IsTotal)
                    {
                        requirementValueString = "Less than " + border.RedReqs[0];
                    }
                    else
                    {
                        requirementValueString = "Less than " + border.RedReqs[DaniPlayManager.GetCurrentSongNumber()];
                    }
                }
                else
                {
                    if (border.IsTotal)
                    {
                        requirementValueString = border.RedReqs[0] + " or more";
                    }
                    else
                    {
                        requirementValueString = border.RedReqs[DaniPlayManager.GetCurrentSongNumber()] + " or more";
                    }
                }

                TMP_FontAsset reqValuefont = fontManager.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                Material reqValueFontMaterial = fontManager.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                //var requirementValue = DaniDojoAssetUtility.CreateText("RequirementValue", requirementValueString, new Rect(28, 40, 334, 46), reqValuefont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), newPanel.transform);
                var requirementValue = AssetUtility.CreateTextChild(newPanel, "RequirementValue", new Rect(28, 40, 334, 46), requirementValueString);
                AssetUtility.SetTextFontAndMaterial(requirementValue, reqValuefont, reqValueFontMaterial);
                AssetUtility.SetTextAlignment(requirementValue, HorizontalAlignmentOptions.Right);

                Plugin.Log.LogInfo("Create Requirement Bars");


                var curReqBarImagePath = "RequirementBarPerSong.png";
                var curReqBarBorderImagePath = "RequirementBarBorderPerSong.png";
                if (border.IsTotal)
                {
                    curReqBarImagePath = "RequirementBarTotal.png";
                    curReqBarBorderImagePath = "RequirementBarBorderTotal.png";
                }
                Vector2 barPositions = new Vector2(389, 20);
                //DaniDojoAssetUtility.CreateImage("CurReqBar", Path.Combine("Enso", "Bars", curReqBarImagePath), barPositions, newPanel.transform);
                AssetUtility.CreateImageChild(newPanel, "CurReqBar", barPositions, Path.Combine("Enso", "Bars", curReqBarImagePath));

                Rect fillBarRect;
                Rect emptyBarRect;
                if (border.IsTotal)
                {
                    fillBarRect = new Rect(396, 36, 966, 80);
                    emptyBarRect = new Rect(396 + 966, 36, 966, 80);
                }
                else
                {
                    fillBarRect = new Rect(396, 36, 642, 80);
                    emptyBarRect = new Rect(396 + 642, 36, 642, 80);
                }
                //var fillBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarFill", PinkBarColor, fillBarRect, newPanel.transform);
                var fillBar = AssetUtility.CreateImageChild(newPanel, "CurReqBarFill", fillBarRect, PinkBarColor);
                fillBar.AddComponent<ColorLerp>();


                //var coverBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarEmpty", GreyBarColor, emptyBarRect, newPanel.transform);
                var coverBar = AssetUtility.CreateImageChild(newPanel, "CurReqBarEmpty", emptyBarRect, GreyBarColor);

                //DaniDojoAssetUtility.CreateImage("CurReqBarBorder", Path.Combine("Enso", "Bars", curReqBarBorderImagePath), barPositions, newPanel.transform);
                AssetUtility.CreateImageChild(newPanel, "CurReqBarBorder", barPositions, Path.Combine("Enso", "Bars", curReqBarBorderImagePath));

                Plugin.Log.LogInfo("Create Previous Song Requirement Bars");

                if (!border.IsTotal)
                {
                    //var prevSongTop = DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarTop", Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 73), newPanel.transform);
                    //var prevSongBot = DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarBot", Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 23), newPanel.transform);
                    var prevSongTop = AssetUtility.CreateImageChild(newPanel, "PrevSongHitReqsBarTop", new Vector2(1083, 73), Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"));
                    var prevSongBot = AssetUtility.CreateImageChild(newPanel, "PrevSongHitReqsBarBot", new Vector2(1083, 23), Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"));
                    Plugin.Log.LogInfo("Create Previous Song Requirement Bars 1");
                    if (DaniPlayManager.GetCurrentSongNumber() >= 1)
                    {
                        //DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneIndicator", Path.Combine("Enso", "PrevSongIndicator1.png"), new Vector2(2, 10), prevSongTop.transform);
                        //DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBar", Path.Combine("Enso", "Bars", "PrevSongBar.png"), new Vector2(44, 4), prevSongTop.transform);
                        //var prevSongBar1 = DaniDojoAssetUtility.CreateNewImage("PrevSongHitReqOneBarFill", PinkBarColor, new Rect(46, 11, 234, 33), prevSongTop.transform);
                        //DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBarBorder", Path.Combine("Enso", "Bars", "PrevSongBarBorder.png"), new Vector2(44, 4), prevSongTop.transform);

                        AssetUtility.CreateImageChild(prevSongTop, "PrevSongHitReqOneIndicator", new Vector2(2, 10), Path.Combine("Enso", "PrevSongIndicator1.png"));
                        AssetUtility.CreateImageChild(prevSongTop, "PrevSongHitReqOneBar", new Vector2(44, 4), Path.Combine("Enso", "PrevSongBar.png"));
                        var prevSongBar1 = AssetUtility.CreateImageChild(prevSongTop, "PrevSongHitReqOneBarFill", new Rect(46, 11, 234, 33), PinkBarColor);
                        AssetUtility.CreateImageChild(prevSongTop, "PrevSongHitReqOneBarBorder", new Vector2(44, 4), Path.Combine("Enso", "PrevSongBarBorder.png"));

                        Plugin.Log.LogInfo("Create Previous Song Requirement Bars 2");
                        var songValues = DaniPlayManager.GetBorderPlayResults(border);
                        var image = prevSongBar1.GetOrAddComponent<Image>();
                        if (image != null)
                        {
                            BorderBarData songData1 = DaniPlayManager.GetBorderBarData(border, DaniPlayManager.GetCurrentPlay(), 0);

                            Plugin.Log.LogInfo("Create Previous Song Requirement Bars 3");
                            var newScale = image.transform.localScale;
                            newScale.x = songData1.FillRatio / 100f;

                            newScale.x = Math.Max(newScale.x, 0);
                            newScale.x = Math.Min(newScale.x, 1);
                            image.transform.localScale = newScale;
                            image.color = songData1.Color;

                            if (songData1.State == BorderBarState.Rainbow)
                            {
                                AssetUtility.ChangeImageSprite(image, Path.Combine("Enso", "PrevSongRainbow", "PrevSongRainbow.png"));
                            }
                            Plugin.Log.LogInfo("Create Previous Song Requirement Bars 4");

                            var barState = DigitAssets.GetRequirementBarState(songData1, border);
                            DigitAssets.CreateRequirementBarNumber(prevSongTop, new Vector2(49, 15), songData1.PlayValue, RequirementBarType.Small, barState);
                        }

                        if (DaniPlayManager.GetCurrentSongNumber() >= 2)
                        {
                            //DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoIndicator", Path.Combine("Enso", "PrevSongIndicator2.png"), new Vector2(2, 10), prevSongBot.transform);
                            //DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoBar", Path.Combine("Enso", "Bars", "PrevSongBar.png"), new Vector2(44, 4), prevSongBot.transform);
                            //var prevSongBar2 = DaniDojoAssetUtility.CreateNewImage("PrevSongHitReqTwoBarFill", PinkBarColor, new Rect(46, 11, 234, 33), prevSongBot.transform);
                            //DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoBarBorder", Path.Combine("Enso", "Bars", "PrevSongBarBorder.png"), new Vector2(44, 4), prevSongBot.transform);

                            AssetUtility.CreateImageChild(prevSongBot, "PrevSongHitReqTwoIndicator", new Vector2(2, 10), Path.Combine("Enso", "PrevSongIndicator1.png"));
                            AssetUtility.CreateImageChild(prevSongBot, "PrevSongHitReqTwoBar", new Vector2(44, 4), Path.Combine("Enso", "PrevSongBar.png"));
                            var prevSongBar2 = AssetUtility.CreateImageChild(prevSongBot, "PrevSongHitReqTwoBarFill", new Rect(46, 11, 234, 33), PinkBarColor);
                            AssetUtility.CreateImageChild(prevSongBot, "PrevSongHitReqTwoBarBorder", new Vector2(44, 4), Path.Combine("Enso", "PrevSongBarBorder.png"));

                            Plugin.Log.LogInfo("Create Previous Song Requirement Bars 5");
                            var image2 = prevSongBar2.GetOrAddComponent<Image>();
                            if (image2 != null)
                            {
                                BorderBarData songData1 = DaniPlayManager.GetBorderBarData(border, DaniPlayManager.GetCurrentPlay(), 1);
                                var newScale = image2.transform.localScale;
                                newScale.x = songData1.FillRatio / 100f;

                                newScale.x = Math.Max(newScale.x, 0);
                                newScale.x = Math.Min(newScale.x, 1);
                                image2.transform.localScale = newScale;
                                image2.color = songData1.Color;

                                if (songData1.State == BorderBarState.Rainbow)
                                {
                                    AssetUtility.ChangeImageSprite(image2, Path.Combine("Enso", "PrevSongRainbow", "PrevSongRainbow.png"));
                                }
                                Plugin.Log.LogInfo("Create Previous Song Requirement Bars 6");

                                var barState = DigitAssets.GetRequirementBarState(songData1, border);
                                DigitAssets.CreateRequirementBarNumber(prevSongBot, new Vector2(49, 15), songData1.PlayValue, RequirementBarType.Small, barState);
                            }
                        }
                    }
                }
            }

            public static void AdvanceSongPanel(DaniCourse course, int songIndex)
            {
                int numPanels = 0;
                for (int j = 0; j < course.Borders.Count && numPanels < 3; j++)
                {
                    if (course.Borders[j].BorderType != BorderType.SoulGauge)
                    {
                        var newPanel = GameObject.Find("Panel" + j);
                        numPanels++;
                        if (newPanel == null || course.Borders[j].IsTotal || songIndex == 0)
                        {
                            continue;
                        }

                        var mainFillBar = AssetUtility.GetChildByName(newPanel, "CurReqBarFill");
                        var colorLerp = mainFillBar.GetOrAddComponent<ColorLerp>();
                        colorLerp.EndRainbow();

                        #region SetRequirementText

                        var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();

                        var requirementValueString = string.Empty;
                        if (course.Borders[j].BorderType == BorderType.Oks || course.Borders[j].BorderType == BorderType.Bads)
                        {
                            requirementValueString = "Less than " + course.Borders[j].RedReqs[songIndex];
                        }
                        else
                        {
                            requirementValueString = course.Borders[j].RedReqs[songIndex] + " or more";
                        }

                        TMP_FontAsset reqValuefont = fontManager.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                        Material reqValueFontMaterial = fontManager.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                        //var requirementValue = DaniDojoAssetUtility.CreateText("RequirementValue", requirementValueString, new Rect(28, 40, 334, 46), reqValuefont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), newPanel.transform);
                        var requirementValue = AssetUtility.GetChildByName(newPanel, "RequirementValue");
                        if (requirementValue == null)
                        {
                            requirementValue = AssetUtility.CreateTextChild(newPanel, "RequirementValue", new Rect(28, 40, 334, 46), requirementValueString);
                        }
                        requirementValue.GetOrAddComponent<TextMeshProUGUI>().text = requirementValueString;
                        AssetUtility.SetTextFontAndMaterial(requirementValue, reqValuefont, reqValueFontMaterial);
                        AssetUtility.SetTextAlignment(requirementValue, HorizontalAlignmentOptions.Right);

                        #endregion

                        var songNumIndImagePath = "CurSongIndicator" + (DaniPlayManager.GetCurrentSongNumber() + 1) + ".png";
                        var songNumIndicator = AssetUtility.GetChildByName(newPanel, "SongNumIndicator");
                        if (songNumIndicator == null)
                        {
                            AssetUtility.CreateImageChild(newPanel, "SongNumIndicator", new Vector2(20, 34), Path.Combine("Enso", songNumIndImagePath));
                        }
                        else
                        {
                            AssetUtility.ChangeImageSprite(songNumIndicator, Path.Combine("Enso", songNumIndImagePath));
                        }

                        //DaniDojoAssetUtility.CreateImage("SongNumIndicator", Path.Combine("Enso", songNumIndImagePath), new Vector2(20, 34), newPanel.transform);


                        GameObject prevSongTop = AssetUtility.GetChildByName(newPanel, "PrevSongHitReqsBarTop");
                        if (prevSongTop == null)
                        {
                            prevSongTop = AssetUtility.CreateImageChild(newPanel, "PrevSongHitReqsBarTop", new Vector2(1083, 73), Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"));
                        }

                        GameObject prevSongBot = AssetUtility.GetChildByName(newPanel, "PrevSongHitReqsBarBot");
                        if (prevSongBot == null)
                        {
                            prevSongBot = AssetUtility.CreateImageChild(newPanel, "PrevSongHitReqsBarBot", new Vector2(1083, 23), Path.Combine("Enso", "Bars", "PrevSongHitReqs.png"));
                        }

                        // If the songIndex is 1, we want to get the score for the 0 song to place in the top spot
                        var parent = songIndex == 1 ? prevSongTop : prevSongBot;

                        var prevSongIndicator = AssetUtility.GetChildByName(parent, "PrevSongHitReqOneIndicator");
                        if (prevSongIndicator == null)
                        {
                            AssetUtility.CreateImageChild(parent, "PrevSongHitReqOneIndicator", new Vector2(2, 10), Path.Combine("Enso", "PrevSongIndicator" + songIndex + ".png"));
                        }

                        var prevSongBar = AssetUtility.GetChildByName(parent, "PrevSongHitReqOneBar");
                        if (prevSongBar == null)
                        {
                            AssetUtility.CreateImageChild(parent, "PrevSongHitReqOneBar", new Vector2(44, 4), Path.Combine("Enso", "Bars", "PrevSongBar.png"));
                        }

                        var prevSongBarFill = AssetUtility.GetChildByName(parent, "PrevSongHitReqOneBarFill");
                        if (prevSongBarFill == null)
                        {
                            prevSongBarFill = AssetUtility.CreateImageChild(parent, "PrevSongHitReqOneBarFill", new Rect(46, 11, 234, 33), PinkBarColor);
                        }

                        var prevSongBorder = AssetUtility.GetChildByName(parent, "PrevSongHitReqOneBarBorder");
                        if (prevSongBorder == null)
                        {
                            AssetUtility.CreateImageChild(parent, "PrevSongHitReqOneBarBorder", new Vector2(44, 4), Path.Combine("Enso", "Bars", "PrevSongBarBorder.png"));
                        }


                        var songValues = DaniPlayManager.GetBorderPlayResults(course.Borders[j]);
                        var image = prevSongBarFill.GetOrAddComponent<Image>();
                        if (image != null)
                        {
                            BorderBarData songData1 = DaniPlayManager.GetBorderBarData(course.Borders[j], DaniPlayManager.GetCurrentPlay(), songIndex - 1);

                            var newScale = image.transform.localScale;
                            newScale.x = songData1.FillRatio / 100f;

                            newScale.x = Math.Max(newScale.x, 0);
                            newScale.x = Math.Min(newScale.x, 1);
                            image.transform.localScale = newScale;
                            image.color = songData1.Color;

                            if (songData1.State == BorderBarState.Rainbow)
                            {
                                AssetUtility.ChangeImageSprite(image, Path.Combine("Enso", "PrevSongRainbow", "PrevSongRainbow.png"));
                            }

                            var barState = DigitAssets.GetRequirementBarState(songData1, course.Borders[j]);
                            DigitAssets.CreateRequirementBarNumber(parent, new Vector2(49, 15), songData1.PlayValue, RequirementBarType.Small, barState);
                        }
                    }
                }

                numPanels = 0;
                for (int j = 0; j < course.Borders.Count && numPanels < 3; j++)
                {
                    if (course.Borders[j].BorderType != BorderType.SoulGauge)
                    {
                        UpdateRequirementBar(course.Borders[j].BorderType);
                        numPanels++;
                    }
                }
            }

            public static void UpdateRequirementBar(BorderType borderType)
            {
                Plugin.LogInfo(LogType.Info, "UpdateRequirementBar Start", 2);
                var indexes = DaniPlayManager.GetIndexexOfBorder(borderType);
                var borders = DaniPlayManager.GetCurrentBorderOfType(borderType);

                for (int j = 0; j < indexes.Count; j++)
                {
                    Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: Index: " + j, 2);
                    GameObject panel = GameObject.Find("Panel" + indexes[j]);
                    if (panel != null)
                    {
                        Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 1", 2);
                        //var bar = panel.transform.Find("CurReqBarFill");
                        var bar = AssetUtility.GetChildByName(panel, "CurReqBarFill");
                        var emptyBar = AssetUtility.GetChildByName(panel, "CurReqBarEmpty");
                        if (bar != null && emptyBar != null)
                        {
                            Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 2", 2);
                            var image = bar.GetOrAddComponent<Image>();
                            var emptyImage = emptyBar.GetOrAddComponent<Image>();
                            var colorLerp = bar.GetOrAddComponent<ColorLerp>();
                            BorderBarData data = DaniPlayManager.GetBorderBarData(borders[j], DaniPlayManager.GetCurrentPlay(), DaniPlayManager.GetCurrentSongNumber());

                            Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 3", 2);

                            bool isGold = data.State == BorderBarState.Rainbow;


                            var newScale = emptyImage.transform.localScale;

                            if (isGold)
                            {
                                colorLerp.BeginRainbow(borders[j].IsTotal);
                            }

                            if (data.StateChanged)
                            {
                                colorLerp.UpdateState(data, borders[j].IsTotal);
                            }

                            Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 4", 2);

                            ChangeReqCurrentValue(panel, data.PlayValue, isGold);

                            newScale.x = data.FillRatio / 100f;

                            // Probably don't need this clamp anymore, but it shouldn't hurt to have it.
                            newScale.x = Math.Max(newScale.x, 0);
                            newScale.x = Math.Min(newScale.x, 1);
                            Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 5", 2);

                            // Previously was resizing the filled area
                            // Currently resizing the empty area
                            // Hopefully this math is correct
                            newScale.x -= 1;
                            emptyImage.transform.localScale = newScale;
                            image.color = data.Color;
                            Plugin.LogInfo(LogType.Info, "UpdateRequirementBar: 6", 2);
                        }

                        // Why is this even here?
                        // This should only be updated once at the start of the 2nd and 3rd song
                        // This is instead being updated any time the main bar is updated for that border, when nothing here should change
                        //if (DaniPlayManager.GetCurrentSongNumber() > 0)
                        //{

                        //    var songValues = DaniPlayManager.GetBorderPlayResults(borders[j]);
                        //    var prevSongBar1 = panel.transform.Find("PrevSongHitReqOneBarFill").gameObject;
                        //    if (prevSongBar1 != null)
                        //    {
                        //        var image = prevSongBar1.GetOrAddComponent<Image>();
                        //        if (image != null)
                        //        {
                        //            BorderBarData songData1 = DaniPlayManager.GetBorderBarData(borders[j], DaniPlayManager.GetCurrentPlay(), 0);

                        //            var newScale = image.transform.localScale;
                        //            newScale.x = songData1.FillRatio / 100f;

                        //            newScale.x = Math.Max(newScale.x, 0);
                        //            newScale.x = Math.Min(newScale.x, 1);
                        //            image.transform.localScale = newScale;
                        //            image.color = songData1.Color;

                        //            if (songData1.State == BorderBarState.Rainbow)
                        //            {
                        //                AssetUtility.ChangeImageSprite(image, Path.Combine(BaseImageFilePath, "Enso", "PrevSongRainbow", "PrevSongRainbow.png"));
                        //            }
                        //        }
                        //    }
                        //    if (DaniPlayManager.GetCurrentSongNumber() > 1)
                        //    {
                        //        var prevSongBar2 = panel.transform.Find("PrevSongHitReqTwoBarFill").gameObject;
                        //        if (prevSongBar2 != null)
                        //        {
                        //            var image = prevSongBar2.GetOrAddComponent<Image>();
                        //            if (image != null)
                        //            {
                        //                BorderBarData songData1 = DaniPlayManager.GetBorderBarData(borders[j], DaniPlayManager.GetCurrentPlay(), 1);
                        //                var newScale = image.transform.localScale;
                        //                newScale.x = songData1.FillRatio / 100f;

                        //                newScale.x = Math.Max(newScale.x, 0);
                        //                newScale.x = Math.Min(newScale.x, 1);
                        //                image.transform.localScale = newScale;
                        //                image.color = songData1.Color;

                        //                if (songData1.State == BorderBarState.Rainbow)
                        //                {
                        //                    AssetUtility.ChangeImageSprite(image, Path.Combine(BaseImageFilePath, "Enso", "PrevSongRainbow", "PrevSongRainbow.png"));
                        //                }
                        //            }
                        //        }

                        //    }
                        //}
                    }
                }
            }

            static void ChangeReqCurrentValue(string panel, int value, bool isGold)
            {
                GameObject panelObj = GameObject.Find(panel);
                ChangeReqCurrentValue(panelObj, value, isGold);
            }


            static List<Sprite> DigitBigBorder;
            static List<Sprite> DigitBigFill;
            static List<Sprite> DigitBigTransparent;

            static List<Sprite> DigitSmallBorder;
            static List<Sprite> DigitSmallFill;
            static List<Sprite> DigitSmallTransparent;

            /// <summary>
            /// Takes in a requirement panel, a value, and whether or not it's gold. Will change the current digits to the proper values, or add, or delete them if needed.
            /// </summary>
            /// <param name="panel"></param>
            /// <param name="value"></param>
            /// <param name="isGold"></param>
            static void ChangeReqCurrentValue(GameObject panel, int value, bool isGold)
            {
                DigitAssets.CreateRequirementBarNumber(panel, new Vector2(403, 44), value, RequirementBarType.Large, isGold ? RequirementBarState.Gold : RequirementBarState.Normal);
                return;
                //if (!Directory.Exists(Path.Combine(BaseImageFilePath, "Digits")))
                //{
                //    return;
                //}
                //InitializeDigitSpriteLists();
                //value = Mathf.Max(value, 0);
                //value = Mathf.Min(value, 99999999); // 99,999,999
                //string num = value.ToString();
                //for (int i = 0; i < "99999999".Length; i++)
                //{
                //    var numLocation = new Vector2(403 + (56 * i), 44);
                //    var baseNumberPath = Path.Combine(BaseImageFilePath, "Digits", "Big");

                //    var digitBorderTransform = panel.transform.Find("CurReqBarValueBorder" + i);
                //    var digitFillTransform = panel.transform.Find("CurReqBarValueFill" + i);
                //    var digitTransparentTransform = panel.transform.Find("CurReqBarValueTransparent" + i);
                //    if (digitBorderTransform == null)
                //    {
                //        digitBorderTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueBorder" + i, Path.Combine(baseNumberPath, "Border", "0.png"), numLocation, panel.transform).transform;
                //    }
                //    if (digitFillTransform == null)
                //    {
                //        digitFillTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueFill" + i, Path.Combine(baseNumberPath, "NoBorder", "0.png"), numLocation, panel.transform).transform;
                //    }
                //    if (digitTransparentTransform == null)
                //    {
                //        digitTransparentTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueTransparent" + i, Path.Combine(baseNumberPath, "Transparent", "0.png"), numLocation, panel.transform).transform;
                //    }

                //    GameObject digitBorder = digitBorderTransform.gameObject;
                //    GameObject digitFill = digitFillTransform.gameObject;
                //    GameObject digitTransparent = digitTransparentTransform.gameObject;

                //    if (i < num.Length)
                //    {
                //        int numValue = int.Parse(num[i].ToString());
                //        if (DigitBigBorder.Count > numValue)
                //        {
                //            DaniDojoAssetUtility.ChangeImageSprite(digitBorder, DigitBigBorder[numValue]);
                //        }
                //        if (DigitBigFill.Count > numValue)
                //        {
                //            DaniDojoAssetUtility.ChangeImageSprite(digitFill, DigitBigFill[numValue]);
                //        }
                //        if (DigitBigTransparent.Count > numValue)
                //        {
                //            DaniDojoAssetUtility.ChangeImageSprite(digitTransparent, DigitBigTransparent[numValue]);
                //        }

                //        var digitBorderImage = digitBorder.GetComponent<Image>();
                //        var digitFillImage = digitFill.GetComponent<Image>();
                //        var digitTransparentImage = digitTransparent.GetComponent<Image>();

                //        if (digitBorderImage != null)
                //        {
                //            if (isGold)
                //            {
                //                digitBorderImage.color = GoldReqTextBorderColor;
                //            }
                //            else if (num == "0")
                //            {
                //                digitBorderImage.color = ZeroTextBorderColor;
                //            }
                //            else
                //            {
                //                digitBorderImage.color = NormalTextBorderColor;
                //            }
                //        }
                //        if (digitFillImage != null)
                //        {
                //            if (isGold)
                //            {
                //                digitFillImage.color = GoldReqTextFillColor;
                //            }
                //            else if (num == "0")
                //            {
                //                digitFillImage.color = ZeroTextFillColor;
                //            }
                //            else
                //            {
                //                digitFillImage.color = NormalTextFillColor;
                //            }
                //        }
                //        if (digitTransparentImage != null)
                //        {
                //            if (isGold)
                //            {
                //                digitTransparentImage.color = GoldReqTextTransparentColor;
                //            }
                //            else if (num == "0")
                //            {
                //                digitTransparentImage.color = ZeroTextTransparentColor;
                //            }
                //            else
                //            {
                //                digitTransparentImage.color = NormalTextTransparentColor;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        var digitBorderImage = digitBorder.GetComponent<Image>();
                //        var digitFillImage = digitFill.GetComponent<Image>();
                //        var digitTransparentImage = digitTransparent.GetComponent<Image>();
                //        if (digitBorderImage != null)
                //        {
                //            digitBorderImage.color = InvisibleColor;
                //        }
                //        if (digitFillImage != null)
                //        {
                //            digitFillImage.color = InvisibleColor;
                //        }
                //        if (digitTransparentImage != null)
                //        {
                //            digitTransparentImage.color = InvisibleColor;
                //        }
                //    }

                //}
            }

            //static void InitializeDigitSpriteLists()
            //{
            //    //Plugin.Log.LogInfo("Initialize DigitBigBorder");

            //    if (DigitBigBorder == null)
            //    {
            //        DigitBigBorder = new List<Sprite>();
            //    }
            //    if (DigitBigBorder == null || DigitBigBorder.Count != 10)
            //    {
            //        DigitBigBorder.Clear();
            //        DigitBigBorder = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitBigBorder.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Big", "Border", i.ToString() + ".png")));
            //        }
            //    }
            //    //Plugin.Log.LogInfo("Initialize DigitBigFill");
            //    if (DigitBigFill == null)
            //    {
            //        DigitBigFill = new List<Sprite>();
            //    }
            //    if (DigitBigFill == null || DigitBigFill.Count != 10)
            //    {
            //        DigitBigFill.Clear();
            //        DigitBigFill = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitBigFill.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Big", "NoBorder", i.ToString() + ".png")));
            //        }
            //    }
            //    //Plugin.Log.LogInfo("Initialize DigitBigTransparent");
            //    if (DigitBigTransparent == null)
            //    {
            //        DigitBigTransparent = new List<Sprite>();
            //    }
            //    if (DigitBigTransparent == null || DigitBigTransparent.Count != 10)
            //    {
            //        DigitBigTransparent.Clear();
            //        DigitBigTransparent = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitBigTransparent.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Big", "Transparent", i.ToString() + ".png")));
            //        }
            //    }

            //    //Plugin.Log.LogInfo("Initialize DigitSmallBorder");
            //    if (DigitSmallBorder == null)
            //    {
            //        DigitSmallBorder = new List<Sprite>();
            //    }
            //    if (DigitSmallBorder == null || DigitSmallBorder.Count != 10)
            //    {
            //        DigitSmallBorder.Clear();
            //        DigitSmallBorder = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitSmallBorder.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Small", "Border", i.ToString() + ".png")));
            //        }
            //    }
            //    //Plugin.Log.LogInfo("Initialize DigitSmallFill");
            //    if (DigitSmallFill == null)
            //    {
            //        DigitSmallFill = new List<Sprite>();
            //    }
            //    if (DigitSmallFill == null || DigitSmallFill.Count != 10)
            //    {
            //        DigitSmallFill.Clear();
            //        DigitSmallFill = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitSmallFill.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Small", "NoBorder", i.ToString() + ".png")));
            //        }
            //    }
            //    //Plugin.Log.LogInfo("Initialize DigitSmallTransparent");
            //    if (DigitSmallTransparent == null)
            //    {
            //        DigitSmallTransparent = new List<Sprite>();
            //    }
            //    if (DigitSmallTransparent == null || DigitSmallTransparent.Count != 10)
            //    {
            //        DigitSmallTransparent.Clear();
            //        DigitSmallTransparent = new List<Sprite>();
            //        for (int i = 0; i < 10; i++)
            //        {
            //            DigitSmallTransparent.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine("Digits", "Small", "Transparent", i.ToString() + ".png")));
            //        }
            //    }
            //    //Plugin.Log.LogInfo("Initialize Digits Finish");
            //}
        }

        public static class SelectAssets
        {
            // This will be the name of the image file
            //enum SelectAssetName
            //{
            //    CourseBorderLeftBlue,
            //    CourseBorderLeftGaiden,
            //    CourseBorderLeftGold,
            //    CourseBorderLeftKyuu,
            //    CourseBorderLeftRed,
            //    CourseBorderLeftSilver,
            //    CourseBorderRightBlue,
            //    CourseBorderRightGaiden,
            //    CourseBorderRightGold,
            //    CourseBorderRightKyuu,
            //    CourseBorderRightRed,
            //    CourseBorderRightSilver,
            //    CourseEasy,
            //    CourseHard,
            //    CourseNormal,
            //    CourseOni,
            //    CourseUra,
            //    Star1,
            //    Star2,
            //    Star3,
            //    Star4,
            //    Star5,
            //    Star6,
            //    Star7,
            //    Star8,
            //    Star9,
            //    Star10,
            //    Star10Plus,
            //    BigGoldClear,
            //    BigGoldDFC,
            //    BigGoldFC,
            //    BigRedClear,
            //    BigRedDFC,
            //    BigRedFC,
            //    SmallGoldClear,
            //    SmallGoldDFC,
            //    SmallGoldFC,
            //    SmallRedClear,
            //    SmallRedDFC,
            //    SmallRedFC,
            //    Top1dan,
            //    Top1kyuu,
            //    Top2dan,
            //    Top2kyuu,
            //    Top3dan,
            //    Top3kyuu,
            //    Top4dan,
            //    Top4kyuu,
            //    Top5dan,
            //    Top5kyuu,
            //    Top6dan,
            //    Top7dan,
            //    Top8dan,
            //    Top9dan,
            //    Top10dan,
            //    TopBlueBg,
            //    TopChojin,
            //    TopGaiden,
            //    TopGoldBg,
            //    TopKuroto,
            //    TopKyuuBg,
            //    TopMeijin,
            //    TopOriginalBg,
            //    TopRedBg,
            //    TopSelectedDan,
            //    TopSilverBg,
            //    TopTatsujin,
            //    ArrowLeft,
            //    ArrowRight,
            //    BgLightsOff,
            //    BgLightsOn,
            //    CourseBg,
            //    PersonalBestBg,
            //    ReqSongIndicator1,
            //    ReqSongIndicator2,
            //    ReqSongIndicator3,
            //    ReqSongIndicatorTotal,
            //    RequirementsEachPanel,
            //    RequirementsMainPanel,
            //    RequirementsPanelBorder,
            //    SongIndicator1,
            //    SongIndicator2,
            //    SongIndicator3,
            //    SongTitleBg,
            //    SoulGaugeReqPanel,
            //    BlueBg,
            //    chojin,
            //    dan1,
            //    dan2,
            //    dan3,
            //    dan4,
            //    dan5,
            //    dan6,
            //    dan7,
            //    dan8,
            //    dan9,
            //    dan10,
            //    gaiden,
            //    GoldBg,
            //    kuroto,
            //    kyuu1,
            //    kyuu2,
            //    kyuu3,
            //    kyuu4,
            //    kyuu5,
            //    meijin,
            //    original,
            //    RedBg,
            //    SilverBg,
            //    TanBg,
            //    tatsujin,
            //    WoodBg,
            //}

            //static Dictionary<SelectAssetName, Sprite> SelectAssetSprites;

            public static void InitializeSceneAssets(GameObject parent, GameObject bgParent)
            {
                TaikoSingletonMonoBehaviour<CommonObjects>.Instance.InputGuide.GetComponentInChildren<Animator>().SetTrigger("Out");

                var daniDojoCanvas = parent.AddComponent<Canvas>();
                daniDojoCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                daniDojoCanvas.worldCamera = null;
                daniDojoCanvas.overrideSorting = true;

                var daniDojoCanvasScaler = parent.AddComponent<CanvasScaler>();
                daniDojoCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                daniDojoCanvasScaler.referenceResolution = new Vector2(1920, 1080);
                daniDojoCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
                daniDojoCanvasScaler.matchWidthOrHeight = 0;

                //DaniDojoAssetUtility.CreateImage("BgLightsOff", Path.Combine("Select", "BgLightsOff.png"), new Vector2(0, 0), parent.transform);
                //DaniDojoAssetUtility.CreateImage("BgLightsOn", Path.Combine("Select", "BgLightsOn.png"), new Vector2(0, 0), parent.transform);
                AssetUtility.CreateImageChild(bgParent, "BgLightsOff", Vector2.zero, Path.Combine("Select", "BgLightsOff.png"));
                AssetUtility.CreateImageChild(bgParent, "BgLightsOn", Vector2.zero, Path.Combine("Select", "BgLightsOn.png"));
                // TODO: Make a coroutine to flicker the BgLightsOn's transparency

                //InitializeSelectAssetSprites();
            }

            

            public static void CreateSeriesAssets(DaniSeries seriesInfo, GameObject parent)
            {
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

                GameObject seriesTitleObject = GameObject.Find("SeriesTitle");
                if (seriesTitleObject == null)
                {
                    seriesTitleObject = DaniDojoAssetUtility.CreateText("SeriesTitle", seriesInfo.Title, new Rect(1920 - 1092, 777, 1000, 100),
                                          fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.Japanese), fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.Japanese, DataConst.DefaultFontMaterialType.OutlineBlack),
                                          HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 255), parent.transform.parent);
                    var seriesTitleText = seriesTitleObject.GetComponent<TextMeshProUGUI>();
                    seriesTitleText.enableAutoSizing = false;
                    seriesTitleText.fontSize = 50;
                }
                else
                {
                    var seriesTitleText = seriesTitleObject.GetComponent<TextMeshProUGUI>();
                    seriesTitleText.text = seriesInfo.Title;
                }


                if (GameObject.Find("TopCourses"))
                {
                    GameObject.Destroy(GameObject.Find("TopCourses"));
                }

                
                var topCoursesParent = AssetUtility.CreateEmptyObject(parent, "TopCourses", new Vector2(0, 0));
                for (int i = 0; i < seriesInfo.Courses.Count; i++)
                {
                    var highScore = SaveDataManager.GetCourseRecord(seriesInfo.Courses[i].Hash);

                    string backgroundName;
                    switch (seriesInfo.Courses[i].Background)
                    {
                        case CourseBackground.Tan:
                            backgroundName = "Tan.png";
                            break;
                        case CourseBackground.Wood:
                            backgroundName = "Wood.png";
                            break;
                        case CourseBackground.Blue:
                            backgroundName = "Blue.png";
                            break;
                        case CourseBackground.Red:
                            backgroundName = "Red.png";
                            break;
                        case CourseBackground.Silver:
                            backgroundName = "Silver.png";
                            break;
                        case CourseBackground.Gold:
                            backgroundName = "Gold.png";
                            break;
                        case CourseBackground.Gaiden:
                            backgroundName = "Gaiden.png";
                            break;
                        default:
                            // This would ideally be Sousaku, but I don't have assets for that
                            backgroundName = "Gaiden.png";
                            break;
                    }
                    string topText;
                    string botText = "";

                    switch (seriesInfo.Courses[i].CourseLevel)
                    {
                        case DaniCourseLevel.kyuuFirst:
                        case DaniCourseLevel.dan1:
                            topText = "Starting.png";
                            break;
                        case DaniCourseLevel.kyuu10:
                        case DaniCourseLevel.dan10:
                            topText = "10th.png";
                            break;
                        case DaniCourseLevel.kyuu9:
                        case DaniCourseLevel.dan9:
                            topText = "9th.png";
                            break;
                        case DaniCourseLevel.kyuu8:
                        case DaniCourseLevel.dan8:
                            topText = "8th.png";
                            break;
                        case DaniCourseLevel.kyuu7:
                        case DaniCourseLevel.dan7:
                            topText = "7th.png";
                            break;
                        case DaniCourseLevel.kyuu6:
                        case DaniCourseLevel.dan6:
                            topText = "6th.png";
                            break;
                        case DaniCourseLevel.kyuu5:
                        case DaniCourseLevel.dan5:
                            topText = "5th.png";
                            break;
                        case DaniCourseLevel.kyuu4:
                        case DaniCourseLevel.dan4:
                            topText = "4th.png";
                            break;
                        case DaniCourseLevel.kyuu3:
                        case DaniCourseLevel.dan3:
                            topText = "3rd.png";
                            break;
                        case DaniCourseLevel.kyuu2:
                        case DaniCourseLevel.dan2:
                            topText = "2nd.png";
                            break;
                        case DaniCourseLevel.kyuu1:
                            topText = "1st.png";
                            break;
                        case DaniCourseLevel.kuroto:
                            topText = "kuro.png";
                            break;
                        case DaniCourseLevel.meijin:
                            topText = "mei.png";
                            break;
                        case DaniCourseLevel.chojin:
                            topText = "cho.png";
                            break;
                        case DaniCourseLevel.tatsujin:
                            topText = "tatsu.png";
                            break;
                        case DaniCourseLevel.gaiden:
                            topText = "gai.png";
                            botText = "den.png";
                            break;
                        default:
                            // Would be sou saku if I had the assets
                            //topText = "sou.png";
                            //botText = "saku.png";
                            topText = "gai.png";
                            botText = "den.png";
                            break;
                    }

                    switch (seriesInfo.Courses[i].CourseLevel)
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
                            botText = "kyuu.png";
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
                            botText = "dan.png";
                            break;
                        case DaniCourseLevel.kuroto:
                        case DaniCourseLevel.meijin:
                        case DaniCourseLevel.chojin:
                        case DaniCourseLevel.tatsujin:
                            botText = "jin.png";
                            break;
                    }
                    //var basePosition = AssetUtility.GetPositionFrom1080p(new Vector2(183, 884));
                    var basePosition = new Vector2(183, 884);
                    var topCourseObject = AssetUtility.CreateEmptyObject(topCoursesParent, seriesInfo.Courses[i].Title, basePosition + new Vector2(68 * i, 0));

                    //GameObject topCourseObject = new GameObject(seriesInfo.Courses[i].Title);
                    //topCourseObject.transform.SetParent(topCoursesParent.transform);
                    //topCourseObject.transform.position = new Vector2(183 + (68 * i), 884);
                    AssetUtility.CreateImageChild(topCourseObject, seriesInfo.Courses[i].Title + "Background", Vector2.zero, Path.Combine("Course", "Top", "Bg", backgroundName));
                    AssetUtility.CreateImageChild(topCourseObject, seriesInfo.Courses[i].Title + "TopText", new Vector2(19, 62), Path.Combine("Course", "Top", "Text", topText));
                    AssetUtility.CreateImageChild(topCourseObject, seriesInfo.Courses[i].Title + "BotText", new Vector2(19, 24), Path.Combine("Course", "Top", "Text", botText));
                    //DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Background", GetAssetSprite(backgroundName), new Vector2(0, 0), topCourseObject.transform);
                    //DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Text", GetAssetSprite(textName), new Vector2(19, 24), topCourseObject.transform);

                    bool skip = false;
                    string recordName = string.Empty;
                    if (highScore.RankCombo.Rank == DaniRank.GoldClear)
                    {
                        if (highScore.RankCombo.Combo == DaniCombo.Rainbow)
                        {
                            recordName = "SmallGoldDFC.png";
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Gold)
                        {
                            recordName = "SmallGoldFC.png";
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Silver)
                        {
                            recordName = "SmallGoldClear.png";
                        }
                    }
                    else if (highScore.RankCombo.Rank == DaniRank.RedClear)
                    {
                        if (highScore.RankCombo.Combo == DaniCombo.Rainbow)
                        {
                            recordName = "SmallRedDFC.png";
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Gold)
                        {
                            recordName = "SmallRedFC.png";
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Silver)
                        {
                            recordName = "SmallRedClear.png";
                        }
                    }

                    if (recordName != string.Empty)
                    {
                        AssetUtility.CreateImageChild(topCourseObject, seriesInfo.Courses[i].Title + "Record", new Vector2(0, 98), Path.Combine("Select", "ResultIcons", "Small", recordName));
                        //DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Record", GetAssetSprite(recordName), new Vector2(0, 98), topCourseObject.transform);
                    }
                }
            }

            public enum CourseCreateDir
            {
                Left,
                Right,
                Up,
                Down,
                Center,
            }
            public enum TopCourseMove
            {
                Left,
                Right,
            }

            static int courseBgNum = 0;
            public static GameObject CreateCourseAssets(DaniCourse courseInfo, GameObject parent, CourseCreateDir dir = CourseCreateDir.Center)
            {
                var highScore = SaveDataManager.GetCourseRecord(courseInfo.Hash);

                Vector2 CourseBgLocation = new Vector2(342, 26);
                switch (dir)
                {
                    case CourseCreateDir.Left:
                        CourseBgLocation.x += 1920;
                        break;
                    case CourseCreateDir.Right:
                        CourseBgLocation.x -= 1920;
                        break;
                    case CourseCreateDir.Up:
                        CourseBgLocation.y -= 1080;
                        break;
                    case CourseCreateDir.Down:
                        CourseBgLocation.y += 1080;
                        break;
                }

                var courseObject = AssetUtility.CreateImageChild(parent, "CourseBg" + courseBgNum++, CourseBgLocation, Path.Combine("Select", "CourseBg.png"));
                //var courseObject = DaniDojoAssetUtility.CreateImage("CourseBg" + courseBgNum++, GetAssetSprite(SelectAssetName.CourseBg), CourseBgLocation, parent.transform);

                string borderFileName = string.Empty;
                borderFileName = courseInfo.Background switch
                {
                    CourseBackground.Tan => "Tan.png",
                    CourseBackground.Wood => "Wood.png",
                    CourseBackground.Blue => "Blue.png",
                    CourseBackground.Red => "Red.png",
                    CourseBackground.Silver => "Silver.png",
                    CourseBackground.Gold => "Gold.png",
                    CourseBackground.Gaiden => "Gaiden.png",
                    _ => "Sousaku.png",
                };

                //SelectAssetName leftBorderName;
                //SelectAssetName rightBorderName;
                //var background = courseInfo.Background;

                //switch (background)
                //{
                //    case CourseBackground.None:
                //    case CourseBackground.Tan:
                //        leftBorderName = SelectAssetName.CourseBorderLeftGaiden;
                //        rightBorderName = SelectAssetName.CourseBorderRightGaiden;
                //        break;
                //    case CourseBackground.Wood:
                //        leftBorderName = SelectAssetName.CourseBorderLeftKyuu;
                //        rightBorderName = SelectAssetName.CourseBorderRightKyuu;
                //        break;
                //    case CourseBackground.Blue:
                //        leftBorderName = SelectAssetName.CourseBorderLeftBlue;
                //        rightBorderName = SelectAssetName.CourseBorderRightBlue;
                //        break;
                //    case CourseBackground.Red:
                //        leftBorderName = SelectAssetName.CourseBorderLeftRed;
                //        rightBorderName = SelectAssetName.CourseBorderRightRed;
                //        break;
                //    case CourseBackground.Silver:
                //        leftBorderName = SelectAssetName.CourseBorderLeftSilver;
                //        rightBorderName = SelectAssetName.CourseBorderRightSilver;
                //        break;
                //    case CourseBackground.Gold:
                //        leftBorderName = SelectAssetName.CourseBorderLeftGold;
                //        rightBorderName = SelectAssetName.CourseBorderRightGold;
                //        break;
                //    default:
                //        leftBorderName = SelectAssetName.CourseBorderLeftGaiden;
                //        rightBorderName = SelectAssetName.CourseBorderRightGaiden;
                //        break;
                //}
                AssetUtility.CreateImageChild(courseObject, "LeftBorder", new Vector2(22, 33), Path.Combine("Course", "CourseSelect", "Borders", "Left", borderFileName));
                AssetUtility.CreateImageChild(courseObject, "RightBorder", new Vector2(1458, 33), Path.Combine("Course", "CourseSelect", "Borders", "Right", borderFileName));
                //DaniDojoAssetUtility.CreateImage("LeftBorder", GetAssetSprite(leftBorderName), new Vector2(22, 33), courseObject.transform);
                //DaniDojoAssetUtility.CreateImage("RightBorder", GetAssetSprite(rightBorderName), new Vector2(1458, 33), courseObject.transform);



                var wordDataMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.WordDataMgr;
                List<MusicDataInterface.MusicInfoAccesser> musicInfoAccessers = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.MusicData.musicInfoAccessers;
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

                var titleFontType = wordDataMgr.GetWordListInfo("song_struck").FontType;
                var detailFontType = wordDataMgr.GetWordListInfo("song_detail_struck").FontType;

                var titleFont = fontTMPMgr.GetDefaultFontAsset(titleFontType);
                var detailFont = fontTMPMgr.GetDefaultFontAsset(detailFontType);

                var titleFontMaterial = fontTMPMgr.GetDefaultFontMaterial(titleFontType, DataConst.DefaultFontMaterialType.KanbanSelect);
                var detailFontMaterial = fontTMPMgr.GetDefaultFontMaterial(detailFontType, DataConst.DefaultFontMaterialType.Plane);


                // Add song info
                // Hard coded to 3 songs per course
                // Changed hard code to be a max of 3 songs per course, helps with testing courses with just 1 song

                for (int i = 0; i < Math.Min(courseInfo.Songs.Count, 3); i++)
                {

                    //GameObject songParent = DaniDojoAssetUtility.CreateImage("SongBg" + (i + 1), GetAssetSprite(SelectAssetName.SongTitleBg), new Vector2(78, 718 - (110 * i)), courseObject.transform);
                    var songParent = AssetUtility.CreateImageChild(courseObject, "SongBg" + (i + 1), new Vector2(78, 718 - (110 * i)), Path.Combine("Select", "SongTitleBg.png"));

                    //SelectAssetName songIndicator;
                    //if (i == 0)
                    //{
                    //    songIndicator = SelectAssetName.SongIndicator1;
                    //}
                    //else if (i == 1)
                    //{
                    //    songIndicator = SelectAssetName.SongIndicator2;
                    //}
                    //else
                    //{
                    //    songIndicator = SelectAssetName.SongIndicator3;
                    //}
                    //DaniDojoAssetUtility.CreateImage("SongIndicator", GetAssetSprite(songIndicator), new Vector2(10, 10), songParent.transform);
                    AssetUtility.CreateImageChild(songParent, "SongIndicator", new Vector2(10, 10), Path.Combine("Select", "SongIndicator" + (i + 1) + ".png"));

                    //var song = musicInfoAccessers.Find((x) => x.Id == courseInfo.Songs[i].SongId);
                    //string songTitle = "Song not found: ";
                    //if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Jp")
                    //{
                    //    songTitle += courseInfo.Songs[i].TitleJp;
                    //}
                    //else if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Eng")
                    //{
                    //    songTitle += courseInfo.Songs[i].TitleEng;
                    //}
                    //else
                    //{
                    //    songTitle += courseInfo.Songs[i].SongId;
                    //}
                    //string songDetail = "";
                    //if (song != null)
                    //{
                    //    songTitle = wordDataMgr.GetWordListInfo("song_" + song.Id).Text;
                    //    songDetail = wordDataMgr.GetWordListInfo("song_detail_" + song.Id).Text;
                    //}

                    //if (courseInfo.Songs[i].IsHidden)
                    //{
                    //    // SongReached is 0 indexed
                    //    if (highScore.SongReached <= i)
                    //    {
                    //        songTitle = "? ? ?";
                    //        songDetail = "";
                    //    }
                    //}


                    //SelectAssetName levelAssetName;
                    //switch (courseInfo.Songs[i].Level)
                    //{
                    //    case EnsoData.EnsoLevelType.Easy: levelAssetName = SelectAssetName.CourseEasy; break;
                    //    case EnsoData.EnsoLevelType.Normal: levelAssetName = SelectAssetName.CourseNormal; break;
                    //    case EnsoData.EnsoLevelType.Hard: levelAssetName = SelectAssetName.CourseHard; break;
                    //    case EnsoData.EnsoLevelType.Mania: levelAssetName = SelectAssetName.CourseOni; break;
                    //    case EnsoData.EnsoLevelType.Ura: levelAssetName = SelectAssetName.CourseUra; break;
                    //    default: levelAssetName = SelectAssetName.CourseOni; break;
                    //}
                    //DaniDojoAssetUtility.CreateImage("SongCourse", GetAssetSprite(levelAssetName), new Vector2(112, 42), songParent.transform);

                    //SelectAssetName starAssetName;

                    //if (song != null)
                    //{
                    //    switch (song.Stars[(int)courseInfo.Songs[i].Level])
                    //    {
                    //        case 1: starAssetName = SelectAssetName.Star1; break;
                    //        case 2: starAssetName = SelectAssetName.Star2; break;
                    //        case 3: starAssetName = SelectAssetName.Star3; break;
                    //        case 4: starAssetName = SelectAssetName.Star4; break;
                    //        case 5: starAssetName = SelectAssetName.Star5; break;
                    //        case 6: starAssetName = SelectAssetName.Star6; break;
                    //        case 7: starAssetName = SelectAssetName.Star7; break;
                    //        case 8: starAssetName = SelectAssetName.Star8; break;
                    //        case 9: starAssetName = SelectAssetName.Star9; break;
                    //        case 10: starAssetName = SelectAssetName.Star10; break;
                    //        default: starAssetName = SelectAssetName.Star10Plus; break; // Might as well have this as a default, it's kinda fun
                    //    }
                    //}
                    //else
                    //{
                    //    starAssetName = SelectAssetName.Star10Plus;
                    //}

                    //DaniDojoAssetUtility.CreateImage("SongLevel", GetAssetSprite(starAssetName), new Vector2(121, 15), songParent.transform);


                    CommonAssets.CreateSongCourseChild(songParent, new Vector2(112, 42), courseInfo.Songs[i]);

                    CommonAssets.CreateSongLevelChild(songParent, new Vector2(121, 15), courseInfo.Songs[i]);


                    Vector2 titleRect = new Vector2(220, 28);
                    Vector2 detailRect = new Vector2(220, 70);

                    CommonAssets.CreateSongTitleChild(songParent, titleRect, courseInfo.Songs[i], courseInfo.Songs[i].IsHidden && highScore.SongReached <= i);
                    CommonAssets.CreateSongDetailChild(songParent, detailRect, courseInfo.Songs[i], courseInfo.Songs[i].IsHidden && highScore.SongReached <= i);



                    //DaniDojoAssetUtility.CreateText("SongTitle", songTitle, titleRect, titleFont, titleFontMaterial, HorizontalAlignmentOptions.Left, Color.black, songParent.transform);
                    //var detail = DaniDojoAssetUtility.CreateText("SongDetail", songDetail, detailRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, Color.black, songParent.transform);
                    //detail.GetComponent<TextMeshProUGUI>().color = Color.black;

                }


                // Add Requirements info
                //DaniDojoAssetUtility.CreateImage("RequirementsMainPanel", GetAssetSprite(SelectAssetName.RequirementsMainPanel), new Vector2(78, 412), courseObject.transform);
                //DaniDojoAssetUtility.CreateImage("RequirementsPanelLeftBorder", GetAssetSprite(SelectAssetName.RequirementsPanelBorder), new Vector2(78, 33), courseObject.transform);
                //DaniDojoAssetUtility.CreateImage("RequirementsPanelRightBorder", GetAssetSprite(SelectAssetName.RequirementsPanelBorder), new Vector2(1440, 33), courseObject.transform);

                AssetUtility.CreateImageChild(courseObject, "RequirementsMainPanel", new Vector2(78, 417), Path.Combine("Select", "RequirementsMainPanel"));
                AssetUtility.CreateImageChild(courseObject, "RequirementsPanelLeftBorder", new Vector2(78, 33), Path.Combine("Select", "RequirementsPanelBorder"));
                AssetUtility.CreateImageChild(courseObject, "RequirementsPanelRightBorder", new Vector2(1440, 33), Path.Combine("Select", "RequirementsPanelBorder"));

                // Add Soul Gauge Requirements
                //var SoulGaugeReqPanel = DaniDojoAssetUtility.CreateImage("SoulGaugeReqPanel", GetAssetSprite(SelectAssetName.SoulGaugeReqPanel), new Vector2(94, 268), courseObject.transform);
                var SoulGaugeReqPanel = AssetUtility.CreateImageChild(courseObject, "SoulGaugeReqPanel", new Vector2(94, 268), Path.Combine("Select", "SoulGaugeReqPanel"));


                TMP_FontAsset reqTypeFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
                Material reqTypeFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);

                TMP_FontAsset reqValueFont = fontTMPMgr.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                Material reqValueFontMaterial = fontTMPMgr.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                Rect SoulGaugeHeaderRect = new Rect(40, 139, 240, 25);
                //DaniDojoAssetUtility.CreateText("SoulGaugeHeader", "Soul Gauge", SoulGaugeHeaderRect, reqTypeFont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), SoulGaugeReqPanel.transform);
                var soulGaugeHeader = AssetUtility.CreateTextChild(SoulGaugeReqPanel, "SoulGaugeHeader", SoulGaugeHeaderRect, "Soul Gauge");
                AssetUtility.SetTextFontAndMaterial(soulGaugeHeader, reqTypeFont, reqTypeFontMaterial);
                AssetUtility.SetTextAlignment(soulGaugeHeader, HorizontalAlignmentOptions.Center);

                int soulGaugeRequirement = 100;
                for (int i = 0; i < courseInfo.Borders.Count; i++)
                {
                    if (courseInfo.Borders[i].BorderType == BorderType.SoulGauge)
                    {
                        soulGaugeRequirement = courseInfo.Borders[i].RedReqs[0];
                        break;
                    }
                }

                Rect SoulGaugeReqValueRect = new Rect(50, 73, 256, 39);
                //DaniDojoAssetUtility.CreateText("SoulGaugeReqValue", soulGaugeRequirement.ToString() + "% or higher", SoulGaugeReqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), SoulGaugeReqPanel.transform);
                var soulGaugeReqValue = AssetUtility.CreateTextChild(SoulGaugeReqPanel, "SoulGaugeReqValue", SoulGaugeReqValueRect, soulGaugeRequirement.ToString() + "% or higher");
                AssetUtility.SetTextFontAndMaterial(soulGaugeReqValue, reqValueFont, reqValueFontMaterial);
                AssetUtility.SetTextAlignment(soulGaugeReqValue, HorizontalAlignmentOptions.Right);

                if (highScore.SongReached > 0)
                {
                    Rect SoulGaugeHighScoreHeaderRect = new Rect(12, 27, 100, 25);
                    //var SoulGaugeHighScoreHeader = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreHeader", "High Score", SoulGaugeHighScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    //SoulGaugeHighScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                    var SoulGaugeHighScoreHeader = AssetUtility.CreateTextChild(SoulGaugeReqPanel, "SoulGaugeHighScoreHeader", SoulGaugeHighScoreHeaderRect, "High Score");
                    AssetUtility.SetTextFontAndMaterial(SoulGaugeHighScoreHeader, detailFont, detailFontMaterial);
                    AssetUtility.SetTextAlignment(SoulGaugeHighScoreHeader, HorizontalAlignmentOptions.Left);
                    AssetUtility.SetTextColor(SoulGaugeHighScoreHeader, Color.black);

                    Rect SoulGaugeHighScoreValueRect = new Rect(200, 20, 100, 40);
                    //var SoulGaugeHighScoreValue = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreValue", highScore.PlayData.Max((x) => x.SoulGauge).ToString() + " %", SoulGaugeHighScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    //SoulGaugeHighScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;

                    var SoulGaugeHighScoreValue = AssetUtility.CreateTextChild(SoulGaugeReqPanel, "SoulGaugeHighScoreValue", SoulGaugeHighScoreValueRect, highScore.PlayData.Max((x) => x.SoulGauge).ToString() + " %");
                    AssetUtility.SetTextFontAndMaterial(SoulGaugeHighScoreValue, detailFont, detailFontMaterial);
                    AssetUtility.SetTextAlignment(SoulGaugeHighScoreValue, HorizontalAlignmentOptions.Right);
                    AssetUtility.SetTextColor(SoulGaugeHighScoreValue, Color.black);
                }


                // Add other Requirements
                bool passedSoulGauge = false;
                for (int i = 0; i < courseInfo.Borders.Count; i++)
                {
                    if (courseInfo.Borders[i].BorderType == BorderType.SoulGauge)
                    {
                        passedSoulGauge = true;
                        continue;
                    }
                    //var reqPanel = DaniDojoAssetUtility.CreateImage("ReqPanel" + (i + 1), GetAssetSprite(SelectAssetName.RequirementsEachPanel), new Vector2(427, 310 - (passedSoulGauge ? (i - 1) * 132 : i * 132)), courseObject.transform);
                    var reqPanel = AssetUtility.CreateImageChild(courseObject, "ReqPanel" + (i + 1), new Vector2(427, 310 - (passedSoulGauge ? (i - 1) * 132 : i * 132)), Path.Combine("Select", "RequirementsEachPanel.png"));

                    Rect reqPanelHeaderRect = new Rect(34, 95, 240, 25);
                    string requirementTypeText = string.Empty;
                    switch (courseInfo.Borders[i].BorderType)
                    {
                        case BorderType.Goods: requirementTypeText = "GOODs"; break;
                        case BorderType.Oks: requirementTypeText = "OKs"; break;
                        case BorderType.Bads: requirementTypeText = "BADs"; break;
                        case BorderType.Combo: requirementTypeText = "Combo"; break;
                        case BorderType.Drumroll: requirementTypeText = "Drumrolls"; break;
                        case BorderType.Score: requirementTypeText = "Score"; break;
                        case BorderType.TotalHits: requirementTypeText = "Total Hits"; break;
                    }
                    //DaniDojoAssetUtility.CreateText("SoulGaugeHeader", requirementTypeText, reqPanelHeaderRect, reqTypeFont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), reqPanel.transform);
                    var soulGaugeHeaderText = AssetUtility.CreateTextChild(reqPanel, "SoulGaugeHeader", reqPanelHeaderRect, requirementTypeText);
                    AssetUtility.SetTextFontAndMaterial(soulGaugeHeaderText, reqTypeFont, reqTypeFontMaterial);
                    AssetUtility.SetTextAlignment(soulGaugeHeaderText, HorizontalAlignmentOptions.Center);


                    if (courseInfo.Borders[i].IsTotal)
                    {
                        //var reqPanelValue = DaniDojoAssetUtility.CreateImage("ReqPanelValue" + (i + 1), GetAssetSprite(SelectAssetName.ReqSongIndicatorTotal), new Vector2(14, 49), reqPanel.transform);
                        var reqPanelValue = AssetUtility.CreateImageChild(reqPanel, "ReqPanelValue" + (i + 1), new Vector2(14, 49), Path.Combine("Select", "ReqSongIndicatorTotal.png"));


                        string valueText = string.Empty;
                        if (courseInfo.Borders[i].BorderType == BorderType.Oks || courseInfo.Borders[i].BorderType == BorderType.Bads)
                        {
                            valueText = "Less than " + courseInfo.Borders[i].RedReqs[0];
                        }
                        else
                        {
                            valueText = courseInfo.Borders[i].RedReqs[0] + " or more";
                        }
                        Rect reqValueRect = new Rect(50, 0, 256, 31);
                        //DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);
                        var reqValueText = AssetUtility.CreateTextChild(reqPanelValue, "ReqValue" + (i + 1), reqValueRect, valueText);
                        AssetUtility.SetTextFontAndMaterial(reqValueText, reqValueFont, reqValueFontMaterial);
                        AssetUtility.SetTextAlignment(reqValueText, HorizontalAlignmentOptions.Right);


                        //var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (i + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14, 11), reqPanel.transform);
                        var reqPanelBest = AssetUtility.CreateImageChild(reqPanel, "ReqPanelBest" + (i + 1), new Vector2(14, 11), Path.Combine("Select", "PersonalBestBg.png"));


                        if (highScore.SongReached > 0)
                        {
                            Rect highScoreHeaderRect = new Rect(12, 3, 100, 25);
                            var highScoreHeader = DaniDojoAssetUtility.CreateText("HighScoreHeader", "High Score", highScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                            highScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                            string highScoreValueText = string.Empty;
                            switch (courseInfo.Borders[i].BorderType)
                            {
                                case BorderType.Goods: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData.Sum((y) => y.Goods)).ToString(); break;
                                case BorderType.Oks: highScoreValueText = highScore.PlayData.Min((x) => x.SongPlayData.Sum((y) => y.Oks)).ToString(); break;
                                case BorderType.Bads: highScoreValueText = highScore.PlayData.Min((x) => x.SongPlayData.Sum((y) => y.Bads)).ToString(); break;
                                case BorderType.Combo: highScoreValueText = highScore.PlayData.Max((x) => x.MaxCombo).ToString(); break;
                                case BorderType.Drumroll: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData.Sum((y) => y.Drumroll)).ToString(); break;
                                case BorderType.Score: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData.Sum((y) => y.Score)).ToString(); break;
                                case BorderType.TotalHits: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData.Sum((y) => y.Goods + y.Oks + y.Drumroll)).ToString(); break;
                            }

                            Rect highScoreValueRect = new Rect(200, -3, 100, 35);
                            //var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                            var highScoreValue = AssetUtility.CreateTextChild(reqPanelBest, "HighScoreValue", highScoreValueRect, highScoreValueText);
                            AssetUtility.SetTextFontAndMaterial(highScoreValue, detailFont, detailFontMaterial);
                            AssetUtility.SetTextAlignment(highScoreValue, HorizontalAlignmentOptions.Right);
                            highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Math.Min(courseInfo.Songs.Count, 3); j++)
                        {
                            int xOffset = (326 * j);
                            //SelectAssetName SongIndicatorAsset = SelectAssetName.ReqSongIndicator1;
                            //if (j == 1)
                            //{
                            //    SongIndicatorAsset = SelectAssetName.ReqSongIndicator2;
                            //}
                            //else if (j == 2)
                            //{
                            //    SongIndicatorAsset = SelectAssetName.ReqSongIndicator3;
                            //}
                            //var reqPanelValue = DaniDojoAssetUtility.CreateImage("ReqPanelValue" + (j + 1), GetAssetSprite(SongIndicatorAsset), new Vector2(14 + xOffset, 49), reqPanel.transform);
                            var reqPanelValue = AssetUtility.CreateImageChild(reqPanel, "ReqPanelValue" + (j + 1), new Vector2(14 + xOffset, 49), Path.Combine("Select", "ReqSongIndicator" + (j + 1) + ".png"));
                            string valueText = string.Empty;
                            if (courseInfo.Borders[i].BorderType == BorderType.Oks || courseInfo.Borders[i].BorderType == BorderType.Bads)
                            {
                                valueText = "Less than " + courseInfo.Borders[i].RedReqs[j];
                            }
                            else
                            {
                                valueText = courseInfo.Borders[i].RedReqs[j] + " or more";
                            }
                            Rect reqValueRect = new Rect(50, 0, 256, 31);
                            //DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);
                            var reqValueText = AssetUtility.CreateTextChild(reqPanelValue, "ReqValue" + (i + 1), reqValueRect, valueText);
                            AssetUtility.SetTextFontAndMaterial(reqValueText, reqValueFont, reqValueFontMaterial);
                            AssetUtility.SetTextAlignment(reqValueText, HorizontalAlignmentOptions.Right);


                            //var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (j + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14 + xOffset, 11), reqPanel.transform);
                            var reqPanelBest = AssetUtility.CreateImageChild(reqPanel, "ReqPanelBest" + (j + 1), new Vector2(14 + xOffset, 11), Path.Combine("Select", "PersonalBestBg.png"));


                            // SongReached of 0 means it wasn't played
                            if (highScore.SongReached > 0)
                            {
                                Rect highScoreHeaderRect = new Rect(17, 3, 100, 25);
                                var highScoreHeader = DaniDojoAssetUtility.CreateText("HighScoreHeader", "High Score", highScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                                highScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                                string highScoreValueText = string.Empty;
                                switch (courseInfo.Borders[i].BorderType)
                                {
                                    case BorderType.Goods: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData[j].Goods).ToString(); break;
                                    case BorderType.Oks: highScoreValueText = highScore.PlayData.Min((x) => x.SongPlayData[j].Oks).ToString(); break;
                                    case BorderType.Bads: highScoreValueText = highScore.PlayData.Min((x) => x.SongPlayData[j].Bads).ToString(); break;
                                    case BorderType.Combo: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData[j].Combo).ToString(); break;
                                    case BorderType.Drumroll: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData[j].Drumroll).ToString(); break;
                                    case BorderType.Score: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData[j].Score).ToString(); break;
                                    case BorderType.TotalHits: highScoreValueText = highScore.PlayData.Max((x) => x.SongPlayData[j].Goods + x.SongPlayData[j].Oks + x.SongPlayData[j].Drumroll).ToString(); break;
                                }

                                Rect highScoreValueRect = new Rect(200, -3, 100, 35);
                                //var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                                //highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                                var highScoreValue = AssetUtility.CreateTextChild(reqPanelBest, "HighScoreValue", highScoreValueRect, highScoreValueText);
                                AssetUtility.SetTextFontAndMaterial(highScoreValue, detailFont, detailFontMaterial);
                                AssetUtility.SetTextAlignment(highScoreValue, HorizontalAlignmentOptions.Right);
                                AssetUtility.SetTextColor(highScoreValue, Color.black);

                            }
                        }
                    }

                }

                var baseCourseTagPos = new Vector2(-212, 332);


                CommonAssets.CreateDaniCourse(courseObject, baseCourseTagPos, courseInfo);
                CommonAssets.CreateCourseTitleBar(courseObject, new Vector2(21, 822), courseInfo);

                //else
                //{
                //    var bgImage = courseInfo.Background switch
                //    {
                //        CourseBackground.Wood => SelectAssetName.WoodBg,
                //        CourseBackground.Blue => SelectAssetName.BlueBg,
                //        CourseBackground.Red => SelectAssetName.RedBg,
                //        CourseBackground.Silver => SelectAssetName.SilverBg,
                //        CourseBackground.Gold => SelectAssetName.GoldBg,
                //        _ => SelectAssetName.TanBg,
                //    };

                //    var courseId = courseInfo.Id;
                //    if (int.TryParse(courseId, out int _))
                //    {
                //        courseId = courseInfo.Title;
                //    }

                //    var textImage = courseId switch
                //    {
                //        "5kyuu" or "五級 5th Kyu" => SelectAssetName.kyuu5,
                //        "4kyuu" or "四級 4th Kyu" => SelectAssetName.kyuu4,
                //        "3kyuu" or "三級 3rd Kyu" => SelectAssetName.kyuu3,
                //        "2kyuu" or "二級 2nd Kyu" => SelectAssetName.kyuu2,
                //        "1kyuu" or "一級 1st Kyu" => SelectAssetName.kyuu1,
                //        "1dan" or "初段 1st Dan" => SelectAssetName.dan1,
                //        "2dan" or "二段 2nd Dan" => SelectAssetName.dan2,
                //        "3dan" or "三段 3rd Dan" => SelectAssetName.dan3,
                //        "4dan" or "四段 4th Dan" => SelectAssetName.dan4,
                //        "5dan" or "五段 5th Dan" => SelectAssetName.dan5,
                //        "6dan" or "六段 6th Dan" => SelectAssetName.dan6,
                //        "7dan" or "七段 7th Dan" => SelectAssetName.dan7,
                //        "8dan" or "八段 8th Dan" => SelectAssetName.dan8,
                //        "9dan" or "九段 9th Dan" => SelectAssetName.dan9,
                //        "10dan" or "十段 10th Dan" => SelectAssetName.dan10,
                //        "11dan" or "玄人 Kuroto" => SelectAssetName.kuroto,
                //        "12dan" or "名人 Meijin" => SelectAssetName.meijin,
                //        "13dan" or "超人 Chojin" => SelectAssetName.chojin,
                //        "14dan" or "達人 Tatsujin" => SelectAssetName.tatsujin,
                //        _ => SelectAssetName.gaiden,
                //    };

                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", GetAssetSprite(bgImage), baseCourseTagPos, courseObject.transform);
                //    DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", GetAssetSprite(textImage), new Vector2(baseCourseTagPos.x + 52, baseCourseTagPos.y + 124), courseObject.transform);
                //}

                if (highScore.RankCombo.Rank != DaniRank.None)
                {
                    string recordName = "Big";

                    switch (highScore.RankCombo.Rank)
                    {
                        case DaniRank.RedClear: recordName += "Red"; break;
                        case DaniRank.GoldClear: recordName += "Gold"; break;
                    }
                    switch (highScore.RankCombo.Combo) // Rename these files
                    {
                        case DaniCombo.Silver: recordName += "Clear"; break;
                        case DaniCombo.Gold: recordName += "FC"; break;
                        case DaniCombo.Rainbow: recordName += "DFC"; break;
                    }

                    // DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Record", GetAssetSprite(recordName), new Vector2(0, 98), topCourseObject.transform);
                    AssetUtility.CreateImageChild(courseObject, "DanResult", new Vector2(-242, 291), Path.Combine("Select", "ResultIcons", "Big", recordName + ".png")); // IDK what the file path will be
                }

                return courseObject;
            }
        }

        public static class ResultAssets
        {

        }
    }
}
