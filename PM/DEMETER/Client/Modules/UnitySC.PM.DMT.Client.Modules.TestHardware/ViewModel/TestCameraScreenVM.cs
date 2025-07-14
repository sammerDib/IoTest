using System.Collections.Generic;
using System.ComponentModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class TestCameraScreenVM : ObservableRecipient, ITabManager
    {
        private readonly CameraSupervisor _cameraSupervisor;
        
        private readonly ScreenSupervisor _screenSupervisor;

        #region Properties

        public string Header { get => "Screen Camera"; }
        
        private Dictionary<Side, TestCameraVM> _cameraVMBySide;
        
        private Dictionary<Side, TestScreenVM> _screenVMBySide;
        
        private TestScreenVM _currentScreenVM;

        public TestScreenVM CurrentScreenVM
        {
            get => _currentScreenVM;
            set
            {
                var previousVM = _currentScreenVM;
                if (SetProperty(ref _currentScreenVM, value))
                {
                    previousVM?.Hide();
                    _currentScreenVM.Display();
                }
            }
        }

        private TestCameraVM _currentCameraVM;

        public TestCameraVM CurrentCameraVM
        {
            get => _currentCameraVM;
            set
            {
                var previousVM = _currentCameraVM;
                if (SetProperty(ref _currentCameraVM, value))
                {
                    if (!(previousVM is null))
                    {
                        previousVM.Hide();
                        previousVM.PropertyChanged -= HandleIsGrabbingChanged;
                    }

                    if (!(_currentCameraVM is null))
                    {
                        _currentCameraVM.PropertyChanged += HandleIsGrabbingChanged;
                        _currentCameraVM.Display();
                    }
                }
            }
        }

        private void HandleIsGrabbingChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is TestCameraVM vm && e.PropertyName == nameof(vm.IsGrabbing))
            {
                var screenVM = _screenVMBySide[vm.WaferSide];
                if (vm.IsGrabbing && screenVM.SetScreenWhiteColorCommand.IsRunning && screenVM.SetScreenWhiteColorCommand.CanBeCanceled)
                {
                    _screenVMBySide[vm.WaferSide].SetScreenWhiteColorCommand.Cancel();
                }
                _screenVMBySide[vm.WaferSide].IsWhiteDisplayedOnScreen = vm.IsGrabbing;
            }
        }

        private Side _currentVisibleSide = Side.Unknown;

        public Side CurrentVisibleSide
        {
            get => _currentVisibleSide;
            set
            {
                if (SetProperty(ref _currentVisibleSide, value))
                {
                    DisplaySelectedSetting(_currentVisibleSide);
                }
            }
        }

        #endregion Properties

        #region Constructor

        public TestCameraScreenVM(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService, IMessenger messenger) : base(messenger)
        {
            _cameraSupervisor = cameraSupervisor;
            _screenSupervisor = screenSupervisor;
            var camerasSide = _cameraSupervisor.GetCameraSides();
            _cameraVMBySide = new Dictionary<Side, TestCameraVM>(2);
            _screenVMBySide = new Dictionary<Side, TestScreenVM>(2);
            if (camerasSide.Contains(Side.Front))
            {
                _cameraVMBySide.Add(Side.Front, new TestCameraVM(Side.Front, _cameraSupervisor, _screenSupervisor, algorithmsSupervisor, dialogService, messenger));
                _screenVMBySide.Add(Side.Front, new TestScreenVM(Side.Front, _screenSupervisor, messenger));
                CurrentVisibleSide = Side.Front;
            }

            if (camerasSide.Contains(Side.Back))
            {
                _cameraVMBySide.Add(Side.Back, new TestCameraVM(Side.Back, _cameraSupervisor, _screenSupervisor, algorithmsSupervisor, dialogService, messenger));
                _screenVMBySide.Add(Side.Back, new TestScreenVM(Side.Back, _screenSupervisor, messenger));
                if (!camerasSide.Contains(Side.Front))
                {
                    CurrentVisibleSide = Side.Back;
                }
            }

            if (camerasSide.IsNullOrEmpty())
            {
                CurrentCameraVM = null;
                CurrentScreenVM = null;
            }
        }

        #endregion Constructor

        private void DisplaySelectedSetting(Side waferSide)
        {
            CurrentCameraVM = _cameraVMBySide[waferSide];
            CurrentScreenVM = _screenVMBySide[waferSide];
        }

        public void Display()
        {
            _currentScreenVM.Display();
            _currentCameraVM.Display();
        }

        public bool CanHide()
        {
            return (_currentCameraVM?.CanHide() ?? true) || (_currentScreenVM?.CanHide() ?? true);
        }

        public void Hide()
        {
            _currentScreenVM.Hide();
            _currentScreenVM.Hide();
        }
    }
}
