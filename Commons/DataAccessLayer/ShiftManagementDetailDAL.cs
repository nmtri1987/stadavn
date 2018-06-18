using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class ShiftManagementDetailDAL : BaseDAL<ShiftManagementDetail>
    {
        public ShiftManagementDetailDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/ShiftManagementDetail";
        }

        public override ShiftManagementDetail ParseToEntity(SPListItem listItem)
        {
            var shiftManagement = new ShiftManagementDetail
            {
                ID = listItem.ID,
                Employee = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.EmployeeField),
                ShiftManagementID = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftManagementIDField),
                ShiftTime1 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime1Field),
                ShiftTime2 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime2Field),
                ShiftTime3 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime3Field),
                ShiftTime4 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime4Field),
                ShiftTime5 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime5Field),
                ShiftTime6 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime6Field),
                ShiftTime7 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime7Field),
                ShiftTime8 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime8Field),
                ShiftTime9 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime9Field),
                ShiftTime10 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime10Field),
                ShiftTime11 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime11Field),
                ShiftTime12 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime12Field),
                ShiftTime13 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime13Field),
                ShiftTime14 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime14Field),
                ShiftTime15 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime15Field),
                ShiftTime16 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime16Field),
                ShiftTime17 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime17Field),
                ShiftTime18 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime18Field),
                ShiftTime19 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime19Field),
                ShiftTime20 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime20Field),
                ShiftTime21 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime21Field),
                ShiftTime22 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime22Field),
                ShiftTime23 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime23Field),
                ShiftTime24 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime24Field),
                ShiftTime25 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime25Field),
                ShiftTime26 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime26Field),
                ShiftTime27 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime27Field),
                ShiftTime28 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime28Field),
                ShiftTime29 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime29Field),
                ShiftTime30 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime30Field),
                ShiftTime31 = listItem.ToLookupItemModel(StringConstant.ShiftManagementDetailList.ShiftTime31Field),
                ShiftTime1Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime1ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime1ApprovalField)),
                ShiftTime2Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime2ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime2ApprovalField)),
                ShiftTime3Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime3ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime3ApprovalField)),
                ShiftTime4Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime4ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime4ApprovalField)),
                ShiftTime5Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime5ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime5ApprovalField)),
                ShiftTime6Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime6ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime6ApprovalField)),
                ShiftTime7Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime7ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime7ApprovalField)),
                ShiftTime8Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime8ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime8ApprovalField)),
                ShiftTime9Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime9ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime9ApprovalField)),
                ShiftTime10Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime10ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime10ApprovalField)),
                ShiftTime11Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime11ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime11ApprovalField)),
                ShiftTime12Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime12ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime12ApprovalField)),
                ShiftTime13Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime13ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime13ApprovalField)),
                ShiftTime14Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime14ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime14ApprovalField)),
                ShiftTime15Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime15ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime15ApprovalField)),
                ShiftTime16Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime16ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime16ApprovalField)),
                ShiftTime17Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime17ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime17ApprovalField)),
                ShiftTime18Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime18ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime18ApprovalField)),
                ShiftTime19Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime19ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime19ApprovalField)),
                ShiftTime20Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime20ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime20ApprovalField)),
                ShiftTime21Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime21ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime21ApprovalField)),
                ShiftTime22Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime22ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime22ApprovalField)),
                ShiftTime23Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime23ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime23ApprovalField)),
                ShiftTime24Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime24ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime24ApprovalField)),
                ShiftTime25Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime25ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime25ApprovalField)),
                ShiftTime26Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime26ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime26ApprovalField)),
                ShiftTime27Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime27ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime27ApprovalField)),
                ShiftTime28Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime28ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime28ApprovalField)),
                ShiftTime29Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime29ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime29ApprovalField)),
                ShiftTime30Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime30ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime30ApprovalField)),
                ShiftTime31Approval = string.IsNullOrEmpty(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime31ApprovalField)) ? false : Convert.ToBoolean(listItem.ToString(StringConstant.ShiftManagementDetailList.ShiftTime31ApprovalField)),

            };
            return shiftManagement;
        }
        public ShiftManagementDetail GetById(int id)
        {
            ShiftManagementDetail shiftManagementDetail = new ShiftManagementDetail();
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                //// Query SPList
                if (id > 0)
                {
                    using (SPSite site = new SPSite(SiteUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList splist = web.GetList($"{web.Url}{ListUrl}");

                            var shiftManagementDetailItem = splist.GetItemById(id);
                            if (shiftManagementDetailItem != null)
                            {
                                // Get First and Parse to EmployeeInfo
                                // var shiftManagementItem = shiftManagementItems[0];
                                shiftManagementDetail = ParseToEntity(shiftManagementDetailItem);
                            }
                        }
                    }
                }
            });

            return shiftManagementDetail;
        }
        public List<ShiftManagementDetail> GetByShiftManagementID(int id)
        {
            List<ShiftManagementDetail> shiftManagementDetailList = new List<ShiftManagementDetail>();
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                //// Query SPList
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList splist = web.GetList($"{web.Url}{ListUrl}");
                        SPQuery spquery = new SPQuery
                        {
                            Query =
                             $@"<Where>
                              <Eq>
                                 <FieldRef Name='ShiftManagementID' />
                                 <Value Type='Lookup' LookupId='TRUE'>{id}</Value>
                              </Eq>
                           </Where>"
                        };

                        var shiftManagementDetailItems = splist.GetItems(spquery);
                        if (shiftManagementDetailItems.Count > 0)
                        {
                            // Get First and Parse to EmployeeInfo
                            //var shiftManagementItem = shiftManagementItems[0];
                            //shiftManagement = ParseToEntity(shiftManagementItem);
                            shiftManagementDetailList.AddRange(from SPListItem item in shiftManagementDetailItems select ParseToEntity(item));
                        }
                    }
                }
            });

            return shiftManagementDetailList;
        }
        public List<ShiftManagementDetail> GetBy_ShiftManagementID_EmployeeID(int ShiftManagementID, int EmployeeID)
        {
            List<ShiftManagementDetail> shiftManagementDetailList = new List<ShiftManagementDetail>();

            string queryStr =
                        $@" <Where>
                              <And>
                                 <Eq>
                                    <FieldRef Name='ShiftManagementID' />
                                    <Value Type='Lookup'>{ShiftManagementID}</Value>
                                 </Eq>
                                 <Eq>
                                    <FieldRef Name='Employee' LookupId='TRUE' />
                                    <Value Type='Lookup'>{EmployeeID}</Value>
                                 </Eq>
                              </And>
                  
                    </Where>";

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var shiftManagementDetailItems = this.GetByQueryToSPListItemCollection(spWeb, queryStr);
                        if (shiftManagementDetailItems != null && shiftManagementDetailItems.Count > 0)
                        {
                            foreach (SPListItem item in shiftManagementDetailItems)
                            {
                                shiftManagementDetailList.Add(ParseToEntity(item));
                            }
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection shiftManagementDetailItems = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    shiftManagementDetailItems = this.GetByQueryToSPListItemCollection(currentWeb, queryStr);
                }
                else
                {
                    shiftManagementDetailItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, queryStr);
                }

                if (shiftManagementDetailItems != null && shiftManagementDetailItems.Count > 0)
                {
                    foreach (SPListItem item in shiftManagementDetailItems)
                    {
                        shiftManagementDetailList.Add(ParseToEntity(item));
                    }
                }
            }

            return shiftManagementDetailList;
        }

        public List<ShiftManagementDetail> GetByShiftManagementIDEmployeeID(int ShiftManagementID, int EmployeeID)
        {
            List<ShiftManagementDetail> shiftManagementDetailList = new List<ShiftManagementDetail>();

            string query = $@"<Where>
                                  <And>
                                     <Eq>
                                        <FieldRef Name='ShiftManagementID' />
                                        <Value Type='Lookup'>{ShiftManagementID}</Value>
                                     </Eq>
                                     <Eq>
                                        <FieldRef Name='Employee' LookupId='TRUE' />
                                        <Value Type='Lookup'>{EmployeeID}</Value>
                                     </Eq>
                                  </And>
                            </Where>";

            shiftManagementDetailList = GetByQuery(query);

            return shiftManagementDetailList;
        }

        public void BulkUpdate(List<ShiftManagementDetail> newItems)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList shiftManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        if (shiftManagementDetail != null)
                        {

                            web.AllowUnsafeUpdates = true;
                            Guid listGuid = shiftManagementDetail.ID;
                            StringBuilder query = new StringBuilder();
                            query.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            foreach (var item in newItems)
                            {
                                query.AppendFormat("<Method>" +
                                    "<SetList Scope=\"Request\" >{1}</SetList>" +
                                    "<SetVar Name=\"ID\">" + item.ID + "</SetVar>" +
                                    "<SetVar Name=\"Cmd\">Save</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftManagementID\">" + item.ShiftManagementID.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}Employee\">" + item.Employee.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime1\">" + item.ShiftTime1.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime2\">" + item.ShiftTime2.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime3\">" + item.ShiftTime3.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime4\">" + item.ShiftTime4.LookupId + "</SetVar>" +
                                   "<SetVar Name=\"{2}ShiftTime5\">" + item.ShiftTime5.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime6\">" + item.ShiftTime6.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime7\">" + item.ShiftTime7.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime8\">" + item.ShiftTime8.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime9\">" + item.ShiftTime9.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime10\">" + item.ShiftTime10.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime11\">" + item.ShiftTime11.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime12\">" + item.ShiftTime12.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime13\">" + item.ShiftTime13.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime14\">" + item.ShiftTime14.LookupId + "</SetVar>" +
                                   "<SetVar Name=\"{2}ShiftTime15\">" + item.ShiftTime15.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime16\">" + item.ShiftTime16.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime17\">" + item.ShiftTime17.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime18\">" + item.ShiftTime18.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime19\">" + item.ShiftTime19.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime20\">" + item.ShiftTime20.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime21\">" + item.ShiftTime21.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime22\">" + item.ShiftTime22.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime23\">" + item.ShiftTime23.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime24\">" + item.ShiftTime24.LookupId + "</SetVar>" +
                                   "<SetVar Name=\"{2}ShiftTime25\">" + item.ShiftTime25.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime26\">" + item.ShiftTime26.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime27\">" + item.ShiftTime27.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime28\">" + item.ShiftTime28.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime29\">" + item.ShiftTime29.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime30\">" + item.ShiftTime30.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime31\">" + item.ShiftTime31.LookupId + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime1Approval\">" + item.ShiftTime1Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime2Approval\">" + item.ShiftTime2Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime3Approval\">" + item.ShiftTime3Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime4Approval\">" + item.ShiftTime4Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime5Approval\">" + item.ShiftTime5Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime6Approval\">" + item.ShiftTime6Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime7Approval\">" + item.ShiftTime7Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime8Approval\">" + item.ShiftTime8Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime9Approval\">" + item.ShiftTime9Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime10Approval\">" + item.ShiftTime10Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime11Approval\">" + item.ShiftTime11Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime12Approval\">" + item.ShiftTime12Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime13Approval\">" + item.ShiftTime13Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime14Approval\">" + item.ShiftTime14Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime15Approval\">" + item.ShiftTime15Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime16Approval\">" + item.ShiftTime16Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime17Approval\">" + item.ShiftTime17Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime18Approval\">" + item.ShiftTime18Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime19Approval\">" + item.ShiftTime19Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime20Approval\">" + item.ShiftTime20Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime21Approval\">" + item.ShiftTime21Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime22Approval\">" + item.ShiftTime22Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime23Approval\">" + item.ShiftTime23Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime24Approval\">" + item.ShiftTime24Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime25Approval\">" + item.ShiftTime25Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime26Approval\">" + item.ShiftTime26Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime27Approval\">" + item.ShiftTime27Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime28Approval\">" + item.ShiftTime28Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime29Approval\">" + item.ShiftTime29Approval + "</SetVar>" +
                                    "<SetVar Name=\"{2}ShiftTime30Approval\">" + item.ShiftTime30Approval + "</SetVar>" +
                                     "<SetVar Name=\"{2}ShiftTime31Approval\">" + item.ShiftTime31Approval + "</SetVar>" +

                                    "</Method>", "ShiftManagementDettail", listGuid,
                                    "urn:schemas-microsoft-com:office:office#");
                            }
                            query.Append("</Batch>");
                            web.ProcessBatchData(query.ToString());
                            web.Update();
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });

        }
        public void BulkInsert(List<ShiftManagementDetail> newItems)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList shiftManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        if (shiftManagementDetail != null)
                        {
                            web.AllowUnsafeUpdates = true;
                            Guid listGuid = shiftManagementDetail.ID;
                            StringBuilder query = new StringBuilder();
                            query.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                            foreach (var item in newItems)
                            {
                                query.AppendFormat("<Method ID=\"{0}\">" +
                                "<SetList>{1}</SetList>" +
                                "<SetVar Name=\"ID\">New</SetVar>" +
                                "<SetVar Name=\"Cmd\">Save</SetVar>" +
                                "<SetVar Name=\"{2}ShiftManagementID\">" + item.ShiftManagementID.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}Employee\">" + item.Employee.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime1\">" + item.ShiftTime1.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime2\">" + item.ShiftTime2.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime3\">" + item.ShiftTime3.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime4\">" + item.ShiftTime4.LookupId + "</SetVar>" +
                               "<SetVar Name=\"{2}ShiftTime5\">" + item.ShiftTime5.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime6\">" + item.ShiftTime6.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime7\">" + item.ShiftTime7.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime8\">" + item.ShiftTime8.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime9\">" + item.ShiftTime9.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime10\">" + item.ShiftTime10.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime11\">" + item.ShiftTime11.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime12\">" + item.ShiftTime12.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime13\">" + item.ShiftTime13.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime14\">" + item.ShiftTime14.LookupId + "</SetVar>" +
                               "<SetVar Name=\"{2}ShiftTime15\">" + item.ShiftTime15.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime16\">" + item.ShiftTime16.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime17\">" + item.ShiftTime17.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime18\">" + item.ShiftTime18.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime19\">" + item.ShiftTime19.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime20\">" + item.ShiftTime20.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime21\">" + item.ShiftTime21.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime22\">" + item.ShiftTime22.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime23\">" + item.ShiftTime23.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime24\">" + item.ShiftTime24.LookupId + "</SetVar>" +
                               "<SetVar Name=\"{2}ShiftTime25\">" + item.ShiftTime25.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime26\">" + item.ShiftTime26.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime27\">" + item.ShiftTime27.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime28\">" + item.ShiftTime28.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime29\">" + item.ShiftTime29.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime30\">" + item.ShiftTime30.LookupId + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime31\">" + item.ShiftTime31.LookupId + "</SetVar>" +
                              "<SetVar Name=\"{2}ShiftTime1Approval\">" + item.ShiftTime1Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime2Approval\">" + item.ShiftTime2Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime3Approval\">" + item.ShiftTime3Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime4Approval\">" + item.ShiftTime4Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime5Approval\">" + item.ShiftTime5Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime6Approval\">" + item.ShiftTime6Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime7Approval\">" + item.ShiftTime7Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime8Approval\">" + item.ShiftTime8Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime9Approval\">" + item.ShiftTime9Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime10Approval\">" + item.ShiftTime10Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime11Approval\">" + item.ShiftTime11Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime12Approval\">" + item.ShiftTime12Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime13Approval\">" + item.ShiftTime13Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime14Approval\">" + item.ShiftTime14Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime15Approval\">" + item.ShiftTime15Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime16Approval\">" + item.ShiftTime16Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime17Approval\">" + item.ShiftTime17Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime18Approval\">" + item.ShiftTime18Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime19Approval\">" + item.ShiftTime19Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime20Approval\">" + item.ShiftTime20Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime21Approval\">" + item.ShiftTime21Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime22Approval\">" + item.ShiftTime22Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime23Approval\">" + item.ShiftTime23Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime24Approval\">" + item.ShiftTime24Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime25Approval\">" + item.ShiftTime25Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime26Approval\">" + item.ShiftTime26Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime27Approval\">" + item.ShiftTime27Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime28Approval\">" + item.ShiftTime28Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime29Approval\">" + item.ShiftTime29Approval + "</SetVar>" +
                                "<SetVar Name=\"{2}ShiftTime30Approval\">" + item.ShiftTime30Approval + "</SetVar>" +
                                 "<SetVar Name=\"{2}ShiftTime31Approval\">" + item.ShiftTime31Approval + "</SetVar>" +

                                "</Method>", "ShiftManagementDettail", listGuid,
                                "urn:schemas-microsoft-com:office:office#");
                            }
                            query.Append("</Batch>");
                            web.ProcessBatchData(query.ToString());
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                }
            });
        }

        public int Approve(ShiftManagementDetail newItem)
        {
            int returnId = 0;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        SPList spList = web.GetList($"{web.Url}{ListUrl}");
                        SPListItem spListItem = spList.GetItemById(newItem.ID); ;

                        spListItem[StringConstant.ShiftManagementDetailList.ShiftManagementIDField] = newItem.ShiftManagementID.LookupId;
                        spListItem[StringConstant.ShiftManagementDetailList.EmployeeField] = newItem.Employee.LookupId;

                        for (int i = 1; i <= 31; i++)
                        {
                            var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                            var shiftTimeApprovalPropertyName = string.Format("ShiftTime{0}Approval", i);
                            var shiftTimeEntity = newItem.GetType().GetProperty(shiftTimePropertyName).GetValue(newItem) as LookupItem;
                            if (shiftTimeEntity != null)
                            {
                                spListItem[shiftTimePropertyName] = shiftTimeEntity.LookupId;
                                spListItem[shiftTimeApprovalPropertyName] = shiftTimeEntity.LookupId > 0 ? true : false;
                            }
                        }

                        spListItem.Update();
                        returnId = spListItem.ID;
                        web.AllowUnsafeUpdates = false;
                    }
                }
            });

            return returnId;
        }

        public void Approve(List<ShiftManagementDetail> newItems)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        web.AllowUnsafeUpdates = true;
                        SPList spList = web.GetList($"{web.Url}{ListUrl}");

                        foreach (var newItem in newItems)
                        {
                            SPListItem spListItem = spList.GetItemById(newItem.ID); ;

                            spListItem[StringConstant.ShiftManagementDetailList.ShiftManagementIDField] = newItem.ShiftManagementID.LookupId;
                            spListItem[StringConstant.ShiftManagementDetailList.EmployeeField] = newItem.Employee.LookupId;

                            for (int i = 1; i <= 31; i++)
                            {
                                var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                                var shiftTimeApprovalPropertyName = string.Format("ShiftTime{0}Approval", i);
                                var shiftTimeEntity = newItem.GetType().GetProperty(shiftTimePropertyName).GetValue(newItem) as LookupItem;
                                if (shiftTimeEntity != null)
                                {
                                    spListItem[shiftTimePropertyName] = shiftTimeEntity.LookupId;
                                    spListItem[shiftTimeApprovalPropertyName] = shiftTimeEntity.LookupId > 0 ? true : false;
                                }
                            }

                            spListItem.Update();
                        }

                        web.AllowUnsafeUpdates = false;
                    }
                }
            });
        }

        public int RefreshApprove(ShiftManagementDetail newItem)
        {
            int returnId = 0;
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList spList = web.GetList($"{web.Url}{ListUrl}");
                    SPListItem spListItem = spList.GetItemById(newItem.ID); ;

                    spListItem[StringConstant.ShiftManagementDetailList.ShiftManagementIDField] = newItem.ShiftManagementID.LookupId;
                    spListItem[StringConstant.ShiftManagementDetailList.EmployeeField] = newItem.Employee.LookupId;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime1ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime2ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime3ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime4ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime5ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime6ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime7ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime8ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime9ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime10ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime11ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime12ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime13ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime14ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime15ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime16ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime17ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime18ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime19ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime20ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime21ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime22ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime23ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime24ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime25ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime26ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime27ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime28ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime29ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime30ApprovalField] = false;
                    spListItem[StringConstant.ShiftManagementDetailList.ShiftTime31ApprovalField] = false;

                    spListItem.Update();
                    returnId = spListItem.ID;
                    web.AllowUnsafeUpdates = false;
                }
            }

            return returnId;
        }

        public bool UpdateNewShiftValue(int itemId, string columnName, int newShiftId)
        {
            bool isUpdated = false;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList shiftManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        var listItem = shiftManagementDetail.GetItemById(itemId);
                        if (listItem != null)
                        {
                            web.AllowUnsafeUpdates = true;
                            listItem[columnName] = newShiftId;
                            if (newShiftId == 0)
                                listItem[columnName + "Approval"] = false;
                            listItem.Update();
                            web.AllowUnsafeUpdates = false;
                            isUpdated = true;
                        }
                        else
                        {
                            isUpdated = false;
                        }
                    }
                }
            });
            return isUpdated;
        }

        public bool UpdateLeaveValue(int itemId, string columnName, int newShiftId)
        {
            bool isUpdated = false;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList shiftManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        var listItem = shiftManagementDetail.GetItemById(itemId);
                        if (listItem != null)
                        {
                            web.AllowUnsafeUpdates = true;
                            listItem[columnName] = newShiftId;
                            listItem[columnName + "Approval"] = true;
                            listItem.Update();
                            web.AllowUnsafeUpdates = false;
                            isUpdated = true;
                        }
                        else
                        {
                            isUpdated = false;
                        }
                    }
                }
            });
            return isUpdated;
        }

        public bool UpdateNewShiftValue(int itemId, List<string> columnNames, int newShiftId)
        {
            bool isUpdated = false;
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(SiteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList shiftManagementDetail = web.GetList($"{web.Url}{ListUrl}");

                        var listItem = shiftManagementDetail.GetItemById(itemId);
                        if (listItem != null)
                        {
                            foreach (var columnName in columnNames)
                            {
                                listItem[columnName] = newShiftId;
                                if (newShiftId == 0)
                                    listItem[columnName + "Approval"] = false;
                            }

                            web.AllowUnsafeUpdates = true;
                            listItem.Update();
                            web.AllowUnsafeUpdates = false;
                            isUpdated = true;
                        }
                        else
                        {
                            isUpdated = false;
                        }
                    }
                }
            });
            return isUpdated;
        }

        public int SaveOrUpdate(ShiftManagementDetail newItem)
        {
            int returnId = 0;
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList spList = web.GetList($"{web.Url}{ListUrl}");
                    SPListItem spListItem;
                    if (newItem.ID > 0)
                        spListItem = spList.GetItemById(newItem.ID);
                    else
                        spListItem = spList.AddItem();

                    spListItem[StringConstant.ShiftManagementDetailList.ShiftManagementIDField] = newItem.ShiftManagementID.LookupId;
                    spListItem[StringConstant.ShiftManagementDetailList.EmployeeField] = newItem.Employee.LookupId;

                    for (int i = 1; i <= 31; i++)
                    {
                        var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                        var shiftTimeApprovalPropertyName = string.Format("ShiftTime{0}Approval", i);
                        var shiftTimeEntity = newItem.GetType().GetProperty(shiftTimePropertyName).GetValue(newItem) as LookupItem;
                        //var shiftTimeApprovedFromEntity = newItem.GetType().GetProperty(shiftTimeApprovalPropertyName).GetValue(newItem);
                        if (shiftTimeEntity != null)
                        {
                            spListItem[shiftTimePropertyName] = shiftTimeEntity.LookupId;
                            if (shiftTimeEntity.LookupValue == "P")
                                spListItem[shiftTimeApprovalPropertyName] = true;
                        }
                    }

                    spListItem.Update();
                    returnId = spListItem.ID;
                    web.AllowUnsafeUpdates = false;
                }
            }

            return returnId;
        }

        public void SaveOrUpdate(List<ShiftManagementDetail> newItems)
        {
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    web.AllowUnsafeUpdates = true;
                    SPList spList = web.GetList($"{web.Url}{ListUrl}");
                    foreach (ShiftManagementDetail newItem in newItems)
                    {
                        SPListItem spListItem;
                        if (newItem.ID > 0)
                            spListItem = spList.GetItemById(newItem.ID);
                        else
                            spListItem = spList.AddItem();

                        spListItem[StringConstant.ShiftManagementDetailList.ShiftManagementIDField] = newItem.ShiftManagementID.LookupId;
                        spListItem[StringConstant.ShiftManagementDetailList.EmployeeField] = newItem.Employee.LookupId;

                        for (int i = 1; i <= 31; i++)
                        {
                            var shiftTimePropertyName = string.Format("ShiftTime{0}", i);
                            var shiftTimeApprovalPropertyName = string.Format("ShiftTime{0}Approval", i);
                            var shiftTimeEntity = newItem.GetType().GetProperty(shiftTimePropertyName).GetValue(newItem) as LookupItem;
                            if (shiftTimeEntity != null)
                            {
                                spListItem[shiftTimePropertyName] = shiftTimeEntity.LookupId;
                                if (shiftTimeEntity.LookupValue == "P")
                                    spListItem[shiftTimeApprovalPropertyName] = true;
                            }
                        }

                        spListItem.Update();
                    }

                    web.AllowUnsafeUpdates = false;
                }
            }
        }
    }
}
