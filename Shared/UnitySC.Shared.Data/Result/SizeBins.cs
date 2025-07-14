using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace UnitySC.Shared.Data
{
    [DataContract]
    public class SizeBins : ICloneable
    {
        public readonly int DefaultSize = 1000;

        // List of SizeBin should be order in areamax ascending order - if not use Arrange method before use
        [DataMember] private readonly List<SizeBin> _bins = new List<SizeBin>();

        public List<SizeBin> ListBins { get => _bins; }

        public SizeBins()
        {
        }

        public void Reset()
        {
            if (_bins != null)
                _bins.Clear();
        }

        public SizeBins(List<SizeBin> list)
        {
            _bins = list;
        }

        public void AddRange(List<SizeBin> list)
        {
            _bins.AddRange(list);
        }

        public void Arrange()
        {
            _bins.Sort((x, y) => x.AreaMax_um.CompareTo(y.AreaMax_um));
        }

        public void AddBin(long areaMax_um, int size_um)
        {
            _bins.Add(new SizeBin(areaMax_um, size_um));
        }

        public int GetSquareWidth(long myArea_um)
        {
            // List of SizeBin should be order in areamax ascending order
            if (_bins.Count == 0)
                return DefaultSize;

            for (int i = 0; i < _bins.Count; i++)
            {
                if (myArea_um <= _bins[i].AreaMax_um)
                    return _bins[i].Size_um;
            }

            return _bins[_bins.Count - 1].Size_um;
        }

        public bool ExportToXml(string filePath)
        {
            bool bSuccess = true;
            try
            {
                var dcs = new DataContractSerializer(typeof(SizeBins));

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

        public static SizeBins ImportFromXml(string filePath)
        {
            SizeBins DeserializeObj;
            var dcs = new DataContractSerializer(typeof(SizeBins));
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas()))
                {
                    // Deserialize the data and read it from the instance.
                    DeserializeObj = (SizeBins)dcs.ReadObject(reader, true);
                    reader.Close();
                }

                stream.Close();
            }

            return DeserializeObj;
        }

        public object Clone()
        {
            var cl = new SizeBins();
            foreach (var bin in ListBins)
                cl.AddBin(bin.AreaMax_um, bin.Size_um);
            return cl;
        }
    }
}
