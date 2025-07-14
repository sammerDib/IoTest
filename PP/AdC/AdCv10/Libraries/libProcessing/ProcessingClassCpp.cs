using System;
using System.Runtime.InteropServices;

using UnitySC.Shared.LibMIL;

namespace LibProcessing
{


    public class ProcessingClassCpp : ProcessingClass
    {
        private static class NativeMethods
        {

            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int InitLoadCall();
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformDensity(long uIn, byte[,] uOut, int iPitchIn, int iSizeX, int iSizeY, int MaskSize, int SignificantDensity);
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformRognage(long uIn, byte[,] uOut, int iPitchIn, int iSizeX, int iSizeY, int SignificantPixel);
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformSmoothing(long uIn, byte[,] uOut, int iPitchIn, int iSizeX, int iSizeY);
            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformMedianFilter(long uIn, byte[,] uOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, int iRadius, long l2CacheMemSize);

            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformMedianFloatFilter(long uIn, IntPtr uOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int nbCores);

            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformMedianFilter_U16(long uIn, IntPtr uOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int nbCores);

            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformMedianFilter_U8(long uIn, IntPtr uOut, int iSizeX, int iSizeY, int iRadiusX, int iRadiusY, int nbCores);

            //------
            [DllImport("CppAlgorithms.dll", CallingConvention = CallingConvention.Cdecl)]
            public static extern int PerformLinearDynScaling(long uIn, byte[,] uOut, int iPitchIn, int iPitchOut, int iSizeX, int iSizeY, byte uMin, byte uMax);

        }

        static ProcessingClassCpp()
        {
            NativeMethods.InitLoadCall();
        }

        public ProcessingClassCpp()
        {

        }

        public override void Rognage(ProcessingImage ProcessImage, int nbNeighbours)
        {
            CSharpImage csharpImage = ProcessImage.GetCSharpImage();

            byte[,] outImage = new byte[csharpImage.width, csharpImage.height];

            int ret = NativeMethods.PerformRognage(csharpImage.ptr, outImage, csharpImage.pitch, csharpImage.width, csharpImage.height, nbNeighbours);
            if (ret < 0)
                throw new ApplicationException("error in PerformRognage");

            ProcessImage.PutCSharpArray(outImage);
        }

        public override void Smoothing(ProcessingImage ProcessImage)
        {
            CSharpImage csharpImage = ProcessImage.GetCSharpImage();

            byte[,] outImage = new byte[csharpImage.width, csharpImage.height];

            int ret = NativeMethods.PerformSmoothing(csharpImage.ptr, outImage, csharpImage.pitch, csharpImage.width, csharpImage.height);
            if (ret < 0)
                throw new ApplicationException("error in Smoothing");

            ProcessImage.PutCSharpArray(outImage);
        }

        public override void LocalDensityFiltering(ProcessingImage ProcessImage, int MaskSize, int SignificantDensity)
        {
            CSharpImage csharpImage = ProcessImage.GetCSharpImage();

            byte[,] outImage = new byte[csharpImage.width, csharpImage.height];

            int ret = NativeMethods.PerformDensity(csharpImage.ptr, outImage, csharpImage.pitch, csharpImage.width, csharpImage.height, MaskSize, SignificantDensity);
            if (ret < 0)
                throw new ApplicationException("error in PerformDensity");

            ProcessImage.PutCSharpArray(outImage);
        }

        public override void MedianFilter(ProcessingImage ProcessImage, int Radius, long l2CacheMemSize)
        {
            int nDepth = ProcessImage.GetMilImage().SizeBit;
            if (nDepth == 8)
            {
                CSharpImage csharpImage = ProcessImage.GetCSharpImage();

                byte[,] outImage = new byte[csharpImage.width, csharpImage.height];

                int ret = NativeMethods.PerformMedianFilter(csharpImage.ptr, outImage, csharpImage.pitch, csharpImage.width, csharpImage.width, csharpImage.height, Radius, l2CacheMemSize);
                if (ret < 0)
                    throw new ApplicationException("error in PerformMedianFilter");

                ProcessImage.PutCSharpArray(outImage);
            }
            else if (nDepth == 16)
            {
                MilImage milmyImg = ProcessImage.GetMilImage();
                UInt16[] pOutData = new UInt16[milmyImg.SizeX * milmyImg.SizeY];
                GCHandle hdlOut = GCHandle.Alloc(pOutData, GCHandleType.Pinned);
                int ret = -3;
                // cas Pitch aligné  
                if (milmyImg.Pitch == milmyImg.SizeX * sizeof(UInt16))
                {
                    milmyImg.Lock();
                    long bufferPtr = milmyImg.HostAddress; // on économise une copy, on passe directement le pointeur MIL en entrée

                    ret = NativeMethods.PerformMedianFilter_U16(bufferPtr, hdlOut.AddrOfPinnedObject(), milmyImg.SizeX, milmyImg.SizeY, Radius, Radius, 0);

                    milmyImg.Unlock();
                }
                else
                {
                    // cas pitch non aligné -- l'algo médian float cpp ne gere pas les buffers à pitch non alignés, il nous lui faut passé le buffer sans pitch 
                    UInt16[] pInData = new UInt16[milmyImg.SizeX * milmyImg.SizeY];
                    milmyImg.Get(pInData);
                    GCHandle hdlIn = GCHandle.Alloc(pInData, GCHandleType.Pinned);

                    ret = NativeMethods.PerformMedianFloatFilter(hdlIn.AddrOfPinnedObject().ToInt64(), hdlOut.AddrOfPinnedObject(), milmyImg.SizeX, milmyImg.SizeY, Radius, Radius, 0);

                    hdlIn.Free();
                    pInData = null;
                }

                if (ret < 0)
                {
                    hdlOut.Free();
                    throw new ApplicationException("error in PerformMedianFloatFilter");
                }

                milmyImg.Put(pOutData); // on met à jour notre image
                hdlOut.Free();
                pOutData = null;

            }
            else
                throw new ApplicationException("error in PerformMedianFilter : Bad Depth of image");

        }

        public override void MedianFloatFilter(ProcessingImage ProcessImage, int RadiusX, int RadiusY, int NbCores)
        {
            /// /!\ cas particulier on sort une image MIL au lieu d'un image CPP /!\

            MilImage milFloatImg = ProcessImage.GetMilImage();
            if (milFloatImg.Type != Matrox.MatroxImagingLibrary.MIL.M_FLOAT + 32)
                throw new ApplicationException("error in MedianFloatFilter : Wrong depth expecting float matrix");

            int ret = -3;

            float[] fOutData = new float[milFloatImg.SizeX * milFloatImg.SizeY];
            GCHandle hdlOut = GCHandle.Alloc(fOutData, GCHandleType.Pinned);

            // cas Pitch aligné  
            if (milFloatImg.Pitch == milFloatImg.SizeX * sizeof(float))
            {
                milFloatImg.Lock();
                long bufferPtr = milFloatImg.HostAddress; // on économise une copy, on passe directement le pointeur MIL en entrée

                ret = NativeMethods.PerformMedianFloatFilter(bufferPtr, hdlOut.AddrOfPinnedObject(), milFloatImg.SizeX, milFloatImg.SizeY, RadiusX, RadiusY, NbCores);

                milFloatImg.Unlock();
            }
            else
            {
                // cas pitch non aligné -- l'algo médian float cpp ne gere pas les buffers à pitch non alignés, il nous lui faut passé le buffer sans pitch 
                float[] fInData = new float[milFloatImg.SizeX * milFloatImg.SizeY];
                milFloatImg.Get(fInData);
                GCHandle hdlIn = GCHandle.Alloc(fInData, GCHandleType.Pinned);

                ret = NativeMethods.PerformMedianFloatFilter(hdlIn.AddrOfPinnedObject().ToInt64(), hdlOut.AddrOfPinnedObject(), milFloatImg.SizeX, milFloatImg.SizeY, RadiusX, RadiusY, NbCores);

                hdlIn.Free();
                fInData = null;
            }

            if (ret < 0)
            {
                hdlOut.Free();
                throw new ApplicationException("error in PerformMedianFloatFilter");
            }

            milFloatImg.Put(fOutData); // on met à jour notre image
            hdlOut.Free();
            fOutData = null;

            // Note de [RTi] le type ProcessImage ne gere à ce jour que du byte[,] le temp manquant et le nb de traitement de type float etant trés faible, on gere ici directement la modification du buffer mil
            //  Mil -> float* -> traitement cpp float -> float* -> Mil
            // l'image a été modifiée-- on reste en mode _eImageType == eImageType.Mil; pas d'appel à SetCSharpImage
        }

        public override void LinearDynamicScale(ProcessingImage ProcessImage, int Min, int Max)
        {
            CSharpImage csharpImage = ProcessImage.GetCSharpImage();

            byte[,] outImage = new byte[csharpImage.width, csharpImage.height];

            int ret = NativeMethods.PerformLinearDynScaling(csharpImage.ptr, outImage, csharpImage.pitch, csharpImage.width, csharpImage.width, csharpImage.height, (byte)Min, (byte)Max);
            if (ret < 0)
                throw new ApplicationException("error in LinearDynamicScale");

            ProcessImage.PutCSharpArray(outImage);

            //  ProcessImage.GetMilImage().ImageJ();
        }

        public override void Threshold(ProcessingImage processImage, double LowParam, double HighParam)
        {
            
        }
    }


}
