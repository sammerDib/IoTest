using System;

namespace UnitySC.DataAccess.Dto.ModelDto.Enum
{
    // ResultState error "enum code" should be <= -2
    // cf result viewver

    public enum ResultState
    {
        /// <summary>
        /// Database Error
        /// </summary>
        DatabaseError = -8,
        /// <summary>
        /// like for [ANA] Mark alignement error 
        /// </summary>
        WaferAlignmentError = -7, 
        /// <summary>
        /// like for [ANA] Bare wafer alignment "BWA" error (i.e Notch + edges)
        /// </summary>
        AlignmentError = -6,
        /// <summary>
        /// error during preparation prior to execution process
        /// </summary>
        PreparationError = -5,
        /// <summary>
        /// Error due to user Abort or Cancel  (hence Result is not finalized)
        /// </summary>
        AbortError = -4,
        /// <summary>
        /// Error during Material Process (i.e Load/unload, Clamp, Vaccum, presence sensor, Arm in chamber...) 
        /// </summary>
        ProcessError = -3,
        /// <summary>
        ///  Error (Standard no explanation needed)
        /// </summary>
        Error = -2,
        /// <summary>
        /// Waiting Result to be processed 
        /// </summary>
        NotProcess = -1,
        /// <summary>
        /// Result processed successfully
        /// </summary>
        Ok = 0,
        /// <summary>
        /// Partial results / [ADC] Truncated results & result sorting Sanction , some defect not killer
        /// </summary>
        Partial,
        /// <summary>
        /// [ADC] result sorting Sanction , some numerous defects need some addtionnal wafer cleaning steps
        /// </summary>
        Rework,
        /// <summary>
        /// [ADC] result sorting Sanction , Killer defects - wafer to trash
        /// </summary>
        Reject
    }

    public static class EnumExtensions
    {
        public static string ToHumanizedString(this ResultState state)
        {
            return ToHumanizedString(state as ResultState?);
        }

        public static string ToHumanizedString(this ResultState? state)
        {
            if (! state.HasValue) 
                return "-";
            switch (state.Value)
            {
                case ResultState.Error:
                    return "Error";
                case ResultState.NotProcess:
                    return "Not Process";
                case ResultState.Ok:
                    return "Ok";
                case ResultState.Partial:
                    return "Partial";
                case ResultState.Rework:
                    return "Rework";
                case ResultState.Reject:
                    return "Reject";

                case ResultState.WaferAlignmentError:
                    return "Wafer Alignment Error";
                case ResultState.AlignmentError:
                    return "Alignement Error";
                case ResultState.PreparationError:
                    return "Preparation Error";
                case ResultState.AbortError:
                    return "Abort";
                case ResultState.ProcessError:
                    return "Process Error";
                case ResultState.DatabaseError:
                    return "Database Error";

                default:
                    throw new ArgumentOutOfRangeException($"Not Humanized ResultState = {(int)state.Value}");
            }
        }
    }
}
