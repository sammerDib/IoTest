using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using BasicModules.Edition.Rendering.ViewModel;

namespace BasicModules.Edition.Rendering.Control
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DefectsControl : FrameworkElement
    {
        private readonly VisualCollection _visualDefects;
        private readonly List<DefectRectangle> _defectRetangles = new List<DefectRectangle>();

        public DefectsControl()
        {
            _visualDefects = new VisualCollection(this);
        }

        /// <summary>
        /// List des défauts à afficher
        /// </summary>
        public ObservableCollection<DefectViewModel> Defects
        {
            get { return (ObservableCollection<DefectViewModel>)GetValue(DefectsProperty); }
            set { SetValue(DefectsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Defects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefectsProperty =
            DependencyProperty.Register("Defects", typeof(ObservableCollection<DefectViewModel>), typeof(DefectsControl), new FrameworkPropertyMetadata(null, OnDefectChanged));


        public static void OnDefectChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DefectsControl defectsControl = GetDefectsControl(obj);
            var old = (ObservableCollection<DefectViewModel>)e.OldValue;
            if (old != null)
            {
                defectsControl.Defects.CollectionChanged -= defectsControl.Defects_CollectionChanged;
            }
            ((ObservableCollection<DefectViewModel>)e.NewValue).CollectionChanged += defectsControl.Defects_CollectionChanged;
            defectsControl.UpdateAllDefects(defectsControl.Defects);
        }

        private void Defects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                AddDefects(e.NewItems.Cast<DefectViewModel>());
        }

        /// Force la mise à jour de l'affichage de tous les défauts sur la wafer
        public bool IsUpToDate
        {
            get { return (bool)GetValue(IsUpToDateProperty); }
            set { SetValue(IsUpToDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsUpToDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUpToDateProperty =
            DependencyProperty.Register("IsUpToDate", typeof(bool), typeof(DefectsControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsUpToDateChanged));


        private static void OnIsUpToDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            DefectsControl defectsControl = GetDefectsControl(obj);
            if (!defectsControl.IsUpToDate)
            {
                defectsControl.UpdateAllDefects(defectsControl.Defects);
                defectsControl.IsUpToDate = true;
            }
        }

        /// <summary>
        /// Récupére le control courant pour les dependancy property
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        private static DefectsControl GetDefectsControl(DependencyObject sender)
        {
            DefectsControl defectsControl = null;
            if (sender is DefectsControl)
                defectsControl = (DefectsControl)sender;
            if (defectsControl == null)
                defectsControl = GetDefectsControl(sender);
            return defectsControl;
        }

        private void AddDefects(IEnumerable<DefectViewModel> defects)
        {
            if (defects == null)
                return;
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            foreach (var defect in defects.Where(x => x.ClassVM != null && x.ClassVM.IsSelected))
            {
                // Create a rectangle and draw it in the DrawingContext.
                Rect rect = new Rect(new Point(defect.LeftPosition, defect.TopPosition), new Size(defect.Width, defect.Height));
                drawingContext.DrawRectangle(new SolidColorBrush(defect.ClassVM.Color), null, rect);
                _defectRetangles.Add(new DefectRectangle() { Rect = rect, DefectId = defect.Id });
            }
            drawingContext.Close();
            _visualDefects.Add(drawingVisual);
        }

        private void UpdateAllDefects(IEnumerable<DefectViewModel> defects)
        {
            _visualDefects.Clear();
            _defectRetangles.Clear();
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            foreach (var defect in defects.Where(x => x.ClassVM.IsSelected))
            {
                // Create a rectangle and draw it in the DrawingContext.
                Rect rect = new Rect(new Point(defect.LeftPosition, defect.TopPosition), new Size(defect.Width, defect.Height));
                drawingContext.DrawRectangle(new SolidColorBrush(defect.ClassVM.Color), null, rect);
                _defectRetangles.Add(new DefectRectangle() { Rect = rect, DefectId = defect.Id });
            }
            drawingContext.Close();
            _visualDefects.Add(drawingVisual);
        }

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
    }

    internal class DefectRectangle
    {
        internal Rect Rect { get; set; }
        internal int DefectId { get; set; }
        internal bool IsInside(Point point)
        {
            return Rect.Contains(point);
        }
    }
}
