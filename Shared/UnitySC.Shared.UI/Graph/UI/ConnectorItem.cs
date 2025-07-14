using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.Shared.UI.Graph.UI
{
    /// <summary>
    /// This is the UI element for a connector.
    /// Each nodes has multiple connectors that are used to connect it to other nodes.
    /// </summary>
    public class ConnectorItem : ContentControl
    {
        #region Dependency Property/Event Definitions

        public static readonly DependencyProperty HotspotProperty =
            DependencyProperty.Register("Hotspot", typeof(Point), typeof(ConnectorItem));

        internal static readonly DependencyProperty ParentGraphViewProperty =
            DependencyProperty.Register("ParentGraphView", typeof(GraphView), typeof(ConnectorItem),
                new FrameworkPropertyMetadata(ParentNGraphView_PropertyChanged));

        internal static readonly DependencyProperty ParentNodeItemProperty =
            DependencyProperty.Register("ParentNodeItem", typeof(NodeItem), typeof(ConnectorItem));

        internal static readonly RoutedEvent ConnectorDragStartedEvent =
            EventManager.RegisterRoutedEvent("ConnectorDragStarted", RoutingStrategy.Bubble, typeof(ConnectorItemDragStartedEventHandler), typeof(ConnectorItem));

        internal static readonly RoutedEvent ConnectorDraggingEvent =
            EventManager.RegisterRoutedEvent("ConnectorDragging", RoutingStrategy.Bubble, typeof(ConnectorItemDraggingEventHandler), typeof(ConnectorItem));

        internal static readonly RoutedEvent ConnectorDragCompletedEvent =
            EventManager.RegisterRoutedEvent("ConnectorDragCompleted", RoutingStrategy.Bubble, typeof(ConnectorItemDragCompletedEventHandler), typeof(ConnectorItem));

        #endregion Dependency Property/Event Definitions

        #region Private Data Members

        /// <summary>
        /// The point the mouse was last at when dragging.
        /// </summary>
        private Point lastMousePoint = new Point(double.NaN, double.NaN);

        /// <summary>
        /// Set to 'true' when the user is dragging the connector.
        /// </summary>
        private bool isDragging = false;

        #endregion Private Data Members

        public ConnectorItem()
        {
            //
            // By default, we don't want a connector to be focusable.
            //
            Focusable = false;

            //
            // Hook layout update to recompute 'Hotspot' when the layout changes.
            //
            this.LayoutUpdated += new EventHandler(ConnectorItem_LayoutUpdated);
        }

        /// <summary>
        /// Automatically updated dependency property that specifies the hotspot (or center point) of the connector.
        /// Specified in content coordinate.
        /// </summary>
        public Point Hotspot
        {
            get
            {
                return (Point)GetValue(HotspotProperty);
            }
            set
            {
                SetValue(HotspotProperty, value);
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
        /// Reference to the data-bound parent NodeItem.
        /// </summary>
        internal NodeItem ParentNodeItem
        {
            get
            {
                return (NodeItem)GetValue(ParentNodeItemProperty);
            }
            set
            {
                SetValue(ParentNodeItemProperty, value);
            }
        }

        #endregion Private Data Members\Properties

        #region Private Methods

        /// <summary>
        /// Static constructor.
        /// </summary>
        static ConnectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ConnectorItem), new FrameworkPropertyMetadata(typeof(ConnectorItem)));
        }

        /// <summary>
        /// A mouse button has been held down.
        /// </summary>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (ParentNodeItem != null)
                ParentNodeItem.BringToFront();

            ParentGraphView.Focus();

            if (e.ChangedButton == MouseButton.Left)
            {
                lastMousePoint = e.GetPosition(Application.Current.MainWindow);
                OnMouseMove(e);
                e.Handled = true;
            }
        }

        /// <summary>
        /// The mouse cursor has been moved.
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (isDragging)
            {
                //
                // Raise the event to notify that dragging is in progress.
                //

                var curMousePoint = e.GetPosition(Application.Current.MainWindow);
                var offset = PointFromScreen(curMousePoint) - PointFromScreen(lastMousePoint);
                if (offset.X != 0.0 && offset.Y != 0.0)
                {
                    lastMousePoint = curMousePoint;

                    RaiseEvent(new ConnectorItemDraggingEventArgs(ConnectorDraggingEvent, this, offset.X, offset.Y));
                }

                e.Handled = true;
            }
            else if (e.LeftButton == MouseButtonState.Pressed && ParentGraphView != null && ParentGraphView.EnableNodeDragging)
            {
                //
                // The user is left-dragging the connector and connection dragging is enabled,

                var curMousePoint = e.GetPosition(Application.Current.MainWindow);
                var dragDelta = curMousePoint - lastMousePoint;
                double dragDistance = Math.Abs(dragDelta.Length);

                //
                // Raise an event to notify that that dragging has commenced.
                //
                var eventArgs = new ConnectorItemDragStartedEventArgs(ConnectorDragStartedEvent, this);
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

        /// <summary>
        /// A mouse button has been released.
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.ChangedButton == MouseButton.Left)
            {
                if (isDragging)
                {
                    RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, this));

                    this.ReleaseMouseCapture();

                    lastMousePoint = new Point(double.NaN, double.NaN);
                    isDragging = false;
                }
                else
                {
                    //
                    // Execute mouse up selection logic only if there was no drag operation.
                    //
                    if (this.ParentNodeItem != null)
                    {
                        //
                        // Delegate to parent node to execute selection logic.
                        //
                        this.ParentNodeItem.MouseSelectionLogic();
                    }
                }

                e.Handled = true;
            }
        }

        /// <summary>
        /// Cancel connection dragging for the connector that was dragged out.
        /// </summary>
        internal void CancelConnectionDragging()
        {
            if (isDragging)
            {
                //
                // Raise ConnectorDragCompleted, with a null connector.
                //
                RaiseEvent(new ConnectorItemDragCompletedEventArgs(ConnectorDragCompletedEvent, null));
                this.ReleaseMouseCapture();
                lastMousePoint = new Point(double.NaN, double.NaN);
                isDragging = false;
            }
        }

        /// <summary>
        /// Event raised when 'ParentNGraphView' property has changed.
        /// </summary>
        private static void ParentNGraphView_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = (ConnectorItem)d;
            c.UpdateHotspot();
        }

        /// <summary>
        /// Event raised when the layout of the connector has been updated.
        /// </summary>
        private void ConnectorItem_LayoutUpdated(object sender, EventArgs e)
        {
            UpdateHotspot();
        }

        /// <summary>
        /// Update the connector hotspot.
        /// </summary>
        private void UpdateHotspot()
        {
            if (this.ParentGraphView == null)
            {
                // No parent GraphView is set.
                return;
            }

            if (!this.ParentGraphView.IsAncestorOf(this))
            {
                //
                // The parent GraphView is no longer an ancestor of the connector.
                // This happens when the connector (and its parent node) has been removed from the graph.
                // Reset the property null so we don't attempt to check again.
                //
                this.ParentGraphView = null;
                return;
            }

            //
            // The parent GraphView is still valid.
            // Compute the center point of the connector.
            //
            var centerPoint = new Point(this.ActualWidth / 2, this.ActualHeight / 2);

            //
            // Transform the center point so that it is relative to the parent GraphView.
            // Then assign it to Hotspot.  Usually Hotspot will be data-bound to the application
            // view-model using OneWayToSource so that the value of the hotspot is then pushed through
            // to the view-model.
            //
            if (this.Hotspot != this.TransformToAncestor(this.ParentGraphView).Transform(centerPoint))
                this.Hotspot = this.TransformToAncestor(this.ParentGraphView).Transform(centerPoint);
        }

        #endregion Private Methods
    }
}