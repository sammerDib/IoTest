using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Sizing
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class SizingViewModel : ObservableRecipient
    {
        public ObservableCollection<SizingClassViewModel> SizingClassCollection { get; private set; } = new ObservableCollection<SizingClassViewModel>();


        private SizingParameter _parameter;

        public SizingViewModel(SizingParameter parameter)
        {
            _parameter = parameter;
        }


        public void Synchronize()
        {
            SizingClassCollection.Clear();

            _parameter.SynchronizeWithClassification();

            foreach (SizingClass sizingClass in _parameter.SizingClasses.Values)
            {
                var vm = new SizingClassViewModel(sizingClass, _parameter);
                SizingClassCollection.Add(vm);
            }
        }

    }
}
