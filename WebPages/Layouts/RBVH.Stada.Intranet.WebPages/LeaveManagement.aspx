<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LeaveManagement.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.LeaveManagement" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
  
    <style type="text/css">
        .ms-list-addnew {
            display: none;
        }

        .custom-margin {
            margin-left: 0px !important;
        }
    </style>

    <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>

    <div style="width: 100%; margin-bottom: 20px;">
        <button type="button"  id="btnAddLeave"  class="btn btn-primary add-new-change-shift">
            <i class='fa fa-plus' aria-hidden='true'></i> &nbsp; <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_AddNewLeaveButton%>" />
        </button>
        
        <button type="button"  id="btnTodayLeave"  runat="server" class="btn btn-primary add-new-change-shift">
            <i class='fa fa-calendar' aria-hidden='true'></i> &nbsp; <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_TodayLeaveButton%>" />
        </button>
    </div>
    <table style="table-layout: fixed; width: 100% !important">
        <tr>
            <td style="width: 100%;" valign="top">
                <asp:Label runat="server" ID="Label1" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_MyLeaveRequestTitle%>" Font-Bold="True"></asp:Label>
                <br />
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main" Width="100%">
                    <ZoneTemplate>
                        <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" AsyncUpdate="TRUE"
                            ViewSelectorFetchAsync="False" InplaceSearchEnabled="False" ServerRender="False" ClientRender="True"
                            InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                            NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False" DisplayName="All Items"
                            ViewContentTypeId=""
                            ListUrl="Lists/LeaveManagement"
                            UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Leave Management" FrameType="Default" SuppressWebPartChrome="False" Description="My Leave Request" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/LeaveManagement" DetailLink="/Lists/LeaveManagement" HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                            ConnectionID="00000000-0000-0000-0000-000000000000"
                            ID="Leave" __MarkupType="vsattributemarkup"
                            __AllowXSLTEditing="true" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="100%">
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
                                <ParameterBinding Name="RequesterLookupID" Location="Control(ParamRequesterLookupIDHidden, Value)" DefaultValue="1"/>
                            </ParameterBindings>
                            <DataFields>
                            </DataFields>
                            <XmlDefinition>
                             <View Name="{1CDFD671-2175-4155-9FED-B1AC1638F270}" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="My Leave Request" Url="/Lists/LeaveManagement/MyLeaveRequest.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
                                    <Query>   
                                          <Where>
                                            <Eq>
                                              <FieldRef Name="Requester" LookupId="TRUE"/>
                                              <Value Type="Lookup">{RequesterLookupID}</Value>
                                            </Eq>
                                          </Where>
                                           <OrderBy><FieldRef Name="ID" Ascending="False" /></OrderBy>
                                         </Query>
                             <ViewFields>
                                <FieldRef Name="Requester" />
                                 <FieldRef Name="CommonFrom" />
                                 <FieldRef Name="To"  />
                                 <FieldRef Name="LeaveHours" />
                                 <FieldRef Name="Reason"/>
                                 <FieldRef Name="TransferworkTo" />
                                 <FieldRef Name="LeftAt"  />
                                 <FieldRef Name="Left" />
                                 <FieldRef Name="ApprovalStatus"  />
                                 
                             </ViewFields><RowLimit Paged="TRUE">15</RowLimit>
                                    <XslLink Default="TRUE">main.xsl</XslLink>
                                    <JSLink>clienttemplates.js</JSLink>
                                    <Toolbar Type="Standard"/></View>
                            </XmlDefinition>
                            <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/JSLink_LeaveManagement_View.js</JSLink>
                        </WebPartPages:XsltListViewWebPart>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
            </td>
        </tr>
    </table>

     <script type="text/javascript">
        $('#btnAddLeave').on('click', function () {
            window.location = 'http://' + location.host + '/Lists/LeaveManagement/NewForm.aspx?Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement.aspx';
        });

        $('#btnTodayLeave').on('click', function () {
            window.location = 'http://' + location.host + '/_layouts/15/RBVH.Stada.Intranet.WebPages/SecurityLeaveManagement.aspx?Source=/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement.aspx';
        });
    </script>
    
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,LeaveManagement_PageTitleArea%>" />
</asp:Content>

