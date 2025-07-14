namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    public class MatrixRange
    {
        public string Name
        {
            get
            {
                if (float.IsNegativeInfinity(Min))
                {
                    return $"Below {Max:F4}";
                }

                if (float.IsPositiveInfinity(Max))
                {
                    return $"Above {Min:F4}";
                }

                return $"{Min:F4} ... {Max:F4}";
            }
        }

        public float Min { get; set; }

        public float Max { get; set; }

        public float Area => Count / (float)TotalCount * 100;

        public int Count { get; set; }

        public int TotalCount { get; set; }
    }
}
