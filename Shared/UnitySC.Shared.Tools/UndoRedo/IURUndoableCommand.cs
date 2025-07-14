namespace UnitySC.Shared.Tools.UndoRedo
{
    public abstract class IURUndoableCommand : IURCommand
    {
        public abstract void Undo();

        public abstract void Redo();
    }
}