(function () {
    var itemCtxImageSlider = {};
    itemCtxImageSlider.Templates = {};
    itemCtxImageSlider.Templates.Header = Home_ImageSlider_RenderHeader;
    itemCtxImageSlider.OnPostRender = Home_ImageSlider_OnPostRender;
    itemCtxImageSlider.Templates.Item = Home_ImageSlider_ItemOverrideFun;
    itemCtxImageSlider.Templates.Footer = Home_ImageSlider_Footer;
    // itemCtxImageSlider.BaseViewID = 1;
    itemCtxImageSlider.ListTemplateType = 101;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(itemCtxImageSlider);
})();

function Home_ImageSlider_RenderHeader(ctx) {
    
    var headerHtml = "<div id='imageSliderHeaderHtml'>" + RenderHeaderTemplate(ctx, false) + "</div>";
    var imageSliderHtml = "<div class='imagesliderEditLink-custom'><a id='linkSlider' href='' class='cutom-link'></a></div> <div id='imagesilder' imagesilder='slidernew'> ";
    return headerHtml + imageSliderHtml;
}

function Home_ImageSlider_ItemOverrideFun(ctx) {
    var regex = /(<([^>]+)>)/ig;
    var addlist = '<img data-src="' + ctx.CurrentItem.FileRef.slice(0, ctx.CurrentItem.FileRef.lastIndexOf("/") + 1) + encodeURIComponent(ctx.CurrentItem.FileLeafRef) + '"  />';
    return addlist;
}

function Home_ImageSlider_OnPostRender(ctx) {
    $('#imageSliderHeaderHtml img').last().hide();
    $("#linkSlider").attr("href", ctx.listUrlDir);
    var slider = new IdealImageSlider.Slider("[imagesilder='slidernew']");
    slider.addBulletNav();
    slider.start();
    $("[imagesilder='slidernew']").css("border", "1px solid #ddd");
}

function Home_ImageSlider_Footer(ctx) {
    return "</div>";
}