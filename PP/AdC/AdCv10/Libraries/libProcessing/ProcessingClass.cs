using System;
using System.ComponentModel;

namespace LibProcessing
{
    //=================================================================
    // Constante tout module
    //=================================================================
    public static class Constants
    {
        public const int constWhiteGreyLevel = 255;
        public const int constBlackGreyLevel = 0;
        public const int constGreyLevelNbByte = 256;
        public const int constNaNValue = -100;
    }
    #region ExportableEnum
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enKindOfPicture
    {
        Binary = 0,
        GreyLevel
    }
    //=========================================================
    public enum enCharacteristicMILBlobFiltering
    {
        en_Breath = 0,
        en_Compactness,
        en_ConvexPerimeter,
        en_Elongation,
        en_EulerNumber,
        en_Perimeter,
        en_Roughness,
        en_Length,
        en_Area,
        en_AxisPrincipal
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfCondition
    {
        [Description("Equal")]
        en_Equal = 0,
        [Description("Greater")]
        en_Greater,
        [Description("Less")]
        en_less,
        [Description("Greater or equal")]
        en_GreaterOrEqual,
        [Description("Less or equal")]
        en_LessOrEqual,
        [Description("Not equal")]
        en_NotEqual
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfSobel
    {
        Average = 0,
        Gradiant,
        Vertical,
        Horizontal
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfConvolve
    {
        Prewitt = 0,
        LaplacianLow,
        LaplacianHigh,
        SharpenLow,
        SharpenHigh,
        Smooth
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfLaplacian
    {
        LaplacianHigh = 0,
        LaplacianLow
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfSharpen
    {
        SharpenHigh = 0,
        SharpenLow
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enEqualizationOperation
    {
        Exponential = 0,
        HyperCubeRoot,
        HyperLogarithm,
        RayLeigh,
        Uniform
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTypeOfTransition
    {
        en_AllTransition = 0,
        en_RisingEdge = 1,
        en_FallingEdge = 2
    }
    //=========================================================
    [System.Reflection.Obfuscation(Exclude = true)]
    public enum enTransitionNumber
    {
        en_First = 0,
        en_Second,
        en_third,
        en_Fourth,
        en_Fifth,
        en_Sixth,
        en_Seventh,
        en_Eighth,
        en_FinalFront,
        en_Last
    }

    #endregion



    public class ProcessingClass
    {
        public virtual void Load(string filename, ProcessingImage image) { throw new NotImplementedException(); }
        public virtual void Save(string filename, ProcessingImage image) { throw new NotImplementedException(); }
        public virtual void ImageDiskInquire(String filename, out int width, out int height, out int depth) { throw new NotImplementedException(); }

        public virtual void RemoveLittleBlob(ProcessingImage processImage, double NoisePercent) { throw new NotImplementedException(); }
        public virtual void UserConvolve(ProcessingImage processImage, enTypeOfConvolve TypeConvolve) { throw new NotImplementedException(); }
        public virtual void Expansion(ProcessingImage processImage, enKindOfPicture KindOfPicture, int NbIterations) { throw new NotImplementedException(); }
        public virtual void Erosion(ProcessingImage processImage, enKindOfPicture KindOfPicture, int NbIterations) { throw new NotImplementedException(); }
        public virtual void Threshold(ProcessingImage processImage, double LowParam, double HighParam) { throw new NotImplementedException(); }
        public virtual void ThresholdMedian(ProcessingImage processImage, int nbMorpho, int contrastThreshold) { throw new NotImplementedException(); }
        public virtual void ThresholdMultiRange(ProcessingImage ProcessImage, double[] LowParam, double[] HighParam) { throw new NotImplementedException(); }
        public virtual void Binarisation(ProcessingImage processImage, double Value) { throw new NotImplementedException(); }
        public virtual void Close(ProcessingImage ProcessImage, enKindOfPicture KindOfPicture, int NbIterations) { throw new NotImplementedException(); }
        public virtual void Sobel(ProcessingImage ProcessImage, enTypeOfSobel SobelType) { throw new NotImplementedException(); }
        public virtual void RemoveBorderBand(ProcessingImage ProcessImage, int removeSize) { throw new NotImplementedException(); }
        public virtual double GetGreyLevelAverage(ProcessingImage ProcessImage, int MinimumValue) { throw new NotImplementedException(); }
        public virtual int GetMedianValue(ProcessingImage ProcessImage, int MinimumValue) { throw new NotImplementedException(); }
        public virtual double GetGreyLevelMedian(ProcessingImage ProcessImage, int MinimumValue) { throw new NotImplementedException(); }
        public virtual void Laplacian(ProcessingImage ProcessImage, enTypeOfLaplacian LaplacianType) { throw new NotImplementedException(); }
        public virtual void Sharpen(ProcessingImage ProcessImage, enTypeOfSharpen SharpenType) { throw new NotImplementedException(); }
        public virtual void Prewitt(ProcessingImage ProcessImage) { throw new NotImplementedException(); }
        public virtual void LowPass(ProcessingImage ProcessImage) { throw new NotImplementedException(); }
        public virtual void MilBlobFilling(ProcessingImage ProcessImage) { throw new NotImplementedException(); }
        public virtual void MilBlobExtractHoles(ProcessingImage ProcessImage, enKindOfPicture kindOfPicture) { throw new NotImplementedException(); }
        public virtual void MilBorderBlobRemove(ProcessingImage ProcessImage, enKindOfPicture kindOfPicture) { throw new NotImplementedException(); }

        public virtual void Rognage(ProcessingImage ProcessImage, int nbNeighbours) { throw new NotImplementedException(); }
        public virtual void Smoothing(ProcessingImage ProcessImage) { throw new NotImplementedException(); }
        public virtual void LocalDensityFiltering(ProcessingImage ProcessImage, int MaskSize, int SignificantDensity) { throw new NotImplementedException(); }
        public virtual void MedianFilter(ProcessingImage ProcessImage, int Radius, long l2CacheMemSize) { throw new NotImplementedException(); }
        public virtual void MedianFloatFilter(ProcessingImage ProcessImage, int RadiusX, int RadiusY, int NbCores) { throw new NotImplementedException(); }
        public virtual void LinearDynamicScale(ProcessingImage ProcessImage, int Min, int Max) { throw new NotImplementedException(); }
        public virtual void Inversion(ProcessingImage processImage) { throw new NotImplementedException(); }
        public virtual void MaskRemove(ProcessingImage ProcessImage, String pathFileName) { throw new NotImplementedException(); }
        public virtual void Equalization(ProcessingImage ProcessImage, enEqualizationOperation Operation, double lfAlpha, double lfMinValue, double lfMaxValue) { throw new NotImplementedException(); }
        public virtual ProcessingImage Rotate(ProcessingImage processImage) { throw new NotImplementedException(); }
        public virtual ProcessingImage Resize(ProcessingImage processImage, int scaleXFactor, int scaleYFactor) { throw new NotImplementedException(); }
        public virtual void SetSubImage(ProcessingImage sourceImage, ProcessingImage destinationImage, int offestPosX, int offsetPosY) { throw new NotImplementedException(); }
        public virtual ProcessingImage ConvertTo8bit(ProcessingImage source) { throw new NotImplementedException(); }
        public virtual void FFTRemovePattern(ProcessingImage ProcessImage, System.Drawing.Rectangle WaferRect_px, System.Drawing.PointF WaferCenter_px, double EdgeRemove_px, double FFTPeaksThresholdRatio, int FrequencyTolerance, bool GenDarkImg, bool SaveFTTImg, bool bAdvDebugExpert, String DebugRecPath, bool DisableSaves) { throw new NotImplementedException(); }
        public virtual void FFTRemovePatternWithComments(ProcessingImage ProcessImage, System.Drawing.Rectangle WaferRect_px, System.Drawing.PointF WaferCenter_px, double EdgeRemove_px, double FFTPeaksThresholdRatio, int FrequencyTolerance, bool GenDarkImg, bool SaveFTTImg, bool bAdvDebugExpert, String DebugRecPath, bool DisableSaves) { throw new NotImplementedException(); }
        public virtual void FFTConvolveMexhat(ProcessingImage ProcessImage, int widthKernel, bool SaveFTTImg, bool bAdvDebugExpert, String DebugRecPath) { throw new NotImplementedException(); }

    }
}
