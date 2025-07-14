using System;

using ADC.ViewModel;
using ADC.ViewModel.Graph;

namespace ADC.UndoRedo.Command
{
    public class InsertChildCommand : IUndoableCommand
    {
        private ModuleNodeViewModel node;
        private ModuleNodeViewModel parent;
        private String factoryName;

        private RecipeGraphViewModel viewModel;

        public InsertChildCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        public void Execute()
        {
            parent = viewModel.SelectedNode;
            node = viewModel.InsertChild();
            if (node != null)
                factoryName = node.Module.Factory.ModuleName;
        }

        public void Undo()
        {
            viewModel.DeleteNode(ref node);
        }

        public void Redo()
        {
            if (parent != null)
            {
                parent = (ModuleNodeViewModel)viewModel.GraphVM.FindNode(parent.Name);
                node = viewModel.InsertChild(parent, factoryName);
            }
        }
    }
}
