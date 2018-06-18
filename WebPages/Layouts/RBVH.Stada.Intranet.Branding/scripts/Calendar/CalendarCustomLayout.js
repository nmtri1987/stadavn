
var CalendarControl = CalendarControl || {};
CalendarControl.CalendarControlSide = ".ms-core-navigation";
CalendarControl.DatePicker = "#DatePickerDiv";
CalendarControl.IsLoaded = false;
$(document).ready(function () {

    var CalendarControlSide = $(CalendarControl.CalendarControlSide);


    CalendarControlSide.each(function () {

        if ($(this).find(CalendarControl.DatePicker).length > 0) {

            CalendarControlSide.css({ "display": "flex", "width": "500px" });
        };
    });

    var calendarControl = $('#AsynchronousViewDefault_CalendarView');
    var datepickerControl = $('#DatePickerDiv');
    if (calendarControl.length > 0 && datepickerControl.length && window.browseris.ie) {
        if (!window.location.hash) {
            window.location = window.location + '#loaded';
            window.location.reload();
        }


        $(window).resize(function () {
            if (CalendarControl.IsLoaded) {
                window.location.reload();
            }
            CalendarControl.IsLoaded = true;
        });

    }

});
$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results == null) {
        return null;
    }
    else {
        return decodeURI(results[1]) || 0;
    }
}