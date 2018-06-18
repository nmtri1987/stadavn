using System.Collections.Generic;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class ViewShiftManagement
    {
        public ViewShiftManagement()
        {
            CodeMessageResult = new CodeMessageResult();
            ShiftManagementData = new Dictionary<string, string>();
        }
        public CodeMessageResult CodeMessageResult { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Dictionary<string, string> ShiftManagementData { get; set; }
    }
}