var spHostUrl = '';
var layoutsRoot = '';
var approverIdVar;
var servivePostURL = '/_vti_bin/Services/Employee/EmployeeService.svc/GetAdditionalApprovers';
function InitPeoplePicker(approverId) {
    approverIdVar = approverId;

    $(document).ready(function () {
        //Get the URI decoded SharePoint site url from the SPHostUrl parameter.
        spHostUrl = _spPageContextInfo.webAbsoluteUrl;
        //Build absolute path to the layouts root with the spHostUrl
        layoutsRoot = spHostUrl + '/_layouts/15/';
        ExecuteOrDelayUntilScriptLoaded(InitRequestExecutor, "sp.js")
    });
}

function InitRequestExecutor() {
    var lcid = SP.Res.lcid;
    var spLanguage = "en";
    if (lcid === "1066") {
        spLanguage = "vi";
    }

    //load scripts for cross site calls (needed to use the people picker control in an IFrame)
    $.getScript(layoutsRoot + 'SP.RequestExecutor.js', function () {
        context = new SP.ClientContext(spHostUrl);
        var factory = new SP.ProxyWebRequestExecutorFactory(spHostUrl);
        context.set_webRequestExecutorFactory(factory);
        //Make a author people picker control
        LeaveRequestPeoplePicker = new CAMControl.PeoplePicker(context, $('#spanLeaveAdditionalPplPic'), $('#ctl00_PlaceHolderMain_UploadLeavePplPic'), $('#divLeaveAdditionalPplPicSearch'), $('#ctl00_PlaceHolderMain_hdnLeavePplPic'), servivePostURL);
        // required to pass the variable name here!
        LeaveRequestPeoplePicker.InstanceName = "LeaveRequestPeoplePicker";
        // Pass current language, if not set defaults to en-US. Use the SPLanguage query string param or provide a string like "nl-BE"
        // Do not set the Language property if you do not have foreseen javascript resource file for your language
        LeaveRequestPeoplePicker.Language = spLanguage;
        // optionally show more/less entries in the people picker dropdown, 4 is the default
        LeaveRequestPeoplePicker.MaxEntriesShown = 100;
        // Can duplicate entries be selected (default = false)
        LeaveRequestPeoplePicker.AllowDuplicates = false;
        // Show the user loginname
        LeaveRequestPeoplePicker.ShowLoginName = false;
        // Show the user title
        LeaveRequestPeoplePicker.ShowTitle = false;
        // Set principal type to determine what is shown (default = 1, only users are resolved). 
        // See http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.utilities.principaltype.aspx for more details
        // Set ShowLoginName and ShowTitle to false if you're resolving groups
        LeaveRequestPeoplePicker.PrincipalType = 1;
        var addtionalApprovers = [];
        if (approverIdVar) {
            addtionalApprovers = [{ PositionName: approverIdVar }]
        }
        LeaveRequestPeoplePicker.FilterData = addtionalApprovers;

        // start user resolving as of 2 entered characters (= default)
        LeaveRequestPeoplePicker.MinimalCharactersBeforeSearching = 1;
        // for dynamically resizing the box size
        LeaveRequestPeoplePicker.textBoxId = "divLeaveAdditionalPplPic";
        // Hookup everything
        LeaveRequestPeoplePicker.Initialize();
    });
}

function LoadPeoplePicker(selector, display, datasource) {
    var displayHTML = "";
    var selectorData = [];
    if (typeof datasource != 'undefined' && datasource != null) {
        for (i = 0; i < datasource.length; i++) {
            displayHTML += "<p style='margin: 0 0'><span class='cam-peoplepicker-userSpan'><span class='cam-entity-resolved'>" + datasource[i].FullName + "</span><a title='Remove person or group " + datasource[i].FullName + "' class='cam-peoplepicker-delImage' onclick='LeaveRequestPeoplePicker.DeleteProcessedUser('" + datasource[i].UserName + "); return false;' href='#'>x</a></span></p>";
            var data = {
                Login: datasource[i].UserName,
                Name: datasource[i].FullName
            }
            selectorData.push(data);
        };
    }
    $(display).html(displayHTML);
    $(selector).val(JSON.stringify(selectorData));
}