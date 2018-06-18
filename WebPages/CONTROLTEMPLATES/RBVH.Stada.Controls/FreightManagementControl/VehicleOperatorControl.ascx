<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VehicleOperatorControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.FreightManagementControl.VehicleOperatorControl" %>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/FreightModule/FreightByDepartment.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>

<table style="table-layout: fixed; width: 100% !important" id="freight-vehicle-operator-container">
    <tr>
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CommonDepartment%>" />
                    </label>
                </div>
                <div class="form-group">
                    <select class="form-control" id="cbFreightDepartment" style="width: 150px;">
                    </select>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%" valign="top">
            <div class="form-inline pt10">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,FreightManagement_Vehicle%>" />
                    </label>
                </div>
                <div class="form-group">
                    <select class="form-control" id="cbo-vehicle" style="width: 150px;">
                    </select>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%" valign="top">
            <div class="form-inline pt10">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarFromDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append  date inner-addon right-addon txtCalendar" id="dpFromDate" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months" style="width: 150px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd; left: 120px;"></i>
                        <asp:TextBox ID="txtFreightFromDate" ClientIDMode="Static" runat="server" Width="150px" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarToDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months" style="width: 150px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd; left: 120px;"></i>
                        <asp:TextBox ID="txtFreightToDate" ClientIDMode="Static" runat="server" Width="150px" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                        <img src="/_layouts/15/RBVH.Stada.Intranet.Branding/images/excel.png" style="width: 27px; height: 27px; cursor:pointer; position: absolute; left: 160px; top: 4px;" id="img-export-excel"/>
                    </div>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="FreightRequestVehicleOperatorZone" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Freight For Vehicle Operator" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/FreightManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Freight Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Freight For Vehicle Operator" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/FreightManagement" DetailLink="/Lists/FreightManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="FreightRequestVehicleOperatorWebPart" WebPart="true" Height="" Width="">
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
                            <ParameterBinding Name="VehicleId" Location="QueryString(AdminVehicleId)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                        <View BaseViewID="7" Type="HTML" WebPartZoneID="Main" DisplayName="Freight For Vehicle Operator" TabularView="FALSE" MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="VehicleOperator.aspx">
                                <Toolbar Type="Standard" />
                                <XslLink Default="TRUE">main.xsl</XslLink>
                                <JSLink>clienttemplates.js</JSLink>
                                <RowLimit Paged="TRUE">20</RowLimit>
                                <ViewFields>
                                      <FieldRef Name="Requester" />
                                      <FieldRef Name="RequestNo" />
                                      <FieldRef Name="Receiver" />
                                      <FieldRef Name="CommonDepartment" />
                                      <FieldRef Name="Bringer" />
                                      <FieldRef Name="BringerName" />
                                      <FieldRef Name="VehicleLookup" />
                                      <FieldRef Name="VehicleVN" />
                                      <FieldRef Name="ApprovalStatus" />
                                      <FieldRef Name="CommonComment" />
                                      <FieldRef Name="Created"/>
                                </ViewFields>
                                <Query>
                                      <Where>
                                          <Eq>
                                             <FieldRef Name='ID' />
                                             <Value Type='Counter'>0</Value>
                                          </Eq>
                                      </Where>
                                      <OrderBy>
                                        <FieldRef Name="ColForSort" Ascending="TRUE" />
                                        <FieldRef Name="Created" Ascending="FALSE" />
                                        <FieldRef Name="ID" Ascending="FALSE" />
                                      </OrderBy>
                                </Query>
                                <ParameterBindings>
                                  <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                  <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                                </ParameterBindings>
                        </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/FreightModule/JSLink_FreightRequest_VehicleOparator.js?v=1.4</JSLink>
                    </WebPartPages:XsltListViewWebPart>
                </ZoneTemplate>
            </WebPartPages:WebPartZone>
        </td>
    </tr>
</table>

<script type="text/javascript">
    $(document).ready(function () {
        var settings = {
            FromDateControlSelector: '#txtFreightFromDate',
            ToDateControlSelector: '#txtFreightToDate',
            DepartmentControlSelector: '#cbFreightDepartment',
            VehicleControlSelector: '#cbo-vehicle',
            CurrentDepartmentId: 0,
            ExportControlSelector: '#img-export-excel'
        };

        freightByDepartmentInstance = new RBVH.Stada.WebPages.pages.FreightByDepartment(settings);
    });
</script>