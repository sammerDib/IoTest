using ADC.Model;
using ADC.ViewModel;
using ADC.ViewModel.Graph;

using GraphModel;

namespace ADC.UndoRedo.Command
{
    public class PasteNodesCommand : IUndoableCommand
    {
        private RecipeGraphViewModel viewModel;
        private GraphViewModel memoGraph = new GraphViewModel();
        private GraphViewModel graphForDisplay;

        public PasteNodesCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        /// <summary>
        /// Execute the paste command. Make a copy of clipboard, builds the list of branches.
        /// Add branches in the graph
        /// </summary>
        public void Execute()
        {
            AdcClipBoard clipBoard = AdcClipBoard.Instance();

            if (clipBoard.Graph.Nodes.Count > 0)
            {
                graphForDisplay = (GraphViewModel)clipBoard.Graph.Clone();
                // Move the new nodes so that they do not recover originals
                foreach (NodeViewModel node in graphForDisplay.Nodes)
                {
                    node.X += 50;
                }

                viewModel.GraphVM.MergeGraph(graphForDisplay);
                ServiceRecipe.Instance().MergeGraph(graphForDisplay);
            }
        }

        public void Undo()
        {
            if (graphForDisplay == null)
                return;

            // remove nodes from de graph adc
            foreach (ModuleNodeViewModel node in graphForDisplay.Nodes)
            {
                viewModel.GraphVM.RemoveNode(node);
                ServiceRecipe.Instance().RemoveModule(node.Module);
            }

            memoGraph = (GraphViewModel)graphForDisplay.Clone();
        }

        public void Redo()
        {
            graphForDisplay = (GraphViewModel)memoGraph.Clone();
            viewModel.GraphVM.MergeGraph(graphForDisplay);
            ServiceRecipe.Instance().MergeGraph(graphForDisplay);
        }
    }
}
