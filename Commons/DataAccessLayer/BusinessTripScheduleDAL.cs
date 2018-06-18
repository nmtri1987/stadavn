using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class BusinessTripScheduleDAL : BaseDAL<BusinessTripSchedule>
    {
        public BusinessTripScheduleDAL(string siteUrl) : base(siteUrl) { }

        public List<BusinessTripSchedule> GetItemsByParentId(int parentId)
        {
            return this.GetByQuery(BuildQueryStringGetBusinessTripSchedule(parentId));
        }

        public int SaveOrUpdate(BusinessTripSchedule item)
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

        public int SaveOrUpdate(SPWeb spWeb, BusinessTripSchedule item)
        {
            int itemId = 0;

            SPList spList = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItem spListItem;
            if (item.ID > 0)
                spListItem = spList.GetItemById(item.ID);
            else
                spListItem = spList.AddItem();

            spListItem[StringConstant.BusinessTripScheduleList.Fields.BusinessTripManagementID] = item.BusinessTripManagementID.LookupId;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.DepartDate] = item.DepartDate.Value;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.FlightName] = item.FlightName;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.City] = item.City;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.Country] = item.Country;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.ContactCompany] = item.ContactCompany;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.ContactPhone] = item.ContactPhone;
            spListItem[StringConstant.BusinessTripScheduleList.Fields.OtherSchedule] = item.OtherSchedule;

            spWeb.AllowUnsafeUpdates = true;
            spListItem.Update();
            itemId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return itemId;
        }

        public void SaveOrUpdate(SPWeb spWeb, List<BusinessTripSchedule> items)
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

                spListItem[StringConstant.BusinessTripScheduleList.Fields.BusinessTripManagementID] = item.BusinessTripManagementID.LookupId;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.DepartDate] = item.DepartDate.Value;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.FlightName] = item.FlightName;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.City] = item.City;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.Country] = item.Country;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.ContactCompany] = item.ContactCompany;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.ContactPhone] = item.ContactPhone;
                spListItem[StringConstant.BusinessTripScheduleList.Fields.OtherSchedule] = item.OtherSchedule;

                spListItem.Update();
            }
            spWeb.AllowUnsafeUpdates = false;
        }

        #region Private Methods
        private string BuildQueryStringGetBusinessTripSchedule(int parentId)
        {
            string queryStr = string.Format(@"<Where>
                                              <Eq>
                                                 <FieldRef Name='" + StringConstant.BusinessTripScheduleList.Fields.BusinessTripManagementID + @"' LookupId='TRUE' />
                                                 <Value Type='Lookup'>{0}</Value>
                                              </Eq>
                                           </Where>", parentId);

            return queryStr;
        }
        #endregion
    }
}
