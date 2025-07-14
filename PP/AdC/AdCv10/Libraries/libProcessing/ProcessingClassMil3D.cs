using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using FloatDataFile;
#warning ** USP ** replace CFloatDataFile with  UnitySC.Shared.Data.FormatFile.MatrixFloatFile

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace LibProcessing
{
    public class ProcessingClassMil3D : ProcessingClassMil
    {
        //=================================================================
        //
        //=================================================================
        public override void Load(string filename, ProcessingImage image)
        {
            MilImage milImage = image.GetMilImage();

            //-------------------------------------------------------------
            // Chargement de l'image DataFloat
            //-------------------------------------------------------------
            List<float[]> dataFloatChunks = null;

            int width, height;
            using (CFloatDataFile floatDataFile = new CFloatDataFile())
                dataFloatChunks = floatDataFile.ReadFromFile_Parallel(filename, out width, out height);

            if (dataFloatChunks == null)
                throw new ApplicationException("Image is empty: " + filename);

            //-------------------------------------------------------------
            // Copie dans une image MIL
            //-------------------------------------------------------------
            milImage.Alloc2d(width, height, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC);

            long nRow = 0;
            for (int i = 0; i < dataFloatChunks.Count; i++)
            {
                long chunk_size = dataFloatChunks[i].LongLength;
                int nbFullRow_toCpy = (int)Math.Floor((double)chunk_size / (double)width);

                milImage.Put2d(0, nRow, width, nbFullRow_toCpy, dataFloatChunks[i]);
                nRow += nbFullRow_toCpy;
            }
        }

        public bool CopyToWriteableBitmap(ref WriteableBitmap writeableBitmap, ProcessingImage previewMilImage)
        {
            ProcessingImage img8bit = null;
            MilImage milImage = null;

            bool reallocated = false;
            try
            {
                //---------------------------------------------------------
                // Get MilImage
                //---------------------------------------------------------
                milImage = previewMilImage.GetMilImage();
                if (milImage == null || milImage.MilId == 0)
                {
                    reallocated = (writeableBitmap == null);
                    writeableBitmap = null;
                    return reallocated;
                }

                // Conversion 32float -> 8bit si besoin
                //.....................................
                img8bit = this.ConvertTo8bit(previewMilImage);
                milImage = img8bit.GetMilImage();

                int width = milImage.SizeX;
                int height = milImage.SizeY;
                int pitch = milImage.Pitch;

                //---------------------------------------------------------
                // Reuse or reallocate the BitmapSource
                //---------------------------------------------------------
                reallocated = reallocated || (writeableBitmap == null);
                reallocated = reallocated || (writeableBitmap.Width != width);
                reallocated = reallocated || (writeableBitmap.Height != height);
                reallocated = reallocated || (writeableBitmap.Format != PixelFormats.Gray8);

                if (reallocated)
                    writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);

                //-----------------------------------------------------
                // Copie des pixels
                //-----------------------------------------------------

                // Lock source image
                //..................
                milImage.Lock(MIL.M_READ);
                long pSrc = milImage.HostAddress;

                // Copy
                //.....
                int bufferSize = pitch * height;
                writeableBitmap.WritePixels(new Int32Rect(0, 0, width, height), new IntPtr(pSrc), bufferSize, pitch);
            }
            finally
            {
                // Unlock source image
                //....................
                if (milImage.IsLocked)
                    milImage.Unlock();

                if (img8bit != null)
                    img8bit.Dispose();
            }

            return reallocated;
        }

        //=================================================================
        //
        //=================================================================
        public override void Save(string filename, ProcessingImage image)
        {
            //-------------------------------------------------------------
            // Conversion MIL -> float
            //-------------------------------------------------------------
            MilImage milImage = image.GetMilImage();
            float[] data = new float[milImage.SizeY * milImage.SizeX];
            milImage.Get(data);

            //-------------------------------------------------------------
            // Ecriture du CFloatDataFile
            //-------------------------------------------------------------
            using (CFloatDataFile floatDataFile = new CFloatDataFile())
            {
                bool compress = (milImage.SizeByte > 8192);
                floatDataFile.WriteInFile(filename, data, milImage.SizeX, milImage.SizeY, compress);
            }
        }

        //=================================================================
        //
        //=================================================================
        public override void ImageDiskInquire(String filename, out int width, out int height, out int depth)
        {
            CFloatDataFile.GetSizeFromFile(filename, out width, out height);
            depth = 32;
        }

        //=================================================================
        /// <summary>
        /// Fonction interne pour convertir l'image de 16 en 8 bits.
        /// La fonction externe est ConvertTo8bit.
        /// </summary>
        //=================================================================
        protected ProcessingImage Convert3DTo2D(ProcessingImage img3D)
        {
            MilImage mil3D = img3D.GetMilImage();

            //-------------------------------------------------------------
            // Calcul du Mix/Max en tenant compte des NaN
            //-------------------------------------------------------------
            double dMin = 0.0;
            double dMax = 0.0;

           // using (MilImage milMask = new MilImage())
            using (MilImageResult milStat = new MilImageResult())
            {
                //milMask.Alloc2d(mil3D.SizeX, mil3D.SizeY, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                //MilImage.Binarize(mil3D, milMask, MIL.M_IN_RANGE, float.MinValue, float.MaxValue);



                // ISSUE M_MASK no longer exist in MilX -- 
                // to do find a new way to deal with nans
                milStat.AllocResult(mil3D.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                //old way Mil 10
                // stat.Calculate(mil3D, MIL.M_MIN + MIL.M_MAX, MIL.M_MASK, milMask.MilId, MIL.M_NULL);
                milStat.Stat(mil3D, MilTo.StatList(MIL.M_STAT_MIN, MIL.M_STAT_MAX /*, MIL.M_MASK*/), MIL.M_NOT_EQUAL, float.NaN);

                dMax = milStat.GetResult(MIL.M_STAT_MAX);
                dMin = milStat.GetResult(MIL.M_STAT_MIN);
            }

            //-------------------------------------------------------------
            // Conversion 2D
            //-------------------------------------------------------------
            double a = 1.0f;
            double b = 0.0f;
            if ((dMax - dMin) != 0.0f)
            {
                a = 255.0f / (dMax - dMin);
                b = -dMin * 255.0f / (dMax - dMin);
            }

            ProcessingImage img2D = new ProcessingImage();
            MilImage mil2D = img2D.GetMilImage();
            mil2D.Alloc2d(mil3D.SizeX, mil3D.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
            MilImage.ArithMultiple(mil3D, a, b, 1, MIL.M_NULL, mil2D, MIL.M_MULTIPLY_ACCUMULATE_1 + MIL.M_SATURATION, MIL.M_DEFAULT);

            return img2D;
        }

        //=================================================================
        /// <summary>
        /// NB: il faut Disposer l'image retournée. 
        /// </summary>
        //=================================================================
        public override ProcessingImage ConvertTo8bit(ProcessingImage source)
        {
            MilImage milSource = source.GetMilImage();
            int type = milSource.Type;
            switch (type)
            {
                case 8 + MIL.M_UNSIGNED:
                    source.AddRef();
                    return source;
                case 16 + MIL.M_UNSIGNED:
                    return Convert16bitTo8bit(source);
                case 32 + MIL.M_FLOAT:
                    return Convert3DTo2D(source);
                default:
                    throw new ApplicationException($"unsupported image type: depth={milSource.SizeBit} type={milSource.Type}");
            }
        }
    }
}
