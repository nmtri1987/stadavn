(function () {
    var leaveTaskDisplayFormContext = {};
    leaveTaskDisplayFormContext.Templates = {};
    leaveTaskDisplayFormContext.Templates.OnPostRender = TaskDisplayForm_HiddenFiledOnPreRender;
    leaveTaskDisplayFormContext.Templates.Fields = {
        // Apply the new rendering for Age field on New and Edit forms
        "StartDate": {
            "DisplayForm": TaskDisplayForm_HiddenFiledTemplate
        },
        "Predecessors":
        {
            "DisplayForm": TaskDisplayForm_HiddenFiledTemplate,
        },
        "Body":
         {
             "DisplayForm": TaskDisplayForm_HiddenFiledTemplate,
         },
        "Priority":
        {
            "DisplayForm": TaskDisplayForm_HiddenFiledTemplate,
        },
        // "Status":
        // {
        //     "DisplayForm": TaskDisplayForm_HiddenFiledTemplate_ReturnLabel,
        // }
    };

    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveTaskDisplayFormContext);
})();

function TaskDisplayForm_HiddenFiledTemplate_ReturnLabel(ctx) {
    return "<div>" + ctx.CurrentItem.Status + "</div>";
}

function TaskDisplayForm_HiddenFiledTemplate() {
    return "<span class='csrHiddenField'></span>";
}
// This function provides the rendering logic
function TaskDisplayForm_HiddenFiledOnPreRender(ctx) {
    jQuery(".csrHiddenField").closest("tr").hide();
    //redirect to edit form.
    var currentUrl = document.location.toString();
    var newUrl = currentUrl.replace("DispForm", "EditForm");
    document.location.replace(newUrl);
}


