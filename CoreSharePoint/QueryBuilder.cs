using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RBVH.Core.SharePoint
{
    public static class QueryBuilder
    {
        private const string QueryFormat = "<Query><Where>{0}</Where></Query >";
        /// <summary>
        /// TODO: break in for every 500 items as SP limit
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="fieldType"></param>
        /// <param name="values"></param>
        /// <param name="lookupId"></param>
        /// <returns></returns>
        public static string In(string fieldName, string fieldType, IList<object> values, bool lookupId)
        {
            if (values != null && values.Count() > 0)
            {
                var inQuery = "<In>{0}</In>";
                var fieldQuery = QueryBuilder.Field(fieldName);
                var valuesQuery = "<Values>{0}</Values>";
                StringBuilder queryBuilder = new StringBuilder();
                foreach (var value in values)
                {
                    queryBuilder.Append(QueryBuilder.Value(fieldType, value, lookupId));
                }
                valuesQuery = string.Format(valuesQuery, queryBuilder.ToString());
                fieldQuery += valuesQuery;
                return string.Format(inQuery, fieldQuery);
            }
            return string.Empty;
        }

        public static string Field(string fieldName)
        {
            return string.Format("<FieldRef Name = \"{0}\" />", fieldName);
        }

        public static string Value(string fieldType, object value, bool lookupId)
        {
            return string.Format("<Value Type='{0}' {1}>{2}</Value>", fieldType, lookupId ? "LookupId='TRUE'" : string.Empty, value);
        }

        public static string In(string fieldName, string fieldType, IList<int> values)
        {
            return In(fieldName, fieldType, values.Cast<object>().ToList(), true);
        }
    }
}
