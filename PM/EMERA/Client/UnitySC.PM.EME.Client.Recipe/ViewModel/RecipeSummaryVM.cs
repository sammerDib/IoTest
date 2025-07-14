using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public class RecipeSummaryVM : ObservableObject
    {
        private UserSupervisor _userSupervisor;

        #region Public Constructors

        public RecipeSummaryVM()
        {
            _userSupervisor = ClassLocator.Default.GetInstance<UserSupervisor>();
            Acquisitions = new ObservableCollection<AcquisitionSummary>();
        }

        #endregion Public Constructors

        #region Public Properties

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    OnPropertyChanged();
                }
            }
        }

        private StepStates _globalAutoFocus;

        public StepStates GlobalAutoFocus
        {
            get => _globalAutoFocus;
            set => SetProperty(ref _globalAutoFocus, value);
        }

        private StepStates _globalBWA;

        public StepStates GlobalBWA
        {
            get => _globalBWA;
            set => SetProperty(ref _globalBWA, value);
        }

        private StepStates _globalAutoExposure;

        public StepStates GlobalAutoExposure
        {
            get => _globalAutoExposure;
            set => SetProperty(ref _globalAutoExposure, value);
        }
        
        private StepStates _convertTo8Bits;
        public StepStates ConvertTo8Bits
        {
            get => _convertTo8Bits;
            set => SetProperty(ref _convertTo8Bits, value);
        }

        private StepStates _reduceResolution;

        public StepStates ReduceResolution
        {
            get => _reduceResolution;
            set => SetProperty(ref _reduceResolution, value);
        }
        
        private StepStates _correctDistortion;

        public StepStates CorrectDistortion
        {
            get => _correctDistortion;
            set => SetProperty(ref _correctDistortion, value);
        }

        private StepStates _normalizePixelValue;

        public StepStates NormalizePixelValue
        {
            get => _normalizePixelValue;
            set => SetProperty(ref _normalizePixelValue, value);
        }

        private StepStates _runStitchFullImage;

        public StepStates RunStitchFullImage
        {
            get => _runStitchFullImage;
            set => SetProperty(ref _runStitchFullImage, value);
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    OnPropertyChanged();
                }
            }
        }

        private EMERecipe _displayedRecipe;

        public EMERecipe DisplayedRecipe
        {
            get => _displayedRecipe;
            set
            {
                if (_displayedRecipe != value)
                {
                    _displayedRecipe = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<AcquisitionSummary> Acquisitions { get; }

        internal void Update(EMERecipe emeRecipe)
        {
            DisplayedRecipe = emeRecipe;
            if (emeRecipe == null)
                throw new Exception("Recipe not found.");

            UserName = _userSupervisor.GetUser(emeRecipe.UserId.Value).Name;
            if (emeRecipe.Execution != null)
            {
                GlobalAutoFocus = GetStepStateFromBoolean(emeRecipe.Execution.RunAutoFocus);
                GlobalBWA = GetStepStateFromBoolean(emeRecipe.Execution.RunBwa);
                GlobalAutoExposure = GetStepStateFromBoolean(emeRecipe.Execution.RunAutoExposure);
                ConvertTo8Bits = GetStepStateFromBoolean(emeRecipe.Execution.ConvertTo8Bits);
                ReduceResolution = GetStepStateFromBoolean(emeRecipe.Execution.ReduceResolution);
                CorrectDistortion = GetStepStateFromBoolean(emeRecipe.Execution.CorrectDistortion);
                NormalizePixelValue = GetStepStateFromBoolean(emeRecipe.Execution.NormalizePixelValue);
                RunStitchFullImage = GetStepStateFromBoolean(emeRecipe.Execution.RunStitchFullImage);

            }

            Acquisitions.Clear();
            var lightBench = ClassLocator.Default.GetInstance<LightBench>();
            foreach (var acquisition in emeRecipe.Acquisitions)
            {
                Acquisitions.Add(new AcquisitionSummary(acquisition, lightBench));
            }
        }

        #endregion Public Properties
        
        private static StepStates GetStepStateFromBoolean(bool value)
        {
            return value ? StepStates.Done : StepStates.NotDone;
        }
    }
}
