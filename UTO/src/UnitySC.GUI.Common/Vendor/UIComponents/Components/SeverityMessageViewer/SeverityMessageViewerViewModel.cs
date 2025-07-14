using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.SeverityMessageViewer
{
    public class SeverityMessageViewerViewModel : Notifier, IDisposable
    {
        public SeverityMessageViewerViewModel()
        {
            foreach (var messagesViewerItem in MessagesViewers)
            {
                messagesViewerItem.CollectionChanged += MessagesViewerItem_CollectionChanged;
            }
        }

        private void MessagesViewerItem_CollectionChanged(object sender, EventArgs e)
        {
            UpdateMessageItems();
        }

        private void UpdateMessageItems()
        {
            OnPropertyChanged(nameof(MoreIsDisplayed));
            OnPropertyChanged(nameof(ReduceIsDisplayed));
            if (!_showAll)
            {
                int value = MessageCountLimitation;
                foreach (var messagesViewerItem in MessagesViewers)
                {
                    messagesViewerItem.Limit(value);
                    if (messagesViewerItem.Messages.Count > value)
                    {
                        value = 0;
                    }
                    else
                    {
                        value -= messagesViewerItem.Messages.Count;
                    }
                }
            }
            else
            {
                foreach (var messagesViewerItem in MessagesViewers)
                {
                    messagesViewerItem.UnLimit();
                }
            }
        }

        public List<SeverityMessageViewerItem> MessagesViewers { get; } = new()
        {
            new (SeverityMessageViewerItem.SeverityEnum.Error),
            new (SeverityMessageViewerItem.SeverityEnum.Warning),
            new (SeverityMessageViewerItem.SeverityEnum.Info)
        };

        public SeverityMessageViewerItem Errors => MessagesViewers[0];

        public SeverityMessageViewerItem Warnings => MessagesViewers[1];

        public SeverityMessageViewerItem Infos => MessagesViewers[2];

        public void Clear()
        {
            foreach (var messagesViewerItem in MessagesViewers)
            {
                messagesViewerItem.Clear();
            }
        }

        public int MessageCountLimitation { get; set; } = 3;

        private bool _showAll;

        public Visibility MoreIsDisplayed
        {
            get
            {
                if (_showAll) return Visibility.Collapsed;
                return MessagesViewers.Sum(messagesViewerItem => messagesViewerItem.Messages.Count) >
                       MessageCountLimitation ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility ReduceIsDisplayed
        {
            get
            {
                int totalMessages = MessagesViewers.Sum(messagesViewerItem => messagesViewerItem.Messages.Count);
                if (totalMessages > MessageCountLimitation && _showAll) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        private ICommand _showAllCommand;

        public ICommand ShowAllCommand => _showAllCommand ??= new DelegateCommand(ShowAllCommandExecute);

        private void ShowAllCommandExecute()
        {
            _showAll = true;
            UpdateMessageItems();
        }

        private ICommand _reduceCommand;

        public ICommand ReduceCommand => _reduceCommand ??= new DelegateCommand(ReduceCommandExecute);

        private void ReduceCommandExecute()
        {
            _showAll = false;
            UpdateMessageItems();
        }

        public void Dispose()
        {
            foreach (var messagesViewerItem in MessagesViewers)
            {
                messagesViewerItem.CollectionChanged -= MessagesViewerItem_CollectionChanged;
            }
        }
    }
}
