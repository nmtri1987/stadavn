<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyOverTime.aspx.cs" Inherits="RBVH.Stada.Intranet.WebPages.Layouts.RBVH.Stada.Intranet.WebPages.MyOverTime" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/css/datepicker.css" />
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jquery-ui.min.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/Bootstrap/DatePicker/js/bootstrap-datepicker.js"></script>
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/OvertimeModule/MyOvertime.js?v=<%= DateTime.Now.ToString("yyyy.MM.dd") %>"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <table style="table-layout: fixed; width: 100% !important">

        <tr>
            <td style="width: 100%;" valign="top">
                <asp:Label runat="server" ID="Label1" Text="<%$Resources:RBVHStadaWebpages,MyOverTime_InMonth%>" Font-Bold="True"></asp:Label>

                <div class="form-group">
                    <div class="input-append  date inner-addon right-addon txtCalendar" id="dpMonths" data-date="102/2012" data-date-format="mm/yyyy" data-date-viewmode="years" data-date-minviewmode="months">
                        <i class="glyphicon glyphicon-calendar" style="padding: 5px !important; font-size: 19px; color: #0865bd"></i>
                        <asp:TextBox ID="DateTextbox" ClientIDMode="Static" runat="server" CssClass="form-control " ReadOnly="true"></asp:TextBox>
                    </div>
                </div>
                <asp:HiddenField ID="ParamRequesterLookupIDHidden" runat="server"></asp:HiddenField>
                <asp:HiddenField ID="HiddenStartMonth" runat="server"></asp:HiddenField>
                <asp:HiddenField ID="HiddenEndMonth" runat="server"></asp:HiddenField>
                <br />
                <WebPartPages:WebPartZone runat="server" FrameType="None" ID="WebPartZone1" Title="loc:Main">
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
                     <ParameterBinding Name="StartMonth" Location="QueryString(StartMonth)" DefaultValue="0"/>
                     <ParameterBinding Name="EndMonth" Location="QueryString(EndMonth)" DefaultValue="0"/>
                            </ParameterBindings>
                            <DataFields></DataFields>
                            <XmlDefinition>
                                 <View Name="My Shift" DefaultView="TRUE" MobileView="TRUE" MobileDefaultView="TRUE" Type="HTML" DisplayName="My Requests" 
                                    Url="MyShiftRequests.aspx" Level="1" BaseViewID="1" ContentTypeID="0x" ImageUrl="/_layouts/15/images/generic.png?rev=23" >
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
                                         <OrderBy><FieldRef Name="ID" Ascending="False" /></OrderBy>
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
                                              <FieldRef Name="HM" />
                                              <FieldRef Name="KD" />
                                    </ViewFields>
                                    <RowLimit Paged="TRUE">10</RowLimit>
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
    <script type="text/javascript">
        $(document).ready(function () {
            var settings = {
                MonthControlSelector: '#DateTextbox',
                MonthHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenMonth',
                YearHiddenControlSelector: '#ctl00_PlaceHolderMain_HiddenYear',
            };
            myOvertimeInstance = new RBVH.Stada.WebPages.pages.MyOvertime(settings);
        });
    </script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyOverTime_PageTitle%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MyOverTime_PageTitle%>" />
</asp:Content>
