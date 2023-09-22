using DaniDojo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace DaniDojo.Assets
{
    internal class DigitColors
    {
        static public Dictionary<RequirementBarState, Color32> RequirementBarBorderColors = new Dictionary<RequirementBarState, Color32>()
        {
            [RequirementBarState.Failed] = new Color32(177, 177, 177, 255),
            [RequirementBarState.Zero] = new Color32(177, 177, 177, 255),
            [RequirementBarState.Normal] = new Color32(177, 177, 177, 255),
            [RequirementBarState.Gold] = new Color32(221, 89, 56, 255),
        };
        static public Dictionary<RequirementBarState, Color32> RequirementBarFillColors = new Dictionary<RequirementBarState, Color32>()
        {
            [RequirementBarState.Failed] = new Color32(0, 0, 0, 0),
            [RequirementBarState.Zero] = new Color32(0, 0, 0, 0),
            [RequirementBarState.Normal] = new Color32(255, 255, 255, 255),
            [RequirementBarState.Gold] = new Color32(255, 93, 127, 255),
        };
        static public Dictionary<RequirementBarState, Color32> RequirementBarTransparentColors = new Dictionary<RequirementBarState, Color32>()
        {
            [RequirementBarState.Failed] = new Color32(0, 0, 0, 0),
            [RequirementBarState.Zero] = new Color32(0, 0, 0, 0),
            [RequirementBarState.Normal] = new Color32(0, 0, 0, 0),
            [RequirementBarState.Gold] = new Color32(255, 244, 45, 255),
        };
    }

    enum RequirementBarState
    {
        Failed,
        Zero,
        Normal,
        Gold
    }
    enum RequirementBarType
    {
        Large,  // Enso Current Song, Result Total Song
        Medium, // Result Per Song
        Small   // Enso Previous Songs
    }
    internal class DigitAssets
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        static public RequirementBarState GetRequirementBarState(BorderBarData barData, DaniBorder border)
        {
            if (barData.State == BorderBarState.Rainbow)
            {
                return RequirementBarState.Gold;
            }
            else if ((border.BorderType == BorderType.Bads || border.BorderType == BorderType.Oks) && barData.PlayValue == 0)
            {
                return RequirementBarState.Failed;
            }
            else if (barData.PlayValue == 0)
            {
                return RequirementBarState.Zero;
            }
            else
            {
                return RequirementBarState.Normal;
            }
        }

        static public GameObject CreateRequirementBarNumber(GameObject parent, Vector2 position, int number, RequirementBarType type, RequirementBarState state)
        {
            // Max number of 99,999,999 maybe?
            const string MaxNumber = "12345678";

            // If it already exists, we want to change the digits, not just add new ones on top
            var numberObject = AssetUtility.GetOrCreateEmptyChild(parent, "RequirementBarNumber", position);
            for (int i = 0; i < MaxNumber.Length; i++)
            {
                CreateRequirementBarDigit(numberObject, number, i, type, state);
            }

            return numberObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent">The parent GameObject.</param>
        /// <param name="number">The full number to be printed.</param>
        /// <param name="index">The index of the full number being created.</param>
        /// <param name="state">Determines what colors the parts of the digit will be.</param>
        /// <returns>The new digit GameObject.</returns>
        static private GameObject CreateRequirementBarDigit(GameObject parent, int number, int index, RequirementBarType type, RequirementBarState state)
        {
            // TODO: Do something about digits that don't exist
            // If you're at 10 bads left, and you go down to 9, that 0 should disappear
            var numberString = number.ToString();
            var digitString = string.Empty;
            int digitSpacing = 0;
            switch (type)
            {
                case RequirementBarType.Large: digitSpacing = 56; break;
                case RequirementBarType.Medium: digitSpacing = 27; break;
                case RequirementBarType.Small: digitSpacing = 20; break;
            }
            var digitObject = AssetUtility.GetOrCreateEmptyChild(parent, "Digit" + (index + 1), new Vector2(index * digitSpacing, 0));
            if (numberString.Length <= index)
            {
                digitObject.SetActive(false);
                return digitObject;
            }
            else
            {
                digitObject.SetActive(true);
                if (numberString.Length == 1)
                {
                    digitString = numberString;
                }
                else
                {
                    digitString = numberString[index].ToString();
                }
            }

            Vector2 borderRect = Vector2.zero;
            Vector2 fillRect = Vector2.zero;
            Vector2 transparentRect = Vector2.zero;

            if (type == RequirementBarType.Medium)
            {
                transparentRect = new Vector2(-2, 2);
            }

            var border = AssetUtility.GetOrCreateImageChild(digitObject, "Border", borderRect, Path.Combine(AssetFilePath, "Digits", "RequirementBar" + type.ToString(), "Border", digitString + ".png"));
            var fill = AssetUtility.GetOrCreateImageChild(digitObject, "Fill", fillRect, Path.Combine(AssetFilePath, "Digits", "RequirementBar" + type.ToString(), "Fill", digitString + ".png"));
            var transparent = AssetUtility.GetOrCreateImageChild(digitObject, "Transparent", transparentRect, Path.Combine(AssetFilePath, "Digits", "RequirementBar" + type.ToString(), "Transparent", digitString + ".png"));

            border.GetOrAddComponent<Image>().color = DigitColors.RequirementBarBorderColors[state];
            fill.GetOrAddComponent<Image>().color = DigitColors.RequirementBarFillColors[state];
            transparent.GetOrAddComponent<Image>().color = DigitColors.RequirementBarTransparentColors[state];

            return digitObject;
        }
    }
}
