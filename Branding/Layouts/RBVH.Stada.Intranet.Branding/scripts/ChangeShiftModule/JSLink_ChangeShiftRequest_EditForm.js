(function () {
    var changeShiftRequestNewFormContext = {};
    changeShiftRequestNewFormContext.Templates = {};
    changeShiftRequestNewFormContext.Templates.Fields = {
        "Requester": {
            "EditForm": ChangeShift_EditForm_Requester_FieldTemplate
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(changeShiftRequestNewFormContext);
})();

function ChangeShift_EditForm_Requester_FieldTemplate(ctx)
{
    return "";
}