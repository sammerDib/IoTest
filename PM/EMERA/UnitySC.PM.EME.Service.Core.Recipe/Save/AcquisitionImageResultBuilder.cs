using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public class AcquisitionImageResultBuilder
    {
        private string _baseName;
        private string _folder;
        private EMEFilter _filter;
        private EMELightType _lightType;
        private Length _pixelSize;
        private Length _waferDiameter;
        private string _acquisitionLabel;

        public VignetteImageResult BuildVignetteImageResult(int nbImagesY, int nbImagesX)
        {
            var resultType = EmeResultTypeConverter.GetResultTypeFromFilterAndLight(_filter, _lightType);
            return new VignetteImageResult(_folder, _baseName, nbImagesY, nbImagesX, resultType, _pixelSize, _waferDiameter, _acquisitionLabel);
        }

        public FullImageResult BuildFullImageResult()
        {
            var resultType = EmeResultTypeConverter.GetResultTypeFromFilterAndLight(_filter, _lightType);
            return new FullImageResult(_folder, _baseName, resultType, _pixelSize, _waferDiameter, _acquisitionLabel);
        }

        public AcquisitionImageResultBuilder AddFolderAndBaseName(ImageFolderAndBaseName imageFolderAndBaseName)
        {
            _baseName = imageFolderAndBaseName.BaseName;
            _folder = imageFolderAndBaseName.Folder;
            return this;
        }

        public AcquisitionImageResultBuilder AddFilter(EMEFilter filter, Length pixelSize,
            bool reduceResolution, double scale)
        {
            _filter = filter;
            
            if (reduceResolution)
            {
                pixelSize /= scale;
            }
            
            _pixelSize = pixelSize;
            return this;
        }

        public AcquisitionImageResultBuilder AddLightType(EMELightType type)
        {
            _lightType = type;
            return this;
        }

        public AcquisitionImageResultBuilder AddWaferDiameter(Length diameter)
        {
            _waferDiameter = diameter;
            return this;
        }
        public AcquisitionImageResultBuilder AddAcquisitionLabel(string acquisitionLabel)
        {
            _acquisitionLabel = acquisitionLabel;
            return this;
        }
    }
}
