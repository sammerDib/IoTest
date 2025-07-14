using AdcBasicObjects;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.MilCharacterization
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ListViewItem : ObservableRecipient
    {
        private MilCharacterizationParameter Parameter;
        public Characteristic Characteristic;
        public ListViewItem(MilCharacterizationParameter parameter, Characteristic characteristic)
        {
            Parameter = parameter;
            Characteristic = characteristic;
        }

        public bool IsSelected
        {
            get { return Parameter.ClusterCharacteristicList.Contains(Characteristic); }
            set
            {
                if (value)
                    Parameter.ClusterCharacteristicList.TryAdd(Characteristic);
                else
                    Parameter.ClusterCharacteristicList.Remove(Characteristic);
                OnPropertyChanged();
                Parameter.ReportChange();
            }
        }

        public string Name
        {
            get { return Characteristic.Name; }
        }

        public string Label
        {
            get
            {
                return Parameter.OptionToLabel(Name);
            }
        }

        public override string ToString()
        {
            return "{" + Name + "}";
        }
    }

}
