using Microsoft.SharePoint.Utilities;

namespace RBVH.Core.SharePoint
{
    public static class ResourceHelper
    {
        // Wraps the SPUtility method of the same name.
        public static string GetLocalizedString(string resourceKey, string resourceFile, int lcid)
        {
            if (string.IsNullOrEmpty(resourceKey))
                return string.Empty;

            // SPUtility.GetLocalized string needs a resource expression as the first argument.
            string resourceExpression = $"$Resources:{resourceKey}";

            // Note: If the named resource does not have a value for the specified language, 
            // SPUtility.GetLocalizedString returns the value for the invariant language.
            // If the named resource does not exist, it returns the original expression.
            return SPUtility.GetLocalizedString(resourceExpression, resourceFile, (uint)lcid);
        }
    }
}