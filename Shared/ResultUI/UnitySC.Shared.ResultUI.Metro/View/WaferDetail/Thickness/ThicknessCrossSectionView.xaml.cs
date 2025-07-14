using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;

namespace UnitySC.Shared.ResultUI.Metro.View.WaferDetail.Thickness
{
    /// <summary>
    /// Interaction logic for ThicknessCrossSectionView.xaml
    /// </summary>
    public partial class ThicknessCrossSectionView
    {
        public ThicknessCrossSectionVM ViewModel => DataContext as ThicknessCrossSectionVM;
        
        public ThicknessCrossSectionView()
        {
            InitializeComponent();
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;

            SetBinding(CenterRotationPointProperty, nameof(ThicknessCrossSectionVM.RadialDraggingFlag));
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            CenterRotationPoint = null;
        }

        private double _initialDifference;

        private double GetDegree(Point currentPosition)
        {
            if (CenterRotationPoint == null) return double.NaN;

            // Calculate an angle
            double radians = Math.Atan((currentPosition.Y - CenterRotationPoint.Value.Y) / (currentPosition.X - CenterRotationPoint.Value.X));
            return (radians + 1.5) * 180 / Math.PI;
        }

        private static double ReduceAngleOutOfBounds(double angle)
        {
            // Apply a 180 degree shift when angle is negative or greater than 180
            while (angle < 0) angle += 180;
            while (angle > 180) angle -= 180;
            return angle;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (CenterRotationPoint != null)
            {
                // Get the current mouse position relative to the volume control
                var currentLocation = Mouse.GetPosition(this);

                double currentDegree = GetDegree(currentLocation) + _initialDifference;

                currentDegree = ReduceAngleOutOfBounds(currentDegree);

                ViewModel.CrossSectionHeatMap.RadialeProfileAngle = currentDegree;
                
                Debug.WriteLine("New degree = " + currentDegree);
            }
            else
            {
                CenterRotationPoint = null;
            }
        }

        public static readonly DependencyProperty CenterRotationPointProperty = DependencyProperty.Register(
            nameof(CenterRotationPoint), typeof(Point?), typeof(ThicknessCrossSectionView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));


        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThicknessCrossSectionView self)
            {
                if (self.CenterRotationPoint == null) return;

                // Prevent mouse position delta by storing the delta value in degrees between the current value of the ViewModel and the value calculated here.
                var initialLocation = Mouse.GetPosition(self);
                double currentDegree = self.GetDegree(initialLocation);
                double initialDegree = self.ViewModel.CrossSectionHeatMap.RadialeProfileAngle;
                double difference = initialDegree - currentDegree;
                difference = ReduceAngleOutOfBounds(difference);
                self._initialDifference = difference;

                Mouse.Capture(self);
            }
        }

        public Point? CenterRotationPoint
        {
            get { return (Point?)GetValue(CenterRotationPointProperty); }
            set { SetValue(CenterRotationPointProperty, value); }
        }
    }
}
