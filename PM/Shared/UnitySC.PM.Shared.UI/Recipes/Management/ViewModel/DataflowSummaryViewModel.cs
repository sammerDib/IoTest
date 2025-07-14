using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class DataflowSummaryViewModel : ObservableObject
    {
        private DataflowRecipeComponent _dataflowRecipeComponent;
        public DataflowSummaryViewModel(DataflowRecipeComponent dataflowRecipeComponent)
        {
            _dataflowRecipeComponent = dataflowRecipeComponent;
            DataFlowComponents = _dataflowRecipeComponent.AllChilds()
                .Where(x => x.ActorType != UnitySC.Shared.Data.Enum.ActorType.DataflowManager)
                .Select(x => new DataflowRecipeComponentViewModel(x));
        }

        public IEnumerable<DataflowRecipeComponentViewModel> DataFlowComponents { get; private set; }
    }

    public class DataflowRecipeComponentViewModel: ObservableObject
    {
        private DataflowRecipeComponent _component;


        public string Name => _component.Name;
        public string Comment => _component.Comment;
        public ActorType ActorType => (ActorType)_component.ActorType;

        public string Inputs { get; private set; }

        public string Ouputs { get; private set; }

        public DataflowRecipeComponentViewModel(DataflowRecipeComponent component)
        {
            _component = component;
            if (_component.Inputs != null)
                Inputs = string.Join(", ", _component.Inputs.Select(x => x.ToString()));
            if(_component.Outputs != null)
                Ouputs = string.Join(", ", _component.Outputs.Select(x => x.ToString()));
            
            Inputs = String.IsNullOrEmpty(Inputs) ? "Wafer" : Inputs;  
        }       

    }
}
