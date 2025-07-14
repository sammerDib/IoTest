namespace UnitySC.DataAccess.Dto.ModelDto.Enum
{
    public enum DataflowRecipeComponentState
    {
        Unknow = 0,
        Idle = 1,
        Ready = 2,
        WaitingForAvailableActor = 3,
        WaintingForInput = 4,
        Success = 5,
        InError = 6,
    }
}
