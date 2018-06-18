﻿(function () {
    var internalNewsCtx = {};
    internalNewsCtx.Templates = {};
    internalNewsCtx.Templates.Header = InternalNews_RenderHeader;
    internalNewsCtx.OnPostRender = InternalNews_OnPostRender;
    internalNewsCtx.Templates.Item = InternalNews_ItemOverride;
    internalNewsCtx.Templates.Footer = InternalNews_Footer;
    internalNewsCtx.ListTemplateType = 12082;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(internalNewsCtx);
})();

function InternalNews_RenderHeader(ctx) {
    //    var defaultHtml = RenderHeroAddNewLink("idHomePageNewItem",ctx);
    var headerHtml = '<ul class="list-group">';
    return headerHtml;
}

function InternalNews_ItemOverride(ctx) {
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
        newsShortContent = SplitWordsFromBodyInternalNews(ctx.CurrentItem["NewsBody"])
    }
    else {
        newsShortContent += '...';
    }
    var stringEmpty = '';

    var htmlItem = RenderItemTemplate(ctx);
    var iconNewCss = '';
    var iconNew = '<span class="ms-newdocument-iconouter"><img class="ms-newdocument-icon" src="/_layouts/15/images/spcommon.png?rev=40" alt="new" title="new"></span>';
    if (htmlItem.indexOf("ms-newdocument-icon") >= 0) {
        iconNewCss = iconNew;
    }
    else {
        iconNewCss = "";
    }

    var html = stringEmpty.concat('<li class="list-group-item list-group-item-custom" style="border:none" >',
    '<div class="row">',
          '<div class="col-md-2"><img src="', newsImageUrl, '" class="img-thumbnail img-thumbnail-custom"></div>',
          '<div class="col-md-10">',
           ' <div class="row">',
             '<b style="font-size: 1.2em"><a href="', itemUrl, '">', newsTitle, '</a>', iconNewCss, '<span class="span_createdate_news">&nbsp;(', createdDate, ')</span></b>',
            '</div>',
            '<div class="row">',
             newsShortContent,
            '</div>',
         ' </div>',
        '</div>',
     ' </li>');
    return html;
}

function InternalNews_Footer(ctx) {
    return "</ul><div>" + RenderFooterTemplate(ctx) + "</div>";
}

function RemoveHtml(text) {
    var r = /(<([^>]+)>)/ig;
    var content = (text.replace(r, ""));
    return content;
}

function InternalNews_OnPostRender() {
}


function SplitWordsFromBodyInternalNews(text) {
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