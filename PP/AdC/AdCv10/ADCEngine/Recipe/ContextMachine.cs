using AdcTools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour définir les contextes-machine de la recette.
    /// ex: LUT-PSL, PAD-Fingers...
    /// 
    /// Les contextes sont stockées dans la base.
    /// de données et ajoutées aux inputs de la recette par le merge contexte.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class ContextMachine
    {
        public string Type;
        public Serializable Configuration;
    }
}
