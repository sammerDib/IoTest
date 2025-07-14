namespace UnitySC.Shared.Dataflow.Shared
{
    // TODO DATAFLOW : seul enum a être réellement utilisé par UTO et extern donc seul classe shared ==> à transfere dans services interfcae par exemple (RT).
    public enum PMStatus
    {
        NotStarted = 0,
        Available = 1,
        IsReserved = 2,
        Idle = 3,
        Error = 4,
        RecipeExecuting = 5,
    }
}
