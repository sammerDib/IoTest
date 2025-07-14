using System.Drawing;

namespace DeepLearningSoft48.Models
{
    /// <summary>
    ///  This class defines a custom colour, because System.Drawing.Color is non-serialisable through XMLSerializer, only with BinaryFormatter.
    ///  However, we do not intend to save files in binary format, because we want them to be readable by humans.
    ///  Therefore, this custom class takes on the same properties as 'System.Drawing.Color', except it's serialisable with XMLSerialiser. 
    /// </summary>
    public class SerializableColor
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public SerializableColor() { }
        public SerializableColor(Color color)
        {
            A = color.A;
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public SerializableColor(System.Windows.Media.Color color) { }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// The conversion operator allows for automatic conversion from a System.Drawing.Color object to a SerializableColor object.
        /// This enables passing of System.Drawing.Color objects to methods that expect SerializableColor objects. 
        /// We need this, since the DefectCategoryPair class now expects a SerializableColor type instead of a System.Drawing.Color type. 
        /// Why? Because System.Drawing.Color isn't serialisable with XMLSerializer.
        /// </summary>

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        public static implicit operator SerializableColor(Color color)
        {
            return new SerializableColor(color);
        }

        public static implicit operator Color(SerializableColor serializableColor)
        {
            return Color.FromArgb(serializableColor.A, serializableColor.R, serializableColor.G, serializableColor.B);
        }

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of DefectAnnotation objects, because each DefectAnnotation has a DefectCategoryPair, 
        /// and each DefectCategoryPair has SerializableColor.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SerializableColor other = (SerializableColor)obj;

            return A == other.A &&
                   R == other.R &&
                   G == other.G &&
                   B == other.B;
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of DefectAnnotation objects, because each DefectAnnotation has a DefectCategoryPair, 
        /// and each DefectCategoryPair has SerializableColor.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + A.GetHashCode();
                hash = hash * 23 + R.GetHashCode();
                hash = hash * 23 + G.GetHashCode();
                hash = hash * 23 + B.GetHashCode();
                return hash;
            }
        }
    }
}
