(function () {
    var FreightVehicleOperatorConfig = {
        FreightManagement_ViewDetail: "View Detail",
        FreightManagement_Requester: "Requester",
        FreightManagement_Department: "Department",
        FreightManagement_Bringer: "Bringer",
        FreightManagement_Receiver: "Received by",
        FreightManagement_Created: "Created",
        FreightManagement_Comment: "Comment",
        FreightManagement_ApprovalStatus: "Approval status",
        FreightManagement_RequestNumber: "Request number",
        FreightManagement_Vehicle: "Vehicle",
        ListResourceFileName: "RBVHStadaLists",
        PageResourceFileName: "RBVHStadaWebpages",
        CompanyVehicle: "Company's Vehicle",
        Locale: '',
        FreightManagement_UpdateFreightSuccess: "Update vehicle successfully!",
        Container: "freight-vehicle-operator-container",
    };

    (function () {
        var overrideCSRDepartmentCtx = {};
        overrideCSRDepartmentCtx.Templates = {};
        overrideCSRDepartmentCtx.Templates.Item = CustomItem_FreightVehicleOperator;
        overrideCSRDepartmentCtx.OnPreRender = function (ctx) {
            $('.ms-menutoolbar').hide();
        };
        overrideCSRDepartmentCtx.ListTemplateType = 10015;
        overrideCSRDepartmentCtx.BaseViewID = 7;
        overrideCSRDepartmentCtx.OnPostRender = PostRender_FreightVehicleOperator;
        overrideCSRDepartmentCtx.Templates.Header = "<div class='col-md-12'><table class='table'>" +
            "<thead><tr><th id='viewDetailTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_ViewDetail + "</th>" +
            "<th id='requestNoTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_RequestNumber + "</th>" +
            "<th id='requesterTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Requester + "</th>" +
            "<th id='departmentTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Department + "</th>" +
            "<th id='bringerTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Bringer + "</th>" +
            "<th id='receiverTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Receiver + "</th>" +
            "<th id='createdTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Created + "</th>" +
            "<th id='commentTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_Comment + "</th>" +
            "<th id='approvalStatusTH_freightOperator'>" + FreightVehicleOperatorConfig.FreightManagement_ApprovalStatus + "</th>" +
            "<th id='vehicle_freightOperator' style='min-width:80px;'>" + FreightVehicleOperatorConfig.FreightManagement_Vehicle + "</th>" +
            "<th></th>" +
            "</tr></thead><tbody>";
        overrideCSRDepartmentCtx.Templates.Footer = CreateFooter_VehicleOperator;
        SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCSRDepartmentCtx);
    })();
    function CreateFooter_VehicleOperator(ctx) {
        return ViewUtilities.Paging.InstanceHtml(ctx);
    }
    function PostRender_FreightVehicleOperator(ctx) {
        SP.SOD.executeOrDelayUntilScriptLoaded(function () {
            FreightVehicleOperatorConfig.Locale = Strings.STS.L_CurrentUICulture_Name;
            SP.SOD.registerSod(FreightVehicleOperatorConfig.ListResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightVehicleOperatorConfig.ListResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightVehicleOperatorConfig.ListResourceFileName, "Res", OnListResourcesReady_FreightVehicleOperator);
            SP.SOD.registerSod(FreightVehicleOperatorConfig.PageResourceFileName, "/_layouts/15/ScriptResx.ashx?name=" + FreightVehicleOperatorConfig.PageResourceFileName + "&culture=" + STSHtmlEncode(Strings.STS.L_CurrentUICulture_Name));
            SP.SOD.executeFunc(FreightVehicleOperatorConfig.PageResourceFileName, "Res", OnPageResourcesReady_FreightVehicleOperator);
            loadVehicleDropdown();
        }, "strings.js");

        var url = _spPageContextInfo.webAbsoluteUrl + '/_vti_bin/Services/Department/DepartmentService.svc/GetDepartments/' + _spPageContextInfo.currentUICultureName + '/' + _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
        $.ajax({
            url: url,
            method: "GET",
            async: true,
            headers: { "Accept": "application/json; odata=verbose" },
            success: function (data) {
                if (data && data.length > 0) {
                    $('#' + FreightVehicleOperatorConfig.Container + ' .department-locale').each(function () {
                        var id = $(this).attr('data-id');
                        var currentDepartment = $(this);
                        $(data).each(function (idx, obj) {
                            if (obj.Id.toString() === id) {
                                currentDepartment.text(obj.DepartmentName)
                            }
                        })
                    });
                }
            },
            error: function (data) {
                status = 'failed';
            }
        });
    }
    function OnListResourcesReady_FreightVehicleOperator() {
        $('#viewDetailTH_freightOperator').text(Res.freightManagement_ViewDetail);
        $('#requesterTH_freightOperator').text(Res.freightManagement_Requester);
        $("#requestNoTH_freightOperator").text(Res.freightManagement_RequestNumber);
        $('#departmentTH_freightOperator').text(Res.overtime_Department);
        $('#bringerTH_freightOperator').text(Res.freightManagement_Bringer);
        $('#receiverTH_freightOperator').text(Res.freightManagement_Receiver);
        $('#createdTH_freightOperator').text(Res.createdDate);
        $('#commentTH_freightOperator').text(Res.commonComment);
        $('#approvalStatusTH_freightOperator').text(Res.freightManagement_ApprovalStatus);
        $('#vehicle_freightOperator').text(Res.freightManagement_Vehicle);
        $('#' + FreightVehicleOperatorConfig.Container + ' .viewDetail').text(Res.freightManagement_ViewDetail);
        $('.label-success').text(Res.approvalStatus_Approved);
        $('.label-warning').text(Res.approvalStatus_Cancelled);
        $('.label-danger').text(Res.approvalStatus_Rejected);
        FreightVehicleOperatorConfig.CompanyVehicle = Res.freightManagement_CompanyVehicle;
        $('.companyvehicle').text(FreightVehicleOperatorConfig.CompanyVehicle);
    }
    function OnPageResourcesReady_FreightVehicleOperator() {
        FreightVehicleOperatorConfig.FreightManagement_UpdateFreightSuccess = Res.freightManagement_UpdateVehicleSuccess;
    }
    function loadVehicleDropdown() {
        var lcid = SP.Res.lcid;
        var vehicleTDs = $("td[name^='vehicleTd_']");
        if (_rbvhContext && _rbvhContext.EmployeeInfo) {
            var serviceUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetVehicleOperatorInfo/" + _rbvhContext.EmployeeInfo.ID;
            var getDataPromise = $.ajax({
                url: serviceUrl,
                method: "GET",
                async: false,
                headers: { "Accept": "application/json; odata=verbose" },
            });
            getDataPromise.done(
                function (data) {
                    if (data) {
                        var isHasEditPermission = data.HasPermission;
                        if (vehicleTDs && vehicleTDs.length > 0) {
                            for (var i = 0; i < vehicleTDs.length; i++) {
                                var item = vehicleTDs[i];
                                var vehicleArray = [];
                                var id = $(item).attr("itemId");
                                var selectedId = $(item).attr("selectedVehicleId");
                                var dropdownHtml = getVehicleDropdown(id, data.FreightVehicles, selectedId);
                                if (isHasEditPermission == true) {
                                    $(item).append(dropdownHtml[0]);
                                    $(dropdownHtml[1]).insertAfter(item);
                                }
                                else {
                                    $(item).append(vehicleValue);
                                }
                            }
                        }
                        if (isHasEditPermission == true) {
                            setSelectedVehicle();
                        }
                    }
                    updateFreightEventRegister();
                },
                function (error) {
                    console.log("Error");
                }
            );
        }
    }
    function getVehicleDropdown(id, dataArray, selectedId) {
        var lcid = SP.Res.lcid;
        var dropdownName = ("ddVehicle_" + id);
        var buttonName = ("button_" + id);
        var controlHtml = '<select style="width: 145px; display: inline;" name="' + dropdownName + '" selectedVehicleId="' + selectedId + '" itemId="' + id + '">';
        controlHtml += '<option vehiclevaluevn="" value="0" vehiclevalue="">(None/Không có)</option>';
        if (dataArray && dataArray.length > 0) {
            for (var i = 0; i < dataArray.length; i++) {
                var textLabel = dataArray[i].Vehicle;
                if (lcid == 1066) {
                    textLabel = dataArray[i].VehicleVN;
                }
                controlHtml += ('<option vehicleLookupId="' + dataArray[i].ID + '" vehiclevaluevn="' + dataArray[i].VehicleVN + '" vehiclevalue="' + dataArray[i].Vehicle + '" value="' + dataArray[i].ID + '">' + textLabel + '</option>');
            }
        }
        controlHtml += '</select>';
        var actionButtonHtml = '<td><button class="btn btn-primary update-Freight" style="display: inline; margin-left: 5px; font-size: smaller" name="' + buttonName + '" ddName="' + dropdownName + '" type="button">OK</button></td>';
        return [controlHtml, actionButtonHtml];
    }
    function setSelectedVehicle() {
        var vehicleDropdowns = $("select[name^='ddVehicle_']");
        if (vehicleDropdowns && vehicleDropdowns.length > 0) {
            for (var i = 0; i < vehicleDropdowns.length; i++) {
                var item = vehicleDropdowns[i];
                var selectedVehicleValue = $(item).attr("selectedVehicleId");
                if (selectedVehicleValue) {
                    $(item).val(selectedVehicleValue.trim()).change();
                }
            }
        }
    }
    function updateFreightEventRegister() {
        $(".update-Freight").on("click", function () {
            var item = $(this);
            var dropdownName = item.attr("ddName");
            var itemId = $("select[name='" + dropdownName + "']").attr("itemId");
            var selectedOption = $("select[name='" + dropdownName + "'] option:selected");
            var selectedValueVn = selectedOption.attr("vehiclevaluevn");
            var selectedValue = selectedOption.attr("vehiclevalue");
            var vehicleLookupId = selectedOption.attr("value");
            if (itemId != undefined && selectedValueVn != undefined && selectedValue != undefined) {
                var updateVehicleServiceUrl = _spPageContextInfo.webAbsoluteUrl + "/_vti_bin/Services/FreightManagement/FreightManagementService.svc/UpdateFreightVehicle";
                var itemData = {};

                itemData.Id = parseInt(itemId);

                var vehicleId = (vehicleLookupId > 0 && vehicleLookupId.length > 0) ? parseInt(vehicleLookupId) : 0;

                itemData.VehicleLookup = { LookupId: parseInt(vehicleId), LookupValue: selectedValue };
                itemData.VehicleVN = { LookupId: parseInt(vehicleId), LookupValue: selectedValueVn };

                $.ajax({
                    url: updateVehicleServiceUrl,
                    method: "POST",
                    async: false,
                    data: JSON.stringify(itemData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data.Code == 0) {
                            alert(FreightVehicleOperatorConfig.FreightManagement_UpdateFreightSuccess);
                        }
                    },
                    error: function (data) {
                        console.log("Update vehicle failed");
                    }
                });
            }
        });
    }
    function CustomItem_FreightVehicleOperator(ctx) {
        var tr = "";
        var requester = '<td>' + ctx.CurrentItem.Requester[0].lookupValue + '</td>';
        var requestNo = ctx.CurrentItem.RequestNo != null ? '<td>' + ctx.CurrentItem.RequestNo + '</td>' : '<td></td>';
        var department = '<td class="department-locale" data-id="' + ctx.CurrentItem.CommonDepartment[0].lookupId + '">' + ctx.CurrentItem.CommonDepartment[0].lookupValue + '</td>';
        var bringer = '<td></td>';
        if (ctx.CurrentItem.Bringer && ctx.CurrentItem.Bringer[0].lookupId > 0) {
            bringer = '<td>' + ctx.CurrentItem.Bringer[0].lookupValue + '</td>';
        }
        else if (ctx.CurrentItem.BringerName && ctx.CurrentItem.BringerName.length > 0) {
            bringer = '<td>' + ctx.CurrentItem.BringerName + '</td>';
        }
        else {
            bringer = '<td class="companyvehicle">' + FreightVehicleOperatorConfig.CompanyVehicle + '</td>';
        }
        var receiver = ctx.CurrentItem.Receiver != null ? '<td>' + ctx.CurrentItem.Receiver + '</td>' : '<td></td>';
        var created = '<td>' + ctx.CurrentItem.Created + '</td>';
        var comment = ctx.CurrentItem.CommonComment != null ? '<td>' + Functions.parseComment(ctx.CurrentItem.CommonComment) + '</td>' : '<td></td>';

        var sourceURL = window.location.href.split('#')[0];
        sourceURL += '#tab5';
        sourceURL = encodeURIComponent(sourceURL);
        var title = '<td><a href="/SitePages/FreightRequest.aspx?subSection=FreightManagement&itemId=' + ctx.CurrentItem.ID + '&Source=' + sourceURL + '"   class="viewDetail" \>View Detail</a></td>';

        var status = ctx.CurrentItem.ApprovalStatus + "";
        status = status.toLowerCase();
        var statusVal = '';
        if (status == 'approved') {
            statusVal = '<td><span class="label label-success">Approved</span></td>';
        }
        else if (status == "cancelled") {
            statusVal = '<td><span class="label label-warning">Cancelled</span></td>';
        }
        else if (status == "rejected") {
            statusVal = '<td><span class="label label-danger">Rejected</span></td>';
        }
        else if (status && status.length > 0) {
            statusVal = '<td><span class="label label-default">' + ctx.CurrentItem.ApprovalStatus + '</span></td>';
        }
        else {
            statusVal = '<td><span class="label label-default">In-Progress</span></td>';
        }
        var vehicleLookupId = "0";
        if (ctx.CurrentItem.VehicleLookup && ctx.CurrentItem.VehicleLookup[0]) {
            vehicleLookupId = ctx.CurrentItem.VehicleLookup[0].lookupId;
        }

        var vehicle = '<td name="vehicleTd_' + ctx.CurrentItem.ID + '" itemId="' + ctx.CurrentItem.ID + '" selectedVehicleId="' + vehicleLookupId + '" ></td>';
        tr = "<tr>" + title + requestNo + requester + department + bringer + receiver + created + comment + statusVal + vehicle + "</tr>";
        return tr;
    }
})();