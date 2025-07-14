using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Agileo.Common.Localization;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Resources;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class ScreenshotHelper
    {
        static ScreenshotHelper()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(Global)));
        }

        public static void MakeCapture(string fileName, string path, FrameworkElement arg, BusinessPanel ownerPanel)
        {
            fileName += $"_{DateTime.Now:yyyyMMdd_hhmmss}";
            var fullPath = Path.Combine(path, fileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var screenShotFilePath = ExportVisual((int)arg.ActualHeight, (int)arg.ActualWidth, arg, fullPath);
            ShowUserMassage(screenShotFilePath, ownerPanel);
        }

        private static void ShowUserMassage(string path, BusinessPanel ownerPanel)
        {
            var message = new LocalizableText(nameof(Global.CAPTURE_SAVED_TO_LOCATION), path);
            var userMessage = new UserMessage(MessageLevel.Success, message)
            {
                SecondsDuration = 5
            };
            userMessage.Commands.Add(OpenFileDirectory.GetUserMessageCommand(nameof(Global.OPEN_FOLDER), path));
            userMessage.CanUserCloseMessage = true;
            ownerPanel.Messages.Show(userMessage);
        }

        private static string ExportVisual(int height, int width, Visual visual, string file)
        {
            var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(visual);

            var bitmapFrame = BitmapFrame.Create(bmp);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(bitmapFrame);

            var path = file + ".png";
            using Stream stream = File.Create(path);
            encoder.Save(stream);

            Clipboard.SetImage(bitmapFrame);

            return path;
        }
    }
}
