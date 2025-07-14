using AcquisitionAdcExchange;

using BasicModules.DataLoader;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.MergedRecipe
{
    /// <summary>
    ///  View model de selection d'un chemin pour la modification des inputs d'une recette mergée
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class PicturePathSelectionViewModelBase : ObservableRecipient
    {
        internal InspectionInputInfoBase _inputInfoBase;
        public PicturePathSelectionViewModelBase(InspectionInputInfoBase inputInfoBase)
        {
            _inputInfoBase = inputInfoBase;

            ResultType = inputInfoBase.ResultType;

            Init();
        }

        internal abstract void Init();

        private ResultType _resTyp;
        public ResultType ResultType
        {
            get => _resTyp; 
            set
            {
                if (_resTyp != value)
                { 
                    _resTyp = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ActorType));
                    OnPropertyChanged(nameof(ResultTypeId));
                }
            }
        }


        public ActorType ActorType
        {
            get => ResultType.GetActorType();
        }

        public string ResultTypeId
        {
            get => DataLoaderHelper.GetResultTypeName(ResultType);
        }

        public abstract string CurrentPath { get; set; }

        public abstract bool IsValid { get; set; }

        public abstract string TypeName { get; }

        public abstract void ApplyChange();

        internal abstract void OpenPath(string path);

        internal abstract void SetAcquistionFolder(string acquisitionFolder);

        #region command

        private AutoRelayCommand<string> _openPathCommand;
        public AutoRelayCommand<string> OpenPathCommand
        {
            get
            {
                return _openPathCommand ?? (_openPathCommand = new AutoRelayCommand<string>(
                    (path) =>
                    {
                        OpenPath(path);
                    }));
            }
        }

        #endregion
    }
}
