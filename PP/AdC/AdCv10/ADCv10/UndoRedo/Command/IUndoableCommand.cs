namespace ADC.UndoRedo.Command
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
        void Redo();
    }
}
