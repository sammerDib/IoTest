using System.Collections.Generic;

using ADC.ViewModel;
using ADC.ViewModel.Graph;

namespace ADC.UndoRedo.Command
{
    public class AddChildCommand : IUndoableCommand
    {
        private ModuleNodeViewModel node;
        private ModuleNodeViewModel memoNode;
        private ModuleNodeViewModel parent;

        private RecipeGraphViewModel viewModel;

        public AddChildCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        public void Execute()
        {

            parent = viewModel.SelectedNode;
            List<ModuleNodeViewModel> nodes = viewModel.CreateNewBranch();
            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    memoNode = (ModuleNodeViewModel)nodes[0].Clone();
                }
            }
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
                viewModel.AddChild(parent, memoNode);
            }
        }
    }
}
