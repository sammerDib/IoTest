using System;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.UC;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Data;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.UI.DummyPM
{
    /// <summary>
    /// Interaction logic for DummyRecipeEditor.xaml
    /// </summary>
    [Export(typeof(IRecipeEditorUc))]
    [UCMetadata(ActorType = ActorType.Unknown)]
    public partial class DummyRecipeEditor : UserControl, IRecipeEditorUc
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        public DummyRecipeEditor()
        {
            InitializeComponent();
        }

        public ActorType ActorType { get; set; }

        public event EventHandler ExitEditor;

        public void Init(bool isStandalone)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            DataContext = new DummyRecipeEditorViewModel(_dbRecipeService);
        }

        public void LoadRecipe(Guid key)
        {
            ((DummyRecipeEditorViewModel)DataContext).LoadRecipe(key);
        }

        private AutoRelayCommand _backCommand;

        public AutoRelayCommand BackCommand
        {
            get
            {
                return _backCommand ?? (_backCommand = new AutoRelayCommand(
              () =>
              {
                  ExitEditor?.Invoke(this, null);
              },
              () => { return true; }));
            }
        }

        public RecipeInfo CreateNewRecipe(string name, int stepId, int userId)
        {
            // Add recipe DEMETER
            UnitySC.DataAccess.Dto.Recipe recipeDemeter = new UnitySC.DataAccess.Dto.Recipe();
            recipeDemeter.Type = ActorType;
            recipeDemeter.Comment = "Comment " + name;
            recipeDemeter.Name = name;
            recipeDemeter.StepId = stepId;
            recipeDemeter.CreatorUserId = userId;
            recipeDemeter.AddOutput(ResultType.DMT_CurvatureX_Back);
            recipeDemeter.AddOutput(ResultType.DMT_CurvatureX_Front);
            recipeDemeter.CreatorChamberId = 61;
            var id = _dbRecipeService.Invoke(x => x.SetRecipe(recipeDemeter, true));
            var recipe = _dbRecipeService.Invoke(x => x.GetRecipe(id, false));
            return new RecipeInfo() { Name = recipe.Name, Version = recipe.Version, Comment = recipe.Comment, StepId = recipe.StepId, IsShared = recipe.IsShared, IsTemplate = recipe.IsTemplate, ActorType = (ActorType)recipe.ActorType, Key = recipe.KeyForAllVersion };
        }

        public bool CanClose()
        {
            return true;
        }

        public async Task ExportRecipeAsync(Guid key)
        {
            
        }

        public async Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId)
        {
            return null;        
        }
    }
}
