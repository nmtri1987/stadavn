<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeaveManagementMember.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.LeaveManagement.LeaveManagementMember" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/LeaveManagementControl/LeaveApprovalControl.ascx" TagPrefix="LeaveApproval" TagName="LeaveApprovalControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/LeaveManagementControl/LeaveRequestControl.ascx" TagPrefix="LeaveRequest" TagName="LeaveRequestControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/LeaveManagementControl/LeaveSecurityGuardControl.ascx" TagPrefix="LeaveSecurity" TagName="LeaveSecurityGuardControl" %>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_MyLeaveRequestTitle%>" />
            </a></li>

            <% if (isTeamLeader || isShiftLeader)
                {%>
            <li role="presentation"><a href="#tab2" aria-controls="tab2" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_LeaveApprovalTitle%>" />
            </a>
            </li>
            <%} %>
            <% if (isSecurityGuard)
                {%>
            <li role="presentation"><a href="#tab4" aria-controls="tab4" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_LeaveSecurityTitle%>" />
            </a></li>
            <%} %>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="tab1">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <LeaveRequest:LeaveRequestControl id="leaveRequestControl" runat="server"></LeaveRequest:LeaveRequestControl>
                    </div>
                </div>
            </div>
            <% if (isTeamLeader || isShiftLeader)
                {%>
            <div role="tabpanel" class="tab-pane" id="tab2">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <LeaveApproval:LeaveApprovalControl id="leaveApprovalControl" runat="server" />
                    </div>
                </div>
            </div>
            <%} %>
            <% if (isSecurityGuard)
                {%>
            <div role="tabpanel" class="tab-pane" id="tab4">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <LeaveSecurity:LeaveSecurityGuardControl id="leaveSecurityControl" runat="server"></LeaveSecurity:LeaveSecurityGuardControl>
                    </div>
                </div>
            </div>
            <%} %>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var targetTab = $(e.target).attr("href") // activated tab
                window.location.hash = targetTab;
            });

            //Get current tabe and set active
            var hashTab = window.location.hash;
            if (!hashTab) {
                hashTab = "#tab1";
            }
           
            window.location.hash = hashTab;
            $('[href="' + hashTab + '"]').trigger('click');
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_PageTitleArea%>" />
</asp:Content>
