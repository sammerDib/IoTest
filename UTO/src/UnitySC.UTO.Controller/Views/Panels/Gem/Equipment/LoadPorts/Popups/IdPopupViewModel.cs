using System.IO;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Equipment.LoadPorts.Popups
{
    public class IdPopupViewModel : NotifyDataError
    {
        #region Constructor

        public IdPopupViewModel()
        {
            Id = "";
            Rules.Add(new DelegateRule(nameof(Id), ValidateEditingId));
            ApplyRules();
        }

        #endregion

        #region Properties

        private string _id;

        public string Id
        {
            get => _id;
            set => SetAndRaiseIfChanged(ref _id, value);
        }

        #endregion

        #region Public methods

        public string ValidateEditingId()
        {
            if (string.IsNullOrEmpty(Id))
            {
                return LocalizationManager.GetString(nameof(GemGeneralRessources.GEM_ERROR_EMPTY));
            }

            if (Id.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return LocalizationManager.GetString(nameof(GemGeneralRessources.INVALID_CHAR_FILE_NAME));
            }

            return null;
        }

        #endregion
    }
}
