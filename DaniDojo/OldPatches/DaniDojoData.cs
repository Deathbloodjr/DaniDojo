//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Nodes;
//using System.Threading.Tasks;
//using DaniDojo.Utility;
//using DaniDojo.Data;
//using DaniDojo.Managers;

//namespace DaniDojo.Patches
//{

//    public class DaniData
//    {
//        public class DaniSong
//        {
//            public string songId { get; set; }
//            public EnsoData.EnsoLevelType level { get; set; }
//            public bool isHidden { get; set; }

//            public DaniSong()
//            {

//            }

//            public DaniSong(JsonNode node)
//            {
//                songId = node["songNo"]!.GetValue<string>();
//                level = (EnsoData.EnsoLevelType)(node["level"]!.GetValue<int>() - 1);
//                isHidden = node["isHiddenSongName"]!.GetValue<bool>();
//            }
//        }

//        public class DaniBorder
//        {
//            public BorderType borderType { get; set; } // odaiType
//            public bool isTotalRequirements { get; set; } // borderType
//            public List<int> redBorders { get; set; }
//            public List<int> goldBorders { get; set; }

//            public DaniBorder()
//            {
//                redBorders = new List<int>();
//                goldBorders = new List<int>();
//            }

//            public DaniBorder(JsonNode node)
//            {
//                redBorders = new List<int>();
//                goldBorders = new List<int>();

//                borderType = (BorderType)node["odaiType"]!.GetValue<int>();
//                isTotalRequirements = node["borderType"]!.GetValue<int>() == 1;

//                if (node["redBorderTotal"] != null)
//                {
//                    redBorders.Add(node["redBorderTotal"]!.GetValue<int>());
//                }
//                else
//                {
//                    redBorders.Add(node["redBorder_1"]!.GetValue<int>());
//                    redBorders.Add(node["redBorder_2"]!.GetValue<int>());
//                    redBorders.Add(node["redBorder_3"]!.GetValue<int>());
//                }

//                if (node["goldBorderTotal"] != null)
//                {
//                    goldBorders.Add(node["goldBorderTotal"]!.GetValue<int>());
//                }
//                else
//                {
//                    goldBorders.Add(node["goldBorder_1"]!.GetValue<int>());
//                    goldBorders.Add(node["goldBorder_2"]!.GetValue<int>());
//                    goldBorders.Add(node["goldBorder_3"]!.GetValue<int>());
//                }
//            }

//            public DanResult CheckRequirement(List<int> value)
//            {
//                DanResult result = DanResult.GoldClear;
//                if (isTotalRequirements)
//                {
//                    var sumValue = value.Sum();

//                    if (borderType == BorderType.Oks || borderType == BorderType.Bads)
//                    {
//                        if (sumValue < goldBorders[0])
//                        {
//                            result = DanResult.GoldClear;
//                        }
//                        else if (sumValue < redBorders[0])
//                        {
//                            result = DanResult.RedClear;
//                        }
//                        else
//                        {
//                            result = DanResult.NotClear;
//                        }
//                    }
//                    else // greater than or equal to
//                    {
//                        if (borderType == BorderType.SoulGauge)
//                        {
//                            sumValue = (int)value.Average();
//                        }
//                        if (sumValue >= goldBorders[0])
//                        {
//                            result = DanResult.GoldClear;
//                        }
//                        else if (sumValue >= redBorders[0])
//                        {
//                            result = DanResult.RedClear;
//                        }
//                        else
//                        {
//                            result = DanResult.NotClear;
//                        }
//                    }
//                }
//                else // separate per song
//                {
//                    if (borderType == BorderType.Oks || borderType == BorderType.Bads)
//                    {
//                        for (int i = 0; i < goldBorders.Count; i++)
//                        {
//                            if (value[i] < goldBorders[i])
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.GoldClear);
//                            }
//                            else if (value[i] < redBorders[i])
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.RedClear);
//                            }
//                            else
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.NotClear);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        for (int i = 0; i < goldBorders.Count; i++)
//                        {
//                            if (value[i] >= goldBorders[i])
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.GoldClear);
//                            }
//                            else if (value[i] >= redBorders[i])
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.RedClear);
//                            }
//                            else
//                            {
//                                result = (DanResult)Math.Min((int)result, (int)DanResult.NotClear);
//                            }
//                        }
//                    }
//                }
//                return result;
//            }
//        }

//        public int danId { get; set; }
//        public string title { get; set; }
//        public DaniCourse course { get; set; }
//        public DaniCourseBg courseBg { get; set; }
//        public bool locked { get; set; }
//        public List<DaniSong> songs { get; set; }
//        public List<DaniBorder> borders { get; set; }
//        public DaniSeriesData series { get; set; }
//        public uint hash { get; set; }

//        public DaniData()
//        {
//            songs = new List<DaniSong>();
//            borders = new List<DaniBorder>();
//        }

//        public DaniData(JsonNode node, DaniSeriesData parent)
//        {
//            songs = new List<DaniSong>();
//            borders = new List<DaniBorder>();

//            danId = node["danId"]!.GetValue<int>();
//            title = node["title"]!.GetValue<string>();
//            AssignCourse(title);

//            // Check to see if "locked" is in the json for this course
//            if (node["locked"] == null)
//            {
//                // If it isn't, set the defaults, where kuroto through tatsujin are locked
//                // Anything that isn't kuroto through tatsujin are not locked
//                if (course >= DaniCourse.kuroto && course <= DaniCourse.tatsujin)
//                {
//                    locked = true;
//                }
//                else
//                {
//                    locked = false;
//                }
//            }
//            else
//            {
//                locked = node["locked"]!.GetValue<bool>();
//            }

//            string danBg = "default";
//            if (node["danBg"] != null)
//            {
//                danBg = node["danBg"]!.GetValue<string>();
//            }
//            AssignCourseBg(danBg);

//            for (int i = 0; i < node["aryOdaiSong"]!.AsArray().Count; i++)
//            {
//                DaniSong song = new DaniSong(node["aryOdaiSong"]![i]!);
//                songs.Add(song);
//            }

//            for (int i = 0; i < node["aryOdaiBorder"]!.AsArray().Count; i++)
//            {
//                DaniBorder border = new DaniBorder(node["aryOdaiBorder"]![i]!);
//                borders.Add(border);
//            }

//            series = parent;
//            hash = GetDanCourseHash();
//        }

//        private void AssignCourse(string title)
//        {
//            switch (title.ToLower())
//            {
//                case "5kyuu":
//                case "五級 5th kyu":
//                    course = DaniCourse.kyuu5;
//                    break;
//                case "4kyuu":
//                case "四級 4th kyu":
//                    course = DaniCourse.kyuu4;
//                    break;
//                case "3kyuu":
//                case "三級 3rd kyu":
//                    course = DaniCourse.kyuu3;
//                    break;
//                case "2kyuu":
//                case "二級 2nd kyu":
//                    course = DaniCourse.kyuu2;
//                    break;
//                case "1kyuu":
//                case "一級 1st kyu":
//                    course = DaniCourse.kyuu1;
//                    break;
//                case "1dan":
//                case "初段 1st dan":
//                    course = DaniCourse.dan1;
//                    break;
//                case "2dan":
//                case "二段 2nd dan":
//                    course = DaniCourse.dan2;
//                    break;
//                case "3dan":
//                case "三段 3rd dan":
//                    course = DaniCourse.dan3;
//                    break;
//                case "4dan":
//                case "四段 4th dan":
//                    course = DaniCourse.dan4;
//                    break;
//                case "5dan":
//                case "五段 5th dan":
//                    course = DaniCourse.dan5;
//                    break;
//                case "6dan":
//                case "六段 6th dan":
//                    course = DaniCourse.dan6;
//                    break;
//                case "7dan":
//                case "七段 7th dan":
//                    course = DaniCourse.dan7;
//                    break;
//                case "8dan":
//                case "八段 8th dan":
//                    course = DaniCourse.dan8;
//                    break;
//                case "9dan":
//                case "九段 9th dan":
//                    course = DaniCourse.dan9;
//                    break;
//                case "10dan":
//                case "十段 10th dan":
//                    course = DaniCourse.dan10;
//                    break;
//                case "11dan":
//                case "玄人 kuroto":
//                    course = DaniCourse.kuroto;
//                    break;
//                case "12dan":
//                case "名人 meijin":
//                    course = DaniCourse.meijin;
//                    break;
//                case "13dan":
//                case "超人 chojin":
//                    course = DaniCourse.chojin;
//                    break;
//                case "14dan":
//                case "達人 tatsujin":
//                    course = DaniCourse.tatsujin;
//                    break;
//                case "gaiden":
//                    course = DaniCourse.gaiden;
//                    break;
//                default:
//                    course = DaniCourse.None;
//                    break;
//            }
//        }

//        private void AssignCourseBg(string danBg)
//        {
//            switch (danBg.ToLower())
//            {
//                case "default": courseBg = DaniCourseBg.Default; break;
//                case "tan": courseBg = DaniCourseBg.Tan; break;
//                case "wood": courseBg = DaniCourseBg.Wood; break;
//                case "blue": courseBg = DaniCourseBg.Blue; break;
//                case "red": courseBg = DaniCourseBg.Red; break;
//                case "silver": courseBg = DaniCourseBg.Silver; break;
//                case "gold": courseBg = DaniCourseBg.Gold; break;
//                default: courseBg = DaniCourseBg.Default; break;
//            }
//        }
//        public (string bgImage, string textImage) GetDanCourseImagePaths()
//        {
//            switch (this.course)
//            {
//                case DaniCourse.None: return ("TanBg.png", "original.png");
//                case DaniCourse.kyuu5: return ("WoodBg.png", "kyuu5.png");
//                case DaniCourse.kyuu4: return ("WoodBg.png", "kyuu4.png");
//                case DaniCourse.kyuu3: return ("WoodBg.png", "kyuu3.png");
//                case DaniCourse.kyuu2: return ("WoodBg.png", "kyuu2.png");
//                case DaniCourse.kyuu1: return ("WoodBg.png", "kyuu1.png");
//                case DaniCourse.dan1: return ("BlueBg.png", "dan1.png");
//                case DaniCourse.dan2: return ("BlueBg.png", "dan2.png");
//                case DaniCourse.dan3: return ("BlueBg.png", "dan3.png");
//                case DaniCourse.dan4: return ("BlueBg.png", "dan4.png");
//                case DaniCourse.dan5: return ("BlueBg.png", "dan5.png");
//                case DaniCourse.dan6: return ("RedBg.png", "dan6.png");
//                case DaniCourse.dan7: return ("RedBg.png", "dan7.png");
//                case DaniCourse.dan8: return ("RedBg.png", "dan8.png");
//                case DaniCourse.dan9: return ("RedBg.png", "dan9.png");
//                case DaniCourse.dan10: return ("RedBg.png", "dan10.png");
//                case DaniCourse.kuroto: return ("SilverBg.png", "kuroto.png");
//                case DaniCourse.meijin: return ("SilverBg.png", "meijin.png");
//                case DaniCourse.chojin: return ("SilverBg.png", "chojin.png");
//                case DaniCourse.tatsujin: return ("GoldBg.png", "tatsujin.png");
//                default: return ("TanBg.png", "gaiden.png");
//            }
//        }

//        public uint GetDanCourseHash()
//        {
//            string hashString = this.series.seriesId;
//            hashString += this.danId.ToString();
//            for (int i = 0; i < this.songs.Count; i++)
//            {
//                hashString += this.songs[i].songId;
//                hashString += this.songs[i].level;
//            }
//            // Removed borders from the hash
//            //for (int i = 0; i < this.borders.Count; i++)
//            //{
//            //    hashString += this.borders[i].borderType;
//            //    hashString += this.borders[i].isTotalRequirements;
//            //    for (int j = 0; j < this.borders[i].redBorders.Count; j++)
//            //    {
//            //        hashString += this.borders[i].redBorders[j];
//            //    }
//            //    for (int j = 0; j < this.borders[i].goldBorders.Count; j++)
//            //    {
//            //        hashString += this.borders[i].goldBorders[j];
//            //    }
//            //}

//            //Plugin.Log.LogInfo("hashString: " + hashString);

//            return MurmurHash2.Hash(hashString);
//        }
//    }

//    public enum DaniCourseBg
//    {
//        Tan,
//        Wood,
//        Blue,
//        Red,
//        Silver,
//        Gold,
//        Default
//    }

//    public enum DaniCourse
//    {
//        None,
//        kyuu5,
//        kyuu4,
//        kyuu3,
//        kyuu2,
//        kyuu1,
//        dan1,
//        dan2,
//        dan3,
//        dan4,
//        dan5,
//        dan6,
//        dan7,
//        dan8,
//        dan9,
//        dan10,
//        kuroto,
//        meijin,
//        chojin,
//        tatsujin,
//        gaiden,
//    }

//    public enum OldBorderType
//    {
//        SoulGauge = 1,
//        Goods,
//        Oks,
//        Bads,
//        Combo,
//        Drumroll,
//        Score,
//        TotalHits,
//    }

//    public enum DanResult
//    {
//        NotClear,
//        RedClear,
//        GoldClear,
//    }

//    public enum DanComboResult
//    {
//        None,
//        Clear,
//        FC,
//        DFC,
//    }

//    public class DaniDojoCurrentPlay
//    {
//        public Data.DaniCourse course { get; set; }
//        public int currentSong { get; set; }
//        public int currentCombo { get; set; }
//        public int combo { get; set; }
//        public int soulGauge { get; set; }
//        public List<SongResult> songResults { get; set; }
//        public DanResult danResult { get; set; }
//        public DanComboResult comboResult { get; set; }
//        public int playCount { get; set; }
//        public int songReached { get; set; }
//        public uint hash { get; set; }

//        /// <summary>
//        /// Constructor for creating a clean new CurrentPlay class
//        /// </summary>
//        /// <param name="newCourse"></param>
//        public DaniDojoCurrentPlay(Data.DaniCourse newCourse)
//        {
//            course = newCourse;
//            hash = course.Hash;
//            combo = 0;
//            currentCombo = 0;
//            songResults = new List<SongResult>();

//            for (int i = 0; i < course.Songs.Count; i++)
//            {
//                SongResult song = new SongResult();
//                songResults.Add(song);
//            }
//            currentSong = 0;
//        }

//        /// <summary>
//        /// Constructor for reading in a CurrentPlay from a json save
//        /// </summary>
//        /// <param name="node"></param>
//        public DaniDojoCurrentPlay(JsonNode node)
//        {

//            songResults = new List<SongResult>();
//            hash = node!["danHash"].GetValue<uint>();
//            danResult = (DanResult)node!["danResult"].GetValue<int>();
//            comboResult = (DanComboResult)node!["danComboResult"].GetValue<int>();
//            playCount = node!["playCount"].GetValue<int>();
//            songReached = node!["songReached"].GetValue<int>();
//            // = courses[k]!["totalSoulGauge"].GetValue<int>(),
//            combo = node!["totalCombo"].GetValue<int>();
//            soulGauge = node!["totalSoulGauge"].GetValue<int>();

//            songResults = new List<SongResult>();
//            for (int i = 0; i < node!["songScores"].AsArray().Count; i++)
//            {
//                SongResult songResult = new SongResult()
//                {
//                    score = node!["songScores"].AsArray()[i]["score"].GetValue<int>(),
//                    goods = node!["songScores"].AsArray()[i]["goods"].GetValue<int>(),
//                    oks = node!["songScores"].AsArray()[i]["oks"].GetValue<int>(),
//                    bads = node!["songScores"].AsArray()[i]["bads"].GetValue<int>(),
//                    songCombo = node!["songScores"].AsArray()[i]["combo"].GetValue<int>(),
//                    renda = node!["songScores"].AsArray()[i]["drumroll"].GetValue<int>(),
//                };
//                songResults.Add(songResult);
//            }
//        }

//        public void SaveResults()
//        {
//            Plugin.Log.LogInfo("");
//            Plugin.Log.LogInfo("Total Combo:     " + combo);
//            Plugin.Log.LogInfo("Total soulGauge: " + GetTotalSoulGauge());
//            Plugin.Log.LogInfo("Total score:     " + songResults.Sum((x) => x.score));
//            Plugin.Log.LogInfo("Total goods:     " + songResults.Sum((x) => x.goods));
//            Plugin.Log.LogInfo("Total oks:       " + songResults.Sum((x) => x.oks));
//            Plugin.Log.LogInfo("Total bads:      " + songResults.Sum((x) => x.bads));
//            Plugin.Log.LogInfo("Total renda:     " + songResults.Sum((x) => x.renda));
//            Plugin.Log.LogInfo("Total totalHits: " + songResults.Sum((x) => x.renda + x.goods + x.oks));

//            Plugin.Log.LogInfo("");
//            for (int i = 0; i < songResults.Count; i++)
//            {
//                Plugin.Log.LogInfo("Song " + i + ":");
//                Plugin.Log.LogInfo("soulGauge: " + songResults[i].GetSoulGaugePercentage());
//                Plugin.Log.LogInfo("score:     " + songResults[i].score);
//                Plugin.Log.LogInfo("goods:     " + songResults[i].goods);
//                Plugin.Log.LogInfo("oks:       " + songResults[i].oks);
//                Plugin.Log.LogInfo("bads:      " + songResults[i].bads);
//                Plugin.Log.LogInfo("renda:     " + songResults[i].renda);
//                Plugin.Log.LogInfo("totalHits: " + (songResults[i].renda + songResults[i].goods + songResults[i].oks));
//                Plugin.Log.LogInfo("songCombo: " + songResults[i].songCombo);
//                Plugin.Log.LogInfo("");
//            }

//            string resultString = string.Empty;
//            switch (CalculateRequirements())
//            {
//                case DanResult.NotClear:
//                    resultString = "Not Clear";
//                    danResult = DanResult.NotClear;
//                    break;
//                case DanResult.RedClear:
//                    resultString = "Red Clear";
//                    danResult = DanResult.RedClear;
//                    break;
//                case DanResult.GoldClear:
//                    resultString = "Gold Clear";
//                    danResult = DanResult.GoldClear;
//                    break;
//            }
//            // Just temporary, I'd want a bit more checks to make sure the songs were all completely played
//            switch (CalculateComboResult())
//            {
//                case DanComboResult.None:
//                    comboResult = DanComboResult.None;
//                    break;
//                case DanComboResult.Clear:
//                    comboResult = DanComboResult.Clear;
//                    break;
//                case DanComboResult.FC:
//                    resultString += " FC";
//                    comboResult = DanComboResult.FC;
//                    break;
//                case DanComboResult.DFC:
//                    resultString += " DFC";
//                    comboResult = DanComboResult.DFC;
//                    break;
//            }


//            for (int i = 0; i < songResults.Count; i++)
//            {
//                songResults[i].totalHits = songResults[i].goods + songResults[i].oks + songResults[i].renda;
//            }
//            soulGauge = GetTotalSoulGauge();

//            Plugin.Log.LogInfo("EndResult: " + resultString);

//            SaveCurrentRecord();
//        }

//        public void AdvanceSong()
//        {
//            currentSong++;
//            songReached = currentSong;
//        }

//        void AddCombo()
//        {
//            currentCombo++;
//            combo = Math.Max(combo, currentCombo);
//            songResults[currentSong].AddCombo();
//        }

//        void ResetCombo()
//        {
//            currentCombo = 0;
//            songResults[currentSong].ResetCombo();
//        }

//        public void UpdateSoulGauge(int newValue)
//        {
//            songResults[currentSong].soulGauge = newValue;
//        }

//        public void AddScore(int addValue)
//        {
//            songResults[currentSong].score += addValue;
//        }

//        public void AddGood()
//        {
//            songResults[currentSong].goods++;
//            AddCombo();
//            songResults[currentSong].AddScore(true);
//            songResults[currentSong].UpdateTamashii(0);
//        }
//        public void AddOk()
//        {
//            songResults[currentSong].oks++;
//            AddCombo();
//            songResults[currentSong].AddScore(false);
//            songResults[currentSong].UpdateTamashii(1);
//        }
//        public void AddBad()
//        {
//            songResults[currentSong].bads++;
//            ResetCombo();
//            songResults[currentSong].UpdateTamashii(2);
//        }
//        public void AddRenda()
//        {
//            songResults[currentSong].renda++;
//        }
//        public void SetConstPoints(int[] newTamashiiPoints, int newShinuchiPoints, int maxTamashiiPoints)
//        {
//            songResults[currentSong].SetConstPoints(newTamashiiPoints, newShinuchiPoints, maxTamashiiPoints);
//        }

//        public bool HasSetConstPoints()
//        {
//            return songResults[currentSong].HasSetConstPoints();
//        }

//        int GetTotalSoulGauge()
//        {
//            int totalMax = 0;
//            int totalGauge = 0;
//            for (int i = 0; i < songResults.Count; i++)
//            {
//                totalMax += songResults[i].tamashiiPointsMax;
//                totalGauge += songResults[i].soulGauge;
//            }
//            totalGauge = Math.Min(totalGauge, totalMax);
//            return Math.Max(0, Math.Min(100, (int)((totalGauge / (float)totalMax) * 100)));
//        }

//        public int GetCurrentResultValues(BorderType type, bool isTotal, int song = -1)
//        {
//            if (isTotal)
//            {
//                switch (type)
//                {
//                    case BorderType.SoulGauge:
//                        return GetTotalSoulGauge();
//                    case BorderType.Goods:
//                        return songResults.Sum((x) => x.goods);
//                    case BorderType.Oks:
//                        return songResults.Sum((x) => x.oks);
//                    case BorderType.Bads:
//                        return songResults.Sum((x) => x.bads);
//                    case BorderType.Combo:
//                        return combo;
//                    case BorderType.Drumroll:
//                        return songResults.Sum((x) => x.renda);
//                    case BorderType.Score:
//                        return songResults.Sum((x) => x.score);
//                    case BorderType.TotalHits:
//                        return songResults.Sum((x) => x.goods + x.oks + x.renda);
//                    default:
//                        return 0;
//                }
//            }
//            else
//            {
//                int selectedSong = currentSong;
//                if (song != -1)
//                {
//                    selectedSong = song;
//                }
//                switch (type)
//                {
//                    case BorderType.SoulGauge:
//                        return songResults[selectedSong].GetSoulGaugePercentage();
//                    case BorderType.Goods:
//                        return songResults[selectedSong].goods;
//                    case BorderType.Oks:
//                        return songResults[selectedSong].oks;
//                    case BorderType.Bads:
//                        return songResults[selectedSong].bads;
//                    case BorderType.Combo:
//                        return songResults[selectedSong].songCombo;
//                    case BorderType.Drumroll:
//                        return songResults[selectedSong].renda;
//                    case BorderType.Score:
//                        return songResults[selectedSong].score;
//                    case BorderType.TotalHits:
//                        return songResults[selectedSong].goods + songResults[selectedSong].oks + songResults[selectedSong].renda;
//                    default:
//                        return 0;
//                }
//            }

//        }

//        public void SaveCurrentRecord()
//        {
//            var hash = course.Hash;

//            playCount = 1;

//            var index = Plugin.AllDaniScores.FindIndex((x) => x.hash == hash);
//            if (index >= 0)
//            {
//                Plugin.Log.LogInfo("playCount Before: " + Plugin.AllDaniScores[index].playCount);
//                Plugin.AllDaniScores[index].playCount++;
//                Plugin.Log.LogInfo("playCount After: " + Plugin.AllDaniScores[index].playCount);

//                bool isImprovement = false;
//                bool isTie = false;
//                // Set the main result to the newly obtained result, if it is higher
//                if (danResult > Plugin.AllDaniScores[index].danResult)
//                {
//                    Plugin.AllDaniScores[index].danResult = danResult;
//                    Plugin.AllDaniScores[index].comboResult = comboResult;
//                    isImprovement = true;
//                }
//                else if (danResult == Plugin.AllDaniScores[index].danResult && comboResult > Plugin.AllDaniScores[index].comboResult)
//                {
//                    Plugin.AllDaniScores[index].comboResult = comboResult;
//                    isImprovement = true;
//                }
//                else if (danResult == Plugin.AllDaniScores[index].danResult && comboResult == Plugin.AllDaniScores[index].comboResult)
//                {
//                    isTie = true;
//                }

//                if (isTie)
//                {
//                    for (int i = 0; i < course.Borders.Count; i++)
//                    {
//                        switch (course.Borders[i].BorderType)
//                        {
//                            case Data.BorderType.Goods:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.goods) > Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.goods))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].goods = songResults[j].goods;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].goods > Plugin.AllDaniScores[index].songResults[j].goods)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].goods = songResults[j].goods;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.Oks:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.oks) < Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.oks))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].oks = songResults[j].oks;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].oks < Plugin.AllDaniScores[index].songResults[j].oks)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].oks = songResults[j].oks;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.Bads:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.bads) < Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.bads))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].bads = songResults[j].bads;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].bads < Plugin.AllDaniScores[index].songResults[j].bads)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].bads = songResults[j].bads;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.Combo:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (combo > Plugin.AllDaniScores[index].combo)
//                                    {
//                                        Plugin.AllDaniScores[index].combo = combo;
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].songCombo > Plugin.AllDaniScores[index].songResults[j].songCombo)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].songCombo = songResults[j].songCombo;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.Drumroll:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.renda) > Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.renda))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].renda = songResults[j].renda;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].renda > Plugin.AllDaniScores[index].songResults[j].renda)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].renda = songResults[j].renda;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.Score:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.score) > Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.score))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].score = songResults[j].score;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].score > Plugin.AllDaniScores[index].songResults[j].score)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].score = songResults[j].score;
//                                        }
//                                    }
//                                }
//                                break;
//                            case Data.BorderType.TotalHits:
//                                if (course.Borders[i].IsTotal)
//                                {
//                                    if (songResults.Sum((SongResult x) => x.totalHits) > Plugin.AllDaniScores[index].songResults.Sum((SongResult x) => x.totalHits))
//                                    {
//                                        for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].totalHits = songResults[j].totalHits;
//                                        }
//                                    }
//                                }
//                                else
//                                {
//                                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                                    {
//                                        if (songResults[j].totalHits > Plugin.AllDaniScores[index].songResults[j].totalHits)
//                                        {
//                                            Plugin.AllDaniScores[index].songResults[j].totalHits = songResults[j].totalHits;
//                                        }
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                }

//                if (isImprovement)
//                {
//                    Plugin.AllDaniScores[index].combo = combo;
//                    for (int j = 0; j < Plugin.AllDaniScores[index].songResults.Count; j++)
//                    {
//                        Plugin.AllDaniScores[index].songResults[j].score = songResults[j].score;
//                        Plugin.AllDaniScores[index].songResults[j].goods = songResults[j].goods;
//                        Plugin.AllDaniScores[index].songResults[j].oks = songResults[j].oks;
//                        Plugin.AllDaniScores[index].songResults[j].bads = songResults[j].bads;
//                        Plugin.AllDaniScores[index].songResults[j].songCombo = songResults[j].songCombo;
//                        Plugin.AllDaniScores[index].songResults[j].renda = songResults[j].renda;
//                        Plugin.AllDaniScores[index].songResults[j].totalHits = songResults[j].totalHits;
//                    }
//                }

//                Plugin.AllDaniScores[index].songReached = Math.Max(songReached, Plugin.AllDaniScores[index].songReached);
//            }
//            else
//            {
//                Plugin.Log.LogInfo("Adding current score 1");
//                Plugin.AllDaniScores.Add(this);
//            }

//            if (Plugin.AllDaniScores.Count == 0)
//            {
//                // There's a chance it will never go into here
//                Plugin.Log.LogInfo("Adding current score 2");
//                Plugin.AllDaniScores.Add(this);
//            }


//            // I could do more checks in here later, but this is enough for now I think
//            Plugin.Instance.SaveDaniRecords();
//        }



//        public DanResult CalculateRequirements()
//        {
//            DanResult result = DanResult.GoldClear;
//            for (int i = 0; i < course.Borders.Count; i++)
//            {
//                List<int> individualRequirements = new List<int>();
//                for (int j = 0; j < songResults.Count; j++)
//                {
//                    switch (course.Borders[i].BorderType)
//                    {
//                        case BorderType.SoulGauge:
//                            individualRequirements.Add(100);
//                            break;
//                            individualRequirements.Add(songResults[j].GetSoulGaugePercentage());
//                            break;
//                        case BorderType.Goods:
//                            individualRequirements.Add(songResults[j].goods);
//                            break;
//                        case BorderType.Oks:
//                            individualRequirements.Add(songResults[j].oks);
//                            break;
//                        case BorderType.Bads:
//                            individualRequirements.Add(songResults[j].bads);
//                            break;
//                        case BorderType.Combo:
//                            if (course.Borders[i].IsTotal)
//                            {
//                                individualRequirements.Add(combo);
//                            }
//                            else
//                            {
//                                individualRequirements.Add(songResults[j].songCombo);
//                            }
//                            break;
//                        case BorderType.Drumroll:
//                            individualRequirements.Add(songResults[j].renda);
//                            break;
//                        case BorderType.Score:
//                            individualRequirements.Add(songResults[j].score);
//                            break;
//                        case BorderType.TotalHits:
//                            individualRequirements.Add(songResults[j].goods + songResults[j].oks + songResults[j].renda);
//                            break;
//                    }
//                }
//                //result = (DanResult)Math.Min((int)result, (int)course.Borders[i].CheckRequirement(individualRequirements));
//            }

//            return result;
//        }

//        public DanComboResult CalculateComboResult()
//        {
//            if (songResults.Sum((x) => x.oks + x.bads) == 0 && songReached == course.Songs.Count - 1)
//            {
//                return DanComboResult.DFC;
//            }
//            else if (songResults.Sum((x) => x.bads) == 0 && songReached == course.Songs.Count - 1)
//            {
//                return DanComboResult.FC;
//            }
//            else if (songReached == course.Songs.Count - 1)
//            {
//                return DanComboResult.Clear;
//            }
//            else
//            {
//                return DanComboResult.None;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns>True if failed, false if not failed yet.</returns>
//        internal bool HasFailed()
//        {
//            bool isDebugLog = true;
//            if (isDebugLog)
//            {
//                Plugin.Log.LogInfo("HasFailed Start");
//            }
//            for (int i = 0; i < course.Borders.Count; i++)
//            {
//                if (course.Borders[i].IsTotal)
//                {
//                    if (course.Borders[i].BorderType == BorderType.Oks)
//                    {
//                        var current = songResults.Sum((x) => x.oks);
//                        var requirement = course.Borders[i].RedReqs.Sum();
//                        if (isDebugLog)
//                        {
//                            Plugin.Log.LogInfo("HasFailed: BorderType.Oks: current = " + current);
//                            Plugin.Log.LogInfo("HasFailed: BorderType.Oks: requirement = " + requirement);
//                        }
//                        if (current >= requirement)
//                        {
//                            return true;
//                        }
//                    }
//                    else if (course.Borders[i].BorderType == BorderType.Bads)
//                    {
//                        var current = songResults.Sum((x) => x.bads);
//                        var requirement = course.Borders[i].RedReqs.Sum();
//                        if (isDebugLog)
//                        {
//                            Plugin.Log.LogInfo("HasFailed: BorderType.Bads: current = " + current);
//                            Plugin.Log.LogInfo("HasFailed: BorderType.Bads: requirement = " + requirement);
//                        }
//                        if (current >= requirement)
//                        {
//                            return true;
//                        }
//                    }
//                }
//                else // !course.borders[i].isTotalRequirements
//                {
//                    for (int j = 0; j < currentSong + 1; j++)
//                    {
//                        switch (course.Borders[i].BorderType)
//                        {
//                            case BorderType.Goods:
//                                var current = songResults[j].goods;
//                                var requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Goods: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Goods: requirement = " + requirement);
//                                }
//                                if (current < requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.Oks:
//                                current = songResults[j].oks;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Oks: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Oks: requirement = " + requirement);
//                                }
//                                if (current >= requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.Bads:
//                                current = songResults[j].bads;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Bads: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Bads: requirement = " + requirement);
//                                }
//                                if (current >= requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.Combo:
//                                current = songResults[j].songCombo;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Combo: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Combo: requirement = " + requirement);
//                                }
//                                if (current < requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.Drumroll:
//                                current = songResults[j].renda;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Drumroll: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Drumroll: requirement = " + requirement);
//                                }
//                                if (current < requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.Score:
//                                current = songResults[j].score;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Score: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.Score: requirement = " + requirement);
//                                }
//                                if (current < requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                            case BorderType.TotalHits:
//                                current = songResults[j].goods + songResults[j].oks + songResults[j].renda;
//                                requirement = course.Borders[i].RedReqs[j];
//                                if (isDebugLog)
//                                {
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.TotalHits: current = " + current);
//                                    Plugin.Log.LogInfo("HasFailed: BorderType.TotalHits: requirement = " + requirement);
//                                }
//                                if (current < requirement)
//                                {
//                                    return true;
//                                }
//                                break;
//                        }
//                    }
//                }
//            }
//            return false;
//        }
//    }

//    public class SongResult
//    {
//        public int soulGauge { get; set; }
//        public int score { get; set; }
//        public int goods { get; set; }
//        public int oks { get; set; }
//        public int bads { get; set; }
//        public int renda { get; set; }
//        public int songCombo { get; set; }
//        public int totalHits { get; set; }
//        internal int currentCombo { get; set; }
//        internal int[] tamashiiPoints { get; set; }
//        internal int tamashiiPointsMax { get; set; }
//        internal int shinuchiPoints { get; set; }

//        public SongResult()
//        {
//            soulGauge = 0;
//            score = 0;
//            goods = 0;
//            oks = 0;
//            bads = 0;
//            renda = 0;
//            songCombo = 0;
//            tamashiiPoints = new int[3] { 0, 0, 0 };
//            tamashiiPointsMax = 100;
//        }

//        public void AddCombo()
//        {
//            currentCombo++;
//            songCombo = Math.Max(songCombo, currentCombo);
//        }
//        public void ResetCombo()
//        {
//            currentCombo = 0;
//        }
//        public void AddScore(bool isGood)
//        {
//            if (isGood)
//            {
//                score += shinuchiPoints;
//            }
//            else
//            {
//                score += (shinuchiPoints / 2);
//            }
//        }

//        /// <param name="resultType">0 is Good, 1 is Ok, 2 is Bad.</param>
//        public void UpdateTamashii(int resultType)
//        {
//            if (resultType < 3)
//            {
//                soulGauge += tamashiiPoints[resultType];
//            }
//        }
//        public void SetConstPoints(int[] newTamashiiPoints, int newShinuchiPoints, int maxTamashiiPoints)
//        {
//            Plugin.Log.LogInfo("Setting Const Points:");
//            Plugin.Log.LogInfo("newTamashiiPoints[0]: " + newTamashiiPoints[0]);
//            Plugin.Log.LogInfo("newTamashiiPoints[1]: " + newTamashiiPoints[1]);
//            Plugin.Log.LogInfo("newTamashiiPoints[2]: " + newTamashiiPoints[2]);
//            Plugin.Log.LogInfo("newShinuchiPoints:    " + newShinuchiPoints);
//            Plugin.Log.LogInfo("maxTamashiiPoints:    " + maxTamashiiPoints);
//            tamashiiPoints = newTamashiiPoints;
//            shinuchiPoints = newShinuchiPoints;
//            tamashiiPointsMax = maxTamashiiPoints;
//        }
//        public bool HasSetConstPoints()
//        {
//            return (tamashiiPoints[0] != 0 && tamashiiPoints[1] != 0 && tamashiiPoints[2] != 0 && tamashiiPointsMax != 0 && shinuchiPoints != 0);
//        }
//        public int GetSoulGaugePercentage()
//        {
//            //return 1;
//            soulGauge = Math.Min(soulGauge, tamashiiPointsMax);
//            return Math.Min((soulGauge / tamashiiPointsMax) * 100, 100);
//        }
//    }

//    public class DaniSeriesData
//    {
//        public List<DaniData> courseData { get; set; }
//        public string seriesTitle { get; set; }
//        public string seriesId { get; set; }
//        public bool isActiveDan { get; set; }
//        public int order { get; set; }

//        public DaniSeriesData()
//        {
//            courseData = new List<DaniData>();
//        }
//    }

//    internal class DaniDojoHighScore
//    {

//    }
//}
