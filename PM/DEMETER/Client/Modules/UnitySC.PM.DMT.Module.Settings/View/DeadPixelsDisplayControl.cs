using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UnitySC.PM.DMT.Modules.Settings.ViewModel;

namespace UnitySC.PM.DMT.Modules.Settings.View
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UnitySC.PM.DMT.CommonUI.ViewModel.Settings"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UnitySC.PM.DMT.CommonUI.ViewModel.Settings;assembly=UnitySC.PM.DMT.CommonUI.ViewModel.Settings"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DeadPixelsDisplay/>
    ///
    /// </summary>
    public class DeadPixelsDisplayControl : Control
    {
        static DeadPixelsDisplayControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeadPixelsDisplayControl), new FrameworkPropertyMetadata(typeof(DeadPixelsDisplayControl)));
        }



        public ObservableCollection<SelectableDeadPixel> DeadPixels
        {
            get { return (ObservableCollection<SelectableDeadPixel>)GetValue(DeadPixelsProperty); }
            set { SetValue(DeadPixelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DeadPixels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeadPixelsProperty =
            DependencyProperty.Register("DeadPixels", typeof(ObservableCollection<SelectableDeadPixel>), typeof(DeadPixelsDisplayControl), new PropertyMetadata(null,OnDeadPixelsChanged));

      

        private Canvas _deadPixelsCanvas;


            private List<DeadPixelControl> _deadPixelControls;

    
             public DeadPixelsDisplayControl()
            {
                _deadPixelControls = new List<DeadPixelControl>();
            }


            public override void OnApplyTemplate()
            {
                Canvas theCanvas = Template.FindName("deadPixelsCanvas", this) as Canvas;

                if (theCanvas != null)
                {
                //<-- Save a reference to the canvas
                    _deadPixelsCanvas = theCanvas;
                    //_pointsCanvas.PreviewMouseUp += _mainCanvas_PreviewMouseUp;
                    _deadPixelsCanvas.MouseUp += _mainCanvas_MouseUp;
                //_pointsCanvas.PreviewMouseDown += _mainCanvas_PreviewMouseDown;
                //<-- Do some stuff.

            
                    
                }
            }


        private void UpdateDeadPixelControls()
        {
            _deadPixelControls.Clear();
            _deadPixelsCanvas.Children.Clear();
 
            if (DeadPixels==null)
                return;
            
            foreach (var deadPixel in DeadPixels)
            {
                var deadPixelControl = new DeadPixelControl() { AssociatedDeadPixel = deadPixel };
                _deadPixelControls.Add(deadPixelControl);
                _deadPixelsCanvas.Children.Add(deadPixelControl);
            }

            DeadPixels.CollectionChanged += DeadPixels_CollectionChanged;
        }


        private static void OnDeadPixelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeadPixelsDisplayControl)d).UpdateDeadPixelControls();
            
        }

        private void DeadPixels_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
 
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                _deadPixelControls.Clear();
                _deadPixelsCanvas.Children.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (object obj in e.OldItems)
                    {
                        var deadPixelControl=_deadPixelControls.FirstOrDefault(dpc => dpc.AssociatedDeadPixel == obj);
                        if (deadPixelControl!=null)
                            _deadPixelsCanvas.Children.Remove(deadPixelControl);
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (object obj in e.NewItems)
                    {
                        if (_deadPixelControls.Count < 10000)
                        {
                            var newDeadPixel = new DeadPixelControl() { AssociatedDeadPixel = (SelectableDeadPixel)obj };
                            _deadPixelControls.Add(newDeadPixel);
                            _deadPixelsCanvas.Children.Add(newDeadPixel);
                        }
                    }
                }
            }


        }

        private void _mainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
            {
                // Retrieve the coordinates of the mouse button event.
                var pt = e.GetPosition((UIElement)sender);

             }


    



    }
}
