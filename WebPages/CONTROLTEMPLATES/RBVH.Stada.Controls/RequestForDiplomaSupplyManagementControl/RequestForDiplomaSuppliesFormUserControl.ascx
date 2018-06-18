<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestForDiplomaSuppliesFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DiplomaManagementControl.RequestForDiplomaSuppliesFormUserControl" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/CommentControl.ascx" TagPrefix="CommonControls" TagName="CommentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/FormButtonsControl.ascx" TagPrefix="CommonControls" TagName="FormButtonsControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/WorkflowHistoryControl.ascx" TagPrefix="CommonControls" TagName="WorkflowHistoryControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/SupportingDocumentControl.ascx" TagPrefix="CommonControls" TagName="SupportingDocumentControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />

<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/toaster/jquery.toaster.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/RequestForDiplomaSupplyModule/RequestForDiplomaSupplyForm.js?v=ASHSHF9F-26F4-4832-9BE5-91D2F913C848"></script>
<style>
    .custom-form input[type=submit].ms-ButtonHeightWidth {
        font-size: 14px !important;
    }
</style>
<div class="border-container custom-form" style="display: inline-block; width: 100%;">
    <table id="tbRequestContainer" class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
         <tbody>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top" width="20%">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litDepartment" runat="server" Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_Department%>" />
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                    <span dir="none">
                        <asp:Label ID="lblDepartment" runat="server" />
                        <br>
                    </span>
                </td>
                <td class="ms-formlabel" nowrap="true" valign="top" width="20%">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litRequester" runat="server" Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_Requester%>" />
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                    <span dir="none">
                        <asp:Label ID="lblRequester" runat="server" />
                        <br>
                    </span>
                </td>
                <td class="ms-formlabel" nowrap="true" valign="top" width="20%">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litPosition" runat="server" Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_Position%>" />
                        </nobr>
                    </span>
                    &nbsp;&nbsp;
                    <span dir="none">
                        <asp:Label ID="lblPosition" runat="server" />
                        <br>
                    </span>
                </td>
                 <td colspan="2">&nbsp;</td>
            </tr>
        </tbody>
    </table>
    <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td width="20%">&nbsp;</td>
                <td >&nbsp;</td>
            </tr>
             <tr>
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <label>
                                <asp:Literal ID="litSupportingDocument" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_SupportingDocument%>" />
                            </label>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <CommonControls:SupportingDocumentControl ID="SupportingDocumentControl" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel " nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litFullName" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_FullName%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlEmployee" runat="server" DataValueField="ID" DataTextField="FullName" CssClass="customSelect"></asp:DropDownList>
                    </span>
                    <span id="ddlFullName_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litEmployeeCode" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_EmployeeCode%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:TextBox ID="txtEmployeeCode" runat="server" CssClass="ms-long customInput" MaxLength="254" Enabled="false" />
                    </span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litPostion" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_Postion%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:TextBox ID="txtPosition" runat="server" CssClass="ms-long customInput" Width="50%" MaxLength="254" />
                    </span>
                    <span id="txtPosition_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr id="trDateOfEmp" runat="server">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litDateOfEmp" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_DateOfEmp%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:Label ID="lblDateOfEmp" runat="server" />
                    </span>
                    <span id="lblDateOfEmp_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top" colspan="2">
                    <span class="ms-h3 ms-standardheader">
                        <asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_ReceivedDiploma%>' />
                        <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <input type="hidden" current-value="" id="hdGridDetailsValue"/>
                    <div id="detailsGrid"></div>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litToTheDailyWorks" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_ToTheDailyWorks%>" />
                             <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:TextBox ID="txtToTheDailyWorks" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                    </span>
                    <span id="txtToTheDailyWorks_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader" style="display: table; white-space: normal;max-width: 80%;">
                            <asp:Literal ID="litNewSuggestions" runat="server" Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_NewSuggestions%>" />
                             <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:TextBox ID="txtNewSuggestions" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                    </span>
                     <span id="txtNewSuggestions_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr id="trDiplomaRevision" runat="server" visible="false">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litDiplomaRevision" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_DiplomaRevision%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:CheckBox ID="chkboxDiplomaRevision" runat="server" Checked="false" />
                    </span>
                </td>
            </tr>
            <tr id="trSalaryRevision" runat="server" visible="false">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litSalaryRevision" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_SalaryRevision%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:CheckBox ID="chkboxSalaryRevision" runat="server" Checked="false" />
                    </span>
                </td>
            </tr>
            <tr id="trFromDate" runat="server" visible="false">
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litFromDate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_From%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <SharePoint:DateTimeControl ID="dtFromDate" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                    </span>
                     <span id="dtFromDate_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr id="tr-approval-status" style="display: none;">
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <label>
                        <asp:Literal ID="litStatus" runat="server" Text="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_Status%>" />
                     </label>
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

<asp:HiddenField ID="hdIsEditable" runat="server" Value="" />
<asp:HiddenField ID="hdErrorMessage" runat="server" Value="" />
<asp:HiddenField ID="hdRequestDiplomaDetails" runat="server"  Value=""/>
<asp:HiddenField ID="hdApprovalStatus" runat="server" />
<script>
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                AddDocumentButtonSelector: "#ctl00_PlaceHolderMain_DiplomaManagementControl_SupportingDocumentControl_AddMoreFile",
                DocumentFilesDivSelector: "#ctl00_PlaceHolderMain_DiplomaManagementControl_SupportingDocumentControl_GridSupportingDocument",
                CommentSelector: "#ctl00_PlaceHolderMain_RequestForDiplomaSuppliesForm_CommentControl_txtComment",
                txtPositionSelector: "#<%=txtPosition.ClientID%>",
                txtPositionSelector_Error: "#txtPosition_Error",
                txtToTheDailyWorksSelector: "#<%=txtToTheDailyWorks.ClientID%>",
                txtToTheDailyWorksSelector_Error: "#txtToTheDailyWorks_Error",
                txtNewSuggestionsSelector: "#<%=txtNewSuggestions.ClientID%>",
                txtNewSuggestionsSelector_Error: "#txtNewSuggestions_Error",
                dtFromDateSelector: "#ctl00_PlaceHolderMain_RequestForDiplomaSuppliesForm_dtFromDate_dtFromDateDate",
                dtFromDateSelector_Error: "#dtFromDate_Error",
                hdErrorMessageSelector: '#<%=hdErrorMessage.ClientID%>',
                hdRequestDiplomaDetailsSelector: '#<%=hdRequestDiplomaDetails.ClientID%>',
                hdIsEditableSelector: '#<%=hdIsEditable.ClientID%>',
                ddlEmployeeSelector: '#<%=ddlEmployee.ClientID%>',
                lblDateOfEmpSelector: '#<%=lblDateOfEmp.ClientID%>',
                txtEmployeeCodeSelector: '#<%=txtEmployeeCode.ClientID%>',
                ddlFullNameSelector_Error: '#ddlFullName_Error',
                ddlEmployeeSelector: '#<%=ddlEmployee.ClientID%>',
                GridDetailsSelector: '#detailsGrid',
                hdGridDetailsValueSelector: '#hdGridDetailsValue',
                ApprovalStatusValueSelector: '#<%=hdApprovalStatus.ClientID%>',
                ApprovalStatusTrSelector: '#tr-approval-status',
                ApprovalStatusTdSelector: '#td-approval-status'
            },
            ResourceText:
            {
                DetailGrid_CurrentDiploma: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_CurrentDiploma%>' />",
                DetailGrid_GraduationYear: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_GraduationYear%>' />",
                DetailGrid_NewDiploma: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_NewDiploma%>' />",
                DetailGrid_Faculty: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_Faculty%>' />",
                DetailGrid_IssuedPlace: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_IssuedPlace%>' />",
                DetailGrid_TrainingDuration: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_DetailGrid_TrainingDuration%>' />",
                ErrorMessage_PleaseInputData: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_ErrorMessage_PleaseInputData%>' />",
                ErrorMessage_WrongFromDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_ErrorMessage_FromDate%>' />",
                ErrorMessage_PleaseInputDetailGrid: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_ErrorMessage_PleaseInputDetailGrid%>' />",
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                PleaseSelectEmp: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_ErrorMessage_UnSelectedEmployee%>' />" ,
                DataMustBeDiffrent:"<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequestDiplomaForm_ErrorMessage_DataMustBeDiffrent%>' />" 
            }
        }
        requestForDiplomaSupplyFormInstance = new RBVH.Stada.WebPages.pages.RequestForDiplomaSupplyForm(settings);
    });
</script>
