using System.Collections.Generic;
namespace RBVH.Stada.Intranet.Webservices.Model
{
    public class ShiftTimeListModel
    {
        public CodeMessageResult CodeMessageResult { get; set; }
        public List<ShiftTimeModel> ShiftTimeList { get; set; }
        public ShiftTimeListModel()
        {
            CodeMessageResult = new CodeMessageResult();
            ShiftTimeList = new List<ShiftTimeModel>();
        }

    }
}
