(function () {
    var internalNewsFormCtx = {};
    internalNewsFormCtx.Templates = {};
    internalNewsFormCtx.Templates.View = CustomInternalNewsForm;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(internalNewsFormCtx);
})();

function CustomInternalNewsForm(ctx) {
    var internalNews_DisplayForm = "<div class='container'>"
          + "<div class='row'><h3>{{InternalNews_NewsTitle}} </h3> </div>"
           + "<div class='row'><p><b>{{InternalNews_NewsShortContent}}</b></p></div>"
            + "<div class='row'><p>{{InternalNews_NewsBody}}</p></div>"
         + "</div>";
    internalNews_DisplayForm = internalNews_DisplayForm.replace("{{InternalNews_NewsTitle}}", InternalNews_GetSPFieldRender(ctx, "Title"));
    internalNews_DisplayForm = internalNews_DisplayForm.replace("{{InternalNews_NewsShortContent}}", InternalNews_GetSPFieldRender(ctx, "NewsShortContent"));
    internalNews_DisplayForm = internalNews_DisplayForm.replace("{{InternalNews_NewsBody}}", InternalNews_GetSPFieldRender(ctx, "NewsBody"));
    return internalNews_DisplayForm;
}

function InternalNews_GetSPFieldRender(ctx, fieldName) {
    var fieldContext = ctx;
    //Get the filed Schema 
    var result = ctx.ListSchema.Field.filter(function (obj) {
        return obj.Name == fieldName;
    });
    fieldContext.CurrentFieldSchema = result[0];
    fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName]; 
    return ctx.Templates.Fields[fieldName](fieldContext);
}

function InternalNewstOnPostRender() {
}




