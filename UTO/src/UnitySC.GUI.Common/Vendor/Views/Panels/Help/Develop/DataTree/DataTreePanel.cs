using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using Agileo.Common.Localization;

using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.DragDrop;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.Develop.DataTree
{
    public enum MyModelType
    {
        AType,
        BType
    }

    public class MyModel
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public List<MyModel> Children { get; set; } = new();

        public MyModelType Type { get; set; }
    }

    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of the panel view
    /// </summary>
    public class DataTreePanel : BusinessPanel
    {
        public const int MaxItemSecurity = 50000;

        #region Properties

        public DataTreeSource<MyModel> DataTreeSource { get; set; }

        public List<int> IntSource { get; } = new List<int>
        {
            1,
            2,
            3,
            4,
            5,
            10,
            100,
            1000
        };

        private List<TreeNode<MyModel>> _allTreeElements;

        public List<TreeNode<MyModel>> AllTreeElements
        {
            get => _allTreeElements;
            set => SetAndRaiseIfChanged(ref _allTreeElements, value);
        }

        private int _itemsByLevel = 1;

        public int ItemsByLevel
        {
            get => _itemsByLevel;
            set
            {
                if (SetAndRaiseIfChanged(ref _itemsByLevel, value))
                {
                    PopulateDataTreeSource();
                }
            }
        }

        private int _recursiveLevel = 1;

        public int RecursiveLevel
        {
            get => _recursiveLevel;
            set
            {
                if (SetAndRaiseIfChanged(ref _recursiveLevel, value))
                {
                    PopulateDataTreeSource();
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
            get { return _selectedValue; }
            set
            {
                SetAndRaiseIfChanged(ref _selectedValue, value);
                OnPropertyChanged(nameof(SelectedTreeNode));
            }
        }

        public TreeNode<MyModel> SelectedTreeNode
        {
            get => DataTreeSource.SelectedElement;
            set => DataTreeSource.SelectedElement = value;
        }

        #endregion

        /// <summary>
        /// Initializes the <see cref="DataTreePanel"/> class.
        /// </summary>
        static DataTreePanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(DataTreeResources)));
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a design time instance of the <see cref="T:AgilController.Vendor.Views.Panels.Help.Develop.DataTree.DataTreePanel" /> class.
        /// </summary>
        public DataTreePanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode) { throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters."); }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:AgilController.Vendor.Views.Panels.Help.Develop.DataTree.DataTreePanel" /> class.
        /// </summary>
        /// <param name="id">Graphical identifier of the View Model. Can be either a <seealso cref="T:System.String" /> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public DataTreePanel(string id, IIcon icon = null) : base(id, icon)
        {
            DataTreeSource = new DataTreeSource<MyModel>(item => item.Children);

            DataTreeSource.Search.AddSearchDefinition(new InvariantText(nameof(MyModel.Name)), item => item.Name, true);
            DataTreeSource.Search.AddSearchDefinition(new InvariantText(nameof(MyModel.Index)), item => item.Index.ToString(CultureInfo.InvariantCulture), true);

            DataTreeSource.Sort.AddSortDefinition(nameof(MyModel.Name), node => node.Model.Name);
            DataTreeSource.Sort.AddSortDefinition(nameof(MyModel.Index), node => node.Model.Index);

            DataTreeSource.Filter.AddEnumFilter(new InvariantText(nameof(MyModel.Type)), node => node.Model.Type);
            DataTreeSource.Filter.AddRangeFilter(new InvariantText(nameof(MyModel.Index)), node => node.Model.Index, () => DataTreeSource.GetFlattenElements());

            DataTreeSource.DragDrop.DragDropImplementation = new DelegateDragDropImplementation<MyModel>(ApplyDragDropExecute, ApplyDragDropCanExecute);

            // Adds a keyboard shortcut for deleting an element.
            DataTreeSource.KeyGestureActions.Add(
                node => DeleteItemCommandExecute(node?.Model),
                node => DeleteItemCommandCanExecute(node?.Model),
                new KeyGesture(Key.Delete));

            PopulateDataTreeSource();
        }

        private bool ApplyDragDropCanExecute(DragDropActionParameter<MyModel> arg)
        {
            return true;
        }

        private bool ApplyDragDropExecute(DragDropActionParameter<MyModel> arg)
        {
            return true;
        }

        #region Populate

        private int _currentCount;

        private void PopulateDataTreeSource()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _currentCount = 0;

            var rootElement = new List<MyModel>();
            for (var i = 0; i < ItemsByLevel; i++)
            {
                if (_currentCount >= MaxItemSecurity) break;
                rootElement.Add(InstantiateModel("Item ", i));
            }

            if (RecursiveLevel > 1)
            {
                PopulateRecursively(rootElement, 2);
            }

            DataTreeSource.Reset(rootElement);

            stopwatch.Stop();

            DisplayFlattenItems();

            PopulateTimeSpan = $"{AllTreeElements.Count} items in {stopwatch.Elapsed.TotalMilliseconds}ms";
            GenerationLimitReached = AllTreeElements.Count >= MaxItemSecurity;
        }

        private bool _generationLimitReached;

        public bool GenerationLimitReached
        {
            get => _generationLimitReached;
            set => SetAndRaiseIfChanged(ref _generationLimitReached, value);
        }

        private void PopulateRecursively(IEnumerable<MyModel> models, int currentLevel)
        {
            foreach (var model in models)
            {
                if (_currentCount >= MaxItemSecurity) break;

                var currentChildren = new List<MyModel>();
                for (var i = 0; i < ItemsByLevel; i++)
                {
                    if (_currentCount >= MaxItemSecurity) break;
                    currentChildren.Add(InstantiateModel(model.Name, i));
                }

                if (currentLevel < RecursiveLevel)
                {
                    PopulateRecursively(currentChildren, currentLevel + 1);
                }
                model.Children = currentChildren;
            }
        }

        private MyModel InstantiateModel(string currentName, int currentIndex)
        {
            _currentCount++;
            return new MyModel
            {
                Index = _currentCount,
                Name = $"{currentName}.{currentIndex}",
                Type = _currentCount % 2 == 0 ? MyModelType.AType : MyModelType.BType
            };
        }

        private void DisplayFlattenItems()
        {
            AllTreeElements = DataTreeSource.GetFlattenElements();
        }

        #endregion

        #region Commands

        #region AutoExpand

        private ICommand _autoExpandCommand;

        public ICommand AutoExpandCommand => _autoExpandCommand ??= new DelegateCommand(AutoExpandCommandExecute);

        private bool _cancelationTokenSource = true;

        private void AutoExpandCommandExecute()
        {
            if (!_cancelationTokenSource)
            {
                _cancelationTokenSource = true;
            }
            else
            {
                _cancelationTokenSource = false;
                new TaskFactory().StartNew(AutoExpandLoop);
            }
        }

        private int _currentIndex;
        private bool _direction;

        private void AutoExpandLoop()
        {
            _currentIndex = 0;
            while (!_cancelationTokenSource)
            {
                var item = AllTreeElements.ElementAt(_currentIndex);

                if (!_direction)
                {
                    _currentIndex--;
                    if (_currentIndex < 0)
                    {
                        _direction = true;
                        _currentIndex++;
                    }
                }
                else
                {
                    _currentIndex++;
                    if (_currentIndex >= AllTreeElements.Count)
                    {
                        _direction = false;
                        _currentIndex--;
                    }
                }

                if (!item.IsExpandable) continue;

                CurrentItemProcess = $"Item {_currentIndex + 1}/{AllTreeElements.Count}";
                item.IsExpanded = _direction;
                Thread.Sleep(20);
            }
        }

        private string _currentItemProcess;

        public string CurrentItemProcess
        {
            get => _currentItemProcess;
            set => SetAndRaiseIfChanged(ref _currentItemProcess, value);
        }

        #endregion

        #region Delete Item

        private ICommand _deleteItemCommand;

        public ICommand DeleteItemCommand => _deleteItemCommand ??= new DelegateCommand<MyModel>(DeleteItemCommandExecute, DeleteItemCommandCanExecute);

        private bool DeleteItemCommandCanExecute(MyModel model) => model != null;

        private void DeleteItemCommandExecute(MyModel model)
        {
            DataTreeSource.Remove(model);
            DisplayFlattenItems();
        }

        #endregion

        #region Add Item

        private ICommand _addItemCommand;

        public ICommand AddItemCommand => _addItemCommand ??= new DelegateCommand<MyModel>(AddItemCommandExecute);

        private void AddItemCommandExecute(MyModel parent)
        {
            var newModel = new MyModel
            {
                Index = -1,
                Name = "Manually added item",
                Type = MyModelType.AType
            };

            if (parent != null)
            {
                DataTreeSource.Add(newModel, parent);
            }
            else
            {
                DataTreeSource.Add(newModel);
            }

            DisplayFlattenItems();
        }

        #endregion

        #endregion
    }
}

