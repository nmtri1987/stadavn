<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MeetingRoomFormUserControl.ascx.cs" Inherits="RBVH.Stada.Intranet.WebPages.CONTROLTEMPLATES.RBVH.Stada.Controls.MeetingRoomManagementControl.MeetingRoomFormUserControl" %>

<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/CommentControl.ascx" TagPrefix="CommonControls" TagName="CommentControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/FormButtonsControl.ascx" TagPrefix="CommonControls" TagName="FormButtonsControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/WorkflowHistoryControl.ascx" TagPrefix="CommonControls" TagName="WorkflowHistoryControl" %>
<%@ Register Src="~/_controltemplates/15/RBVH.Stada.Controls/Common/SupportingDocumentControl.ascx" TagPrefix="CommonControls" TagName="SupportingDocumentControl" %>

<link type="text/css" rel="stylesheet" href="/_layouts/15/RBVH.Stada.Intranet.Branding/css/MeetingRoomModule/default.css?v=8080EBBE-4D6B-4FB4-9F13-F52D0438E5D2" />



<div class="border-container custom-form meeting-room-container" style="display: inline-block; width: 100%;">
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_GeneralInfoTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-md-2 col-xs-3">
            <span class="ms-h3 ms-standardheader wrap">
                <asp:Literal ID="litFullNameTitle" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_FullNameTitle%>" />
            </span>
        </div>
        <div class="col-md-4 col-xs-3">
            <asp:Label ID="lblRequester" runat="server" />
        </div>
        <div class="col-md-2 col-xs-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litDepartmentTitle" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_DepartmentTitle%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-4 col-xs-4">
            <asp:Label ID="lblDepartment" runat="server" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader wrap">
                <asp:Literal ID="litDiscussionMeetingTitle" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_DiscussionMeetingTitle%>" />
                <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
            </span>
        </div>
        <div class="col-md-10">
            <asp:TextBox ID="txtDiscussionMeeting" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
            <span id="discussion-meeting-error" class="ms-formvalidation" style="margin-top: 0px;" target-id="<%=txtDiscussionMeeting.ClientID %>"></span>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader wrap">
                    <asp:Literal ID="litParticipation" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_ParticipationTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
            </span>
        </div>
        <div class="col-md-10">
            <asp:TextBox ID="txtParticipation" runat="server" TextMode="MultiLine" Rows="4" Columns="20" CssClass="ms-long" />
            <span id="participation-error" class="ms-formvalidation" style="margin-top: 0px;" target-id="<%=txtParticipation.ClientID %>"></span>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_RegisteredEquipmentTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-md-2 col-xs-3">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litLocation" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_LocationTitle%>"/>
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </nobr>
            </span>
        </div>
        <div class="col-md-4 col-xs-3">
            <span dir="none">
                <asp:DropDownList ID="ddlLocation" runat="server" DataValueField="ID" DataTextField="Title" CssClass="customSelect" Style="width: 100%;"></asp:DropDownList>
            </span>
            <span id="location-error" class="ms-formvalidation" style="margin-top: 0px;" target-id="<%=ddlLocation.ClientID %>"></span>
        </div>

        <div class="col-md-2 col-xs-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litEquipment" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_EquipmentTitle%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-4 col-xs-4">
            <span dir="none">
                <asp:CheckBoxList ID="cblEquipment" runat="server" DataValueField="ID" CssClass="wrap-item" />
            </span>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litSeats" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_SeatsTitle%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-2 col-xs-6">
            <asp:TextBox ID="txtSeats" runat="server" CssClass="ms-long customInput" MaxLength="3" Style="width: 100%;" />
        </div>
        <div class="col-md-8">
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litOthers" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_OthersTitle%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-10">
            <asp:TextBox ID="txtOthers" runat="server" CssClass="ms-long customInput" Style="width: 100%;" MaxLength="255" />
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-12">
            <span class="ms-h2 ms-standardheader ">
                <asp:Literal runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_RequestedDateTitle%>" />
            </span>
        </div>
    </div>
    <hr />
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litStartTime" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_StartTimeTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </nobr>
            </span>
        </div>
        <div class="col-md-2">
            <span dir="none">
                <SharePoint:DateTimeControl ID="dtStartTime" runat="server" LocaleId="2057" HoursMode24="True" />
            </span>
            <span id="start-time-error" class="ms-formvalidation" style="margin-top: 0px;" target-id="ctl00_PlaceHolderMain_MeetingFormUserControl_dtStartTime_dtStartTimeDate"></span>
        </div>
        <div class="col-md-8">
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litEndTime" runat="server" Text="<%$Resources:RBVHStadaWebpages,MeetingRoomManagement_EndTimeTitle%>" />
                    <span title="<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,RequiredField%>' />" class="ms-accentText">*</span>
                </nobr>
            </span>
        </div>
        <div class="col-md-2">
            <span dir="none">
                <SharePoint:DateTimeControl ID="dtEndTime" runat="server" LocaleId="2057" HoursMode24="True" />
            </span>
            <span id="end-time-error" class="ms-formvalidation" style="margin-top: 0px;" target-id="ctl00_PlaceHolderMain_MeetingFormUserControl_dtEndTime_dtEndTimeDate"></span>
        </div>
        <div class="col-md-8">
        </div>
    </div>
    <div class="row ms-formlabel" style="display: none;" id="tr-approval-status">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litStatus" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Status%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-10">
            <span id="td-approval-status"></span>
        </div>
    </div>
    <div class="row ms-formlabel">
        <div class="col-md-2">
            <span class="ms-h3 ms-standardheader">
                <nobr>
                    <asp:Literal ID="litComments" runat="server" Text="<%$Resources:RBVHStadaWebpages,RecruimentForm_Comments%>" />
                </nobr>
            </span>
        </div>
        <div class="col-md-10">
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

<script type="text/javascript" src="/_layouts/15/RBVH.Stada.Intranet.Branding/scripts/MeetingRoomModule/MeetingRoomForm.js?v=ASHSHF9F-26F4-4832-9BE5-91D2F913C848"></script>

<script>
    $(document).ready(function () {
        var settings = {
            Controls:
            {
                ContainerSelector: ".meeting-room-container",
                RequiredFieldClassSelector: ".ms-formvalidation",
                DiscussionMeetingSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_txtDiscussionMeeting",
                DiscussionMeetingErrorSelector: "#discussion-meeting-error",
                ParticipationSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_txtParticipation",
                ParticipationErrorSelector: "#participation-error",
                LocationSelector: '#<%=ddlLocation.ClientID%>',
                LocationErrorSelector: "#location-error",
                EquipmentSelector: '#<%=cblEquipment.ClientID%>',
                SeatsSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_txtSeats",
                OthersSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_txtOthers",
                StartTimeSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_dtStartTime_dtStartTimeDate",
                StartTimeImageSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_dtStartTime_dtStartTimeDateDatePickerImage",
                StartTimeHourSelector: "#<%=dtStartTime.Controls[1].ClientID%>",
                StartTimeMinuteSelector: "#<%=dtStartTime.Controls[2].ClientID%>",
                StartTimeErrorSelector: "#start-time-error",
                EndTimeSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_dtEndTime_dtEndTimeDate",
                EndTimeImageSelector: "#ctl00_PlaceHolderMain_MeetingFormUserControl_dtEndTime_dtEndTimeDateDatePickerImage",
                EndTimeHourSelector: "#<%=dtEndTime.Controls[1].ClientID%>",
                EndTimeMinuteSelector: "#<%=dtEndTime.Controls[2].ClientID%>",
                EndTimeErrorSelector: "#end-time-error",
                ApprovalStatusValueSelector: '#<%=hdApprovalStatus.ClientID%>',
                ApprovalStatusTrSelector: '#tr-approval-status',
                ApprovalStatusTdSelector: '#td-approval-status'
            },
            ResourceText:
            {
                CantLeaveTheBlank: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,CantLeaveTheBlank%>' />",
                CantLessThanToday: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,SelectedDateMustGreaterThanCurrentDate%>' />",
                StartDateLessThanEndDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,StartDateLessThanEndDate%>' />",
                InvalidDate: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,InvalidDate%>' />",
                DataMustBeDifferentAfterEdit: "<asp:Literal runat='server' Text='<%$Resources:RBVHStadaWebpages,DataMustBeDifferentAfterEdit%>' />",
            }
        }

        MeetingRoomModule.Initialize(settings);
    });
</script>
