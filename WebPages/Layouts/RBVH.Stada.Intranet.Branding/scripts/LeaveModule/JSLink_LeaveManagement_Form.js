var LeaveManagementForm =
    {
        IsTextOnly: false
    };
(function () {
    ExecuteOrDelayUntilScriptLoaded(function () {
        if (LeaveManagementForm.IsTextOnly) {
            $('#s4-ribbonrow').hide();
            $(".ms-formtoolbar").hide();
        }
    }, 'sp.ribbon.js');
    var leaveManagementFormContext = {};
    leaveManagementFormContext.Templates = {};
    leaveManagementFormContext.Templates.Fields = {
        "Requester": {
            "DisplayForm": showLabel,
        },
        "RequestFor": {
            "DisplayForm": showLabel,
        },
        "TransferworkTo": {
            "DisplayForm": showLabel,
        },
        "CommonDepartment": {
            "DisplayForm": showLabel,
        },
        "CommonLocation": {
            "DisplayForm": showLabel,
        },
    };
    leaveManagementFormContext.OnPostRender = leaveManagementFormOnPostRender;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveManagementFormContext);
})();
function leaveManagementFormOnPostRender(ctx) {
    var paramTextOnly = Functions.getParameterByName('TextOnly');
    if (paramTextOnly && paramTextOnly == 'true') {
        LeaveManagementForm.IsTextOnly = true;
    }
}
function showLabel(ctx) {
    var currentItem = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    var paramTextOnly = Functions.getParameterByName('TextOnly');
    if (paramTextOnly && paramTextOnly == 'true' && currentItem) {
        var itemArray = currentItem.split("#");
        if (itemArray && itemArray.length > 1) {
            return itemArray[1];
        }
        else {
            return "";
        }
    }
    var fieldType = ctx.CurrentFieldSchema.FieldType;
    var defaultTemplates = SPClientTemplates._defaultTemplates.Fields.default.all.all;
    return defaultTemplates[fieldType]["DisplayForm"](ctx);
}
