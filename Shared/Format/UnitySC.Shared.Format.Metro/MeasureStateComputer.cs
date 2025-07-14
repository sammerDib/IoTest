using System;

using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro
{
    public static class MeasureStateComputer
    {
        public static MeasureState GetMeasureState(Length measure, Length minAllowed, Length maxAllowed)
        {
            if (measure == null) return MeasureState.NotMeasured;
            if (double.IsNaN(measure.Value) || double.IsInfinity(measure.Value)) return MeasureState.NotMeasured;
            return measure >= minAllowed && measure <= maxAllowed ? MeasureState.Success : MeasureState.Error;
        }

        public static MeasureState GetMeasureState(Length measure, LengthTolerance tolerance, Length target)
        {
            if (measure == null) return MeasureState.NotMeasured;
            if (double.IsNaN(measure.Value) || double.IsInfinity(measure.Value)) return MeasureState.NotMeasured;
            return tolerance.IsInTolerance(measure, target) ? MeasureState.Success : MeasureState.Error;
        }

        public static MeasureState GetMeasureState(double value, Tolerance tolerance, double target)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) return MeasureState.NotMeasured;
            return tolerance.IsInTolerance(value, target) ? MeasureState.Success : MeasureState.Error;
        }

        public static MeasureState GetMeasureState_NoLimit(Length measure)
        {
            if (measure == null) return MeasureState.NotMeasured;
            if (double.IsNaN(measure.Value) || double.IsInfinity(measure.Value)) return MeasureState.NotMeasured;
            return MeasureState.Success;
        }

        /// <summary>
        /// Combines measurements states that are used by a unique measure on a unique point into
        /// a single measure state.
        ///
        /// <para>
        /// Some measures may perform several internal measurements but return one state (for example, the TSV has
        /// 3 internal measures: depth width and length). This function combines the states of these measurements
        /// into one MeasureState.
        /// </para>
        /// </summary>
        public static MeasureState CombineInternalMeasurementsStates(params MeasureState[] measureStates)
        {
            bool hasError = false;
            foreach (var measureState in measureStates)
            {
                switch (measureState)
                {
                    // If one measurement has not been performed, state is NotMeasured
                    case MeasureState.NotMeasured:
                        return MeasureState.NotMeasured;

                    case MeasureState.Error:
                        hasError = true; break;

                    case MeasureState.Partial:
                        throw new ArgumentException("Unexpected measure state partial from internal measurement.");
                }
            }
            return hasError ? MeasureState.Error : MeasureState.Success;
        }
    }
}
