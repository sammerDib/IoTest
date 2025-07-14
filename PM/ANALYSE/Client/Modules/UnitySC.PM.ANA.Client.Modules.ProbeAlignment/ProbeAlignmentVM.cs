using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel;
using UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.Lise;
using UnitySC.PM.ANA.Client.Modules.ProbeAlignment.ViewModel.LiseHF;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.Modules.ProbeAlignment
{
    public class ProbeAlignmentVM : ObservableObject, IMenuContentViewModel
    {
        #region Fields

        private ProbeAlignmentNavigationManager _navigationManager;

        #endregion

        #region Properties

        public bool IsEnabled => true;

        public ProbeAlignmentNavigationManager NavigationManager
        {
            get => _navigationManager ?? (_navigationManager = new ProbeAlignmentNavigationManager());
        }

        #endregion

        #region Constructors

        public ProbeAlignmentVM()
        {
            Refresh();
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            NavigationManager.RemoveAllPages();
            var probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            var configs = probeSupervisor.GetProbesConfig().Result;
            foreach (var probeConfig in configs)
            {
                var liseProbe = probeSupervisor.Probes.Find(probe => probe.DeviceID == probeConfig.DeviceID);
                switch (probeConfig)
                {
                    case ProbeLiseHFConfig _:
                        NavigationManager.AllPages.Add(new LiseHFAlignmentPanelVM(liseProbe as ProbeLiseHFVM));
                        break;
                    case ProbeLiseConfig _:
                        NavigationManager.AllPages.Add(new LiseAlignmentPanelVM(liseProbe as ProbeLiseVM));
                        break;
                    default:
                        break;
                }
            }

            NavigationManager.NavigateToPage(NavigationManager.GetFirstPage());
        }

        public bool CanClose()
        {
            NavigationManager.RemoveAllPages();
            return true;
        }

        #endregion
    }
}
