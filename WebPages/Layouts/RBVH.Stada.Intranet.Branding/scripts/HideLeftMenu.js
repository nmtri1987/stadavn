
//Insert links here  to display leftmenu
var displayUrls = [
    "Overview",
    "Leave",
    //"Policies",
    "/Shift",
    "Login",
    "/Overtime",
    "/LeaveManagement",
    "/ChangeShiftManagement",
    "/NotOverTimeManagement"
];

var DisplayFullUrls = ["/SitePages/Overview.aspx",
			"/_layouts/15/people.aspx",
            "/SitePages/OvertimeRequest.aspx",
            "/SitePages/OvertimeApprovalList.aspx",
            "/SitePages/OvertimeApproval.aspx",
            "/SitePages/ShiftRequest.aspx",
            "/SitePages/ShiftApprovalList.aspx",
            "/SitePages/ShiftApproval.aspx",
            "/SitePages/OvertimeManagement.aspx",
            "/SitePages/LeaveOfAbsenceManagement.aspx",
            "/SitePages/LeaveOfAbsenceManagementTaskList.aspx",
            "/SitePages/ChangeShiftManagement.aspx",
            "/SitePages/ChangeShiftManagementTaskList.aspx",
            "/SitePages/LeaveRequest.aspx",
            "/SitePages/FreightRequest.aspx",
            "/SitePages/BusinessTripRequest.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/MyShiftTime.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/MyOvertime.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/SecurityLeaveManagement.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagementApprovalTask.aspx",
            "/SitePages/TransportationManagement.aspx",
            "/SitePages/TransportationManagementTaskList.aspx",
            "/Lists/VehicleManagement/NewForm.aspx",
            "/Lists/VehicleManagement/EditForm.aspx",
            "/Lists/VehicleManagement/DispForm.aspx",
            "/Lists/NotOverTimeManagement/DispForm.aspx",
            "/Lists/NotOvertimeManagement/NewForm.aspx",
            "/Lists/ChangeShiftManagement/DispForm.aspx",
            "/Lists/ChangeShiftManagement/NewForm.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/OvertimeManagement/OvertimeManagementMember.aspx",

             "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceManagementMember.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementAdmin.aspx",
            "_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementMember.aspx",
            "_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ShiftManagement/ShiftManagementBOD.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementMember.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftManagementBOD.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/TransportationManagement/TransportationManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/TransportationManagement/TransportationManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/TransportationManagement/TransportationManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/TransportationManagement/TransportationManagementMember.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveManagementMember.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveManagement/LeaveHistory.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestList.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/RequestManagement/RequestForm.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/RecruitmentManagement/RecruitmentList.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/RecruitmentManagement/RecruitmentForm.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/FreightManagement/FreightManagementMember.aspx",

            "_layouts/15/RBVH.Stada.Intranet.WebPages/RequestForDiplomaSupplyManagement/RequestForDiplomaSupplyForm.aspx",
            "_layouts/15/RBVH.Stada.Intranet.WebPages/RequestForDiplomaSupplyManagement/RequestForDiplomaSupplyList.aspx",

            "_layouts/15/RBVH.Stada.Intranet.WebPages/MeetingRoomManagement/MeetingRoomForm.aspx",
            "_layouts/15/RBVH.Stada.Intranet.WebPages/MeetingRoomManagement/MeetingRoomList.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementAdmin.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementBOD.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementManager.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/BusinessTripManagement/BusinessTripManagementMember.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationList.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/DelegationManagement/DelegationForm.aspx",

            "/_layouts/15/RBVH.Stada.Intranet.WebPages/LeaveOfAbsenceManagement/LeaveOfAbsenceApprovalDelegation.aspx",
            "/_layouts/15/RBVH.Stada.Intranet.WebPages/ChangeShiftManagement/ChangeShiftApprovalDelegation.aspx",

            "_layouts/15/RBVH.Stada.Intranet.WebPages/RequirementForGuestReceptionManagement/GuestReceptionForm.aspx",
            "_layouts/15/RBVH.Stada.Intranet.WebPages/RequirementForGuestReceptionManagement/GuestReceptionList.aspx",
];

var DisplayGroup = ["/_layouts/15/people.aspx",


];

$(document).ready(function () {
    var currentUrl = window.location.href;
    // Check Site Admin -> Hide left menu
    if (_spPageContextInfo && (_spPageContextInfo.userId === 1073741823 || _spPageContextInfo.isSiteAdmin))
    {
        $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "none");
        if (currentUrl.indexOf("_layouts") < 0) {
            $('#contentRow').css("margin-left", "2%");
            $('#contentRow').css("margin-right", "2%");
        }
        else {
            $('#contentRow').css("margin-left", "10px");
        }

        $('#contentBox').css("margin-left", "0%");

        ShowPolicyMenu(currentUrl);

        return;
    }
    
    $("#ContentDiv .ms-core-listMenu-verticalBox").css("display", "none");
    $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "none");
    //$('.stada-leftmenu').css('border',"none");
    $('#contentBox').css("margin-left", "0");
    if (currentUrl.indexOf("_layouts") < 0) {
        $('#contentRow').css("margin-left", "2%");
        $('#contentRow').css("margin-right", "2%");
    }
    else {
        $('#contentRow').css("margin-left", "10px");
    }
    HideLeftMenu(currentUrl);
    ShowPolicyMenu(currentUrl);
});

function HideLeftMenu(url) {

    $.each(displayUrls, function (index, value) {
        if (url.indexOf(value) >= 0 && url.indexOf("_layouts") < 0) //display left menu
        {
            $("#ContentDiv .ms-core-listMenu-verticalBox").css("display", "block");
            $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "block");
            $('#contentBox').css("margin-left", "");
            $('#contentRow').css("margin-left", "");
            $('#contentRow').css("margin-right", "");
            $('.ms-core-sideNavBox-removeLeftMargin').css("border", "solid 1px #e6e0e0");
        }
    });

    $.each(DisplayFullUrls, function (index, value) {
        if (url.toLowerCase().indexOf(value.toLowerCase()) >= 0) //display left menu
        {
            $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "block");
            $("#ContentDiv .ms-core-listMenu-verticalBox").css("display", "block");
            $('#contentBox').css("margin-left", "");
            $('#contentRow').css("margin-left", "");
            $('#contentRow').css("margin-right", "");
            //$('.ms-core-sideNavBox-removeLeftMargin').css("border", "solid 1px #e6e0e0");
            $('.ms-core-sideNavBox-removeLeftMargin').css("border", "none");
        }
    });
    $.each(DisplayGroup, function (index, value) {
        if (url.toLowerCase().indexOf(value.toLowerCase()) >= 0) //display left menu
        {
            $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "none");
            $("#ContentDiv .ms-core-listMenu-verticalBox").css("display", "block");
            $('#contentBox').css("margin-left", "");
            $('#contentRow').css("margin-left", "");
            $('#contentRow').css("margin-right", "");
            $('.ms-core-sideNavBox-removeLeftMargin').css("border", "solid 1px #e6e0e0");
        }
    });
}

function ShowPolicyMenu(url)
{
    $('.default-leftmenu').hide();
    $(".ms-core-listMenu-verticalBox", ".default-leftmenu").hide();
    url = url.toLowerCase();
    if (url.indexOf('/policies/pages/forms/') < 0 && (url.indexOf('/policies/pages/') >= 0 || url.indexOf('/policies/sitepages/') >= 0)) //display left menu
    {
        //$("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").show();
        //$("#ContentDiv .ms-core-listMenu-verticalBox").show();
        //$('.stada-leftmenu').hide();
        //$("#ContentDiv .ms-core-listMenu-verticalBox", ".stada-leftmenu").hide();
        $('.default-leftmenu').show();
        $(".ms-core-listMenu-verticalBox", ".default-leftmenu").show();

        // Restyle CSS:
        $('#contentRow').css('margin', '0%');
    }
    // Add margin left for Home page:
    if (url.indexOf('/sitepages/home.aspx') > 0)
    {
        $('#contentRow').css('margin-left', '2%');
    }
}