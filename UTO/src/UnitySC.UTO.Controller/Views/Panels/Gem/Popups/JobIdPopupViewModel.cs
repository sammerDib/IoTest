using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.Popups
{
    public class JobIdPopupViewModel : NotifyDataError
    {
        #region Constructor

        static JobIdPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(JobIdPopupViewModel), typeof(JobIdPopupView));
        }

        public JobIdPopupViewModel()
        {
            //Pour le designer
        }

        public JobIdPopupViewModel(List<string> existingJobNames)
        {
            ExistingJobNames = existingJobNames;
            JobName = "";
            Rules.Add(new DelegateRule(nameof(JobName), ValidateEditingId));
            ApplyRules();
        }

        #endregion

        #region Properties

        public List<string> ExistingJobNames { get; }

        private string _jobName;

        public string JobName
        {
            get => _jobName;
            set => SetAndRaiseIfChanged(ref _jobName, value);
        }

        #endregion

        #region Public methods

        public static string ValidateEditingId(List<string> existingIds, string idToCheck)
        {
            if (string.IsNullOrEmpty(idToCheck))
            {
                return LocalizationManager.GetString(nameof(GemGeneralRessources.GEM_ERROR_EMPTY));
            }

            if (existingIds.Any(id => id == idToCheck))
            {
                return LocalizationManager.GetString(
                    nameof(GemGeneralRessources.GEM_ERROR_ALREADY_USED),
                    idToCheck);
            }

            return null;
        }

        #endregion

        #region Private methods

        private string ValidateEditingId()
        {
            return ValidateEditingId(ExistingJobNames, JobName);
        }

        #endregion
    }
}
