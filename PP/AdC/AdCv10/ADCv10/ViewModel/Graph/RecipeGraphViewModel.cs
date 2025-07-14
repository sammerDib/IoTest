using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Xml;

using ADC.AdcEnum;
using ADC.Model;
using ADC.UndoRedo.Command;
using ADC.View;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using GraphModel;

using Microsoft.Win32;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils;

namespace ADC.ViewModel.Graph
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class RecipeGraphViewModel : ObservableRecipient
    {
        public string Name { get; set; }
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

        private GraphViewModel _graphVM;

        public RecipeGraphViewModel()
        {
            _graphVM = new GraphViewModel();
        }

        public GraphViewModel GraphVM
        {
            get { return _graphVM; }
        }

        private Size nodeSize = new Size(130, 45);
        private Size interNode = new Size(15, 40);
        private Size pas = new Size(130 + 15, 45 + 40); // Pas d'un noeud au suivant
        private Point defaultRootPosition = new Point(900, 60);
        private Recipe Recipe { get { return ServiceRecipe.Instance().RecipeCurrent; } }

        /// <summary>
        /// This node is the first node created for the graph
        /// </summary>
        private ModuleNodeViewModel rootnode = null;


        #region Undo - Redo
        public UndoRedo.UndoRedoManager UndoRedocommandManager
        {
            get { return RecipeViewModel.UndoRedocommandManager; }
        }
        #endregion

        //===================================================================
        // Le noeud sélectionné dans le graph
        //===================================================================
        public ModuleNodeViewModel SelectedNode
        {
            get
            {
                ModuleNodeViewModel selected = null;

                foreach (ModuleNodeViewModel n in _graphVM.Nodes)
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
                ModuleNodeViewModel oldnode = SelectedNode;

                // On selectionne le noeud dans le graph
                //......................................
                foreach (ModuleNodeViewModel n in _graphVM.Nodes)
                    n.IsSelected = (n == value);

                if (value != null)
                    VisibleRect = value.NodeRect();
                ClassLocator.Default.GetInstance<IMessenger>().Send(new Messages.SelectedModuleChanged(this) { OldModule = oldnode, NewModule = value });
                UpdateCommandState();
            }
        }

        public void RefreshSelectedNode()
        {
            OnPropertyChanged(nameof(SelectedNode));
        }

        //=================================================================
        //
        //=================================================================
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

        //=================================================================
        //
        //=================================================================
        public void LoadRecipe(XmlDocument xmldoc)
        {
            //TODO ce dico devrait être global ou mieux dans le _graphVM
            Dictionary<int, ModuleNodeViewModel> dico = new Dictionary<int, ModuleNodeViewModel>();

            //-------------------------------------------------------------
            // Chargement de la Recipe ADC
            //-------------------------------------------------------------
            Dictionary<int, Point> nodesPosList;
            nodesPosList = ServiceRecipe.Instance().LoadRecipe(xmldoc);

            //-------------------------------------------------------------
            // Création du noeud root
            //-------------------------------------------------------------
            ModuleNodeViewModel rootnode = CreateRootNode();
            dico[rootnode.Module.Id] = rootnode;

            Point pos;
            bool found = nodesPosList.TryGetValue(rootnode.Module.Id, out pos);
            if (!found)
                pos = defaultRootPosition;
            rootnode.X = pos.X;
            rootnode.Y = pos.Y;

            //-------------------------------------------------------------
            // Création des Nodes
            //-------------------------------------------------------------
            foreach (ModuleBase module in Recipe.ModuleList.Values)
            {
                if (module is RootModule || module is TerminationModule)
                    continue;

                ModuleNodeViewModel node = new ModuleNodeViewModel(module);
                node.Size = nodeSize;
                _graphVM.Nodes.Add(node);

                dico[node.Module.Id] = node;
            }

            //-------------------------------------------------------------
            // Calcul des positions
            //-------------------------------------------------------------
            foreach (ModuleNodeViewModel node in dico.Values)
            {
                found = nodesPosList.TryGetValue(node.Module.Id, out pos);
                if (found)
                {
                    node.X = pos.X;
                    node.Y = pos.Y;
                }
            }

            //-------------------------------------------------------------
            // Connexion des noeuds
            //-------------------------------------------------------------
            foreach (ModuleNodeViewModel node in dico.Values)
            {
                if (node.Module is TerminationModule)
                    continue;

                foreach (ModuleBase childmodule in node.Module.Children)
                {
                    if (childmodule is TerminationModule)
                        continue;

                    ModuleNodeViewModel childnode = dico[childmodule.Id];
                    _graphVM.ConnecteNode(node, childnode);
                }
            }

            //-------------------------------------------------------------
            // Positions manquantes
            //-------------------------------------------------------------
            foreach (ModuleNodeViewModel node in dico.Values)
            {
                if (node.Module is TerminationModule)
                    continue;
                if (node.X == 0 && node.Y == 0)
                {
                    ModuleBase parentmodule = node.Module.Parents.First();
                    ModuleNodeViewModel parentNode = dico[parentmodule.Id];
                    AlignSubTree(parentNode);
                }
            }

            //-------------------------------------------------------------
            // On informe l'IHM
            //-------------------------------------------------------------
            VisibleRect = rootnode.NodeRect();

            OnPropertyChanged(nameof(GraphContainsNodes));
            OnPropertyChanged(nameof(GraphVM));
        }


        /// <summary>
        /// find node with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ModuleNodeViewModel FindNode(int id)
        {
            foreach (ModuleNodeViewModel node in Nodes)
            {
                if (node.Module.Id == id)
                    return node;
            }
            return null;
        }


        /// <summary>
        /// Export selected modules as a meta bloc
        /// Selected nodes must follow
        /// </summary>
        public void ExportAsMetaBloc()
        {
            if (rootnode != null)   // Start from root
            {
                List<ModuleNodeViewModel> Metabloc = new List<ModuleNodeViewModel>();
                MetaBlocState MetablocState = MetaBlocState.Empty;

                // Search for nodes that not have a parent or have Root as parent
                // For each branch search for selected nodes

                List<NodeViewModel> listBranches = GraphVM.GetChilds(rootnode);
                listBranches.AddRange(GraphVM.GetOrphans(rootnode));



                foreach (ModuleNodeViewModel node in listBranches)
                {
                    GetMetaBlock(node, ref Metabloc, ref MetablocState);
                    // Now, rule on the state of the metabloc
                    if (MetablocState == MetaBlocState.Failed)
                    {
                        break;      // non conforming
                    }
                    else
                    if (MetablocState == MetaBlocState.InProgress)
                    {
                        MetablocState = MetaBlocState.Completed;
                    }
                }
                if (MetablocState == MetaBlocState.Failed)
                {
                    MessageBox.Show(Properties.Resources.MetaBlocIncorrect);
                }
                else
                if (Metabloc.Count == 0)
                {
                    MessageBox.Show("No node selected !");
                }
                else
                if (MetablocState == MetaBlocState.Completed)
                {
                    SaveMetaBlock(Metabloc);  // Now, we have to save the metabloc
                }
            }
        }

        /// <summary>
        /// Get MetaBlock
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="Metablock"></param>
        /// <param name="MetablockState"></param>
        private void GetMetaBlock(ModuleNodeViewModel parentNode, ref List<ModuleNodeViewModel> Metablock, ref MetaBlocState MetablockState)
        {

            if (parentNode.IsSelected)
            {

                // tests the validity of this node
                if ((GraphVM.GetChilds(parentNode).Count > 1) || (GraphVM.GetParents(parentNode).Count > 1))
                {
                    MetablockState = MetaBlocState.Failed;  // more than one child or one parent -> failed
                    return;
                }

                if (Metablock.Count == 0)
                    MetablockState = MetaBlocState.InProgress;

                if (MetablockState == MetaBlocState.InProgress)
                {
                    Metablock.Add(parentNode);
                }
                else // Metablock is Completed . 
                {
                    // test if parentNode is already in Metablock
                    if (parentNode == Metablock.Find(x => x.Data == parentNode.Data))
                    {
                        // this  node was already in Metablok
                        // No need to continue in this branch
                        return;
                    }
                    MetablockState = MetaBlocState.Failed;
                    return;
                }
            }
            else
            {
                if (MetablockState == MetaBlocState.InProgress)
                {
                    MetablockState = MetaBlocState.Completed;
                }
            }

            foreach (ModuleNodeViewModel node in GraphVM.GetChilds(parentNode))
            {
                GetMetaBlock(node, ref Metablock, ref MetablockState);
                if (MetablockState == MetaBlocState.Failed)
                {
                    break;
                }
            }
        }

        private bool SaveMetaBlock(List<ModuleNodeViewModel> Metablock)
        {
            SaveFileDialog saveFileDlg = new SaveFileDialog();
            saveFileDlg.Filter = "Recipe files (*.adcmtb)|*.adcmtb";
            saveFileDlg.InitialDirectory = ConfigurationManager.AppSettings["Editor.MetablockFolder"];

            if (saveFileDlg.ShowDialog() == true)
            {
                SaveMetaBlock(Metablock, saveFileDlg.FileName);
                return true;
            }
            return false;
        }

        public void SaveMetaBlock(List<ModuleNodeViewModel> metablock, String fileName)
        {
            try
            {

                ServiceRecipe.Instance().SaveMetaBlock(metablock, fileName);
            }
            catch (Exception ex)
            {
                string msg = "Failed to save \"" + fileName + "\"";
                ExceptionMessageBox.Show(msg, ex);
            }
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
                feedbackIndicator = new View.Graph.ConnectionBadIndicator();
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
                    feedbackIndicator = new View.Graph.ConnectionOkIndicator();
                }
                else
                {
                    //
                    // Connectors with the same connector type (eg input & input, or output & output)
                    // can't be connected.
                    // Only connectors with separate connector type (eg input & output).
                    // Provide feedback to indicate that this connection is not valid!
                    //
                    feedbackIndicator = new View.Graph.ConnectionBadIndicator();
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
            ModuleNodeViewModel parentNode, childNode;
            if (newConnection.DestConnector == null)
            {
                parentNode = (ModuleNodeViewModel)connectorDraggedOut.ParentNode;
                childNode = (ModuleNodeViewModel)connectorDraggedOver.ParentNode;
            }
            else
            {
                parentNode = (ModuleNodeViewModel)connectorDraggedOver.ParentNode;
                childNode = (ModuleNodeViewModel)connectorDraggedOut.ParentNode;
            }

            //
            // Verifie la compatibilité des modules
            //
            bool compatible = Recipe.CompatibilityManager.IsFactoryCompatibleWithParent(childNode.Module.Factory, parentNode.Module);
            if (!compatible)
            {
                _graphVM.Connections.Remove(newConnection);
                AttentionMessageBox.Show("Modules are not compatible.");
                return;
            }

            ModuleBase childModule = childNode.Module;
            if (childModule.Factory.AcceptMultipleParents == false && childModule.Parents.Count > 0 && !(childModule.Parents[0] is RootModule))
            {
                _graphVM.Connections.Remove(newConnection);
                AttentionMessageBox.Show("Destination module \"" + childModule + "\" has already a parent.");
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
            ServiceRecipe.Instance().ConnectModules(parentNode.Module, childNode.Module);
            AlignSubTree(parentNode);
            SelectedNode = null;

        }


        /// <summary>
        /// Called when the user has finished dragging out the node.
        /// </summary>
        public void NodeDragCompletedEvent()
        {
            ServiceRecipe.Instance().MustBeSaved = true;
        }
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

        ///////////////////////////////////////////////////////////////////
        #region Repositionnement des noeuds
        ///////////////////////////////////////////////////////////////////

        // TODO FDE ce dico devrait être global ou mieux dans le _graphVM
        /// <summary> Dico pour retrouver facilement un noeud à partir de l'ID du module </summary>
        private Dictionary<int, ModuleNodeViewModel> nodes = new Dictionary<int, ModuleNodeViewModel>();

        /// <summary> Liste des modules du sous-arbre (i.e. les descendants du module racine) </summary>
        private HashSet<ModuleBase> subtreeNodes;
        private HashSet<ModuleBase> otherNodes;

        /// <summary> Position en Y du 1er noeud de chaque stage </summary>
        private Dictionary<int, double> stageY = new Dictionary<int, double>();

        ///================================================================
        /// <summary>
        /// Aligne les noeuds à partir du noeud sélectionné.
        /// </summary>
        ///================================================================
        public void AligneNodes()
        {
            if (_graphVM == null || _graphVM.Nodes.Count == 0)
                return;

            int countSelect = 0;

            foreach (ModuleNodeViewModel node in _graphVM.Nodes)
            {
                if (node.IsSelected)
                {
                    AlignSubTree(node);
                    countSelect++;
                }
            }

            if (countSelect > 0)
                ServiceRecipe.Instance().MustBeSaved = true;
            else
                MessageBox.Show("No node selected !");
        }

        //================================================================
        /// <summary>
        /// Recalcule la position des noeuds dans une sous-partie de 
        /// l'arbre.
        /// </summary>
        /// <param name="node"> noeud "racine" du sous-arbre </param>
        //================================================================
        public void AlignSubTree(ModuleNodeViewModel node)
        {
            //-------------------------------------------------------------
            // Il faut d'abord recalculer les stages
            //-------------------------------------------------------------
            ComputeStages();

            //-------------------------------------------------------------
            // Création des dicos
            //-------------------------------------------------------------
            nodes = new Dictionary<int, ModuleNodeViewModel>();
            stageY = new Dictionary<int, double>();

            subtreeNodes = node.Module.GetAllDescendants();
            otherNodes = new HashSet<ModuleBase>();
            foreach (ModuleNodeViewModel n in _graphVM.Nodes)
            {
                nodes[n.Module.Id] = n;
                if (n != node && !subtreeNodes.Contains(n.Module))
                    otherNodes.Add(n.Module);
            }

            //-------------------------------------------------------------
            // Calcul du Y des stages dans les autres branches
            //-------------------------------------------------------------
            int maxstage = -1;
            foreach (ModuleNodeViewModel n in nodes.Values)
            {
                if (!subtreeNodes.Contains(n.Module))
                {
                    double maxy;
                    bool exists = stageY.TryGetValue(n.Module.StageIndex + 1, out maxy);
                    if (!exists)
                        maxy = double.NegativeInfinity;

                    stageY[n.Module.StageIndex + 1] = Math.Max(n.Y + pas.Height, maxy);
                }
                maxstage = Math.Max(maxstage, n.Module.StageIndex + 1);
            }
            maxstage++;

            // Remplissage des stages qu'on n'a pas trouvé
            for (int i = 0; i < maxstage; i++)
            {
                if (!stageY.ContainsKey(i))
                {
                    if (i == 0)
                        stageY[i] = 0;
                    else
                        stageY[i] = stageY[i - 1];
                }
            }

            // On ignore le stage -1 dans le placement
            stageY[0] = double.NegativeInfinity;

            //-------------------------------------------------------------
            // Placement des noeuds
            //-------------------------------------------------------------
            for (int stage = node.Module.StageIndex; stage < maxstage; stage++)
                SetSubTreePosition(node, node.X, node.Y, stage);
        }

        //================================================================
        /// <summary>
        /// Définit la position d'un noeud et positionne récursivement ses descendants.
        /// L'algo ne s'intéresse qu'à un stage, il faut donc l'appeler plusieurs
        /// fois pour couvrir tous les stages.
        /// </summary>
        /// <param name="node"> racine du sous-arbre à positionner </param>
        /// <param name="x"> position en X du noeud racine </param>
        /// <param name="y"> position en Y du noeud racine </param>
        /// <param name="stage"> stage auquel on s'intéresse, les noeuds de stage supérieur ne sont pas traités. </param>
        //================================================================
        private void SetSubTreePosition(ModuleNodeViewModel node, double x, double y, int stage)
        {
            if (node.Module is TerminationModule)
                return;

            if (node.Module.StageIndex > stage)
            {
                // le module et ses descendants seront positionnés plus tard
                return;
            }

            //-------------------------------------------------------------
            // Position du module
            //-------------------------------------------------------------
            node.X = x;
            node.Y = Math.Max(stageY[node.Module.StageIndex], y);

            stageY[node.Module.StageIndex + 1] = Math.Max(stageY[node.Module.StageIndex + 1], node.Y + pas.Height);

            //-------------------------------------------------------------
            // Enfants
            //-------------------------------------------------------------
            ImpObservableCollection<NodeViewModel> listChildsNodes = node.GetOutNodesConnectedList();
            double totalwidth = 0;// (node.Module.Children.Count - 1) * pas.Width + nodeSize.Width;

            foreach (ModuleNodeViewModel child in listChildsNodes)
            {
                if (totalwidth > 0)
                    totalwidth += interNode.Width;
                totalwidth += child.Size.Width;
            }

            // Position du fils courant. 
            double xChild = node.X + node.Size.Width / 2 - totalwidth / 2;
            // Simulation du 1er fils pour détecter les collisions à gauche 
            Rect RectChild0 = new Rect(xChild, node.Y + pas.Height, nodeSize.Width, nodeSize.Height);

            if (listChildsNodes.Count > 0)
            {
                // Gérer les collisions à gauche
                foreach (ModuleNodeViewModel n in _graphVM.Nodes)
                {
                    if ((listChildsNodes.Contains(n) == false) && (n != node))
                    {
                        Rect nRect = n.NodeRect();
                        Rect IntersectNode = Rect.Intersect(nRect, RectChild0);
                        if (IntersectNode.IsEmpty == false) // Y-at'il une collision ?
                        {
                            if ((n.X <= RectChild0.X) && (IntersectNode.Width > 0))
                            {
                                RectChild0.X += IntersectNode.Width + interNode.Width;  // On se décale à droite // Todo : il faudrait décaler aussi les ascendants
                            }
                        }
                    }
                }
                if (xChild < RectChild0.X) // on a décalé à droite
                    node.X += RectChild0.X - xChild;
                xChild = RectChild0.X;
            }

            // Placement des fils
            for (int i = 0; i < node.Module.Children.Count; i++)
            {
                ModuleBase childmod = node.Module.Children[i];
                if (childmod is TerminationModule)
                    continue;

                ModuleNodeViewModel childnode = nodes[childmod.Id];
                if (childmod.Parents.Count == 1)
                {
                    x = xChild;
                    y = node.Y + pas.Height;
                    xChild += childnode.Size.Width + interNode.Width; // On incrémente xChild

                }
                else
                {
                    CenterModuleBelowItsParents(childnode, out x, out y);
                }
                SetSubTreePosition(childnode, x, y, stage);
            }
        }

        //================================================================
        /// <summary>
        /// Calcul du X/Y pour aligner un noeud sous ses parents. 
        /// Utile quand il en a plusieurs.
        /// NB: On fait le calcul mais on ne déplace pas le noeud.
        /// </summary>
        //================================================================
        private void CenterModuleBelowItsParents(ModuleNodeViewModel node, out double x, out double y)
        {
            double xmin = double.PositiveInfinity;
            double xmax = double.NegativeInfinity;
            double ymax = double.NegativeInfinity;

            foreach (ModuleBase parentmod in node.Module.Parents)
            {
                ModuleNodeViewModel parentnode = nodes[parentmod.Id];
                xmin = Math.Min(xmin, parentnode.X);
                xmax = Math.Max(xmax, parentnode.X);
                ymax = Math.Max(ymax, parentnode.Y);
            }

            x = (xmin + xmax) / 2;
            y = ymax + pas.Height;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////
        #region Calcul des stages
        ///////////////////////////////////////////////////////////////////

        ///================================================================
        /// <summary>
        /// Calcul du numéro de "stage" de chaque module
        /// </summary>        
        ///================================================================
        public void ComputeStages()
        {
            // Reset des index
            //................
            foreach (ModuleBase module in Recipe.ModuleList.Values)
                module.StageIndex = -1;

            // Calcul récursif en remontant à partir du module Terminaison 
            //............................................................
            ComputeStage(Recipe.Root);

            // Mise à jour des noeuds
            //.......................
            foreach (ModuleNodeViewModel n in Nodes)
            {
                if (n.Module is UnknownModule)
                    n.BackgroundColorIndex = -2;
                else
                    n.BackgroundColorIndex = n.Module.StageIndex;
            }
        }

        private void ComputeStage(ModuleBase module)
        {
            if (module.Parents.Count == 0)
            {
                module.StageIndex = 0;      // Racine
            }
            else
            {
                int stage = module.Parents.Max(p => p.StageIndex);
                if (module.ModuleProperty == eModuleProperty.Stage)
                    stage++;

                if (stage == module.StageIndex) // Déjà calculé
                    return;
                module.StageIndex = stage;
            }

            foreach (ModuleBase child in module.Children)
                ComputeStage(child);
        }
        #endregion

        /// <summary>
        /// Utility method to delete a connection from the view-model.
        /// </summary>
        public void DeleteConnection(ConnectionViewModel connection)
        {
            ModuleNodeViewModel parentNode = (ModuleNodeViewModel)connection.SourceConnector.ParentNode;
            ModuleNodeViewModel childNode = (ModuleNodeViewModel)connection.DestConnector.ParentNode;
            ServiceRecipe.Instance().DisconnectModules(parentNode.Module, childNode.Module);
            _graphVM.Connections.Remove(connection);
        }


        /// <summary>
        /// Retourne la liste des ascendants de node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<ModuleNodeViewModel> GetNodeAscendants(ModuleNodeViewModel node)
        {
            if (node == null)
                return null;

            // Take a copy of the selected nodes list so we can delete nodes while iterating.
            var ascendants = new List<ModuleNodeViewModel>();
            var nodes = new List<NodeViewModel>();
            var parents = new List<NodeViewModel>();

            parents = GraphVM.GetParents(node);
            if (parents.Count > 0)
            {
                nodes.AddRange(parents);
                foreach (ModuleNodeViewModel parent in parents)
                    nodes.AddRange(GetNodeAscendants(parent));

                foreach (ModuleNodeViewModel ascendant in nodes)
                {
                    if (ascendants.Contains(ascendant) == false)
                        ascendants.Add(ascendant);
                }
            }
            return ascendants;
        }


        public void ClearGraph()
        {
            _graphVM.Nodes.Clear();
            _graphVM.Connections.Clear();
            rootnode = null;
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
                MessageBox.Show("No node selected !");
                return;
            }

            while (nodes.Count > 0)
            {
                ModuleNodeViewModel node = (ModuleNodeViewModel)nodes.Last();
                nodes.Remove(node);
                DeleteNode(ref node);
            }
        }

        /// <summary>
        /// Delete the node from the view-model.
        /// Also deletes any connections to or from the node.
        /// </summary>
        public void DeleteNode(ref ModuleNodeViewModel node)
        {
            if (node != null)
            {
                // Don't delete Root !
                if (node.Module is RootModule)
                    return;
                //
                // Remove all connections attached to the node.
                //
                _graphVM.Connections.RemoveRange(node.AttachedConnections);

                //
                // Remove the node from the graph.
                //
                _graphVM.Nodes.Remove(node);
                ServiceRecipe.Instance().RemoveModule(node.Module);
                node = null;
            }
        }

        //=================================================================
        /// <summary>
        /// Ajoute un nouveau noeud au ViewModel.
        /// </summary>
        //=================================================================
        public void AddChild(ModuleNodeViewModel parentNode, ModuleNodeViewModel childNode)
        {
            // Ajout dans la ServiceRecipe
            //............................
            ServiceRecipe.Instance().AddChild(parentNode.Module, childNode.Module);

            // Ajoute dans le graphe
            //......................
            _graphVM.ConnecteNode(parentNode, childNode);
            _graphVM.Nodes.Add(childNode);
        }

        //=================================================================
        /// <summary>
        /// Création d'une nouvelle branche en ajoutant un métabloc. 
        /// </summary>
        /// <returns>
        /// La liste des noeuds de la nouvelle branche. 
        /// </returns>
        //=================================================================
        public List<ModuleNodeViewModel> AddMetablock(ModuleNodeViewModel parentNode, List<ModuleBase> metablock)
        {
            List<ModuleNodeViewModel> newNodeList = new List<ModuleNodeViewModel>();

            ModuleNodeViewModel previousnode = parentNode;
            foreach (ModuleBase module in metablock)
            {
                // Ajout à la SericeRecipe
                module.Id = Recipe.GetNewId();

                // Ajout au ViewModel
                ModuleNodeViewModel newnode = new ModuleNodeViewModel(module);
                AddChild(previousnode, newnode);
                newnode.Size = nodeSize;
                previousnode = newnode;
                newNodeList.Add(previousnode);
            }

            return newNodeList;
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
        public List<ModuleNodeViewModel> CreateNewBranch()
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
        public List<ModuleNodeViewModel> AddModules(bool inNewBranch)
        {
            ModuleNodeViewModel parentNode = SelectedNode;
            if (parentNode == null)
            {
                MessageBox.Show("No node selected !");
                return null;
            }

            if (parentNode.Module.Factory.DataProducer == DataProducerType.NoData)
            {
                MessageBox.Show("Can't add children to this module because it doesn't produce any data.");
                return null;
            }

            // Boîte de dialogue pour sélectionner le module
            //..............................................
            bool checkChildren;
            if (inNewBranch)
            {
                checkChildren = false;
            }
            else
            {
                if (SelectedNode.Module.Children.Count == 0)
                    checkChildren = false;
                else if ((SelectedNode.Module.Children.Count == 1) && (SelectedNode.Module.Children[0] == Recipe.Termination))
                    checkChildren = false;
                else
                    checkChildren = true;
            }
            bool allowMetaBlock = !checkChildren;

            var viewmodel = new SelectModuleViewModel(parentNode.Module, checkChildren, allowMetaBlock);
            var dialog = new SelectModuleDialog(viewmodel);

            if (dialog.ShowDialog() != true)
                return null;

            // Ajout d'un meta-bloc
            //.....................
            List<ModuleNodeViewModel> newNodeList = new List<ModuleNodeViewModel>();

            bool isMetablock = (dialog.SelectedModuleFactory.GetType().Name == typeof(SelectModuleViewModel.MetablocInfo).Name);
            if (isMetablock)
            {
                List<ModuleBase> metablock = ((SelectModuleViewModel.MetablocInfo)dialog.SelectedModuleFactory).metablockList.OfType<ModuleBase>().ToList();
                newNodeList = AddMetablock(parentNode, metablock); // toujours dans une nouvelle branche
            }
            // Ou ajout d'un module simple
            //............................
            else
            {
                string moduleName = ((IModuleFactory)(dialog.SelectedModuleFactory)).ModuleName;
                ModuleNodeViewModel childNode = CreateNode(moduleName);
                if (childNode != null)
                {
                    newNodeList.Add(childNode);

                    if (inNewBranch)
                        AddChild(parentNode, childNode);
                    else
                        InsertChild(parentNode, childNode);
                }
            }
            if (newNodeList.Count > 0)
            {
                AlignSubTree(parentNode);
                SelectedNode = newNodeList.First();
            }
            return newNodeList;
        }

        //=================================================================
        /// <summary>
        /// Insère un noeud entre un parent et ses fils.
        /// Version sans IHM.
        /// </summary>
        //=================================================================
        public ModuleNodeViewModel InsertChild(ModuleNodeViewModel parentNode, ModuleNodeViewModel newNode)
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
            foreach (ModuleNodeViewModel childNode in childNodes)
            {
                _graphVM.ConnecteNode(newNode, childNode);
                ServiceRecipe.Instance().ConnectModules(newNode.Module, childNode.Module);
            }

            //-------------------------------------------------------------
            // Repositionne les noeuds
            //-------------------------------------------------------------
            AlignSubTree(parentNode);
            SelectedNode = newNode;

            return newNode;
        }

        //=================================================================
        /// <summary>
        /// Crée un nouveau noeud entre un parent et ses fils.
        /// Version sans IHM.
        /// </summary>
        //=================================================================
        public ModuleNodeViewModel InsertChild(ModuleNodeViewModel parentNode, string moduleName)
        {
            ModuleNodeViewModel newnode = CreateNode(moduleName);
            InsertChild(parentNode, newnode);

            return newnode;
        }

        //=================================================================
        /// <summary>
        /// Crée un noeud entre un parent et ses fils.
        /// Version avec IHM pour sélectionner le modules.
        /// </summary>
        //=================================================================
        public ModuleNodeViewModel InsertChild()
        {
            List<ModuleNodeViewModel> newnodes = AddModules(inNewBranch: false);

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

        //=================================================================
        //
        //=================================================================
        public ModuleNodeViewModel CreateNode(string moduleName)
        {
            ModuleBase module = Recipe.CreateModule(moduleName);
            ModuleNodeViewModel node = null;

            if (module != null)
            {
                node = new ModuleNodeViewModel(module);
                node.Size = nodeSize;
                ServiceRecipe.Instance().MustBeSaved = true;
            }

            return node;
        }

        /// <summary>
        /// A function to conveniently populate the view-model with test data.
        /// </summary>
        public ModuleNodeViewModel CreateRootNode()
        {
            rootnode = new ModuleNodeViewModel(Recipe.Root);
            rootnode.X = defaultRootPosition.X;
            rootnode.Y = defaultRootPosition.Y;
            rootnode.Size = nodeSize;
            _graphVM.Nodes.Add(rootnode);

            return rootnode;
        }


        #region commands

        private AutoRelayCommand _manageExportsCommand = null;
        public AutoRelayCommand ManageExportsCommand
        {
            get
            {
                return _manageExportsCommand ?? (_manageExportsCommand = new AutoRelayCommand(
              () => { ClassLocator.Default.GetInstance<IMessenger>().Send(new Messages.SelectVisibleParameters()); },
              () => { return GraphHasSingleSelection; }));
            }
        }

        private AutoRelayCommand _exportAsMetaBlocCommand = null;
        public AutoRelayCommand ExportAsMetaBlocCommand
        {
            get
            {
                return _exportAsMetaBlocCommand ?? (_exportAsMetaBlocCommand = new AutoRelayCommand(
                    () => { ExportAsMetaBloc(); },
                    () => { return IsGraphHasSelectedNodes && IsEditable; }));
            }
        }

        private AutoRelayCommand _copyNodeCommand = null;
        public AutoRelayCommand CopyNodeCommand
        {
            get
            {
                return _copyNodeCommand ?? (_copyNodeCommand = new AutoRelayCommand(
              () => { UndoRedocommandManager.ExecuteCmd(new CopyNodesCommand(this)); },
              () => { return IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _pastNodeCommand = null;
        public AutoRelayCommand PastNodeCommand
        {
            get
            {
                return _pastNodeCommand ?? (_pastNodeCommand = new AutoRelayCommand(
              () => { UndoRedocommandManager.ExecuteCmd(new PasteNodesCommand(this)); },
                () => { return UndoRedo.AdcClipBoard.Instance().IsEmpty() == false; }));
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
                  UndoRedocommandManager.ExecuteCmd(new CopyNodesCommand(this));
                  UndoRedocommandManager.ExecuteCmd(new DeleteNodesCommand(this));
              },
              () => { return IsEditable && IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _deleteNodeCommand = null;
        public AutoRelayCommand DeleteNodeCommand
        {
            get
            {
                return _deleteNodeCommand ?? (_deleteNodeCommand = new AutoRelayCommand(
              () => { UndoRedocommandManager.ExecuteCmd(new DeleteNodesCommand(this)); },
              () => { return IsEditable && IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand _addChildNodeCommand = null;
        public AutoRelayCommand AddChildNodeCommand
        {
            get
            {
                return _addChildNodeCommand ?? (_addChildNodeCommand = new AutoRelayCommand(
              () => { UndoRedocommandManager.ExecuteCmd(new AddChildCommand(this)); },
              () => { return IsEditable && GraphHasSingleSelection; }));
            }
        }

        private AutoRelayCommand _insertChildNodeCommand = null;
        public AutoRelayCommand InsertChildNodeCommand
        {
            get
            {
                return _insertChildNodeCommand ?? (_insertChildNodeCommand = new AutoRelayCommand(
              () => { UndoRedocommandManager.ExecuteCmd(new InsertChildCommand(this)); },
              () => { return IsEditable && GraphHasSingleSelection; }));
            }
        }

        private AutoRelayCommand _alignNodesCommand = null;
        public AutoRelayCommand AlignNodesCommand
        {
            get
            {
                return _alignNodesCommand ?? (_alignNodesCommand = new AutoRelayCommand(
              () => { AligneNodes(); },
              () => { return IsEditable && IsGraphHasSelectedNodes; }));
            }
        }

        private AutoRelayCommand<ConnectionViewModel> _deleteConnectionCommand = null;
        public AutoRelayCommand<ConnectionViewModel> DeleteConnectionCommand
        {
            get
            {
                return _deleteConnectionCommand ?? (_deleteConnectionCommand = new AutoRelayCommand<GraphModel.ConnectionViewModel>(
              (e) =>
              {
                  DeleteConnection(e);
              },
             (e) => { return IsEditable; }));
            }
        }

        private void UpdateCommandState()
        {
            ManageExportsCommand.NotifyCanExecuteChanged();
            CopyNodeCommand.NotifyCanExecuteChanged();
            PastNodeCommand.NotifyCanExecuteChanged();
            CutNodeCommand.NotifyCanExecuteChanged();
            DeleteNodeCommand.NotifyCanExecuteChanged();
            AddChildNodeCommand.NotifyCanExecuteChanged();
            InsertChildNodeCommand.NotifyCanExecuteChanged();
            AlignNodesCommand.NotifyCanExecuteChanged();
            DeleteConnectionCommand.NotifyCanExecuteChanged();
            ExportAsMetaBlocCommand.NotifyCanExecuteChanged();
        }

        #endregion
    }
}
