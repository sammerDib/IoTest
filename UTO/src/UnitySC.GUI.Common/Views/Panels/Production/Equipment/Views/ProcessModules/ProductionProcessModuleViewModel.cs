using System;

using Agileo.Common.Localization;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.Views.Panels.Production.Equipment.Views.ProcessModules
{
    public abstract class ProductionProcessModuleViewModel<T> : UnityDeviceCardViewModel, IProductionProcessModuleViewModel where T : DriveableProcessModule
    {
        #region Properties

        public T ProcessModule { get; }

        #endregion

        #region Constructor

        static ProductionProcessModuleViewModel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ProductionProcessModuleResource)));
        }

        protected ProductionProcessModuleViewModel(T processModule)
        {
            ProcessModule = processModule;
        }

        #endregion

        #region Commands

        private SafeDelegateCommand _abortRecipeCommand;

        public SafeDelegateCommand AbortRecipeCommand
            => _abortRecipeCommand ??= new SafeDelegateCommand(
                AbortRecipeCommandExecute,
                AbortRecipeCommandCanExecute);

        public Func<bool> AbortRecipeCanExecute { get; set; }

        protected virtual void AbortRecipeCommandExecute()
        {
            ProcessModule.AbortRecipeAsync();
        }

        protected virtual bool AbortRecipeCommandCanExecute()
        {
            if (ProcessModule == null)
            {
                return false;
            }

            if (AbortRecipeCanExecute != null && !AbortRecipeCanExecute())
            {
                return false;
            }

            var context = ProcessModule.NewCommandContext(nameof(IDriveableProcessModule.AbortRecipe));
            return ProcessModule.CanExecute(context);
        }

        #endregion
    }

    public interface IProductionProcessModuleViewModel
    {
        SafeDelegateCommand AbortRecipeCommand { get; }

        Func<bool> AbortRecipeCanExecute { get; set; }
    }
}
