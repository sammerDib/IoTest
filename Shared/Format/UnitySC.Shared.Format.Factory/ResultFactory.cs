using System;
using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Display._001;
using UnitySC.Shared.Display.ASO;
using UnitySC.Shared.Display.HAZE;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format._001;
using UnitySC.Shared.Format.ASO;
using UnitySC.Shared.Format.HAZE;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Base.Acquisition;

namespace UnitySC.Shared.Format.Factory
{
    public sealed class ResultDataFactory : IResultDataFactory
    {
        private readonly Dictionary<int, IResultDisplay> _mapFormatView = new Dictionary<int, IResultDisplay>();

        public ResultDataFactory()
        {
            InitializeFormatMapViews();
        }

        private void InitializeFormatMapViews()
        {
            _mapFormatView.Add(ResultType.ADC_Klarf.GetResultExtensionId(), new KlarfDisplay());
            _mapFormatView.Add(ResultType.ADC_ASO.GetResultExtensionId(), new AsoDisplay());
            _mapFormatView.Add(ResultType.ADC_Haze.GetResultExtensionId(), new HazeDisplay());

            // METROLOGY DISPLAY SPECIFIC
            var metroDisplay = new MetroDisplay();
            _mapFormatView.Add(ResultType.ANALYSE_TSV.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_NanoTopo.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Thickness.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Topography.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Step.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Trench.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Pillar.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_PeriodicStructure.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Bow.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_Warp.GetResultExtensionId(), metroDisplay);
            _mapFormatView.Add(ResultType.ANALYSE_EdgeTrim.GetResultExtensionId(), metroDisplay);
        }

        #region IResultDataFactory.Implementation

        public IResultDataObject Create(ResultType resType, long dBResId)
        {
            IResultDataObject obj;
            switch (resType.GetResultFormat())
            {
                case ResultFormat.Klarf: // klarf - 0
                    obj = new DataKlarf(resType, dBResId);
                    break;

                case ResultFormat.ASO: // ASO - 2
                    obj = new DataAso(resType, dBResId);
                    break;

                case ResultFormat.Haze: // Haze - 10
                    obj = new DataHaze(resType, dBResId);
                    break;

                case ResultFormat.Metrology: // Metro
                    obj = new MetroResult(resType, dBResId);
                    break;

                default:
                    throw new NotImplementedException();
            }
            return obj;
        }

        public IResultDataObject CreateFromFile(ResultType resType, long dBResId, string resFilePath)
        {
            switch (resType.GetResultFormat())
            {
                case ResultFormat.Klarf: // klarf - 0
                    return new DataKlarf(resType, dBResId, resFilePath);

                case ResultFormat.ASO: // ASO - 2
                    return new DataAso(resType, dBResId, resFilePath);

                case ResultFormat.Haze: // Haze - 10
                    return new DataHaze(resType, dBResId, resFilePath);

                case ResultFormat.Metrology: // Metro
                    return new MetroResult(resType, dBResId, resFilePath);

                // Acquisition maps
                case ResultFormat.FullImage: // fullimage
                    return new FullImageResult(resType, dBResId, resFilePath);

                default:
                    throw new NotImplementedException();
            }
        }

        public IResultDisplay GetDisplayFormat(ResultType resType)
        {
            int resExtId = resType.GetResultExtensionId();
            if (_mapFormatView.ContainsKey(resExtId))
                return _mapFormatView[resExtId];
            throw new NotImplementedException(string.Format("ViewFormat for the type ({0}:{1}) has not been correctly registered in Display Dictionnary", (int)resType, resType.ToString()));
        }

        public bool GenerateThumbnailFile(IResultDataObject dataobj, params object[] p_Inprm)
        {
            var resView = GetDisplayFormat(dataobj.ResType);
            return resView.GenerateThumbnailFile(dataobj, p_Inprm);
        }

        public List<ResultDataStats> GenerateStatisticsValues(IResultDataObject dataobj, params object[] p_Inprm)
        {
            var resView = GetDisplayFormat(dataobj.ResType);
            return resView.GenerateStatisticsValues(dataobj, p_Inprm);
        }

        public SizeBins KlarfDisplay_SizeBins()
        {
            var resKlarfDisplay = GetDisplayFormat(ResultType.ADC_Klarf) as KlarfDisplay;
            if (resKlarfDisplay != null)
                return resKlarfDisplay.TableBinSize;
            return null;
        }

        public DefectBins KlarfDisplay_DefectBins()
        {
            var resKlarfDisplay = GetDisplayFormat(ResultType.ADC_Klarf) as KlarfDisplay;
            if (resKlarfDisplay != null)
                return resKlarfDisplay.TableBinDefect;
            return null;
        }

        #endregion  IResultDataFactory.Implementation
    }
}
