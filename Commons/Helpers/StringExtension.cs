using RBVH.Stada.Intranet.Biz.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBVH.Stada.Intranet.Biz.Helpers
{
    public static class StringExtension
    {
        public static DateTime ToMMDDYYYYDate(this string inputString, bool isEndDate)
        {
            var fromDateValues = inputString.Split('-'); // mm-dd-yyyy
            if (fromDateValues.Length != 3)
                return DateTime.MinValue;
            if(isEndDate)
                return new DateTime(Convert.ToInt32(fromDateValues[2]), Convert.ToInt32(fromDateValues[0]), Convert.ToInt32(fromDateValues[1]), 23, 59, 59);
            return new DateTime(Convert.ToInt32(fromDateValues[2]), Convert.ToInt32(fromDateValues[0]), Convert.ToInt32(fromDateValues[1]), 0, 0, 0);
        }

        public static string BuildComment(this string currentString, string newString)
        {
            if (!string.IsNullOrEmpty(newString))
            {
                StringBuilder commentBuilder = new StringBuilder();
                if (!string.IsNullOrEmpty(currentString))
                {
                    commentBuilder.Append($"{currentString}###");
                }
                commentBuilder.Append(newString);

                return commentBuilder.ToString();
            }
            else
            {
                return currentString;
            }
        }

        public static List<string> SplitStringOfLocations(this string currentString)
        {
            return currentString.Split(new string[] { StringConstant.SeparatorFactoryLocation }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}
