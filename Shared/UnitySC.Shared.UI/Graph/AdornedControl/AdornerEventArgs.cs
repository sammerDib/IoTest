using System.Windows;

namespace UnitySC.Shared.UI.Graph.AdornedControl
{
    public class AdornerEventArgs : RoutedEventArgs
    {
        private readonly FrameworkElement adorner = null;

        public AdornerEventArgs(RoutedEvent routedEvent, object source, FrameworkElement adorner) :
            base(routedEvent, source)
        {
            this.adorner = adorner;
        }

        public FrameworkElement Adorner
        {
            get
            {
                return adorner;
            }
        }
    }

    public delegate void AdornerEventHandler(object sender, AdornerEventArgs e);
}