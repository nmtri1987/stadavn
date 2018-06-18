using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DelegationManagement;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Interfaces;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class NotOvertimeManagementDAL : BaseDAL<NotOvertimeManagement>, IDelegationManager, IFilterTaskManager
    {
        public NotOvertimeManagementDAL(string siteUrl) : base(siteUrl) { }

        public NotOvertimeManagement GetByDate(int employeeId, DateTime date)
        {
            NotOvertimeManagement notOvertime = new NotOvertimeManagement();
            DateTime start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            var notOvertimeResult = GetByQuery($@"
                <Where>
                      <And>
                         <Eq>
                            <FieldRef Name='Requester' LookupId='TRUE' />
                            <Value Type='Lookup'>{employeeId}</Value>
                         </Eq>
                         <And>
                            <Geq>
                               <FieldRef Name='CommonDate' />
                               <Value IncludeTimeValue='TRUE' Type='DateTime'>{start.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                            </Geq>
                            <And>
                               <Leq>
                                  <FieldRef Name='CommonDate' />
                                  <Value IncludeTimeValue='TRUE' Type='DateTime'>{end.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                               </Leq>
                               <Or>
                                  <IsNull>
                                     <FieldRef Name='ApprovalStatus' />
                                  </IsNull>
                                  <Eq>
                                     <FieldRef Name='ApprovalStatus' />
                                     <Value Type='Text'>approved</Value>
                                  </Eq>
                               </Or>
                            </And>
                         </And>
                      </And>
                   </Where>", string.Empty);
            return notOvertimeResult.FirstOrDefault();
        }

        /// <summary>
        /// Get Not Overtime Approval List
        /// </summary>
        /// <param name="fromEmployee">Approver</param>
        /// <returns>List of Not Overtime Delegation</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> listOfTasks = new List<Delegation>();

            // Step 1: Build view fields
            List<string> viewFields = new List<string>() { };
            viewFields.Add(CommonSPListField.RequesterField);
            viewFields.Add(CommonSPListField.CommonDepartmentField);
            viewFields.Add(NotOvertimeList.DateField);
            viewFields.Add(StringConstant.DefaultSPListField.CreatedField);

            // Step 2: Build query
            var query = $@"<Where>
                                      <And>
	                                     <Eq>
		                                    <FieldRef Name='CommonApprover1' LookupId='TRUE' />
		                                    <Value Type='User'>{fromEmployee.ADAccount.ID}</Value>
	                                     </Eq>
	                                     <IsNull>
		                                    <FieldRef Name='ApprovalStatus' />
	                                     </IsNull>
                                      </And>
                                    </Where>";

            List<NotOvertimeManagement> itemCollection = this.GetByQuery(query, viewFields.ToArray());

            // Step 3: Convert SPItem -> Delegation Entity
            if (itemCollection != null)
            {
                foreach (var item in itemCollection)
                {
                    Delegation delegation = new Delegation(item, fromEmployee);
                    listOfTasks.Add(delegation);
                }
            }

            return listOfTasks;
        }

        public Delegation GetDelegatedTaskInfo(string Id)
        {
            Delegation delegation = new Delegation(null);

            int listItemId = 0;
            if (int.TryParse(Id, out listItemId))
            {
                string[] viewFields = new string[] { StringConstant.NotOvertimeList.DHField, StringConstant.CommonSPListField.ApprovalStatusField,
                    StringConstant.CommonSPListField.CommonDepartmentField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                string siteUrl = SPContext.Current.Site.Url;
                List<Biz.Models.NotOvertimeManagement> notOvertimeManagementCollection = this.GetByQuery(queryStr, viewFields);
                if (notOvertimeManagementCollection != null && notOvertimeManagementCollection.Count > 0)
                {
                    Biz.Models.NotOvertimeManagement notOvertimeManagement = notOvertimeManagementCollection[0];

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(siteUrl);
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(notOvertimeManagement.DH.ID);
                    if (currentApprover != null)
                    {
                        delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.NotOvertimeList.ListUrl, notOvertimeManagement.ID);
                    }
                }
            }

            return delegation;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            NotOvertimeManagement notOvertimeManagement = this.ParseToEntity(listItem);

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            var fromEmployee = _employeeInfoDAL.GetByADAccount(notOvertimeManagement.DH.ID);
            Delegation delegation = new Delegation(notOvertimeManagement, fromEmployee, currentWeb);

            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            NotOvertimeManagement notOvertimeManagement = this.ParseToEntity(listItem);
            if (notOvertimeManagement.DH != null && notOvertimeManagement.DH.ID > 0)
            {
                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                var currentStepApprover = _employeeInfoDAL.GetByADAccount(notOvertimeManagement.DH.ID);
                string approvalStatus = notOvertimeManagement.ApprovalStatus != null ? notOvertimeManagement.ApprovalStatus.ToLower() : string.Empty;
                if (currentStepApprover != null && (approvalStatus != ApprovalStatus.Approved.ToLower() && approvalStatus != ApprovalStatus.Rejected.ToLower() && approvalStatus != ApprovalStatus.Cancelled.ToLower()))
                {
                    ret = new LookupItem() { LookupId = currentStepApprover.ID, LookupValue = currentStepApprover.FullName };
                }
            }

            return ret;
        }

        public bool IsValidTask(int listItemID)
        {
            return true;
        }

        public void Accept(IFilterTaskVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
