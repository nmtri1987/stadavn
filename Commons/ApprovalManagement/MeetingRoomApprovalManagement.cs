using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.Extension;
using RBVH.Stada.Intranet.Biz.Models;
using System;
using System.Collections.Generic;

namespace RBVH.Stada.Intranet.Biz.ApprovalManagement
{
    public class MeetingRoomApprovalManagement : ApprovalBaseManager
    {
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        protected MeetingRoomApprovalManagement() : base()
        {
        }

        /// <summary>
        /// Constructor with site url, list name, current item and current web
        /// </summary>
        /// <param name="siteUrl">The current site url</param>
        /// <param name="listName">The current list name</param>
        /// <param name="currentItem">The current item object</param>
        /// <param name="currentWeb">The current web object</param>
        public MeetingRoomApprovalManagement(string siteUrl, string listName, SPListItem currentItem, SPWeb currentWeb) :
            base(siteUrl, listName, currentItem, currentWeb)
        {
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

                // Trưởng phòng [Hành Chánh Nhân Sự] thực hiện  sau bước DEH
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
