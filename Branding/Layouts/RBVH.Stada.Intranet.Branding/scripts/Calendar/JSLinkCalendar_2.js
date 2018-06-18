(function () {
 
    var itemTwoCtxCalendar = {};
    itemTwoCtxCalendar.Templates = {};
    itemTwoCtxCalendar.Templates.Header = '<div id="divDatePickerTwo">';
    itemTwoCtxCalendar.OnPostRender = CalendarTwo_OnPostRender;
    itemTwoCtxCalendar.Templates.Item = CalendarTwo_OverrideFun;
    itemTwoCtxCalendar.Templates.Footer = "</div>";

    itemTwoCtxCalendar.BaseViewID = 1;
    itemTwoCtxCalendar.ListTemplateType = 1060;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(itemTwoCtxCalendar);
})();

function CalendarTwo_OverrideFun(ctxTwo) {
    return "";
}

function CalendarTwo_OnPostRender(ctxTwo) {

    //calendar 2

    console.log("11111");
    
    JSRequest.EnsureSetup();

    var today = new Date();
    var currDate = new Date();
    var liHtml;
    var calliHtml;
    var itemURL;
    var eventDuration;
    var date;
    var endDate;
    var oneDay = 24 * 60 * 60 * 1000; // hours*minutes*seconds*milliseconds
    var category = "";
    var recurrenceData = "";
    var currentDate = null;
    var currWeekDate = null;
    var eventDateExisted = [];
    var datetext;
    var calendarDate;
    var siteRelUrl = ctxTwo.listUrlDir;
    var currentTime;
    var hours;
    var minutes;
    var lastday;
    var isNextMnth;
    var camlFields;
    var camlQuery;
    var camlOptions;
    var formattedTime;
    var i;
    var calendarListItems2;
    var calendarListItems1 = [];
    var calendarListItems = [];
    var calendarListItemsExpandedTwo = [];
    var Weekdays = ['su', 'mo', 'tu', 'we', 'th', 'fr', 'sa'];

    var deletedKeys = [];

    (function ($) {
        var _updateDatepicker_o = $.datepicker._updateDatepicker;
        $.datepicker._updateDatepicker = function (inst) {
            _updateDatepicker_o.apply(this, [inst]);
            processResult2();
        };

        var calendarListItems2 = ctxTwo.ListData.Row;

        $("#divDatePickerTwo").datepicker();

        function processResult2() {


            //Get customize or deleted Repeat events
            $(calendarListItems2).each(function (index, item) {
                if (item.RecurrenceID != "") {
                    var dateDeleted = new Date(item.RecurrenceID);
                    dateDeleted.setMinutes(dateDeleted.getMinutes() + new Date().getTimezoneOffset());  //Reset Zone time to Default
                    deletedKeys.push(BuildRepeatItemId(item.UID, dateDeleted));

                    if (item.Title.indexOf('Deleted:') == -1 && item.Title.indexOf('Đã xóa:') == -1) {
                        //Customize
                        calendarListItemsExpandedTwo.push(item);
                    }
                }
            });

            //Expend all events
            $(calendarListItems2).each(function (index, item) {
                try {
                    if (item.RecurrenceID == "") {
                        recurrenceData = item.RecurrenceData;
                        if (recurrenceData != "") {

                            //Parse to DOM
                            recurrenceData = recurrenceData.replace(new RegExp('&lt;', 'g'), '<')
                                .replace(new RegExp('&gt;', 'g'), '>')
                                .replace(new RegExp('&quot;', 'g'), "'");
                            recurrenceXmlNode = $.parseXML(recurrenceData);

                            var dailyData = recurrenceXmlNode.getElementsByTagName('daily');
                            var weeklyData = recurrenceXmlNode.getElementsByTagName('weekly');
                            var monthlyData = recurrenceXmlNode.getElementsByTagName('monthly');

                            var repeatForever = recurrenceXmlNode.getElementsByTagName('repeatForever');
                            var repeatInstances = recurrenceXmlNode.getElementsByTagName('repeatInstances');
                            var windowEnd = recurrenceXmlNode.getElementsByTagName('windowEnd');

                            //Daily
                            if (dailyData.length > 0) {
                                CreateDailyEventTwo(item, dailyData, repeatForever, repeatInstances, windowEnd);
                            }
                                //Weekly
                            else if (weeklyData.length > 0) {
                                CreateWeeklyEventTwo(item, weeklyData, repeatForever, repeatInstances, windowEnd);
                            }
                                //Monthly
                            else if (monthlyData.length > 0) {
                                CreateMonthlyEventTwo(item, monthlyData, repeatForever, repeatInstances, windowEnd);
                            }
                            else {
                                //Exception: Something went wrong!!!
                            }
                        }
                        else {
                            calendarListItemsExpandedTwo.push(item);
                        }
                    }
                    else {
                        //Skip deleted / customize event
                    }
                } catch (e) {

                }
            });

            //Render events

            $(calendarListItemsExpandedTwo).each(function (index, item) {
                try {
                    date = new Date(item.EventDate);
                    endDate = new Date(item.EndDate);
                    var days = Math.floor(Math.abs((endDate.getTime() - date.getTime()) / (oneDay)));
                    category = item.Category;
                    console.log("1111" + category);
                    console.log(item);
                    itemURL = siteRelUrl + "/DispForm.aspx?ID=" + item.ID;

                    //Check Event Create
                    if (item["fAllDayEvent.value"] === "1") {
                        eventDuration = "(All day event)";
                    }
                    else {
                        if (new Date(item.EventDate).getDate() == new Date(item.EndDate).getDate()) {
                            //show only time
                            eventDuration = '(' + moment(item.EventDate).format('LT') + ' - ' + moment(item.EndDate).format('LT') + ')';
                        }
                        else {
                            //show date + time
                            eventDuration = '(' + moment(item.EventDate).format('DD/MM/YYYY HH:mm') + ' - ' + moment(item.EndDate).format('DD/MM/YYYY HH:mm') + ')';
                        }
                    }
                    for (var i = 0; i <= days; i++) {
                        //Check and create popup
                        if ($('#divDatePickerTwo .ui-datepicker-calendar a').first().parent('td').attr("data-year") == date.getFullYear() && $('#divDatePickerTwo .ui-datepicker-calendar a').first().parent('td').attr("data-month") == date.getMonth()) {

                            //check date to Existed
                            dateToCheckText = moment(date).format("DD/MM");
                            dateIsExisted = CheckExistedDateTwo(dateToCheckText);

                            if (!dateIsExisted) {
                                //Paint Calendar Item
                                PaintEventTwo(date, category);

                                //Create new Popup
                                CreatePopUp(date, eventDuration, itemURL, item.Title);

                                //Push to Date Existed
                                eventDateExisted.push(dateToCheckText);
                            }
                            else {
                                //Paint Calendar Item Multi
                                PaintEventTwo(date, "Multi");

                                //Append to Popup content
                                calliHtml = '<li class="divCalendarLI2"><a class="anchCalLi"' + " onclick='window.open(this.href)'" + 'href="' + itemURL + '&Source=' + encodeURIComponent(window.location.href) + '">' + item.Title + '</a> ' + eventDuration + '</li>';

                                $("#" + date.getDate() + "_eventPopUp2" + " .divCalendarUL2").append(calliHtml);
                            }
                        }

                        date.setDate(date.getDate() + 1)
                    }
                } catch (e) {

                }
            });

            eventDateExisted = [];
            calendarListItemsExpandedTwo = [];
        }

        function CreateDailyEventTwo(item, dailyData, repeatForever, repeatInstances, windowEnd) {
            try {
                var dayFrequencyAttribute = dailyData[0].attributes['dayFrequency'];
                if (dayFrequencyAttribute != null) {
                    var dayFrequency = parseInt(dayFrequencyAttribute.value);

                    var RepeatEventDate = new Date(item.EventDate);
                    var RepeatEndDate = new Date(item.EndDate);

                    var eventTimes = 0;

                    //Repeat Forever
                    if (repeatForever.length > 0) {
                        eventTimes = 500;
                    }
                        //Repeat Instances
                    else if (repeatInstances.length > 0) {
                        eventTimes = parseInt(repeatInstances[0].innerHTML);
                    }
                        //Repeat End Date
                    else if (windowEnd.length > 0) {
                        var dailyDays = Math.floor(Math.abs((RepeatEndDate.getTime() - RepeatEventDate.getTime()) / (oneDay))) + 1;
                        eventTimes = Math.ceil(Math.abs(dailyDays / dayFrequency));
                    }
                    else {
                        //Exception: Something went wrong!!!
                    }

                    for (var i = 0; i < eventTimes; i++) {
                        //Check Event was deleted?
                        var key = BuildRepeatItemId(item.UID, RepeatEventDate);
                        if (deletedKeys.indexOf(key) == -1) {
                            calendarListItemsExpandedTwo.push({
                                ID: BuildRepeatItemId(item.ID, RepeatEventDate),
                                Title: item.Title,
                                EventDate: new Date(RepeatEventDate),
                                EndDate: new Date(RepeatEventDate.getFullYear(), RepeatEventDate.getMonth(), RepeatEventDate.getDate(), RepeatEndDate.getHours(), RepeatEndDate.getMinutes(), 0, 0),
                                Category: item.Category,
                                fAllDayEvent: item.fAllDayEvent,
                            });
                        }

                        RepeatEventDate.setDate(RepeatEventDate.getDate() + dayFrequency);
                    }
                }
            } catch (e) {

            }
        }
        function CreateWeeklyEventTwo(item, weeklyData, repeatForever, repeatInstances, windowEnd) {
            try {
                var weekFrequencyAttribute = weeklyData[0].attributes['weekFrequency'];
                if (weekFrequencyAttribute != null) {
                    var weekFrequency = parseInt(weekFrequencyAttribute.value);

                    var RepeatEventDate = new Date(item.EventDate);
                    var RepeatEndDate = new Date(item.EndDate);

                    //Repeat Forever
                    if (repeatForever.length > 0) {
                        var eventTimes = 500;
                        CreateWeeklyEventTwoByEventTimes(item, weeklyData, weekFrequency, RepeatEventDate, RepeatEndDate, eventTimes);
                    }
                        //Repeat Instances
                    else if (repeatInstances.length > 0) {
                        var eventTimes = parseInt(repeatInstances[0].innerHTML);
                        CreateWeeklyEventTwoByEventTimes(item, weeklyData, weekFrequency, RepeatEventDate, RepeatEndDate, eventTimes);
                    }
                        //Repeat End Date
                    else if (windowEnd.length > 0) {
                        CreateWeeklyEventTwoByEndDate(item, weeklyData, weekFrequency, RepeatEventDate, RepeatEndDate);
                    }
                    else {
                        //Exception: Something went wrong!!!
                    }
                }
            } catch (e) {

            }
        }
        function CreateWeeklyEventTwoByEventTimes(item, weeklyData, weekFrequency, RepeatEventDate, RepeatEndDate, eventTimes) {
            i = 0;
            while (i < eventTimes) {
                for (var ii = 0; ii < 7 && i < eventTimes; ii++) {
                    var EventDateToCheck = new Date(RepeatEventDate);
                    EventDateToCheck.setDate(EventDateToCheck.getDate() + ii)
                    var dayInWeek = EventDateToCheck.getDay();
                    var weekday = Weekdays[dayInWeek];
                    var weekdayAttribute = weeklyData[0].attributes[weekday];
                    if (weekdayAttribute != null) {
                        var key = BuildRepeatItemId(item.UID, EventDateToCheck);
                        if (deletedKeys.indexOf(key) == -1) {
                            calendarListItemsExpandedTwo.push({
                                ID: BuildRepeatItemId(item.ID, EventDateToCheck),
                                Title: item.Title,
                                EventDate: new Date(EventDateToCheck),
                                EndDate: new Date(EventDateToCheck.getFullYear(), EventDateToCheck.getMonth(), EventDateToCheck.getDate(), RepeatEndDate.getHours(), RepeatEndDate.getMinutes(), 0, 0),
                                Category: item.Category,
                                fAllDayEvent: item.fAllDayEvent,
                            });
                        }
                        i++;
                    }

                    //Check Weekend
                    if (dayInWeek == 6 && RepeatEventDate.getDay() != 0) {
                        //Reset to first day in week (sunday)
                        EventDateToCheck.setDate(EventDateToCheck.getDate() + 1 - 7);
                        RepeatEventDate = new Date(EventDateToCheck);
                        ii = 7; //break;
                    }
                }

                RepeatEventDate.setDate(RepeatEventDate.getDate() + (weekFrequency * 7));
            }
        }
        function CreateWeeklyEventTwoByEndDate(item, weeklyData, weekFrequency, RepeatEventDate, RepeatEndDate) {
            var EventDateToCheck = new Date(RepeatEventDate);
            while (EventDateToCheck <= RepeatEndDate) {
                for (var ii = 0; ii < 7; ii++) {
                    EventDateToCheck = new Date(RepeatEventDate);
                    EventDateToCheck.setDate(EventDateToCheck.getDate() + ii)
                    var dayInWeek = EventDateToCheck.getDay();
                    var weekday = Weekdays[dayInWeek];
                    var weekdayAttribute = weeklyData[0].attributes[weekday];
                    if (weekdayAttribute != null && EventDateToCheck <= RepeatEndDate) {
                        var key = BuildRepeatItemId(item.UID, EventDateToCheck);
                        if (deletedKeys.indexOf(key) == -1) {
                            calendarListItemsExpandedTwo.push({
                                ID: BuildRepeatItemId(item.ID, EventDateToCheck),
                                Title: item.Title,
                                EventDate: new Date(EventDateToCheck),
                                EndDate: new Date(EventDateToCheck.getFullYear(), EventDateToCheck.getMonth(), EventDateToCheck.getDate(), RepeatEndDate.getHours(), RepeatEndDate.getMinutes(), 0, 0),
                                Category: item.Category,
                                fAllDayEvent: item.fAllDayEvent,
                            });
                        }
                    }

                    //Check Weekend
                    if (dayInWeek == 6 && RepeatEventDate.getDay() != 0) {
                        //Reset to first day in week (sunday)
                        EventDateToCheck.setDate(EventDateToCheck.getDate() + 1 - 7);
                        RepeatEventDate = new Date(EventDateToCheck);
                        ii = 7; //break;
                    }
                }

                RepeatEventDate.setDate(RepeatEventDate.getDate() + (weekFrequency * 7));
            }
        }
        function CreateMonthlyEventTwo(item, monthlyData, repeatForever, repeatInstances, windowEnd) {
            try {
                var monthFrequencyAttribute = monthlyData[0].attributes['monthFrequency'];
                if (monthFrequencyAttribute != null) {
                    var monthFrequency = parseInt(monthFrequencyAttribute.value);

                    var RepeatEventDate = new Date(item.EventDate);
                    var RepeatEndDate = new Date(item.EndDate);

                    var eventTimes = 0;

                    //Repeat Forever
                    if (repeatForever.length > 0) {
                        eventTimes = 20;
                    }
                        //Repeat Instances
                    else if (repeatInstances.length > 0) {
                        eventTimes = parseInt(repeatInstances[0].innerHTML);
                    }
                        //Repeat End Date
                    else if (windowEnd.length > 0) {
                        CreateMonthlyEventTwoByEndDate(item, monthlyData, monthFrequency, RepeatEventDate, RepeatEndDate);
                    }
                    else {
                        //Exception: Something went wrong!!!
                    }
                    var dayInMonth = parseInt(monthlyData[0].attributes['day'].value);
                    for (var i = 0; i < eventTimes; i++) {
                        var key = BuildRepeatItemId(item.UID, RepeatEventDate);
                        if (deletedKeys.indexOf(key) == -1) {
                            calendarListItemsExpandedTwo.push({
                                ID: BuildRepeatItemId(item.ID, RepeatEventDate),
                                Title: item.Title,
                                EventDate: new Date(RepeatEventDate.getFullYear(), RepeatEventDate.getMonth(), dayInMonth, RepeatEventDate.getHours(), RepeatEventDate.getMinutes(), 0, 0),
                                EndDate: new Date(RepeatEventDate.getFullYear(), RepeatEventDate.getMonth(), dayInMonth, RepeatEndDate.getHours(), RepeatEndDate.getMinutes(), 0, 0),
                                Category: item.Category,
                                fAllDayEvent: item.fAllDayEvent,
                            });
                        }

                        RepeatEventDate.setMonth(RepeatEventDate.getMonth() + monthFrequency);
                    }
                }
            } catch (e) {

            }
        }
        function CreateMonthlyEventTwoByEndDate(item, monthlyData, monthFrequency, RepeatEventDate, RepeatEndDate) {
            var dayInMonth = parseInt(monthlyData[0].attributes['day'].value);
            while (RepeatEventDate <= RepeatEndDate) {
                //Check Event was deleted?
                var key = BuildRepeatItemId(item.UID, RepeatEventDate);
                if (deletedKeys.indexOf(key) == -1) {
                    calendarListItemsExpandedTwo.push({
                        ID: BuildRepeatItemId(item.ID, RepeatEventDate),
                        Title: item.Title,
                        EventDate: new Date(RepeatEventDate.getFullYear(), RepeatEventDate.getMonth(), dayInMonth, RepeatEventDate.getHours(), RepeatEventDate.getMinutes(), 0, 0),
                        EndDate: new Date(RepeatEventDate.getFullYear(), RepeatEventDate.getMonth(), dayInMonth, RepeatEndDate.getHours(), RepeatEndDate.getMinutes(), 0, 0),
                        Category: item.Category,
                        fAllDayEvent: item.fAllDayEvent,
                    });
                }

                RepeatEventDate.setMonth(RepeatEventDate.getMonth() + monthFrequency);
            }
        }

        function BuildRepeatItemId(id, date) {
            return id + ".0." + moment(new Date(date.valueOf() + date.getTimezoneOffset() * 60000)).format("YYYY-MM-DDTHH:mm:ss") + "Z";    //Ex: ID=38.0.2016-12-18T01:00:00Z
        }

        function CheckExistedDateTwo(dateToCheck) {
            //Check Existed Date
            var dateIsExisted = false;
            for (var ii = 0; ii < eventDateExisted.length; ii++) {
                if (eventDateExisted[ii] == dateToCheckText) {
                    return true;
                }
            }

            return dateIsExisted;
        }
        function PaintEventTwo(date, category) {
            var background = "";
            var border = "";

            //Skip Today
            if (date.format("dd/MM") != (new Date()).format("dd/MM")) {
                switch (category) {
                    case "Meeting":
                        background = "#fff2cc";
                        border = "1px solid #bf8f00";
                        break;
                    case "Business":
                        background = "#bdd7ee";
                        border = "1px solid #2f75b5";
                        break;
                    case "Birthday":
                        background = "#fce4d6";
                        border = "1px solid #c65911";
                        break;
                    case "Holiday":
                        background = "#31943a";
                        border = "1px solid #548235";
                        break;
                    case "Get-together":
                        background = "#8EA9DB";
                        border = "1px solid #305496";
                        break;
                    case "Gifts":
                        background = "#D6DCE4";
                        border = "1px solid #333F4F";
                        break;
                    case "Anniversary":
                        background = "#FDDBF7";
                        border = "1px solid #86086E";
                        break;

                    case "Weekend":
                        background = "#fc3424";
                        border = "1px solid #4A442A";
                        break;

                    case "Compensation Day-Off":
                        background = "#49DF5D";
                        border = "1px solid #49DF5D";
                        break;

                    case "On Leave":
                        background = "F5EB67";
                        border = "1px solid #49DF5D";
                        break;

                    case "Multi":
                        background = "";
                        border = "2px solid #2f75b5";
                        break;

                    default:
                        background = "";
                        border = "1px solid #2989d1";
                        break;
                }
            }

            console.log("Category" + category);
            console.log("Background" + background);
            $('#divDatePickerTwo .ui-datepicker-calendar a')
                .filter(function (index) {
                    return $(this).text() == date.getDate() &&
                        $(this).parent('td').attr("data-year") == date.getFullYear() &&
                        $(this).parent('td').attr("data-month") == date.getMonth();
                }).css("border", border).css("background", background);
        }

        function CreatePopUp(eventDate, eventDuration, itemURL, title) {

            //if ($("#" + eventDate.getDate() + "_eventPopUp").html() != null) {
            //    $("#" + eventDate.getDate() + "_eventPopUp").empty();
            //}
            if ($('#divDatePickerTwo .ui-datepicker-calendar').find("#" + eventDate.getDate() + "_eventPopUp2").html() != null) {
                $('#divDatePickerTwo .ui-datepicker-calendar').find("#" + eventDate.getDate() + "_eventPopUp2").empty();
            }
            

            $('#divDatePickerTwo .ui-datepicker-calendar a')
                .filter(function (index) {
                    return $(this).text() == eventDate.getDate() &&
                        $(this).parent('td').attr("data-year") == eventDate.getFullYear() &&
                        $(this).parent('td').attr("data-month") == eventDate.getMonth();
                }).parent('td').append("<div class='eventPopUpDiv' id='" + eventDate.getDate() + "_eventPopUp2' style='display:none'></div> ");

            $('#divDatePickerTwo .ui-datepicker-calendar a')
               .filter(function (index) {
                   return $(this).text() == eventDate.getDate() &&
                       $(this).parent('td').attr("data-year") == eventDate.getFullYear() &&
                       $(this).parent('td').attr("data-month") == eventDate.getMonth();
               }).parent('td').mouseover(function () {

                   document.getElementById($(this).find('a').first().text() + "_eventPopUp2").style.display = "inline";

               });

            $('#divDatePickerTwo .ui-datepicker-calendar a')
                .filter(function (index) {
                    return $(this).text() == eventDate.getDate() &&
                        $(this).parent('td').attr("data-year") == eventDate.getFullYear() &&
                        $(this).parent('td').attr("data-month") == eventDate.getMonth();
                }).parent('td').mouseout(function () {
                    document.getElementById($(this).find('a').first().text() + "_eventPopUp2").style.display = "none";


                });
            calliHtml = '<li class="divCalendarLI2" ><a class="anchCalLi"' + " onclick='window.open(this.href)'" + ' href="' + itemURL + '&Source=' + encodeURIComponent(window.location.href) + '">' + title + '</a> ' + eventDuration + '</li>';
            $('#divDatePickerTwo .ui-datepicker-calendar').find("#" + eventDate.getDate() + "_eventPopUp2").append("<h1 class='calHead'>" + moment(eventDate).format('LL') + "</h1 >");
            $('#divDatePickerTwo .ui-datepicker-calendar').find("#" + eventDate.getDate() + "_eventPopUp2").append("<ul class = 'divCalendarUL1'>");
            $('#divDatePickerTwo .ui-datepicker-calendar').find("#" + eventDate.getDate() + "_eventPopUp2" + " .divCalendarUL1").append(calliHtml);

           
         
        }

    })(jQuery);
}