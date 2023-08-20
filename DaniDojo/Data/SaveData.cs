using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaniDojo.Data
{
    internal class SaveData
    {


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

    internal struct DaniRankCombo
    {
        DaniRank rank;
        DaniCombo combo;
        public DaniRankCombo(DaniRank newRank, DaniCombo newCombo)
        {
            rank = newRank;
            combo = newCombo;
        }
    }
}
