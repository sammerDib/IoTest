using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Fringe;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    [DataContract(Namespace = "")]
    public class Fringe
    {
        /// <summary>
        /// Is custom
        /// </summary>
        [DataMember]
        public FringeType FringeType;

        /// <summary>
        /// Fringe name, valid si Custom ou Multi
        /// </summary>
        [DataMember]
        public string Name;

        /// <summary>
        /// Période des franges, en pixels, dans le cas où il n'y en a qu'une
        /// </summary>
        [XmlIgnore]
        public int Period
        {
            get => Periods.Count > 0 ? Periods[0] : 0;
            set
            {
                if (Periods.Count > 0)
                    Periods[0] = value;
                else
                    Periods.Add(value);
            }
        }

        /// <summary>
        /// Liste des périodes des franges, en pixels.
        /// </summary>
        [DataMember]
        public List<int> Periods = new List<int>();

        /// <summary>
        /// Number of images by direction X/Y.
        /// </summary>
        [DataMember]
        public int NbImagesPerDirection;

        /// <summary>
        /// Comparaison avec un objet Fringe
        /// </summary>
        public bool HasSameValue(Fringe fringe)
        {
            bool eq = (fringe != null) &&
                      (Name == fringe.Name) &&
                      (FringeType == fringe.FringeType) &&
                      (Periods.Count == fringe.Periods.Count) &&
                      (NbImagesPerDirection == fringe.NbImagesPerDirection) &&
                      Periods.SequenceEqual(fringe.Periods);
            return eq;
        }

        public override string ToString()
        {
            if (FringeType == FringeType.Standard)
                return $"Fringe-{Period}pix-{NbImagesPerDirection}";
            else
                return $"Fringe-{Name}";
        }

        public int FirstImageIndex(int periodIndex, char direction)
        {
            int index;

            if (direction == 'X')
                index = periodIndex * NbImagesPerDirection;
            else
                index = (Periods.Count() + periodIndex) * NbImagesPerDirection;

            return index;
        }
    }
}
