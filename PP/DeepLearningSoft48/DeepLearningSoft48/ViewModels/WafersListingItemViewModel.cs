using System;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Models;
using DeepLearningSoft48.Utils.Enums;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// DataContext of each WafersListingView.xaml's items (each wafer).
    /// </summary>
    public class WafersListingItemViewModel : ObservableRecipient
    {
        //====================================================================
        // Wafer Fields Initialization
        //====================================================================
        public Wafer Wafer { get; private set; }
        public string BaseName => Wafer.BaseName;

        /// <summary>
        /// State linked to the locker.
        /// </summary>
        public bool IsLocked => Wafer.IsLocked;

        /// <summary>
        /// Event raised when the current wafer's state changes.
        /// </summary>
        public event Action<Wafer> WaferStateChanged;

        /// <summary>
        /// Permits to display the locker according to the tab.
        /// True if we are in the LearningTab.
        /// False if we are in the TestTab.
        /// </summary>
        public bool IsInLearningTab { get; set; }

        //====================================================================
        // Constructor
        //====================================================================
        public WafersListingItemViewModel(Wafer wafer, TabType tabType)
        {
            Wafer = wafer;

            if (tabType == TabType.Learning)
                IsInLearningTab = true;
            else IsInLearningTab = false;
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Permits to lock the current wafer.
        /// </summary>
        private AutoRelayCommand<string> _lockWaferStateCommand;
        public AutoRelayCommand<string> LockWaferStateCommand
        {
            get
            {
                return _lockWaferStateCommand ?? (_lockWaferStateCommand = new AutoRelayCommand<string>(
              baseName =>
              {
                  if (baseName == BaseName)
                  {
                      Wafer.IsLocked = true;
                      WaferStateChanged?.Invoke(Wafer);
                  }
              }));
            }
        }

        /// <summary>
        /// Unlock the current wafer.
        /// </summary>
        private AutoRelayCommand<string> _unlockWaferStateCommand;
        public AutoRelayCommand<string> UnlockWaferStateCommand
        {
            get
            {
                return _unlockWaferStateCommand ?? (_unlockWaferStateCommand = new AutoRelayCommand<string>(
              baseName =>
              {
                  if (baseName == BaseName)
                  {
                      Wafer.IsLocked = false;
                      WaferStateChanged?.Invoke(Wafer);
                  }
              }));
            }
        }
    }
}
