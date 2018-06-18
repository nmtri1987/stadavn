(function () {
    var changeShiftViewContext = {};
    changeShiftViewContext.Templates = {};
    changeShiftViewContext.Templates.Fields = {
        "ApprovalStatus": {
            "View": ChangeShift_ApprovalStatusFieldTemplate
        }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(changeShiftViewContext);
})();

// This function provides the rendering logic for New and Edit forms
function ChangeShift_ApprovalStatusFieldTemplate(ctx) {
    switch (ctx.CurrentItem.ApprovalStatus) {
        case "Approved":
            return "<div> <b>" + ctx.CurrentItem.ApprovalStatus + "</b></div>";
            break;
        case "Rejected":
            return "<div style=' color: red;'> " + ctx.CurrentItem.ApprovalStatus + "</div>";
            break;
        default:
            return "<div>" + ctx.CurrentItem.ApprovalStatus + "</div>";
    }
}
