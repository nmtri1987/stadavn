using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using BizConstants = RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RecruitmentManagementControl
{
    /// <summary>
    /// RecruitmentListUserControl
    /// </summary>
    public partial class RecruitmentListUserControl : ListBaseUserUserControl
    {
        #region Constants
        /// <summary>
        /// glyphicon glyphicon-ok
        /// </summary>
        private const string RequestIsValid_CssClass = "glyphicon glyphicon-ok";

        /// <summary>
        /// glyphicon glyphicon-remove
        /// </summary>
        private const string RequestIsInvalid_CssClass = "glyphicon glyphicon-remove";

        private const string InvalidRequest_ColorRow = "#fff7e6";
        #endregion

        #region Fields

        private EmployeeRequirementSheetDAL employeeRequirementSheetDAL;

        private RecruitmentTeamDAL recruitmentTeamDAL;

        #endregion

        #region overrides

        protected override void InitListOfEmployeePosition_AddNewItem()
        {
            base.InitListOfEmployeePosition_AddNewItem();

            // Van Thu
            listOfEmployeePosition_AddNewItem.Add(BizConstants.EmployeePositionCode.AMD);
        }

        protected override void InitListOfEmployeePosition_ViewMyRequest()
        {
            base.InitListOfEmployeePosition_ViewMyRequest();

            // Van Thu
            listOfEmployeePosition_ViewMyRequest.Add(BizConstants.EmployeePositionCode.AMD);
        }

        protected override void InitListOfEmployeePosition_ViewRequestToBeApproved()
        {
            base.InitListOfEmployeePosition_ViewRequestToBeApproved();

            // Truong Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.DEH);
            // Feedback 14.11.2017 Pho Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.GRL);
            // Quan Ly Truc Tiep
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.DRM);
            // Giam Doc
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.BOD);
        }

        protected override bool HavePermissionAddNew()
        {
            var res = base.HavePermissionAddNew();

            if (res)
            {
                // Neu khong phai account AD
                if (string.Compare(EmployeeType.ADUser, this.CurrentEmployeeType, true) != 0)
                {
                    res = false;
                }
            }

            return res;
        }

        protected override void OnInit(EventArgs e)
        {
            try
            {
                // Set current list url.
                this.listUrl = EmployeeRequirementSheetsList.Url;
                this.employeeRequirementSheetDAL = new EmployeeRequirementSheetDAL(SPContext.Current.Web.Url);
                this.recruitmentTeamDAL = new RecruitmentTeamDAL(SPContext.Current.Web.Url);

                base.OnInit(e);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        protected override bool IsRequestByDepartmentActived()
        {
            var res = base.IsRequestByDepartmentActived();
            if (res == false)
            {
                res = this.IsCurrrentEmployeeRecruitmentTeam();
            }

            return res;
        }

        protected override void InitQueryStringRequestsByDepartment()
        {
            if (this.IsCurrrentEmployeeRecruitmentTeam())
            {
                this.queryStringRequestsByDepartment = BuildQueryStringRequestsByDepartmentForAMDHR();
            }
            else
            {
                base.InitQueryStringRequestsByDepartment();
            }
        }

        protected override void LoadListOfDepartment()
        {
            base.LoadListOfDepartment();

            if (this.IsCurrrentEmployeeRecruitmentTeam())
            {
                this.ddlDepartments.SelectedValue = this.Page.Request[DepartmentParamName];
                this.ddlDepartments.Enabled = true;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// GridMyRquests_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void GridMyRquests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.EmployeeRequirementSheet;
                    if (dataItem != null)
                    {
                        //string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        //var litTitle = e.Row.FindControl("litTitle") as Literal;
                        //litTitle.Text = title;

                        string departmentName = string.Empty;
                        if (dataItem.RecruitmentDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.RecruitmentDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        string quantity = dataItem.Quantity.ToString();
                        var litQuantity = e.Row.FindControl("litQuantity") as Literal;
                        litQuantity.Text = quantity;

                        #region ADD. 2017.10.9. TFS#1594
                        var litIsValid = e.Row.FindControl("litIsValid") as Label;
                        if (dataItem.IsValidRequest)
                        {
                            litIsValid.CssClass = RequestIsValid_CssClass;
                        }
                        else
                        {
                            litIsValid.CssClass = RequestIsInvalid_CssClass;
                            e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml(InvalidRequest_ColorRow);
                        }
                        #endregion

                        base.GridMyRquests_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// RequestToBeApproved_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RequestToBeApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.EmployeeRequirementSheet;
                    if (dataItem != null)
                    {
                        //string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        //var litTitle = e.Row.FindControl("litTitle") as Literal;
                        //litTitle.Text = title;

                        string requestFrom = string.Empty;
                        if (dataItem.CommonCreator != null)
                        {
                            requestFrom = dataItem.CommonCreator.LookupValue;
                        }
                        var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                        litRequestFrom.Text = requestFrom;

                        string departmentName = string.Empty;
                        if (dataItem.RecruitmentDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.RecruitmentDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        string quantity = dataItem.Quantity.ToString();
                        var litQuantity = e.Row.FindControl("litQuantity") as Literal;
                        litQuantity.Text = quantity;

                        #region ADD. 2017.10.9. TFS#1594
                        var litIsValid = e.Row.FindControl("litIsValid") as Label;
                        if (dataItem.IsValidRequest)
                        {
                            litIsValid.CssClass = RequestIsValid_CssClass;
                        }
                        else
                        {
                            litIsValid.CssClass = RequestIsInvalid_CssClass;
                            e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml(InvalidRequest_ColorRow);
                        }
                        #endregion

                        base.RequestToBeApproved_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// RequestByDepartment_RowDataBound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void RequestByDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.EmployeeRequirementSheet;
                    if (dataItem != null)
                    {
                        //string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        //var litTitle = e.Row.FindControl("litTitle") as Literal;
                        //litTitle.Text = title;

                        string requestFrom = string.Empty;
                        if (dataItem.CommonCreator != null)
                        {
                            requestFrom = dataItem.CommonCreator.LookupValue;
                        }
                        var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                        litRequestFrom.Text = requestFrom;

                        string departmentName = string.Empty;
                        if (dataItem.RecruitmentDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.RecruitmentDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        string quantity = dataItem.Quantity.ToString();
                        var litQuantity = e.Row.FindControl("litQuantity") as Literal;
                        litQuantity.Text = quantity;

                        #region ADD. 2017.10.9. TFS#1594
                        var litIsValid = e.Row.FindControl("litIsValid") as Label;
                        if (dataItem.IsValidRequest)
                        {
                            litIsValid.CssClass = RequestIsValid_CssClass;
                        }
                        else
                        {
                            litIsValid.CssClass = RequestIsInvalid_CssClass;
                            e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml(InvalidRequest_ColorRow);
                        }
                        #endregion

                        base.RequestByDepartment_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// LoadMyRequests
        /// </summary>
        protected override void LoadListOfMyRequests()
        {
            try
            {
                StringBuilder queryStringMyRequestBuilder = new StringBuilder();
                queryStringMyRequestBuilder.Append(this.queryStringMyRequest);
                queryStringMyRequestBuilder.Append(this.orderByQueryString);

                List<Biz.Models.EmployeeRequirementSheet> myRequests = employeeRequirementSheetDAL.GetByQuery(queryStringMyRequestBuilder.ToString());
                this.gridMyRquests.DataSource = myRequests;
                this.gridMyRquests.DataBind();

                this.Page.Session[MyRequestsSessionKey] = myRequests;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// LoadRequestsToBeApproved
        /// </summary>
        protected override void LoadListOfRequestToBeApproved()
        {
            try
            {
                StringBuilder queryStringRequestsToBeApprovedBuilder = new StringBuilder();
                queryStringRequestsToBeApprovedBuilder.Append(this.queryStringReequestsToBeApproved);
                queryStringRequestsToBeApprovedBuilder.Append(this.orderByQueryString);
                List<Biz.Models.EmployeeRequirementSheet> requestsToBeApproved = employeeRequirementSheetDAL.GetByQuery(queryStringRequestsToBeApprovedBuilder.ToString());
                this.gridRequestToBeApproved.DataSource = requestsToBeApproved;
                this.gridRequestToBeApproved.DataBind();

                this.Page.Session[RequestsToBeApprovedSessionKey] = requestsToBeApproved;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// LoadListOfRequestByDepartment
        /// </summary>
        protected override void LoadListOfRequestByDepartment()
        {
            try
            {
                StringBuilder queryStringRequestsByDepartmentBuilder = new StringBuilder();
                queryStringRequestsByDepartmentBuilder.Append(this.queryStringRequestsByDepartment);
                queryStringRequestsByDepartmentBuilder.Append(this.orderByQueryString);
                List<Biz.Models.EmployeeRequirementSheet> requestsByDepartment = employeeRequirementSheetDAL.GetByQuery(queryStringRequestsByDepartmentBuilder.ToString());
                this.gridRequestByDepartment.DataSource = requestsByDepartment;
                this.gridRequestByDepartment.DataBind();

                this.Page.Session[RequestsByDepartmentSessionKey] = requestsByDepartment;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }
        }

        /// <summary>
        /// IsDraftOrRejectedOrSubmittedItem
        /// </summary>
        /// <param name="dataItem"></param>
        /// <returns></returns>
        protected override bool IsDraftOrRejectedOrSubmittedItem(object dataItem)
        {
            bool res = false;

            Biz.Models.EmployeeRequirementSheet request = dataItem as Biz.Models.EmployeeRequirementSheet;

            if (request != null)
            {
                string status = request.ApprovalStatus;

                if (string.IsNullOrEmpty(status))
                {
                    res = true;
                }
                else
                {
                    if ((string.Compare(status, BizConstants.Status.Draft, true) == 0) ||
                        (string.Compare(status, BizConstants.Status.Rejected, true) == 0) ||
                        (string.Compare(status, BizConstants.Status.Submitted, true) == 0))
                    {
                        res = true;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// IsCurrrentEmployeeRecruitmentTeam
        /// </summary>
        /// <returns></returns>
        protected bool IsCurrrentEmployeeRecruitmentTeam()
        {
            var res = false;

            try
            {
                string queryString = string.Format(@"<Where>
                                                        <Eq>
                                                            <FieldRef Name='{0}' LookupId='True' />
                                                            <Value Type='Lookup'>{1}</Value>
                                                        </Eq>
                                                    </Where>", RecruitmentTeamList.Fields.Employees, this.CurrentEmployeeInfoObj.ID);

                var recruitmentTeamItems = this.recruitmentTeamDAL.GetByQuery(queryString);
                if (recruitmentTeamItems != null && recruitmentTeamItems.Count > 0)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RecruitmentListUserControl: {ex.Message}");
            }

            return res;
        }

        #endregion
    }
}
