var LeaveTaskListQuickApproveResource =
{
    PageResourceFileName: "RBVHStadaWebpages",
    ApproveButton: "Approve",
    RejectButton: "Reject",
    SelectAtLeastToApprove: "Please select at least 1 item to approve!",
    SelectAtLeastToReject: "Please select at least 1 item to reject!",
    ConfirmQuickApproveMessage: "Are you sure you want to update the status of selected items to 'Approved'? ",
    ConfirmQuickRejectMessage: "Are you sure you want to update the status of selected items to 'Rejected'?",
    QuickApproveItemUpdateSuccess: "Item(s) updated success",
    QuickApproveItemUpdateFail: "Item(s) updated fail",
    QuickApproveItemUpdateExist: "Item(s) could not change status"
};

(function () {
    //Register resource
    SP.SOD.registerSod(LeaveTaskListQuickApproveResource.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + LeaveTaskListQuickApproveResource.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
    SP.SOD.executeFunc(LeaveTaskListQuickApproveResource.PageResourceFileName, "Res", OnPageLeaveTaskListResourcesReady);

    var leaveTaskListQuickAppoveCtx = {};
    leaveTaskListQuickAppoveCtx.Templates = {};
    leaveTaskListQuickAppoveCtx.Templates.Header = LeaveTaskList_RenderHeader;
    leaveTaskListQuickAppoveCtx.OnPostRender = LeaveTaskList_OnPostRender;
    leaveTaskListQuickAppoveCtx.Templates.Item = LeaveTaskList_ItemOverrideFun;
    leaveTaskListQuickAppoveCtx.Templates.Fields = {
        "Title":
        {
            "View" : LeaveView_Title
        }
    }
    leaveTaskListQuickAppoveCtx.Templates.Footer = LeaveTaskList_Footer;
    leaveTaskListQuickAppoveCtx.BaseViewID = 5;
    leaveTaskListQuickAppoveCtx.ListTemplateType = 171;
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(leaveTaskListQuickAppoveCtx);
})();

function OnPageLeaveTaskListResourcesReady() {
    LeaveTaskListQuickApproveResource.ApproveButton = Res.approveButton;
    LeaveTaskListQuickApproveResource.RejectButton = Res.rejectButton;
    LeaveTaskListQuickApproveResource.SelectAtLeastToApprove = Res.selectAtLeastToApprove;
    LeaveTaskListQuickApproveResource.SelectAtLeastToReject = Res.selectAtLeastToReject;
    LeaveTaskListQuickApproveResource.ConfirmQuickApproveMessage = Res.confirmQuickApproveMessage;
    LeaveTaskListQuickApproveResource.ConfirmQuickRejectMessage = Res.confirmQuickRejectMessage;
    LeaveTaskListQuickApproveResource.QuickApproveItemUpdateSuccess = Res.quickApproveItemUpdateSuccess;
    LeaveTaskListQuickApproveResource.QuickApproveItemUpdateFail = Res.quickApproveItemUpdateFail;
    LeaveTaskListQuickApproveResource.QuickApproveItemUpdateExist = Res.quickApproveItemUpdateExist;

}

function LeaveTaskList_RenderHeader(ctx) {
    var imageSliderHtml = "<div class='row'><div class='col-md-12' style='padding-right: 33px;' ><div class='pull-right'>" +
                    "<button type='button' id='leave_quick_approve' class='ms-ButtonHeightWidth'  style='margin-right:10px' >" + LeaveTaskListQuickApproveResource.ApproveButton + "</button>" +
                    "<button type='button' id='leave_quick_reject'  class='ms-ButtonHeightWidth'  >" + LeaveTaskListQuickApproveResource.RejectButton + "</button>" +
                    "</div></div></div>";
    var headerHtml = "<div>" + RenderHeaderTemplate(ctx) + "</div>";
    return headerHtml + imageSliderHtml;
}

function LeaveTaskList_ItemOverrideFun(ctx) {
    return RenderItemTemplate(ctx);
}

function LeaveTaskList_Footer(ctx) {
    return RenderFooterTemplate(ctx);
}

function LeaveView_Title(ctx) {
    
}

function LeaveTaskList_OnPostRender(ctx) {
    function getListItemWithId(itemId, listName, siteurl, success, failure) {
        var url = siteurl + "/_api/web/lists/getbytitle('" + listName + "')/items?$filter=Id eq " + itemId;
        $.ajax({
            url: url,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data.d.results.length == 1) {
                    success(data.d.results[0]);
                }
                else {
                    console.log("Multiple results obtained for the specified Id value");
                }
            },
            error: function (data) {
                failure(data);
            }
        });
    }

    function GetItemTypeForListName(name) {
        return "SP.Data." + name.charAt(0).toUpperCase() + name.split(" ").join("").slice(1) + "ListItem";
    }

    function UpdateListitem(itemId, listName, siteUrl, percentComplete, outcome, status, success, failure) {
        var itemType = GetItemTypeForListName(listName);
        var item = {
            "__metadata": { "type": itemType },
            "PercentComplete": percentComplete,
            "TaskOutcome": outcome,
            "Status": status
        };

        getListItemWithId(itemId, listName, siteUrl, function (data) {
            $.ajax({
                url: data.__metadata.uri,
                type: "POST",
                contentType: "application/json;odata=verbose",
                data: JSON.stringify(item),
                headers: {
                    "Accept": "application/json;odata=verbose",
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "X-HTTP-Method": "MERGE",
                    "If-Match": data.__metadata.etag
                },
                success: function (data) {
                    success(data);
                },
                error: function (data) {
                    failure(data);
                }
            });
        }, function (data) {
            failure(data);
        });
    }
    $('#leave_quick_approve').click(function () {
        var successApproveListItems = [];
        var failureApproveListItems = [];
        var notUpdateApproveListItems = [];

        var countApproveItem = 0;
        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems != undefined && selectedItems.length > 0) {
            var cofirmApproveMessageResult = confirm(LeaveTaskListQuickApproveResource.ConfirmQuickApproveMessage);
            if (cofirmApproveMessageResult == true) {

                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Leave Workflow Task List')/items(" + selectedItems[index].id + ")",
                        method: "GET",
                        headers: { "Accept": "application/json; odata=verbose" },
                        success: function (data) {
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    ajaxGetListItemPromise.then(function (data) {
                        if (data != undefined) {
                            if (data.d.TaskOutcome != "Approved" && data.d.TaskOutcome != "Rejected") {
                                UpdateListitem(data.d.Id, "Leave Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Approved", "Completed", function () {
                                    countApproveItem++;
                                    successApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured");
                                    countApproveItem++;

                                    failureApproveListItems.push(data.d.Id);
                                    if (countApproveItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                    }
                                });
                            }
                            else {
                                countApproveItem++;
                                notUpdateApproveListItems.push(data.d.Id);
                                if (countApproveItem === selectedItems.length) {
                                    LeaveTask_ShowResultMessage(successApproveListItems, failureApproveListItems, notUpdateApproveListItems);
                                }
                            }
                        }
                    });
                }

            }
            else {
                //DO nothing
            }
        }
        else {
            alert(LeaveTaskListQuickApproveResource.SelectAtLeastToApprove);
        }
    });

    $('#leave_quick_reject').click(function () {
        var successRejectListItems = [];
        var failureRejectListItems = [];
        var notUpdateRejectListItems = [];

        
        var countRejectItem = 0;
        var context = SP.ClientContext.get_current();
        var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
        if (selectedItems && selectedItems != undefined && selectedItems.length > 0) {
            var cofirmRejectMessageResult = confirm(LeaveTaskListQuickApproveResource.ConfirmQuickRejectMessage);
            if (cofirmRejectMessageResult == true) {
                for (var index = 0; index < selectedItems.length; index++) {
                    var ajaxGetListItemPromise = $.ajax({
                        url: _spPageContextInfo.webAbsoluteUrl + "/_api/web/lists/getbytitle('Leave Workflow Task List')/items(" + selectedItems[index].id + ")",
                        method: "GET",
                        headers: { "Accept": "application/json; odata=verbose" },
                        success: function (data) {
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    ajaxGetListItemPromise.then(function (data) {
                        if (data != undefined) {
                            if (data.d.TaskOutcome != "Approved" && data.d.TaskOutcome != "Rejected") {
                                UpdateListitem(data.d.Id, "Leave Workflow Task List", _spPageContextInfo.webAbsoluteUrl, 1, "Rejected", "Completed", function () {
                                    countRejectItem++;
                                    successRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                }, function () {
                                    console.log("Ooops, an error occured. Please try again");
                                    countRejectItem++;
                                    failureRejectListItems.push(data.d.Id);
                                    if (countRejectItem === selectedItems.length) {
                                        LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                    }
                                });
                            }
                            else {
                                countRejectItem++;
                                notUpdateRejectListItems.push(data.d.Id);
                                if (countRejectItem === selectedItems.length) {
                                    LeaveTask_ShowResultMessage(successRejectListItems, failureRejectListItems, notUpdateRejectListItems);
                                }
                            }
                        }
                    });
                }
            }
            else {
                //DO nothing
            }
        }
        else {
            alert(LeaveTaskListQuickApproveResource.SelectAtLeastToReject);
        }
    });

    function LeaveTask_ShowResultMessage(successApproveListItem, failureApproveListItem, notUpdateApproveListItem) {
        var resultMessage = '';
        if (typeof successApproveListItem != undefined && successApproveListItem.length > 0) {
            resultMessage += LeaveTaskListQuickApproveResource.QuickApproveItemUpdateSuccess + ": " + successApproveListItem.join(", ");
        }
        if (typeof failureApproveListItem != undefined && failureApproveListItem.length > 0) {
            resultMessage += "\n" + LeaveTaskListQuickApproveResource.QuickApproveItemUpdateFail + ": " + failureApproveListItem.join(", ");
        }
        if (typeof notUpdateApproveListItem != undefined && notUpdateApproveListItem.length > 0) {
            resultMessage += "\n" + LeaveTaskListQuickApproveResource.QuickApproveItemUpdateExist + ": " + notUpdateApproveListItem.join(", ");
        }
        var informMessageResult = confirm(resultMessage);
        if (informMessageResult == true) {
            location.reload();
        }
        else {
            location.reload();
        }
    }
}
