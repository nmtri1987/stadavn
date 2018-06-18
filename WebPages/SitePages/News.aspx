<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Page Language="C#" MasterPageFile="~masterurl/default.master" Inherits="Microsoft.SharePoint.WebPartPages.WebPartPage,Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
      <%--Ideal Image Slider--%>
      <SharePoint:ScriptLink ID="IdealImageSliderJS" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/IdealImageSlider/ideal-image-slider.js" runat="server" />
      <SharePoint:ScriptLink ID="IdealImageSliderJS1" Name="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/IdealImageSlider/iis-bullet-nav.js" runat="server" />

      <%-- Nivo ImageSlider CSS--%>
      <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/css/IdealImageSlider/normalize.css" runat="server" />
      <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/css/IdealImageSlider/ideal-image-slider.css" runat="server" />
      <SharePoint:CssRegistration Name="/_layouts/15/RBVH.Stada.Intranet.Branding/css/IdealImageSlider/ideal-default.css" runat="server" />
    <style>
        .ms-webpart-zone-custom {
            display: table;
            position: absolute;
            width: 70% !important;
            margin-right: 15%;
            min-width: 1024px;
        }
    </style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
   <div class="container container-custom">
        <div class="row">
           <div class="col-md-12">
             <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row2" FrameType="TitleBarOnly">
                    <ZoneTemplate>
                        <!--Company Pictures-->
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
           </div>
       </div>
       <div class="row">
        <div class="col-md-6">
            <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row1" FrameType="TitleBarOnly">
                    <ZoneTemplate>
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
        </div>
           <div class="col-md-6">
                 <WebPartPages:WebPartZone runat="server" Title="loc:Row1" ID="Row1_right" FrameType="TitleBarOnly">
                    <ZoneTemplate>
                        <!--Medical News-->
                    </ZoneTemplate>
                </WebPartPages:WebPartZone>
           </div>
      </div>

   </div>
<%--    <table id="Home" style="table-layout: fixed; min-width:1024px; ">
        <tr>
            <td style="width: 45%" valign="top">
                <!--Internal News-->
               
            </td>
            <!--Right side-->
            <td style="width: 55%; padding: 0 0 0 3%" valign="top" rowspan="2">
              
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="ms-webpart-zone-custom">

                    </div>
            </td>
        </tr>
    </table>--%>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    <SharePoint:ProjectProperty Property="Title" runat="server"/> - <asp:Literal runat="server" Text="<%$Resources:RBVHStadaMasterPage,InternalNews_TopMenu%>" />
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
    <asp:Literal runat="server" Text="<%$Resources:RBVHStadaMasterPage,InternalNews_TopMenu%>" />
</asp:Content>
