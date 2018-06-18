using System;
using System.Collections.Generic;
using System.Reflection;

namespace RBVH.Stada.Intranet.Biz.Helpers.EvalExpression
{
    /// <summary>
    /// Utility class that caches with custom attributes of various types (attribute retrieval is a rather costly operation)
    /// </summary>
    public static class CustomAttributeHelper
    {
        /// <summary>
        /// Check if type has given attribute
        /// </summary>
        /// <returns>True if type <paramref name="t"/> has attribute <typeparamref name="T"/></returns>
        public static bool Has<T>(Type t) where T : Attribute
        {
            return All<T>(t).Length != 0;
        }

        /// <summary>
        /// Check if property has given attribute
        /// </summary>
        /// <returns>True if property <paramref name="t"/> has attribute <typeparamref name="T"/></returns>
        public static bool Has<T>(PropertyInfo t) where T : Attribute
        {
            return All<T>(t).Length != 0;
        }

        /// <summary>
        /// Returns first attribute of specified type a given property
        /// </summary>
        /// <returns>True first attribute of type <typeparamref name="T"/> of property <paramref name="t"/>, or null if not found</returns>
        public static T First<T>(PropertyInfo t) where T : Attribute
        {
            foreach (T a in All<T>(t))
                if (a != null)
                    return a;
            return null;
        }

        /// <summary>
        /// Returns first attribute of specified type a given type
        /// </summary>
        /// <returns>True first attribute of type <typeparamref name="T"/> of type <paramref name="t"/>, or null if not found</returns>
        public static T First<T>(Type t) where T : Attribute
        {
            foreach (T a in All<T>(t))
                if (a != null)
                    return a;
            return null;
        }

        /// <summary>
        /// Returns all attributes of specified type a given property
        /// </summary>
        /// <returns>All attributes of type <typeparamref name="T"/> of property <paramref name="p"/>, or empty array if not found</returns>
        public static T[] All<T>(PropertyInfo p) where T : Attribute
        {
            var m = new Mid(typeof (T), p);
            return all<T>(m, p);
        }

        /// <summary>
        /// Returns all attributes of specified type a given type
        /// </summary>
        /// <returns>All attributes of type <typeparamref name="T"/> of type <paramref name="t"/>, or empty array if not found</returns>
        public static T[] All<T>(Type t) where T : Attribute
        {
            return all<T>(new Mid(typeof(T), t), t);
        }

        class Mid
        {
            private readonly Type _type;
            private readonly int _propToken;
            private readonly Type _attributeType;
            private readonly string _propName;

            public Mid(Type attrType, Type t)
            {
                _attributeType = attrType;
                _type = t;
                _propToken = 0;
                _propName = null;
            }
            public Mid(Type attrType, PropertyInfo p)
            {
                _attributeType = attrType;
                _type = p.ReflectedType;
                _propToken = p.MetadataToken;
                _propName = p.Name;
            }

            // override object.Equals
            public override bool Equals(object obj)
            {
                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }
                Mid m = (Mid)obj;
                return (_attributeType == m._attributeType &&
                        _type == m._type &&
                        _propToken == m._propToken &&
                        _propName == m._propName);

            }

            public override int GetHashCode()
            {
                return _attributeType.GetHashCode() ^ _type.GetHashCode() ^ _propToken;
            }
            public override string ToString()
            {
                return _attributeType + "; Type=" + _type + "; token=" + _propToken + "; propName=" + _propName;
            }
        }

        private static readonly Dictionary<Type, bool> s_inherit = new Dictionary<Type, bool>(500);
        private static readonly Dictionary<Mid, object> s_map = new Dictionary<Mid, object>(500);

        private static T[] all<T>(Mid mid, ICustomAttributeProvider provider) where T : Attribute
        {
            lock (s_map)
            {
                object o;
                if (s_map.TryGetValue(mid, out o))
                    return (T[])o;
            }

            Type at = typeof(T);
            bool inherit;
            lock (s_inherit)
            {
                if (!s_inherit.TryGetValue(at, out inherit))
                {
                    inherit = false;
                    foreach (AttributeUsageAttribute a in at.GetCustomAttributes(typeof(AttributeUsageAttribute), false))
                    {
                        inherit = a.Inherited;
                        break;
                    }
                    s_inherit[at] = inherit;
                }
            }
            
            T[] att = (T[])provider.GetCustomAttributes(typeof(T), inherit);
            lock (s_map)
            {
                s_map[mid] = att;
            }
            return att;
        }


        
    }
}