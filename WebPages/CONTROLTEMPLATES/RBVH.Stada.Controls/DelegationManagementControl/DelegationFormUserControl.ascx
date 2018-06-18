<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DelegationFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DelegationManagementControl.DelegationFormUserControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.multi-checkboxes.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/DelegationModule/DelegationForm.js?v=34F85B79-E31F-4E9F-A230-A3F76C75EC85"></script>

<div class="border-container custom-form" style="display: inline-block; width: 100%;">
    <table class="ms-formtable" style="margin-top: 8px; width: 100%;" border="0" cellspacing="0" cellpadding="0">
        <tbody>
            <tr>
                <td class="ms-formlabel " nowrap="true" valign="top" style="width: 20%;">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litFromDate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_FromDate%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top" style="width: 30%;">
                    <span dir="none">
                        <SharePoint:DateTimeControl ID="dtFromDate" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                    </span>
                    <span id="dtFromDate_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
                <td class="ms-formlabel " nowrap="true" valign="top" style="width: 20%;">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litToDate" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_ToDate%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top" style="width: 30%;">
                    <span dir="none">
                        <SharePoint:DateTimeControl ID="dtToDate" runat="server" LocaleId="2057" HoursMode24="True" DateOnly="true" />
                    </span>
                    <span id="dtToDate_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litFromEmployee" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_FromEmployee%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlFromEmployee" runat="server" DataValueField="ID" DataTextField="FullName" CssClass="customSelect"></asp:DropDownList>
                    </span>
                    <span id="ddlFromEmployee_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="Literal1" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_ToEmployee%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlToEmployee" runat="server" multiple DataValueField="ID" DataTextField="FullName" CssClass="customSelect"></asp:DropDownList>
                    </span><br />
                    <span id="ddlToEmployee_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
            </tr>
            <tr>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            <asp:Literal ID="litModule" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_Module%>" />
                            <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText"> *</span>
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        <asp:DropDownList ID="ddlModule" runat="server" multiple DataValueField="ListUrl" DataTextField="ModuleName" CssClass="customSelect"></asp:DropDownList>
                    </span><br />
                    <span id="ddlModule_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                </td>
                <td class="ms-formlabel" nowrap="true" valign="top">
                    <span class="ms-h3 ms-standardheader">
                        <nobr>
                            &nbsp;
                        </nobr>
                    </span>
                </td>
                <td class="ms-formbody" valign="top">
                    <span dir="none">
                        &nbsp;
                    </span>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: right;">
                    <asp:LinkButton ID="btnSearch" CssClass="ms-ButtonHeightWidth btn btn-primary" OnClientClick="return validateBeforeSearch()" runat="server" CausesValidation="false">
                        <span class="glyphicon glyphicon-search"></span> <asp:Literal ID="litSearchTitle" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_SearchButton%>" />
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnDelegate" OnClientClick="return ValidateBeforeDelegate();" CssClass="ms-ButtonHeightWidth btn btn-success" runat="server">
                        <asp:Literal ID="litDelegateTitle" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_DelegateButton%>" />
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnClose" runat="server" CausesValidation="false" CssClass="ms-ButtonHeightWidth btn btn-default">
                        <asp:Literal ID="litCloseTitle" runat="server"  Text="<%$Resources:RBVHStadaWebpages,DelegationForm_CloseButton%>" />
                    </asp:LinkButton>
                </td>
            </tr>
            <tr>
                <td colspan="4">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:GridView ID="gridTasks" runat="server" CssClass="table gridView"  AutoGenerateColumns="false" ShowHeaderWhenEmpty="true">
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="cbSelectAll" runat="server" CssClass="select-all-task" />
                                </HeaderTemplate>
                                <ItemTemplate>
<%--                                    <a id="linkViewDetails" href="javascript:void(0);" runat="server">
                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,DelegationForm_gridTasks_ViewDetails%>" />
                                    </a>--%>
                                    <asp:CheckBox ID="cbSelect" runat="server" CssClass="select-task" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationForm_gridTasks_Module%>">
                                <ItemTemplate>
                                    <asp:Literal ID="litModule" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DiplomaRequestForm_gridTasks_Description%>">
                                <ItemTemplate>
                                    <asp:Literal ID="litDescription" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationForm_gridTasks_Requester%>">
                                <ItemTemplate>
                                    <asp:Literal ID="litRequester" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationForm_gridTasks_Department%>">
                                <ItemTemplate>
                                    <asp:Literal ID="litDepartment" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationForm_gridTasks_CreatedDate%>" HeaderStyle-Width="150px">
                                <ItemTemplate>
                                    <asp:Literal ID="litCreatedDate" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="4">&nbsp;</td>
            </tr>
        </tbody>
    </table>
</div>

<asp:HiddenField ID="hdErrorMessage" runat="server" Value="" />
<asp:HiddenField ID="hdSelectedToEmployees" runat="server" Value="" />
<asp:HiddenField ID="hdSelectedModules" runat="server" Value="" />

<script>
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                hdErrorMessageSelector: '#<%=hdErrorMessage.ClientID%>',
                fromDateSelector: '#ctl00_PlaceHolderMain_DelegationFormUserControl_dtFromDate_dtFromDateDate',
                toDateSelector: '#ctl00_PlaceHolderMain_DelegationFormUserControl_dtToDate_dtToDateDate',
                fromEmployeeSelector: '#<%=ddlFromEmployee.ClientID%>',
                toEmployeeSelector: '#<%=ddlToEmployee.ClientID%>',
                dtFromDateSelector_Error: '#dtFromDate_Error',
                dtToDateSelector_Error: '#dtToDate_Error',
                ddlFromEmployeeSelector_Error: '#ddlFromEmployee_Error',
                ddlToEmployeeSelector_Error: '#ddlToEmployee_Error',
                hdSelectedToEmployeeSelector: '#<%=hdSelectedToEmployees.ClientID%>',
                btnSearchSelector: '#<%=btnSearch.ClientID%>',
                btnDelegateSelector: '#<%=btnDelegate.ClientID%>',
                btnCloseSelector: '#<%=btnClose.ClientID%>',
                ddlModuleSelector: '#<%=ddlModule.ClientID%>',
                hdSelectedModulesSelector: '#<%=hdSelectedModules.ClientID%>',
                ddlModuleSelector_Error: '#ddlModule_Error'
            },
            Resources:
            {
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                FromDateErrorMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,Delegation_FromDateErrorMessage%>' />",
                ToDateErrorMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,Delegation_ToDateErrorMessage%>' />",
                DelegateTaskCheckBoxErrorMessage: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,Delegation_DelegateTaskCheckBoxErrorMessage%>' />",
                DelegationModuleSelectText: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,Delegation_Select%>' />",
            }
        }
        delegationFormInstance = new RBVH.Stada.WebPages.pages.DelegationForm(settings);
    });
</script>