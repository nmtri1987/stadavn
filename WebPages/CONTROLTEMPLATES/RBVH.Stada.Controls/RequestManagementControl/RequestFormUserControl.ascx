<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RequestManagementControl.RequestFormUserControl" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/CommentControl.ascx" TagPrefix="CommonControls" TagName="CommentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/FormButtonsControl.ascx" TagPrefix="CommonControls" TagName="FormButtonsControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/WorkflowHistoryControl.ascx" TagPrefix="CommonControls" TagName="WorkflowHistoryControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/SupportingDocumentControl.ascx" TagPrefix="CommonControls" TagName="SupportingDocumentControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/RequestModule/RequestsForm.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
    
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>

<%--<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/RequestModule/RequestForm.js?v=<%= DateTime.Now.Millisecond %>"></script>--%>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/RequestModule/RequestForm.js?v=18F7642F-A508-4196-8201-C96277CE0177"></script>

<style>
    #tbRequestContainer
    {
            border-collapse: separate;
            /*border-spacing: 1em*/
    }
    .customSelect
    {
        width: inherit;
    }
    #ctl00_PlaceHolderMain_RequestFormUserControl_txtTitle
    {
        width: 80%;
        border:1px solid #ababab !important;
    }
</style>

<div class="border-container custom-form" style="display: inline-block; width: 100%;">
    <table id="tbRequestContainer" class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
         <tbody>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litRequester" runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsForm_Requester%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                    <span dir="none">
                        <asp:Label ID="lblRequester" runat="server" />
                        <br>
                    </span>
                </td>

                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litEmployeeID" runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsForm_EmployeeID%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                     <span dir="none">
                        <asp:Label ID="lblEmployeeId" runat="server" />
                        <br>
                    </span>
                </td>
    
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litRequestsForm_Department" runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsForm_Department%>" />
                            <span class="ms-accentText" title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />">*</span>
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                     <span dir="none">
                        <asp:Label ID="lblDepartment" runat="server" />
                        <br>
                    </span>
                </td>
                <td colspan="3"></td>
            </tr>
        </tbody>
    </table>
    <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litTitle" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_RequestTitle%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top" colspan="2">
                    <span dir="none">
                        <asp:TextBox ID="txtTitle" runat="server" CssClass="ms-long" MaxLength="254" />
                        <br>
                    </span>
                    <span id="Title_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                </td>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litRequestType" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_RequestType%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td id="radioButtonGroups" runat="server" class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:RadioButtonList ID="rblRequetTypes" runat="server" DataTextField="RequestTypeName" DataValueField="ID" />
                        <br>
                    </span>
                </td>
            </tr>
            <tr id="trReceivedBy" runat="server" visible="true">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litReceivedBy" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_ReceivedBy%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlReceivedBy" runat="server" DataValueField="ID" DataTextField="Name" CssClass="customSelect"></asp:DropDownList>
                        <span id="ReceivedBy_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                        <br/>
                    </span>
                </td>
            </tr>
            <!--ToDo: add detail form here-->
            <tr>
                <td colspan="2">
                    <div id="details1" style="display: none">
                        
                    </div>
                    <div id="details2" style="display: none">
                        
                    </div>
                    <div id="details3" style="display: none">
                    </div>
                </td>
            </tr>
            <tr id="trFinishDate" runat="server">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litFinishDate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_FinishDate%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <SharePoint:DateTimeControl ID="dtFinishDate" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                    </span>
                    <span id="dtFinishDate_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                </td>
            </tr>
            <tr id="trRequireBODdApprove" runat="server" visible="false">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litRequiredBODApprove" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestForms_RequiredBODApprove%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:CheckBox ID="chkboxRequireBODdApprove" runat="server" Enabled="false"  />
                        <br />
                    </span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litReferTo" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_ReferTo%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlReferTo" runat="server" Visible="false" CssClass="customSelect" style="display: inline-block; margin-right: 5px;"/>
                        <a id="linkReferTo" runat="server" href="javascript:void(0);" visible="false" />
                        <br />
                    </span>
                </td>
            </tr>
            <tr id="trDueDate" runat="server" visible="false">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litDueDate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_DueDate%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <SharePoint:DateTimeControl ID="dtDueDate" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                    </span>
                     <span id="dtDueDate_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                </td>
            </tr>

            <tr id="tr1">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litSupportingDocument" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_SupportingDocument%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <CommonControls:SupportingDocumentControl ID="SupportingDocumentControl" runat="server" />
                </td>
            </tr>
            <tr id="tr-approval-status" style="display: none;">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litStatus" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RequestsForm_Status%>" />
                        </nobr>
                    </span>
                </td>
                <td id="td-approval-status">
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <label>
                        <asp:Literal ID="litComments" runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsForm_Comments%>" />
                    </label>
                </td>
                <td class="ms-formbody" valign="top">
                    <CommonControls:CommentControl ID="CommentControl" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
    <table>
        <tbody>
            <tr>
                <td style="width: 100%;">&nbsp;</td>
                <td>
                    <CommonControls:FormButtonsControl ID="FormButtonsControl" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
    <table style="width: 100% !important;">
        <tbody>
            <tr>
                <td>
                    <CommonControls:WorkflowHistoryControl ID="WorkflowHistoryControl" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
</div>

<asp:HiddenField ID="hdRequestType" runat="server" />
<asp:HiddenField ID="hdDetailsBuyData" runat="server" />
<asp:HiddenField ID="hdDetailsRepair" runat="server" />
<asp:HiddenField ID="hdDetailsOthers" runat="server" />
<asp:HiddenField ID="hdIsEditable" runat="server" />
<asp:HiddenField ID="hdIsShowReceivedBy" runat="server" Value="0" />
<asp:HiddenField ID="hdSelectedReceivedBy" runat="server" Value="0" />
<asp:HiddenField ID="hdIsShowFinishDate" runat="server" Value="0" />
<asp:HiddenField ID="hdIsShowRequiredApprovalByBOD" runat="server" Value="0" />
<asp:HiddenField ID="hdIsShowDueDate" runat="server" Value="0" />
<asp:HiddenField ID="hdErrorMessage" runat="server" Value="" />
<asp:HiddenField ID="hdDisplayFormUrl" runat="server" Value="" />
<asp:HiddenField ID="hdRequestBuyTypeId" runat="server" Value="1" />
<asp:HiddenField ID="hdRequestRepairTypeId" runat="server" Value="2" />
<asp:HiddenField ID="hdRequestOtherTypeId" runat="server" Value="3" />
<asp:HiddenField ID="hdIsEmployeeDEH" runat="server" Value="0" />
<asp:HiddenField ID="hdApprovalStatus" runat="server" />
<asp:HiddenField ID="hdPrintCounter" runat="server" Value="0" />

<script type="text/javascript">
    $(document).ready(function () {
      
        var settings = {
            Controls:
            {
                GridDetail1ControlSelector: "#details1",
                GridDetail2ControlSelector: "#details2",
                GridDetail3ControlSelector: "#details3",
                txtTitleSelector: '#<%=txtTitle.ClientID%>',
                RadioButtonsNameSelector: '<%=rblRequetTypes.UniqueID%>',
                RadioButtonsGroupNameSelector: '#<%=rblRequetTypes.ClientID%>',
                hdRequestTypeSelector: '#<%=hdRequestType.ClientID%>',
                hdRequestBuySelector :'#<%=hdDetailsBuyData.ClientID%>',
                hdDetailsRepairSelector: '#<%=hdDetailsRepair.ClientID%>',
                hdDetailsOthersSelector: '#<%=hdDetailsOthers.ClientID%>',
                hdIsEditableSelector: '#<%=hdIsEditable.ClientID%>',
                trReceivedBySelector: '#<%=trReceivedBy.ClientID%>',
                ddlReceivedBySelector: '#<%=ddlReceivedBy.ClientID%>',
                trFinishDateSelector: '#<%=trFinishDate.ClientID%>',
                trRequireBODdApproveSelector: '#<%=trRequireBODdApprove.ClientID%>',
                trDueDateSelector: '#<%=trDueDate.ClientID%>',
                hdIsShowReceivedBySelector: '#<%=hdIsShowReceivedBy.ClientID%>',
                hdSelectedReceivedBySelector: '#<%=hdSelectedReceivedBy.ClientID%>',
                hdIsShowFinishDateSelector: '#<%=hdIsShowFinishDate.ClientID%>',
                hdIsShowRequiredApprovalByBODSelector: '#<%=hdIsShowRequiredApprovalByBOD.ClientID%>',
                hdIsShowDueDateSelector: '#<%=hdIsShowDueDate.ClientID%>',
                ddlReferToSelector: '#<%=ddlReferTo.ClientID%>',
                linkReferToSelector: '#<%=linkReferTo.ClientID%>',
                hdDisplayFormUrlSelector: '#<%=hdDisplayFormUrl.ClientID%>',
                hdRequestBuyTypeIdSelector: '#<%=hdRequestBuyTypeId.ClientID%>',
                hdRequestRepairTypeIdSelector: '#<%=hdRequestRepairTypeId.ClientID%>',
                hdRequestOtherTypeIdSelector: '#<%=hdRequestOtherTypeId.ClientID%>',
                hdErrorMessageSelector: '#<%=hdErrorMessage.ClientID%>',
                btnSaveDraftSelector: '#ctl00_PlaceHolderMain_RequestFormUserControl_FormButtonsControl_btnSaveDraft',
                btnSaveAndSubmitSelector: '#ctl00_PlaceHolderMain_RequestFormUserControl_FormButtonsControl_btnSaveAndSubmit',
                FinishDateSelector: '#ctl00_PlaceHolderMain_RequestFormUserControl_dtFinishDate_dtFinishDateDate',
                DueDateSelector: '#ctl00_PlaceHolderMain_RequestFormUserControl_dtDueDate_dtDueDateDate',
                TitleSelector_Error: '#Title_Error',
                FinishDateSelector_Error: '#dtFinishDate_Error',
                DueDateSelector_Error: '#dtDueDate_Error',
                ReceivedBy_Error:"#ReceivedBy_Error",
                //AddDocumentButtonSelector: "#ctl00_PlaceHolderMain_RequestFormUserControl_SupportingDocumentControl_AddMoreFile",
                //DocumentFilesDivSelector: "#ctl00_PlaceHolderMain_RequestFormUserControl_SupportingDocumentControl_GridSupportingDocument",
                CommentSelector: "#ctl00_PlaceHolderMain_RequestFormUserControl_CommentControl_txtComment",
                hdIsEmployeeDEHSelector: '#<%=hdIsEmployeeDEH.ClientID%>',
                ApprovalStatusValueSelector: '#<%=hdApprovalStatus.ClientID%>',
                PrintCounterSelector: '#<%=hdPrintCounter.ClientID%>',
                ApprovalStatusTrSelector: '#tr-approval-status',
                ApprovalStatusTdSelector: '#td-approval-status'
            },
            Messages:
            {
                PleaseInputContentData: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_PleaseInputData%>' />",
                QuantityDataError: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_InputQuantityError%>' />",
                FromDateToDateError: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_FromLessThanToDate%>' />",
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                PleaseInputGridDetail: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_PleaseInputGridDetail%>' />",
                SelectedDateMustGreaterThanCurrentDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SelectedDateMustGreaterThanCurrentDate%>' />",
                InvalidFileName: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_InvalidFileName%>' />",
                PleaseInputReasonData: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_PleaseInputReasonData%>' />",
                PleaseSelectedReceivedDepartment: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_PleaseSelectReceivedDepartment%>' />",
                DataMustBeDifferentAfterEdit : "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DataMustBeDifferentAfterEdit%>' />",
            },
            GridResources:
            {
                GridOrderNumber :"<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_Grid_OrderNumber%>' />",
                GridBuyDetail_Content: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_BuyDetailGrid_Content%>' />",
                GridBuyDetail_Form: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_BuyDetailGrid_Form%>' />",
                GridBuyDetail_Unit: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_BuyDetailGrid_Unit%>' />",
                GridBuyDetail_Quantity: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_BuyDetailGrid_Quantity%>' />",
                GridBuyDetail_Reason: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_BuyDetailGrid_Reason%>' />",

                GridRepairDetail_Content: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_RepairGrid_Content%>' />",
                GridRepairDetail_Reason: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_RepairGrid_Reason%>' />",
                GridRepairDetail_Place: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_RepairGrid_Place%>' />",
                GridRepairDetail_FromDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_RepairGrid_From%>' />",
                GridRepairDetail_ToDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_RepairGrid_To%>' />",

                GridOthersDetail_Content: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_OthersGrid_Content%>' />",
                GridOthersDetail_Unit: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_OthersGrid_Unit%>' />",
                GridOthersDetail_Quantity: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_OthersGrid_Quantity%>' />",
                GridOthersDetail_Reason: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestForm_OthersGrid_Reason%>' />",
            }

        }
        requestFormInstance = new RBVH.Stada.WebPages.pages.RequestForm(settings);

    })
    
</script>
