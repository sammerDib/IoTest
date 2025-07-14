using System;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.Tools;

namespace LibProcessing
{
    public partial class ProcessingClassMil : ProcessingClass
    {
        //---------------------------------------------------------------------
        public override void Load(string filename, ProcessingImage image)
        {
            image.GetMilImage().Restore(filename);
        }

        //---------------------------------------------------------------------
        public override void Save(string filename, ProcessingImage image)
        {
            string ext = new PathString(filename).Extension.ToLower();

            MIL_INT milformat;
            if (ext == ".bmp")
                milformat = MIL.M_BMP;
            else if (ext == ".jpg" || ext == ".jpeg")
                milformat = MIL.M_JPEG_LOSSY;
            else if (ext == ".tif" || ext == ".tiff")
                milformat = MIL.M_MIL;
            //   else if (ext == ".png")
            //       milformat = MIL.M_PNG; MIL.M_VERSION
            // // le png existe pas en mil 9, on commente pour pouvoir effectuer des tests/dev/debug sur du mil 9
            else
                throw new ApplicationException("unknown file extension: " + ext);

            image.GetMilImage().Export(filename, milformat);
        }

        //---------------------------------------------------------------------
        public override void ImageDiskInquire(String filename, out int width, out int height, out int depth)
        {
            width = MilImage.DiskInquire(filename, MIL.M_SIZE_X);
            height = MilImage.DiskInquire(filename, MIL.M_SIZE_Y);
            depth = MilImage.DiskInquire(filename, MIL.M_SIZE_BIT);
        }
    }
}
