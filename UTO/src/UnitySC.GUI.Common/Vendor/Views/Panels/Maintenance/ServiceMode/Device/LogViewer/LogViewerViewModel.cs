using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Tracing;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

using Microsoft.Win32;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public sealed class LogViewerViewModel : Notifier, IListener, IDisposable
    {
        /// <summary>
        /// Maximum lines number of the LogViewer
        /// Once this limit is reached, it acts as a circular buffer : the first line is deleted before adding the new
        /// </summary>
        private const int MaximumLinesNumber = 1000;

        public Agileo.EquipmentModeling.Device Device { get; }

        public ICommand ClearLogsCommand { get; }

        public ICommand SaveAsCommand { get; }

        public LogViewerViewModel(Agileo.EquipmentModeling.Device device)
        {
            Device = device;
            ClearLogsCommand = new DelegateCommand(DeleteLogs);
            SaveAsCommand = new DelegateCommand(SaveLogs);

            // Events
            TraceManager.Instance().AddListener(this);
        }

        #region Listener Implementation

        public string Name
        {
            get => string.Empty;
            set => throw new NotImplementedException();
        }

        public void Close()
        {
            // Method intentionally left empty.
        }

        public void DoLog(TraceLine traceLine)
        {
            if (traceLine.Source != Device.QualifiedName) return;

            string text = $"{traceLine.Timestamp:h:mm:ss.fff} [{Enum.GetName(typeof(TraceLevelType),traceLine.LogLevel)}] {traceLine.Text}";
            Logs.Add(text);

            if (Logs.Count > MaximumLinesNumber)
            {
                Logs.RemoveAt(0);
                OnNewTraceAdded(new NewTraceEventArgs(text, true, traceLine));
            }
            else
            {
                OnNewTraceAdded(new NewTraceEventArgs(text, false, traceLine));
            }
        }
        #endregion

        public List<string> Logs { get; } = new();

        public Highlighter SyntaxHighlighting { get; } = new();

        #region Zoom

        public IReadOnlyList<int> PossibleZooms { get; } = new List<int> { 20, 50, 70, 100, 150, 200, 400 };

        private int _selectedZoom = 100;

        public int SelectedZoom
        {
            get { return _selectedZoom; }
            set
            {
                if (value > PossibleZooms.Max()) value = PossibleZooms.Max();
                if (value < PossibleZooms.Min()) value = PossibleZooms.Min();
                _selectedZoom = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LogFontSize));
            }
        }

        public void IncreaseFont() => SelectedZoom += 10;

        public void DecreaseFont() => SelectedZoom -= 10;

        public int LogFontSize => _selectedZoom * 14 / 100;

        #endregion Zoom

        public event EventHandler<EventArgs> LogsCleared;

        private void DeleteLogs()
        {
            Logs.Clear();
            LogsCleared?.Invoke(this, EventArgs.Empty);
        }

        private void SaveLogs()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt"
            };
            saveFileDialog.FileOk += SaveFileDialog_FileOk;
            saveFileDialog.ShowDialog();
        }

        private void SaveFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            File.WriteAllText(((SaveFileDialog)sender).FileName, string.Join("\n", Logs));
        }

        public event EventHandler<NewTraceEventArgs> NewTraceAdded;

        private void OnNewTraceAdded(NewTraceEventArgs args)
        {
            NewTraceAdded?.Invoke(this, args);
        }

        public void Dispose()
        {
            TraceManager.Instance().RemoveListener(this);
        }
    }
}
