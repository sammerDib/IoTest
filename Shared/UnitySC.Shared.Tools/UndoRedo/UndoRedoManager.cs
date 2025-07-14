using System.Collections;

namespace UnitySC.Shared.Tools.UndoRedo
{
    public class UndoRedoManager
    {
        private readonly Stack _undoStack = new Stack();
        private readonly Stack _redoStack = new Stack();

        public UndoRedoManager()
        {
        }

        public int UndoCount { get => _undoStack.Count; }
        public int RedoCount { get => _redoStack.Count; }

        public void ExecuteCmd(IURCommand command)
        {
            command.Execute();
            if (command is IURUndoableCommand)
            {
                _redoStack.Clear(); // clear the redo stack
                _undoStack.Push(command);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count <= 0)
            {
                return;
            }
            ((IURUndoableCommand)_undoStack.Peek()).Undo();          // undo most recently executed command
            _redoStack.Push(_undoStack.Pop()); // add undone command to redo stack and remove top entry from undo stack
        }

        public void Redo()
        {
            if (_redoStack.Count <= 0)
            {
                return;
            }
            ((IURUndoableCommand)_redoStack.Peek()).Redo();          // redo most recently executed command
            _undoStack.Push(_redoStack.Pop()); // add undone command to redo stack and remove top entry from redo stack
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}