namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Attribut indiquant si la propriété est exportable pour la vue 
    /// simplifiée d'édition des paramètres recette)
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ExportableParameterAttribute : System.Attribute
    {
        public bool exportable;

        public ExportableParameterAttribute(bool exportable = true)
        {
            this.exportable = exportable;
        }
    }
}
