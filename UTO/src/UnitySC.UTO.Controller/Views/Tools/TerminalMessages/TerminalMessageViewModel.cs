using System;

using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.UIComponents.Components;

namespace UnitySC.UTO.Controller.Views.Tools.TerminalMessages
{
    public class TerminalMessageViewModel : Agileo.GUI.Components.Notifier, IDisposable
    {
        public TerminalMessageSource Source { get; }

        public LocalizableText SourceName { get; }

        public LocalizableDateTime DateTime { get; }

        public string Content { get; }

        private bool _acknowledged;

        public bool Acknowledged
        {
            get { return _acknowledged; }
            set { SetAndRaiseIfChanged(ref _acknowledged, value); }
        }

        public TerminalMessageViewModel(TerminalMessageSource source, LocalizableText sourceName, DateTime dateTime, string content)
        {
            Source = source;
            SourceName = sourceName;
            DateTime = LocalizableDateTime.WithStandardFormat(dateTime);
            Content = content;
        }

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    SourceName?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
