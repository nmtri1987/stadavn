
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.WebPages.Models
{
    public  class SaveOvertimeResult
    {
        public CodeMessageResult CodeMessageResult { get; set; }
        public List<string> SuceessList { get; set; }
        public List<string> FailureList { get; set; }
        public SaveOvertimeResult()
        {
            CodeMessageResult = new CodeMessageResult();
            SuceessList = new List<string>();
            FailureList = new List<string>();
        }
    }
}
