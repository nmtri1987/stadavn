<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TransportationApprovalControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.TransportationManagementControl.TransportationApprovalControl" %>

<table style="table-layout: fixed; width: 100% !important" id="vehicle-approval-container">
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="TransportationApprovalControlWebpartZone" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Approval Tasks" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/VehicleManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Task List" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Approval Tasks" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/VehicleManagement" DetailLink="/Lists/VehicleManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="TransportationApprovalControlWP" WebPart="true" Height="" Width="">
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
                        <View BaseViewID="1" Type="HTML" WebPartZoneID="Main" DisplayName="Approval Tasks" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="AllItems.aspx">
                            <Toolbar Type="Standard" />
                            <XslLink Default="TRUE">main.xsl</XslLink>
                            <JSLink>clienttemplates.js</JSLink>
                            <RowLimit Paged="TRUE">20</RowLimit>
                            <ViewFields>
                                <FieldRef Name="Requester"/>
                                <FieldRef Name="VehicleType"/>
                                <FieldRef Name="CommonFrom" />
                                <FieldRef Name="To"/>
                                <FieldRef Name="Reason"/>
                                <FieldRef Name="CompanyPickup"/>
                                <FieldRef Name="ApprovalStatus"/>
                                <FieldRef Name="CommonApprover2"/>
                                <FieldRef Name="CommonApprover1"/>
                                <FieldRef Name="CommonComment" />
                                <FieldRef Name="CommonDepartment"/>
                                <FieldRef Name="Created" />
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
                                    <FieldRef Name='CommonFrom' Ascending='FALSE' />
                                    <FieldRef Name="ID" Ascending="FALSE" />
                                </OrderBy>
                            </Query>
                            <ParameterBindings>
                                <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                            </ParameterBindings>
                            </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/TransportationModule/JSLink_VehicleRequest_Approval.js?v=1.2</JSLink>
                    </WebPartPages:XsltListViewWebPart>
                </ZoneTemplate>
            </WebPartPages:WebPartZone>
        </td>
    </tr>
</table>
