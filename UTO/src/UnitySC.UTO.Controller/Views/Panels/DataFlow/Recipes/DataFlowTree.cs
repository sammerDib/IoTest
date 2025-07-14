using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.GUI.Components;

using UnitySC.DataAccess.Dto;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Recipes
{
    public class DataFlowTree : Notifier, IDisposable
    {
        #region Fields
        
        private readonly AbstractDataFlowManager _dataFlowManager;

        #endregion

        #region Properties

        public DataTreeSource<object> DataTreeSource { get; }

        private object _selectedValue;

        public object SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (SetAndRaiseIfChanged(ref _selectedValue, value))
                {
                    OnPropertyChanged(nameof(SelectedRecipe));
                }
            }
        }

        public DataflowRecipeInfo SelectedRecipe => SelectedValue as DataflowRecipeInfo;

        #endregion

        #region Constructor

        static DataFlowTree()
        {
            DataTemplateGenerator.Create(typeof(DataFlowTree), typeof(DataFlowTreeView));
        }

        public DataFlowTree(AbstractDataFlowManager dataFlowManager)
        {
            _dataFlowManager = dataFlowManager;

            DataTreeSource = new DataTreeSource<object>(GetChildren);
            DataTreeSource.Search.AddSearchDefinition("Name", GetComparableStringFunc);
            DataTreeSource.Sort.AddSortDefinition("Name", GetComparableStringFunc);

            _dataFlowManager.DataFlowRecipeAdded += DataFlowManager_DataFlowRecipeAdded;
            _dataFlowManager.DataFlowRecipeDeleted += DataFlowManager_DataFlowRecipeDeleted;

            UpdateRecipes();
        }

        #endregion

        #region Public Methods

        public void Refresh()
        {
            UpdateRecipes();
        }

        #endregion

        #region Event Handlers

        private void DataFlowManager_DataFlowRecipeDeleted(object sender, DataFlowRecipeEventArgs e)
        {
            UpdateRecipes();
        }

        private void DataFlowManager_DataFlowRecipeAdded(object sender, DataFlowRecipeEventArgs e)
        {
            UpdateRecipes();
        }

        #endregion

        #region Commands

        #region Refresh

        private SafeDelegateCommandAsync _refreshCommand;

        public SafeDelegateCommandAsync RefreshCommand
            => _refreshCommand ??= new SafeDelegateCommandAsync(
                RefreshCommandExecute);

        private Task RefreshCommandExecute()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    _dataFlowManager.GetAvailableRecipes();
                    UpdateRecipes();
                });
        }

        #endregion

        #endregion

        #region Private Methods

        private IEnumerable<object> GetChildren(object item)
        {
            switch (item)
            {
                case DataFlowProduct product:
                    return product.Steps;

                case DataFlowStep step:
                    return step.Recipes;

                default:
                    return new List<object>();
            }
        }

        private string GetComparableStringFunc(object arg)
        {
            if (arg is TreeNode<object> treeNode)
            {
                return GetComparableStringFunc(treeNode.Model);
            }

            return arg switch
            {
                DataFlowProduct product => product.Name,
                DataFlowStep step => step.Name,
                DataflowRecipeInfo info => info.Name,
                _ => null
            };
        }

        private void UpdateRecipes()
        {
            var dataFlowProducts = new List<DataFlowProduct>();
            if (_dataFlowManager?.AvailableRecipes != null)
            {
                var products = _dataFlowManager.AvailableRecipes.GroupBy(r => r.ProductName);
                foreach (var product in products)
                {
                    List<DataFlowStep> dataFlowSteps = new List<DataFlowStep>();

                    var steps = product.GroupBy(r => r.StepName);
                    foreach (var step in steps)
                    {
                        dataFlowSteps.Add(new DataFlowStep(step.Key,step.ToList()));
                    }

                    dataFlowProducts.Add(new DataFlowProduct(product.Key, dataFlowSteps));
                }
            }

            DataTreeSource.Reset(dataFlowProducts);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _dataFlowManager != null)
            {
                _dataFlowManager.DataFlowRecipeAdded -= DataFlowManager_DataFlowRecipeAdded;
                _dataFlowManager.DataFlowRecipeDeleted -= DataFlowManager_DataFlowRecipeDeleted;
            }
        }

        #endregion
    }
}
