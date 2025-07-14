using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AlignmentLiseHFSpectrumCharacInput : IANAInputFlow
    {
        public int PixelNumber { get; set; }
        public List<double> Signal { get; set; }
        public List<double> Dark { get; set; }
        public List<double> Ref { get; set; }
        public int CharNumber { get; set; }
        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);
            if (Signal is null || Dark is null || Ref is null || Signal.Count != Dark.Count ||
                Signal.Count != Ref.Count)
            {
                result.IsValid = false;
                result.Message.Add($"Signal, Dark and Ref must be the same length.");
                return result;
            }
            return result;
        }

        public ANAContextBase InitialContext { get; set; }
    }
}
