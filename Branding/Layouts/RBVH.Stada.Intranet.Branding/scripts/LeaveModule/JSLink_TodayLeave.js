var todayLeaveContext;
(function () {

    var itemCtxTodayLeave = {};
    itemCtxTodayLeave.Templates = {};
    //  itemCtxTodayLeave.Templates.Header = SecurityLeave_RenderHeader;
    itemCtxTodayLeave.OnPreRender = hideToolbar;
    itemCtxTodayLeave.Templates.Fields = {
        "Leaved": {
            "View": TodayLeaveFieldTemplate
        },
        "Requester":
        {
            "View": TodayLeaveRequesterFieldTemplate
        }
        ,
        // "RequesterPhoto":
        // {
        //     "View": TodayLeaveRequesterPhotoFieldTemplate
        // }
    };
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(itemCtxTodayLeave);

})();

function hideToolbar(ctx) {
    ctx.DisableHeroButton = true;

}

function TodayLeaveRequesterFieldTemplate(ctx) {

    return ctx.CurrentItem["Requester"][0].lookupValue;
}

function TodayLeaveRequesterPhotoFieldTemplate(ctx) {
    //  ShowPhoto(1);
    // return  ctx.CurrentItem["Requester"][0].; 
}

function ShowPhoto(itemId) {
    debugger;
    var clientContext = SP.ClientContext.get_current();
    var oList = clientContext.get_web().get_lists().getByTitle('Empployee Info');
    this.collListItem = oList.getItemById(itemId);
    clientContext.load(collListItem);
    clientContext.executeQueryAsync(
        Function.createDelegate(this, function (data) {

        }),
        Function.createDelegate(this, function (data) {
        })
    );
}



function GetItemTypeForListName(name) {
    return "SP.Data." + name.charAt(0).toUpperCase() + name.split(" ").join("").slice(1) + "ListItem";
}

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

function updateListItem(itemId, listName, siteUrl, value, leavedAt, success, failure) {
    var itemType = GetItemTypeForListName(listName);

    var item = {
        "__metadata": { "type": itemType },
        "Leaved": value,
        "LeavedAt": leavedAt
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

function Update(itemId, value) {
    var listName = "Leave Management";
    var url = _spPageContextInfo.webAbsoluteUrl;

    var leavedAt = new Date($.now());
    updateListItem(itemId, listName, url, value, leavedAt, function () {
        location.reload();
    }, function () {
        alert("Ooops, an error occured. Please try again");
        location.reload();
    });
}

function OnCheckboxClick(element) {
    var chbCurrentItemValue = $(element).attr("value");
    if (element.checked) {
        Update(chbCurrentItemValue, true);
    }
    else {
        Update(chbCurrentItemValue, false);
    }
}

function TodayLeaveFieldTemplate(ctx) {
    var checkboxValue = ctx.CurrentItem["Leaved.value"];
    var curentItemId = ctx.CurrentItem.ID;
    var checkbox;
    if (checkboxValue == '0') {
        checkbox = '<input type="checkbox" name="chboxLeaved" style="transform: scale(1.2);  cursor: pointer;" onclick="OnCheckboxClick(this)"  value= "' + curentItemId + '">';
    }
    else {
        checkbox = '<input type="checkbox"  name="chboxLeaved" style="transform: scale(1.2);  cursor: pointer;" onclick="OnCheckboxClick(this)" checked value= "' + curentItemId + '"/>';
    }
    return checkbox;
}
