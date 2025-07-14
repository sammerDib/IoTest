using System;
using System.ServiceModel;

using UnitySC.PM.AGS.Data;
using UnitySC.PM.AGS.Hardware.Manager;
using UnitySC.PM.AGS.Service.Interface.AcquisitionService;
using UnitySC.PM.AGS.Service.Interface.Flow;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Data.ProcessingImage;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ArgosAcquisitionService : DuplexServiceBase<IAcquisitionServiceCallback>, IAcquisitionService
    {
        private object _mutex;

        public ArgosAcquisitionService(ILogger logger, ExceptionType serviceExceptionType) : base(logger,
            ExceptionType.RecipeException)
        {
        }

#pragma warning disable CS0067 //Event used but not detected by compiler

        public event ReportProgressEventHandler Progress;

        private ArgosHardwareManager _hardwareManager = ClassLocator.Default.GetInstance<ArgosHardwareManager>();

        Response<VoidResult> IAcquisitionService.Subscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_mutex)
                {
                    _logger.Information("Subscribed to Acquisition status change");
                    Subscribe();
                }
            });
        }

        Response<VoidResult> IAcquisitionService.Unsubscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_mutex)
                {
                    _logger.Information("Unsubscribed to Acquisition status change");
                    Unsubscribe();
                }
            });
        }

        Response<USPImageMil> IAcquisitionService.GetSingleImage(string camera)
        {
            // GVA : Too far from Analysis and modere
            //return InvokeDataResponse(messagesContainer => _hardwareManager.Cameras[camera].SingleGrab());
            throw new NotImplementedException();
        }

        Response<PmWaferInfo> IAcquisitionService.GetDefaultWaferInfo()
        {
            return InvokeDataResponse(messagesContainer => new PmWaferInfo());
        }

        Response<AcquisitionResult> IAcquisitionService.StartAcquisition(ArgosRecipe recipe, PmWaferInfo waferInfo, bool overwriteOutput)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                Acquisition.Acquisition acquisition = new Acquisition.Acquisition(_logger, recipe, waferInfo, overwriteOutput);
                return acquisition.Start();
            });
        }

        Response<ArgosRecipe> IAcquisitionService.StartAutoSetting(string screenId)
        {
            return InvokeDataResponse(messagesContainer => new AutoSettingTask().Start());
        }

        Response<VoidResult> IAcquisitionService.AbortAcquisition()
        {
            /*return InvokeVoidResponse(messagesContainer =>
            {
                InvokeCallback(x => Acquisition.Abort());
            });*/
            throw new NotImplementedException();
        }
    }
}
