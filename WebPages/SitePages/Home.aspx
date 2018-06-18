<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>

<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/calendar/minicalendar.css" />
    <script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/HandleCalendarView.js"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="container container-custom" style="width: 100%;">
        <div class="row">
            <div class="col-md-8 col-sm-6">
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row1" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row2" ID="Row2" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Documents-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
                <div class="row">

                    <%--                    <h2 style="text-align: justify;" class="ms-webpart-titleText">
                        <a accesskey="W" href="/Lists/CompanyDiscussion">
                            <nobr>
                                 <span><asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,CompanyDiscussion_ContentTypeTitle%>" /> </span>
                                </nobr>
                        </a>
                    </h2>--%>
                </div>

                <div class="row">
                    <WebPartPages:WebPartZone runat="server" FrameType="None" ID="DiscussionWebPartZone" Title="loc:Main">
                        <ZoneTemplate>
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
            </div>
            <div class="col-md-4 col-sm-6" style="padding-left: 25px">
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row_right" ID="Row_right" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Calendar here-->
                            <!--Calendar 1 here-->
                            <!--Links here-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>

                </div>
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row1_right" ID="Row1_right" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Calendar here-->

                            <!--Calendar 2 here-->
                            <!--Links here-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
                <div class="row">
                    <WebPartPages:WebPartZone runat="server" Title="loc:Row1_right" ID="LinkID" FrameType="TitleBarOnly">
                        <ZoneTemplate>
                            <!--Links Application here
                            <!--Links here-->
                        </ZoneTemplate>
                    </WebPartPages:WebPartZone>
                </div>
            </div>
        </div>

        <div id="calendarMonth" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarMonth%>" />
        </div>
        <div id="calendarWeek" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarWeek%>" />
        </div>
        <div id="calendarDay" hidden="hidden">
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,CalendarDay%>" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:core,nav_Home%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    <asp:Literal runat="server" Text="<%$Resources:core,nav_Home%>" />
</asp:Content>
