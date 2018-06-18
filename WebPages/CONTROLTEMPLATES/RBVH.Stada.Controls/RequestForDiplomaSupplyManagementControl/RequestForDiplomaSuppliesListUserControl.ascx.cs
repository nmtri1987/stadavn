using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using static RBVH.Stada.Intranet.Biz.Constants.StringConstant;
using BizConstants = RBVH.Stada.Intranet.Biz.Constants;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DiplomaManagementControl
{
    public partial class RequestForDiplomaSuppliesListUserControl : ListBaseUserUserControl
    {
        #region Attributes

        private RequestForDiplomaSupplyDAL requestForDiplomaSupplyDAL;

        #endregion

        #region Overrides

        protected override void OnInit(EventArgs e)
        {
            try
            {
                // Set current list url.
                this.listUrl = RequestForDiplomaSuppliesList.Url;
                this.requestForDiplomaSupplyDAL = new RequestForDiplomaSupplyDAL(SPContext.Current.Web.Url);

                base.OnInit(e);
            }
            catch(Exception ex)
            {
                ULSLogging.LogError(ex);
            }
        }

        protected override void InitListOfEmployeePosition_AddNewItem()
        {
            base.InitListOfEmployeePosition_AddNewItem();

            // Head Of Department
            listOfEmployeePosition_AddNewItem.Add(BizConstants.EmployeePositionCode.DEH);
        }

        protected override void InitListOfEmployeePosition_ViewMyRequest()
        {
            base.InitListOfEmployeePosition_ViewMyRequest();

            // Head Of Department
            listOfEmployeePosition_ViewMyRequest.Add(BizConstants.EmployeePositionCode.DEH);
        }

        protected override void InitListOfEmployeePosition_ViewRequestToBeApproved()
        {
            base.InitListOfEmployeePosition_ViewRequestToBeApproved();

            // Truong Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.DEH);
            // Giam Doc
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.BOD);
            // Truong Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.DEH);
            // Feedback 14.11.2017: Pho Phong
            listOfEmployeePosition_RequestToBeApproved.Add(BizConstants.EmployeePositionCode.GRL);
        }

        protected override void LoadListOfMyRequests()
        {
            try
            {
                StringBuilder queryStringMyRequestBuilder = new StringBuilder();
                queryStringMyRequestBuilder.Append(this.queryStringMyRequest);
                queryStringMyRequestBuilder.Append(this.orderByQueryString);

                List<Biz.Models.RequestForDiplomaSupply> myRequests = this.requestForDiplomaSupplyDAL.GetByQuery(queryStringMyRequestBuilder.ToString());
                this.gridMyRquests.DataSource = myRequests;
                this.gridMyRquests.DataBind();

                this.Page.Session[MyRequestsSessionKey] = myRequests;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override void LoadListOfRequestToBeApproved()
        {
            try
            {
                StringBuilder queryStringRequestsToBeApprovedBuilder = new StringBuilder();
                queryStringRequestsToBeApprovedBuilder.Append(this.queryStringReequestsToBeApproved);
                queryStringRequestsToBeApprovedBuilder.Append(this.orderByQueryString);
                List<Biz.Models.RequestForDiplomaSupply> requestsToBeApproved = this.requestForDiplomaSupplyDAL.GetByQuery(queryStringRequestsToBeApprovedBuilder.ToString());
                this.gridRequestToBeApproved.DataSource = requestsToBeApproved;
                this.gridRequestToBeApproved.DataBind();

                this.Page.Session[RequestsToBeApprovedSessionKey] = requestsToBeApproved;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override void LoadListOfRequestByDepartment()
        {
            try
            {
                StringBuilder queryStringRequestsByDepartmentBuilder = new StringBuilder();
                queryStringRequestsByDepartmentBuilder.Append(this.queryStringRequestsByDepartment);
                queryStringRequestsByDepartmentBuilder.Append(this.orderByQueryString);
                List<Biz.Models.RequestForDiplomaSupply> requestsByDepartment = this.requestForDiplomaSupplyDAL.GetByQuery(queryStringRequestsByDepartmentBuilder.ToString());
                this.gridRequestByDepartment.DataSource = requestsByDepartment;
                this.gridRequestByDepartment.DataBind();

                this.Page.Session[RequestsByDepartmentSessionKey] = requestsByDepartment;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override void GridMyRquests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequestForDiplomaSupply;
                    if (dataItem != null)
                    {
                        //string title = dataItem.Title != null ? dataItem.Title : string.Empty;
                        //var litTitle = e.Row.FindControl("litTitle") as Literal;
                        //litTitle.Text = title;

                        string departmentName = string.Empty;
                        if (dataItem.CommonDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.CommonDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string employeeName = dataItem.Position != null ? dataItem.EmployeeName : string.Empty;
                        var litEmpoyeeName = e.Row.FindControl("litEmpoyeeName") as Literal;
                        litEmpoyeeName.Text = employeeName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        base.GridMyRquests_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override void RequestToBeApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequestForDiplomaSupply;
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
                        if (dataItem.CommonDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.CommonDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string employeeName = dataItem.Position != null ? dataItem.EmployeeName : string.Empty;
                        var litEmpoyeeName = e.Row.FindControl("litEmpoyeeName") as Literal;
                        litEmpoyeeName.Text = employeeName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        base.RequestToBeApproved_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override void RequestByDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequestForDiplomaSupply;
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
                        if (dataItem.CommonDepartment != null)
                        {
                            departmentName = GetDepartmentName(dataItem.CommonDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string employeeName = dataItem.Position != null ? dataItem.EmployeeName : string.Empty;
                        var litEmpoyeeName = e.Row.FindControl("litEmpoyeeName") as Literal;
                        litEmpoyeeName.Text = employeeName;

                        string position = dataItem.Position != null ? dataItem.Position : string.Empty;
                        var litPosition = e.Row.FindControl("litPosition") as Literal;
                        litPosition.Text = position;

                        base.RequestByDepartment_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on RequestForDiplomaSuppliesListUserControl: {ex.Message}");
            }
        }

        protected override bool IsRequestToBeApprovedTabActived()
        {
            var res = false;

            if (ListOfEmployeePosition_ViewRequestToBeApproved != null && ListOfEmployeePosition_ViewRequestToBeApproved.Length > 0)
            {
                foreach (var employeePositionCode in this.ListOfEmployeePosition_ViewRequestToBeApproved)
                {
                    if (string.Compare(employeePositionCode, this.CurrentEmployeePositionCode, true) == 0)
                    {
                        // DEH
                        if (string.Compare(this.CurrentEmployeePositionCode, BizConstants.EmployeePositionCode.DEH, true) == 0)
                        {
                            // Nếu là Trưởng Phòng - Hành Chánh thì mới show tab này.
                            // Vì Trưởng Phòng - Hánh Chánh có xử lý phê duyệt còn các Trưởng Phòng khác chỉ gửi yêu cầu.
                            if (string.Compare(this.CurrentEmployeeDepartmentCode, BizConstants.DepartmentCode.HR, true) == 0)
                            {
                                res = true;
                                break;
                            }
                        }
                        else // BOD
                        {
                            res = true;
                            break;
                        }
                    }
                }
            }

            return res;
        }
        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
