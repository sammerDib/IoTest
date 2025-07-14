using System;
using System.Windows;

namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Une ColumnDefinition qui supporte un attribut Visible
    ///////////////////////////////////////////////////////////////////////
    public class ColumnDefinition : System.Windows.Controls.ColumnDefinition
    {
        //=================================================================
        // DependencyProperty
        //=================================================================
        public static DependencyProperty VisibleProperty;

        public Boolean Visible { get { return (Boolean)GetValue(VisibleProperty); } set { SetValue(VisibleProperty, value); } }

        //=================================================================
        // Constructor
        //=================================================================
        static ColumnDefinition()
        {
            VisibleProperty = DependencyProperty.Register("Visible", typeof(Boolean), typeof(ColumnDefinition), new PropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged)));
            ColumnDefinition.WidthProperty.OverrideMetadata(typeof(ColumnDefinition), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, new CoerceValueCallback(CoerceWidth)));
            ColumnDefinition.MinWidthProperty.OverrideMetadata(typeof(ColumnDefinition), new FrameworkPropertyMetadata((Double)0, null, new CoerceValueCallback(CoerceMinWidth)));
        }

        //=================================================================
        // Callbacks
        //=================================================================
        private static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(ColumnDefinition.WidthProperty);
            obj.CoerceValue(ColumnDefinition.MinWidthProperty);
        }

        private static Object CoerceWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinition)obj).Visible) ? nValue : new GridLength(0);
        }

        private static Object CoerceMinWidth(DependencyObject obj, Object nValue)
        {
            return (((ColumnDefinition)obj).Visible) ? nValue : (Double)0;
        }
    }
}
