using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;

namespace Format001
{
    public class PrmDefectType
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }  // The data type of the characteristic (ex: double, Rectangle...)

        public delegate object ConvertDelegate(string s);
        public ConvertDelegate _DlgConvert;

        public delegate String ToStringDelegate(object s);
        public ToStringDelegate _DlgToString;

        public PrmDefectType(Type type, string name, ConvertDelegate Convertdlg = null, ToStringDelegate toStringdlg = null)
        {
            Type = type;
            Name = name;
            _DlgConvert = Convertdlg;
            _DlgToString = toStringdlg;
        }

        public object ConvertFromString(string sValue)
        {
            if (_DlgConvert != null)
                return _DlgConvert(sValue);
            return null;
        }

        public String ConvertToString(object sValue)
        {
            if (_DlgToString != null)
                return _DlgToString(sValue);
            return sValue.ToString();
        }

        public override string ToString()
        {
            return Name;
        }

        static public object ToInt(string s)
        {
            return (object)Convert.ToInt32(s, CultureInfo.InvariantCulture.NumberFormat);
        }
        static public object ToDouble(string s)
        {
            return (object)Convert.ToDouble(s, CultureInfo.InvariantCulture.NumberFormat);
        }
        static public object ToIntFromDbl(string s)
        {
            return (object)Convert.ToInt32(Math.Floor(Convert.ToDouble(s, CultureInfo.InvariantCulture.NumberFormat)), CultureInfo.InvariantCulture.NumberFormat);
        }
        static public object ToImageData(string s)
        {
            // to do rti : not yet implemented
            return (object)new PrmImageData();
        }

        static public String DoubleToString_2(object obj)
        {
            double dval = (double)obj;
            return (String)dval.ToString("#0.00");
        }
        static public String DoubleToString_3(object obj)
        {
            double dval = (double)obj;
            return (String)dval.ToString("#0.000");
        }

    }

    public sealed class RegisteredDefectType
    {
        private static RegisteredDefectType instance;
        private static object syncRoot = new Object();
        public static RegisteredDefectType Singleton
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new RegisteredDefectType();
                    }
                    return instance;
                }
            }
        }

        // private readonly Dictionary<String, Type> _Reg = new Dictionary<String, Type>();
        private readonly Dictionary<String, KeyValuePair<Type, PrmDefectType.ConvertDelegate>> _Reg = new Dictionary<String, KeyValuePair<Type, PrmDefectType.ConvertDelegate>>();
        private readonly Dictionary<String, PrmDefectType.ToStringDelegate> _RegtoStr = new Dictionary<String, PrmDefectType.ToStringDelegate>();

        public RegisteredDefectType()
        {
            _Reg.Add("DEFECTID", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("XREL", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));   // en µm
            _Reg.Add("YREL", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));  // en µm
            _Reg.Add("XINDEX", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));  // die index X
            _Reg.Add("YINDEX", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt))); // die index Y 
            _Reg.Add("XSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _Reg.Add("YSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _Reg.Add("DEFECTAREA", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm²
            _Reg.Add("DSIZE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble))); // en µm
            _Reg.Add("CLASSNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("ROUGHBINNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("TEST", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("CLUSTERNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("IMAGECOUNT", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("IMAGELIST", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(PrmImageData), new PrmDefectType.ConvertDelegate(PrmDefectType.ToImageData)));
            _Reg.Add("GRADE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));
            _Reg.Add("ZABS", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(double), new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble)));
            _Reg.Add("ZONEID", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("FINEBINNUMBER", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));
            _Reg.Add("REVIEWSAMPLE", new KeyValuePair<Type, PrmDefectType.ConvertDelegate>(typeof(int), new PrmDefectType.ConvertDelegate(PrmDefectType.ToInt)));

            _RegtoStr.Add("XREL", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _RegtoStr.Add("YREL", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            _RegtoStr.Add("XSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _RegtoStr.Add("YSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _RegtoStr.Add("DEFECTAREA", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _RegtoStr.Add("DSIZE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _RegtoStr.Add("GRADE", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
            _RegtoStr.Add("ZABS", new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_3));
        }

        public Type GetType(String p_sRegisteredName)
        {
            return _Reg[p_sRegisteredName].Key;
        }

        public PrmDefectType.ConvertDelegate GetConverter(String p_sRegisteredName)
        {
            return _Reg[p_sRegisteredName].Value;
        }

        public PrmDefectType.ToStringDelegate GetToString(String p_sRegisteredName)
        {
            if (_RegtoStr.ContainsKey(p_sRegisteredName))
                return _RegtoStr[p_sRegisteredName];
            else
                return null;
        }
    }

    public class PrmDefectTypeList
    {
        // La liste de toutes les type de Defect recordable dans le klarf
        private List<PrmDefectType> _list = new List<PrmDefectType>();
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

        public void Add(String p_RegisteredName)
        {
            Add(RegisteredDefectType.Singleton.GetType(p_RegisteredName),
                p_RegisteredName,
                RegisteredDefectType.Singleton.GetConverter(p_RegisteredName),
                RegisteredDefectType.Singleton.GetToString(p_RegisteredName));
        }

        public void Add(String[] p_RegisteredNames)
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
        private Dictionary<string, object> _Dic = new Dictionary<string, object>();
        public double Size { get; set; }
        public RectangleF SurroundingRectangleMicron { get; set; }


        public PrmDefect(PrmDefectTypeList deflist)
        {
            _deflist = deflist;
        }

        public void SetFromString(String sLineString)
        {
            String[] lSeparator = new String[] { " " };
            String[] lData = sLineString.Trim().Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < lData.Length; i++)
            {
                PrmDefectType dtyp = _deflist.Parse(i);
                if (dtyp == null)
                    continue;

                if (dtyp.GetType() == typeof(PrmImageData))
                {
                    // cas particulier on consomme les data jusqu'à la fin de la ligne
                    StringBuilder sb = new StringBuilder();
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

        public void Set(String sDefectName, object Value)
        {
            PrmDefectType typ = _deflist.Parse(sDefectName);
            if (typ == null)
                throw new Exception("Unknown defect type name added");
            Set(typ, Value);
        }

        public void Set(int nColIdx, object Value)
        {
            PrmDefectType typ = _deflist.Parse(nColIdx);
            if (typ == null)
                throw new Exception("Unknown defect type columns index added");
            Set(typ, Value);
        }


        private void Set(PrmDefectType typ, object Value)
        {
            if (Value.GetType() != typ.Type)
                throw new Exception("Wrong defect type added");
            _Dic[typ.Name] = Value;
        }

        public object Get(String sDefectName)
        {
            PrmDefectType typ = _deflist.Parse(sDefectName);
            if (typ == null)
                throw new Exception("Unknown defect type name to get");
            return Get(typ);
        }

        public object Get(int nColIdx)
        {
            PrmDefectType typ = _deflist.Parse(nColIdx);
            if (typ == null)
                throw new Exception("Unknown defect type Idx to get");
            return Get(typ);
        }

        private object Get(PrmDefectType typ)
        {
            object obj;
            if (!_Dic.TryGetValue(typ.Name, out obj))
                return null;
            return obj;
        }

        public override string ToString()
        {
            String sDef = " ";
            foreach (PrmDefectType dtyp in _deflist.List)
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
        private static RegisteredSummaryType instance;
        private static object syncRoot = new Object();
        public static RegisteredSummaryType Singleton
        {
            get
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new RegisteredSummaryType();
                    }
                    return instance;
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
