namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class StatusEventArgs<T> : System.EventArgs
    {
        public StatusEventArgs(T status)
        {
            Status = status;
        }

        public T Status { get; }
    }
}
