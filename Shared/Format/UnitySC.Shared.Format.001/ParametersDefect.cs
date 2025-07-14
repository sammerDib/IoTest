using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace UnitySC.Shared.Format._001
{
    public class PrmDefectType
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }  // The data type of the characteristic (ex: double, Rectangle...)

        public delegate object ConvertDelegate(string s);

        public ConvertDelegate DlgConvert;

        public delegate string ToStringDelegate(object s);

        public ToStringDelegate DlgToString;

        public PrmDefectType(Type type, string name, ConvertDelegate convertdlg = null, ToStringDelegate toStringdlg = null)
        {
            Type = type;
            Name = name;
            DlgConvert = convertdlg;
            DlgToString = toStringdlg;
        }

        public object ConvertFromString(string sValue)
        {
            if (DlgConvert != null)
                return DlgConvert(sValue);
            return null;
        }

        public string ConvertToString(object sValue)
        {
            if (DlgToString != null)
                return DlgToString(sValue);
            return sValue.ToString();
        }

        public override string ToString()
        {
            return Name;
        }

        static public object ToInt(string s)
        {
            return Convert.ToInt32(s, CultureInfo.InvariantCulture.NumberFormat);
        }

        static public object ToDouble(string s)
        {
            return Convert.ToDouble(s, CultureInfo.InvariantCulture.NumberFormat);
        }

        static public object ToIntFromDbl(string s)
        {
            return Convert.ToInt32(Math.Floor(Convert.ToDouble(s, CultureInfo.InvariantCulture.NumberFormat)), CultureInfo.InvariantCulture.NumberFormat);
        }

        static public object ToImageData(string s)
        {
            if (string.IsNullOrEmpty(s))
                return new PrmImageData();

            var imgdata = new PrmImageData();
            try
            {
                string[] sItems = s.Split(new char[] { ' ' }, 16, StringSplitOptions.RemoveEmptyEntries);
                if (sItems.Length > 0)
                {
                    if (Int32.TryParse(sItems[0], out int nImgCnt))
                    {
                        for (int i = 0; i < nImgCnt; i++)
                        {
                            if (Int32.TryParse(sItems[2 * i + 1], out int nPageIndex))
                                imgdata.List.Add(nPageIndex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string smsg = ex.Message;
            }
            return imgdata;
        }

        static public string DoubleToString_2(object obj)
        {
            double dval = (double)obj;
            return dval.ToString("#0.00");
        }

        static public string DoubleToString_3(object obj)
        {
            double dval = (double)obj;
            return dval.ToString("#0.000");
        }
    }

    public sealed class RegisteredDefectType
    {
        private static RegisteredDefectType s_instance;
        private static readonly object s_syncRoot = new object();

        public static RegisteredDefectType Singleton
        {
            get
            {
                lock (s_syncRoot)
                {
                    if (s_instance == null)
                    {
                        s_instance = new RegisteredDefectType();
                    }
                    return s_instance;
                }
            }
        }

        // private readonly Dictionary<string, Type> _Reg = new Dictionary<string, Type>();
        private readonly Dictionary<string, KeyValuePair<Type, PrmDefectType.ConvertDelegate>> _reg = new Dictionary<string, KeyValuePair<Type, PrmDefectType.ConvertDelegate>>();

        private readonly Dictionary<string, PrmDefectType.ToStringDelegate> _regtoStr = new Dictionary<string, PrmDefectType.ToStringDelegate>();

        public RegisteredDefectType()
        {
            _reg.Add("DEFECTID", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("XREL", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));   // en µm
            _reg.Add("YREL", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));  // en µm
            _reg.Add("XINDEX", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));  // die index X
            _reg.Add("YINDEX", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt))); // die index Y
            _reg.Add("XSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _reg.Add("YSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _reg.Add("DEFECTAREA", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm²
            _reg.Add("DSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _reg.Add("CLASSNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("ROUGHBINNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("TEST", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("CLUSTERNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("IMAGECOUNT", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("IMAGELIST", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(PrmImageData), new PrmDefectType.ConvertDelegate(PrmDefectType.ToImageData)));
            _reg.Add("GRADE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));
            _reg.Add("ZABS", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));
            _reg.Add("ZONEID", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("FINEBINNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _reg.Add("REVIEWSAMPLE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));

            _regtoStr.Add("XREL", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("YREL", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("XSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("YSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("DEFECTAREA", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("DSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _regtoStr.Add("GRADE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _regtoStr.Add("ZABS", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
        }

        public Type GetType(string p_sRegisteredName)
        {
            return _reg[p_sRegisteredName].Key;
        }

        public PrmDefectType.ConvertDelegate GetConverter(string p_sRegisteredName)
        {
            return _reg[p_sRegisteredName].Value;
        }

        public PrmDefectType.ToStringDelegate GetToString(string p_sRegisteredName)
        {
            if (_regtoStr.ContainsKey(p_sRegisteredName))
                return _regtoStr[p_sRegisteredName];
            else
                return null;
        }
    }

    public class PrmDefectTypeList
    {
        // La liste de toutes les type de Defect recordable dans le klarf
        private readonly List<PrmDefectType> _list = new List<PrmDefectType>();

        public List<PrmDefectType> List { get { return _list; } }

        public PrmDefectTypeList()
        {
        }

        public void Add(Type type, string name, PrmDefectType.ConvertDelegate convdlg = null, PrmDefectType.ToStringDelegate tostrdlg = null)
        {
            if (Parse(name) == null)
                _list.Add(new PrmDefectType(type, name, convdlg, tostrdlg));
            else
                throw new Exception("PrmDefectTypeList try to add already existing Type");
        }

        public void Add(string p_RegisteredName)
        {
            Add(RegisteredDefectType.Singleton.GetType(p_RegisteredName),
                p_RegisteredName,
                RegisteredDefectType.Singleton.GetConverter(p_RegisteredName),
                RegisteredDefectType.Singleton.GetToString(p_RegisteredName));
        }

        public void Add(string[] p_RegisteredNames)
        {
            if (p_RegisteredNames != null)
            {
                foreach (string sRegName in p_RegisteredNames)
                    Add(sRegName);
            }
        }

        public PrmDefectType Parse(string str)
        {
            return _list.Find(x => x.Name == str);
        }

        public PrmDefectType Parse(int nIdx)
        {
            if ((nIdx < 0) || (nIdx >= _list.Count))
                return null;
            return _list[nIdx];
        }
    }

    public class PrmDefect
    {
        private readonly PrmDefectTypeList _deflist = null;
        private readonly Dictionary<string, object> _dic = new Dictionary<string, object>();
        public double Size { get; set; }
        public RectangleF SurroundingRectangleMicron { get; set; }

        // Add by YSI
        public PrmDefect()
        {
        }

        public PrmDefect(PrmDefectTypeList deflist)
        {
            _deflist = deflist;
        }

        public void SetFromString(string sLineString)
        {
            string[] lSeparator = new string[] { " " };
            string[] lData = sLineString.Trim().Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lData.Length; i++)
            {
                var dtyp = _deflist.Parse(i);
                if (dtyp == null)
                    continue;

                if (dtyp.Type == typeof(PrmImageData))
                {
                    // cas particulier on consomme les data jusqu'à la fin de la ligne
                    var sb = new StringBuilder();
                    for (int j = i; j < lData.Length - 1; j++)
                        sb.AppendFormat("{0} ", lData[j]); // whitespace between !!
                    sb.Append(lData[lData.Length - 1]);

                    object obj = dtyp.ConvertFromString(sb.ToString());
                    Set(i, obj);

                    i += lData.Length - i;
                }
                else
                {
                    object obj = dtyp.ConvertFromString(lData[i]);
                    Set(i, obj);
                }
            }
        }

        public void Set(string sDefectName, object p_Value)
        {
            var typ = _deflist.Parse(sDefectName);
            if (typ == null)
                throw new Exception("Unknown defect type name added");
            Set(typ, p_Value);
        }

        public void Set(int nColIdx, object p_Value)
        {
            var typ = _deflist.Parse(nColIdx);
            if (typ == null)
                throw new Exception("Unknown defect type columns index added");
            Set(typ, p_Value);
        }

        private void Set(PrmDefectType typ, object p_Value)
        {
            if (p_Value.GetType() != typ.Type)
                throw new Exception("Wrong defect type added");
            _dic[typ.Name] = p_Value;
        }

        public object Get(string sDefectName)
        {
            var typ = _deflist.Parse(sDefectName);
            if (typ == null)
                throw new Exception("Unknown defect type name to get");
            return Get(typ);
        }

        public object Get(int nColIdx)
        {
            var typ = _deflist.Parse(nColIdx);
            if (typ == null)
                throw new Exception("Unknown defect type Idx to get");
            return Get(typ);
        }

        private object Get(PrmDefectType typ)
        {
            if (!_dic.TryGetValue(typ.Name, out object obj))
                return null;
            return obj;
        }

        public Dictionary<string, string> GetFeaturesDico()
        {
            var featuresdic = new Dictionary<string, string>();
            foreach (var dtyp in _deflist.List)
            {
                object objdef = Get(dtyp.Name);
                if (objdef != null)
                {
                    featuresdic.Add(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dtyp.Name.ToLower()), dtyp.ConvertToString(objdef));
                }
            }
            return featuresdic;
        }

        public override string ToString()
        {
            string sDef = " ";
            foreach (var dtyp in _deflist.List)
            {
                object objdef = Get(dtyp.Name);
                if (objdef != null)
                    sDef += dtyp.ConvertToString(objdef);
                else
                    sDef += "0";
                sDef += " ";
            }
            return sDef;
        }
    }

    public sealed class RegisteredSummaryType
    {
        private static RegisteredSummaryType s_instance;
        private static readonly object s_syncRoot = new object();

        public static RegisteredSummaryType Singleton
        {
            get
            {
                lock (s_syncRoot)
                {
                    if (s_instance == null)
                    {
                        s_instance = new RegisteredSummaryType();
                    }
                    return s_instance;
                }
            }
        }
    }

    ///pour la zone ID
    // calcul de la "zone" d'inspection :
    // Top => 8
    // TopBevel => 16
    // Apex => 32
    // BottomBevel => 64
    // Bottom => 128
    // Si defaut vu sur plusieurs sensor, ajout des valeurs chaque "zones" : ATTENTION, Pour le moment, impossible, pas l'info disponible
    // si on veut savoir si un defaut a des copains ailleurs, seul le CMC pourra tenir l'info, et la transmettre aux blobs.

    // GRADE : position polaire => angle
    // ZABS : position polaire => rayon
}
