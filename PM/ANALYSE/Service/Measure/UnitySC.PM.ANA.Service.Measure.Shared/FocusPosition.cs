using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Measure.AutofocusTracker
{
    public class FocusPosition
    {
        /// <summary>
        /// XYZTopZBottom Focus position adjusted for main objective (5X NIR)
        /// </summary>
        public XYZTopZBottomPosition Position;

        public FocusPosition(XYZTopZBottomPosition position)
        {
            Position = new XYZTopZBottomPosition(position.Referential, position.X, position.Y, position.ZTop, position.ZBottom);
        }
    }
}
