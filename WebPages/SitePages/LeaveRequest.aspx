<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/LeaveModule/default.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/LeaveRequest.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
    <style>
        #ctl00_PlaceHolderMain_dtFrom_dtFromDate:disabled, #ctl00_PlaceHolderMain_dtTo_dtToDate:disabled {
            border-color: #ababab !important;
            color: #464a4c !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="border-container" style="display: inline-block; width: 100%;">
        <table style="width: 100%;" id="tbLeaveRequestContainer">
            <thead>
                <tr class="row">
                    <th class="col-lg-1 col-md-2"></th>
                    <th class="col-lg-3 col-md-3"></th>
                    <th class="col-lg-1 col-md-1"></th>
                    <th class="col-lg-1 col-md-2"></th>
                    <th class="col-lg-3 col-md-3"></th>
                    <th class="col-lg-3 col-md-1"></th>
                </tr>
            </thead>
            <tbody>
                <tr class="row">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_Requester%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <span class="requester"></span>
                        <span id="requester_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                    <td></td>
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_RequestFor%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <select class="form-control" id="cbRequestFor">
                        </select>
                        <span id="cbRequestFor_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                </tr>
                <tr class="row">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_From%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <SharePoint:DateTimeControl ID="dtFrom" runat="server" LocaleId="2057" HoursMode24="True" />
                        <span id="dtFrom_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                    <td></td>
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_To%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <SharePoint:DateTimeControl ID="dtTo" runat="server" LocaleId="2057" HoursMode24="True" />
                        <span id="dtTo_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                </tr>
                <tr class="row">
                    <td class="header pr10">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_LeaveHours%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <input type="text" class="form-control txt-small" id="txtTotalHours" disabled />
                        <span id="txtTotalHours_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                    <td></td>
                    <td class="header pr10">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_TransferWorkTo%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <select class="form-control" id="cbTransferWorkTo"></select>
                        <span id="cbTransferWorkTo_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                </tr>
                <tr class="row">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_Approver%>" />
                        <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                    </td>
                    <td>
                        <span class="approver"></span>
                        <span id="approver_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                    <td></td>
                    <%-- <td class="header" style="padding-right: 8px; display:none">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_AdditionalApprover%>" />
                    </td>--%>
                    <%--Bỏ additional approvers--%>
                    <td style="display:none">
                        <div class="ms-PeoplePicker">
                            <div id="divLeaveAdditionalPplPic" class="cam-peoplepicker-userlookup ms-fullWidth">
                                <span id="spanLeaveAdditionalPplPic"></span>
                                <input type="text" id="UploadLeavePplPic" style="display: inline-block; border: medium none; width: 100%;" runat="server" />
                            </div>
                            <div id="divLeaveAdditionalPplPicSearch" class="cam-peoplepicker-usersearch ms-emphasisBorder"></div>
                            <input type="hidden" id="hdnLeavePplPic" runat="server" />
                            <div class="ms-core-form-line">
                                <label id="lblLeaveAdditionalPplPicEnteredData" runat="server"></label>
                            </div>
                        </div>
                        <span id="addtionalApprover_Error" class="ms-formvalidation ms-csrformvalidation" style="margin-top:0px;"></span>
                    </td>
                    <td id="approval-status-header" class="header" style="padding-right: 8px; display:none;">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_ApprovalStatus%>" />
                    </td>
                    <td id="approval-status-val" style="display:none;">
                    </td>
                </tr>
                <tr class="row">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_Reason%>" />
                    </td>
                    <td colspan="4">
                        <textarea class="form-control" id="txtReason" rows="5"></textarea>
                    </td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_Comment%>" />
                    </td>
                    <td colspan="4">
                        <div id="txtOldComment">
                        </div>
                    </td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_YourComment%>" />
                    </td>
                    <td colspan="4">
                        <textarea class="form-control" id="txtComment" rows="5" disabled=""></textarea>
                        <span class="ms-formvalidation ms-csrformvalidation comment_Error"></span>
                    </td>
                </tr>
                <tr class="row" id="error-msg-container" style="display:none;">
                    <td class="header">&nbsp</td>
                    <td id="error-msg" colspan="4"></td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">&nbsp</td>
                    <td colspan="4">
                        <a id="leaveHistory" href="#"><asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_LeaveHistoryTitle%>" /></a>
                    </td>
                </tr>
                <tr class="row">
                    <td colspan="5">
                        <br />
                        <div class="row" style="float: right;">
                            <button id="btApproveLeaveRequest" class="btn btn-success" style="margin-right: 15px; display:none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>" />
                            </button>
                            <button id="btRejectLeaveRequest" class="btn btn-primary" style="margin-right: 15px; display:none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RejectButton%>" />
                            </button>
                            <button id="btSaveLeaveRequest" class="btn btn-primary" style="margin-right: 15px;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,SaveButton%>" />
                            </button>
                            <button id="btnCancelLeaveRequest" class="btn btn-default shift-cancel-btn">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CancelButton%>" />
                            </button>
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
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
                    RequesterControlSelector: '.requester',
                    RequestForControlSelector: '#cbRequestFor',
                    FromDateControlSelector: '#ctl00_PlaceHolderMain_dtFrom_dtFromDate',
                    FromHourControlSelector: '#ctl00_PlaceHolderMain_dtFrom_dtFromDateHours',
                    FromMinuteControlSelector: '#ctl00_PlaceHolderMain_dtFrom_dtFromDateMinutes',
                    ToDateControlSelector: '#ctl00_PlaceHolderMain_dtTo_dtToDate',
                    ToHourControlSelector: '#ctl00_PlaceHolderMain_dtTo_dtToDateHours',
                    ToMinuteControlSelector: '#ctl00_PlaceHolderMain_dtTo_dtToDateMinutes',
                    TotalHoursControlSelector: '#txtTotalHours',
                    TransferWorkToControlSelector: '#cbTransferWorkTo',
                    ApproverToControlSelector: '.approver',
                    AddtionalDisplayControlSelector: '#spanLeaveAdditionalPplPic',
                    ReasonControlSelector: '#txtReason',
                    OldCommentControlSelector: '#txtOldComment',
                    CommentControlSelector: '#txtComment',
                    ApproveControlSelector: '#btApproveLeaveRequest',
                    RejectControlSelector: '#btRejectLeaveRequest',
                    SaveControlSelector: '#btSaveLeaveRequest',
                    CancelControlSelector: '#btnCancelLeaveRequest',
                    AddtionalControlSelector: '#ctl00_PlaceHolderMain_hdnLeavePplPic',
                    FromDateControlSelector_Error: "#dtFrom_Error",
                    ToDateControlSelector_Error: "#dtTo_Error",
                    RequesterControlSelector_Error: "#requester_Error",
                    RequestForControlSelector_Error: '#cbRequestFor_Error',
                    TotalHoursControlSelector_Error: '#txtTotalHours_Error',
                    ApproverToControlSelector_Error: '#approver_Error',
                    TransferWorkToControlSelector_Error: '#cbTransferWorkTo_Error',
                    AddtionalControlSelector_Error: '#addtionalApprover_Error',
                    ApprovalStatusHeaderSelector: "#approval-status-header",
                    ApprovalStatusValSelector: "#approval-status-val",
                    LeaveHistorySelector: '#leaveHistory',
                    ApprovalHistoryButtonSelector: "#loadApprovalHistory",
                    ApprovalHistoryContainerSelector: "#approvalHistoryContainer",
                    ErrorMsgContainerSelector: "#error-msg-container",
                    ErrorMsgSelector: "#error-msg",
                },
                Modal:
                {
                    WrongPolicies: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_WrongPolicies%>" />',
                    CloseButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_Modal_CloseButton%>" />',
                    Title: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_Confirm%>" />',
                    SeePolicesButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_SeeMorePoliciesButton%>" />'
                },
                ValidationMessage:
                {
                    CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                    TransferWorkToInvalid: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,TransferWorkToInvalid%>' />",
                    AdditionApproversInValid: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,AdditionApproversInValid%>' />",
                    RequestForDateInvalid: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LeaveRequest_RequestForDateInvalid%>' />",
                    LeaveHoursInvalid: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LeaveRequest_LeaveHoursInvalid%>' />"
                },
                ResourceText:
                {
                    CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                    LeaveHistoryTitle: "<asp:Literal id='leavehistorytitle' runat='server' Text='<%$Resources:RBVHStadaWebpages,LeaveManagement_PageTitle%>'/>",
                    ApprovalStatusTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_ApprovalStatusTitle%>' />",
                    PostedByTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_PostedByTitle%>' />",
                    DateTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_DateTitle%>' />",
                    CommentTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_CommentTitle%>' />",
                    NoDataAvaibleMsg: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,ApprovalHistory_NoDataAvaibleMsg%>' />",
                    ApprovalStatus_Approved: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,ApprovalStatus_Approved%>' />",
                    ApprovalStatus_Rejected: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,ApprovalStatus_Rejected%>' />",
                    RequestExpiredMsgFormat: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestExpiredMsgFormat%>' />",
                }
            }
            leaveRequestFormInstance = new RBVH.Stada.WebPages.pages.LeaveRequest(settings);
        });
    </script>

</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_PageTitle%>" />
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveRequest_PageTitleArea%>" />
</asp:Content>
