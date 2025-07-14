using System;
using System.Drawing;
using System.Xml.Serialization;

namespace DeepLearningSoft48.Models
{
    /// <summary>
    /// Defect class for categorization. 
    /// Used in the Learning part to associate a defect category to a color.
    /// </summary>
    [Serializable]
    public class DefectCategoryPair
    {
        [XmlElement("color")]
        public SerializableColor Color { get; set; } // custom color because System.Drawing.Color non-serialisable using XMLSerializer
        [XmlAttribute("label")]
        public string Label { get; set; }

        public DefectCategoryPair(Color color, string label)
        {
            Color = new SerializableColor(color);
            Label = label;
        }

        public DefectCategoryPair() { }


        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of DefectAnnotation objects, because each DefectAnnotation has a DefectCategoryPair.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DefectCategoryPair other = (DefectCategoryPair)obj;
            return Color.Equals(other.Color) && Label.Equals(other.Label);
        }

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of DefectAnnotation objects, because each DefectAnnotation has a DefectCategoryPair.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Color.GetHashCode();
                hash = hash * 23 + Label.GetHashCode();
                return hash;
            }
        }
    }
}
