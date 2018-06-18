using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    /// <summary>
    /// ObjectExtensions.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Get all of properties of object.
        /// </summary>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetProperties(this object entityObject)
        {
            if (entityObject == null)
            {
                return new Dictionary<string, object>();
            }
            var typeInfo = entityObject.GetType();
            var listOfProperties = typeInfo.GetProperties();
            var result = new Dictionary<string, object>();
            foreach (var property in listOfProperties)
            {
                result.Add(property.Name, property.GetValue(entityObject, new object[] { }));
            }

            return result;
        }

        /// <summary>
        /// Get value of property of object.
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetProperties(this object entityObject, string propertyName)
        {
            var listOfProperties = entityObject.GetProperties();
            return listOfProperties[propertyName];
        }
    }
}
