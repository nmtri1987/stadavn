<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusinessTripManagementManager.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.BusinessTripManagement.BusinessTripManagementManager" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripApprovalControl.ascx" TagPrefix="BusinessTripApproval" TagName="BusinessTripApprovalControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripByDepartmentControl.ascx" TagPrefix="BusinessTripByDepartment" TagName="BusinessTripByDepartmentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripRequestControl.ascx" TagPrefix="BusinessTripRequest" TagName="BusinessTripRequestControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/MyBusinessTripControl.ascx" TagPrefix="MyBusinessTrip" TagName="MyBusinessTripControl" %>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/BusinessTripModule/BusinessTripByDepartment.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <% if (hasRequestPermission)
            {%>
            <li role="presentation"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_MyBusinessTripRequestTitle%>" />
            </a></li>
            <%} %>
            <% else
            {%>
            <li role="presentation" class="active"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_MyBusinessTripTitle%>" />
            </a></li>
            <%} %>

            <li role="presentation" class="active"><a href="#tab2" aria-controls="tab2" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_ApprovalListTitle%>" />
            </a></li>
            <li role="presentation"><a href="#tab3" aria-controls="tab3" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_RequestByDepartmentTitle%>" />
            </a></li>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <% if (hasRequestPermission)
            {%>
            <div role="tabpanel" class="tab-pane" id="tab1">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <BusinessTripRequest:BusinessTripRequestControl id="businessTripRequestControl" runat="server"></BusinessTripRequest:BusinessTripRequestControl>
                    </div>
                </div>
            </div>
            <%} %>
            <% else
            {%>
            <div role="tabpanel" class="tab-pane active" id="tab1">
                <div class="panel panel-primary">
                    <div class="panel-body">
                       <MyBusinessTrip:MyBusinessTripControl id="myBusinessTripControl" runat="server"></MyBusinessTrip:MyBusinessTripControl>
                    </div>
                </div>
            </div>
            <%} %>
            <div role="tabpanel" class="tab-pane active" id="tab2">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <BusinessTripApproval:BusinessTripApprovalControl id="businessTripApprovalControl" runat="server" />
                    </div>
                </div>
            </div>
            <div role="tabpanel" class="tab-pane" id="tab3">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <BusinessTripByDepartment:BusinessTripByDepartmentControl id="businessTripByDepartmentControl" runat="server" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                var targetTab = $(e.target).attr("href") // activated tab
                window.location.hash = targetTab;
            });
            var settings = {
                MonthControlSelector: '#txtBusinessTripMonth',
                DepartmentControlSelector: '#cbBusinessTripDepartment',
                CurrentDepartmentId: <%= IsAdminDepartment ? 0 : CurrentDepartmentId %> };

            businessTripByDepartment = new RBVH.Stada.WebPages.pages.BusinessTripByDepartment(settings);

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
 <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_PageTitleArea%>" />
</asp:Content>
