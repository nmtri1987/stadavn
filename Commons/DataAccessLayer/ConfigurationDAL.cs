using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    /// <summary>
    /// ConfigurationDAL
    /// </summary>
    public class ConfigurationDAL : BaseDAL<Configuration>
    {
        /// <summary>
        /// ConfigurationDAL object.
        /// </summary>
        private static ConfigurationDAL instance;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="siteUrl"></param>
        private ConfigurationDAL(string siteUrl) : base(siteUrl)
        {
        }

        /// <summary>
        /// Get value of key from configuration list.
        /// </summary>
        /// <param name="siteUrl">The site url.</param>
        /// <param name="key">The key which needs to get value.</param>
        /// <returns></returns>
        public static string GetValue(string siteUrl, string key)
        {
            if (instance == null)
            {
                instance = new ConfigurationDAL(siteUrl);
            }

            string value = string.Empty;
            var configurations = GetValues(siteUrl, new List<string>() { key });
            if (configurations != null && configurations.Count > 0)
            {
                var configuration = configurations[0];
                value = configuration.Value;
            }

            return value;
        }

        public static List<Configuration> GetValues(string siteUrl, List<string> keys)
        {
            if (instance == null)
            {
                instance = new ConfigurationDAL(siteUrl);
            }

            string queryStr = BuildQueryByKeys(keys);

            return instance.GetByQuery(queryStr);
        }

        private static string BuildQueryByKeys(List<string> keys)
        {
            string queryStr = "";

            foreach (string key in keys)
            {
                queryStr += string.Format("<Value Type='Text'>{0}</Value>", key);
            }

            queryStr = $@"<Where><In><FieldRef Name='{ConfigurationList.Fields.Key}' /><Values>{queryStr}</Values></In></Where>";

            return queryStr;
        }
    }
}
