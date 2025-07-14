using System;

using UnitySC.Shared.Data.Composer;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    public class EmeraVignetteResultPathParams : IParamComposerObject
    {
        public const string FmtDatetime = "yyyyMMdd_HHmmss";
        public static readonly EmeraVignetteResultPathParams Empty = new EmeraVignetteResultPathParams();
        public string[] ToParamLabels()
            => new [] {
                nameof(ActorType),
                nameof(Tool),
                nameof(Product),
                nameof(Step),
                nameof(Recipe),
                nameof(WaferCategory),
                nameof(Filter),
                nameof(Light),
                nameof(Column),
                nameof(Line),
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
                Column,
                Line,
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
        public int Column { get; set; }
        public int Line { get; set; }
        public DateTime StartProcessDate { get; set; } = DateTime.MinValue;
    }
}
