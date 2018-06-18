<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecruitmentFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RecruitmentManagementControl.RecruitmentFormUserControl" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/CommentControl.ascx" TagPrefix="CommonControls" TagName="CommentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/FormButtonsControl.ascx" TagPrefix="CommonControls" TagName="FormButtonsControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/WorkflowHistoryControl.ascx" TagPrefix="CommonControls" TagName="WorkflowHistoryControl" %>
<%--<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/SupportingDocumentControl.ascx" TagPrefix="CommonControls" TagName="SupportingDocumentControl" %>--%>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/RecruitmentModule/RecruitmentModule.css?v=403CCD20-1B11-401F-BD7F-5BF0A7259A0F" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/RecruitmentModule/RecruitmentForm.js?v=C0350B2C-05BA-4C12-AC34-C6FDE9E5A5E6"></script>

<!-- HTML Form -->
<div class="border-container custom-form" style="display: inline-block; width: 100%;" >
    <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litDepartment" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Department%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlDepartment" runat="server" CssClass="customSelect" />
                    </span>
                    <span id="ddlDepartment_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                </td>
                <td class="ms-formlabel" nowrap="true" valign="top" id="tdLitTemplate" runat="server" visible="false">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litTemplate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Template%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top" id="tdDllTemplate" runat="server" visible="false">
                    <span dir="none">
                        <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="customSelect" DataValueField="ID" DataTextField="Title" AutoPostBack="true" style="display: inherit;" />
                        &nbsp;&nbsp;
                        <asp:LinkButton ID="lbtnDeleteTemplate" runat="server" CausesValidation="false" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_lbtnDeleteTemplate%>" OnClientClick="return recruitmentFormInstance.ConfirmDeleteTemplate()" Visible="false" />
                    </span>
                </td>
            </tr>
            <tr id="trSavingTemplate" runat="server" visible="false">
                <td nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litSavingTemplate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_SavingTemplate%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:CheckBox ID="cbSavingTemplate" runat="server" />
                        <br>
                    </span>
                </td>
                <td nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litTemplateName" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_TemplateName%>" />
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:TextBox ID="txtTemplateName" runat="server" CssClass="ms-long customInput" MaxLength="255" />
                    </span>
                    <span id="txtTemplateName_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                </td>
            </tr>
        </tbody>
    </table>
    <fieldset>
        <legend><asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruimentForm_General%>' /></legend>
        <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litPosition" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Position%>" />
                                <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtPosition" runat="server" CssClass="ms-long" MaxLength="254" />
                        </span>
                        <span id="txtPosition_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litQuantity" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Quantity%>" />
                                <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtQuantity" runat="server" CssClass="ms-long customInput" MaxLength="9" />
                        </span>
                        <span id="txtQuantity_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litReasonForRecruitment" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_ReasonForRecruitment%>" />
                                <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtReasonForRecruitment" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                        </span>
                         <span id="txtReasonForRecruitment_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litGender" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Gender%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblGender" runat="server" />
                        </span>
                    </td>
                 </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litFromAge" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_FromAge%>" />
                                 <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtFromAge" runat="server" CssClass="ms-long customInput" MaxLength="2" /> 
                        </span>
                         <span id="txtFromAge_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litToAge" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_ToAge%>" />
                                 <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtToAge" runat="server" CssClass="ms-long customInput" MaxLength="2" />
                        </span>
                         <span id="txtToAge_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litMartialStatus" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_MartialStatus%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblMartialStatus" runat="server" />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litAvailableTime" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_AvailableTime%>" />
                                 <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <SharePoint:DateTimeControl ID="dtAvailableTime" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                          
                        </span>
                        <span id="dtAvailableTime_Error" class="ms-formvalidation" style="margin-top:0px;"></span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litWorkingTime" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_WorkingTime%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblWorkingTime" runat="server"  />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litEducationLeval" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_EducationLevel%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblEducationLevel" runat="server"  />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litAppearance" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Appearance%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblAppearance" runat="server" />
                          
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litWorkingExperience" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_WorkingExperience%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblWorkingExperience" runat="server" />
                            <asp:TextBox ID="txtOtherWorkingExperience" runat="server" />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litSpecialities" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Specialities%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtSpecialities" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                        
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litDescriptionOfBasicWork" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_BasicWork%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtDescriptionOfBasicWork" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
    <fieldset>
        <legend><asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruimentForm_Necessary%>' /></legend>
        <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litMoralVocations" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_MoralVocations%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtMoralVocations" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                        <span class="ms-h3 ms-standardheader">
                            <nobr>
                                <asp:Literal ID="litWorkingAbilities" runat="server"  Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_WorkingAbilities%>" />
                            </nobr>
                        </span>
                    </td>
                    <td class="ms-formbody" valign="top">
                        <span dir="none">
                            <asp:TextBox ID="txtWorkingAbilities" runat="server" TextMode="MultiLine" Rows="6" Columns="20" CssClass="ms-long" />
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
    <fieldset>
        <legend><asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruimentForm_Skill%>' /></legend>
        <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="ms-formbody" valign="top" colspan="2">
                        <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td class="ms-formlabel" nowrap="true" valign="top" width="50%">
                                        <span class="ms-h3 ms-standardheader">
                                            <nobr>
                                                <asp:Literal ID="litLanguageSkills" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_LanguageSkills%>" />
                                            </nobr>
                                        </span>
                                    </td>
                                    <td class="ms-formlabel" nowrap="true" valign="top" width="20%" style="padding-left: 1%">
                                        <span class="ms-h3 ms-standardheader">
                                            <nobr>
                                                <asp:Literal ID="litComputerSkills" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_ComputerSkills%>" />
                                            </nobr>
                                        </span>
                                    </td>
                                    <td class="ms-formlabel" nowrap="true" valign="top" width="30%">
                                        <span class="ms-h3 ms-standardheader">
                                            <nobr>
                                                <asp:Literal ID="litOtherSkills" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_AnotherSkills%>" />
                                            </nobr>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="ms-formbody" valign="top">
                                        <span dir="none">
                                            <input type="hidden" id="hdlanguageSkills" value="" />
                                            <div id="languageSkills"></div>
                                            <br/>
                                        </span>
                                    </td>
                                    <td class="ms-formbody" valign="top" style="padding-left: 1%">
                                        <span dir="none">
                                            <asp:CheckBoxList ID="cblComputerSkills" runat="server" />
                                            <asp:TextBox ID="txtOtherComputerSkills" runat="server" />
                                            <br/>
                                        </span>
                                    </td>
                                    <td class="ms-formbody" valign="top">
                                        <span dir="none">
                                            <asp:CheckBoxList ID="cblOtherSkills" runat="server" />
                                            <br/>
                                        </span>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
    <fieldset>
        <legend><asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruimentForm_Anothers%>' /></legend>
        <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="ms-formbody" valign="top" colspan="2">
                        <span dir="none">
                            <asp:CheckBoxList ID="cblOtherRiquirement" runat="server" />
                            <br/>
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
    <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr id="tr-approval-status" style="display: none;">
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <label>
                        <asp:Literal ID="litStatus" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Status%>" />
                     </label>
                </td>
                <td id="td-approval-status">
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel leftColumnWidth" nowrap="true" valign="top">
                    <label>
                        <asp:Literal ID="litComments" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Comments%>" />
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

<asp:HiddenField ID="hdIsEditable" runat="server" />
<asp:HiddenField ID="hdOtherValueChoiceFieldValue" runat="server" />
<asp:HiddenField ID="hdForeignLanguages" runat="server"  Value=""/>
<asp:HiddenField ID="hdForeignLanguageLevels" runat="server"  Value=""/>
<asp:HiddenField ID="hdRecruitmentLanguageSkills" runat="server"  Value=""/>
<asp:HiddenField ID="hdErrorMessage" runat="server"  />
<asp:HiddenField ID="hdToday" runat="server" Value="" />
<asp:HiddenField ID="hdNumberOfStandardizedDaysBeforesubmission" runat="server" Value="15" />
<asp:HiddenField ID="hdNoneTemplateValue" runat="server" Value="0" />
<asp:HiddenField ID="hdApprovalStatus" runat="server" />

<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                hdIsEditableSelector: '#<%=hdIsEditable.ClientID%>',
                cblAppearanceSelector: '#<%=cblAppearance.ClientID%>',
                cblWorkingExperience: '#<%=cblWorkingExperience.ClientID%>',
                txtOtherWorkingExperience: '#<%=txtOtherWorkingExperience.ClientID%>',
                hdErrorMessageSelector: '#<%=hdErrorMessage.ClientID%>',
                ddlDepartmentSelector: '#<%=ddlDepartment.ClientID%>',
                txtPositionSelector: '#<%=txtPosition.ClientID%>',
                txtQuantitySelector: '#<%=txtQuantity.ClientID%>',
                txtReasonForRecruitmentSelector: '#<%=txtReasonForRecruitment.ClientID%>',
                txtFromAgeSelector: '#<%=txtFromAge.ClientID%>',
                txtToAgeSelector: '#<%=txtToAge.ClientID%>',
                txtOtherComputerSkillsSelector: '#<%=txtOtherComputerSkills.ClientID%>',
                dtAvailableTimeSelector: '#ctl00_PlaceHolderMain_RecruitmentFormUserControl_dtAvailableTime_dtAvailableTimeDate',
                ddlDepartmentSelector_Error: '#ddlDepartment_Error',
                txtPositionSelector_Error: '#txtPosition_Error',
                txtQuantitySelector_Error: '#txtQuantity_Error',
                txtReasonForRecruitmentSelector_Error: "#txtReasonForRecruitment_Error",
                txtFromAgeSelector_Error: "#txtFromAge_Error",
                txtToAgeSelector_Error: "#txtToAge_Error",
                dtAvailableTimeSelector_Error: "#dtAvailableTime_Error",
                divLanguageSkillsSelector: "#languageSkills",
                hdOtherValueChoiceFieldValueSelector: '#<%=hdOtherValueChoiceFieldValue.ClientID%>',
                hdForeignLanguagesSelector: '#<%=hdForeignLanguages.ClientID%>',
                hdForeignLanguageLevelsSelector: '#<%=hdForeignLanguageLevels.ClientID%>',
                hdRecruitmentLanguageSkillsSelector: '#<%=hdRecruitmentLanguageSkills.ClientID%>',
                cblComputerSkillsSelector: '#<%=cblComputerSkills.ClientID%>',
                cblEducationLevelSelector: '#<%=cblEducationLevel.ClientID%>',
                cblMartialStatusSelector: '#<%=cblMartialStatus.ClientID%>',
                cblOtherSkillsSelector: '#<%=cblOtherSkills.ClientID%>',
                cblGenderSelector: '#<%=cblGender.ClientID%>',
                cblWorkingTimeSelector: '#<%=cblWorkingTime.ClientID%>',
                txtSpecialitiesSelector: '#<%=txtSpecialities.ClientID%>',
                txtDescriptionOfBasicWorkSelector: '#<%=txtDescriptionOfBasicWork.ClientID%>',
                txtMoralVocationsSelector: '#<%=txtMoralVocations.ClientID%>',
                txtWorkingAbilitiesSelector: '#<%=txtWorkingAbilities.ClientID%>',
                txtCommentSelector: '#ctl00_PlaceHolderMain_RecruitmentFormUserControl_CommentControl_txtComment',
                hdlanguageSkillsSelector: '#hdlanguageSkills',
                cblOtherRequirementSelector: '#<%=cblOtherRiquirement.ClientID%>',
                hdTodaySelector: '#<%=hdToday.ClientID%>',
                hdNumberOfStandardizedDaysBeforesubmissionSelector: '#<%=hdNumberOfStandardizedDaysBeforesubmission.ClientID%>',
                ddlTemplateSelector: '#<%=ddlTemplate.ClientID%>',
                cbSavingTemplateSelector: '#<%=cbSavingTemplate.ClientID%>',
                txtTemplateNameSelector: '#<%=txtTemplateName.ClientID%>',
                hdNoneTemplateValueSelector: '#<%=hdNoneTemplateValue.ClientID%>',
                txtTemplateNameSelector_Error: '#txtTemplateName_Error',
                ApprovalStatusValueSelector: '#<%=hdApprovalStatus.ClientID%>',
                ApprovalStatusTrSelector: '#tr-approval-status',
                ApprovalStatusTdSelector: '#td-approval-status'
            },
            OtherOptionValue: $('#<%=hdOtherValueChoiceFieldValue.ClientID%>').val(),
            ResourceText:
            {
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                WrongNumber: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,Recruiment_WrongNumber%>' />",
                LanguageSkillGrid_No: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_No%>' />",
                LanguageSkillGrid_Language: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_ForeignLanguage%>' />",
                LanguageSkillGrid_Level: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_Level%>' />",
                LanguageSkillGrid_OtherLevel: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_OtherLevel%>' />",
                LanguageSkillGrid_InputOtherMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_InputOtherMessage %>' />",
                LanguageSkillGrid_ErrorMessage_FromAge_LessThan_ToAge: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_FromAge_LessThan_ToAge %>' />",
                LanguageSkillGrid_AvailabelDateWrong: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_AvailabelDateWrong %>' />",
                LanguageSkillGrid_PleaseInputData: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,LanguageSkillGrid_ErrorMessage_PleaseInputData %>' />",
                DataMustBeDiffrent: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_DataMustBeDiffrent%>' />",
                WrongPolicy: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_WrongPolicy%>' />",
                DeleteTemplateConfirmationMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_DeleteTemplateConfirmationMessage%>' />",
                EmptyTemplateNameMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_EmptyTemplateName%>' />",
                ExistedTemplateNameMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RecruitmentForm_ErrorMessage_ExistedTemplateName%>' />",
            },
        }
        recruitmentFormInstance = new RBVH.Stada.WebPages.pages.RecruitmentForm(settings);
    });
</script>