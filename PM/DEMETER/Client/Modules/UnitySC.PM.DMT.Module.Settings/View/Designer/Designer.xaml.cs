using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace UnitySC.PM.DMT.Modules.Settings.View.Designer
{
    /// <summary>
    /// Interaction logic for Designer.xaml
    /// </summary>
    public partial class Designer : UserControl, ICollection<FrameworkElement>
    {

        /// <summary>
            /// Event to indicate that the drag is completed
            /// </summary>
        public event EventHandler DragCompleted;
        /// <summary>
        /// Called to signal to subscribers that drag is completed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDragCompleted(EventArgs e)
        {
            EventHandler eh = DragCompleted;
            if (eh != null)
            {
                eh(this, e);
            }
        }


        protected List<FrameworkElement> frameworkElements = new List<FrameworkElement>();

        //protected List<DrawingItem> drawingItems = new List<DrawingItem>();

        /// <summary>
        /// The <see cref="DrawingItems" /> dependency property's name.
        /// </summary>
        public const string DrawingItemsPropertyName = "DrawingItems";

        /// <summary>
        /// Gets or sets the value of the <see cref="DrawingItems" />
        /// property. This is a dependency property.
        /// </summary>
        public ObservableCollection<DrawingItem> DrawingItems
        {
            get
            {
                return (ObservableCollection<DrawingItem>)GetValue(DrawingItemsProperty);
            }
            set
            {
                SetValue(DrawingItemsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DrawingItems" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawingItemsProperty = DependencyProperty.Register(
            DrawingItemsPropertyName,
            typeof(ObservableCollection<DrawingItem>),
            typeof(Designer),
            new UIPropertyMetadata(null,OnDrawingItemsChanged));

        private static void OnDrawingItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue == null)
                return;

            var drawingItems = (ObservableCollection<DrawingItem>)e.NewValue;
            // Subscribe to CollectionChanged on the new collection
            //coll.CollectionChanged += action;

            (d as Designer)?.Clear();

            foreach (var drawingItem in drawingItems)
            {
                (d as Designer)?.Add(drawingItem);

            }
        }

        private void Add(DrawingItem drawingItem)
        {
            if (drawingItem is PolygonDrawingItem)
                Add(drawingItem as PolygonDrawingItem);
            if (drawingItem is EllipseDrawingItem)
                Add(drawingItem as EllipseDrawingItem);
        }

        public Designer()
        {
            InitializeComponent();
        }

        #region ICollection<FrameworkElement> Members

        public void Add(FrameworkElement item)
        {
            this.frameworkElements.Add(item);
            //if (item is Path)
            //    this.DesignArea.Children.Add(new PathDesignerComponent(item));
            //else
                this.DesignArea.Children.Add(new DesignerComponent(item));

        }


        public void Add(PolygonDrawingItem polygonItem)
        {
            var path = new Path();
            path.Fill = new SolidColorBrush(Colors.Black);
            path.Width = this.Width;
            path.Height = this.Height;
            this.frameworkElements.Add(path);
            var pathComponent = new PathDesignerComponent(path, polygonItem);
            AddDesignAreaChild(pathComponent);
        }

     

        public void Add(EllipseDrawingItem ellipseItem)
        {
            var ellipse = new Ellipse();

            ellipse.Fill = new SolidColorBrush(Colors.Black);
            this.frameworkElements.Add(ellipse);

            var ellipseComponent = new EllipseDesignerComponent(ellipse, ellipseItem);
            AddDesignAreaChild(ellipseComponent);

        }

        private void AddDesignAreaChild(UIElement uiElement)
        {
            if (uiElement is INotify)
                (uiElement as INotify).DragCompleted += Component_DragCompleted;
            this.DesignArea.Children.Add(uiElement);
        }

        private void Component_DragCompleted(object sender, EventArgs e)
        {
            OnDragCompleted(new EventArgs());
        }

        public void Clear()
        {
            foreach (var designAreaChild in this.DesignArea.Children)
            {
                if (designAreaChild is INotify)
                    (designAreaChild as INotify).DragCompleted -= Component_DragCompleted;
            }
            this.frameworkElements.Clear();
            this.DesignArea.Children.Clear();
            GC.Collect();
        }

        public bool Contains(FrameworkElement item)
        {
            return this.frameworkElements.Contains(item);
        }

        public void CopyTo(FrameworkElement[] array, int arrayIndex)
        {
            this.frameworkElements.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.frameworkElements.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(FrameworkElement item)
        {
            if (frameworkElements.Remove(item))
            {
                for (int i = 0; i < DesignArea.Children.Count; i++)
                    if (((DesignerComponent)DesignArea.Children[i]).Content == item)
                        DesignArea.Children.RemoveAt(i);
                return true;
            }
            return false;
        }

       

        #endregion

        #region IEnumerable<FrameworkElement> Members

        public IEnumerator<FrameworkElement> GetEnumerator()
        {
            return frameworkElements.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return frameworkElements.GetEnumerator();
        }

        #endregion

        #region Event Handlers

        private void DesignArea_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (FrameworkElement fElem in DesignArea.Children)
            {
                if (fElem is PathDesignerComponent)
                    ((PathDesignerComponent)fElem).IsSelected = false;
                else
                    ((EllipseDesignerComponent)fElem).IsSelected = false;
            }
        }

        #endregion

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (var drawingItem in frameworkElements)
            {
                if (drawingItem is Path)
                {
                    drawingItem.Width = e.NewSize.Width;
                    drawingItem.Height = e.NewSize.Height;
                }

            }
        }
    }
}
