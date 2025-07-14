using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    /// <summary>
    /// BusinessPanel that implements <see cref="INotifyDataErrorInfo"/> by a internal <see cref="NotifyDataError"/> instance.
    /// </summary>
    public abstract class NotifyDataErrorPanel : BusinessPanel, INotifyDataErrorInfo, INotifyConversionErrorInfo
    {
        private readonly NotifyDataError _notifyDataErrorInfoImplementation = new();

        #region Overrides of Notifier

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                _notifyDataErrorInfoImplementation.ApplyRules();
            }
            else
            {
                _notifyDataErrorInfoImplementation.ApplyRules(propertyName);
            }

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        #region Delegated Implementation of INotifyDataErrorInfo

        public bool HasErrors => _notifyDataErrorInfoImplementation.HasErrors;

        protected RuleCollection Rules => _notifyDataErrorInfoImplementation.Rules;

        public bool RulesHasApplied => _notifyDataErrorInfoImplementation.RulesHasApplied;

        public IEnumerable<string> GetAllErrors() => _notifyDataErrorInfoImplementation.GetAllErrors();

        public IEnumerable GetErrors(string propertyName) => _notifyDataErrorInfoImplementation.GetErrors(propertyName);

        public void ApplyRules() => _notifyDataErrorInfoImplementation.ApplyRules();

        public void ClearAllErrors() => _notifyDataErrorInfoImplementation.ClearAllErrors();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add => _notifyDataErrorInfoImplementation.ErrorsChanged += value;
            remove => _notifyDataErrorInfoImplementation.ErrorsChanged -= value;
        }

        #endregion

        #region Delegated Implementation of INotifyConversionErrorInfo

        public void AddConversionError(string propertyName, string error)
        {
            _notifyDataErrorInfoImplementation.AddConversionError(propertyName, error);
        }

        public void ClearConversionError(string propertyName)
        {
            _notifyDataErrorInfoImplementation.ClearConversionError(propertyName);
        }

        #endregion

        protected NotifyDataErrorPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            _notifyDataErrorInfoImplementation.PropertyChanged += NotifyDataErrorInfoImplementationOnPropertyChanged;
        }

        private void NotifyDataErrorInfoImplementationOnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => base.OnPropertyChanged(e.PropertyName);

        #region Overrides of IdentifiableElement

        protected override void Dispose(bool disposing)
        {
            _notifyDataErrorInfoImplementation.PropertyChanged -= NotifyDataErrorInfoImplementationOnPropertyChanged;
            base.Dispose(disposing);
        }

        #endregion
    }
}
