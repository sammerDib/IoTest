using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Input;


using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable
{
    public enum MyModelType
    {
        AType,
        BType
    }

    public class MyModel
    {
        public string Name { get; set; }

        public int Id { get; set; }

        public MyModelType Type { get; set; }

        public string Value { get; set; }
    }

    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class DataTablePanel : BusinessPanel
    {
        private const int MaxItemSecurity = 1000000;

        #region Properties

        public DataTableSource<MyModel> DataTableSource { get; set; } = new();

        public List<int> IntSource { get; } = new()
        {
            1,
            10,
            100,
            1000,
            10000,
            100000
        };

        private int _itemCount = 100;

        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (SetAndRaiseIfChanged(ref _itemCount, value))
                {
                    PopulateDataTableSource();
                }
            }
        }
        
        private string _populateTimeSpan;

        public string PopulateTimeSpan
        {
            get => _populateTimeSpan;
            private set => SetAndRaiseIfChanged(ref _populateTimeSpan, value);
        }

        private MyModel _selectedValue;

        public MyModel SelectedValue
        {
            get => _selectedValue;
            set => SetAndRaiseIfChanged(ref _selectedValue, value);
        }

        #endregion

        /// <summary>
        /// Initializes the <see cref="DataTablePanel"/> class.
        /// </summary>
        static DataTablePanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataTablePanelResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable.DataTablePanel" /> class.
        /// </summary>
        public DataTablePanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTable.DataTablePanel" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public DataTablePanel(string id, IIcon icon = null) : base(id, icon)
        {
            DataTableSource.Search.AddSearchDefinition(new InvariantText(nameof(MyModel.Id)), item => item.Id.ToString(CultureInfo.InvariantCulture), true);
            DataTableSource.Search.AddSearchDefinition(new InvariantText(nameof(MyModel.Name)), item => item.Name, true);
            DataTableSource.Search.AddSearchDefinition(new InvariantText(nameof(MyModel.Value)), item => item.Value, true);

            DataTableSource.Sort.AddSortDefinition(nameof(MyModel.Type), node => node.Type);
            DataTableSource.Sort.AddSortDefinition(nameof(MyModel.Id), node => node.Id);
            DataTableSource.Sort.AddSortDefinition(nameof(MyModel.Name), node => node.Name);
            DataTableSource.Sort.AddSortDefinition(nameof(MyModel.Value), node => node.Value);
            DataTableSource.Sort.SetCurrentSorting(nameof(MyModel.Id));

            DataTableSource.Filter.AddEnumFilter(nameof(DataTablePanelResources.TYPE), node => node.Type);
            DataTableSource.Filter.AddRangeFilter(nameof(DataTablePanelResources.ID), node => node.Id, () => DataTableSource);

            PopulateDataTableSource();
        }

        #region Populate
        
        private void PopulateDataTableSource()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var items = new List<MyModel>();
            for (var i = 0; i < ItemCount; i++)
            {
                if (i >= MaxItemSecurity) break;
                items.Add(InstantiateModel(i + 1));
            }
            
            DataTableSource.Reset(items);

            stopwatch.Stop();

            PopulateTimeSpan = $"{stopwatch.Elapsed.TotalMilliseconds}ms";
        }
        
        private MyModel InstantiateModel(int currentIndex)
        {
            return new MyModel
            {
                Id = currentIndex,
                Name = $"Item {currentIndex}",
                Type = currentIndex % 2 == 0 ? MyModelType.AType : MyModelType.BType,
                Value = Guid.NewGuid().ToString()
            };
        }

        #endregion

        #region Commands

        #region Delete Item

        private ICommand _deleteItemCommand;

        public ICommand DeleteItemCommand => _deleteItemCommand ??= new DelegateCommand(DeleteItemCommandExecute, DeleteItemCommandCanExecute);

        private bool DeleteItemCommandCanExecute() => SelectedValue != null;

        private void DeleteItemCommandExecute() => DataTableSource.Remove(SelectedValue);

        #endregion

        #region Add Item

        private ICommand _addItemCommand;

        public ICommand AddItemCommand => _addItemCommand ??= new DelegateCommand(AddItemCommandExecute);

        private void AddItemCommandExecute()
        {
            var nextId = DataTableSource.Max(model => model.Id);
            var newModel = InstantiateModel(nextId + 1);
            DataTableSource.Add(newModel);

            SelectedValue = newModel;
        }

        #endregion

        #endregion
    }
}

