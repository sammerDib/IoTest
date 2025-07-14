using System;
using System.IO;
using System.Windows;
using System.Windows.Markup;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components
{
    public class UserInterfaceManager : Notifier
    {
        public static string ThemeFolder { get; private set; }

        public void Initialize(UserInterfaceConfiguration configuration)
        {
            ThemeFolder = configuration.ThemeFolder;
            Theme = configuration.Theme;
            FontScale = configuration.FontScale;
            GlobalScale = configuration.GlobalScale;
        }

        private string _theme;
        public string Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                if (_theme == value) return;
                _theme = value;
                ApplyTheme();
                OnPropertyChanged();
            }
        }

        private double _previewFontScale = 1.0;

        public double PreviewFontScale
        {
            get
            {
                return _previewFontScale;
            }
            set
            {
                if (Math.Abs(_previewFontScale - value) < 0.01) return;
                _previewFontScale = value;
                OnPropertyChanged();
            }
        }

        private double _previewGlobalScale = 1.0;

        public double PreviewGlobalScale
        {
            get
            {
                return _previewGlobalScale;
            }
            set
            {
                if (Math.Abs(_previewGlobalScale - value) < 0.01) return;
                _previewGlobalScale = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalPreviewGlobalScale));
            }
        }

        public double FinalPreviewGlobalScale => 1 / GlobalScale * PreviewGlobalScale;

        private void ApplyTheme()
        {
            var path = GetThemePath(_theme);
            var dictionary = BuildThemeFromXaml(path);
            ThemeHelper.Apply(dictionary);
        }

        private double _fontScale = 1.0;

        public double FontScale
        {
            get
            {
                return _fontScale;
            }
            set
            {
                if (Math.Abs(_fontScale - value) < 0.01) return;
                _fontScale = value;
                OnPropertyChanged();
            }
        }

        private double _globalScale = 1.0;

        public double GlobalScale
        {
            get
            {
                return _globalScale;
            }
            set
            {
                if (Math.Abs(_globalScale - value) < 0.01) return;
                _globalScale = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FinalPreviewGlobalScale));
            }
        }

        public static string GetThemePath(string name)
        {
            return $"{ThemeFolder}{name}.xaml";
        }

        public static ResourceDictionary BuildThemeFromXaml(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);
            using StreamReader xamlStream = new StreamReader(path);
            var context = new ParserContext { XamlTypeMapper = new XamlTypeMapper(new string[] { }) };
            string xamlString = xamlStream.ReadToEnd();
            return XamlReader.Parse(xamlString, context) as ResourceDictionary;
        }
    }
}
