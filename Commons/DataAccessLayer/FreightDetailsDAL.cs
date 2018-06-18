using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Builder;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DTO;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class FreightDetailsDAL : BaseDAL<FreightDetails>
    {
        public FreightDetailsDAL(string siteUrl) : base(siteUrl) { }

        public int SaveOrUpdate(FreightDetails item)
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

        public List<FreightDetails> GetItemsByParentId(int parentId)
        {
            return this.GetByQuery(BuildQueryGetFreightDetailsByParentId(parentId));
        }

        public List<FreightDetails> GetItemByIds(List<string> Ids)
        {
            return this.GetByQuery(BuildQueryGetFreightDetailsByIds(Ids));
        }

        public int SaveOrUpdate(SPWeb spWeb, FreightDetails item)
        {
            int itemId = 0;

            SPList spList = spWeb.GetList($"{spWeb.Url}{ListUrl}");
            SPListItem spListItem;
            if (item.ID > 0)
                spListItem = spList.GetItemById(item.ID);
            else
                spListItem = spList.AddItem();

            spListItem[StringConstant.FreightDetailsList.FreightManagementIDField] = item.FreightManagementID.LookupId;
            spListItem[StringConstant.FreightDetailsList.GoodsNameField] = item.GoodsName;
            spListItem[StringConstant.FreightDetailsList.UnitField] = item.Unit;
            spListItem[StringConstant.FreightDetailsList.QuantityField] = item.Quantity;
            spListItem[StringConstant.FreightDetailsList.RemarksField] = item.Remarks;
            spListItem[StringConstant.FreightDetailsList.ShippingInField] = item.ShippingIn.HasValue == true ? item.ShippingIn : null;
            spListItem[StringConstant.FreightDetailsList.ShippingOutField] = item.ShippingOut.HasValue == true ? item.ShippingOut : null;
            spListItem[StringConstant.FreightDetailsList.CheckInByField] = item.CheckInBy != null ? item.CheckInBy.LookupId : 0;
            spListItem[StringConstant.FreightDetailsList.CheckOutByField] = item.CheckOutBy != null ? item.CheckOutBy.LookupId : 0;

            spWeb.AllowUnsafeUpdates = true;
            spListItem.Update();
            itemId = spListItem.ID;
            spWeb.AllowUnsafeUpdates = false;

            return itemId;
        }

        public void SaveOrUpdate(SPWeb spWeb, List<FreightDetails> items)
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

                spListItem[StringConstant.FreightDetailsList.FreightManagementIDField] = item.FreightManagementID.LookupId;
                spListItem[StringConstant.FreightDetailsList.GoodsNameField] = item.GoodsName;
                spListItem[StringConstant.FreightDetailsList.UnitField] = item.Unit;
                spListItem[StringConstant.FreightDetailsList.QuantityField] = item.Quantity;
                spListItem[StringConstant.FreightDetailsList.RemarksField] = item.Remarks;
                spListItem[StringConstant.FreightDetailsList.ShippingInField] = item.ShippingIn.HasValue == true ? item.ShippingIn : null;
                spListItem[StringConstant.FreightDetailsList.ShippingOutField] = item.ShippingOut.HasValue == true ? item.ShippingOut : null;
                spListItem[StringConstant.FreightDetailsList.CheckInByField] = item.CheckInBy != null ? item.CheckInBy.LookupId : 0;
                spListItem[StringConstant.FreightDetailsList.CheckOutByField] = item.CheckOutBy != null ? item.CheckOutBy.LookupId : 0;

                spListItem.Update();
            }
            spWeb.AllowUnsafeUpdates = false;
        }

        #region Private Methods
        private string BuildQueryGetFreightDetailsByParentId(int parentId)
        {
            string queryStr = string.Format(@"<Where>
                                              <Eq>
                                                 <FieldRef Name='"+ StringConstant.FreightDetailsList.FreightManagementIDField + @"' LookupId='TRUE' />
                                                 <Value Type='Lookup'>{0}</Value>
                                              </Eq>
                                           </Where>", parentId);

            return queryStr;
        }

        private string BuildQueryGetFreightDetailsByIds(List<string> Ids)
        {
            string queryString = "";

            if (Ids != null && Ids.Count > 0)
            {
                foreach (var Id in Ids)
                {
                    queryString += string.Format("<Value Type = 'Number'>{0}</Value>", Id);
                }
                if (!string.IsNullOrEmpty(queryString))
                {
                    queryString = string.Format("<Where><In><FieldRef Name = 'ID'/><Values>{0}</Values></In></Where>", queryString);
                }
            }
            else
            {
                queryString = "<Where><Eq><FieldRef Name = 'ID'/><Value Type='Counter'>0</Value></Eq></Where>";
            }

            return queryString;
        }
        #endregion
    }
}
