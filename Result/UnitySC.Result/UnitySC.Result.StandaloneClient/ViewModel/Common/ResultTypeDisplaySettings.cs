using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Result.StandaloneClient.ViewModel.Common
{
    [Serializable]
    public class ResultTypeDisplaySettings
    {
        [XmlArray("DicoSettings")]
        [XmlArrayItem("Setting")]
        public List<ResTypeDicoSetting> Settings = new List<ResTypeDicoSetting>();

        public ResultTypeDisplaySettings()
        { }

        public ResultTypeDisplaySettings(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
               XmlSerializer xs = new XmlSerializer(typeof(ResultTypeDisplaySettings));
                using (var rd = new StreamReader(xmlPath))
                {
                  var xmlcfg = xs.Deserialize(rd) as ResultTypeDisplaySettings;
                  Settings.AddRange(xmlcfg.Settings);
                }            
            }
        }

        //// for xml creation --- 
        //public void WriteSTANDARD_DisplaySettings(string xmlPath)
        //{
        //    Settings.Clear();
        //    foreach (var rtyp in (ResultType[])Enum.GetValues(typeof(ResultType)))
        //    {
        //        var sbrush = rtyp.GetColor();
        //        Settings.Add(new ResTypeDicoSetting(rtyp, rtyp.GetDisplayName(), System.Drawing.Color.FromArgb(sbrush.Color.R, sbrush.Color.G, sbrush.Color.B)));
        //    }
        //    Settings = Settings.OrderBy(x => x.ResType.ToString()).ToList();
        //    try
        //    {
        //        System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ViewModel.Common.ResultTypeDisplaySettings));
        //        using (System.IO.StreamWriter wr = new System.IO.StreamWriter(xmlPath))
        //        {
        //            xs.Serialize(wr, this /*set*/);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        string smsg = ex.Message;
        //    }
        //}


    }

    [Serializable]
    public class ResTypeDicoSetting
    {
        [XmlAttribute("RType")]
        public ResultType ResType;
        [XmlAttribute("Label")]
        public string Lbl;
        [XmlAttribute("R")]
        public int R;
        [XmlAttribute("G")]
        public int G;
        [XmlAttribute("B")]
        public int B;

        [XmlIgnore]
        public Color Clr
        {
            get { return Color.FromArgb(R, G, B); }
        }

        public ResTypeDicoSetting()
        {
            ResType = ResultType.Empty;
            Lbl = null;
            R = 0; G = 0; B = 0; 
        }

        public ResTypeDicoSetting(ResultType rtyp, string lbl, Color clr)
        {
            ResType = rtyp;
            Lbl = lbl;
            R = clr.R;
            G = clr.G;
            B = clr.B;


        }
    }
}
