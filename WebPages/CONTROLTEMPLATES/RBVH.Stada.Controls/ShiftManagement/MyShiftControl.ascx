<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyShiftControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement.MyShiftControl" %>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/MyShift.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
<style>
    button.ui-dialog-titlebar-close
    {
        min-width: 25px;
    }
    #leave-link
    {
        cursor: pointer;
    }
</style>
<div class="panel panel-primary">
    <div id="leave-dialog" style="display: none;" title='<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_LeavePopupTitle%>" />'>
        <br/>
        <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftRequest_LeavePopupContent%>" />
    </div>
   <div class="panel-body">
      <table style="table-layout: fixed; width: 100% !important" id="my-shift-container">
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
                        <asp:TextBox ID="txtMyShiftMonth" ClientIDMode="Static" runat="server" Width="100%" CssClass="form-control" style="cursor: pointer;" ReadOnly="true"></asp:TextBox>
                     </div>
                  </div>
               </div>
            </td>
         </tr>
         <tr>
         <tr>
            <td style="width: 100%;" valign="top">
               <asp:HiddenField ID="ParamMyShiftRequesterLookupIDHidden" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="HiddenMonth" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="HiddenYear" runat="server"></asp:HiddenField>
               <br />
               <WebPartPages:WebPartZone runat="server" FrameType="None" ID="MyShiftWebPartZone" Title="loc:Main">
                  <ZoneTemplate>
                     <WebPartPages:XsltListViewWebPart
                        runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                        ClientRender="False" InitialAsyncDataFetch="False" ViewGuid="00000000-0000-0000-0000-000000000000" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                        NoDefaultStyle="" EnableOriginalValue="False"
                        DisplayName="My Shift" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/ShiftManagementDetail" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                        UseSQLDataSourcePaging="True" DataSourceID=""
                        ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                        SuppressWebPartChrome="False" Description="Shift Management" IsIncluded="True" ZoneID="wpz" PartOrder="4" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                        AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/ShiftManagementDetail" DetailLink="/Lists/ShiftManagementDetail"
                        HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                        ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                        __AllowXSLTEditing="true" ID="MyShiftWebPart" WebPart="true" Height="" Width="">
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
                           <ParameterBinding Name="MyShift_RequesterLookupID" Location="Control(ParamMyShiftRequesterLookupIDHidden, Value)" DefaultValue="1"/>
                           <ParameterBinding Name="MyShift_Month" Location="QueryString(MyMonth)" DefaultValue="0"/>
                           <ParameterBinding Name="MyShift_Year" Location="QueryString(MyYear)" DefaultValue="0"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                           <View Name="MyShiftControlView" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="My Requests" 
                              Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
                              <Query>
                                 <Where>
                                    <And>
                                       <Eq>
                                          <FieldRef Name="Employee" LookupId="TRUE"/>
                                          <Value Type="Lookup">{MyShift_RequesterLookupID}</Value>
                                       </Eq>
                                       <And>
                                          <Eq>
                                             <FieldRef Name='CommonMonth' />
                                             <Value Type='Lookup'>{MyShift_Month}</Value>
                                          </Eq>
                                          <Eq>
                                             <FieldRef Name='CommonYear' />
                                             <Value Type='Lookup'>{MyShift_Year}</Value>
                                          </Eq>
                                       </And>
                                    </And>
                                 </Where>
                                 <OrderBy>
                                    <FieldRef Name="ID" Ascending="FALSE" />
                                 </OrderBy>
                              </Query>
                              <ViewFields>
                                 <FieldRef  Name="ShiftManagementID" />
                                 <FieldRef  Name="CommonMonth"  />
                                 <FieldRef  Name="CommonYear"  />
                                 <FieldRef  Name="ShiftTime1"  />
                                 <FieldRef  Name="ShiftTime2"  />
                                 <FieldRef  Name="ShiftTime3"  />
                                 <FieldRef  Name="Employee" />
                                 <FieldRef  Name="ShiftTime4" />
                                 <FieldRef  Name="ShiftTime5" />
                                 <FieldRef  Name="ShiftTime6" />
                                 <FieldRef  Name="ShiftTime7" />
                                 <FieldRef  Name="ShiftTime8" />
                                 <FieldRef  Name="ShiftTime9" />
                                 <FieldRef  Name="ShiftTime10"  />
                                 <FieldRef  Name="ShiftTime11"  />
                                 <FieldRef  Name="ShiftTime12"  />
                                 <FieldRef  Name="ShiftTime13"  />
                                 <FieldRef  Name="ShiftTime14"  />
                                 <FieldRef  Name="ShiftTime15"  />
                                 <FieldRef  Name="ShiftTime16"  />
                                 <FieldRef  Name="ShiftTime17"  />
                                 <FieldRef  Name="ShiftTime18"  />
                                 <FieldRef  Name="ShiftTime19"  />
                                 <FieldRef  Name="ShiftTime20"  />
                                 <FieldRef  Name="ShiftTime21"  />
                                 <FieldRef  Name="ShiftTime22"  />
                                 <FieldRef  Name="ShiftTime23"  />
                                 <FieldRef  Name="ShiftTime24"  />
                                 <FieldRef  Name="ShiftTime25"  />
                                 <FieldRef  Name="ShiftTime26"  />
                                 <FieldRef  Name="ShiftTime27"  />
                                 <FieldRef  Name="ShiftTime28"  />
                                 <FieldRef  Name="ShiftTime29"  />
                                 <FieldRef  Name="ShiftTime30"  />
                                 <FieldRef  Name="ShiftTime31"  />
                                 <FieldRef  Name="EmployeesShift"  />
                                 <FieldRef  Name="ShiftTime1Approval"  />
                                 <FieldRef  Name="ShiftTime2Approval"  />
                                 <FieldRef  Name="ShiftTime3Approval"  />
                                 <FieldRef  Name="ShiftTime4Approval"  />
                                 <FieldRef  Name="ShiftTime5Approval"  />
                                 <FieldRef  Name="ShiftTime6Approval"  />
                                 <FieldRef  Name="ShiftTime7Approval"  />
                                 <FieldRef  Name="ShiftTime8Approval"  />
                                 <FieldRef  Name="ShiftTime9Approval"  />
                                 <FieldRef  Name="ShiftTime10Approval"  />
                                 <FieldRef  Name="ShiftTime11Approval"  />
                                 <FieldRef  Name="ShiftTime12Approval"  />
                                 <FieldRef  Name="ShiftTime13Approval"  />
                                 <FieldRef  Name="ShiftTime14Approval"  />
                                 <FieldRef  Name="ShiftTime15Approval"  />
                                 <FieldRef  Name="ShiftTime16Approval"  />
                                 <FieldRef  Name="ShiftTime17Approval"  />
                                 <FieldRef  Name="ShiftTime18Approval"  />
                                 <FieldRef  Name="ShiftTime19Approval"  />
                                 <FieldRef  Name="ShiftTime20Approval"  />
                                 <FieldRef  Name="ShiftTime21Approval"  />
                                 <FieldRef  Name="ShiftTime22Approval"  />
                                 <FieldRef  Name="ShiftTime23Approval"  />
                                 <FieldRef  Name="ShiftTime24Approval"  />
                                 <FieldRef  Name="ShiftTime25Approval"  />
                                 <FieldRef  Name="ShiftTime26Approval"  />
                                 <FieldRef  Name="ShiftTime27Approval"  />
                                 <FieldRef  Name="ShiftTime28Approval"  />
                                 <FieldRef  Name="ShiftTime29Approval"  />
                                 <FieldRef  Name="ShiftTime30Approval"  />
                                 <FieldRef  Name="ShiftTime31Approval"  />
                              </ViewFields>
                              <RowLimit Paged="TRUE">20</RowLimit>
                              <JSLink>clienttemplates.js</JSLink>
                              <XslLink Default="TRUE">main.xsl</XslLink>
                              <Toolbar Type="Standard"/>
                           </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/JSLink_MyShift_View.js?v=1.0</JSLink>
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
           MonthControlSelector: '#txtMyShiftMonth',
           MonthHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenMonth',
           YearHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenYear',
       };
       myShiftInstance = new RBVH.Stada.WebPages.pages.MyShift(settings);
   });
</script>