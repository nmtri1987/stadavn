<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
     <!-- BEGIN: JS Grid -->
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/OvertimeModule/default.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
    
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/common/Utilities.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/OvertimeRequest.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="width: 100%;" id="tbOvertimeMaster">
        <thead>
            <tr class="row">
                <th class="col-md-1"></th>
                <th class="col-md-3"></th>
                <th class="col-md-1"></th>
                <th class="col-md-3"></th>
                <th class="col-md-1"></th>
                <th class="col-md-3"></th>
            </tr>
        </thead>
        <tbody>
             <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Requester%>" />
                </td>
                <td >
                    <span class="requester"></span>
                </td>
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Department%>" />
                </td>
                <td>
                    <select class="form-control overtime-combobox" id="cbDepartment" disabled>
                    </select>
                </td>
                 <td>
                </td>
                 <td>
                </td>
            </tr>
            <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Date%>" />
                </td>
                <td>
                    <div class="input-append  date inner-addon right-addon" style="width: 100%" id="dpFromDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                        <input class="span2 form-control overtime-calendar" size="16" type="text" readonly id="txtFromDate" disabled />
                    </div>
                </td>
                <td >
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Location%>" />
                </td>
                <td>
                    <span class="factoryLocation"></span>
                </td>
                 <td >
                </td>
                 <td>
                </td>
            </tr>
            <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_SumEmployee%>" />
                </td>
                <td>
                    <input type="text" class="form-control overtime-textbox" id="txtQuantity" disabled/>
                </td>
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_SumMeal%>" />
                </td>
                <td>
                    <input type="text" class="form-control overtime-textbox" id="txtServing" />
                </td>
                 <td >
                </td>
                 <td >
                </td>
            </tr>
            <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Place%>" />
                </td>
                <td>
                    <input type="text" class="form-control overtime-textbox" id="txtPlace" />
                </td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_OtherRequirement%>" />
                </td>
                <td colspan="5">
                    <textarea class="form-control" placeholer="Message" id="txtOtherRequest">
                    </textarea>
                </td>
            </tr>
            <tr class="row">
                <td colspan="6">
                    <br/>
                    <!--Grid Here-->
                    <div id="jsGridOvertimeRequest"></div>
                </td>
            </tr>
            <tr class="row">
                <td>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_Comments%>" />
                </td>
                <td colspan="5" style="padding-top: 10px;">
                    <textarea class="form-control" placeholer="Message" id="txt-comments">
                    </textarea>
                </td>
            </tr>
            <tr class="row" id="error-msg-container" style="display: none;">
                <td>&nbsp</td>
                <td colspan="5" style="padding-top: 10px;" id="error-msg"></td>
            </tr>
            <tr class="row">
                <td colspan="6">
                    <br/>
                    <div class="row" style=" float: right;">
                        <button id="btnOvertimeRequestSave" class="btn btn-success" style="margin-right: 15px;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>"  />
                        </button>
                        <button id="btnOvertimeRequestReject" class="btn btn-primary" style="margin-right: 15px;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RejectButton%>"  />
                        </button>
                        <button id="btnOvertimeRequestCancel" class="btn btn-default overtime-cancel-btn">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CancelButton%>" />
                        </button>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>
    
    <script type="text/javascript">
        $(document).ready(function () {
            var settings = {
                FromDateControlSelector: '#dpFromDate .span2',
                ToDateControlSelector: '#dpToDate .span2',
                GridOvertimeRequestControlSelector: '#jsGridOvertimeRequest',
                DepartmentControlSelector: '#cbDepartment',
                LocationControlSelector: '#cbLocation',
                PlaceControlSelector: '#txtPlace',
                QuantityControlSelector: '#txtQuantity',
                ServingControlSelector: '#txtServing',
                OtherRequestControlSelector: '#txtOtherRequest',
                CommentsControlSelector: '#txt-comments',
                SaveControlSelector: '#btnOvertimeRequestSave',
                RejectControlSelector: '#btnOvertimeRequestReject',
                CancelControlSelector: '#btnOvertimeRequestCancel',
                RequesterControlSelector: '.requester',
                FactoryLocationControlSelector: '.factoryLocation',
                ApprovedByControlSelector: '.approver',
                ErrorMsgContainerSelector: "#error-msg-container",
                ErrorMsgSelector: "#error-msg",
                InvalidTwoDatesMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_InvalidTwoDates%>" />',
                ConfirmDeleteMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_DeleteRow%>" />',
                InvalidHoursMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_InvalidHours%>" />',
                RequiredFieldsErrorMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_RequiredFieldsMessage%>" />',
                ApprovalButtonTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>" />',
                RequestExpiredMsgFormat: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestExpiredMsgFormat%>' />",
                DataChanged_Msg: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DataChanged_Msg%>' />",
                ListPageUrl: '//{0}/SitePages/OvertimeApprovalList.aspx',
                View: 'Approval',
                Grid:
                {
                    Fields: [],
                    ColumnTitles:
                    {
                        GridColumn_Order: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_OrderNumber%>" />',
                        GridColumn_FullName: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,EmployeeInfo_FullName%>" />',
                        GridColumn_EmployeeID: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_EmployeeID%>" />',
                        GridColumn_WorkingHour: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_WorkingHour%>" />',
                        GridColumn_OvertimeHour: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_OvertimeHour%>" />',
                        GridColumn_OvertimeHourFrom: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_OvertimeHourFrom%>" />',
                        GridColumn_OvertimeHourTo: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_OvertimeHourTo%>" />',
                        GridColumn_Task: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_Task%>" />',
                        GridColumn_TransportAtHM: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_TransportAtHM%>" />',
                        GridColumn_TransportAtKD: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,OvertimeDetail_TransportAtKD%>" />',
                        GridColumn_EmployeeSignature: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,OvertimeDetail_EmployeeSignature%>' />"
                    }
                },
                Modal:
                {
                    SaveButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeDataChanged_Modal_SaveButton%>" />',
                    ReloadButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeDataChanged_Modal_ReloadButton%>" />',
                    CloseButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeDataChanged_Modal_CloseButton%>" />',
                    Title: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeDataChanged_Confirm%>" />'
                },
            };

            if (typeof overtimeApprovalFormInstance === "undefined") {
                overtimeApprovalFormInstance = new RBVH.Stada.WebPages.pages.OvertimeRequest(settings);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
   <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeRequest_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,OvertimeRequest_PageTitleArea%>" />
</asp:Content>