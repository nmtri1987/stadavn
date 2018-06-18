<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/BusinessTripModule/BusinessTripRequestForm.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />

    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/BusinessTripModule/BusinessTripRequest.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="border-container right-container" id="business-trip-container">
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title l-required">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_BusinessTripTypeTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <label>
                    <input type="radio" name="Domestic" id="rb-internal-trip" />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_BusinessTripTypeInternalTitle%>" />
                </label>
                <br />
                <label>
                    <input type="radio" name="Domestic" id="rb-external-trip" />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_BusinessTripTypeExternalTitle%>" />
                </label>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title l-required">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_EmployeeListTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <%--<div id="js-grid-employee-list"></div>--%>
                <div style="margin-bottom:10px;">
                    <select id="cbo-employee-list"></select>
                    <span class="glyphicon glyphicon-plus" id="btn-add-employee"></span>
                </div>
                <div id="js-grid-employee-list"></div>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <%--<div class="row">
            <div class="col-sm-2 col-md-2">
            </div>
            <div class="col-sm-7 col-md-7">
                <div id="js-grid-employee-list"></div>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>--%>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title l-required">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_PurposeTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <textarea class="form-control t-required" id="txt-purpose" rows="3" maxlength="254"></textarea>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title l-required">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_ScheduleListTitle%>" />
                </span>
            </div>
            <div class="col-sm-10 col-md-10">
                <div id="js-grid-schedule"></div>
            </div>
            <%--<div class="col-sm-2 col-md-2"></div>--%>
        </div>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_HotelBookingTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <label>
                    <input type="checkbox" id="cb-hotel-booking" />
                    <asp:Literal runat="server" Text="Có/Yes" />
                </label>
                &nbsp;&nbsp;
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row" id="paid-by-container">
            <div class="col-sm-2 col-md-2">
                <span class="sub-title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_PaidByTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <input type="text" class="form-control" id="txt-paid-by" maxlength="254" />
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row" id="other-service-container">
            <div class="col-sm-2 col-md-2">
                <span class="sub-title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_OtherServiceTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <input type="text" class="form-control" id="txt-other-service" maxlength="254" />
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <label>
                    <input type="radio" name="TransportationType" id="rb-company-transport" value="company" checked />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeCompanyTitle%>" />
                </label>
                <br />
                <label>
                    <input type="radio" name="TransportationType" id="rb-private-transport" value="private"/>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypePrivateTitle%>" />
                </label>
                <br />
                <label>
                    <input type="radio" name="TransportationType" id="rb-other-transport" value="others"/>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeOthersTitle%>" />&nbsp;<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeOthersDetailsTitle%>" />
                    &nbsp;&nbsp;
                    <input type="text" class="form-control t-required" id="txt-other-transport-detail" maxlength="100" />
                </label>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_OtherRequestsTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <label style="width: 100%;">
                    <input type="checkbox" id="cb-visa-request" />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_VisaRequestTitle%>" />
                </label>
                <br />
                <label style="width: 100%;">
                    <input type="checkbox" id="cb-cash-cheque-request" />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_CashOrChequeRequestTitle%>" />
                    &nbsp;&nbsp;
                    <input type="text" class="form-control t-required" id="txt-cash-cheque" maxlength="100" />
                </label>
                <br />
                <label style="width: 100%;">
                    <input type="checkbox" id="cb-other-request" />
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_OtherRequestTitle%>" />
                    &nbsp;&nbsp;
                    <input type="text" class="form-control t-required" id="txt-other-request" maxlength="254" />
                </label>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row" id="high-priority-container" style="display: none;">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_HighPriorityTitle%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <input type="checkbox" id="cb-high-priority" />
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row" id="comment-container" style="display: none;">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,CommonComment%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <div id="lbl-comment">
                </div>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>
        <div class="row" id="your-comment-container" style="display: none;">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,CommonYourComment%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <textarea class="form-control" id="txt-comment" rows="3" maxlength="254"></textarea>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>

        <div class="row" id="approval-status-container" style="display: none;">
            <div class="col-sm-2 col-md-2">
                <span class="title">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_ApprovalStatus%>" />
                </span>
            </div>
            <div class="col-sm-7 col-md-7">
                <div id="approval-status-val"></div>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>

        <div class="row" id="error-msg-container" style="display: none;">
            <div class="col-sm-2 col-md-2">
            </div>
            <div class="col-sm-7 col-md-7">
                <div id="error-msg"></div>
            </div>
            <div class="col-sm-2 col-md-2"></div>
        </div>

        <div class="row footer">
            <button id="btn-business-trip-approve" class="btn btn-success mr15" style="display: none;">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>" />
            </button>
            <button id="btn-business-trip-reject" class="btn btn-primary mr15" style="display: none;">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RejectButton%>" />
            </button>
            <button id="btn-business-trip-save" class="btn btn-primary mr15">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,SaveButton%>" />
            </button>
            <button id="btn-business-trip-cancel" class="btn btn-default">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CancelButton%>" />
            </button>
        </div>
        <div style="display:none;">
            <button id="loadApprovalHistory" class="btn btn-default">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApprovalHistoryButton%>" />
            </button>
            <br />
            <div id="approvalHistoryContainer"></div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            var settings = {
                Controls:
                {
                    BusinessTripContainerSelector: "#business-trip-container",
                    InternalTripTypeControlSelector: "#rb-internal-trip",
                    ExternalTripTypeControlSelector: "#rb-external-trip",
                    EmployeeListGridControlSelector: "#js-grid-employee-list",
                    EmployeeListControlSelector: "#cbo-employee-list",
                    AddEmployeeControlSelector: "#btn-add-employee",
                    PurposeControlSelector: "#txt-purpose",
                    ScheduleGridControlSelector: "#js-grid-schedule",
                    HotelBookingControlSelector: "#cb-hotel-booking",
                    PaidByContainerSelector: "#paid-by-container",
                    PaidByControlSelector: "#txt-paid-by",
                    OtherServiceContainerSelector: "#other-service-container",
                    OtherServiceControlSelector: "#txt-other-service",
                    TransportationTypeControlSelector: "transportation-type",
                    CompanyTransportOptionControlSelector: "#rb-company-transport", // Transportation
                    PrivateTransportOptionControlSelector: "#rb-private-transport", // Transportation
                    OtherTransportOptionControlSelector: "#rb-other-transport",     // Transportation
                    OtherTransportDetailControlSelector: "#txt-other-transport-detail",     // Other Transportation
                    VisaRequestOptionControlSelector: "#cb-visa-request",
                    CashChequeRequestOptionControlSelector: "#cb-cash-cheque-request",
                    OtherRequestOptionControlSelector: "#cb-other-request",
                    CashOrChequeControlSelector: "#txt-cash-cheque",
                    OtherRequestControlSelector: "#txt-other-request",
                    CommentContainerSelector: "#comment-container",
                    CommentControlSelector: "#lbl-comment",
                    YourCommentContainerSelector: "#your-comment-container",
                    YourCommentControlSelector: "#txt-comment",
                    HighPriorityContainerSelector: "#high-priority-container",
                    HighPriorityControlSelector: "#cb-high-priority",
                    ApproveControlSelector: "#btn-business-trip-approve",
                    RejectControlSelector: "#btn-business-trip-reject",
                    SaveControlSelector: "#btn-business-trip-save",
                    CancelControlSelector: "#btn-business-trip-cancel",
                    ApproveStatusValSelector:"#approval-status-val",
                    ApproveStatusContainerSelector: "#approval-status-container",
                    ApprovalHistoryButtonSelector:"#loadApprovalHistory",
                    ApprovalHistoryContainerSelector: "#approvalHistoryContainer",
                    ErrorMsgContainerSelector: "#error-msg-container",
                    ErrorMsgSelector: "#error-msg",
                },
                EmployeeGrid:
                {
                    Fields: [],
                    ColumnTitles:
                    {
                        GridColumn_FullName: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_EmployeeFullName%>" />',
                        GridColumn_Code: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_EmployeeCode%>" />',
                        GridColumn_Department: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_EmployeeDept%>" />',
                    }
                },
                ScheduleGrid:
                {
                    Fields: [],
                    ColumnTitles:
                    {
                        GridColumn_DepartTime: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_DepartTimeTitle%>" />',
                        GridColumn_DepartDate: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_DepartDateTitle%>" />',
                        GridColumn_FlightName: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_FlightNameTitle%>" />',
                        GridColumn_City: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_CityTitle%>" />',
                        GridColumn_Country:  "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,BusinessTripManagement_CountryTitle%>' />",
                        GridColumn_ContactCompany: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,BusinessTripManagement_ContactCompanyTitle%>' />",
                        GridColumn_ContactPhone: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_ContactPhoneTitle%>" />',
                        GridColumn_OtherSchedule: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,BusinessTripManagement_OtherScheduleTitle%>" />',
                    }
                },
                ResourceText:
                {
                    ConfirmDeleteMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_ConfirmDeleteMessage%>" />',
                    PleaseInputDataMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_PleaseInputDataMessage%>" />',
                    QuantityWrongMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_QuantityIsWrongMessage%>" />',
                    CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                    CompanyTransportationTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeCompanyTitle%>' />",
                    PrivateTransportationTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypePrivateTitle%>' />",
                    OtherTransportationTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,BusinessTripManagement_TransportationTypeOthersTitle%>' />",
                    ApprovalStatusTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_ApprovalStatusTitle%>' />",
                    PostedByTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_PostedByTitle%>' />",
                    DateTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_DateTitle%>' />",
                    CommentTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_CommentTitle%>' />",
                    NoDataAvaibleMsg: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_NoDataAvaibleMsg%>' />",
                    ApprovalStatus_Approved: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,ApprovalStatus_Approved%>' />",
                    ApprovalStatus_Rejected: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,ApprovalStatus_Rejected%>' />",
                    RequestExpiredMsgFormat: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestExpiredMsgFormat%>' />",
                }
            };

            businessTripRequestFormInstance = new RBVH.Stada.WebPages.pages.BusinessTripRequest(settings);
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripRequest_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripRequest_PageTitleArea%>" />
</asp:Content>
