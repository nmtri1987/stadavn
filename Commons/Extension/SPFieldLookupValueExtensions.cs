using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    /// <summary>
    /// SPFieldLookupValueExtensions class.
    /// </summary>
    public static class SPFieldLookupValueExtensions
    {
        public static LookupItem ToLookupItem(this SPFieldLookupValue fieldLookupValue)
        {
            LookupItem lookupItem = null;

            if (fieldLookupValue != null)
            {
                lookupItem = new LookupItem { LookupId = fieldLookupValue.LookupId, LookupValue = fieldLookupValue.LookupValue };
            }

            return lookupItem;
        }
    }
}
