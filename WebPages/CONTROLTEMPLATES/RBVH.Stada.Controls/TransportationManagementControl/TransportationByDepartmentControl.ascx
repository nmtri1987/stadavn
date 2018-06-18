<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransportationByDepartmentControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.TransportationManagementControl.TransportationByDepartmentControl" %>
<div class="panel panel-primary">
    <div class="panel-body">
        <table style="table-layout: fixed; width: 100% !important" id="vehicle-bydept-container">
            <tr>
                <td style="width: 100%" valign="top">
                    <div class="form-inline">
                        <div class="form-group header-left lbl-fixed-width">
                            <label>
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CommonDepartment%>" />
                            </label>
                        </div>
                        <div class="form-group">
                            <select class="form-control" id="cbTranspotationAdminDepartment">
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
                                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarMonth%>" />
                            </label>
                        </div>
                        <div class="form-group">
                            <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                <asp:TextBox ID="txtTransportationDateByDepartment" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <tr>
                <td style="width: 100%;" valign="top">
                    <br />
                    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="TransportationDepartmentControlWebpartZone" Title="loc:Main">
                        <ZoneTemplate>
                            <WebPartPages:XsltListViewWebPart
                                runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                                ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                                NoDefaultStyle="" EnableOriginalValue="False"
                                DisplayName="My Change Shift" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/VehicleManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                                UseSQLDataSourcePaging="True" DataSourceID=""
                                ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Change Shift Management" FrameType="Default"
                                SuppressWebPartChrome="False" Description="Overtime Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                                AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/VehicleManagement" DetailLink="/Lists/VehicleManagement"
                                HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                                ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                                __AllowXSLTEditing="true" ID="TransportationDepartmentControlWP" WebPart="true" Height="" Width="">
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
                                 <ParameterBinding Name="DepartmentId" Location="QueryString(AdminDeptId)" DefaultValue="1"/>
                                 <ParameterBinding Name="StartMonth" Location="QueryString(AdminStartMonth)" DefaultValue=""/>
                                 <ParameterBinding Name="EndMonth" Location="QueryString(AdminEndMonth)" DefaultValue=""/>
                                </ParameterBindings>
                                <DataFields></DataFields>
                                <XmlDefinition>
                                <View BaseViewID="2" Type="HTML" WebPartZoneID="Main" DisplayName="Transpotation Management View 2"  SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="TranspotationManagementView2.aspx">
                                  <Query>
                                        <Where>
                                            <Eq>
                                               <FieldRef Name='ID' />
                                               <Value Type='Counter'>0</Value>
                                            </Eq>
                                        </Where>
                                        <OrderBy>
                                            <FieldRef Name="ColForSort" Ascending="TRUE" />
                                            <FieldRef Name='CommonFrom' Ascending='FALSE' />
                                            <FieldRef Name="ID" Ascending="FALSE" />
                                        </OrderBy>
                                   </Query>
                                    <ViewFields>
                                        <FieldRef Name="Requester" />
                                        <FieldRef Name="CommonDepartment" />
                                        <FieldRef Name="VehicleType" />
                                        <FieldRef Name="CommonFrom" />
                                        <FieldRef Name="To" />
                                        <FieldRef Name="Reason" />
                                        <FieldRef Name="CompanyPickup" />
                                        <FieldRef Name="ApprovalStatus" />
                                        <FieldRef Name="ApprovalStatus" />
                                        <FieldRef Name="Created" />
                                    </ViewFields>
                                    <RowLimit Paged="TRUE">20</RowLimit>
                                    <JSLink>clienttemplates.js</JSLink>
                                    <XslLink Default="TRUE">main.xsl</XslLink>
                                    <Toolbar Type="Standard"/>
                                 </View>
                              </XmlDefinition>
                              <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/TransportationModule/JSLink_VehicleRequest_ByDepartment.js?v=1.1</JSLink>
                            </WebPartPages:XsltListViewWebPart>
                          </ZoneTemplate>
                       </WebPartPages:WebPartZone>
                </td>
            </tr>
        </table>
    </div>
</div>
