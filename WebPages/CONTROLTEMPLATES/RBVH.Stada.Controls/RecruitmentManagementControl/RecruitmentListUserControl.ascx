<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="RBVH.Stada.Intranet.WebPages.Common" %>
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecruitmentListUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RecruitmentManagementControl.RecruitmentListUserControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css"/>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/URI/URI.js?v=E7297DEA-F322-413F-A8CE-8BEBBC316B66"></script>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/Common/BaseListPage.css?v=8A39B87C-8210-4F14-A8A5-5E336E9B6377" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/Common/ListBasePage.js?v=25BD2447-8482-4488-A9D8-DD1001BDFE61"></script>

<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                ActivedTabSelector: '#<%=hdActivedTab.ClientID%>',
                DepartmentSelector: '#<%=ddlDepartments.ClientID%>',
                MonthControlSelector: '#<%=txtMonth.ClientID%>',
                TabParamName: '<%=TabParamName%>',
                DepartmentParamName: '<%=DepartmentParamName%>',
                MonthParamname: '<%=MonthParamname%>',
                YearParamName: '<%=YearParamName%>',
            }
        };
        listBasePageInstance = new RBVH.Stada.WebPages.pages.ListBasePage(settings);
    });
</script>

<div>
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" id="MyRequestLi" runat="server" visible="false">
                <a href="#MyRequestsTab" aria-controls="MyRequestsTab" role="tab" data-toggle="tab" id="my-requests-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyRecruitmentRequests%>" />
                </a>
            </li>
            <li role="presentation" id="RequestToBeApprovedLi" runat="server" visible="false">
                <a href="#RequestsToBeApprovedTab" aria-controls="RequestsToBeApprovedTab" role="tab" data-toggle="tab" id="requests-to-be-approved-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruitmentRequestsApproval%>" />
                </a>
            </li>
            <li role="presentation" id="RequestByDepartmentLi" runat="server" visible="false">
                <a href="#RequestsByDepartmentTab" aria-controls="RequestsByDepartmentTab" role="tab" data-toggle="tab" id="requests-by-department-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruitmentRequestsByDepartment%>" />
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
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                            <asp:TemplateField HeaderText="Title">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litTitle" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>--%>
                                           <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Department%>">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litDepartment" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Position%>">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litPosition" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Quantity%>">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litQuantity" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Status%>">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblStatus" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_CreatedDate%>">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litCreatedDate" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_IsValid%>">
                                                <ItemTemplate>
                                                    <asp:Label ID="litIsValid" runat="server" CssClass="" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Button ID="btnCancelWF" runat="server" CausesValidation="false" CommandName="CancelWF" Text="<%$Resources:RBVHStadaWebpages,FormButtonsControl_TerminateProcess%>" 
                                                        CommandArgument='<%# Eval("ID") %>' CssClass="ms-ButtonHeightWidth btn btn-default" Enabled="false" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <PagerStyle HorizontalAlign = "Left" CssClass = "GridPager" />
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
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="true">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:TemplateField HeaderText="Title">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_RequestFrom%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestFrom" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Department%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDepartment" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Position%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litPosition" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Quantity%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litQuantity" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Status%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_CreatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_IsValid%>">
                                            <ItemTemplate>
                                                <asp:Label ID="litIsValid" runat="server" CssClass="" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:TemplateField HeaderText="Edit | View">
                                            <ItemTemplate>
                                                <span>
                                                    <a id="linkEdit" href="#" runat="server" class="table-action" style="margin-left: 10px; text-decoration: none;">
                                                        <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                                <!--always invisible -->
                                                <span style="visibility: hidden;">
                                                    <a id="linkView" href="#" runat="server" class="table-action">
                                                        <i class="fa fa-eye" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                    </Columns>
                                    <PagerStyle HorizontalAlign = "Left" CssClass = "GridPager" />
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
                                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarMonth%>" />
                                     </label>
                                  </div>
                                  <div class="form-group">
                                     <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                        <asp:TextBox ID="txtMonth" runat="server" CssClass="form-control txtMonth" ReadOnly="true"></asp:TextBox>
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
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" visible="false">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                                <a id="linkView" href="javascript:void(0);" runat="server" visible="true">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:TemplateField HeaderText="Title">
                                            <ItemTemplate>
                                                <asp:Literal ID="litTitle" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_RequestFrom%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequestFrom" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Department%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDepartment" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Position%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litPosition" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Quantity%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litQuantity" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_Status%>">
                                            <ItemTemplate>
                                                <asp:Label ID="lblStatus" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_CreatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,RecruitmentList_IsValid%>">
                                            <ItemTemplate>
                                                <asp:Label ID="litIsValid" runat="server" CssClass="" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
<%--                                        <asp:TemplateField HeaderText="Edit | View">
                                            <ItemTemplate>
                                                <!--always invisible -->
                                                <span style="visibility: hidden;">
                                                    <a id="linkEdit" href="#" runat="server" class="table-action" style="margin-left: 10px; text-decoration: none;">
                                                        <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                                <span>
                                                    <a id="linkView" href="#" runat="server" class="table-action">
                                                        <i class="fa fa-eye" aria-hidden="true"></i>
                                                    </a>
                                                </span>
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
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