﻿var spHostUrl = '';
var layoutsRoot = '';
var serviveURL = '/_vti_bin/Services/Employee/EmployeeService.svc/GetPeoplePickerData';
var servivePostURL = '/_vti_bin/Services/Employee/EmployeeService.svc/PostPeoplePickerData';
$(document).ready(function () {

    //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
    spHostUrl = _spPageContextInfo.webAbsoluteUrl;

    //Build absolute path to the layouts root with the spHostUrl
    layoutsRoot = spHostUrl + '/_layouts/15/';

    ExecuteOrDelayUntilScriptLoaded(InitRequestExecutor, "sp.js")

});


function InitRequestExecutor() {

    var spLanguage = "en";

    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {

        context = new SP.ClientContext(spHostUrl);
        var factory = new SP.ProxyWebRequestExecutorFactory(spHostUrl);
        context.set_webRequestExecutorFactory(factory);
        //Make a author people picker control
        //1. context = SharePoint Client Context object
        //2. $('#spanRnDAuthorPplPic') = SPAN that will 'host' the people picker control
        //3. $('#ContentPlaceHolder1_UploadRnDAuthorPplPic') = INPUT that will be used to capture user input
        //4. $('#divRnDAuthorPplPicSearch') = DIV that will show the 'dropdown' of the people picker
        //5. $('#ContentPlaceHolder1_hdnRnDAuthorPplPic') = INPUT hidden control that will host a JSON string of the resolved users
        //6. data url on the server (webmethod in webforms, controller action in MVC)
        RnDAuthorPeoplePicker = new CAMControl.PeoplePicker(context, $('#spanRnDAuthorPplPic'), $('#ctl00_PlaceHolderMain_UploadRnDAuthorPplPic'), $('#divRnDAuthorPplPicSearch'), $('#ctl00_PlaceHolderMain_hdnRnDAuthorPplPic'), serviveURL);
        // required to pass the variable name here!
        RnDAuthorPeoplePicker.InstanceName = "RnDAuthorPeoplePicker";
        // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
        // Do not set the Language property if you do not have foreseen javascript resource file for your language
        RnDAuthorPeoplePicker.Language = spLanguage;
        // optionally show more/less entries in the people picker dropdown, 4 is the default
        RnDAuthorPeoplePicker.MaxEntriesShown = 100;
        // Can duplicate entries be selected (default = false)
        RnDAuthorPeoplePicker.AllowDuplicates = false;
        // Show the user loginname
        RnDAuthorPeoplePicker.ShowLoginName = false;
        // Show the user title
        RnDAuthorPeoplePicker.ShowTitle = false;
        // Set principal type to determine what is shown (default = 1, only users are resolved). 
        // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
        // Set ShowLoginName and ShowTitle to false if you're resolving groups
        RnDAuthorPeoplePicker.PrincipalType = 2;

        RnDAuthorPeoplePicker.FilterData = [{ PositionName: 'BOD' }, { PositionName: 'Department Head' }, { PositionName: 'Group Leader' }, { PositionName: 'Direct Manager' }];
        // start user resolving as of 2 entered characters (= default)
        RnDAuthorPeoplePicker.MinimalCharactersBeforeSearching = 4;
        // for dynamically resizing the box size
        RnDAuthorPeoplePicker.textBoxId = "divRnDAuthorPplPic";
        // Hookup everything
        RnDAuthorPeoplePicker.Initialize();

        initAuthorizedPeoplePicker(spLanguage);
    });
}
function initAuthorizedPeoplePicker(spLanguage) {
    //Make a RnD Authorized People Picker
    RnDAuthorizedPeoplePicker = new CAMControl.PeoplePicker(context, $('#spanRnDAuthorizedPplPic'),

    $('#ContentPlaceHolder1_UploadRnDAuthorizedPplPic'), $('#divRnDAuthorizedPplPicSearch'),

    $('#ContentPlaceHolder1_hdnRnDAuthorizedPplPic'), servivePostURL);
    RnDAuthorizedPeoplePicker.InstanceName = "RnDAuthorizedPeoplePicker";
    RnDAuthorizedPeoplePicker.Language = spLanguage;
    RnDAuthorizedPeoplePicker.MaxEntriesShown = 100;
    RnDAuthorizedPeoplePicker.AllowDuplicates = false;
    RnDAuthorizedPeoplePicker.ShowLoginName = false;
    RnDAuthorizedPeoplePicker.ShowTitle = false;
    RnDAuthorizedPeoplePicker.PrincipalType = 1;

    RnDAuthorizedPeoplePicker.MinimalCharactersBeforeSearching = 4;
    RnDAuthorizedPeoplePicker.textBoxId = "divRnDAuthorizedPplPic";
    RnDAuthorizedPeoplePicker.Initialize();
}
function LoadPeoplePicker(selector, display, datasource) {
    var displayHTML = "";
    var selectorData = [];
    for (i = 0; i < datasource.length; i++) {
        displayHTML += "<p style='margin: 0 0'><span class='cam-peoplepicker-userSpan'><span class='cam-entity-resolved'>" + datasource[i].FullName + "</span><a title='Remove person or group " + datasource[i].FullName + "' class='cam-peoplepicker-delImage' onclick='RnDAuthorPeoplePicker.DeleteProcessedUser('" + datasource[i].UserName + "); return false;' href='#'>x</a></span></p>";

        var data = {
            Login: datasource[i].UserName,
            Name: datasource[i].FullName
        }
        selectorData.push(data);
    };
    $(display).html(displayHTML);
    $(selector).val(JSON.stringify(selectorData));

}