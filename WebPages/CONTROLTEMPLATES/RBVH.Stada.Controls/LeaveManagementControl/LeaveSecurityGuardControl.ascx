<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaveSecurityGuardControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveManagementControl.LeaveSecurityGuardControl" %>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/LeaveSecurityGuard.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
<table style="table-layout: fixed; width: 100% !important" id="leave-sec-list-container">
    <tr id="searchcontainer">
        <td style="width: 100%" valign="top">
            <div class="form-inline">
                <div class="form-group header-left lbl-fixed-width">
                    <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,EmployeeInfo_EmployeeID%>" />
                    </label>
                </div>
                <div class="form-group">
                    <input type="text" class="form-control" id="txtEmployeeID" style="width:200px;" />
                    <input type="button" id="btnSearch" value="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SearchButton%>' />" />
                </div>
            </div>
        </td>
    </tr>
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="LeaveRequestForSecurityZone" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Leave Request For Security" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/LeaveManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="My OverTime" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/LeaveManagement" DetailLink="/Lists/LeaveManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="LeaveRequestForSecurityWebPart" WebPart="true" Height="" Width="">
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
                        <DataFields></DataFields>
                        <XmlDefinition>
                         <View BaseViewID="6" Type="HTML" WebPartZoneID="Main" DisplayName="Leave Requests For Security Guard " TabularView="FALSE" MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="LeaveRequestSecurityGuard.aspx">
                                <Toolbar Type="Standard" />
                                <XslLink Default="TRUE">main.xsl</XslLink>
                                <JSLink>clienttemplates.js</JSLink>
                                <RowLimit Paged="TRUE">20</RowLimit>
                                <ViewFields>
                                      <FieldRef Name="Requester" />
                                      <FieldRef Name="RequestFor" />
                                      <FieldRef Name="CommonDepartment" />
                                      <FieldRef Name="CommonFrom" />
                                      <FieldRef Name="To" />
                                      <FieldRef Name="LeftAt" />
                                      <FieldRef Name="CheckOutBy" />
                                      <FieldRef Name="Left" />
                                      <FieldRef Name="EnterTime" />
                                      <FieldRef Name="CheckInBy" />
                                </ViewFields>
                                <Query>
                                      <Where>
                                          <Eq>
                                             <FieldRef Name='ID' />
                                             <Value Type='Counter'>0</Value>
                                          </Eq>
                                      </Where>
                                  <OrderBy>
                                    <FieldRef Name="CommonFrom" Ascending="FALSE"></FieldRef>
                                     <FieldRef Name="ID" Ascending="FALSE" />
                                  </OrderBy>
                                </Query>
                                <ParameterBindings>
                                  <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                  <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                                </ParameterBindings>
                              </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/LeaveModule/JSLink_LeaveRequest_SecurityGuard.js?v=1.3</JSLink>
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
                SearchContainer: "#searchcontainer",
                InputEmployeeID: '#txtEmployeeID',
                SearchButtonID: '#btnSearch'
            }
        };
        leaveSecurityGuardInstance = new RBVH.Stada.WebPages.pages.LeaveSecurityGuard(settings);
    });
</script>
