using Microsoft.SharePoint;
using System;

namespace RBVH.Core.SharePoint
{
    /// <summary>
    /// ObjectHelper class.
    /// </summary>
    public static class ObjectHelper
    {
        #region Extensions
        /// <summary>
        /// Convert object to date object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static double ToNumber(this object objSource)
        {
            return double.Parse(objSource.ToString());
        }

        /// <summary>
        /// Convert object to date object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object objSource)
        {
            return DateTime.Parse(objSource.ToString()).Date;
        }

        /// <summary>
        /// Convert object to date time object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object objSource)
        {
            return DateTime.Parse(objSource.ToString());
        }

        /// <summary>
        /// Convert object to SPFieldLookupValue object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static SPFieldLookupValue ToSPFieldLookupValue(this object objSource)
        {
            return new SPFieldLookupValue(objSource.ToString());
        }

        /// <summary>
        /// Convert object to SPFieldLookupValueCollection object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static SPFieldLookupValueCollection ToSPFieldLookupValueCollection(this object objSource)
        {
            return new SPFieldLookupValueCollection(objSource.ToString());
        }

        /// <summary>
        /// Convert object to boolean object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static bool ToBoolean(this object objSource)
        {
            return bool.Parse(objSource.ToString());
        }

        /// <summary>
        /// Convert object to SPFieldUserValue object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static SPFieldUserValue ToSPFieldUserValue(this object objSource, SPWeb web)
        {
            return new SPFieldUserValue(web, objSource.ToString());
        }

        /// <summary>
        /// Convert object to SPFieldUserValueCollection object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public static SPFieldUserValueCollection ToSPFieldUserValueCollection(this object objSource, SPWeb web)
        {
            return new SPFieldUserValueCollection(web, objSource.ToString());
        }
        #endregion

        #region Utilities
        /// <summary>
        /// If objSource is null, return empty string. Otherwise return value of objSource.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static string GetString(object objSource)
        {
            return objSource != null ? objSource.ToString() : string.Empty;
        }

        /// <summary>
        /// If objSource is null, return defaultValue. Otherwise return value of objSource.
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="defaultValue">If objSource is null, return defaultValue parameter. Otherwise return value of objSource.</param>
        /// <returns></returns>
        public static double GetNumber(object objSource, double defaultValue = 0)
        {
            if (objSource != null)
            {
                return objSource.ToNumber();
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// If objSource is null, return DateTime.MinValue. Otherwise return value of objSource.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static DateTime GetDate(object objSource)
        {
            if (objSource != null)
            {
                return objSource.ToDate();
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// If objSource is null, return DateTime.MinValue. Otherwise return value of objSource.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static DateTime GetDateTime(object objSource)
        {
            if (objSource != null)
            {
                return objSource.ToDateTime();
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// If objSource is null, return null. Otherwise return SPFieldLookupValue object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static SPFieldLookupValue GetSPFieldLookupValue(object objSource)
        {
            return objSource != null ? objSource.ToSPFieldLookupValue() : null;
        }

        /// <summary>
        /// If objSource is null, return null. Otherwise return SPFieldLookupValueCollection object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static SPFieldLookupValueCollection GetSPFieldLookupValueCollection(object objSource)
        {
            return objSource != null ? objSource.ToSPFieldLookupValueCollection() : null;
        }

        /// <summary>
        /// If objSource is null, return false. Otherwise return boolean object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static bool GetBoolean(object objSource)
        {
            return objSource != null ? objSource.ToBoolean() : false;
        }

        /// <summary>
        /// If objSource is null, return null. Otherwise return GetSPFieldUserValue object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public static SPFieldUserValue GetSPFieldUserValue(object objSource, SPWeb web)
        {
            return objSource != null ? objSource.ToSPFieldUserValue(web) : null;
        }

        /// <summary>
        /// If objSource is null, return null. Otherwise return SPFieldUserValueCollection object.
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="web"></param>
        /// <returns></returns>
        public static SPFieldUserValueCollection GetSPFieldUserValueCollection(object objSource, SPWeb web)
        {
            return objSource != null ? objSource.ToSPFieldUserValueCollection(web) : null;
        }

        /// <summary>
        /// GetChoiceValues
        /// </summary>
        /// <param name="objSource"></param>
        /// <returns></returns>
        public static string[] GetChoiceValues(object objSource)
        {
            string[] choiceValues = null;

            if (objSource != null)
            {
                choiceValues = objSource.ToString().Split(new string[] { ";#" }, StringSplitOptions.RemoveEmptyEntries);
            }

            return choiceValues;
        }

        /// <summary>
        /// GetChoiceValues
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] GetChoiceValues(object objSource, string separator)
        {
            string[] choiceValues = null;

            if (objSource != null)
            {
                choiceValues = objSource.ToString().Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }

            return choiceValues;
        }

        /// <summary>
        /// GetChoiceValues
        /// </summary>
        /// <param name="objSource"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] GetChoiceValues(object objSource, char[] separator)
        {
            string[] choiceValues = null;

            if (objSource != null)
            {
                choiceValues = objSource.ToString().Split(separator, StringSplitOptions.RemoveEmptyEntries);
            }

            return choiceValues;
        }
        #endregion
    }
}
