using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Format001
{
    public class PrmSampleSize
    {
        public int Nb { get; set; } = 1;
        public int waferDiameter_mm { get; set; } = 200;

        public PrmSampleSize(int p_waferDiameter_mm)
        {
            Nb = 1;
            waferDiameter_mm = p_waferDiameter_mm;
        }

        public PrmSampleSize(int p_waferDiameter_mm, int p_Nb)
        {
            Nb = p_Nb;
            waferDiameter_mm = p_waferDiameter_mm;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", Nb, waferDiameter_mm);
        }
    }

    public class PrmSetupId
    {
        public String Name { get; set; } = " ";
        public DateTime? Date { get; set; } = null;

        public PrmSetupId(string p_Name, DateTime? p_Date)
        {
            Name = p_Name;
            Date = p_Date;
        }

        public override string ToString()
        {
            String s = "\"" + Name + "\"";
            if (Date != null)
            {
                s += " ";
                s += Date.Value.ToString(KlarfFile.fmtDatetime, System.Globalization.CultureInfo.InvariantCulture);
            }
            return s;
        }
    }

    public class PrmSampleOrientationMarkType
    {
        public enum somtType
        {
            NOTCH = 0,
            FLAT = 1,
            DFLAT = 2,
        }
        public somtType Value { get; set; } = somtType.NOTCH;

        public PrmSampleOrientationMarkType(somtType type)
        {
            Value = type;
        }

        public PrmSampleOrientationMarkType(int p_nomtType)
        {
            if (Enum.IsDefined(typeof(somtType), p_nomtType))
                Value = (somtType)p_nomtType;
            else
                throw new Exception("PrmSampleOrientationMarkType : Unknown Orientation Mark Type <" + p_nomtType + ">");
        }

        public PrmSampleOrientationMarkType(string p_somtType)
        {
            if (Enum.IsDefined(typeof(somtType), p_somtType.ToUpper()))
                Value = (somtType)Enum.Parse(typeof(somtType), p_somtType);
            else
                throw new Exception("PrmSampleOrientationMarkType : Unknown Orientation Mark Type <" + p_somtType + ">");
        }

        public override string ToString()
        {
            return Value.ToString("G");
        }
    }

    public class PrmOrientationMarkValue
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum omvType
        {
            DOWN = 0,
            UP = 1,
            LEFT = 2,
            RIGHT = 3,
        }
        public omvType Value { get; set; } = omvType.DOWN;

        public PrmOrientationMarkValue(int p_nType)
        {
            if (Enum.IsDefined(typeof(omvType), p_nType))
                Value = (omvType)p_nType;
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value <" + p_nType + ">");
        }

        public PrmOrientationMarkValue(string p_sType)
        {
            if (Enum.IsDefined(typeof(omvType), p_sType.ToUpper()))
                Value = (omvType)Enum.Parse(typeof(omvType), p_sType);
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value  <" + p_sType + ">");
        }

        public override string ToString()
        {
            return Value.ToString("G");
        }
    }

    public class PrmOrientationInstruction
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum oiType
        {
            FRONT = 0,
            BACK = 1,
            BEVEL = 2
        }
        public oiType Value { get; set; } = oiType.FRONT;

        public PrmOrientationInstruction(int p_nType)
        {
            if (Enum.IsDefined(typeof(oiType), p_nType))
                Value = (oiType)p_nType;
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value <" + p_nType + ">");
        }

        public PrmOrientationInstruction(string p_sType)
        {
            if (Enum.IsDefined(typeof(oiType), p_sType.ToUpper()))
                Value = (oiType)Enum.Parse(typeof(oiType), p_sType);
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value  <" + p_sType + ">");
        }

        public override string ToString()
        {
            return Value.ToString("G");
        }
    }

    public class PrmPtFloat
    {
        private string str_specifier = "E10";
        public float x { get; set; } = 0.0f;
        public float y { get; set; } = 0.0f;

        public PrmPtFloat(float p_x, float p_y)
        {
            x = p_x;
            y = p_y;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", x.ToString(str_specifier), y.ToString(str_specifier));
        }
    }


    public class PrmYesNo
    {
        public bool Value = false; // YES if true or !=0
        public PrmYesNo(bool bYes)
        {
            Value = bYes;
        }

        public static explicit operator PrmYesNo(bool b)  // explicit bool to PrmYesNo conversion operator
        {
            PrmYesNo prm = new PrmYesNo(b);  // explicit conversion
            return prm;
        }
        public static implicit operator bool(PrmYesNo prm)  // implicit PrmYesNo to bool conversion operator
        {
            return prm.Value;  // implicit conversion
        }

        public static explicit operator PrmYesNo(int n)  // explicit bool to PrmYesNo conversion operator
        {
            PrmYesNo prm = new PrmYesNo(n != 0);  // explicit conversion
            return prm;
        }
        public static implicit operator int(PrmYesNo prm)  // implicit PrmYesNo to bool conversion operator
        {
            return prm.Value ? 1 : 0;  // implicit conversion
        }

        public static explicit operator PrmYesNo(string s)  // explicit bool to PrmYesNo conversion operator
        {
            PrmYesNo prm = new PrmYesNo((0 == String.Compare(s, "YES", true)));  // explicit conversion
            return prm;
        }
        public static implicit operator string(PrmYesNo prm)  // implicit PrmYesNo to bool conversion operator
        {
            return prm.ToString(); ;  // implicit conversion
        }

        public override string ToString()
        {
            return Value ? "Yes" : "No";
        }
    }

    public class PrmSampleTestPlan
    {
        private List<KeyValuePair<int, int>> Plan = new List<KeyValuePair<int, int>>();

        public int NbDies { get { return Plan.Count; } }
        public List<KeyValuePair<int, int>> PlanList { get { return Plan; } }

        public PrmSampleTestPlan()
        {

        }

        public PrmSampleTestPlan(int Nx, int Ny)
        {
            //default constructor  for full wafer -- not die to die
            Add(Nx, Ny);
        }

        public PrmSampleTestPlan(List<KeyValuePair<int, int>> ListDieIndexes)
        {
            Plan = ListDieIndexes;
        }

        public void Add(int nDieIndexX, int nDieIndexY)
        {
            Plan.Add(new KeyValuePair<int, int>(nDieIndexX, nDieIndexY));
        }

        public bool TryAdd(int nDieIndexX, int nDieIndexY)
        {
            bool bAdded = false;
            try
            {
                Plan.First(x => (x.Key == nDieIndexX) && (x.Value == nDieIndexY));
            }
            catch (InvalidOperationException)
            {
                Plan.Add(new KeyValuePair<int, int>(nDieIndexX, nDieIndexY));
                bAdded = true;
            }
            return bAdded;
        }

        public void ClearPlan()
        {
            Plan.Clear();
        }

        private static int CompareIdx(KeyValuePair<int, int> a, KeyValuePair<int, int> b)
        {
            // du die le plus en haut à gauche au plus en bas à droite
            int n1 = b.Value.CompareTo(a.Value);
            if (n1 == 0)
                return a.Key.CompareTo(b.Key);

            return n1;
        }

        public void SortIndxList()
        {
            Plan.Sort(CompareIdx);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder((NbDies + 1) * 8);
            sb.AppendLine(NbDies.ToString());
            if (NbDies > 0)
            {
                for (int i = 0; i < (NbDies - 1); i++)
                {
                    sb.AppendFormat("{0} {1}\n", Plan[i].Key.ToString(), Plan[i].Value.ToString());
                }
                sb.AppendFormat("{0} {1}", Plan[NbDies - 1].Key.ToString(), Plan[NbDies - 1].Value.ToString());
            }
            return sb.ToString();
        }
    }

    public class PrmImageData
    {
        public List<int> List { get; set; }
        public PrmImageData()
        {
            List = new List<int>();
        }

        public override string ToString()
        {
            String s = List.Count.ToString();
            foreach (int nid in List)
            {
                s += " ";
                s += nid.ToString();
                s += " 0";
            }
            return s;
        }
    }

    public class PrmSicPlChannelId
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum SicPlChannelId
        {
            PlChannelID_0 = 0,
            PlChannelID_1 = 1,
            PlChannelID_2 = 2,
            PlChannelID_3 = 3
        }
        public SicPlChannelId Value { get; set; } = SicPlChannelId.PlChannelID_0;

        public PrmSicPlChannelId(int p_ChanneId)
        {
            if (Enum.IsDefined(typeof(SicPlChannelId), p_ChanneId))
                Value = (SicPlChannelId)p_ChanneId;
            else
                throw new Exception("PrmSicPlChannelId : Unknown channel Id Value <" + p_ChanneId + ">");
        }

        public PrmSicPlChannelId(string p_ChanneId)
        {
            if (Enum.IsDefined(typeof(SicPlChannelId), p_ChanneId.ToUpper()))
                Value = (SicPlChannelId)Enum.Parse(typeof(SicPlChannelId), p_ChanneId);
            else
                throw new Exception("PrmOrientationMarkValue : Unknown channel Id Value <" + p_ChanneId + ">");
        }

        public override string ToString()
        {
            return Value.ToString("G");
        }
    }
}
