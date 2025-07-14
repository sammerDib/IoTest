using ADC.ViewModel;
using ADC.ViewModel.Graph;

using GraphModel;

namespace ADC.UndoRedo.Command
{
    public class CopyNodesCommand : ICommand
    {
        private RecipeGraphViewModel viewModel;
        private AdcClipBoard clipBoard = AdcClipBoard.Instance();


        public CopyNodesCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        public void Execute()
        {
            GraphViewModel memoGraph = new GraphViewModel();

            // Add a copy of selected nodes in new graph
            foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
            {
                memoGraph.Nodes.Add((NodeViewModel)node.Clone());
            }

            foreach (ModuleNodeViewModel memoNode in memoGraph.Nodes)
            {
                ModuleNodeViewModel SourceNode = (ModuleNodeViewModel)viewModel.GraphVM.FindNode(memoNode.Name);
                if (SourceNode != null)
                {
                    foreach (ModuleNodeViewModel child in SourceNode.GetOutNodesConnectedList())
                    {
                        ModuleNodeViewModel memoChild = (ModuleNodeViewModel)memoGraph.FindNode(child.Name);
                        if (memoChild != null)
                        {
                            memoGraph.ConnecteNode(memoNode, memoChild);
                        }

                    }
                }
            }
            // Clear the clipBoard
            clipBoard.Clear();
            // Copy selected nodes in the clipboard
            clipBoard.Graph = memoGraph;
        }

    }
}
