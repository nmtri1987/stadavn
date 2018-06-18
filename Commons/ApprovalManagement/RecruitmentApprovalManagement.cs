using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.ApprovalManagement
{
    /// <summary>
    /// Management approval for RECRUITMENT module.
    /// </summary>
    public class RecruitmentApprovalManagement : ApprovalBaseManager
    {
        #region Constants
        /// <summary>
        /// DEH
        /// </summary>
        //private const string EmployeePosition_DEH_CODE = "DEH";

        /// <summary>
        /// HR
        /// </summary>
        //private const string HR_Administration_Department_Code = "HR";

        /// <summary>
        /// PrintLinkEN
        /// </summary>
        public const string PrintLinkEN_Key = "PrintLinkEN";

        /// <summary>
        /// PrintLinkVN
        /// </summary>
        public const string PrintLinkVN_Key = "PrintLinkVN";
        #endregion

        #region Constructors
        /// <summary>
        /// RecruitmentApprovalManagement
        /// </summary>
        protected RecruitmentApprovalManagement() : base()
        {
            this.AdditionalInfoEmailObject.Add(PrintLinkEN_Key, "");
            this.AdditionalInfoEmailObject.Add(PrintLinkVN_Key, "");
        }

        /// <summary>
        /// RecruitmentApprovalManagement
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="listName"></param>
        /// <param name="currentItem"></param>
        /// <param name="currentEmployee"></param>
        public RecruitmentApprovalManagement(string siteUrl, string listName, SPListItem currentItem, SPWeb currentWeb) : 
            base(siteUrl, listName, currentItem, currentWeb)
        {
            this.AdditionalInfoEmailObject.Add(PrintLinkEN_Key, "");
            this.AdditionalInfoEmailObject.Add(PrintLinkVN_Key, "");
        }
        #endregion

        #region Overrides

        /// <summary>
        /// Approve
        /// </summary>
        /// <returns></returns>
        public override bool Approve()
        {
            bool res = false;

            try
            {
                res = base.Approve();

                // Trưởng phòng [Hành Chánh Nhân Sự] thực hiện  sau bước BOD
                SPFieldLookupValue additionalPreviousStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalPreviousStep]);
                SPFieldLookupValue additionalStep = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalStep]);
                SPFieldLookupValue additionalDepartment = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.AdditionalDepartment]);
                if (((this.PendingAtEmployees == null) || (this.PendingAtEmployees != null && this.PendingAtEmployees.Count == 0)) &&
                    (additionalPreviousStep == null) && (additionalStep == null) && (additionalDepartment == null))
                {
                    int currentLocationId = 0;
                    int additionalApprovalDepartmentId = 0;
                    int additionalApprovalPositionId = GetDEHPositionId(); // DEH;
                    SPFieldLookupValue locationLookupValue = ObjectHelper.GetSPFieldLookupValue(this.CurrentItem[ApprovalFields.CommonLocation]);
                    if (locationLookupValue != null)
                    {
                        currentLocationId = locationLookupValue.LookupId;
                    }
                    this.CurrentItem[ApprovalFields.IsAdditionalStep] = true;
                    this.CurrentItem[ApprovalFields.AdditionalPreviousStep] = this.CurrentItem[ApprovalFields.CurrentStep];
                    this.CurrentItem[ApprovalFields.AdditionalStep] = additionalApprovalPositionId; // DEH
                    // Get ID of HR & Administration Department
                    string queryString = string.Format(@"<Where>
                                                            <Eq>
                                                                <FieldRef Name='{0}'  />
                                                                <Value Type='Text'>{1}</Value>
                                                            </Eq>
                                                       </Where>", "Code", DepartmentCode.HR);
                    Department additionalApprovalDepartment = null;
                    var departments = this.DepartmentDAL.GetByQuery(queryString);
                    if (departments != null && departments.Count > 0)
                    {
                        additionalApprovalDepartment = departments[0];
                    }
                    if (additionalApprovalDepartment != null)
                    {
                        additionalApprovalDepartmentId = additionalApprovalDepartment.ID;
                        this.CurrentItem[ApprovalFields.AdditionalDepartment] = additionalApprovalDepartmentId;
                    }

                    List<EmployeeInfo> listOfEmployees = GetListOfEmployees(currentLocationId, additionalApprovalDepartmentId, additionalApprovalPositionId);
                    if (listOfEmployees != null && listOfEmployees.Count > 0)
                    {
                        this.PendingAtEmployees = listOfEmployees.ToSPFieldLookupValueCollection();
                        this.ListOfEmployeesEmailTo.AddRange(listOfEmployees);
                    }
                    // Set PendingAt
                    this.CurrentItem[ApprovalFields.PendingAt] = this.PendingAtEmployees;

                    // Set Status
                    this.CurrentItem[ApprovalFields.Status] = string.Format("{0} {1}", this.CurrentEmployee.EmployeePosition.LookupValue, Status.Approved);

                    // Set WFStauts
                    this.CurrentItem[ApprovalFields.WFStatus] = StringConstant.ApprovalStatus.InProgress;
                    this.IsWorkflowCompleted = false;

                    this.CurrentItem.Update();
                }
            }
            catch (Exception ex)
            {
                res = false;
                ULSLogging.LogError(ex);
            }

            return res;
        }

        /// <summary>
        /// Get BOD Employee.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="departmentId"></param>
        /// <param name="positionId"></param>
        /// <returns></returns>
        protected override List<EmployeeInfo> GetListOfBODEmployees(int locationId, int departmentId, int positionId)
        {
            List<EmployeeInfo> bodEmployees = null;

            List<EmployeeInfo> employees = base.GetListOfBODEmployees(locationId, departmentId, positionId);

            if (employees != null && employees.Count > 0)
            {
                bodEmployees = new List<EmployeeInfo>();

                foreach (var employee in employees)
                {
                    // Truong hop: Co Manager thi lay Manager
                    if (employee.Manager != null && employee.Manager.LookupId > 0)
                    {
                        var bodEmployee = this.EmployeeInfoDAL.GetByID(employee.Manager.LookupId);
                        if (bodEmployee != null)
                        {
                            bodEmployees.Add(bodEmployee);
                        }
                    }
                    // Truong hop: Khong co Manager thi lay chinh ho
                    else
                    {
                        bodEmployees.Add(employee);
                    }
                }
            }

            return bodEmployees;
        }

        #endregion
        
    }
}
