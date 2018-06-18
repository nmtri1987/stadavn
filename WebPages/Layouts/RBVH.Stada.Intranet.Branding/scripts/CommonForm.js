var Functions =
    {
        getDisplayName: function (ctx, name) {
            var FieldName = 'FieldName';
            for (var i = 0, len = ctx.ListSchema.Field.length; i < len; i++) {
                if (ctx.ListSchema.Field[i].Name == name) {
                    FieldName = ctx.ListSchema.Field[i].DisplayName
                }
            }
            return FieldName;
        },
        getSPFieldRender: function (ctx, fieldName) {
            var fieldContext = ctx;
            var result = ctx.ListSchema.Field.filter(function (obj) {
                return obj.Name === fieldName;
            });
            fieldContext.CurrentFieldSchema = result[0];
            fieldContext.CurrentFieldValue = ctx.ListData.Items[0][fieldName];
            return ctx.Templates.Fields[fieldName](fieldContext);
        },
        getSPFieldValue: function (ctx, fieldName) {
            var isExisted = false;
            for (var i = 0; i < ctx.ListSchema.Field.length; i++) {
                if (ctx.ListSchema.Field[i].Name === fieldName) {
                    isExisted = true;
                    break;
                }
            }

            if (isExisted === true) {
                return ctx.ListData.Items[0][fieldName];
            }
            return "";
        },
        getSPFieldLookupId: function (ctx, fieldName) {
            var currentItem = ctx.ListData.Items[0];
            var itemArray = currentItem[fieldName].split("#");
            if (itemArray && itemArray.length > 1) {
                return itemArray[0];
            }
            else {
                return "";
            }
        },
        getSPFieldLookupValue: function (ctx, fieldName) {
            var currentItem = ctx.ListData.Items[0];
            var itemArray = currentItem[fieldName].split("#");
            if (itemArray && itemArray.length > 1) {
                return itemArray[itemArray.length - 1];
            }
            else {
                return "";
            }
        },
        getSPFieldTitle: function (ctx, fieldName) {
            var result = ctx.ListSchema.Field.filter(function (obj) {
                return obj.Name === fieldName;
            });
            return result[0].Title;
        },
        populateApprovertoPeoplePicker: function (dataArray, currentEmployeeListIemId, control) {
            var loadApproverURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/" + currentEmployeeListIemId; //listItemID
            var loadApproversPromise = $.ajax({
                type: "GET",
                url: loadApproverURL,
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });

            loadApproversPromise.then(
                function (data) {
                    if (data != null) {
                        if (data != undefined && data.Approver1 != null && dataArray[0] != null) {
                            Functions.setUserFieldValue(dataArray[0].InternalFieldName, dataArray[0].FullLoginUserName, control)
                        }
                        if (data.Approver2 != undefined && data.Approver2 != null && dataArray[1] != null) {
                            Functions.setUserFieldValue(dataArray[1].InternalFieldName, dataArray[1].FullLoginUserName, control)
                        }
                        if (data.Approver3 != undefined && data.Approver3 != null && dataArray[2] != null) {
                            Functions.setUserFieldValue(dataArray[2].InternalFieldName, dataArray[2].FullLoginUserName, control)
                        }
                    }
                    else {
                        //console.log("Common function: can not get approvers")
                    }
                },
                function () {
                    //console.log("Common function: can not get approvers")
                });
        },
        setUserFieldValue: function (internalFieldName, userName, control) {
            var _PeoplePicker = $("div[id ^='" + internalFieldName + "']");
            var _PeoplePickerTopId = _PeoplePicker.attr('id');

            var _PeoplePickerEditer = $("input[id ^='" + internalFieldName + "']");
            _PeoplePickerEditer.val(userName);
            var _PeoplePickerOject = SPClientPeoplePicker.SPClientPeoplePickerDict[_PeoplePickerTopId];
            _PeoplePickerOject.AddUnresolvedUserFromEditor(true);
            if (control != undefined) {
                control.css('pointer-events', 'none');
            }
        },

        hideDatePartInDatetimePicker: function (controlNameArray) {
            $.each(controlNameArray, function (index, value) {
                $("table[id ^='" + value + "_'] input:first").hide();
                $("table[id ^='" + value + "_'] a").hide();
                $("table[id ^='" + value + "_'] td[class='ms-dttimeinput']").css("margin-left", "-6px")
            });
        },
        hideDatePickerDisplayForm: function (ctx) {
            var datetime = ctx.CurrentItem[ctx.CurrentFieldSchema.Name].trim();
            var datetimeArray = datetime.split(" ");
            var newDatetime = "";
            if (datetimeArray.length == 3) {
                newDatetime = datetimeArray[1] + " " + datetimeArray[2];
            }
            else newDatetime = text;
            return newDatetime;
        },
        getParameterByName: function (name) {
            url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        },
        getParamByName: function (name, url) {
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        },
        loadDepartment: function (departmentID, controlToBindData, departmentLabelResoure, lcid) {
            var languageParam = "";
            if (lcid == "") {
                languageParam = Functions.getParameterByName("lang");
                if (languageParam === undefined || languageParam == null || languageParam == "") {
                    languageParam = "en-US"
                }
            }
            else {
                languageParam = lcid;
            }

            var loadDepartmentURL = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/" + departmentID + "/" + languageParam;

            var loadDepartmentPromise = $.ajax({
                type: "GET",
                url: loadDepartmentURL,
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });

            loadDepartmentPromise.then(
                function (result) {
                    if (result.DepartmentName != undefined && result.DepartmentName != "") {
                        if (departmentLabelResoure != "") {
                            $(controlToBindData).html(departmentLabelResoure + ": " + result.DepartmentName);
                        }
                        else {
                            $(controlToBindData).html(result.DepartmentName);
                        }
                    }
                    else {
                        $(controlToBindData).html("");
                    }
                },
                function () {
                    //console.log("Common Functions: can not load department")
                }
            )
        },

        isTwoApproverDifference: function (control1, control1TitleResoure, control2, control2TitleResource, controlToBindError, mustBeDifferent) {
            var control1Value = $(control1).val();
            var control2Value = $(control2).val();
            var isValid = false;
            if (control1Value.length > 0 && control1Value != "" && control1Value != "[]" && control2Value.length > 0 && control2Value != "" && control2Value != "[]") {
                var value1Object = $.parseJSON(control1Value);
                var value2Object = $.parseJSON(control2Value);
                if (value1Object[0].Key === value2Object[0].Key) {
                    isValid = false;
                    $(controlToBindError).html(control1TitleResoure + ", " + control2TitleResource + " " + mustBeDifferent);
                }
                else {
                    isValid = true;
                    $(controlToBindError).html("");
                }
            }
            else {
                isValid = true;
            }
            return isValid;
        },

        removeParam: function (key, sourceURL) {
            var rtn = sourceURL.split("?")[0],
                param,
                params_arr = [],
                queryString = (sourceURL.indexOf("?") !== -1) ? sourceURL.split("?")[1] : "";
            if (queryString !== "") {
                params_arr = queryString.split("&");
                for (var i = params_arr.length - 1; i >= 0; i -= 1) {
                    param = params_arr[i].split("=")[0];
                    if (param === key) {
                        params_arr.splice(i, 1);
                    }
                }
                rtn = rtn + "?" + params_arr.join("&");
            }
            return rtn;
        },

        redirectToSource: function (substituteRedirectUrl) {
            var url = "";
            var sourceParam = Functions.getParameterByName("Source");
            if (sourceParam) {
                url = sourceParam;
            }
            else if (substituteRedirectUrl) {
                url = substituteRedirectUrl;
            }
            else {
                url = "/"
            }
            window.location.href = url;
        },
        padDate: function (n) {
            return (n < 10) ? ("0" + n) : n;
        },
        parseDate: function (dateString) {
            // debugger;
            // if (!dateString || dateString.length < 6 || dateString.length > 10)
            //     return false;
            // //var f = new Date(from[2], from[1] - 1, from[0]);
            // var dateStringArray = dateString.split("/"); //DD/mm/yyyy
            // return new Date(dateStringArray[2], dateStringArray[1] * 1 - 1, dateStringArray[0]);
            return new Date(dateString);
        },
        parseVietNameseDate: function (dateString) {
            // debugger;
            var parts = dateString.split("/");
            return new Date(parts[2], parts[1] - 1, parts[0]);
            //return new Date(dateString);
        },
        parseDateTimeToMMDDYYYY: function (dateString) {
            var date = Functions.parseDate(dateString);
            return Functions.padDate(date.getMonth() * 1 + 1) + "-" + Functions.padDate(date.getDate()) + "-" + Functions.padDate(date.getFullYear());
        },
        parseVietnameseDateTimeToMMDDYYYY: function (dateString) {
            var date = Functions.parseVietNameseDate(dateString);
            return Functions.padDate(date.getMonth() * 1 + 1) + "-" + Functions.padDate(date.getDate()) + "-" + Functions.padDate(date.getFullYear());
        },

        parseDateTimeToMMDDYYYY2: function (dateString) {
            var date = Functions.parseDate(dateString);
            return Functions.padDate(date.getMonth() * 1 + 1) + "/" + Functions.padDate(date.getDate()) + "/" + Functions.padDate(date.getFullYear());
        },
        parseVietnameseDateTimeToDDMMYYYY2: function (dataObject) {
            return Functions.padDate(dataObject.getDate()) + "/" + Functions.padDate(dataObject.getMonth() * 1 + 1) + "/" + Functions.padDate(dataObject.getFullYear());
        },

        removeInvalidValue: function (string) {
            if (string) {
                return string;
            }
            else {
                return "";
            }
        },
        //Groups:
        isCurrentUserMemberOfGroup: function (groupName, OnComplete) {
            var currentContext = new SP.ClientContext.get_current();
            var currentWeb = currentContext.get_web();
            var currentUser = currentContext.get_web().get_currentUser();
            currentContext.load(currentUser);

            var allGroups = currentWeb.get_siteGroups();
            currentContext.load(allGroups);

            var group = allGroups.getByName(groupName);
            currentContext.load(group);

            var groupUsers = group.get_users();
            currentContext.load(groupUsers);

            currentContext.executeQueryAsync(OnSuccess, OnFailure);
            function OnSuccess(sender, args) {
                var userInGroup = false;
                var groupUserEnumerator = groupUsers.getEnumerator();
                while (groupUserEnumerator.moveNext()) {
                    var groupUser = groupUserEnumerator.get_current();
                    if (groupUser.get_id() == currentUser.get_id()) {
                        userInGroup = true;
                        break;
                    }
                }
                OnComplete(userInGroup);
            }
            function OnFailure(sender, args) {
                OnComplete(false);
            }
        },
        getAttachments: function (listName, itemId) {
            var url = _spPageContextInfo.webAbsoluteUrl;
            var requestUri = url + "/_api/web/lists/getbytitle('" + listName + "')/items(" + itemId + ")/AttachmentFiles";
            var str = "";
            // execute AJAX request
            $.ajax({
                url: requestUri,
                type: "GET",
                headers: { "ACCEPT": "application/json;odata=verbose" },
                async: false,
                success: function (data) {
                    if (data && data.d && data.d.results[0]) {
                        str = encodeURI(data.d.results[0].ServerRelativeUrl);
                    }
                },
                error: function (err) {
                    //alert(err);
                }
            });
            return str;
        },
        parseComment: function (comment) {
            var htmlComments = '';
            if (comment && comment.length > 0) {
                var arrComment = comment.split('###');
                arrComment.forEach(function (item) {
                    var modComment = item.replace(/\:/, '___').split('___'); // [0] User: [1] Comment
                    htmlComments = htmlComments + '<b>' + modComment[0] + '</b>:' + modComment[1] + '<br />';
                });
            }

            return htmlComments;
        },
        generateApprovalHistoryTable: function (taskManagements, tableHeaders, approvalStatus, noDataAvailableMsg) {
            var tableHtml = '<table class="table gridView" style="border-collapse: collapse;" border="1" rules="all" cellspacing="0">';
            if (taskManagements && taskManagements.length > 0) {
                tableHtml += '<tr><th scope="col">' + tableHeaders[0] + '</th><th scope="col">' + tableHeaders[1] + '</th><th scope="col">' + tableHeaders[2] + '</th><th scope="col">' + tableHeaders[3] + '</th></tr>';
                for (var i = 0; i < taskManagements.length; i++) {
                    var desc = '';
                    if (taskManagements[i].Description) {
                        desc = taskManagements[i].Description;
                    }
                    var taskOutcome = approvalStatus[0];
                    if (taskManagements[i].TaskOutcome !== 'Approved') {
                        taskOutcome = approvalStatus[1];
                    }
                    tableHtml += '<tr><td style="width: 200px;">' + taskOutcome + '</td><td style="width: 250px;">' + taskManagements[i].AssignedTo.FullName + '</td><td style="width: 200px;">' + taskManagements[i].Modified + '</td><td>' + desc + '</td></tr>';
                }
            }
            else {
                tableHtml += '<span>' + noDataAvailableMsg + '</span>';
            }
            tableHtml += '</table>';

            return tableHtml;
        },
        getConfigValue: function (source, key) {
            var ret = null;
            if (source) {
                $.each(source, function (idx, obj) {
                    if (obj.Key.trim().toLowerCase() === key.trim().toLowerCase()) {
                        if (obj.Value && obj.Value.length > 0) {
                            ret = obj.Value.trim();
                            return false;
                        }
                    }
                });
            }

            return ret;
        }
    }

var ViewUtilities = ViewUtilities || {};
ViewUtilities.Paging = ViewUtilities.Paging || {};
ViewUtilities.Paging.InstanceHtml = function (ctx) {
    var firstRow = ctx.ListData.FirstRow;
    var lastRow = ctx.ListData.LastRow;
    var next = ctx.ListData.NextHref;
    var html = "<div class='Paging'>";

    ///custom PreLink
    if (ctx.ListData.PrevHref) {

        var prevIndex = firstRow - RBVH.Stada.WebPages.Constants.PageLimit;
        var prev = RBVH.Stada.WebPages.Utilities.UpdateQueryStringParameter(ctx.ListData.PrevHref, 'PageFirstRow', prevIndex);
    }
    if (lastRow != undefined) {
        html += prev ? "<a class='ms-commandLink ms-promlink-button ms-promlink-button-enabled navigate-page' data-href='" + prev + "' ><span class='ms-promlink-button-image'><img class='ms-promlink-button-left' src='/_layouts/15/images/spcommon.png?rev=23' /></span></a>" : "";
        html += "<span class='ms-paging'><span class='First'>" + firstRow + "</span> - <span class='Last'>" + lastRow + "</span></span>";
        html += next ? "<a class='ms-commandLink ms-promlink-button ms-promlink-button-enabled navigate-page' data-href='" + next + "&PagingEnd' ><span class='ms-promlink-button-image'><img class='ms-promlink-button-right' src='/_layouts/15/images/spcommon.png?rev=23'/></span></a>" : "";
        html += "</div>";
    }
    else {
        html = '';
    }
    return "</tbody></table></div>" + html;
}
ViewUtilities.Paging.RemovePagingCurrentURL = function () {
    var currentURl = window.location.search.replace('?', '');
    var keySubstring = '&PagingEnd&';
    if (currentURl.indexOf(keySubstring) > 0) {

        var substring = currentURl.substring(currentURl.indexOf(keySubstring) + keySubstring.length, currentURl.length);
        currentURl = substring;

    }
    return currentURl;
}
ViewUtilities.Paging.RemovePagingURL = function (url) {
    var currentURl = url;
    var keySubstring = '&PagingEnd&';
    if (currentURl.indexOf(keySubstring) > 0) {

        var substring = currentURl.substring(currentURl.indexOf(keySubstring) + keySubstring.length, currentURl.length);
        currentURl = "?" + substring;

    }
    return currentURl;
}

$.fn.showOption = function () {
    this.each(function () {
        if (this.tagName == "OPTION") {
            var opt = this;
            if ($(this).parent().get(0).tagName == "SPAN") {
                var span = $(this).parent().get(0);
                $(span).replaceWith(opt);
                $(span).remove();
            }
            opt.disabled = false;
            $(opt).show();
        }
    });
    return this;
}
$.fn.hideOption = function () {
    this.each(function () {
        if (this.tagName == "OPTION") {
            var opt = this;
            if ($(this).parent().get(0).tagName == "SPAN") {
                var span = $(this).parent().get(0);
                $(span).hide();
            } else {
                $(opt).wrap("span").hide();
            }
            opt.disabled = true;
        }
    });
    return this;
}
/// instance control event
$(document).ready(function () {
    $('.navigate-page').click(function () {
        var currentURl = ViewUtilities.Paging.RemovePagingCurrentURL();
        var url = $(this).attr('data-href') + '&' + currentURl + window.location.hash;
        window.location = url;
    });
});
