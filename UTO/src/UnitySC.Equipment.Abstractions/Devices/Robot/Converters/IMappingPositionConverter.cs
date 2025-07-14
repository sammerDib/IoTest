using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Converters
{
    public interface IMappingPositionConverter
    {
        /// <summary>
        /// Converts a Mapping Position into a transfer location.
        /// </summary>
        /// <param name="mappingPosition">The number representing the mapping position (validity range is from 1 to 400).</param>
        /// <param name="is0BasedIndexing">Indicates if the mapping position is provided with a 0-based number.</param>
        /// <remarks>
        /// The mapping position range is from 0 to 399 if <paramref name="is0BasedIndexing"/> is set to <value>true</value>.
        /// If <paramref name="is0BasedIndexing"/> is set to <value>false</value>, validity range is from 1 to 400.
        /// Usage of 0 based indexing is typically for accessing robot HW internal mapping position data matrix.
        /// Usage of 1 based indexing is typically for sending a robot HW move command which take a mapping position into parameter.
        /// </remarks>
        public TransferLocation ToTransferLocation(uint mappingPosition, bool is0BasedIndexing);

        /// <summary>
        /// Converts a <see cref="TransferLocation"/> into a robot mapping position.
        /// </summary>
        /// <param name="transferLocation">The target destination for <paramref name="arm"/>.</param>
        /// <param name="arm">The arm to be used to go to the destination.</param>
        /// <param name="isFirstPosition">Indicates which mapping position get.</param>
        /// <param name="is0BasedIndexing">Indicates if the mapping position must be returned with a 0-based number.</param>
        /// <remarks>
        /// The mapping position range is from 0 to 399 if <paramref name="is0BasedIndexing"/> is set to <value>true</value>.
        /// If <paramref name="is0BasedIndexing"/> is set to <value>false</value>, validity range is from 1 to 400.
        /// Usage of 0 based indexing is typically for accessing robot HW internal stopping position data matrix.
        /// Usage of 1 based indexing is typically for sending a robot HW move command which take a stopping position into parameter.
        /// </remarks>
        public uint ToMappingPosition(TransferLocation transferLocation, RobotArm arm, bool isFirstPosition ,bool is0BasedIndexing);
    }
}
