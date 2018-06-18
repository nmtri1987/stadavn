<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Import Namespace="RBVH.Stada.Intranet.WebPages.Utils" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<script language="CS" runat="server" visible="false">
    void Page_Load(object sender, System.EventArgs e)
    {
        UserHelper userHelper = new UserHelper();
        var currentEmployee = userHelper.GetCurrentLoginUser();
        if (currentEmployee == null || currentEmployee.ID <= 0)
        {
            ParamRequesterLookupIDHidden.Value = "";
        }
        else
        {
            ParamRequesterLookupIDHidden.Value = currentEmployee.ID.ToString();
        }
    }
</script>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div  style="margin-bottom: 20px; margin-left:-5px">
        <button type="button"  id="btnAddNewChangeShift"  class="btn btn-primary add-new-change-shift">
            <i class='fa fa-plus' aria-hidden='true'></i> &nbsp; <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShiftManagement_AddNew%>" />
        </button>
    </div>
    <table style="table-layout: fixed; width: 100% !important">
   <tr>
      <td style="width: 100%;" valign="top">
         <asp:Label runat="server" ID="Label1" Text="<%$Resources:RBVHStadaWebpages,OvertimeManagement_MyChangeShiftRequestTitle%>" Font-Bold="True"></asp:Label>
         <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
         <br />
         <WebPartPages:WebPartZone runat="server" frametype="None" id="WebPartZone1" title="loc:Main">
            <ZoneTemplate>
               <WebPartPages:XsltListViewWebPart  CacheXslStorage="false" 
                  runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                  ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl" 
                  NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                  DisplayName="My Change Shift" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/ChangeShiftManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                  UseSQLDataSourcePaging="True" DataSourceID=""
                  ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Change Shift Management" FrameType="Default"
                  SuppressWebPartChrome="False" Description="Overtime Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                  AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/ChangeShiftManagement" DetailLink="/Lists/ChangeShiftManagement"
                  HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                  ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                  __AllowXSLTEditing="true" ID="MyChangeShift"  WebPart="true" Height="" Width="">
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
                  <DataFields></DataFields>
                  <XmlDefinition >
                     <View  Name="My Change Shift" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="My Requests" 
                        Url="MyChangeShiftRequests.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
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
                           <FieldRef Name="Requester"/>
                           <FieldRef Name="CommonFrom"/>
                           <FieldRef Name="FromShift"/>
                           <FieldRef Name="To"/>
                           <FieldRef Name="ToShift"/>
                           <FieldRef Name="Reason"/>
                           <FieldRef Name="ApprovalStatus"/>
                           <FieldRef Name="CommonComment"/>
                        </ViewFields>
                        <RowLimit Paged="TRUE">10</RowLimit>
                        <JSLink>clienttemplates.js</JSLink>
                        <XslLink Default="TRUE">main.xsl</XslLink>
                        <Toolbar Type="Standard"/>
                     </View>
                  </XmlDefinition>
                   <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ChangeShiftModule/JSLink_ChangeShiftRequest_View.js?v=1.0</JSLink>
               </WebPartPages:XsltListViewWebPart>
            </ZoneTemplate>
         </WebPartPages:WebPartZone>
      </td>
   </tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        $('.add-new-change-shift').on('click', function () {
            var sourceURL = window.location.href;
            sourceURL = Functions.removeParam('lang', sourceURL);
            sourceURL = encodeURIComponent(sourceURL);
            window.location = '//' + location.host + '/Lists/ChangeShiftManagement/NewForm.aspx?subSection=ChangeShiftManagement&Source=' + sourceURL;
        });
    });
</script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShiftManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ChangeShiftManagement_PageTitleArea%>" />
</asp:Content>
