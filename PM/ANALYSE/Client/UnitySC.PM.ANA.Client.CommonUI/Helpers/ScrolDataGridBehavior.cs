using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.Helpers
{
    public static class ScrolDataGridBehavior
    {
        public static readonly DependencyProperty AutoscrollProperty = DependencyProperty.RegisterAttached(
            "Autoscroll", typeof(bool), typeof(ScrolDataGridBehavior), new PropertyMetadata(default(bool), AutoscrollChangedCallback));

        private static readonly Dictionary<DataGrid, NotifyCollectionChangedEventHandler> handlersDict = new Dictionary<DataGrid, NotifyCollectionChangedEventHandler>();

        private static void AutoscrollChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var dataGrid = dependencyObject as DataGrid;
            if (dataGrid == null)
            {
                throw new InvalidOperationException("Dependency object is not DataGrid.");
            }

            if ((bool)args.NewValue)
            {
                Subscribe(dataGrid);
                dataGrid.Unloaded += DataGridOnUnloaded;
                dataGrid.Loaded += DataGridOnLoaded;
            }
            else
            {
                Unsubscribe(dataGrid);
                dataGrid.Unloaded -= DataGridOnUnloaded;
                dataGrid.Loaded -= DataGridOnLoaded;
            }
        }

        private static void Subscribe(DataGrid dataGrid)
        {
            NotifyCollectionChangedEventHandler handler;
            // If Already subscribed
            handlersDict.TryGetValue(dataGrid, out handler);
            if (!(handler is null))
            {
                ScrollToEnd(dataGrid);
                return;
            }
            handler = new NotifyCollectionChangedEventHandler((sender, eventArgs) => ScrollToEnd(dataGrid));

            ((INotifyCollectionChanged)dataGrid.Items).CollectionChanged += handler;
            handlersDict.Add(dataGrid, handler);
            ScrollToEnd(dataGrid);
        }
        private static void Unsubscribe(DataGrid dataGrid)
        {
            NotifyCollectionChangedEventHandler handler;
            handlersDict.TryGetValue(dataGrid, out handler);
            if (handler == null)
            {
                return;
            }
            ((INotifyCollectionChanged)dataGrid.Items).CollectionChanged -= handler;
            handlersDict.Remove(dataGrid);
        }

        private static void DataGridOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Subscribe(dataGrid);
            }
        }

        private static void DataGridOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var dataGrid = (DataGrid)sender;
            if (GetAutoscroll(dataGrid))
            {
                Unsubscribe(dataGrid);
            }
        }

        private static void ScrollToEnd(DataGrid datagrid)
        {
            if (datagrid.Items.Count == 0)
            {
                return;
            }
            datagrid.ScrollIntoView(datagrid.Items[datagrid.Items.Count - 1]);
        }

        public static void SetAutoscroll(DependencyObject element, bool value)
        {
            element.SetValue(AutoscrollProperty, value);
        }

        public static bool GetAutoscroll(DependencyObject element)
        {
            return (bool)element.GetValue(AutoscrollProperty);
        }
    }
}
   
