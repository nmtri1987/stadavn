using RBVH.Stada.Intranet.Biz.Models;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class MeetingRoomDAL : BaseDAL<MeetingRoom>
    {
        public MeetingRoomDAL(string siteUrl) : base(siteUrl)
        {
        }
    }
}
