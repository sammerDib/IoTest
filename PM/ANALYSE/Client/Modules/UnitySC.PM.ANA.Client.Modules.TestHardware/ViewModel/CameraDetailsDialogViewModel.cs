using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class CameraDetailsDialogViewModel : ObservableObject, IModalDialogViewModel
    {
        #region Fields
        private CamerasSupervisor _camerasSupervisor;
        private bool? _dialogResult;
        #endregion

        #region Properties
        public CamerasSupervisor CamerasSupervisor
        {
            get
            {
                if (_camerasSupervisor == null)
                {
                    _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
                }

                return _camerasSupervisor;
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value, nameof(DialogResult));
        }
        #endregion
    }
}
