using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.About.SupportRequest
{
    public class ArchiveCompleteEventArgs : EventArgs
    {
        public ArchiveCompleteEventArgs(LocalizableText message, bool isArchiveComplete, bool doesArchiveExists, bool exceptionThrown = false)
        {
            Message = message;
            IsArchiveComplete = isArchiveComplete;
            DoesArchiveExists = doesArchiveExists;
            ExceptionThrown = exceptionThrown;
        }

        public bool DoesArchiveExists { get; }

        public bool IsArchiveComplete { get; }

        public bool ExceptionThrown { get; }

        public LocalizableText Message { get; }
    }
}
