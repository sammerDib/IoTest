namespace UnitySC.Shared.ResultUI.Common.Message
{
    public enum DefectActionEnum
    {
        ChangeCurrentDefect,
        NavigateToDefect,
        SearchDefectByClusterNumber,
        SelectAndDisplayFirstDefect
    }

    public class DefectActionMessage
    {
        public int Id { get; set; }
        public DefectActionEnum Action { get; set; }

        public DefectActionMessage(DefectActionEnum actionType)
        {
            Action = actionType;
        }

        public DefectActionMessage(DefectActionEnum actionType, int id)
        {
            Action = actionType;
            Id = id;
        }
    }
}
