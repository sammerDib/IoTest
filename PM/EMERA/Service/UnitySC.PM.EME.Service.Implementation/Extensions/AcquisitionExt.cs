using System;
using System.Collections.Generic;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Service.Implementation.Extensions
{
    public static class AcquisitionExt
    {

        /// <summary>
        /// Determines the type of result for a given acquisition as a function of its filter and the associated lighting type.
        /// </summary>
        /// <param name=‘acquisition’>The acquisition for which we wish to obtain the type of result.</param>
        /// <param name=‘hardwareManager’>EmeHardwareManager instance used to access equipment configurations.</param>
        /// <returns>The type of result corresponding to the acquisition.</returns>        
        public static ResultType GetOutputType(this Acquisition acquisition, EmeHardwareManager hardwareManager)
        {
            if (acquisition == null)
                throw new ArgumentNullException(nameof(acquisition));

            if (hardwareManager == null)
                throw new ArgumentNullException(nameof(hardwareManager));

            if (!hardwareManager.EMELights.TryGetValue(acquisition.LightDeviceId, out var light))
                throw new KeyNotFoundException($"LightDeviceId {acquisition.LightDeviceId} not found in EMELights.");

            return EmeResultTypeConverter.GetResultTypeFromFilterAndLight(acquisition.Filter, light.Config.Type);
        }
    }
}
