<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FreightManagementMember.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.FreightManagement.FreightManagementMember" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/FreightManagementControl/MyFreightControl.ascx" TagPrefix="MyFreight" TagName="MyFreightControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/FreightManagementControl/FreightRequestControl.ascx" TagPrefix="FreightRequest" TagName="FreightRequestControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/FreightManagementControl/FreightSecurityGuardControl.ascx" TagPrefix="FreightSecurity" TagName="FreightSecurityGuardControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/FreightManagementControl/VehicleOperatorControl.ascx" TagPrefix="VehicleOperator" TagName="VehicleOperatorControl" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <% if (!isSecurityGuard)
                {%>
            <li role="presentation" class="active"><a href="#tab0" aria-controls="tab0" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_MyFreightTitle%>" />
            </a></li>
            <%} %>

            <% if (hasRequestPermission)
            {%>
            <li role="presentation"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_MyFreightRequestTitle%>" />
            </a></li>
            <%} %>

            <% if (isSecurityGuard)
                {%>
            <li role="presentation"><a href="#tab4" aria-controls="tab4" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_FreightSecurityTitle%>" />
            </a></li>
            <%} %>
            <% if (isVehicleOperator)
                {%>
            <li role="presentation"><a href="#tab5" aria-controls="tab5" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_VehicleOperatorTitle%>" />
            </a></li>
            <%} %>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <% if (!isSecurityGuard)
                {%>
            <div role="tabpanel" class="tab-pane active" id="tab0">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <MyFreight:MyFreightControl id="myFreightControl" runat="server"></MyFreight:MyFreightControl>
                    </div>
                </div>
            </div>
            <%} %>

            <% if (hasRequestPermission)
            {%>
            <div role="tabpanel" class="tab-pane" id="tab1">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <FreightRequest:FreightRequestControl id="freightRequestControl" runat="server"></FreightRequest:FreightRequestControl>
                    </div>
                </div>
            </div>
            <%} %>

            <% if (isSecurityGuard)
                {%>
            <div role="tabpanel" class="tab-pane" id="tab4">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <FreightSecurity:FreightSecurityGuardControl id="freightSecurityControl" runat="server"></FreightSecurity:FreightSecurityGuardControl>
                    </div>
                </div>
            </div>
            <%} %>
             <% if (isVehicleOperator)
                {%>
            <div role="tabpanel" class="tab-pane" id="tab5">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <VehicleOperator:VehicleOperatorControl id="freightVehicleOperatorControl" runat="server"></VehicleOperator:VehicleOperatorControl>
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
            var defaultTab = "#tab0";
            <% if (isSecurityGuard){%>
                defaultTab = "#tab4";
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
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightManagement_PageTitleArea%>" />
</asp:Content>
