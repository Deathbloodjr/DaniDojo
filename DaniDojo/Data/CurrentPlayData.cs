using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Data
{
    internal class CurrentPlayData
    {
        public uint Hash { get; set; }
        public int CurrentSongIndex { get; set; }
        public int CurrentCombo { get; set; }
        public int CurrentSongCombo { get; set; }
        public int CurrentSoulGauge { get; set; } // Not 0-100, actual game numbers (whatever they may be)
        public PlayData PlayData { get; set; }
        public CurrentPlayData(DaniCourse course)
        {
            Hash = course.Hash;
            CurrentSongIndex = 0;
            CurrentCombo = 0;
            CurrentSoulGauge = 0;
            PlayData = new PlayData();
            // So you can just do PlayData.SongPlayData[2] right away, without needing to worry about it being created or added yet.
            for (int i = 0; i < course.Songs.Count; i++)
            {
                SongPlayData songPlay = new SongPlayData();
                PlayData.SongPlayData.Add(songPlay);
            }
        }
    }

    public class PlayData
    {
        public DaniRankCombo RankCombo { get; set; }
        public DateTime PlayDateTime { get; set; }
        public PlayModifiers Modifiers { get; set; }
        public int MaxCombo { get; set; }
        public int SoulGauge { get; set; } // 0 - 100
        public int SongReached { get; set; } // 1 indexed, where 0 means no song played
        public List<SongPlayData> SongPlayData { get; set; }
        public PlayData()
        {
            RankCombo = new DaniRankCombo(DaniRank.None, DaniCombo.None);
            PlayDateTime = DateTime.Now;
            Modifiers = new PlayModifiers();
            MaxCombo = 0;
            SoulGauge = 0;
            SongReached = 0;
            SongPlayData = new List<SongPlayData>();
        }
    }

    public class SongPlayData
    {
        public int Score { get; set; }
        public int Goods { get; set; }
        public int Oks { get; set; }
        public int Bads { get; set; }
        public int Drumroll { get; set; }
        public int Combo { get; set; }
        public SongPlayData()
        {
            Score = 0;
            Goods = 0;
            Oks = 0;
            Bads = 0;
            Drumroll = 0;
            Combo = 0;
        }
    }
}
