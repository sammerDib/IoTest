using DeepLearningSoft48.Models;

namespace DeepLearningSoft48.ViewModels.DefectAnnotations
{
    public class BoundingBoxVM : DefectAnnotationVM
    {
        public BoundingBoxVM() { }
        public BoundingBoxVM(double xCoordinate, double yCoordinate, double width, double height, DefectCategoryPair category, string source)
        {
            OriginX = xCoordinate;
            OriginY = yCoordinate;
            Width = width;
            Height = height;
            Category = category;
            Source = source;
            Type = "Bounding Box";
        }

        //------------------------------------------------------------
        // Helpers
        //------------------------------------------------------------

        /// <summary>
        /// Override the 'Equals' method to allow for proper comparision of BoundingBoxVM objects.
        /// Notably useful when we want to check whether an ObservableCollection of BoundingBoxVMs contains a specific BoundingBoxVM object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            BoundingBoxVM bbVM = (BoundingBoxVM)obj;
            return OriginX.Equals(bbVM.OriginX)
                && OriginY.Equals(bbVM.OriginY)
                && Width.Equals(bbVM.Width)
                && Height.Equals(bbVM.Height)
                && Category.Equals(bbVM.Category)
                && Source.Equals(bbVM.Source)
                && Type.Equals(bbVM.Type);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of BoundingBoxVM objects.
        /// Notably useful when we want to check whether an ObservableCollection of BoundingBoxVMs contains a specific BoundingBoxVM object.
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
                return hash;
            }
        }
    }
}
