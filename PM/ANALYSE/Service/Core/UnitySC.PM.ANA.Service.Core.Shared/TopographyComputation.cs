using System.Collections.Generic;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.FormatFile;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    public class TopographyComputation
    {
        public struct TopographyPSIResult
        {
            public ServiceImage WrappedPhaseMap;
            public ServiceImage UnwrappedPhaseMap;
            public ServiceImage RawTopographyMap;
            public ServiceImage TopographyMap;
            public ServiceImage Plane;
        }

        public virtual TopographyPSIResult ComputeTopography(List<USPImage> imgs, Length wavelength, int stepNb, MatrixFloatFile.AdditionnalHeaderData headerAddData = null)
        {
            List<ImageData> imagesData = new List<ImageData>();
            foreach (USPImage img in imgs)
            {
                imagesData.Add(AlgorithmLibraryUtils.CreateImageData(img));
            }

            PSIParams psiParams = new PSIParams(wavelength.Nanometers, stepNb, false, UnwrapMode.Goldstein);

            TopographyPSI psiResult = PhaseShiftingInterferometry.ComputeTopography(imagesData.ToArray(), psiParams);

            var result = new TopographyPSIResult();
            result.WrappedPhaseMap = AlgorithmLibraryUtils.ConvertTo3DAServiceImage(psiResult.WrappedPhaseMap, headerAddData);
            result.UnwrappedPhaseMap = AlgorithmLibraryUtils.ConvertTo3DAServiceImage(psiResult.UnwrappedPhaseMap, headerAddData);
            result.RawTopographyMap = AlgorithmLibraryUtils.ConvertTo3DAServiceImage(psiResult.RawTopographyMap, headerAddData);
            result.TopographyMap = AlgorithmLibraryUtils.ConvertTo3DAServiceImage(psiResult.TopographyMap, headerAddData);
            result.Plane = AlgorithmLibraryUtils.ConvertTo3DAServiceImage(psiResult.Plane, headerAddData);

            return result;
        }
    }
}
