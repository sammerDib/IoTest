using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphUI
{
    /// <summary>
    /// This is a UI element that represents a graph/flow-chart node.
    /// </summary>
    public class NodeItem : ListBoxItem
    {
        #region Dependency Property/Event Definitions

        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(NodeItem),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(NodeItem),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty ZIndexProperty =
            DependencyProperty.Register("ZIndex", typeof(int), typeof(NodeItem),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        internal static readonly DependencyProperty ParentGraphViewProperty =
            DependencyProperty.Register("ParentGraphView", typeof(GraphView), typeof(NodeItem),
                new FrameworkPropertyMetadata(ParentGraphView_PropertyChanged));

        internal static readonly RoutedEvent NodeDragStartedEvent =
            EventManager.RegisterRoutedEvent("NodeDragStarted", RoutingStrategy.Bubble, typeof(NodeDragStartedEventHandler), typeof(NodeItem));

        internal static readonly RoutedEvent NodeDraggingEvent =
            EventManager.RegisterRoutedEvent("NodeDragging", RoutingStrategy.Bubble, typeof(NodeDraggingEventHandler), typeof(NodeItem));

        internal static readonly RoutedEvent NodeDragCompletedEvent =
            EventManager.RegisterRoutedEvent("NodeDragCompleted", RoutingStrategy.Bubble, typeof(NodeDragCompletedEventHandler), typeof(NodeItem));

        internal static readonly RoutedEvent NodeLeftClicEvent =
            EventManager.RegisterRoutedEvent("NodeLeftClic", RoutingStrategy.Bubble, typeof(NodeLeftClicEventHandler), typeof(NodeItem));


        #endregion Dependency Property/Event Definitions

        public NodeItem()
        {
            //
            // By default, we don't want this UI element to be focusable.
            //
            Focusable = false;
        }

        /// <summary>
        /// The X coordinate of the node.
        /// </summary>
        public double X
        {
            get
            {
                return (double)GetValue(XProperty);
            }
            set
            {
                SetValue(XProperty, value);
            }
        }

        /// <summary>
        /// The Y coordinate of the node.
        /// </summary>
        public double Y
        {
            get
            {
                return (double)GetValue(YProperty);
            }
            set
            {
                SetValue(YProperty, value);
            }
        }

        /// <summary>
        /// The Z index of the node.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return (int)GetValue(ZIndexProperty);
            }
            set
            {
                SetValue(ZIndexProperty, value);
            }
        }

        #region Private Data Members\Properties

        /// <summary>
        /// Reference to the data-bound parent GraphView.
        /// </summary>
        internal GraphView ParentGraphView
        {
            get
            {
                return (GraphView)GetValue(ParentGraphViewProperty);
            }
            set
            {
                SetValue(ParentGraphViewProperty, value);
            }
        }

        /// <summary>
        /// The point the mouse was last at when dragging. Screen coordinates.
        /// </summary>
        private Point lastMousePoint = new Point(double.NaN, double.NaN);

        /// <summary>
        /// Set to 'true' when dragging has started.
        /// </summary>
        private bool isDragging = false;

        /// <summary>
        /// The threshold distance the mouse-cursor must move before dragging begins.
        /// </summary>
        private static readonly double DragThreshold = 5;

        #endregion Private Data Members\Properties

        #region Private Methods

        /// <summary>
        /// Static constructor.
        /// </summary>
        static NodeItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NodeItem), new FrameworkPropertyMetadata(typeof(NodeItem)));
        }

        /// <summary>
        /// Bring the node to the front of other elements.
        /// </summary>
        internal void BringToFront()
        {
            if (this.ParentGraphView == null)
            {
                return;
            }

            int maxZ = this.ParentGraphView.FindMaxZIndex();
            this.ZIndex = maxZ + 1;
        }

        /// <summary>
        /// Called when a mouse button is held down.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            BringToFront();
            if (this.ParentGraphView != null)
                this.ParentGraphView.Focus();

            if (e.ChangedButton == MouseButton.Left)
            {
                lastMousePoint = PointToScreen(e.GetPosition(this));
                MouseSelectionLogic();
                e.Handled = true;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                MouseSelectionLogic();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called when the mouse cursor is moved.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isDragging)
            {
                //
                // Raise the event to notify that dragging is in progress.
                //

                Point curMousePoint = PointToScreen(e.GetPosition(this));

                object item = this;
                if (DataContext != null)
                {
                    item = DataContext;
                }

                Vector offset = PointFromScreen(curMousePoint) - PointFromScreen(lastMousePoint);
                if (offset.X != 0.0 || offset.Y != 0.0)
                {
                    lastMousePoint = curMousePoint;

                    RaiseEvent(new NodeDraggingEventArgs(NodeDraggingEvent, this, new object[] { item }, offset.X, offset.Y));
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed && this.ParentGraphView.EnableNodeDragging)
            {
                //
                // The user is left-dragging the node,
                // but don't initiate the drag operation until 
                // the mouse cursor has moved more than the threshold distance.
                //
                Point curMousePoint = PointToScreen(e.GetPosition(this));
                var dragDelta = curMousePoint - lastMousePoint;
                double dragDistance = Math.Abs(dragDelta.Length);
                if (dragDistance > DragThreshold)
                {
                    //
                    // When the mouse has been dragged more than the threshold value commence dragging the node.
                    //

                    //
                    // Raise an event to notify that that dragging has commenced.
                    //
                    NodeDragStartedEventArgs eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, new NodeItem[] { this });
                    RaiseEvent(eventArgs);

                    if (eventArgs.Cancel)
                    {
                        //
                        // Handler of the event disallowed dragging of the node.
                        //
                        return;
                    }

                    isDragging = true;
                    this.CaptureMouse();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Called when a mouse button is released.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton != MouseButton.Left)
                return;

            if (isDragging)
            {
                //
                // Raise an event to notify that node dragging has finished.
                //
                RaiseEvent(new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, new NodeItem[] { this }));
                this.ReleaseMouseCapture();
                isDragging = false;
            }

            lastMousePoint = new Point(double.NaN, double.NaN);
            e.Handled = true;
        }

        /// <summary>
        /// This method contains selection logic that is invoked when the left mouse button is released.
        /// The reason this exists in its own method rather than being included in OnMouseUp/Down is 
        /// so that ConnectorItem can reuse this logic from its OnMouseUp.
        /// </summary>
        internal void MouseSelectionLogic()
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                //
                // Control key was held down.
                // Toggle the selection.
                //
                this.IsSelected = !this.IsSelected;
            }
            else
            {
                //
                // Control key was not held down.
                //
                if (this.ParentGraphView.SelectedNodes.Count == 1 &&
                    (this.ParentGraphView.SelectedNode == this ||
                     this.ParentGraphView.SelectedNode == this.DataContext))
                {
                    //
                    // The item that was clicked is already the only selected item.
                    //this.ParentGraphView.ClicNode(this);
                    //
                    // Raise an event to notify that this node was clicked.
                    //
                    RaiseEvent(new NodeLeftClicEventArgs(NodeLeftClicEvent, this, this.ParentGraphView.SelectedNode));
                }
                else
                {
                    //
                    // Clear the selection and select the clicked item as the only selected item.
                    //
                    this.ParentGraphView.SelectedNodes.Clear();
                    this.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Event raised when the ParentGraphView property has changed.
        /// </summary>
        private static void ParentGraphView_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            //
            // Bring new nodes to the front of the z-order.
            //
            var nodeItem = (NodeItem)o;
            nodeItem.BringToFront();
        }

        #endregion Private Methods
    }
}
