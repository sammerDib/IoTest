using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLiseHF;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.ANA.Hardware.Probe.Lise.LiseSignalAcquisition;

namespace UnitySC.PM.ANA.Service.Core.TSV
{
    public class TSVDepthFlow : FlowComponent<TSVDepthInput, TSVDepthResult, TSVDepthConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly IReferentialManager _referentialManager;

        public TSVDepthFlow(TSVDepthInput input) : base(input, "TSVDepthFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
        }

        protected override void Process()
        {
            try
            {
                //for now we only have 1 probe (lisehf) so we don't need to provide a probe id but we will need to implement that later
                switch (Input.Probe)
                {
                    case SingleLiseSettings singleLiseSettings:
                        MeasureDepthWithLise(singleLiseSettings);
                        break;

                    case LiseHFSettings liseHFSettings:
                        ChangeStageReferentialSettingsAndMove(true);
                        MeasureDepthWithLiseHF(Input);
                        break;

                    default:
                        throw new Exception("Probe not supported to measure a TSV");
                }
            }
            finally
            {
                Result.Depth = (Result?.Depth is null) ? Result?.Depth : Configuration.ResultCorrectionSettings.ApplyCorrectionAndLog(Result.Depth, "TSV depth", Logger);
                ChangeStageReferentialSettingsAndMove(false);
            }
        }

        private void MeasureDepthWithLise(SingleLiseSettings singleLiseSettings)
        {
            var autofocusLiseParams = HardwareUtils.GetAutofocusLiseParameters(_hardwareManager, _calibrationManager, singleLiseSettings.ProbeId);

            double gain = double.IsNaN(singleLiseSettings.LiseGain) ? autofocusLiseParams.MaxGain : singleLiseSettings.LiseGain;
            var acquisitionParams = new LiseAcquisitionParams(gain, HighPrecisionMeasurement);
            var probeLise = HardwareUtils.GetProbeLiseFromID(_hardwareManager, singleLiseSettings.ProbeId);

            var step = 0.5.Micrometers();
            var maxBadSignalsBetweenGoodSignals = 5;
            var depthMeasurePrecisionInMillimeters = 10;

            var currentPos = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);

            List<double> depths = new List<double>();

            var positions = Input.AcquisitionStrategy == TSVAcquisitionStrategy.Standard ? CalculatePositionsAlongSegment(currentPos, step) : CalculatePositionsAlongArchimedeanSpiral(currentPos, step, Input.ApproximateWidth.Millimeters / 2, 2);
            var edgeWasFinded = false;
            var badSignalsNumberAfterHavingGoodSignals = 0;

            foreach (var position in positions)
            {
                CheckCancellation();
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, position, Configuration.Speed);

                var rawSignal = AcquireRawSignal(probeLise, acquisitionParams);
                var analyzedSignal = new LISESignalAnalyzed(rawSignal);

                if (Configuration.IsAnyReportEnabled())
                {
                    string filename = $"liseRawSignal_at_position(x_{position.X}_y_{position.Y})_{analyzedSignal.SelectedPeaks.Count}_selected_peaks_detected.csv";
                    LiseSignalReport.WriteRawSignalInCSVFormat(analyzedSignal.RawValues.ToList(), Path.Combine(ReportFolder, filename));
                }

                bool signalAnalyzedIsValid = analyzedSignal.SignalStatus == LISESignalAnalyzed.SignalAnalysisStatus.Valid;
                bool enoughPeaksNb = analyzedSignal.SelectedPeaks.Count >= 2;
                if (signalAnalyzedIsValid && enoughPeaksNb)
                {
                    edgeWasFinded = true;
                    badSignalsNumberAfterHavingGoodSignals = 0;
                    double surfaceX = analyzedSignal.SelectedPeaks[0].X;
                    double bottomX = analyzedSignal.SelectedPeaks[1].X;
                    var stepX = analyzedSignal.StepX.Nanometers();
                    var depth = (bottomX - surfaceX) * stepX;

                    if (Math.Abs(depth.Millimeters - Input.ApproximateDepth.Millimeters) < depthMeasurePrecisionInMillimeters)
                    {
                        if (Input.MeasurePrecision == TSVMeasurePrecision.Fast)
                        {
                            Result.Depth = depth;
                            Result.Quality = 100.0; // to do quality measure
                            return;
                        }

                        depths.Add(depth.Micrometers);
                        Logger.Information($"{LogHeader} Calculated depth at position (x= {position.X},y= {position.Y}): {depth.Micrometers} μm.");
                    }
                }
                else if (edgeWasFinded)
                {
                    badSignalsNumberAfterHavingGoodSignals++;
                    if (badSignalsNumberAfterHavingGoodSignals >= maxBadSignalsBetweenGoodSignals)
                    {
                        break;
                    }
                }
            }

            if (depths.Count == 0)
            {
                throw new Exception($"No depth could be measured from lise signals.");
            }

            double averagedDepth = depths.Count > 0 ? depths.Average() : 0.0;
            Result.Depth = averagedDepth.Micrometers();
            Result.Quality = 100.0; // to do quality measure
        }

        private void MeasureDepthWithLiseHF(TSVDepthInput input)
        {
            // exception should be thrown when the measure fails. Then The message of the exception could be displayed to the user via Property message of FlowStatus.

            var probeLiseHF = HardwareUtils.GetProbeFromID<IProbe>(_hardwareManager, input.Probe.ProbeId);
            LiseHFSettings settings = (input.Probe) as LiseHFSettings;
            LiseHFInputParamsForTSV inputparam = new LiseHFInputParamsForTSV()
            {
                NbMeasuresAverage = settings.NbMeasuresAverage,
                IsLowIlluminationPower = settings.IsLowIlluminationPower,
                IntensityFactor = settings.IntensityFactor, // new
                Threshold = settings.Threshold,
                ThresholdPeak = settings.ThresholdPeak,
                SaveFFTSignal = settings.SaveFFTSignal,
                CalibrationFreq = settings.CalibrationFreq,
                TSVDiameter = input.ApproximateWidth,
                TSVDepth = input.ApproximateDepth,
                TSVDepthTolerance = input.DepthTolerance,
                ObjectiveId = settings.ProbeObjectiveContext.ObjectiveId, 
                PhysicalLayers = settings.Layers,
            };

            if (Configuration.IsAnyReportEnabled())
            {
                inputparam.ReportPath = ReportFolder;
                inputparam.ReportSignals = true;
                inputparam.ReportOutputs = true;
            }

            SetProgressMessage($"[LiseHF Depth flow] Measuring");
            try
            {
                var proberesult = probeLiseHF.DoMeasure(inputparam) as ProbeLiseHFResult;

                CheckCancellation();

                Result.Quality = proberesult?.Quality ?? 0.0;
                Result.DepthRawSignal = proberesult?.FFTSignal ?? MakeFlatFFTSignalMemory(settings.SaveFFTSignal);

                //to do check le success
                if (proberesult.LayersThickness?.Count > 0)
                {
                    var Totaldepth = new Length(0.0, input.ApproximateDepth.Unit);
                    foreach (var layer in proberesult.LayersThickness)
                    {
                        Totaldepth += layer.Thickness;
                    }
                    Result.Depth = Totaldepth;
                }
                else
                {
                    Result.Depth = null;
                    Result.Quality = 0.0;
                    throw new Exception($"[LiseHF Depth flow] Fails : {proberesult.Message}");
                }
            }
            catch (Exception ex)
            {
                Result.Depth = null;
                Result.Quality = 0.0;
                if(Result.DepthRawSignal == null)
                    Result.DepthRawSignal =  MakeFlatFFTSignalMemory(settings.SaveFFTSignal);
                throw;
            }
        }

        private byte[] MakeFlatFFTSignalMemory(bool bSaveSignal)
        {
            if(!bSaveSignal)
                return null;

            int nCnt = 2;
            float[] memfloat = new float[2 * nCnt];
            memfloat[0] = 0.0f; // X0
            memfloat[1] = 0.0001f; //Y0 
            memfloat[2] = 200.0f; // Xlast
            memfloat[3] = 0.0f;   //YLast 

            byte[] memBytes = new byte[sizeof(float) * memfloat.Length + 2];
            memBytes[0] = (byte) 0; // version buffer for lter compatibilty
            memBytes[1] = (byte) 0; // saturation percentage
            Buffer.BlockCopy(memfloat, 0, memBytes, 2, memBytes.Length - 2);

            return memBytes;
        }

        public List<XYPosition> CalculatePositionsAlongSegment(XYPosition initialPosition, Length step)
        {
            var currentX = initialPosition.X - (Input.ApproximateWidth.Millimeters / 2 + 2 * step.Millimeters);
            var finalX = initialPosition.X + (Input.ApproximateWidth.Millimeters / 2 + 2 * step.Millimeters);

            List<XYPosition> points = new List<XYPosition>();
            points.Add(initialPosition);

            while (currentX < finalX)
            {
                currentX += step.Millimeters;
                points.Add(new XYPosition(initialPosition.Referential, currentX, initialPosition.Y));
            }

            return points;
        }

        public static List<XYPosition> CalculatePositionsAlongArchimedeanSpiral(XYPosition initialPosition, Length step, double distBetweenSpiralRings, double spiralRingsNumber)
        {
            double spiralInsideDiameter = 0;
            double spiralOutsideDiameter = spiralInsideDiameter + (spiralRingsNumber * 2 * (distBetweenSpiralRings + 2 * step.Millimeters));

            double maxAngle = spiralRingsNumber * 2 * Math.PI;
            double spiralOutsideRadius = spiralOutsideDiameter / 2;
            double distFromCenterForEachAngleStep = spiralOutsideRadius / maxAngle;

            List<XYPosition> points = new List<XYPosition>();
            points.Add(initialPosition);

            //The points are approximately on a circle, so the angle between them is chord / radius
            //We considere a clockwize rotation of the spiral
            for (double angle = step.Millimeters / distFromCenterForEachAngleStep; angle <= maxAngle;)
            {
                double distFromCenter = distFromCenterForEachAngleStep * angle;

                double x = initialPosition.X + Math.Cos(angle) * distFromCenter;
                double y = initialPosition.Y + Math.Sin(angle) * distFromCenter;

                points.Add(new XYPosition(initialPosition.Referential, x, y));

                angle += step.Millimeters / distFromCenter;
            }

            return points;
        }

        public void ChangeStageReferentialSettingsAndMove(bool applyProbeOffset)
        {
            var position = _hardwareManager.Axes.GetPos();
            position = _referentialManager.ConvertTo(position, ReferentialTag.Stage);
            _referentialManager.SetSettings(new StageReferentialSettings() { EnableProbeSpotOffset = applyProbeOffset });
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, position, AxisSpeed.Slow);
        }
    }
}
