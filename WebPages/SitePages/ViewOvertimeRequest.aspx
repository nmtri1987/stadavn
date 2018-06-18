<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>

<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint,Version=15.0.0.0,Culture=neutral,PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="RBVH.Stada.Intranet.WebPages.Utils" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<script language="CS" runat="server" visible="false">
    public string OvertimeManagementID { get; set; }
    protected void ViewOvertimeRequestWP_OnInit(object sender, EventArgs e)
    {
        OvertimeManagementID = Request.QueryString["omId"];
        SPSite stadaWebsite = SPContext.Current.Site;
        using (var site = new SPSite(stadaWebsite.Url))
        {
            using (var web = site.OpenWeb())
            {
                SPList overtimeManagementSpList =
                    web.GetList(string.Format("{0}{1}", web.Url,  RBVH.Stada.Intranet.Biz.Constants.StringConstant.OvertimeManagementURL));
                if (overtimeManagementSpList != null)
                {
                    var listId = Convert.ToString(overtimeManagementSpList.ID);
                    ListFormWebPart lfwp = (ListFormWebPart)sender;
                    lfwp.ListName = listId;
                    lfwp.ListId = new Guid(listId);
                    lfwp.ListItemId = Convert.ToInt32(OvertimeManagementID);
                }
            }
        }
    }

    void Page_Load(object sender, System.EventArgs e)
    {
        string requesterLookupId = Request.QueryString["rlId"];
        string overtimeManagementId = OvertimeManagementID;
        string webUrl = SPContext.Current.Web.Url;
        if(string.IsNullOrEmpty(requesterLookupId) || string.IsNullOrEmpty(overtimeManagementId))
        {
              Response.Redirect(string.Format("{0}{1}", webUrl,  RBVH.Stada.Intranet.Biz.Constants.StringConstant.SitePageOvertimeManagementURL));
        }
        ParamRequesterLookupIDHidden.Text = requesterLookupId;
    }
</script>
<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    
    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main">
        <ZoneTemplate>
            <WebPartPages:ListFormWebPart OnInit="ViewOvertimeRequestWP_OnInit" runat="server" __MarkupType="xmlmarkup" WebPart="true" __WebPartId="{E0916C99-BE51-4E3C-8AE5-0CE8CA94CE2A}">
                <WebPart xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.microsoft.com/WebPart/v2">
                  <Title>Overtime Management</Title>
                  <FrameType>Default</FrameType>
                  <Description />
                  <IsIncluded>true</IsIncluded>
                  <PartOrder>2</PartOrder>
                  <FrameState>Normal</FrameState>
                  <Height />
                  <Width />
                  <AllowRemove>true</AllowRemove>
                  <AllowZoneChange>true</AllowZoneChange>
                  <AllowMinimize>true</AllowMinimize>
                  <AllowConnect>true</AllowConnect>
                  <AllowEdit>true</AllowEdit>
                  <AllowHide>true</AllowHide>
                  <IsVisible>true</IsVisible>
                  <DetailLink />
                  <HelpLink />
                  <HelpMode>Modeless</HelpMode>
                  <Dir>Default</Dir>
                  <PartImageSmall />
                  <MissingAssembly>Cannot import this Web Part.</MissingAssembly>
                  <PartImageLarge />
                  <IsIncludedFilter />
                  <ExportControlledProperties>true</ExportControlledProperties>
                  <ConnectionID>00000000-0000-0000-0000-000000000000</ConnectionID>
                  <ID>g_e0916c99_be51_4e3c_8ae5_0ce8ca94ce2a</ID>
                  <PageType xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">PAGE_DISPLAYFORM</PageType>
                  <FormType xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">4</FormType>
                  <ControlMode xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">Display</ControlMode>
                  <ViewFlag xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">1048576</ViewFlag>
                  <ViewFlags xmlns="http://schemas.microsoft.com/WebPart/v2/ListForm">Default</ViewFlags>
                </WebPart>
            </WebPartPages:ListFormWebPart>
        </ZoneTemplate>
    </WebPartPages:WebPartZone>
    <asp:Label ID="ParamRequesterLookupIDHidden" runat="server" Visible="false"></asp:Label>
    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone2" Title="loc:Main">
        <ZoneTemplate>
            <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ShowToolbarWithRibbon="True" ViewSelectorFetchAsync="False"
                InplaceSearchEnabled="False" ServerRender="False" ClientRender="False"
                InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000"
                IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle=""
                ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                ViewContentTypeId="" ListUrl="Lists/OvertimeEmployeeDetails" ListDisplayName=""
                PageSize="-1"
                UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False"
                ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60"
                Title="Overtime Employee Details" FrameType="Default" SuppressWebPartChrome="False"
                Description="Overtime Employee Details" IsIncluded="True" ZoneID="Main"
                PartOrder="4" FrameState="Normal" AllowRemove="True"
                AllowZoneChange="True" AllowMinimize="True" AllowConnect="True"
                AllowEdit="True" AllowHide="True" IsVisible="True"
                CatalogIconImageUrl="/_layouts/15/images/itgen.png"
                TitleUrl="/Lists/OvertimeEmployeeDetails" DetailLink="/Lists/OvertimeEmployeeDetails" HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="/_layouts/15/images/itgen.png" IsIncludedFilter="" ExportControlledProperties="False" ConnectionID="00000000-0000-0000-0000-000000000000" ID="g_99aeaa1c_9d4c_45df_b812_a4a9d087f3a5" __MarkupType="vsattributemarkup" __WebPartId="{99AEAA1C-9D4C-45DF-B812-A4A9D087F3A5}" __AllowXSLTEditing="true" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="">
                <ParameterBindings>
                            <ParameterBinding Name="dvt_sortdir" Location="Postback;Connection" />
                            <ParameterBinding Name="dvt_sortfield" Location="Postback;Connection" />
                            <ParameterBinding Name="dvt_startposition" Location="Postback" DefaultValue="" />
                            <ParameterBinding Name="dvt_firstrow" Location="Postback;Connection" />
                            <ParameterBinding Name="OpenMenuKeyAccessible" Location="Resource(wss,OpenMenuKeyAccessible)" />
                            <ParameterBinding Name="open_menu" Location="Resource(wss,open_menu)" />
                            <ParameterBinding Name="select_deselect_all" Location="Resource(wss,select_deselect_all)" />
                            <ParameterBinding Name="idPresEnabled" Location="Resource(wss,idPresEnabled)" />
                            <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                            <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                            <ParameterBinding Name="RequesterLookupID" Location="Control(ParamRequesterLookupIDHidden)" DefaultValue=""/>
                </ParameterBindings>
                <DataFields>
                </DataFields>
                <XmlDefinition>
                            <View Name="OvertimeEmployeetDetailView" MobileView="TRUE"  Type="HTML" Hidden="TRUE"
                                DisplayName="" Url="/Lists/OvertimeManagement/NewForm.aspx" Level="1" BaseViewID="1" 
                                 ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23">
                                <Query>
                                    <Where><Eq><FieldRef Name="OvertimeManagementID" /><Value Type="Lookup">{RequesterLookupID}</Value></Eq></Where>
                                    <OrderBy><FieldRef Name="ID" Ascending="FALSE"/></OrderBy>
                                </Query>
                                <ViewFields>
                                    <FieldRef Name="Employee"/>
                                    <FieldRef Name="WorkingHours"/>
                                    <FieldRef Name="OvertimeFrom"/><FieldRef Name="OvertimeTo"/>
                                    <FieldRef Name="Task"/>
                                    <FieldRef Name="TransportAt"/>
                                    </ViewFields>
                                <RowLimit Paged="TRUE">10</RowLimit>
                                <JSLink>clienttemplates.js</JSLink>
                                <XslLink Default="TRUE">main.xsl</XslLink><Toolbar Type="Standard"/>
                                </View>
                </XmlDefinition>
                <JSLink>~siteCollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/JSLink_OvertimeEmployeeDetail_View.js?v=1.0</JSLink>
            </WebPartPages:XsltListViewWebPart>
        </ZoneTemplate>
    </WebPartPages:WebPartZone>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ViewOvertimeRequest_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ViewOvertimeRequest_PageTitleArea%>" />
</asp:Content>
