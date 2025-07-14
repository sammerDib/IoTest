using Agileo.GUI.Components;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.DataDictionary
{
    public class E30VariableViewModel : Notifier
    {
        public E30VariableViewModel(E30Variable variable)
        {
            Variable = variable;
        }

        public E30Variable Variable { get; }

        public DataItem Value => Variable.Value;

        public void NotifyValueChanged()
        {
            OnPropertyChanged(nameof(Value));
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetAndRaiseIfChanged(ref _isExpanded, value);
        }
    }
}
