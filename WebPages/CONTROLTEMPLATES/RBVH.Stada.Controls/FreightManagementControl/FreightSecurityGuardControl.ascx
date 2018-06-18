<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FreightSecurityGuardControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.FreightManagementControl.FreightSecurityGuardControl" %>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/FreightModule/FreightSecurityGuard.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
<table style="table-layout: fixed; width: 100% !important" id="freight-security-container">
    <tr>
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width"></div>
                <div class="form-group">
                    <label>
                        <input type="radio" name="searchType" value="0" checked />
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightSecurity_BasicSearch%>" />
                    </label>
                    <br />
                    <label>
                        <input type="radio" name="searchType" value="1" />
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,FreightSecurity_SearchByReqNumber%>" />
                    </label>
                </div>
            </div>
        </td>
    </tr>
    <tr class="basicsearch" style="display:none;">
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CommonDepartment%>" />
                    </label>
                </div>
                <div class="form-group">
                    <select class="form-control" id="cbFreightSecGuardDepartment" style="width: 200px;">
                    </select>
                </div>
            </div>
        </td>
    </tr>
    <tr class="basicsearch" style="display:none;">
        <td style="width: 100%" valign="top">
            <div class="form-inline pt10">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarFromDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append  date inner-addon right-addon txtCalendar" id="dpFromDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months" style="width: 200px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd;"></i>
                        <asp:TextBox ID="txtFreightSecGuardFromDate" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarToDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months" style="width: 200px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd;"></i>
                        <asp:TextBox ID="txtFreightSecGuardToDate" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </td>
    </tr>
    <tr class="searchbyreqnum" style="display:none;">
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_RequestNumber%>" />
                    </label>
                </div>
                <div class="form-group">
                    <input type="text" class="form-control" id="txtReqNum" style="width:200px;" />
                    <input type="button" id="btnSearch" value="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SearchButton%>' />" />
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="FreightRequestForSecurityZone" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Freight Request For Security" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/FreightManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Freight Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Freight Request For Security" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/FreightManagement" DetailLink="/Lists/FreightManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="FreightRequestForSecurityWebPart" WebPart="true" Height="" Width="">
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
                            <ParameterBinding Name="SelectedDate" Location="QueryString(AdminSelectedDate)" DefaultValue=""/>
                            <ParameterBinding Name="SelectedToDate" Location="QueryString(AdminSelectedToDate)" DefaultValue=""/>
                            <ParameterBinding Name="DepartmentId" Location="QueryString(AdminDeptId)" DefaultValue="0"/>
                            <ParameterBinding Name="reqnum" Location="QueryString(reqnum)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                        <View BaseViewID="6" Type="HTML" WebPartZoneID="Main" DisplayName="Freight Requests For Security Guard " TabularView="FALSE" MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="FreightRequestSecurityGuard.aspx">
                                <Toolbar Type="Standard" />
                                <XslLink Default="TRUE">main.xsl</XslLink>
                                <JSLink>clienttemplates.js</JSLink>
                                <RowLimit Paged="TRUE">20</RowLimit>
                                <ViewFields>
                                    <FieldRef Name="Requester" />
                                    <FieldRef Name="CommonDepartment" />
                                    <FieldRef Name="RequestNo" />
                                    <FieldRef Name="Bringer" />
                                    <FieldRef Name="BringerName" />
                                    <FieldRef Name="Receiver" />
                                    <FieldRef Name="CompanyVehicle" />
                                    <FieldRef Name="VehicleLookup" />
                                    <FieldRef Name="VehicleVN" />
                                    <FieldRef Name="Created"/>
                                </ViewFields>
                                <Query>
                                      <Where>
                                          <And>
                                            <Eq>
                                                <FieldRef Name='IsFinished' />
                                                <Value Type='Boolean'>0</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='ApprovalStatus' />
                                                <Value Type='Text'>Approved</Value>
                                            </Eq>
                                        </And>
                                      </Where>
                                      <OrderBy>
                                        <FieldRef Name="ColForSort" Ascending="TRUE" />
                                        <FieldRef Name="Created" Ascending="FALSE" />
                                      </OrderBy>
                                </Query>
                                <ParameterBindings>
                                  <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                  <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                                </ParameterBindings>
                        </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/FreightModule/JSLink_FreightRequest_SecurityGuard.js?v=1.3</JSLink>
                    </WebPartPages:XsltListViewWebPart>
                </ZoneTemplate>
            </WebPartPages:WebPartZone>
        </td>
    </tr>
</table>
<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                DepartmentControlSelector: "#cbFreightSecGuardDepartment",
                FromDateControlSelector: "#txtFreightSecGuardFromDate",
                ToDateControlSelector: "#txtFreightSecGuardToDate",
                BasicSearchClass: ".basicsearch",
                SearchByReqNumClass: ".searchbyreqnum",
                SearchButtonID: '#btnSearch',
                TxtRequestNumberID: '#txtReqNum'
            }
        };
        freightSecurityGuardInstance = new RBVH.Stada.WebPages.pages.FreightSecurityGuard(settings);
    });
</script>