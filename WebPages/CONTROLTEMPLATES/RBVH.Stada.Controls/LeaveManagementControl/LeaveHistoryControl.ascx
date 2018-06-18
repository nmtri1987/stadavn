<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaveHistoryControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl.LeaveHistoryControl" %>

<table style="table-layout: fixed; width: 100% !important" id="leave-history-list-container">
    <tr>
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarFromDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append date inner-addon right-addon txtCalendar" id="dpFromDate" data-date="102/2012" data-date-format="dd/mm/yyyy" data-autoclose="true" style="width: 120px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd;"></i>
                        <asp:TextBox ID="txtLeaveFromDate" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                    </div>
                </div>

                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarToDate%>" />
                    </label>
                </div>
                <div class="form-group">
                    <div class="input-append date inner-addon right-addon txtCalendar" id="dpToDate" data-date="102/2012" data-date-format="dd/mm/yyyy" data-autoclose="true" style="width: 120px;">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd;"></i>
                        <asp:TextBox ID="txtLeaveToDate" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="LeaveHistoryZone" Title="loc:Main">
            <ZoneTemplate>
                <WebPartPages:XsltListViewWebPart
                runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                NoDefaultStyle="" EnableOriginalValue="False"
                DisplayName="My Leave" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/LeaveManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                UseSQLDataSourcePaging="True" DataSourceID=""
                ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                SuppressWebPartChrome="False" Description="My OverTime" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/LeaveManagement" DetailLink="/Lists/LeaveManagement"
                HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                __AllowXSLTEditing="true" ID="LeaveHistoryWebPart" WebPart="true" Height="" Width="">
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
                    <ParameterBinding Name="FromDateParam" Location="QueryString(FromDate)" DefaultValue=""/>
                    <ParameterBinding Name="ToDateParam" Location="QueryString(ToDate)" DefaultValue=""/>
                    <ParameterBinding Name="EmployeeIdParam" Location="QueryString(EmployeeId)" DefaultValue="0"/>
                </ParameterBindings>
                <DataFields></DataFields>
                <XmlDefinition>
                    <View BaseViewID="5" Type="HTML" WebPartZoneID="Main" DisplayName="My Leave List" TabularView="FALSE" MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="MyLeaveList.aspx">
                          <Query>
                              <Where>
                                  <Eq>
                                      <FieldRef Name='ID' />
                                      <Value Type='Counter'>0</Value>
                                  </Eq>
                              </Where>
                              <OrderBy>
                                  <FieldRef Name="CommonFrom" Ascending="FALSE" />
                                  <FieldRef Name="ID" Ascending="FALSE" />
                              </OrderBy>
                            </Query>
                        <ViewFields>
                            <FieldRef Name="Requester" />
                            <FieldRef Name="RequestFor" />
                            <FieldRef Name="CommonDepartment" />
                            <FieldRef Name="CommonLocation" />
                            <FieldRef Name="CommonFrom" />
                            <FieldRef Name="To" />
                            <FieldRef Name="LeaveHours" />
                            <FieldRef Name="Reason" />
                            <FieldRef Name="IsValidRequest" />
                            <FieldRef Name="UnexpectedLeave" />
                        </ViewFields>
                        <RowLimit Paged="TRUE">20</RowLimit>
                        <JSLink>clienttemplates.js</JSLink>
                        <XslLink Default="TRUE">main.xsl</XslLink>
                        <Toolbar Type="Standard"/>
                    </View>
                </XmlDefinition>
                <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/JSLink_LeaveHistory.js?v=1.2</JSLink>
                </WebPartPages:XsltListViewWebPart>
            </ZoneTemplate>
        </WebPartPages:WebPartZone>
        </td>
    </tr>
</table>