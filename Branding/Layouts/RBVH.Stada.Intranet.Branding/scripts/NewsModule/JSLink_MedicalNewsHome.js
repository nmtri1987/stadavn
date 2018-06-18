(function () {
    var newsPageCtx = {};
    newsPageCtx.Templates = {};
    newsPageCtx.Templates.Header = NewsPage_RenderHeader;
    newsPageCtx.OnPostRender = NewsPage_OnPostRender;
    newsPageCtx.Templates.Item = NewsPage_ItemOverride;
    newsPageCtx.Templates.Footer = NewsPage_Footer;
    newsPageCtx.ListTemplateType = 12083;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(newsPageCtx);
})();

function NewsPage_RenderHeader(ctx) {

    //    var defaultHtml = RenderHeroAddNewLink("idHomePageNewItem",ctx);
    var headerHtml = '<ul class="list-group">';
    return headerHtml;
}

function NewsPage_ItemOverride(ctx) {
    var defaultImageUrl = _spPageContextInfo.webAbsoluteUrl + "/_layouts/15/rbvh.stada.intranet.branding/images/STADA_image.png";
    var itemUrl = String.format('{0}&amp;ID={1}', ctx.displayFormUrl, ctx.CurrentItem.ID) + '&Source=/SitePages/News.aspx';
    var newsTitle = ctx.CurrentItem['Title'];
    var newsImage = ctx.CurrentItem["PublishingPageImage"];
    var newsImageUrl = $(newsImage).find("img").attr("src");

    if (newsImageUrl == '' || newsImageUrl == undefined) {
        newsImageUrl = defaultImageUrl;
    }
    var createdDate = ctx.CurrentItem.Created;
    var newsShortContent = ctx.CurrentItem["NewsShortContent"];
    if (newsShortContent == '' || newsShortContent == undefined) {
        newsShortContent = SplitWordsFromBodyNewsPage(ctx.CurrentItem["NewsBody"])
    }
    else {
        newsShortContent += '...';
    }
    var stringEmpty = '';
    var html = stringEmpty.concat('<li class="list-group-item list-group-item-custom" style="border:none" >',
    '<div class="row">',
          '<div class="col-md-4"><img src="', newsImageUrl, '" class="img-thumbnail img-thumbnail-custom"></div>',
          '<div class="col-md-8">',
           ' <div class="row">',
             '<b style="font-size: 1.2em"><a href="', itemUrl, '">', newsTitle, '</a><span class="span_createdate_news">&nbsp;(', createdDate, ')</span></b>',
            '</div>',
            '<div class="row">',
             newsShortContent,
            '</div>',
         ' </div>',
        '</div>',
     ' </li>');
    return html;
}

function NewsPage_Footer(ctx) {
    return "</ul><div>" + RenderFooterTemplate(ctx) + "</div>";
}

function RemoveHtml(text) {
    var r = /(<([^>]+)>)/ig;
    var content = (text.replace(r, ""));
    return content;
}

function NewsPage_OnPostRender() {
}


function SplitWordsFromBodyNewsPage(text) {
    var r = /(<([^>]+)>)/ig;
    var content = (text.replace(r, ""));
    if (content.length > 255) {
        content = content.substring(0, 254);
    }
    var returnString = '';
    //Get 10 first of string
    var words = content.split(" ");
    if (words.length > 20) {
        for (var i = 0; i < 20; i++) {
            returnString += (words[i] + ' ');
        }
    }
    else {
        returnString = content;
    }

    return returnString + '...';
}