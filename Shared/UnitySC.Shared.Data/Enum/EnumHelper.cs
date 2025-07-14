using System;

namespace UnitySC.Shared.Data.Enum
{
    public static class PMEnumHelper
    {
        // ------
        // [Specific result (8 bits)][Result format (8 bits)][Result Category (4 bits)][Side(2 bits)][Actor Type (10 bits)]
        // -------

        // [Actor Type (10 bits)] cf ActorType
        internal const int ActorTypeSize = ActorCategorySize + SpecificActorSize;

        internal const int ActorTypeShift = 0;

        // Sub section of actor type [Actor Category (4 bit)]
        internal const int ActorCategorySize = 4;

        internal const int ActorCategoryShift = 0;

        //Sub section of actor Specific Actor Type (6 bits)]
        internal const int SpecificActorSize = 6;

        internal const int SpecificActorShift = ActorCategoryShift + ActorCategorySize;

        // [Side(2 bits)] cf Side
        internal const int SideSize = 2;

        internal const int SideShift = ActorTypeShift + ActorTypeSize;

        // [Result Category (4 bits)] cf ResultCategory
        internal const int ResultCategorySize = 4;

        internal const int ResultCategoryShift = SideShift + SideSize;

        // [Result format(8 bits)] cf ResultFormat
        internal const int ResultFormatSize = 8;

        internal const int ResultFormatShift = ResultCategoryShift + ResultCategorySize;

        // [Specific Module result (8 bits)] cf ResultType
        internal const int ResultSpecificSize = 8;

        internal const int ResultSpecificShift = ResultFormatShift + ResultFormatSize;

        public static Side GetSide(this ResultType resultType)
        {
            return (Side)ApplyMask(resultType, SideShift, SideSize);
        }

        public static ResultCategory GetResultCategory(this ResultType resultType)
        {
            return (ResultCategory)ApplyMask(resultType, ResultCategoryShift, ResultCategorySize);
        }

        public static ActorType GetActorType(this ResultType resultType)
        {
            return (ActorType)ApplyMask(resultType, ActorTypeShift, ActorTypeSize);
        }

        public static ResultFormat GetResultFormat(this ResultType resultType)
        {
            return (ResultFormat)ApplyMask(resultType, ResultFormatShift, ResultFormatSize);
        }

        public static int GetSpecificModuleId(this ResultType resultType)
        {
            return ApplyMask(resultType, ResultSpecificShift, ResultSpecificSize) >> ResultSpecificShift;
        }

        public static int GetResultExtensionId(this ResultType resultType)
        {
            int res = ApplyMask(resultType, ResultFormatShift, ResultFormatSize + ResultSpecificSize) >> ResultFormatShift;
            return res;
        }

        public static ResultFormat GetResultFormatFromResultExtensionId(int resExtId)
        {
            int resExtIdShift = resExtId << ResultFormatShift;
            int maskWithoutShift = (1 << ResultFormatSize) - 1;
            int mask = maskWithoutShift << ResultFormatShift;
            int res = resExtIdShift & mask;
            return (ResultFormat)res;
        }

        public static ActorCategory GetCatgory(this ActorType actorType)
        {
            return (ActorCategory)ApplyMask(actorType, ActorCategoryShift, ActorCategorySize);
        }

        internal static int ApplyMask(ResultType resultType, int shift, int size)
        {
            int maskWithoutShift = (1 << size) - 1;
            int mask = maskWithoutShift << shift;
            int res = (int)resultType & mask;
            return res;
        }

        internal static int ApplyMask(ActorType actorType, int shift, int size)
        {
            int maskWithoutShift = (1 << size) - 1;
            int mask = maskWithoutShift << shift;
            int res = (int)actorType & mask;
            return res;
        }

        public static string ToHumanizedString(this UnitType unit)
        {
            switch (unit)
            {
                case UnitType.NoUnit:
                    return string.Empty;
                case UnitType.Nb:
                    return string.Empty;
                case UnitType.um:
                    return "µm";
                case UnitType.um2:
                    return "µm²";
                case UnitType.mm:
                    return "mm";
                case UnitType.mm2:
                    return "mm²";
                case UnitType.px:
                    return "px";
                case UnitType.px2:
                    return "px²";
                case UnitType.nm:
                    return "nm";
                case UnitType.nm2:
                    return "nm²";
                case UnitType.m:
                    return "m";
                case UnitType.m2:
                    return "m²";
                default:
                    throw new ArgumentOutOfRangeException(nameof(unit), unit, null);
            }
        }
    }
}
