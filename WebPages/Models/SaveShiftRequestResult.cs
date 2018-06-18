using System.Collections.Generic;
namespace RBVH.Stada.Intranet.WebPages.Models
{
    public class SaveShiftRequestResult
    {
        public CodeMessageResult CodeMessageResult { get; set; }
        public List<string> SuccessItemList { get; set; }
        public List<string> FailItemList { get; set; }
        public List<int> DeleteFailItemList { get; set; }
        public SaveShiftRequestResult()
        {
            SuccessItemList = new List<string>();
            FailItemList = new List<string>();
            DeleteFailItemList = new List<int>();
            CodeMessageResult = new CodeMessageResult();
        }
    }
}
