/*!
------------------------------------------------------------------------------
Unity-sc Technical Software Department
------------------------------------------------------------------------------
Copyright (c) 2017, Unity-sc.
611, rue Aristide Bergès  Z.A. de Pré Millet 38320 Montbonnot-Saint-Martin (France)
All rights reserved.
This source program is the property of Unity-sc Company and may not be copied
in any form or by any means, whether in part or in whole, except under license
expressly granted by Unity-sc company
All copies of this program, whether in part or in whole, and
whether modified or not, must display this and all other
embedded copyright and ownership notices in full.
------------------------------------------------------------------------------
Project : ADCv10
Module : UndoRedoManager 
@file
@brief 
 This module is the base classes for Undo Redo. 
 Design pattern Command
 
@date 
@remarks
@todo
------------------------------------------------------------------------------
*/

using System.Collections;

namespace ADCv10.UndoRedo
{
    public class UndoRedoManager
    {

        private Stack _UndoStack = new Stack();
        private Stack _RedoStack = new Stack();

        public UndoRedoManager() { }

        public void ExecuteCmd(ICommand command)
        {
            command.Execute();
            if (command is IUndoableCommand)
            {
                _RedoStack.Clear(); // clear the redo stack
                _UndoStack.Push(command);
            }
        }

        public void Undo()
        {
            if (_UndoStack.Count <= 0)
            {
                return;
            }
            ((IUndoableCommand)(_UndoStack.Peek())).Undo();          // undo most recently executed command
            _RedoStack.Push(_UndoStack.Pop()); // add undone command to redo stack and remove top entry from undo stack
        }

        public void Redo()
        {
            if (_RedoStack.Count <= 0)
            {
                return;
            }
            ((IUndoableCommand)(_RedoStack.Peek())).Redo();          // redo most recently executed command
            _UndoStack.Push(_RedoStack.Pop()); // add undone command to redo stack and remove top entry from redo stack
        }

        public void Clear()
        {
            _UndoStack.Clear();
            _RedoStack.Clear();
        }

    }

    public abstract class ICommand
    {
        public abstract void Execute();
    }


    public abstract class IUndoableCommand : ICommand
    {
        public abstract void Undo();
        public abstract void Redo();
    }


}
