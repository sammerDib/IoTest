using System.Collections.Generic;
using System.Drawing;

using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;

namespace UnitySC.PM.ANA.Service.Shared.TestUtils
{
    public static class CameraTestUtils
    {
        public struct Shift
        {
            public Shift(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X;
            public double Y;
        }

        public static DummyUSPImage CreateProcessingImageFromFile(string imgPath)
        {
            var img = new DummyUSPImage(imgPath);
            return img;
        }

        public static string GetDefaultDataPath(string imgName)
        {
            return DirTestUtils.GetWorkingDirectoryDataPath("\\data\\CameraSignal\\" + imgName);
        }

        public static string GetDataPathInAlgoLibDir(string imgName)
        {
            return DirTestUtils.GetWorkingDirectoryDataPath("\\..\\..\\..\\ExternalLibs\\UnitySC.Algo\\tests\\data\\" + imgName);
        }

        public static string GetDataPathInSharedAlgos(string imgPath)
        {
            return DirTestUtils.GetWorkingDirectoryDataPath("\\..\\..\\..\\..\\Shared\\Algos\\" + imgPath);
        }

        public static DummyUSPImage CreateImageCenteredOnTopLeftDieCorner()
        {
            // The overall image contains a repeating pattern of dies
            var overallImg = new Bitmap(GetDefaultDataPath("checkPatternRec\\" + "image_512x512.jpg"));

            // Set the reference image field of view so that its center is the upper left corner of a die
            var topLeftDieCorner = new Point(159, 141);
            var imageSize = new Size(120, 100);
            var imageTopLeftPoint = new Point(topLeftDieCorner.X - imageSize.Width / 2, topLeftDieCorner.Y - imageSize.Height / 2);
            var imgFov = new Rectangle(imageTopLeftPoint, imageSize);
            var img = new DummyUSPImage(overallImg.Clone(imgFov, overallImg.PixelFormat));

            return img;
        }

        public static DummyUSPImage CreateImageCenteredOnBottomRightDieCorner()
        {
            // The overall image contains a repeating pattern of dies
            var overallImg = new Bitmap(GetDefaultDataPath("checkPatternRec\\" + "image_512x512.jpg"));

            // Set the reference image field of view so that its center is the bottom right corner of a die
            var bottomRightDieCorner = new Point(258, 241);
            var imageSize = new Size(120, 100);
            var imageTopLeftPoint = new Point(bottomRightDieCorner.X - imageSize.Width / 2, bottomRightDieCorner.Y - imageSize.Height / 2);
            var imgFov = new Rectangle(imageTopLeftPoint, imageSize);
            var img = new DummyUSPImage(overallImg.Clone(imgFov, overallImg.PixelFormat));

            return img;
        }

        public static DummyUSPImage CreateImageCenteredOnShiftedTopLeftDieCorner(Shift pixelShifts)
        {
            return CreateImageCenteredOnShiftedTopLeftDieCorner(new List<Shift> { pixelShifts })[0];
        }

        public static List<DummyUSPImage> CreateImageCenteredOnShiftedTopLeftDieCorner(List<Shift> pixelShifts)
        {
            var overallImg = new Bitmap(GetDefaultDataPath("checkPatternRec\\" + "image_512x512.jpg"));

            var imagesShifted = new List<DummyUSPImage>();
            foreach (var shift in pixelShifts)
            {
                double shiftX = shift.X;
                double shiftY = -shift.Y; // wafer referential to image referential

                var topLeftDieCorner = new Point(159, 141);
                var topLeftDieCornerShifted = new Point((int)(topLeftDieCorner.X - shiftX), (int)(topLeftDieCorner.Y - shiftY));
                var imageSize = new Size(120, 100);
                var imageTopLeftPoint = new Point(topLeftDieCornerShifted.X - imageSize.Width / 2, topLeftDieCornerShifted.Y - imageSize.Height / 2);
                var imgFov = new Rectangle(imageTopLeftPoint, imageSize);
                imagesShifted.Add(new DummyUSPImage(overallImg.Clone(imgFov, overallImg.PixelFormat)));
            }

            return imagesShifted;
        }
    }
}
