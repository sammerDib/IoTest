using System;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;
using System.Linq;

namespace LibProcessing
{

    #region ExportableClass
    //=========================================================
    public class BlobFilteringParameters
    {
        MIL_INT operation;
        double value;
        bool used;

        public double getValue { get { return value; } }
        public bool getUsed { get { return used; } }
        public MIL_INT getOperation { get { return operation; } }

        public BlobFilteringParameters(double dValue, bool bUsed, enCharacteristicMILBlobFiltering enOperation)
        {
            value = dValue;
            used = bUsed;

            switch (enOperation)
            {
                case enCharacteristicMILBlobFiltering.en_Breath: operation = MIL.M_BREADTH; break;
                case enCharacteristicMILBlobFiltering.en_Compactness: operation = MIL.M_COMPACTNESS; break;
                case enCharacteristicMILBlobFiltering.en_ConvexPerimeter: operation = MIL.M_CONVEX_PERIMETER; break;
                case enCharacteristicMILBlobFiltering.en_Elongation: operation = MIL.M_ELONGATION; break;
                case enCharacteristicMILBlobFiltering.en_EulerNumber: operation = MIL.M_EULER_NUMBER; break;
                case enCharacteristicMILBlobFiltering.en_Length: operation = MIL.M_LENGTH; break;
                case enCharacteristicMILBlobFiltering.en_Perimeter: operation = MIL.M_PERIMETER; break;
                case enCharacteristicMILBlobFiltering.en_Roughness: operation = MIL.M_ROUGHNESS; break;
                case enCharacteristicMILBlobFiltering.en_Area: operation = MIL.M_AREA; break;
                case enCharacteristicMILBlobFiltering.en_AxisPrincipal: operation = MIL.M_AXIS_PRINCIPAL_ANGLE; break;
            }
        }
    }
    #endregion

    public partial class ProcessingClassMil : ProcessingClass
    {
        public ProcessingClassMil()
        {

        }

        //--------------------------------------------------------

        //=================================================================
        // 
        //=================================================================


        //=================================================================
        public override void UserConvolve(ProcessingImage ProcessImage, enTypeOfConvolve TypeConvolve)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (TypeConvolve)
            {
                case enTypeOfConvolve.Prewitt:
                    milImage.Convolve(MIL.M_EDGE_DETECT2);
                    break;
                case enTypeOfConvolve.LaplacianLow:
                    milImage.Convolve(MIL.M_LAPLACIAN_EDGE);
                    break;
                case enTypeOfConvolve.LaplacianHigh:
                    milImage.Convolve(MIL.M_LAPLACIAN_EDGE2);
                    break;
                case enTypeOfConvolve.SharpenLow:
                    milImage.Convolve(MIL.M_SHARPEN2);
                    break;
                case enTypeOfConvolve.SharpenHigh:
                    milImage.Convolve(MIL.M_SHARPEN);
                    break;
                case enTypeOfConvolve.Smooth:
                    milImage.Convolve(MIL.M_SMOOTH);
                    break;
            }
        }
        //=================================================================
        public override void Expansion(ProcessingImage ProcessImage, enKindOfPicture KindOfPicture, int NbIterations)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (KindOfPicture)
            {
                case enKindOfPicture.Binary:
                    milImage.Dilate(NbIterations, MIL.M_BINARY);
                    break;
                case enKindOfPicture.GreyLevel:
                    milImage.Dilate(NbIterations, MIL.M_GREYSCALE);
                    break;
            }
        }
        //=================================================================
        public override void Erosion(ProcessingImage ProcessImage, enKindOfPicture KindOfPicture, int NbIterations)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (KindOfPicture)
            {
                case enKindOfPicture.Binary:
                    milImage.Erode(NbIterations, MIL.M_BINARY);
                    break;
                case enKindOfPicture.GreyLevel:
                    milImage.Erode(NbIterations, MIL.M_GREYSCALE);
                    break;
            }
        }
        //=================================================================
        public override void Threshold(ProcessingImage ProcessImage, double LowParam, double HighParam)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            if (milImage.Type == 8 + MIL.M_UNSIGNED)
            {
                milImage.Binarize(MIL.M_IN_RANGE, LowParam, HighParam);
            }
            else
            {
                using (MilImage milBinImage = new MilImage())
                {
                    milBinImage.Alloc2d(milImage.OwnerSystem, milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, milImage.Attribute);

                    MilImage.Binarize(milImage, milBinImage, MIL.M_IN_RANGE, LowParam, HighParam);
                    ProcessImage.SetMilImage(milBinImage);
                }
            }
        }

        //=================================================================
        public override void ThresholdMedian(ProcessingImage ProcessImage, int nbMorpho, int contrastThreshold)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            using (MilImage MilMaxImage = new MilImage())
            using (MilImage MilMinImage = new MilImage())
            using (MilImage milBinImage = new MilImage())
            {
                MilMaxImage.Alloc2dCompatibleWith(milImage);
                MilMinImage.Alloc2dCompatibleWith(milImage);

                // Grayscale dilation is the same as finding the maximum value in a neighborhood.
                MilImage.Dilate(milImage, MilMaxImage, nbMorpho, MIL.M_GRAYSCALE);

                // Grayscale erosion is the same as finding the minimum value in a neighborhood.
                MilImage.Erode(milImage, MilMinImage, nbMorpho, MIL.M_GRAYSCALE);

                // m_MilMinImage := (Max + Min)/2 (a kind of average)
                MilImage.ArithMultiple(MilMaxImage, 2.0, MilMinImage, MIL.M_NULL, MIL.M_NULL, MilMinImage, MIL.M_WEIGHTED_AVERAGE, MIL.M_DEFAULT);
                MilMaxImage.Dispose();

                // m_MilMinImage := Source - (Max + Min)/2, saturated to 0
                MilImage.Arith(milImage, MilMinImage, milImage, MIL.M_SUB + MIL.M_SATURATION);
                MilMinImage.Dispose();

                // Binarized pixel is white if   ( Source - (Max + Min)/2 ) > CONTRAST_THRESHOLD
                milBinImage.Alloc2d(milImage.OwnerSystem, milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, milImage.Attribute);
                MilImage.Binarize(milImage, milBinImage, MIL.M_FIXED + MIL.M_GREATER_OR_EQUAL, contrastThreshold, MIL.M_NULL);
                ProcessImage.SetMilImage(milBinImage);
            }
        }

        //=================================================================
        public override void ThresholdMultiRange(ProcessingImage ProcessImage, double[] LowParam, double[] HighParam)
        {
            if (LowParam.Length != HighParam.Length)
                throw new ApplicationException("Hi & Low arrays Paremeters have not the same length");

            MilImage milImage = ProcessImage.GetMilImage();

            using (MilImage milTmp = new MilImage())
            using (MilImage milAdd = new MilImage())
            {
                milTmp.Alloc2dCompatibleWith(milImage);
                milAdd.Alloc2dCompatibleWith(milImage);
                milAdd.Clear(0.0);

                int nNbRanges = LowParam.Length;
                for (int k = 0; k < nNbRanges - 1; k++)
                {
                    MIL.MimBinarize(milImage, milTmp, MIL.M_IN_RANGE, LowParam[k], HighParam[k]);
                    MIL.MimArith(milTmp.MilId, milAdd.MilId, milAdd.MilId, MIL.M_ADD + MIL.M_SATURATION);
                }

                MIL.MimBinarize(milImage, milTmp, MIL.M_IN_RANGE, LowParam[nNbRanges - 1], HighParam[nNbRanges - 1]);
                MIL.MimArith(milTmp.MilId, milAdd.MilId, milImage.MilId, MIL.M_ADD + MIL.M_SATURATION);

                using (MilImage binImage = new MilImage())
                {
                    binImage.Alloc2d(milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                    MilImage.Binarize(milImage, binImage, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);
                    ProcessImage.SetMilImage(binImage);
                }
            }
            MilImage.checkMilError("ThresholdMultiRange");
        }

        //=================================================================
        public override void Binarisation(ProcessingImage ProcessImage, double Value)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            milImage.Binarize(MIL.M_GREATER_OR_EQUAL, Value, MIL.M_NULL);
        }
        //=================================================================
        public override void Close(ProcessingImage ProcessImage, enKindOfPicture KindOfPicture, int NbIterations)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (KindOfPicture)
            {
                case enKindOfPicture.Binary:
                    milImage.Close(NbIterations, MIL.M_BINARY);
                    break;
                case enKindOfPicture.GreyLevel:
                    milImage.Close(NbIterations, MIL.M_GREYSCALE);
                    break;
            }
        }
        //=================================================================
        public override void Sobel(ProcessingImage ProcessImage, enTypeOfSobel SobelType)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (SobelType)
            {
                case enTypeOfSobel.Average:
                    milImage.EdgeDetect(MIL.M_SOBEL, MIL.M_DEFAULT, MIL.M_NULL);
                    break;
                case enTypeOfSobel.Gradiant:
                    milImage.EdgeDetect(MIL.M_SOBEL, MIL.M_REGULAR_EDGE_DETECT, MIL.M_NULL);
                    break;
                case enTypeOfSobel.Horizontal:
                    milImage.Convolve(MIL.M_HORIZ_EDGE);
                    break;
                case enTypeOfSobel.Vertical:
                    milImage.Convolve(MIL.M_VERT_EDGE);
                    break;
            }
        }
        //=================================================================
        public override void Laplacian(ProcessingImage ProcessImage, enTypeOfLaplacian LaplacianType)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (LaplacianType)
            {
                case enTypeOfLaplacian.LaplacianLow:
                    milImage.Convolve(MIL.M_LAPLACIAN_EDGE);
                    break;
                case enTypeOfLaplacian.LaplacianHigh:
                    milImage.Convolve(MIL.M_LAPLACIAN_EDGE2);
                    break;
            }
        }
        //=================================================================
        public override void Sharpen(ProcessingImage ProcessImage, enTypeOfSharpen SharpenType)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (SharpenType)
            {
                case enTypeOfSharpen.SharpenLow:
                    milImage.Convolve(MIL.M_SHARPEN2);
                    break;
                case enTypeOfSharpen.SharpenHigh:
                    milImage.Convolve(MIL.M_SHARPEN);
                    break;
            }
        }
        //=================================================================
        public override void Prewitt(ProcessingImage ProcessImage)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            milImage.Convolve(MIL.M_EDGE_DETECT2);
        }
        //=================================================================
        public override void LowPass(ProcessingImage ProcessImage)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            milImage.Convolve(MIL.M_SMOOTH);
        }
        //=================================================================
        public override void Inversion(ProcessingImage ProcessImage)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            using (MilImage operandImg = new MilImage())
            {
                operandImg.Alloc2d(milImage.OwnerSystem, milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                operandImg.Clear(0xff);

                MilImage.Arith(milImage, operandImg.MilId, milImage, MIL.M_XOR);
            }
        }
        //=================================================================
        public override void MaskRemove(ProcessingImage ProcessImage, String pathfile)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            MilImage MaskImage = new MilImage();
            MaskImage.Restore(pathfile);

            MilImage.Arith(milImage, MaskImage, milImage, MIL.M_AND);
        }
        //=================================================================
        public override void Equalization(ProcessingImage ProcessImage, enEqualizationOperation Operation, double lfAlpha, double lfMinValue, double lfMaxValue)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            switch (Operation)
            {
                case enEqualizationOperation.Exponential:
                    milImage.Equalization(MIL.M_EXPONENTIAL, lfAlpha, lfMinValue, lfMaxValue);
                    break;
                case enEqualizationOperation.HyperCubeRoot:
                    milImage.Equalization(MIL.M_HYPER_CUBE_ROOT, 0, lfMinValue, lfMaxValue);
                    break;
                case enEqualizationOperation.HyperLogarithm:
                    milImage.Equalization(MIL.M_HYPER_LOG, 0, lfMinValue, lfMaxValue);
                    break;
                case enEqualizationOperation.RayLeigh:
                    milImage.Equalization(MIL.M_RAYLEIGH, lfAlpha, lfMinValue, lfMaxValue);
                    break;
                case enEqualizationOperation.Uniform:
                    milImage.Equalization(MIL.M_UNIFORM, 0, lfMinValue, lfMaxValue);
                    break;
            }
        }
        //=================================================================
        public override void RemoveBorderBand(ProcessingImage ProcessImage, int removeSize)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            byte[] inImage = new byte[milImage.SizeX * milImage.SizeY];

            milImage.Get(inImage);

            // bord gauche
            for (int i = 0; i < removeSize; i++)
            {
                for (int j = 0; j < milImage.SizeY; j++)
                {
                    inImage[i + j * milImage.SizeX] = (byte)LibProcessing.Constants.constBlackGreyLevel;
                }
            }

            // bord haut
            for (int i = 0; i < milImage.SizeX; i++)
            {
                for (int j = 0; j < removeSize; j++)
                {
                    inImage[i + j * milImage.SizeX] = (byte)LibProcessing.Constants.constBlackGreyLevel;
                }
            }

            // bord droit
            for (int i = milImage.SizeX - removeSize - 1; i < milImage.SizeX; i++)
            {
                for (int j = 0; j < milImage.SizeY; j++)
                {
                    inImage[i + j * milImage.SizeX] = (byte)LibProcessing.Constants.constBlackGreyLevel;
                }
            }

            // bord bas
            for (int i = 0; i < milImage.SizeX; i++)
            {
                for (int j = milImage.SizeY - removeSize - 1; j < milImage.SizeY; j++)
                {
                    inImage[i + j * milImage.SizeX] = (byte)LibProcessing.Constants.constBlackGreyLevel;
                }
            }

            milImage.Put(inImage);

            inImage = null; // delete
        }
        //=================================================================
        public override double GetGreyLevelAverage(ProcessingImage ProcessImage, int MinimumValue)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            double lfAverage = 0.0;
            using (MilImageResult milStat = new MilImageResult())
            {

                milStat.AllocResult(milImage.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(milImage, MilTo.StatList(MIL.M_STAT_MEAN), MIL.M_IN_RANGE, MinimumValue, Constants.constWhiteGreyLevel);
                lfAverage = milStat.GetResult(MIL.M_STAT_MEAN);
            }

            return lfAverage;
        }

        //=================================================================
        public override int GetMedianValue(ProcessingImage ProcessImage, int MinimumValue)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            //------------------------------        
            var cumulativeHistoValues = MilImageResult.Histogram(milImage, Constants.constGreyLevelNbByte, true);
            var N = cumulativeHistoValues.Last();
            var halfN = N / 2;
            int medianValue = 0;
            foreach (var cvalue in cumulativeHistoValues)
            {
                if (cvalue >= halfN)
                {
                    break;
                }
                ++medianValue;
            }
            return medianValue;
        }

        //=================================================================
        public override double GetGreyLevelMedian(ProcessingImage ProcessImage, int MinimumValue)
        {
            MilImage milImage = ProcessImage.GetMilImage();

            //------------------------------
            double lfAverage = 0.0;
            var histoValues = MilImageResult.Histogram(milImage, Constants.constGreyLevelNbByte, true);

            //TODO = Calculate Median average with histogram -- ????
            return lfAverage;
        }

        public override ProcessingImage Resize(ProcessingImage processImage, int scaleXFactor, int scaleYFactor)
        {
            MilImage milImage = processImage.GetMilImage();
            ProcessingImage result = new ProcessingImage();
            using (MilImage compressedImage = new MilImage())
            {
                int compressionImageSizeX = milImage.SizeX / scaleXFactor;
                int compressionImageSizeY = milImage.SizeY / scaleYFactor;
                compressedImage.Alloc2d(compressionImageSizeX, compressionImageSizeY, milImage.Type, milImage.Attribute);
                MIL.MimResize(milImage.MilId, compressedImage.MilId, MIL.M_FILL_DESTINATION, MIL.M_FILL_DESTINATION, MIL.M_INTERPOLATE + MIL.M_REGULAR);
                result.SetMilImage(compressedImage);
            }

            return result;
        }

        public override ProcessingImage Rotate(ProcessingImage processImage)
        {
            MilImage milImage = processImage.GetMilImage();
            ProcessingImage result = new ProcessingImage();
            using (MilImage rotationImage = new MilImage())
            {
                int rotateImageSizeX = milImage.SizeY;
                int rotateImageSizeY = milImage.SizeX;
                rotationImage.Alloc2d(rotateImageSizeX, rotateImageSizeY, milImage.Type, milImage.Attribute);
                MIL.MimRotate(milImage.MilId, rotationImage.MilId, 90.0, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT, MIL.M_DEFAULT);
                result.SetMilImage(rotationImage);
            }

            return result;
        }

        public ProcessingImage Crop(ProcessingImage processImage, int xOrigin, int yOrigin, int width, int height)
        {
            var resultProcessingImage = (ProcessingImage)processImage.DeepClone();
            var milImage = resultProcessingImage.GetMilImage();
            var milChildBuffer = MIL.M_NULL;
            
            MIL.MbufAlloc2d(Mil.Instance.HostSystem,width, height, milImage.PixelDepth_bit + MIL.M_UNSIGNED, MIL.M_IMAGE+MIL.M_PROC, ref milChildBuffer);
            
            var milImageCurrentChildId = MIL.M_NULL;
            MIL.MbufChild2d(milImage, xOrigin, yOrigin, width,height ,ref milImageCurrentChildId);
            
            var newImage = new MilImage(milChildBuffer,false);

            MIL.MbufCopy(milImageCurrentChildId, newImage.MilId);
            MIL.MbufFree(milImageCurrentChildId);
            
            resultProcessingImage.SetMilImage(newImage);
            return resultProcessingImage;
        }

        //--------------------------------------
        static private int pow2roundup(int nInVal)
        {
            int val = nInVal; // Get input
            val--;
            val = (val >> 1) | val;
            val = (val >> 2) | val;
            val = (val >> 4) | val;
            val = (val >> 8) | val;
            val = (val >> 16) | val;
            val++;
            return val;
        }

        //--------------------------------------  
        static private float[] GaussianKernelLineCoefs(double dRadius_px, out int nKernelSize, out int nKernelCenter, bool bnormalize = false)
        {

            //the standard deviation corresponds to the half width of the peak at about 60% of the full height. 
            //In some applications, however, the full width at half maximum (FWHM) is often used instead. This is somewhat larger than sigma and can easily be shown to be :
            // FWHM = 2*Sigma * Sqrt(2*Ln(2)) ~ 2.35* sigma

            // lets assume half max should should half diameter hence
            double dSigma = dRadius_px / (2.0 * Math.Sqrt(2.0 * Math.Log(2.0)));
            nKernelSize = (int)Math.Floor(3.0 * dRadius_px);
            if (nKernelSize % 2 == 0)
                nKernelSize++; // on veux être impair
            int nRadius = nKernelSize / 2;

            float[] buf = new float[nKernelSize];
            double d2Sigma2 = -1.0 / (2.0 * dSigma * dSigma);

            float fSum = 0.0f;
            for (int k = 0; k < buf.Length; k++)
            {
                double x = k - (nKernelSize - 1) * 0.5;
                double dVal = Math.Exp(x * x * d2Sigma2);
                buf[k] = (float)dVal;
                fSum += buf[k];
            }

            if (bnormalize)
            {
                fSum = 1.0f / fSum;
                // Normalize so that total area (sum of all weights) is 1
                for (int k = 0; k < buf.Length; k++)
                {
                    buf[k] *= fSum;
                }
            }

            nKernelCenter = nRadius;
            return buf;
        }
        //--------------------------------------  
        static private float[,] MexhatKernel2DCoefs(double dRadius_px, out int nKernelSize, out int nKernelCenter, bool bnormalize = false)
        {
            //mexhat kernel
            //I don't know how to write C# code so everything is derived from the Gaussian code

            /* the sigma value is chosen so that the max sensitivity is around dRadius_px
            this value is not exact as my test show that the scaling should actually be non linear
            but it's good enough */
            double dSigma = dRadius_px * 0.5 ;
            nKernelSize = (int)Math.Floor(4.0 * dRadius_px);
            if (nKernelSize % 2 == 0)
                nKernelSize++; // on veux être impair
            int nRadius = nKernelSize / 2;

            /*this normalisation value is chosen so that the signal of a defect of brightness 1 against a background 0
            gives a max signal around 1 (not exact) when the radius matches a bar shaped defect size
            if the defect has a 2d shape then the value will be higher(eg I get ~1.5 for a square defect)
            the edges of the bar shaped defect will give a signal of 0.5 when the defect size is much larger then the radius */
            double dNorm = 2.63 / (dRadius_px * dRadius_px);

            float[,] buf = new float[nKernelSize, nKernelSize];
            double dSigma2 = 1.0 / (dSigma * dSigma);

            for (int i = 0; i < nKernelSize; i++)
                for (int j = 0; j < nKernelSize; j++)
                {
                    double x = i - (nKernelSize - 1) * 0.5;
                    double y = j - (nKernelSize - 1) * 0.5;
                    double dVal = dNorm * (1 - (x * x + y * y) * 0.5 *  dSigma2) * Math.Exp((x * x  + y * y ) * -0.5 * dSigma2);
                    buf[i,j] = (float)dVal;
            }

            nKernelCenter = nRadius;
            return buf;
        }
        //--------------------------------------  
        public override void FFTRemovePattern(ProcessingImage ProcessImage,
            System.Drawing.Rectangle WaferRect_px, System.Drawing.PointF WaferCenter_px,
            double dEdgeremoveRadius_px,
            double FFTPeaksThresholdRatio,
            int FrequencyTolerance, 
            bool GenDarkImg,
            bool SaveFTTImg,
            bool bAdvDebugExpert, String DebugRecPath, bool DisableSaves)
        {
            MilImage milImage = ProcessImage.GetMilImage();
            MIL_ID milSys = milImage.OwnerSystem;

            // Input Image initial parameters
            int nW_in = milImage.SizeX;
            int nH_in = milImage.SizeY;
            int nFFTSize = Math.Max(pow2roundup(nW_in), pow2roundup(nH_in));
            int delta_X = (nFFTSize - nW_in) / 2;
            int delta_Y = (nFFTSize - nH_in) / 2;
            double dMiddleFFT = (double)nFFTSize / 2.0 - 1.0;

            MIL_ID MilStatContextId = MIL.M_NULL;
            MIL_ID MilStatResult = MIL.M_NULL;
            MIL.MimAlloc(milSys, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref MilStatContextId);
            MIL.MimAllocResult(milSys, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref MilStatResult);

            // creation buffer float entrée
            MIL_ID milInput_Float = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milInput_Float);
            MIL.MbufClear(milInput_Float, 0.0);
            // creation mask FFT (sert aussi de matrice intermédiaire pour la cration de l'input mosaic fft)
            MIL_ID milFFT_Msk = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Msk);
            MIL.MbufCopyClip(milImage.MilId, milFFT_Msk, delta_X, delta_Y);

            // Création des mask d'erosion basé sur le mask de edge remove
            MIL_ID MilErodeMsk = MIL.M_NULL;
            MIL_ID MilErodeMskINV = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilErodeMsk);
            MIL.MbufClear(MilErodeMsk, 0.0);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilErodeMskINV);

            MIL.MgraColor(MIL.M_DEFAULT, 1.0);
            MIL.MgraArcFill(MIL.M_DEFAULT, MilErodeMsk, WaferCenter_px.X + delta_X, WaferCenter_px.Y + delta_Y, dEdgeremoveRadius_px, dEdgeremoveRadius_px, 0.0, 360.0);
            MIL_ID ChildErodeMsk = MIL.M_NULL;
            //left
            MIL.MbufChild2d(MilErodeMsk, 0, 0, WaferRect_px.Left + delta_X, nFFTSize, ref ChildErodeMsk);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // right
             MIL.MbufChildMove(ChildErodeMsk, WaferRect_px.Right + delta_X, MIL.M_DEFAULT, nFFTSize - (WaferRect_px.Right + delta_X), MIL.M_DEFAULT, MIL.M_DEFAULT);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // top middle
            MIL.MbufChildMove(ChildErodeMsk, WaferRect_px.Left + delta_X, MIL.M_DEFAULT, MIL.M_DEFAULT, WaferRect_px.Top + delta_Y, MIL.M_DEFAULT);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // bottom middle
            MIL.MbufChildMove(ChildErodeMsk, MIL.M_DEFAULT, WaferRect_px.Bottom + delta_Y, MIL.M_DEFAULT, nFFTSize - (WaferRect_px.Bottom + delta_Y), MIL.M_DEFAULT);

            MIL.MbufClear(ChildErodeMsk, 0.0);
            if (ChildErodeMsk != MIL.M_NULL)
            {
                MIL.MbufFree(ChildErodeMsk);
                ChildErodeMsk = MIL.M_NULL;
            }
            MIL.MimArith(MilErodeMsk, -1.0, MilErodeMskINV, MIL.M_MULT_CONST);
            MIL.MimArith(MilErodeMskINV, 1.0, MilErodeMskINV, MIL.M_ADD_CONST);

            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FltErodeMskbin.usc", DebugRecPath), MIL.M_BMP, MilErodeMsk);
            }

            // Création de l'image Mosaique d'entrée de FTT
            // on répete le motifs central du wafer en fond de notre image puis on y insère notre image d'entrée 
            MIL_ID milInnerPattern = MIL.M_NULL;

            // valeur en dur on prends le rectangle contenu dans le cercle du edge remove à "dPercentInnerValue" prés
            double dPercentInnerValue = 0.9;
            int nInnerPatSize = (int)Math.Floor(dPercentInnerValue * 2.0 * dEdgeremoveRadius_px / Math.Sqrt(2.0));
            MIL.MbufAlloc2d(milSys, nInnerPatSize, nInnerPatSize, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milInnerPattern);
            MIL.MbufCopyColor2d(milImage.MilId, milInnerPattern,
                MIL.M_ALL_BANDS, (nW_in - nInnerPatSize) / 2, (nH_in - nInnerPatSize) / 2,
                MIL.M_ALL_BANDS, 0, 0,
                nInnerPatSize, nInnerPatSize);

            int npatXnbet = (int)nFFTSize / nInnerPatSize + 1;
            int npatYnbet = (int)nFFTSize / nInnerPatSize + 1;

            for (int y = 0; y < npatYnbet; y++)
            {
                for (int x = 0; x < npatXnbet; x++)
                {
                    MIL.MbufCopyClip(milInnerPattern, milInput_Float, x * nInnerPatSize, y * nInnerPatSize);
                }
            }
            MIL.MimConvolve(milInput_Float, milInput_Float, MIL.M_SMOOTH); // on lisse pour adoucire les effets jointure et ne pas créer des fréquence trop marquée

            // on ajout l'image d'entrée détouré de l'edge remove au fond masaic
            MIL.MimArith(milFFT_Msk, MilErodeMsk, milFFT_Msk, MIL.M_MULT);
            MIL.MimArith(milInput_Float, MilErodeMskINV, milInput_Float, MIL.M_MULT);
            MIL.MimArith(milInput_Float, milFFT_Msk, milInput_Float, MIL.M_ADD);

            // on libére Le pattern wafer inner
            if (milInnerPattern != MIL.M_NULL)
            {
                MIL.MbufFree(milInnerPattern);
                milInnerPattern = MIL.M_NULL;
            }

            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FltIn.usc", DebugRecPath), MIL.M_BMP, milInput_Float);
            }

            // on creet les matrice auxiliare nécéssaire à la FFT
            MIL_ID milFFT_Real = MIL.M_NULL;
            MIL_ID milFFT_Im = MIL.M_NULL; // this buffer will be also be used as intermédiate computation buffer some time to time
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Real);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Im);

            MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE);
            double dgauss = (double)nFFTSize * 0.01;
            int nkgausssize; int nkgausscenter;
            MIL_ID magfilter = milFFT_Msk; // attention on evite une nouvelle alloc  et pour lisibilté on chnage sont nom mag filter ne doit pas être free

            // on filtre la magnitude avec une gaussienne
            // we are using seprable gaussian filtering to optimize computation
            float[] GaussFilterCoefs = GaussianKernelLineCoefs(dgauss, out nkgausssize, out nkgausscenter, true);
            MIL_ID gaussfilterkernel_H = MIL.M_NULL;
            MIL_ID gaussfilterkernel_V = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nkgausssize, 1, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref gaussfilterkernel_H);
            MIL.MbufPut(gaussfilterkernel_H, GaussFilterCoefs);
            MIL.MbufAlloc2d(milSys, 1, nkgausssize, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref gaussfilterkernel_V);
            MIL.MbufPut(gaussfilterkernel_V, GaussFilterCoefs);

            // we are using milFFT_Im as auxilary matrix fro intermediate results
            MIL.MimConvolve(milFFT_Real, milFFT_Im, gaussfilterkernel_V);
            MIL.MimConvolve(milFFT_Im, magfilter, gaussfilterkernel_H);
            // on libere les kernels
            if (gaussfilterkernel_V != MIL.M_NULL)
            {
                MIL.MbufFree(gaussfilterkernel_V);
                gaussfilterkernel_V = MIL.M_NULL;
            }
            if (gaussfilterkernel_H != MIL.M_NULL)
            {
                MIL.MbufFree(gaussfilterkernel_H);
                gaussfilterkernel_H = MIL.M_NULL;
            }

            // on fait la différence entre la mgnitude et la magnitude filtrée
            MIL.MimArith(milFFT_Real, magfilter, magfilter, MIL.M_SUB + MIL.M_SATURATION);

            // on suprime la zone du pic centrale de la FFT
            MIL_ID childMagFilter = MIL.M_NULL;
            MIL.MbufChild2d(magfilter, (int)(dMiddleFFT - dgauss * 0.5), (int)(dMiddleFFT - dgauss * 0.5), (int)(2.0 * dgauss * 0.5 + 1.0), (int)(2.0 * dgauss * 0.5 + 1.0), ref childMagFilter);
            MIL.MbufClear(childMagFilter, 0.0);

            if (!DisableSaves && (bAdvDebugExpert || SaveFTTImg))
            {
                // calcul du LOG SCALE
                MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE + MIL.M_LOG_SCALE);
                MIL.MbufExport(String.Format(@"{0}Dbg_FFTLogMagnitude.bmp", DebugRecPath), MIL.M_BMP, milFFT_Real);
            }

            MIL.MimControl(MilStatContextId, MIL.M_STAT_STANDARD_DEVIATION, MIL.M_ENABLE);
            MIL.MimControl(MilStatContextId, MIL.M_STAT_MEAN, MIL.M_ENABLE);
            MIL.MimControl(MilStatContextId, MIL.M_CONDITION, MIL.M_GREATER_OR_EQUAL);
            MIL.MimControl(MilStatContextId, MIL.M_COND_LOW, 0.0);

            MIL.MimStatCalculate(MilStatContextId, magfilter, MilStatResult, MIL.M_DEFAULT);
            double dStdfft = 0.0;
            MIL.MimGetResult(MilStatResult, MIL.M_STAT_STANDARD_DEVIATION, ref dStdfft);
            double dMeanFft = 0.0;
            MIL.MimGetResult(MilStatResult, MIL.M_STAT_MEAN, ref dMeanFft);
            double dThFFTRun = 0.0;
            dThFFTRun = dMeanFft + FFTPeaksThresholdRatio * dStdfft;
            // creation du mask de frequence à couper (1= frequence que l'on garde, 0= frequence que 'lon coupe) /!\ magfilter et milFFT_Msk sont le meme buffer
            MIL.MimBinarize(magfilter, milFFT_Msk, MIL.M_FIXED + MIL.M_LESS_OR_EQUAL, dThFFTRun, MIL.M_NULL);

            bool bRemoveFFTcenter = GenDarkImg;
            if (bRemoveFFTcenter)
            {
                MIL.MbufClear(childMagFilter, 0.0); // c'est le child du buffer milFFT_Msk
            }

            if (childMagFilter != MIL.M_NULL)
            {
                MIL.MbufFree(childMagFilter);
                childMagFilter = MIL.M_NULL;
            }
            magfilter = MIL.M_NULL; // on s'en servira plus

            if (FrequencyTolerance > 0)
            {
                MIL.MimErode(milFFT_Msk, milFFT_Im, FrequencyTolerance, MIL.M_BINARY);
                for (int i = 0; i < 2 * FrequencyTolerance; i++)
                    MIL.MimConvolve(milFFT_Im, milFFT_Im, MIL.M_SMOOTH);
                MIL.MimArith(milFFT_Im, milFFT_Msk, milFFT_Msk, MIL.M_MULT);
            }

            if (!DisableSaves && (bAdvDebugExpert || SaveFTTImg))
            {
                MIL.MimArith(milFFT_Msk, 1.0, milFFT_Im, MIL.M_SUB_CONST);
                MIL.MimArith(milFFT_Im, -255.0, milFFT_Im, MIL.M_MULT_CONST + MIL.M_SATURATION);
                MIL.MbufExport(String.Format(@"{0}Dbg_FFTMask.bmp", DebugRecPath), MIL.M_BMP, milFFT_Im);
            }

            //
            // Calcul de la FFT
            //
            MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER);
            // application du mask sur les frequences
            MIL.MimArith(milFFT_Real, milFFT_Msk, milFFT_Real, MIL.M_MULT);
            MIL.MimArith(milFFT_Im, milFFT_Msk, milFFT_Im, MIL.M_MULT);
            // Calcul de la FFT inverse
            MIL_ID MilOut_Real = milInput_Float;    // to avoid realloc
            MIL_ID MilOut_Im = milFFT_Msk;          // to avoid realloc
            MIL.MimTransform(milFFT_Real, milFFT_Im, MilOut_Real, MilOut_Im, MIL.M_FFT, MIL.M_REVERSE + MIL.M_CENTER);
            // get magnitude outputs
            MIL.MimArith(MilOut_Real, MIL.M_NULL, MilOut_Real, MIL.M_SQUARE + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Im, MIL.M_NULL, MilOut_Im, MIL.M_SQUARE + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Real, MilOut_Im, MilOut_Real, MIL.M_ADD + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Real, MIL.M_NULL, MilOut_Real, MIL.M_SQUARE_ROOT + MIL.M_SATURATION);

            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FFTInverseMagnitude.usc", DebugRecPath), MIL.M_BMP, MilOut_Real);
            }

            MIL.MimArith(MilOut_Real, MilErodeMsk, MilOut_Real, MIL.M_MULT);
            MIL.MbufCopyClip(MilOut_Real, milImage.MilId, -delta_X, -delta_Y);

            // On libère toute la mémoire alloué
            //
            if (milFFT_Im != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Im);
                milFFT_Im = MIL.M_NULL;
            }
            if (milFFT_Real != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Real);
                milFFT_Real = MIL.M_NULL;
            }
            if (MilErodeMskINV != MIL.M_NULL)
            {
                MIL.MbufFree(MilErodeMskINV);
                MilErodeMskINV = MIL.M_NULL;
            }
            if (MilErodeMsk != MIL.M_NULL)
            {
                MIL.MbufFree(MilErodeMsk);
                MilErodeMsk = MIL.M_NULL;
            }
            if (milFFT_Msk != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Msk);
                milFFT_Msk = MIL.M_NULL;
            }
            if (milInput_Float != MIL.M_NULL)
            {
                MIL.MbufFree(milInput_Float);
                milInput_Float = MIL.M_NULL;
            }
            if (MilStatResult != MIL.M_NULL)
            {
                MIL.MimFree(MilStatResult);
                MilStatResult = MIL.M_NULL;
            }
            if (MilStatContextId != MIL.M_NULL)
            {
                MIL.MimFree(MilStatContextId);
                MilStatContextId = MIL.M_NULL;
            }
        }
        
        public override void FFTRemovePatternWithComments(ProcessingImage ProcessImage,
            System.Drawing.Rectangle WaferRect_px, System.Drawing.PointF WaferCenter_px,
            double dEdgeremoveRadius_px,
            double FFTPeaksThresholdRatio,
            int FrequencyTolerance,
            bool GenDarkImg,
            bool SaveFTTImg,
            bool bAdvDebugExpert, String DebugRecPath, bool DisableSaves)
            // all the params
        {
            // i guess milImage is the image to process
            MilImage milImage = ProcessImage.GetMilImage();
            MIL_ID milSys = milImage.OwnerSystem;
            // not sure why we get the system ?
            // seems necessary to allocate new buffers

            // Input Image initial parameters
            int nW_in = milImage.SizeX;
            int nH_in = milImage.SizeY;
            // round to nearest power of 2 for efficiency 
            int nFFTSize = Math.Max(pow2roundup(nW_in), pow2roundup(nH_in));
            int delta_X = (nFFTSize - nW_in) / 2;
            int delta_Y = (nFFTSize - nH_in) / 2;
            double dMiddleFFT = (double)nFFTSize / 2.0 - 1.0;
            // keep track of the localation of the center and necessary offset (for original image I guess) 


            // not sure whet MilStatResult is about
            MIL_ID MilStatContextId = MIL.M_NULL;
            MIL_ID MilStatResult = MIL.M_NULL;
            MIL.MimAlloc(milSys, MIL.M_STATISTICS_CONTEXT, MIL.M_DEFAULT, ref MilStatContextId);
            MIL.MimAllocResult(milSys, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT, ref MilStatResult);

            // creation buffer float entrée
            MIL_ID milInput_Float = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milInput_Float);
            MIL.MbufClear(milInput_Float, 0.0); // write 0 to the whole buffer ?
            // creation mask FFT (sert aussi de matrice intermédiaire pour la cration de l'input mosaic fft)
            MIL_ID milFFT_Msk = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Msk);
            MIL.MbufCopyClip(milImage.MilId, milFFT_Msk, delta_X, delta_Y);


            // maybe not necessary for me ?
            // Création des mask d'erosion basé sur le mask de edge remove
            MIL_ID MilErodeMsk = MIL.M_NULL;
            MIL_ID MilErodeMskINV = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilErodeMsk);
            MIL.MbufClear(MilErodeMsk, 0.0);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref MilErodeMskINV);

            // what is this about ??
            // -> I think these a calls to draw some shapes in the buffers
            // here it seems we're drawing a circle to serve as a edge exclusion mask
            MIL.MgraColor(MIL.M_DEFAULT, 1.0);
            MIL.MgraArcFill(MIL.M_DEFAULT, MilErodeMsk, WaferCenter_px.X + delta_X, WaferCenter_px.Y + delta_Y, dEdgeremoveRadius_px, dEdgeremoveRadius_px, 0.0, 360.0);
            
            // now it seems we're writing zeros to some parts of the ErodeMask, maybe the edges, not sure
            // I guess this is meant to handle the case where the part of the wafer is outside of the original image
            // so we have to enforce the zones outside the original image to be excluded because they can't contain data
            MIL_ID ChildErodeMsk = MIL.M_NULL;
            //left
            MIL.MbufChild2d(MilErodeMsk, 0, 0, WaferRect_px.Left + delta_X, nFFTSize, ref ChildErodeMsk);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // right
            MIL.MbufChildMove(ChildErodeMsk, WaferRect_px.Right + delta_X, MIL.M_DEFAULT, nFFTSize - (WaferRect_px.Right + delta_X), MIL.M_DEFAULT, MIL.M_DEFAULT);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // top middle
            MIL.MbufChildMove(ChildErodeMsk, WaferRect_px.Left + delta_X, MIL.M_DEFAULT, MIL.M_DEFAULT, WaferRect_px.Top + delta_Y, MIL.M_DEFAULT);
            MIL.MbufClear(ChildErodeMsk, 0.0);
            // bottom middle
            MIL.MbufChildMove(ChildErodeMsk, MIL.M_DEFAULT, WaferRect_px.Bottom + delta_Y, MIL.M_DEFAULT, nFFTSize - (WaferRect_px.Bottom + delta_Y), MIL.M_DEFAULT);
            MIL.MbufClear(ChildErodeMsk, 0.0);

            // free the memory of the child
            if (ChildErodeMsk != MIL.M_NULL)
            {
                MIL.MbufFree(ChildErodeMsk);
                ChildErodeMsk = MIL.M_NULL;
            }

            // compute the imverted mask by multipliying by -1 and then adding 1
            MIL.MimArith(MilErodeMsk, -1.0, MilErodeMskINV, MIL.M_MULT_CONST);
            MIL.MimArith(MilErodeMskINV, 1.0, MilErodeMskINV, MIL.M_ADD_CONST);

            // this is some debbug stuff
            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FltErodeMskbin.usc", DebugRecPath), MIL.M_BMP, MilErodeMsk);
            }

            // now we're getting in the meat of the function
            // Création de l'image Mosaique d'entrée de FTT
            // on répete le motifs central du wafer en fond de notre image puis on y insère notre image d'entrée 
            MIL_ID milInnerPattern = MIL.M_NULL;

            //allocate a buffer that is strictly included in a the circle defined by the edge remove *0.9
            // valeur en dur on prends le rectangle contenu dans le cercle du edge remove à "dPercentInnerValue" prés
            double dPercentInnerValue = 0.9;
            int nInnerPatSize = (int)Math.Floor(dPercentInnerValue * 2.0 * dEdgeremoveRadius_px / Math.Sqrt(2.0));
            MIL.MbufAlloc2d(milSys, nInnerPatSize, nInnerPatSize, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC, ref milInnerPattern);
            // copy the data from the original image to the "innerValue" buffer, it's only a portion of the original image 
            MIL.MbufCopyColor2d(milImage.MilId, milInnerPattern,
                MIL.M_ALL_BANDS, (nW_in - nInnerPatSize) / 2, (nH_in - nInnerPatSize) / 2,
                MIL.M_ALL_BANDS, 0, 0,
                nInnerPatSize, nInnerPatSize);

            // we compute the ration bewteen the "innerValue" size and the full fft buffer size
            int npatXnbet = (int)nFFTSize / nInnerPatSize + 1;
            int npatYnbet = (int)nFFTSize / nInnerPatSize + 1;


            // we copy paste the inner portion over the whole image
            // MbufCopyClip copies the data while handling size mismatch between source and destination
            for (int y = 0; y < npatYnbet; y++)
            {
                for (int x = 0; x < npatXnbet; x++)
                {
                    MIL.MbufCopyClip(milInnerPattern, milInput_Float, x * nInnerPatSize, y * nInnerPatSize);
                }
            }
            // apply a smoothing pass to the tiled image
            MIL.MimConvolve(milInput_Float, milInput_Float, MIL.M_SMOOTH); // on lisse pour adoucire les effets jointure et ne pas créer des fréquence trop marquée

            // milInput_Float this a tiled version of the origanal image
            // MilErodeMsk this is a mask that covers the usfull wafer surface
            // milFFT_Msk contains the orignal image data, recentered
            // milInput_Float contains the tiled patern

            // on ajout l'image d'entrée détouré de l'edge remove au fond masaic
            MIL.MimArith(milFFT_Msk, MilErodeMsk, milFFT_Msk, MIL.M_MULT);
            MIL.MimArith(milInput_Float, MilErodeMskINV, milInput_Float, MIL.M_MULT);
            MIL.MimArith(milInput_Float, milFFT_Msk, milInput_Float, MIL.M_ADD);

            // after this operation milInput_Float contains the full original usefull data, and a the tiled pattern outside the usefull area




            // on libére Le pattern wafer inner
            if (milInnerPattern != MIL.M_NULL)
            {
                MIL.MbufFree(milInnerPattern);
                milInnerPattern = MIL.M_NULL;
            }

            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FltIn.usc", DebugRecPath), MIL.M_BMP, milInput_Float);
            }



            // on creet les matrice auxiliare nécéssaire à la FFT
            MIL_ID milFFT_Real = MIL.M_NULL;
            MIL_ID milFFT_Im = MIL.M_NULL; // this buffer will be also be used as intermédiate computation buffer some time to time
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Real);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Im);

            //  ------ interesting part --------
            // this computes the FFT 
            // void MimTransform( MIL_ID SrcImageRBufId, MIL_ID SrcImageIBufId, MIL_ID DstImageRBufId, MIL_ID DstImageIBufId, MIL_INT64 TransformType, MIL_INT64 ControlFlag ) 
            // MIL.M_FORWARD  = from real space to frequency space
            // M_CENTER  = center the frequency ino the image to SizeX/2-1
            // M_MAGNITUDE = return R**2 + I**2 instead of the MIL_ID DstImageRBufId
            // M_PHASE = returns the phase instead of MIL_ID DstImageRBufId, this option is neede if we want to compute the reverse FFT with the Magnitude
            MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE);
            double dgauss = (double)nFFTSize * 0.01;
            int nkgausssize; int nkgausscenter;
            MIL_ID magfilter = milFFT_Msk; // attention on evite une nouvelle alloc  et pour lisibilté on chnage sont nom mag filter ne doit pas être free


            // now we do a dumb convolve with a gaussian kernel and apply it on the FT we just computed

            // on filtre la magnitude avec une gaussienne
            // we are using seprable gaussian filtering to optimize computation
            float[] GaussFilterCoefs = GaussianKernelLineCoefs(dgauss, out nkgausssize, out nkgausscenter, true);
            MIL_ID gaussfilterkernel_H = MIL.M_NULL;
            MIL_ID gaussfilterkernel_V = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nkgausssize, 1, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref gaussfilterkernel_H);
            MIL.MbufPut(gaussfilterkernel_H, GaussFilterCoefs);
            // MbufPut allows the tranfer from an array we define to a MIL image buffer
            MIL.MbufAlloc2d(milSys, 1, nkgausssize, 32 + MIL.M_FLOAT, MIL.M_KERNEL, ref gaussfilterkernel_V);
            MIL.MbufPut(gaussfilterkernel_V, GaussFilterCoefs);

            // we are using milFFT_Im as auxilary matrix fro intermediate results
            MIL.MimConvolve(milFFT_Real, milFFT_Im, gaussfilterkernel_V);
            MIL.MimConvolve(milFFT_Im, magfilter, gaussfilterkernel_H);
            // on libere les kernels
            if (gaussfilterkernel_V != MIL.M_NULL)
            {
                MIL.MbufFree(gaussfilterkernel_V);
                gaussfilterkernel_V = MIL.M_NULL;
            }
            if (gaussfilterkernel_H != MIL.M_NULL)
            {
                MIL.MbufFree(gaussfilterkernel_H);
                gaussfilterkernel_H = MIL.M_NULL;
            }
            // computation is gauss(milFFT_Real) -> magfilter, and milFFT_Im was overwritten in the process

            // substraction to only keep the high "frequency" frequency patterns ?
            // on fait la différence entre la mgnitude et la magnitude filtrée
            MIL.MimArith(milFFT_Real, magfilter, magfilter, MIL.M_SUB + MIL.M_SATURATION);

            // I guess there is no direct acces to the image buffer so access is done through MbufClear and a mask crafted
            // this is used to wipe the center area of the FFT
            // on suprime la zone du pic centrale de la FFT
            MIL_ID childMagFilter = MIL.M_NULL;
            MIL.MbufChild2d(magfilter, (int)(dMiddleFFT - dgauss * 0.5), (int)(dMiddleFFT - dgauss * 0.5), (int)(2.0 * dgauss * 0.5 + 1.0), (int)(2.0 * dgauss * 0.5 + 1.0), ref childMagFilter);
            MIL.MbufClear(childMagFilter, 0.0);

            if (!DisableSaves && (bAdvDebugExpert || SaveFTTImg))
            {
                // calcul du LOG SCALE
                MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE + MIL.M_LOG_SCALE);
                MIL.MbufExport(String.Format(@"{0}Dbg_FFTLogMagnitude.bmp", DebugRecPath), MIL.M_BMP, milFFT_Real);
            }

            // here there are some stats about the image being computed
            // the mean and std over the image is comuted and a mask is defined to cut the values exceeding the given threshold (defined relative to the std)

            MIL.MimControl(MilStatContextId, MIL.M_STAT_STANDARD_DEVIATION, MIL.M_ENABLE);
            MIL.MimControl(MilStatContextId, MIL.M_STAT_MEAN, MIL.M_ENABLE);
            MIL.MimControl(MilStatContextId, MIL.M_CONDITION, MIL.M_GREATER_OR_EQUAL);
            MIL.MimControl(MilStatContextId, MIL.M_COND_LOW, 0.0);

            MIL.MimStatCalculate(MilStatContextId, magfilter, MilStatResult, MIL.M_DEFAULT);
            double dThFFTRun = 0.0;
            double dStdfft = 0.0;
            MIL.MimGetResult(MilStatResult, MIL.M_STAT_STANDARD_DEVIATION, ref dStdfft);
            double dMeanFft = 0.0;
            MIL.MimGetResult(MilStatResult, MIL.M_STAT_MEAN, ref dMeanFft);
            dThFFTRun = dMeanFft + FFTPeaksThresholdRatio * dStdfft;
            // creation du mask de frequence à couper (1= frequence que l'on garde, 0= frequence que 'lon coupe) /!\ magfilter et milFFT_Msk sont le meme buffer
            MIL.MimBinarize(magfilter, milFFT_Msk, MIL.M_FIXED + MIL.M_LESS_OR_EQUAL, dThFFTRun, MIL.M_NULL);


            bool bRemoveFFTcenter = GenDarkImg;
            if (bRemoveFFTcenter)
            {
                MIL.MbufClear(childMagFilter, 0.0); // c'est le child du buffer milFFT_Msk
            }

            if (childMagFilter != MIL.M_NULL)
            {
                MIL.MbufFree(childMagFilter);
                childMagFilter = MIL.M_NULL;
            }
            magfilter = MIL.M_NULL; // on s'en servira plus


            // I don't understand what is being done here and what the goal of this operation is
            if (FrequencyTolerance > 0)
            {
                MIL.MimErode(milFFT_Msk, milFFT_Im, FrequencyTolerance, MIL.M_BINARY);
                for (int i = 0; i < 2 * FrequencyTolerance; i++)
                    MIL.MimConvolve(milFFT_Im, milFFT_Im, MIL.M_SMOOTH);
                MIL.MimArith(milFFT_Im, milFFT_Msk, milFFT_Msk, MIL.M_MULT);
            }

            if (!DisableSaves && (bAdvDebugExpert || SaveFTTImg))
            {
                MIL.MimArith(milFFT_Msk, 1.0, milFFT_Im, MIL.M_SUB_CONST);
                MIL.MimArith(milFFT_Im, -255.0, milFFT_Im, MIL.M_MULT_CONST + MIL.M_SATURATION);
                MIL.MbufExport(String.Format(@"{0}Dbg_FFTMask.bmp", DebugRecPath), MIL.M_BMP, milFFT_Im);
            }

            //
            // Calcul de la FFT
            //
            MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Real, milFFT_Im, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER);
            // application du mask sur les frequences
            MIL.MimArith(milFFT_Real, milFFT_Msk, milFFT_Real, MIL.M_MULT);
            MIL.MimArith(milFFT_Im, milFFT_Msk, milFFT_Im, MIL.M_MULT);
            // Calcul de la FFT inverse
            MIL_ID MilOut_Real = milInput_Float;    // to avoid realloc
            MIL_ID MilOut_Im = milFFT_Msk;          // to avoid realloc
            MIL.MimTransform(milFFT_Real, milFFT_Im, MilOut_Real, MilOut_Im, MIL.M_FFT, MIL.M_REVERSE + MIL.M_CENTER);
            // get magnitude outputs
            MIL.MimArith(MilOut_Real, MIL.M_NULL, MilOut_Real, MIL.M_SQUARE + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Im, MIL.M_NULL, MilOut_Im, MIL.M_SQUARE + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Real, MilOut_Im, MilOut_Real, MIL.M_ADD + MIL.M_SATURATION);
            MIL.MimArith(MilOut_Real, MIL.M_NULL, MilOut_Real, MIL.M_SQUARE_ROOT + MIL.M_SATURATION);

            if (!DisableSaves && bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}AdvDbg_FFTInverseMagnitude.usc", DebugRecPath), MIL.M_BMP, MilOut_Real);
            }

            MIL.MimArith(MilOut_Real, MilErodeMsk, MilOut_Real, MIL.M_MULT);
            MIL.MbufCopyClip(MilOut_Real, milImage.MilId, -delta_X, -delta_Y);

            // On libère toute la mémoire alloué
            //
            if (milFFT_Im != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Im);
                milFFT_Im = MIL.M_NULL;
            }
            if (milFFT_Real != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Real);
                milFFT_Real = MIL.M_NULL;
            }
            if (MilErodeMskINV != MIL.M_NULL)
            {
                MIL.MbufFree(MilErodeMskINV);
                MilErodeMskINV = MIL.M_NULL;
            }
            if (MilErodeMsk != MIL.M_NULL)
            {
                MIL.MbufFree(MilErodeMsk);
                MilErodeMsk = MIL.M_NULL;
            }
            if (milFFT_Msk != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Msk);
                milFFT_Msk = MIL.M_NULL;
            }
            if (milInput_Float != MIL.M_NULL)
            {
                MIL.MbufFree(milInput_Float);
                milInput_Float = MIL.M_NULL;
            }
            if (MilStatResult != MIL.M_NULL)
            {
                MIL.MimFree(MilStatResult);
                MilStatResult = MIL.M_NULL;
            }
            if (MilStatContextId != MIL.M_NULL)
            {
                MIL.MimFree(MilStatContextId);
                MilStatContextId = MIL.M_NULL;
            }
        }
        
        public override void FFTConvolveMexhat(ProcessingImage ProcessImage,
            int widthKernel,
            bool SaveFTTImg,
            bool bAdvDebugExpert, String DebugRecPath)
        // all the params
        {


            // i guess milImage is the image to process
            MilImage milImage = ProcessImage.GetMilImage();
            MIL_ID milSys = milImage.OwnerSystem;
            // not sure why we get the system ?
            // seems necessary to allocate new buffers

            // Input Image initial parameters
            int nW_in = milImage.SizeX;
            int nH_in = milImage.SizeY;
            // round to nearest power of 2 for efficiency 
            int nFFTSize = Math.Max(pow2roundup(nW_in), pow2roundup(nH_in));
            int delta_X = (nFFTSize - nW_in) / 2;
            int delta_Y = (nFFTSize - nH_in) / 2;
            double dMiddleFFT = (double)nFFTSize / 2.0 - 1.0;
            // keep track of the localation of the center and necessary offset (for original image I guess) 


            // not sure whet MilStatResult is about
            //MIL_ID MilStatResult = MIL.M_NULL;
            //MIL.MimAllocResult(milSys, MIL.M_DEFAULT, MIL. M_STATISTICS_RESULT , ref MilStatResult);

            // creation buffer float entrée
            MIL_ID milInput_Float = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milInput_Float);
            MIL.MbufClear(milInput_Float, 0.0); // write 0 to the whole buffer ?

            // the strategy in the original code was to center everything
            // I have to check first if that actually works
            // other wise I'll try to not center, and in that configuration I think I just need to pay attention to the kernel original position
            // looking at the results using a radius of 25 result in an image of 50
            MIL.MbufCopyClip(milImage.MilId, milInput_Float, delta_X, delta_Y);
            // first step is a call to my coef generating function
            // I computing all the coef values over a relevant area (which in principle should be much smaller than the whole image)
            int sizeKernel; int centerKernel;
            float[,] MexHatKernel = MexhatKernel2DCoefs(widthKernel, out sizeKernel, out centerKernel, true);
            
            //I have to store the kernel coef in a first image buffer before putting it in the final kernel buffer
            MIL_ID milChildKernel = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, sizeKernel, sizeKernel, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milChildKernel);
            MIL.MbufPut(milChildKernel, MexHatKernel);


 

            // second step is to create the whole buffer that will conain the final kernel image
            // we zero all the values for starters
            MIL_ID milKernel = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milKernel);
            MIL.MbufClear(milKernel, 0.0);

            MIL_ID milMidKernel = MIL.M_NULL;
            MIL.MbufAlloc2d(milSys, nW_in, nH_in, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milMidKernel);
            MIL.MbufClear(milMidKernel, 0.0);

            // my test in python showed that I need to have the center of the small kernel located exactly at [0,0] relative to the original image
            // this is why in need to make an image with input dimentions, then copy th ekernel coeff in this and only then copy the result to the full buffer
            // this means the big kernel will be split in the four corner and that I need 4 allocations to fill it the big buffer
            // I checked the following offsets in python, I they're correct
            //MIL.MbufCopyClip(milChildKernel, milMidKernel, -centerKernel, -centerKernel);
            //MIL.MbufCopyClip(milChildKernel, milMidKernel, nW_in - centerKernel, -centerKernel);
            //MIL.MbufCopyClip(milChildKernel, milMidKernel, centerKernel, nH_in - centerKernel);
            //MIL.MbufCopyClip(milChildKernel, milMidKernel, nW_in - centerKernel, nH_in - centerKernel);

            MIL.MbufCopyClip(milChildKernel, milKernel, -centerKernel, -centerKernel);
            MIL.MbufCopyClip(milChildKernel, milKernel, nFFTSize - centerKernel, -centerKernel);
            MIL.MbufCopyClip(milChildKernel, milKernel, -centerKernel, nFFTSize - centerKernel);
            MIL.MbufCopyClip(milChildKernel, milKernel, nFFTSize - centerKernel, nFFTSize - centerKernel);


            //MIL.MbufCopyClip(milMidKernel, milKernel, delta_X, delta_Y);
            // milMidKernel was only usefull for this so I'll trash right away
            if (milMidKernel != MIL.M_NULL)
            {
                MIL.MbufFree(milMidKernel);
                milMidKernel = MIL.M_NULL;
            }

            //MIL.MbufCopyClip(milChildKernel, milKernel, delta_X-centerKernel, delta_Y-centerKernel);

            //Mexhat Kernel preparation is done
            // Ideally we would want to have the FFT of this kernel already computed and saved to disk
            // I'm expecting to use a few different kernel sizes and after the initial recipe krafting it is unlikely to change that often
            // Ideally what I would want is for the soft to try to load it from disk and fall back to recomputing the thing if not found, with an option to write the result to disk
            // maybe the option could be  "save intermediate result and reload on subsequent run" 
            // the only parameters we need to know are the size of the full image, and the kernel width
            if (bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}Kernel.tiff", DebugRecPath), MIL.M_MIL, milChildKernel);
                MIL.MbufExport(String.Format(@"{0}KernelFull.tiff", DebugRecPath), MIL.M_MIL, milKernel);
            }

            // allocate the buffer for the FFT computation
            MIL_ID milFFT_Magnitude = MIL.M_NULL;
            MIL_ID milFFT_Phase = MIL.M_NULL; // this buffer will be also be used as intermédiate computation buffer some time to time
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Magnitude);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_Phase); 
            
            MIL_ID milFFT_kernel_Magnitude = MIL.M_NULL;
            MIL_ID milFFT_kernel_Phase = MIL.M_NULL; // this buffer will be also be used as intermédiate computation buffer some time to time
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_kernel_Magnitude);
            MIL.MbufAlloc2d(milSys, nFFTSize, nFFTSize, 32 + MIL.M_FLOAT, MIL.M_IMAGE + MIL.M_PROC, ref milFFT_kernel_Phase);

            // this computes the FFT 
            // void MimTransform( MIL_ID SrcImageRBufId, MIL_ID SrcImageIBufId, MIL_ID DstImageRBufId, MIL_ID DstImageIBufId, MIL_INT64 TransformType, MIL_INT64 ControlFlag ) 
            // MIL.M_FORWARD  = from real space to frequency space
            // M_CENTER  = center the frequency ino the image to SizeX/2-1
            // M_MAGNITUDE = return R**2 + I**2 instead of the MIL_ID DstImageRBufId
            // M_PHASE = returns the phase instead of MIL_ID DstImageRBufId, this option is neede if we want to compute the reverse FFT with the Magnitude

            // I'll use Magnitude and Phase to simplify the multiplication of the FFTs
            MIL.MimTransform(milInput_Float, MIL.M_NULL, milFFT_Magnitude, milFFT_Phase, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE + MIL.M_PHASE);
            MIL.MimTransform(milKernel, MIL.M_NULL, milFFT_kernel_Magnitude, milFFT_kernel_Phase, MIL.M_FFT, MIL.M_FORWARD + MIL.M_CENTER + MIL.M_MAGNITUDE + MIL.M_PHASE);

            // we mutiply the result in the frequency space
            // amplitudes are multiplied and phases are added
            MIL.MimArith(milFFT_Magnitude, milFFT_kernel_Magnitude, milFFT_Magnitude, MIL.M_MULT);
            MIL.MimArith(milFFT_Phase, milFFT_kernel_Phase, milFFT_Phase, MIL.M_ADD);



            // we compute the inverse FFT to get the result and put it in the input buffer to save memory
            // we only keep the real part of the result so we dump the imaginary part into MIL.M_NULL
            MIL.MimTransform(milFFT_Magnitude, milFFT_Phase, milInput_Float, MIL.M_NULL, MIL.M_FFT, MIL.M_REVERSE + MIL.M_CENTER + MIL.M_MAGNITUDE + MIL.M_PHASE);

            if (SaveFTTImg)
            {
                // M_MIL format is a tiff with some extra info in the comment field
                MIL.MbufExport(String.Format(@"{0}FFTKernelMagnitude.tiff", DebugRecPath), MIL.M_MIL, milFFT_kernel_Magnitude);
                MIL.MbufExport(String.Format(@"{0}FFTKernelPhase.tiff", DebugRecPath), MIL.M_MIL, milFFT_kernel_Phase);
            }

            // milInput_Float contains the original image padded to 2**n so we have to put it back in the origian milbuffer at the right place
            // MbufCopyClip automatically handles the values that are outside of the index range
            MIL.MbufCopyClip(milInput_Float, milImage.MilId, -delta_X, -delta_Y);

            if (bAdvDebugExpert)
            {
                MIL.MbufExport(String.Format(@"{0}MexhatConvolveResult.tiff", DebugRecPath), MIL.M_MIL, milInput_Float);
                MIL.MbufExport(String.Format(@"{0}MexhatConvolveResultClipped.tiff", DebugRecPath), MIL.M_MIL, milImage.MilId);
            }
            // the processing is done we can free all the mil buffers
            // i guess the rest variables defined in this local scope will be handles by a garbage collector so i'll leave them be
            if (milInput_Float != MIL.M_NULL)
            {
                MIL.MbufFree(milInput_Float);
                milInput_Float = MIL.M_NULL;
            }
            if (milFFT_Magnitude != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Magnitude);
                milFFT_Magnitude = MIL.M_NULL;
            }
            if (milFFT_Phase != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_Phase);
                milFFT_Phase = MIL.M_NULL;
            }
            if (milFFT_kernel_Magnitude != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_kernel_Magnitude);
                milFFT_kernel_Magnitude = MIL.M_NULL;
            }
            if (milFFT_kernel_Phase != MIL.M_NULL)
            {
                MIL.MbufFree(milFFT_kernel_Phase);
                milFFT_kernel_Phase = MIL.M_NULL;
            }
            if (milChildKernel != MIL.M_NULL)
            {
                MIL.MbufFree(milChildKernel);
                milChildKernel = MIL.M_NULL;
            }
            if (milKernel != MIL.M_NULL)
            {
                MIL.MbufFree(milKernel);
                milKernel = MIL.M_NULL;
            }
        }

        public override void SetSubImage(ProcessingImage sourceImage, ProcessingImage destinationImage, int offestPosX, int offsetPosY)
        {
            MIL.MbufCopyClip(sourceImage.GetMilImage().MilId, destinationImage.GetMilImage().MilId, offestPosX, offsetPosY);
        }

        //=================================================================
        /// <summary>
        /// Fonction interne pour convertir l'image de 16 en 8 bits.
        /// La fonction externe est ConvertTo8bit.
        /// </summary>
        //=================================================================
        protected ProcessingImage Convert16bitTo8bit(ProcessingImage source)
        {
            //-------------------------------------------------------------
            // Get Source MilImage
            //-------------------------------------------------------------
            MilImage milSource = source.GetMilImage();
            if (milSource.SizeBit != 16)
                throw new ApplicationException("invalid image depth: " + milSource.SizeBit + " expected: 16");

            //-------------------------------------------------------------
            // Convertion
            //-------------------------------------------------------------
            using (var milStat = new MilImageResult())
            {                   
                milStat.AllocResult(milSource.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);
                milStat.Stat(milSource, MIL.M_STAT_MAX, MIL.M_STAT_MIN);
                int max = (int) milStat.GetResult(MIL.M_STAT_MAX);
                int min = (int) milStat.GetResult(MIL.M_STAT_MIN);

                using (MilImage milDest = new MilImage())
                {
                    milDest.Alloc2d(milSource.OwnerSystem, milSource.SizeX, milSource.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_PROC + MIL.M_IMAGE);

                    if (max < 255)
                    {
                        MilImage.Copy(milSource, milDest);
                    }
                    else
                    {
                        using (MilImage milTemp = (MilImage)milSource.DeepClone())
                        {
                            milTemp.Arith(min, MIL.M_SUB_CONST);
                            double factor = (max - min) / 256.0;
                            milTemp.Arith(factor, MIL.M_MULT_CONST);
                            MilImage.Copy(milTemp, milDest);
                        }
                    }

                    //-----------------------------------------------------
                    // Création de l'image résultat
                    //-----------------------------------------------------
                    ProcessingImage result = new ProcessingImage();
                    result.SetMilImage(milDest);
                    return result;
                }
            }
        }

       
    }
}
