<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ShiftRequestControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.ShiftManagement.ShiftRequestControl" %>
<div class="panel panel-primary">
   <div class="panel-body">
      <div style="margin-bottom: 20px; margin-left: -5px">
         <button type="button" id="btnAddNewOvertime" class="btn btn-primary add-new-shift" runat="server">
            <i class='fa fa-plus' aria-hidden='true'></i>&nbsp;
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_AddNew%>" />
         </button>
      </div>
      <table style="table-layout: fixed; width: 100% !important" id="shift-request-list-container">
         <tr>
            <td style="width: 100%;" valign="top">
               <asp:HiddenField ID="ParamShiftRequestIDHidden" runat="server"></asp:HiddenField>
               <asp:HiddenField ID="ParamDepartmentIDHidden" runat="server"></asp:HiddenField>
               <br />
               <WebPartPages:WebPartZone runat="server" FrameType="None" ID="ShiftRequestControlWebPartZone" Title="loc:Main">
                  <ZoneTemplate>
                     <WebPartPages:XsltListViewWebPart runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" PartOrder="3" ServerRender="False" ClientRender="False" InitialAsyncDataFetch="False"
                        WebId="00000000-0000-0000-0000-000000000000"  IsClientRender="False" GhostedXslLink="main.xsl" NoDefaultStyle=""  EnableOriginalValue="False"
                        DisplayName="All Items" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/ShiftManagement" ListDisplayName=""
                        PageType="PAGE_DEFAULTVIEW" PageSize="-1" UseSQLDataSourcePaging="True" DataSourceID="" ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False"
                        AutoRefreshInterval="60" Title="Shift Management" FrameType="Default" SuppressWebPartChrome="False" Description="Manage working shift " IsIncluded="True" ZoneID="wpz"
                        FrameState="Normal" AllowRemove="True" AllowZoneChange="True" AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True"
                        TitleUrl="/Lists/ShiftManagement" DetailLink="/Lists/ShiftManagement" HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part."
                        PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False" ConnectionID="00000000-0000-0000-0000-000000000000"
                        __MarkupType="vsattributemarkup" __AllowXSLTEditing="true" ID="ShiftRequestsWebPart" __designer:CustomXsl="fldtypes_Ratings.xsl" WebPart="true" Height="" Width="">
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
                           <ParameterBinding Name="RequesterLookupID" Location="Control(ParamShiftRequestIDHidden, Value)" DefaultValue="1"/>
                           <ParameterBinding Name="DepartmentLookupID" Location="Control(ParamDepartmentIDHidden, Value)" DefaultValue="1"/>
                        </ParameterBindings>
                        <DataFields></DataFields>
                        <XmlDefinition>
                           <View BaseViewID="4" DisplayName="Shift Request List" TabularView="FALSE" Type="HTML" ReadOnly="TRUE" WebPartZoneID="Main" SetupPath="pages\viewpage.aspx" Url="ShiftRequestList.aspx">
                              <Query>
                                 <Where>
                                    <Eq>
                                       <FieldRef Name="CommonDepartment" LookupId="TRUE"/>
                                       <Value Type="Lookup">{DepartmentLookupID}</Value>
                                    </Eq>
                                 </Where>
                                 <OrderBy>
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
                                 <FieldRef Name="Editor"/>
                              </ViewFields>
                              <RowLimit Paged="TRUE">20</RowLimit>
                              <JSLink>clienttemplates.js</JSLink>
                              <XslLink Default="TRUE">main.xsl</XslLink>
                              <Toolbar Type="Standard"/>
                           </View>
                        </XmlDefinition>
                        <JSLink>~sitecollection/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/JSLink_Management.js?v=1.1</JSLink>
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
        $('.add-new-shift').on('click', function () {
            var sourceURL = window.location.href;
            sourceURL = Functions.removeParam('lang', sourceURL);
            sourceURL = encodeURIComponent(sourceURL);
            window.location = '//' + location.host + '/SitePages/ShiftRequest.aspx?subSection=ShiftManagement&Source=' + sourceURL;
        });
    });
</script>