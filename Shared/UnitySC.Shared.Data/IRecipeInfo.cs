using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Data
{
    public interface IRecipeInfo
    {
        ActorType ActorType { get; set; }
        string Comment { get; set; }
        DateTime Created { get; set; }
        bool IsShared { get; set; }
        bool IsTemplate { get; set; }
        Guid Key { get; set; }
        string Name { get; set; }
        int? StepId { get; set; }
        int? UserId { get; set; }
        int? CreatorChamberId { get; set; }
        int Version { get; set; }
    }
}