
//Insert links here  to display leftmenu
var displayUrls = [
    "Overview",
    "Leave",
    "Policies",
    "Shift",
    "Login",
    "Overtime"
];

var DisplayFullUrls = ["/_layouts/15/RBVH.Stada.Intranet.WebPages/Overview.aspx"];

$(document).ready(function () {

    var currentUrl = window.location.href;
    $("#ContentDiv .ms-core-listMenu-verticalBox").css("display", "none");
    $("#ctl00_PlaceHolderLeftNavBar_QuickLaunchNavigationManager").css("display", "none");
    $('#contentBox').css("margin-left", "0");
    if (currentUrl.indexOf("_layouts") < 0) {
        $('#contentRow').css("margin-left", "6%");
        $('#contentRow').css("margin-right", "6%");
    }
    else {
        $('#contentRow').css("margin-left", "10px");
    }
    HideLeftMenu(currentUrl);
});


function HideLeftMenu(url) {
    $.each(displayUrls, function (index, value) {
        if (url.indexOf(value) >= 0 && url.indexOf("_layouts") < 0) //display left menu
        {
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
            $('#contentBox').css("margin-left", "");
            $('#contentRow').css("margin-left", "");
            $('#contentRow').css("margin-right", "");
            $('.ms-core-sideNavBox-removeLeftMargin').css("border", "solid 1px #e6e0e0");
        }
    });
}