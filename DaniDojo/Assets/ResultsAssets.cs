using DaniDojo.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DaniDojo.Assets
{
    internal class ResultsAssets
    {
        static string AssetFilePath => Plugin.Instance.ConfigDaniDojoAssetLocation.Value;

        static public GameObject CreateBg(GameObject parent)
        {
            return AssetUtility.AddImageChild(parent, "DaniResultBg", new Vector2(0, 0), Path.Combine(AssetFilePath, "Results", "Background.png"));
        }

        static public GameObject CreateSongPanel(GameObject parent)
        {
            var songBg = AssetUtility.AddImageChild(parent, "SongMainBg", new Vector2(352, 69), Path.Combine(AssetFilePath, "Results", "SongsWoodBackground.png"));
            //CreateEachSongBg(songBg);
            return songBg;
        }

        static public void CreateEachSongBg(GameObject parent, DaniCourse course)
        {
            for (int i = 0; i < Math.Max(course.Songs.Count, 3); i++)
            {
                int x = 28;
                int y = 55 + (i * 276);
                var songBg = AssetUtility.AddImageChild(parent, "SongBg", new Vector2(x, y), Path.Combine(AssetFilePath, "Results", "SongBg.png"));
                var songPanel = AssetUtility.AddImageChild(songBg, "SongPanel", new Vector2(37, 119), Path.Combine(AssetFilePath, "Results", "SongPanel.png"));
                int valuesX = 136;
                int valuesY = 37;
                int valuesInterval = 317;
                var songGoodsPanel = AssetUtility.AddImageChild(songBg, "SongGoodsPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongGoodsBg.png"));
                valuesX += valuesInterval;
                var songOksPanel = AssetUtility.AddImageChild(songBg, "SongOksPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongOksBg.png"));
                valuesX += valuesInterval;
                var songBadsPanel = AssetUtility.AddImageChild(songBg, "SongBadsPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongBadsBg.png"));
                valuesX += valuesInterval;
                var songDrumrollsPanel = AssetUtility.AddImageChild(songBg, "SongDrumrollPanel", new Vector2(valuesX, valuesY), Path.Combine(AssetFilePath, "Results", "SongDrumrollBg.png"));
            }
        }

        static public GameObject CreatePlayRecordBg(GameObject parent)
        {
            var playRecordBg = AssetUtility.AddImageChild(parent, "PlayRecord", new Vector2(337 + 1920, 44), Path.Combine(AssetFilePath, "Results", "PlayResultsBackground.png"));
            return playRecordBg;
        }

        static public GameObject CreatePlayRecordScoreBg(GameObject parent)
        {
            var scoreBg = AssetUtility.AddImageChild(parent, "ScoreBg", new Vector2(128, 752), Path.Combine(AssetFilePath, "Results", "PlayScoreBg.png"));
            return scoreBg;
        }

        static public GameObject CreatePlayRecordGoodOkBadBg(GameObject parent)
        {
            var playRecordBg = AssetUtility.AddImageChild(parent, "PlayRecordBg1", new Vector2(571, 696), Path.Combine(AssetFilePath, "Results", "PlayRecord1.png"));
            return playRecordBg;
        }

        static public GameObject CreatePlayRecordDrumrollComboTotalHitsBg(GameObject parent)
        {
            var playRecordBg = AssetUtility.AddImageChild(parent, "PlayRecordBg2", new Vector2(955, 696), Path.Combine(AssetFilePath, "Results", "PlayRecord2.png"));
            return playRecordBg;
        }
    }
}
