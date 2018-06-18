<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>

<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
     <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/calendar/minicalendar.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/SiteTemplate/SetTitleForDepartmentSite.js"></script>
       <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/HandleCalendarView.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="container container-custom" style="width: 100%;">
        <div class="row">
            <div class="col-lg-8 col-md-7 col-sm-6">
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row1" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Announcement-->
                        </ZoneTemplate>
                        </WebPartPages:WebPartZone>
                </div>
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row2" ID="Row2" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Task-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
                <div class="row">
                    <div class="ms-webpart-chrome-title" id="WebPartWPQ3_ChromeTitle">
					<span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,DepartmentDiscussion_ListTitle%>'/> - <asp:Literal runat='server' Text='<%$Resources:RBVHStadaLists,DepartmentDiscussion_Description%>'/>"  class="js-webpart-titleCell">
					    <h2 style="text-align:justify;" class="ms-webpart-titleText">
					        <a accesskey="W" id="discussionTitle" href="/Lists/DepartmentDiscussion"><nobr><span><asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,DepartmentDiscussion_ListTitle%>" /> </span><span ></span></nobr></a>
					    </h2>
					</span>
				</div>
                </div>

                <div class="row">
                   <WebPartPages:WebPartZone runat="server" FrameType="None" ID="DiscussionWebPartZone" Title="loc:Main">
                        <ZoneTemplate>
                            <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True"
                                ServerRender="False" ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000"
                                IsClientRender="False" GhostedXslLink="thread.xsl" NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000"
                                EnableOriginalValue="False" DisplayName="Subject" ViewContentTypeId="" Default="TRUE"
                                ListUrl="Lists/DepartmentDiscussion"
                                PageType="PAGE_DEFAULTVIEW" PageSize="-1" UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False"
                                AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" ListDisplayName="Department Discussion"
                                FrameType="Default" SuppressWebPartChrome="False"
                                Description="My List Instance" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True"
                                AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True"
                                TitleUrl="/Lists/DepartmentDiscussion" DetailLink="/Lists/DepartmentDiscussion" HelpLink="" HelpMode="Modeless" Dir="Default"
                                PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                                ConnectionID="00000000-0000-0000-0000-000000000000" ID="g_556e0473_a4c4_4805_9d32_f460c555e608" __MarkupType="vsattributemarkup"
                                __AllowXSLTEditing="true" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true"
                                Height="" Width="">
                                <ParameterBindings>
                                        <ParameterBinding Name="dvt_sortdir" Location="Postback;Connection"/>
                                        <ParameterBinding Name="dvt_sortfield" Location="Postback;Connection"/>
                                        <ParameterBinding Name="dvt_startposition" Location="Postback" DefaultValue=""/>
                                        <ParameterBinding Name="dvt_firstrow" Location="Postback;Connection"/>
                                        <ParameterBinding Name="OpenMenuKeyAccessible" Location="Resource(wss,OpenMenuKeyAccessible)" />
                                        <ParameterBinding Name="open_menu" Location="Resource(wss,open_menu)" />
                                        <ParameterBinding Name="select_deselect_all" Location="Resource(wss,select_deselect_all)" />
                                        <ParameterBinding Name="idPresEnabled" Location="Resource(wss,idPresEnabled)" />
                                        <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noitemsinview_discboard)" />
                                        <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noitemsinview_discboard_howto3)" />
                                </ParameterBindings>
                                <DataFields>
                                </DataFields>
                                <XmlDefinition>
                            <View Name="Department Discussion" DefaultView="TRUE" Type="HTML" ReadOnly="TRUE" DisplayName="Department Discussion" Url="/Lists/DepartmentDiscussion/AllItems.aspx" Level="1" BaseViewID="3" ContentTypeID="0x012001" ImageUrl="/_layouts/15/images/vwdisc.png?rev=23" >
                                <Query><OrderBy UseIndexForOrderBy="TRUE" Override="TRUE"/>
                                <Where>
                                    <Geq><FieldRef Name="DiscussionLastUpdated"/>
                                        <Value Type="DateTime">1900-01-01T00:00:00Z
                                        </Value>
                                    </Geq>
                                </Where>
                                </Query>
                                <ViewFields>
                                    <FieldRef Name="LinkDiscussionTitle"/>
                                    <FieldRef Name="Author"/>
                                    <FieldRef Name="ItemChildCount"/>
                                    <FieldRef Name="Created"/>
                                    <FieldRef Name="Body"/>
                                    <FieldRef Name="LastReplyBy"/>
                                    <FieldRef Name="DiscussionLastUpdated"/>
                                    <FieldRef Name="BestAnswerId"/>
                                    <FieldRef Name="IsFeatured"/>
                                </ViewFields>
                                <RowLimit Paged="TRUE">10</RowLimit>
                                <JSLink>sp.ui.discussions.js</JSLink><XslLink>thread.xsl</XslLink>
                                <Toolbar Type="Standard"/></View></XmlDefinition>
                            </WebPartPages:XsltListViewWebPart>
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
            </div>
            <div class="col-lg-4 col-md-5 col-sm-6" style="padding-left: 25px">
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row_right" ID="Row_right" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Calendar.-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row_right" ID="Row_right2" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Document.-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
            </div>
        </div>
         <div id="calendarMonth" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarMonth%>" />
        </div>
        <div id="calendarWeek" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarWeek%>" />
        </div>
        <div id="calendarDay" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarDay%>" />
        </div>
    </div>
       <script>
        $(document).ready(function(){
            var url      = window.location.href;
            var sitePageUrl = "SitePages/Home.aspx";
            if(url.indexOf(sitePageUrl) != -1){
                   $("#discussionTitle").prop("href",  url.replace(sitePageUrl, "Lists/DepartmentDiscussion")) ;
            }
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <SharePoint:ProjectProperty Property="Title" runat="server"/>
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
   <SharePoint:ProjectProperty Property="Title" runat="server"/>
</asp:Content>
