using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Service.Interface;

namespace UnitySC.PM.DMT.Service.Implementation.Curvature
{
    public class NativeMethods
    {
        public enum TypeOfFrame
        {
            CurvatureX = 1,
            CurvatureY = 2,
            AmplitudeX = 4,
            AmplitudeY = 8,
            UnwrappedPhaseX = 16,
            UnwrappedPhaseY = 32,
            GlobalTopoNX = 64,
            GlobalTopoNY = 128,
            GlobalTopoNZ = 256,
            GlobalTopoX = 512,
            GlobalTopoY = 1024,
            GlobalTopoZ = 2048,
            PhaseX = 4096,
            PhaseY = 8192,
            PhaseMask = 16384,
            Dark= 32768,
        }

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct CURVATURE_CONFIG
        {
            public int nTargetBackgroundGrayLevel;   // Should be in AlgorithmsConfiguration.xml
            public float fUserCurvatureDynamicsCoeff;  // Comes from the recipe (default in GUI: 1). Range: >0.
            [XmlIgnore] public float fCalibratedNoise; // Comes from curvature calibration
            [XmlIgnore] public int nCalibrationPeriod; // Comes from curvature calibration    
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FILTER_CONFIG
        {
            public int Contraste_min_curvature;
            public int Intensite_min_curvature;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GLOBAL_TOPOCONFIG
        {
            public int KnownHeightPixelX, KnownHeightPixelY;
            public int PixelRefX, PixelRefY;            //ne devrait plus être utilisé
            public float Height;
            public float Ratio;
            public int CrossSearchThreshold;
            [XmlIgnore] public bool UnwrappedPhase;
            [XmlIgnore] public int FringePeriod;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT_INFO
        {
            public int NbPeriods;
            public int NbImgX, NbImgY;
            public int SizeX, SizeY;
            public TypeOfFrame TypeOfFrame;
        }

        #endregion


        #region DLLImports wrapper C(++)
        [DllImport("NanoCore.dll")]
        public static extern sCalibPaths GetCalibFolderStructure();

        [DllImport("CalculationDLL.dll")]
        public static extern long CreateNewGlobalTopoInstance(string calibFolder);

        [DllImport("CalculationDLL.dll")]
        public static extern int InitializeGlobalTopo(MIL_ID instanceId);

        [DllImport("CalculationDLL.dll")]
        public static extern bool DeleteGlobalTopoInstance(long instanceId);

        [DllImport("CalculationDLL.dll")]
        public static extern int CreateNewInstance(MIL_ID milSystemId, long instanceId);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetCurvatureConfig(long instanceId, CURVATURE_CONFIG config);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetFilterConfig(long instanceId, FILTER_CONFIG config);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetGlobalTopoConfig(long instanceId, GLOBAL_TOPOCONFIG config);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetInputInfo(long lInstanceID, INPUT_INFO info, int[] periods);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetInputImage(long instanceId, MIL_ID milImageId, int index);

        [DllImport("CalculationDLL.dll")]
        public static extern bool SetCrossImg(long instanceId, MIL_ID milImageId);

        [DllImport("CalculationDLL.dll")]
        public static extern bool PerformCalculation(long instanceId);

        [DllImport("CalculationDLL.dll")]
        public static extern float PerformCurvatureCalibration(long instanceId);

        [DllImport("CalculationDLL.dll")]
        public static extern bool PerformIncrementalCalculation(long lInstanceID, int period, char direction);

        [DllImport("CalculationDLL.dll")]
        public static extern bool UpdateCurvatureConfig(long lInstanceID, CURVATURE_CONFIG config, TypeOfFrame typeOfFrame);

        [DllImport("CalculationDLL.dll")]
        public static extern MIL_ID GetResultImage(long lInstanceID, TypeOfFrame typeOfFrame, int index = 0);

        [DllImport("CalculationDLL.dll")]
        public static extern IntPtr AccessWrappedPhaseOrMask(long lInstanceID, TypeOfFrame typeOfFrame, int index = 0);

        [DllImport("CalculationDLL.dll")]
        public static extern bool GetIncrementalResultList(long lInstanceID, TypeOfFrame[] pTypeOfFrame, int[] pIndex, ref int nbResults);

        [DllImport("CalculationDLL.dll")]
        public static extern bool DeleteInstance(long lInstanceID);

        public static bool GetIncrementalResultList(long lInstanceID, List<TypeOfFrame> typeOfFrameList, List<int> indexList)
        {
            typeOfFrameList.Clear();
            indexList.Clear();

            int nbResults = 20; // ça doit suffire
            TypeOfFrame[] tofArray = new TypeOfFrame[nbResults];
            int[] idxArray = new int[nbResults];

            bool ok = GetIncrementalResultList(lInstanceID, tofArray, idxArray, ref nbResults);
            if (!ok)
                return ok;

            for (int i = 0; i < nbResults; i++)
            {
                typeOfFrameList.Add(tofArray[i]);
                indexList.Add(idxArray[i]);
            }
            return ok;
        }

        #endregion
    }
}
