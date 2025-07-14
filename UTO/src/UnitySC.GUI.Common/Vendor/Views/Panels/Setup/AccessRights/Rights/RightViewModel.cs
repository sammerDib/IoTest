using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Access;
using Agileo.GUI.Components;
using Agileo.GUI.Components.AccessRights;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights
{
    public class RightViewModel : Notifier
    {
        private readonly RightDataViewModel _rightData;
        private readonly IAccessManager _accessManager;
        private readonly Action<RightViewModel> _onLevelChanged;

        public RightViewModel(string elementPath, RightDataViewModel rightData, IAccessManager accessManager, Action<RightViewModel> onLevelChanged)
        {
            Path = $"{elementPath}.{rightData.Id}";
            _rightData = rightData;
            _accessManager = accessManager;
            _onLevelChanged = onLevelChanged;
            InitialLevel = rightData.AccessLevel;
            _level = _rightData.AccessLevel;
        }

        #region ReadOnly Properties

        public string Path { get; }

        public bool HasModified => Level != InitialLevel;

        public bool CanEditLevel => InitialLevel <= _accessManager.CurrentUser.AccessLevel;

        public ReadOnlyCollection<AccessLevel> AvailableAccessLevels
        {
            get
            {
                if (!CanEditLevel)
                {
                    return new ReadOnlyCollection<AccessLevel>(new List<AccessLevel>()
                    {
                        _level
                    });
                }

                return Enum.GetValues(typeof(AccessLevel))
                    .Cast<AccessLevel>()
                    .Where(level => level <= _accessManager.CurrentUser.AccessLevel)
                    .ToList()
                    .AsReadOnly();
            }
        }

        #endregion

        #region Notified Properties

        private AccessLevel _level;

        public AccessLevel Level
        {
            get => _level;
            set
            {
                if (SetAndRaiseIfChanged(ref _level, value))
                {
                    _onLevelChanged(this);
                    OnPropertyChanged(nameof(HasModified));
                }
            }
        }

        #endregion

        #region Properties

        public AccessLevel InitialLevel { get; private set; }

        #endregion

        public void Apply()
        {
            _rightData.AccessLevel = Level;
            InitialLevel = Level;

            OnPropertyChanged(nameof(HasModified));
            OnPropertyChanged(nameof(CanEditLevel));
        }
    }
}
