namespace UnitySC.PM.Shared.Referentials.Interface
{
    public interface IReferentialConverter<T> where T : PositionBase
    {
        T Convert(T positionBase);

        bool Accept(ReferentialTag from, ReferentialTag to);

        bool IsEnabled { get; set; }
    }
}
