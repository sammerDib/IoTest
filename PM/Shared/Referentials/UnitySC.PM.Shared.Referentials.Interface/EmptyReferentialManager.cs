namespace UnitySC.PM.Shared.Referentials.Interface
{
    public class EmptyReferentialManager : IReferentialManager
    {
        public PositionBase ConvertToAxes(PositionBase position)
        {
            return position;
        }
    }
}
