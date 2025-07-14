namespace BasicModules.DataLoader
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Input de la recette pour une couche de données "Mosaïque".
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class MosaicInputInfo : InspectionInputInfoBase
    {
        ///<summary> nom de base des fichiers mosaïque </summary>
        public string Basename;

        public int NbColumns;
        public int NbLines;
        public int MosaicImageWidth;    // en pixels, taille d'un élément de la mosaïque
        public int MosaicImageHeight;
    }
}
