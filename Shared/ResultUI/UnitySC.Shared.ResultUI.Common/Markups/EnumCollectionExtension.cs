using System;
using System.Windows.Markup;

namespace UnitySC.Shared.ResultUI.Common.Markups
{
    [MarkupExtensionReturnType(typeof(object[]))]
    public class EnumCollectionExtension : MarkupExtension
    {
        public EnumCollectionExtension()
        {
        }

        public EnumCollectionExtension(Type enumType)
        {
            EnumType = enumType;
        }

        [ConstructorArgument("enumType")]
        public Type EnumType { get; set; }

        #region Overrides of MarkupExtension

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null)
            {
                throw new ArgumentException("The enumeration's type is not set.");
            }

            return Enum.GetValues(EnumType);
        }

        #endregion
    }
}
