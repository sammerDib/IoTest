using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Graph.Utils;

namespace UnitySC.Shared.UI.Graph.Model
{
    /// <summary>
    /// Defines a node in the view-model.
    /// Nodes are connected to other nodes through attached connectors (aka anchor/connection points).
    /// </summary>
    [Serializable]
    public class NodeViewModel : ObservableObject
    {
        #region Private Data Members

        /// <summary>
        /// The name of the node.
        /// </summary>
        private string name = string.Empty;

        /// <summary>
        /// The info display in the node.
        /// </summary>
        private string info = string.Empty;

        private Color _fillColor;

        private GradientStop colorStop = new GradientStop(Color.FromRgb(126, 0, 255), 0.6);

        /// <summary>
        /// The associated data object
        /// </summary>
        private object data;

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        private double x = 0;

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        private double y = 0;

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        private int zIndex = 0;

        /// <summary>
        /// The size of the node.
        ///
        /// Important Note:
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        private Size size = Size.Empty;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> inputConnectors = null;

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        private ImpObservableCollection<ConnectorViewModel> outputConnectors = null;

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        private bool isSelected = false;

        #endregion Private Data Members

        public NodeViewModel()
        {
            InputConnectors.Add(new ConnectorViewModel(""));
            OutputConnectors.Add(new ConnectorViewModel(""));
        }

        public NodeViewModel(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// The name of the node.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The info display in the node.
        /// </summary>
        public string Info
        {
            get
            {
                return info;
            }
            set
            {
                if (info == value)
                {
                    return;
                }

                info = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The info display in the node.
        /// </summary>
        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                if (_fillColor == value)
                {
                    return;
                }

                _fillColor = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The info display in the node.
        /// </summary>
        public GradientStop ColorStop
        {
            get
            {
                return colorStop;
            }
            set
            {
                if (colorStop == value)
                {
                    return;
                }

                colorStop = value;

                OnPropertyChanged();
            }
        }

        public virtual object Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        /// <summary>
        /// The X coordinate for the position of the node.
        /// </summary>
        public double X
        {
            get
            {
                return x;
            }
            set
            {
                if (x == value)
                {
                    return;
                }

                x = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Y coordinate for the position of the node.
        /// </summary>
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                if (y == value)
                {
                    return;
                }

                y = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return zIndex;
            }
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// The size of the node.
        ///
        /// Important Note:
        ///     The size of a node in the UI is not determined by this property!!
        ///     Instead the size of a node in the UI is determined by the data-template for the Node class.
        ///     When the size is computed via the UI it is then pushed into the view-model
        ///     so that our application code has access to the size of a node.
        /// </summary>
        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                if (size == value)
                {
                    return;
                }

                size = value;

                if (SizeChanged != null)
                {
                    SizeChanged(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The rectange of the node.
        ///
        ///
        /// </summary>
        public Rect NodeRect()
        {
            return new Rect(X, Y, Size.Width, Size.Height);
        }

        /// <summary>
        /// Event raised when the size of the node is changed.
        /// The size will change when the UI has determined its size based on the contents
        /// of the nodes data-template.  It then pushes the size through to the view-model
        /// and this 'SizeChanged' event occurs.
        /// </summary>
        public event EventHandler<EventArgs> SizeChanged;

        /// <summary>
        /// List of input connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> InputConnectors
        {
            get
            {
                if (inputConnectors == null)
                {
                    inputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    inputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsAdded);
                    inputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(inputConnectors_ItemsRemoved);
                }

                return inputConnectors;
            }
            set
            {
                if (inputConnectors == value)
                {
                    return;
                }

                inputConnectors = value;
            }
        }

        /// <summary>
        /// List of output connectors (connections points) attached to the node.
        /// </summary>
        public ImpObservableCollection<ConnectorViewModel> OutputConnectors
        {
            get
            {
                if (outputConnectors == null)
                {
                    outputConnectors = new ImpObservableCollection<ConnectorViewModel>();
                    outputConnectors.ItemsAdded += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsAdded);
                    outputConnectors.ItemsRemoved += new EventHandler<CollectionItemsChangedEventArgs>(outputConnectors_ItemsRemoved);
                }

                return outputConnectors;
            }
            set
            {
                if (outputConnectors == value)
                {
                    return;
                }

                outputConnectors = value;
            }
        }

        /// <summary>
        /// A helper property that retrieves a list (a new list each time) of all connections attached to the node.
        /// </summary>
        public ICollection<ConnectionViewModel> AttachedConnections
        {
            get
            {
                var attachedConnections = new List<ConnectionViewModel>();

                foreach (var connector in this.InputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                foreach (var connector in this.OutputConnectors)
                {
                    attachedConnections.AddRange(connector.AttachedConnections);
                }

                return attachedConnections;
            }
        }

        /// <summary>
        /// Get the number of childs in the nodeview
        /// </summary>
        /// <returns></returns>
        public int GetOutNodesConnectedCount()
        {
            return outputConnectors[0].AttachedConnections.Count;
        }

        /// <summary>
        /// Get the number of parents in the nodeview
        /// </summary>
        /// <returns></returns>
        public int GetInNodesConnectedCount()
        {
            return inputConnectors[0].AttachedConnections.Count;
        }

        /// <summary>
        /// Set to 'true' when the node is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                if (isSelected == value)
                {
                    return;
                }

                isSelected = value;

                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Return the collection of nodes connected by out connectors
        /// </summary>
        /// <returns></returns>
        public ImpObservableCollection<NodeViewModel> GetOutNodesConnectedList()
        {
            var listOutNodes = new ImpObservableCollection<NodeViewModel>();

            foreach (var connector in outputConnectors)
            {
                foreach (var connexion in connector.AttachedConnections)
                {
                    if (connexion.DestConnector != null)
                    {
                        if (connexion.DestConnector.ParentNode != null)
                        {
                            listOutNodes.Add(connexion.DestConnector.ParentNode);
                        }
                    }
                }
            }
            return listOutNodes;
        }

        /// <summary>
        /// Return the collection of nodes connected by in connectors
        /// </summary>
        /// <returns></returns>
        public ImpObservableCollection<NodeViewModel> GetInNodesConnectedList()
        {
            var listInNodes = new ImpObservableCollection<NodeViewModel>();

            foreach (var connector in inputConnectors)
            {
                foreach (var connexion in connector.AttachedConnections)
                {
                    if (connexion.SourceConnector != null)
                    {
                        if (connexion.SourceConnector.ParentNode != null)
                        {
                            listInNodes.Add(connexion.SourceConnector.ParentNode);
                        }
                    }
                }
            }
            return listInNodes;
        }

        #region Private Methods

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void inputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Input;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void inputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
            }
        }

        /// <summary>
        /// Event raised when connectors are added to the node.
        /// </summary>
        private void outputConnectors_ItemsAdded(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = this;
                connector.Type = ConnectorType.Output;
            }
        }

        /// <summary>
        /// Event raised when connectors are removed from the node.
        /// </summary>
        private void outputConnectors_ItemsRemoved(object sender, CollectionItemsChangedEventArgs e)
        {
            foreach (ConnectorViewModel connector in e.Items)
            {
                connector.ParentNode = null;
                connector.Type = ConnectorType.Undefined;
            }
        }

        #endregion Private Methods
    }
}