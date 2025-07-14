using System;

using Agileo.Common.Access;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights
{
    public class GraphicalElementRightsViewModel<T> : RightsViewModelBase where T : GraphicalElement
    {
        protected Action<RightViewModel> OnLevelChangedAction { get; }

        public GraphicalElementRightsViewModel(T element, IAccessManager accessManager, Action<RightViewModel> onLevelChanged)
        {
            OnLevelChangedAction = onLevelChanged;
            Element = element;
            
            EnabledRight = new RightViewModel(
                element.Id,
                Element.AccessRights.IsEnabledRight,
                accessManager,
                OnLevelChanged);
            
            VisibilityRight = new RightViewModel(
                element.Id,
                Element.AccessRights.IsVisibleRight,
                accessManager,
                OnLevelChanged);
            
            ResetCommand = new DelegateCommand(
                () =>
                {
                    EnabledRight.Level = EnabledRight.InitialLevel;
                    VisibilityRight.Level = VisibilityRight.InitialLevel;
                },
                () => EnabledRight.HasModified || VisibilityRight.HasModified);
        }

        private bool _hasModified;

        public bool HasModified
        {
            get => _hasModified;
            protected set => SetAndRaiseIfChanged(ref _hasModified, value);
        }

        protected virtual void OnLevelChanged(RightViewModel obj)
        {
            HasModified = EnabledRight.HasModified || VisibilityRight.HasModified;
            OnPropertyChanged(nameof(HighestLevel));
            OnLevelChangedAction(obj);
        }

        public T Element { get; }
        
        public override GraphicalElement GraphicalElement => Element;
    }
}
