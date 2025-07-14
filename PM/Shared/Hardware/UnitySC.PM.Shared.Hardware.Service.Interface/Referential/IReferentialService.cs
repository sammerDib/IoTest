using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Referential
{
    [ServiceContract]
    [ServiceKnownType(typeof(XPosition))]
    [ServiceKnownType(typeof(YPosition))]
    [ServiceKnownType(typeof(ZTopPosition))]
    [ServiceKnownType(typeof(ZBottomPosition))]
    [ServiceKnownType(typeof(ZPiezoPosition))]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomMove))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(MotorReferential))]
    [ServiceKnownType(typeof(StageReferential))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(DieReferential))]
    [ServiceKnownType(typeof(WaferReferentialSettings))]
    [ServiceKnownType(typeof(StageReferentialSettings))]
    [ServiceKnownType(typeof(DieReferentialSettings))]
    public interface IReferentialService
    {
        /// <summary>
        /// Convert a position into a position in a new referential
        /// </summary>
        /// <param name="positionToConvert">Position to convert</param>
        /// <param name="referentialTo">Destination referential</param>
        /// <returns></returns>
        [OperationContract]
        Response<PositionBase> ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo);

        /// <summary>
        /// Get referential settings used by converter
        /// </summary>
        [OperationContract]
        Response<ReferentialSettingsBase> GetSettings(ReferentialTag referentialTag);

        /// <summary>
        /// Set referential settings used by converter
        /// </summary>
        [OperationContract]
        Response<VoidResult> SetSettings(ReferentialSettingsBase settings);

        /// <summary>
        /// Delete referential settings used by converter
        /// </summary>
        [OperationContract]
        Response<VoidResult> DeleteSettings(ReferentialTag referentialTag);

        /// <summary>
        /// Enable converter between two referentials
        /// </summary>
        /// <param name="referentialTag1"></param>
        /// <param name="referentialTag2"></param>

        [OperationContract]
        Response<VoidResult> EnableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2);

        /// <summary>
        /// Disable converter between two referentials
        /// </summary>
        /// <param name="referentialTag1"></param>
        /// <param name="referentialTag2"></param>

        [OperationContract]
        Response<VoidResult> DisableReferentialConverter(ReferentialTag referentialTag1, ReferentialTag referentialTag2);
    }
}
