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
using System.Threading.Tasks;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;

namespace RBVH.Stada.Intranet.Biz.DataAccessLayer
{
    public class ChangeShiftManagementDAL : BaseDAL<ChangeShiftManagement>, IDelegationManager, IFilterTaskManager
    {
        public ChangeShiftManagementDAL(string siteUrl) : base(siteUrl) { }

        public ChangeShiftManagement GetByDate(int requesertId, DateTime date)
        {
            ChangeShiftManagement changeShiftManagement = new ChangeShiftManagement();
            DateTime from = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime to = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            //Get changeshift item with approval status is null or Approved in date of employee with passed id
            var results = GetByQuery(
                $@"
                     <Where>
                      <And>
                         <Eq>
                            <FieldRef Name='{StringConstant.CommonSPListField.RequesterField}' LookupId='TRUE' />
                            <Value Type='Lookup'>{requesertId}</Value>
                         </Eq>
                         <And>
                            <Geq>
                               <FieldRef Name='{StringConstant.ChangeShiftList.FromDateField}' />
                               <Value IncludeTimeValue='TRUE' Type='DateTime'>{from.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                            </Geq>
                            <And>
                               <Leq>
                                  <FieldRef Name='{StringConstant.ChangeShiftList.FromDateField}' />
                                  <Value IncludeTimeValue='TRUE' Type='DateTime'>{to.ToString(StringConstant.DateFormatTZForCAML)}</Value>
                               </Leq>
                               <Or>
                                  <IsNull>
                                     <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                  </IsNull>
                                  <Eq>
                                     <FieldRef Name='{StringConstant.CommonSPListField.ApprovalStatusField}' />
                                     <Value Type='Text'>Approved</Value>
                                  </Eq>
                               </Or>
                            </And>
                         </And>
                      </And>
                   </Where>",
                string.Empty);
            return results.FirstOrDefault();
        }

        /// <summary>
        /// Get Change Shift Approval List
        /// </summary>
        /// <param name="fromEmployee">Approver</param>
        /// <returns>List of Change Shift Delegation</returns>
        public List<Delegation> GetListOfTasks(EmployeeInfo fromEmployee)
        {
            List<Delegation> delegations = new List<Delegation>();

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

            var changeShiftManagementList = GetByQuery(query);
            if (changeShiftManagementList != null && changeShiftManagementList.Count > 0)
            {
                delegations = new List<Delegation>();
                foreach (var changeShiftManagement in changeShiftManagementList)
                {
                    var delegation = new Delegation(changeShiftManagement, fromEmployee);
                    delegations.Add(delegation);
                }
            }

            return delegations;
        }

        public Delegation GetDelegatedTaskInfo(string Id)
        {
            Delegation delegation = new Delegation(null);

            int listItemId = 0;
            if (int.TryParse(Id, out listItemId))
            {
                string[] viewFields = new string[] { StringConstant.ChangeShiftList.DHField, StringConstant.CommonSPListField.ApprovalStatusField,
                    StringConstant.CommonSPListField.CommonDepartmentField};
                string queryStr = $@"<Where>
                                      <Eq>
                                         <FieldRef Name='ID' />
                                         <Value Type='Counter'>{listItemId}</Value>
                                      </Eq>
                                   </Where>";
                List<Biz.Models.ChangeShiftManagement> changeShiftManagementCollection = this.GetByQuery(queryStr, viewFields);
                if (changeShiftManagementCollection != null && changeShiftManagementCollection.Count > 0)
                {
                    Biz.Models.ChangeShiftManagement changeShiftManagement = changeShiftManagementCollection[0];

                    EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                    EmployeeInfo currentApprover = _employeeInfoDAL.GetByADAccount(changeShiftManagement.DepartmentHead.ID);
                    if (currentApprover != null)
                    {
                        delegation = DelegationPermissionManager.IsDelegation(currentApprover.ID, StringConstant.ChangeShiftList.ListUrl, changeShiftManagement.ID);
                    }
                }
            }

            return delegation;
        }

        public Delegation GetDelegationListItem(SPListItem listItem, SPWeb currentWeb)
        {
            ChangeShiftManagement changeShiftManagement = this.ParseToEntity(listItem);

            EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
            EmployeeInfo fromEmployee = _employeeInfoDAL.GetByADAccount(changeShiftManagement.DepartmentHead.ID);
            Delegation delegation = new Delegation(changeShiftManagement, fromEmployee, currentWeb);
            return delegation;
        }

        public LookupItem GetCurrentEmployeeProcessing(SPListItem listItem)
        {
            LookupItem ret = null;

            ChangeShiftManagement changeShiftManagement = this.ParseToEntity(listItem);
            if (changeShiftManagement.DepartmentHead != null && changeShiftManagement.DepartmentHead.ID > 0)
            {
                EmployeeInfoDAL _employeeInfoDAL = new EmployeeInfoDAL(this.SiteUrl);
                var currentStepApprover = _employeeInfoDAL.GetByADAccount(changeShiftManagement.DepartmentHead.ID);
                string approvalStatus = changeShiftManagement.ApprovalStatus != null ? changeShiftManagement.ApprovalStatus.ToLower() : string.Empty;
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
