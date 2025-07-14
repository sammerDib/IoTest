using System.ComponentModel;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Image d'un die au format ADC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class DieImage : ImageBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public DieImage(DieLayer layer)
            : base(layer)
        {
        }

        //=================================================================
        // Proprietées Browsables
        //=================================================================
        [Category("Die"), Browsable(true)]
        public int DieIndexX { get; set; }
        [Category("Die"), Browsable(true)]
        public int DieIndexY { get; set; }
    }
}
