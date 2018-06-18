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
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/FreightModule/FreightRequestForm.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />

    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/FreightModule/FreightRequest.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.js"></script>

    <style type="text/css">
        .jsgrid-delete-button
        {
            min-width:16px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="border-container">
        <table style="width: 100%;" id="tbFreightMaster">
            <thead>
                <tr class="row">
                    <th style="width:15%"></th>
                    <th style="width:50%"></th>
                    <th style="width:15%"></th>
                    <th style="width:20%"></th>
                </tr>
            </thead>
            <tbody>
                <tr class="row">
                    <td>
                        <span style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FrieghtRequest_Bringer%>" />
                             <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </span>
                    </td>
                    <td>
                        <label>
                            <input type="radio" name="bringerType" value="0" checked />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_EmployeeBelongToCompany%>" />
                        </label>
                        <br />
                        <label>
                            <input type="radio" name="bringerType" value="1" />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_CompanyVehicle%>" />
                        </label>
                        <br />
                        <label>
                            <input type="radio" name="bringerType" value="2" />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_EmployeeNotBelongToCompany%>" />
                        </label>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row" id="interEmpl" style="display:none;">
                    <td>
                    </td>
                    <td>
                        <select id="selectBringer"></select>
                         <span class="ms-formvalidation ms-csrformvalidation bringer_Error"></span>
                    </td>
                    <td class="employee-id-Td">
                        <span style="font-weight: bold; padding-left: 10px;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FrieghtRequest_EmployeeID%>" />&nbsp;&nbsp;
                        </span>
                        <span id="spanEmployeeID"></span>
                    </td>
                    <td class="department-Td">
                        <span style="font-weight: bold; padding-left: 10px;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FrieghtRequest_Department%>" />&nbsp;&nbsp;
                        </span>
                        <span id="spanDepartment"></span>
                    </td>
                </tr>
                <tr class="row" id="extEmpl" style="display:none;">
                    <td>
                    </td>
                    <td>
                         <input type="text" class="form-control" id="txtBringer" maxlength="254" />
                         <span class="ms-formvalidation ms-csrformvalidation bringer_Error"></span>
                    </td>
                    <td class="company-name-Td">
                        <span style="font-weight: bold; padding-right: 10px; float:right">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_CompanyName%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </span>
                    </td>
                    <td class="company-name-Td">
                        <input type="text" class="form-control" id="txtCompanyName" maxlength="254" />
                        <span class="ms-formvalidation ms-csrformvalidation company_Error"></span>
                    </td>
                </tr>
                <tr class="row">
                    <td class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Reason%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </div>
                    </td>
                    <td>
                        <textarea class="form-control" id="txtReason" rows="3" maxlength="254"></textarea>
                        <span class="ms-formvalidation ms-csrformvalidation reason_Error"></span>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row">
                    <td class="header">
                    </td>
                     <td>
                        <label>
                            <input type="radio" name="radioReason" id="rdSendGoods" value="0" checked />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_radioSendGoods%>" />
                        </label>
                         &nbsp;
                        <label>
                            <input type="radio" name="radioReason" id="rdOtherReason" value="1" />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_radioOtherReason%>" />
                        </label>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row sendGoodsTr">
                    <td class="header">
                        <label  style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FrieghtRequest_ReceivedBy%>" />
                        </label>
                    </td>
                    <td>
                        <input type="text" class="form-control" id="receivedBy" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row sendGoodsTr">
                    <td class="header">
                        <label style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FrieghtRequest_ReceivedDepartment%>" />
                        </label>
                    </td>
                    <td>
                        <input type="text" class="form-control" id="receiverDepartment" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row sendGoodsTr">
                    <td class="header">
                        <label style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_PhoneContact%>" />
                        </label>
                        
                    </td>
                    <td>
                        <input type="text" class="form-control" id="txtPhoneContact" />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row othersTr" >
                    <td class="header">
                        <label  style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Others%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </label>
                    </td>
                    <td>
                        <input type="text" class="form-control" maxlength="254" id="txtOthers" />
                        <span class="ms-formvalidation ms-csrformvalidation others_Error"></span>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row">
                    <td>
                        <label style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Date%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </label>
                    </td>
                    <td style="padding-left: 0px;">
                        <SharePoint:DateTimeControl ID="dtDate" runat="server" LocaleId="2057" HoursMode24="True"/>
                        <span id="dtDate_Error" class="ms-formvalidation ms-csrformvalidation"></span>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row">
                    <td>
                        <span style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_FreightType%>" />
                        </span>
                    </td>
                    <td>
                        <label>
                            <input type="radio" name="freightType" value="Yes" checked/>
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_BringOut%>" />
                        </label>
                        &nbsp;
                        <label>
                            <input type="radio" name="freightType" value="No" />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_BringIn%>" />
                        </label>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row">
                    <td>
                        <span style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_ReturnedGoods%>" />
                        </span>
                    </td>
                    <td>
                        <label>
                            <input type="radio" name="returnGoods" value="Yes"/>
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_RadioYes%>" />
                        </label>
                        &nbsp;
                        <label>
                            <input type="radio" name="returnGoods" value="No" checked />
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_RadioNo%>" />
                        </label>
                        <br />
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row highPriorityTr" style="display:none;">
                    <td class="header">
                        <span style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_IsHighPriority%>" />
                        </span>
                    </td>
                    <td>
                        <label>
                            <input type="checkbox" name="highPriority" id="highPriority" />
                        </label>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row" style="display:none;"">
                    <td class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,CommonComment%>" />
                        </div>
                    </td>
                    <td>
                        <div id="txtOldComment">
                        </div>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_YourComment%>" />
                        </div>
                    </td>
                    <td>
                        <textarea class="form-control" id="txtComment" rows="3" maxlength="254" disabled=""></textarea>
                        <span class="ms-formvalidation ms-csrformvalidation comment_Error"></span>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_SecurityNotes%>" />
                        </div>
                    </td>
                    <td>
                        <div id="txtOldSecurityNotes">
                        </div>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                <tr class="row" style="display:none;">
                    <td class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,LeaveList_YourComment%>" />
                        </div>
                    </td>
                    <td>
                        <textarea class="form-control" id="txtSecurityNotes" rows="3" maxlength="254" disabled=""></textarea>
                        <span class="ms-formvalidation ms-csrformvalidation securityNotes_Error"></span>
                    </td>
                    <td></td>
                    <td></td>
                </tr>

                <tr class="row" style="display:none;">
                    <td id="approval-status-header" class="header">
                        <div style="font-weight: bold;">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_ApprovalStatus%>" />
                        </div>
                    </td>
                    <td id="approval-status-val">
                    </td>
                    <td></td>
                    <td></td>
                </tr>

                <tr class="row" id="error-msg-container" style="display:none;">
                    <td class="header">
                    </td>
                    <td id="error-msg">
                    </td>
                    <td></td>
                    <td></td>
                </tr>

                <tr class="row">
                    <td colspan="4">
                        <br />
                        <!--Grid Here-->
                        <div id="jsGridFreightRequest"></div>
                        <span class="ms-formvalidation ms-csrformvalidation freightdetails_Error"></span>
                    </td>
                </tr>

                <tr class="row">
                    <td colspan="4">
                        <br />
                        <div class="row" style="float: right;">
                            <button id="btnFreightSecReject" class="btn btn-primary" style="margin-right: 15px; display: none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RejectButton%>" />
                            </button>
                             <button id="btnFreightUpdate" class="btn btn-success" style="margin-right: 15px; display: none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,UpdateButton%>" />
                            </button>
                            <button id="btnFreightRequestApprove" class="btn btn-success" style="margin-right: 15px; display: none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ApproveButton%>" />
                            </button>
                            <button id="btnFreightRequestReject" class="btn btn-primary" style="margin-right: 15px; display: none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RejectButton%>" />
                            </button>
                            <button id="btnFreightRequestPrint" class="btn btn-info" style="margin-right: 15px; display: none;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,PrintButton%>" />
                            </button>
                            <button id="btnFreightRequestSave" class="btn btn-primary" style="margin-right: 15px;">
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,SaveButton%>" />
                            </button>
                            <button id="btnFreightRequestCancel" class="btn btn-default shift-cancel-btn">
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
                    FreightRequestFormSelector: "#jsGridFreightRequest",
                    SpanEmployeeIDSelector: "#spanEmployeeID",
                    TxtBringerSelector: "#txtBringer",
                    SpanDepartmentSelector: "#spanDepartment",
                    TxtCompanyNameSelector: "#txtCompanyName",
                    TxtReason: "#txtReason",
                    DateSelector: '#ctl00_PlaceHolderMain_dtDate_dtDateDate',
                    HoursSelector: '#ctl00_PlaceHolderMain_dtDate_dtDateDateHours',
                    MinutesSelector: '#ctl00_PlaceHolderMain_dtDate_dtDateDateMinutes',
                    SelectBringerSelector: "#selectBringer",
                    HourSelector: "#ctl00_PlaceHolderMain_dtDate_dtDateDateHours",
                    MinuteSelector: "#ctl00_PlaceHolderMain_dtDate_dtDateDateMinutes",
                    TxtReceivedBy: "#receivedBy",
                    TxtReceiverDepartment: "#receiverDepartment",
                    TxtPhoneContact: "#txtPhoneContact",
                    TxtOthersReason: "#txtOthers",
                    ButtonSaveSelector: "#btnFreightRequestSave",
                    ButtonCancelSelector: "#btnFreightRequestCancel",
                    CkbHighPriority: "#highPriority",
                    DateSelector_Error: "#dtDate_Error",
                    BringerSelector_Error: ".bringer_Error",
                    CompanySelector_Error: ".company_Error",
                    ReasonSelector_Error: ".reason_Error",
                    Comment_Error: ".comment_Error",
                    SecurityNotes_Error: ".securityNotes_Error",
                    Others_Error: ".others_Error",
                    FreightDetails_Error: ".freightdetails_Error",
                    TxtOldComment: "#txtOldComment",
                    TxtComment: "#txtComment",
                    TxtOldSecurityNotes: "#txtOldSecurityNotes",
                    TxtSecurityNotes: "#txtSecurityNotes",
                    ButtonPrint: "#btnFreightRequestPrint",
                    ButtonApprove: "#btnFreightRequestApprove",
                    ButtonReject: "#btnFreightRequestReject",
                    ButtonSecReject: "#btnFreightSecReject",
                    ButtonUpdate: "#btnFreightUpdate",
                    RDSendGoods: "#rdSendGoods",
                    RDOtherReason: "#rdOtherReason",
                    ApprovalStatusValSelector: "#approval-status-val",
                    ApprovalHistoryButtonSelector: "#loadApprovalHistory",
                    ApprovalHistoryContainerSelector: "#approvalHistoryContainer",
                    ErrorMsgContainerSelector: "#error-msg-container",
                    ErrorMsgSelector: "#error-msg",
                },
                Modal:
                {
                    WrongPolicies: '<asp:Literal runat="server" Text="<% $Resources:RBVHStadaWebpages,FreightRequest_WrongPolicies%>" />',
                    CloseButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Modal_CloseButton%>" />',
                    SaveButton: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Modal_SaveButton%>" />',
                    Title: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_Confirm%>" />'
                },
                Grid:
                {
                    Fields: [],
                    ColumnTitles:
                    {
                        GridColumn_Order: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_OrderNumber%>" />',
                        GridColumn_GoodsDescription: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_GoodsDescription%>" />',
                        GridColumn_Unit: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_Unit%>" />',
                        GridColumn_Quantity: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_Quantity%>" />',
                        GridColumn_Remarks: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_Remarks%>" />',
                        GridColumn_CheckIn: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_CheckIn%>" />',
                        GridColumn_CheckOut: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_CheckOut%>" />',
                        GridColumn_ShippingInBy: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_ShippingInBy%>" />',
                        GridColumn_ShippingOutBy: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_ShippingOutBy%>" />',
                        GridColumn_ShippingInTime: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_ShippingInTime%>" />',
                        GridColumn_ShippingOutTime: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequestForm_ShippingOutTime%>" />',
                    }
                },
                ResourceText:
                {
                    ConfirmDeleteMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_ConfirmDeleteMessage%>" />',
                    PleaseInputDataMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_PleaseInputDataMessage%>" />',
                    QuantityWrongMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_QuantityIsWrongMessage%>" />',
                    CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                    TransportTimeErrorMessage_1: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,TransportTimeErrorMessage_1%>' />",
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

            freightRequestFormInstance = new RBVH.Stada.WebPages.pages.FreightRequest(settings);
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightRequest_PageTitleArea%>" />
</asp:Content>
