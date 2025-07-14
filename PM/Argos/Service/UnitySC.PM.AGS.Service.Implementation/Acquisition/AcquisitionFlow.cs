using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.PM.AGS.Data.Enum;
using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Interface.Axes;
using UnitySC.PM.AGS.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.AGS.Service.Implementation.Acquisition
{
    public class AcquisitionFlow : FlowComponent<AcquisitionInput, AcquisitionResult, DefaultConfiguration>
    {
        private ArgosHardwareManager _hardwareManager;
        private ArgosAxesBase _axes;
        private Dictionary<string, CameraBase> _cameras;

        public AcquisitionFlow(AcquisitionInput input) : base(input, "AcquisitionFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<ArgosHardwareManager>();
            _axes = _hardwareManager.Axes as ArgosAxesBase;
            _cameras = _hardwareManager.Cameras;
        }

        protected override void Process()
        {
            SetState(FlowState.InProgress, "Starting Acquisition");
            ComputeParameters();
            ExecuteAcquisition();
            if (Result.Status.State != FlowState.Success)
            {
                SetState(FlowState.Error, "Acquisition finished");
                throw new Exception($"Acquisition failure.");
            }

            SetState(FlowState.Success, "Acquisition finished");
        }

        ///////////////////////////////////
        // Acquisition subparameters     //
        ///////////////////////////////////

        #region Acquisition subparameters

        /// <summary>
        /// Constant angular speed in degree/sec
        /// </summary>
        protected double AngularSpeed;

        /// <summary>
        /// Constant angular interval between 2 slots of the pso signal
        /// </summary>
        protected double PsoAngularInterval;

        /// <summary>
        /// Total angle distance for acquisition (it may be a little higher than 360 degree)
        /// </summary>
        private double _angleDistance;

        /// <summary>
        /// Number of acquired images for each acquisition round
        /// </summary>
        private int _numberOfImagesPerCol;

        /// <summary>
        /// The angle from which we start the acquisition
        /// </summary>
        private double _startAngle;

        /// <summary>
        /// The angle in degree to add so the stage is in stabilized speed before starting the analysis
        /// </summary>
        private double _psoAccelerationAngle;

        /// <summary>
        ///
        /// </summary>
        private Length _xOffsetPerRound;

        #endregion Acquisition subparameters

        private void ComputeParameters()
        {
            double pixelSize =
                _axes.GetNearestPSOPixelSize(Input.PixelSize.Micrometers, 2.0 * Input.WaferRadius.Millimeters);
            if (pixelSize == -1) { throw new Exception("No PSO Axis configured, acquisition cannot continue"); }

            AngularSpeed = (360.0 / (2.0 * Math.PI)) * (double)Input.Frequency * (pixelSize / 1000.0) /
                            Input.WaferRadius.Millimeters;
            if (AngularSpeed > _axes.GetAxisConfigById("T").SpeedMaxScan)
            {
                throw new Exception("Angular speed to high. Frequency or Y pixel size must be reduced.");
            }

            PsoAngularInterval = 360.0 * pixelSize / (Input.WaferRadius.Millimeters * 2.0 * Math.PI * 1000.0);
            _numberOfImagesPerCol = (int)Math.Ceiling(360.0 / PsoAngularInterval / Input.YResolution);
            _angleDistance = (_numberOfImagesPerCol * Input.YResolution) * PsoAngularInterval;
            _psoAccelerationAngle = Input.PsoAccelerationAngle;
            _startAngle = Input.StartAngle;
            _xOffsetPerRound = (Input.XResolution * Input.PixelSize.Millimeters).Millimeters();
        }

        private void ExecuteAcquisition()
        {
            if (Input.SensorRecipes.Count == 0)
            {
                throw new ArgumentOutOfRangeException("No sensors selected for acquisition");
            }

            var roundCount = Input.SensorRecipes.Max(x => x.Value.RevolutionCount);
            for (int i = 1; i < roundCount; i++)
            {
                List<SensorID> selectedSensors = new List<SensorID>();
                foreach (var recipe in Input.SensorRecipes)
                {
                    if (recipe.Value.RevolutionCount >= i)
                    {
                        selectedSensors.Add(recipe.Key);
                    }
                }

                MoveSensors(selectedSensors, i);
                PerformAcquisitionRound(selectedSensors);
            }
        }

        /// <summary>
        /// Performs a round of acquisition. Before performing this function, sensors must be moving to their position or already placed.
        /// This function will throw an error if it cannot perform this acquisition.
        /// </summary>
        private void PerformAcquisitionRound(List<SensorID> usedSensorsList)
        {
            _axes.WaitMotionEnd(10000);
            // disable pso.
            _axes.DisablePSO();
            CheckCancellation();
            var axisTConfig = _axes.GetAxisConfigById("T") as AerotechAxisConfig;
            var anglePreStart = new Angle(_startAngle - _psoAccelerationAngle, AngleUnit.Degree);
            var movePreStart = new RotationalAxisMove("T", anglePreStart, new Speed(axisTConfig.SpeedFast),
                new Acceleration(axisTConfig.AccelFast));
            _axes.Move(movePreStart);

            // wait for end of stage theta move
            _axes.WaitMotionEnd(10000);

            // set pso with _psoAngularInterval
            try
            {
                _axes.SetPSOInFixedWindowMode(_psoAccelerationAngle,
                    _psoAccelerationAngle + _angleDistance,
                    PsoAngularInterval);
            }
            catch
            {
                throw new Exception("Unable to set pso window mode for round acquisition");
            }

            CheckCancellation();

            // start digitizers
            foreach (SensorID SensorUnit in usedSensorsList)
            {
                if (!_hardwareManager.Cameras.TryGetValue(SensorUnit.ToString(), out var camera))
                {
                    throw new Exception("Unable to start digitizer grab for round acquisition. Sensor : " + SensorUnit);
                }

                camera.StartSequentialGrab((uint)_numberOfImagesPerCol);
            }

            // start acquisition --> move stage to p_dStartAngle + m_dPsoAccelerationAngle + p_dAngleDistance
            var angle = new Angle(_startAngle + _angleDistance + _psoAccelerationAngle, AngleUnit.Degree);
            var moveAcquisition = new RotationalAxisMove("T", angle, new Speed(AngularSpeed),
                new Acceleration(axisTConfig.AccelMeasure));
            _axes.Move(moveAcquisition);

            // disable pso
            _axes.DisablePSO();
            CheckCancellation();

            // TODO: check if the acquisition timed out

            CheckCancellation();
        }

        private void MoveSensors(List<SensorID> sensorList, int round)
        {
            foreach (var sensor in sensorList)
            {
                if (round == 1)
                {
                    switch (sensor)
                    {
                        case SensorID.Top:
                            var moveTopX = GenerateCalibrationMove("TopX");
                            var moveTopZ = GenerateCalibrationMove("TopZ");
                            _axes.Move(moveTopX, moveTopZ);
                            break;

                        case SensorID.TopBevel:
                            var moveTopBevel = GenerateCalibrationMove("BevelTopZ");
                            _axes.Move(moveTopBevel);
                            break;

                        case SensorID.Bottom:
                            var moveBottomX = GenerateCalibrationMove("BottomX");
                            _axes.Move(moveBottomX);
                            break;

                        case SensorID.Apex:
                            var moveApex = GenerateCalibrationMove("ApexZ");
                            _axes.Move(moveApex);
                            break;

                        case SensorID.BottomBevel:
                            break; //No newport drive on bottom bevel sensor
                    }
                }
                else
                {
                    var offset = new Length((round - 1) * _xOffsetPerRound.Millimeters, LengthUnit.Millimeter);
                    switch (sensor)
                    {
                        case SensorID.Top:
                            var moveTopX = GenerateCalibrationMove("TopX");
                            moveTopX.Position += offset;
                            _axes.Move(moveTopX);
                            break;

                        case SensorID.Bottom:
                            var moveBottomX = GenerateCalibrationMove("BottomX");
                            moveBottomX.Position += offset;
                            _axes.Move(moveBottomX);
                            break;
                    }
                }
            }
        }

        private PMAxisMove GenerateCalibrationMove(string axID)
        {
            var axConf = _axes.GetAxisConfigById(axID) as PiezoAxisConfig;
            if (axConf is null)
            {
                throw new Exception($"AxisId {axID} cannot be found");
            }

            var speed = new Speed(axConf.SpeedFast);
            var accel = new Acceleration(axConf.AccelFast);
            var position = new Length(Input.SensorCalibrationByAxesId[axID], LengthUnit.Millimeter);
            return new PMAxisMove(axID, position, speed, accel);
        }
    }
}
