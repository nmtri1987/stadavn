(function () {
    var propArray = [
		{ PropertyName: 'FirstName', FieldName: "FirstName" },
		{ PropertyName: 'LastName', FieldName: "LastName" },
		{ PropertyName: 'WorkEmail', FieldName: "Email" },
		{ PropertyName: 'Title', FieldName: "Position" }
    ];
    function renderPeoplePickerTemplate(renderCtx) {
        var fieldCtx = SPClientTemplates.Utility.GetFormContextForCurrentField(renderCtx);
        //1. Add event handler for People Picker control 
        fieldCtx.fieldSchema.OnUserResolvedClientScript = function (topLevelElementId, usersInfo) {
            populateFields(renderCtx, usersInfo);
        };
        var renderTemplate = SPClientPeoplePickerCSRTemplate(renderCtx);
        return renderTemplate;

    }

    function populateFields(renderCtx, usersInfo) {
        if (usersInfo.length === 0)
            return;

        var userKey = usersInfo[0].Description;
        var url = _spPageContextInfo.webAbsoluteUrl + "/_api/SP.UserProfiles.PeopleManager/getpropertiesfor(@v)?@v='" + userKey + "'$select=FirstName,LastName,WorkEmail,Title";
        try {
            $.ajax({
                url: url,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                cache: false,
                success: function (data) {
                    //2. Retrieving an additional data like user profile properties goes here 
                    var properties = data.d.UserProfileProperties.results;
                    if (properties.length > 0) {
                        for (var i = 0; i < properties.length; i++) {
                            for (var j = 0; j < propArray.length; j++) {
                                //3. Set fields values  
                                if (properties[i].Key === propArray[j].PropertyName) {
                                    setFieldControlValue(propArray[j].FieldName, properties[i].Value);
                                }
                            }
                        }
                    }
                },
                error: function (x, y, z) {
                    console.log(JSON.stringify(x) + '\n' + JSON.stringify(y) + '\n' + JSON.stringify(z));
                }
            });
        } catch (e) {
            console.log(e.message);
        }
    }

    function setFieldControlValue(fieldName, fieldValue) {
        var fieldControl = $('[id ^=' + fieldName + '][id $=Field]');
        fieldControl.val(fieldValue);
    }

    function registerFormRenderer() {
        var formContext = {};
        formContext.Templates = {};
        formContext.Templates.Fields = {
            "ADAccount": {
                "NewForm": renderPeoplePickerTemplate
            }
        };

        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(formContext);
    }
    ExecuteOrDelayUntilScriptLoaded(registerFormRenderer, 'clienttemplates.js');

})();