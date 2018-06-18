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
    /// RequestForDiplomaSupplyApprovalManager
    /// </summary>
    public class RequestForDiplomaSupplyApprovalManager : ApprovalBaseManager
    {
        #region Constants

        /// <summary>
        /// Requester_DepartmentName_EN
        /// </summary>
        private const string Requester_DepartmentName_EN = "Requester_DepartmentName_EN";

        /// <summary>
        /// Requester_DepartmentName_VN
        /// </summary>
        private const string Requester_DepartmentName_VN = "Requester_DepartmentName_VN";

        /// <summary>
        /// Requester_Position_EN
        /// </summary>
        private const string Requester_Position_EN = "Requester_Position_EN";

        /// <summary>
        /// Requester_Position_VN
        /// </summary>
        private const string Requester_Position_VN = "Requester_Position_VN";

        #endregion

        #region Constructors
        protected RequestForDiplomaSupplyApprovalManager() : base()
        {
            this.AdditionalInfoEmailObject.Add(Requester_DepartmentName_EN, "");
            this.AdditionalInfoEmailObject.Add(Requester_DepartmentName_VN, "");
            this.AdditionalInfoEmailObject.Add(Requester_Position_EN, "");
            this.AdditionalInfoEmailObject.Add(Requester_Position_VN, "");

            this.OnBeforeBuildBodyEmail += RequestForDiplomaSupplyApprovalManager_OnBeforeBuildBodyEmail;
        }

        public RequestForDiplomaSupplyApprovalManager(string siteUrl, string listName, SPListItem currentItem, SPWeb currentWeb) : 
            base(siteUrl, listName, currentItem, currentWeb)
        {
            this.AdditionalInfoEmailObject.Add(Requester_DepartmentName_EN, "");
            this.AdditionalInfoEmailObject.Add(Requester_DepartmentName_VN, "");
            this.AdditionalInfoEmailObject.Add(Requester_Position_EN, "");
            this.AdditionalInfoEmailObject.Add(Requester_Position_VN, "");

            this.OnBeforeBuildBodyEmail += RequestForDiplomaSupplyApprovalManager_OnBeforeBuildBodyEmail;
        }
        #endregion

        #region Methods

        private void RequestForDiplomaSupplyApprovalManager_OnBeforeBuildBodyEmail(object sender, System.EventArgs e)
        {
            try
            {
                if(this.CurrentDepartment != null)
                {
                    var department = this.DepartmentDAL.GetByID(this.CurrentDepartment.LookupId);
                    if (department != null)
                    {
                        this.AdditionalInfoEmailObject[Requester_DepartmentName_EN] = department.Name;
                        this.AdditionalInfoEmailObject[Requester_DepartmentName_VN] = department.VietnameseName;
                    }

                    if (this.Creator != null)
                    {
                        if (this.Creator.EmployeePosition != null)
                        {
                            var employeePosition = this.EmployeePositionDAL.GetByID(this.Creator.EmployeePosition.LookupId);
                            if (employeePosition != null)
                            {
                                this.AdditionalInfoEmailObject[Requester_Position_EN] = employeePosition.Name;
                                this.AdditionalInfoEmailObject[Requester_Position_VN] = employeePosition.VietnameseName;
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        #endregion

        #region Overrides

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

        #endregion
    }
}
