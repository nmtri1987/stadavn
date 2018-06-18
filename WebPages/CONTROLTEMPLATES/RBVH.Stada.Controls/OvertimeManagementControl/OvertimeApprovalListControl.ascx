<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OvertimeApprovalListControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.OvertimeManagementControl.OvertimeApprovalListControl" %>
<div class="panel panel-primary">
   <div class="panel-body">
      <table style="table-layout: fixed; width: 100% !important" id="overtime-approval-list-container">
         <tr>
            <td style="width: 100%;" valign="top">
               <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
               <br />
               <WebPartPages:WebPartZone runat="server" FrameType="None" ID="OvertimeApprovalWP" Title="loc:Main">
                  <ZoneTemplate>
                     <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False" ViewGuid="00000000-0000-0000-0000-000000000000"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Overtime Management" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/OvertimeManagement" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Change Shift Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Overtime Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/OvertimeManagement" DetailLink="/Lists/OvertimeManagement"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="OvertimeApprovalControl" WebPart="true" Height="" Width="">
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
                           <View BaseViewID="2" Type="HTML" WebPartZoneID="Main" TabularView="FALSE" DisplayName="Overtime Approval List"  MobileView="TRUE" MobileDefaultView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="ApprovalList.aspx">
                              <Toolbar Type="Standard" />
                              <XslLink Default="TRUE">main.xsl</XslLink>
                              <RowLimit Paged="TRUE">20</RowLimit>
                              <ViewFields>
                                 <FieldRef Name="Requester" />
                                 <FieldRef Name="CommonDepartment" />
                                 <FieldRef Name="CommonDate" />
                                 <FieldRef Name="SumOfEmployee" />
                                 <FieldRef Name="SumOfMeal" />
                                 <FieldRef Name="CommonLocation" />
                                 <FieldRef Name="OtherRequirements" />
                                 <FieldRef Name="ApprovalStatus" />
                                 <FieldRef Name="Created" />
                              </ViewFields>
                              <Query>
                                 <Where>
                                     <And>
                                        <Eq>
                                           <FieldRef Name='CommonApprover1' LookupId="True" />
                                           <Value Type='User' LookupId="True">
                                              <UserID/>
                                           </Value>
                                        </Eq>
                                        <IsNull>
                                            <FieldRef Name='ApprovalStatus' />
                                        </IsNull>
                                     </And>
                                 </Where>
                                 <OrderBy>
                                     <FieldRef Name="ColForSort" Ascending="TRUE" />
                                     <FieldRef Name="CommonDate" Ascending="FALSE" />
                                     <FieldRef Name="ID" Ascending="FALSE" />
                                 </OrderBy>
                              </Query>
                              <ParameterBindings>
                                 <ParameterBinding Name="NoAnnouncements" Location="Resource(wss,noXinviewofY_LIST)" />
                                 <ParameterBinding Name="NoAnnouncementsHowTo" Location="Resource(wss,noXinviewofY_DEFAULT)" />
                              </ParameterBindings>
                           </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/JSLink_ApprovalView.js?v=1.2</JSLink>
                     </WebPartPages:XsltListViewWebPart>
                  </ZoneTemplate>
               </WebPartPages:WebPartZone>
            </td>
         </tr>
      </table>
   </div>
</div>