using System.Windows.Media;

using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.ViewModels.DefectAnnotations
{
    public class PolygonAnnotationVM : DefectAnnotationVM
    {
        public PointCollection Points { get; set; }
        public PolygonAnnotationVM() { }
        public PolygonAnnotationVM(double originX, double originY, double width, double height, DefectCategoryPair category, string source)
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

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of PolygonAnnotationVM objects.
        /// Notably useful when we want to check whether a List of PolygonAnnotationVMs contains a specific PolygonAnnotationVM object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PolygonAnnotationVM paVM = (PolygonAnnotationVM)obj;
            return
                OriginX.Equals(paVM.OriginX)
                && OriginY.Equals(paVM.OriginY)
                && Width.Equals(paVM.Width)
                && Height.Equals(paVM.Height)
                && Category.Equals(paVM.Category)
                && Source.Equals(paVM.Source)
                && Type.Equals(paVM.Type)
                && PointCollectionsEqual(Points, paVM.Points);
        }

        /// Override the 'GetHashCode' method to allow for proper comparision of PolygonAnnotationVM objects.
        /// Notably useful when we want to check whether a List of PolygonAnnotationVMs contains a specific PolygonAnnotationVM object.
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
        /// 'PointsEqual' compares the Points Collection of two PolygonAnnotationVM instances.
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
