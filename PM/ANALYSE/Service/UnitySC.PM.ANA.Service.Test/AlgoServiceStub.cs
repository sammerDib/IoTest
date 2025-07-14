using System;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Test
{
    /**
     * Stub implementation for testing purposes.
     */

    //internal class AlgoServiceStub : IAlgoService, IFlowEvent
    //{
    //    public void AddFlowEventListener(EventHandler<FlowEventArgs> l)
    //    {
    //        // nop
    //    }

    //    public void RemoveFlowEventListener(EventHandler<FlowEventArgs> l)
    //    {
    //        // nop
    //    }

    //    public Response<VoidResult> CancelAFCamera()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelAFLise()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelAutoAlign()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelAutoLight()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelBWA()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelPatternRec()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void OnRaiseFlowEvent(FlowEventArgs e)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartAFCamera(AFCameraInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartAFLise(AFLiseInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartAutoAlign(WaferCategory wafer)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartAutoLight(AutolightInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartBWA(BareWaferAlignmentInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartPatternRec(ServiceImage refImage, double gamma)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> SubscribeToAlgoChanges()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> UnsubscribeToAlgoChanges()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartBWAImage(BareWaferAlignmentImageInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelBWAImage()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<AFLiseSettings> GetAFLiseSettings(AFLiseInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<BareWaferAlignmentSettings> GetBWASettings(BareWaferAlignmentInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartCheckPatternRec(CheckPatternRecInput checkPatternRecInput)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelCheckPatternRec()
    //    {
    //        throw new NotImplementedException();
    //    }
    //public Response<VoidResult> StartDieSizeAndPitch(DieSizeAndPitchInput input)
    //{
    //    throw new NotImplementedException();
    //}

    //    public Response<CheckPatternRecSettings> GetCheckPatternRecSettings()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartDieSizeAndPitch(DieSizeAndPitchInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelDieSizeAndPitch()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> StartWaferMap(WaferMapInput input)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Response<VoidResult> CancelWaferMap()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
