$(document).ready(function () {
    SP.SOD.executeOrDelayUntilScriptLoaded(CheckUserPermissions, "SP.js");
});

var context;
var web;

function CheckUserPermissions() {
    context = new SP.ClientContext.get_current();
    web = context.get_web();
    context.load(web, 'EffectiveBasePermissions');
    context.executeQueryAsync(onSuccess, onFailure);
}

function onSuccess() {
    if (web.get_effectiveBasePermissions().has(SP.PermissionKind.manageWeb)) {
        //is site admin
        $("#topmenu_Admin").show();
    }
    else {
        $("#topmenu_Admin").hide();
    }
}
function onFailure(sender, args) {
    console.log('Request failed  - TopMenuItemActive.js file' + args.get_message() + 'n' + args.get_stackTrace());
}
