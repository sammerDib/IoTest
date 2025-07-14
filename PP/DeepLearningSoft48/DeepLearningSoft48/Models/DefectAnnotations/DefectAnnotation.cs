using System.Xml.Serialization;

namespace DeepLearningSoft48.Models.DefectAnnotations
{
    [XmlInclude(typeof(BoundingBox))]
    [XmlInclude(typeof(LineAnnotation))]
    [XmlInclude(typeof(PolygonAnnotation))]
    [XmlInclude(typeof(PolylineAnnotation))]
    public abstract class DefectAnnotation
    {
        public string Type { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public DefectCategoryPair Category { get; set; }
        public string Source { get; set; }
        public double OriginX { get; set; }
        public double OriginY { get; set; }

        public double GetArea() { return Height * Width; }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of DefectAnnotation objects.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            DefectAnnotation da = (DefectAnnotation)obj;
            return OriginX.Equals(da.OriginX)
                && OriginY.Equals(da.OriginY)
                && Width.Equals(da.Width)
                && Height.Equals(da.Height)
                && Category.Equals(da.Category)
                && Source.Equals(da.Source)
                && Type.Equals(da.Type);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of DefectAnnotation objects.
        /// Notably useful when we want to check whether a List of DefectAnnotations contains a specific DefectAnnotation object.
        /// </summary>
        public override int GetHashCode()
        {
            return OriginX.GetHashCode() ^ OriginY.GetHashCode() ^
                Width.GetHashCode() ^ Height.GetHashCode() ^
                Category.GetHashCode() ^
                Source.GetHashCode() ^ Type.GetHashCode();
        }
    }
}
