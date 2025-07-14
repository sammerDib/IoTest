using System.Windows;
using System.Windows.Media;

using UnitySC.Shared.Data;

namespace UnitySC.Shared.UI.Controls
{
    public class Wafer : FrameworkElement
    {
        private readonly VisualCollection _visualCollection;

        public Wafer()
        {
            Height = 1000;
            Width = 1000;
            _visualCollection = new VisualCollection(this);
            _visualCollection.Add(new WaferVisual(new WaferDimensionalCharacteristic(), Width, Height));
        }

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get { return (WaferDimensionalCharacteristic)GetValue(WaferDimentionalCharacProperty); }
            set { SetValue(WaferDimentionalCharacProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaferDimention.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaferDimentionalCharacProperty =
            DependencyProperty.Register("WaferDimentionalCharac", typeof(WaferDimensionalCharacteristic), typeof(Wafer), new FrameworkPropertyMetadata(null, OnWaferChanged));

        public static void OnWaferChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var wafer = GetWaferControl(obj);
            wafer.Update();
        }

        public void Update()
        {
            _visualCollection.Clear();
            if (WaferDimentionalCharac != null)
                _visualCollection.Add(new WaferVisual(WaferDimentionalCharac, Width, Height));
        }

        protected override int VisualChildrenCount => _visualCollection.Count;

        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        private static Wafer GetWaferControl(DependencyObject sender)
        {
            Wafer defectsControl = null;
            if (sender is Wafer)
                defectsControl = (Wafer)sender;
            if (defectsControl == null)
                defectsControl = GetWaferControl(sender);
            return defectsControl;
        }
    }
}