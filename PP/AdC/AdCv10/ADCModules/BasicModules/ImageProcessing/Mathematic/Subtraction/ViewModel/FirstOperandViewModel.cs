using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Mathematic
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class FirstOperandParameterViewModel : ObservableRecipient
    {
        public ObservableCollection<KeyValuePair<int, string>> BranchCollection { get; private set; } = new ObservableCollection<KeyValuePair<int, string>>();

        public string Label { get { return _parameter.Label; } }

        public int FirstOperand
        {
            get { return _parameter.FirstOperandIndex; }
            set
            {
                if (value == _parameter.FirstOperandIndex)
                    return;
                _parameter.FirstOperandIndex = value;
                OnPropertyChanged();
            }
        }

        private FirstOperandParameter _parameter;

        public FirstOperandParameterViewModel(FirstOperandParameter parameter)
        {
            _parameter = parameter;
        }

        public void Synchronize()
        {
            BranchCollection.Clear();

            for (int i = 0; i < _parameter.Module.Parents.Count(); i++)
            {
                ModuleBase mod = _parameter.Module.Parents[i];
                BranchCollection.Add(new KeyValuePair<int, string>(i, mod.DisplayName));
            }
            OnPropertyChanged(nameof(FirstOperand));
        }
    }
}
