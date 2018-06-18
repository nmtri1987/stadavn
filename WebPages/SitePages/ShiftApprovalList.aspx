<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="RBVH.Stada.Intranet.WebPages.Utils" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="table-layout: fixed; width: 100% !important">
        <tr>
            <td style="width: 100%;" valign="top">
                <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
                <br />
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main">
                    <ZoneTemplate>
                        <WebPartPages:XsltListViewWebPart
                            runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                            ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                            NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                            DisplayName="Shift Management" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/ShiftManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                            UseSQLDataSourcePaging="True" DataSourceID=""
                            ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                            SuppressWebPartChrome="False" Description="ShiftManagement" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                            AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/ShiftManagement" DetailLink="/Lists/ShiftManagement"
                            HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                            ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                            __AllowXSLTEditing="true" ID="MyChangeShift" WebPart="true" Height="" Width="">
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
                     <ParameterBinding Name="CurrentDepartment" Location="Control(ParamRequesterLookupIDHidden, Value)" DefaultValue="1"/>
                            </ParameterBindings>
                            <DataFields></DataFields>
                            <XmlDefinition>
                      <View BaseViewID="2" Name="2dd0a166-548b-4821-95d0-886fe328c0c8" DisplayName="Approval List" Type="HTML" ReadOnly="TRUE" WebPartZoneID="Main" SetupPath="pages\viewpage.aspx" Url="Approval List.aspx">
                                <RowLimit>30</RowLimit>
                                <ViewFields>
                                  <FieldRef Name="Title" />
                                  <FieldRef Name="CommonMonth" />
                                  <FieldRef Name="CommonYear" />
                                  <FieldRef Name="CommonDepartment" />
                                    <FieldRef Name="CommonLocation" />
                                  <FieldRef Name="Requester" />
                                </ViewFields>
                                 <Query>
                               <Where>
                                   <Or>
                                        <Eq>
                                            <FieldRef Name='CommonApprover1' LookupId="True" />
                                            <Value Type='User' LookupId="True"><UserID/></Value>
                                        </Eq>
                                        <Eq>
                                            <FieldRef Name='CommonAddApprover1' LookupId="True" />
                                            <Value Type='User' LookupId="True"><UserID/></Value>
                                        </Eq>
                                    </Or>
                              </Where>
                              <OrderBy>
                                <FieldRef Name="ID" Ascending="FALSE"></FieldRef>
                              </OrderBy>
                            </Query>
                            <ParameterBindings>
                              <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                              <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                            </ParameterBindings>
                            </View>
                            </XmlDefinition>
                            <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/JSLink_ApprovalView.js?v=1.2</JSLink>
                        </WebPartPages:XsltListViewWebPart>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_PageTitleArea%>" />
</asp:Content>

