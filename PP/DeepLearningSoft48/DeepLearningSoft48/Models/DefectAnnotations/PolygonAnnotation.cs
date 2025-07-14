using System;
using System.Windows.Media;

namespace DeepLearningSoft48.Models.DefectAnnotations
{
    [Serializable]
    public class PolygonAnnotation : DefectAnnotation
    {
        public PointCollection Points { get; set; }

        public PolygonAnnotation() { }
        public PolygonAnnotation(double originX, double originY, double width, double height, DefectCategoryPair category, string source)
        {
            OriginX = originX;
            OriginY = originY;
            Width = width;
            Height = height;
            Category = category;
            Source = source;
            Type = "Polygon";
            Points = new PointCollection();
        }

        // Constructor that allows to serialise the modification of a PolygonAnnotation with PointCollecction as an extra parameter
        public PolygonAnnotation(double originX, double originY, double width, double height, DefectCategoryPair category, string source, PointCollection points)
        {
            OriginX = originX;
            OriginY = originY;
            Width = width;
            Height = height;
            Category = category;
            Source = source;
            Type = "Polygon";
            Points = points;
        }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of PolygonAnnotation objects.
        /// Notably useful when we want to check whether a List of PolygonAnnotations contains a specific PolygonAnnotation object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PolygonAnnotation pa = (PolygonAnnotation)obj;
            return
                OriginX.Equals(pa.OriginX)
                && OriginY.Equals(pa.OriginY)
                && Width.Equals(pa.Width)
                && Height.Equals(pa.Height)
                && Category.Equals(pa.Category)
                && Source.Equals(pa.Source)
                && Type.Equals(pa.Type)
               && PointCollectionsEqual(Points, pa.Points);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of PolygonAnnotation objects.
        /// Notably useful when we want to check whether a List of PolygonAnnotations contains a specific PolygonAnnotation object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + OriginX.GetHashCode();
                hash = hash * 23 + OriginY.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                hash = hash * 23 + Category.GetHashCode();
                hash = hash * 23 + Source.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                hash = hash * 23 + Points.GetHashCode();
                return hash;
            }
        }

        /// <summary>
        /// 'PointsEqual' compares the Points Collection of two PolygonAnnotation instances.
        /// </summary>
        private bool PointCollectionsEqual(PointCollection pointsCollection1, PointCollection pointsCollection2)
        {
            if (pointsCollection1 == null && pointsCollection2 == null)
                return true;

            if (pointsCollection1 == null || pointsCollection2 == null || pointsCollection1.Count != pointsCollection2.Count)
                return false;

            for (int i = 0; i < pointsCollection1.Count; i++)
                if (!pointsCollection1[i].Equals(pointsCollection2[i]))
                    return false;

            return true;
        }
    }
}
