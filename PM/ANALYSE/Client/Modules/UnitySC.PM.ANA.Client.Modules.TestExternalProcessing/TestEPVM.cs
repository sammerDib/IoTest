using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Modules.TestExternalProcessing.ViewModel;
using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.ANA.Client.Modules.TestExternalProcessing
{
    public class TestEPVM : ObservableObject, IMenuContentViewModel
    {
        public List<EPBaseVM> EPs { get; private set; }

        private EPBaseVM _selectedEP;
        public EPBaseVM SelectedEP
        {
            get => _selectedEP; set { if (_selectedEP != value) { _selectedEP = value; OnPropertyChanged(); } }
        }
        public TestEPVM()
        {
            EPs = new List<EPBaseVM>();
            EPs.Add(new MountainsVM());
            SelectedEP = EPs.First();
        }

        public bool IsEnabled => true;

        public bool CanClose()
        {
            return true;
        }

        public void Refresh()
        {
            foreach(var epVM in EPs)
            {
                epVM.Init();
            }
        }
    }
}
