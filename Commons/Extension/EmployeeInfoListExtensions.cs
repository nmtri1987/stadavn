using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    /// <summary>
    /// EmployeeInfoListExtensions
    /// </summary>
    public static class EmployeeInfoListExtensions
    {
        /// <summary>
        /// ToSPFieldLookupValueCollection
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static SPFieldLookupValueCollection ToSPFieldLookupValueCollection(this List<EmployeeInfo> list)
        {
            SPFieldLookupValueCollection fieldLookupValueCollection = null;

            if (list != null && list.Count > 0)
            {
                fieldLookupValueCollection = new SPFieldLookupValueCollection();
                foreach(var item in list)
                {
                    SPFieldLookupValue fieldLookupValue = new SPFieldLookupValue { LookupId = item.ID };
                    fieldLookupValueCollection.Add(fieldLookupValue);
                }
            }

            return fieldLookupValueCollection;
        }
    }
}
