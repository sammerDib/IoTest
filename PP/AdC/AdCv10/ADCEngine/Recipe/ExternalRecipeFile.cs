using System;

namespace ADCEngine
{
    /// <summary>
    ///  Fichier externe utile à l'execution de la recette.
    /// </summary>
    public class ExternalRecipeFile
    {
        public string FileName { get; set; }

        public ExternalRecipeFile(string fileName)
        {
            FileName = fileName;
        }

        public override string ToString()
        {
            return FileName;
        }

        public static implicit operator ExternalRecipeFile(string s)
        {
            ExternalRecipeFile ps = new ExternalRecipeFile(s);
            return ps;
        }

        public static implicit operator String(ExternalRecipeFile ps)
        {
            if (ps == null)
                return null;
            return ps.FileName;
        }
    }
}
