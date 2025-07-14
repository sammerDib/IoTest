namespace UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.PeriodicStruct.StructDetails
{
    public readonly struct PeriodicStructListStructure
    {
        public PeriodicStructListStructure(int id, int repetaId, string height, string width)
        {
            Id = id;
            RepetaId = repetaId;
            Height = height;
            Width = width;
        }

        public int Id { get; }

        public int RepetaId { get; }

        public string Height { get; }

        public string Width { get; }

        public override string ToString() => $"{Id} ({Height}, {Width})";
    }
}
