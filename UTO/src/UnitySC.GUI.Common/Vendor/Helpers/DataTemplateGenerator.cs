using System;
using System.Windows;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class DataTemplateGenerator
    {
        /// <summary>
        /// Creates and adds a <see cref="DataTemplate"/> to the application resources to make the association between the view and the view-model.
        /// </summary>
        /// <param name="typeOfViewModel">Type of the view-model</param>
        /// <param name="typeOfView">Type of the view</param>
        public static void Create(Type typeOfViewModel, Type typeOfView)
        {
            var dataTemplateKey = new DataTemplateKey(typeOfViewModel);
            if (Application.Current.Resources.Contains(dataTemplateKey)) return;
            var viewFactory = new FrameworkElementFactory(typeOfView);
            var dataTemplate = new DataTemplate
            {
                DataType = typeOfViewModel,
                VisualTree = viewFactory
            };
            Action action = () =>
            {
                Application.Current.Resources.Add(dataTemplateKey, dataTemplate);
            };
            //Use the dispatcher with highest thread priority to accelerate the rendering
            Application.Current.Dispatcher.BeginInvoke(action, DispatcherPriority.Send);
        }

        public static void CreateSync(Type typeOfViewModel, Type typeOfView)
        {
            var dataTemplateKey = new DataTemplateKey(typeOfViewModel);
            if (Application.Current.Resources.Contains(dataTemplateKey)) return;
            var viewFactory = new FrameworkElementFactory(typeOfView);
            var dataTemplate = new DataTemplate
            {
                DataType = typeOfViewModel,
                VisualTree = viewFactory
            };
            Action action = () =>
            {
                Application.Current.Resources.Add(dataTemplateKey, dataTemplate);
            };
            //Use the dispatcher with highest thread priority to accelerate the rendering
            Application.Current.Dispatcher.Invoke(action);
        }
    }
}
