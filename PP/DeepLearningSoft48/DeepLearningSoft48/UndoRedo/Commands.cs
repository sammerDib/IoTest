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
 Commnands for undo redo
 Design pattern Command
 
@date 
@remarks
@todo
------------------------------------------------------------------------------
*/

using System;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

namespace ADCv10.UndoRedo
{

    /// <summary>
    /// 
    /// </summary>
    public class InsertChildCommand : IUndoableCommand
    {
        //rivate DefectAnnotationVM node;
        //private DefectAnnotationVM parent;
        //private String factoryName;

        //private RecipeGraphViewModel viewModel;

        //public InsertChildCommand(RecipeGraphViewModel vModel)
        //{
        //	viewModel = vModel;
        //}

        public override void Execute()
        {
            //parent = viewModel.SelectedNode;
            //node = viewModel.InsertChild();
            ////if (node != null)
            //	factoryName = node.Module.Factory.ModuleName;
        }

        public override void Undo()
        {
            //viewModel.DeleteNode(ref node);
        }

        public override void Redo()
        {
            //if (parent != null)
            //{
                //parent = (DefectAnnotationVM)viewModel.GraphVM.FindNode(parent.Name);
                //node = viewModel.InsertChild(parent, factoryName);
            //}
        }
    }



    //public class AddChildCommand : IUndoableCommand
    //{
    //	private ModuleNodeViewModel node;
    //	private ModuleNodeViewModel memoNode;
    //	private ModuleNodeViewModel parent;

    //	private RecipeGraphViewModel viewModel;

    //	public AddChildCommand(RecipeGraphViewModel vModel)
    //	{
    //		viewModel = vModel;
    //	}

    //	public override void Execute()
    //	{

    //		parent = viewModel.SelectedNode;
    //		List<ModuleNodeViewModel> nodes = viewModel.CreateNewBranch();
    //		if (nodes != null)
    //		{
    //			if (nodes.Count > 0)
    //			{
    //				memoNode = (ModuleNodeViewModel)nodes[0].Clone();
    //			}
    //		}
    //	}

    //	public override void Undo()
    //	{
    //		viewModel.DeleteNode(ref node);
    //	}

    //	public override void Redo()
    //	{
    //		if (parent != null)
    //		{

    //			parent = (ModuleNodeViewModel)viewModel.GraphVM.FindNode(parent.Name);
    //               viewModel.AddChild(parent, memoNode);
    //		}
    //	}
    //}



    /// <summary>
    /// 
    /// </summary>
    //public class DeleteNodesCommand : IUndoableCommand
    //{
    //       private RecipeGraphViewModel viewModel;
    //       private GraphViewModel memoGraph = new GraphViewModel();
    //       private GraphViewModel graphForDisplay;

    //       public DeleteNodesCommand(RecipeGraphViewModel vModel)
    //	{
    //		viewModel = vModel;
    //	}

    //	public override void Execute()
    //	{
    //		ModuleNodeViewModel memoNode;
    //		// Create a new Graph 
    //		memoGraph = new GraphViewModel();

    //		// Add a copy of selected nodes in new graph
    //		foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
    //		{
    //			// do not add the root node
    //			if (!(node.Module is RootModule))
    //			{
    //				memoNode = (ModuleNodeViewModel)node.Clone();
    //				memoGraph.Nodes.Add((NodeViewModel) memoNode);

    //				// memo childs Id
    //				foreach (ModuleNodeViewModel child in viewModel.GraphVM.GetChilds(node))
    //				{
    //					//if (child.IsSelected == false)
    //					{
    //						memoNode.MemoChild(child);
    //					}
    //				}
    //				// memo parents Id
    //				foreach (ModuleNodeViewModel parent in viewModel.GraphVM.GetParents(node))
    //				{
    //					//if (parent.IsSelected == false)
    //					{
    //						memoNode.MemoParent(parent);
    //					}
    //				}
    //			}
    //		}

    //		//memoGraph.UpdateConnections();

    //		// if root is selected, remove it from memoGraph
    //		/*foreach (NodeViewModel node in memoGraph.Nodes)
    //		{
    //			if (ServiceRecipe.Instance().IsRoot((ServiceModule)node.Data))
    //			{
    //				memoGraph.RemoveNode(node);
    //				break;
    //			}
    //		}*/

    //		// Delete nodes in Viewmodel  graph
    //		viewModel.DeleteSelectedNodes();
    //	}

    //	public override void Undo()
    //	{
    //		graphForDisplay = (GraphViewModel)memoGraph.Clone();
    //		ServiceRecipe.Instance().MergeGraph(graphForDisplay,false);
    //		viewModel.GraphVM.MergeGraph(graphForDisplay);


    //		// Have to connect node with former parents and childs ?
    //		foreach (ModuleNodeViewModel memoNode in memoGraph.Nodes)
    //		{
    //			foreach (int idParent in memoNode.listeParentId)
    //			{
    //				ModuleNodeViewModel parent = viewModel.FindNode(idParent);
    //				if (parent != null)
    //				{
    //					ModuleNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
    //					if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node,parent) == false))
    //					{
    //						viewModel.GraphVM.ConnecteNode(parent, node);

    //						ServiceRecipe.Instance().ConnectModules(parent.Module, node.Module);
    //					}
    //				}
    //			}
    //			foreach (int idChild in memoNode.listeChildId)
    //			{
    //				ModuleNodeViewModel child = viewModel.FindNode(idChild);
    //				if (child != null)
    //				{
    //					ModuleNodeViewModel node = viewModel.FindNode(memoNode.Module.Id);
    //					if ((node != null) && (viewModel.GraphVM.IsNodesConnected(node, child) == false))
    //					{
    //						viewModel.GraphVM.ConnecteNode(node, child);

    //						ServiceRecipe.Instance().ConnectModules(node.Module, child.Module);
    //					}
    //				}
    //			}
    //		}
    //	}

    //       public override void Redo()
    //       {
    //           if (graphForDisplay == null)
    //               return;

    //           // remove nodes from de graph adc
    //           foreach (ModuleNodeViewModel node in graphForDisplay.Nodes)
    //           {
    //               viewModel.GraphVM.RemoveNode(node);
    //               ServiceRecipe.Instance().RemoveModule(node.Module);
    //           }
    //       }
    //}


    /// <summary>
    /// 
    /// </summary>
    //public class CopyNodesCommand : ICommand
    //{
    //	private RecipeGraphViewModel viewModel;
    //	private AdcClipBoard clipBoard = AdcClipBoard.Instance();


    //	public CopyNodesCommand(RecipeGraphViewModel vModel)
    //	{
    //		viewModel = vModel;
    //	}

    //	public override void Execute()
    //	{
    //		GraphViewModel memoGraph = new GraphViewModel();

    //		// Add a copy of selected nodes in new graph
    //		foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
    //		{
    //			memoGraph.Nodes.Add((NodeViewModel)node.Clone());
    //		}

    //		foreach (ModuleNodeViewModel memoNode in memoGraph.Nodes)
    //		{
    //			ModuleNodeViewModel SourceNode = (ModuleNodeViewModel)viewModel.GraphVM.FindNode(memoNode.Name);
    //			if (SourceNode != null)
    //			{
    //				foreach (ModuleNodeViewModel child in SourceNode.GetOutNodesConnectedList())
    //				{
    //					ModuleNodeViewModel memoChild = (ModuleNodeViewModel) memoGraph.FindNode(child.Name);
    //					if (memoChild != null)
    //					{
    //						memoGraph.ConnecteNode(memoNode, memoChild);
    //					}

    //				}
    //			}
    //		}
    //		// Clear the clipBoard
    //		clipBoard.Clear();
    //		// Copy selected nodes in the clipboard
    //		clipBoard.Graph = memoGraph;
    //	}

    //}


    /// <summary>
    /// 
    /// </summary>
    //public class PastNodesCommand : IUndoableCommand
    //{
    //	private RecipeGraphViewModel viewModel;
    //	private GraphViewModel memoGraph = new GraphViewModel();
    //	private GraphViewModel graphForDisplay;

    //	public PastNodesCommand(RecipeGraphViewModel vModel)
    //	{
    //		viewModel = vModel;
    //	}

    //	/// <summary>
    //	/// Execute the paste command. Make a copy of clipboard, builds the list of branches.
    //	/// Add branches in the graph
    //	/// </summary>
    //	public override void Execute()
    //	{
    //		AdcClipBoard clipBoard = AdcClipBoard.Instance();

    //		if (clipBoard.Graph.Nodes.Count > 0)
    //		{
    //			graphForDisplay = (GraphViewModel) clipBoard.Graph.Clone();
    //			// Move the new nodes so that they do not recover originals
    //			foreach (NodeViewModel node in graphForDisplay.Nodes)
    //			{
    //				node.X += 50;
    //			}

    //			viewModel.GraphVM.MergeGraph(graphForDisplay);
    //			ServiceRecipe.Instance().MergeGraph(graphForDisplay);
    //		}
    //	}

    //       public override void Undo()
    //       {
    //           if (graphForDisplay == null)
    //               return;

    //           // remove nodes from de graph adc
    //           foreach (ModuleNodeViewModel node in graphForDisplay.Nodes)
    //           {
    //               viewModel.GraphVM.RemoveNode(node);
    //               ServiceRecipe.Instance().RemoveModule(node.Module);
    //           }
    //       }

    //	public override void Redo()
    //	{
    //		graphForDisplay = (GraphViewModel)memoGraph.Clone();
    //		viewModel.GraphVM.MergeGraph(graphForDisplay);
    //		ServiceRecipe.Instance().MergeGraph(graphForDisplay);
    //	}
    //}

    //public class NodesDragCommand : IUndoableCommand
    //{
    //	struct NodePosInfo
    //	{
    //		public String name;
    //		public System.Windows.Rect nodeRect;
    //	}

    //	List<NodePosInfo> listOriginNodesPos = new List<NodePosInfo>();
    //	List<NodePosInfo> listRedoNodesPos = new List<NodePosInfo>();

    //	private RecipeGraphViewModel viewModel;

    //	public NodesDragCommand(RecipeGraphViewModel vModel)
    //	{
    //		viewModel = vModel;
    //	}

    //	public override void Execute()
    //	{
    //		NodePosInfo nodePosInfo;

    //           // if root is selected, remove it frome memoGraph
    //           foreach (ModuleNodeViewModel node in viewModel.GraphVM.GetselectedNodes())
    //		{
    //			if (!(node.Module is RootModule))
    //			{
    //				nodePosInfo = GetNodePos(node);
    //				listOriginNodesPos.Add(nodePosInfo);
    //			}
    //		}
    //	}

    //	public override void Undo()
    //	{
    //		NodePosInfo nodePosInfo;
    //		NodeViewModel node;

    //		listRedoNodesPos.Clear();

    //		foreach (NodePosInfo nodeOriginPosInfo in listOriginNodesPos)
    //		{
    //			node = viewModel.GraphVM.FindNode(nodeOriginPosInfo.name);
    //			if (node != null)
    //			{
    //				// Get actual node pos
    //				nodePosInfo = GetNodePos(node);
    //				listRedoNodesPos.Add(nodePosInfo);

    //				// restore original pos
    //				node.X = nodeOriginPosInfo.nodeRect.X;
    //				node.Y = nodeOriginPosInfo.nodeRect.Y;
    //			}
    //		}
    //	}

    //	public override void Redo()
    //	{
    //		NodeViewModel node;

    //		foreach (NodePosInfo nodePosInfo in listRedoNodesPos)
    //		{
    //			node = viewModel.GraphVM.FindNode(nodePosInfo.name);
    //			if (node != null)
    //			{
    //				// restore last node pos
    //				node.X = nodePosInfo.nodeRect.X;
    //				node.Y = nodePosInfo.nodeRect.Y;
    //			}
    //		}
    //	}

    //	private NodePosInfo GetNodePos(NodeViewModel node)
    //	{
    //		NodePosInfo nodePosInfo = new NodePosInfo();
    //		nodePosInfo.name = node.Name;
    //		nodePosInfo.nodeRect = node.NodeRect();
    //		return nodePosInfo;
    //	}
    //}

}
