using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort;
using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration;
using UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Converters;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status;
using UnitySC.Equipment.Abstractions.Devices.Robot.Converters;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Configuration;
using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x
{
    public partial class MapperRR75x : IConfigurableDevice<MapperRR75xConfiguration>
    {
        #region Private Methods

        private readonly Queue<ILayingPlanLoadPort> _mappingQueue = new();
        private readonly List<List<RR75xSlotState>> _mappingResultList = new();
        private bool _mappingInProgress;
        #endregion

        #region Properties

        private IMappingPositionConverter RobotMappingPositionConverter { get; set; }
  
        #endregion

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
                        Driver.CarrierMapped += Driver_CarrierMapped;
                    }

                    RobotMappingPositionConverter = new RobotMappingPositionConverter(this);
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion

        #region Commands

        protected override void InternalInitialize(bool mustForceInit)
        {
            _mappingQueue.Clear();
            base.InternalInitialize(mustForceInit);
        }

        protected virtual void InternalMapLocation(IMaterialLocationContainer location)
        {
            try
            {
                if (location is not LayingPlanLoadPort loadPort)
                {
                    throw new InvalidOperationException(
                        $"Specified location must be of type {nameof(LayingPlanLoadPort)}");
                }

                var locationId = RegisteredLocations[loadPort.Name];
                RunMappingSequence(locationId);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
                throw;
            }
        }

        protected virtual void InternalMapTransferLocation(TransferLocation location)
        {
            try
            {
                var foundDeviceName = RegisteredLocations.FirstOrDefault(x => x.Value == location);
                var loadPort = this.GetEquipment()
                    .AllDevices<LayingPlanLoadPort>()
                    .FirstOrDefault(lp => lp.Name == foundDeviceName.Key);
                if (loadPort == null)
                {
                    throw new InvalidOperationException(
                        $"Specified location must be of type {nameof(LayingPlanLoadPort)}");
                }

                RunMappingSequence(location);
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
                throw;
            }
        }

        protected override void InternalPlace(
            RobotArm arm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot)
        {
            try
            {
                base.InternalPlace(arm, destinationDevice, destinationSlot);
                DequeueMapping();
            }
            catch (Exception e)
            {
                MarkExecutionAsFailed(e);
                throw;
            }
        }

        #endregion

        #region Public Methods

        public void EnqueueMapping(ILayingPlanLoadPort loadPort)
        {
            _mappingQueue.Enqueue(loadPort);
        }

        #endregion

        #region Configuration

        public override string RelativeConfigurationDir
            => $"./Devices/{nameof(Equipment.Abstractions.Devices.Robot.Robot)}/{nameof(MapperRR75x)}/Resources";

        public new MapperRR75xConfiguration Configuration
            => ConfigurationExtension.Cast<MapperRR75xConfiguration>(base.Configuration);

        public new MapperRR75xConfiguration CreateDefaultConfiguration()
        {
            return new MapperRR75xConfiguration();
        }

        public override void LoadConfiguration(string deviceConfigRootPath = "")
        {
            ConfigManager ??= this.LoadDeviceConfiguration<MapperRR75xConfiguration>(
                deviceConfigRootPath,
                ConfigurationFileName,
                Logger);
        }

        #endregion

        #region Private Methods

        private void RunMappingSequence(TransferLocation locationId)
        {
            _mappingInProgress = true;
            _mappingResultList.Clear();
            for (var indexMapping = 1; indexMapping <= 2; indexMapping++)
            {
                Map(locationId, indexMapping);
            }

            //Wait Rorze logic
            SetMappingOnLoadPort(locationId, _mappingResultList[0]);
            _mappingInProgress = false;
            DequeueMapping();
        }

        private void WaitMappingResult(int expectedCount)
        {
            var startTime = DateTime.Now;
            while (_mappingResultList.Count < expectedCount
                   && (DateTime.Now - startTime).TotalSeconds < Configuration.InitializationTimeout)
            {
                Thread.Sleep(100);
            }
        }

        private void SetMappingOnLoadPort(TransferLocation locationId, List<RR75xSlotState> mapping)
        {
            var foundDeviceName = RegisteredLocations.FirstOrDefault(x => x.Value == locationId);
            var loadPort = this.GetEquipment()
                .AllDevices<LayingPlanLoadPort>()
                .FirstOrDefault(lp => lp.Name == foundDeviceName.Key);
            if (loadPort == null)
            {
                return;
            }

            loadPort.SetMapping(new Collection<RR75xSlotState>(mapping));
        }

        private void Map(TransferLocation locationId, int indexMapping)
        {
            var stg = RobotMappingPositionConverter.ToMappingPosition(
                locationId,
                RobotArm.Arm1,
                indexMapping == 1,
                false);
            DriverWrapper.RunCommand(delegate { Driver.Map(locationId, stg); }, RobotCommand.Map);
            DriverWrapper.RunCommand(
                delegate { Driver.GetLastMapping(stg); },
                RobotCommand.GetLastMapping);
            WaitMappingResult(indexMapping);
        }

        private void DequeueMapping()
        {
            while (_mappingQueue.Count > 0)
            {
                Thread.Sleep(100);
                var queuedLoadPort = _mappingQueue.Dequeue();
                if (ExecutionMode == ExecutionMode.Real)
                {
                    InternalMapLocation(queuedLoadPort);
                }
                else
                {
                    InternalSimulateMapLocation(
                        queuedLoadPort,
                        new Tempomat(Ratio.FromPercent(100)));
                }
            }
        }

        #endregion

        #region Overrides

        protected override void SetRobotPosition(RobotGposStatus status)
        {
            // Robot is at origin
            if (status.XAxis == 0)
            {
                Position = TransferLocation.DummyPortA;
            }

            // Robot is moving
            else if (status.XAxis == 999 || status.XAxis != status.RotationAxis)
            {
                Position = TransferLocation.Robot;
            }
            else
            {
                Position = DriverWrapper.RunningCommand == RobotCommand.Map
                    ? RobotMappingPositionConverter.ToTransferLocation(status.XAxis, false)
                    : RobotStoppingPositionConverter.ToTransferLocation(status.XAxis, false);
            }

            RobotPositionReverted = _mappingInProgress;
        }

        protected override void UpdateArmState()
        {
            if (CurrentCommand is nameof(IMapperRR75x.MapLocation)
                or nameof(IMapperRR75x.MapTransferLocation))
            {
                // Update abstraction statuses
                UpperArmState = O_UpperArmOrigin_LogicSignal
                    ? ArmState.Retracted
                    : ArmState.Inverted;
                LowerArmState = O_LowerArmOrigin_LogicSignal
                    ? ArmState.Retracted
                    : ArmState.Inverted;
            }
            else
            {
                // Update abstraction statuses
                UpperArmState = O_UpperArmOrigin_LogicSignal
                    ? ArmState.Retracted
                    : ArmState.Extended;
                LowerArmState = O_LowerArmOrigin_LogicSignal
                    ? ArmState.Retracted
                    : ArmState.Extended;
            }
        }

        #endregion

        #region Event Handlers

        private void Driver_CarrierMapped(object sender, RR75x.Driver.EventArgs.MappingEventArgs e)
        {
            _mappingResultList.Add(e.Mapping.ToList());
        }

        #endregion
    }
}
