using System;
using System.Windows;
using System.Windows.Controls;

//namespace AdcBasicObjects.Rendering
namespace BasicModules.Edition.Rendering
{
    /// <summary>
    /// Interaction logic for ImageRendering.xaml
    /// </summary>
    public partial class ResultView : UserControl
    {

        private static ResultView instance;

        public static ResultView Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResultView();
                }
                return instance;
            }
        }


        public ResultView()
        {
            InitializeComponent();
            instance = this;
        }


        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            //((ImageRendering)DataContext).Cleanup();
            //	CleanRectangles();

        }


        public void CleanRectangles()
        {
            //   List<Rectangle> rectangles = myCanvas.Children.OfType<Rectangle>().ToList<Rectangle>();
            //foreach (Rectangle rectangle in rectangles)
            //{
            //	myCanvas.Children.Remove(rectangle);
            //}
            //rectangles.Clear();

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
    }
}
