using System;
using System.Collections.Generic;

using Agileo.Common.Localization;
using Agileo.ProcessingFramework.Instructions;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;

using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public abstract class ProcessInstruction : Instruction, IDisposable
    {
        #region Fields

        protected bool _disposedValue;

        #endregion Fields

        #region Constructor

        static ProcessInstruction()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ProcessInstructionsResources)));
        }

        protected ProcessInstruction()
        {
            App.Instance.Localizer.CultureChanged += Localizer_CultureChanged;
            Localizer_CultureChanged(this, null); // useful to initialize UI
        }

        #endregion Constructor

        #region Properties

        public List<AdvancedStringFormatDefinition> FormattedLabel { get; set; }

        #endregion

        #region Event Handlers

        private void Localizer_CultureChanged(object sender, EventArgs e)
        {
            LocalizeName();
        }

        #endregion

        #region Abstract Methods

        protected abstract void LocalizeName();

        #endregion

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;
            if (disposing)
            {
                App.Instance.Localizer.CultureChanged -= Localizer_CultureChanged;
            }
            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
