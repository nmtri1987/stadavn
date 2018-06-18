<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyovertimeControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.OvertimeManagementControl.MyovertimeControl" %>
<div class="panel panel-primary">
   <div class="panel-body">
      <table style="table-layout: fixed; width: 100% !important" id="my-overtime-container">
         <tr>
            <td style="width: 100%" valign="top">
               <div class="form-inline">
                  <div class="form-group header-left lbl-fixed-width" style="width: 100px;">
                     <label>
                        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarMonth%>" />
                     </label>
                  </div>
                  <div class="form-group">
                     <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd; left: inherit;"></i>
                        <asp:TextBox ID="txtMyOvertimeMonth" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control"  style="cursor: pointer;" ReadOnly="true"></asp:TextBox>
                     </div>
                  </div>
               </div>
            </td>
         </tr>
         <tr>
            <td style="width: 100%;" valign="top">
               <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="HiddenStartMonth" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="HiddenEndMonth" runat="server"></asp:HiddenField>
               <br />
               <WebPartPages:WebPartZone runat="server" FrameType="None" ID="MyOvertimeControl" Title="loc:Main">
                  <ZoneTemplate>
                     <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                        DisplayName="My OverTime" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/OvertimeEmployeeDetails" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="My OverTime" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/OvertimeEmployeeDetails" DetailLink="/Lists/OvertimeEmployeeDetails"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="MyOvertime" WebPart="true" Height="" Width="">
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
                           <ParameterBinding Name="StartMonth" Location="QueryString(MyStartMonth)" DefaultValue="0"/>
                           <ParameterBinding Name="EndMonth" Location="QueryString(MyEndMonth)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                           <View BaseViewID="1" Type="HTML" WebPartZoneID="Main" DisplayName="My Overtime" TabularView="FALSE" MobileView="TRUE" SetupPath="pages\viewpage.aspx" ImageUrl="/_layouts/15/images/generic.png?rev=23" Url="AllItems.aspx">
                              <Query>
                                 <Where>
                                    <And>
                                       <Geq>
                                          <FieldRef Name='OvertimeFrom' />
                                          <Value IncludeTimeValue='TRUE' Type='DateTime'>{StartMonth}</Value>
                                       </Geq>
                                       <And>
                                          <Leq>
                                             <FieldRef Name='OvertimeFrom' />
                                             <Value IncludeTimeValue='TRUE' Type='DateTime'>{EndMonth}</Value>
                                          </Leq>
                                          <Eq>
                                             <FieldRef Name='Employee' LookupId="TRUE"/>
                                             <Value Type='Lookup'>{RequesterLookupID}</Value>
                                          </Eq>
                                       </And>
                                    </And>
                                 </Where>
                                 <OrderBy>
                                    <FieldRef Name="ColForSort" Ascending="TRUE" />
                                    <FieldRef Name="CommonDate" Ascending="FALSE" />
                                    <FieldRef Name="ID" Ascending="FALSE" />
                                 </OrderBy>
                              </Query>
                              <ViewFields>
                                 <FieldRef Name="OvertimeManagementID" />
                                 <FieldRef Name="CommonDate" />
                                 <FieldRef Name="ApprovalStatus" />
                                 <FieldRef Name="Employee" />
                                 <FieldRef Name="WorkingHours" />
                                 <FieldRef Name="OvertimeFrom" />
                                 <FieldRef Name="OvertimeTo" />
                                 <FieldRef Name="Task" />
                                 <FieldRef Name="CompanyTransport" />
                                 <FieldRef Name="SummaryLinks" />
                              </ViewFields>
                              <RowLimit Paged="TRUE">20</RowLimit>
                              <JSLink>clienttemplates.js</JSLink>
                              <XslLink Default="TRUE">main.xsl</XslLink>
                              <Toolbar Type="Standard"/>
                           </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/JSLink_MyOverTime_View.js?v=1.1</JSLink>
                     </WebPartPages:XsltListViewWebPart>
                  </ZoneTemplate>
               </WebPartPages:WebPartZone>
            </td>
         </tr>
      </table>
   </div>
</div>
<script type="text/javascript">
   $(document).ready(function () {
       var settings = {
           MonthControlSelector: '#txtMyOvertimeMonth',
           MonthHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenMonth',
           YearHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenYear',
       };
       myShiftInstance = new RBVH.Stada.WebPages.pages.MyOvertime(settings);
   });
</script>