using System;
using System.Windows;
using System.Windows.Controls;

using ADC.Model;
using ADC.View;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel
{
    /// <summary>
    /// Partial class for rendering
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public partial class RecipeViewModel : ObservableRecipient
    {
        private String _renderingInfo;
        public String RenderingInfo
        {
            get { return _renderingInfo; }
            set
            {
                _renderingInfo = value;
                OnPropertyChanged();
            }
        }

        private bool _renderinOnlySelectedNode;
        public bool RenderingOnlySelectedNode
        {
            get { return _renderinOnlySelectedNode; }
            set
            {
                _renderinOnlySelectedNode = value;
                OnPropertyChanged();
            }
        }

        private AutoRelayCommand _showRenderingGraphCommand = null;
        public AutoRelayCommand ShowRenderingGraphCommand
        {
            get
            {
                return _showRenderingGraphCommand ?? (_showRenderingGraphCommand = new AutoRelayCommand(
                    () => ShowRendering(),
                    () => !IsRecipeRunning && EditionMode == AdcEnum.RecipeEditionMode.ExpertRecipeEdition && _recipeGraphVM.GraphContainsNodes));
            }
        }

        private AutoRelayCommand _saveRenderingCommand = null;
        public AutoRelayCommand SaveRenderingCommand
        {
            get
            {
                //TODO FDE save data
                return _saveRenderingCommand ?? (_saveRenderingCommand = new AutoRelayCommand(
                    () => { }/* SelectedModule.Rendering.SaveData() */ ));
            }
        }

        private AutoRelayCommand _unmergeCommand = null;
        public AutoRelayCommand UnmergeCommand
        {
            get
            {
                return _unmergeCommand ?? (_unmergeCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (ServiceRecipe.Instance().RecipeCurrent != null && ServiceRecipe.Instance().RecipeCurrent.Wafer != null)
                        {
                            ServiceRecipe.Instance().RecipeCurrent.Wafer = null;
                            OnPropertyChanged(nameof(Recipe.IsMerged));
                            if (RenderingUI != null)
                                RenderingUI.Close();
                            ShowRenderingGraphCommand.Execute(null);
                        }
                    }));
            }
        }

        private AutoRelayCommand _runRenderingGraphCommand = null;
        public AutoRelayCommand RunRenderingGraphCommand
        {
            get
            {
                return _runRenderingGraphCommand ?? (_runRenderingGraphCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (IsRecipeRunning)
                            AbortGraph();
                        else
                            RunTo();
                        OnPropertyChanged(nameof(ModuleRenderingUI));
                    }));
            }
        }

        private RenderingView _renderingUI;
        public RenderingView RenderingUI
        {
            get { return _renderingUI; }
            set
            {
                if (_renderingUI == value)
                    return;
                _renderingUI = value;
                OnPropertyChanged();
            }
        }

        public UserControl ModuleRenderingUI
        {
            get
            {
                if (SelectedModule == null || SelectedModule.State == eModuleState.Disabled)
                    return null;
                return SelectedModule.RenderingUI;
            }
        }

        private void OnModuleParametersChanged(ModuleBase sender, EventArgs e)
        {
            if (sender != null)
                RenderingInfo = "Execute <Run> to update image";
        }

        private void ShowRendering()
        {
            if (ServiceRecipe.Instance().RecipeCurrent == null)
                return;

            if (!ServiceRecipe.Instance().RecipeCurrent.IsMerged)
            {
                SelectAda(false);
                if (!ServiceRecipe.Instance().RecipeCurrent.IsMerged)
                    return;
            }

            RenderingUI = new RenderingView();
            RenderingUI.Owner = Application.Current.MainWindow;
            RenderingUI.Show();
            ModuleBase.ParametersChanged += OnModuleParametersChanged;
        }


        private void CloseRendering()
        {
            if (RenderingUI != null)
            {
                RenderingUI.Close();
                RenderingUI = null;
            }

            if (Recipe != null)
                Recipe.ClearRenderingObjects();
        }

        public bool RenderingView_Closing()
        {
            if (IsRecipeRunning)
                return false;

            if (Recipe != null)
                Recipe.ClearRenderingObjects();

            ModuleBase.ParametersChanged -= OnModuleParametersChanged;
            RenderingOnlySelectedNode = false;
            return true;
        }

        private void RunTo()
        {
            if (SelectedModule == null)
            {
                MessageBox.Show("No node selected !");
                return;
            }

            // Execute Recipe
            //...............
            PrepareRendering(Recipe);
            IsRecipeRunning = true;
            _timerGraph.Start();
            _recipeId = AdcExecutor.ExecuteRecipe(Recipe);
            RefreshNodeStatistics();
        }

        /// <summary>
        /// Prepare the Rendering
        /// </summary>
        private void PrepareRendering(Recipe recipe)
        {
            RenderingInfo = "";

            Recipe.ClearRenderingObjects();
            Recipe.IsRendering = true;

            foreach (ModuleBase module in recipe.ModuleList.Values)
                module.IsRendering = false;

            var renderingNodes = RecipeGraphVM.GetNodeAscendants(RecipeGraphVM.SelectedNode);
            renderingNodes.Add(RecipeGraphVM.SelectedNode);
            foreach (ModuleNodeViewModel node in renderingNodes)
                node.Module.IsRendering = true;

            recipe.IsRenderingNodeSeletedOnly = RenderingOnlySelectedNode;
        }

        public void RenderingProcess(ModuleBase module)
        {
            // NCH TODO cleanup
            //GalaSoft.MvvmLight.Threading.DispatcherHelper.CheckBeginInvokeOnUI(
            //    () => NodeRenderingUI = module.GetRenderingUI()
            //    );
        }

   
    }
}
