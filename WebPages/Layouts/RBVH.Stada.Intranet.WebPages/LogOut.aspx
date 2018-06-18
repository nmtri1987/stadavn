<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LogOut.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.LogOut" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Logout_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,Logout_PageTitleArea%>" />
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

    <div class="container">
        <div id="Div_Messages" runat="server">
            <%--Messages--%>
            <div id="Div_Success" class="alert alert-success" visible="false" runat="server">
                <a href="#" class="close">&times;</a>
                <strong>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,General_Prefix_Success%>" /></strong>
                <asp:Label ID="Label_Success" runat="server"></asp:Label>
            </div>
            <div id="Div_Info" class="alert alert-info hidden" visible="false" runat="server">
                <a href="#" class="close">&times;</a>
                <strong>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,General_Prefix_Info%>" /></strong>
                <asp:Label ID="Label_Info" runat="server"></asp:Label>
            </div>
            <div id="Div_Warning" class="alert alert-warning" visible="false" runat="server">
                <a href="#" class="close">&times;</a>
                <strong>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,General_Prefix_Warning%>" /></strong>
                <asp:Label ID="Label_Warning" runat="server"></asp:Label>
            </div>
            <div id="Div_Error" class="alert alert-danger" visible="false" runat="server">
                <a href="#" class="close">&times;</a>
                <strong>
                    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,General_Prefix_Error%>" /></strong>
                <asp:Label ID="Label_Error" runat="server"></asp:Label>
            </div>
        </div>
    </div>

</asp:Content>
