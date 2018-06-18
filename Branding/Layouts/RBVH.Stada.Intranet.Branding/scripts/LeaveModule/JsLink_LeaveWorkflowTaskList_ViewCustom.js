(function () {
    var leaveTaskViewContext = {};
    leaveTaskViewContext.Templates = {};
    leaveTaskViewContext.Templates.OnPostRender = TaskEditForm_HiddenFiledOnPreRender;
    leaveTaskViewContext.Templates.Fields = {
        "Title": {
            "EditForm": TaskEditView_Title
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveTaskViewContext);
})();

function TaskEditView_Title(ctx) {
    
}