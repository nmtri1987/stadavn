<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DelegationListUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.DelegationManagementControl.DelegationListUserControl" %>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/DelegationModule/DelegationList.css?v=51BB1B33-C143-47A8-BCB2-1EDDCAB3E208" />
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/DelegationModule/DelegationList.js?v=2540-C0C7-4334-B4AF-5AA37858D8C7"></script>

<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                ActivedTabSelector: '#<%=hdActivedTab.ClientID%>',
                linkAddNewDelegationSelector: '#<%=linkAddNewDelegation.ClientID%>',
                linkAddNewDelegationOfNewTaskSelector: '#<%=linkAddNewDelegationOfNewTask.ClientID%>',
                linkDeleteDelegationSeletor: '#<%=lnkbtnDeleteDelegation.ClientID%>',
                linkDeleteNewDelegationSelector: '#<%=lnkbtnDeleteNewDelegation.ClientID%>',
                txtDelegateFromDateSelector: '#<%=txtDelegateFromDate.ClientID%>',
                txtDelegateToDateSelector: '#<%=txtDelegateToDate.ClientID%>',
                btnViewMyDelegationsSelector: '#<%=btnViewMyDelegations.ClientID%>',
                btnViewMyDelegationsNewTaskSelctor: '#<%=btnViewMyDelegationsNewTask.ClientID%>',
                txtDelegateNewTaskFromDateSelector: '#<%=txtDelegateNewTaskFromDate.ClientID%>',
                txtDelegateNewTaskToDateSelector: '#<%=txtDelegateNewTaskToDate.ClientID%>',
                txtDelegateNewTaskFromDateSelector_Error: "#txtDelegateNewTaskFromDate_Error",
                txtDelegateToDateSelector_Error: "#txtDelegateToDate_Error"
            },
            Resources:
            {
                PageTitle: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DelegationManagement_PageTitle%>' />",
                SelectAtLeastOneItem: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DelegationManagement_SelectOneItem%>' />",
                ConfirmDeleteItems: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DelegationManagement_ConfirmDelete%>' />",
                ToDateGreaterThanFromDate:  "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DelegationManagement_FilterToDateFromDate%>' />",
            }
        };
        delegationListInstance = new RBVH.Stada.WebPages.pages.DelegationList(settings);
    });
</script>

<div>
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" id="MyDelegationsLi" runat="server" visible="false">
                <a href="#MyDelegationsTab" aria-controls="MyDelegationsTab" role="tab" data-toggle="tab" id="my-delegations-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyDelegations%>" />
                </a>
            </li>
            <li role="presentation" id="MyDelegationsOfNewTaskLi" runat="server" visible="false">
                <a href="#MyDelegationsOfNewTaskTab" aria-controls="MyDelegationsOfNewTaskTab" role="tab" data-toggle="tab" id="my-delegations-of-new-task-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyDelegationsOfNewTask%>" />
                </a>
            </li>
            <li role="presentation" id="DelegationsApprovalLi" runat="server" visible="false">
                <a href="#DelegationsApprovalTab" aria-controls="DelegationsApprovalTab" role="tab" data-toggle="tab" id="delegations-approval-tab">
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,DelegationsApproval%>" />
                </a>
            </li>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane" id="MyDelegationsTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div style="margin-bottom: 20px; margin-left: -5px">
                            <a id="linkAddNewDelegation" runat="server" href="javascript:void(0);" class="btn btn-primary linkAddNewItem" visible="true">
                                <i class='fa fa-plus' aria-hidden='true'></i>&nbsp;<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,AddNewDelegation%>" />
                            </a>
                            <asp:LinkButton ID="lnkbtnDeleteDelegation" CssClass="btn btn-danger" Style="float: right;" runat="server" OnClientClick="return delegationListInstance.ConfirmDeleteDelegationItems();" CausesValidation="false" disabled>
                                <i class='fa fa-remove' aria-hidden='true'></i>&nbsp;<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,DeleteDelegation%>" />
                            </asp:LinkButton>
                        </div>
                        <div class="form-inline filter-delegation">
                            <div class="form-group header-left">
                                <label>
                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_CreatedDelegationDateFrom%>" />
                                </label>
                            </div>
                            <div class="form-group">
                                <div class="input-append  date inner-addon right-addon txtCalendar" id="dpDelagetionFromDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                    <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                    <asp:TextBox ID="txtDelegateFromDate" runat="server" Width="100%" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group header-left">
                                <label>
                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_CreatedDelegationDateTo%>" />
                                </label>
                            </div>
                            <div class="form-group">
                                <div class="input-append  date inner-addon right-addon txtCalendar" id="dpDelegationToDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                    <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                    <asp:TextBox ID="txtDelegateToDate" Width="100%" runat="server" CssClass="form-control" />
                                </div>
                                 <span id="txtDelegateToDate_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                            </div>
                            <div class="form-group">
                                <asp:Button ID="btnViewMyDelegations" runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_BtnView%>"  OnClientClick="return delegationListInstance.ValidateFilterMyDelegation();"/>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="upMyDelegations" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridMyDelegations" runat="server" AutoGenerateColumns="false" CssClass="table gridView" DataKeyNames="ID" ShowHeaderWhenEmpty="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" class="linkEdit">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_ToEmployee%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litToEmployee" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_FromDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFromDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_ToDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litToDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Module%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litModule" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Description%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDescription" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Requester%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequester" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Department%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDepartment" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_CreatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_DelegatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDelegatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <%--            <asp:TemplateField HeaderStyle-Width="100px">
                                            <ItemTemplate>
                                                <asp:Button ID="btnDeleteDelagation" runat="server" CausesValidation="false" OnClientClick="return delegationListInstance.ConfirmDeleteDelegation(this);" confirmation-message="<%$Resources:RBVHStadaWebpages,GridViewItem_ConfirmDeleteItem%>" CommandName="DeleteDelegation" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_DeleteLink%>" 
                                                    CommandArgument='<%# Eval("ID") %>' CssClass="ms-ButtonHeightWidth btn btn-default" />
                                            </ItemTemplate>
                                        </asp:TemplateField>--%>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="cbSelectDeleteAll" runat="server" CssClass="select-all-task-delete" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSelectDelete" runat="server" CssClass="select-task-delete" />
                                                <asp:HiddenField ID="hdSelectDeleteID" runat="server" Value="" />
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
            <div role="tabpanel" class="tab-pane" id="MyDelegationsOfNewTaskTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div style="margin-bottom: 20px; margin-left: -5px">
                            <a id="linkAddNewDelegationOfNewTask" runat="server" href="javascript:void(0);" class="btn btn-primary linkAddNewItem" visible="true">
                                <i class='fa fa-plus' aria-hidden='true'></i>&nbsp;<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,AddNewDelegationOfNewTask%>" />
                            </a>
                            <asp:LinkButton ID="lnkbtnDeleteNewDelegation" CssClass="btn btn-danger" Style="float: right;" runat="server" CausesValidation="false" disabled OnClientClick="return delegationListInstance.ConfirmDeleteNewDelegationItems();">
                                <i class='fa fa-remove' aria-hidden='true'></i>&nbsp;<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,DeleteDelegation%>" />
                            </asp:LinkButton>
                        </div>
                        <div class="form-inline filter-delegation">
                            <div class="form-group header-left">
                                <label>
                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_CreatedDelegationDateFrom%>" />
                                </label>
                            </div>
                            <div class="form-group">
                                <div class="input-append  date inner-addon right-addon txtCalendar" id="dpDelagationNewTaskFromDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                    <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                    <asp:TextBox ID="txtDelegateNewTaskFromDate" runat="server" Width="100%" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group header-left">
                                <label>
                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_CreatedDelegationDateTo%>" />
                                </label>
                            </div>
                            <div class="form-group">
                                <div class="input-append  date inner-addon right-addon txtCalendar" id="dpDelagationNewTaskToDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                    <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                    <asp:TextBox ID="txtDelegateNewTaskToDate" Width="100%" runat="server" CssClass="form-control" />
                                </div>
                                <span id="txtDelegateNewTaskFromDate_Error" class="ms-formvalidation" style="margin-top: 0px;"></span>
                            </div>
                            <div class="form-group">
                                <asp:Button ID="btnViewMyDelegationsNewTask" runat="server" Text="<%$Resources:RBVHStadaWebpages,Delegation_BtnView%>"  OnClientClick="return delegationListInstance.ValidateFilterNewTaskDelegation();"/>
                            </div>
                        </div>
                        <asp:UpdatePanel ID="upMyDelegationsOfNewTask" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridMyDelegationsOfNewTask" runat="server" AutoGenerateColumns="false" CssClass="table gridView" ShowHeaderWhenEmpty="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Module%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litModule" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_ToEmployee%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litToEmployee" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_FromDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFromDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_ToDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litToDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_DelegatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDelegatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="cbSelectDeleteAllNewTask" runat="server" CssClass="select-all-newtask-delete" />
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="cbSelectDeleteNewTask" runat="server" CssClass="select-newtask-delete" />
                                                <asp:HiddenField ID="hdSelectDeleteID" runat="server" Value="" />
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
            <div role="tabpanel" class="tab-pane" id="DelegationsApprovalTab" runat="server" visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <asp:UpdatePanel ID="upDelegationsApproval" runat="server">
                            <ContentTemplate>
                                <asp:GridView ID="gridDelegationsApproval" runat="server" AutoGenerateColumns="false" CssClass="table gridView" ShowHeaderWhenEmpty="true">
                                    <Columns>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>">
                                            <ItemTemplate>
                                                <a id="linkEdit" href="javascript:void(0);" runat="server" class="linkEdit">
                                                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GridViewItem_ViewDetail%>" />
                                                </a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_FromEmployee%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFromEmployee" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_FromDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litFromDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_ToDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litToDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Module%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litModule" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Description%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDescription" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Requester%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litRequester" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_Department%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDepartment" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_CreatedDate%>" HeaderStyle-Width="150px">
                                            <ItemTemplate>
                                                <asp:Literal ID="litCreatedDate" runat="server" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="<%$Resources:RBVHStadaWebpages,DelegationsList_DelegatedDate%>">
                                            <ItemTemplate>
                                                <asp:Literal ID="litDelegatedDate" runat="server" />
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

