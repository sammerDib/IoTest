using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

using AdcTools;

namespace AdcBasicObjects.Rendering
{
    internal class ClusterControl : FrameworkElement
    {
        public ClusterControl()
        {
            _visualDefects = new VisualCollection(this);
        }

        //=================================================================
        // Affichage du Visual
        //=================================================================
        private readonly VisualCollection _visualDefects;

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount => _visualDefects.Count;


        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _visualDefects.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _visualDefects[index];
        }

        private void Redraw()
        {
            _visualDefects.Clear();

            if (Clusters.IsNullOrEmpty())
                return;

            Pen pen = new Pen(Brushes.Red, 1 / Scale);
            double scale = Scale;
            double xmin = XMin;
            double ymin = YMin;
            double minsize = 4 / Scale;

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            foreach (Cluster cluster in Clusters)
            {
                System.Drawing.Rectangle r = cluster.pixelRect;
                double x = r.X - xmin;
                double y = r.Y - ymin;
                double w = Math.Max(r.Width * scale, minsize);
                double h = Math.Max(r.Height * scale, minsize);

                Rect rect = new Rect(x, y, r.Width, r.Height);
                drawingContext.DrawRectangle(Brushes.Red, null, rect);
            }
            drawingContext.Close();
            _visualDefects.Add(drawingVisual);
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
            DependencyProperty.Register("Clusters", typeof(ObservableCollection<Cluster>), typeof(ClusterControl), new PropertyMetadata(null, new PropertyChangedCallback(ClustersPropertyChanged)));

        public double XMin
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }
        public static readonly DependencyProperty MinXProperty =
            DependencyProperty.Register("XMin", typeof(double), typeof(ClusterControl), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register("XMax", typeof(double), typeof(ClusterControl), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register("YMin", typeof(double), typeof(ClusterControl), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));


        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }
        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register("YMax", typeof(double), typeof(ClusterControl), new PropertyMetadata(0.0, new PropertyChangedCallback(XYPropertyChanged)));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(ClusterControl), new PropertyMetadata(0.0, new PropertyChangedCallback(ScalePropertyChanged)));


        //=================================================================
        // Gestion des clusters
        //=================================================================

        private static void ClustersPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ClusterControl myThis = dependencyObject as ClusterControl;
            myThis.ClustersPropertyChanged(eventArgs);
        }

        private void ClustersPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            ObservableCollection<Cluster> oldlist = eventArgs.OldValue as ObservableCollection<Cluster>;
            if (oldlist != null)
                oldlist.CollectionChanged -= ClusterListChanged;

            ObservableCollection<Cluster> newlist = eventArgs.NewValue as ObservableCollection<Cluster>;
            if (newlist != null)
                newlist.CollectionChanged += ClusterListChanged;
            ClusterListChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void ClusterListChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                //if (Clusters != null)
                //{
                //    foreach (Cluster cluster in Clusters)
                //        AddCluster(cluster);
                //}
                Redraw();
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

        private void AddCluster(Cluster cluster)
        {
            //Shape shape;

            //if (UsePixelUnit)
            //{
            //    System.Drawing.Rectangle r = cluster.pixelRect;

            //    shape = new System.Windows.Shapes.Rectangle();
            //    shape.Width = r.Width;
            //    shape.Height = r.Height;
            //    Canvas.SetLeft(shape, r.Left);
            //    Canvas.SetTop(shape, r.Top);
            //}
            //else
            //{
            //    var polygon = new System.Windows.Shapes.Polygon();
            //    foreach (var c in cluster.micronQuad.corners)
            //        polygon.Points.Add(new Point(c.X, c.Y));

            //    shape = polygon;
            //}

            //shape.Fill = ClusterBrush;
            //shape.Stroke = Brushes.Red;
            //shape.SetBinding(Shape.StrokeThicknessProperty, new Binding("StrokeThickness"));
            //shape.SetBinding(Shape.MinWidthProperty, new Binding("StrokeThickness"));
            //shape.SetBinding(Shape.MinHeightProperty, new Binding("MinRectangleSize"));
            //Shapes[cluster] = shape;
            //TheCanvas.Children.Add(shape);
        }

        private void RemoveCluster(Cluster cluster)
        {
            //Shape shape = Shapes[cluster];
            //TheCanvas.Children.Remove(shape);
            //Shapes.Remove(cluster);
        }


        //=================================================================
        // Couleurs
        //=================================================================
        private static Brush ClusterBrush = CreateTransparentBrush(System.Windows.Media.Colors.IndianRed);

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
            ClusterControl myThis = dependencyObject as ClusterControl;
            myThis.XYPropertyChanged(eventArgs);
        }

        private void XYPropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            Redraw();
        }

        private static void ScalePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            ClusterControl myThis = dependencyObject as ClusterControl;
            myThis.ScalePropertyChanged(eventArgs);
        }

        private void ScalePropertyChanged(DependencyPropertyChangedEventArgs eventArgs)
        {
            Redraw();
        }


    }
}
