using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class InputOutputDataType
    {
        public ResultType ResultType { get; set; }

        /// <summary>
        /// Indicates whether the data is available,
        /// if it is input data, that the previous step has finished producing it
        /// if it is output data, that the actor has finished producing it
        /// </summary>
        public bool Available { get; set; }
    }

    public class InputOutputValue
    {
        public InputOutputValue(InputOutputDataType inputOutputDataType)
        {
            InputOutputDataType = inputOutputDataType;
        }

        public InputOutputDataType InputOutputDataType { get; private set; }

        public string Value { get; set; }
        public Guid DAPToken { get; set; } = Guid.Empty;

        //[TODO] manage a list of DAPToken if the data arrives in dribs and drabs.

        public bool IsDataReady
        { get { return DAPToken != Guid.Empty; } }
    }
}
