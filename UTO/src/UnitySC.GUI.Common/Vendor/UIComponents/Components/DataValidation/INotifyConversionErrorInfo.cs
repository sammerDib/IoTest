namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    /// <summary>
    /// Interface allowing the management of conversion errors made internally in the custom controls.
    /// </summary>
    public interface INotifyConversionErrorInfo
    {
        /// <summary>
        /// Adds a conversion error associated with the property passed as a parameter.
        /// This method must be called when there is a conversion error made by custom controls.
        /// </summary>
        public void AddConversionError(string propertyName, string error);

        /// <summary>
        /// Removes conversion errors associated with the property passed as a parameter.
        /// This method must be called when canceling conversion errors made by custom controls.
        /// </summary>
        public void ClearConversionError(string propertyName);
    }
}
