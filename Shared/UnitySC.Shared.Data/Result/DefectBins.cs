using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class DefectBins : ICloneable
    {
        [DataMember]
        private readonly Dictionary<int, DefectBin> _dicoRoughBinDefect;

        public List<int> RoughBinList { get { return _dicoRoughBinDefect.Keys.ToList(); } }
        public List<DefectBin> DefectBinList { get { return _dicoRoughBinDefect.Values.OrderBy(x => x.RoughBin).ToList(); } }

        public DefectBins()
        {
            _dicoRoughBinDefect = new Dictionary<int, DefectBin>();
        }

        public void Reset()
        {
            _dicoRoughBinDefect.Clear();
        }

        public void Add(DefectBin def)
        {
            _dicoRoughBinDefect.Add(def.RoughBin, def);
        }

        public DefectBin GetDefectBin(int roughbincode)
        {
            return _dicoRoughBinDefect[roughbincode];
        }

        public System.Drawing.Color GetDefectBinColor(int roughbincode)
        {
            return System.Drawing.Color.FromArgb(_dicoRoughBinDefect[roughbincode].Color);
        }

        public System.Drawing.Color GetDefectBinColorOrDefault(int roughbincode)
        {
            if (_dicoRoughBinDefect.ContainsKey(roughbincode))
                return System.Drawing.Color.FromArgb(_dicoRoughBinDefect[roughbincode].Color);
            return System.Drawing.Color.Transparent;
        }

        public string GetDefectBinLabel(int roughbincode)
        {
            return _dicoRoughBinDefect[roughbincode].Label;
        }

        public bool ExportToXml(string filePath)
        {
            bool bSuccess = true;
            try
            {
                var dcs = new DataContractSerializer(typeof(DefectBins));

                using (Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    using (var writer = XmlDictionaryWriter.CreateTextWriter(stream, System.Text.Encoding.UTF8))
                    {
                        writer.WriteStartDocument();
                        dcs.WriteObject(writer, this);
                        writer.Close();
                    }
                    stream.Close();
                }
            }
            catch (Exception)
            {
                bSuccess = false;
            }
            return bSuccess;
        }

        public static DefectBins ImportFromXml(string filePath)
        {
            DefectBins deserializeObj;
            var dcs = new DataContractSerializer(typeof(DefectBins));
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
                {
                    // Deserialize the data and read it from the instance.
                    deserializeObj = (DefectBins)dcs.ReadObject(reader, true);
                    reader.Close();
                }

                stream.Close();
            }

            return deserializeObj;
        }

        public object Clone()
        {
            var cl = new DefectBins();
            foreach (var bin in _dicoRoughBinDefect)
                cl.Add(new DefectBin(bin.Value.RoughBin, bin.Value.Label, bin.Value.Color));
            return cl;
        }
    }
}
