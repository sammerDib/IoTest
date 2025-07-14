using System;
using System.Linq;

using Agileo.EquipmentModeling;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Configuration;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0
{
    public partial class Dio0
        : IConfigurableDevice<Dio0Configuration>, IFfuIos, ILayingPlanLoadPortIos
    {
        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            base.SetUp(phase);
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    break;
                case SetupPhase.SettingUp:
                    if (ExecutionMode == ExecutionMode.Real)
                    {
                        Driver.GpioReceived += Driver_GpioReceived;
                        Driver.FansRotationSpeedChanged += Driver_FansRotationSpeedChanged;
                        Driver.PressureSensorsValueChanged += Driver_PressureSensorsValueChanged;
                        Driver.Dio0SignalDataReceived += Driver_Dio0SignalDataReceived;
                    }
                    else
                    {
                        SetUpSimulatedMode();
                    }

                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region Properties

        public new Dio0Driver Driver
        {
            get => base.Driver as Dio0Driver;
            set => base.Driver = value;
        }

        #endregion Properties

        #region IDio0

        protected virtual void InternalSetFfuSpeed(RotationalSpeed setPoint)
        {
            try
            {
                if (setPoint <= RotationalSpeed.FromRevolutionsPerMinute(0))
                {
                    DriverWrapper.RunCommand(
                        delegate { Driver.StopFanRotation(); },
                        Dio0Command.StopFanRotation);
                }
                else
                {
                    DriverWrapper.RunCommand(
                        delegate
                        {
                            Driver.StartFanRotation(
                                (uint)setPoint.As(RotationalSpeedUnit.RevolutionPerMinute));
                        },
                        Dio0Command.StartFanRotation);
                }
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
            }
        }

        #endregion IDio0

        #region Configuration

        public Dio0Configuration CreateDefaultConfiguration()
        {
            return new Dio0Configuration();
        }

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(IoModule)}/{nameof(RC550)}/{nameof(Dio0)}/Resources";

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration(
                deviceConfigRootPath,
                Logger,
                InstanceId);
        }

        #endregion Configuration

        #region Event Handlers

        private void Driver_GpioReceived(object sender, StatusEventArgs<RC550GeneralIoStatus> e)
        {
            // Inputs
            I_FAN1Rotating = e.Status.FAN1Rotating;
            I_FAN2Rotating = e.Status.FAN2Rotating;
            I_FAN3Rotating = e.Status.FAN3Rotating;
            I_FAN4Rotating = e.Status.FAN4Rotating;
            I_FAN5Rotating = e.Status.FAN5Rotating;
            I_FAN6Rotating = e.Status.FAN6Rotating;
            I_FAN7Rotating = e.Status.FAN7Rotating;
            I_FAN8Rotating = e.Status.FAN8Rotating;
            I_FAN9Rotating = e.Status.FAN9Rotating;
            I_FAN10Rotating = e.Status.FAN10Rotating;
            I_FAN11Rotating = e.Status.FAN11Rotating;
            I_FAN12Rotating = e.Status.FAN12Rotating;
            I_FAN13Rotating = e.Status.FAN13Rotating;
            I_FAN14Rotating = e.Status.FAN14Rotating;
            I_FAN15Rotating = e.Status.FAN15Rotating;
            I_FAN16Rotating = e.Status.FAN16Rotating;
            I_FAN17Rotating = e.Status.FAN17Rotating;
            I_FAN18Rotating = e.Status.FAN18Rotating;
            I_FAN19Rotating = e.Status.FAN19Rotating;
            I_FAN20Rotating = e.Status.FAN20Rotating;
            I_FAN1AlarmOccurred = e.Status.FAN1AlarmOccurred;
            I_FAN2AlarmOccurred = e.Status.FAN2AlarmOccurred;
            I_FAN3AlarmOccurred = e.Status.FAN3AlarmOccurred;
            I_FAN4AlarmOccurred = e.Status.FAN4AlarmOccurred;
            I_FAN5AlarmOccurred = e.Status.FAN5AlarmOccurred;
            I_FAN6AlarmOccurred = e.Status.FAN6AlarmOccurred;
            I_FAN7AlarmOccurred = e.Status.FAN7AlarmOccurred;
            I_FAN8AlarmOccurred = e.Status.FAN8AlarmOccurred;
            I_FAN9AlarmOccurred = e.Status.FAN9AlarmOccurred;
            I_FAN10AlarmOccurred = e.Status.FAN10AlarmOccurred;
            I_FAN11AlarmOccurred = e.Status.FAN11AlarmOccurred;
            I_FAN12AlarmOccurred = e.Status.FAN12AlarmOccurred;
            I_FAN13AlarmOccurred = e.Status.FAN13AlarmOccurred;
            I_FAN14AlarmOccurred = e.Status.FAN14AlarmOccurred;
            I_FAN15AlarmOccurred = e.Status.FAN15AlarmOccurred;
            I_FAN16AlarmOccurred = e.Status.FAN16AlarmOccurred;
            I_FAN17AlarmOccurred = e.Status.FAN17AlarmOccurred;
            I_FAN18AlarmOccurred = e.Status.FAN18AlarmOccurred;
            I_FAN19AlarmOccurred = e.Status.FAN19AlarmOccurred;
            I_FAN20AlarmOccurred = e.Status.FAN20AlarmOccurred;
            Alarm = I_FAN1AlarmOccurred
                    || I_FAN2AlarmOccurred
                    || I_FAN3AlarmOccurred
                    || I_FAN4AlarmOccurred
                    || I_FAN5AlarmOccurred
                    || I_FAN6AlarmOccurred
                    || I_FAN7AlarmOccurred
                    || I_FAN8AlarmOccurred
                    || I_FAN9AlarmOccurred
                    || I_FAN10AlarmOccurred
                    || I_FAN11AlarmOccurred
                    || I_FAN12AlarmOccurred
                    || I_FAN13AlarmOccurred
                    || I_FAN14AlarmOccurred
                    || I_FAN15AlarmOccurred
                    || I_FAN16AlarmOccurred
                    || I_FAN17AlarmOccurred
                    || I_FAN18AlarmOccurred
                    || I_FAN19AlarmOccurred
                    || I_FAN20AlarmOccurred;
            I_Sensor1_WithinUpperLimitThresholdValue =
                e.Status.Sensor1_WithinUpperLimitThresholdValue;
            I_Sensor1_WithinLowerLimitThresholdValue =
                e.Status.Sensor1_WithinLowerLimitThresholdValue;
            I_Sensor2_WithinUpperLimitThresholdValue =
                e.Status.Sensor2_WithinUpperLimitThresholdValue;
            I_Sensor2_WithinLowerLimitThresholdValue =
                e.Status.Sensor2_WithinLowerLimitThresholdValue;
            I_Sensor3_WithinUpperLimitThresholdValue =
                e.Status.Sensor3_WithinUpperLimitThresholdValue;
            I_Sensor3_WithinLowerLimitThresholdValue =
                e.Status.Sensor3_WithinLowerLimitThresholdValue;
            I_Sensor4_WithinUpperLimitThresholdValue =
                e.Status.Sensor4_WithinUpperLimitThresholdValue;
            I_Sensor4_WithinLowerLimitThresholdValue =
                e.Status.Sensor4_WithinLowerLimitThresholdValue;
            I_Sensor5_WithinUpperLimitThresholdValue =
                e.Status.Sensor5_WithinUpperLimitThresholdValue;
            I_Sensor5_WithinLowerLimitThresholdValue =
                e.Status.Sensor5_WithinLowerLimitThresholdValue;
            I_Sensor6_WithinUpperLimitThresholdValue =
                e.Status.Sensor6_WithinUpperLimitThresholdValue;
            I_Sensor6_WithinLowerLimitThresholdValue =
                e.Status.Sensor6_WithinLowerLimitThresholdValue;
            I_Sensor7_WithinUpperLimitThresholdValue =
                e.Status.Sensor7_WithinUpperLimitThresholdValue;
            I_Sensor7_WithinLowerLimitThresholdValue =
                e.Status.Sensor7_WithinLowerLimitThresholdValue;
            I_Sensor8_WithinUpperLimitThresholdValue =
                e.Status.Sensor8_WithinUpperLimitThresholdValue;
            I_Sensor8_WithinLowerLimitThresholdValue =
                e.Status.Sensor8_WithinLowerLimitThresholdValue;
            I_Sensor9_WithinUpperLimitThresholdValue =
                e.Status.Sensor9_WithinUpperLimitThresholdValue;
            I_Sensor9_WithinLowerLimitThresholdValue =
                e.Status.Sensor9_WithinLowerLimitThresholdValue;
            I_Sensor10_WithinUpperLimitThresholdValue =
                e.Status.Sensor10_WithinUpperLimitThresholdValue;
            I_Sensor10_WithinLowerLimitThresholdValue =
                e.Status.Sensor10_WithinLowerLimitThresholdValue;
            I_Sensor11_WithinUpperLimitThresholdValue =
                e.Status.Sensor11_WithinUpperLimitThresholdValue;
            I_Sensor11_WithinLowerLimitThresholdValue =
                e.Status.Sensor11_WithinLowerLimitThresholdValue;
            I_Sensor12_WithinUpperLimitThresholdValue =
                e.Status.Sensor12_WithinUpperLimitThresholdValue;
            I_Sensor12_WithinLowerLimitThresholdValue =
                e.Status.Sensor12_WithinLowerLimitThresholdValue;
            I_ControllerDirectInput_IN0 = e.Status.ControllerDirectInput_IN0;
            I_ControllerDirectInput_IN1 = e.Status.ControllerDirectInput_IN1;
            I_ControllerDirectInput_IN2 = e.Status.ControllerDirectInput_IN2;
            I_ControllerDirectInput_IN3 = e.Status.ControllerDirectInput_IN3;

            // Outputs
            O_SystemIsReady = e.Status.SystemIsReady;
            O_BatchAlarmClear_1ShotOutput = e.Status.BatchAlarmClear_1ShotOutput;
            O_Fan_OperationStop_AllUsingFans_1ShotOutput =
                e.Status.Fan_OperationStop_AllUsingFans_1ShotOutput;
            O_Fan_OperationStart_AllUsingFans_1ShotOutput =
                e.Status.Fan_OperationStart_AllUsingFans_1ShotOutput;
            O_FAN1_OperationStart_1ShotOutput = e.Status.FAN1_OperationStart_1ShotOutput;
            O_FAN2_OperationStart_1ShotOutput = e.Status.FAN2_OperationStart_1ShotOutput;
            O_FAN3_OperationStart_1ShotOutput = e.Status.FAN3_OperationStart_1ShotOutput;
            O_FAN4_OperationStart_1ShotOutput = e.Status.FAN4_OperationStart_1ShotOutput;
            O_FAN5_OperationStart_1ShotOutput = e.Status.FAN5_OperationStart_1ShotOutput;
            O_FAN6_OperationStart_1ShotOutput = e.Status.FAN6_OperationStart_1ShotOutput;
            O_FAN7_OperationStart_1ShotOutput = e.Status.FAN7_OperationStart_1ShotOutput;
            O_FAN8_OperationStart_1ShotOutput = e.Status.FAN8_OperationStart_1ShotOutput;
            O_FAN9_OperationStart_1ShotOutput = e.Status.FAN9_OperationStart_1ShotOutput;
            O_FAN10_OperationStart_1ShotOutput = e.Status.FAN10_OperationStart_1ShotOutput;
            O_FAN11_OperationStart_1ShotOutput = e.Status.FAN11_OperationStart_1ShotOutput;
            O_FAN12_OperationStart_1ShotOutput = e.Status.FAN12_OperationStart_1ShotOutput;
            O_FAN13_OperationStart_1ShotOutput = e.Status.FAN13_OperationStart_1ShotOutput;
            O_FAN14_OperationStart_1ShotOutput = e.Status.FAN14_OperationStart_1ShotOutput;
            O_FAN15_OperationStart_1ShotOutput = e.Status.FAN15_OperationStart_1ShotOutput;
            O_FAN16_OperationStart_1ShotOutput = e.Status.FAN16_OperationStart_1ShotOutput;
            O_FAN17_OperationStart_1ShotOutput = e.Status.FAN17_OperationStart_1ShotOutput;
            O_FAN18_OperationStart_1ShotOutput = e.Status.FAN18_OperationStart_1ShotOutput;
            O_FAN19_OperationStart_1ShotOutput = e.Status.FAN19_OperationStart_1ShotOutput;
            O_FAN20_OperationStart_1ShotOutput = e.Status.FAN20_OperationStart_1ShotOutput;
            O_FAN1_AlarmClear_1ShotOutput = e.Status.FAN1_AlarmClear_1ShotOutput;
            O_FAN2_AlarmClear_1ShotOutput = e.Status.FAN2_AlarmClear_1ShotOutput;
            O_FAN3_AlarmClear_1ShotOutput = e.Status.FAN3_AlarmClear_1ShotOutput;
            O_FAN4_AlarmClear_1ShotOutput = e.Status.FAN4_AlarmClear_1ShotOutput;
            O_FAN5_AlarmClear_1ShotOutput = e.Status.FAN5_AlarmClear_1ShotOutput;
            O_FAN6_AlarmClear_1ShotOutput = e.Status.FAN6_AlarmClear_1ShotOutput;
            O_FAN7_AlarmClear_1ShotOutput = e.Status.FAN7_AlarmClear_1ShotOutput;
            O_FAN8_AlarmClear_1ShotOutput = e.Status.FAN8_AlarmClear_1ShotOutput;
            O_FAN9_AlarmClear_1ShotOutput = e.Status.FAN9_AlarmClear_1ShotOutput;
            O_FAN10_AlarmClear_1ShotOutput = e.Status.FAN10_AlarmClear_1ShotOutput;
            O_FAN11_AlarmClear_1ShotOutput = e.Status.FAN11_AlarmClear_1ShotOutput;
            O_FAN12_AlarmClear_1ShotOutput = e.Status.FAN12_AlarmClear_1ShotOutput;
            O_FAN13_AlarmClear_1ShotOutput = e.Status.FAN13_AlarmClear_1ShotOutput;
            O_FAN14_AlarmClear_1ShotOutput = e.Status.FAN14_AlarmClear_1ShotOutput;
            O_FAN15_AlarmClear_1ShotOutput = e.Status.FAN15_AlarmClear_1ShotOutput;
            O_FAN16_AlarmClear_1ShotOutput = e.Status.FAN16_AlarmClear_1ShotOutput;
            O_FAN17_AlarmClear_1ShotOutput = e.Status.FAN17_AlarmClear_1ShotOutput;
            O_FAN18_AlarmClear_1ShotOutput = e.Status.FAN18_AlarmClear_1ShotOutput;
            O_FAN19_AlarmClear_1ShotOutput = e.Status.FAN19_AlarmClear_1ShotOutput;
            O_FAN20_AlarmClear_1ShotOutput = e.Status.FAN20_AlarmClear_1ShotOutput;
            O_FAN1_OperationStop_1ShotOutput = e.Status.FAN1_OperationStop_1ShotOutput;
            O_FAN2_OperationStop_1ShotOutput = e.Status.FAN2_OperationStop_1ShotOutput;
            O_FAN3_OperationStop_1ShotOutput = e.Status.FAN3_OperationStop_1ShotOutput;
            O_FAN4_OperationStop_1ShotOutput = e.Status.FAN4_OperationStop_1ShotOutput;
            O_FAN5_OperationStop_1ShotOutput = e.Status.FAN5_OperationStop_1ShotOutput;
            O_FAN6_OperationStop_1ShotOutput = e.Status.FAN6_OperationStop_1ShotOutput;
            O_FAN7_OperationStop_1ShotOutput = e.Status.FAN7_OperationStop_1ShotOutput;
            O_FAN8_OperationStop_1ShotOutput = e.Status.FAN8_OperationStop_1ShotOutput;
            O_FAN9_OperationStop_1ShotOutput = e.Status.FAN9_OperationStop_1ShotOutput;
            O_FAN10_OperationStop_1ShotOutput = e.Status.FAN10_OperationStop_1ShotOutput;
            O_FAN11_OperationStop_1ShotOutput = e.Status.FAN11_OperationStop_1ShotOutput;
            O_FAN12_OperationStop_1ShotOutput = e.Status.FAN12_OperationStop_1ShotOutput;
            O_FAN13_OperationStop_1ShotOutput = e.Status.FAN13_OperationStop_1ShotOutput;
            O_FAN14_OperationStop_1ShotOutput = e.Status.FAN14_OperationStop_1ShotOutput;
            O_FAN15_OperationStop_1ShotOutput = e.Status.FAN15_OperationStop_1ShotOutput;
            O_FAN16_OperationStop_1ShotOutput = e.Status.FAN16_OperationStop_1ShotOutput;
            O_FAN17_OperationStop_1ShotOutput = e.Status.FAN17_OperationStop_1ShotOutput;
            O_FAN18_OperationStop_1ShotOutput = e.Status.FAN18_OperationStop_1ShotOutput;
            O_FAN19_OperationStop_1ShotOutput = e.Status.FAN19_OperationStop_1ShotOutput;
            O_FAN20_OperationStop_1ShotOutput = e.Status.FAN20_OperationStop_1ShotOutput;
        }

        private void Driver_FansRotationSpeedChanged(
            object sender,
            StatusEventArgs<FansRotationSpeed> e)
        {
            var meanSpeed = e.Status.FansSpeed.Values.Aggregate(
                0d,
                (current, speed) => current + speed);
            meanSpeed /= e.Status.FansSpeed.Count;
            FanSpeed = RotationalSpeed.FromRevolutionsPerMinute(meanSpeed);
        }

        private void Driver_PressureSensorsValueChanged(
            object sender,
            StatusEventArgs<PressureSensorsValues> e)
        {
            var meanPressure = e.Status.PressureValuesPerSensor.Values.Aggregate(
                Pressure.Zero,
                (current, pressure) => current + pressure);
            meanPressure /= e.Status.PressureValuesPerSensor.Count;
            MeanPressure = Pressure.FromMillipascals(meanPressure.Millipascals);
        }

        private void Driver_Dio0SignalDataReceived(object sender, StatusEventArgs<SignalData> e)
        {
            switch (e.Status)
            {
                case FanDetectionSignalData fanDetectionSignalData:
                    I_FANDetection1 = fanDetectionSignalData.FanDetection1;
                    I_FANDetection2 = fanDetectionSignalData.FanDetection2;
                    I_DvrAlarm = fanDetectionSignalData.DvrAlarm;
                    break;
                case LayingPlanLoadPortSignalData layingPlanLoadPortSignalData:
                    switch ((IoModuleIds)layingPlanLoadPortSignalData.IoModuleNo)
                    {
                        // LP1
                        case IoModuleIds.RC550_HCL1_ID0:
                            PlacementSensorALoadPort1 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorA;
                            PlacementSensorBLoadPort1 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorB;
                            PlacementSensorCLoadPort1 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorC;
                            PlacementSensorDLoadPort1 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorD;
                            WaferProtrudeSensor1LoadPort1 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor1;
                            WaferProtrudeSensor2LoadPort1 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor2;
                            WaferProtrudeSensor3LoadPort1 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor3;
                            break;

                        // LP2
                        case IoModuleIds.RC550_HCL1_ID1:
                            PlacementSensorALoadPort2 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorA;
                            PlacementSensorBLoadPort2 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorB;
                            PlacementSensorCLoadPort2 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorC;
                            PlacementSensorDLoadPort2 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorD;
                            WaferProtrudeSensor1LoadPort2 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor1;
                            WaferProtrudeSensor2LoadPort2 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor2;
                            WaferProtrudeSensor3LoadPort2 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor3;
                            break;

                        // LP3
                        case IoModuleIds.RC550_HCL1_ID2:
                            PlacementSensorALoadPort3 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorA;
                            PlacementSensorBLoadPort3 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorB;
                            PlacementSensorCLoadPort3 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorC;
                            PlacementSensorDLoadPort3 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorD;
                            WaferProtrudeSensor1LoadPort3 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor1;
                            WaferProtrudeSensor2LoadPort3 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor2;
                            WaferProtrudeSensor3LoadPort3 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor3;
                            break;

                        // LP4
                        case IoModuleIds.RC550_HCL1_ID3:
                            PlacementSensorALoadPort4 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorA;
                            PlacementSensorBLoadPort4 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorB;
                            PlacementSensorCLoadPort4 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorC;
                            PlacementSensorDLoadPort4 =
                                !layingPlanLoadPortSignalData.I_PlacementSensorD;
                            WaferProtrudeSensor1LoadPort4 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor1;
                            WaferProtrudeSensor2LoadPort4 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor2;
                            WaferProtrudeSensor3LoadPort4 =
                                !layingPlanLoadPortSignalData.I_WaferProtrudeSensor3;
                            break;
                    }

                    break;
                case E84SignalData e84SignalData:
                    switch ((IoModuleIds)e84SignalData.IoModuleNo)
                    {
                        // LP1 E84
                        case IoModuleIds.SB078_Port4_HCL0_ID0:
                            I_Lp1_Valid = e84SignalData.I_Valid;
                            I_Lp1_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp1_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp1_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp1_Busy = e84SignalData.I_Busy;
                            I_Lp1_Compt = e84SignalData.I_Compt;
                            I_Lp1_Cont = e84SignalData.I_Cont;
                            O_Lp1_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp1_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp1_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL0_ID1:
                            O_Lp1_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp1_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP2 E84
                        case IoModuleIds.SB078_Port4_HCL0_ID2:
                            I_Lp2_Valid = e84SignalData.I_Valid;
                            I_Lp2_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp2_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp2_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp2_Busy = e84SignalData.I_Busy;
                            I_Lp2_Compt = e84SignalData.I_Compt;
                            I_Lp2_Cont = e84SignalData.I_Cont;
                            O_Lp2_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp2_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp2_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL0_ID3:
                            O_Lp2_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp2_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP3 E84
                        case IoModuleIds.SB078_Port4_HCL1_ID0:
                            I_Lp3_Valid = e84SignalData.I_Valid;
                            I_Lp3_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp3_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp3_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp3_Busy = e84SignalData.I_Busy;
                            I_Lp3_Compt = e84SignalData.I_Compt;
                            I_Lp3_Cont = e84SignalData.I_Cont;
                            O_Lp3_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp3_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp3_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL1_ID1:
                            O_Lp3_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp3_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP4 E84
                        case IoModuleIds.SB078_Port4_HCL1_ID2:
                            I_Lp4_Valid = e84SignalData.I_Valid;
                            I_Lp4_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp4_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp4_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp4_Busy = e84SignalData.I_Busy;
                            I_Lp4_Compt = e84SignalData.I_Compt;
                            I_Lp4_Cont = e84SignalData.I_Cont;
                            O_Lp4_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp4_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp4_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL1_ID3:
                            O_Lp4_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp4_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP5 E84
                        case IoModuleIds.SB078_Port4_HCL2_ID0:
                            I_Lp5_Valid = e84SignalData.I_Valid;
                            I_Lp5_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp5_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp5_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp5_Busy = e84SignalData.I_Busy;
                            I_Lp5_Compt = e84SignalData.I_Compt;
                            I_Lp5_Cont = e84SignalData.I_Cont;
                            O_Lp5_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp5_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp5_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL2_ID1:
                            O_Lp5_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp5_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP6 E84
                        case IoModuleIds.SB078_Port4_HCL2_ID2:
                            I_Lp6_Valid = e84SignalData.I_Valid;
                            I_Lp6_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp6_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp6_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp6_Busy = e84SignalData.I_Busy;
                            I_Lp6_Compt = e84SignalData.I_Compt;
                            I_Lp6_Cont = e84SignalData.I_Cont;
                            O_Lp6_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp6_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp6_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL2_ID3:
                            O_Lp6_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp6_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP7 E84
                        case IoModuleIds.SB078_Port4_HCL3_ID0:
                            I_Lp7_Valid = e84SignalData.I_Valid;
                            I_Lp7_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp7_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp7_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp7_Busy = e84SignalData.I_Busy;
                            I_Lp7_Compt = e84SignalData.I_Compt;
                            I_Lp7_Cont = e84SignalData.I_Cont;
                            O_Lp7_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp7_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp7_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL3_ID1:
                            O_Lp7_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp7_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;

                        // LP8 E84
                        case IoModuleIds.SB078_Port4_HCL3_ID2:
                            I_Lp8_Valid = e84SignalData.I_Valid;
                            I_Lp8_Cs_0 = e84SignalData.I_Cs_0;
                            I_Lp8_Cs_1 = e84SignalData.I_Cs_1;
                            I_Lp8_Tr_Req = e84SignalData.I_Tr_Req;
                            I_Lp8_Busy = e84SignalData.I_Busy;
                            I_Lp8_Compt = e84SignalData.I_Compt;
                            I_Lp8_Cont = e84SignalData.I_Cont;
                            O_Lp8_L_Req = e84SignalData.O_L_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp8_U_Req = e84SignalData.O_U_Req
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            O_Lp8_Ready = e84SignalData.O_Ready
                                          ?? throw new InvalidOperationException(
                                              "Given status could not be null when received from equipment.");
                            break;
                        case IoModuleIds.SB078_Port4_HCL3_ID3:
                            O_Lp8_Ho_Avbl = e84SignalData.O_Ho_Avbl
                                            ?? throw new InvalidOperationException(
                                                "Given status could not be null when received from equipment.");
                            O_Lp8_Es = e84SignalData.O_Es
                                       ?? throw new InvalidOperationException(
                                           "Given status could not be null when received from equipment.");
                            break;
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        $"{nameof(Dio0)} does not manage received {nameof(SignalData)}\n"
                        + $"Received signal module: \"{e.Status.IoModuleNo}\"\n"
                        + $"Received signal data: \"{e.Status.GetSignal()}\"");
            }
        }

        #endregion Event Handlers

        #region Overrides

        protected override GenericRC5xxDriver CreateDriver()
        {
            return new Dio0Driver(
                Logger,
                Configuration.CommunicationConfig.ConnectionMode,
                Configuration.CommunicationConfig.AliveBitPeriod,
                ConfigManager.Current.Cast<Dio0Configuration>().IsPressureSensorAvailable);
        }

        protected override void UpdateErrorDescription(int partOfEfemInError, int errorCode)
        {
            IoModuleInErrorDescription = (IoModuleInError)partOfEfemInError;
            ErrorDescription = (ErrorCode)errorCode;
        }

        protected override void SetOrClearAlarmByKey(int statusErrorCode)
        {
            //if ((ErrorCode) statusErrorCode != IoModule.RC550.Dio0.Driver.ErrorCode.NoError
            //    && CurrentCommand != nameof(Initialize))
            //{
            //    //New alarm detected
            //    SetAlarmById((statusErrorCode + 1000).ToString());
            //}
            //else
            //{
            //    //Clear the previously set alarm
            //    ClearAlarmById(((int)ErrorDescription + 1000).ToString());
            //}
        }

        #endregion Overrides

        #region IDisposable

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (Driver != null)
            {
                Driver.GpioReceived -= Driver_GpioReceived;
                Driver.FansRotationSpeedChanged -= Driver_FansRotationSpeedChanged;
                Driver.PressureSensorsValueChanged -= Driver_PressureSensorsValueChanged;
                Driver.Dio0SignalDataReceived -= Driver_Dio0SignalDataReceived;
            }

            if (SimulationData != null)
            {
                DisposeSimulatedMode();
            }

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
