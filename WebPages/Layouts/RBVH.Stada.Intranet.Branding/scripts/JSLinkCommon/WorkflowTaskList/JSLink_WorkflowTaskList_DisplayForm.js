(function () {
    var workflowTaskListDisplayFormContext = {};
    workflowTaskListDisplayFormContext.Templates = {};
    workflowTaskListDisplayFormContext.Templates.OnPostRender = WorkflowTaskDisplayForm_HiddenFiledOnPreRender;
    workflowTaskListDisplayFormContext.Templates.Fields = {
        "StartDate": {
            "DisplayForm": WorkflowTaskDisplayForm_HiddenFiledTemplate
        },
        "Predecessors":
        {
            "DisplayForm": WorkflowTaskDisplayForm_HiddenFiledTemplate,
        },
        "Body":
         {
             "DisplayForm": WorkflowTaskDisplayForm_HiddenFiledTemplate,
         },
        "Priority":
        {
            "DisplayForm": WorkflowTaskDisplayForm_HiddenFiledTemplate,
        },
        // "Status":
        // {
        //     "DisplayForm": WorkflowTaskDisplayForm_HiddenFiledTemplate_ReturnLabel,
        // }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(workflowTaskListDisplayFormContext);
})();

function WorkflowTaskDisplayForm_HiddenFiledTemplate_ReturnLabel(ctx) {
    return "<div>" + ctx.CurrentItem.Status + "</div>";
}

function WorkflowTaskDisplayForm_HiddenFiledTemplate() {
    return "<span class='csrHiddenField'></span>";
}
// This function provides the rendering logic
function WorkflowTaskDisplayForm_HiddenFiledOnPreRender(ctx) {
    jQuery(".csrHiddenField").closest("tr").hide();
}


