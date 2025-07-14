using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.Shared.UI.Commands;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.View;
using UnitySC.PM.Shared.UI.Recipes.Management.View.Graph;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.InsertModule;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.UndoRedo;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Graph.Model;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.Graph
{
    public class DataflowGraphViewModel : ObservableObject
    {
        private readonly Size _nodeSize = new Size(130, 45);
        private readonly Size _interNode = new Size(15, 40);
        private Size _spaceBetweenNode = new Size(130 + 15, 45 + 40);
        private Point _defaultRootPosition = new Point(200, 60);
        private DataflowRecipeComponent _dataflow;
        public string Name { get; set; }
        public string Comment { get; set; }
        private GraphViewModel _graphVM;
        private DataflowNodeViewModel _rootNode = null;
        private IDialogOwnerService _dialogService;
        private UnitySC.Shared.Logger.ILogger _logger;
        private IDFClientConfiguration _dfClientConfiguration;
        public int StepId { get; set; }

        private SharedSupervisors _sharedSupervisors;


        #region Undo - Redo
        private UndoRedoManager _undoRedoCommandManager = null;
        public UndoRedoManager UndoRedoCommandManager
        {
            get { return _undoRedoCommandManager ?? (_undoRedoCommandManager = new UndoRedoManager()); }
        }
        #endregion

        public DataflowNodeViewModel RootNode
        {
            get { return _rootNode; }
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                OnPropertyChanged();
                UpdateCommandState();
            }
        }

        private UserControl _currentRecipeSummaryUC;
        public UserControl CurrentRecipeSummaryUC
        {
            get => _currentRecipeSummaryUC; set { if (_currentRecipeSummaryUC != value) { _currentRecipeSummaryUC = value; OnPropertyChanged(); } }
        }

        public DataflowGraphViewModel(int stepId)
        {
            _graphVM = new GraphViewModel();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            IsEditable = true;
            _logger = ClassLocator.Default.GetInstance<ILogger<DataflowGraphViewModel>>();
            _sharedSupervisors = ClassLocator.Default.GetInstance<SharedSupervisors>();
            _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();
            StepId = stepId;
        }

        public GraphViewModel GraphVM
        {
            get { return _graphVM; }
        }

        public DataflowNodeViewModel SelectedNode
        {
            get
            {
                DataflowNodeViewModel selected = null;

                foreach (DataflowNodeViewModel n in _graphVM.Nodes)
                {
                    if (n.IsSelected)
                    {
                        if (selected != null)   // plus d'un noeud sélectionné
                            return null;
                        selected = n;
                    }
                }

                return selected;
            }
            set
            {
                var oldnode = SelectedNode;

                // On selectionne le noeud dans le graph
                //......................................
                foreach (DataflowNodeViewModel n in _graphVM.Nodes)
                    n.IsSelected = (n == value);

                if (value != null)
                    VisibleRect = value.NodeRect();

                UpdateCommandState();
                OnPropertyChanged();
                UpdateRecipeSummary();

            }
        }

        private void UpdateRecipeSummary()
        {
            if (SelectedNode != null && SelectedNode.ActorType != ActorType.DataflowManager)
            {
                var recipeSummary = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeSummary(SelectedNode.ActorType);
                if (recipeSummary != null)
                {
                    recipeSummary.Init(true);
                    recipeSummary.LoadRecipe(SelectedNode.Key);
                }
                CurrentRecipeSummaryUC = recipeSummary as UserControl;
            }
            else if (_rootNode != null)
            {
                var vm = new DataflowSummaryViewModel(_rootNode.Component);
                CurrentRecipeSummaryUC = new DataflowSummary();
                CurrentRecipeSummaryUC.DataContext = vm;
            }
        }


        private Rect _visibleRect = Rect.Empty;
        public Rect VisibleRect
        {
            get { return _visibleRect; }
            set
            {
                if (_visibleRect == value)
                    return;
                _visibleRect = value;
                OnPropertyChanged();
            }
        }

        public void LoadDataflow(DataflowRecipeComponent dataflow, string dataflowID = "")
        {
            if (dataflow != null)
                _dataflow = dataflow;
            else
            {
                // new empty dataflow
                _dataflow = new DataflowRecipeComponent();
                _dataflow.Name = Name;
                _dataflow.ActorType = ActorType.DataflowManager;
            }
            var dico = new Dictionary<Guid, DataflowNodeViewModel>();

            // Add root node
            var rootNode = CreateRootNode();
            rootNode.Info = dataflowID;
            dico[rootNode.Key] = rootNode;
            rootNode.X = _defaultRootPosition.X;
            rootNode.Y = _defaultRootPosition.Y;


            var tt = _dataflow.AllChilds().Skip(1).ToArray();

            // Add other nodes
            foreach (var component in _dataflow.AllChilds().Skip(1))
            {
                var node = new DataflowNodeViewModel(component, _dataflow);
                node.Size = _nodeSize;

                _graphVM.Nodes.Add(node);
                dico[node.Key] = node;

            }

            // Add connection between node
            foreach (var node in dico.Values)
            {
                foreach (var child in node.Children)
                {
                    var childnode = dico[child.Key];
                    _graphVM.ConnecteNode(node, childnode);
                }
            }

            // Align 
            Align(rootNode);

            FitContent?.Invoke(this, null);

            VisibleRect = _rootNode.NodeRect();
            VisibleRect = fullAreaRect();
            OnPropertyChanged(nameof(GraphContainsNodes));
            OnPropertyChanged(nameof(GraphVM));
        }

        //=================================================================
        //
        //=================================================================
        public DataflowNodeViewModel CreateNode(DataflowRecipeComponent dataflowRecipeComponent)
        {
            var node = new DataflowNodeViewModel(dataflowRecipeComponent, _dataflow);
            node.Size = _nodeSize;
            return node;
        }

        public DataflowNodeViewModel CreateRootNode()
        {
            _rootNode = new DataflowNodeViewModel(_dataflow, _dataflow);
            _rootNode.X = _defaultRootPosition.X;
            _rootNode.Y = _defaultRootPosition.Y;
            _rootNode.Size = _nodeSize;
            _graphVM.Nodes.Add(_rootNode);

            return _rootNode;
        }

        /// <summary>
        /// find node with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataflowNodeViewModel FindNode(Guid key)
        {
            foreach (DataflowNodeViewModel node in Nodes)
            {
                if (node.Key == key)
                    return node;
            }
            return null;
        }


        private Rect fullAreaRect()
        {
            if (!GraphContainsNodes)
                return Rect.Empty;

            var actualContentRect = Rect.Empty;
            foreach (DataflowNodeViewModel node in Nodes)
            {
                var nodeRect = new Rect(node.X, node.Y, node.Size.Width, node.Size.Height);
                if (actualContentRect.IsEmpty)
                    actualContentRect = nodeRect;
                else
                    actualContentRect = Rect.Union(actualContentRect, nodeRect);
            }
            return actualContentRect;
        }

        public IEnumerable<NodeViewModel> Nodes
        {
            get
            {
                return _graphVM.Nodes;
            }
        }

        public bool GraphContainsNodes
        {
            get
            {
                return _graphVM.Nodes.Any();
            }
        }

        ///////////////////////////////////////////////////////////////////
        #region Drag'n'Drop
        ///////////////////////////////////////////////////////////////////

        /// <summary>
        /// Called when the user has started to drag out a connector, thus creating a new connection.
        /// </summary>
        public ConnectionViewModel ConnectionDragStarted(ConnectorViewModel draggedOutConnector, Point curDragPoint)
        {
            //
            // Create a new connection to add to the view-model.
            //
            var connection = new ConnectionViewModel();

            if (draggedOutConnector.Type == ConnectorType.Output)
            {
                //
                // The user is dragging out a source connector (an output) and will connect it to a destination connector (an input).
                //
                connection.SourceConnector = draggedOutConnector;
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                //
                // The user is dragging out a destination connector (an input) and will connect it to a source connector (an output).
                //
                connection.DestConnector = draggedOutConnector;
                connection.SourceConnectorHotspot = curDragPoint;
            }

            //
            // Add the new connection to the view-model.
            //
            _graphVM.Connections.Add(connection);

            return connection;
        }

        /// <summary>
        /// Called to query the application for feedback while the user is dragging the connection.
        /// </summary>
        public void QueryConnnectionFeedback(ConnectorViewModel draggedOutConnector, ConnectorViewModel draggedOverConnector, out object feedbackIndicator, out bool connectionOk)
        {
            if (draggedOutConnector == draggedOverConnector)
            {
                //
                // Can't connect to self!
                // Provide feedback to indicate that this connection is not valid!
                //
                feedbackIndicator = new ConnectionBadIndicator();
                connectionOk = false;
            }
            else
            {
                var sourceConnector = draggedOutConnector;
                var destConnector = draggedOverConnector;

                //
                // Only allow connections from output connector to input connector (ie each
                // connector must have a different type).
                // Also only allocation from one node to another, never one node back to the same node.
                //
                connectionOk = sourceConnector.ParentNode != destConnector.ParentNode &&
                                 sourceConnector.Type != destConnector.Type;

                if (connectionOk)
                {
                    // 
                    // Yay, this is a valid connection!
                    // Provide feedback to indicate that this connection is ok!
                    //
                    feedbackIndicator = new ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new ConnectionBadIndicator();
                }
            }
        }

        /// <summary>
        /// Called as the user continues to drag the connection.
        /// </summary>
        public void ConnectionDragging(Point curDragPoint, ConnectionViewModel connection)
        {
            if (connection.DestConnector == null)
            {
                connection.DestConnectorHotspot = curDragPoint;
            }
            else
            {
                connection.SourceConnectorHotspot = curDragPoint;
            }
        }

        /// <summary>
        /// Called when the user has finished dragging out the new connection.
        /// </summary>
        public void ConnectionDragCompleted(ConnectionViewModel newConnection, ConnectorViewModel connectorDraggedOut, ConnectorViewModel connectorDraggedOver)
        {
            if (connectorDraggedOver == null)
            {
                //
                // The connection was unsuccessful.
                // Maybe the user dragged it out and dropped it in empty space.
                //
                _graphVM.Connections.Remove(newConnection);
                return;
            }

            //
            // Only allow connections from output connector to input connector (ie each
            // connector must have a different type).
            // Also only allocation from one node to another, never one node back to the same node.
            //
            bool connectionOk =
                                connectorDraggedOut.ParentNode != connectorDraggedOver.ParentNode &&
                                connectorDraggedOut.Type != connectorDraggedOver.Type;

            if (!connectionOk)
            {
                _graphVM.Connections.Remove(newConnection);
                return;
            }

            //
            // On récupère les noeuds parent/fils
            //
            DataflowNodeViewModel parentNode, childNode;
            if (newConnection.DestConnector == null)
            {
                parentNode = (DataflowNodeViewModel)connectorDraggedOut.ParentNode;
                childNode = (DataflowNodeViewModel)connectorDraggedOver.ParentNode;
            }
            else
            {
                parentNode = (DataflowNodeViewModel)connectorDraggedOver.ParentNode;
                childNode = (DataflowNodeViewModel)connectorDraggedOut.ParentNode;
            }

            //
            // Verifie la compatibilité des modules
            //
            bool compatible = IsCompatibleWithParent(childNode.Component, parentNode.Component);
            if (!compatible)
            {
                _graphVM.Connections.Remove(newConnection);
                _dialogService.ShowMessageBox("Modules are not compatible", "Module compatibility", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //
            // The user has dragged the connection on top of another valid connector.
            //

            //
            // Remove any existing connection between the same two connectors.
            //
            var existingConnection = FindConnection(connectorDraggedOut, connectorDraggedOver);
            if (existingConnection != null)
            {
                _graphVM.Connections.Remove(existingConnection);
            }

            //
            // Finalize the connection by attaching it to the connector
            // that the user dragged the mouse over.
            //
            if (newConnection.DestConnector == null)
                newConnection.DestConnector = connectorDraggedOver;
            else
                newConnection.SourceConnector = connectorDraggedOver;

            parentNode.Component.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, childNode.Component));

            // Old adc v9
            //ServiceRecipe.Instance().ConnectModules(parentNode.Module, childNode.Module);
            Align(parentNode);

            SelectedNode = null;

        }

        private bool IsCompatibleWithParent(DataflowRecipeComponent child, DataflowRecipeComponent parent)
        {
            var res = false;
            switch (parent.ActorCategory)
            {
                case ActorCategory.ProcessModule:
                case ActorCategory.PostProcessing:
                    res = parent.Outputs.Intersect(child.Inputs).Count() > 0;
                    break;
                case ActorCategory.Manager:
                    res = child.ActorType.GetCatgory() == ActorCategory.ProcessModule;
                    break;
                default:
                    throw new InvalidOperationException("Unknow actor category");
            }

            return res;
        }


        /// <summary>
        /// Called when the user has finished dragging out the node.
        /// </summary>
        public void NodeDragCompletedEvent()
        {
            // ServiceRecipe.Instance().MustBeSaved = true;
        }
        #endregion

        #region FitContent
        public delegate void FitContentEvent(object sender, System.Windows.Input.ExecutedRoutedEventArgs e);
        public event FitContentEvent FitContent;
        public event DataflowGraphChangedEvent Changed;
        #endregion

        /// <summary>
        /// Retrieve a connection between the two connectors.
        /// Returns null if there is no connection between the connectors.
        /// </summary>
        public ConnectionViewModel FindConnection(ConnectorViewModel connector1, ConnectorViewModel connector2)
        {
            Trace.Assert(connector1.Type != connector2.Type);

            //
            // Figure out which one is the source connector and which one is the
            // destination connector based on their connector types.
            //
            var sourceConnector = connector1.Type == ConnectorType.Output ? connector1 : connector2;
            var destConnector = connector1.Type == ConnectorType.Output ? connector2 : connector1;

            //
            // Now we can just iterate attached connections of the source
            // and see if it each one is attached to the destination connector.
            //

            foreach (var connection in sourceConnector.AttachedConnections)
            {
                if (connection.DestConnector == destConnector)
                {
                    //
                    // Found a connection that is outgoing from the source connector
                    // and incoming to the destination connector.
                    //
                    return connection;
                }
            }

            return null;
        }


        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            DataflowNodeViewModel parentNode = (DataflowNodeViewModel)connection.SourceConnector.ParentNode;
            DataflowNodeViewModel childNode = (DataflowNodeViewModel)connection.DestConnector.ParentNode;

            // disconnect component
            parentNode.Component.ChildRecipes.RemoveAll(a => a.Component.Key == childNode.Component.Key);

            Changed?.Invoke();

            _graphVM.Connections.Remove(connection);
        }

        /// <summary> Dico pour retrouver facilement un noeud à partir de l'ID du module </summary>
        private Dictionary<Guid, DataflowNodeViewModel> _nodeVMs;

        private void Align(DataflowNodeViewModel node)
        {
            _nodeVMs = new Dictionary<Guid, DataflowNodeViewModel>();
            foreach (DataflowNodeViewModel nodeVm in _graphVM.Nodes)
            {
                _nodeVMs.Add(nodeVm.Key, nodeVm);
            }
            SetSubTreePosition(node, node.X, node.Y);
        }

        private void SetSubTreePosition(DataflowNodeViewModel node, double x, double y)
        {
            //-------------------------------------------------------------
            // Position du module
            //-------------------------------------------------------------
            node.X = x;
            node.Y = y;

            //-------------------------------------------------------------
            // Enfants
            //-------------------------------------------------------------
            var listChildsNodes = node.GetOutNodesConnectedList();
            double totalwidth = 0;

            foreach (DataflowNodeViewModel child in listChildsNodes)
            {
                if (totalwidth > 0)
                    totalwidth += _interNode.Width;
                totalwidth += child.Size.Width;
            }

            // Position du fils courant. 
            double xChild = node.X + node.Size.Width / 2 - totalwidth / 2;
            // Simulation du 1er fils pour détecter les collisions à gauche 
            Rect RectChild0 = new Rect(xChild, node.Y + _spaceBetweenNode.Height, _nodeSize.Width, _nodeSize.Height);

            if (listChildsNodes.Count > 0)
            {
                // Gérer les collisions à gauche
                foreach (DataflowNodeViewModel n in _graphVM.Nodes)
                {
                    if ((listChildsNodes.Contains(n) == false) && (n != node))
                    {
                        Rect nRect = n.NodeRect();
                        Rect IntersectNode = Rect.Intersect(nRect, RectChild0);
                        if (IntersectNode.IsEmpty == false) // Y-at'il une collision ?
                        {
                            if ((n.X < RectChild0.X) && (IntersectNode.Width > 0))
                            {
                                RectChild0.X += IntersectNode.Width + _interNode.Width;  // On se décale à droite 
                            }
                        }
                    }
                }
                if (xChild < RectChild0.X) // on a décalé à droite
                    node.X += RectChild0.X - xChild;
                xChild = RectChild0.X;
            }

            // Placement des fils
            for (int i = 0; i < node.Children.Count(); i++)
            {
                var recipeComponent = node.Children[i];

                var childnode = _nodeVMs[recipeComponent.Key];
                if (childnode.Parents.Count == 1)
                {
                    x = xChild;
                    y = node.Y + _spaceBetweenNode.Height;
                    xChild += childnode.Size.Width + _interNode.Width; // On incrémente xChild

                }
                else
                {
                    CenterModuleBelowItsParents(childnode, out x, out y);
                }

                SetSubTreePosition(childnode, x, y);
            }
        }

        //================================================================
        /// <summary>
        /// Calcul du X/Y pour aligner un noeud sous ses parents. 
        /// Utile quand il en a plusieurs.
        /// NB: On fait le calcul mais on ne déplace pas le noeud.
        /// </summary>
        //================================================================
        private void CenterModuleBelowItsParents(DataflowNodeViewModel node, out double x, out double y)
        {
            double xmin = double.PositiveInfinity;
            double xmax = double.NegativeInfinity;
            double ymax = double.NegativeInfinity;

            foreach (var parent in node.Parents)
            {
                var parentVM = _nodeVMs[parent.Key];
                xmin = Math.Min(xmin, parentVM.X);
                xmax = Math.Max(xmax, parentVM.X);
                ymax = Math.Max(ymax, parentVM.Y);
            }

            x = (xmin + xmax) / 2;
            y = ymax + _spaceBetweenNode.Height;
        }


        /// <summary>
        /// Retourne la liste des ascendants de node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<DataflowNodeViewModel> GetNodeAscendants(DataflowNodeViewModel node)
        {
            if (node == null)
                return null;

            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var ascendants = new List<DataflowNodeViewModel>();
            var nodes = new List<NodeViewModel>();
            var parents = new List<NodeViewModel>();

            parents = GraphVM.GetParents(node);
            if (parents.Count > 0)
            {
                nodes.AddRange(parents);
                foreach (DataflowNodeViewModel parent in parents)
                    nodes.AddRange(GetNodeAscendants(parent));

                foreach (DataflowNodeViewModel ascendant in nodes)
                {
                    if (ascendants.Contains(ascendant) == false)
                        ascendants.Add(ascendant);
                }
            }
            return ascendants;
        }


        public void ClearGraph()
        {
            _undoRedoCommandManager.Clear();

            _graphVM.Nodes.Clear();
            _graphVM.Connections.Clear();
            _rootNode = null;
            OnPropertyChanged(nameof(GraphContainsNodes));
        }

        public bool GraphHasSingleSelection
        {
            get
            {
                int count = _graphVM.Nodes.Count(n => n.IsSelected);
                return count == 1;
            }

        }

        public bool IsGraphHasSelectedNodes
        {
            get
            {
                return _graphVM.GetselectedNodes().Any();
            }
        }

        /// <summary>
        /// Delete the currently selected nodes from the view-model.
        /// </summary>
        public void DeleteSelectedNodes()
        {

            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var nodes = _graphVM.GetselectedNodes();
            //new List<NodeViewModel>(from n in Graph.Nodes where n.IsSelected select n);
            if (nodes.Count() == 0)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("No node selected !");
                return;
            }

            while (nodes.Count > 0)
            {
                DataflowNodeViewModel node = (DataflowNodeViewModel)nodes.Last();
                nodes.Remove(node);
                DeleteNode(ref node);
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(ref DataflowNodeViewModel node)
        {
            if (node != null)
            {
                // Don't delete Root !
                if (node == RootNode)
                    return;

                Changed?.Invoke();

                //
                // Remove all connections attached to the node.
                //
                _graphVM.Connections.RemoveRange(node.AttachedConnections);

                //
                // Remove the node from the graph.
                //
                _graphVM.Nodes.Remove(node);

                if (node.Parents.Count > 0)
                {
                    var recipeKeyToDel = node.Component.Key;
                    foreach (var ancestor in node.Parents)
                        ancestor.ChildRecipes.RemoveAll(a => a.Component.Key == recipeKeyToDel);
                }
                node = null;
            }
        }

        //=================================================================
        /// <summary>
        /// Ajoute un nouveau noeud au ViewModel.
        /// </summary>
        //=================================================================
        public void AddChild(DataflowNodeViewModel parentNode, DataflowNodeViewModel childNode)
        {
            Changed?.Invoke();

            //// Ajout dans le dataflow
            ////............................
            parentNode.Component.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, childNode.Component));

            //// Ajoute dans le graphe
            ////......................
            _graphVM.ConnecteNode(parentNode, childNode);
            _graphVM.Nodes.Add(childNode);
        }


        //=================================================================
        /// <summary>
        /// Création d'une nouvelle branche à partir du noeud sélectionné,
        /// avec une boîte de dialogue pour choisir le(s) module(s) à ajouter.
        /// </summary>
        /// <returns>
        /// La liste des noeuds de la nouvelle branche. 
        /// Elle peut contenir plusieurs noeuds si on ajoute un méta-bloc.
        /// </returns>
        //=================================================================
        public List<DataflowNodeViewModel> CreateNewBranch()
        {
            return AddModules(inNewBranch: true);
        }

        //=================================================================
        /// <summary>
        /// Ajoute des modules avec une boîte de dialogue pour choisir le(s)
        /// module(s) à ajouter.
        /// </summary>
        /// 
        /// <param name="inNewBranch"> 
        /// Les modules sont ajoutés soit dans une nouvelle branche,
        /// soit entre le nodule et ses fils.
        /// </param>
        /// 
        /// <returns>
        /// La liste des noeuds de la nouvelle branche. 
        /// Elle peut contenir plusieurs noeuds si on ajoute un méta-bloc.
        /// </returns>
        //=================================================================
        public List<DataflowNodeViewModel> AddModules(bool inNewBranch)
        {
            DataflowNodeViewModel parentNode = SelectedNode;
            if (parentNode == null)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("No Node selected");
                return null;
            }

            //TODO : find stepId ??           
            Guid? parentGuid = new Guid?();
            if (parentNode.ActorType != ActorType.DataflowManager)
                parentGuid = parentNode.Key;

            var insertVM = new InsertModuleViewModel(StepId, parentGuid, _rootNode.Component.AllChilds().Select(x => x.Key).ToList());
            var res = _dialogService.ShowDialog<InsertModuleWindow>(insertVM);


            List<DataflowNodeViewModel> newNodeList = new List<DataflowNodeViewModel>();

            if (res.HasValue && res.Value)
            {
                DataflowNodeViewModel childNode = CreateNode(insertVM.InsertResult);
                newNodeList.Add(childNode);

                if (inNewBranch)
                    AddChild(parentNode, childNode);
                else
                    InsertChild(parentNode, childNode);

                Align(parentNode);
                SelectedNode = newNodeList.First();
            }

            return newNodeList;
        }

        public DataflowNodeViewModel InsertChild(DataflowNodeViewModel parentNode, string s)
        {
            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("NOT IMPLEMENTED InsertChild(DataflowNodeViewModel, string)");
            return null;
        }

        //=================================================================
        /// <summary>
        /// Insère un noeud entre un parent et ses fils.
        /// Version sans IHM.
        /// </summary>
        //=================================================================
        public DataflowNodeViewModel InsertChild(DataflowNodeViewModel parentNode, DataflowNodeViewModel newNode)
        {
            //-------------------------------------------------------------
            // Ajoute le nouveau module dans le ViewModel
            //-------------------------------------------------------------
            // Déconnecte les fils du parent
            List<NodeViewModel> childNodes = GraphVM.GetChilds(parentNode); // A faire avant d'ajouter le nouveau fils
            foreach (var childNode in childNodes)
                DisConnectNodes(parentNode, childNode);

            // Ajout du nouveau noeud
            AddChild(parentNode, newNode);

            // Reconnecte les fils au nouveau noeud
            foreach (DataflowNodeViewModel childNode in childNodes)
            {
                _graphVM.ConnecteNode(newNode, childNode);
                // Connect component
                newNode.Component.ChildRecipes.Add(new DataflowRecipeAssociation(AssociationRecipeType.All, childNode.Component));
            }

            //-------------------------------------------------------------
            // Repositionne les noeuds
            //-------------------------------------------------------------
            //  AlignSubTree(parentNode);
            SelectedNode = newNode;

            return newNode;
        }


        //=================================================================
        /// <summary>
        /// Crée un noeud entre un parent et ses fils.
        /// Version avec IHM pour sélectionner le modules.
        /// </summary>
        //=================================================================
        public DataflowNodeViewModel InsertChild()
        {
            List<DataflowNodeViewModel> newnodes = AddModules(inNewBranch: false);
            return newnodes?.FirstOrDefault();
        }

        /// <summary>
        /// DisConnect Nodes
        /// </summary>
        /// <param name="nodeSrce"></param>
        /// <param name="nodeDst"></param>
        public void DisConnectNodes(NodeViewModel nodeSrce, NodeViewModel nodeDst)
        {
            if (nodeSrce == null)
                return;
            if (nodeDst == null)
                return;

            // Delete  connection between the nodes. 
            foreach (var connection in _graphVM.Connections)
            {
                if ((connection.DestConnector == nodeDst.InputConnectors[0]) &&
                    (connection.SourceConnector == nodeSrce.OutputConnectors[0]))
                {
                    DeleteConnection(connection);
                    return;
                }
            }
        }



        #region commands		


        private AutoRelayCommand _alignNodesCommand = null;
        public AutoRelayCommand AlignNodesCommand
        {
            get
            {
                return _alignNodesCommand ?? (_alignNodesCommand = new AutoRelayCommand(
              () =>
              {
                  if (_graphVM.Nodes.Any(x => x.IsSelected))
                      Align(_graphVM.Nodes.OfType<DataflowNodeViewModel>().First(x => x.IsSelected));
              },
              () => { return IsEditable; }));
            }
        }

        private AutoRelayCommand _editRecipeCommand;
        public AutoRelayCommand EditRecipeCommand
        {
            get
            {
                return _editRecipeCommand ?? (_editRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  if (SelectedNode.ActorType != ActorType.DataflowManager)
                  {
                      var mainDataflowVM = ClassLocator.Default.GetInstance<DataflowManagementViewModel>();

                      if (SelectedNode != null)
                      {
                          bool hardwareReserved = false;

                          // Reserve hardware
                          if (SelectedNode.ActorType.GetCatgory() == ActorCategory.ProcessModule)
                          {
                              hardwareReserved = _sharedSupervisors.GetGlobalStatusSupervisor(SelectedNode.ActorType).ReserveHardware()?.Result ?? false;
                          }
                          else
                          {
                              hardwareReserved = true;
                          }

                          if (hardwareReserved)
                          {
                              // Load recipe                               
                              var recipeEditor = ClassLocator.Default.GetInstance<ExternalUserControls>().GetRecipeEditor(SelectedNode.ActorType);
                              if (recipeEditor != null)
                              {

                                  try
                                  {
                                      recipeEditor.Init(true);
                                      recipeEditor.LoadRecipe(SelectedNode.Key);
                                  }
                                  catch (Exception ex)
                                  {
                                      if (SelectedNode.ActorType.GetCatgory() == ActorCategory.ProcessModule)
                                      {
                                          _sharedSupervisors.GetGlobalStatusSupervisor(SelectedNode.ActorType).ReleaseHardware();
                                      }
                                      hardwareReserved=false;


                                          throw new Exception(ex.Message);
                                  }

                                  recipeEditor.ExitEditor -= Uc_ExitEditor;
                                  recipeEditor.ExitEditor += Uc_ExitEditor;
                              }

                              mainDataflowVM.CurrentControl = recipeEditor;
                          }
                          else
                          {
                              mainDataflowVM.CurrentControl = null;
                              string message = string.Format("{0} is not available for recipe editing", SelectedNode.ActorType);
                              _logger.Warning("Reserve hardware -" + message);
                              _dialogService.ShowMessageBox(message, "Reserve Hardware", MessageBoxButton.OK, MessageBoxImage.Warning);
                          }
                      }
                      else
                      {
                          mainDataflowVM.CurrentControl = null;
                      }
                  }
              },
              () =>
              {
                  var user = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser;
                  return SelectedNode != null && SelectedNode.ActorType != ActorType.DataflowManager && (user != null && user.Rights.Contains(UserRights.RecipeEdition));
              }));
            }
        }

        private AutoRelayCommand _shareUnShareNodeCommand;
        public AutoRelayCommand ShareUnShareNodeCommand
        {
            get
            {
                return _shareUnShareNodeCommand ?? (_shareUnShareNodeCommand = new AutoRelayCommand(
              () =>
              {
                  if (SelectedNode != null && SelectedNode.Component != null)
                  {
                      try
                      {
                          var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                          ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>().Invoke(x => x.ChangeRecipeSharedState(SelectedNode.Key, userId, !SelectedNode.IsShared));
                          SelectedNode.IsShared = !SelectedNode.IsShared;
                      }
                      catch (Exception ex)
                      {
                          _logger.Error("Share unshare error", ex);
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Share unshare error");
                      }
                  }
              },
              () => { return SelectedNode != null && SelectedNode.ActorType != ActorType.DataflowManager; }));
            }
        }

        private AutoRelayCommand _shareUnShareCommand;
        public AutoRelayCommand ShareUnShareCommand
        {
            get
            {
                return _shareUnShareCommand ?? (_shareUnShareCommand = new AutoRelayCommand(
              () =>
              {
                  if (_rootNode != null && _rootNode.Component != null)
                  {
                      try
                      {
                          if (_rootNode.IsShared || _rootNode.Component.AllChilds().Where(x => x.ActorType != ActorType.DataflowManager).All(x => x.IsShared))
                          {
                              var userId = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser.Id;
                              ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>().Invoke(x => x.ChangeRecipeSharedState(_rootNode.Key, userId, !_rootNode.IsShared));
                              _rootNode.IsShared = !_rootNode.IsShared;
                          }
                          else
                          {
                              ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("All child recipes must be shared", "Child recipes", MessageBoxButton.OK, MessageBoxImage.Warning);
                          }

                      }
                      catch (Exception ex)
                      {
                          _logger.Error("Share unshare error", ex);
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Share unshare error");
                      }
                  }
              },
              () => { return true; }));
            }
        }
        private void Uc_ExitEditor(object sender, EventArgs e)
        {
            if (SelectedNode != null)
            {
                SelectedNode.Update();
                if (SelectedNode.ActorType.GetCatgory() == ActorCategory.ProcessModule)
                {
                    _sharedSupervisors.GetGlobalStatusSupervisor(SelectedNode.ActorType).ReleaseHardware();
                }
            }

            ClassLocator.Default.GetInstance<DataflowManagementViewModel>().CurrentControl = null;
        }

        private AutoRelayCommand _deleteNodeCommand = null;
        public AutoRelayCommand DeleteNodeCommand
        {
            get
            {
                return _deleteNodeCommand ?? (_deleteNodeCommand = new AutoRelayCommand(
              () =>
              {
                  UndoRedoCommandManager.ExecuteCmd(new DeleteNodesCommand(this));
              },
              () => { return IsEditable && IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _addChildNodeCommand = null;
        public AutoRelayCommand AddChildNodeCommand
        {
            get
            {
                return _addChildNodeCommand ?? (_addChildNodeCommand = new AutoRelayCommand(
              () =>
              {
                  UndoRedoCommandManager.ExecuteCmd(new AddChildCommand(this));
              },
              () => { return IsEditable && GraphHasSingleSelection; }));
            }
        }

        private AutoRelayCommand _insertChildNodeCommand = null;
        public AutoRelayCommand InsertChildNodeCommand
        {
            get
            {
                return _insertChildNodeCommand ?? (_insertChildNodeCommand = new AutoRelayCommand(
              () =>
              {
                  UndoRedoCommandManager.ExecuteCmd(new InsertChildCommand(this));
              },
              () => { return IsEditable && GraphHasSingleSelection; }));
            }
        }

        private AutoRelayCommand<ConnectionViewModel> _deleteConnectionCommand = null;
        public AutoRelayCommand<ConnectionViewModel> DeleteConnectionCommand
        {
            get
            {
                return _deleteConnectionCommand ?? (_deleteConnectionCommand = new AutoRelayCommand<ConnectionViewModel>(
              (e) =>
              {
                  DeleteConnection(e);
              },
             (e) => { return IsEditable; }));
            }
        }

        private AutoRelayCommand _copyNodeCommand = null;
        public AutoRelayCommand CopyNodeCommand
        {
            get
            {
                return _copyNodeCommand ?? (_copyNodeCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Todo");
                  // Adcv9 => UndoRedoCommandManager.ExecuteCmd(new UndoRedo.CopyNodesCommand(this));
              },
              () => { return IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _pastNodeCommand = null;
        public AutoRelayCommand PastNodeCommand
        {
            get
            {
                return _pastNodeCommand ?? (_pastNodeCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Todo");
                  // Adcv9 => UndoRedoCommandManager.ExecuteCmd(new UndoRedo.PastNodesCommand(this)); 
              },
              () =>
              {
                  // Adcv9 => return UndoRedo.AdcClipBoard.Instance().IsEmpty() == false;
                  return true;
              }));
            }
        }

        private AutoRelayCommand _cutNodeCommand = null;
        public AutoRelayCommand CutNodeCommand
        {
            get
            {
                return _cutNodeCommand ?? (_cutNodeCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Todo");
                  // Adcv9 => 
                  //UndoRedoCommandManager.ExecuteCmd(new UndoRedo.CopyNodesCommand(this));
                  //UndoRedoCommandManager.ExecuteCmd(new UndoRedo.DeleteNodesCommand(this));
              },
              () => { return IsEditable && IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _undoRecipeCommand = null;
        public AutoRelayCommand UndoRecipeCommand
        {
            get { return _undoRecipeCommand ?? (_undoRecipeCommand = new AutoRelayCommand(() => { UndoRedoCommandManager.Undo(); }, () => { return true; })); }
        }

        private AutoRelayCommand _redoRecipeCommand = null;
        public AutoRelayCommand RedoRecipeCommand
        {
            get { return _redoRecipeCommand ?? (_redoRecipeCommand = new AutoRelayCommand(() => { UndoRedoCommandManager.Redo(); }, () => { return true; })); }
        }

        private void UpdateCommandState()
        {
            ShareUnShareNodeCommand.NotifyCanExecuteChanged();
            EditRecipeCommand.NotifyCanExecuteChanged();
            DeleteNodeCommand.NotifyCanExecuteChanged();
            AddChildNodeCommand.NotifyCanExecuteChanged();
            InsertChildNodeCommand.NotifyCanExecuteChanged();
            DeleteConnectionCommand.NotifyCanExecuteChanged();


            UndoRecipeCommand.NotifyCanExecuteChanged();
            RedoRecipeCommand.NotifyCanExecuteChanged();


        }

        #endregion
    }
}
