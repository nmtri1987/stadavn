using System.Linq;

namespace RBVH.Stada.Intranet.Biz.Extension
{
    public static class ArrayStringExtensions
    {
        /// <summary>
        /// Check value is existed in values and ignore case.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string[] values, string value)
        {
            var res = false;

            var existedValue = values.Where(sex => (string.Compare(sex, value, true) == 0)).FirstOrDefault();
            if (!string.IsNullOrEmpty(existedValue))
            {
                res = true;
            }

            return res;
        }

        /// <summary>
        /// Get first value in values which is not existed in arrayValues.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="arrayValues"></param>
        /// <returns></returns>
        public static string GetValueNotInArrayValues(this string[] values, string[] arrayValues)
        {
            var res = string.Empty;

            if (values.Length > 0)
            {
                if (arrayValues != null && arrayValues.Length > 0)
                {
                    foreach (var value in values)
                    {
                        if (!arrayValues.ContainsIgnoreCase(value))
                        {
                            res = value;
                            break;
                        }
                    }
                }
            }

            return res;
        }
    }
}
