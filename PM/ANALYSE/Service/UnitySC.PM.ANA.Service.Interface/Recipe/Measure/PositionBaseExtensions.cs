using System;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    public static class PositionBaseExtensions
    {
        /// <summary>
        /// Returns the position with its referential replaced by the given.
        /// No conversion/translation/etc... is applied.
        /// It is mostly useful for DieReferencial, to switch the position from one die to another.
        /// </summary>
        /// <exception cref="ArgumentException">When the referential is not compatible with the position's referential.</exception>
        public static PositionBase PositionInReferential(this PositionBase position, ReferentialBase referential)
        {
            if (position.Referential.Tag != referential.Tag)
                throw new ArgumentException($"the referential <{referential.Tag}> is not compatible with the position's referential <{position.Referential.Tag}>");

            // No need to convert, we want to keep the position "values" and just manually change the referential:
            PositionBase positionInReferential = (PositionBase)position.Clone();
            //  - If `position` was in a wafer referential, `referential` is supposed to still be a wafer referential.
            //  - If `position` was in a die referential, `referential` is supposed to still be a die referential, potentially on another die.
            positionInReferential.Referential = referential;

            return positionInReferential;
        }
    }
}
