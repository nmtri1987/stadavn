var ViewType = {
    Day: "day",
    Week: "week",
    Month: 'month'
}

var ApprovalConfig = {
    PageResourceFileName: "RBVHStadaWebpages",
    ListResourceFileName: "RBVHStadaLists",
    Month: "Month",
    Week: "Week",
    Day: "Day"
}

$(document).ready(function () {
    $('.js-webpart-titleCell[title*="Calendar"]').each(function () {
        var parent = $(this).parent();
        var nextItem = $(this).parent().next();
        parent.append(generateHtmlCalendarType());
    })

    SP.SOD.executeOrDelayUntilScriptLoaded(function () {
        SP.SOD.registerSod(ApprovalConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + ApprovalConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
        SP.SOD.executeFunc(ApprovalConfig.ListResourceFileName, "Res", OnListResourcesReady);
    }, "strings.js");

});

function OnListResourcesReady() {
    var calendarTitle = Res.departmentCalendar_ListTitle;
    var calendarDiscription = Res.departmentCalendar_Description;
    var companyCalender1Title = Res.companyCalendar_ContentTypeTitle;
    var companyCalender2Title = Res.companyCalendar2_ContentTypeTitle;
    var parentSpan = $("span[title*='Calendar']");
    var isRun = false;
    for (var i = 0; i <= parentSpan.length; i++) {
        var element = parentSpan[i];
        if (element && element.innerText.toUpperCase().indexOf("COMPANY CALENDAR") !== -1) {
            //company calendar
            if (element.innerText.toUpperCase().indexOf("LOCATION 1") !== -1) {
                $(element).attr('title', companyCalender1Title);
                $(element).find('a span:first').text(companyCalender1Title);
            } else
                if (element.innerText.toUpperCase().indexOf("LOCATION 2") !== -1) {
                    $(element).attr('title', companyCalender2Title);
                    $(element).find('a span:first').text(companyCalender2Title);
                }
        }
        else {
            //department calendar
            $(element).attr('title', calendarTitle + " - " + calendarDiscription);
            $(element).find('a span:first').text(calendarTitle);
        }
    }
}

function LoadResource() {
    ApprovalConfig.Month = $("#calendarMonth").text();
    ApprovalConfig.Day = $("#calendarDay").text();
    ApprovalConfig.Week = $("#calendarWeek").text();
}

function generateHtmlCalendarType() {
    LoadResource();
    var html = '<span  class="ms-cui-layout" ><span  class="ms-cui-section" ><span  class="ms-cui-row-onerow"><a onclick="daymove(this)" class="ms-cui-ctl-large" ><span class="ms-cui-ctl-largeIconContainer" ><span class=" ms-cui-img-32by32 ms-cui-img-cont-float" ><img style="top: -239px; left: -239px;" src="/_layouts/15/1033/images/formatmap32x32.png?rev=40" alt="" ></span></span><span class="ms-cui-ctl-largelabel"  >' + ApprovalConfig.Day + '</span></a><a  class="ms-cui-ctl-large" onclick="weekmove(this)" ><span class="ms-cui-ctl-largeIconContainer" ><span class=" ms-cui-img-32by32 ms-cui-img-cont-float" ><img style="top: -1px; left: -477px;" src="/_layouts/15/1033/images/formatmap32x32.png?rev=40" alt="" ></span></span><span class="ms-cui-ctl-largelabel" >' + ApprovalConfig.Week + '</span></a><a   class="ms-cui-ctl-large"  onclick="monthmove(this)" ><span class="ms-cui-ctl-largeIconContainer" ><span class=" ms-cui-img-32by32 ms-cui-img-cont-float" ><img style="top: -1px; left: -273px;" src="/_layouts/15/1033/images/formatmap32x32.png?rev=40" alt="" ></span></span><span class="ms-cui-ctl-largelabel" >' + ApprovalConfig.Month + '</span></a></span></span></span>'
    return html;
}

function daymove(current) {
    var parent = $(current).closest('.ms-webpart-chrome-title');
    var nextItem = parent.next();
    var ctxid = nextItem.find('.ms-acal-rootdiv').attr('ctxid');
    _MoveToViewDate(null, ViewType.Day, ctxid);
}

function weekmove(current) {
    var parent = $(current).closest('.ms-webpart-chrome-title');
    var nextItem = parent.next();
    var ctxid = nextItem.find('.ms-acal-rootdiv').attr('ctxid');
    _MoveToViewDate(null, ViewType.Week, ctxid);
}

function monthmove(current) {
    var parent = $(current).closest('.ms-webpart-chrome-title');
    var nextItem = parent.next();
    var ctxid = nextItem.find('.ms-acal-rootdiv').attr('ctxid');
    _MoveToViewDate(null, ViewType.Month, ctxid);
}
