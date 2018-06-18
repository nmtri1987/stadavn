(function () {
    var itemCtxEmpLeaveRequest = {};
    itemCtxEmpLeaveRequest.Templates = {};
 
    itemCtxEmpLeaveRequest.Templates.Item = LeaveModule_ItemOverride;

    itemCtxEmpLeaveRequest.BaseViewID = 1;
    itemCtxEmpLeaveRequest.ListTemplateType = 10004;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(itemCtxEmpLeaveRequest);
   
}
 
)();
var Info = null;
function aaa()
{
}
function LeaveModule_RenderHeader() {

    return '';
}
function LeaveModule_OnPostPreRender()
{
}

function LeaveModule_ItemOverride(ctx) {
  
    return RenderItemTemplate(ctx);
}

