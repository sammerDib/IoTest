using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.EME.Service.Interface.Recipe.Execution
{
    [DataContract]
    public class ExecutionSettings : IEquatable<ExecutionSettings>
    {
        [DataMember]
        public bool RunAutoFocus { get; set; } = true;

        [DataMember]
        public bool RunAutoExposure { get; set; }

        [DataMember]
        public bool RunBwa { get; set; }

        [DataMember]
        public bool RunStitchFullImage { get; set; } = true;

        [DataMember]
        public AcquisitionStrategy Strategy { get; set; } = AcquisitionStrategy.Serpentine;

        [DataMember]
        public bool ReduceResolution { get; set; } = true;

        [DataMember]
        public bool ConvertTo8Bits { get; set; } = true;
        
        [DataMember]
        public bool CorrectDistortion { get; set; } = true;

        [DataMember]
        public bool NormalizePixelValue { get; set; } = true;

        public bool Equals(ExecutionSettings other)
        {
            if (other is null)
            {
                return false;
            }

            return other.RunAutoFocus == RunAutoFocus &&
                   other.RunAutoExposure == RunAutoExposure &&
                   other.RunBwa == RunBwa &&
                   other.Strategy == Strategy &&
                   other.ReduceResolution == ReduceResolution &&
                   other.ConvertTo8Bits == ConvertTo8Bits &&
                   other.CorrectDistortion == CorrectDistortion &&
                   other.NormalizePixelValue == NormalizePixelValue &&
                   other.RunStitchFullImage == RunStitchFullImage;
        }

        public static bool operator ==(ExecutionSettings lSettings, ExecutionSettings rSettings)
        {
            if (lSettings is null)
            {
                return rSettings is null;
            }

            return lSettings.Equals(rSettings);
        }

        public static bool operator !=(ExecutionSettings lSettings, ExecutionSettings rSettings)
        {
            if (lSettings is null)
            {
                return !(rSettings is null);
            }
            return !lSettings.Equals(rSettings);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ExecutionSettings);
        }
        public override int GetHashCode() => (RunAutoFocus, RunAutoExposure, RunBwa, Strategy, ReduceResolution, ConvertTo8Bits, CorrectDistortion, NormalizePixelValue, RunStitchFullImage).GetHashCode();
    }
}
