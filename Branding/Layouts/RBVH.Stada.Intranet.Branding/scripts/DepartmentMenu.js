var DepartmentMenu = {
    DepartmentList: "Departments",
    DepartmentNameField: "CommonName",
    UpperDepartmentField:"UpperDepartmentId",
    EnglishLanguageCultureId: "1033",
    VietnameseLanguageCultureId: "1066",
    MenuClientId: "#DepartmentMenuul",
    renderDepartments: function () {
        // ReSharper disable once UseOfImplicitGlobalInFunctionScope
        var lcid = SP.Res.lcid;
        var query = "?$filter= " + DepartmentMenu.UpperDepartmentField + " eq null";
        var orderby = "&$orderby= " + DepartmentMenu.DepartmentNameField;
        if (lcid === DepartmentMenu.VietnameseLanguageCultureId) {
            orderby = "&$orderby= " + DepartmentMenu.DepartmentNameField + lcid;
        }
        var url = _spPageContextInfo.siteAbsoluteUrl + "/_api/web/lists/getByTitle('" + DepartmentMenu.DepartmentList + "')/items" + query + orderby;
        try {
            $.ajax({
                url: url,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                cache: false,
                success: function (data) {
                    $(DepartmentMenu.MenuClientId).empty();
                    for (var index = 0; index < data.d.results.length; index++) {
                        var department = data.d.results[index];
                        var departmentName = department.CommonName;
                        if (lcid === DepartmentMenu.VietnameseLanguageCultureId) {
                            departmentName = department[DepartmentMenu.DepartmentNameField + lcid];
                        }
                        $(DepartmentMenu.MenuClientId).append("<li id='Department" + department.Code + "li'><a href='/" + department.Code + "' item-data='/" + department.Code + "'>" + departmentName + "</a></li>");
                        DepartmentMenu.renderSubDepartments(department, lcid);
                    }
                }
            });
        } catch (e) {
            console.log(e.message);
        }
    },
    renderSubDepartments: function (parentDepartment, lcid) {
        var queryDep = "?$filter= " + DepartmentMenu.UpperDepartmentField + " eq " + parentDepartment.ID;
        var orderby = "&$orderby= " + DepartmentMenu.DepartmentNameField;
        if (lcid !== "1033") {
            orderby = "&$orderby= " + DepartmentMenu.DepartmentNameField + lcid;
        }
        $.ajax({
            url: _spPageContextInfo.siteAbsoluteUrl + "/_api/web/lists/getByTitle('" + DepartmentMenu.DepartmentList + "')/items" + queryDep + orderby,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
            cache: false,
            success: function (data) {
                var subMenuItemContent = "";
                for (var index = 0; index < data.d.results.length; index++) {
                    var subDepartment = data.d.results[index];
                    var departmentName = subDepartment.CommonName;
                    if (lcid === DepartmentMenu.VietnameseLanguageCultureId) {
                        departmentName = subDepartment[DepartmentMenu.DepartmentNameField + lcid];
                    }
                    subMenuItemContent += "<li><a href='/" + subDepartment.Code + "' item-data='/" + subDepartment.Code + "'> " + departmentName + "</a></li>";
                }
                if (subMenuItemContent !== "") {
                    subMenuItemContent = "<ul class='dropdown-menu'>" + subMenuItemContent + "</ul>";
                    $("#Department" + parentDepartment.Code + "li").addClass("dropdown-submenu");
                    $("#Department" + parentDepartment.Code + "li").append(subMenuItemContent);
                }
            }
        });
    }
};
