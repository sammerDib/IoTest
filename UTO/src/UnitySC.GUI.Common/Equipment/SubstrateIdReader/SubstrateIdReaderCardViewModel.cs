using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.GUI.Common.Equipment.SubstrateIdReader
{
    public class SubstrateIdReaderCardViewModel : UnityDeviceCardViewModel
    {
        #region Fields

        private UnitySC.Equipment.Abstractions.Devices.Efem.Efem _efem;
        private UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner _aligner;
        private UnitySC.Equipment.Abstractions.Devices.Robot.Robot _robot;

        #endregion

        #region Constructor

        static SubstrateIdReaderCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(SubstrateIdReaderCardViewModel), typeof(SubstrateIdReaderCard));
        }

        public SubstrateIdReaderCardViewModel(
            UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader substrateIdReader)
        {
            SubstrateIdReader = substrateIdReader;

        }

        #endregion

        #region Setup

        public void Setup(UnitySC.Equipment.Abstractions.Devices.Efem.Efem efem)
        {
            _efem = efem;
            _aligner = _efem.TryGetDevice<UnitySC.Equipment.Abstractions.Devices.Aligner.Aligner>();
            _robot = _efem.TryGetDevice<UnitySC.Equipment.Abstractions.Devices.Robot.Robot>();
        }

        #endregion

        #region Properties

        private UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader _substrateIdReader;
        public UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader SubstrateIdReader
        {
            get => _substrateIdReader;
            set => SetAndRaiseIfChanged(ref _substrateIdReader, value);
        }

        private ObservableCollection<string> _recipes;
        public ObservableCollection<string> Recipes
        {
            get => _recipes;
            set => SetAndRaiseIfChanged(ref _recipes, value);
        }

        private string _selectedRecipe;
        public string SelectedRecipe
        {
            get => _selectedRecipe;
            set => SetAndRaiseIfChanged(ref _selectedRecipe, value);
        }

        #endregion

        #region Command

        #region Read

        private SafeDelegateCommand _readCommand;

        public SafeDelegateCommand ReadCommand
            => _readCommand ??= new SafeDelegateCommand(
                ReadCommandExecute,
                ReadCommandCanExecute);

        private void ReadCommandExecute() => Task.Factory.StartNew(ReadWaferId);

        private bool ReadCommandCanExecute()
        {
            if (SubstrateIdReader == null)
            {
                return false;
            }

            var context = SubstrateIdReader.NewCommandContext(nameof(SubstrateIdReader.Read))
                .AddArgument("recipeName", SelectedRecipe);

            return SubstrateIdReader.CanExecute(context)
                   && !string.IsNullOrWhiteSpace(SelectedRecipe)
                   && _robot.State == OperatingModes.Idle
                   && _aligner.State == OperatingModes.Idle;
        }

        #endregion

        #region Get Recipes

        private SafeDelegateCommandAsync _getRecipesCommand;

        public SafeDelegateCommandAsync GetRecipesCommand
            => _getRecipesCommand ??= new SafeDelegateCommandAsync(
                GetRecipesCommandExecute,
                GetRecipesCommandCanExecute);

        private Task GetRecipesCommandExecute()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    SubstrateIdReader.RequestRecipes();
                    Recipes = new ObservableCollection<string>(
                        SubstrateIdReader.Recipes.Where(x => x.IsStored).Select(r => r.Name));
                });
        }

        private bool GetRecipesCommandCanExecute()
        {
            if (SubstrateIdReader == null)
            {
                return false;
            }

            var context =
                SubstrateIdReader.NewCommandContext(nameof(SubstrateIdReader.RequestRecipes));

            return SubstrateIdReader.CanExecute(context);
        }

        #endregion

        #region Init

        private SafeDelegateCommandAsync _initCommand;

        public SafeDelegateCommandAsync InitCommand
            => _initCommand ??= new SafeDelegateCommandAsync(
                InitCommandExecute,
                InitCommandCanExecute);

        private Task InitCommandExecute() => SubstrateIdReader.InitializeAsync(false);

        private bool InitCommandCanExecute()
        {
            if (SubstrateIdReader == null)
            {
                return false;
            }

            var context = SubstrateIdReader.NewCommandContext(nameof(SubstrateIdReader.Initialize))
                .AddArgument("mustForceInit", false);
            return SubstrateIdReader.CanExecute(context);
        }

        #endregion

        #region Abort

        private SafeDelegateCommandAsync _abortCommand;

        public SafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new SafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute() => SubstrateIdReader.InterruptAsync(InterruptionKind.Abort);

        private bool AbortCommandCanExecute()
        {
            if (SubstrateIdReader == null)
            {
                return false;
            }

            return SubstrateIdReader.State != OperatingModes.Maintenance
                   && SubstrateIdReader.State != OperatingModes.Idle;
        }

        #endregion

        #endregion

        #region Private Methods

        private void ReadWaferId()
        {
            var readerSide = SubstrateIdReader.InstanceId == 1 ? ReaderSide.Front : ReaderSide.Back;

            _efem.ReadSubstrateId(readerSide, SelectedRecipe, SelectedRecipe);
        }

        #endregion
    }
}
