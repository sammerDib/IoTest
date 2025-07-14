using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel;
using UnitySC.PM.ANA.Client.Proxy.KeyboardMouseHook;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.ANA.Client.Controls.NavigationControls;
using UnitySC.PM.Shared.Configuration;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class AlgoSupervisor : ViewModelBase, IAlgoServiceCallback
    {
        private IKeyboardMouseHook _keyboardMouseHook;

        private DuplexServiceInvoker<IAlgoService> _algoService;
        public List<AlgoBaseVM> Algos { get; private set; }
        public List<WaferCategory> WaferCategories { get; private set; }

        private OpticalReferenceDefinition _selectedReference;

        public OpticalReferenceDefinition SelectedReference
        {
            get
            {
                return _selectedReference;
            }

            set
            {
                if (_selectedReference == value)
                {
                    return;
                }

                _selectedReference = value;
                RaisePropertyChanged();
            }
        }

        private WaferCategory _selectedCategory;

        public WaferCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    if (_selectedCategory != null)
                        WaferDimentionalCharac = _selectedCategory.DimentionalCharacteristic;
                    RaisePropertyChanged();
                }
            }
        }

        private WaferDimensionalCharacteristic _waferDimentionalCharac;

        public WaferDimensionalCharacteristic WaferDimentionalCharac
        {
            get => _waferDimentionalCharac; set { if (_waferDimentionalCharac != value) { _waferDimentionalCharac = value; RaisePropertyChanged(); } }
        }

        public DieDimensionalCharacteristic DieDimentionalCharac => null;

        public bool IsBusy
        {
            get => Algos.Any(x => x.IsBusy);
        }

        private AlgoBaseVM _selectedAlgo;

        public AlgoBaseVM SelectedAlgo
        {
            get => _selectedAlgo; set { if (_selectedAlgo != value) { _selectedAlgo = value; RaisePropertyChanged(); } }
        }

        public AlgoSupervisor()
        {
            SelectedReference = ClassLocator.Default.GetInstance<ChuckSupervisor>().ChuckVM?.ConfigurationChuck?.ReferencesList?.FirstOrDefault();
            _keyboardMouseHook = ClassLocator.Default.GetInstance<IKeyboardMouseHook>();
            var instanceContext = new InstanceContext(this);
            var toolService = new ServiceInvoker<IToolService>("ToolService",
                ClassLocator.Default.GetInstance<SerilogLogger<IToolService>>(), ClassLocator.Default.GetInstance<IMessenger>(),
                ClientConfiguration.GetDataAccessAddress());
            WaferCategories = toolService.Invoke(x => x.GetWaferCategories());
            SelectedCategory = WaferCategories.OrderByDescending(x => x.DimentionalCharacteristic.Diameter).First();
            _algoService = new DuplexServiceInvoker<IAlgoService>(instanceContext,
                "ANALYSEAlgoService", ClassLocator.Default.GetInstance<SerilogLogger<IAlgoService>>(), ClassLocator.Default.GetInstance<IMessenger>(), x => x.SubscribeToAlgoChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
            Algos = new List<AlgoBaseVM>();
            Algos.Add(new AFLiseVM());
            Algos.Add(new AutolightVM());
            Algos.Add(new AFCameraVM());
            Algos.Add(new PatternRecVM());
            Algos.Add(new BwaVM(this));
            //Algos.Add(new AutoAlignVM()); // Todo remove this test
            SelectedAlgo = Algos.First();
            _keyboardMouseHook.KeyUpDownEvent += KeyboardMouseHook_KeyUpDownEvent;
        }

        private void KeyboardMouseHook_KeyUpDownEvent(object sender, KeyGlobalEventArgs e)
        {
            if (e.CurrentKey == Key.F4)
            {
                if (!Algos.Any(x => x.IsBusy))
                {
                    SelectedAlgo = Algos.OfType<AFLiseVM>().First();
                    //StartAFLise(); TODO fix this
                }
            }
        }

        #region AF Lise

        public void StartAFLise(AFLiseInput input)
        {
            _algoService.InvokeAndGetMessages(x => x.StartAFLise(input));
            Algos.OfType<AFLiseVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void AFLiseChanged(AFLiseResult afResult)
        {
            var liseAF = Algos.OfType<AFLiseVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (afResult != null)
                    liseAF.UpdateResult(afResult);
                RaisePropertyChanged(() => IsBusy);
            }));
        }

        public void CancelAFLise()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelAFLise());
        }

        #endregion AF Lise

        #region AF Image

        public void StartAFCamera(AFCameraInput input)
        {
            _algoService.InvokeAndGetMessages(x => x.StartAFCamera(input));
            Algos.OfType<AFCameraVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void AFCameraChanged(AFCameraResult afResult)
        {
            var imageAF = Algos.OfType<AFCameraVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (afResult != null)
                    imageAF.UpdateResult(afResult);
                RaisePropertyChanged(() => IsBusy);
            }));
        }

        public void CancelAFCamera()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelAFCamera());
        }

        #endregion AF Image

        #region Auto Focus

        public void AutoFocusChanged(AutoFocusResult afResult)
        {
            throw new NotImplementedException();
        }

        #endregion Auto Focus

        #region AutoLight

        public void StartAutoLight(AutolightInput input)
        {
            _algoService.InvokeAndGetMessages(x => x.StartAutoLight(input));
            Algos.OfType<AutolightVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void AutoLightChanged(AutolightResult alResult)
        {
            var autoLight = Algos.OfType<AutolightVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (alResult != null)
                {
                    autoLight.UpdateResult(alResult);
                    RaisePropertyChanged(() => IsBusy);
                }
            }));
        }

        public void CancelAutoLight()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelAutoLight());
        }

        #endregion AutoLight

        #region Preprocessing Image

        public void StartImagePreprocessing(PatternRecInput input)
        {
            _algoService.InvokeAndGetMessages(x => x.StartPatternRec(input));
            Algos.OfType<PatternRecVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }
        public void ImagePreprocessingChanged(ImagePreprocessingResult prResult)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Pattern Rec

        public void StartPatternRec(PatternRecInput input)
        {
            _algoService.InvokeAndGetMessages(x => x.StartPatternRec(input));
            Algos.OfType<PatternRecVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void PatternRecChanged(PatternRecResult prResult)
        {
            var patternRec = Algos.OfType<PatternRecVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (prResult != null)
                    patternRec.UpdateResult(prResult);
                RaisePropertyChanged(() => IsBusy);
            }));
        }

        public void CancelPatternRec()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelPatternRec());
        }

        #endregion Pattern Rec

        #region BWA

        public void StartBWA()
        {
            var activeCameraId = ClassLocator.Default.GetInstance<CamerasSupervisor>().Camera.Configuration.DeviceID;
            var bwaInput = new BareWaferAlignmentInput(SelectedCategory.DimentionalCharacteristic, activeCameraId);
            _algoService.InvokeAndGetMessages(x => x.StartBWA(bwaInput));
            Algos.OfType<BwaVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void BwaChanged(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            var bwa = Algos.OfType<BwaVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (bwaChangeInfo is BareWaferAlignmentResult newResult)
                    bwa.UpdateResult(newResult);
                RaisePropertyChanged(() => IsBusy);
            }));
        }

        public void CancelBwa()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelBWA());
        }

        public void BwaImageChanged(BareWaferAlignmentImage bwaResult)
        {
            // Nothing
        }

        #endregion BWA

        #region AutoAlign

        public void StartAutoAlign()
        {
            _algoService.InvokeAndGetMessages(x => x.StartAutoAlign(SelectedCategory));
            Algos.OfType<AutoAlignVM>().Single().IsBusy = true;
            RaisePropertyChanged(() => IsBusy);
        }

        public void AutoAlignChanged(AutoAlignResult autoAlignResultd)
        {
            var bwa = Algos.OfType<AutoAlignVM>().Single();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (autoAlignResultd != null)
                    bwa.UpdateResult(autoAlignResultd);
                RaisePropertyChanged(() => IsBusy);
            }));
        }

        public void CancelAutoAlign()
        {
            _algoService.InvokeAndGetMessages(x => x.CancelAutoAlign());
        }

        #endregion AutoAlign

        public void DieAndStreetSizesChanged(DieAndStreetSizesResult dsapResult)
        {
            throw new NotImplementedException();
        }

        public void CheckPatternRecChanged(CheckPatternRecResult checkPatternRecResult)
        {
            // Nothing
        }

        public void WaferMapChanged(WaferMapResult dsapResult)
        {
            throw new NotImplementedException();
        }

        public void AlignmentMarksChanged(AlignmentMarksResult alResult)
        {
            throw new NotImplementedException();
        }
    }
}
