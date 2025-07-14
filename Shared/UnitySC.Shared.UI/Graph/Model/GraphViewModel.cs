using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.UI.Graph.Utils;

namespace UnitySC.Shared.UI.Graph.Model
{
    /// <summary>
	/// Defines a graph of nodes and connections between the nodes.
	/// </summary>
	public sealed class GraphViewModel : ICloneable
    {
        #region Internal Data Members

        /// <summary>
        /// The collection of nodes in the graph.
        /// </summary>
        private ImpObservableCollection<NodeViewModel> nodes = null;

        /// <summary>
        /// The collection of connections in the graph.
        /// </summary>
        private ImpObservableCollection<ConnectionViewModel> connections = null;

        public GraphViewModel()
        {
        }

        public GraphViewModel(List<NodeViewModel> listNodes)
        {
            NodeViewModel copyNode;

            // Copy connections and connect nodes with  these new connection
            listNodes.ForEach((item) =>
            {
                Nodes.Add(item);
                Nodes.Clone();
            });

            var graphInList = Nodes.ToList();

            foreach (var node in listNodes)
            {
                // Find childs
                foreach (var child in node.GetOutNodesConnectedList())
                {
                    if (child.IsSelected)
                    {  // Find the child copy
                        copyNode = graphInList.Find(x => x.Name.Equals(child.Name));
                        if (copyNode != null)
                        {
                            // Connect it to the child copy
                            ConnecteNode(node, copyNode);
                        }
                    }
                };
                DerefIllicitConnections(node);
            }
        }

        public GraphViewModel(GraphViewModel graph)
        {
            //NodeViewModel copyNode;

            //List<NodeViewModel> graphInList = Nodes.ToList();

            Nodes.AddRange((ImpObservableCollection<NodeViewModel>)graph.Nodes.Clone());
            /*foreach (NodeViewModel node in graph.Nodes)
			{
				// Find childs
				foreach (NodeViewModel child in node.GetOutNodesConnectedList())
				{
					if (child.IsSelected)
					{  // Find the child copy
						copyNode = (NodeViewModel)graphInList.Find(x => ((NodeViewModel)x).Name.Equals(child.Name));
						if (copyNode != null)
						{
							// Connect it to the child copy
							ConnecteNode(node, copyNode);
						}
					}
				};
				DerefIllicitConnections(node);
			}*/
        }

        /// <summary>
        /// Return the node named nodeName
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public NodeViewModel FindNode(string nodeName)
        {
            foreach (var node in Nodes)
            {
                if (node.Name.Equals(nodeName))
                {
                    return node;
                }
            }
            return null;
        }

        #endregion Internal Data Members

        /// <summary>
        /// The collection of nodes in the graph.
        /// </summary>
        public ImpObservableCollection<NodeViewModel> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new ImpObservableCollection<NodeViewModel>();
                }

                return nodes;
            }
        }

        /// <summary>
        /// The collection of connections in the graph.
        /// </summary>
        public ImpObservableCollection<ConnectionViewModel> Connections
        {
            get
            {
                if (connections == null)
                {
                    connections = new ImpObservableCollection<ConnectionViewModel>();
                    connections.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(connections_ItemsRemoved);
                }

                return connections;
            }
        }

        #region Private Methods

        /// <summary>
        /// Event raised then Connections have been removed.
        /// </summary>
        private void connections_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectionViewModel connection in e.Items)
            {
                connection.SourceConnector = null;
                connection.DestConnector = null;
            }
        }

        #endregion Private Methods

        #region public Methods

        public object Clone()
        {
            var cloneGraph = new GraphViewModel();

            if (nodes != null)
            {
                cloneGraph.nodes = (ImpObservableCollection<NodeViewModel>)this.nodes.Clone();
            }

            NodeViewModel cloneChild;
            // faire pointer les connections sur les nodes clonés

            foreach (var cloneNode in cloneGraph.Nodes)
            {
                var SourceNode = this.FindNode(cloneNode.Name);
                if (SourceNode != null)
                {
                    foreach (var child in SourceNode.GetOutNodesConnectedList())
                    {
                        cloneChild = cloneGraph.FindNode(child.Name);
                        if (cloneChild != null)
                        {
                            cloneGraph.ConnecteNode(cloneNode, cloneChild);
                        }
                    }
                }
                cloneGraph.DerefIllicitConnections(cloneNode);
            }

            return cloneGraph;
        }

        public void UpdateConnections()
        {
            Connections.Clear();
            foreach (var node in Nodes)
            {
                foreach (var connection in node.AttachedConnections)
                {
                    if (Connections.Contains(connection) == false)
                    {
                        Connections.Add(connection);
                    }
                }
            }
        }

        public List<NodeViewModel> GetselectedNodes()
        {
            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var nodes = new List<NodeViewModel>(from n in Nodes where n.IsSelected select n);
            return nodes;
        }

        /// <summary>
        /// Connecte nodeSrce with nodeDst
        /// </summary>
        /// <param name="nodeSrce"></param>
        /// <param name="nodeDst"></param>
        public void ConnecteNode(NodeViewModel nodeSrce, NodeViewModel nodeDst)
        {
            if (nodeSrce == null)
                return;
            if (nodeDst == null)
                return;

            // Create a connection between the nodes.
            var connection = new ConnectionViewModel
            {
                SourceConnector = nodeSrce.OutputConnectors[0],
                DestConnector = nodeDst.InputConnectors[0]
            };

            // Add the connection to the view-model.
            Connections.Add(connection);
        }

        /// <summary>
        /// return true if node1 is connected with node2
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        public bool IsNodesConnected(NodeViewModel node1, NodeViewModel node2)
        {
            if ((node1 == null) || (node2 == null))
            {
                return false;
            }

            foreach (var child in GetChilds(node1))
            {
                if (child == node2)
                {
                    return true;
                }
            }
            foreach (var parent in GetParents(node1))
            {
                if (parent == node2)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Delete the links with the old parents an childrens
        /// </summary>
        /// <param name="nodeSrce"></param>
        /// <param name="nodeDst"></param>
        public void DerefIllicitConnections(NodeViewModel node)
        {
            if (node == null)
                return;

            // Deletion of the links with the old parents
            for (int i = 0; i < node.OutputConnectors.Count; i++)
            {
                if (node.OutputConnectors[i].IsConnected)
                {
                    for (int j = node.OutputConnectors[i].AttachedConnections.Count - 1; j >= 0; j--)
                    {
                        var connection = node.OutputConnectors[i].AttachedConnections[j];
                        if (connection.SourceConnector.ParentNode != node)
                        {
                            node.OutputConnectors[i].AttachedConnections.Remove(connection);
                        }
                    }
                }
            }
            // Deletion of the links with the old children
            for (int i = 0; i < node.InputConnectors.Count; i++)
            {
                if (node.InputConnectors[i].IsConnected)
                {
                    for (int j = node.InputConnectors[i].AttachedConnections.Count - 1; j >= 0; j--)
                    {
                        var connection = node.InputConnectors[i].AttachedConnections[j];
                        if (connection.DestConnector.ParentNode != node)
                        {
                            node.InputConnectors[i].AttachedConnections.Remove(connection);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Search the node named as nodeName
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public NodeViewModel Find(string nodeName)
        {
            foreach (var node in Nodes)
            {
                if (node.Name.Equals(nodeName))
                    return node;
            }
            return null;
        }

        /// <summary>
        /// Add graphToAdd to this graph
        /// </summary>
        /// <param name="graph"></param>
        public void MergeGraph(GraphViewModel graphToMerge)
        {
            foreach (var node in graphToMerge.Nodes)
            {
                Nodes.Add(node);
            }
            foreach (var connection in graphToMerge.Connections)
            {
                Connections.Add(connection);
            }
        }

        public void RemoveNode(NodeViewModel node)
        {
            connections.RemoveRange(node.AttachedConnections);
            Nodes.Remove(node);
        }

        /// <summary>
        /// Return the list of parentNode's childs
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public List<NodeViewModel> GetChilds(NodeViewModel parentNode)
        {
            if (parentNode == null)
                return null;

            return (from connection in parentNode.OutputConnectors[0].AttachedConnections
                    where connection.SourceConnector.ParentNode == parentNode
                    select connection.DestConnector.ParentNode).ToList();
        }

        /// <summary>
        /// Get list of nodes that not have parent
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public List<NodeViewModel> GetOrphans(NodeViewModel nodeRoot)
        {
            var resultListe = new List<NodeViewModel>();
            foreach (var node in Nodes)
            {
                if (node != nodeRoot)
                {
                    if (GetParents(node).Count == 0)
                    {
                        resultListe.Add(node);
                    }
                }
            }
            return resultListe;
        }

        /// <summary>
        /// Return the list of Node's parents
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        public List<NodeViewModel> GetParents(NodeViewModel Node)
        {
            if (Node == null)
                return null;

            return (from connection in Node.InputConnectors[0].AttachedConnections
                    where connection.DestConnector == Node.InputConnectors[0]
                    select connection.SourceConnector.ParentNode).ToList();
        }

        #endregion public Methods
    }
}