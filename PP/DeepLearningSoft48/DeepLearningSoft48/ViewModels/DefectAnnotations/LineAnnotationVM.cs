using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.ViewModels.DefectAnnotations
{
    public class LineAnnotationVM : DefectAnnotationVM
    {
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public LineAnnotationVM() { }

        public LineAnnotationVM(double originX, double originY, double x1, double y1, double x2, double y2, double width, double height, DefectCategoryPair category, string source)
        {
            OriginX = originX;
            OriginY = originY;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            Width = width;
            Height = height;
            Category = category;
            Source = source;
            Type = "Line";
        }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of LineVM objects.
        /// Notably useful when we want to check whether a List of LineVMs contains a specific LineVM object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            LineAnnotationVM laVM = (LineAnnotationVM)obj;
            return OriginX.Equals(laVM.OriginX)
                && OriginY.Equals(laVM.OriginY)
                && X1.Equals(laVM.X1)
                && Y1.Equals(laVM.Y1)
                && X2.Equals(laVM.X2)
                && Y2.Equals(laVM.Y2)
                && Width.Equals(laVM.Width)
                && Height.Equals(laVM.Height)
                && Category.Equals(laVM.Category)
                && Source.Equals(laVM.Source)
                && Type.Equals(laVM.Type);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of LineVM objects.
        /// Notably useful when we want to check whether a List of LineVMs contains a specific LineVM object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + OriginX.GetHashCode();
                hash = hash * 23 + OriginY.GetHashCode();
                hash = hash * 23 + X1.GetHashCode();
                hash = hash * 23 + Y1.GetHashCode();
                hash = hash * 23 + X2.GetHashCode();
                hash = hash * 23 + Y2.GetHashCode();
                hash = hash * 23 + Width.GetHashCode();
                hash = hash * 23 + Height.GetHashCode();
                hash = hash * 23 + Category.GetHashCode();
                hash = hash * 23 + Source.GetHashCode();
                hash = hash * 23 + Type.GetHashCode();
                return hash;
            }
        }
    }
}
