<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GuestReceptionFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.RequirementForGuestReceptionManagementControl.GuestReceptionFormUserControl" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/CommentControl.ascx" TagPrefix="CommonControls" TagName="CommentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/FormButtonsControl.ascx" TagPrefix="CommonControls" TagName="FormButtonsControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/WorkflowHistoryControl.ascx" TagPrefix="CommonControls" TagName="WorkflowHistoryControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/SupportingDocumentControl.ascx" TagPrefix="CommonControls" TagName="SupportingDocumentControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid-theme.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/GuestReceptionModule/default.css" />
<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/bootstrap-datetimepicker/bootstrap-datetimepicker.css" />

<div class="border-container custom-form guest-reception-container" style="display: inline-block; width: 100%;">
    <!-- Information of guest -->
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestInformationTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal ID="litCompanyName" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_CompanyNameTitle%>" />
                <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:TextBox ID="txtCompanyName" runat="server" CssClass="ms-long customInput s-required" Style="width: 100%;" MaxLength="255" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <div id="grid-guest" class="s-required">
            </div>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-12 note">
            <asp:Literal ID="litGuestInfo" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestVisaRequiredFieldsTitle%>" />
        </div>
    </div>
    <!-- Visa Application -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_VisaApplicationTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbVisaApplication" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-md-12 note">
            <asp:Literal ID="litVisaApplicationInfo" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_VisaApplicationInfomationTitle%>" />
        </div>
    </div>
    <!-- Hotel booking -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelBookingTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbHotelBooking" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div id="hotel-booking-container">
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelName" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelNameTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelName" runat="server" CssClass="ms-long customInput s-required" Style="width: 100%;" MaxLength="255" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelAddress" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelAddressTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelAddress" runat="server" CssClass="ms-long customInput s-required" Style="width: 100%;" MaxLength="255" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:CheckBoxList ID="cblHotelRoomType" runat="server" RepeatDirection="Horizontal" CssClass="spacing-right">
                    <asp:ListItem Value="Single" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelSingleRoomTitle%>" />
                    <asp:ListItem Value="Double" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelDoubleRoomTitle%>" />
                </asp:CheckBoxList>
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4 col-xs-2">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litCheckInDate" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelCheckinDateTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </span>
            </div>
            <div class="col-lg-4 col-md-2 col-xs-4">
                <SharePoint:DateTimeControl ID="dtCheckInDate" runat="server" LocaleId="2057" CssClassTextBox="s-required ms-input date-picker" />
            </div>
            <div class="col-lg-2 col-md-4 col-xs-2">
                <span class="ms-standardheader">
                    <asp:Literal ID="litCheckOutDate" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelCheckoutDateTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </span>
            </div>
            <div class="col-lg-4 col-md-2 col-xs-4">
                <SharePoint:DateTimeControl ID="dtCheckOutDate" runat="server" LocaleId="2057" CssClassTextBox="s-required ms-input date-picker" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelOtherRequirement" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelOtherRequirement%>" />
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelOtherRequirement" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelPaidBy" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelRoomPaidByTitle%>" />
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelPaidBy" runat="server" CssClass="ms-long customInput" Style="width: 100%;" MaxLength="255" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelMinibarPaidBy" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelMinibarPaidByTitle%>" />
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelMiniBarPaidBy" runat="server" CssClass="ms-long customInput" Style="width: 100%;" MaxLength="255" />
            </div>
        </div>
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litHotelOtherServicePaidBy" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelOtherServicePaidByTitle%>" />
                </span>
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:TextBox ID="txtHotelOtherServicePaidBy" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
            </div>
        </div>
    </div>
    <!-- Pick-up car -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupCarTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbPickupCarAtAirport" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div id="pick-up-car-airport-container">
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-2 col-sm-2">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litPickupDateTime" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_DatetimeTitle%>" />
                </span>
            </div>
            <div class="col-lg-4 col-md-4 col-sm-4">
                <SharePoint:DateTimeControl ID="dtPickupDatetime" runat="server" LocaleId="2057" HoursMode24="True" CssClassTextBox="date-picker" />
            </div>
            <div class="col-lg-2 col-md-2 col-sm-2">
                <span class="ms-standardheader">
                    <asp:Literal ID="litFlightNo" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupCarFlightNoTitle%>" />
                </span>
            </div>
            <div class="col-lg-4 col-md-4 col-sm-4">
                <asp:TextBox ID="txtFlightNo" runat="server" CssClass="ms-long customInput" Style="width: 100%;" MaxLength="255" />
            </div>
        </div>
    </div>

    <!-- Pick up to the company -->
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <div id="grid-company-pick-up">
            </div>
        </div>
    </div>
    <!-- Lunch service -->
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_LunchServiceTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:CheckBoxList ID="cblLunchService" runat="server" CssClass="spacing-right">
                <asp:ListItem Value="AtCompany" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_LunchServiceAtCompanyTitle%>" />
                <asp:ListItem Value="OtherPlace" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_LunchServiceAtOtherPlaceTitle%>" />
            </asp:CheckBoxList>
            <asp:TextBox ID="txtLunchServiceOtherPlace" runat="server" CssClass="ms-long customInput" MaxLength="255" Style="width: 100%;" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal ID="litLunchServiceSpecialRequirement" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_LunchServiceSpecialRequirement%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:TextBox ID="txtLunchServiceSpecialRequirement" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
        </div>
    </div>
    <!-- Reception level -->
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_ReceptionTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:CheckBoxList ID="cblReception" runat="server" RepeatDirection="Horizontal" CssClass="spacing-right" ClientIDMode="Static">
                <asp:ListItem Value="Casual" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_CasualTitle%>" />
                <asp:ListItem Value="Formal" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_FormalTitle%>" />
                <asp:ListItem Value="HighlyFormal" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HighlyFormalTitle%>" />
            </asp:CheckBoxList>
        </div>
    </div>
    <!-- Souvenir -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_SouvenirTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbSouvenir" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div id="souvenir-container">
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4">
            </div>
            <div class="col-lg-10 col-md-8">
                <asp:CheckBoxList ID="cblSouvenir" runat="server" RepeatDirection="Horizontal" CssClass="spacing-right" ClientIDMode="Static">
                    <asp:ListItem Value="Casual" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_CasualTitle%>" />
                    <asp:ListItem Value="Formal" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_FormalTitle%>" />
                    <asp:ListItem Value="HighlyFormal" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HighlyFormalTitle%>" />
                </asp:CheckBoxList>
            </div>
        </div>
    </div>

    <!-- Car to the airport -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_CarToTheAirportTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbCarToAirport" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div id="car-to-airport-container">
        <div class="row ms-formlabel">
            <div class="col-lg-2 col-md-4 col-sm-4">
                <span class="ms-h3 ms-standardheader">
                    <asp:Literal ID="litCheckIn" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_DatetimeTitle%>" />
                </span>
            </div>
            <div class="col-lg-4 col-md-4 col-sm-8">
                <span dir="none">
                    <SharePoint:DateTimeControl ID="dtCarToTheAirport" runat="server" LocaleId="2057" HoursMode24="True" CssClassTextBox="date-picker" />
                </span>
            </div>
            <div class="col-lg-6 col-md-4">
            </div>
        </div>
    </div>

    <!-- Guest Working Schedule -->
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingScheduleTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBox ID="cbGuestWorkingSchedule" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HasHotelBookingTitle%>" />
        </div>
    </div>
    <hr />
    <div id="guest-working-schedule-container">
        <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal ID="litWorkingFrom" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingScheduleFromTitle%>" />
            </span>
        </div>
        <div class="col-lg-4 col-md-2 col-sm-2">
            <SharePoint:DateTimeControl ID="dtWorkingFrom" runat="server" LocaleId="2057" DateOnly="true" CssClassTextBox="date-picker" />
        </div>
        <div class="col-lg-2 col-md-3 col-sm-3">
            <span class="ms-standardheader">
                <asp:Literal ID="litWorkingTo" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingScheduleToTitle%>" />
            </span>
        </div>
        <div class="col-lg-4 col-md-3 col-sm-3">
            <SharePoint:DateTimeControl ID="dtWorkingTo" runat="server" LocaleId="2057" DateOnly="true" CssClassTextBox="date-picker" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal ID="litWorkingPlace" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingPlaceTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:TextBox ID="txtWorkingPlace" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal ID="litWorkingContent" runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingContentTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:TextBox ID="txtWorkingContent" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
        </div>
    </div>
    </div>
    
    <!-- Requirements -->
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingRequirementTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingMeetingRoomTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:TextBox ID="txtSeats" runat="server" CssClass="ms-long customInput" Style="width: 50px; margin-right: 5px; display: inline;" MaxLength="3" />
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingMeetingRoomSeatsTitle%>" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBoxList ID="cblMeetingRoomStuffs" runat="server" CssClass="spacing-right">
                <asp:ListItem Value="FaceCloth" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_FaceClothTitle%>" />
                <asp:ListItem Value="Cake" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_CakeTitle%>" />
                <asp:ListItem Value="Fruits" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_FruitsTitle%>" />
            </asp:CheckBoxList>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingEquipmentTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBoxList ID="cblEquipments" runat="server" CssClass="spacing-right">
                <asp:ListItem Value="Projector" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_ProjectorTitle%>" />
                <asp:ListItem Value="LaserPen" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_LaserPenTitle%>" />
                <asp:ListItem Value="Micro" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_MicroTitle%>" />
                <asp:ListItem Value="BlackBoard" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_BlackBoardTitle%>" />
                <asp:ListItem Value="Other" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_OtherTitle%>" />
            </asp:CheckBoxList>
            <asp:TextBox ID="txtEquipmentOther" runat="server" CssClass="ms-long customInput" Style="width: 100%;" MaxLength="255" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingRoutineTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <asp:TextBox ID="txtWorkingRoutine" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingEntranceTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:CheckBoxList ID="cblWorkingEntrance" runat="server" RepeatDirection="Horizontal" CssClass="spacing-right">
                <asp:ListItem Value="Visitor" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingEntranceVisitors%>" />
                <asp:ListItem Value="Staff" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingEntranceStaffTitle%>" />
            </asp:CheckBoxList>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4 col-sm-4">
            <span class="ms-h3 ms-standardheader">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingClothTitle%>" />
            </span>
        </div>
        <div class="col-lg-10 col-md-8 col-sm-8">
            <asp:TextBox ID="txtWorkingCloth" runat="server" CssClass="ms-long customInput" Style="width: 50px; display: inline; margin-right: 5px;" MaxLength="3" />
            <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingClothSetTitle%>" />
        </div>
    </div>

    <!-- Common fields -->
    <div class="row ms-formlabel" style="display: none;">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litStatus" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Status%>" />
                </nobr>
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <span id="td-approval-status"></span>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-lg-2 col-md-4">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litComments" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Comments%>" />
                </nobr>
            </span>
        </div>
        <div class="col-lg-10 col-md-8">
            <CommonControls:CommentControl ID="CommentControl" runat="server" />
        </div>
    </div>

    <table>
        <tbody>
            <tr>
                <td style="width: 100%;">&nbsp;</td>
                <td>
                    <CommonControls:FormButtonsControl ID="FormButtonsControl" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
    <table style="width: 100% !important;">
        <tbody>
            <tr>
                <td>
                    <CommonControls:WorkflowHistoryControl ID="WorkflowHistoryControl" runat="server" />
                </td>
            </tr>
        </tbody>
    </table>
</div>

<asp:HiddenField ID="hdApprovalStatus" runat="server" />
<asp:HiddenField ID="hdGuestInfoData" runat="server" />
<asp:HiddenField ID="hdGuestPickupData" runat="server" />





<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/jsGrid/jsgrid.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/select2/select2.min.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/GuestReceptionModule/GuestReceptionForm.js?v=ASHSHF9F-26F4-4832-9BE5-91D2F913C848"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/bootstrap-datetimepicker/moment-with-locales.js"></script>
<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/libs/bootstrap-datetimepicker/bootstrap-datetimepicker.js"></script>


<script>
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                ContainerSelector: ".guest-reception-container",
                CheckInTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelCheckinDateTitle%>" />',
                CheckOutTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_HotelCheckoutDateTitle%>" />',
                WorkingFromTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingScheduleFromTitle%>" />',
                WorkingToTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestWorkingScheduleToTitle%>" />',

                VisaApplicationSelector: "#<%=cbVisaApplication.ClientID%>",
                HotelBookingSelector: "#<%=cbHotelBooking.ClientID%>",
                HotelBookingContainerSelector: "#hotel-booking-container",

                PickupCarAtAirportSelector: "#<%=cbPickupCarAtAirport.ClientID%>",
                PickupCarAtAirportContainerSelector: "#pick-up-car-airport-container",

                SouvenirSelector: "#<%=cbSouvenir.ClientID%>",
                SouvenirContainerSelector: "#souvenir-container",

                CarToAirportSelector: "#<%=cbCarToAirport.ClientID%>",
                CarToAirportContainerSelector: "#car-to-airport-container",

                GuestWorkingScheduleSelector: "#<%=cbGuestWorkingSchedule.ClientID%>",
                GuestWorkingScheduleContainerSelector: "#guest-working-schedule-container",

                OtherLunchServiceSelector: "#<%=cblLunchService.ClientID%> :checkbox[value=OtherPlace]",
                OtherLunchServiceTextSelector: "#<%=txtLunchServiceOtherPlace.ClientID%>",

                OtherEquipmentSelector: "#<%=cblEquipments.ClientID%> :checkbox[value=Other]",
                OtherEquipmentTextSelector: "#<%=txtEquipmentOther.ClientID%>",


                CheckInSelector: "#<%=dtCheckInDate.ClientID%>" + "_dtCheckInDateDate",
                CheckInImageSelector: "#<%=dtCheckInDate.ClientID%>" + "_dtCheckInDateDateDatePickerImage",
                CheckInHourSelector: "#<%=dtCheckInDate.Controls[1].ClientID%>",
                CheckInMinuteSelector: "#<%=dtCheckInDate.Controls[2].ClientID%>",

                CheckOutSelector: "#<%=dtCheckOutDate.ClientID%>" + "_dtCheckOutDateDate",
                CheckOutImageSelector: "#<%=dtCheckOutDate.ClientID%>" + "_dtCheckOutDateDateDatePickerImage",
                CheckOutHourSelector: "#<%=dtCheckInDate.Controls[1].ClientID%>",
                CheckOutMinuteSelector: "#<%=dtCheckInDate.Controls[2].ClientID%>",

                PickupSelector: "#<%=dtPickupDatetime.ClientID%>" + "_dtPickupDatetimeDate",
                PickupImageSelector: "#<%=dtPickupDatetime.ClientID%>" + "_dtPickupDatetimeDateDatePickerImage",
                PickupHourSelector: "#<%=dtPickupDatetime.Controls[1].ClientID%>",
                PickupMinuteSelector: "#<%=dtPickupDatetime.Controls[2].ClientID%>",

                CarToTheAirportSelector: "#<%=dtCarToTheAirport.ClientID%>" + "_dtCarToTheAirportDate",
                CarToTheAirportImageSelector: "#<%=dtCarToTheAirport.ClientID%>" + "_dtCarToTheAirportDateDatePickerImage",
                CarToTheAirportHourSelector: "#<%=dtCarToTheAirport.Controls[1].ClientID%>",
                CarToTheAirportMinuteSelector: "#<%=dtCarToTheAirport.Controls[2].ClientID%>",

                WorkingFromSelector: "#<%=dtWorkingFrom.ClientID%>" + "_dtWorkingFromDate",
                WorkingFromImageSelector: "#<%=dtWorkingFrom.ClientID%>" + "_dtWorkingFromDateDatePickerImage",

                WorkingToSelector: "#<%=dtWorkingTo.ClientID%>" + "_dtWorkingToDate",
                WorkingToImageSelector: "#<%=dtWorkingTo.ClientID%>" + "_dtWorkingToDateDatePickerImage",

                SeatSelector: "#<%=txtSeats.ClientID%>",
                WorkingClothSelector: "#<%=txtWorkingCloth.ClientID%>",

                ReceptionId: "cblReception",
                SouvenirId: "cblSouvenir",
                LunchServiceId: "cblLunchService",

                GuestInfoDataSelector: '#<%=hdGuestInfoData.ClientID%>',
                GuestPickupDataSelector: '#<%=hdGuestPickupData.ClientID%>',

                RequiredFieldAttribute: "s-required",
                RequiredFieldClass: ".s-required",
                RequiredErrorElement: "<span class='ms-formvalidation'></span>",

                ApprovalStatusValueSelector: '#<%=hdApprovalStatus.ClientID%>',
                ApprovalStatusTrSelector: '#tr-approval-status',
                ApprovalStatusTdSelector: '#td-approval-status',

                GuestInfoGridSelector: '#grid-guest',
                GuestPickupGridSelector: '#grid-company-pick-up',
                Grids:
                {
                    "GuestInfo": {
                        Fields: [],
                        Columns:
                        {
                            GuestName: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestNameTitle%>" />',
                            GuestGender: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestGenderTitle%>" />',
                            GuestNationality: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestNationalityTitle%>" />',
                            GuestJobTitle: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestJobTitle%>" />',
                            GuestPassportNo: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestPassportNoTitle%>" />',
                            GuestDateOfIssue: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestDateOfIssueTitle%>" />',
                            GuestDateOfArrival: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestDateOfArrivalTitle%>" />',
                            GuestVisaValidity: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_GuestVisaValidityTitle%>" />',
                        }
                    },
                    "GuestPickup": {
                        Fields: [],
                        Columns:
                        {
                            PickupDatetime: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupDatetimeTitle%>" />',
                            PickupWorkingPlace: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupWorkingPlaceTitle%>" />',
                            PickupAttendant: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,GuestReceptionManagement_PickupAttendantTitle%>" />',
                        }
                    }
                }


            },
            ResourceText:
            {
                NotGreaterThan_WithParam: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,NotGreaterThan_WithParam%>' />",
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                CantLessThanToday: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SelectedDateMustGreaterThanCurrentDate%>' />",
                StartDateLessThanEndDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,StartDateLessThanEndDate%>' />",
                InvalidDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,InvalidDate%>' />",
                DataMustBeDifferentAfterEdit: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DataMustBeDifferentAfterEdit%>' />",
                ConfirmDeleteMessage: '<asp:Literal runat="server" Text="<%$Resources:RBVHStadaLists,Overtime_DeleteRow%>" />',
            }
        }

        GuestReceptionModule.Initialize(settings);
    });
</script>
