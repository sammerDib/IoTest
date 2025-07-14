using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    /// <summary>
    /// Notifier that implements <see cref="INotifyDataErrorInfo"/> with a generic implementation of <see cref="Rule"/>s.
    /// </summary>
    public class NotifyDataError : Notifier, INotifyDataErrorInfo, INotifyConversionErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #region Properties

        public virtual bool HasErrors => _errors.Count > 0 || _convertErrors.Count > 0;

        public RuleCollection Rules { get; } = new();

        private bool _rulesHasApplied;

        public bool RulesHasApplied
        {
            get => _rulesHasApplied;
            private set => SetAndRaiseIfChanged(ref _rulesHasApplied, value);
        }

        #endregion

        public IEnumerable<string> GetAllErrors()
        {
            return _errors.SelectMany(kvp => kvp.Value).Concat(_convertErrors.Values);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return GetAllErrors();
            }

            if (_convertErrors.ContainsKey(propertyName))
            {
                return new List<string> { _convertErrors[propertyName] };
            }

            return _errors.ContainsKey(propertyName) ? _errors[propertyName] : Enumerable.Empty<string>();
        }

        #region Apply Rules

        public void ApplyRules()
        {
            foreach (var propertyName in Rules.Select(x => x.PropertyName))
            {
                ApplyRules(propertyName);
            }

            RulesHasApplied = true;
        }

        public void ApplyRules(string propertyName)
        {
            var propertyErrors = Rules.Apply(this, propertyName).ToList();

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (propertyErrors.Count > 0)
            {
                if (_errors.ContainsKey(propertyName))
                {
                    _errors[propertyName].Clear();
                }
                else
                {
                    _errors[propertyName] = new List<string>();
                }

                _errors[propertyName].AddRange(propertyErrors);
                OnErrorsChanged(propertyName);
            }
            else if (_errors.ContainsKey(propertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }

            base.OnPropertyChanged(nameof(HasErrors));
        }

        public void ClearAllErrors()
        {
            foreach (var propertyName in Rules.Select(x => x.PropertyName))
            {
                _errors.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }

            RulesHasApplied = false;
        }

        public void ClearErrors(string propertyName)
        {
            if (_errors.TryGetValue(propertyName, out var errors))
            {
                errors.Clear();
                OnErrorsChanged(propertyName);
            }
        }


        #endregion

        #region Events

        #region Overrides of Notifier

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
            {
                ApplyRules();
            }
            else
            {
                ApplyRules(propertyName);
            }

            base.OnPropertyChanged(propertyName);
        }

        #endregion

        /// <summary>
        /// Called when the errors have changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnErrorsChanged([CallerMemberName] string propertyName = null)
            => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        #endregion

        #region INotifyConversionErrorInfo implementation

        private readonly Dictionary<string, string> _convertErrors = new ();

        public void AddConversionError(string propertyName, string error)
        {
            if (!_convertErrors.ContainsKey(propertyName))
            {
                _convertErrors.Add(propertyName, error);
            }
            else
            {
                _convertErrors[propertyName] = error;
            }

            OnErrorsChanged(propertyName);

            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged(nameof(HasErrors));
        }

        public void ClearConversionError(string propertyName)
        {
            if(_convertErrors.ContainsKey(propertyName))
            {
                _convertErrors.Remove(propertyName);
            }

            OnErrorsChanged(propertyName);

            base.OnPropertyChanged(propertyName);
            base.OnPropertyChanged(nameof(HasErrors));
        }

        #endregion
    }
}
