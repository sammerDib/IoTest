using System;
using System.Collections.Generic;
using System.Linq;

using Utils;

namespace GraphModel
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
                Nodes.Add(((NodeViewModel)item));
                Nodes.Clone();
            });

            List<NodeViewModel> graphInList = Nodes.ToList();

            foreach (NodeViewModel node in listNodes)
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
            }

        }

        public GraphViewModel(GraphViewModel graph)
        {
            //NodeViewModel copyNode;

            //List<NodeViewModel> graphInList = Nodes.ToList();

            Nodes.AddRange((ImpObservableCollection<NodeViewModel>)(graph.Nodes.Clone()));
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
        public NodeViewModel FindNode(String nodeName)
        {
            foreach (NodeViewModel node in Nodes)
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
            GraphViewModel cloneGraph = new GraphViewModel();

            if (nodes != null)
            {
                cloneGraph.nodes = (ImpObservableCollection<NodeViewModel>)this.nodes.Clone();
            }

            NodeViewModel cloneChild;
            // faire pointer les connections sur les nodes clonés

            foreach (NodeViewModel cloneNode in cloneGraph.Nodes)
            {
                NodeViewModel SourceNode = (NodeViewModel)this.FindNode(cloneNode.Name);
                if (SourceNode != null)
                {
                    foreach (NodeViewModel child in SourceNode.GetOutNodesConnectedList())
                    {
                        cloneChild = cloneGraph.FindNode(child.Name);
                        if (cloneChild != null)
                        {
                            cloneGraph.ConnecteNode(cloneNode, cloneChild);
                        }

                    }
                }
                cloneGraph.DerefIllicitConnections((NodeViewModel)cloneNode);
            }


            /*		foreach (NodeViewModel cloneNode in cloneGraph.Nodes)
					{
						// Rechercher les fils 
						foreach (NodeViewModel child in cloneNode.GetOutNodesConnectedList())
						{
							// Find the child copy
							cloneChild = (NodeViewModel)cloneGraph.Find(child.Name);
							if (cloneChild != null)
							{
								// Connect it to the child copy
								cloneGraph.ConnecteNode(cloneNode, cloneChild);
							}

						};
						cloneGraph.DerefIllicitConnections((NodeViewModel)cloneNode);
					};
		*/
            return cloneGraph;
        }

        public void UpdateConnections()
        {
            Connections.Clear();
            foreach (NodeViewModel node in Nodes)
            {
                foreach (ConnectionViewModel connection in node.AttachedConnections)
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
            ConnectionViewModel connection = new ConnectionViewModel();
            connection.SourceConnector = nodeSrce.OutputConnectors[0];
            connection.DestConnector = nodeDst.InputConnectors[0];

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

            foreach (NodeViewModel child in GetChilds(node1))
            {
                if (child == node2)
                {
                    return true;
                }
            }
            foreach (NodeViewModel parent in GetParents(node1))
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
        public NodeViewModel Find(String nodeName)
        {
            foreach (NodeViewModel node in Nodes)
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
            foreach (NodeViewModel node in graphToMerge.Nodes)
            {
                Nodes.Add(node);
            }
            foreach (ConnectionViewModel connection in graphToMerge.Connections)
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
                    select (NodeViewModel)(connection.DestConnector.ParentNode)).ToList();
        }

        /// <summary>
        /// Get list of nodes that not have parent
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public List<NodeViewModel> GetOrphans(NodeViewModel nodeRoot)
        {
            List<NodeViewModel> resultListe = new List<NodeViewModel>();
            foreach (NodeViewModel node in Nodes)
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
                    select (NodeViewModel)(connection.SourceConnector.ParentNode)).ToList();
        }

        #endregion


    }
}
