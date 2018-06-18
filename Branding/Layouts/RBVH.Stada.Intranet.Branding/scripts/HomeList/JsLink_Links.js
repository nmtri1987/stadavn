(function () {
    var homeLinkCtx = {};
    homeLinkCtx.Templates = {};
    homeLinkCtx.Templates.Header = link_RenderHeader;
    homeLinkCtx.Templates.Item = link_ItemOverride;
    homeLinkCtx.Templates.Footer = link_Footer;
    homeLinkCtx.ListTemplateType = 103;
    homeLinkCtx.BaseViewId = 1;
    // ReSharper disable UseOfImplicitGlobalInFunctionScope
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(homeLinkCtx);
    // ReSharper restore UseOfImplicitGlobalInFunctionScope
})();

function link_RenderHeader() {
    var headerHtml = "<ul class='list-group'>";
    return headerHtml;
}

function link_ItemOverride(ctx) {
    var linkUrl = ctx.CurrentItem["URL"];
    var linkText = ctx.CurrentItem["URL.desc"];
    return "<li class='list-group-item' style='border: none; padding:3px' ><h5><span class='glyphicon glyphicon-link' aria-hidden='true'></span>&nbsp<a href='" + linkUrl + "' > " + linkText + "</a></h5></li>";
}

function link_Footer(ctx) {
    // ReSharper disable once UseOfImplicitGlobalInFunctionScope
    return "</ul><div>" + RenderFooterTemplate(ctx) + "</div>";
}

