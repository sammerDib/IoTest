using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitySC.Shared.Format._001
{
    public class PrmSampleSize
    {
        public int Nb { get; set; } = 1;
        public int WaferDiameter_mm { get; set; } = 200;

        public PrmSampleSize(int p_waferDiameter_mm)
        {
            Nb = 1;
            WaferDiameter_mm = p_waferDiameter_mm;
        }

        public PrmSampleSize(int p_waferDiameter_mm, int p_Nb)
        {
            Nb = p_Nb;
            WaferDiameter_mm = p_waferDiameter_mm;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Nb, WaferDiameter_mm);
        }
    }

    public class PrmSetupId
    {
        public string Name { get; set; } = " ";
        public DateTime? Date { get; set; } = null;

        public PrmSetupId(string p_Name, DateTime? p_Date)
        {
            Name = p_Name;
            Date = p_Date;
        }

        public override string ToString()
        {
            string s = "\"" + Name + "\"";
            if (Date != null)
            {
                s += " ";
                s += Date.Value.ToString(KlarfFile.FmtDatetime, System.Globalization.CultureInfo.InvariantCulture);
            }
            return s;
        }
    }

    public class PrmSampleOrientationMarkType
    {
        public enum SomtType
        {
            NOTCH = 0,
            FLAT = 1,
            DFLAT = 2,
        }

        public SomtType Value { get; set; } = SomtType.NOTCH;

        public PrmSampleOrientationMarkType(SomtType type)
        {
            Value = type;
        }

        public PrmSampleOrientationMarkType(int p_nomtType)
        {
            if (Enum.IsDefined(typeof(SomtType), p_nomtType))
                Value = (SomtType)p_nomtType;
            else
                throw new Exception("PrmSampleOrientationMarkType : Unknown Orientation Mark Type <" + p_nomtType + ">");
        }

        public PrmSampleOrientationMarkType(string p_somtType)
        {
            if (Enum.IsDefined(typeof(SomtType), p_somtType.ToUpper()))
                Value = (SomtType)Enum.Parse(typeof(SomtType), p_somtType.ToUpper());
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
        public enum OmvType
        {
            DOWN = 0,
            UP = 1,
            LEFT = 2,
            RIGHT = 3,
        }

        public OmvType Value { get; set; } = OmvType.DOWN;

        public PrmOrientationMarkValue(int p_nType)
        {
            if (Enum.IsDefined(typeof(OmvType), p_nType))
                Value = (OmvType)p_nType;
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value <" + p_nType + ">");
        }

        public PrmOrientationMarkValue(string p_sType)
        {
            if (Enum.IsDefined(typeof(OmvType), p_sType.ToUpper()))
                Value = (OmvType)Enum.Parse(typeof(OmvType), p_sType.ToUpper());
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
        public enum OiType
        {
            FRONT = 0,
            BACK = 1,
            BEVEL = 2
        }

        public OiType Value { get; set; } = OiType.FRONT;

        public PrmOrientationInstruction(int p_nType)
        {
            if (Enum.IsDefined(typeof(OiType), p_nType))
                Value = (OiType)p_nType;
            else
                throw new Exception("PrmOrientationMarkValue : Unknown Orientation Mark Value <" + p_nType + ">");
        }

        public PrmOrientationInstruction(string p_sType)
        {
            if (Enum.IsDefined(typeof(OiType), p_sType.ToUpper()))
                Value = (OiType)Enum.Parse(typeof(OiType), p_sType.ToUpper());
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
        private readonly string _str_specifier = "E10";
        public float X { get; set; } = 0.0f;
        public float Y { get; set; } = 0.0f;

        public PrmPtFloat(float p_x, float p_y)
        {
            X = p_x;
            Y = p_y;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", X.ToString(_str_specifier), Y.ToString(_str_specifier));
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
            var prm = new PrmYesNo(b);  // explicit conversion
            return prm;
        }

        public static implicit operator bool(PrmYesNo prm)  // implicit PrmYesNo to bool conversion operator
        {
            return prm.Value;  // implicit conversion
        }

        public static explicit operator PrmYesNo(int n)  // explicit bool to PrmYesNo conversion operator
        {
            var prm = new PrmYesNo(n != 0);  // explicit conversion
            return prm;
        }

        public static implicit operator int(PrmYesNo prm)  // implicit PrmYesNo to bool conversion operator
        {
            return prm.Value ? 1 : 0;  // implicit conversion
        }

        public static explicit operator PrmYesNo(string s)  // explicit bool to PrmYesNo conversion operator
        {
            var prm = new PrmYesNo(0 == string.Compare(s, "YES", true));  // explicit conversion
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
        private readonly List<KeyValuePair<int, int>> _plan = new List<KeyValuePair<int, int>>();

        public int NbDies { get { return _plan.Count; } }
        public List<KeyValuePair<int, int>> PlanList { get { return _plan; } }

        public PrmSampleTestPlan()
        {
        }

        public PrmSampleTestPlan(int nNx, int nNy)
        {
            //default constructor  for full wafer -- not die to die
            Add(nNx, nNy);
        }

        public PrmSampleTestPlan(List<KeyValuePair<int, int>> p_ListDieIndexes)
        {
            _plan = p_ListDieIndexes;
        }

        public void Add(int nDieIndexX, int nDieIndexY)
        {
            _plan.Add(new KeyValuePair<int, int>(nDieIndexX, nDieIndexY));
        }

        public bool TryAdd(int nDieIndexX, int nDieIndexY)
        {
            bool bAdded = false;
            try
            {
                _plan.First(x => (x.Key == nDieIndexX) && (x.Value == nDieIndexY));
            }
            catch (InvalidOperationException)
            {
                _plan.Add(new KeyValuePair<int, int>(nDieIndexX, nDieIndexY));
                bAdded = true;
            }
            return bAdded;
        }

        public void ClearPlan()
        {
            _plan.Clear();
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
            _plan.Sort(CompareIdx);
        }

        public override string ToString()
        {
            var sb = new StringBuilder((NbDies + 1) * 8);
            sb.AppendLine(NbDies.ToString());
            if (NbDies > 0)
            {
                for (int i = 0; i < (NbDies - 1); i++)
                {
                    sb.AppendFormat("{0} {1}\n", _plan[i].Key.ToString(), _plan[i].Value.ToString());
                }
                sb.AppendFormat("{0} {1}", _plan[NbDies - 1].Key.ToString(), _plan[NbDies - 1].Value.ToString());
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
            string s = List.Count.ToString();
            foreach (int nid in List)
            {
                s += " ";
                s += nid.ToString();
                s += " 0";
            }
            return s;
        }
    }
}