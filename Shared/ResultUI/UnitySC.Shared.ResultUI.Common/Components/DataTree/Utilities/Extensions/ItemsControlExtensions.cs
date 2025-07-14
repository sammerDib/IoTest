using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.Shared.ResultUI.Common.Components.DataTree.Utilities.Extensions
{
    internal static class ItemsControlExtensions
    {
        public static bool IsTheMouseOnTargetBounds(this UIElement self, Func<IInputElement, Point> getPosition)
        {
            var relativeMousePosition = getPosition(self);
            if (relativeMousePosition.X < 0 || relativeMousePosition.Y < 0) return false;
            var renderSize = self.RenderSize;
            return relativeMousePosition.X < renderSize.Width && relativeMousePosition.Y < renderSize.Height;
        }

        public static UIElement GetItem(this ItemsControl self, int index)
        {
            if (self.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated) return null;
            return self.ItemContainerGenerator.ContainerFromIndex(index) as UIElement;
        }

        public static FrameworkElement GetItemByMousePos(this ItemsControl self, Func<IInputElement, Point> mousePos)
        {
            for (var index = 0; index < self.Items.Count; index++)
            {
                var item = self.GetItem(index);

                if (item == null) return null;

                // Continue if the current item is not mouseOver 
                if (item.IsTheMouseOnTargetBounds(mousePos))
                {
                    return item as FrameworkElement;
                }
            }

            return null;
        }

        public static FrameworkElement GetNextItemByMousePos(this ItemsControl self, Func<IInputElement, Point> mousePos)
        {
            for (var index = 0; index < self.Items.Count; index++)
            {
                var item = self.GetItem(index);

                if (item == null) return null;

                // Continue if the current item is not mouseOver 
                if (item.IsTheMouseOnTargetBounds(mousePos))
                {
                    return self.GetItem(index + 1) as FrameworkElement;
                }
            }

            return null;
        }

        public static FrameworkElement GetItemByMousePos(this ItemsControl self, Func<IInputElement, Point> mousePos, string childContainerName)
        {
            for (var index = 0; index < self.Items.Count; index++)
            {
                var item = self.GetItem(index);

                if (item == null) return null;

                // Continue if the current item is not mouseOver 
                if (!item.IsTheMouseOnTargetBounds(mousePos)) continue;

                // Get the child's itemsControl
                var itemsControl = FindChild<ItemsControl>(item, childContainerName);
                if (itemsControl.Items.Count > 0)
                {
                    // Get if a subChild item is mouseOver
                    var mouseOverItem = itemsControl.GetItemByMousePos(mousePos, childContainerName);
                    if (mouseOverItem != null) return mouseOverItem;
                }

                return item as FrameworkElement;
            }

            return null;
        }

        public static FrameworkElement GetItemByDataContext(this ItemsControl self, object dataContext, string childContainerName)
        {
            for (var index = 0; index < self.Items.Count; index++)
            {
                var item = self.GetItem(index) as FrameworkElement;
                if (item == null) return null;
                if (item.DataContext == dataContext) return item;

                // Get the child's itemsControl
                var itemsControl = FindChild<ItemsControl>(item, childContainerName);
                if (itemsControl.Items.Count > 0)
                {
                    // Get if a subChild item is mouseOver
                    var foundItem = itemsControl.GetItemByDataContext(dataContext, childContainerName);
                    if (foundItem != null) return foundItem;
                }
            }

            return null;
        }

        public static int GetNextIndexFromMousePos(this FrameworkElement self, Func<IInputElement, Point> getPosition, int currentIndex)
        {
            if (getPosition(self).Y > self.ActualHeight / 2)
                return currentIndex + 1;
            else
                return currentIndex;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            // Confirm parent is valid
            if (parent == null) return null;

            T foundChild = null;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                var typedChild = child as T;
                if (typedChild == null)
                {
                    // Recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T) child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T) child;
                    break;
                }
            }

            return foundChild;
        }
    }
}
