using Agileo.ModelingFramework;

using Humanizer;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Extensions
{
    public static class NamedElementExtensions
    {
        public static string GetHumanizedName(this INamedElement namedElement)
        {
            return namedElement.Name.Humanize(LetterCasing.Sentence);
        }
    }
}
