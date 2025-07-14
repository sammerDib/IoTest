using System.Collections.Generic;
using System.Drawing;

namespace FormatCRW
{
    public struct DataMeasure
    {
        public string sLabel; // label de la mesure
        public string sValue; // valeur de la mesure (penser à y inclure son unité)
    }

    public struct ProfilScan
    {
        public string sLabel;          // label du profil scan
        public Color cColor;           // couleur du profil dans le viewer
        public List<double> DeltaList;  // liste des points "deltas" (différence par rapport bord du crown) en µm  
    }
}
