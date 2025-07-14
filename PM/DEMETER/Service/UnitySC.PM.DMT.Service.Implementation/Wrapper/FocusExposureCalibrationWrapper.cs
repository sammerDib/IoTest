using System;
using System.Linq;
using System.Runtime.InteropServices;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.OpticalMount;

namespace UnitySC.PM.DMT.Service.Implementation.Wrapper
{
    internal class FocusExposureCalibrationWrapper
    {
        #region DLLImports wrapper C(++)

        [DllImport("FocusQuality.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int CalibrateExposureMatching(byte[] pIm, uint nSizeH, uint nSizeV,
            uint[] listOfGoldenLuminanceValues, [Out] out float exposureMatchingCoeff);

        [DllImport("FocusQuality.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int FocusQuality(byte[] pIm, uint nSizeH, uint nSizeV, double dWaferSize,
            double dPatternSize, uint nSubImPosition, [In] [Out] SubImageProperties[] pSubImages);

        [DllImport("FocusQuality.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetGoldenValuesFromImage(byte[] pIm, uint nSizeH, uint nSizeV,
            [In] [Out] uint[] goldenValues);

        public static int FocusQuality(byte[] pIm, uint imageWidth, uint imageHeight, double dWaferSize, double dPatternSize,
            OpticalMountShape nSubImPosition, ref SubImageProperties[] pSubImages)
        {
            return FocusQuality(pIm, imageWidth, imageHeight, dWaferSize, dPatternSize, (uint)nSubImPosition, pSubImages);
        }

        public static ulong[] GetGoldenValuesFromImage(byte[] pIm, uint imageWidth, uint imageHeight)
        {
            ulong[] goldenLuminanceValues = new ulong[5];
            uint[] cppGoldenLuminanceValues = new uint[5];
            int exitCode = GetGoldenValuesFromImage(pIm, imageWidth, imageHeight, cppGoldenLuminanceValues);
            if (exitCode == 0)
            {
                foreach ((int index, uint luminanceValue) in cppGoldenLuminanceValues.Select((value, index) => (index, value)))
                {
                    goldenLuminanceValues[index] = luminanceValue;
                }

                return goldenLuminanceValues;
            }
            else
            {
                throw new Exception("Failed to get golden values from image, because no wafer was found in the image");
            }
        }

        #endregion DLLImports wrapper C(++)
    }
}
