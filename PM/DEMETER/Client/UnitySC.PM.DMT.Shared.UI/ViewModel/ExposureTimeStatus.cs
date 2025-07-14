namespace UnitySC.PM.DMT.Shared.UI.ViewModel
{
    /// <summary>
    /// Le status du temps d'exposition
    /// </summary>
    public enum ExposureTimeStatus
    {
        /// <summary> L'utilisateur a modifié la valeur </summary>
        Modified,
        /// <summary> La caméra est en train d'acquérir une image avec le temps demandé </summary>
        InProgress,
        /// <summary> L'image affiché correspond au temps d'exposition souhaité </summary>
        Valid,
    }
}
