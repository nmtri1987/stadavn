RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.FreightRequest = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        ServiceUrls:
        {
            GetBringerList: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetBringerList/{1}/{2}',
            GetApprovers: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetApprovers/{1}/{2}',
            GetApprovalPermission: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/HasApprovalPermission/{1}',
            GetDelegatedTaskInfo: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetDelegatedTaskInfo/{1}',
            ValidateSumitTime: this.Protocol + '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/ValidateSumitTime',
            LoadFreightRequest: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetFreightManagementById/{1}/{2}',
            InsertFreightRequest: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/InsertFreight',
            ApproveFreightRequest: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/ApproveFreight',
            RejectFreightRequest: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/RejectFreight',
            SecurityUpdateFreightRequest: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/SecurityUpdateFreight',
            ExportFreight: this.Protocol + '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/ExportFreight',
            GetFreightDepartmentList: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetAllReceiverDepartment',
            IsSecurity: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/IsSecurity',
            GetTaskHistoryInfo: '//{0}/_vti_bin/Services/FreightManagement/FreightManagementService.svc/GetTaskHistory/{1}/{2}',
            GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations'
        },
        Grid:
        {
            CustomFields: {
                HiddenIDField: null,
                HiddenShippingInField: null,
                HiddenShippingOutField: null,
                OrderNumberField: null,
                GoodsDescriptionField: null,
                UnitField: null,
                QuantityField: null,
                RemarksField: null,
                IsShippingInField: null,
                IsShippingOutField: null
            }
        },

        Data: {},
        DelegatedTaskInfo: {},
        Configurations: {},
        ConfigKey_ValidTransportDate: "FreightForm_ValidTransportDate",
        GridJsonArray: [],
        SelectedBringerType: "0", //belongs to company; default is 0

        CurrentEmployeeID: '',
        CurrentEmployeeLookupId: '',
        CurrentEmployeeDepartmentID: '',
        CurrentEmployeeLocationID: '',
        CurrentEmployeeLevel: 0,
        CurrentEmployeeFullName: '',
        CurrentLcid: 1033,

        ApproverDH: {},
        ApproverBOD: {},
        ApproverHeadofAdminDept: {},
        IsViewMode: false,
        FreightManagementId: 0,
        SelectedReasonType: "0",
        FreightManagement_ListTitle: "Freight Management",
        DepartmentArray: [],
        IsSecurityGuard: false,
        ShippingInIdsArray: [],
        ShippingOutIdsArray: []
    },
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.FreightRequest.prototype =
{
    Initialize: function () {
        var that = this;
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.Settings.CurrentLcid = SP.Res.lcid;
            that.InitControls();
            that.EventsRegister();
            that.TryLoadInformation();
            that.PopulateData();
        }, "sp.js", "strings.js");
    },
    GetConfigurations: function () {
        var that = this;
        var postData = [that.Settings.ConfigKey_ValidTransportDate];
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetConfigurations, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && response.length > 0) {
                that.Settings.Configurations = response;
            }
        });
    },
    InitControls: function () {
        var that = this;
        that.SetCurrentEmployeeInfo();
        that.ResetForm();
        that.LoadBringerRelatedControl();
        that.LoadReasonRelatedControl();
        $(that.Settings.Controls.DateSelector).keydown(function (e) { e.preventDefault(); return false; });
    },
    CheckSecurityGuard: function () {
        var that = this;
        var urlCheckSecurity = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.IsSecurity, location.host);
        $.ajax({
            type: "GET",
            url: urlCheckSecurity,
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            if (data && data === true) {
                that.Settings.IsSecurityGuard = true;
                that.ShowObjectForSecurity();
            }
        });
    },
    SetCurrentEmployeeInfo: function () {
        var that = this;
        if (_rbvhContext && _rbvhContext.EmployeeInfo) {
            this.Settings.CurrentEmployeeID = _rbvhContext.EmployeeInfo.EmployeeID;
            this.Settings.CurrentEmployeeFullName = _rbvhContext.EmployeeInfo.FullName;
            this.Settings.CurrentEmployeeLocationID = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
            this.Settings.CurrentEmployeeDepartmentID = (_rbvhContext && _rbvhContext.EmployeeInfo && _rbvhContext.EmployeeInfo.Department) ? _rbvhContext.EmployeeInfo.Department.LookupId : 0;
            this.Settings.CurrentEmployeeLookupId = _rbvhContext.EmployeeInfo.ID; //default is current requester
            this.Settings.CurrentEmployeeLevel = parseFloat(_rbvhContext.EmployeeInfo.EmployeeLevel.LookupValue);
        }
    },
    LoadBringerRelatedControl: function () {
        var that = this;
        if (that.Settings.SelectedBringerType === "0") { //employee is employee of the company
            $("#interEmpl").show();
            $("#extEmpl").hide();
        }
        else if (that.Settings.SelectedBringerType === "1") { //company's vehicle
            $("#interEmpl").hide();
            $("#extEmpl").hide();
        }
        else {
            $("#interEmpl").hide();
            $("#extEmpl").show();
        }
        $(that.Settings.Controls.SelectBringerSelector).select2({ width: '100%' });
    },
    LoadReasonRelatedControl: function () {
        var that = this;
        if (that.Settings.SelectedReasonType === "0") {
            $(".sendGoodsTr").show();
            $(".othersTr").hide();
        }
        else if (that.Settings.SelectedReasonType === "1") {
            $(".sendGoodsTr").hide();
            $(".othersTr").show();
        }
    },
    LoadEmployeeList: function (departmentId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetBringerList, location.host, departmentId, _rbvhContext.EmployeeInfo.FactoryLocation.LookupId);
        $.ajax({
            type: "GET",
            url: url,
            //cache: false,
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            if (data) {
                $(that.Settings.Controls.SelectBringerSelector).empty();
                for (var i = 0; i < data.length; i++) {
                    $(that.Settings.Controls.SelectBringerSelector).append($("<option>").attr('value', data[i].ID).attr("EmployeeId", data[i].EmployeeId).attr("DepartmentName", data[i].DepartmentName).attr("DepartmentId", data[i].DepartmentId).attr("LocationId", data[i].LocationId).text(data[i].FullName));
                }

                if (that.Settings.Data && that.Settings.Data.Bringer && !!that.Settings.Data.Bringer.LookupId) {
                    $(that.Settings.Controls.SelectBringerSelector).val(that.Settings.Data.Bringer.LookupId.toString()).change();
                }
                else {
                    that.ReloadEmployeeData();
                }
            }
        });
    },
    LoadApprovers: function (departmentId, locationId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApprovers, location.host, departmentId, locationId);
        $.ajax({
            type: "GET",
            url: url,
            //cache: false,
            async: true,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (data) {
            if (data) {
                that.Settings.ApproverDH = data.Approver1;
                that.Settings.ApproverBOD = data.Approver2;
                that.Settings.ApproverHeadofAdminDept = data.Approver3;
            }
        });
    },
    LoadFreightDepartment: function () {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetFreightDepartmentList, location.host);
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: true,
            success: function (result) {
                that.Settings.DepartmentArray = result;
                var departmentFieldName = _spPageContextInfo.currentLanguage == '1066' ? 'ReceiverDepartmentVN' : 'ReceiverDepartment';
                var departmentStringArray = $.map(that.Settings.DepartmentArray, function (val, i) {
                    return val[departmentFieldName];
                });
                $(that.Settings.Controls.TxtReceiverDepartment).autocomplete({
                    source: departmentStringArray,
                });
            }
        });
    },
    ResetForm: function () {
        var that = this;
        $("input[type='radio'][name='bringerType']:first").prop("checked", "checked");
        $("input[type='radio'][name='radioReason']:first").prop("checked", "checked");
        $("input[type='radio'][name='freightType']:first").prop("checked", "checked");
        $("input[type='radio'][name='returnGoods']:last").prop("checked", "checked");
        $(that.Settings.Controls.CkbHighPriority).prop("checked", false);
        $(that.Settings.Controls.TxtBringerSelector).val("");
        $(that.Settings.Controls.TxtCompanyNameSelector).val("");
        $(that.Settings.Controls.TxtReason).val("");
        $(that.Settings.Controls.TxtReceivedBy).val("");
        $(that.Settings.Controls.TxtReceiverDepartment).val("");
        $(that.Settings.Controls.TxtPhoneContact).val("");
        $(that.Settings.Controls.TxtOthersReason).val("");
        $(that.Settings.Controls.DateSelector).val("");
        $(that.Settings.Controls.HoursSelector).val("00:");
        $(that.Settings.Controls.MinutesSelector).val("00");
    },
    EventsRegister: function () {
        var that = this;
        $("input[type='radio'][name='bringerType']").click(function () {
            var selectRadioVal = $(this).val();
            that.Settings.SelectedBringerType = selectRadioVal;
            that.LoadBringerRelatedControl();
        });
        $("input[type='radio'][name='radioReason']").click(function () {
            var selectRadioVal = $(this).val();
            that.Settings.SelectedReasonType = selectRadioVal;
            that.LoadReasonRelatedControl();
        });
        $(that.Settings.Controls.SelectBringerSelector).change(function () {
            that.ReloadEmployeeData();
        });
        $(that.Settings.Controls.ButtonSaveSelector).on('click', function (event) {
            event.preventDefault();
            that.DoSubmitData(event);
        });
        $(that.Settings.Controls.ButtonCancelSelector).on('click', function (event) {
            event.preventDefault();
            Functions.redirectToSource();
        });
        $(that.Settings.Controls.ButtonPrint).on('click', function (event) {
            event.preventDefault();
            that.ExportFreight();
            return false;
        });
        $(that.Settings.Controls.ButtonApprove).on('click', function (event) {
            event.preventDefault();
            that.DoApproveFreight(event);
        });
        $(that.Settings.Controls.ButtonReject).on('click', function (event) {
            event.preventDefault();
            that.DoRejectFreight(event);
        });

        $(that.Settings.Controls.ButtonUpdate).on('click', function (event) {
            event.preventDefault();
            that.Update();
        });
        $(that.Settings.Controls.ButtonSecReject).on('click', function (event) {
            event.preventDefault();
            that.SecurityReject();
        });
        $(that.Settings.Controls.ApprovalHistoryButtonSelector).on('click', function (event) {
            event.preventDefault();
            var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetTaskHistoryInfo, location.host, itemId, 0);
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                async: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            }).done(function (response) {
                var tableHeaders = [that.Settings.ResourceText.ApprovalStatusTitle, that.Settings.ResourceText.PostedByTitle, that.Settings.ResourceText.DateTitle, that.Settings.ResourceText.CommentTitle];
                var approvalStatus = [that.Settings.ResourceText.ApprovalStatus_Approved, that.Settings.ResourceText.ApprovalStatus_Rejected]
                var approvalHistoryTable = Functions.generateApprovalHistoryTable(response, tableHeaders, approvalStatus, that.Settings.ResourceText.NoDataAvaibleMsg);
                $(that.Settings.Controls.ApprovalHistoryContainerSelector).html(approvalHistoryTable);
            });
        });
    },
    ExportFreight: function () {
        var that = this;
        var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (itemId && itemId > 0) {
            url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ExportFreight, location.host) + "/" + itemId;
            window.location = url;
        }
    },
    Update: function () {
        var that = this;
        that.ShowOrHideErrorMessage($(that.Settings.Controls.SecurityNotes_Error), "");
        $(that.Settings.Controls.ButtonUpdate).prop('disabled', true);
        var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (itemId && itemId > 0) {
            var postData = {};
            postData.Id = itemId;
            postData.ActionNo = 1;
            postData.CheckInFreightIds = that.Settings.ShippingInIdsArray;
            postData.CheckOutFreightIds = that.Settings.ShippingOutIdsArray;
            postData.UpdateById = _rbvhContext.EmployeeInfo.ID;
            postData.UpdateByName = _rbvhContext.EmployeeInfo.FullName;
            postData.SecurityNotes = $(that.Settings.Controls.TxtSecurityNotes).val();
            that.UpdateTime(postData);
        }
        else {
            $(that.Settings.Controls.ButtonUpdate).prop('disabled', false);
        }
    },
    SecurityReject: function () {
        var that = this;
        that.ShowOrHideErrorMessage($(that.Settings.Controls.SecurityNotes_Error), "");
        $(that.Settings.Controls.ButtonSecReject).prop('disabled', true);
        var itemId = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (itemId && itemId > 0) {
            if ($(that.Settings.Controls.TxtSecurityNotes).val().length > 0) {
                var postData = {};
                postData.Id = itemId;
                postData.ActionNo = 3;
                postData.UpdateById = _rbvhContext.EmployeeInfo.ID;
                postData.UpdateByName = _rbvhContext.EmployeeInfo.FullName;
                postData.SecurityNotes = $(that.Settings.Controls.TxtSecurityNotes).val();
                that.UpdateTime(postData);
            } else {
                that.ShowOrHideErrorMessage($(that.Settings.Controls.SecurityNotes_Error), that.Settings.ResourceText.CantLeaveTheBlank);
                $(that.Settings.Controls.ButtonSecReject).prop('disabled', false);
            }
        }
        else {
            $(that.Settings.Controls.ButtonSecReject).prop('disabled', false);
        }
    },
    UpdateTime: function (postData) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.SecurityUpdateFreightRequest, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && response.Code === 0) {
                Functions.redirectToSource();
            } else if (response.Code === 4) {
                alert(response.Message);
                window.location.reload();
            }
        });
    },
    ReloadEmployeeData: function () {
        var that = this;
        var selectedOption = $(that.Settings.Controls.SelectBringerSelector + " option:selected");
        var employeeId = selectedOption.attr("EmployeeId");
        $(that.Settings.Controls.SpanEmployeeIDSelector).text(employeeId);
        var departmentName = selectedOption.attr("DepartmentName");
        $(that.Settings.Controls.SpanDepartmentSelector).text(departmentName);
    },
    TryLoadInformation: function () {
        var that = this;
        var id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (id) {
            $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").show();
            that.Settings.FreightManagementId = parseInt(id);
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.LoadFreightRequest, location.host, id, "true");
            $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                dataType: "json",
            }).done(function (data) {
                if (data.Code > 0) {
                    console.log(data.Message);
                    return;
                }

                that.Settings.Data = data;
                that.DisableControls(true);

                that.LoadEmployeeList(data.Department.LookupId);
                //that.LoadApprovers(data.Department.LookupId, data.Location.LookupId);
                if (that.Settings.CurrentEmployeeLookupId === data.Requester.LookupId) {
                    if ((data.ModifiedBy.ID === data.CreatedBy.ID && data.ApprovalStatus && data.ApprovalStatus.toLowerCase() !== 'cancelled') // submited
                        || (data.ModifiedBy.ID !== data.CreatedBy.ID && data.ApprovalStatus && data.ApprovalStatus.toLowerCase() === 'rejected')) { // rejected/reopened
                        that.DisableControls(false);
                    }

                    if (data.ModifiedBy.ID !== data.CreatedBy.ID && data.ApprovalStatus && data.ApprovalStatus.toLowerCase() === 'rejected') {
                        $(that.Settings.Controls.TxtOldComment).closest("tr").show();
                        $(that.Settings.Controls.TxtOldSecurityNotes).closest("tr").show();
                    }
                }

                if (data.CreatedBy.ID === _rbvhContext.EmployeeInfo.ADAccount.ID && data.ModifiedBy.ID !== data.CreatedBy.ID && data.ApprovalStatus &&
                    (data.ApprovalStatus.toLowerCase() !== 'cancelled' && data.ApprovalStatus.toLowerCase() !== 'rejected')) {
                    $(that.Settings.Controls.ButtonPrint).show();
                }

                if (data && data.Id > 0) {
                    that.GetApprovalPermission(data.Id)
                        .then(function (result) {
                            if (result && result === true) {
                                if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                    errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                    errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                    RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                                }
                                else {
                                    that.ShowControlForApprover(true);
                                }
                            }
                            else {
                                that.GetDelegatedTaskInfo(data.Id)
                                    .then(function (respData) {
                                        if (respData && respData.Requester.LookupId > 0) {
                                            if (that.Settings.Data && that.Settings.Data.RequestExpired == true) {
                                                errMsg = decodeURI(that.Settings.ResourceText.RequestExpiredMsgFormat);
                                                errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.Data.RequestDueDate);
                                                RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.Controls.ErrorMsgContainerSelector, that.Settings.Controls.ErrorMsgSelector, errMsg);
                                            }
                                            else {
                                                that.Settings.DelegatedTaskInfo = respData;
                                                that.ShowControlForApprover(true);
                                            }
                                        }
                                        else {
                                            that.ShowControlForApprover(false);
                                        }
                                    });
                            }
                        });
                }

                if (!!data.Bringer.LookupId) {
                    $(that.Settings.Controls.SelectBringerSelector).val(data.Bringer.LookupId.toString()).change();
                }

                if (data.Bringer.LookupId > 0) {
                    $('input[name="bringerType"][value="' + 0 + '"]').prop('checked', true);
                    that.Settings.SelectedBringerType = "0";
                }
                else if (data.CompanyVehicle === true) {
                    $('input[name="bringerType"][value="' + 1 + '"]').prop('checked', true);
                    that.Settings.SelectedBringerType = "1";
                }
                else {
                    $('input[name="bringerType"][value="' + 2 + '"]').prop('checked', true);
                    that.Settings.SelectedBringerType = "2";
                }
                that.LoadBringerRelatedControl();

                $(that.Settings.Controls.TxtReason).val(data.Reason);

                if (!!data.OtherReason && data.OtherReason.length > 0) {
                    $(that.Settings.Controls.RDOtherReason).prop('checked', true);
                    $(that.Settings.Controls.RDOtherReason).click();
                }
                else {
                    $(that.Settings.Controls.RDSendGoods).prop('checked', true);
                    $(that.Settings.Controls.RDSendGoods).click();
                }

                if (data.BringerName) {
                    $(that.Settings.Controls.TxtBringerSelector).val(data.BringerName);
                }
                if (data.CompanyName) {
                    $(that.Settings.Controls.TxtCompanyNameSelector).val(data.CompanyName);
                }

                $(that.Settings.Controls.TxtReceivedBy).val(data.Receiver);
                var receiverDepartmentValue = data.ReceiverDepartmentText;
                if (_spPageContextInfo.currentLanguage === '1066') { // vi-VN
                    // check ReceiverDepartmentVN object
                    if (data.ReceiverDepartmentVN != null && data.ReceiverDepartmentVN.LookupValue != null)
                        receiverDepartmentValue = data.ReceiverDepartmentVN.LookupValue;
                }
                else {
                    // check ReceiverDepartmentLookup object
                    if (data.ReceiverDepartmentLookup != null && data.ReceiverDepartmentLookup.LookupValue != null)
                        receiverDepartmentValue = data.ReceiverDepartmentLookup.LookupValue;
                }
                _spPageContextInfo.currentLanguage === '1066' ? 'VehicleVN' : 'Vehicle';
                $(that.Settings.Controls.TxtReceiverDepartment).val(receiverDepartmentValue);
                $(that.Settings.Controls.TxtPhoneContact).val(data.ReceiverPhone);

                $(that.Settings.Controls.TxtOthersReason).val(data.OtherReason);
                $(that.Settings.Controls.TxtOldComment).html(Functions.parseComment(data.Comment));
                $(that.Settings.Controls.TxtOldSecurityNotes).html(Functions.parseComment(data.SecurityNotes));
                if (data.HighPriority && data.HighPriority === true) {
                    $(that.Settings.Controls.CkbHighPriority).prop("checked", true);
                }
                else {
                    $(that.Settings.Controls.CkbHighPriority).prop("checked", false);
                }
                var freightTypeVal = (data.FreightType === true) ? "Yes" : "No";
                $('input[name="freightType"][value="' + freightTypeVal + '"]').prop('checked', true);

                var returnGoodsValue = (data.ReturnedGoods === true) ? "Yes" : "No";
                $('input[name="returnGoods"][value="' + returnGoodsValue + '"]').prop('checked', true);

                if (data.ApprovalStatus && data.ApprovalStatus.length > 0) {
                    $(that.Settings.Controls.ApprovalStatusValSelector).html(RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus(data.ApprovalStatus));
                    $(that.Settings.Controls.ApprovalStatusValSelector).closest('tr').show();
                }

                var dateString = data.DateString;
                $(that.Settings.Controls.DateSelector).val(dateString);
                var hour = Functions.padDate(data.Hour) + ":";
                var minute = Functions.padDate(data.Minute);
                $(that.Settings.Controls.HourSelector).val(hour.toString()).change();
                $(that.Settings.Controls.MinuteSelector).val(minute.toString()).change();
                var detailData = data.FreightDetails;
                if (detailData && detailData.length > 0) {
                    for (var i = 0; i < detailData.length; i++) {
                        var item = {};
                        item.Id = detailData[i].Id;
                        item.GoodsDescription = detailData[i].GoodsName;
                        item.Unit = detailData[i].Unit;
                        item.Remarks = detailData[i].Remarks;
                        item.Quantity = detailData[i].Quantity;
                        item.IsShippingIn = detailData[i].IsShippingIn;
                        item.IsShippingOut = detailData[i].IsShippingOut;

                        item.ShippingInBy = detailData[i].ShippingInBy;
                        item.ShippingOutBy = detailData[i].ShippingOutBy;
                        item.ShippingInTime = detailData[i].ShippingInTime;
                        item.ShippingOutTime = detailData[i].ShippingOutTime;
                        that.Settings.GridJsonArray.push(item);
                    }
                }

                that.CheckSecurityGuard();
            });
        } else {
            $(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").hide();
            that.LoadEmployeeList(that.Settings.CurrentEmployeeDepartmentID);
            //that.LoadApprovers(that.Settings.CurrentEmployeeDepartmentID, that.Settings.CurrentEmployeeLocationID);
        }
    },
    GetApprovalPermission: function (freightId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetApprovalPermission, location.host, freightId);
        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    },
    GetDelegatedTaskInfo: function (itemId) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetDelegatedTaskInfo, location.host, itemId);
        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            async: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    },
    ShowControlForApprover: function (hasApprovalPermission) {
        var that = this;
        if (hasApprovalPermission === true) {
            $(that.Settings.Controls.ButtonApprove).show();
            $(that.Settings.Controls.ButtonReject).show();
            that.ShowHighPriority();
            that.ShowComment();
            //$(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").show();
        }
        else {
            $(that.Settings.Controls.ButtonApprove).hide();
            $(that.Settings.Controls.ButtonReject).hide();
            //$(that.Settings.Controls.ApprovalHistoryButtonSelector).closest("div").hide();
            if (that.Settings.Data.Comment && that.Settings.Data.Comment.length > 0) {
                $(that.Settings.Controls.TxtOldComment).closest("tr").show();
            }
            if (that.Settings.Data.SecurityNotes && that.Settings.Data.SecurityNotes.length > 0) {
                $(that.Settings.Controls.TxtOldSecurityNotes).closest("tr").show();
            }
        }
    },
    ShowHighPriority: function () {
        var that = this;
        if ((that.Settings.CurrentEmployeeLevel === 5 && _rbvhContext.EmployeeInfo.ADAccount.ID === that.Settings.Data.DH.ID) || // department head
            (that.Settings.DelegatedTaskInfo.FromEmployee && that.Settings.DelegatedTaskInfo.FromEmployee.ID === that.Settings.Data.DH.ID)) { // delegated person
            $(".highPriorityTr").show();
            $(that.Settings.Controls.CkbHighPriority).prop("disabled", false);
        }
        else {
            $(".highPriorityTr").hide();
            $(that.Settings.Controls.CkbHighPriority).prop("disabled", true);
        }
    },
    ShowComment: function () {
        var that = this;
        if (that.Settings.Data.Comment && that.Settings.Data.Comment.length > 0) {
            $(that.Settings.Controls.TxtOldComment).closest("tr").show();
        }
        $(that.Settings.Controls.TxtComment).closest("tr").show();
        $(that.Settings.Controls.TxtComment).prop("disabled", false);
    },
    ShowObjectForSecurity: function () {
        var that = this;
        $(that.Settings.Controls.TxtComment).closest("tr").hide();
        $(that.Settings.Controls.TxtOldComment).closest("tr").hide();
        if ($(that.Settings.Controls.TxtOldSecurityNotes).val().length > 0) {
            $(that.Settings.Controls.TxtOldSecurityNotes).closest("tr").show();
        }
        $(that.Settings.Controls.TxtSecurityNotes).prop("disabled", false);
        $(that.Settings.Controls.TxtSecurityNotes).closest("tr").show();
        if (that.Settings.Data.IsFinished == true) {
            $(that.Settings.Controls.ButtonUpdate).prop("disabled", true);
        }
        $(that.Settings.Controls.ButtonSecReject).show();
        $(that.Settings.Controls.ButtonUpdate).show();
    },
    DisableControls: function (value) {
        var that = this;
        that.Settings.IsViewMode = value;
        $('input[name="bringerType"]').prop("disabled", value);

        if (value === true) {
            $(that.Settings.Controls.SelectBringerSelector).select2('destroy');
        }
        $(that.Settings.Controls.SelectBringerSelector).prop("disabled", value);

        $(that.Settings.Controls.TxtBringerSelector).prop("disabled", value);
        $(that.Settings.Controls.TxtCompanyNameSelector).prop("disabled", value);
        $(that.Settings.Controls.TxtReason).prop("disabled", value);
        $('input[name="radioReason"]').prop("disabled", value);

        $(that.Settings.Controls.TxtReceivedBy).prop("disabled", value);
        $(that.Settings.Controls.TxtReceiverDepartment).prop("disabled", value);
        $(that.Settings.Controls.TxtPhoneContact).prop("disabled", value);
        $(that.Settings.Controls.TxtOthersReason).prop("disabled", value);
        if (value === true) {
            $("#ctl00_PlaceHolderMain_dtDate_dtDateDateDatePickerImage").hide();
        }
        else {
            $("#ctl00_PlaceHolderMain_dtDate_dtDateDateDatePickerImage").show();
        }
        $(that.Settings.Controls.DateSelector).prop("disabled", value);
        $(that.Settings.Controls.HourSelector).prop("disabled", value);
        $(that.Settings.Controls.MinuteSelector).prop("disabled", value);
        $('input[name="freightType"]').prop("disabled", value);
        $('input[name="returnGoods"]').prop('disabled', value);
        $(that.Settings.Controls.CkbHighPriority).prop("disabled", value);
        $(that.Settings.Controls.ButtonSaveSelector).prop("disabled", value);
    },
    PopulateData: function () {
        var that = this;
        that.GetConfigurations();
        that.LoadFreightDepartment();
        that.PopulateGrid();
        that.DisableShippingCheckboxes();
        that.EventsRegisterForCheckBoxs();
    },
    EventsRegisterForCheckBoxs: function () {
        var that = this;
        $("input[type='checkbox'][name='chbox-check-in']").on("change", function () {
            var $idElement = $(this).parent().siblings(":first");
            if ($idElement && $idElement.text() && $idElement.text().length > 0) {
                if ($(this).is(":checked") === true) {
                    that.Settings.ShippingInIdsArray.push($idElement.text())
                }
                else {
                    that.Settings.ShippingInIdsArray = $.grep(that.Settings.ShippingInIdsArray, function (value) {
                        return value != $idElement.text();
                    });
                }
            }
        })

        $("input[type='checkbox'][name='chbox-check-out']").on("change", function () {
            var $idElement = $(this).parent().siblings(":first");
            if ($idElement && $idElement.text() && $idElement.text().length > 0) {
                if ($(this).is(":checked") === true) {
                    that.Settings.ShippingOutIdsArray.push($idElement.text())
                }
                else {
                    that.Settings.ShippingOutIdsArray = $.grep(that.Settings.ShippingOutIdsArray, function (value) {
                        return value != $idElement.text();
                    });
                }
            }
        })
    },
    PopulateGrid: function () {
        var that = this;
        that.BindGridColumns();
        $(that.Settings.Controls.FreightRequestFormSelector).jsGrid({
            width: "100%",
            height: "300px",
            align: "center",
            inserting: !that.Settings.IsViewMode,
            editing: !that.Settings.IsViewMode,// || that.Settings.IsSecurityGuard,
            sorting: true,
            autoload: true,
            noDataContent: '',
            deleteConfirm: that.Settings.ResourceText.ConfirmDeleteMessage,
            data: that.Settings.Data,
            onDataLoaded: function (args) {
                // if (that.Settings.IsFormEditable == false) {
                //     that.DisbleDeleteButtonOnGrid();
                // } else {
                //     that.EnableButtonControls();
                // }
                // if (that.Settings.FormModeParam == "new") {
                //     that.DisableButtonControls();
                // }
            },
            controller: {
                loadData: function (filter) {
                    var d = $.Deferred();
                    for (var i = 0; i < that.Settings.GridJsonArray.length; i++) {
                        that.Settings.GridJsonArray[i].OrderNumber = (i + 1);
                    }

                    d.resolve(that.Settings.GridJsonArray);
                    return d.promise();
                },
                insertItem: function (item) {
                },
                updateItem: function (item) {
                },
                deleteItem: function (item) {
                }
            },
            onItemInserting: function (args) {
                var $selectedRow = args.grid._insertRow;
                args.cancel = !this.ValidateFreightItem(args, $selectedRow) || !this.ValidateQuantity(args.item.Quantity, $selectedRow);
            },
            onItemInserted: function (args) {
                // args.item.Id = 0;
            },
            onItemEditing: function (args) {

            },
            onItemUpdated: function (args) {
                //args.item.Id = 0;
            },
            onItemUpdating: function (args) {
                var $selectedRow = args.grid._editingRow.prev();
                args.cancel = !this.ValidateFreightItem(args, $selectedRow) || !this.ValidateQuantity(args.item.Quantity, $selectedRow);
            },

            ValidateQuantity: function (quantity, $selectedRow) {
                var $quantity = $selectedRow.find('td:nth-child(5) input');
                if (!$.isNumeric(quantity) || quantity <= 0) {
                    $quantity.addClass('require-error');
                    $quantity.attr('title', that.Settings.ResourceText.QuantityWrongMessage);

                    return false;
                }
                else {
                    $quantity.attr('title', '');
                    $quantity.removeClass('require-error');
                }

                return true;
            },
            ValidateFreightItem: function (args, $selectedRow) {
                var valid = true;
                // GoodsDescription
                if ($.trim(args.item.GoodsDescription) === '') {
                    $selectedRow.find('td:nth-child(3) input').addClass('require-error');
                    valid = false;
                }
                else
                    $selectedRow.find('td:nth-child(3) input').removeClass('require-error');
                // Quantity
                if ($.trim(args.item.Quantity) === '') {
                    $selectedRow.find('td:nth-child(5) input').addClass('require-error');
                    valid = false;
                }
                else
                    $selectedRow.find('td:nth-child(5) input').removeClass('require-error');
                // Unit
                if ($.trim(args.item.Unit) === '') {
                    $selectedRow.find('td:nth-child(4) input').addClass('require-error');
                    valid = false;
                }
                else
                    $selectedRow.find('td:nth-child(4) input').removeClass('require-error');

                return valid;
            },
            ValidateCellGridNotEmptyValue: function (args) {
                var goodsDesription = args.item.GoodsDescription;
                var quantity = args.item.Quantity;
                var unit = args.item.Unit;
                if (goodsDesription === "" || quantity === "" || unit === "") {
                    return false;
                }
                return true;
            },
            onItemDeleted: function (args) {
                for (var i = 0; i < that.Settings.GridJsonArray.length; i++) {
                    that.Settings.GridJsonArray[i].OrderNumber = (i + 1);
                }
                if (that.Settings.GridJsonArray.length > 1)
                    $(that.Settings.Controls.FreightRequestFormSelector).jsGrid("loadData");
            },
            onItemEditCancelling: function (args) {
            },
            fields:
            that.Settings.Grid.Fields,
        });
    },
    // ShowCheckBoxSecurity: function () {
    //     var that = this;
    // },
    DoSubmitData: function (event) {
        var that = this;
        $(that.Settings.Controls.ButtonSaveSelector).prop('disabled', true);
        var postData = that.GetPostData();
        if (postData && that.ValidateForm(postData) === true) {
            //if (postData.Id === 0) {
            postData.IsValidRequest = true;
            var urlValidateSumitTime = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ValidateSumitTime, location.host);
            $.ajax({
                type: "GET",
                url: urlValidateSumitTime,
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response && response.Code === 5) {
                    postData.IsValidRequest = false;
                    that.ShowInvalidRuleMessage(that.ProcessData, response.Message, postData);
                }
                else
                    that.ProcessData(postData);
            });
            //}
        }
        else {
            $(that.Settings.Controls.ButtonSaveSelector).prop('disabled', false);
        }
    },

    ProcessData: function (postData) {
        var that = this;
        var urlInsertFreightRequest = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.InsertFreightRequest, location.host);
        $.ajax({
            type: "POST",
            url: urlInsertFreightRequest,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && response.Code === 0) {
                Functions.redirectToSource();
            }
            else if (response && response.Code === 4) {
                alert(response.Message);
                window.location.reload();
            }
            //$(that.Settings.Controls.ButtonSaveSelector).prop('disabled', false);
        });
    },

    DoApproveFreight: function (event) {
        var that = this;
        that.ShowOrHideErrorMessage($(that.Settings.Controls.Comment_Error), "");
        $(that.Settings.Controls.ButtonApprove).prop('disabled', true);
        var postData = that.GetPostData();
        if (postData && that.ValidateForm(postData) === true) {
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ApproveFreightRequest, location.host);
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(postData),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (response && response.Code === 0) {
                    Functions.redirectToSource();
                }
                else if (response.Code === 2) {
                    alert(response.Message);
                    window.location.reload();
                }
            });
        }
        else {
            $(that.Settings.Controls.ButtonApprove).prop('disabled', false);
        }
    },
    DoRejectFreight: function (event) {
        var that = this;
        that.ShowOrHideErrorMessage($(that.Settings.Controls.Comment_Error), "");
        $(that.Settings.Controls.ButtonReject).prop('disabled', true);
        var postData = that.GetPostData();
        if (postData && that.ValidateForm(postData) === true) {
            if (postData.Comment && postData.Comment.length > 0) {
                var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.RejectFreightRequest, location.host);
                $.ajax({
                    type: "POST",
                    url: url,
                    data: JSON.stringify(postData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                }).done(function (response) {
                    if (response && response.Code === 0) {
                        Functions.redirectToSource();
                    } else if (response.Code === 3) {
                        alert(response.Message);
                        window.location.reload();
                    }
                });
            } else {
                that.ShowOrHideErrorMessage($(that.Settings.Controls.Comment_Error), that.Settings.ResourceText.CantLeaveTheBlank);
                $(that.Settings.Controls.ButtonReject).prop('disabled', false);
            }
        }
        else {
            $(that.Settings.Controls.ButtonReject).prop('disabled', false);
            event.preventDefault();
        }
    },
    ValidateForm: function (postData) {
        var that = this;
        return that.ValidateRequiredField(postData);
    },
    ValidateRequiredField: function (postData) {
        $("span[class='ms-formvalidation ms-csrformvalidation']").html("");
        var that = this;
        var emptyFieldCount = 0;

        if (that.Settings.SelectedBringerType === "0") {
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.SelectBringerSelector).val(), that.Settings.Controls.BringerSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
        }
        else if (that.Settings.SelectedBringerType === "2") {
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.TxtBringerSelector).val(), that.Settings.Controls.BringerSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.TxtCompanyNameSelector).val(), that.Settings.Controls.CompanySelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
        }

        emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.TxtReason).val(), that.Settings.Controls.ReasonSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;

        if (that.Settings.SelectedReasonType === "1") {
            emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.TxtOthersReason).val(), that.Settings.Controls.Others_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
        }

        if (postData.FreightDetails === null || postData.FreightDetails === undefined || postData.FreightDetails.length === 0) {
            emptyFieldCount += that.ValidateEmptyField("", that.Settings.Controls.FreightDetails_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
        }
        else {
            emptyFieldCount += that.ValidateEmptyField(postData.FreightDetails, that.Settings.Controls.FreightDetails_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;
        }

        emptyFieldCount += that.ValidateEmptyField($(that.Settings.Controls.DateSelector).val(), that.Settings.Controls.DateSelector_Error, that.Settings.ResourceText.CantLeaveTheBlank) === true ? 0 : 1;

        that.GetConfigurations();
        
        if (!(postData.Id > 0) ||
            (that.Settings.CurrentEmployeeLookupId === that.Settings.Data.Requester.LookupId && that.Settings.Data.ModifiedBy.ID !== that.Settings.Data.CreatedBy.ID &&
            that.Settings.Data.ApprovalStatus && that.Settings.Data.ApprovalStatus.toLowerCase() === 'rejected')) {
            var transportDate = Functions.parseVietNameseDate($(that.Settings.Controls.DateSelector).val());
            var diffDays = 0;
            try {
                var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidTransportDate);
                if(configVal)
                {
                    diffDays = parseInt(Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidTransportDate));
                }
            }
            catch (err) { diffDays = 0; }

            var nowDate = new Date();
            var minDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
            minDate.setDate(minDate.getDate() + diffDays);
            if (transportDate.valueOf() < minDate.valueOf()) {
                var errMsg = decodeURI(that.Settings.ResourceText.TransportTimeErrorMessage_1);
                errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, diffDays);
                emptyFieldCount += that.ValidateEmptyField(null, that.Settings.Controls.DateSelector_Error, errMsg) === true ? 0 : 1;
            }
        }

        return emptyFieldCount === 0;
    },
    ValidateEmptyField: function (controlValue, errorControl, message) {
        if (!controlValue || controlValue === "") {
            $(errorControl).html(message);
            $(errorControl).show();
        }
        else {
            $(errorControl).hide();
            return true;
        }
    },
    ShowOrHideErrorMessage: function (ctrl, msg) {
        if (msg && msg.length > 0) {
            $(ctrl).html(msg);
            $(ctrl).show();
        }
        else {
            $(ctrl).hide();
            $(ctrl).html("");
        }
    },
    GetPostData: function () {
        var that = this;
        var postData = {};
        var selectedBringerOption = $(that.Settings.Controls.SelectBringerSelector + " option:selected");
        postData.Id = that.Settings.FreightManagementId || 0;
        if (that.Settings.SelectedBringerType === "0") {
            postData.Bringer = { LookupId: parseInt(selectedBringerOption.val()) }
            postData.BringerDepartment = { LookupId: parseInt(selectedBringerOption.attr("DepartmentId")) };
            postData.BringerLocation = { LookupId: parseInt(selectedBringerOption.attr("LocationId")) };
            postData.CompanyVehicle = false;
            postData.BringerName = "";
            postData.CompanyName = "";
        }
        else if (that.Settings.SelectedBringerType === "1") {
            postData.Bringer = { LookupId: 0 }
            postData.BringerDepartment = { LookupId: 0 };
            postData.BringerLocation = { LookupId: 0 };
            postData.CompanyVehicle = true;
            postData.BringerName = "";
            postData.CompanyName = "";
        }
        else {
            postData.Bringer = { LookupId: 0 }
            postData.BringerDepartment = { LookupId: 0 };
            postData.BringerLocation = { LookupId: 0 };
            postData.CompanyVehicle = false;
            postData.BringerName = $(that.Settings.Controls.TxtBringerSelector).val();
            postData.CompanyName = $(that.Settings.Controls.TxtCompanyNameSelector).val();
        }
        postData.Requester = { LookupId: that.Settings.CurrentEmployeeLookupId };
        postData.Department = { LookupId: that.Settings.CurrentEmployeeDepartmentID };
        postData.Location = { LookupId: that.Settings.CurrentEmployeeLocationID };

        if (that.Settings.SelectedReasonType === "0") {
            postData.Receiver = $(that.Settings.Controls.TxtReceivedBy).val();
            // Check department -> get id
            var freightDepartmentId = 0;
            $.each(that.Settings.DepartmentArray, function (index, item) {
                if ($.trim(item.ReceiverDepartment).toUpperCase() == $.trim(postData.ReceiverDepartmentText).toUpperCase() ||
                    $.trim(item.ReceiverDepartmentVN).toUpperCase() == $.trim(postData.ReceiverDepartmentText).toUpperCase()) {
                    freightDepartmentId = item.Id;
                    return;
                }
            });

            postData.ReceiverDepartmentLookup = { LookupId: freightDepartmentId };
            if (freightDepartmentId === 0) {
                postData.ReceiverDepartmentText = $(that.Settings.Controls.TxtReceiverDepartment).val();
            }
            else {
                postData.ReceiverDepartmentText = "";
            }
            postData.ReceiverPhone = $(that.Settings.Controls.TxtPhoneContact).val();
            postData.OtherReason = "";
        }
        else {
            postData.Receiver = "";
            postData.ReceiverDepartmentText = "";
            postData.ReceiverDepartmentLookup = { LookupId: 0 };
            postData.ReceiverPhone = "";
            postData.OtherReason = $(that.Settings.Controls.TxtOthersReason).val();
        }

        var returnGoodsVal = $("input[type='radio'][name='returnGoods']:checked").val() === "Yes" ? true : false;
        postData.ReturnedGoods = returnGoodsVal;

        var freightTypeVal = $("input[type='radio'][name='freightType']:checked").val() === "Yes" ? true : false;
        postData.FreightType = freightTypeVal;
        postData.HighPriority = $(that.Settings.Controls.CkbHighPriority).prop("checked") === true ? true : false;

        var dateValue = $(that.Settings.Controls.DateSelector).val();
        var hourValue = $(that.Settings.Controls.HourSelector).val().replace(":", "");
        var minuteValue = $(that.Settings.Controls.MinuteSelector).val();
        postData.Reason = $(that.Settings.Controls.TxtReason).val();
        postData.Comment = $(that.Settings.Controls.TxtComment).val();
        postData.SecurityNotes = $(that.Settings.Controls.TxtSecurityNotes).val();
        postData.DateString = dateValue;
        postData.Hour = parseInt(hourValue);
        postData.Minute = parseInt(minuteValue);
        var dh = "", bod = "", adminDept = "";
        if (that.Settings.ApproverDH && that.Settings.ApproverDH.LoginName !== "") {
            dh = that.Settings.ApproverDH.LoginName;
        }

        if (that.Settings.ApproverBOD && that.Settings.ApproverBOD.LoginName !== "") {
            bod = that.Settings.ApproverBOD.LoginName;
        }
        if (that.Settings.ApproverHeadofAdminDept && that.Settings.ApproverHeadofAdminDept.LoginName !== "") {
            adminDept = that.Settings.ApproverHeadofAdminDept.LoginName;
        }
        postData.DH = { UserName: dh };
        postData.BOD = { UserName: bod };
        postData.AdminDept = { UserName: adminDept };
        postData.FreightDetails = that.GetDetailGridData();
        return postData;
    },

    GetDetailGridData: function () {
        var that = this;
        var detailData = [];
        var dataArray = that.Settings.GridJsonArray;
        if (dataArray && dataArray.length > 0) {
            for (var i = 0; i < dataArray.length; i++) {
                var item = {};
                item.GoodsName = dataArray[i].GoodsDescription;
                item.Unit = dataArray[i].Unit;
                item.Remarks = dataArray[i].Remarks;
                item.Quantity = dataArray[i].Quantity;
                item.IsShippingIn = false;
                item.IsShippingOut = false;
                detailData.push(item);
            }
        }
        return detailData;
    },
    BindOrderNumber: function () {
        var that = this;
        that.Settings.Grid.CustomFields.OrderNumberField = function (config) {
            jsGrid.Field.call(this, config);
        };
        that.Settings.Grid.CustomFields.OrderNumberField.prototype = new jsGrid.Field({
            insertTemplate: function (value) {
            },
            editTemplate: function (value) {
            },
            insertValue: function () {
                var itemCounts = that.Settings.GridJsonArray.length;
                return itemCounts + 1;
            },
            editValue: function () {
            }
        });
    },
    CustomCheckBox: function () {
        var that = this;
        that.Settings.Grid.CustomFields.IsShippingInField = function (config) {
            jsGrid.Field.call(this, config);
        };
        that.Settings.Grid.CustomFields.IsShippingInField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                if (that.Settings.IsSecurityGuard === true) {
                    return that.GetShippingCheckBox("ShippingIn", value, that.Settings.Data.FreightType, that.Settings.Data.ReturnedGoods);
                }
                else {
                    var checked = "";
                    if (value === true) {
                        checked = ' checked="checked" ';
                    }
                    return '<input name="chbox-check-in" type="checkbox" ' + checked + ' disabled="disabled" />'
                }
            },
            insertTemplate: function (value) {
            },
            editTemplate: function (value) {
            },
            insertValue: function () {
            },
            editValue: function () {
            }
        });

        that.Settings.Grid.CustomFields.IsShippingOutField = function (config) {
            jsGrid.Field.call(this, config);
        };
        that.Settings.Grid.CustomFields.IsShippingOutField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                if (that.Settings.IsSecurityGuard === true) {
                    return that.GetShippingCheckBox("ShippingOut", value, that.Settings.Data.FreightType, that.Settings.Data.ReturnedGoods);
                }
                else {
                    var checked = "";
                    if (value === true) {
                        checked = ' checked="checked" ';
                    }
                    return '<input name="chbox-check-out" type="checkbox" ' + checked + ' disabled="disabled" />'
                }
            },
            insertTemplate: function (value) {
            },
            editTemplate: function (value) {
            },
            insertValue: function () {
            },
            editValue: function () {
            }
        });
    },
    GetShippingCheckBox: function (checkboxType, value, freightType, isReturnGoods) {
        var that = this;
        var checked = "";
        var disabled = "";
        if (value && value == true) {
            checked = ' checked="checked" ';
            disabled = ' disabled="disabled" ';

        }
        switch (checkboxType) {
            case "ShippingIn":
                var checkboxName = "chbox-check-in";
                if (freightType == true && isReturnGoods == false) {
                    //disable by default
                    disabled = ' disabled="disabled" '
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + '  />';
                }
                else if (freightType == true && isReturnGoods == true) {
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + '/>';
                }
                else if (freightType == false && isReturnGoods == false) {
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + ' />';
                }
                else if (freightType == false && isReturnGoods == true) {
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + ' />';
                }
                break;
            case "ShippingOut":
                var checkboxName = "chbox-check-out";
                if (freightType == true && isReturnGoods == false) {
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + '/>';
                }
                else if (freightType == true && isReturnGoods == true) {

                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + '/>';
                }
                else if (freightType == false && isReturnGoods == false) {
                    //disable by default
                    disabled = ' disabled="disabled" ';
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + '/>';
                }
                else if (freightType == false && isReturnGoods == true) {
                    return '<input name="' + checkboxName + '" type="checkbox" ' + checked + disabled + ' />';
                }
                break;
        }
    },
    DisableShippingCheckboxes: function () {
        var that = this;
        var shippingInCheckBoxes = $("input[type='checkbox'][name='chbox-check-in']");
        var shippingOutCheckBoxes = $("input[type='checkbox'][name='chbox-check-out']");
        if (shippingInCheckBoxes && shippingOutCheckBoxes && shippingInCheckBoxes.length > 0 && shippingOutCheckBoxes.length > 0 && shippingInCheckBoxes.length == shippingOutCheckBoxes.length) {
            if ((that.Settings.Data.FreightType == true && that.Settings.Data.ReturnedGoods == true)) {
                for (var i = 0; i < shippingInCheckBoxes.length; i++) {
                    var item = $(shippingOutCheckBoxes[i]);
                    if ($(item).is(":checked") == false && $(shippingInCheckBoxes[i]).is(":checked") == false) {
                        $(shippingInCheckBoxes[i]).attr("disabled", "disabled");
                    }
                }
            }
            else if (that.Settings.Data.FreightType == false && that.Settings.Data.ReturnedGoods == true) {
                for (var i = 0; i < shippingOutCheckBoxes.length; i++) {
                    var item = $(shippingInCheckBoxes[i]);
                    if ($(item).is(":checked") == false && $(shippingOutCheckBoxes[i]).is(":checked") == false) {
                        $(shippingOutCheckBoxes[i]).attr("disabled", "disabled");
                    }
                }
            }
        }

        var checkedShippingInCheckBoxes = $("input[type='checkbox'][name='chbox-check-in']:checked");
        var checkedOutCheckBoxes = $("input[type='checkbox'][name='chbox-check-out']:checked");

        if ((checkedShippingInCheckBoxes && checkedShippingInCheckBoxes.length > 0) || (checkedOutCheckBoxes && checkedOutCheckBoxes.length > 0)) {
            $(that.Settings.Controls.ButtonSecReject).prop("disabled", true);
        }
    },
    BindGridColumns: function () {
        var that = this;
        that.BindOrderNumber();
        that.CustomCheckBox();
        jsGrid.fields.custOrderField = that.Settings.Grid.CustomFields.OrderNumberField;
        jsGrid.fields.customIsShippingInField = that.Settings.Grid.CustomFields.IsShippingInField;
        jsGrid.fields.customIsShippingOutField = that.Settings.Grid.CustomFields.IsShippingOutField;

        that.Settings.Grid.Fields = [
            { name: "Id", title: "Id", align: "center", width: 0, type: "text", css: "jsgrid-hide-column" },
            { name: "OrderNumber", title: that.Settings.Grid.ColumnTitles.GridColumn_Order, align: "center", width: 30, type: "custOrderField" },
            { name: "GoodsDescription", title: that.Settings.Grid.ColumnTitles.GridColumn_GoodsDescription, width: 250, align: "left", type: "text" },
            { name: "Unit", title: that.Settings.Grid.ColumnTitles.GridColumn_Unit, width: 65, align: "center", type: "text" },
            { name: "Quantity", title: that.Settings.Grid.ColumnTitles.GridColumn_Quantity, width: 90, align: "center", type: "text" },
            { name: "Remarks", title: that.Settings.Grid.ColumnTitles.GridColumn_Remarks, width: 300, align: "left", type: "text" }
            //{ name: "IsShippingIn", title: that.Settings.Grid.ColumnTitles.ColumnTitles_CheckIn, width: 80, align: "center", type: "customIsShippingInField" },
            //{ name: "IsShippingOut", title: that.Settings.Grid.ColumnTitles.ColumnTitles_CheckOut, width: 80, align: "center", type: "customIsShippingOutField" },
            //{ type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false }
        ];

        if (that.Settings.IsViewMode === true) {
            that.Settings.Grid.Fields.push({ name: "ShippingInTime", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingInTime, width: 120, align: "center", type: "text" });
            that.Settings.Grid.Fields.push({ name: "ShippingInBy", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingInBy, width: 120, align: "center", type: "text" });
            that.Settings.Grid.Fields.push({ name: "IsShippingIn", title: that.Settings.Grid.ColumnTitles.GridColumn_CheckIn, width: 120, align: "center", type: "customIsShippingInField" });

            that.Settings.Grid.Fields.push({ name: "ShippingOutTime", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingOutTime, width: 120, align: "center", type: "text" });
            that.Settings.Grid.Fields.push({ name: "ShippingOutBy", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingOutBy, width: 120, align: "center", type: "text" });
            that.Settings.Grid.Fields.push({ name: "IsShippingOut", title: that.Settings.Grid.ColumnTitles.GridColumn_CheckOut, width: 120, align: "center", type: "customIsShippingOutField" });
        }
        else {
            that.Settings.Grid.Fields.push({ name: "ShippingInTime", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingInTime, width: 0, align: "center", type: "text", css: "jsgrid-hide-column" });
            that.Settings.Grid.Fields.push({ name: "ShippingInBy", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingInBy, width: 0, align: "center", type: "text", css: "jsgrid-hide-column" });
            that.Settings.Grid.Fields.push({ name: "IsShippingIn", title: that.Settings.Grid.ColumnTitles.GridColumn_CheckIn, width: 0, align: "center", type: "customIsShippingInField", css: "jsgrid-hide-column" });

            that.Settings.Grid.Fields.push({ name: "ShippingOutTime", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingOutTime, width: 0, align: "center", type: "text", css: "jsgrid-hide-column" });
            that.Settings.Grid.Fields.push({ name: "ShippingOutBy", title: that.Settings.Grid.ColumnTitles.GridColumn_ShippingOutBy, width: 0, align: "center", type: "text", css: "jsgrid-hide-column" });
            that.Settings.Grid.Fields.push({ name: "IsShippingOut", title: that.Settings.Grid.ColumnTitles.GridColumn_CheckOut, width: 0, align: "center", type: "customIsShippingOutField", css: "jsgrid-hide-column" });
        }
        that.Settings.Grid.Fields.push({ type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false });
    },
    ShowInvalidRuleMessage: function (processFunc, msg, postData) {
        var that = this;
        $.confirm({
            title: that.Settings.Modal.Title,
            content: msg,
            buttons: {
                somethingElse: {
                    text: that.Settings.Modal.SaveButton,
                    btnClass: 'btn-blue',
                    action: function () {
                        if (typeof processFunc == 'function')
                            if (typeof processFunc == 'function') {
                                var processData = processFunc.bind(that);
                                processData(postData);
                            }
                    }
                },
                close: {
                    text: that.Settings.Modal.CloseButton,
                    action: function () {
                        $(that.Settings.Controls.ButtonSaveSelector).prop('disabled', false);
                    }
                },
            }
        });
    }
};
