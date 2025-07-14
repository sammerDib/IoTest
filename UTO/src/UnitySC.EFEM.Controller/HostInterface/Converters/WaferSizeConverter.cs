using System;

using Agileo.SemiDefinitions;

namespace UnitySC.EFEM.Controller.HostInterface.Converters
{
    static class WaferSizeConverter
    {
        /// <summary>
        /// Convert a <see cref="SampleDimension"/> into a uint command parameter as defined in EFEM Controller Comm Specs 211006.pdf.
        /// </summary>
        public static uint ToUint(SampleDimension sampleDimension)
        {
            switch (sampleDimension)
            {
                case SampleDimension.S100mm:
                    return 4;

                case SampleDimension.S150mm:
                    return 6;

                case SampleDimension.S200mm:
                    return 2;

                case SampleDimension.S300mm:
                    return 1;

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(sampleDimension),
                        sampleDimension,
                        $@"Could not convert {sampleDimension} into uint according to ""EFEM Controller Comm Specs 211006.pdf");
            }
        }

        public static int ToInch(SampleDimension sampleDimension)
        {
            switch (sampleDimension)
            {
                case SampleDimension.NoDimension:
                    return 0;

                case SampleDimension.S100mm:
                    return 4;

                case SampleDimension.S150mm:
                    return 6;

                case SampleDimension.S200mm:
                    return 8;

                case SampleDimension.S300mm:
                    return 12;

                case SampleDimension.S450mm:
                    return 18;

                default:
                    return 0;
            }
        }
    }
}
