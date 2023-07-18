﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DaniDojo.Patches.DaniDojoDaniCourseSelect;

namespace DaniDojo.Patches
{
    internal static class DaniDojoAssets
    {
        static string BaseImageFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;


        static Color32 InvisibleColor = new Color32(0, 0, 0, 0);

        static Color32 PinkBarColor = new Color32(254, 161, 183, 255);
        static Color32 YellowBarColor = new Color32(249, 254, 55, 255);
        static Color32 RedBarColor = new Color32(250, 124, 78, 255);
        static Color32 GreyBarColor = new Color32(68, 69, 68, 255);

        static Color32 GoldReqTextBorderColor = new Color32(221, 89, 56, 255);
        static Color32 GoldReqTextFillColor = new Color32(255, 93, 127, 255);
        static Color32 GoldReqTextTransparentColor = new Color32(255, 244, 45, 255);

        static Color32 NormalTextBorderColor = new Color32(177, 177, 177, 255);
        static Color32 NormalTextFillColor = new Color32(255, 255, 255, 255);
        static Color32 NormalTextTransparentColor = new Color32(0, 0, 0, 0);

        static Color32 ZeroTextBorderColor = new Color32(177, 177, 177, 255);
        static Color32 ZeroTextFillColor = new Color32(0, 0, 0, 0);
        static Color32 ZeroTextTransparentColor = new Color32(0, 0, 0, 0);

        public static class EnsoAssets
        {
            public static void CreateBottomAssets(GameObject parent)
            {
                DaniDojoAssetUtility.CreateImage("DaniBottomBg", Path.Combine(BaseImageFilePath, "Enso", "bottomBg.png"), new Vector2(0, 0), parent.transform);
                DaniDojoAssetUtility.CreateImage("DaniTopBg", Path.Combine(BaseImageFilePath, "Enso", "topBg.png"), new Vector2(0, 800), parent.transform);


                if (DaniDojoSelectManager.currentCourse != null)
                {
                    int numPanels = 0;
                    for (int j = 0; j < DaniDojoSelectManager.currentCourse.borders.Count && numPanels < 3; j++)
                    {
                        if (DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.SoulGauge)
                        {
                            CreatePanel("Panel" + j, new Vector2(117, 353 - (159 * numPanels)), parent.transform, DaniDojoSelectManager.currentCourse.borders[j]);
                            numPanels++;
                        }
                    }

                    numPanels = 0;
                    for (int j = 0; j < DaniDojoSelectManager.currentCourse.borders.Count && numPanels < 3; j++)
                    {
                        if (DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.SoulGauge)
                        {
                            UpdateRequirementBar(DaniDojoSelectManager.currentCourse.borders[j].borderType);
                            numPanels++;
                        }
                    }
                }
            }

            public static void ChangeCourseIcon(GameObject gameObject)
            {
                if (gameObject == null)
                {
                    gameObject = GameObject.Find("icon_course");
                }
                DaniDojoAssetUtility.ChangeSprite(gameObject, Path.Combine(BaseImageFilePath, "Course", "DifficultyIcons", "DaniDojo.png"));
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Dan-i dojo";
            }

            private static void CreatePanel(string name, Vector2 location, Transform parent, DaniData.DaniBorder border)
            {
                Plugin.Log.LogInfo("Create Panel");
                var newPanel = DaniDojoAssetUtility.CreateImage(name, Path.Combine(BaseImageFilePath, "Enso", "RequirementPanel.png"), location, parent);
                string requirementText = "Goods";

                Plugin.Log.LogInfo("Initialize Requirement Text");

                var fontManager = GameObject.Find("FontTMPManager").GetComponent<FontTMPManager>();
                TMP_FontAsset reqTypefont = fontManager.GetDefaultFontAsset(DataConst.FontType.EFIGS);
                Material reqTypeFontMaterial = fontManager.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);
                switch (border.borderType)
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

                var requirementTypeText = DaniDojoAssetUtility.CreateText("RequirementTypeText", requirementText, new Rect(24, 109, 334, 36), reqTypefont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), newPanel.transform);

                Plugin.Log.LogInfo("Create SongIndicators");

                string songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator1.png");//@"enso_dani\enso\dani_enso\dani_enso_detail\dani_enso_detail dani_enso_detail_cur_song_indicator_1.png";
                if (border.isTotalRequirements)
                {
                    songNumIndImagePath = Path.Combine("Enso", "CurSongIndicatorTotal.png");//@"enso_dani\enso\dani_enso\dani_enso_detail\dani_enso_detail dani_enso_detail_cur_song_indicator_total.png";
                }
                else
                {
                    if (DaniDojoTempEnso.result.currentSong == 1)
                    {
                        songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator2.png");
                    }
                    else if (DaniDojoTempEnso.result.currentSong == 2)
                    {
                        songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator3.png");
                    }
                }
                DaniDojoAssetUtility.CreateImage("SongNumIndicator", Path.Combine(BaseImageFilePath, songNumIndImagePath), new Vector2(20, 34), newPanel.transform);

                Plugin.Log.LogInfo("Create Requirement Value Text");

                var requirementValueString = string.Empty;
                if (border.borderType == BorderType.Oks || border.borderType == BorderType.Bads)
                {
                    if (border.isTotalRequirements)
                    {
                        requirementValueString = "Less than " + border.redBorders[0];
                    }
                    else
                    {
                        requirementValueString = "Less than " + border.redBorders[DaniDojoTempEnso.result.currentSong];
                    }
                }
                else
                {
                    if (border.isTotalRequirements)
                    {
                        requirementValueString = border.redBorders[0] + " or more";
                    }
                    else
                    {
                        requirementValueString = border.redBorders[DaniDojoTempEnso.result.currentSong] + " or more";
                    }
                }

                TMP_FontAsset reqValuefont = fontManager.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                Material reqValueFontMaterial = fontManager.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                var requirementValue = DaniDojoAssetUtility.CreateText("RequirementValue", requirementValueString, new Rect(28, 40, 334, 46), reqValuefont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), newPanel.transform);

                Plugin.Log.LogInfo("Create Requirement Bars");


                var curReqBarImagePath = Path.Combine("Enso", "Bars", "RequirementBarPerSong.png");
                var curReqBarBorderImagePath = Path.Combine("Enso", "Bars", "RequirementBarBorderPerSong.png");
                if (border.isTotalRequirements)
                {
                    curReqBarImagePath = Path.Combine("Enso", "Bars", "RequirementBarTotal.png");
                    curReqBarBorderImagePath = Path.Combine("Enso", "Bars", "RequirementBarBorderTotal.png");
                }
                Vector2 barPositions = new Vector2(389, 20);
                DaniDojoAssetUtility.CreateImage("CurReqBar", Path.Combine(BaseImageFilePath, curReqBarImagePath), barPositions, newPanel.transform);

                Rect fillBarRect;
                Rect emptyBarRect;
                if (border.isTotalRequirements)
                {
                    fillBarRect = new Rect(396, 36, 966, 80);
                    emptyBarRect = new Rect(396 + 966, 36, 966, 80);
                }
                else
                {
                    fillBarRect = new Rect(396, 36, 642, 80);
                    emptyBarRect = new Rect(396 + 642, 36, 642, 80);
                }
                var fillBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarFill", PinkBarColor, fillBarRect, newPanel.transform);
                fillBar.AddComponent<ColorLerp>();


                var coverBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarEmpty", GreyBarColor, emptyBarRect, newPanel.transform);
                DaniDojoAssetUtility.CreateImage("CurReqBarBorder", Path.Combine(BaseImageFilePath, curReqBarBorderImagePath), barPositions, newPanel.transform);

                Plugin.Log.LogInfo("Create Previous Song Requirement Bars");

                if (!border.isTotalRequirements)
                {
                    DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarTop", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 73), newPanel.transform);
                    DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarBot", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 23), newPanel.transform);
                    if (DaniDojoTempEnso.result.currentSong >= 1)
                    {
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneIndicator", Path.Combine(BaseImageFilePath, "Enso", "PrevSongIndicator1.png"), new Vector2(1085, 83), newPanel.transform);
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBar", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBar.png"), new Vector2(1128, 83), newPanel.transform);
                        DaniDojoAssetUtility.CreateNewImage("PrevSongHitReqOneBarFill", PinkBarColor, new Rect(1130, 90, 234, 33), newPanel.transform);
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBarBorder", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBarBorder.png"), new Vector2(1128, 83), newPanel.transform);
                        if (DaniDojoTempEnso.result.currentSong >= 2)
                        {
                            DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoIndicator", Path.Combine(BaseImageFilePath, "Enso", "PrevSongIndicator2.png"), new Vector2(1085, 33), newPanel.transform);
                            DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoBar", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBar.png"), new Vector2(1128, 33), newPanel.transform);
                            DaniDojoAssetUtility.CreateNewImage("PrevSongHitReqTwoBarFill", PinkBarColor, new Rect(1130, 40, 234, 33), newPanel.transform);
                            DaniDojoAssetUtility.CreateImage("PrevSongHitReqTwoBarBorder", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBarBorder.png"), new Vector2(1128, 33), newPanel.transform);
                        }
                    }
                }
            }

            public static void UpdateRequirementBar(BorderType borderType)
            {
                if (DaniDojoSelectManager.currentCourse != null)
                {
                    int numPanels = 0;
                    for (int j = 0; j < DaniDojoSelectManager.currentCourse.borders.Count && numPanels < 3; j++)
                    {
                        if (DaniDojoSelectManager.currentCourse.borders[j].borderType == borderType)
                        {
                            GameObject panel = GameObject.Find("Panel" + j);
                            if (panel != null)
                            {
                                //var bar = panel.transform.Find("CurReqBarFill");
                                var bar = panel.transform.Find("CurReqBarFill");
                                var emptyBar = panel.transform.Find("CurReqBarEmpty");
                                if (bar != null && emptyBar != null)
                                {
                                    var image = bar.GetComponent<Image>();
                                    var emptyImage = emptyBar.GetComponent<Image>();
                                    var colorLerp = bar.GetComponent<ColorLerp>();
                                    if (image != null && emptyImage != null)
                                    {
                                        int requirementValue;
                                        int currentValue;
                                        bool isGold = false;
                                        currentValue = DaniDojoTempEnso.result.GetCurrentResultValues(DaniDojoSelectManager.currentCourse.borders[j].borderType, DaniDojoSelectManager.currentCourse.borders[j].isTotalRequirements);
                                        if (DaniDojoSelectManager.currentCourse.borders[j].isTotalRequirements)
                                        {
                                            requirementValue = DaniDojoSelectManager.currentCourse.borders[j].redBorders[0];
                                            if (DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.Oks &&
                                            DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.Bads)
                                            {
                                                if (currentValue >= DaniDojoSelectManager.currentCourse.borders[j].goldBorders[0])
                                                {
                                                    isGold = true;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            requirementValue = DaniDojoSelectManager.currentCourse.borders[j].redBorders[DaniDojoTempEnso.result.currentSong];
                                            if (DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.Oks &&
                                                DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.Bads)
                                            {
                                                if (currentValue >= DaniDojoSelectManager.currentCourse.borders[j].goldBorders[DaniDojoTempEnso.result.currentSong])
                                                {
                                                    isGold = true;
                                                }
                                            }
                                        }

                                        if (isGold && colorLerp != null)
                                        {
                                            colorLerp.BeginRainbow(DaniDojoSelectManager.currentCourse.borders[j].isTotalRequirements);
                                        }

                                        var newScale = emptyImage.transform.localScale;
                                        if (DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Oks ||
                                            DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Bads)
                                        {
                                            currentValue = requirementValue - currentValue;
                                            currentValue = Math.Max(currentValue, 0);
                                        }

                                        ChangeReqCurrentValue(panel, currentValue, isGold);


                                        newScale.x = currentValue / (float)requirementValue;

                                        newScale.x = Math.Max(newScale.x, 0);
                                        newScale.x = Math.Min(newScale.x, 1);

                                        // Previously was resizing the filled area
                                        // Currently resizing the empty area
                                        // Hopefully this math is correct
                                        newScale.x -= 1;
                                        emptyImage.transform.localScale = newScale;
                                        if (!isGold)
                                        {
                                            if (newScale.x + 1 > 0.66)
                                            {
                                                image.color = PinkBarColor;
                                            }
                                            else if (newScale.x + 1 > 0.33)
                                            {
                                                image.color = YellowBarColor;
                                            }
                                            else
                                            {
                                                image.color = RedBarColor;
                                            }
                                        }
                                    }
                                }

                                if (DaniDojoTempEnso.result.currentSong > 0)
                                {
                                    var prevSongBar1 = panel.transform.Find("PrevSongHitReqOneBarFill");
                                    if (prevSongBar1 != null)
                                    {
                                        var image = prevSongBar1.GetComponent<Image>();
                                        if (image != null)
                                        {
                                            int requirementValue = DaniDojoSelectManager.currentCourse.borders[j].redBorders[0];
                                            int currentValue = DaniDojoTempEnso.result.GetCurrentResultValues(DaniDojoSelectManager.currentCourse.borders[j].borderType, DaniDojoSelectManager.currentCourse.borders[j].isTotalRequirements, 0);
                                            var newScale = image.transform.localScale;
                                            if (DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Oks ||
                                                DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Bads)
                                            {
                                                newScale.x = (requirementValue - currentValue) / (float)requirementValue;
                                            }
                                            else
                                            {
                                                newScale.x = currentValue / (float)requirementValue;
                                            }
                                            newScale.x = Math.Max(newScale.x, 0);
                                            newScale.x = Math.Min(newScale.x, 1);
                                            image.transform.localScale = newScale;
                                            if (newScale.x > 0.66)
                                            {
                                                image.color = PinkBarColor;
                                            }
                                            else if (newScale.x > 0.33)
                                            {
                                                image.color = YellowBarColor;
                                            }
                                            else
                                            {
                                                image.color = RedBarColor;
                                            }
                                        }
                                    }
                                    if (DaniDojoTempEnso.result.currentSong > 1)
                                    {
                                        var prevSongBar2 = panel.transform.Find("PrevSongHitReqTwoBarFill");
                                        if (prevSongBar2 != null)
                                        {
                                            var image = prevSongBar2.GetComponent<Image>();
                                            if (image != null)
                                            {
                                                int requirementValue = DaniDojoSelectManager.currentCourse.borders[j].redBorders[1];
                                                int currentValue = DaniDojoTempEnso.result.GetCurrentResultValues(DaniDojoSelectManager.currentCourse.borders[j].borderType, DaniDojoSelectManager.currentCourse.borders[j].isTotalRequirements, 1);
                                                var newScale = image.transform.localScale;
                                                if (DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Oks ||
                                                    DaniDojoSelectManager.currentCourse.borders[j].borderType == BorderType.Bads)
                                                {
                                                    newScale.x = (requirementValue - currentValue) / (float)requirementValue;
                                                }
                                                else
                                                {
                                                    newScale.x = currentValue / (float)requirementValue;
                                                }
                                                newScale.x = Math.Max(newScale.x, 0);
                                                newScale.x = Math.Min(newScale.x, 1);
                                                image.transform.localScale = newScale;
                                                if (newScale.x > 0.66)
                                                {
                                                    image.color = PinkBarColor;
                                                }
                                                else if (newScale.x > 0.33)
                                                {
                                                    image.color = YellowBarColor;
                                                }
                                                else
                                                {
                                                    image.color = RedBarColor;
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if (DaniDojoSelectManager.currentCourse.borders[j].borderType != BorderType.SoulGauge)
                        {
                            numPanels++;
                        }
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
                InitializeDigitSpriteLists();
                value = Mathf.Max(value, 0);
                value = Mathf.Min(value, 99999999); // 99,999,999
                string num = value.ToString();
                for (int i = 0; i < "99999999".Length; i++)
                {
                    var numLocation = new Vector2(403 + (56 * i), 44);
                    var baseNumberPath = Path.Combine(BaseImageFilePath, "Digits", "Big");

                    var digitBorderTransform = panel.transform.Find("CurReqBarValueBorder" + i);
                    var digitFillTransform = panel.transform.Find("CurReqBarValueFill" + i);
                    var digitTransparentTransform = panel.transform.Find("CurReqBarValueTransparent" + i);
                    if (digitBorderTransform == null)
                    {
                        digitBorderTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueBorder" + i, Path.Combine(baseNumberPath, "Border", "0.png"), numLocation, panel.transform).transform;
                    }
                    if (digitFillTransform == null)
                    {
                        digitFillTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueFill" + i, Path.Combine(baseNumberPath, "NoBorder", "0.png"), numLocation, panel.transform).transform;
                    }
                    if (digitTransparentTransform == null)
                    {
                        digitTransparentTransform = DaniDojoAssetUtility.CreateImage("CurReqBarValueTransparent" + i, Path.Combine(baseNumberPath, "Transparent", "0.png"), numLocation, panel.transform).transform;
                    }

                    GameObject digitBorder = digitBorderTransform.gameObject;
                    GameObject digitFill = digitFillTransform.gameObject;
                    GameObject digitTransparent = digitTransparentTransform.gameObject;

                    if (i < num.Length)
                    {
                        int numValue = int.Parse(num[i].ToString());
                        DaniDojoAssetUtility.ChangeImageSprite(digitBorder, DigitBigBorder[numValue]);
                        DaniDojoAssetUtility.ChangeImageSprite(digitFill, DigitBigFill[numValue]);
                        DaniDojoAssetUtility.ChangeImageSprite(digitTransparent, DigitBigTransparent[numValue]);

                        if (isGold)
                        {
                            digitBorder.GetComponent<Image>().color = GoldReqTextBorderColor;
                            digitFill.GetComponent<Image>().color = GoldReqTextFillColor;
                            digitTransparent.GetComponent<Image>().color = GoldReqTextTransparentColor;
                        }
                        else if (num == "0")
                        {
                            digitBorder.GetComponent<Image>().color = ZeroTextBorderColor;
                            digitFill.GetComponent<Image>().color = ZeroTextFillColor;
                            digitTransparent.GetComponent<Image>().color = ZeroTextTransparentColor;
                        }
                        else
                        {
                            digitBorder.GetComponent<Image>().color = NormalTextBorderColor;
                            digitFill.GetComponent<Image>().color = NormalTextFillColor;
                            digitTransparent.GetComponent<Image>().color = NormalTextTransparentColor;
                        }
                    }
                    else
                    {
                        digitBorder.GetComponent<Image>().color = InvisibleColor;
                        digitFill.GetComponent<Image>().color = InvisibleColor;
                        digitTransparent.GetComponent<Image>().color = InvisibleColor;
                    }

                }
            }

            static void InitializeDigitSpriteLists()
            {
                //Plugin.Log.LogInfo("Initialize DigitBigBorder");
                if (DigitBigBorder == null)
                {
                    DigitBigBorder = new List<Sprite>();
                }
                if (DigitBigBorder == null || DigitBigBorder.Count != 10)
                {
                    DigitBigBorder.Clear();
                    DigitBigBorder = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Big", "Border", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitBigBorder.Add(sprite);
                    }
                }
                //Plugin.Log.LogInfo("Initialize DigitBigFill");
                if (DigitBigFill == null)
                {
                    DigitBigFill = new List<Sprite>();
                }
                if (DigitBigFill == null || DigitBigFill.Count != 10)
                {
                    DigitBigFill.Clear();
                    DigitBigFill = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Big", "NoBorder", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitBigFill.Add(sprite);
                    }
                }
                //Plugin.Log.LogInfo("Initialize DigitBigTransparent");
                if (DigitBigTransparent == null)
                {
                    DigitBigTransparent = new List<Sprite>();
                }
                if (DigitBigTransparent == null || DigitBigTransparent.Count != 10)
                {
                    DigitBigTransparent.Clear();
                    DigitBigTransparent = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Big", "Transparent", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitBigTransparent.Add(sprite);
                    }
                }

                //Plugin.Log.LogInfo("Initialize DigitSmallBorder");
                if (DigitSmallBorder == null)
                {
                    DigitSmallBorder = new List<Sprite>();
                }
                if (DigitSmallBorder == null || DigitSmallBorder.Count != 10)
                {
                    DigitSmallBorder.Clear();
                    DigitSmallBorder = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Small", "Border", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitSmallBorder.Add(sprite);
                    }
                }
                //Plugin.Log.LogInfo("Initialize DigitSmallFill");
                if (DigitSmallFill == null)
                {
                    DigitSmallFill = new List<Sprite>();
                }
                if (DigitSmallFill == null || DigitSmallFill.Count != 10)
                {
                    DigitSmallFill.Clear();
                    DigitSmallFill = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Small", "NoBorder", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitSmallFill.Add(sprite);
                    }
                }
                //Plugin.Log.LogInfo("Initialize DigitSmallTransparent");
                if (DigitSmallTransparent == null)
                {
                    DigitSmallTransparent = new List<Sprite>();
                }
                if (DigitSmallTransparent == null || DigitSmallTransparent.Count != 10)
                {
                    DigitSmallTransparent.Clear();
                    DigitSmallTransparent = new List<Sprite>();
                    for (int i = 0; i < 10; i++)
                    {
                        Texture2D tex = new Texture2D(2, 2, TextureFormat.ARGB32, 1, false);
                        tex.LoadImage(File.ReadAllBytes(Path.Combine(BaseImageFilePath, "Digits", "Small", "Transparent", i.ToString() + ".png")));

                        Rect rect = new Rect(0, 0, tex.width, tex.height);
                        Sprite sprite = Sprite.Create(tex, rect, new Vector2(0, 0));

                        DigitSmallTransparent.Add(sprite);
                    }
                }
                //Plugin.Log.LogInfo("Initialize Digits Finish");
            }
        }

        public static class SelectAssets
        {
            // This will be the name of the image file
            enum SelectAssetName
            {
                CourseBorderLeftBlue,
                CourseBorderLeftGaiden,
                CourseBorderLeftGold,
                CourseBorderLeftKyuu,
                CourseBorderLeftRed,
                CourseBorderLeftSilver,
                CourseBorderRightBlue,
                CourseBorderRightGaiden,
                CourseBorderRightGold,
                CourseBorderRightKyuu,
                CourseBorderRightRed,
                CourseBorderRightSilver,
                CourseEasy,
                CourseHard,
                CourseNormal,
                CourseOni,
                CourseUra,
                Star1,
                Star2,
                Star3,
                Star4,
                Star5,
                Star6,
                Star7,
                Star8,
                Star9,
                Star10,
                Star10Plus,
                BigGoldClear,
                BigGoldDFC,
                BigGoldFC,
                BigRedClear,
                BigRedDFC,
                BigRedFC,
                SmallGoldClear,
                SmallGoldDFC,
                SmallGoldFC,
                SmallRedClear,
                SmallRedDFC,
                SmallRedFC,
                Top1dan,
                Top1kyuu,
                Top2dan,
                Top2kyuu,
                Top3dan,
                Top3kyuu,
                Top4dan,
                Top4kyuu,
                Top5dan,
                Top5kyuu,
                Top6dan,
                Top7dan,
                Top8dan,
                Top9dan,
                Top10dan,
                TopBlueBg,
                TopChojin,
                TopGaiden,
                TopGoldBg,
                TopKuroto,
                TopKyuuBg,
                TopMeijin,
                TopOriginalBg,
                TopRedBg,
                TopSelectedDan,
                TopSilverBg,
                TopTatsujin,
                ArrowLeft,
                ArrowRight,
                BgLightsOff,
                BgLightsOn,
                CourseBg,
                PersonalBestBg,
                ReqSongIndicator1,
                ReqSongIndicator2,
                ReqSongIndicator3,
                ReqSongIndicatorTotal,
                RequirementsEachPanel,
                RequirementsMainPanel,
                RequirementsPanelBorder,
                SongIndicator1,
                SongIndicator2,
                SongIndicator3,
                SongTitleBg,
                SoulGaugeReqPanel,
                BlueBg,
                chojin,
                dan1,
                dan2,
                dan3,
                dan4,
                dan5,
                dan6,
                dan7,
                dan8,
                dan9,
                dan10,
                gaiden,
                GoldBg,
                kuroto,
                kyuu1,
                kyuu2,
                kyuu3,
                kyuu4,
                kyuu5,
                meijin,
                original,
                RedBg,
                SilverBg,
                TanBg,
                tatsujin,
                WoodBg,
            }

            static Dictionary<SelectAssetName, Sprite> SelectAssetSprites;

            public static void InitializeSceneAssets(GameObject parent)
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

                DaniDojoAssetUtility.CreateImage("BgLightsOn", Path.Combine(BaseImageFilePath, "Select", "BgLightsOn.png"), new Vector2(0, 0), parent.transform);

                InitializeSelectAssetSprites();
            }

            private static void InitializeSelectAssetSprites()
            {
                if (SelectAssetSprites == null)
                {
                    SelectAssetSprites = new Dictionary<SelectAssetName, Sprite>();
                }
                foreach (SelectAssetName asset in Enum.GetValues(typeof(SelectAssetName)))
                {
                    InitializeSelectAssetSprite(asset);
                }
            }

            private static void InitializeSelectAssetSprite(SelectAssetName asset)
            {
                if (!SelectAssetSprites.ContainsKey(asset))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(BaseImageFilePath));
                    var assetFile = dirInfo.GetFiles(asset.ToString() + ".png", SearchOption.AllDirectories);
                    if (assetFile.Length >= 0)
                    {
                        if (assetFile.Length > 1)
                        {
                            Plugin.Log.LogInfo("Multiple files for " + asset.ToString() + " found, loading the first one found!");

                        }
                        var sprite = DaniDojoAssetUtility.CreateSprite(assetFile[0].FullName);
                        SelectAssetSprites.Add(asset, sprite);
                    }
                    else
                    {
                        Plugin.Log.LogInfo(asset.ToString() + ".png not found!");
                    }
                }
            }

            private static Sprite GetAssetSprite(SelectAssetName asset)
            {
                InitializeSelectAssetSprite(asset);
                return SelectAssetSprites[asset];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="seriesInfo"></param>
            /// <param name="currentCourse"></param>
            /// <param name="parent"></param>
            /// <returns>The current Course GameObject.</returns>
            public static void CreateSeriesAssets(DaniSeriesData seriesInfo, GameObject parent)
            {
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

                GameObject seriesTitleObject = GameObject.Find("SeriesTitle");
                if (seriesTitleObject == null)
                {
                    seriesTitleObject = DaniDojoAssetUtility.CreateText("SeriesTitle", seriesInfo.seriesTitle, new Rect(1920 - 592, 777, 500, 100),
                                          fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.Japanese), fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.Japanese, DataConst.DefaultFontMaterialType.OutlineBlack),
                                          HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 255), parent.transform);
                    var seriesTitleText = seriesTitleObject.GetComponent<TextMeshProUGUI>();
                    seriesTitleText.enableAutoSizing = false;
                    seriesTitleText.fontSize = 50;
                }
                else
                {
                    var seriesTitleText = seriesTitleObject.GetComponent<TextMeshProUGUI>();
                    seriesTitleText.text = seriesInfo.seriesTitle;
                }


                if (GameObject.Find("TopCourses"))
                {
                    GameObject.Destroy(GameObject.Find("TopCourses"));
                }

                var topCoursesParent = new GameObject("TopCourses");
                topCoursesParent.transform.SetParent(parent.transform);
                for (int i = 0; i < seriesInfo.courseData.Count; i++)
                {
                    var highScore = Plugin.AllDaniScores.Find((x) => x.hash == seriesInfo.courseData[i].hash);

                    SelectAssetName backgroundName;
                    SelectAssetName textName;
                    switch (seriesInfo.courseData[i].course)
                    {
                        case DaniCourse.None:
                            backgroundName = SelectAssetName.TopOriginalBg;
                            textName = SelectAssetName.TopGaiden;
                            break;
                        case DaniCourse.kyuu5:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            textName = SelectAssetName.Top5kyuu;
                            break;
                        case DaniCourse.kyuu4:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            textName = SelectAssetName.Top4kyuu;
                            break;
                        case DaniCourse.kyuu3:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            textName = SelectAssetName.Top3kyuu;
                            break;
                        case DaniCourse.kyuu2:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            textName = SelectAssetName.Top2kyuu;
                            break;
                        case DaniCourse.kyuu1:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            textName = SelectAssetName.Top1kyuu;
                            break;
                        case DaniCourse.dan1:
                            backgroundName = SelectAssetName.TopBlueBg;
                            textName = SelectAssetName.Top1dan;
                            break;
                        case DaniCourse.dan2:
                            backgroundName = SelectAssetName.TopBlueBg;
                            textName = SelectAssetName.Top2dan;
                            break;
                        case DaniCourse.dan3:
                            backgroundName = SelectAssetName.TopBlueBg;
                            textName = SelectAssetName.Top3dan;
                            break;
                        case DaniCourse.dan4:
                            backgroundName = SelectAssetName.TopBlueBg;
                            textName = SelectAssetName.Top4dan;
                            break;
                        case DaniCourse.dan5:
                            backgroundName = SelectAssetName.TopBlueBg;
                            textName = SelectAssetName.Top5dan;
                            break;
                        case DaniCourse.dan6:
                            backgroundName = SelectAssetName.TopRedBg;
                            textName = SelectAssetName.Top6dan;
                            break;
                        case DaniCourse.dan7:
                            backgroundName = SelectAssetName.TopRedBg;
                            textName = SelectAssetName.Top7dan;
                            break;
                        case DaniCourse.dan8:
                            backgroundName = SelectAssetName.TopRedBg;
                            textName = SelectAssetName.Top8dan;
                            break;
                        case DaniCourse.dan9:
                            backgroundName = SelectAssetName.TopRedBg;
                            textName = SelectAssetName.Top9dan;
                            break;
                        case DaniCourse.dan10:
                            backgroundName = SelectAssetName.TopRedBg;
                            textName = SelectAssetName.Top10dan;
                            break;
                        case DaniCourse.kuroto:
                            backgroundName = SelectAssetName.TopSilverBg;
                            textName = SelectAssetName.TopKuroto;
                            break;
                        case DaniCourse.meijin:
                            backgroundName = SelectAssetName.TopSilverBg;
                            textName = SelectAssetName.TopMeijin;
                            break;
                        case DaniCourse.chojin:
                            backgroundName = SelectAssetName.TopSilverBg;
                            textName = SelectAssetName.TopChojin;
                            break;
                        case DaniCourse.tatsujin:
                            backgroundName = SelectAssetName.TopGoldBg;
                            textName = SelectAssetName.TopTatsujin;
                            break;
                        default:
                            backgroundName = SelectAssetName.TopOriginalBg;
                            textName = SelectAssetName.TopGaiden;
                            break;
                    }
                    GameObject topCourseObject = new GameObject(seriesInfo.courseData[i].title);
                    topCourseObject.transform.SetParent(topCoursesParent.transform);
                    topCourseObject.transform.position = new Vector2(183 + (68 * i), 884);
                    DaniDojoAssetUtility.CreateImage(seriesInfo.courseData[i].title + "Background", GetAssetSprite(backgroundName), new Vector2(0, 0), topCourseObject.transform);
                    DaniDojoAssetUtility.CreateImage(seriesInfo.courseData[i].title + "Text", GetAssetSprite(textName), new Vector2(19, 24), topCourseObject.transform);

                    if (highScore != null)
                    {
                        bool skip = false;
                        SelectAssetName recordName;
                        if (highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.DFC)
                        {
                            recordName = SelectAssetName.SmallGoldDFC;
                        }
                        else if (highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.FC)
                        {
                            recordName = SelectAssetName.SmallGoldFC;
                        }
                        else if (highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.Clear)
                        {
                            recordName = SelectAssetName.SmallGoldClear;
                        }
                        else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.DFC)
                        {
                            recordName = SelectAssetName.SmallRedDFC;
                        }
                        else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.FC)
                        {
                            recordName = SelectAssetName.SmallRedFC;
                        }
                        else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.Clear)
                        {
                            recordName = SelectAssetName.SmallRedClear;
                        }
                        else
                        {
                            recordName = SelectAssetName.CourseNormal; // This line is just here so it won't yell at me for being uninitialized;
                            skip = true;
                        }
                        if (!skip)
                        {
                            DaniDojoAssetUtility.CreateImage(seriesInfo.courseData[i].title + "Record", GetAssetSprite(recordName), new Vector2(0, 98), topCourseObject.transform);
                        }
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
            public static GameObject CreateCourseAssets(DaniData courseInfo, GameObject parent, CourseCreateDir dir = CourseCreateDir.Center)
            {
                //if (GameObject.Find("CourseBg"))
                //{
                //    GameObject.Destroy(GameObject.Find("CourseBg"));
                //}

                var highScore = Plugin.AllDaniScores.Find((x) => x.hash == courseInfo.hash);

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

                var courseObject = DaniDojoAssetUtility.CreateImage("CourseBg" + courseBgNum++, GetAssetSprite(SelectAssetName.CourseBg), CourseBgLocation, parent.transform);

                SelectAssetName leftBorderName;
                SelectAssetName rightBorderName;
                switch (courseInfo.course)
                {
                    case DaniCourse.None:
                        leftBorderName = SelectAssetName.CourseBorderLeftGaiden;
                        rightBorderName = SelectAssetName.CourseBorderRightGaiden;
                        break;
                    case DaniCourse.kyuu5:
                    case DaniCourse.kyuu4:
                    case DaniCourse.kyuu3:
                    case DaniCourse.kyuu2:
                    case DaniCourse.kyuu1:
                        leftBorderName = SelectAssetName.CourseBorderLeftKyuu;
                        rightBorderName = SelectAssetName.CourseBorderRightKyuu;
                        break;
                    case DaniCourse.dan1:
                    case DaniCourse.dan2:
                    case DaniCourse.dan3:
                    case DaniCourse.dan4:
                    case DaniCourse.dan5:
                        leftBorderName = SelectAssetName.CourseBorderLeftBlue;
                        rightBorderName = SelectAssetName.CourseBorderRightBlue;
                        break;
                    case DaniCourse.dan6:
                    case DaniCourse.dan7:
                    case DaniCourse.dan8:
                    case DaniCourse.dan9:
                    case DaniCourse.dan10:
                        leftBorderName = SelectAssetName.CourseBorderLeftRed;
                        rightBorderName = SelectAssetName.CourseBorderRightRed;
                        break;
                    case DaniCourse.kuroto:
                    case DaniCourse.meijin:
                    case DaniCourse.chojin:
                        leftBorderName = SelectAssetName.CourseBorderLeftSilver;
                        rightBorderName = SelectAssetName.CourseBorderRightSilver;
                        break;
                    case DaniCourse.tatsujin:
                        leftBorderName = SelectAssetName.CourseBorderLeftGold;
                        rightBorderName = SelectAssetName.CourseBorderRightGold;
                        break;
                    default:
                        leftBorderName = SelectAssetName.CourseBorderLeftGaiden;
                        rightBorderName = SelectAssetName.CourseBorderRightGaiden;
                        break;
                }
                DaniDojoAssetUtility.CreateImage("LeftBorder", GetAssetSprite(leftBorderName), new Vector2(22, 33), courseObject.transform);
                DaniDojoAssetUtility.CreateImage("RightBorder", GetAssetSprite(rightBorderName), new Vector2(1458, 33), courseObject.transform);


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

                for (int i = 0; i < Math.Min(courseInfo.songs.Count, 3); i++)
                {
                    GameObject songParent = DaniDojoAssetUtility.CreateImage("SongBg" + (i + 1), GetAssetSprite(SelectAssetName.SongTitleBg), new Vector2(78, 718 - (110 * i)), courseObject.transform);
                    SelectAssetName songIndicator;
                    if (i == 0)
                    {
                        songIndicator = SelectAssetName.SongIndicator1;
                    }
                    else if (i == 1)
                    {
                        songIndicator = SelectAssetName.SongIndicator2;
                    }
                    else
                    {
                        songIndicator = SelectAssetName.SongIndicator3;
                    }
                    DaniDojoAssetUtility.CreateImage("SongIndicator", GetAssetSprite(songIndicator), new Vector2(10, 10), songParent.transform);

                    var song = musicInfoAccessers.Find((x) => x.Id == courseInfo.songs[i].songId);
                    string songTitle = "Song not found: " + courseInfo.songs[i].songId;
                    string songDetail = "";
                    if (song != null)
                    {
                        songTitle = wordDataMgr.GetWordListInfo("song_" + song.Id).Text;
                        songDetail = wordDataMgr.GetWordListInfo("song_detail_" + song.Id).Text;
                    }

                    if (courseInfo.songs[i].isHidden)
                    {
                        if (highScore != null)
                        {
                            if (highScore.songReached < i)
                            {
                                songTitle = "? ? ?";
                                songDetail = "";
                            }
                        }
                        else
                        {
                            songTitle = "? ? ?";
                            songDetail = "";
                        }
                    }


                    SelectAssetName levelAssetName;
                    switch (courseInfo.songs[i].level)
                    {
                        case EnsoData.EnsoLevelType.Easy:   levelAssetName = SelectAssetName.CourseEasy;   break;
                        case EnsoData.EnsoLevelType.Normal: levelAssetName = SelectAssetName.CourseNormal; break;
                        case EnsoData.EnsoLevelType.Hard:   levelAssetName = SelectAssetName.CourseHard;   break;
                        case EnsoData.EnsoLevelType.Mania:  levelAssetName = SelectAssetName.CourseOni;    break;
                        case EnsoData.EnsoLevelType.Ura:    levelAssetName = SelectAssetName.CourseUra;    break;
                        default:                            levelAssetName = SelectAssetName.CourseOni;    break;
                    }
                    DaniDojoAssetUtility.CreateImage("SongCourse", GetAssetSprite(levelAssetName), new Vector2(112, 42), songParent.transform);

                    SelectAssetName starAssetName;

                    if (song != null)
                    {
                        switch (song.Stars[(int)courseInfo.songs[i].level])
                        {
                            case 1: starAssetName = SelectAssetName.Star1; break;
                            case 2: starAssetName = SelectAssetName.Star2; break;
                            case 3: starAssetName = SelectAssetName.Star3; break;
                            case 4: starAssetName = SelectAssetName.Star4; break;
                            case 5: starAssetName = SelectAssetName.Star5; break;
                            case 6: starAssetName = SelectAssetName.Star6; break;
                            case 7: starAssetName = SelectAssetName.Star7; break;
                            case 8: starAssetName = SelectAssetName.Star8; break;
                            case 9: starAssetName = SelectAssetName.Star9; break;
                            case 10: starAssetName = SelectAssetName.Star10; break;
                            default: starAssetName = SelectAssetName.Star10Plus; break; // Might as well have this as a default, it's kinda fun
                        }
                    }
                    else
                    {
                        starAssetName = SelectAssetName.Star10Plus;
                    }

                    DaniDojoAssetUtility.CreateImage("SongLevel", GetAssetSprite(starAssetName), new Vector2(121, 15), songParent.transform);

                    Rect titleRect = new Rect(220, 28, 1920, 40);
                    Rect detailRect = new Rect(220, 70, 1920, 20);




                    DaniDojoAssetUtility.CreateText("SongTitle", songTitle, titleRect, titleFont, titleFontMaterial, HorizontalAlignmentOptions.Left, Color.black, songParent.transform);
                    var detail = DaniDojoAssetUtility.CreateText("SongDetail", songDetail, detailRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, Color.black, songParent.transform);
                    detail.GetComponent<TextMeshProUGUI>().color = Color.black;

                }


                // Add Requirements info
                DaniDojoAssetUtility.CreateImage("RequirementsMainPanel", GetAssetSprite(SelectAssetName.RequirementsMainPanel), new Vector2(78, 417), courseObject.transform);
                DaniDojoAssetUtility.CreateImage("RequirementsPanelLeftBorder", GetAssetSprite(SelectAssetName.RequirementsPanelBorder), new Vector2(78, 33), courseObject.transform);
                DaniDojoAssetUtility.CreateImage("RequirementsPanelRightBorder", GetAssetSprite(SelectAssetName.RequirementsPanelBorder), new Vector2(1440, 33), courseObject.transform);

                // Add Soul Gauge Requirements
                var SoulGaugeReqPanel = DaniDojoAssetUtility.CreateImage("SoulGaugeReqPanel", GetAssetSprite(SelectAssetName.SoulGaugeReqPanel), new Vector2(94, 268), courseObject.transform);


                TMP_FontAsset reqTypeFont = fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.EFIGS);
                Material reqTypeFontMaterial = fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.EFIGS, DataConst.DefaultFontMaterialType.OutlineBrown03);

                TMP_FontAsset reqValueFont = fontTMPMgr.GetDescriptionFontAsset(DataConst.FontType.EFIGS);
                Material reqValueFontMaterial = fontTMPMgr.GetDescriptionFontMaterial(DataConst.FontType.EFIGS, DataConst.DescriptionFontMaterialType.OutlineSongInfo);

                Rect SoulGaugeHeaderRect = new Rect(40, 139, 240, 25);
                DaniDojoAssetUtility.CreateText("SoulGaugeHeader", "Soul Gauge", SoulGaugeHeaderRect, reqTypeFont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), SoulGaugeReqPanel.transform);

                int soulGaugeRequirement = 100;
                for (int i = 0; i < courseInfo.borders.Count; i++)
                {
                    if (courseInfo.borders[i].borderType == BorderType.SoulGauge)
                    {
                        soulGaugeRequirement = courseInfo.borders[i].redBorders[0];
                        break;
                    }
                }

                Rect SoulGaugeReqValueRect = new Rect(50, 73, 256, 39);
                DaniDojoAssetUtility.CreateText("SoulGaugeReqValue", soulGaugeRequirement.ToString() + "% or higher", SoulGaugeReqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), SoulGaugeReqPanel.transform);
                
                if (highScore != null)
                {
                    Rect SoulGaugeHighScoreHeaderRect = new Rect(12, 27, 100, 25);
                    var SoulGaugeHighScoreHeader = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreHeader", "High Score", SoulGaugeHighScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    SoulGaugeHighScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                    Rect SoulGaugeHighScoreValueRect = new Rect(200, 20, 100, 40);
                    var SoulGaugeHighScoreValue = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreValue", highScore.soulGauge.ToString() + " %", SoulGaugeHighScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    SoulGaugeHighScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                }

                // Add other Requirements
                bool passedSoulGauge = false;
                for (int i = 0; i < courseInfo.borders.Count; i++)
                {
                    if (courseInfo.borders[i].borderType == BorderType.SoulGauge)
                    {
                        passedSoulGauge = true;
                        continue;
                    }
                    var reqPanel = DaniDojoAssetUtility.CreateImage("ReqPanel" + (i + 1), GetAssetSprite(SelectAssetName.RequirementsEachPanel), new Vector2(427, 310 - (passedSoulGauge ? (i - 1) * 132 : i * 132)), courseObject.transform);

                    Rect reqPanelHeaderRect = new Rect(34, 95, 240, 25);
                    string requirementTypeText = string.Empty;
                    switch (courseInfo.borders[i].borderType)
                    {
                        case BorderType.Goods:     requirementTypeText = "GOODs"; break;
                        case BorderType.Oks:       requirementTypeText = "OKs"; break;
                        case BorderType.Bads:      requirementTypeText = "BADs"; break;
                        case BorderType.Combo:     requirementTypeText = "Combo"; break;
                        case BorderType.Drumroll:  requirementTypeText = "Drumrolls"; break;
                        case BorderType.Score:     requirementTypeText = "Score"; break;
                        case BorderType.TotalHits: requirementTypeText = "Total Hits"; break;
                    }
                    DaniDojoAssetUtility.CreateText("SoulGaugeHeader", requirementTypeText, reqPanelHeaderRect, reqTypeFont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), reqPanel.transform);

                    if (courseInfo.borders[i].isTotalRequirements)
                    {
                        var reqPanelValue = DaniDojoAssetUtility.CreateImage("ReqPanelValue" + (i + 1), GetAssetSprite(SelectAssetName.ReqSongIndicatorTotal), new Vector2(14, 49), reqPanel.transform);

                        string valueText = string.Empty;
                        if (courseInfo.borders[i].borderType == BorderType.Oks || courseInfo.borders[i].borderType == BorderType.Bads)
                        {
                            valueText = "Less than " + courseInfo.borders[i].redBorders[0];
                        }
                        else
                        {
                            valueText = courseInfo.borders[i].redBorders[0] + " or more";
                        }

                        Rect reqValueRect = new Rect(50, 0, 256, 31);
                        DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);

                        var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (i + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14, 11), reqPanel.transform);
                
                        if (highScore != null)
                        {
                            Rect highScoreHeaderRect = new Rect(12, 3, 100, 25);
                            var highScoreHeader = DaniDojoAssetUtility.CreateText("HighScoreHeader", "High Score", highScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                            highScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                            string highScoreValueText = string.Empty;
                            switch (courseInfo.borders[i].borderType)
                            {
                                case BorderType.Goods:     highScoreValueText = highScore.songResults.Sum((x) => x.goods).ToString(); break;
                                case BorderType.Oks:       highScoreValueText = highScore.songResults.Sum((x) => x.oks).ToString(); break;
                                case BorderType.Bads:      highScoreValueText = highScore.songResults.Sum((x) => x.bads).ToString(); break;
                                case BorderType.Combo:     highScoreValueText = highScore.combo.ToString(); break;
                                case BorderType.Drumroll:  highScoreValueText = highScore.songResults.Sum((x) => x.renda).ToString(); break;
                                case BorderType.Score:     highScoreValueText = highScore.songResults.Sum((x) => x.score).ToString(); break;
                                case BorderType.TotalHits: highScoreValueText = highScore.songResults.Sum((x) => x.totalHits).ToString(); break;
                            }

                            Rect highScoreValueRect = new Rect(200, -3, 100, 35);
                            var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                            highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Math.Min(courseInfo.songs.Count, 3); j++)
                        {
                            int xOffset = (326 * j);
                            SelectAssetName SongIndicatorAsset = SelectAssetName.ReqSongIndicator1;
                            if (j == 1)
                            {
                                SongIndicatorAsset = SelectAssetName.ReqSongIndicator2;
                            }
                            else if (j == 2)
                            {
                                SongIndicatorAsset = SelectAssetName.ReqSongIndicator3;
                            }
                            var reqPanelValue = DaniDojoAssetUtility.CreateImage("ReqPanelValue" + (j + 1), GetAssetSprite(SongIndicatorAsset), new Vector2(14 + xOffset, 49), reqPanel.transform);

                            string valueText = string.Empty;
                            if (courseInfo.borders[i].borderType == BorderType.Oks || courseInfo.borders[i].borderType == BorderType.Bads)
                            {
                                valueText = "Less than " + courseInfo.borders[i].redBorders[j];
                            }
                            else
                            {
                                valueText = courseInfo.borders[i].redBorders[j] + " or more";
                            }

                            Rect reqValueRect = new Rect(50, 0, 256, 31);
                            DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);

                            var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (j + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14 + xOffset, 11), reqPanel.transform);

                            if (highScore != null)
                            {
                                Rect highScoreHeaderRect = new Rect(17, 3, 100, 25);
                                var highScoreHeader = DaniDojoAssetUtility.CreateText("HighScoreHeader", "High Score", highScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                                highScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                                string highScoreValueText = string.Empty;
                                switch (courseInfo.borders[i].borderType)
                                {
                                    case BorderType.Goods: highScoreValueText = highScore.songResults[j].goods.ToString(); break;
                                    case BorderType.Oks: highScoreValueText = highScore.songResults[j].oks.ToString(); break;
                                    case BorderType.Bads: highScoreValueText = highScore.songResults[j].bads.ToString(); break;
                                    case BorderType.Combo: highScoreValueText = highScore.songResults[j].songCombo.ToString(); break;
                                    case BorderType.Drumroll: highScoreValueText = highScore.songResults[j].renda.ToString(); break;
                                    case BorderType.Score: highScoreValueText = highScore.songResults[j].score.ToString(); break;
                                    case BorderType.TotalHits: highScoreValueText = highScore.songResults[j].totalHits.ToString(); break;
                                }

                                Rect highScoreValueRect = new Rect(200, -3, 100, 35);
                                var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                                highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                            }
                        }
                    }

                }

                SelectAssetName bgImage;
                SelectAssetName textImage;
                switch (courseInfo.course)
                {
                    case DaniCourse.kyuu5:
                        bgImage = SelectAssetName.WoodBg;
                        textImage = SelectAssetName.kyuu5;
                        break;
                    case DaniCourse.kyuu4:
                        bgImage = SelectAssetName.WoodBg;
                        textImage = SelectAssetName.kyuu4;
                        break;
                    case DaniCourse.kyuu3:
                        bgImage = SelectAssetName.WoodBg;
                        textImage = SelectAssetName.kyuu3;
                        break;
                    case DaniCourse.kyuu2:
                        bgImage = SelectAssetName.WoodBg;
                        textImage = SelectAssetName.kyuu2;
                        break;
                    case DaniCourse.kyuu1:
                        bgImage = SelectAssetName.WoodBg;
                        textImage = SelectAssetName.kyuu1;
                        break;
                    case DaniCourse.dan1:
                        bgImage = SelectAssetName.BlueBg;
                        textImage = SelectAssetName.dan1;
                        break;
                    case DaniCourse.dan2:
                        bgImage = SelectAssetName.BlueBg;
                        textImage = SelectAssetName.dan2;
                        break;
                    case DaniCourse.dan3:
                        bgImage = SelectAssetName.BlueBg;
                        textImage = SelectAssetName.dan3;
                        break;
                    case DaniCourse.dan4:
                        bgImage = SelectAssetName.BlueBg;
                        textImage = SelectAssetName.dan4;
                        break;
                    case DaniCourse.dan5:
                        bgImage = SelectAssetName.BlueBg;
                        textImage = SelectAssetName.dan5;
                        break;
                    case DaniCourse.dan6:
                        bgImage = SelectAssetName.RedBg;
                        textImage = SelectAssetName.dan6;
                        break;
                    case DaniCourse.dan7:
                        bgImage = SelectAssetName.RedBg;
                        textImage = SelectAssetName.dan7;
                        break;
                    case DaniCourse.dan8:
                        bgImage = SelectAssetName.RedBg;
                        textImage = SelectAssetName.dan8;
                        break;
                    case DaniCourse.dan9:
                        bgImage = SelectAssetName.RedBg;
                        textImage = SelectAssetName.dan9;
                        break;
                    case DaniCourse.dan10:
                        bgImage = SelectAssetName.RedBg;
                        textImage = SelectAssetName.dan10;
                        break;
                    case DaniCourse.kuroto:
                        bgImage = SelectAssetName.SilverBg;
                        textImage = SelectAssetName.kuroto;
                        break;
                    case DaniCourse.meijin:
                        bgImage = SelectAssetName.SilverBg;
                        textImage = SelectAssetName.meijin;
                        break;
                    case DaniCourse.chojin:
                        bgImage = SelectAssetName.SilverBg;
                        textImage = SelectAssetName.chojin;
                        break;
                    case DaniCourse.tatsujin:
                        bgImage = SelectAssetName.GoldBg;
                        textImage = SelectAssetName.tatsujin;
                        break;
                    default:
                        bgImage = SelectAssetName.TanBg;
                        textImage = SelectAssetName.gaiden;
                        break;
                }

                var baseCourseTagPos = new Vector2(-212, 332);
                DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", GetAssetSprite(bgImage), baseCourseTagPos, courseObject.transform);
                DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", GetAssetSprite(textImage), new Vector2(baseCourseTagPos.x + 52, baseCourseTagPos.y + 124), courseObject.transform);


                if (highScore != null)
                {
                    bool skip = false;
                    SelectAssetName resultAsset;
                    if (highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.DFC)
                    {
                        resultAsset = SelectAssetName.BigGoldDFC;
                    }
                    else if (highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.FC)
                    {
                        resultAsset = SelectAssetName.BigGoldFC;
                    }
                    else if(highScore.danResult == DanResult.GoldClear && highScore.comboResult == DanComboResult.Clear)
                    {
                        resultAsset = SelectAssetName.BigGoldClear;
                    }
                    else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.DFC)
                    {
                        resultAsset = SelectAssetName.BigRedDFC;
                    }
                    else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.FC)
                    {
                        resultAsset = SelectAssetName.BigRedFC;
                    }
                    else if (highScore.danResult == DanResult.RedClear && highScore.comboResult == DanComboResult.Clear)
                    {
                        resultAsset = SelectAssetName.BigRedClear;
                    }
                    else
                    {
                        resultAsset = SelectAssetName.CourseNormal; // This line is just here so it won't yell at me for being uninitialized;
                        skip = true;
                    }

                    if (!skip)
                    {
                        DaniDojoAssetUtility.CreateImage("DanResult", GetAssetSprite(resultAsset), new Vector2(-242, 291), courseObject.transform);
                    }
                }


                return courseObject;
            }
        }

        public static class ResultAssets
        {

        }
    }
}
