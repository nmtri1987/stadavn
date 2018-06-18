<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="RBVH.Stada.Intranet.WebPages.Utils" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
        <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/css/slick.grid.css" runat="server" />
    <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/controls/slick.pager.css" runat="server" />
    <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/smoothness/jquery-ui-1.8.24.custom.css" runat="server" />
    <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/examples.css" runat="server" />
    <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/controls/slick.columnpicker.css" runat="server" />

    <SharePoint:ScriptLink ID="ScriptLink6" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/lib/firebugx.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink1" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/lib/jquery-1.7.min.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink2" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/lib/jquery-ui-1.8.24.custom.min.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink3" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/lib/jquery.event.drag-2.2.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink4" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/lib/jquery.mousewheel.js" runat="server" />

    <SharePoint:ScriptLink ID="ScriptLink5" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/js/slick.core.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink9" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/js/slick.editors.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink10" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/js/slick.formatters.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink16" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/plugins/slick.checkboxselectcolumn.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink11" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/plugins/slick.rowselectionmodel.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink15" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/plugins/slick.cellrangedecorator.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink7" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/js/slick.grid.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink12" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/js/slick.dataview.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink13" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/controls/slick.pager.js" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink14" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Slickgrid/controls/slick.columnpicker.js" runat="server" />
     <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/css/ShiftModule/gird-style.css" runat="server" />
    <SharePoint:ScriptLink ID="ScriptLink8" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/ViewShiftManagementPage.js" runat="server" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
        <table style="table-layout: fixed; width: 100% !important" class="main-table">
        <tr>
            <td>
                <br />
                <!--Slick grid here-->
                <div id="ViewShiftRequestGrid" style="width: 100%;"></div>
                <div id="viewShiftGridNodata" style="display:none"></div>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <div class="row">
                    <div class="col-md-offset-9 col-md-3">
                        <div class="row">
                            <button id="btnViewShiftBack" class="ms-ButtonHeightWidth" style="margin-right:15px !important; font-weight: normal !important; float: right;"><asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BackButton%>" /></button>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <script  type="text/javascript">
        ViewShiftRequestPage.EmployeeIdGridColumn = '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_EmployeeIdGridColumn%>" />';
        ViewShiftRequestPage.EmployeeNameGridColumn = '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_EmployeeNameGridColumn%>" />';
        ViewShiftRequestPage.BackButton = '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,BackButton%>" />';
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ViewShiftManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ViewShiftManagement_PageTitleArea%>" />
</asp:Content>

