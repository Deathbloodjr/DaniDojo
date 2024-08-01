using Blittables;
using CriMana;
using DaniDojo.Data;
using DaniDojo.Hooks;
using DaniDojo.Patches;
using DaniDojo.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static EnsoData.PlayerResult;
using static TaikoCoreTypes;

namespace DaniDojo.Managers
{
    internal class SoulGaugeManager
    {
        internal class SoulGaugeData
        {
            public int MaxGauge { get; set; }
            public int PerGood { get; set; }
            public int PerOk { get; set; }
            public int PerBad { get; set; }
            public float RatioProf { get; set; }
            public float RatioMaster { get; set; }
        }

        static List<SoulGaugeData> AllSongSoulGaugeData = new List<SoulGaugeData>();
        static int CurrentSongIndex = 0;
        static int CurrentSoulGaugeValue = 0;

        public static void InitializeSoulGaugeData(DaniCourse course)
        {
            AllSongSoulGaugeData.Clear();

            for (int i = 0; i < course.Songs.Count; i++)
            {
                var songData = GetSoulGaugeData(course.Songs[i]);
                AllSongSoulGaugeData.Add(songData);
            }

            CurrentSongIndex = 0;
            CurrentSoulGaugeValue = 0;

            TamasiiGaugeHooks.Initialize = true;
        }

        static SoulGaugeData GetSoulGaugeData(DaniSongData song)
        {
            SoulGaugeData data = new SoulGaugeData();
            // Read from the fumen files to get the few necessary bytes
            var bytes = GetFumenData(song.SongId, song.Level).ToArray();

            data.MaxGauge = BitConverter.ToInt32(bytes, 0x1B4);
            data.PerGood = BitConverter.ToInt32(bytes, 0x1BC);
            data.PerOk = BitConverter.ToInt32(bytes, 0x1C0);
            data.PerBad = BitConverter.ToInt32(bytes, 0x1C4);

            // I'm a bit unsure of this calculation, but we'll go with it for now
            var tmpRatioProf = BitConverter.ToInt32(bytes, 0x1CC);
            var tmpRatioMaster = BitConverter.ToInt32(bytes, 0x1D0);
            data.RatioProf = tmpRatioProf / 65536;
            data.RatioMaster = tmpRatioMaster / 65536;

            return data;
        }

        static List<OnpuTypes> ValidOnpuTypes = new List<OnpuTypes>
        {
            OnpuTypes.Don,
            OnpuTypes.Do,
            OnpuTypes.Ko,
            OnpuTypes.Katsu,
            OnpuTypes.Ka,
            OnpuTypes.DaiDon,
            OnpuTypes.DaiKatsu,
            OnpuTypes.WDon,
            OnpuTypes.WKatsu,
        };
        public static void AddSoulGaugeValue(TaikoCoreFrameResults frameResult)
        {
            // My other function shows this going to frameResult.hitResultInfoNum - 1
            // That doesn't sound correct to me, but maybe it is, idk
            for (int i = 0; i < frameResult.hitResultInfoNum; i++)
            {
                if (frameResult.hitResultInfo[i].player == 0)
                {
                    var info = frameResult.hitResultInfo[i];
                    int hitResult = info.hitResult;
                    var branch = (TaikoCoreTypes.BranchTypes)info.onpu.branchType;
                    var increaseValue = 0;
                    if (ValidOnpuTypes.Contains((OnpuTypes)info.onpuType))
                    {
                        if (hitResult == (int)HitResultTypes.Fuka || hitResult == (int)HitResultTypes.Drop)
                        {
                            increaseValue = AllSongSoulGaugeData[CurrentSongIndex].PerBad;
                        }
                        else if (hitResult == (int)HitResultTypes.Ka)
                        {
                            increaseValue = AllSongSoulGaugeData[CurrentSongIndex].PerOk;
                        }
                        else if (hitResult == (int)HitResultTypes.Ryo)
                        {
                            increaseValue = AllSongSoulGaugeData[CurrentSongIndex].PerGood;
                        }
                    }
                    if (branch == BranchTypes.Kurouto)
                    {
                        increaseValue = (int)(increaseValue * AllSongSoulGaugeData[CurrentSongIndex].RatioProf);
                    }
                    else if (branch == BranchTypes.Tatsujin)
                    {
                        increaseValue = (int)(increaseValue * AllSongSoulGaugeData[CurrentSongIndex].RatioMaster);
                    }
                    // Value cannot drop below 0
                    CurrentSoulGaugeValue = Math.Max(CurrentSoulGaugeValue + increaseValue, 0);
                    // And value cannot go above the sum of max
                    CurrentSoulGaugeValue = Math.Min(CurrentSoulGaugeValue, AllSongSoulGaugeData.Sum(x => x.MaxGauge));
                }
            }
            if (frameResult.hitResultInfoNum > 0)
            {
                var tamasiiGauge = GameObject.FindObjectOfType<TamasiiGaugePlayer>();
                if (tamasiiGauge != null)
                {
                    tamasiiGauge.SetTamasiiGauge(CurrentSoulGaugeValue, 69f);
                }
                var tamasiiGaugePlayerSprite = GameObject.FindObjectOfType<TamasiiGaugePlayerSprite>();
                if (tamasiiGaugePlayerSprite != null)
                {
                    tamasiiGaugePlayerSprite.SetTamasiiGauge(CurrentSoulGaugeValue, 69f);
                }
            }
        }

        public static float GetCurrentSoulGaugePercent()
        {
            return ((float)CurrentSoulGaugeValue / (float)AllSongSoulGaugeData.Sum(x => x.MaxGauge)) * 100;
        }

        static List<byte> GetFumenData(string songId, EnsoData.EnsoLevelType level)
        {
            string[] array =
            [
                "_e",
                "_n",
                "_h",
                "_m",
                "_x"
            ];

            var filePath = Path.Combine(Application.streamingAssetsPath, string.Concat(
            [
                "fumen/",
                songId,
                "/",
                songId,
                array[(int)level],
                ".bin"
            ]));

            try
            {
                return Cryptgraphy.ReadAllAesAndGZipBytes(filePath, Cryptgraphy.AesKeyType.Type2).ToList();
            }
            catch (Exception)
            {
                // I'm upset at TakoTako for making me do this, rather than just ReadAllAesAndGZipBytes to get the data
                var customPath = FumenReading.GetCustomFumenPath(songId, level);
                var bytes = File.ReadAllBytes(customPath).ToList();

                bool gzipped = true;
                List<byte> gzippedFileHeader = new List<byte>() { 0x1F, 0x8B, 0x08 };
                for (int i = 0; i < gzippedFileHeader.Count; i++)
                {
                    if (bytes[i] != gzippedFileHeader[i])
                    {
                        gzipped = false;
                        break;
                    }
                }
                if (!gzipped)
                {
                    return bytes;
                }
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    using (GZipStream gzipStream = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        gzipStream.CopyTo(memoryStream);
                    }
                    return memoryStream.ToArray().ToList();
                }
            }
        }
    }
}
