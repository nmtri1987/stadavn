<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SecurityLeaveManagement.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.SecurityLeaveManagement" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:Label ID="LabelTodayLeave" Font-Bold="true" Font-Size="Medium" runat="server" Text="<%$Resources:RBVHStadaWebpages,SecurityLeave_TodayLeave%>"></asp:Label>
    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZoneMain" Title="loc:Main">
        <ZoneTemplate>
            <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="False" ServerRender="False" ClientRender="False"
                InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000"
                IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                DisplayName="All Items" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/LeaveManagement" ListDisplayName=""
                PageType="PAGE_DEFAULTVIEW" PageSize="-1" UseSQLDataSourcePaging="True"
                DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Leave Management"
                FrameType="Default" SuppressWebPartChrome="False" Description="Leave Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal"
                AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True"
                AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/LeaveManagement" DetailLink="/Lists/LeaveManagement"
                HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part."
                PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                ConnectionID="00000000-0000-0000-0000-000000000000" ID="g_c8e31d7e_1ea8_4e37_8806_ea82aca80ecf"
                __MarkupType="vsattributemarkup" __WebPartId="{C8E31D7E-1EA8-4E37-8806-EA82ACA80ECF}"
                __AllowXSLTEditing="true" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="">
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
                    <View Name="TodayLeave"  DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="Today Leave" Url="/Lists/LeaveManagement/AllItems.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
                        <Query>
                            <Where>
                              <And>
                                 <Eq>
                                     <FieldRef Name="CommonFrom"/>
                                     <Value Type="DateTime"><Today/></Value>
                                 </Eq>
                                 <And>
                                    <Lt>
                                        <FieldRef Name="LeaveHours" />
                                        <Value Type="Number">8</Value>
                                    </Lt>
                                    <Eq>
                                       <FieldRef Name="ApprovalStatus" />
                                       <Value Type="Text">Approved</Value>
                                    </Eq>
                                 </And>
                              </And>
                           </Where>
                            <OrderBy ><FieldRef Name="ID" Ascending="False"/></OrderBy>
                        </Query>
                        <ViewFields>
                            <FieldRef Name="RequestFor"/>
                            <FieldRef Name="CommonFrom"/>
                            <FieldRef Name="To"/>
                            <FieldRef Name="LeftAt"/>
                            <FieldRef Name="Left"/>
                        </ViewFields>
                        <RowLimit Paged="TRUE">10</RowLimit>
                        <JSLink>clienttemplates.js</JSLink>
                        <XslLink Default="TRUE">main.xsl</XslLink>
                        <Toolbar Type="Standard"/>
                    </View>
                </XmlDefinition>
                <JSLink>~siteCollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/JSLink_TodayLeave.js?v=1.2</JSLink>
            </WebPartPages:XsltListViewWebPart>
        </ZoneTemplate>
    </WebPartPages:WebPartZone>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,SecurityLeaveManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,SecurityLeaveManagement_PageTitleArea%>" />
</asp:Content>

