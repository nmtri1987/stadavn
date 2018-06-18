using Microsoft.SharePoint;

namespace RBVH.Core.SharePoint.Extension
{
    /// <summary>
    /// SPFieldLookupValueCollectionExtensions class.
    /// </summary>
    public static class SPFieldLookupValueCollectionExtensions
    {
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="fieldLookupValueCollection"></param>
        /// <param name="lookupId"></param>
        /// <returns></returns>
        public static bool Remove(this SPFieldLookupValueCollection fieldLookupValueCollection, int lookupId)
        {
            bool res = false;

            for (int i = fieldLookupValueCollection.Count - 1; i >= 0; i--)
            {
                if (fieldLookupValueCollection[i].LookupId == lookupId)
                {
                    SPFieldLookupValue fieldLookupValue = fieldLookupValueCollection[i];
                    fieldLookupValueCollection.Remove(fieldLookupValue);
                    res = true;
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// IsEmpty
        /// </summary>
        /// <param name="fieldLookupValueCollection"></param>
        /// <returns></returns>
        public static bool IsEmpty(this SPFieldLookupValueCollection fieldLookupValueCollection)
        {
            return fieldLookupValueCollection.Count == 0 ? true : false;
        }
    }
}
