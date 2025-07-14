using System;

using UnitySC.PM.EME.Service.Interface.Light;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    public class EmeResultTypeConverter
    {
        public static ResultType GetResultTypeFromFilterAndLight(EMEFilter filterType, EMELightType lightType)
        {
            switch (lightType)
            {
                case EMELightType.DirectionalDarkField0Degree:
                    return ResultType.EME_Visible0;
                case EMELightType.DirectionalDarkField90Degree:
                    return ResultType.EME_Visible90;
                case EMELightType.UltraViolet270nm:
                    switch (filterType)
                    {
                        case EMEFilter.NoFilter:
                            return ResultType.EME_UV_NoFilter;
                        case EMEFilter.BandPass450nm50:
                            return ResultType.EME_UV_BandPass450nm50;
                        case EMEFilter.BandPass470nm50:
                            return ResultType.EME_UV_BandPass470nm50;
                        case EMEFilter.BandPass550nm50:
                            return ResultType.EME_UV_BandPass550nm50;
                        case EMEFilter.LowPass650nm:
                            return ResultType.EME_UV_LowPass650nm;
                        case EMEFilter.LowPass750nm:
                            return ResultType.EME_UV_LowPass750nm;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
                    }
                case EMELightType.UltraViolet310nm:
                    switch (filterType)
                    {
                        case EMEFilter.NoFilter:
                            return ResultType.EME_UV2_NoFilter;
                        case EMEFilter.BandPass450nm50:
                            return ResultType.EME_UV2_BandPass450nm50;
                        case EMEFilter.BandPass470nm50:
                            return ResultType.EME_UV2_BandPass470nm50;
                        case EMEFilter.BandPass550nm50:
                            return ResultType.EME_UV2_BandPass550nm50;
                        case EMEFilter.LowPass650nm:
                            return ResultType.EME_UV2_LowPass650nm;
                        case EMEFilter.LowPass750nm:
                            return ResultType.EME_UV2_LowPass750nm;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
                    }
                case EMELightType.UltraViolet365nm:
                    switch (filterType)
                    {
                        case EMEFilter.NoFilter:
                            return ResultType.EME_UV3_NoFilter;
                        case EMEFilter.BandPass450nm50:
                            return ResultType.EME_UV3_BandPass450nm50;
                        case EMEFilter.BandPass470nm50:
                            return ResultType.EME_UV3_BandPass470nm50;
                        case EMEFilter.BandPass550nm50:
                            return ResultType.EME_UV3_BandPass550nm50;
                        case EMEFilter.LowPass650nm:
                            return ResultType.EME_UV3_LowPass650nm;
                        case EMEFilter.LowPass750nm:
                            return ResultType.EME_UV3_LowPass750nm;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(filterType), filterType, null);
                    }
                default:
                    throw new Exception($"Can't find Result type from filter {filterType} and light {lightType}");
            }
        }
    }
}
