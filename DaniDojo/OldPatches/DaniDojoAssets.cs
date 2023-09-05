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
                        CreatePanel("Panel" + j, new Vector2(117, 353 - (159 * numPanels)), parent.transform, borders[j]);
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
                DaniDojoAssetUtility.ChangeSprite(gameObject, Path.Combine(BaseImageFilePath, "Course", "DifficultyIcons", "DaniDojo.png"));
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Dan-i dojo";
            }

            private static void CreatePanel(string name, Vector2 location, Transform parent, DaniBorder border)
            {
                Plugin.Log.LogInfo("Create Panel");
                var newPanel = DaniDojoAssetUtility.CreateImage(name, Path.Combine(BaseImageFilePath, "Enso", "RequirementPanel.png"), location, parent);


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

                var requirementTypeText = DaniDojoAssetUtility.CreateText("RequirementTypeText", requirementText, new Rect(24, 109, 334, 36), reqTypefont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), newPanel.transform);

                Plugin.Log.LogInfo("Create SongIndicators");

                string songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator1.png");
                if (border.IsTotal)
                {
                    songNumIndImagePath = Path.Combine("Enso", "CurSongIndicatorTotal.png");
                }
                else
                {
                    if (DaniPlayManager.GetCurrentSongNumber() == 1)
                    {
                        songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator2.png");
                    }
                    else if (DaniPlayManager.GetCurrentSongNumber() == 2)
                    {
                        songNumIndImagePath = Path.Combine("Enso", "CurSongIndicator3.png");
                    }
                }
                DaniDojoAssetUtility.CreateImage("SongNumIndicator", Path.Combine(BaseImageFilePath, songNumIndImagePath), new Vector2(20, 34), newPanel.transform);

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

                var requirementValue = DaniDojoAssetUtility.CreateText("RequirementValue", requirementValueString, new Rect(28, 40, 334, 46), reqValuefont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), newPanel.transform);

                Plugin.Log.LogInfo("Create Requirement Bars");


                var curReqBarImagePath = Path.Combine("Enso", "Bars", "RequirementBarPerSong.png");
                var curReqBarBorderImagePath = Path.Combine("Enso", "Bars", "RequirementBarBorderPerSong.png");
                if (border.IsTotal)
                {
                    curReqBarImagePath = Path.Combine("Enso", "Bars", "RequirementBarTotal.png");
                    curReqBarBorderImagePath = Path.Combine("Enso", "Bars", "RequirementBarBorderTotal.png");
                }
                Vector2 barPositions = new Vector2(389, 20);
                DaniDojoAssetUtility.CreateImage("CurReqBar", Path.Combine(BaseImageFilePath, curReqBarImagePath), barPositions, newPanel.transform);

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
                var fillBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarFill", PinkBarColor, fillBarRect, newPanel.transform);
                fillBar.AddComponent<ColorLerp>();


                var coverBar = DaniDojoAssetUtility.CreateNewImage("CurReqBarEmpty", GreyBarColor, emptyBarRect, newPanel.transform);
                DaniDojoAssetUtility.CreateImage("CurReqBarBorder", Path.Combine(BaseImageFilePath, curReqBarBorderImagePath), barPositions, newPanel.transform);

                Plugin.Log.LogInfo("Create Previous Song Requirement Bars");

                if (!border.IsTotal)
                {
                    DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarTop", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 73), newPanel.transform);
                    DaniDojoAssetUtility.CreateImage("PrevSongHitReqsBarBot", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongHitReqs.png"), new Vector2(1083, 23), newPanel.transform);
                    if (DaniPlayManager.GetCurrentSongNumber() >= 1)
                    {
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneIndicator", Path.Combine(BaseImageFilePath, "Enso", "PrevSongIndicator1.png"), new Vector2(1085, 83), newPanel.transform);
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBar", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBar.png"), new Vector2(1128, 83), newPanel.transform);
                        DaniDojoAssetUtility.CreateNewImage("PrevSongHitReqOneBarFill", PinkBarColor, new Rect(1130, 90, 234, 33), newPanel.transform);
                        DaniDojoAssetUtility.CreateImage("PrevSongHitReqOneBarBorder", Path.Combine(BaseImageFilePath, "Enso", "Bars", "PrevSongBarBorder.png"), new Vector2(1128, 83), newPanel.transform);
                        if (DaniPlayManager.GetCurrentSongNumber() >= 2)
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
                Plugin.LogInfo("UpdateRequirementBar Start", 2);
                var indexes = DaniPlayManager.GetIndexexOfBorder(borderType);
                var borders = DaniPlayManager.GetCurrentBorderOfType(borderType);
                var currentValue = DaniPlayManager.GetCurrentBorderValue(borderType);

                for (int j = 0; j < indexes.Count; j++)
                {
                    Plugin.LogInfo("UpdateRequirementBar: Index: " + j, 2);
                    GameObject panel = GameObject.Find("Panel" + indexes[j]);
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
                                int requirementValue = DaniPlayManager.GetCurrentBorderRequirement(borders[j]);


                                bool isGold = false;

                                isGold = DaniPlayManager.CalculateBorderMidSong(borders[j]) == DaniRank.GoldClear;

                                var newScale = emptyImage.transform.localScale;
                                if (borderType == BorderType.Oks ||
                                    borderType == BorderType.Bads)
                                {
                                    currentValue[j] = requirementValue - currentValue[j];
                                    currentValue[j] = Math.Max(currentValue[j], 0);

                                    // Sorta hardcoded, but it'll basically not be gold ever
                                    isGold = false;
                                }

                                if (isGold && colorLerp != null)
                                {
                                    colorLerp.BeginRainbow(borders[j].IsTotal);
                                }

                                Plugin.LogInfo("ChangeReqCurrentValue: currentValue[j]: " + currentValue[j], 2);

                                ChangeReqCurrentValue(panel, currentValue[j], isGold);

                                newScale.x = currentValue[j] / (float)requirementValue;

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

                        // Why is this even here?
                        // This should only be updated once at the start of the 2nd and 3rd song
                        // This is instead being updated any time the main bar is updated for that border, when nothing here should change
                        if (DaniPlayManager.GetCurrentSongNumber() > 0)
                        {
                            var songValues = DaniPlayManager.GetBorderPlayResults(borders[j]);
                            var prevSongBar1 = panel.transform.Find("PrevSongHitReqOneBarFill");
                            if (prevSongBar1 != null)
                            {
                                var image = prevSongBar1.GetComponent<Image>();
                                if (image != null)
                                {
                                    int requirementValue = borders[j].RedReqs[0];
                                    var newScale = image.transform.localScale;
                                    if (borderType == BorderType.Oks ||
                                        borderType == BorderType.Bads)
                                    {
                                        newScale.x = (requirementValue - songValues[0]) / (float)requirementValue;
                                    }
                                    else
                                    {
                                        newScale.x = songValues[0] / (float)requirementValue;
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
                            if (DaniPlayManager.GetCurrentSongNumber() > 1)
                            {
                                var prevSongBar2 = panel.transform.Find("PrevSongHitReqTwoBarFill");
                                if (prevSongBar2 != null)
                                {
                                    var image = prevSongBar2.GetComponent<Image>();
                                    if (image != null)
                                    {
                                        int requirementValue = borders[j].RedReqs[1];
                                        var newScale = image.transform.localScale;
                                        if (borderType == BorderType.Oks ||
                                            borderType == BorderType.Bads)
                                        {
                                            newScale.x = (requirementValue - songValues[1]) / (float)requirementValue;
                                        }
                                        else
                                        {
                                            newScale.x = songValues[1] / (float)requirementValue;
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
                if (!Directory.Exists(Path.Combine(BaseImageFilePath, "Digits")))
                {
                    return;
                }
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
                        if (DigitBigBorder.Count > numValue)
                        {
                            DaniDojoAssetUtility.ChangeImageSprite(digitBorder, DigitBigBorder[numValue]);
                        }
                        if (DigitBigFill.Count > numValue)
                        {
                            DaniDojoAssetUtility.ChangeImageSprite(digitFill, DigitBigFill[numValue]);
                        }
                        if (DigitBigTransparent.Count > numValue)
                        {
                            DaniDojoAssetUtility.ChangeImageSprite(digitTransparent, DigitBigTransparent[numValue]);
                        }

                        var digitBorderImage = digitBorder.GetComponent<Image>();
                        var digitFillImage = digitFill.GetComponent<Image>();
                        var digitTransparentImage = digitTransparent.GetComponent<Image>();

                        if (digitBorderImage != null)
                        {
                            if (isGold)
                            {
                                digitBorderImage.color = GoldReqTextBorderColor;
                            }
                            else if (num == "0")
                            {
                                digitBorderImage.color = ZeroTextBorderColor;
                            }
                            else
                            {
                                digitBorderImage.color = NormalTextBorderColor;
                            }
                        }
                        if (digitFillImage != null)
                        {
                            if (isGold)
                            {
                                digitFillImage.color = GoldReqTextFillColor;
                            }
                            else if (num == "0")
                            {
                                digitFillImage.color = ZeroTextFillColor;
                            }
                            else
                            {
                                digitFillImage.color = NormalTextFillColor;
                            }
                        }
                        if (digitTransparentImage != null)
                        {
                            if (isGold)
                            {
                                digitTransparentImage.color = GoldReqTextTransparentColor;
                            }
                            else if (num == "0")
                            {
                                digitTransparentImage.color = ZeroTextTransparentColor;
                            }
                            else
                            {
                                digitTransparentImage.color = NormalTextTransparentColor;
                            }
                        }
                    }
                    else
                    {
                        var digitBorderImage = digitBorder.GetComponent<Image>();
                        var digitFillImage = digitFill.GetComponent<Image>();
                        var digitTransparentImage = digitTransparent.GetComponent<Image>();
                        if (digitBorderImage != null)
                        {
                            digitBorderImage.color = InvisibleColor;
                        }
                        if (digitFillImage != null)
                        {
                            digitFillImage.color = InvisibleColor;
                        }
                        if (digitTransparentImage != null)
                        {
                            digitTransparentImage.color = InvisibleColor;
                        }
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
                        DigitBigBorder.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Big", "Border", i.ToString() + ".png")));
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
                        DigitBigFill.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Big", "NoBorder", i.ToString() + ".png")));
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
                        DigitBigTransparent.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Big", "Transparent", i.ToString() + ".png")));
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
                        DigitSmallBorder.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Small", "Border", i.ToString() + ".png")));
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
                        DigitSmallFill.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Small", "NoBorder", i.ToString() + ".png")));
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
                        DigitSmallTransparent.Add(DaniDojoAssetUtility.CreateSprite(Path.Combine(BaseImageFilePath, "Digits", "Small", "Transparent", i.ToString() + ".png")));
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
                    if (dirInfo.Exists)
                    {
                        var assetFile = dirInfo.GetFiles(asset.ToString() + ".png", SearchOption.AllDirectories);
                        if (assetFile.Length > 0)
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
                            Plugin.Log.LogError(asset.ToString() + ".png not found!");
                        }
                    }
                    else
                    {
                        Plugin.Log.LogError("DaniDojoAsset file path not found!");
                    }
                }
            }

            private static Sprite GetAssetSprite(SelectAssetName asset)
            {
                InitializeSelectAssetSprite(asset);
                if (SelectAssetSprites.ContainsKey(asset))
                {
                    return SelectAssetSprites[asset];
                }
                else
                {
                    return DaniDojoAssetUtility.CreateSprite("");
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="seriesInfo"></param>
            /// <param name="currentCourse"></param>
            /// <param name="parent"></param>
            /// <returns>The current Course GameObject.</returns>
            public static void CreateSeriesAssets(DaniSeries seriesInfo, GameObject parent)
            {
                FontTMPManager fontTMPMgr = TaikoSingletonMonoBehaviour<CommonObjects>.Instance.MyDataManager.FontTMPMgr;

                GameObject seriesTitleObject = GameObject.Find("SeriesTitle");
                if (seriesTitleObject == null)
                {
                    seriesTitleObject = DaniDojoAssetUtility.CreateText("SeriesTitle", seriesInfo.Title, new Rect(1920 - 592, 777, 500, 100),
                                          fontTMPMgr.GetDefaultFontAsset(DataConst.FontType.Japanese), fontTMPMgr.GetDefaultFontMaterial(DataConst.FontType.Japanese, DataConst.DefaultFontMaterialType.OutlineBlack),
                                          HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 255), parent.transform);
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

                var topCoursesParent = new GameObject("TopCourses");
                topCoursesParent.transform.SetParent(parent.transform);
                for (int i = 0; i < seriesInfo.Courses.Count; i++)
                {
                    var highScore = SaveDataManager.GetCourseRecord(seriesInfo.Courses[i].Hash);

                    SelectAssetName backgroundName;
                    switch (seriesInfo.Courses[i].Background)
                    {
                        case CourseBackground.Tan:
                            backgroundName = SelectAssetName.TopOriginalBg;
                            break;
                        case CourseBackground.Wood:
                            backgroundName = SelectAssetName.TopKyuuBg;
                            break;
                        case CourseBackground.Blue:
                            backgroundName = SelectAssetName.TopBlueBg;
                            break;
                        case CourseBackground.Red:
                            backgroundName = SelectAssetName.TopRedBg;
                            break;
                        case CourseBackground.Silver:
                            backgroundName = SelectAssetName.TopSilverBg;
                            break;
                        case CourseBackground.Gold:
                            backgroundName = SelectAssetName.TopGoldBg;
                            break;
                        default:
                            backgroundName = SelectAssetName.TopOriginalBg;
                            break;
                    }
                    string courseId = seriesInfo.Courses[i].Id;
                    if (int.TryParse(courseId, out int _))
                    {
                        courseId = seriesInfo.Courses[i].Title;
                    }
                    var textName = courseId switch
                    {
                        "5kyuu" or "五級 5th Kyu" => SelectAssetName.Top5kyuu,
                        "4kyuu" or "四級 4th Kyu" => SelectAssetName.Top4kyuu,
                        "3kyuu" or "三級 3rd Kyu" => SelectAssetName.Top3kyuu,
                        "2kyuu" or "二級 2nd Kyu" => SelectAssetName.Top2kyuu,
                        "1kyuu" or "一級 1st Kyu" => SelectAssetName.Top1kyuu,
                        "1dan" or "初段 1st Dan" => SelectAssetName.Top1dan,
                        "2dan" or "二段 2nd Dan" => SelectAssetName.Top2dan,
                        "3dan" or "三段 3rd Dan" => SelectAssetName.Top3dan,
                        "4dan" or "四段 4th Dan" => SelectAssetName.Top4dan,
                        "5dan" or "五段 5th Dan" => SelectAssetName.Top5dan,
                        "6dan" or "六段 6th Dan" => SelectAssetName.Top6dan,
                        "7dan" or "七段 7th Dan" => SelectAssetName.Top7dan,
                        "8dan" or "八段 8th Dan" => SelectAssetName.Top8dan,
                        "9dan" or "九段 9th Dan" => SelectAssetName.Top9dan,
                        "10dan" or "十段 10th Dan" => SelectAssetName.Top10dan,
                        "11dan" or "玄人 Kuroto" => SelectAssetName.TopKuroto,
                        "12dan" or "名人 Meijin" => SelectAssetName.TopMeijin,
                        "13dan" or "超人 Chojin" => SelectAssetName.TopChojin,
                        "14dan" or "達人 Tatsujin" => SelectAssetName.TopTatsujin,
                        _ => SelectAssetName.TopGaiden,
                    };
                    GameObject topCourseObject = new GameObject(seriesInfo.Courses[i].Title);
                    topCourseObject.transform.SetParent(topCoursesParent.transform);
                    topCourseObject.transform.position = new Vector2(183 + (68 * i), 884);
                    DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Background", GetAssetSprite(backgroundName), new Vector2(0, 0), topCourseObject.transform);
                    DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Text", GetAssetSprite(textName), new Vector2(19, 24), topCourseObject.transform);

                    bool skip = false;
                    SelectAssetName recordName = SelectAssetName.CourseNormal;
                    if (highScore.RankCombo.Rank == DaniRank.GoldClear)
                    {
                        if (highScore.RankCombo.Combo == DaniCombo.Rainbow)
                        {
                            recordName = SelectAssetName.SmallGoldDFC;
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Gold)
                        {
                            recordName = SelectAssetName.SmallGoldFC;
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Silver)
                        {
                            recordName = SelectAssetName.SmallGoldClear;
                        }
                    }
                    else if (highScore.RankCombo.Rank == DaniRank.RedClear)
                    {
                        if (highScore.RankCombo.Combo == DaniCombo.Rainbow)
                        {
                            recordName = SelectAssetName.SmallRedDFC;
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Gold)
                        {
                            recordName = SelectAssetName.SmallRedFC;
                        }
                        else if (highScore.RankCombo.Combo == DaniCombo.Silver)
                        {
                            recordName = SelectAssetName.SmallRedClear;
                        }
                    }

                    if (recordName != SelectAssetName.CourseNormal)
                    {
                        DaniDojoAssetUtility.CreateImage(seriesInfo.Courses[i].Title + "Record", GetAssetSprite(recordName), new Vector2(0, 98), topCourseObject.transform);
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

                var courseObject = DaniDojoAssetUtility.CreateImage("CourseBg" + courseBgNum++, GetAssetSprite(SelectAssetName.CourseBg), CourseBgLocation, parent.transform);

                SelectAssetName leftBorderName;
                SelectAssetName rightBorderName;
                var background = courseInfo.Background;

                switch (background)
                {
                    case CourseBackground.None:
                    case CourseBackground.Tan:
                        leftBorderName = SelectAssetName.CourseBorderLeftGaiden;
                        rightBorderName = SelectAssetName.CourseBorderRightGaiden;
                        break;
                    case CourseBackground.Wood:
                        leftBorderName = SelectAssetName.CourseBorderLeftKyuu;
                        rightBorderName = SelectAssetName.CourseBorderRightKyuu;
                        break;
                    case CourseBackground.Blue:
                        leftBorderName = SelectAssetName.CourseBorderLeftBlue;
                        rightBorderName = SelectAssetName.CourseBorderRightBlue;
                        break;
                    case CourseBackground.Red:
                        leftBorderName = SelectAssetName.CourseBorderLeftRed;
                        rightBorderName = SelectAssetName.CourseBorderRightRed;
                        break;
                    case CourseBackground.Silver:
                        leftBorderName = SelectAssetName.CourseBorderLeftSilver;
                        rightBorderName = SelectAssetName.CourseBorderRightSilver;
                        break;
                    case CourseBackground.Gold:
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

                for (int i = 0; i < Math.Min(courseInfo.Songs.Count, 3); i++)
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

                    var song = musicInfoAccessers.Find((x) => x.Id == courseInfo.Songs[i].SongId);
                    string songTitle = "Song not found: ";
                    if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Jp")
                    {
                        songTitle += courseInfo.Songs[i].TitleJp;
                    }
                    else if (Plugin.Instance.ConfigSongTitleLanguage.Value == "Eng")
                    {
                        songTitle += courseInfo.Songs[i].TitleEng;
                    }
                    else
                    {
                        songTitle += courseInfo.Songs[i].SongId;
                    }
                    string songDetail = "";
                    if (song != null)
                    {
                        songTitle = wordDataMgr.GetWordListInfo("song_" + song.Id).Text;
                        songDetail = wordDataMgr.GetWordListInfo("song_detail_" + song.Id).Text;
                    }

                    if (courseInfo.Songs[i].IsHidden)
                    {
                        // SongReached is 0 indexed
                        if (highScore.SongReached <= i)
                        {
                            songTitle = "? ? ?";
                            songDetail = "";
                        }
                    }


                    SelectAssetName levelAssetName;
                    switch (courseInfo.Songs[i].Level)
                    {
                        case EnsoData.EnsoLevelType.Easy: levelAssetName = SelectAssetName.CourseEasy; break;
                        case EnsoData.EnsoLevelType.Normal: levelAssetName = SelectAssetName.CourseNormal; break;
                        case EnsoData.EnsoLevelType.Hard: levelAssetName = SelectAssetName.CourseHard; break;
                        case EnsoData.EnsoLevelType.Mania: levelAssetName = SelectAssetName.CourseOni; break;
                        case EnsoData.EnsoLevelType.Ura: levelAssetName = SelectAssetName.CourseUra; break;
                        default: levelAssetName = SelectAssetName.CourseOni; break;
                    }
                    DaniDojoAssetUtility.CreateImage("SongCourse", GetAssetSprite(levelAssetName), new Vector2(112, 42), songParent.transform);

                    SelectAssetName starAssetName;

                    if (song != null)
                    {
                        switch (song.Stars[(int)courseInfo.Songs[i].Level])
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
                for (int i = 0; i < courseInfo.Borders.Count; i++)
                {
                    if (courseInfo.Borders[i].BorderType == BorderType.SoulGauge)
                    {
                        soulGaugeRequirement = courseInfo.Borders[i].RedReqs[0];
                        break;
                    }
                }

                Rect SoulGaugeReqValueRect = new Rect(50, 73, 256, 39);
                DaniDojoAssetUtility.CreateText("SoulGaugeReqValue", soulGaugeRequirement.ToString() + "% or higher", SoulGaugeReqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), SoulGaugeReqPanel.transform);

                if (highScore.SongReached > 0)
                {
                    Rect SoulGaugeHighScoreHeaderRect = new Rect(12, 27, 100, 25);
                    var SoulGaugeHighScoreHeader = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreHeader", "High Score", SoulGaugeHighScoreHeaderRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Left, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    SoulGaugeHighScoreHeader.GetComponent<TextMeshProUGUI>().color = Color.black;

                    Rect SoulGaugeHighScoreValueRect = new Rect(200, 20, 100, 40);
                    var SoulGaugeHighScoreValue = DaniDojoAssetUtility.CreateText("SoulGaugeHighScoreValue", highScore.PlayData.Max((x) => x.SoulGauge).ToString() + " %", SoulGaugeHighScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), SoulGaugeReqPanel.transform);
                    SoulGaugeHighScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
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
                    var reqPanel = DaniDojoAssetUtility.CreateImage("ReqPanel" + (i + 1), GetAssetSprite(SelectAssetName.RequirementsEachPanel), new Vector2(427, 310 - (passedSoulGauge ? (i - 1) * 132 : i * 132)), courseObject.transform);

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
                    DaniDojoAssetUtility.CreateText("SoulGaugeHeader", requirementTypeText, reqPanelHeaderRect, reqTypeFont, reqTypeFontMaterial, HorizontalAlignmentOptions.Center, new Color32(74, 64, 51, 255), reqPanel.transform);

                    if (courseInfo.Borders[i].IsTotal)
                    {
                        var reqPanelValue = DaniDojoAssetUtility.CreateImage("ReqPanelValue" + (i + 1), GetAssetSprite(SelectAssetName.ReqSongIndicatorTotal), new Vector2(14, 49), reqPanel.transform);

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
                        DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);

                        var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (i + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14, 11), reqPanel.transform);

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
                            var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                            highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < Math.Min(courseInfo.Songs.Count, 3); j++)
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
                            if (courseInfo.Borders[i].BorderType == BorderType.Oks || courseInfo.Borders[i].BorderType == BorderType.Bads)
                            {
                                valueText = "Less than " + courseInfo.Borders[i].RedReqs[j];
                            }
                            else
                            {
                                valueText = courseInfo.Borders[i].RedReqs[j] + " or more";
                            }
                            Rect reqValueRect = new Rect(50, 0, 256, 31);
                            DaniDojoAssetUtility.CreateText("ReqValue" + (i + 1), valueText, reqValueRect, reqValueFont, reqValueFontMaterial, HorizontalAlignmentOptions.Right, new Color32(74, 64, 51, 255), reqPanelValue.transform);

                            var reqPanelBest = DaniDojoAssetUtility.CreateImage("ReqPanelBest" + (j + 1), GetAssetSprite(SelectAssetName.PersonalBestBg), new Vector2(14 + xOffset, 11), reqPanel.transform);

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
                                var highScoreValue = DaniDojoAssetUtility.CreateText("HighScoreValue", highScoreValueText, highScoreValueRect, detailFont, detailFontMaterial, HorizontalAlignmentOptions.Right, new Color32(0, 0, 0, 0), reqPanelBest.transform);
                                highScoreValue.GetComponent<TextMeshProUGUI>().color = Color.black;
                            }
                        }
                    }

                }

                var bgImage = courseInfo.Background switch
                {
                    CourseBackground.Wood => SelectAssetName.WoodBg,
                    CourseBackground.Blue => SelectAssetName.BlueBg,
                    CourseBackground.Red => SelectAssetName.RedBg,
                    CourseBackground.Silver => SelectAssetName.SilverBg,
                    CourseBackground.Gold => SelectAssetName.GoldBg,
                    _ => SelectAssetName.TanBg,
                };

                var courseId = courseInfo.Id;
                if (int.TryParse(courseId, out int _))
                {
                    courseId = courseInfo.Title;
                }

                var textImage = courseId switch
                {
                    "5kyuu" or "五級 5th Kyu" => SelectAssetName.kyuu5,
                    "4kyuu" or "四級 4th Kyu" => SelectAssetName.kyuu4,
                    "3kyuu" or "三級 3rd Kyu" => SelectAssetName.kyuu3,
                    "2kyuu" or "二級 2nd Kyu" => SelectAssetName.kyuu2,
                    "1kyuu" or "一級 1st Kyu" => SelectAssetName.kyuu1,
                    "1dan" or "初段 1st Dan" => SelectAssetName.dan1,
                    "2dan" or "二段 2nd Dan" => SelectAssetName.dan2,
                    "3dan" or "三段 3rd Dan" => SelectAssetName.dan3,
                    "4dan" or "四段 4th Dan" => SelectAssetName.dan4,
                    "5dan" or "五段 5th Dan" => SelectAssetName.dan5,
                    "6dan" or "六段 6th Dan" => SelectAssetName.dan6,
                    "7dan" or "七段 7th Dan" => SelectAssetName.dan7,
                    "8dan" or "八段 8th Dan" => SelectAssetName.dan8,
                    "9dan" or "九段 9th Dan" => SelectAssetName.dan9,
                    "10dan" or "十段 10th Dan" => SelectAssetName.dan10,
                    "11dan" or "玄人 Kuroto" => SelectAssetName.kuroto,
                    "12dan" or "名人 Meijin" => SelectAssetName.meijin,
                    "13dan" or "超人 Chojin" => SelectAssetName.chojin,
                    "14dan" or "達人 Tatsujin" => SelectAssetName.tatsujin,
                    _ => SelectAssetName.gaiden,
                };

                var baseCourseTagPos = new Vector2(-212, 332);
                DaniDojoAssetUtility.CreateImage("CurrentDanMarkerBack", GetAssetSprite(bgImage), baseCourseTagPos, courseObject.transform);
                DaniDojoAssetUtility.CreateImage("CurrentDanMarkerText", GetAssetSprite(textImage), new Vector2(baseCourseTagPos.x + 52, baseCourseTagPos.y + 124), courseObject.transform);


                if (highScore.SongReached > 0)
                {
                    bool skip = false;
                    SelectAssetName resultAsset = SelectAssetName.CourseNormal;
                    if (highScore.RankCombo.Rank == DaniRank.GoldClear)
                    {
                        switch (highScore.RankCombo.Combo)
                        {
                            case DaniCombo.Silver: resultAsset = SelectAssetName.BigGoldClear; break;
                            case DaniCombo.Gold: resultAsset = SelectAssetName.BigGoldFC; break;
                            case DaniCombo.Rainbow: resultAsset = SelectAssetName.BigGoldDFC; break;
                        }
                    }
                    else if (highScore.RankCombo.Rank == DaniRank.RedClear)
                    {
                        switch (highScore.RankCombo.Combo)
                        {
                            case DaniCombo.Silver: resultAsset = SelectAssetName.BigRedDFC; break;
                            case DaniCombo.Gold: resultAsset = SelectAssetName.BigRedFC; break;


                            case DaniCombo.Rainbow: resultAsset = SelectAssetName.BigRedClear; break;
                        }
                    }

                    if (resultAsset != SelectAssetName.CourseNormal)
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
