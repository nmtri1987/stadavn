using Microsoft.SharePoint;
using RBVH.Core.SharePoint;
using RBVH.Stada.Intranet.Biz.ApprovalManagement;
using RBVH.Stada.Intranet.Biz.Constants;
using RBVH.Stada.Intranet.Biz.DataAccessLayer;
using RBVH.Stada.Intranet.WebPages.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.MeetingRoomManagementControl
{
    public partial class MeetingRoomListUserControl : ListBaseUserUserControl
    {
        #region Fields
        private RequisitionOfMeetingRoomDAL requisitionOfMeetingRoomDAL;
        private readonly string moduleId = "requisitionofmr";
        #endregion
        protected override void OnInit(EventArgs e)
        {
            try
            {
                this.listUrl = Biz.Constants.StringConstant.RequisitionOfMeetingRoomList.Url;
                this.requisitionOfMeetingRoomDAL = new RequisitionOfMeetingRoomDAL(SPContext.Current.Web.Url);

                base.OnInit(e);
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override bool HavePermissionAddNew()
        {
            return this.IsCurrentEmployeeADUser();
        }

        protected override bool IsMyRequestTabActived()
        {
            return this.IsCurrentEmployeeADUser();
        }

        protected override void InitListOfEmployeePosition_ViewRequestToBeApproved()
        {
            base.InitListOfEmployeePosition_ViewRequestToBeApproved();

            listOfEmployeePosition_RequestToBeApproved.Add(Biz.Constants.EmployeePositionCode.DEH);
            listOfEmployeePosition_RequestToBeApproved.Add(Biz.Constants.EmployeePositionCode.BOD);
            listOfEmployeePosition_RequestToBeApproved.Add(Biz.Constants.EmployeePositionCode.GRL);
        }

        protected override void InitOderByQueryString()
        {
            this.orderByQueryString = string.Format(@"<OrderBy>
	                                                    <FieldRef Name='{0}' Ascending='TRUE'/>
                                                        <FieldRef Name='{1}' Ascending='FALSE'/>
                                                    </OrderBy>", StringConstant.CommonSPListField.ColForSortField, "Created");
        }

        protected override void LoadListOfMyRequests()
        {
            try
            {
                StringBuilder queryStringMyRequestBuilder = new StringBuilder();
                queryStringMyRequestBuilder.Append(this.queryStringMyRequest);
                queryStringMyRequestBuilder.Append(this.orderByQueryString);
                List<Biz.Models.RequisitionOfMeetingRoom> myRequests = requisitionOfMeetingRoomDAL.GetByQuery(queryStringMyRequestBuilder.ToString());
                this.gridMyRquests.DataSource = myRequests;
                this.gridMyRquests.DataBind();

                this.Page.Session[MyRequestsSessionKey + this.moduleId] = myRequests;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override void LoadListOfRequestToBeApproved()
        {
            try
            {
                StringBuilder queryStringRequestsToBeApprovedBuilder = new StringBuilder();
                queryStringRequestsToBeApprovedBuilder.Append(this.queryStringReequestsToBeApproved);
                queryStringRequestsToBeApprovedBuilder.Append(this.orderByQueryString);
                List<Biz.Models.RequisitionOfMeetingRoom> requestsToBeApproved = requisitionOfMeetingRoomDAL.GetByQuery(queryStringRequestsToBeApprovedBuilder.ToString());
                this.gridRequestToBeApproved.DataSource = requestsToBeApproved;
                this.gridRequestToBeApproved.DataBind();

                this.Page.Session[RequestsToBeApprovedSessionKey + this.moduleId] = requestsToBeApproved;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override void LoadListOfRequestByDepartment()
        {
            try
            {
                StringBuilder queryStringRequestsByDepartmentBuilder = new StringBuilder();
                queryStringRequestsByDepartmentBuilder.Append(this.queryStringRequestsByDepartment);
                queryStringRequestsByDepartmentBuilder.Append(this.orderByQueryString);
                List<Biz.Models.RequisitionOfMeetingRoom> requestsByDepartment = requisitionOfMeetingRoomDAL.GetByQuery(queryStringRequestsByDepartmentBuilder.ToString());
                this.gridRequestByDepartment.DataSource = requestsByDepartment;
                this.gridRequestByDepartment.DataBind();

                this.Page.Session[RequestsByDepartmentSessionKey + this.moduleId] = requestsByDepartment;
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override string GetFromDateString(TypeOfDateRange typeOfDateRange = TypeOfDateRange.MonthYear)
        {
            return base.GetFromDateString(TypeOfDateRange.FromTo);
        }

        protected override string GetToDateString(TypeOfDateRange typeOfDateRange = TypeOfDateRange.MonthYear)
        {
            return base.GetToDateString(TypeOfDateRange.FromTo);
        }

        protected override void GridMyRquests_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequisitionOfMeetingRoom;
                    if (dataItem != null)
                    {
                        string meetingLocation = dataItem.MeetingRoomLocation.LookupValue;
                        var litMeetingLocation = e.Row.FindControl("litMeetingLocation") as Literal;
                        litMeetingLocation.Text = meetingLocation;

                        string startTimeStr = dataItem.StartDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litStartTime = e.Row.FindControl("litStartTime") as Literal;
                        litStartTime.Text = startTimeStr;

                        string endTimeStr = dataItem.EndDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litEndTime = e.Row.FindControl("litEndTime") as Literal;
                        litEndTime.Text = endTimeStr;

                        base.GridMyRquests_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override void RequestToBeApproved_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequisitionOfMeetingRoom;
                    if (dataItem != null)
                    {
                        string requestFrom = string.Empty;
                        if (dataItem.CommonCreator != null)
                        {
                            requestFrom = dataItem.CommonCreator.LookupValue;
                        }
                        var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                        litRequestFrom.Text = requestFrom;

                        string departmentName = string.Empty;
                        if (dataItem.CommonDepartment != null && dataItem.CommonDepartment.LookupId > 0)
                        {
                            departmentName = GetDepartmentName(dataItem.CommonDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string meetingLocation = string.Empty;
                        if (dataItem.MeetingRoomLocation != null)
                        {
                            meetingLocation = dataItem.MeetingRoomLocation.LookupValue;
                        }
                        var litMeetingLocation = e.Row.FindControl("litMeetingLocation") as Literal;
                        litMeetingLocation.Text = meetingLocation;

                        string startTimeStr = dataItem.StartDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litStartTime = e.Row.FindControl("litStartTime") as Literal;
                        litStartTime.Text = startTimeStr;

                        string endTimeStr = dataItem.EndDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litEndTime = e.Row.FindControl("litEndTime") as Literal;
                        litEndTime.Text = endTimeStr;

                        base.RequestToBeApproved_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override void RequestByDepartment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    var dataItem = e.Row.DataItem as Biz.Models.RequisitionOfMeetingRoom;
                    if (dataItem != null)
                    {
                        string requestFrom = string.Empty;
                        if (dataItem.CommonCreator != null)
                        {
                            requestFrom = dataItem.CommonCreator.LookupValue;
                        }
                        var litRequestFrom = e.Row.FindControl("litRequestFrom") as Literal;
                        litRequestFrom.Text = requestFrom;

                        string departmentName = string.Empty;
                        if (dataItem.CommonDepartment != null && dataItem.CommonDepartment.LookupId > 0)
                        {
                            departmentName = GetDepartmentName(dataItem.CommonDepartment.LookupId);
                        }
                        var litDepartment = e.Row.FindControl("litDepartment") as Literal;
                        litDepartment.Text = departmentName;

                        string meetingLocation = string.Empty;
                        if (dataItem.MeetingRoomLocation != null)
                        {
                            meetingLocation = dataItem.MeetingRoomLocation.LookupValue;
                        }
                        var litMeetingLocation = e.Row.FindControl("litMeetingLocation") as Literal;
                        litMeetingLocation.Text = meetingLocation;

                        string startTimeStr = dataItem.StartDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litStartTime = e.Row.FindControl("litStartTime") as Literal;
                        litStartTime.Text = startTimeStr;

                        string endTimeStr = dataItem.EndDate.ToString(StringConstant.DateFormatddMMyyyyHHmm);
                        var litEndTime = e.Row.FindControl("litEndTime") as Literal;
                        litEndTime.Text = endTimeStr;

                        base.RequestByDepartment_RowDataBound(sender, e);
                    }
                }
            }
            catch (Exception ex)
            {
                ULSLogging.LogError(ex);
                ULSLogging.LogMessageToFile($"-- Error occurs on MeetingRoomListUserControl: {ex.Message}");
            }
        }

        protected override string BuildQueryStringRequestsByDepartmentForBOD()
        {
            var queryString = "<Where><Eq><FieldRef Name='ID'/><Value Type='Number'>0</Value></Eq></Where>";

            int departmentId = 0;
            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            queryString = string.Format(@"<And>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{0}'/>
                                                    <Value Type='DateTime' IncludeTimeValue='False'>{1}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{2}'/>
                                                    <Value Type='DateTime' IncludeTimeValue='False'>{3}</Value>
                                                </Geq>
                                            </And>
                                            <Eq>
                                                <FieldRef Name='{4}' LookupId='True'/>
                                                <Value Type='Lookup'>{5}</Value>
                                            </Eq>
                                        </And>", StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate, this.GetToDateString(),
                                            StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate, this.GetFromDateString(),
                                            ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId);

            if (departmentId > 0)
            {
                queryString = $"<And>{queryString}<Eq><FieldRef Name='{ApprovalFields.CommonDepartment}' LookupId='True'/><Value Type='Lookup'>{departmentId}</Value></Eq></And>";
            }

            queryString = $"<Where>{queryString}</Where>";

            return queryString;
        }

        protected override string BuildQueryStringRequestsByDepartmentForAMDHR()
        {
            var queryString = "<Where><Eq><FieldRef Name='ID'/><Value Type='Number'>0</Value></Eq></Where>";

            int departmentId = 0;

            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            queryString = string.Format(@"<And>
                                            <And>
                                                <And>
                                                    <Leq>
                                                        <FieldRef Name='{0}'/>
                                                        <Value Type='DateTime' IncludeTimeValue='False'>{1}</Value>
                                                    </Leq>
                                                    <Geq>
                                                        <FieldRef Name='{2}'/>
                                                        <Value Type='DateTime' IncludeTimeValue='False'>{3}</Value>
                                                    </Geq>
                                                </And>            
                                                <Eq>
                                                    <FieldRef Name='{4}' />
                                                    <Value Type='Text'>{5}</Value>
                                                </Eq>
                                            </And>
                                            <Eq>
                                                <FieldRef Name='{6}' LookupId='True' />
                                                <Value Type='Lookup'>{7}</Value>
                                            </Eq>
                                        </And>", StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate, this.GetToDateString(),
                                                StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate, this.GetFromDateString(),
                                                ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved,
                                                ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId);
            
            if (departmentId > 0)
            {
                queryString = string.Format(@"<And>
                                                <And>
                                                    <And>
                                                        <And>
                                                            <Or>
                                                                <Eq>
                                                                    <FieldRef Name='{0}'/>
                                                                    <Value Type='Text'>{1}</Value>
                                                                </Eq>
                                                                <Eq>
                                                                    <FieldRef Name='{0}'/>
                                                                    <Value Type='Text'>{2}</Value>
                                                                </Eq>
                                                            </Or>
                                                            <Eq>
                                                                <FieldRef Name='{3}' LookupId='True'/>
                                                                <Value Type='Lookup'>{4}</Value>
                                                            </Eq>
                                                        </And>
                                                        <Geq>
                                                            <FieldRef Name='{5}'/>
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{6}</Value>
                                                        </Geq>
                                                    </And>
                                                    <Leq>
                                                        <FieldRef Name='{7}'/>
                                                        <Value Type='DateTime' IncludeTimeValue='False'>{8}</Value>
                                                    </Leq>
                                                </And>
                                                <Eq>
                                                    <FieldRef Name='{9}' LookupId='True'/>
                                                    <Value Type='Lookup'>{10}</Value>
                                                </Eq>
                                            </And>", ApprovalFields.WFStatus, StringConstant.ApprovalStatus.Approved, StringConstant.ApprovalStatus.Completed,
                                                ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId,
                                                StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate, this.GetFromDateString(),
                                                StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate, this.GetToDateString(),
                                                ApprovalFields.CommonDepartment, departmentId);
            }

            queryString = $"<Where>{queryString}</Where>";

            return queryString;
        }

        protected override string BuildQueryStringRequestsByDepartmentForDEH()
        {
            var queryString = string.Format(@"<Where>
                                                <And>
                                                    <And>
                                                        <And>
                                                            <Eq>
                                                                <FieldRef Name='{0}' LookupId='True'/>
                                                                <Value Type='Lookup'>{1}</Value>
                                                            </Eq>
                                                            <Eq>
                                                                <FieldRef Name='{2}' LookupId='True'/>
                                                                <Value Type='Lookup'>{3}</Value>
                                                            </Eq>
                                                        </And>
                                                        <Geq>
                                                            <FieldRef Name='{4}'/>
                                                            <Value Type='DateTime' IncludeTimeValue='False'>{5}</Value>
                                                        </Geq>
                                                    </And>
                                                    <Leq>
                                                        <FieldRef Name='{6}'/>
                                                        <Value Type='DateTime' IncludeTimeValue='False'>{7}</Value>
                                                    </Leq>
                                                </And>
                                            </Where>", ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId,
                                                    ApprovalFields.CommonDepartment, this.CurrentEmployeeDepartmentId,
                                                    StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate, this.GetFromDateString(),
                                                    StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate, this.GetToDateString());

            return queryString;
        }

        protected override string BuildQueryStringRequestsByDepartmentForDEHHR()
        {
            var queryString = "<Where><Eq><FieldRef Name='ID'/><Value Type='Number'>0</Value></Eq></Where>";

            int departmentId = 0;

            if (this.Page.Request[DepartmentParamName] != null)
            {
                int.TryParse(this.Page.Request[DepartmentParamName], out departmentId);
            }

            queryString = string.Format(@"<And>
                                            <And>
                                                <Leq>
                                                    <FieldRef Name='{0}'/>
                                                    <Value Type='DateTime' IncludeTimeValue='False'>{1}</Value>
                                                </Leq>
                                                <Geq>
                                                    <FieldRef Name='{2}'/>
                                                    <Value Type='DateTime' IncludeTimeValue='False'>{3}</Value>
                                                </Geq>
                                            </And>
                                            <Eq>
                                                <FieldRef Name='{4}' LookupId='True'/>
                                                <Value Type='Lookup'>{5}</Value>
                                            </Eq>
                                        </And>", StringConstant.RequisitionOfMeetingRoomList.Fields.StartDate, this.GetToDateString(),
                                            StringConstant.RequisitionOfMeetingRoomList.Fields.EndDate, this.GetFromDateString(),
                                            ApprovalFields.CommonLocation, this.CurrentEmployeeLocationId);

            if (departmentId > 0)
            {
                queryString = $"<And>{queryString}<Eq><FieldRef Name='{ApprovalFields.CommonDepartment}' LookupId='True'/><Value Type='Lookup'>{departmentId}</Value></Eq></And>";
            }

            queryString = $"<Where>{queryString}</Where>";

            return queryString;
        }
    }
}
