using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
    ///     <MyNamespace:DeadPixelControl/>
    ///
    /// </summary>
    public class DeadPixelControl : Control
    {
        private SelectableDeadPixel associatedDeadPixel;

        static DeadPixelControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DeadPixelControl), new FrameworkPropertyMetadata(typeof(DeadPixelControl)));
        }

        public DeadPixelControl()
        {
            this.Width = 3;
            this.Height = 3;

            var descriptorLeft = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(DeadPixelControl));
            descriptorLeft.AddValueChanged(this, OnCanvasLeftChanged);

            var descriptorTop = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(DeadPixelControl));
            descriptorTop.AddValueChanged(this, OnCanvasTopChanged);
            IsHitTestVisible = false;

        }

        private void OnCanvasLeftChanged(object sender, EventArgs e)
        {
        }


        private void OnCanvasTopChanged(object sender, EventArgs e)
        {
        }


        public SelectableDeadPixel AssociatedDeadPixel
        {
            get { return associatedDeadPixel; }
            set 
            { 
                associatedDeadPixel = value;
                Position = new Point(associatedDeadPixel.AssociatedDeadPixel.X, associatedDeadPixel.AssociatedDeadPixel.Y);
            }
        }




        public Point Position
        {
            get
            {
                var Y = Canvas.GetTop(this) + (int)this.Height / 2;
                var X = Canvas.GetLeft(this) + (int)this.Width / 2;
                return new Point(X, Y);
            }
            set
            {
                Canvas.SetTop(this, value.Y - (int)this.Width / 2);
                Canvas.SetLeft(this, value.X - (int)this.Height / 2);
            }
        }



    }
}
