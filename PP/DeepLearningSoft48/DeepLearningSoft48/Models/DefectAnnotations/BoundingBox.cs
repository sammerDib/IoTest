using System;

namespace DeepLearningSoft48.Models.DefectAnnotations
{
    [Serializable]
    public class BoundingBox : DefectAnnotation
    {
        public BoundingBox() { }
        public BoundingBox(double xCoordinate, double yCoordinate, double width, double height, DefectCategoryPair category, string source)
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
        /// Override the 'Equals' method to allow for proper comparision of BoundingBox objects.
        /// Notably useful when we want to check whether a List of BoundingBoxes contains a specific BoundingBox object.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            BoundingBox bb = (BoundingBox)obj;
            return OriginX.Equals(bb.OriginX)
                && OriginY.Equals(bb.OriginY)
                && Width.Equals(bb.Width)
                && Height.Equals(bb.Height)
                && Category.Equals(bb.Category)
                && Source.Equals(bb.Source)
                && Type.Equals(bb.Type);
        }

        /// <summary>
        /// Override the 'GetHashCode' method to allow for proper comparision of BoundingBox objects.
        /// Notably useful when we want to check whether a List of BoundingBoxes contains a specific BoundingBox object.
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
