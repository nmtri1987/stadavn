using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RBVH.Stada.Intranet.Biz.Models;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class GroupDAL : BaseDAL<Group>
    {
        public GroupDAL(string siteUrl) : base(siteUrl)
        {
        }

        public Group GetByName(string groupName)
        {
            Group group = null;

            string queryStr = $@"<Where><Eq><FieldRef Name='CommonName' /><Value Type='Text'>{groupName}</Value></Eq></Where>";
            var groupItems = this.GetByQuery(queryStr);
            if (groupItems != null && groupItems.Count > 0)
            {
                group = groupItems[0];
            }

            return group;
        }

        public int Insert(Group newItem)
        {
            int returnId = 0;
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList groupList = web.GetList($"{web.Url}{ListUrl}");

                    SPListItem groupListItem = groupList.AddItem();
                    groupListItem[StringConstant.GroupList.NameField] = newItem.Name;
                    groupListItem.Update();
                    returnId = groupListItem.ID;
                    web.AllowUnsafeUpdates = false;
                }
            }
            return returnId;
        }
    }
}
