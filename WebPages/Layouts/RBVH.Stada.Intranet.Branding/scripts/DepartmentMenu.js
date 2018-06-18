var DepartmentMenu = {
    DepartmentList: "Departments",
    DepartmentNameField: "CommonName",
    VietnameseLanguageCultureId: 1066,
    MenuClientId: "#DepartmentMenuul",
    renderDepartments: function () {
        var lcid = _spPageContextInfo.currentLanguage;
        var orderby = "?$orderby= " + DepartmentMenu.DepartmentNameField;
        if (lcid === DepartmentMenu.VietnameseLanguageCultureId) {
            orderby = "?$orderby= " + DepartmentMenu.DepartmentNameField + lcid;
        }
        var url = _spPageContextInfo.siteAbsoluteUrl + "/_api/web/lists/getByTitle('" + DepartmentMenu.DepartmentList + "')/items" + orderby;// + query + orderby;
        try {
            $.ajax({
                url: url,
                method: "GET",
                headers: { "Accept": "application/json; odata=verbose" },
                cache: true,
                async: true,
                success: function (data) {
                    $(DepartmentMenu.MenuClientId).empty();
                    if (data && data.d && data.d.results) {
                        var departmentResults = data.d.results;
                        var parentDepartmentList = [];
                        if (departmentResults && departmentResults.length > 0) {
                            var parentDepartmentList = [];
                            var childDepartmentList = [];

                            for (var index1 = 0; index1 < departmentResults.length; index1++) {
                                if (typeof departmentResults[index1].IsVisible == 'undefined' || departmentResults[index1].IsVisible == true) {
                                    if (departmentResults[index1].UpperDepartmentId == null) {
                                        parentDepartmentList.push(departmentResults[index1]);
                                    }
                                    else {
                                        childDepartmentList.push(departmentResults[index1]);
                                    }
                                }
                            }
                            for (var index = 0; index < parentDepartmentList.length; index++) {
                                var department = parentDepartmentList[index];
                                var departmentName = department.CommonName;
                                if (lcid === DepartmentMenu.VietnameseLanguageCultureId) {
                                    departmentName = department[DepartmentMenu.DepartmentNameField + lcid];
                                }
                                $(DepartmentMenu.MenuClientId).append("<li id='Department" + department.Code + "li'><a href='/" + department.Code + "' item-data='/" + department.Code + "'>" + departmentName + "</a></li>");
                                DepartmentMenu.renderSubDepartments(childDepartmentList, department, lcid);
                            }
                        }
                    }
                }
            });
        } catch (e) {
            //console.log(e.message);
        }
    },
    renderSubDepartments: function (childDepartmentList, parentDepartment, lcid) {
        var childrenList = [];
        for (var index = 0; index < childDepartmentList.length; index++) {
            if (parentDepartment.ID === childDepartmentList[index].UpperDepartmentId) {
                childrenList.push(childDepartmentList[index]);
            }
        }
        var subMenuItemContent = "";
        for (var index = 0; index < childrenList.length; index++) {
            var subDepartment = childrenList[index];
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
};
