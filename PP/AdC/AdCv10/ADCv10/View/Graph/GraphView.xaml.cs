using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ADC.UndoRedo;
using ADC.UndoRedo.Command;
using ADC.ViewModel;
using ADC.ViewModel.Graph;

using GraphModel;

using GraphUI;

namespace ADC.View.Graph
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class GraphView : UserControl
    {
        #region DependencyProperty

        /// <summary>
        /// True if the node information text is visible
        /// </summary>
        public bool NodeInfoIsVisible
        {
            get { return (bool)GetValue(NodeInfoIsVisibleProperty); }
            set { SetValue(NodeInfoIsVisibleProperty, value); }
        }

        // Default dependency value : true
        public static readonly DependencyProperty NodeInfoIsVisibleProperty =
            DependencyProperty.Register("NodeInfoIsVisible", typeof(bool), typeof(GraphView), new UIPropertyMetadata(true));


        /// <summary>
        /// True if the node progress information text is visible
        /// </summary>
        public bool NodeProgessInfoIsVisible
        {
            get { return (bool)GetValue(NodeProgessInfoIsVisibleProperty); }
            set { SetValue(NodeProgessInfoIsVisibleProperty, value); }
        }

        // Default dependency value : false
        public static readonly DependencyProperty NodeProgessInfoIsVisibleProperty =
            DependencyProperty.Register("NodeProgessInfoIsVisible", typeof(bool), typeof(GraphView), new UIPropertyMetadata(false));


        /// <summary>
        /// True if the graph is editable
        /// </summary>
        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Default dependency value : true
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(GraphView), new UIPropertyMetadata(true));

        /// <summary>
        /// Couleur de fond pour le graphe lui-même, (ie il ne s'applique pas aux scrollbares, aux bordures, etc...)
        /// </summary>
        public Brush GraphBackground
        {
            get { return (Brush)GetValue(CanvasBackgroundProperty); }
            set { SetValue(CanvasBackgroundProperty, value); }
        }

        public static readonly DependencyProperty CanvasBackgroundProperty =
            DependencyProperty.Register("GraphBackground", typeof(Brush), typeof(GraphView), new UIPropertyMetadata(Brushes.Transparent));


        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set
            {
                SetValue(ScaleProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Scale.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(GraphView), new PropertyMetadata(1.0));




        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(GraphView), new PropertyMetadata(560.0));



        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OffsetY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(GraphView), new PropertyMetadata(0.0));



        /// <summary>
        /// Rectangle visible, utilisé poour assurer que ce rectangle est bien visible
        /// </summary>
        public Rect VisibleRect
        {
            get { return (Rect)GetValue(VisibleRectProperty); }
            set { SetValue(VisibleRectProperty, value); }
        }

        public static readonly DependencyProperty VisibleRectProperty =
            DependencyProperty.Register("VisibleRect", typeof(Rect), typeof(GraphView), new UIPropertyMetadata(Rect.Empty, OnVisibleRectChanged));

        private static void OnVisibleRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GraphView graphview = d as GraphView;
            graphview.OnVisibleRectChanged(e);
        }
        #endregion

        static public UndoRedoManager undoRedocommandManager = new UndoRedoManager();

        public UndoRedoManager UndoRedocommandManager
        {
            get { return undoRedocommandManager; }
        }

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public RecipeGraphViewModel ViewModel
        {
            get
            {
                return (RecipeGraphViewModel)DataContext;
            }
        }

        public GraphView()
        {
            InitializeComponent();
        }

        #region GraphControl
        /// <summary>
        /// Event raised when the user has started to drag out a connection.
        /// </summary>
        private void graphControl_ConnectionDragStarted(object sender, ConnectionDragStartedEventArgs e)
        {
            if (!IsEditable)
                return;

            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var curDragPoint = Mouse.GetPosition(graphControl);

            //
            // Delegate the real work to the view model.
            //
            var connection = ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

            //
            // Must return the view-model object that represents the connection via the event args.
            // This is so that GraphView can keep track of the object while it is being dragged.
            //
            e.Connection = connection;
        }

        /// <summary>
        /// Event raised, to query for feedback, while the user is dragging a connection.
        /// </summary>
        private void graphControl_QueryConnectionFeedback(object sender, QueryConnectionFeedbackEventArgs e)
        {
            var draggedOutConnector = (ConnectorViewModel)e.ConnectorDraggedOut;
            var draggedOverConnector = (ConnectorViewModel)e.DraggedOverConnector;
            object feedbackIndicator = null;
            bool connectionOk = true;

            ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

            //
            // Return the feedback object to GraphView.
            // The object combined with the data-template for it will be used to create a 'feedback icon' to
            // display (in an adorner) to the user.
            //
            e.FeedbackIndicator = feedbackIndicator;

            //
            // Let GraphView know if the connection is ok or not ok.
            //
            e.ConnectionOk = connectionOk;
        }

        /// <summary>
        /// Event raised while the user is dragging a connection.
        /// </summary>
        private void graphControl_ConnectionDragging(object sender, ConnectionDraggingEventArgs e)
        {
            Point curDragPoint = Mouse.GetPosition(graphControl);
            var connection = (ConnectionViewModel)e.Connection;
            ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void graphControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a node.
        /// </summary>
        private void graphControl_NodeDragCompletedEvent(object sender, NodeDragCompletedEventArgs e)
        {
            ViewModel.NodeDragCompletedEvent();
        }


        /// <summary>
        /// Event raised when the user has started dragging out a node.
        /// </summary>
        private void graphControl_NodeDragStartedEvent(object sender, NodeDragStartedEventArgs e)
        {
            undoRedocommandManager.ExecuteCmd(new NodesDragCommand(ViewModel));
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void graphControl_NodeLeftClic(object sender, NodeLeftClicEventArgs e)
        {
        }


        private void graphControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                if (graphControl.SelectedNodes.Count == 0)
                    ViewModel.SelectedNode = null;
                else if (graphControl.SelectedNodes.Count == 1)
                    ViewModel.SelectedNode = (ModuleNodeViewModel)graphControl.SelectedNode;
            }
        }


        /// <summary>
        /// Event raised when the size of a node has changed.
        /// </summary>
        private void Node_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //
            // The size of a node, as determined in the UI by the node's data-template,
            // has changed.  Push the size of the node through to the view-model.
            //
            var element = (FrameworkElement)sender;
            var node = (ModuleNodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }


        #endregion GraphControl
    }
}
