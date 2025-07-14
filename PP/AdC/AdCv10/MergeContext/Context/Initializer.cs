namespace MergeContext.Context
{
    public class Initializer
    {
        // Il faut appeler une fonction de la DLL pour qu'elle
        // soit chargée. Et il faut qu'elle soit chargée pour 
        // pouvoir désérialiser des object de cette DLL à partir
        // du type de base (i.e. AdcTools.Serializable).
        public static void Init() { }
    }
}
