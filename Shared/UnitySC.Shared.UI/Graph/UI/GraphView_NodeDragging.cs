using System;
using System.Collections.Generic;

namespace UnitySC.Shared.UI.Graph.UI
{
    /// <summary>
    /// Partial definition of the GraphView class.
    /// This file only contains private members related to dragging nodes.
    /// </summary>
    public partial class GraphView
    {
        #region Private Methods

        /// <summary>
        /// Event raised when the user starts to drag a node.
        /// </summary>
        private void NodeItem_DragStarted(object source, NodeDragStartedEventArgs e)
        {
            e.Handled = true;

            this.IsDragging = true;
            this.IsNotDragging = false;
            this.IsDraggingNode = true;
            this.IsNotDraggingNode = false;

            var eventArgs = new NodeDragStartedEventArgs(NodeDragStartedEvent, this, this.SelectedNodes);
            RaiseEvent(eventArgs);

            e.Cancel = eventArgs.Cancel;
        }

        /// <summary>
        /// Event raised while the user is dragging a node.
        /// </summary>
        private void NodeItem_Dragging(object source, NodeDraggingEventArgs e)
        {
            e.Handled = true;

            //
            // Cache the NodeItem for each selected node whilst dragging is in progress.
            //
            if (this.cachedSelectedNodeItems == null)
            {
                this.cachedSelectedNodeItems = new List<NodeItem>();

                foreach (object selectedNode in this.SelectedNodes)
                {
                    var nodeItem = FindAssociatedNodeItem(selectedNode);
                    if (nodeItem == null)
                    {
                        throw new ApplicationException("Unexpected code path!");
                    }

                    this.cachedSelectedNodeItems.Add(nodeItem);
                }
            }

            //
            // Update the position of the node within the Canvas.
            //
            foreach (var nodeItem in this.cachedSelectedNodeItems)
            {
                nodeItem.X += e.HorizontalChange;
                nodeItem.Y += e.VerticalChange;
            }

            var eventArgs = new NodeDraggingEventArgs(NodeDraggingEvent, this, this.SelectedNodes, e.HorizontalChange, e.VerticalChange);
            RaiseEvent(eventArgs);
        }

        /// <summary>
        /// Event raised when the user has finished dragging a node.
        /// </summary>
        private void NodeItem_DragCompleted(object source, NodeDragCompletedEventArgs e)
        {
            e.Handled = true;

            var eventArgs = new NodeDragCompletedEventArgs(NodeDragCompletedEvent, this, this.SelectedNodes);
            RaiseEvent(eventArgs);

            if (cachedSelectedNodeItems != null)
            {
                cachedSelectedNodeItems = null;
            }

            this.IsDragging = false;
            this.IsNotDragging = true;
            this.IsDraggingNode = false;
            this.IsNotDraggingNode = true;
        }

        private void NodeItem_LeftClic(object source, NodeLeftClicEventArgs e)
        {
            e.Handled = true;

            var eventArgs = new NodeLeftClicEventArgs(NodeLeftClicEvent, this, e.Node);
            RaiseEvent(eventArgs);
        }

        #endregion Private Methods
    }
}