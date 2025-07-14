namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.DragDrop
{
    public enum DragDropAction
    {
        ChangeOrder,
        ChangParent,
        ChangeOrderAndParent,
        None
    }

    public static class DragDropActionExtension
    {
        public static bool ChangeParent(this DragDropAction action)
        {
            return action == DragDropAction.ChangParent || action == DragDropAction.ChangeOrderAndParent;
        }

        public static bool ChangeOrder(this DragDropAction action)
        {
            return action == DragDropAction.ChangeOrder || action == DragDropAction.ChangeOrderAndParent;
        }
    }
}