using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Graph.Model;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel.Graph
{
    public class DataflowNodeViewModel: NodeViewModel, ICloneable
    {
        private DataflowRecipeComponent _root;
        private DataflowRecipeComponent _dataflowRecipeComponent;
        public Guid Key => _dataflowRecipeComponent.Key;
        public ActorType ActorType => _dataflowRecipeComponent.ActorType;
        public List<DataflowRecipeComponent> Children => _dataflowRecipeComponent.ChildRecipes.Select(x => x.Component).ToList();
        public DataflowRecipeComponent Model => _dataflowRecipeComponent;


        public bool IsShared
        {
            get => _dataflowRecipeComponent.IsShared; 
            set 
            { 
                if (_dataflowRecipeComponent.IsShared != value) 
                { 
                    _dataflowRecipeComponent.IsShared = value;
                    OnPropertyChanged();
                }
            }
        }



        private DataflowNodeStatus _status = DataflowNodeStatus.Unknow;
        public DataflowNodeStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }


        }

        public List<DataflowRecipeComponent> Parents
        {
            get
            {                
                var parents = new List<DataflowRecipeComponent>();
                foreach(var recipeComponent in _root.AllChilds())
                {
                    if (recipeComponent.ChildRecipes.Any(x => x.Component.Key == Key))
                        parents.Add(recipeComponent);
                }
                return parents;
            }
        }



        public DataflowNodeViewModel(DataflowRecipeComponent dataflowRecipeComponent, DataflowRecipeComponent root)
        {
            _dataflowRecipeComponent = dataflowRecipeComponent;
            Data = _dataflowRecipeComponent;
            _root = root;
        }

        public DataflowRecipeComponent Component { get { return _dataflowRecipeComponent; } }

        public override Object Data
        {
            get { return base.Data; }
            set
            {
                base.Data = value;
                if (_dataflowRecipeComponent.ActorType == ActorType.DataflowManager)
                    Name = "Dataflow";
                else
                    Name = _dataflowRecipeComponent.Name;
            }
        }

        public object Clone()
        {
            DataflowNodeViewModel cloneNode = new DataflowNodeViewModel(_dataflowRecipeComponent, _root);

            cloneNode.Name = Name;
            cloneNode.Info = Info;
            cloneNode.X = X;
            cloneNode.Y = Y;
            cloneNode.ZIndex = ZIndex;
            cloneNode.Size = Size;

            foreach (ConnectorViewModel connector in cloneNode.OutputConnectors)
                connector.ParentNode = cloneNode;
            foreach (ConnectorViewModel connector in cloneNode.InputConnectors)
                connector.ParentNode = cloneNode;

            return cloneNode;
        }

        internal void Update()
        {
            var _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();           
            _dataflowRecipeComponent = _dbRecipeService.Invoke(x => x.GetLastRecipe(Key, false, false)).ToDataflowRecipeComponent();
            Data = _dataflowRecipeComponent;
        }
    }


    public enum DataflowNodeStatus
    {
        Unknow = 0,

        Available = 1,
        Executing = 2,
        Terminated = 3,
        Warning = 4,
        Error = 5,
    }
}
