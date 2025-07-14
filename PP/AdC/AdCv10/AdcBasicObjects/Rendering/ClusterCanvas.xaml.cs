using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

using ADCEngine;

namespace AdcBasicObjects.Rendering
{
    /// <summary>
    /// Interaction logic for ClusterCanvas.xaml
    /// </summary>
    public partial class ClusterCanvas : UserControl
    {
        public ClusterCanvas()
        {
            InitializeComponent();
        }

        //=================================================================
        // Dependency properties
        //=================================================================

        public ObservableCollection<Cluster> Clusters
        {
            get { return (ObservableCollection<Cluster>)GetValue(ClustersProperty); }
            set { SetValue(ClustersProperty, value); }
        }
        public static readonly DependencyProperty ClustersProperty =
            DependencyProperty.Register("Clusters", typeof(ObservableCollection<Cluster>), typeof(ClusterCanvas), new PropertyMetadata(null, new PropertyChangedCallback(ClustersPropertyChanged)));


        public bool UsePixelUnit
        {
            get { return (bool)GetValue(UsePixelUnitProperty); }
            set { SetValue(UsePixelUnitProperty, value); }
        }
        public static readonly DependencyProperty UsePixelUnitProperty =
            DependencyProperty.Register("UsePixelUnit", typeof(bool), typeof(ClusterCanvas), new PropertyMetadata(true));


        public double XMin
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("XMin", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register("XMax", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register("YMin", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register("YMax", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0, new PropertyChangedCallback(ScalePropertyChanged)));


        //=================================================================
        // TODO propriétés normales
        //=================================================================
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private double _verticalDirection = 1;
        public double VerticalDirection
        {
            get => _verticalDirection; set { if (_verticalDirection != value) { _verticalDirection = value; RaisePropertyChanged(); } }
        }

        public double SizeX
        {
            get { return (double)GetValue(SizeXProperty); }
            set { SetValue(SizeXProperty, value); }
        }
        public static readonly DependencyProperty SizeXProperty =
            DependencyProperty.Register("SizeX", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));


        public double SizeY
        {
            get { return (double)GetValue(SizeYProperty); }
            set { SetValue(SizeYProperty, value); }
        }
        public static readonly DependencyProperty SizeYProperty =
            DependencyProperty.Register("SizeY", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));


        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }
        public static readonly DependencyProperty OffsetXProperty =
            DependencyProperty.Register("OffsetX", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));


        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }
        public static readonly DependencyProperty OffsetYProperty =
            DependencyProperty.Register("OffsetY", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));


        public WaferBase Wafer
        {
            get { return (WaferBase)GetValue(WaferProperty); }
            set { SetValue(WaferProperty, value); }
        }
        public static readonly DependencyProperty WaferProperty =
            DependencyProperty.Register("Wafer", typeof(WaferBase), typeof(ClusterCanvas), new PropertyMetadata(null));


        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));



        public double MinRectangleSize
        {
            get { return (double)GetValue(MinRectangleSizeProperty); }
            set { SetValue(MinRectangleSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinRectangleSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinRectangleSizeProperty =
            DependencyProperty.Register("MinRectangleSize", typeof(double), typeof(ClusterCanvas), new PropertyMetadata(0.0));




        //=================================================================
        // Gestion des clusters
        //=================================================================

        private static void ClustersPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ClusterCanvas myThis = dependencyObject as ClusterCanvas;
            myThis.ClustersPropertyChanged(eventArgs);
        }

        private void ClustersPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            ObservableCollection<Cluster> oldlist = eventArgs.OldValue as ObservableCollection<Cluster>;
            if (oldlist != null)
                oldlist.CollectionChanged -= new NotifyCollectionChangedEventHandler(ClusterListChanged);

            ObservableCollection<Cluster> newlist = eventArgs.NewValue as ObservableCollection<Cluster>;
            if (newlist != null)
                newlist.CollectionChanged += new NotifyCollectionChangedEventHandler(ClusterListChanged);
            ClusterListChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void ClusterListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                TheCanvas.Children.Clear();
                Shapes.Clear();
                if (Clusters != null)
                {
                    foreach (Cluster cluster in Clusters)
                        AddCluster(cluster);
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (Cluster cluster in e.OldItems)
                        RemoveCluster(cluster);
                }

                if (e.NewItems != null)
                {
                    foreach (Cluster cluster in e.NewItems)
                        AddCluster(cluster);
                }
            }
        }

        private Dictionary<Cluster, Shape> Shapes = new Dictionary<Cluster, Shape>();

        private void AddCluster(Cluster cluster)
        {
            Shape shape;

            if (UsePixelUnit)
            {
                System.Drawing.Rectangle r = cluster.pixelRect;

                shape = new System.Windows.Shapes.Rectangle();
                shape.Width = r.Width;
                shape.Height = r.Height;
                Canvas.SetLeft(shape, r.Left);
                Canvas.SetTop(shape, r.Top);
            }
            else
            {
                var polygon = new System.Windows.Shapes.Polygon();
                foreach (var c in cluster.micronQuad.corners)
                    polygon.Points.Add(new Point(c.X, c.Y));

                shape = polygon;
            }

            shape.Stroke = Brushes.Red;
            //shape.Fill = ClusterBrush;
            if (Shapes.Count() < 1000)
            {
                shape.SetBinding(Shape.StrokeThicknessProperty, new Binding("StrokeThickness"));
                shape.SetBinding(Shape.MinWidthProperty, new Binding("MinRectangleSize"));
                shape.SetBinding(Shape.MinHeightProperty, new Binding("MinRectangleSize"));
            }
            Shapes[cluster] = shape;
            TheCanvas.Children.Add(shape);
        }

        private void RemoveCluster(Cluster cluster)
        {
            Shape shape = Shapes[cluster];
            TheCanvas.Children.Remove(shape);
            Shapes.Remove(cluster);
        }

        //=================================================================
        // Couleurs
        //=================================================================
        public static Brush ClusterBrush = CreateTransparentBrush(System.Windows.Media.Colors.IndianRed);

        private static Brush CreateTransparentBrush(Color c)
        {
            c.A = 128;
            Brush b = new SolidColorBrush(c);
            return b;
        }

        //=================================================================
        // Gestion des XMin/XMax/YMin/YMax/Scale
        //=================================================================

        private static void XYPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ClusterCanvas myThis = dependencyObject as ClusterCanvas;
            myThis.XYPropertyChanged(eventArgs);
        }

        private void XYPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            SizeX = XMax - XMin;
            SizeY = YMax - YMin;
            OffsetX = -XMin - SizeX / 2;
            OffsetY = -YMin - SizeY / 2;
        }

        private static void ScalePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ClusterCanvas myThis = dependencyObject as ClusterCanvas;
            myThis.ScalePropertyChanged(eventArgs);
        }

        private void ScalePropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            StrokeThickness = 1 / Scale;
            MinRectangleSize = 4 / Scale;
        }

        //=================================================================
        // TODO gestion du wafer
        //=================================================================


    }
}
