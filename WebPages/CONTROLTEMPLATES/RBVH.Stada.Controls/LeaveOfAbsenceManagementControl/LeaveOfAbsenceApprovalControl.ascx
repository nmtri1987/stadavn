<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LeaveOfAbsenceApprovalControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.LeaveOfAbsenceManagementControl.LeaveOfAbsenceApprovalControl" %>
<table style="table-layout: fixed; width: 100% !important" id="leaveOfAbsenceApprovalContainer">
    <tr>
        <td style="width: 100%;" valign="top">
            <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="NotOverTimeApprovalWP" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="False"
                        ServerRender="False" ClientRender="True" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000"
                        IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle=""
                        EnableOriginalValue="False" DisplayName="My Leave Absence Request" ViewContentTypeId="" Default="TRUE"
                        ListUrl="Lists/NotOverTimeManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False"
                        AutoRefresh="False" AutoRefreshInterval="60" Title="Leave Of Absence For Ovetime Management"
                        FrameType="Default" SuppressWebPartChrome="False" Description="Not OverTime Management"
                        IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal"
                        AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True"
                        AllowHide="True" IsVisible="True" TitleUrl="/Lists/NotOverTimeManagement" DetailLink="/Lists/NotOverTimeManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part."
                        PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" ID="NotOverTimeApprovalControl" __MarkupType="vsattributemarkup"
                        WebPart="true" Height="" Width="">
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
                            <View BaseViewID="4" Type="HTML" WebPartZoneID="Main" DisplayName="Approval List View"  MobileView="TRUE"  SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="ApprovalList.aspx">
                                <Query>
                                    <Where>
                                        <Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>
                                    </Where>
                                    <OrderBy>
                                        <FieldRef Name="CommonFrom" Ascending="FALSE"></FieldRef>
                                        <FieldRef Name="ID" Ascending="FALSE" />
                                    </OrderBy>
                                </Query>
                                <ViewFields>
                                    <FieldRef Name="Requester"/>
                                    <FieldRef Name="CommonDepartment"/>
                                    <FieldRef Name="HoursPerDay"/>
                                    <FieldRef Name="CommonDate"/>
                                    <FieldRef Name="CommonFrom"/>
                                    <FieldRef Name="To"/>
                                    <FieldRef Name="Reason"/>
                                    <FieldRef Name="ApprovalStatus"/>
                                    <FieldRef Name="CommonReqDueDate"/>
                                    <FieldRef Name="Title"/>
                                </ViewFields>
                                <RowLimit Paged="TRUE">20</RowLimit>
                                <JSLink>clienttemplates.js</JSLink>
                                <XslLink Default="TRUE">main.xsl</XslLink>
                                <Toolbar Type="Standard"/>
                            </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/NotOvertimeModule/JSLink_TaskList_NotOvertime_Approval.js?v=1.4</JSLink>
                    </WebPartPages:XsltListViewWebPart>
                </ZoneTemplate>
            </WebPartPages:WebPartZone>
        </td>
    </tr>
</table>
