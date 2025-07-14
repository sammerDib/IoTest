using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;


namespace AdvancedModules.ClusterOperation.Sieve.ViewModel
{
    internal class SieveViewModel : ObservableRecipient
    {
        public SieveParameter Parameter { get; private set; }

        public ObservableCollection<SieveClassViewModel> SieveClassViewModelList { get; private set; }

        public SieveViewModel(SieveParameter pParameter)
        {
            Parameter = pParameter;
            SieveClassViewModelList = new ObservableCollection<SieveClassViewModel>();

        }
        private AutoRelayCommand _loadCommand;
        public AutoRelayCommand LoadCommand
        {
            get
            {
                return _loadCommand ?? (_loadCommand = new AutoRelayCommand(
              () =>
              {
                  Init();
              },
              () => { return true; }));
            }
        }
        public void Init()
        {
            Parameter.Synchronize();
            //-------------------------------------------------------------
            // Vérifie que toutes les classes de défauts sont bien gérées par
            // l'object Parameter et la Collection
            //-------------------------------------------------------------
            foreach (SieveClass sieveClass in Parameter.SieveClasses.Values)
            {
                bool found = SieveClassViewModelList.FirstOrDefault(x => x.DefectLabel == sieveClass.DefectLabel) != null;
                if (!found)
                {
                    SieveClassViewModel vm = new SieveClassViewModel(sieveClass, this);
                    SieveClassViewModelList.Add(vm);
                }
            }
            //-------------------------------------------------------------
            // Supprime les classes en trop dans le paramètre et la collection
            //-------------------------------------------------------------
            foreach (SieveClassViewModel vm in SieveClassViewModelList.ToList())
            {
                bool found = Parameter.SieveClasses.ContainsKey(vm.DefectLabel);
                if (!found)
                    SieveClassViewModelList.Remove(vm);
            }

        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            OnPropertyChanged(propertyName);
            Parameter.ReportChange();
        }
    }
}
