using Agileo.Common.Access;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class AccessRightsHelper
    {
        /// <summary>
        /// Compares the highest access level of the tree structure of the element passed as parameter with the current access level of the application.
        /// </summary>
        /// <param name="graphicalElement"><see cref="GraphicalElement"/> instance.</param>
        /// <returns>True if the current access level is greater than or equal to the access level of the element passed as a parameter, otherwise false.</returns>
        public static bool IsAccessible(GraphicalElement graphicalElement)
        {
            var application = App.Instance;
            if (application == null) return false;

            AccessLevel currentAccessLevel = application.AccessRights.CurrentUser?.AccessLevel ?? AccessLevel.Visibility;
            AccessLevel graphicalElementAccessLevel = GetAccessibilityLevel(graphicalElement);
            return graphicalElementAccessLevel <= currentAccessLevel;
        }

        /// <summary>
        /// Get the access level of the element taking into account the level of its parents.
        /// </summary>
        /// <param name="graphicalElement"><see cref="GraphicalElement"/> instance.</param>
        /// <returns>Highest access level in the tree structure of the element passed as parameter.</returns>
        public static AccessLevel GetAccessibilityLevel(GraphicalElement graphicalElement)
        {
            AccessLevel maxAccessLevel = AccessLevel.Visibility;
            var currentElement = graphicalElement;

            while (currentElement != null)
            {
                var accessLevel = currentElement.AccessRights?.IsEnabledRight?.AccessLevel ?? AccessLevel.Visibility;
                if (maxAccessLevel < accessLevel)
                {
                    maxAccessLevel = accessLevel;
                }

                currentElement = currentElement.Parent;
            }

            return maxAccessLevel;
        }
    }
}
