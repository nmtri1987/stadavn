using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Models;
using RBVH.Stada.Intranet.Biz.Extension;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class ShiftTimeDAL : BaseDAL<ShiftTime>
    {
        private ShiftManagementDAL _shiftManagementDAL;
        private ShiftManagementDetailDAL ShiftManagementDetailDAL;
        public ShiftTimeDAL(string siteUrl) : base(siteUrl)
        {
            ListUrl = "/Lists/ShiftTime";
            _shiftManagementDAL = new ShiftManagementDAL(siteUrl);
            ShiftManagementDetailDAL = new ShiftManagementDetailDAL(siteUrl);
        }

        public override ShiftTime ParseToEntity(SPListItem listItem)
        {
            var shiftTime = new ShiftTime
            {
                ID = listItem.ID,
                Code = Convert.ToString(listItem[StringConstant.ShiftTime.CodeField]),
                Name = Convert.ToString(listItem[StringConstant.ShiftTime.NameField]),
                WorkingHourFromHour = Convert.ToDateTime(listItem[StringConstant.ShiftTime.ShiftTimeWorkingHourFromField]),
                WorkingHourToHour = Convert.ToDateTime(listItem[StringConstant.ShiftTime.ShiftTimeWorkingHourToField]),
                WorkingHourMidHour = string.IsNullOrEmpty(listItem[StringConstant.ShiftTime.ShiftTimeWorkingHourMidField] + "") ? (DateTime?)null : Convert.ToDateTime(listItem[StringConstant.ShiftTime.ShiftTimeWorkingHourMidField]),
                BreakHourFromHour = Convert.ToDateTime(listItem[StringConstant.ShiftTime.ShiftTimeBreakingHourFromField]),
                BreakHourToHour = Convert.ToDateTime(listItem[StringConstant.ShiftTime.ShiftTimeBreakingHourToField]),
                ShiftTimeWorkingHourNumber = Convert.ToDouble(listItem[StringConstant.ShiftTime.ShiftTimeWorkingHourField]),
                ShiftTimeBreakHourNumber = Convert.ToDouble(listItem[StringConstant.ShiftTime.ShiftTimeBreakingHourField]),
                Description = Convert.ToString(listItem[StringConstant.ShiftTime.DescriptionField]),
                UnexpectedLeaveFirstApprovalRole = listItem.ToLookupItemModel(StringConstant.ShiftTime.UnexpectedLeaveFirstApprovalRoleField),
                ShiftRequired = Convert.ToBoolean(listItem[StringConstant.ShiftTime.ShiftRequiredField])
            };
            return shiftTime;
        }

        public ShiftTime GetShiftTimeByDate(int day, int month, int year, int department, int locationId, int employeeId)
        {
            ShiftTime shiftTime = new ShiftTime();
            if (day > 20)
            {
                if (month == 12)
                {
                    month = 1;
                    year = year + 1;
                }
                else
                {
                    month = month + 1;
                }
            }
            var shiftManagementList = _shiftManagementDAL.GetByMonthYearDepartment(month, year, department, locationId).SingleOrDefault();
            if (shiftManagementList != null)
            {
                var shiftmanagementDetail = ShiftManagementDetailDAL.GetBy_ShiftManagementID_EmployeeID(shiftManagementList.ID, employeeId).SingleOrDefault();
                if (shiftmanagementDetail != null)
                {
                    var lookupItem = (LookupItem)shiftmanagementDetail.GetType().GetProperty("ShiftTime" + day).GetValue(shiftmanagementDetail);
                    var isApproved = (Boolean)shiftmanagementDetail.GetType().GetProperty("ShiftTime" + day + "Approval").GetValue(shiftmanagementDetail);
                    if (lookupItem != null && lookupItem.LookupId > 0 && isApproved)
                    {
                        shiftTime = GetByID(lookupItem.LookupId);
                    }

                }
            }
            
            return shiftTime;
        }

        public List<ShiftTime> GetShiftTimes()
        {
            List<ShiftTime> shiftTimes = new List<ShiftTime>();

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var shiftTimeItems = this.GetAllToSPListItemCollection(spWeb);
                        if (shiftTimeItems != null && shiftTimeItems.Count > 0)
                        {
                            foreach (SPListItem item in shiftTimeItems)
                            {
                                shiftTimes.Add(ParseToEntity(item));
                            }
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection shiftTimeItems = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    shiftTimeItems = this.GetAllToSPListItemCollection(currentWeb);
                }
                else
                {
                    shiftTimeItems = this.GetAllToSPListItemCollection(SPContext.Current.Site.RootWeb);
                }

                if (shiftTimeItems != null && shiftTimeItems.Count > 0)
                {
                    foreach (SPListItem item in shiftTimeItems)
                    {
                        shiftTimes.Add(ParseToEntity(item));
                    }
                }
            }

            return shiftTimes;
        }

        public ShiftTime GetShiftTimeByCode(string shiftCode)
        {
            ShiftTime shiftTime = null;

            string queryString = @"<Where>
                                      <Eq>
                                         <FieldRef Name='Code' />
                                         <Value Type='Text'>" + shiftCode + @"</Value>
                                      </Eq>
                                   </Where>";

            if (SPContext.Current == null)
            {
                using (SPSite spSite = new SPSite(SiteUrl))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        var shiftTimeItems = this.GetByQueryToSPListItemCollection(spWeb, queryString);
                        if (shiftTimeItems != null && shiftTimeItems.Count > 0)
                        {
                            shiftTime = new ShiftTime();
                            shiftTime = ParseToEntity(shiftTimeItems[0]);
                        }
                    }
                }
            }
            else
            {
                SPListItemCollection shiftTimeItems = null;
                var currentWeb = SPContext.Current.Web;
                if (currentWeb.IsRootWeb == true)
                {
                    shiftTimeItems = this.GetByQueryToSPListItemCollection(currentWeb, queryString);
                }
                else
                {
                    shiftTimeItems = this.GetByQueryToSPListItemCollection(SPContext.Current.Site.RootWeb, queryString);
                }

                if (shiftTimeItems != null && shiftTimeItems.Count > 0)
                {
                    shiftTime = new ShiftTime();
                    shiftTime = ParseToEntity(shiftTimeItems[0]);
                }
            }

            return shiftTime;
        }
    }
}
