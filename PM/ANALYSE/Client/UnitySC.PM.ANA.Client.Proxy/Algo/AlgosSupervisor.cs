using System.ComponentModel;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.Proxy
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class AlgosSupervisor : IAlgoService, IAlgoServiceCallback
    {
        public delegate void AFLiseChangedHandler(AFLiseResult afLiseResult);

        public event AFLiseChangedHandler AFLiseChangedEvent;

        public delegate void AFCameraChangedHandler(AFCameraResult afCameraResult);

        public event AFCameraChangedHandler AFCameraChangedEvent;

        public delegate void AutoFocusChangedHandler(AutofocusResult autoFocusResult);

        public event AutoFocusChangedHandler AutoFocusChangedEvent;

        public delegate void AutoLightChangedHandler(AutolightResult autoLightResult);

        public event AutoLightChangedHandler AutoLightChangedEvent;

        public delegate void CheckWaferPresenceChangedHandler(CheckWaferPresenceResult waferPresenceResult);

        public event CheckWaferPresenceChangedHandler CheckWaferPresenceChangedEvent;

        public delegate void ImagePreprocessingChangedHandler(ImagePreprocessingResult imagePreprocessingResult);

        public event ImagePreprocessingChangedHandler ImagePreprocessingChangedEvent;

        public delegate void PatternRecChangedHandler(PatternRecResult patternRecResult);

        public event PatternRecChangedHandler PatternRecChangedEvent;

        public delegate void BWAChangedHandler(BareWaferAlignmentChangeInfo bwaResult);

        public event BWAChangedHandler BWAChangedEvent;

        public event BWAChangedHandler BWAImageChangedEvent;

        public delegate void AutoAlignChangedHandler(AutoAlignResult autoAlignResult);

        public event AutoAlignChangedHandler AutoAlignChangedEvent;

        public delegate void AlignmentMarksChangedHandler(AlignmentMarksResult alignmentMarksResult);

        public event AlignmentMarksChangedHandler AlignmentMarksChangedEvent;

        public delegate void CheckPatternRecChangedHandler(CheckPatternRecResult checkPatternRecResult);

        public event CheckPatternRecChangedHandler CheckPatternRecChangedEvent;

        public delegate void DieAndStreetSizesChangedHandler(DieAndStreetSizesResult dieAndStreeSizesResult);

        public event DieAndStreetSizesChangedHandler DieAndStreetSizesChangedEvent;

        public delegate void WaferMapChangedHandler(WaferMapResult waferMapResult);

        public event WaferMapChangedHandler WaferMapChangedEvent;

        private DuplexServiceInvoker<IAlgoService> _algoService;

        public AlgosSupervisor()
        {
            var instanceContext = new InstanceContext(this);
            var isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
            if (isInDesignMode)
                return;
            _algoService = new DuplexServiceInvoker<IAlgoService>(instanceContext,
               "ANALYSEAlgoService", ClassLocator.Default.GetInstance<SerilogLogger<IAlgoService>>(), ClassLocator.Default.GetInstance<IMessenger>(), x => x.SubscribeToAlgoChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
        }

        #region AF Lise

        public Response<AFLiseSettings> GetAFLiseSettings(AFLiseInput input, ObjectiveConfig objectiveConfig = null)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.GetAFLiseSettings(input, objectiveConfig));
        }

        public Response<VoidResult> StartAFLise(AFLiseInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAFLise(input));
        }

        public void AFLiseChanged(AFLiseResult afResult)
        {
            AFLiseChangedEvent?.Invoke(afResult);
        }

        public Response<VoidResult> CancelAFLise()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAFLise());
        }

        #endregion AF Lise

        #region AF Image

        public Response<VoidResult> StartAFCamera(AFCameraInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAFCamera(input));
        }

        public void AFCameraChanged(AFCameraResult afResult)
        {
            AFCameraChangedEvent?.Invoke(afResult);
        }

        public Response<VoidResult> CancelAFCamera()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAFCamera());
        }

        #endregion AF Image

        #region AutoFocus

        public Response<VoidResult> StartAutoFocus(AutofocusInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAutoFocus(input));
        }

        public void AutoFocusChanged(AutofocusResult afResult)
        {
            AutoFocusChangedEvent?.Invoke(afResult);
        }

        public Response<VoidResult> CancelAutoFocus()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAutoFocus());
        }

        #endregion AutoFocus

        #region AutoLight

        public Response<VoidResult> StartAutoLight(AutolightInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAutoLight(input));
        }

        public void AutoLightChanged(AutolightResult alResult)
        {
            AutoLightChangedEvent?.Invoke(alResult);
        }

        public Response<VoidResult> CancelAutoLight()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAutoLight());
        }

        #endregion AutoLight

        #region Pattern Rec

        public Response<VoidResult> StartPatternRec(PatternRecInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartPatternRec(input));
        }

        public void PatternRecChanged(PatternRecResult prResult)
        {
            PatternRecChangedEvent?.Invoke(prResult);
        }

        public Response<VoidResult> CancelPatternRec()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelPatternRec());
        }

        #endregion Pattern Rec

        #region BWA

        public Response<BareWaferAlignmentSettings> GetBWASettings(BareWaferAlignmentInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.GetBWASettings(input));
        }

        public Response<VoidResult> StartBWA(BareWaferAlignmentInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartBWA(input));
        }

        public Response<VoidResult> CancelBWA()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelBWA());
        }

        public void BwaChanged(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            BWAChangedEvent?.Invoke(bwaChangeInfo);
        }

        public void BwaImageChanged(BareWaferAlignmentImage bwaChangeInfo)
        {
            BWAImageChangedEvent?.Invoke(bwaChangeInfo);
        }

        public Response<VoidResult> StartBWAImage(BareWaferAlignmentImageInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartBWAImage(input));
        }

        public Response<VoidResult> CancelBWAImage()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelBWAImage());
        }

        #endregion BWA

        #region AutoAlign

        public Response<VoidResult> StartAutoAlign(AutoAlignInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAutoAlign(input));
        }

        public void AutoAlignChanged(AutoAlignResult autoAlignResult)
        {
            AutoAlignChangedEvent?.Invoke(autoAlignResult);
        }

        public Response<VoidResult> CancelAutoAlign()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAutoAlign());
        }

        #endregion AutoAlign

        #region AlignmentMarks

        public Response<VoidResult> StartAlignmentMarks(AlignmentMarksInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartAlignmentMarks(input));
        }

        public void AlignmentMarksChanged(AutolightResult alResult)
        {
            AutoLightChangedEvent?.Invoke(alResult);
        }

        public Response<VoidResult> CancelAlignmentMarks()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelAlignmentMarks());
        }

        public void AlignmentMarksChanged(AlignmentMarksResult amResult)
        {
            AlignmentMarksChangedEvent?.Invoke(amResult);
        }

        #endregion AlignmentMarks

        #region ProcessingImage

        public Response<VoidResult> StartImagePreprocessing(ImagePreprocessingInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartImagePreprocessing(input));
        }

        public void ImagePreprocessingChanged(ImagePreprocessingResult imagePreprocessingResult)
        {
            ImagePreprocessingChangedEvent?.Invoke(imagePreprocessingResult);
        }

        #endregion ProcessingImage

        #region CheckPatternRec

        public Response<CheckPatternRecSettings> GetCheckPatternRecSettings()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.GetCheckPatternRecSettings());
        }

        public Response<VoidResult> StartCheckPatternRec(CheckPatternRecInput checkPatternRecInput)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartCheckPatternRec(checkPatternRecInput));
        }

        public Response<VoidResult> CancelCheckPatternRec()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelCheckPatternRec());
        }

        public void CheckPatternRecChanged(CheckPatternRecResult checkPatternRecResult)
        {
            CheckPatternRecChangedEvent?.Invoke(checkPatternRecResult);
        }

        #endregion CheckPatternRec

        public Response<VoidResult> SubscribeToAlgoChanges()
        {
            return _algoService.TryInvokeAndGetMessages(s => s.SubscribeToAlgoChanges());
        }

        public Response<VoidResult> UnsubscribeToAlgoChanges()
        {
            return _algoService.TryInvokeAndGetMessages(s => s.UnsubscribeToAlgoChanges());
        }

        #region DieAndStreetSizes

        public Response<VoidResult> StartDieAndStreetSizes(DieAndStreetSizesInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartDieAndStreetSizes(input));
        }

        public Response<VoidResult> CancelDieAndStreetSizes()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelDieAndStreetSizes());
        }

        public void DieAndStreetSizesChanged(DieAndStreetSizesResult dsapResult)
        {
            DieAndStreetSizesChangedEvent?.Invoke(dsapResult);
        }

        #endregion DieAndStreetSizes

        #region WaferMap

        public Response<VoidResult> StartWaferMap(WaferMapInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.StartWaferMap(input));
        }

        public Response<VoidResult> CancelWaferMap()
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CancelWaferMap());
        }

        public void WaferMapChanged(WaferMapResult wmResult)
        {
            WaferMapChangedEvent?.Invoke(wmResult);
        }

        #endregion WaferMap

        #region AdvencedFlow

        public Response<VoidResult> CheckWaferPresence(CheckWaferPresenceInput input)
        {
            return _algoService.TryInvokeAndGetMessages(x => x.CheckWaferPresence(input));
        }

        public void CheckWaferPresenceChanged(CheckWaferPresenceResult wpResult)
        {
            CheckWaferPresenceChangedEvent?.Invoke(wpResult);
        }

        #endregion AdvencedFlow
    }
}
