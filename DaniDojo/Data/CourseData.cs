using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Data
{
    public class DaniSeries
    {
        public string Id { get; set; }
        // Should I keep this as just title
        // Or should I separate it to JP and Eng title?
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public List<DaniCourse> Courses { get; set; }

        public DaniSeries()
        {
            Courses = new List<DaniCourse>();
        }
    }

    public enum CourseBackground
    {
        None,
        Tan,
        Wood,
        Blue,
        Red,
        Silver,
        Gold,
        Gaiden,
        Sousaku,
    }

    // I'm not sure if I'll end up using this, but I'd like to keep it for now just in case.
    public enum DaniCourseLevel
    {
        None,
        kyuuFirst,
        kyuu10,
        kyuu9,
        kyuu8,
        kyuu7,
        kyuu6,
        kyuu5,
        kyuu4,
        kyuu3,
        kyuu2,
        kyuu1,
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
        kuroto,
        meijin,
        chojin,
        tatsujin,
        gaiden,
        sousaku,
    }

    public class DaniCourse
    {
        public DaniSeries Parent { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string JpTitle { get; set; }
        public string EngTitle { get; set; }
        public int Order { get; set; }
        public bool IsLocked { get; set; }
        public CourseBackground Background { get; set; }
        public DaniCourseLevel CourseLevel { get; set; }
        public List<DaniSongData> Songs { get; set; }
        public List<DaniBorder> Borders { get; set; }
        public uint Hash { get; set; }

        public DaniCourse()
        {
            Songs = new List<DaniSongData>();
            Borders = new List<DaniBorder>();

            Id = string.Empty;
            Title = string.Empty;

            Order = 0;

            IsLocked = false;
            Hash = 0;

            CourseLevel = DaniCourseLevel.None;
        }
    }

    public class DaniSongData
    {
        public string SongId { get; set; } = string.Empty;
        public string TitleEng { get; set; } = string.Empty;
        public string TitleJp { get; set; } = string.Empty;
        public EnsoData.EnsoLevelType Level { get; set; } = EnsoData.EnsoLevelType.Mania;
        public bool IsHidden { get; set; } = false;
    }

    public enum BorderType
    {
        SoulGauge = 1,
        Goods,
        Oks,
        Bads,
        Combo,
        Drumroll,
        Score,
        TotalHits,
    }

    public class DaniBorder
    {
        public BorderType BorderType { get; set; }
        public bool IsTotal { get; set; }
        public List<int> RedReqs { get; set; }
        public List<int> GoldReqs { get; set; }

        public DaniBorder()
        {
            RedReqs = new List<int>();
            GoldReqs = new List<int>();
        }
    }


}
