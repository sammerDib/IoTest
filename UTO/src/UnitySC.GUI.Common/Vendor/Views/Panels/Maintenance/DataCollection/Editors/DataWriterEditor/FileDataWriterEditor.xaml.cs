using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;

using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.DataWriter.File.StorageStrategy;
using Agileo.DataMonitoring.DataWriter.File.WriteEventTrigger;

using UnitsNet;

using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Editors.DataWriterEditor
{
    /// <summary>
    /// Interaction logic for FileWriterEditorView.xaml
    /// </summary>
    public partial class FileDataWriterEditor
    {
        #region Caches

        private static Dictionary<string, Type> _availableFileExtensionsCache;

        private static List<Type> _availableFileWritingStrategiesCache;

        private static List<Type> _availableFileStorageStrategiesCache;

        #endregion Caches

        #region Contructors

        public FileDataWriterEditor(FileDataWriter dataWriter = null)
        {
            InitializeComponent();
            AvailableFileExtensions = GetAvailableFileExtensionsFromCache();
            AvailableFileWritingStrategies = GetAvailableFileWritingStrategiesFromCache();
            AvailableFileStorageStrategies = GetAvailableFileStorageStrategiesFromCache();
            SetDefaultValues(dataWriter);
        }

        #endregion Constructors

        #region Properties

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        public KeyValuePair<string, Type> SelectedFileExtension
        {
            get { return (KeyValuePair<string, Type>)GetValue(SelectedFileExtensionProperty); }
            set { SetValue(SelectedFileExtensionProperty, value); }
        }

        public string StorageFolderPath
        {
            get { return (string)GetValue(StorageFolderPathProperty); }
            set { SetValue(StorageFolderPathProperty, value); }
        }

        public Type SelectedFileWriteStrategy
        {
            get { return (Type)GetValue(SelectedFileWriteStrategyProperty); }
            set { SetValue(SelectedFileWriteStrategyProperty, value); }
        }

        public Frequency FileWritingFrequency
        {
            get { return (Frequency)GetValue(FileWritingFrequencyProperty); }
            set { SetValue(FileWritingFrequencyProperty, value); }
        }

        public Type SelectedFileStorageStrategy
        {
            get { return (Type)GetValue(SelectedFileStorageStrategyProperty); }
            set { SetValue(SelectedFileStorageStrategyProperty, value); }
        }

        public Dictionary<string, Type> AvailableFileExtensions
        {
            get { return (Dictionary<string, Type>)GetValue(AvailableFileExtensionsProperty); }
            set { SetValue(AvailableFileExtensionsProperty, value); }
        }

        public List<Type> AvailableFileWritingStrategies
        {
            get { return (List<Type>)GetValue(AvailableFileWritingStrategiesProperty); }
            set { SetValue(AvailableFileWritingStrategiesProperty, value); }
        }

        public List<Type> AvailableFileStorageStrategies
        {
            get { return (List<Type>)GetValue(AvailableFileStorageStrategiesProperty); }
            set { SetValue(AvailableFileStorageStrategiesProperty, value); }
        }

        #endregion Properties

        #region DependencyProperties

        public static readonly DependencyProperty AvailableFileWritingStrategiesProperty = DependencyProperty.Register("AvailableFileWritingStrategies", typeof(List<Type>), typeof(FileDataWriterEditor), new PropertyMetadata(default(List<Type>)));
        public static readonly DependencyProperty AvailableFileStorageStrategiesProperty = DependencyProperty.Register("AvailableFileStorageStrategies", typeof(List<Type>), typeof(FileDataWriterEditor), new PropertyMetadata(default(List<Type>)));
        public static readonly DependencyProperty AvailableFileExtensionsProperty = DependencyProperty.Register("AvailableFileExtensions", typeof(Dictionary<string, Type>), typeof(FileDataWriterEditor), new PropertyMetadata(default(Dictionary<string, Type>)));
        public static readonly DependencyProperty SelectedFileStorageStrategyProperty = DependencyProperty.Register("SelectedFileStorageStrategy", typeof(Type), typeof(FileDataWriterEditor), new PropertyMetadata(default(Type)));
        public static readonly DependencyProperty FileWritingFrequencyProperty = DependencyProperty.Register("FileWritingFrequency", typeof(Frequency), typeof(FileDataWriterEditor), new PropertyMetadata(default(Frequency)));
        public static readonly DependencyProperty SelectedFileExtensionProperty = DependencyProperty.Register("SelectedFileExtension", typeof(KeyValuePair<string, Type>), typeof(FileDataWriterEditor), new PropertyMetadata(default(KeyValuePair<string, Type>)));
        public static readonly DependencyProperty SelectedFileWriteStrategyProperty = DependencyProperty.Register("SelectedFileWriteStrategy", typeof(Type), typeof(FileDataWriterEditor), new PropertyMetadata(default(Type), PropertyChangedCallback));
        public static readonly DependencyProperty StorageFolderPathProperty = DependencyProperty.Register("StorageFolderPath", typeof(string), typeof(FileDataWriterEditor), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileDataWriterEditor), new PropertyMetadata(default(string), PropertyChangedCallback));

        #endregion DependencyProperties

        #region Private

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileDataWriterEditor;
            if (control == null) return;
            switch (e.Property.Name)
            {
                case nameof(SelectedFileWriteStrategy):
                    control.FrequencyArea.Visibility = ((Type)e.NewValue).Name == nameof(CyclicFileWriteEventTrigger) ? Visibility.Visible : Visibility.Collapsed;
                    break;

                case nameof(FileName):
                    // if file name is wrong, do not take into account
                    if (control.FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                    {
                        control.FileName = e.OldValue?.ToString();
                    }
                    break;
            }
        }

        private void SetDefaultValues(FileDataWriter dataWriter)
        {
            FileWritingFrequency = (dataWriter?.WriteEventTrigger as CyclicFileWriteEventTrigger)?.WriteFrequency ?? Frequency.FromHertz(1);
            StorageFolderPath = dataWriter?.FileStorageStrategy.StorageFolderPath ?? "OutputFiles\\DataCollectionPlan\\CollectedData";
            SelectedFileWriteStrategy = dataWriter?.WriteEventTrigger.GetType() ?? typeof(CyclicFileWriteEventTrigger);
            SelectedFileStorageStrategy = dataWriter?.FileStorageStrategy.GetType() ?? typeof(TimestampedFileStorageStrategy);
            SelectedFileExtension = dataWriter != null ? AvailableFileExtensions.FirstOrDefault(kvp => kvp.Key == dataWriter.GetExtensionName()) : AvailableFileExtensions.FirstOrDefault();
            FileName = dataWriter?.FileStorageStrategy.FileName ?? string.Empty;
        }

        private static Dictionary<string, Type> GetAvailableFileExtensionsFromCache()
        {
            return _availableFileExtensionsCache ?? (_availableFileExtensionsCache = GetAvailableFileExtensions());
        }

        private static List<Type> GetAvailableFileWritingStrategiesFromCache()
        {
            return _availableFileWritingStrategiesCache ?? (_availableFileWritingStrategiesCache =
                GetTypesBasedOn(typeof(FileWriteEventTrigger)));
        }

        private static List<Type> GetAvailableFileStorageStrategiesFromCache()
        {
            return _availableFileStorageStrategiesCache ?? (_availableFileStorageStrategiesCache =
                GetTypesBasedOn(typeof(IFileStorageStrategy)));
        }

        private static Dictionary<string, Type> GetAvailableFileExtensions()
        {
            var availableFileExtensions = new Dictionary<string, Type>();

            var fileWriterTypes = GetTypesBasedOn(typeof(FileDataWriter));
            foreach (var type in fileWriterTypes)
            {
                var instance = Activator.CreateInstance(type, string.Empty, string.Empty);
                var toInvoke = type.GetMethod(nameof(FileDataWriter.GetExtensionName));
                var extension = toInvoke?.Invoke(instance, null);
                if (extension != null)
                {
                    availableFileExtensions.Add(extension.ToString(), type);
                }
            }

            return availableFileExtensions;
        }

        private static List<Type> GetTypesBasedOn(Type baseType)
        {
            var derivedTypes = new List<Type>();

            foreach (var s in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    derivedTypes.AddRange(s.GetTypes().Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract));
                }
                catch (ReflectionTypeLoadException e)
                {
                    derivedTypes.AddRange(e.Types.Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract));
                }
            }

            return derivedTypes;
        }

        private void ChooseStorageFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var currentPath = StorageFolderPath;
            var selectedPath = ShowOpenFolderDialog(currentPath);
            if (!string.IsNullOrWhiteSpace(selectedPath))
            {
                StorageFolderPath = GetRelativePath(selectedPath);
            }
        }

        private static string ShowOpenFolderDialog(string initialFolderPath)
        {
            string currentFullPath = null;
            var currentPathExists = false;
            if (!string.IsNullOrWhiteSpace(initialFolderPath))
            {
                try
                {
                    currentFullPath = Path.GetFullPath(initialFolderPath);
                    currentPathExists = Directory.Exists(currentFullPath);
                }
                catch
                {
                    // ignored
                }
            }

            var fileName = Assembly.GetEntryAssembly()?.Location;
            if (fileName != null)
            {
                var currentDirectoryPath = currentPathExists
                    ? currentFullPath
                    : new FileInfo(fileName).Directory?.FullName;
                var dlg = new FolderBrowserDialog
                {
                    ShowNewFolderButton = false,
                    SelectedPath = currentDirectoryPath
                };
                return dlg.ShowDialog() != DialogResult.OK ? null : dlg.SelectedPath;
            }

            return string.Empty;
        }

        private static string GetRelativePath(string filePath)
        {
            var fileName = Assembly.GetEntryAssembly()?.Location;
            if (fileName != null)
            {
                var currentFolderPath = new FileInfo(fileName).Directory?.FullName;
                if (string.IsNullOrEmpty(currentFolderPath)) return filePath;

                var pathUri = new Uri(filePath);
                // Folders must end in a slash
                if (!currentFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    currentFolderPath += Path.DirectorySeparatorChar;
                }

                var folderUri = new Uri(currentFolderPath);
                return Uri.UnescapeDataString(
                    folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
            }
            return string.Empty;
        }

        #endregion

        public bool OkCommandCanExecute()
        {
            return !string.IsNullOrWhiteSpace(FileName)
                   && FileWritingFrequency.Value > 0
                   && !string.IsNullOrWhiteSpace(StorageFolderPath)
                   && SelectedFileWriteStrategy != null
                   && SelectedFileStorageStrategy != null;
        }

        private void Info_OnMouseEnter(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement)?.Name == "StorageStrategyInfo")
            {
                StorageInfoPopup.IsOpen = true;
            }
            else
            {
                WritingInfoPopup.IsOpen = true;
            }
        }

        private void Info_OnMouseLeave(object sender, MouseEventArgs e)
        {
            if ((sender as FrameworkElement)?.Name == "StorageStrategyInfo")
            {
                StorageInfoPopup.IsOpen = false;
            }
            else
            {
                WritingInfoPopup.IsOpen = false;
            }
        }
    }
}
