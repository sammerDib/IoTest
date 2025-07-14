using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;

namespace UnitySC.Equipment.Devices.Controller.JobDefinition
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class OcrProfile
    {
        public OcrProfile()
        {
            Name = string.Empty;
            CreationDate = DateTime.Now;
            ModificationDate = DateTime.Now;
            Parameters = new OcrParameters();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember]
        public OcrParameters Parameters { get; set; }

        public List<string> Validate()
        {
            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                result.Add("Name can not be null or empty");
            }

            if (string.IsNullOrWhiteSpace(Author))
            {
                result.Add("Author can not be null or empty");
            }

            if (Parameters.ReaderSide is ReaderSide.Front or ReaderSide.Both && string.IsNullOrWhiteSpace(Parameters.FrontRecipeName))
            {
                result.Add("Front Recipe Name can not be null or empty");
            }

            if (Parameters.ReaderSide is ReaderSide.Back or ReaderSide.Both && string.IsNullOrWhiteSpace(Parameters.BackRecipeName))
            {
                result.Add("Back Recipe Name can not be null or empty");
            }

            return result;
        }

        public OcrProfile Clone()
        {
            return new OcrProfile()
            {
                Author = Author,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Name = Name + "_Clone",
                Parameters = Parameters.Clone()
            };
        }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class OcrParameters
    {
        [DataMember]
        public string FrontRecipeName { get; set; }

        [DataMember]
        public string BackRecipeName { get; set; }

        [DataMember]
        public double ScribeAngle { get; set; }

        [DataMember]
        public ReaderSide ReaderSide { get; set; }

        public OcrParameters Clone()
        {
            return new OcrParameters()
            {
                BackRecipeName = BackRecipeName,
                FrontRecipeName = FrontRecipeName,
                ReaderSide = ReaderSide,
                ScribeAngle = ScribeAngle
            };
        }
    }
}
