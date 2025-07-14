using System.Windows.Media;

using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.ViewModels.DefectAnnotations
{
    public class PolylineAnnotationVM : DefectAnnotationVM
    {
        public PointCollection Points { get; set; }

        public PolylineAnnotationVM() { }
        public PolylineAnnotationVM(double originX, double originY, PointCollection points, double width, double height, DefectCategoryPair category, string source)
        {
            OriginX = originX;
            OriginY = originY;
            Points = points;
            Width = width;
            Height = height;
            Category = category;
            Source = source;
            Type = "Free Layout";
        }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of PolylineAnnotationVM objects.
        /// Notably useful when we want to check whether a List of PolylineAnnotationVMs contains a specific PolylineAnnotationVM object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            PolylineAnnotationVM plAVM = (PolylineAnnotationVM)obj;
            return
                OriginX.Equals(plAVM.OriginX)
                && OriginY.Equals(plAVM.OriginY)
                && Width.Equals(plAVM.Width)
                && Height.Equals(plAVM.Height)
                && Category.Equals(plAVM.Category)
                && Source.Equals(plAVM.Source)
                && Type.Equals(plAVM.Type)
                && PointCollectionsEqual(Points, plAVM.Points);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of PolylineAnnotationVM objects.
        /// Notably useful when we want to check whether a List of PolylineAnnotationVMs contains a specific PolylineAnnotationVM object.
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
        /// 'PointsEqual' compares the Points Collection of two PolylineAnnotationVM instances.
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
