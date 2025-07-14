using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Hardware.FilterWheel;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Service.Core.Flows.Vignetting
{
    public class VignettingFlow : FlowComponent<VignettingInput, VignettingResult, VignettingConfiguration>
    {
        private readonly FilterWheel _filterWheel;
        private readonly IEmeraCamera _camera;
        private readonly ScanRangeType _rangeType;

        public VignettingFlow(VignettingInput input, IEmeraCamera camera) : base(input, "VignettingFlow")
        {
            _camera = camera;

            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            if (!(hardwareManager.Wheel is FilterWheel filterWheel))
            {
                throw new Exception("No Filter Wheel found. Check the initialization.");
            }
            _filterWheel = filterWheel;

            if (hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }

            if (Input != null)
            {
                _rangeType = Input.RangeType;
            }
            else
            {
                Logger.Information("No Input");
            }
        }

        protected override void Process()
        {           
            var scanRange = GetScanRange();
            var positionsWithVignettingValues = new Dictionary<double, double>();
            for (double i = scanRange.Min; i <= scanRange.Max; i += scanRange.Step)
            {
                double normalizedPosition = GetNormalizedRotation(i);
                double vignettingValue = MoveToPositionAndGetVignettingValue(normalizedPosition);
                positionsWithVignettingValues.Add(normalizedPosition, vignettingValue);
            }

            double bestPos = positionsWithVignettingValues.Aggregate((x, y) => x.Value < y.Value ? x : y).Key;
            Logger.Information($"Best value is at {bestPos}");
            _filterWheel.Move(bestPos);
            _filterWheel.WaitMotionEnd(5000);
            Result.FilterWheelPosition = bestPos;           
        }

        private ScanRangeWithStep GetScanRange()
        {
            double range = (Configuration.ScanRange.Millimeters);

            switch (_rangeType)
            {
                case ScanRangeType.Medium:
                    {
                        range *= Configuration.MediumRangeCoeff;
                        Logger.Debug($"{LogHeader} Use a 'Medium' preconfigured scan range.");
                        break;
                    }
                case ScanRangeType.Large:
                    {
                        range *= Configuration.LargeRangeCoeff;
                        Logger.Debug($"{LogHeader} Use a 'Large' preconfigured scan range.");
                        break;
                    }
                default:
                    {
                        range *= Configuration.SmallRangeCoeff;
                        Logger.Debug($"{LogHeader} Use a 'Small' default preconfigured scan range.");
                        break;
                    }
            }

            double rotationScanCenter = _filterWheel.GetCurrentPosition();
            double max = rotationScanCenter + range / 2.0;
            double min = rotationScanCenter - range / 2.0;

            double stepSize = _rangeType == ScanRangeType.Small ? Configuration.MinStepSize.Millimeters : Configuration.MaxStepSize.Millimeters;

            Logger.Debug($"{LogHeader} Use scan range of [{min},{max}] with step of {stepSize}");
            return new ScanRangeWithStep(min, max, stepSize);
        }

        private double MoveToPositionAndGetVignettingValue(double position)
        {
            _filterWheel.Move(position);
            _filterWheel.WaitMotionEnd(5000);
            var serviceImage = _camera.SingleScaledAcquisition(Int32Rect.Empty, 0.2);
            serviceImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(serviceImage);
            var imageData = AlgorithmLibraryUtils.CreateImageData(serviceImage);
            return UnitySCSharedAlgosOpenCVWrapper.ImageOperators.VignettingOperator(imageData, imageData.Height / 6);
        }

        private double GetNormalizedRotation(double inputRotation)
        {
            double normalizedRotation = inputRotation % 360;

            if (normalizedRotation < 0)
            {
                normalizedRotation += 360;
            }

            return normalizedRotation;
        }
    }
}
