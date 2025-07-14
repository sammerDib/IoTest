namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class WaferIdStatus
    {
        public WaferIdStatus(string waferIdFrontSide, string waferIdBackSide = "")
        {
            WaferIdFrontSide = waferIdFrontSide;
            WaferIdBackSide  = waferIdBackSide;
        }

        public string WaferIdFrontSide { get; }

        public string WaferIdBackSide { get; }
    }
}
