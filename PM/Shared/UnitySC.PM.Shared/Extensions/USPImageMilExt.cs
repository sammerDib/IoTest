using System.Windows.Media;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Image;
using UnitySC.Shared.LibMIL;

namespace UnitySC.PM.Shared.Extensions
{
    public static class USPImageMilExt
    {
        public static USPImageMil ColorizeImage(this USPImageMil procimg, Color color)
        {
            if (procimg.IsMilSimulated)
            {
                return procimg.Clone();
            }

            if (color.ToString() == "White" || color.ToString() == "#FFFFFFFF")
            {
                return procimg.Clone();
            }
            else
            {
                USPImageMil colorimg = new USPImageMil();
                MilImage milMatrix = new MilImage();
                MilImage milImage = procimg.GetMilImage();

                float[,] matrix = new float[3, 3];
                matrix[0, 0] = matrix[0, 1] = matrix[0, 2] = color.R / 3.0f / 255;
                matrix[1, 0] = matrix[1, 1] = matrix[1, 2] = color.G / 3.0f / 255;
                matrix[2, 0] = matrix[2, 1] = matrix[2, 2] = color.B / 3.0f / 255;
                milMatrix.Alloc2d(3, 3, 32 + MIL.M_FLOAT, MIL.M_ARRAY);
                milMatrix.Put(matrix);

                MilImage milColorImage = colorimg.GetMilImage();
                milColorImage.AllocColor(milImage.OwnerSystem, 3, milImage.SizeX, milImage.SizeY, milImage.Type, milImage.Attribute);
                if (milImage.SizeBand == 1)
                {
                    MilImage.Convert(milImage, milColorImage, MIL.M_L_TO_RGB);
                    MilImage.Convert(milColorImage, milColorImage, milMatrix);
                }
                else
                {
                    MilImage.Convert(milImage, milColorImage, milMatrix);
                }
                //TODO faut-il calibrer

                return colorimg;
            }
        }
    }
}
