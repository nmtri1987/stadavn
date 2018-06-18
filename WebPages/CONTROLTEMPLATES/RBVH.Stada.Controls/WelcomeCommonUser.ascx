<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WelcomeCommonUser.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.WelcomeCommonUser" %>
<div class="ms-menu-althov ms-welcome-root">
    <a class="ms-core-menu-root" data-toggle="dropdown">
        <asp:Literal ID="EmployeeNameLiteral" runat="server" />
        <span class="caret"></span>
    </a>
    <ul class="dropdown-menu">
        <li><a id="Link_ChangePassword" href="/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangePassword.aspx" runat="server">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangePassword_PageTitleArea%>" /></a></li>
        <li><a id="Link_Logout" href="/_layouts/15/RBVH.Stada.Intranet.WebPages/LogOut.aspx" runat="server">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Logout_PageTitleArea%>" /></a></li>
    </ul>
</div>
<script type="text/javascript">
    function showWelcomeCommon()
    {
        $("#welcomeMenuBox").hide();
        $("#welcomeCommonMenuBox").show();
        $(".dropdown-menu").css("margin", "2px -58px 0");
        hideMenuTopBar();
    }

    function hideWelcomeCommon() {
        $("#welcomeMenuBox").hide();
        $("#welcomeCommonMenuBox").hide();
        hideMenuTopBar();
    }

    function showWelcome() {
        $("#welcomeMenuBox").show();
        $("#welcomeCommonMenuBox").hide();

        $("#suiteBarButtons").show();
        $("#suiteLinksBox").show();
        $("#ctl00_site_share_button").show();
        $("#site_follow_button").show();
        $("#SearchBox").show();
    }

    function hideMenuTopBar() {
        $("#suiteBarButtons").hide();
        $("#suiteLinksBox").hide();
        $("#ctl00_site_share_button").hide();
        $("#site_follow_button").hide();
        $("#SearchBox").hide();
    }

    var _rbvhContext = {};

    _rbvhContext.EmployeeInfo = JSON.parse('<%= EmployeeInfo%>');
</script>
