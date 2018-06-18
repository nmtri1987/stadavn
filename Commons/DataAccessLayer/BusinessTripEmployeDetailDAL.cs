using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class BusinessTripEmployeeDetailDAL : BaseDAL<BusinessTripEmployeeDetail>
    {
        public BusinessTripEmployeeDetailDAL(string siteUrl) : base(siteUrl) { }

        public List<BusinessTripEmployeeDetail> GetItemsByParentId(int parentId)
        {
            return this.GetByQuery(BuildQueryStringGetBusinessTripEmployeeDetail(parentId));
        }

        public int SaveOrUpdate(BusinessTripEmployeeDetail item)
        {
            int itemId = 0;
            using (SPSite spSite = new SPSite(SiteUrl))
            {
                using (SPWeb spWeb = spSite.OpenWeb())
                {
                    itemId = SaveOrUpdate(spWeb, item);
                }
            }

            return itemId;
        }

        public int SaveOrUpdate(SPWeb spWeb, BusinessTripEmployeeDetail item)
        {
            int itemId = 0;

            SPList spList = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItem spListItem;
            if (item.ID > 0)
                spListItem = spList.GetItemById(item.ID);
            else
                spListItem = spList.AddItem();

            spListItem[StringConstant.BusinessTripEmployeeDetailsList.Fields.BusinessTripManagementID] = item.BusinessTripManagementID.LookupId;
            spListItem[StringConstant.BusinessTripEmployeeDetailsList.Fields.Employee] = item.Employee.LookupId;

            spWeb.AllowUnsafeUpdates = true;
            spListItem.Update();
            itemId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return itemId;
        }

        public void SaveOrUpdate(SPWeb spWeb, List<BusinessTripEmployeeDetail> items)
        {
            SPList spList = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            spWeb.AllowUnsafeUpdates = true;
            foreach (var item in items)
            {
                SPListItem spListItem;
                if (item.ID > 0)
                    spListItem = spList.GetItemById(item.ID);
                else
                    spListItem = spList.AddItem();

                spListItem[StringConstant.BusinessTripEmployeeDetailsList.Fields.BusinessTripManagementID] = item.BusinessTripManagementID.LookupId;
                spListItem[StringConstant.BusinessTripEmployeeDetailsList.Fields.Employee] = item.Employee.LookupId;

                spListItem.Update();
            }
            spWeb.AllowUnsafeUpdates = false;
        }

        #region Private Methods
        private string BuildQueryStringGetBusinessTripEmployeeDetail(int parentId)
        {
            string queryStr = string.Format(@"<Where>
                                              <Eq>
                                                 <FieldRef Name='" + StringConstant.BusinessTripEmployeeDetailsList.Fields.BusinessTripManagementID + @"' LookupId='TRUE' />
                                                 <Value Type='Lookup'>{0}</Value>
                                              </Eq>
                                           </Where>", parentId);

            return queryStr;
        }
        #endregion
    }
}
