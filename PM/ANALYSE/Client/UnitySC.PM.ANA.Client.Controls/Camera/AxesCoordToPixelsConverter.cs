using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

using UnitySC.PM.ANA.Client.Proxy;

namespace UnitySC.PM.ANA.Client.Controls.Camera
{
    public class AxesCoordToPixelsConverterX : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!double.TryParse(values[0].ToString(), out double positionPointAxesCoord))
                return DependencyProperty.UnsetValue;

            // Camera position in axes coordinates
            if (!double.TryParse(values[1].ToString(), out double positionCameraAxesCoord))
                return DependencyProperty.UnsetValue;
            // PixelSize
            if (!double.TryParse(values[2].ToString(), out double pixelSizemm))
                return DependencyProperty.UnsetValue;
            // Size of the displayed image in pixels
            if (!double.TryParse(values[3].ToString(), out double actualImageSizeInPixels))
                return DependencyProperty.UnsetValue;
            // Size of the real image in pixels
            if (!double.TryParse(values[4].ToString(), out double realImageSizeInPixels))
                return DependencyProperty.UnsetValue;

            // Position in pixels with origin at the center of the image
            var posInPixels = (positionPointAxesCoord - positionCameraAxesCoord) / pixelSizemm;

            // Scale and convert to canvas pixels
            var actualPosInPixels = posInPixels * actualImageSizeInPixels / realImageSizeInPixels + actualImageSizeInPixels / 2;

            return actualPosInPixels;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class AxesCoordToPixelsConverterY : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!double.TryParse(values[0].ToString(), out double positionPointAxesCoord))
                return DependencyProperty.UnsetValue;

            // Camera position in axes coordinates
            if (!double.TryParse(values[1].ToString(), out double positionCameraAxesCoord))
                return DependencyProperty.UnsetValue;
            // PixelSize
            if (!double.TryParse(values[2].ToString(), out double pixelSizemm))
                return DependencyProperty.UnsetValue;
            // Size of the displayed image in pixels
            if (!double.TryParse(values[3].ToString(), out double actualImageSizeInPixels))
                return DependencyProperty.UnsetValue;
            // Size of the real image in pixels
            if (!double.TryParse(values[4].ToString(), out double realImageSizeInPixels))
                return DependencyProperty.UnsetValue;

            // Position in pixels with origin at the center of the image
            var posInPixels = -(positionPointAxesCoord - positionCameraAxesCoord) / pixelSizemm;

            // Scale and convert to canvas pixels
            var actualPosInPixels = posInPixels * actualImageSizeInPixels / realImageSizeInPixels + actualImageSizeInPixels / 2;

            return actualPosInPixels;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class AxesSizeToPixelsConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((!(parameter is CameraDisplay)) || (!(value is Size)))
                return DependencyProperty.UnsetValue;

            var cameraDisplay = parameter as CameraDisplay;
            var sizeAxes = (Size)value;

            // Converts pixel sizes
            var sizePixels = new Size(sizeAxes.Width / ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters, sizeAxes.Height / ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters);

            // Size in pixels on the displayed image
            var sizeDisplay = new Size(sizePixels.Width * cameraDisplay.CameraImage.ActualWidth / ServiceLocator.CamerasSupervisor.Camera.ImageWidth, sizePixels.Height * cameraDisplay.CameraImage.ActualHeight / ServiceLocator.CamerasSupervisor.Camera.ImageHeight);

            return sizeDisplay;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return new Size(20, 50);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((!(parameter is CameraDisplay)) || (!(value is Size)))
                return DependencyProperty.UnsetValue;

            var cameraDisplay = parameter as CameraDisplay;
            var sizeDisplay = (Size)value;

            // Size in real pixels of the image
            var sizePixels = new Size(sizeDisplay.Width * ServiceLocator.CamerasSupervisor.Camera.ImageWidth / cameraDisplay.CameraImage.ActualWidth, sizeDisplay.Height * ServiceLocator.CamerasSupervisor.Camera.ImageHeight / cameraDisplay.CameraImage.ActualHeight);

            // Converts pixels to axes units
            var sizeAxes = new Size(sizePixels.Width * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters, sizePixels.Height * ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeY.Millimeters);

            return sizeAxes;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    [ContentProperty(nameof(Binding))]
    public class ConverterBindableParameter : MarkupExtension
    {
        #region Public Properties

        public Binding Binding { get; set; }
        public BindingMode Mode { get; set; }
        public IValueConverter Converter { get; set; }
        public Binding ConverterParameter { get; set; }

        #endregion Public Properties

        public ConverterBindableParameter()
        { }

        public ConverterBindableParameter(string path)
        {
            Binding = new Binding(path);
        }

        public ConverterBindableParameter(Binding binding)
        {
            Binding = binding;
        }

        #region Overridden Methods

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var multiBinding = new MultiBinding();
            Binding.Mode = Mode;
            multiBinding.Bindings.Add(Binding);
            if (ConverterParameter != null)
            {
                ConverterParameter.Mode = BindingMode.OneWay;
                multiBinding.Bindings.Add(ConverterParameter);
            }
            var adapter = new MultiValueConverterAdapter
            {
                Converter = Converter
            };
            multiBinding.Converter = adapter;
            return multiBinding.ProvideValue(serviceProvider);
        }

        #endregion Overridden Methods

        [ContentProperty(nameof(Converter))]
        private class MultiValueConverterAdapter : IMultiValueConverter
        {
            public IValueConverter Converter { get; set; }

            private object lastParameter;

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                if (Converter == null) return values[0]; // Required for VS design-time
                if (values.Length > 1) lastParameter = values[1];
                return Converter.Convert(values[0], targetType, lastParameter, culture);
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                if (Converter == null) return new object[] { value }; // Required for VS design-time

                return new object[] { Converter.ConvertBack(value, targetTypes[0], lastParameter, culture) };
            }
        }
    }
}
