using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.SeverityMessageViewer
{
    public class SeverityMessageViewerItem : Notifier
    {
        public enum SeverityEnum
        {
            Error,
            Warning,
            Info
        }

        #region Decoration

        private SolidColorBrush _colorBrush;

        public SolidColorBrush ColorBrush
        {
            get { return _colorBrush; }
            set
            {
                if (_colorBrush == value) return;
                _colorBrush = value;
                OnPropertyChanged(nameof(ColorBrush));
            }
        }

        private string _pathGeometry;

        public string PathGeometry
        {
            get { return _pathGeometry; }
            set
            {
                if (_pathGeometry == value) return;
                _pathGeometry = value;
                OnPropertyChanged(nameof(PathGeometry));
            }
        }

        #endregion Decoration

        public event EventHandler CollectionChanged;

        private void RaiseCollectionChanged() => CollectionChanged?.Invoke(this, new EventArgs());

        public SeverityEnum Severity { get; }

        public SeverityMessageViewerItem(SeverityEnum severityEnum)
        {
            Severity = severityEnum;
        }

        private readonly List<string> _messages = new List<string>();

        public ReadOnlyCollection<string> Messages => _messages.AsReadOnly();

        private List<string> _displayedMessages = new List<string>();

        public List<string> DisplayedMessages
        {
            get { return _displayedMessages; }
            set
            {
                if (_displayedMessages == value) return;
                _displayedMessages = value;
                OnPropertyChanged(nameof(DisplayedMessages));
            }
        }

        public bool AnyDisplayed => DisplayedMessages.Any();

        public void Add(string message)
        {
            if (_messages.Contains(message)) return;
            _messages.Add(message);
            RaiseCollectionChanged();
        }

        public void Remove(string message)
        {
            _messages.Remove(message);
            RaiseCollectionChanged();
        }

        public void Clear()
        {
            _messages.Clear();
            RaiseCollectionChanged();
        }

        public void Limit(int maxCount)
        {
            var messageList = new List<string>();
            for (var index = 0; index < _messages.Count; index++)
            {
                if (index < maxCount)
                {
                    messageList.Add(_messages[index]);
                }
                else break;
            }
            DisplayedMessages = messageList;
            OnPropertyChanged(nameof(AnyDisplayed));
        }

        public void UnLimit()
        {
            DisplayedMessages = _messages.ToList();
            OnPropertyChanged(nameof(AnyDisplayed));
        }
    }
}
