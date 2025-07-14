using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.ModelingFramework;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule
{
    public partial class ProcessModule : IExtendedMaterialLocationContainer
    {
        #region Properties

        public OneToManyComposition<MaterialLocation> MaterialLocations { get; protected set; }

        #endregion

        #region Private Methods

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        #endregion

        #region IExtendedMaterialLocationContainer

        public virtual void ReadyToLoad(byte slot, CommandContext context)
        {
            CheckReady(context);
            if (MaterialLocations.Any(l => l.Material != null))
            {
                context.AddContextError(
                    $"Can not execute {context.Command.Name} command, {Name} has a material present.");
            }
        }

        public virtual void ReadyToUnload(byte slot, CommandContext context)
        {
            CheckReady(context);
            if (MaterialLocations.All(l => l.Material == null))
            {
                context.AddContextError(
                    $"Can not execute {context.Command.Name} command, {Name} has no material present.");
            }
        }

        protected virtual void CheckReady(CommandContext context)
        {
            if (State != OperatingModes.Idle && State != OperatingModes.Maintenance)
            {
                context.AddContextError(
                    $"Can not execute {context.Command.Name} command, {Name} is busy.");
            }
        }

        public virtual SampleDimension GetMaterialDimension(byte slot)
        {
            if (MaterialLocations[slot] == null)
            {
                throw new InvalidOperationException(
                    $"Material location does not exist: {Name} slot {slot}");
            }

            return (MaterialLocations[slot].Material as Substrate)?.MaterialDimension
                   ?? SampleDimension.NoDimension;
        }

        protected abstract void InternalPrepareForTransfer(byte slot, TransferType transferType);

        protected abstract void InternalPrepareForProcess(byte slot, bool automaticStart);

        #endregion
    }
}
