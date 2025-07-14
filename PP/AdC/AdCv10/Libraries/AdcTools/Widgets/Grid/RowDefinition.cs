using System;
using System.Windows;

namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Une RowDefinition qui supporte un attribut Visible
    ///////////////////////////////////////////////////////////////////////
    public class RowDefinition : System.Windows.Controls.RowDefinition
    {
        //=================================================================
        // DependencyProperty
        //=================================================================
        public static DependencyProperty VisibleProperty;

        public Boolean Visible { get { return (Boolean)GetValue(VisibleProperty); } set { SetValue(VisibleProperty, value); } }

        //=================================================================
        // Constructor
        //=================================================================
        static RowDefinition()
        {
            VisibleProperty = DependencyProperty.Register("Visible", typeof(Boolean), typeof(RowDefinition), new PropertyMetadata(true, new PropertyChangedCallback(OnVisibleChanged)));
            RowDefinition.HeightProperty.OverrideMetadata(typeof(RowDefinition), new FrameworkPropertyMetadata(new GridLength(1, GridUnitType.Star), null, new CoerceValueCallback(CoerceHeight)));
            RowDefinition.MinHeightProperty.OverrideMetadata(typeof(RowDefinition), new FrameworkPropertyMetadata((Double)0, null, new CoerceValueCallback(CoerceMinHeight)));
        }

        //=================================================================
        // Callbacks
        //=================================================================
        private static void OnVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            obj.CoerceValue(RowDefinition.HeightProperty);
            obj.CoerceValue(RowDefinition.MinHeightProperty);
        }

        private static Object CoerceHeight(DependencyObject obj, Object nValue)
        {
            return (((RowDefinition)obj).Visible) ? nValue : new GridLength(0);
        }

        private static Object CoerceMinHeight(DependencyObject obj, Object nValue)
        {
            return (((RowDefinition)obj).Visible) ? nValue : (Double)0;
        }
    }
}
