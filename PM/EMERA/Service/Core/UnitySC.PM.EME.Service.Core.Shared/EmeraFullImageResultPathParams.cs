using System;

using UnitySC.Shared.Data.Composer;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public class EmeraFullImageResultPathParams : IParamComposerObject
    {
        public const string FmtDatetime = "yyyyMMdd_HHmmss";
        public static readonly EmeraFullImageResultPathParams Empty = new EmeraFullImageResultPathParams();
        public string[] ToParamLabels()
            => new[] {
                nameof(ActorType),
                nameof(Tool),
                nameof(Product),
                nameof(Step),
                nameof(Recipe),
                nameof(WaferCategory),
                nameof(Filter),
                nameof(Light),
                 nameof(StartProcessDate)
            };

        public object[] ToParamObjects()
        {
            return new object[]
            {
                ActorType,
                Tool,
                Product,
                Step,
                Recipe,
                WaferCategory,
                Filter,
                Light,
                StartProcessDate.ToString(FmtDatetime)
            };
        }

        public string ActorType { get; set; } = string.Empty;
        public string Tool { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public string Step { get; set; } = string.Empty;
        public string Recipe { get; set; } = string.Empty;
        public string WaferCategory { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
        public string Light { get; set; } = string.Empty;
        public DateTime StartProcessDate { get; set; } = DateTime.MinValue;
    }
}
