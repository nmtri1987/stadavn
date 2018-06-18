using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    /// <summary>
    /// LookupItemExtension
    /// </summary>
    public static class LookupItemExtension
    {
        /// <summary>
        /// ToSPFieldLookupValue
        /// </summary>
        /// <param name="lookupItem"></param>
        /// <returns></returns>
        public static SPFieldLookupValue ToSPFieldLookupValue(this LookupItem lookupItem)
        {
            SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue() { LookupId = lookupItem.LookupId };
            return fieldLookupValue;
        }
    }
}
