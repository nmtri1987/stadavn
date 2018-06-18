(function () {
    var workflowTaskListEditFormContext = {};
    workflowTaskListEditFormContext.Templates = {};
    workflowTaskListEditFormContext.Templates.OnPostRender = WorkflowTaskListEditForm_HiddenFiledOnPreRender;
    workflowTaskListEditFormContext.Templates.Fields = {
        "StartDate": {
            "EditForm": WorkflowTaskListEditForm_HiddenFiledTemplate
        },
        "Predecessors":
        {
            "EditForm": WorkflowTaskListEditForm_HiddenFiledTemplate,
        },
        "Body":
         {
             "EditForm": WorkflowTaskListEditForm_HiddenFiledTemplate,
         },
        "Priority":
        {
            "EditForm": WorkflowTaskListEditForm_HiddenFiledTemplate,
        },
        // "Status":
        //  {
        //      "EditForm": WorkflowTaskListEditForm_ReturnLalel,
        //  },
        "DueDate":
        {
            "EditForm": WorkflowTaskListEditForm_ReturnLalel
        }
        , "AssignedTo":
        {
            "EditForm": WorkflowTaskListEditForm_ReturnLalel_AssignTo
        }
        , "Title":
        {
            "EditForm": WorkflowTaskListEditForm_ReturnLalel
        }
        //  , "PercentComplete":
        //  {
        //     "EditForm": WorkflowTaskListEditForm_ReturnLalel
        //  }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(workflowTaskListEditFormContext);
})();

function WorkflowTaskListEditForm_HiddenFiledTemplate() {
    return "<span class='csrHiddenField'></span>";
}

function WorkflowTaskListEditForm_HiddenFiledOnPreRender(ctx) {
    jQuery(".csrHiddenField").closest("tr").hide();
}
function WorkflowTaskListEditForm_ReturnLalel_AssignTo(ctx) {
    var text = ctx.CurrentItem.AssignedTo[0].DisplayText;
    return "<div>" + text + "</div>";
}
function WorkflowTaskListEditForm_ReturnLalel(ctx) {
    var text = ctx.CurrentItem[ctx.CurrentFieldSchema.Name];
    return "<div>" + text + "</div>";
}