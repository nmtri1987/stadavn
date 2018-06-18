<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskApprovalControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.Common.TaskApprovalControl" %>
<table style="table-layout: fixed; width: 100% !important" class="custom-table-view">
    <tr>
        <td style="width: 100%;" valign="top">
            <br />
            <WebPartPages:WebPartZone runat="server" FrameType="None" ID="ApprovalControlWebpartZone" Title="loc:Main">
                <ZoneTemplate>
                    <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False" 
                        DisplayName="Task List" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/TaskList" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Task List" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Task List" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/TaskList" DetailLink="/Lists/TaskList"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="TaskApprovalControlWP" WebPart="true" Height="" Width="">
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
                                <ParameterBinding Name="Module" Location="QueryString(AdminModule)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                        <View TabularView="FALSE" BaseViewID="10" Type="HTML" WebPartZoneID="Main" DisplayName="Approval Tasks" MobileView="True" MobileDefaultView="False" Url="ApprovalTasks.aspx" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/issues.png?rev=23" ReqAuth="TRUE">
                            <Query>
                                <Where>
                                    <And>   
                                        <In>
                                            <FieldRef Name="AssignedTo" LookupId="TRUE" />
                                            <Values>
                                                <Value Type="Integer">
                                                    <UserID />
                                                </Value>
                                            </Values>
                                        </In> 
                                        <And>
                                            <Eq>
                                                <FieldRef Name='StepModule' />
                                                <Value Type='Choice'>{Module}</Value>
                                            </Eq>
                                            <Eq>
                                                <FieldRef Name='Status' />
                                                <Value Type='Choice'>In Progress</Value>
                                            </Eq>
                                        </And>
                                    </And>
                                </Where>
                                <OrderBy><FieldRef Name="DueDate" Ascending="TRUE" /></OrderBy>
                            </Query>
                            <ViewFields>
                                <FieldRef Name="Title"></FieldRef>
                                <FieldRef Name="StartDate"></FieldRef>
                                <FieldRef Name="DueDate"></FieldRef>
                                <FieldRef Name="AssignedTo"></FieldRef>
                                <FieldRef Name="CurrentStepStatus"></FieldRef>
                            </ViewFields>
                            <RowLimit Paged="TRUE">20</RowLimit>
                                <XslLink Default="TRUE">main.xsl</XslLink>
                            </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/TaskModule/JSLink_TaksList_View.js?v=1.0</JSLink>
                    </WebPartPages:XsltListViewWebPart>
                </ZoneTemplate>
            </WebPartPages:WebPartZone>
        </td>
    </tr>
    <tr>
        <td id="newTasks" runat="server"></td>
    </tr>
</table>