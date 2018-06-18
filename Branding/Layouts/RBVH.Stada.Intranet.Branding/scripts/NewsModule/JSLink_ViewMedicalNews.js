(function () {
    var medicalNewsFormCtx = {};
    medicalNewsFormCtx.Templates = {};
    medicalNewsFormCtx.Templates.View = CustomInternalNewsForm;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(medicalNewsFormCtx);
})();

function CustomInternalNewsForm(ctx) {
    var MedicalNews_DisplayForm = "<div class='container'>"
          + "<div class='row'><h3>{{MedicalNews_NewsTitle}} </h3> </div>"
           + "<div class='row'><p><b>{{MedicalNews_NewsShortContent}}</b></p></div>"
            + "<div class='row'><p>{{MedicalNews_NewsBody}}</p></div>"
         + "</div>";
    MedicalNews_DisplayForm = MedicalNews_DisplayForm.replace("{{MedicalNews_NewsTitle}}", MedicalNews_GetSPFieldRender(ctx, "Title"));
    MedicalNews_DisplayForm = MedicalNews_DisplayForm.replace("{{MedicalNews_NewsShortContent}}", MedicalNews_GetSPFieldRender(ctx, "NewsShortContent"));
    MedicalNews_DisplayForm = MedicalNews_DisplayForm.replace("{{MedicalNews_NewsBody}}", MedicalNews_GetSPFieldRender(ctx, "NewsBody"));
    return MedicalNews_DisplayForm;
}

function MedicalNews_GetSPFieldRender(ctx, fieldName) {
    var fieldContext = ctx
    var result = ctx.ListSchema.Field.filter(function (obj) {
        return obj.Name == fieldName;
    });
    fieldContext.CurrentFieldSchema = result[0];
    fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName];
    return ctx.Templates.Fields[fieldName](fieldContext);
}





