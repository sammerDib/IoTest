using System.ComponentModel;

namespace AdcBasicObjects
{

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Image d'une tesselle d'une mosaïqe au format ADC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class MosaicImage : ImageBase
    {
        //=================================================================
        // 
        //=================================================================
        public MosaicImage(MosaicLayer layer)
            : base(layer)
        {
        }

        //=================================================================
        // Proprietées Browsables
        //=================================================================
        [Category("Mosaic"), Browsable(true)]
        public int Line { get; set; }
        [Category("Mosaic"), Browsable(true)]
        public int Column { get; set; }
    }
}
