<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangeShiftManagementAdmin.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.ChangeShiftManagement.ChangeShiftManagementAdmin" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/ChangeShiftManagementControl/ChangeShiftByDepartmentControl.ascx" TagPrefix="ChangeShiftByDepartment" TagName="ChangeShiftByDepartmentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/ChangeShiftManagementControl/ChangeShiftRequestControl.ascx" TagPrefix="ChangeShiftRequest" TagName="ChangeShiftRequestControl" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ChangeShiftModule/ChangeShiftByDepartment.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
        <ul class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active"><a href="#tab1" aria-controls="tab1" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShift_MyChangeShiftRequestTitle%>" />
            </a></li>
            <% if (IsAdminDepartment)
                { %>
            <li role="presentation"><a href="#tab3" aria-controls="tab3" role="tab" data-toggle="tab">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShift_ChangeShiftByDepartment%>" />
            </a></li>
            <% } %>
        </ul>
        <!-- Tab panes -->
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane active" id="tab1">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <ChangeShiftRequest:ChangeShiftRequestControl id="ChangeShiftRequestControl" runat="server"></ChangeShiftRequest:ChangeShiftRequestControl>
                    </div>
                </div>
            </div>
            <% if (IsAdminDepartment)
                { %>
            <div role="tabpanel" class="tab-pane" id="tab3">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <ChangeShiftByDepartment:ChangeShiftByDepartmentControl id="ChangeShiftByDepartmentControl" runat="server"></ChangeShiftByDepartment:ChangeShiftByDepartmentControl>
                    </div>
                </div>
            </div>
            <% } %>
        </div>
    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            <% if (IsAdminDepartment)
        { %>
            var settings = {
                MonthControlSelector: '#txtChangeShiftDateByDepartment',
                DepartmentControlSelector: '#cbChangeShiftAdminDepartment',
                CurrentDepartmentId: 0
            };
            changeShiftByDepartmentInstance = new RBVH.Stada.WebPages.pages.ChangeShiftByDepartment(settings);
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
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShiftManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShiftManagement_PageTitleArea%>" />
</asp:Content>
