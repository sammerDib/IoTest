using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public class RecipeSummaryVM : ObservableRecipient
    {
        #region Private Fields

        private DMTRecipe _displayedRecipe = null;
        private readonly ServiceInvoker<IToolService> _toolService;
        private readonly UserSupervisor _userSupervisor;

        #endregion Private Fields

        #region Public Constructors

        public RecipeSummaryVM(UserSupervisor userSupervisor, ILogger<IToolService> toolServiceLogger)
        {
            _userSupervisor = userSupervisor;
            _toolService = new ServiceInvoker<IToolService>("ToolService", toolServiceLogger, Messenger, ClientConfiguration.GetDataAccessAddress());
        }

        #endregion Public Constructors

        #region Public Properties

        public DMTRecipe DisplayedRecipe
        {
            get => _displayedRecipe;
            set
            {
                if (_displayedRecipe != value)
                {
                    _displayedRecipe = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EnabledMeasures));
                    OnPropertyChanged(nameof(UserName));
                    OnPropertyChanged(nameof(StepName));
                }
            }
        }

        public IEnumerable<MeasureSummaryVM> EnabledMeasures
        {
            get
            {
                List<MeasureSummaryVM> measuresSummaries = new List<MeasureSummaryVM>();

                if ((_displayedRecipe == null) || (_displayedRecipe.Measures == null))
                    return null;

                foreach (var measure in _displayedRecipe?.Measures.Where(m => m.IsEnabled))
                {
                    var newMeasureSummary = new MeasureSummaryVM() { Name = measure.MeasureName, Side = measure.Side };

                    var outputTypes = measure.GetOutputTypes();
                    foreach (var outputType in outputTypes)
                    {
                        newMeasureSummary.Outputs.Add(outputType.ToString());
                    }
                    measuresSummaries.Add(newMeasureSummary);
                }
                return measuresSummaries;
            }
        }

        public string UserName
        {
            get
            {
                if (DisplayedRecipe == null || DisplayedRecipe.UserId == null)
                    return null;

                var user = _userSupervisor.GetUser((int)DisplayedRecipe.UserId);

                return $"{user.Name}";
            }
        }

        public string StepName
        {
            get
            {
                if (DisplayedRecipe == null || DisplayedRecipe.StepId == null)
                    return null;

                var step = _toolService.Invoke(r => r.GetStep((int)DisplayedRecipe.StepId));

                return $"{step.Name}";
            }
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        #endregion Public Properties
    }
}
