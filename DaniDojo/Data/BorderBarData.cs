using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Data
{
    class BorderBarColors
    {
        static public Dictionary<BorderBarState, Color32> BorderBarColor = new Dictionary<BorderBarState, Color32>()
        {
            [BorderBarState.Rainbow] = new Color32(255, 255, 255, 255),
            [BorderBarState.Pink] = new Color32(254, 161, 182, 255),
            [BorderBarState.Yellow] = new Color32(249, 254, 54, 255),
            [BorderBarState.DarkYellow] = new Color32(249, 204, 35, 255),
            [BorderBarState.Orange] = new Color32(250, 124, 77, 255),
            [BorderBarState.Grey] = new Color32(69, 69, 69, 255),
        };
    }


    enum BorderBarState
    {
        Rainbow,
        Pink,
        //PinkFlashSlow,
        //PinkFlashFast,
        Yellow,
        //YellowFlash, // Only 1 speed, same speed as PinkFlashSlow
        DarkYellow,
        Orange,
        //RedOrange, // Is the same as Orange
                   //RedOrangeFlash, // Flashes black
        Grey, // for 0 I guess
    }

    enum BorderBarFlashState
    {
        None,
        WhiteOneTime,
        WhiteSlow,
        WhiteFast,
        BlackSlow,
    }

    internal class BorderBarData
    {
        public BorderBarState State { get; set; }
        public BorderBarFlashState FlashState { get; set; }
        /// <summary>
        /// Set true when state is changed, stating the bar should flash white once.
        /// </summary>
        public bool StateChanged { get; set; }
        /// <summary>
        /// The value that will be displayed.
        /// </summary>
        public int PlayValue { get; set; }
        /// <summary>
        /// int from 0-100, as a percent of how full the bar should be.
        /// </summary>
        public int FillRatio { get; set; }

        public bool Failed { get; set; }
        public Color32 Color => GetColor();

        Color32 GetColor()
        {
            return BorderBarColors.BorderBarColor[State];
        }
    }
}
