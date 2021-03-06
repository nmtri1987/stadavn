﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyShiftTime.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.MyShiftTime" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/ShiftModule/MyShift.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="col-md-12">
    </div>
    <table style="table-layout: fixed; width: 100% !important">

        <tr>
            <td style="width: 100%;" valign="top">
                <asp:Label runat="server" ID="Label1" Text="<%$Resources:RBVHStadaWebpages,MyShiftTime_InMonth%>" Font-Bold="True"></asp:Label>
                <br />
                <div class="form-group">
                            <div class="input-append  date inner-addon right-addon txtCalendar"  id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                                <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                                <asp:TextBox ID="DateTextbox" ClientIDMode="Static" runat="server"  CssClass="form-control " ReadOnly="true" ></asp:TextBox>
                            </div>
                    <sa
                </div>
                <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
                <asp:HiddenField ID="HiddenMonth" runat="server"></asp:HiddenField>
                <asp:HiddenField ID="HiddenYear" runat="server"></asp:HiddenField>
                <br />
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main">
                    <ZoneTemplate>
                        <WebPartPages:XsltListViewWebPart
                            runat="server" ViewFlag="" ViewSelectorFetchAsync="False" InplaceSearchEnabled="True" ServerRender="False"
                            ClientRender="False" InitialAsyncDataFetch="False" WebId="00000000-0000-0000-0000-000000000000" IsClientRender="False" GhostedXslLink="main.xsl"
                            NoDefaultStyle="" ViewGuid="00000000-0000-0000-0000-000000000000" EnableOriginalValue="False"
                            DisplayName="My Shift" ViewContentTypeId="" Default="TRUE" ListUrl="Lists/ShiftManagementDetail" ListDisplayName="" PageType="PAGE_DEFAULTVIEW" PageSize="-1"
                            UseSQLDataSourcePaging="True" DataSourceID=""
                            ShowWithSampleData="False" AsyncRefresh="False" ManualRefresh="False" AutoRefresh="False" AutoRefreshInterval="60" Title="Shift Management" FrameType="Default"
                            SuppressWebPartChrome="False" Description="Shift Management" IsIncluded="True" ZoneID="Main" PartOrder="2" FrameState="Normal" AllowRemove="True" AllowZoneChange="True"
                            AllowMinimize="True" AllowConnect="True" AllowEdit="True" AllowHide="True" IsVisible="True" TitleUrl="/Lists/ShiftManagementDetail" DetailLink="/Lists/ShiftManagementDetail"
                            HelpLink="" HelpMode="Modeless" Dir="Default" PartImageSmall="" MissingAssembly="Cannot import this Web Part." PartImageLarge="" IsIncludedFilter="" ExportControlledProperties="False"
                            ConnectionID="00000000-0000-0000-0000-000000000000" __MarkupType="vsattributemarkup"
                            __AllowXSLTEditing="true" ID="MyShift" WebPart="true" Height="" Width="">
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
                     <ParameterBinding Name="Month" Location="QueryString(Month)" DefaultValue="0"/>
                     <ParameterBinding Name="Year" Location="QueryString(Year)" DefaultValue="0"/>
                            </ParameterBindings>
                            <DataFields></DataFields>
                            <XmlDefinition>
                                 <View Name="My Shift" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="My Requests" 
                                    Url="MyShiftRequests.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
                                   <Query>
                                        <Where>
                                            <And>
                                                  <Eq>
                                                        <FieldRef Name="Employee" LookupId="TRUE"/>
                                                        <Value Type="Lookup">{RequesterLookupID}</Value>
                                                  </Eq> 
                                                   <And>
                                                        <Eq>
                                                           <FieldRef Name='CommonMonth' />
                                                           <Value Type='Lookup'>{Month}</Value>
                                                        </Eq>
                                                        <Eq>
                                                           <FieldRef Name='CommonYear' />
                                                           <Value Type='Lookup'>{Year}</Value>
                                                        </Eq>
                                                   </And>
                                            </And>
                                              
                                           </Where>
                                         <OrderBy><FieldRef Name="ID" Ascending="False" /></OrderBy>
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
                                    <RowLimit Paged="TRUE">10</RowLimit>
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
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server" >
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_MShiftRequestTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,ShiftManagement_MShiftRequestTitle%>" />
</asp:Content>
