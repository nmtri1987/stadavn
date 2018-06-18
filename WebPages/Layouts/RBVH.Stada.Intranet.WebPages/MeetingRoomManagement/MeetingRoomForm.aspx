<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MeetingRoomForm.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.MeetingRoomManagement.MeetingRoomForm" DynamicMasterPageFile="~masterurl/default.master" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/MeetingRoomManagementControl/MeetingRoomFormUserControl.ascx" TagPrefix="MeetingRoomForm" TagName="MeetingRoomFormUserControl" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <MeetingRoomForm:MeetingRoomFormUserControl ID="MeetingFormUserControl" runat="server" />
    <script type="text/javascript">
        function OnIframeLoadFinish() {
            ULSvmd:
            ;
            var picker;
            if (typeof this.Picker != 'undefined')
                picker = this.Picker;
            if (picker != null && typeof picker.readyState != 'undefined' && picker.readyState != null && picker.readyState == "complete") {
                document.body.scrollLeft = g_scrollLeft;
                document.body.scrollTop = g_scrollTop;
                g_scrollTop = document.getElementById('s4-workspace').scrollTop;
                picker.style.display = "block";
                if (typeof document.frames != 'undefined' && Boolean(document.frames)) {  /* "document.frames" doesn't work on chrome use "window.frames" instead*/
                    var frame = document.frames[picker.id];
                    if (frame != null && typeof frame.focus == 'function')
                        frame.focus();
                }
                else {
                    picker.focus();
                }
            }
            setTimeout(function () {
                document.getElementById('s4-workspace').scrollTop = g_scrollTop;
            }, 1);
        }
</script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoom_PageTitle%>" /><%=FormTitle%>
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoom_TitleInTitleArea%>" /><%=FormTitle%>
</asp:Content>
