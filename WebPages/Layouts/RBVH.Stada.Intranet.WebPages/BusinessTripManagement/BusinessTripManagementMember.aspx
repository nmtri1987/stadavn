<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BusinessTripManagementMember.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.BusinessTripManagement.BusinessTripManagementMember" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripCashierControl.ascx" TagPrefix="BusinessTripCashier" TagName="BusinessTripCashierControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripDriverControl.ascx" TagPrefix="BusinessTripDriver" TagName="BusinessTripDriverControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/MyBusinessTripControl.ascx" TagPrefix="MyBusinessTrip" TagName="MyBusinessTripControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripRequestControl.ascx" TagPrefix="BusinessTripRequest" TagName="BusinessTripRequestControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/BusinessTripManagementControl/BusinessTripByExtAdminControl.ascx" TagPrefix="BusinessTripByExtAdmin" TagName="BusinessTripByExtAdminControl" %>
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
            
            <% if (IsExtAdmin == true)
                {%>
            <li role="presentation"><a href="#tab3" aria-controls="tab3" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_RequestByDepartmentTitle%>" />
            </a></li>
            <%} %>

            <% if (IsCashier == true)
                {%>
            <li role="presentation" ><a href="#tab4" aria-controls="tab4" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_BusniessTripForCashier%>" />
            </a></li>
            <%} %>
            <% if (IsDriver == true)
                {%>
            <li role="presentation" ><a href="#tab5" aria-controls="tab5" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BusinessTripManagement_BusinessTripForDriver%>" />
            </a></li>
            <%} %>
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

            <% if (IsExtAdmin == true)
                {%>
            <div role="tabpanel" class="tab-pane" id="tab3">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <BusinessTripByExtAdmin:BusinessTripByExtAdminControl id="businessTripByExtAdminControl" runat="server" />
                    </div>
                </div>
            </div>
            <%} %>

            <% if (IsCashier == true){%>
            <div role="tabpanel" class="tab-pane" id="tab4">
                <div class="panel panel-primary">
                    <div class="panel-body">
                       <BusinessTripCashier:BusinessTripCashierControl id="businessTripCashierControl" runat="server"></BusinessTripCashier:BusinessTripCashierControl>
                    </div>
                </div>
            </div>
            <%} %>
               <% if (IsDriver == true){%>
            <div role="tabpanel" class="tab-pane" id="tab5">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <BusinessTripDriver:BusinessTripDriverControl id="businessTripDriverControl" runat="server"></BusinessTripDriver:BusinessTripDriverControl>
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

            <% if (IsExtAdmin){%>
            var settings = {
                MonthControlSelector: '#txtBusinessTripMonth',
                DepartmentControlSelector: '#cbBusinessTripDepartment',
                CurrentDepartmentId: 0 };

            businessTripByExtAdmin = new RBVH.Stada.WebPages.pages.BusinessTripByDepartment(settings);
            <%} %>

            //Get current tabe and set active
            var defaultTab = "#tab1";
            <% if (IsCashier){%>
                defaultTab = "#tab4";
            <%} %>

            <% if (IsDriver){%>
                defaultTab = "#tab5";
            <%} %>

            var hashTab = window.location.hash;
            if (!hashTab) {
                hashTab = defaultTab;
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
