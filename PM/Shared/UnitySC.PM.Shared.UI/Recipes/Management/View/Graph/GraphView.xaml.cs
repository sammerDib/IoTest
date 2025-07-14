using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.Graph;
using UnitySC.Shared.UI.Graph.Model;
using UnitySC.Shared.UI.Graph.UI;
using UnitySC.Shared.UI.Graph.Utils.ZoomAndPan;

namespace UnitySC.PM.Shared.UI.Recipes.Management.View.Graph
{
    /// <summary>
    /// Interaction logic for GraphView.xaml
    /// </summary>
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
            DependencyProperty.Register("OffsetX", typeof(double), typeof(GraphView), new PropertyMetadata(0.0));


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

        private bool _visibleRectinternalChangeDone = false;
        private static void OnVisibleRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
             GraphView graphview = d as GraphView;
             graphview.OnVisibleRectChanged(e);   
        }
        #endregion

        //static public UndoRedoManager undoRedocommandManager = new UndoRedoManager();

        //public UndoRedoManager UndoRedocommandManager
        //{
        //    get { return undoRedocommandManager; }
        //}

        /// <summary>
        /// Convenient accessor for the view-model.
        /// </summary>
        public DataflowGraphViewModel ViewModel
        {
            get
            {
                return (DataflowGraphViewModel)DataContext;
            }
        }

        public GraphView()
        {
            InitializeComponent();

            DataContextChanged += GraphView_DataContextChanged;
            Loaded += GraphView_Loaded; 
        }

        private void GraphView_Loaded(object sender, RoutedEventArgs e)
        {
           // FitContent_Executed(this, null);
        }

        private void GraphView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((DataflowGraphViewModel)e.OldValue).FitContent -= FitContent_Executed;
            if (e.NewValue != null)
            {
                ((DataflowGraphViewModel)e.NewValue).FitContent += FitContent_Executed;
            }
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
            var connection = this.ViewModel.ConnectionDragStarted(draggedOutConnector, curDragPoint);

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

            this.ViewModel.QueryConnnectionFeedback(draggedOutConnector, draggedOverConnector, out feedbackIndicator, out connectionOk);

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
            this.ViewModel.ConnectionDragging(curDragPoint, connection);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a connection.
        /// </summary>
        private void graphControl_ConnectionDragCompleted(object sender, ConnectionDragCompletedEventArgs e)
        {
            var connectorDraggedOut = (ConnectorViewModel)e.ConnectorDraggedOut;
            var connectorDraggedOver = (ConnectorViewModel)e.ConnectorDraggedOver;
            var newConnection = (ConnectionViewModel)e.Connection;
            this.ViewModel.ConnectionDragCompleted(newConnection, connectorDraggedOut, connectorDraggedOver);
        }

        /// <summary>
        /// Event raised when the user has finished dragging out a node.
        /// </summary>
        private void graphControl_NodeDragCompletedEvent(object sender, NodeDragCompletedEventArgs e)
        {
            this.ViewModel.NodeDragCompletedEvent();
        }


        /// <summary>
        /// Event raised when the user has started dragging out a node.
        /// </summary>
        private void graphControl_NodeDragStartedEvent(object sender, NodeDragStartedEventArgs e)
        {
           // UndoRedocommandManager.ExecuteCmd(new NodesDragCommand(ViewModel));
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
                    ViewModel.SelectedNode = (DataflowNodeViewModel)graphControl.SelectedNode;
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
            var node = (DataflowNodeViewModel)element.DataContext;
            node.Size = new Size(element.ActualWidth, element.ActualHeight);
        }


        #endregion GraphControl


        /// <summary>
        /// Specifies the current state of the mouse handling logic.
        /// </summary>
        private MouseHandlingMode mouseHandlingMode = MouseHandlingMode.None;

        /// <summary>
        /// The point that was clicked relative to the ZoomAndPanControl.
        /// </summary>
        private Point origZoomAndPanControlMouseDownPoint;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
        /// </summary>
        private Point origContentMouseDownPoint;

        /// <summary>
        /// Records which mouse button clicked during mouse dragging.
        /// </summary>
        private MouseButton mouseButtonDown;

        /// <summary>
        /// Saves the previous zoom rectangle, pressing the backspace key jumps back to this zoom rectangle.
        /// </summary>
        private Rect prevZoomRect;

        /// <summary>
        /// Save the previous content scale, pressing the backspace key jumps back to this scale.
        /// </summary>
        private double prevZoomScale;

        /// <summary>
        /// Set to 'true' when the previous zoom rect is saved.
        /// </summary>
        private bool prevZoomRectSet = false;

        /// <summary>
        /// Event raised on mouse down in the GraphView.
        /// </summary> 
        private void graphControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            graphControl.Focus();
            Keyboard.Focus(graphControl);

            mouseButtonDown = e.ChangedButton;
            origZoomAndPanControlMouseDownPoint = e.GetPosition(zoomAndPanControl);
            origContentMouseDownPoint = e.GetPosition(graphControl);

            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
                (e.ChangedButton == MouseButton.Left ||
                 e.ChangedButton == MouseButton.Right))
            {
                // Shift + left- or right-down initiates zooming mode.
                mouseHandlingMode = MouseHandlingMode.Zooming;
            }
            else if (mouseButtonDown == MouseButton.Left &&
                     (Keyboard.Modifiers & ModifierKeys.Control) == 0)
            {
                //
                // Initiate panning, when control is not held down.
                // When control is held down left dragging is used for drag selection.
                // After panning has been initiated the user must drag further than the threshold value to actually start drag panning.
                //
                mouseHandlingMode = MouseHandlingMode.Panning;
                // todo  DisplayComposantParam(); autrement !

                // Todo : trouver une autre solution
                //this.ViewModel.NodeDeselected();
            }

            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                // Capture the mouse so that we eventually receive the mouse up event.
                graphControl.CaptureMouse();
                e.Handled = true;
            }
        }


        /// <summary>
        /// Event raised on mouse up in the GraphView.
        /// </summary>
        private void graphControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseHandlingMode != MouseHandlingMode.None)
            {
                if (mouseHandlingMode == MouseHandlingMode.Panning)
                {
                    //
                    // Panning was initiated but dragging was abandoned before the mouse
                    // cursor was dragged further than the threshold distance.
                    // This means that this basically just a regular left mouse click.
                    // Because it was a mouse click in empty space we need to clear the current selection.
                    //
                }
                else if (mouseHandlingMode == MouseHandlingMode.Zooming)
                {
                    if (mouseButtonDown == MouseButton.Left)
                    {
                        // Shift + left-click zooms in on the content.
                        ZoomIn(origContentMouseDownPoint);
                    }
                    else if (mouseButtonDown == MouseButton.Right)
                    {
                        // Shift + left-click zooms out from the content.
                        ZoomOut(origContentMouseDownPoint);
                    }
                }
                else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
                {
                    // When drag-zooming has finished we zoom in on the rectangle that was highlighted by the user.
                    ApplyDragZoomRect();
                }

                //
                // Reenable clearing of selection when empty space is clicked.
                // This is disabled when drag panning is in progress.
                //
                graphControl.IsClearSelectionOnEmptySpaceClickEnabled = true;

                //
                // Reset the override cursor.
                // This is set to a special cursor while drag panning is in progress.
                //
                Mouse.OverrideCursor = null;

                graphControl.ReleaseMouseCapture();
                mouseHandlingMode = MouseHandlingMode.None;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised on mouse move in the GraphView.
        /// </summary>
        private void graphControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseHandlingMode == MouseHandlingMode.Panning)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (Math.Abs(dragOffset.X) > dragThreshold ||
                    Math.Abs(dragOffset.Y) > dragThreshold)
                {
                    //
                    // The user has dragged the cursor further than the threshold distance, initiate
                    // drag panning.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragPanning;
                    graphControl.IsClearSelectionOnEmptySpaceClickEnabled = false;
                    Mouse.OverrideCursor = Cursors.ScrollAll;
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragPanning)
            {
                //
                // The user is left-dragging the mouse.
                // Pan the viewport by the appropriate amount.
                //
                Point curContentMousePoint = e.GetPosition(graphControl);
                Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

                zoomAndPanControl.ContentOffsetX -= dragOffset.X;
                zoomAndPanControl.ContentOffsetY -= dragOffset.Y;

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.Zooming)
            {
                Point curZoomAndPanControlMousePoint = e.GetPosition(zoomAndPanControl);
                Vector dragOffset = curZoomAndPanControlMousePoint - origZoomAndPanControlMouseDownPoint;
                double dragThreshold = 10;
                if (mouseButtonDown == MouseButton.Left &&
                    (Math.Abs(dragOffset.X) > dragThreshold ||
                    Math.Abs(dragOffset.Y) > dragThreshold))
                {
                    //
                    // When Shift + left-down zooming mode and the user drags beyond the drag threshold,
                    // initiate drag zooming mode where the user can drag out a rectangle to select the area
                    // to zoom in on.
                    //
                    mouseHandlingMode = MouseHandlingMode.DragZooming;
                    Point curContentMousePoint = e.GetPosition(graphControl);
                    InitDragZoomRect(origContentMouseDownPoint, curContentMousePoint);
                }

                e.Handled = true;
            }
            else if (mouseHandlingMode == MouseHandlingMode.DragZooming)
            {
                //
                // When in drag zooming mode continously update the position of the rectangle
                // that the user is dragging out.
                //
                Point curContentMousePoint = e.GetPosition(graphControl);
                SetDragZoomRect(origContentMouseDownPoint, curContentMousePoint);

                e.Handled = true;
            }
        }

        /// <summary>
        /// Event raised by rotating the mouse wheel.
        /// </summary>
        /// 

        private void graphControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;

            if (e.Delta > 0)
            {
                zoomAndPanControl.MouseWheelUp();
            }
            else if (e.Delta < 0)
            {
                zoomAndPanControl.MouseWheelDown();
            }
        }
        /*
               private void graphControl_MouseWheel(object sender, MouseWheelEventArgs e)
                {
                    e.Handled = true;

                    if (e.Delta > 0)
                    {
                        Point curContentMousePoint = e.GetPosition(graphControl);
                        ZoomIn(curContentMousePoint);
                    }
                    else if (e.Delta < 0)
                    {
                        Point curContentMousePoint = e.GetPosition(graphControl);
                        ZoomOut(curContentMousePoint);
                    }
                }
        */

        /// <summary>
        /// Event raised when the user has double clicked in the zoom and pan control.
        /// </summary>
        private void graphControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
            {
                Point doubleClickPoint = e.GetPosition(graphControl);
                zoomAndPanControl.AnimatedSnapTo(doubleClickPoint);
            }
        }

        /// <summary>
        /// The 'ZoomIn' command (bound to the plus key) was executed.
        /// </summary>
        private void ZoomIn_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var o = graphControl.SelectedNode;

            ZoomIn(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'ZoomOut' command (bound to the minus key) was executed.
        /// </summary>
        private void ZoomOut_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ZoomOut(new Point(zoomAndPanControl.ContentZoomFocusX, zoomAndPanControl.ContentZoomFocusY));
        }

        /// <summary>
        /// The 'JumpBackToPrevZoom' command was executed.
        /// </summary>
        private void JumpBackToPrevZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            JumpBackToPrevZoom();
        }

        /// <summary>
        /// Determines whether the 'JumpBackToPrevZoom' command can be executed.
        /// </summary>
        private void JumpBackToPrevZoom_CanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = prevZoomRectSet;
        }

        /// <summary>
        /// The 'Fit' command was executed.
        /// </summary>
        private void FitContent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            IList nodes = null;

            if (graphControl.SelectedNodes.Count > 0)
            {
                nodes = graphControl.SelectedNodes;
            }
            else
            {
                nodes = this.ViewModel.GraphVM.Nodes;
                if (nodes.Count == 0)
                {
                    return;
                }
            }

            SavePrevZoomRect();

            Rect actualContentRect = DetermineAreaOfNodes(nodes);

            //
            // Inflate the content rect by a fraction of the actual size of the total content area.
            // This puts a nice border around the content we are fitting to the viewport.
            //
            actualContentRect.Inflate(graphControl.ActualWidth / 40, graphControl.ActualHeight / 40);

            zoomAndPanControl.AnimatedZoomTo(actualContentRect);
        }

        ///=================================================================
        /// <summary>
        /// Assure que les noeuds sélectionnés sont visibles.
        /// </summary>
        ///=================================================================
        private void OnVisibleRectChanged(DependencyPropertyChangedEventArgs e)
        {
            // to avoid multiple successive OnVisibleRectChanged call from within the view
            if (_visibleRectinternalChangeDone)
            {
                // down the flag
                _visibleRectinternalChangeDone = false;
                return;
            }

            if (VisibleRect.IsEmpty)
                return;

            SavePrevZoomRect();
            var viewportRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);

            //
            // Inflate the content rect by a fraction of the actual size of the total content area.
            // This puts a nice border around the content we are fitting to the viewport.
            //
            Rect visibleRect = VisibleRect;
            visibleRect.Inflate(graphControl.ActualWidth / 40, graphControl.ActualHeight / 40);

            // Déjà visible ?
            if (viewportRect.Contains(visibleRect))
                return;

            // Besoin de zoomer ?
            if (viewportRect.Width >= visibleRect.Width && viewportRect.Height >= visibleRect.Height)
            {
                // non => translation
                if (visibleRect.X < viewportRect.X)
                    viewportRect.X = visibleRect.X;
                else if (viewportRect.Right < visibleRect.Right)
                    viewportRect.X += visibleRect.Right - viewportRect.Right;
                if (visibleRect.Y < viewportRect.Y)
                    viewportRect.Y = visibleRect.Y;
                else if (viewportRect.Bottom < visibleRect.Bottom)
                    viewportRect.Y += visibleRect.Bottom - viewportRect.Bottom;

                zoomAndPanControl.AnimatedMoveTo(viewportRect.TopLeft);
            }
            else
            {
                // oui => zoom + translation
                zoomAndPanControl.AnimatedZoomTo(viewportRect);
            }

            // raise the flag
            _visibleRectinternalChangeDone = true;
            VisibleRect = viewportRect;
        }

        /// <summary>
        /// Determine the area covered by the specified list of nodes.
        /// </summary>
        private Rect DetermineAreaOfNodes(IList nodes)
        {
            NodeViewModel firstNode = (NodeViewModel)nodes[0];
            Rect actualContentRect = new Rect(firstNode.X, firstNode.Y, firstNode.Size.Width, firstNode.Size.Height);

            for (int i = 1; i < nodes.Count; ++i)
            {
                NodeViewModel node = (NodeViewModel)nodes[i];
                Rect nodeRect = new Rect(node.X, node.Y, node.Size.Width, node.Size.Height);
                actualContentRect = Rect.Union(actualContentRect, nodeRect);
            }
            return actualContentRect;
        }

        /// <summary>
        /// The 'Fill' command was executed.
        /// </summary>
        private void Fill_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomAndPanControl.AnimatedScaleToFit();
        }

        /// <summary>
        /// The 'OneHundredPercent' command was executed.
        /// </summary>
        private void OneHundredPercent_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavePrevZoomRect();

            zoomAndPanControl.AnimatedZoomTo(1.0);
        }

        /// <summary>
        /// Jump back to the previous zoom level.
        /// </summary>
        private void JumpBackToPrevZoom()
        {
            zoomAndPanControl.AnimatedZoomTo(prevZoomScale, prevZoomRect);

            ClearPrevZoomRect();
        }

        /// <summary>
        /// Zoom the viewport out, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomOut(Point contentZoomCenter)
        {
            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale - 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Zoom the viewport in, centering on the specified point (in content coordinates).
        /// </summary>
        private void ZoomIn(Point contentZoomCenter)
        {
            zoomAndPanControl.ZoomAboutPoint(zoomAndPanControl.ContentScale + 0.1, contentZoomCenter);
        }

        /// <summary>
        /// Initialize the rectangle that the use is dragging out.
        /// </summary>
        private void InitDragZoomRect(Point pt1, Point pt2)
        {
            SetDragZoomRect(pt1, pt2);

            dragZoomCanvas.Visibility = Visibility.Visible;
            dragZoomBorder.Opacity = 0.5;
        }

        /// <summary>
        /// Update the position and size of the rectangle that user is dragging out.
        /// </summary>
        private void SetDragZoomRect(Point pt1, Point pt2)
        {
            double x, y, width, height;

            //
            // Deterine x,y,width and height of the rect inverting the points if necessary.
            // 

            if (pt2.X < pt1.X)
            {
                x = pt2.X;
                width = pt1.X - pt2.X;
            }
            else
            {
                x = pt1.X;
                width = pt2.X - pt1.X;
            }

            if (pt2.Y < pt1.Y)
            {
                y = pt2.Y;
                height = pt1.Y - pt2.Y;
            }
            else
            {
                y = pt1.Y;
                height = pt2.Y - pt1.Y;
            }

            //
            // Update the coordinates of the rectangle that is being dragged out by the user.
            // The we offset and rescale to convert from content coordinates.
            //
            Canvas.SetLeft(dragZoomBorder, x);
            Canvas.SetTop(dragZoomBorder, y);
            dragZoomBorder.Width = width;
            dragZoomBorder.Height = height;
        }

        /// <summary>
        /// When the user has finished dragging out the rectangle the zoom operation is applied.
        /// </summary>
        private void ApplyDragZoomRect()
        {
            //
            // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
            //
            SavePrevZoomRect();

            //
            // Retrieve the rectangle that the user draggged out and zoom in on it.
            //
            double contentX = Canvas.GetLeft(dragZoomBorder);
            double contentY = Canvas.GetTop(dragZoomBorder);
            double contentWidth = dragZoomBorder.Width;
            double contentHeight = dragZoomBorder.Height;
            zoomAndPanControl.AnimatedZoomTo(new Rect(contentX, contentY, contentWidth, contentHeight));

            FadeOutDragZoomRect();
        }

        //
        // Fade out the drag zoom rectangle.
        //
        private void FadeOutDragZoomRect()
        {
            AnimationHelper.StartAnimation(dragZoomBorder, Border.OpacityProperty, 0.0, 0.1,
                delegate (object sender, EventArgs e)
                {
                    dragZoomCanvas.Visibility = Visibility.Collapsed;
                });
        }

        //
        // Record the previous zoom level, so that we can jump back to it when the backspace key is pressed.
        //
        private void SavePrevZoomRect()
        {
            prevZoomRect = new Rect(zoomAndPanControl.ContentOffsetX, zoomAndPanControl.ContentOffsetY, zoomAndPanControl.ContentViewportWidth, zoomAndPanControl.ContentViewportHeight);
            prevZoomScale = zoomAndPanControl.ContentScale;
            prevZoomRectSet = true;
        }

        /// <summary>
        /// Clear the memory of the previous zoom level.
        /// </summary>
        private void ClearPrevZoomRect()
        {
            prevZoomRectSet = false;
        }
    }
}
