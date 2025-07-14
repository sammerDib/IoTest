using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.Robot.Converters
{
    public interface IStoppingPositionConverter
    {
        /// <summary>
        /// Converts a Stopping Position (also called stage) into a transfer location.
        /// </summary>
        /// <param name="stoppingPosition">The number representing the stopping position (validity range is from 1 to 400).</param>
        /// <param name="is0BasedIndexing">Indicates if the stopping position is provided with a 0-based number.</param>
        /// <remarks>
        /// The stopping position range is from 0 to 399 if <paramref name="is0BasedIndexing"/> is set to <value>true</value>.
        /// If <paramref name="is0BasedIndexing"/> is set to <value>false</value>, validity range is from 1 to 400.
        /// Usage of 0 based indexing is typically for accessing robot HW internal stopping position data matrix.
        /// Usage of 1 based indexing is typically for sending a robot HW move command which take a stopping position into parameter.
        /// </remarks>
        public TransferLocation ToTransferLocation(double stoppingPosition, bool is0BasedIndexing);

        /// <summary>
        /// Converts a <see cref="TransferLocation"/> into a robot stopping position (also called stage).
        /// </summary>
        /// <param name="transferLocation">The target destination for <paramref name="arm"/>.</param>
        /// <param name="arm">The arm to be used to go to the destination.</param>
        /// <param name="is0BasedIndexing">Indicates if the stopping position must be returned with a 0-based number.</param>
        /// <remarks>
        /// The stopping position range is from 0 to 399 if <paramref name="is0BasedIndexing"/> is set to <value>true</value>.
        /// If <paramref name="is0BasedIndexing"/> is set to <value>false</value>, validity range is from 1 to 400.
        /// Usage of 0 based indexing is typically for accessing robot HW internal stopping position data matrix.
        /// Usage of 1 based indexing is typically for sending a robot HW move command which take a stopping position into parameter.
        /// </remarks>
        public double ToStoppingPosition(TransferLocation transferLocation, RobotArm arm, bool is0BasedIndexing);
    }
}
