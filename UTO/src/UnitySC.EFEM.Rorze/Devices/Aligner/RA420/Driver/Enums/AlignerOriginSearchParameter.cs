using System.ComponentModel;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums
{
    public enum AlignerOriginSearchParameter
    {
        [Description("Performs origin search without checking the substrate,"
            + " and release chucking of the substrate.")]
        NoCheckIfWaferPlaced = 0,

        [Description("Confirms chucking of the substrate."
            + "Ends in the error - occurred state if chucking is possible."
            + "Origin search is performed only when the substrate is not placed on the Aligner.")]
        NoWaferPlaced = 1,

        [Description("Confirms chucking of the substrate. Origin search is performed in the chucked state if chucking is possible."
            + "Origin search is performed after turning off chucking if chucking is not possible.")]
        AlignInChuckedState = 2
    }
}
