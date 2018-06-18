RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.OvertimeRequest = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        ServiceUrls:
        {
            DepartmentList: this.Protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentsByLcid/{1}/{2}',
            DepartmentId: this.Protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentByIdLanguageCode/{1}/{2}',
            GetOverTimeManagementById: this.Protocol + '//{0}/_vti_bin/Services/Overtime/OvertimeService.svc/GetById/{1}',
            UserInformation: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser',
            EmployeeList: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetByDepartmentLocation/{1}/{2}/{3}',
            OvertimeSubmit: this.Protocol + '//{0}/_vti_bin/Services/Overtime/OvertimeService.svc/InsertOvertime',
            Approvers: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/{1}',
            GetModifiedDate: this.Protocol + '//{0}/_vti_bin/Services/Common/CommonService.svc/GetModifiedDate/{1}/{2}',
            //UpdateOvertime: this.Protocol + '//{0}/_vti_bin/Services/Overtime/OvertimeService.svc/UpdateApprove',
            EmployeeNotOvertimeList: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeListDontHaveOvertimeInDate/{1}/{2}/{3}',
            IsDelegated: this.Protocol + '//{0}/_vti_bin/Services/Overtime/OvertimeService.svc/IsDelegated/{1}/{2}', // {fromApprover}
            GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations'
        },

        OvertimeManagementListUrl: this.Protocol + '//{0}/SitePages/OvertimeManagement.aspx',

        ISODateFormat: '{0}-{1}-{2}',

        OvertimeDate: '',
        Id: 0,
        Grid:
        {
            CustomFields: {
                FullNameField: null,
                OvertimeHourFromField: null,
                OvertimeHourToField: null,
                TaskField: null
            },
            CompanyTransportList: [
                { Name: "Tự túc", Id: 'Tự túc' },
                { Name: "HM", Id: 'HM' },
                { Name: "KD", Id: 'KD' }
            ],
        },

        OvertimeJsonArray: [],
        Data: [
        ],
        EmployeeList: [
            { "Name": "Tri Ngo", "Id": 11111, "Code": "Emp11111" },
            { "Name": "Chau Tran", "Id": 22222, "Code": "Emp22222" }
        ],
        FullEmployeeList: [
        ],
        EmployeeListAdd: [],
        EmployeeListEdit: [],
        EmployeeListDelete: [],
        AddedEmployeeCodeList: [
            //"Emp11111"
        ],

        RegisteredEmployeeList: [],

        // Date picker
        StartDate: null,
        EndDate: null,
        DefaultDurationHour: 23,

        // Requester
        Requester: {
            Id: 0,
            Name: '',
            DepartmentId: 0,
            LocationId: 0
        },

        ApprovedBy: {
            UserName: '',
            FullName: ''
        },

        OvertimeRequestId: 0,

        // ADMIN ?
        IsSystemAdmin: false,

        // Reject
        IsReject: false,

        // Indicator:
        ShaderCssClass: '.jsgrid-load-shader',
        LoadPanelCssClass: '.jsgrid-load-panel',

        IsValidRow: true,
        IsInsertError: false,
        IsItemEditing: false,

        IsViewMode: false,

        GridBeforeEdit: null,

        IsValidGridDetails: false,
        IsValidMaster: true,

        IsBODApprovalRequired: false,
        CurrentUserInfo: {
            RoleId: 0
        },
        Roles: {
            BOD: 1,
            DepartmentHead: 2
        },
        BORoleId: 1, // BOD,
        BODApprover: null,

        // From/To hour/minute INSERT TEMPLATE
        InsertTemplate:
        {
            WorkingHourFrom: '0',
            WorkingHourTo: '9',
            WorkingMinuteFrom: '0',
            WorkingMinuteTo: '0',
            Task: '',
            CompanyTransport: ''
        },
        ApproverFullName: '',
        Modified: '',

        ApprovalHistory: {},
        Configurations: {},
        ConfigKey_ValidOverTimeDate: "OvertimeForm_ValidOverTimeDate",
        DiffDays: 0
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.OvertimeRequest.prototype = {
    Initialize: function () {
        var that = this;
        that.Settings.Id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemId');
        var prevURL = RBVH.Stada.WebPages.Utilities.GetValueByParam('Source');
        prevURL = !!prevURL ? decodeURIComponent(prevURL) : document.referrer;
        that.Settings.PrevURL = prevURL;//RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ShiftManagementListUrl, location.host);
        $(document).ready(function () {
            ExecuteOrDelayUntilScriptLoaded(function () {
                that.GetConfigurations();
                that.InitControls();
                that.SetDefaultValue();
                that.PopulateData();
                that.RegisterEvents();
            }, "sp.js");
        });
    },
    GetConfigurations: function () {
        var that = this;
        var postData = [that.Settings.ConfigKey_ValidOverTimeDate];
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
    RegisterEvents: function () {
        var that = this;

        $(that.Settings.FromDateControlSelector).on('changeDate', function (ev) {
            var dpDay = ev.date.getDate();
            var dpMonth = ev.date.getMonth() + 1; // 0 -> 11
            var dpYear = ev.date.getYear() + 1900;
            var overtimeDate = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ISODateFormat, dpYear, dpMonth, dpDay);
            that.Settings.OvertimeDate = overtimeDate;
            $('.datepicker').hide();

            if (that.ValidateInputDate() == true) {
                that.GetEmployeeList();
            }
            else {
                that.Settings.EmployeeList = [];
            }
            //that.GetEmployeeList();

            // Reload
            that.Settings.OvertimeJsonArray = [];
            that.Settings.AddedEmployeeCodeList = [];
            that.Settings.EmployeeListAdd = [];
            that.Settings.EmployeeListEdit = [];
            that.Settings.EmployeeListDelete = [];
            $(that.Settings.QuantityControlSelector).val('0');
            $(that.Settings.ServingControlSelector).val('0');

            $(that.Settings.GridOvertimeRequestControlSelector).jsGrid("destroy");
            that.PopulateGrid();
        });

        $(that.Settings.QuantityControlSelector).on('keypress', function (e) {
            var charCode = (e.which) ? e.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        });

        $(that.Settings.ServingControlSelector).on('keypress', function (e) {
            var charCode = (e.which) ? e.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            return true;
        });

        $(that.Settings.ServingControlSelector).on('blur keyup', function () {
            var quantity = $(that.Settings.QuantityControlSelector).val() != '' ? parseInt($(that.Settings.QuantityControlSelector).val()) : 0;
            var serving = $(this).val() != '' ? parseInt($(this).val()) : 0;
            var blurEl = $(this);
            setTimeout(function () {
                if (serving > quantity) {
                    blurEl.addClass('required-error');
                    blurEl.focus();
                    that.Settings.IsValidMaster = false;
                    that.ToggleSaveButton();
                }
                else {
                    blurEl.removeClass('required-error');
                    that.Settings.IsValidMaster = true;
                    that.ToggleSaveButton();
                }
            }, 10);
        });

        $(that.Settings.DepartmentControlSelector).on('change', function () {
            // Set Department Id
            that.Settings.Requester.DepartmentId = $(this).val();
            that.GetEmployeeList();
            // Re-populate grid
            $(that.Settings.GridOvertimeRequestControlSelector).jsGrid("destroy");
            that.PopulateGrid();
        });

        $(document).on('change', 'select.workingHourFrom', function () {
            var workingHourFromValue = $(this).val();
            var workingHourToValue = $(this).closest('td').next('td').find('.workingHourTo').val();

            var validHours = that.ValidateHours(parseInt(workingHourFromValue), parseInt(workingHourToValue));
            that.Settings.IsInsertError = false;
            if (!validHours) {
                that.Settings.IsInsertError = false;
                if ($(this).parents('.jsgrid-insert-row').length > 0) // INSERT template
                    that.Settings.IsInsertError = true;
                else
                    that.Settings.IsValidRow = false;
                $(this).attr('title', that.Settings.InvalidHoursMessage);

                $(this).addClass('time-invalid');
                $(this).closest('tr.jsgrid-insert-row').find('.jsgrid-insert-button').prop('disabled', true);
                $(this).closest('tr.jsgrid-edit-row').find('.jsgrid-update-button').prop('disabled', true);
            }
            else {
                $(this).attr('title', '');
                $(this).removeClass('time-invalid');
                var $overtimeToItem = $(this).parent().parent().find('.workingHourTo');
                if ($overtimeToItem.length > 0) {
                    $overtimeToItem.attr('title', '');
                    $overtimeToItem.removeClass('time-invalid');
                }

                that.Settings.IsValidRow = true;
                $(this).closest('tr.jsgrid-insert-row').find('.jsgrid-insert-button').prop('disabled', false);
                $(this).closest('tr.jsgrid-edit-row').find('.jsgrid-update-button').prop('disabled', false);
            }
        });

        $(document).on('change', 'select.workingHourTo', function () {
            var workingHourToValue = $(this).val();
            var workingHourFromValue = $(this).closest('td').prev('td').find('.workingHourFrom').val();

            var validHours = that.ValidateHours(parseInt(workingHourFromValue), parseInt(workingHourToValue));
            that.Settings.IsInsertError = false;
            if (!validHours) {

                if ($(this).parents('.jsgrid-insert-row').length > 0) // INSERT template
                    that.Settings.IsInsertError = true;
                else
                    that.Settings.IsValidRow = false;
                $(this).attr('title', that.Settings.InvalidHoursMessage);
                $(this).addClass('time-invalid');
                $(this).closest('tr.jsgrid-insert-row').find('.jsgrid-insert-button').prop('disabled', true);
                $(this).closest('tr.jsgrid-edit-row').find('.jsgrid-update-button').prop('disabled', true);
            }
            else {
                $(this).attr('title', '');
                $(this).removeClass('time-invalid');
                var $overtimeFromItem = $(this).parent().parent().find('.workingHourFrom');
                if ($overtimeFromItem.length > 0) {
                    $overtimeFromItem.attr('title', '');
                    $overtimeFromItem.removeClass('time-invalid');
                }
                that.Settings.IsValidRow = true;
                $(this).closest('tr.jsgrid-insert-row').find('.jsgrid-insert-button').prop('disabled', false);
                $(this).closest('tr.jsgrid-edit-row').find('.jsgrid-update-button').prop('disabled', false);
            }
        });

        $(that.Settings.SaveControlSelector).click(function () {
            if (!that.ValidateFields()) {
                alert(that.Settings.RequiredFieldsErrorMessage);
                return false;
            }
            // Validate employee list
            that.ValidateEmployeeList(that.OnSaveData);

            return false;
        });

        $(that.Settings.RejectControlSelector).click(function () {
            if (!that.ValidateFields()) {
                alert(that.Settings.RequiredFieldsErrorMessage);
                return false;
            }

            $(that.Settings.CommentsControlSelector).removeClass('required-error');
            // Validate Comments
            if ($(that.Settings.CommentsControlSelector).val() == '') {
                $(that.Settings.CommentsControlSelector).addClass('required-error');
                return false;
            }

            that.Settings.IsReject = true;
            that.OnSaveData();
            return false;
        });

        $(that.Settings.CancelControlSelector).click(function () {
            var sourceUrl = Functions.getParameterByName("Source");
            if (sourceUrl && sourceUrl.length > 0) {
                window.location.href = sourceUrl;
            }
            else {
                window.location = that.Settings.PrevURL;
            }

            return false;
        });

        // Custome events
        $(that.Settings.GridOvertimeRequestControlSelector).on("keyup", ".jsgrid-edit-row input, .jsgrid-edit-row select", function (e) {
            if (e.which == 13 || e.which == 40 || e.which == 38) { // Enter - Move down - Move up
                if (that.Settings.IsValidRow) {
                    $('.jsgrid-update-button').click();
                    $(that.Settings.SaveControlSelector).focus();
                }
            }
            else if (e.which == 27) {
                $('.jsgrid-cancel-edit-button').click();
            }
        });

        $(document).on('click', '.jsgrid-row , .jsgrid-alt-row', function () {
            var currentRow = $(this);
            var editedRow = $(this).prev();

            editedRow.find("td input:enabled:visible:not([readonly]):first").focus();
        });

        $(that.Settings.GridOvertimeRequestControlSelector).on("keyup", ".jsgrid-insert-row input, .jsgrid-insert-row select", function (e) {
            if (e.which == 13 || e.which == 40 || e.which == 38) { // Enter - Move down - Move up
                if (!that.Settings.IsInsertError)
                    $('.jsgrid-insert-button').click();
            }
        });

        $(window).keydown(function (event) {
            if (event.which == 13) {
                event.preventDefault();
                return false;
            }
        });

        $(document).on('click', 'a.viewDetail', function () {
            var dialogUrl = $(this).attr('data-url');
            var ModalDialogOptions = { url: dialogUrl, width: 650, height: 500, showClose: true, allowMaximize: false, title: that.Settings.ViewDetail };
            SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', ModalDialogOptions);
        });

        // Approval History
        $(that.Settings.ApprovalHistoryButtonSelector).on('click', function (event) {
            event.preventDefault();

            that.RenderOvertimeHistory();
        });
    },
    PopulateData: function () {
        this.PopulateDepartment();
        this.PopulatePersonalInformation();
        this.TryLoadInformation();
        this.PopulateEmployeeList();
        this.PopulateGrid();
    },
    TryLoadInformation: function () {
        var that = this;
        var listMode = RBVH.Stada.WebPages.Utilities.GetValueByParam('Mode');
        if (listMode && listMode.toUpperCase() == 'VIEW') {
            that.Settings.IsViewMode = true;
            that.DisableControls();
        }

        var id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemid');
        if (id) {
            that.Settings.OvertimeRequestId = id;
            $(that.Settings.FromDateControlSelector).prop('disabled', true);
            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetOverTimeManagementById, location.host, id);
            $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json; charset=utf-8",
                async: false,
                cache: false,
                dataType: "json",
            }).done(function (response) {
                // Load approval history:
                that.Settings.ApprovalHistory.ApprovedLevel = response.ApprovedLevel;
                that.Settings.ApprovalHistory.ApprovalStatus = response.ApprovalStatus;
                that.Settings.ApprovalHistory.FirstApprovedBy = response.FirstApprovedBy;
                that.Settings.ApprovalHistory.FirstApprovedDate = response.FirstApprovedDate;
                that.Settings.ApprovalHistory.Modified = response.Modified;
                that.Settings.Modified = response.Modified;
                that.Settings.ApprovalHistory.ApprovedBy = response.ApprovedBy;
                that.Settings.ApprovalHistory.DHComments = response.DHComments;
                that.Settings.ApprovalHistory.BODComments = response.BODComments;
                that.Settings.ApprovalHistory.SecurityComments = response.SecurityComments;


                that.Settings.OvertimeJsonArray = response.OvertimeDetailModelList;
                // Bind data to control:
                var fromDate = new Date(response.Date);
                $(that.Settings.FromDateControlSelector).val(fromDate.getDate() + '/' + (fromDate.getMonth() + 1) + '/' + (fromDate.getYear() + 1900));
                that.Settings.OvertimeDate = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ISODateFormat, fromDate.getYear() + 1900, fromDate.getMonth() + 1, fromDate.getDate());

                // Department
                that.PopulateDepartment(response.CommonDepartment.LookupId);
                //$(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', response.CommonDepartment.LookupId).text(response.CommonDepartment.LookupValue));
                //$(that.Settings.DepartmentControlSelector).prop('disabled', true);
                //$(that.Settings.DepartmentControlSelector).val(response.CommonDepartment.LookupId);

                that.Settings.LocationId = response.CommonLocation.LookupId;
                $(that.Settings.FactoryLocationControlSelector).html(response.CommonLocation.LookupValue);
                $(that.Settings.PlaceControlSelector).val(response.Place);
                $(that.Settings.QuantityControlSelector).val(response.SumOfEmployee);
                $(that.Settings.ServingControlSelector).val(response.SumOfMeal);
                // Requester
                $(that.Settings.RequesterControlSelector).html(response.Requester.LookupValue);
                that.Settings.Requester.Id = response.Requester.LookupId;
                that.Settings.Requester.Name = response.Requester.LookupValue;
                $(that.Settings.OtherRequestControlSelector).val(response.OtherRequirements);

                that.Settings.DepartmentId = response.CommonDepartment.LookupId;
                that.Settings.ApprovedBy = response.ApprovedBy;
                $(that.Settings.ApprovedByControlSelector).html(response.ApprovedBy.FullName);

                if (that.Settings.View == "Approval") {
                    // Check current department + location -> Prevent 
                    if (_rbvhContext && _rbvhContext.EmployeeInfo && (_rbvhContext.EmployeeInfo.ADAccount.ID != response.ApprovedBy.ID)) {
                        // Check current department + location -> Prevent 
                        if (_rbvhContext && _rbvhContext.EmployeeInfo && (_rbvhContext.EmployeeInfo.ADAccount.ID != response.ApprovedBy.ID)) {
                            var delegateUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.IsDelegated, location.host, response.ApprovedBy.ID, id);
                            $.ajax({
                                type: "GET",
                                url: delegateUrl,
                                cache: false,
                                async: false,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                            }).done(function (response) {
                                // [Delegation] Check current user is delegated:
                                if (response == false)
                                    window.location = window.location.protocol + '//' + location.host + '/_layouts/15/AccessDenied.aspx';
                                that.Settings.ApproverFullName = _rbvhContext.EmployeeInfo.FullName;
                            });
                        }
                    }

                    if (that.Settings.OvertimeJsonArray.length == 0) {
                        that.DisableControls();
                    }
                }

                // Check status -> Disable:
                if (response.ApprovalStatus == 'false' || response.ApprovalStatus == 'true' || (that.Settings.View != "Approval" && response.ApprovalStatus == '' && response.FirstApprovedBy != null && response.FirstApprovedBy.ID > 0)) {
                    that.Settings.IsViewMode = true;
                    that.DisableControls();
                }
                else {
                    if (that.Settings.View == "Approval" && response.RequestExpired == true) {
                        errMsg = decodeURI(that.Settings.RequestExpiredMsgFormat);
                        errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, response.RequestDueDate);
                        RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.ErrorMsgContainerSelector, that.Settings.ErrorMsgSelector, errMsg);

                        that.Settings.IsViewMode = true;
                        that.DisableControls();
                    }
                }

                if (listMode && listMode.toUpperCase() == 'VIEW') {
                    var attachmentFileUrl = Functions.getAttachments("Overtime Management", id);
                    if (attachmentFileUrl != "") {
                        if (that.Settings.CurrentUserInfo.RoleId != that.Settings.Roles.BOD) {
                            $(that.Settings.PrintControlSelector).show();
                        }

                    }
                    $(that.Settings.PrintControlSelector).on('click', function (e) {
                        window.open(attachmentFileUrl);
                        e.preventDefault();
                    })
                }
            });
        }
    },
    PopulateAdminRequester: function () {
        var that = this;
        ExecuteOrDelayUntilScriptLoaded(function () {
            var currentContext = new SP.ClientContext.get_current();
            var currentWeb = currentContext.get_web();
            var currentUser = currentContext.get_web().get_currentUser();
            currentContext.load(currentUser);
            currentContext.executeQueryAsync(function () {
                var id = currentUser.get_id();
                var title = currentUser.get_title();
                // Requester
                $(that.Settings.RequesterControlSelector).html(title);
                that.Settings.Requester = {
                    Id: id,
                    Name: title,
                    DepartmentId: $(that.Settings.DepartmentControlSelector).val()
                };
            }, function () { });
        }, "sp.js");
    },

    PopulatePersonalInformation: function () {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.UserInformation, location.host);
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function (result) {
                if (result) {
                    if (result.IsSystemAdmin == true) {
                        that.Settings.IsSystemAdmin = true;
                        that.PopulateDepartment();
                        that.PopulateAdminRequester();
                    }
                    else {
                        // Insert
                        if (that.Settings.View != "Approval" && !!that.Settings.Id == false) {
                            that.Settings.Requester = {
                                Id: result.ID,
                                Name: result.FullName,
                                DepartmentId: result.Department.LookupId,
                                LocationId: result.Location.LookupId
                            };
                            that.Settings.DepartmentId = result.Department.LookupId;
                            that.Settings.LocationId = result.Location.LookupId;
                            $(that.Settings.RequesterControlSelector).html(result.FullName);
                            $(that.Settings.FactoryLocationControlSelector).html(result.Location.LookupValue);
                            $(that.Settings.DepartmentControlSelector).val(result.Department.LookupId).change();
                            $(that.Settings.DepartmentControlSelector).prop('disabled', true);
                        }
                    }
                    // Set role for current user
                    that.Settings.CurrentUserInfo.RoleId = result.EmployeePosition;

                    // BOD approve?
                    that.Settings.IsBODApprovalRequired = result.IsBODApprovalRequired;
                    if (!!that.Settings.ApprovedBy.UserName == false) {
                        // Load Approved by:
                        that.PopulateApprovedBy(result.ID);
                    }
                }
            }
        });
    },

    PopulateDepartment: function (departmentId) {
        var that = this;
        var lcid = SP.Res.lcid;
        var currentDepartmentId = 0;
        if (typeof (departmentId) != 'undefined')
            currentDepartmentId = departmentId;

        if (currentDepartmentId == 0) {
            if (_rbvhContext != null && _rbvhContext.EmployeeInfo.Department != null)
                currentDepartmentId = _rbvhContext.EmployeeInfo.Department.LookupId;
        }

        if (currentDepartmentId == 0)
            return;

        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DepartmentId, location.host, currentDepartmentId, lcid);
        $(that.Settings.DepartmentControlSelector).attr("disabled", false);
        $(that.Settings.DepartmentControlSelector).empty();
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                $(result).each(function () {
                    $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', this.Id).text(this.DepartmentName));
                });
            }
        });

    },

    PopulateApprovedBy: function (employeeID) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.Approvers, location.host, employeeID);
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (result) {
                    if (that.Settings.IsBODApprovalRequired) // Is BOD required ? -> Load BOD
                    {
                        if (that.Settings.View == "Approval") {
                            if (result.Approver3) {
                                that.Settings.BODApprover = {};
                                that.Settings.BODApprover.UserName = result.Approver3.LoginName;
                                that.Settings.BODApprover.FullName = result.Approver3.FullLoginName;

                            }
                        }
                    }
                    if (result.Approver2) {
                        $(that.Settings.ApprovedByControlSelector).html(result.Approver2.FullLoginName);
                        that.Settings.ApprovedBy.UserName = result.Approver2.LoginName;
                        that.Settings.ApprovedBy.FullName = result.Approver2.FullLoginName;
                    }
                }
            }
        });
    },

    PopulateGrid: function () {

        var that = this;

        that.BindGridColumns();
        $(that.Settings.GridOvertimeRequestControlSelector).jsGrid({
            width: "100%",
            height: "400px",
            align: "center",
            inserting: !that.Settings.IsViewMode && that.Settings.View != "Approval",
            editing: !that.Settings.IsViewMode,
            sorting: true,
            autoload: true,
            noDataContent: '',//that.Settings.Grid.Titles.GridNoData,
            deleteConfirm: that.Settings.ConfirmDeleteMessage,
            onDataLoaded: function (args) {
            },
            controller: {
                loadData: function (filter) {
                    var d = $.Deferred();
                    d.resolve(that.Settings.OvertimeJsonArray);
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
                var workingHourFromValue = args.item.OvertimeHourFrom.split(':')[0];
                var workingHourToValue = args.item.OvertimeHourTo.split(':')[0];
                var validHours = that.ValidateHours(parseInt(workingHourFromValue), parseInt(workingHourToValue));
                if (!validHours) {
                    alert(that.Settings.InvalidHoursMessage);
                    args.cancel = true;
                    return;
                }

                if (that.FilterEmployeeList().length == 0) {
                    args.cancel = true;
                }
            },
            onItemInserted: function (args) {
                var $item = args.item.FullName;
                that.Settings.AddedEmployeeCodeList.push($item.split('_')[1]);
                args.item.ID = 0;
                that.Settings.EmployeeListAdd.push(args.item);

                // Update Quantity & Meal
                that.UpdateQuantity(1);
                that.ToggleSaveButton();

                // Init comboboxes with prev value:
                var workingHourFromValue = args.item.OvertimeHourFrom.split(':')[0];
                that.Settings.InsertTemplate.WorkingHourFrom = workingHourFromValue;
                var workingHourToValue = args.item.OvertimeHourTo.split(':')[0];
                that.Settings.InsertTemplate.WorkingHourTo = workingHourToValue;
                var workingMinuteFromValue = args.item.OvertimeHourFrom.split(':')[1];
                that.Settings.InsertTemplate.WorkingMinuteFrom = workingMinuteFromValue;
                var workingMinuteToValue = args.item.OvertimeHourTo.split(':')[1];
                that.Settings.InsertTemplate.WorkingMinuteTo = workingMinuteToValue;

                that.Settings.InsertTemplate.Task = args.item.Task;
                that.Settings.InsertTemplate.CompanyTransport = args.item.CompanyTransport;
            },
            onItemEditing: function (args) {
                that.Settings.IsItemEditing = true;
                that.ToggleSaveButton();
            },
            onItemUpdated: function (args) {
                var $item = args.item.FullName;
                args.item.ID = args.item.ID == "" ? "0" : args.item.ID;
                var id = parseInt(args.item.ID);
                if (id > 0) {
                    that.RefreshEmployeeEditList(args.item.FullName);
                    that.Settings.EmployeeListEdit.push(args.item);
                }
                else {
                    that.RefreshEmployeeAddList(args.item.FullName);
                    that.Settings.EmployeeListAdd.push(args.item);
                }

                that.Settings.IsItemEditing = false;
                that.ToggleSaveButton();


            },
            onItemDeleted: function (args) {
                var $item = args.item.FullName;
                // Update LIST
                args.item.ID = args.item.ID == "" ? "0" : args.item.ID;
                var id = parseInt(args.item.ID);
                if (id > 0) {
                    var empID = $item.split('_')[1];
                    var empCode = that.GetEmployeeCodeById(empID);
                    var index = that.Settings.AddedEmployeeCodeList.indexOf(empCode);
                    that.Settings.AddedEmployeeCodeList.splice(index, 1);

                    args.item.ID = -id;
                    that.Settings.EmployeeListDelete.push(args.item);
                }
                else {
                    var index = that.Settings.AddedEmployeeCodeList.indexOf($item.split('_')[1]);
                    that.Settings.AddedEmployeeCodeList.splice(index, 1);

                    that.RefreshEmployeeAddList(args.item.FullName);
                    that.RefreshEmployeeEditList(args.item.FullName);
                }
                // Enable 'Insert' icon
                if (that.FilterEmployeeList().length > 0) {
                    $(that.Settings.GridOvertimeRequestControlSelector).jsGrid("clearInsert");
                }

                // Update Quantity & Meal
                that.UpdateQuantity(-1);
                that.ToggleSaveButton();
            },
            onItemEditCancelling: function (args) {
                that.Settings.IsItemEditing = false;
                that.Settings.IsValidRow = true;
                that.ToggleSaveButton();
            },
            fields:
                that.Settings.Grid.Fields,
        });
    },

    InitControls: function () {
        var that = this;

        $(that.Settings.FromDateControlSelector).datepicker({
            format: 'dd/mm/yyyy',
            autoclose: true
        });

        $(that.Settings.SaveControlSelector).click(function () {
            return false;
        });
        if (!!that.Settings.Id == false) {
            $(that.Settings.SaveControlSelector).prop('disabled', true);
            $(that.Settings.SaveControlSelector).addClass('disable');
        }
    },

    DisableControls: function () {
        var that = this;
        $(that.Settings.DepartmentControlSelector).prop('disabled', true);
        $(that.Settings.FromDateControlSelector).prop('disabled', true);
        $(that.Settings.PlaceControlSelector).prop('disabled', true);
        $(that.Settings.QuantityControlSelector).prop('disabled', true);
        $(that.Settings.ServingControlSelector).prop('disabled', true);
        $(that.Settings.OtherRequestControlSelector).prop('disabled', true);
        $(that.Settings.SaveControlSelector).prop('disabled', true);
        $(that.Settings.SaveControlSelector).addClass('disable');
        $(that.Settings.RejectControlSelector).prop('disabled', true);
        $(that.Settings.RejectControlSelector).addClass('disable');
    },

    ValidateInputDate: function () {
        var that = this;
        $(that.Settings.OvertimeDateErrorSelector).html('');

        try {
            var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidOverTimeDate);
            if (configVal) {
                that.Settings.DiffDays = parseInt(configVal);
            }
        }
        catch (err) { that.Settings.DiffDays = 0; }

        var dateString = $(that.Settings.FromDateControlSelector).val();
        var selectedDate = Functions.parseVietNameseDate(dateString);

        var nowDate = new Date();
        var minDateObj = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
        minDateObj.setDate(minDateObj.getDate() + that.Settings.DiffDays);

        if (selectedDate.valueOf() >= minDateObj.valueOf()) {
            return true;
        }
        else {
            $(that.Settings.OvertimeDateErrorSelector).html(RBVH.Stada.WebPages.Utilities.String.format(decodeURI(that.Settings.OvertimeDateErrorMsgFormat), that.Settings.DiffDays))
            return false;
        }
    },

    SetDefaultValue: function () {
        var that = this;
        $(that.Settings.OtherRequestControlSelector).val('');
        $(that.Settings.CommentsControlSelector).val('');

        try {
            var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidOverTimeDate);
            if (configVal) {
                that.Settings.DiffDays = parseInt(configVal);
            }
        }
        catch (err) { that.Settings.DiffDays = 0; }

        var nowDate = new Date();
        var minDateObj = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
        minDateObj.setDate(minDateObj.getDate() + that.Settings.DiffDays);
        var dateString = Functions.parseVietnameseDateTimeToDDMMYYYY2(minDateObj);

        $(that.Settings.FromDateControlSelector).val(dateString);
        that.Settings.OvertimeDate = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ISODateFormat, minDateObj.getYear() + 1900, minDateObj.getMonth() + 1, minDateObj.getDate());

        $(that.Settings.QuantityControlSelector).val('0');
        $(that.Settings.ServingControlSelector).val('0');
    },

    BindGridColumns: function () {
        var that = this;

        // Build custome fields:
        that.BindFullNameField();
        that.BindOvertimeHourFromField();
        that.BindOvertimeHourToField();
        that.BindTaskField();

        jsGrid.fields.custFullNameField = that.Settings.Grid.CustomFields.FullNameField;
        jsGrid.fields.custOvertimeHourFromField = that.Settings.Grid.CustomFields.OvertimeHourFromField;
        jsGrid.fields.custOvertimeHourToField = that.Settings.Grid.CustomFields.OvertimeHourToField;
        jsGrid.fields.custTaskField = that.Settings.Grid.CustomFields.TaskField;

        that.Settings.Grid.Fields = [
            { name: "ID", title: "ID", readOnly: true, headercss: "hide" },
            { name: "FullName", title: that.Settings.Grid.ColumnTitles.GridColumn_FullName, width: 300, headercss: "header-center", type: "custFullNameField" },
            { name: "OvertimeHourFrom", title: that.Settings.Grid.ColumnTitles.GridColumn_OvertimeHourFrom, width: 150, align: "center", type: "custOvertimeHourFromField" },
            { name: "OvertimeHourTo", title: that.Settings.Grid.ColumnTitles.GridColumn_OvertimeHourTo, width: 150, align: "center", type: "custOvertimeHourToField" },
            { name: "Task", title: that.Settings.Grid.ColumnTitles.GridColumn_Task, type: "text", width: 'auto', headercss: "header-center", type: "custTaskField" },
            { name: "CompanyTransport", title: that.Settings.Grid.ColumnTitles.GridColumn_CompanyTransport, type: "select", items: that.Settings.Grid.CompanyTransportList, valueField: "Id", textField: "Name", width: 150, headercss: "header-center" },
            { type: "control", editButton: false, deleteButton: !that.Settings.IsViewMode, width: 60, modeSwitchButton: false }
        ];
    },

    BindFullNameField: function () {
        var that = this;
        that.Settings.Grid.CustomFields.FullNameField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Grid.CustomFields.FullNameField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                var $item = value;

                return $item.split('_')[0];//$item.text() + '-' + $item.text();
            },
            insertTemplate: function (value) {
                // FilterList
                var employeeList = that.FilterEmployeeList();
                if (employeeList.length > 0) {
                    // Create combobox
                    var $select = $("<select class='full-name form-control' style='width: 150px;' />");
                    $(employeeList).each(function () {
                        $select.append($("<option>").attr('value', this.EmployeeID).text(this.FullName));
                    });

                    // First element:
                    var firstElement = employeeList[0].EmployeeID;
                    var $label = $("<span class='full-name'>").html(' - ' + firstElement);// .html("Loading......");;

                    $select.on('change', function () {
                        $(this).parent().find('span.full-name').html(' - ' + $(this).val()); //.next('td').text($(this).val());
                    });

                    var $div = $("<div>");
                    $div.append($select).append($label);
                    $select.select2()
                    return this._insertFullName = $div;//$select; //$($fullName);
                }
            },

            editTemplate: function (value) {
                // FilterList
                var employeeInfo = value.split('_');
                var employeeCode = employeeInfo[1];

                var $label = $("<span class='full-name'>").html(employeeInfo[0]);// .html("Loading......");;
                var $div = $("<div style='font-weight: bold; text-align: center;'>");//.html(employeeInfo[0]);
                $div.append($label);
                var $hiddenId = $('<input/>', { type: 'hidden', name: 'empId', value: employeeInfo[1] });
                $div.append($hiddenId);//.append($label);

                return this._editFullName = $div;//$select; //$($fullName);
            },
            insertValue: function () {
                if (this._insertFullName) {
                    var $item = this._insertFullName.find('option:selected');
                    return $item.text() + '_' + $item.val();
                }
            },
            editValue: function () {
                var empId = this._editFullName.find('input[type=hidden]:first').val();
                var empName = this._editFullName.text();
                //var $item = this._editFullName.find('.empId');
                return empName + '_' + empId;
            },
        });
    },
    FormatNumber: function (num, size) {
        var s = "000000000" + num;
        return s.substr(s.length - size);
    },

    BindOvertimeHourFromField: function () {
        var that = this;
        that.Settings.Grid.CustomFields.OvertimeHourFromField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Grid.CustomFields.OvertimeHourFromField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                var $item = value;
                var times = $item.split(':');

                return times[0] + ':' + that.FormatNumber(times[1], 2);
            },
            insertTemplate: function (value) {
                return this._insertWorkingHourTime = this._createWorkingHourTimeBox();
            },

            editTemplate: function (value) {
                var workingTimes = value.split(':');
                $result = this._editWorkingHourTime = this._createWorkingHourTimeBox();
                $result[0].value = workingTimes[0];
                $result[2].value = workingTimes[1];

                return $result;
            },
            insertValue: function () {
                var fromHour = $(this._insertWorkingHourTime[0]).val();
                var fromMinute = $(this._insertWorkingHourTime[2]).val();
                return fromHour + ':' + fromMinute;
            },

            editValue: function () {
                var fromHour = $(this._editWorkingHourTime[0]).val();
                var fromMinute = $(this._editWorkingHourTime[2]).val();
                return fromHour + ':' + fromMinute;
            },

            _createWorkingHourTimeBox: function () {
                var $selectHourFrom = $("<select id='workingHourFrom' class='workingHourFrom form-control' style='width: 55px;' />");
                for (var i = 0; i < 24; i++) {
                    $selectHourFrom.append($("<option>").attr('value', i).text(i));
                }

                var $selectMinuteFrom = $("<select id='workingMinuteFrom' class='workingMinuteFrom form-control' style='width: 55px;' />");
                $selectMinuteFrom.append($("<option>").attr('value', 0).text('00'));
                $selectMinuteFrom.append($("<option>").attr('value', 15).text('15'));
                $selectMinuteFrom.append($("<option>").attr('value', 30).text('30'));
                $selectMinuteFrom.append($("<option>").attr('value', 45).text('45'));

                // Feedback 19/7: Load default value from prev value
                $selectHourFrom.val(that.Settings.InsertTemplate.WorkingHourFrom);
                $selectMinuteFrom.val(that.Settings.InsertTemplate.WorkingMinuteFrom);
                return $selectHourFrom.add("<span>:<span>").add($selectMinuteFrom);
                //return $("<input>").attr({ "value": "", "type": "number", "step":"1", "style": "width:40px;display:inline;", "min": "0" }).add("<span>&nbsp;-&nbsp;<span>").add("<input type='number' style='width:40px; display:inline;' min='0'></input>");
            },
        });
    },

    BindOvertimeHourToField: function () {
        var that = this;
        that.Settings.Grid.CustomFields.OvertimeHourToField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Grid.CustomFields.OvertimeHourToField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                var $item = value;
                var times = $item.split(':');

                return times[0] + ':' + that.FormatNumber(times[1], 2);
            },
            insertTemplate: function (value) {
                return this._insertWorkingHourTime = this._createWorkingHourTimeBox();
            },

            editTemplate: function (value) {
                var workingTimes = value.split(':');
                $result = this._editWorkingHourTime = this._createWorkingHourTimeBox();
                $result[0].value = workingTimes[0];
                $result[2].value = workingTimes[1];

                return $result;
            },
            insertValue: function () {
                var fromHour = $(this._insertWorkingHourTime[0]).val();
                var fromMinute = $(this._insertWorkingHourTime[2]).val();
                return fromHour + ':' + fromMinute;
            },

            editValue: function () {
                var fromHour = $(this._editWorkingHourTime[0]).val();
                var fromMinute = $(this._editWorkingHourTime[2]).val();
                return fromHour + ':' + fromMinute;
            },

            _createWorkingHourTimeBox: function () {
                var $selectHourTo = $("<select id='workingHourTo' class='workingHourTo form-control' style='width: 55px;' />");
                for (var i = 0; i < 24; i++) {
                    $selectHourTo.append($("<option>").attr('value', i).text(i));
                }

                var $selectMinuteTo = $("<select id='workingMinuteTo' class='workingMinuteTo form-control' style='width: 55px;' />");
                $selectMinuteTo.append($("<option>").attr('value', 0).text('00'));
                $selectMinuteTo.append($("<option>").attr('value', 15).text('15'));
                $selectMinuteTo.append($("<option>").attr('value', 30).text('30'));
                $selectMinuteTo.append($("<option>").attr('value', 45).text('45'));

                // Feedback 19/7: Load default value from prev value
                $selectHourTo.val(that.Settings.InsertTemplate.WorkingHourTo);
                $selectMinuteTo.val(that.Settings.InsertTemplate.WorkingMinuteTo);

                return $selectHourTo.add("<span>:<span>").add($selectMinuteTo);
                //return $("<input>").attr({ "value": "", "type": "number", "step":"1", "style": "width:40px;display:inline;", "min": "0" }).add("<span>&nbsp;-&nbsp;<span>").add("<input type='number' style='width:40px; display:inline;' min='0'></input>");
            },
        });
    },

    BindTaskField: function () {
        var that = this;
        that.Settings.Grid.CustomFields.TaskField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Grid.CustomFields.TaskField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                if (value == null || value == '')
                    return value;

                var $item = value;
                if ($item.split('_').length > 1) {
                    var hyperlinkNotOT = '<a class="viewDetail" href="#" data-url="' + $item.split('_')[1] + '">' + that.Settings.ViewDetailTitle + '</a>';//;

                    return $item.split('_')[0] + ' - ' + hyperlinkNotOT;
                }

                return value;
            },
            insertTemplate: function (value) {
                var $taskTextbox = $('<input/>', { type: 'text', name: 'txtOvertimeTask', value: value, maxlength: 254 });
                $taskTextbox.val(that.Settings.InsertTemplate.Task);
                return this._insertTask = $taskTextbox;
            },
            editTemplate: function (value) {
                var $taskTextbox = $('<input/>', { type: 'text', name: 'txtOvertimeTask', value: value, maxlength: 254 });

                return this._editTask = $taskTextbox;
            },
            insertValue: function () {
                return this._insertTask.val();
            },
            editValue: function () {
                return this._editTask.val();
            },
        });
    },

    /*
        Business 
    */

    ToggleSaveButton: function () {
        var isDirty = false;
        var isEmptyApproval = false;
        var isValidDetail = true;
        if (!!this.Settings.Id == false) // Add new
        {
            if (this.Settings.OvertimeJsonArray.length == 0) {
                isDirty = true;
            }
        }
        else {
            isDirty = !this.Settings.IsValidMaster;

            if (this.Settings.View == "Approval") { // Check empty grid -> disable
                isEmptyApproval = this.Settings.OvertimeJsonArray.length == 0; // && this.Settings.EmployeeListDelete.length == 0;
                //this.Settings.IsReject = isEmptyApproval;
            }
            else {
                isValidDetail = !this.Settings.OvertimeJsonArray.length == 0;
            }
        }

        if (isDirty || isEmptyApproval || !this.Settings.IsValidRow || !this.Settings.IsValidMaster || !isValidDetail || this.Settings.IsItemEditing) {
            $(this.Settings.SaveControlSelector).prop('disabled', true);
            $(this.Settings.SaveControlSelector).addClass('disable');
            if ($(this.Settings.RejectControlSelector).length > 0) {
                $(this.Settings.RejectControlSelector).prop('disabled', true);
                $(this.Settings.RejectControlSelector).addClass('disable');
            }
        }
        else {
            $(this.Settings.SaveControlSelector).prop('disabled', false);
            $(this.Settings.SaveControlSelector).removeClass('disable');
            if ($(this.Settings.RejectControlSelector).length > 0) {
                $(this.Settings.RejectControlSelector).prop('disabled', false);
                $(this.Settings.RejectControlSelector).removeClass('disable');
            }
        }
        //this.Settings.IsValidGridDetails = !isDirty;
    },

    CheckDirtyGrid: function () {
        // Add new
        if (!!this.Settings.Id == false)
            return this.Settings.OvertimeJsonArray.length == 0;
        else
            return that.Settings.EmployeeListAdd.push.apply(that.Settings.EmployeeListAdd, that.Settings.EmployeeListEdit, that.Settings.EmployeeListDelete).length == 0;
        //var employeeList = that.Settings.EmployeeListAdd.push.apply(that.Settings.EmployeeListAdd, that.Settings.EmployeeListEdit, that.Settings.EmployeeListDelete);
    },

    FilterEmployeeList: function (currentEmployeeCode) {
        var that = this;
        var result = that.Settings.EmployeeList.filter(function (item) {
            return (typeof currentEmployeeCode != 'undefined' && item.EmployeeID == currentEmployeeCode) || ($.inArray(item.EmployeeID, that.Settings.AddedEmployeeCodeList) < 0);
        });

        $.each(this.Settings.EmployeeListDelete, function (i, v) {
            result.push({
                FullName: v.FullName.split('_')[0],
                EmployeeID: that.GetEmployeeCodeById(v.FullName.split('_')[1])
            });
        });

        return result;
    },

    PopulateEmployeeList: function () {
        var that = this;
        this.GetEmployeeList();
        this.GetFullEmployeeList();
        if (!!this.Settings.Id) {
            this.Settings.AddedEmployeeCodeList = $.map(this.Settings.OvertimeJsonArray, function (val, i) {
                var code = that.GetEmployeeCodeById(val.Employee.LookupId);
                return code;
            });
        }
    },

    GetEmployeeList: function () {
        var that = this;
        var departmentId = $(that.Settings.DepartmentControlSelector).val();
        var locationId = that.Settings.LocationId;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.EmployeeNotOvertimeList, location.host, departmentId, locationId, that.Settings.OvertimeDate);

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function (result) {
                that.Settings.EmployeeList = result;
            }
        });
    },

    GetFullEmployeeList: function () {
        var that = this;
        var departmentId = $(that.Settings.DepartmentControlSelector).val();
        var locationId = that.Settings.LocationId;
        var maxLevel = 6; // Quản lý trực tiếp
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.EmployeeList, location.host, locationId, departmentId, maxLevel);

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function (result) {
                that.Settings.FullEmployeeList = result;
            }
        });
    },

    ValidateHours: function (workingHourFromValue, workingHourToValue) {
        //var that = this;
        //var durationHour = 0;
        //var validDates = false;
        //if (workingHourFromValue == workingHourToValue)
        //    return validDates;
        //if (workingHourFromValue < workingHourToValue) {
        //    durationHour = workingHourToValue - workingHourFromValue;
        //    validDates = durationHour <= that.Settings.DefaultDurationHour ? true : false;
        //}
        //else if (workingHourFromValue <= 23 && workingHourFromValue >= 16) {
        //    /*
        //        23 - 7
        //        22 - 6
        //        21 - 5
        //        20 - 4
        //        19 - 3
        //        18 - 2
        //        17 - 1
        //        16 - 0
        //    */
        //    validDates = (24 + workingHourToValue) - workingHourFromValue <= that.Settings.DefaultDurationHour ? true : false;
        //}

        //return validDates;
        //return true; // Ignore validating dates: Some departments can work overtime > 8h ...

        // Feedback 26/07/2017
        return !(workingHourFromValue == 12 && workingHourToValue == 13);
    },

    GetEmployeeIdByCode: function (code) {
        if (code != '') {
            var id = '';
            $.each(this.Settings.FullEmployeeList, function (i, v) {
                if (v.EmployeeID == code) {
                    id = v.ID;
                    return;
                }
            });
            return id;
        }

        return "";
    },

    GetEmployeeCodeById: function (id) {
        if (id != '') {
            var code = '';
            $.each(this.Settings.FullEmployeeList, function (i, v) {
                if (v.ID == id) {
                    code = v.EmployeeID;
                    return;
                }
            });
            return code;
        }

        return "";
    },

    RefreshEmployeeAddList: function (fullName) {
        var that = this;
        if (fullName != '') {
            var index = -1;
            $.each(this.Settings.EmployeeListAdd, function (i, v) {
                if (v.FullName == fullName) {
                    index = i;
                    return;
                }
            });
            if (index >= 0)
                that.Settings.EmployeeListAdd.splice(index, 1);
        }
    },

    RefreshEmployeeEditList: function (fullName) {
        var that = this;
        if (fullName != '') {
            var index = -1;
            $.each(this.Settings.EmployeeListEdit, function (i, v) {
                if (v.FullName == fullName) {
                    index = i;
                    return;
                }
            });
            if (index >= 0)
                that.Settings.EmployeeListEdit.splice(index, 1);
        }
    },

    RemoveRequiredError: function () {
        var that = this;
        $(that.Settings.DepartmentControlSelector).removeClass('required-error');
        //$(that.Settings.LocationControlSelector).removeClass('required-error');
        $(that.Settings.QuantityControlSelector).removeClass('required-error');
        $(that.Settings.ServingControlSelector).removeClass('required-error');
        $(that.Settings.FromDateControlSelector).removeClass('required-error');
    },

    ValidateFields: function () {
        var that = this;
        that.RemoveRequiredError();
        var valid = true;
        if ($(that.Settings.DepartmentControlSelector).val() == null || $(that.Settings.DepartmentControlSelector).val() == '') {
            $(that.Settings.DepartmentControlSelector).addClass('required-error');
            valid = false;
        }
        if ($(that.Settings.QuantityControlSelector).val() == '') {
            $(that.Settings.QuantityControlSelector).addClass('required-error');
            valid = false;
        }
        if ($(that.Settings.ServingControlSelector).val() == '') {
            $(that.Settings.ServingControlSelector).addClass('required-error');
            valid = false;
        }
        if ($(that.Settings.FromDateControlSelector).val() == '') {
            $(that.Settings.FromDateControlSelector).addClass('required-error');
            valid = false;
        }
        else {
            if (!!that.Settings.Id == false) {
                var nowDate = new Date();
                var currentDate = new Date(nowDate.getFullYear(), nowDate.getMonth(), nowDate.getDate());
                var fromDateValue = $(that.Settings.FromDateControlSelector).val().split('/');
                var fromDate = new Date(fromDateValue[2], fromDateValue[1] - 1, fromDateValue[0]);
                if (fromDate.valueOf() < currentDate.valueOf()) {
                    $(that.Settings.FromDateControlSelector).addClass('required-error');
                    valid = false;
                }
            }
        }

        return valid;
    },

    UpdateQuantity: function (num) {
        var that = this;
        var quantity = $(that.Settings.QuantityControlSelector).val();
        var serving = $(that.Settings.ServingControlSelector).val();
        if (!isNaN(quantity))
            quantity = parseInt(quantity) + num;
        if (!isNaN(serving))
            serving = parseInt(serving) + num;
        quantity = quantity < 0 ? 0 : quantity;
        serving = serving < 0 ? 0 : serving;
        $(that.Settings.QuantityControlSelector).val(quantity);
        $(that.Settings.ServingControlSelector).val(serving);
    },

    OnBeginSaveData: function () {
        var that = this;
        $(that.Settings.ShaderCssClass).show();
        $(that.Settings.LoadPanelCssClass).show();
        $(that.Settings.SaveControlSelector).prop('disabled', true);
        $(that.Settings.SaveControlSelector).addClass('disable');
        $(that.Settings.RejectControlSelector).prop('disabled', true);
        $(that.Settings.RejectControlSelector).addClass('disable');
    },

    OnEndSaveData: function () {
        var that = this;
        $(that.Settings.ShaderCssClass).hide();
        $(that.Settings.LoadPanelCssClass).hide();
        $(that.Settings.SaveControlSelector).prop('disabled', false);
        $(that.Settings.SaveControlSelector).removeClass('disable');
        $(that.Settings.RejectControlSelector).prop('disabled', false);
        $(that.Settings.RejectControlSelector).removeClass('disable');
    },

    SetMasterObject: function () {
        var that = this;
        var overTimeModel = {};

        var overTimeModel = {};

        overTimeModel.ApprovalStatus = "";
        overTimeModel.CommonDepartment = {};
        overTimeModel.CommonDepartment.LookupId = that.Settings.Requester.DepartmentId || $(that.Settings.DepartmentControlSelector).val();
        overTimeModel.CommonLocation = {};
        overTimeModel.CommonLocation.LookupId = that.Settings.LocationId; //$(that.Settings.LocationControlSelector).val();
        overTimeModel.Place = $(that.Settings.PlaceControlSelector).val();
        overTimeModel.SumOfEmployee = $(that.Settings.QuantityControlSelector).val();
        overTimeModel.SumOfMeal = $(that.Settings.ServingControlSelector).val();
        overTimeModel.OtherRequirements = $(that.Settings.OtherRequestControlSelector).val();

        overTimeModel.Requester = {};
        overTimeModel.Requester.LookupId = that.Settings.Requester.Id;
        overTimeModel.Requester.LookupValue = that.Settings.Requester.Name;
        overTimeModel.ApprovedBy = that.Settings.ApprovedBy || {};
        overTimeModel.ApproverFullName = that.Settings.ApproverFullName != '' ? that.Settings.ApproverFullName : overTimeModel.ApprovedBy.FullName;
        overTimeModel.ID = that.Settings.OvertimeRequestId;

        overTimeModel.OvertimeDetailModelList = [];
        return overTimeModel;
    },

    OnSaveData: function () {
        var that = this;

        if (that.Settings.Id === 0 && that.ValidateInputDate() === false) {
            return false;
        }

        that.OnBeginSaveData();

        var childArray = that.Settings.OvertimeJsonArray; //that.Settings.EmployeeListAdd.concat(that.Settings.EmployeeListEdit).concat(that.Settings.EmployeeListDelete);
        if (childArray.length == 0 && !!that.Settings.Id == false) {
            that.OnEndSaveData();
            return;
        }

        var overTimeModel = that.SetMasterObject();

        var overtimeDetailModel = {};
        var fromDateValue = $(that.Settings.FromDateControlSelector).val().split('/');
        var fromDate = new Date(fromDateValue[2], fromDateValue[1] - 1, fromDateValue[0]);
        var day = fromDate.getDate();
        var month = fromDate.getMonth();
        var year = fromDate.getYear() + 1900;
        overTimeModel.Date = fromDate.toDateString();
        var nextDate = fromDate;

        for (var i = 0 ; i < childArray.length ; i++) {
            overtimeDetailModel = {};
            overtimeDetailModel.ID = childArray[i].ID != '' ? parseInt(childArray[i].ID) : 0;

            var workingHourFrom = parseInt(childArray[i].OvertimeHourFrom.split(':')[0]);
            var workingMinuteFrom = parseInt(childArray[i].OvertimeHourFrom.split(':')[1]);

            var workingHourTo = parseInt(childArray[i].OvertimeHourTo.split(':')[0]);
            var workingMinuteTo = parseInt(childArray[i].OvertimeHourTo.split(':')[1]);

            var workingDateFrom = new Date(year, month, day, workingHourFrom, workingMinuteFrom, 0);
            if (!!childArray[i].OvertimeTo) {

            }
            var workingDateTo = new Date(year, month, day, workingHourTo, workingMinuteTo, 0);
            var workingDuration = 0;
            //if (workingHourFrom > 12 && workingHourTo < 12) // Next date
            if (workingHourTo < workingHourFrom) // Next date
            {
                nextDate.setDate(nextDate.getDate() + 1);
                workingDateTo = new Date(nextDate.getYear() + 1900, nextDate.getMonth(), nextDate.getDate(), workingHourTo, workingMinuteTo, 0);

                workingDuration = (24 + workingHourTo) - workingHourFrom;
            }
            else {
                workingDuration = workingHourTo - workingHourFrom;
            }

            var employeeId = childArray[i].FullName.split('_')[1];
            var realEmployeeId = that.GetEmployeeIdByCode(employeeId);
            employeeId = realEmployeeId == '' ? parseInt(employeeId) : realEmployeeId
            overtimeDetailModel.Employee = {};
            overtimeDetailModel.Employee.LookupId = employeeId; //overtimeDetailModel.ID != 0 ? parseInt(employeeId) : that.GetEmployeeIdByCode(employeeId);

            overtimeDetailModel.OvertimeFrom = workingDateFrom.toISOString();
            overtimeDetailModel.OvertimeTo = workingDateTo.toISOString();

            overtimeDetailModel.Task = childArray[i].Task;
            overtimeDetailModel.CompanyTransport = childArray[i].CompanyTransport;
            //overtimeDetailModel.HM = childArray[i].HM;
            //overtimeDetailModel.KD = childArray[i].KD;

            var minutesDiff = workingMinuteTo - workingMinuteFrom;
            minutesDiff = minutesDiff / 60;
            overtimeDetailModel.WorkingHours = (workingDuration + minutesDiff).toString();

            // Check Break-time 'Ca HC': 12h -> 13h
            if (workingHourFrom <= 12 && workingHourTo >= 13)
                overtimeDetailModel.WorkingHours -= 0.75;

            overtimeDetailModel.WorkingHours = Math.round(overtimeDetailModel.WorkingHours);
            overTimeModel.OvertimeDetailModelList.push(overtimeDetailModel);

            //if (overtimeDetailModel.WorkingHours > 0.25) // 12h -> 13h => IGNORE
            //    overTimeModel.OvertimeDetailModelList.push(overtimeDetailModel);

            nextDate = new Date(year, month, day);
        }

        if (that.Settings.View == "Approval") {
            // Update approver: Current user
            overTimeModel.ApprovedBy = {
                UserName: _rbvhContext.EmployeeInfo.ADAccount.UserName,
                ID: _rbvhContext.EmployeeInfo.ADAccount.ID
            };

            if (that.Settings.IsReject) {
                overTimeModel.ApprovalStatus = false;
                if (that.Settings.BODApprover != null) {
                    overTimeModel.RequiredBODApprove = true;
                }
            }
            else {
                if (that.Settings.OvertimeJsonArray.length > 0) // If empty grid -> Keep current approver
                {
                    // Current user is BOD: Approve
                    if (that.Settings.CurrentUserInfo.RoleId == that.Settings.Roles.BOD) {
                        overTimeModel.ApprovalStatus = true;
                        overTimeModel.RequiredBODApprove = true;
                        //overTimeModel.BODComments = $(that.Settings.CommentsControlSelector).val();
                    }
                    else {
                        // SET BOD is Approver
                        overTimeModel.DHComments = $(that.Settings.CommentsControlSelector).val();
                        if (that.Settings.BODApprover != null) {
                            //overTimeModel.DHComments = $(that.Settings.CommentsControlSelector).val();
                            overTimeModel.ApprovedBy = that.Settings.BODApprover;
                            overTimeModel.RequiredBODApprove = true;
                            overTimeModel.ApprovalStatus = '';
                        }
                        else {
                            overTimeModel.ApprovalStatus = true;
                        }
                    }
                }
            }

            // BOD
            if (that.Settings.CurrentUserInfo.RoleId == that.Settings.Roles.BOD) {
                overTimeModel.BODComments = $(that.Settings.CommentsControlSelector).val();
            }
            else {
                overTimeModel.FirstApprovedBy = {
                    UserName: _rbvhContext.EmployeeInfo.ADAccount.UserName,
                    ID: _rbvhContext.EmployeeInfo.ADAccount.ID
                };
                overTimeModel.DHComments = $(that.Settings.CommentsControlSelector).val();
            }
        }

        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.GetModifiedDate, location.host, "3", overTimeModel.ID); //"3 = StepModuleList.OvertimeManagement"
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response.Code == 0 && response.Message !== that.Settings.Modified) {
                $.confirm({
                    title: that.Settings.Modal.Title,
                    content: that.Settings.DataChanged_Msg,
                    buttons: {
                        save: {
                            text: that.Settings.Modal.SaveButton,
                            action: function () {
                                that.SubmitData(overTimeModel);
                            }
                        },
                        reload: {
                            text: that.Settings.Modal.ReloadButton,
                            btnClass: 'btn-blue',
                            action: function () {
                                window.location.reload();
                            }
                        },
                        close: {
                            text: that.Settings.Modal.CloseButton,
                            action: function () {
                                that.OnEndSaveData();
                            }
                        },
                    }
                });
            }
            else if (response.Code != 0) {
                that.OnEndSaveData();
            }
            else {
                that.SubmitData(overTimeModel);
            }
        });
    },
    SubmitData: function (overTimeModel) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.OvertimeSubmit, location.host);
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(overTimeModel),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && (response.Code === 1 || response.Code === 2 || response.Code === 3 || response.Code === 4)) {
                alert(response.Message);
            }
            else if (response && response.Code === 999) {
                alert(response.Message);
                window.location.reload();
            }

            that.Settings.OvertimeRequestId = response.ObjectId;
            $(that.Settings.ShaderCssClass).hide();
            $(that.Settings.LoadPanelCssClass).hide();
            $(that.Settings.SaveControlSelector).prop('disabled', false);
            $(that.Settings.SaveControlSelector).removeClass('disable');

            //// Refresh data
            //$(that.Settings.GridOvertimeRequestControlSelector).jsGrid("loadData");
            //that.Settings.OvertimeJsonArray = [];
            that.Settings.EmployeeListAdd = [];
            that.Settings.EmployeeListEdit = [];
            that.Settings.EmployeeListDelete = [];
            that.Settings.AddedEmployeeCodeList = [];

            var sourceUrl = Functions.getParameterByName("Source");
            if (sourceUrl && sourceUrl.length > 0) {
                window.location.href = sourceUrl;
            }
            else {
                window.location = that.Settings.PrevURL;//RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ListPageUrl, location.host);
            }
        });
    },
    ValidateEmployeeList: function (OnSaveData) {
        var that = this;
        var departmentId = $(that.Settings.DepartmentControlSelector).val();
        var locationId = that.Settings.LocationId;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.EmployeeNotOvertimeList, location.host, departmentId, locationId, that.Settings.OvertimeDate);

        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            cache: false,
            success: function (result) {
                if (typeof OnSaveData == 'function') {
                    that.Settings.RegisteredEmployeeList = [];

                    var notOvertimeList = result;
                    var newEmployeeList = that.Settings.EmployeeListAdd;

                    $(newEmployeeList).each(function () {
                        var employeeId = this.FullName.split('_')[1];
                        var existed = true;
                        $.grep(notOvertimeList, function (item) {
                            if (item.EmployeeID == employeeId)
                                existed = false;
                        });
                        if (existed == true) {
                            that.Settings.RegisteredEmployeeList.push(this);
                        }
                    });

                    if (that.Settings.RegisteredEmployeeList.length > 0) {
                        // TODO: Resource
                        alert(that.Settings.RegisteredEmployeeMessage);
                    }
                    else {
                        // Save Data
                        var fnSaveData = OnSaveData.bind(that);
                        fnSaveData();
                    }
                }
            }
        });
    },

    RenderOvertimeHistory: function () {
        var that = this;
        var taskList = [];
        var overtimeModel = that.Settings.ApprovalHistory;

        if (overtimeModel.ApprovedLevel == 1) // Truong phong duyet
        {
            var dhApproval = {};
            dhApproval.Status = overtimeModel.ApprovalStatus == 'false' ? that.Settings.ApprovalStatus_Rejected : that.Settings.ApprovalStatus_Approved;
            dhApproval.Approver = overtimeModel.FirstApprovedBy.FullName;
            dhApproval.Date = overtimeModel.FirstApprovedDate;
            dhApproval.Comment = overtimeModel.DHComments;
            taskList.push(dhApproval);
        }
        else if (overtimeModel.ApprovedLevel == 2) // BOD duyet
        {
            var dhApproval = {};
            dhApproval.Status = that.Settings.ApprovalStatus_Approved;
            dhApproval.Approver = overtimeModel.FirstApprovedBy.FullName;
            dhApproval.Date = overtimeModel.FirstApprovedDate;
            dhApproval.Comment = overtimeModel.DHComments;
            taskList.push(dhApproval);

            var bodApproval = {};
            bodApproval.Status = overtimeModel.ApprovalStatus == 'false' ? that.Settings.ApprovalStatus_Rejected : that.Settings.ApprovalStatus_Approved;
            bodApproval.Approver = overtimeModel.ApprovedBy.FullName;
            bodApproval.Date = overtimeModel.Modified;
            bodApproval.Comment = overtimeModel.BODComments;
            taskList.push(bodApproval);

        }

        var noDataAvailableMsg = that.Settings.NoDataAvaibleMsg; // Resource


        var tableHtml = '<table class="table gridView" style="border-collapse: collapse;" border="1" rules="all" cellspacing="0">';
        if (taskList && taskList.length > 0) {
            var colStatus = that.Settings.ApprovalStatusTitle; // Resource
            var colApprover = that.Settings.PostedByTitle; // Resource
            var colDate = that.Settings.DateTitle; // Resource
            var colComment = that.Settings.CommentTitle; // Resource

            tableHtml += '<tr><th scope="col">' + colStatus + '</th><th scope="col">' + colApprover + '</th><th scope="col">' + colDate + '</th><th scope="col">' + colComment + '</th></tr>';
            for (var i = 0; i < taskList.length; i++) {
                tableHtml += '<tr><td style="width: 200px;">' + taskList[i].Status + '</td><td style="width: 250px;">' + taskList[i].Approver + '</td><td style="width: 200px;">' + taskList[i].Date + '</td>' + '<td style="width: 200px;">' + taskList[i].Comment + '</td></tr>';
            }
        }
        else {
            tableHtml += '<span>' + noDataAvailableMsg + '</span>';
        }
        tableHtml += '</table>';

        $(that.Settings.ApprovalHistoryContainerSelector).html(tableHtml);
    }

};
