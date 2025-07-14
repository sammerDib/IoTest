namespace BasicModules.DataLoader
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Input de la recette pour une couche de données "Die".
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DieInputInfo : InspectionInputInfoBase
    {
        ///<summary> nom de base des fichiers die </summary>
        public string Basename = "CutUp_DieTo";

        ///<summary> chemin du fichier dies.xml </summary>
        public string diesxml;

        // Index min/max des dies
        public int MinIndexX;
        public int MinIndexY;
        public int MaxIndexX;
        public int MaxIndexY;

        public double DiePitchX_µm;
        public double DiePicthY_µm;

        public double DieOriginX_µm;  // décalage en X du centre du DieOrigin (0,0) par au centre du Wafer
        public double DieOriginY_µm;  // décalage en X du centre du DieOrigin (0,0) par au centre du Wafer

        public double DieSizeX_µm;
        public double DieSizeY_µm;

    }
}
