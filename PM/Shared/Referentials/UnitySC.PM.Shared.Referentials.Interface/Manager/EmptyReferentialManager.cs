using System;

namespace UnitySC.PM.Shared.Referentials.Interface
{
    public class EmptyReferentialManager : IReferentialManager
    {
        public PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            switch (referentialTo)
            {
                case ReferentialTag.Motor:
                    positionToConvert.Referential = new MotorReferential();
                    break;
                case ReferentialTag.Stage:
                    positionToConvert.Referential = new StageReferential();
                    break;
                case ReferentialTag.Wafer:
                    positionToConvert.Referential = new WaferReferential();
                    break;
                case ReferentialTag.Die:
                    positionToConvert.Referential = new DieReferential();
                    break;
                default:
                    throw new ArgumentException($"Can't convert to referential tag {referentialTo}");
            }
            return positionToConvert;
        }

        public void DeleteSettings(ReferentialTag referentialTag)
        {
            // Nothing
        }

        public void DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            // Nothing
        }

        public void EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2)
        {
            // Nothing
        }

        public ReferentialSettingsBase GetSettings(ReferentialTag referentialTag)
        {
            // Nothing
            return null;
        }

        public void SetSettings(ReferentialSettingsBase settings)
        {
            // Nothing
        }
    }
}
