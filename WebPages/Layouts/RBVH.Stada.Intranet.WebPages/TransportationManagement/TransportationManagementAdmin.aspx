﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransportationManagementAdmin.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.TransportationManagement.TransportationManagementAdmin" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/TransportationManagementControl/TransportationByDepartmentControl.ascx" TagPrefix="transpotationDepartment" TagName="TransportationByDepartmentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/TransportationManagementControl/TransportationRequestControl.ascx" TagPrefix="transpotationRequest" TagName="TransportationRequestControl" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/TransportationModule/VehicleDepartment.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <ul class="nav nav-tabs" role="tablist">
        <li role="presentation" class="active"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Transportation_MyTransportationRequestTitle%>" />
        </a></li>
        <% if (IsAdminDepartment)
            { %>
        <li role="presentation"><a href="#tab3" aria-controls="tab3" role="tab" data-toggle="tab">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Transportation_TranspotationByDepartment%>" />
        </a></li>
        <% } %>
    </ul>
    <div class="tab-content">
        <div role="tabpanel" class="tab-pane active" id="tab1">
            <transpotationRequest:TransportationRequestControl id="TransportationRequestControl" runat="server" />
        </div>
        <% if (IsAdminDepartment)
            { %>
        <div role="tabpanel" class="tab-pane" id="tab3">
            <transpotationDepartment:TransportationByDepartmentControl id="TransportationByDepartmentControl" runat="server" />
        </div>
        <% } %>
    </div>
     <script type="text/javascript">
        $(document).ready(function () {
            var settings = {
                MonthControlSelector: '#txtTransportationDateByDepartment',
                DepartmentControlSelector: '#cbTranspotationAdminDepartment',
            };

             <% if (IsAdminDepartment){ %>
                InstanceTransportationByDepartment = new RBVH.Stada.WebPages.pages.TransportationByDepartment(settings);
             <% } %>

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
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,TransportationManagement_PageTitleArea%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,TransportationManagement_PageTitleArea%>" />
</asp:Content>
