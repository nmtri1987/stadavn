<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RequestListUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RequestManagementControl.RequestListUserControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/URI/URI.js?v=E7297DEA-F322-413F-A8CE-8BEBBC316B66"></script>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/Common/BaseListPage.css?v=8A39B87C-8210-4F14-A8A5-5E336E9B6377" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/RequestModule/RequestList.js?v=AB68E871-1D47-4EB4-BF5A-265073875A4A"></script>

<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                ActivedTabSelector: '#<%=hdActivedTab.ClientID%>',
                DepartmentSelector: '#<%=ddlDepartments.ClientID%>',
                FromDateControlSelector: '#<%=txtFromDate.ClientID%>',
                ToDateControlSelector: '#<%=txtToDate.ClientID%>',
                TabParamName: '<%=TabParamName%>',
                DepartmentParamName: '<%=DepartmentParamName%>',
                FromDateParamName: '<%=FromDateParamName%>',
                ToDateParamName: '<%=ToDateParamName%>',

                FromDateErrorMsgSelector: '#<%=dtcFromDate.ClientID%>_ctl00',
                ToDateErrorMsgSelector: '#<%=dtcToDate.ClientID%>_ctl00'
            }
        };
        requestListInstance = new RBVH.Stada.WebPages.pages.RequestList(settings);
    });

    function submitForm() {
        window.WebForm_OnSubmit = function ()
        { return true; };
    }

    function onClientClickButton() {
        $(".se-pre-con").fadeIn(0);
        return submitForm();
    }
</script>
<style>
    #ctl00_PlaceHolderMain_linkAddNewItem:visited {
        color: white;
    }

    .lbl-fixed-width {
        padding-left: 8px !important;
    }

    .txtMonth {
        width: 100px !important;
    }
    .right{
        float: right;
    }
</style>
<%--<div class="border-container" style="display: inline-block; width: 100%;">--%>
<div>
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" id="MyRequestLi" runat="server" visible="false">
                <a href="#MyRequestsTab" aria-controls="MyRequestsTab" role="tab" data-toggle="tab" id="my-requests-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyRequests%>" />
                </a>
            </li>
            <li role="presentation" id="RequestToBeApprovedLi" runat="server" visible="false">
                <a href="#RequestsToBeApprovedTab" aria-controls="RequestsToBeApprovedTab" role="tab" data-toggle="tab" id="requests-to-be-approved-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsToBeApproved%>" />
                </a>
            </li>
            <li role="presentation" id="RequestByDepartmentLi" runat="server" visible="false">
                <a href="#RequestsByDepartmentTab" aria-controls="RequestsByDepartmentTab" role="tab" data-toggle="tab" id="requests-by-department-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsByDepartment%>" />
                </a>
            </li>
            <li role="presentation" id="RequestByReceivedDepartmentLi" runat="server" visible="false">
                <a href="#RequestsByReceivedDepartmentTab" aria-controls="RequestsByReceivedDepartmentTab" role="tab" data-toggle="tab" id="requests-by-received-department-tab">
                    <asp:Literal runat="server" Text=" <%$Resources:RBVHStadaWebpages,RequestsByReceivedDepartmentTab%>" />
                </a>
            </li>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane" id="MyRequestsTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div style="margin-bottom: 20px; margin-left: -5px">
                            <a id="linkAddNewItem" runat="server" href="#" class="btn btn-primary linkAddNewItem" visible="false">
                                <i class='fa fa-plus' aria-hidden='true'></i>
                                &nbsp;
                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,AddNewRequests%>" />
                            </a>
                        </div>
                        <asp:UpdatePanel ID="upMyRequests" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridMyRquests" runat="server" AutoGenerateColumns="false" CssClass="table gridView" DataKeyNames="ID">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" ItemStyle-Width="92px">
                                            <ItemTemplate>
                                                <%--                                                <span id="spanEdit" runat="server">
                                                    <a id="linkEdit" href="#" runat="server" class="table-action" style="margin-left: 10px; text-decoration: none;">
                                                        <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                                <span>
                                                    <a id="linkView" href="#" runat="server" class="table-action">
                                                        <i class="fa fa-eye" aria-hidden="true"></i>
                                                    </a>
                                                </span>--%>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Title%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_RequestTypeRef%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestType" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,Approval_ApprovalStatus%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_CreatedDate%>" ItemStyle-Width="140px">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button ID="btnCancelWF" runat="server" CausesValidation="false" CommandName="CancelWF" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_TerminateProcess%>"
                                                    CommandArgument='<%# Eval("ID") %>' CssClass="ms-ButtonHeightWidth btn btn-default" Enabled="false" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Left" CssClass="GridPager" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane" id="RequestsToBeApprovedTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <asp:UpdatePanel ID="upRequestToBeApproved" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridRequestToBeApproved" runat="server" AutoGenerateColumns="false" CssClass="table gridView">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" ItemStyle-Width="92px">
                                            <ItemTemplate>
                                                <%--                                                <span>
                                                    <a id="linkEdit" href="#" runat="server" class="table-action" style="margin-left: 10px; text-decoration: none;">
                                                        <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                                <span style="visibility: hidden;">
                                                    <!-- Alwyas invisible -->
                                                    <a id="linkView" href="#" runat="server" class="table-action">
                                                        <i class="fa fa-eye" aria-hidden="true"></i>
                                                    </a>
                                                    <a id="linkView" href="javascript:void(0);" runat="server" class="table-action">
                                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                    </a>
                                                </span>--%>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="true">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Title%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_RequestTypeRef%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestType" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Requester%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestFrom" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,Approval_ApprovalStatus%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_CreatedDate%>" ItemStyle-Width="140px">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Left" CssClass="GridPager" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane" id="RequestsByDepartmentTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                      <table style="table-layout: fixed; width: 100% !important">
                         <tr>
                            <td style="width: 100%" valign="top">
                               <div class="form-inline">
                                  <div class="form-group header-left lbl-fixed-width">
                                     <label>
                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CommonDepartment%>" />
                                     </label>
                                  </div>
                                  <div class="form-group">
                                      <asp:DropDownList ID="ddlDepartments" runat="server" CssClass="form-control" />
                                  </div>
                               </div>
                            </td>
                         </tr>
                         <tr>
                            <td style="width: 100%" valign="top">
                               <div class="form-inline pt10">
                                  <div class="form-group header-left lbl-fixed-width">
                                     <label>
                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarFromDate%>" />
                                     </label>
                                  </div>
                                  <div class="form-group">
                                     <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd; left: 80px;"></i>
                                        <asp:TextBox ID="txtFromDate" runat="server" CssClass="form-control" ReadOnly="true" Width="110px"></asp:TextBox>
                                     </div>
                                  </div>
                               </div>
                            </td>
                         </tr>
                          <tr>
                            <td style="width: 100%" valign="top">
                               <div class="form-inline pt10">
                                  <div class="form-group header-left lbl-fixed-width">
                                     <label>
                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarToDate%>" />
                                     </label>
                                  </div>
                                  <div class="form-group">
                                     <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd; left: 80px;"></i>
                                        <asp:TextBox ID="txtToDate" runat="server" CssClass="form-control" ReadOnly="true" Width="110px"></asp:TextBox>
                                     </div>
                                  </div>
                               </div>
                            </td>
                         </tr>
                        </table>
                        <asp:UpdatePanel ID="upRequestByDepartment" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridRequestByDepartment" runat="server" AutoGenerateColumns="false" CssClass="table gridView">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" ItemStyle-Width="92px">
                                            <ItemTemplate>
                                                <%--<span style="visibility: hidden;">
                                                    <!-- Alwyas invisible -->
                                                    <a id="linkEdit" href="#" runat="server" class="table-action" style="margin-left: 10px; text-decoration: none;">
                                                        <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                                <span>
                                                    <a id="linkView" href="#" runat="server" class="table-action">
                                                        <i class="fa fa-eye" aria-hidden="true"></i>
                                                    </a>
                                                </span>--%>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="true">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Title%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_RequestTypeRef%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestType" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Requester%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestFrom" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,Approval_ApprovalStatus%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_CreatedDate%>" ItemStyle-Width="140px">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Left" CssClass="GridPager" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane" id="RequestsByReceivedDepartmentTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <table style="table-layout: fixed; width: 100% !important">
                            <tr>
                                <td style="width: 100%" valign="top">
                                    <div class="form-inline">
                                        <div class="form-group header-left lbl-fixed-width">
                                            <label>
                                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsByReceivedDepartmentTab_FromDate%>" />
                                            </label>
                                        </div>
                                        <div class="form-group">
                                            <SharePoint:DateTimeControl ID="dtcFromDate" runat="server" LocaleId="2057" DateOnly="true" IsRequiredField="true" CssClassTextBox="form-control" CssClassDescription="ms-formvalidation" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%" valign="top">
                                    <div class="form-inline" style="margin-top: 10px; margin-bottom: 10px;">
                                        <div class="form-group header-left lbl-fixed-width">
                                            <label>
                                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsByReceivedDepartmentTab_ToDate%>" />
                                            </label>
                                        </div>
                                        <div class="form-group">
                                            <SharePoint:DateTimeControl ID="dtcToDate" runat="server" LocaleId="2057" DateOnly="true" IsRequiredField="true" CssClassTextBox="form-control" CssClassDescription="ms-formvalidation" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%" valign="top">
                                    <div class="form-inline" style="margin-bottom: 10px;">
                                        <div class="form-group header-left lbl-fixed-width">
                                            <label>
                                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RequestsByReceivedDepartmentTab_Status%>" />
                                            </label>
                                        </div>
                                        <div class="form-group">
                                            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 100%" valign="top">
                                    <div class="form-inline" style="margin-bottom: 10px;">
                                        <div class="form-group header-left lbl-fixed-width">
                                        </div>
                                        <div class="form-group">
                                            <asp:LinkButton ID="lbtnView" EnableViewState="false" Text="<%$Resources:RBVHStadaWebpages,Requests_btnView%>" runat="server" CssClass="ms-ButtonHeightWidth btn btn-primary" style="margin-left: 0px;" OnClientClick="return onClientClickButton();" />
                                            <asp:LinkButton ID="lbtnExport" EnableViewState="false" Text="<%$Resources:RBVHStadaWebpages,Requests_ExportExcel%>" runat="server" CssClass="ms-ButtonHeightWidth btn btn-primary" CausesValidation="true" OnClientClick="return submitForm();" />
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </table>
                        <asp:UpdatePanel ID="upRequestsByReceivedDepartment" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridRequestsByReceivedDepartment" runat="server" AutoGenerateColumns="false" CssClass="table gridView">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" ItemStyle-Width="92px">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="false" target="_blank">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="true" target="_blank">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Title%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_RequestTypeRef%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestType" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_Requester%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestFrom" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,Approval_ApprovalStatus%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaLists,RequestList_CreatedDate%>" ItemStyle-Width="140px">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle HorizontalAlign="Left" CssClass="GridPager" />
                                </asp:GridView>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

<asp:HiddenField ID="hdActivedTab" runat="server" Value="" />
