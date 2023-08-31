using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Data
{
    public class DaniSaveData
    {
        public List<SaveCourse> Courses { get; set; }

        public DaniSaveData()
        {
            Courses = new List<SaveCourse>();
        }
    }

    public enum DaniRank
    {
        None,
        RedClear,
        GoldClear,
    }

    public enum DaniCombo
    {
        None,
        Silver,
        Gold,
        Rainbow,
    }

    public struct DaniRankCombo : IEquatable<DaniRankCombo>, IComparable<DaniRankCombo>
    {
        public DaniRank Rank;
        public DaniCombo Combo;
        public DaniRankCombo(DaniRank newRank, DaniCombo newCombo)
        {
            Rank = newRank;
            Combo = newCombo;
        }

        public bool Equals(DaniRankCombo other)
        {
            return other.Rank == Rank && other.Combo == Combo;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Rank.GetHashCode();
                hash = hash * 23 + Combo.GetHashCode();
                return hash;
            }
        }

        public int CompareTo(DaniRankCombo other)
        {
            if (this > other)
            {
                return 1;
            }
            else if (this < other)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public static bool operator ==(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            return rank1.Equals(rank2);
        }

        public static bool operator !=(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            return !(rank1 == rank2);
        }

        public static bool operator >(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            if (rank1.Rank == rank2.Rank)
            {
                return rank1.Combo > rank2.Combo;
            }
            return rank1.Rank > rank2.Rank;
        }

        public static bool operator <(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            if (rank1.Rank == rank2.Rank)
            {
                return rank1.Combo < rank2.Combo;
            }
            return rank1.Rank < rank2.Rank;
        }

        public static bool operator >=(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            return rank1 > rank2 || rank1 == rank2;
        }

        public static bool operator <=(DaniRankCombo rank1, DaniRankCombo rank2)
        {
            return rank1 < rank2 || rank1 == rank2;
        }
    }

    public class SaveCourse
    {
        public uint Hash { get; set; }
        public DaniCourse Course { get; set; }
        public DaniRankCombo RankCombo { get; set; }
        public List<PlayData> PlayData { get; set; }
        public int SongReached { get; set; } // 1 indexed, where 0 means no song played

        public SaveCourse()
        {
            SetDefaultData();
        }

        public SaveCourse(uint hash)
        {
            SetDefaultData();
            Hash = hash;
        }

        private void SetDefaultData()
        {
            PlayData = new List<PlayData>();
            SongReached = 0;
            RankCombo = new DaniRankCombo(DaniRank.None, DaniCombo.None);
        }
    }

    public struct PlayModifiers
    {
        public DataConst.SpeedTypes Speed { get; set; }
        public DataConst.OptionOnOff Vanish { get; set; } // Dron
        public DataConst.OptionOnOff Inverse { get; set; } // Reverse
        public DataConst.RandomLevel Random { get; set; }
        public DataConst.SpecialTypes Special { get; set; }
        public PlayModifiers(DataConst.SpeedTypes speed, DataConst.OptionOnOff vanish, DataConst.OptionOnOff inverse, DataConst.RandomLevel random, DataConst.SpecialTypes special)
        {
            Speed = speed;
            Vanish = vanish;
            Inverse = inverse;
            Random = random;
            Special = special;
        }
    }
}
