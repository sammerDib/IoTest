using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.Graph;
using UnitySC.Shared.Tools.UndoRedo;
using UnitySC.Shared.UI.Graph.Model;

namespace UnitySC.PM.Shared.UI.Commands
{
    /// <summary>
    /// for insert after
    /// </summary>
    public class InsertChildCommand : IURUndoableCommand
    {
        private DataflowNodeViewModel _node;
        private DataflowNodeViewModel _memoNode;
        private DataflowNodeViewModel _parent;

        private readonly DataflowGraphViewModel _viewModel;

        public InsertChildCommand(DataflowGraphViewModel vModel)
        {
            _viewModel = vModel;
        }

        public override void Execute()
        {
            _parent = _viewModel.SelectedNode;
            _node = _viewModel.InsertChild();
            if (_node != null)
                _memoNode = (DataflowNodeViewModel)_node.Clone();
        }

        public override void Undo()
        {
            _viewModel.DeleteNode(ref _node);
        }

        public override void Redo()
        {
            if (_parent != null)
            {
                _parent = (DataflowNodeViewModel)_viewModel.GraphVM.FindNode(_parent.Name);
                _node = _viewModel.InsertChild(_parent, _memoNode);
            }
        }
    }

    /// <summary>
    /// for create new branch
    /// </summary>
    public class AddChildCommand : IURUndoableCommand
    {
        private DataflowNodeViewModel _node;
        private DataflowNodeViewModel _memoNode;
        private DataflowNodeViewModel _parent;

        private readonly DataflowGraphViewModel _viewModel;

        public AddChildCommand(DataflowGraphViewModel vModel)
        {
            _viewModel = vModel;
        }

        public override void Execute()
        {
            _parent = _viewModel.SelectedNode;
            List<DataflowNodeViewModel> nodes = _viewModel.CreateNewBranch();
            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    _memoNode = (DataflowNodeViewModel)nodes[0].Clone();
                }
            }
        }

        public override void Undo()
        {
            _viewModel.DeleteNode(ref _node);
        }

        public override void Redo()
        {
            if (_parent != null)
            {
                _parent = (DataflowNodeViewModel)_viewModel.GraphVM.FindNode(_parent.Name);
                _viewModel.AddChild(_parent, _memoNode);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public class DeleteNodesCommand : IURUndoableCommand
    {
        private DataflowGraphViewModel _viewModel;
        private GraphViewModel _memoGraph = new GraphViewModel();
        //private GraphViewModel graphForDisplay;

        public DeleteNodesCommand(DataflowGraphViewModel vModel)
        {
            _viewModel = vModel;
        }

        public override void Execute()
        {
            DataflowNodeViewModel memoNode;
            // Create a new Graph
            _memoGraph = new GraphViewModel();

            // Add a copy of selected nodes in new graph
            foreach (DataflowNodeViewModel node in _viewModel.GraphVM.GetselectedNodes())
            {
                // do not add the root node
                if (!(node == _viewModel.RootNode))
                {
                    memoNode = (DataflowNodeViewModel)node.Clone();
                    _memoGraph.Nodes.Add((NodeViewModel)memoNode);
                }
            }

            // Delete nodes in Viewmodel  graph
            _viewModel.DeleteSelectedNodes();
        }

        public override void Undo()
        {
            /* graphForDisplay = (GraphViewModel)memoGraph.Clone();
              ServiceRecipe.Instance().MergeGraph(graphForDisplay, false);
              viewModel.GraphVM.MergeGraph(graphForDisplay);

              // Have to connect node with former parents and childs ?
              foreach (DataflowNodeViewModel memoNode in memoGraph.Nodes)
              {
                  foreach (int idParent in memoNode.listeParentId)
                  {
                      DataflowNodeViewModel parent = viewModel.FindNode(idParent);
                      if (parent != null)
                      {
                          DataflowNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
                          if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node, parent) == false))
                          {
                              viewModel.GraphVM.ConnecteNode(parent, node);

                              ServiceRecipe.Instance().ConnectModules(parent.Module, node.Module);
                          }
                      }
                  }
                  foreach (int idChild in memoNode.listeChildId)
                  {
                      DataflowNodeViewModel child = viewModel.FindNode(idChild);
                      if (child != null)
                      {
                          DataflowNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
                          if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node, child) == false))
                          {
                              viewModel.GraphVM.ConnecteNode(node, child);

                              ServiceRecipe.Instance().ConnectModules(node.Module, child.Module);
                          }
                      }
                  }
              }*/
        }

        public override void Redo()
        {
            /*      if (graphForDisplay == null)
                      return;

                  // remove nodes from de graph adc
                  foreach (DataflowNodeViewModel node in graphForDisplay.Nodes)
                  {
                      viewModel.GraphVM.RemoveNode(node);
                      ServiceRecipe.Instance().RemoveModule(node.Module);
                  }
                  */
        }
    }

    public class NodesDragCommand : IURUndoableCommand
    {
        private struct NodePosInfo
        {
            public string Name;
            public System.Windows.Rect NodeRect;
        }

        private List<NodePosInfo> _listOriginNodesPos = new List<NodePosInfo>();
        private List<NodePosInfo> _listRedoNodesPos = new List<NodePosInfo>();

        private DataflowGraphViewModel _viewModel;

        public NodesDragCommand(DataflowGraphViewModel vModel)
        {
            _viewModel = vModel;
        }

        public override void Execute()
        {
            NodePosInfo nodePosInfo;

            // if root is selected, remove it frome memoGraph
            foreach (DataflowNodeViewModel node in _viewModel.GraphVM.GetselectedNodes())
            {
                if (!(node.ActorType != UnitySC.Shared.Data.Enum.ActorType.DataflowManager))
                {
                    nodePosInfo = GetNodePos(node);
                    _listOriginNodesPos.Add(nodePosInfo);
                }
            }
        }

        public override void Undo()
        {
            NodePosInfo nodePosInfo;
            NodeViewModel node;

            _listRedoNodesPos.Clear();

            foreach (NodePosInfo nodeOriginPosInfo in _listOriginNodesPos)
            {
                node = _viewModel.GraphVM.FindNode(nodeOriginPosInfo.Name);
                if (node != null)
                {
                    // Get actual node pos
                    nodePosInfo = GetNodePos(node);
                    _listRedoNodesPos.Add(nodePosInfo);

                    // restore original pos
                    node.X = nodeOriginPosInfo.NodeRect.X;
                    node.Y = nodeOriginPosInfo.NodeRect.Y;
                }
            }
        }

        public override void Redo()
        {
            NodeViewModel node;

            foreach (NodePosInfo nodePosInfo in _listRedoNodesPos)
            {
                node = _viewModel.GraphVM.FindNode(nodePosInfo.Name);
                if (node != null)
                {
                    // restore last node pos
                    node.X = nodePosInfo.NodeRect.X;
                    node.Y = nodePosInfo.NodeRect.Y;
                }
            }
        }

        private NodePosInfo GetNodePos(NodeViewModel node)
        {
            NodePosInfo nodePosInfo = new NodePosInfo();
            nodePosInfo.Name = node.Name;
            nodePosInfo.NodeRect = node.NodeRect();
            return nodePosInfo;
        }
    }
}
