<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeaveManagementApprovalTask.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.LeaveManagementApprovalTask" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main"><ZoneTemplate>
		<WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" 
            ServerRender="False" ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" 
            IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle="" 
            ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False" DisplayName="Approval Task List" ViewContentTypeId="" 
            Default="TRUE" ListUrl="Lists/LeaveManagement" ListDisplayName="" 
             PageType="PAGE_DEFAULTVIEW" PageSize="-1" UseSQLDataSourcePaging="True" DataSourceID="" 
            ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False"
             AutoRefreshInterval="60" Title="Leave Management" FrameType="Default" SuppressWebPartChrome="False" 
            Description="Leave Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" 
            AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" 
            TitleUrl="/Lists/LeaveManagement" DetailLink="/Lists/LeaveManagement" HelpLink="" HelpMode="Modeless" Dir="Default"
             PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
             ConnectionID="00000000-0000-0000-0000-000000000000" ID="LeaveTaskList" __AllowXSLTEditing="true" __designer:CustomXsl="fldtypes_Ratings.xsl" 
            WebPart="true" Height="" Width="">
     <ParameterBindings>
            <ParameterBinding Name="dvt_sortdir" Location="Postback;Connection"/>
            <ParameterBinding Name="dvt_sortfield" Location="Postback;Connection"/>
            <ParameterBinding Name="dvt_startposition" Location="Postback" DefaultValue=""/>
            <ParameterBinding Name="dvt_firstrow" Location="Postback;Connection"/>
            <ParameterBinding Name="OpenMenuKeyAccessible" Location="Resource(wss,OpenMenuKeyAccessible)" />
            <ParameterBinding Name="open_menu" Location="Resource(wss,open_menu)" />
            <ParameterBinding Name="select_deselect_all" Location="Resource(wss,select_deselect_all)" />
            <ParameterBinding Name="idPresEnabled" Location="Resource(wss,idPresEnabled)" />
            <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
            <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
	</ParameterBindings>
<DataFields>
</DataFields>
<XmlDefinition>
<View Name="LeaveApprovalList" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="Approval Task List"
     Url="/Lists/LeaveManagement/ApprovalTasks.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
    <Query><OrderBy><FieldRef Name="ID" Ascending="FALSE"/></OrderBy></Query>
    <ViewFields>
        <FieldRef Name="Requester"/>
        <FieldRef Name="CommonFrom"/>
        <FieldRef Name="To"/>
        <FieldRef Name="LeaveHours"/>
        <FieldRef Name="Reason"/>
        <FieldRef Name="TransferworkTo"/>
        <FieldRef Name="LeftAt"/>
        <FieldRef Name="Left"/>
        <FieldRef Name="ApprovalStatus"/></ViewFields>
    <RowLimit Paged="TRUE">10</RowLimit>
    <JSLink>clienttemplates.js</JSLink>
    <XslLink Default="TRUE">main.xsl</XslLink>
    <Toolbar Type="Standard"/></View>
</XmlDefinition>
<JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/JsLink_LeaveApprovalTaskList_View.js?v=1.0</JSLink>
</WebPartPages:XsltListViewWebPart>

</ZoneTemplate></WebPartPages:WebPartZone>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveTask_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveTask_PageTitleArea%>" />
</asp:Content>
