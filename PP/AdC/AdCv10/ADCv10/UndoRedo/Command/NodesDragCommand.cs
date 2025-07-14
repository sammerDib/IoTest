using System;
using System.Collections.Generic;

using ADC.ViewModel;
using ADC.ViewModel.Graph;

using ADCEngine;

using GraphModel;

namespace ADC.UndoRedo.Command
{
    public class NodesDragCommand : IUndoableCommand
    {
        private struct NodePosInfo
        {
            public String name;
            public System.Windows.Rect nodeRect;
        }

        private List<NodePosInfo> listOriginNodesPos = new List<NodePosInfo>();
        private List<NodePosInfo> listRedoNodesPos = new List<NodePosInfo>();

        private RecipeGraphViewModel viewModel;

        public NodesDragCommand(RecipeGraphViewModel vModel)
        {
            viewModel = vModel;
        }

        public void Execute()
        {
            NodePosInfo nodePosInfo;

            // if root is selected, remove it frome memoGraph
            foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
            {
                if (!(node.Module is RootModule))
                {
                    nodePosInfo = GetNodePos(node);
                    listOriginNodesPos.Add(nodePosInfo);
                }
            }
        }

        public void Undo()
        {
            NodePosInfo nodePosInfo;
            NodeViewModel node;

            listRedoNodesPos.Clear();

            foreach (NodePosInfo nodeOriginPosInfo in listOriginNodesPos)
            {
                node = viewModel.GraphVM.FindNode(nodeOriginPosInfo.name);
                if (node != null)
                {
                    // Get actual node pos
                    nodePosInfo = GetNodePos(node);
                    listRedoNodesPos.Add(nodePosInfo);

                    // restore original pos
                    node.X = nodeOriginPosInfo.nodeRect.X;
                    node.Y = nodeOriginPosInfo.nodeRect.Y;
                }
            }
        }

        public void Redo()
        {
            NodeViewModel node;

            foreach (NodePosInfo nodePosInfo in listRedoNodesPos)
            {
                node = viewModel.GraphVM.FindNode(nodePosInfo.name);
                if (node != null)
                {
                    // restore last node pos
                    node.X = nodePosInfo.nodeRect.X;
                    node.Y = nodePosInfo.nodeRect.Y;
                }
            }
        }

        private NodePosInfo GetNodePos(NodeViewModel node)
        {
            NodePosInfo nodePosInfo = new NodePosInfo();
            nodePosInfo.name = node.Name;
            nodePosInfo.nodeRect = node.NodeRect();
            return nodePosInfo;
        }
    }
}
