(function () {
    var announcementFormCtx = {};
    announcementFormCtx.Templates = {};
    announcementFormCtx.Templates.View = CustomInternalNewsForm;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(announcementFormCtx);
})();

function CustomInternalNewsForm(ctx) {
    var announcement_DisplayForm = "<div class='container'>"
          + "<div class='row'><h3>{{Announcement_NewsTitle}} </h3> </div>"
           + "<div class='row'><p><b>{{Announcement_NewsShortContent}}</b></p></div>"
            + "<div class='row'><p>{{Announcement_NewsBody}}</p></div>"
         + "</div>";
    announcement_DisplayForm = announcement_DisplayForm.replace("{{Announcement_NewsTitle}}", Announcement_GetSPFieldRender(ctx, "Title"));
    announcement_DisplayForm = announcement_DisplayForm.replace("{{Announcement_NewsShortContent}}", Announcement_GetSPFieldRender(ctx, "NewsShortContent"));
    announcement_DisplayForm = announcement_DisplayForm.replace("{{Announcement_NewsBody}}", Announcement_GetSPFieldRender(ctx, "Body"));
    return announcement_DisplayForm;
}

function Announcement_GetSPFieldRender(ctx, fieldName) {
    var fieldContext = ctx;
    var result = ctx.ListSchema.Field.filter(function (obj) {
        return obj.Name == fieldName;
    });
    fieldContext.CurrentFieldSchema = result[0];
    fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName];
    return ctx.Templates.Fields[fieldName](fieldContext);
}





