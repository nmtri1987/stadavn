<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShiftForDepartmentControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement.ShiftForDepartmentControl" %>
<div class="panel panel-primary">
   <div class="panel-body">
      <table style="table-layout: fixed; width: 100% !important" id="shift-by-department-list-container">
         <tr>
            <td style="width: 100%" valign="top">
               <div class="form-inline">
                  <div class="form-group header-left lbl-fixed-width">
                     <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CommonDepartment%>" />
                     </label>
                  </div>
                  <div class="form-group">
                     <%--<span class="department" id="lblDepartment"></span>--%>
                     <select class="form-control" id="cbShiftAdminDepartment">
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
                        <asp:TextBox ID="txtShiftDateByDepartment" ClientIDMode="Static" runat="server" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                     </div>
                  </div>
               </div>
            </td>
         </tr>
         <tr>
            <td style="width: 100%;" valign="top" colspan="2">
               <asp:HiddenField ID="ParamShiftDepartmentRequesterIDHidden" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="ShiftDepartment_Month" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="ShiftDepartment_Year" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="ShiftDepartment_DepartmentID" runat="server"></asp:HiddenField>
               <br />
               <WebPartPages:WebPartZone runat="server" FrameType="None" ID="ShiftRequestDepartmentWebPartZone" Title="loc:Main">
                  <ZoneTemplate>
                     <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" PartOrder="1" ViewSelectorFetchAsync="False" InplaceSearchEnabled="False" ServerRender="False" ClientRender="False" InitialAsyncDataFetch="False"
                        WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="Shift In Department" ViewContentTypeId="" ViewGuid="00000000-0000-0000-0000-000000000000" Default="TRUE" ListUrl="Lists/ShiftManagement" ListDisplayName=""
                        PageType="PAGE_DEFAULTVIEW" PageSize="-1" UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False"
                        AutoRefreshInterval="60" Title="Shift Management" FrameType="Default" SuppressWebPartChrome="False" Description="Manage working shift " IsIncluded="True" ZoneID="wpz"
                        FrameState="Normal" AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True"
                        TitleUrl="/Lists/ShiftManagement" DetailLink="/Lists/ShiftManagement" HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part."
                        PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False" ConnectionID="00000000-0000-0000-0000-000000000000"
                        __MarkupType="vsattributemarkup" __AllowXSLTEditing="true" ID="ShiftRequestDepartmentWebPart" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="">
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
                           <ParameterBinding Name="ShiftDepartmentParam_Month" Location="QueryString(AdminMonth)" DefaultValue="0"/>
                           <ParameterBinding Name="ShiftDepartmentParam_Year" Location="QueryString(AdminYear)" DefaultValue="0"/>
                           <ParameterBinding Name="ShiftDepartmentParam_DeptId" Location="QueryString(AdminDeptId)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                           <View BaseViewID="3" DisplayName="Shift By Department List" TabularView="FALSE" Type="HTML" ReadOnly="TRUE" WebPartZoneID="Main" SetupPath="pages\viewpage.aspx" Url="ShiftByDepartmentList.aspx">
                              <Query>
                                 <Where>
                                     <Eq><FieldRef Name='ID' /><Value Type='Counter'>0</Value></Eq>
                                 </Where>
                                 <OrderBy>
                                    <FieldRef Name='CommonDepartment' Ascending='TRUE' />
                                    <FieldRef Name='CommonYear' Ascending='FALSE' />
                                    <FieldRef Name="CommonMonth" Ascending="FALSE"></FieldRef>
                                    <FieldRef Name="ID" Ascending="FALSE" />
                                 </OrderBy>
                              </Query>
                              <ViewFields>
                                 <FieldRef Name="Requester"/>
                                 <FieldRef Name="CommonMonth"/>
                                 <FieldRef Name="CommonYear"/>
                                 <FieldRef Name="CommonDepartment"/>
                                 <FieldRef Name="CommonLocation"/>
                                 <FieldRef Name="ApprovalStatus"/>
                                 <FieldRef Name="CommonApprover1"/>
                                 <FieldRef Name="CommonAddApprover1"/>
                              </ViewFields>
                              <RowLimit Paged="TRUE">20</RowLimit>
                              <JSLink>clienttemplates.js</JSLink>
                              <XslLink Default="TRUE">main.xsl</XslLink>
                              <Toolbar Type="Standard"/>
                           </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/JSLink_Management_Department.js?v=1.2</JSLink>
                     </WebPartPages:XsltListViewWebPart>
                  </ZoneTemplate>
               </WebPartPages:WebPartZone>
            </td>
         </tr>
      </table>
   </div>
</div>