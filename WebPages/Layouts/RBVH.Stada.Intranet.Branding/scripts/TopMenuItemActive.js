function ShowHideAdminMenu() {
    var userIsInGroup = false;
    $.ajax({
        headers: { "accept": "application/json; odata=verbose" },
        method: "GET",
        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/currentuser/groups",
        success: function (data) {
            if (data && data.d && data.d.results) {
                data.d.results.forEach(function (value) {
                    if (value.Title.toUpperCase() == 'SYSTEM ADMIN') {
                        userIsInGroup = true;
                    }
                });
            }
            DoShowHide(userIsInGroup);
        },
        error: function (response) {
            DoShowHide(false);
            console.log(response.status);
        },
    });
}

function DoShowHide(userIsInGroup) {
    if (userIsInGroup == true) {
        $("#topmenu_Admin").show();
    }
    else {
        $("#topmenu_Admin").hide();
    }
}

$(document).ready(function () {
    ShowHideAdminMenu();
});