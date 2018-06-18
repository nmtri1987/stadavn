using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Helpers;

namespace RBVH.Stada.Intranet.Biz.Models
{
    [ListUrl(StringConstant.MeetingRoomsList.Url)]
    public class MeetingRoom : EntityBase
    {
        public MeetingRoom()
        {
        }

        [ListColumn(StringConstant.MeetingRoomsList.Fields.Title)]
        public string Title { get; set; }
    }
}
