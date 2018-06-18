<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShiftManagementAdmin.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ShiftManagement.ShiftManagementAdmin" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/ShiftManagement/MyShiftControl.ascx" TagPrefix="ms" TagName="MyShiftControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/ShiftManagement/ShiftForDepartmentControl.ascx" TagPrefix="sd" TagName="ShiftForDepartmentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/ShiftManagement/ShiftRequestControl.ascx" TagPrefix="sr" TagName="ShiftRequestControl" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/ShiftModule/default.css?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/ShiftByDepartment.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_MyShiftTitle%>" />
            </a></li>
            <li role="presentation"><a href="#tab2" aria-controls="tab2" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_MyShiftRequestTitle%>" />
            </a></li>
            <% if (IsAdminDepartment)
                { %>
                    <li role="presentation">
                        <a href="#tab3" aria-controls="tab3" role="tab" data-toggle="tab">
                            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_ShiftsInDepartmentTitle%>" />
                        </a>
                    </li>
            <% } %>
            
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="tab1">
                <ms:MyShiftControl id="myShiftControlId" runat="server" />
            </div>
            <div role="tabpanel" class="tab-pane" id="tab2">
                <sr:ShiftRequestControl id="shiftControlRequestId" runat="server" />
            </div>
            <% if (IsAdminDepartment)
                { %>
            <div role="tabpanel" class="tab-pane" id="tab3">
                <sd:ShiftForDepartmentControl id="shiftDepartmentControlId" runat="server" />
            </div>
            <% } %>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
        <% if (IsAdminDepartment)
        { %>
            var settings = {
                MonthControlSelector: '#txtShiftDateByDepartment',
                DepartmentControlSelector: '#cbShiftAdminDepartment',
                CurrentDepartmentId: 0 };
            shiftByDepartmentInstance = new RBVH.Stada.WebPages.pages.ShiftByDepartment(settings);
            
            
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
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_PageTitleArea%>" />
</asp:Content>
