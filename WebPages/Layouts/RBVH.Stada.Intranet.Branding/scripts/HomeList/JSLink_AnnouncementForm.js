(function () {
    var announcementFormTemplate = {};
    announcementFormTemplate.Templates = {};
    announcementFormTemplate.OnPostRender = newsFormOnPostRender;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(announcementFormTemplate);

})();

function newsFormOnPostRender(ctx) {
    var $bodyElement = $("[aria-labelledby^='Body_']");
    if ($bodyElement) {
        var $tableParents = $bodyElement.parents("table");
        if ($tableParents && $tableParents.length > 0) {
            var $tableContainer = $tableParents[1];
            if ($tableContainer) {
                $($tableContainer).attr("width", "85%");
                var $parentTableContainer = $($tableContainer).parents("table");
                if ($parentTableContainer && $parentTableContainer.length > 0) {
                    $parentTableContainer.attr("width", "100%");
                }
            }
        }
    }
    $("select[id^='AnnouncementPriority_']").attr("style", "width:15%");
    $("input[id^='NewsShortContent_']").attr("maxlength", 254);
}