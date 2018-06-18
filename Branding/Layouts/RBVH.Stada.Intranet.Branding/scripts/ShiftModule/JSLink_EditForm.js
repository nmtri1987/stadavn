(function () {
    var viewRequestApproveContext = {};
    viewRequestApproveContext.Templates = {};
    viewRequestApproveContext.Templates.Fields = {
        "Body": {
            "EditForm": Body_Basic_FieldTemplate
        },
        "Title": {
            "EditForm": Title_Basic_FieldTemplate
        },
        "DueDate": {
            "EditForm": DueDate_Basic_FieldTemplate
        },
        "PercentComplete": {
            "EditForm": PercentComplete_Basic_FieldTemplate
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(viewRequestApproveContext);
})();

function Body_Basic_FieldTemplate(ctx) {
    function getUrlParam(sParam) {
        var sPageURL = decodeURIComponent(window.location.search.substring(1)),
            sURLVariables = sPageURL.split('&'),
            sParameterName,
            i;

        for (i = 0; i < sURLVariables.length; i++) {
            sParameterName = sURLVariables[i].split('=');

            if (sParameterName[0] === sParam) {
                return sParameterName[1] === undefined ? true : sParameterName[1];
            }
        }
    };
    var taskId = getUrlParam("ID");
    if (taskId != undefined && taskId > 0) {

        var stringData = ctx.CurrentItem.Body;
        if (stringData != '') {
            stringData = stringData.replace("StadaTaskID", taskId);
        }
        return stringData;
    }
    else {
       return RenderItemTemplate(ctx);
    }
}
function Title_Basic_FieldTemplate(ctx) {
    return "<div>" + ctx.CurrentItem.Title + "</div>";
}
function DueDate_Basic_FieldTemplate(ctx) {
    return "<div>" + ctx.CurrentItem.DueDate + "</div>";
}
function PercentComplete_Basic_FieldTemplate(ctx) {
    return "<div>" + ctx.CurrentItem.PercentComplete + "</div>";
}
