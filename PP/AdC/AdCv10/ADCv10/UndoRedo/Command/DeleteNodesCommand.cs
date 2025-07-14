using ADC.Model;
using ADC.ViewModel;
using ADC.ViewModel.Graph;

using ADCEngine;

using GraphModel;

namespace ADC.UndoRedo.Command
{
    public class DeleteNodesCommand : IUndoableCommand
    {
        private RecipeGraphViewModel viewModel;
        private GraphViewModel memoGraph = new GraphViewModel();
        private GraphViewModel graphForDisplay;

        public DeleteNodesCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        public void Execute()
        {
            ModuleNodeViewModel memoNode;
            // Create a new Graph 
            memoGraph = new GraphViewModel();

            // Add a copy of selected nodes in new graph
            foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
            {
                // do not add the root node
                if (!(node.Module is RootModule))
                {
                    memoNode = (ModuleNodeViewModel)node.Clone();
                    memoGraph.Nodes.Add((NodeViewModel)memoNode);

                    // memo childs Id
                    foreach (ModuleNodeViewModel child in viewModel.GraphVM.GetChilds(node))
                    {
                        memoNode.MemoChild(child);
                    }
                    // memo parents Id
                    foreach (ModuleNodeViewModel parent in viewModel.GraphVM.GetParents(node))
                    {
                        memoNode.MemoParent(parent);
                    }
                }
            }

            // Delete nodes in Viewmodel  graph
            viewModel.DeleteSelectedNodes();
        }

        public void Undo()
        {
            graphForDisplay = (GraphViewModel)memoGraph.Clone();
            ServiceRecipe.Instance().MergeGraph(graphForDisplay, false);
            viewModel.GraphVM.MergeGraph(graphForDisplay);


            // Have to connect node with former parents and childs ?
            foreach (ModuleNodeViewModel memoNode in memoGraph.Nodes)
            {
                foreach (int idParent in memoNode.listeParentId)
                {
                    ModuleNodeViewModel parent = viewModel.FindNode(idParent);
                    if (parent != null)
                    {
                        ModuleNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
                        if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node, parent) == false))
                        {
                            viewModel.GraphVM.ConnecteNode(parent, node);

                            ServiceRecipe.Instance().ConnectModules(parent.Module, node.Module);
                        }
                    }
                }
                foreach (int idChild in memoNode.listeChildId)
                {
                    ModuleNodeViewModel child = viewModel.FindNode(idChild);
                    if (child != null)
                    {
                        ModuleNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
                        if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node, child) == false))
                        {
                            viewModel.GraphVM.ConnecteNode(node, child);

                            ServiceRecipe.Instance().ConnectModules(node.Module, child.Module);
                        }
                    }
                }
            }
        }

        public void Redo()
        {
            if (graphForDisplay == null)
                return;

            // remove nodes from de graph adc
            foreach (ModuleNodeViewModel node in graphForDisplay.Nodes)
            {
                viewModel.GraphVM.RemoveNode(node);
                ServiceRecipe.Instance().RemoveModule(node.Module);
            }
        }
    }
}
