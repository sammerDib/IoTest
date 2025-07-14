using System;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.AutoFocus;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Flows.PixelSize;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.FilterCalibration
{
    public class FilterCalibrationFlow : FlowComponent<FilterCalibrationInput, FilterCalibrationResult,
        FilterCalibrationConfiguration>
    {
        private readonly GetZFocusFlow _getZFocusFlow;
        private readonly AutoExposureFlow _autoExposureFlow;
        private readonly IEmeraCamera _camera;
        private readonly FilterWheel _filterWheel;
        private readonly double _imageScale;
        private readonly PatternRecFlow _patternRecFlow;
        private readonly PixelSizeComputationFlow _pixelSizeFlow;

        public FilterCalibrationFlow(FilterCalibrationInput input, IFlowsConfiguration flowsConfiguration,
            IEmeraCamera camera, PatternRecFlow patternRecFlow = null,
            PixelSizeComputationFlow pixelSizeFlow = null, AutoExposureFlow autoExposureFlow = null, GetZFocusFlow getZFocusFlow = null)
            : base(input, "Filter Calibration")
        {
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            if (hardwareManager.Wheel is FilterWheel filterWheel)
            {
                _filterWheel = filterWheel;
            }
            else
            {
                throw new Exception("No filter wheel found");
            }

            _camera = camera;

            _imageScale = (flowsConfiguration as FlowsConfiguration)?.ImageScale ?? 1.0;

            _patternRecFlow = patternRecFlow ?? new PatternRecFlow(new PatternRecInput(), _camera);
            _pixelSizeFlow = pixelSizeFlow ?? new PixelSizeComputationFlow(new PixelSizeComputationInput(), _camera);
            _autoExposureFlow = autoExposureFlow ?? new AutoExposureFlow(new AutoExposureInput(), _camera);
            _getZFocusFlow = getZFocusFlow ?? new GetZFocusFlow(new GetZFocusInput());
        }

        protected override void Process()
        {
            double initialPosition = _filterWheel.GetCurrentPosition();
            var uncalibratedFilters = Input.Filters;
            double initialExposureTime = _camera.GetCameraExposureTime();

            try
            {
                MoveFilter(uncalibratedFilters.First(x => x.Type == EMEFilter.NoFilter).Position);

                //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
                //double exposureTime = GetExposureTime();
                //_camera.SetCameraExposureTime(exposureTime);

                var serviceImage = _camera.SingleScaledAcquisition(Int32Rect.Empty, _imageScale);
                serviceImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(serviceImage);
                _patternRecFlow.Input = CreatePatternRecInput(serviceImage);

                var emptyFilterShift = GetPatternShift();
                var emptyFilterPixelSize = GetPixelSize();

                Result.Filters = uncalibratedFilters.ConvertAll(filter =>
                    GetCalibratedFilter(filter, emptyFilterShift, emptyFilterPixelSize));

                if (Result.Filters.Select(filter => filter.CalibrationStatus.State)
                    .Contains(FilterCalibrationState.CalibrationError))
                {
                    throw new PartialException("At least one filter could not be calibrated");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Filter Calibration Flow : {ex.Message}");
                throw;
            }
            finally
            {
                _filterWheel.Move(initialPosition);
                _camera.SetCameraExposureTime(initialExposureTime);
            }
        }

        private Filter GetCalibratedFilter(Filter filter, PatternRecResult emptyFilterShift,
            Length emptyFilterPixelSize)
        {

            if (filter.Type == EMEFilter.NoFilter)
            {
                filter.ShiftX = 0.Millimeters();
                filter.ShiftY = 0.Millimeters();
                filter.PixelSize = emptyFilterPixelSize;
                filter.CalibrationStatus = new FilterCalibrationStatus { State = FilterCalibrationState.Calibrated };
                return filter;
            }

            if (filter.DistanceOnFocus.Near(0, 1e-6))
            {
                filter.CalibrationStatus.State = FilterCalibrationState.CalibrationError;
                filter.CalibrationStatus.Message = $"Filter calibration failed for {filter.Name}: The focus distance for this filter needs to be calibrated first";
                return filter;
            }

            MoveFilter(filter.Position);

            _getZFocusFlow.Input.TargetDistanceSensor = filter.DistanceOnFocus;
            _getZFocusFlow.Execute();

            try
            {
                var filterResult = GetPatternShift();
                (filter.ShiftX, filter.ShiftY) = (filterResult.ShiftX - emptyFilterShift.ShiftX,
                    filterResult.ShiftY - emptyFilterShift.ShiftY);
                filter.PixelSize = GetPixelSize();
                filter.CalibrationStatus = new FilterCalibrationStatus { State = FilterCalibrationState.Calibrated };
            }
            catch (Exception ex)
            {
                filter.CalibrationStatus.State = FilterCalibrationState.CalibrationError;
                filter.CalibrationStatus.Message = $"Filter calibration failed for {filter.Name}: ${ex.Message}";
            }

            return filter;
        }

        private void MoveFilter(double position)
        {
            _filterWheel.Move(position);
            _filterWheel.WaitMotionEnd(5000);
            CheckCancellation();
        }

        private PatternRecResult GetPatternShift()
        {
            var result = _patternRecFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern Recognition Failed.");
            }

            CheckCancellation();

            return result;
        }

        private Length GetPixelSize()
        {
            var result = _pixelSizeFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pixel size Failed.");
            }

            CheckCancellation();
            return result.PixelSize;
        }

        private double GetExposureTime()
        {
            var result = _autoExposureFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Exposure time Failed.");
            }

            CheckCancellation();
            return result.ExposureTime;
        }

        private PatternRecInput CreatePatternRecInput(ServiceImage refImage)
        {
            var patternRecData =
                new PatternRecognitionData { PatternReference = refImage.ToExternalImage(), Gamma = 0.9 };

            return new PatternRecInput(patternRecData);
        }
    }
}
