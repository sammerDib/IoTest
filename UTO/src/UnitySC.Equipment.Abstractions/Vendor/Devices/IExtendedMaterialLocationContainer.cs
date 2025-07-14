using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices
{
    public interface IExtendedMaterialLocationContainer : IMaterialLocationContainer
    {
        /// <summary>
        /// Request if the device is ready to be loaded by an external device
        /// </summary>
        /// <param name="slot">The slot to be loaded</param>
        /// <param name="context">The context where the errors will be added if the device is not ready to load</param>
        void ReadyToLoad(byte slot, CommandContext context);

        /// <summary>
        /// Request if the device is ready to be unloaded by an external device
        /// </summary>
        /// <param name="slot">The slot to be unloaded</param>
        /// <param name="context">The context where the errors will be added if the device is not ready to load</param>
        void ReadyToUnload(byte slot, CommandContext context);

        /// <summary>
        /// Gets the material dimension
        /// </summary>
        /// <param name="slot">The slot where the material dimension is requested</param>
        /// <returns>The material dimension on the desired slot</returns>
        SampleDimension GetMaterialDimension(byte slot);

        /// <summary>
        /// Asks the device to prepare itself for a transfer by an external device
        /// </summary>
        /// <param name="slot">The slot where the transfer will be made</param>
        /// <param name="transferType">The type of the transfer</param>
        void PrepareForTransfer(byte slot, TransferType transferType);

        /// <summary>
        /// Asks the device to prepare itself to start its process
        /// </summary>
        /// <param name="slot">The slot where the process must be prepared</param>
        /// <param name="automaticStart">True : the process will start automatically</param>
        void PrepareForProcess(byte slot, bool automaticStart = false);

        /// <summary>
        /// Asks the device to prepare itself asynchronously for a transfer by an external device
        /// </summary>
        /// <param name="slot">The slot where the transfer will be made</param>
        /// <param name="transferType">The type of the transfer</param>
        /// <returns>The asynchronous task</returns>
        Task PrepareForTransferAsync(byte slot, TransferType transferType);

        /// <summary>
        /// Asks the device to prepare itself to start its process asynchronously
        /// </summary>
        /// <param name="slot">The slot where the process must be prepared</param>
        /// <param name="automaticStart">True : the process will start automatically</param>
        /// <returns>The asynchronous task</returns>
        Task PrepareForProcessAsync(byte slot, bool automaticStart = false);

        /// <summary>
        /// True if the device is ready for a transfer by an external device
        /// </summary>
        bool IsReadyForTransfer { get; }

        /// <summary>
        /// True if the device is ready to be prepared for a transfer
        /// Needed if the device must keep in certain (door close for example) when no transfer is required
        /// </summary>
        bool IsReadyToAcceptTransfer { get; }
    }
}
