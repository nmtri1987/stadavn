RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");
RBVH.Stada.WebPages.pages.ShiftRequest = function (settings) {
    this.Protocol = window.location.protocol; // http: or https:
    this.Settings = {
        ServiceUrls:
        {
            DepartmentList: this.Protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentForShift/{1}/{2}',
            ShiftTimeList: this.Protocol + '//{0}/_vti_bin/Services/ShiftTime/ShiftTimeService.svc/GetShiftTimes',
            ShiftRequestList: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/GetByPreviousShift',
            //ShiftRequestById: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/GetShiftManagementById',
            ShiftRequestGet: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/GetShiftManagementById/{1}',
            //ShiftRequestSubmit: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/InsertShiftManagement',
            //ShiftRequestUpdate: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/UpdateShiftManagement',
            ShiftRequestExport: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/ExportShifts',
            CalendarList: this.Protocol + '//{0}/_vti_bin/Services/Calendar/CalendarService.svc/GetHolidayInRange/{1}/{2}',
            UserInformation: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetCurrentUser',
            Approvers: this.Protocol + '//{0}/_vti_bin/Services/Employee/EmployeeService.svc/GetEmployeeApprovers/{1}',
            LeavesByDepartment: this.Protocol + '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/GetLeavesInRangeByDepartment/{1}/{2}/{3}/{4}', // {LocationId}/{DepartmentId}/{FromDate}/{ToDate}
            //LeavesByEmployee: this.Protocol + '//{0}/_vti_bin/Services/leavemanagement/leavemanagementservice.svc/GetLeavesInRange/{1}/{2}/{3}/{4}/{5}', // {employeeID}/{DepartmentId}/{LocationId}/{FromDate}/{ToDate}
            ImportFile: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/ImportShifts/{1}/{2}/{3}/{4}/{5}',
            IsDelegated: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/IsDelegated/{1}/{2}', // {fromApprover}

            // Load test:
            //ShiftManagementMasterInsert: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/InsertShiftManagementMaster',
            //ShiftManagementDetailInsert: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/InsertShiftManagementDetail',
            ShiftManagementMasterDetailInsert: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/InsertShiftManagementMasterDetail',
            ShiftManagementDetailApprove: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/ApproveShiftManagementDetail',
            //ShiftManagementDetailRefresh: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/RefreshShiftManagementDetail',
            ShiftManagementSendAdminApprovalEmail: this.Protocol + '//{0}/_vti_bin/Services/ShiftManagement/ShiftManagementService.svc/SendAdminApprovalEmail',
            GetConfigurations: '//{0}/_vti_bin/Services/Configurations/ConfigurationsService.svc/GetConfigurations'
        },

        ShiftImportReturnUrl: this.Protocol + '//{0}/SitePages/ShiftRequest.aspx?subSection=ShiftManagement&itemid={1}&Source={2}',
        Configurations: {},
        ConfigKey_ValidInputDate: "ShiftForm_ValidInputDate",
        Id: settings.Id,
        View: settings.View,
        ISODateFormat: '{0}-{1}-{2}',

        BeginDate: 21,
        EndDate: 20,
        MaxDate: 31,
        DaysInMonth: 30,
        CurrentMonth: 3,
        CurrentYear: 2017,

        StartDateRange: '',
        EndDateRange: '',

        ShiftTimes: [
            { Name: "", Id: 0 },
            { Name: "Ca 1", Id: 1 },
            { Name: "Ca 2", Id: 2 },
            { Name: "Ca 3", Id: 3 }
        ],
        ShiftTimeCodes: [],

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
        //--- Highlight
        NonWorkingDays: [],
        NonWorkingDaysNM1: [],
        NonWorkingDaysNM2: [],

        StartIndex: 2, // Highlight Date 21: index = 2
        Start1StDateIndex: 13, // Highlight Date 1: index = 13
        //---
        EmployeeJsonArray: [],

        // Shift Id
        ShiftManagementId: 0,
        ShiftManagementDataSource: [],

        // GRID
        Grid:
        {
            CustomFields: {
                ShiftTimeField: null,
            }
        },

        // Indicator:
        ShaderCssClass: '.jsgrid-load-shader',
        LoadPanelCssClass: '.jsgrid-load-panel',

        // Others
        Fields: [],
        ReadOnlyGrid: false,

        // Personal information
        DepartmentId: 0,
        LocationId: 0,
        LocationName: '',

        // ADMIN ?
        IsSystemAdmin: false,

        // Check dirty:
        ItemBeforeEdit: null,
        ItemAfterEdit: null,
        GridBeforeEdit: null,
        LeavesByDepartmentArray: [],
        ModifiedBy: null,
        ApproverFullName: '',
        RequestDueDate: '',
        RequestExpired: false,
    };

    $.extend(true, this.Settings, settings);

    this.Initialize();
};
RBVH.Stada.WebPages.pages.ShiftRequest.prototype = {
    Initialize: function () {
        var that = this;
        that.Settings.Id = RBVH.Stada.WebPages.Utilities.GetValueByParam('itemId');
        var prevURL = RBVH.Stada.WebPages.Utilities.GetValueByParam('Source');
        prevURL = !!prevURL ? decodeURIComponent(prevURL) : document.referrer;
        that.Settings.PrevURL = prevURL;
        var listMode = RBVH.Stada.WebPages.Utilities.GetValueByParam('Mode');
        if (listMode && listMode.toUpperCase() == 'EDIT') {
            that.Settings.View = 'Edit';
        }
        else if (listMode && listMode.toUpperCase() == 'VIEW') {
            that.Settings.View = 'View';
        }
        ExecuteOrDelayUntilScriptLoaded(function () {
            that.GetConfigurations();
            that.InitControls();
            that.SetDefaultValue();
            that.PopulateData();
            that.RegisterEvents();
        }, "sp.js");
    },
    GetConfigurations: function () {
        var that = this;
        var postData = [that.Settings.ConfigKey_ValidInputDate];
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
        $(that.Settings.MonthControlSelector).on('changeDate', function (ev) {
            if (that.Settings.EmployeeJsonArray.length > 0 && !that.CheckIsDirty()) {
                var mRes = confirm(that.Settings.SaveCurrentDataConfirmMessage);
                if (mRes == true) {
                    that.OnSaveData();
                }
                else {
                    that.Settings.EmployeeJsonArray = [];
                }
            }

            var diffDays = 1;
            try {
                var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidInputDate);
                if (configVal) {
                    diffDays = parseInt(configVal);
                }
            }
            catch (err) { diffDays = 1; }

            var today = new Date();
            var day = today.getDate() + diffDays; // 0 -> 11
            var month = today.getMonth(); // 0 -> 11
            //month = month == 0 ? 12 : month;
            var year = today.getYear() + 1900;
            var dpMonth = ev.date.getMonth(); // 0 -> 11
            //dpMonth = dpMonth == 0 ? 12 : dpMonth;
            that.Settings.CurrentMonth = dpMonth + 1;
            var dpYear = ev.date.getYear() + 1900;
            //dpYear = dpMonth == 12 ? dpYear - 1 : dpYear;
            that.Settings.CurrentYear = dpYear;
            that.Settings.ReadOnlyGrid = false;
            if (that.Settings.View == "Approval") {
                //that.Settings.ReadOnlyGrid = true;
            }
            else {
                if (dpYear < year)
                    that.Settings.ReadOnlyGrid = true;
                else if (dpYear == year) {
                    if (dpMonth < month)
                        that.Settings.ReadOnlyGrid = true;
                    else if (dpMonth == month) {
                        if (day > 20)
                            that.Settings.ReadOnlyGrid = true;
                    }
                }
            }

            that.Settings.DaysInMonth = new Date(dpYear, dpMonth, 0).getDate();
            $('.datepicker').hide();
            // Get non-working dates

            that.CalculateDateRange();
            that.GetNonWorkingDays();
            that.PopulateLeavesByDepartment();
            // Reload
            $(that.Settings.GridEmployeeControlSelector).jsGrid("destroy");
            that.PopulateGrid();
        });

        $(that.Settings.DepartmentControlSelector).on('change', function () {
            // Set Department Id
            that.Settings.DepartmentId = $(this).val();
            if (that.Settings.EmployeeJsonArray.length > 0) {
                var mRes = confirm(that.Settings.SaveCurrentDataConfirmMessage);
                if (mRes == true) {
                    that.OnSaveData();
                }
                else {
                    that.Settings.EmployeeJsonArray = [];
                }
            }
            $(that.Settings.GridEmployeeControlSelector).jsGrid("loadData");
        });

        // Submit data
        $(that.Settings.SaveControlSelector).click(function () {
            // Load test
            that.OnSaveData();
            //that.SaveMaster();
            return false;
        });

        $(that.Settings.ExportControlSelector).click(function () {
            if (that.Settings.ShiftManagementId && that.Settings.ShiftManagementId > 0) {
                url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftRequestExport, location.host) + "/" + that.Settings.ShiftManagementId;
                window.location = url;
            }
            return false;
        });

        $(that.Settings.CancelControlSelector).click(function () {
            //that.OnRefreshData();
            window.location = that.Settings.PrevURL;
            return false;
        });

        $(that.Settings.FileSelector).on("change", function () {
            $(that.Settings.ImportControlSelector).removeAttr("disabled");
            $(that.Settings.ImportControlSelector).removeClass("disable");
        });
        $(that.Settings.ImportControlSelector).on("click", function (e) {
            // Load Indicator:
            $(".se-pre-con").fadeIn(0);
            that.OnUploadFileData();
            e.preventDefault();
        });
    },
    OnUploadFileData: function () {
        var that = this;
        uploadFile(that);

        // Upload the file.
        // You can upload files up to 2 GB with the REST API.
        function uploadFile(that) {
            // Define the folder path for this example.

            var serverRelativeUrlToFolder = '/Shared%20Documents';
            // Get test values from the file input and text input page controls.

            var fileInput = $("#ShiftFileUpload");
            var newName = "";
            if (!fileInput || !fileInput[0].files[0]) {
                alert(that.Settings.InputFileErrorMessage);
                $(".se-pre-con").fadeOut(0);
                return;
            }
            else {
                newName = fileInput[0].files[0].name.toString();
            }
            if (newName == "")
                return;
            var currentTime = new Date().format('mdyhms');
            newName = (currentTime + newName);
            // Get the server URL.
            var serverUrl = _spPageContextInfo.webAbsoluteUrl;

            // Initiate method calls using jQuery promises.
            // Get the local file as an array buffer.
            var getFile = getFileBuffer();
            getFile.done(function (arrayBuffer) {
                // Add the file to the SharePoint folder.
                var addFile = addFileToFolder(arrayBuffer);
                addFile.done(function (file, status, xhr) {
                    // Get the list item that corresponds to the uploaded file.
                    var getItem = getListItem(file.d.ListItemAllFields.__deferred.uri);
                    getItem.done(function (listItem, status, xhr) {
                        that.ProcessUploading(newName);

                        //// Change the display name and title of the list item.
                        //var changeItem = updateListItem(listItem.d.__metadata);
                        //changeItem.done(function (data, status, xhr) {
                        //    that.ProcessUploading(newName);
                        //    // alert('file uploaded and updated');
                        //});

                        //changeItem.fail(onError);
                    });
                    getItem.fail(onError);
                });
                addFile.fail(onError);
            });
            getFile.fail(onError);

            // Get the local file as an array buffer.
            function getFileBuffer() {
                var deferred = jQuery.Deferred();
                var reader = new FileReader();
                reader.onloadend = function (e) {
                    deferred.resolve(e.target.result);
                }
                reader.onerror = function (e) {
                    deferred.reject(e.target.error);
                }
                reader.readAsArrayBuffer(fileInput[0].files[0]);
                return deferred.promise();
            }

            // Add the file to the file collection in the Shared Documents folder.
            function addFileToFolder(arrayBuffer) {
                // Get the file name from the file input control on the page.
                var parts = fileInput[0].value.split('\\');
                //var fileName = parts[parts.length - 1];
                var fileName = newName;
                // Construct the endpoint.
                var fileCollectionEndpoint = String.format(
                    "{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files" +
                    "/add(overwrite=true, url='{2}')",
                    serverUrl, serverRelativeUrlToFolder, fileName);

                // Send the request and return the response.
                // This call returns the SharePoint file.
                return jQuery.ajax({
                    url: fileCollectionEndpoint,
                    type: "POST",
                    data: arrayBuffer,
                    processData: false,
                    headers: {
                        "accept": "application/json;odata=verbose",
                        "X-RequestDigest": jQuery("#__REQUESTDIGEST").val(),
                        "content-length": arrayBuffer.byteLength
                    }
                })
            }

            // Get the list item that corresponds to the file by calling the file's ListItemAllFields property.
            function getListItem(fileListItemUri) {

                // Send the request and return the response.
                return jQuery.ajax({
                    url: fileListItemUri,
                    type: "GET",
                    headers: { "accept": "application/json;odata=verbose" }
                });
            }

            // Change the display name and title of the list item.
            function updateListItem(itemMetadata) {

                // Define the list item changes. Use the FileLeafRef property to change the display name. 
                // For simplicity, also use the name as the title. 
                // The example gets the list item type from the item's metadata, but you can also get it from the
                // ListItemEntityTypeFullName property of the list.
                var body = String.format("{{'__metadata':{{'type':'{0}'}},'FileLeafRef':'{1}','Title':'{2}'}}",
                    itemMetadata.type, newName, newName);

                // Send the request and return the promise.
                // This call does not return response content from the server.
                return jQuery.ajax({
                    url: itemMetadata.uri,
                    type: "POST",
                    data: body,
                    headers: {
                        "X-RequestDigest": jQuery("#__REQUESTDIGEST").val(),
                        "content-type": "application/json;odata=verbose",
                        "content-length": body.length,
                        "IF-MATCH": itemMetadata.etag,
                        "X-HTTP-Method": "MERGE"
                    }
                });
            }
        }

        // Display error messages. 
        function onError(error) {
            alert(error.responseText);
        }
    },
    ProcessUploading: function (newfileName) {
        if (!newfileName)
            return;
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ImportFile, location.host, that.Settings.CurrentMonth, that.Settings.CurrentYear, that.Settings.DepartmentId, that.Settings.LocationId, newfileName);
        $.ajax({
            type: "GET",
            url: url,
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            $(".se-pre-con").fadeOut(0);
            if (response && response.ObjectId > 0) {
                // Rebind Url:
                var sourceURL = RBVH.Stada.WebPages.Utilities.GetValueByParam('Source');
                var listMode = RBVH.Stada.WebPages.Utilities.GetValueByParam('Mode');
                var lang = RBVH.Stada.WebPages.Utilities.GetValueByParam('lang');
                var newURL = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ShiftImportReturnUrl, location.host, response.ObjectId, sourceURL);
                if (listMode)
                    newURL = newURL + '&mode=' + listMode;
                if (lang)
                    newURL = newURL + '&lang=' + lang;

                window.location = newURL;
            }
        });
    },
    OnBeginSaveData: function () {
        var that = this;
        $(that.Settings.ShaderCssClass).show();
        $(that.Settings.LoadPanelCssClass).show();
        that.ToggleSaveButton(false);
    },

    OnEndSaveData: function () {
        var that = this;
        $(that.Settings.ShaderCssClass).hide();
        $(that.Settings.LoadPanelCssClass).hide();
        that.ToggleSaveButton(true);
        $.toaster(that.Settings.SaveDataOK);

        that.Settings.EmployeeJsonArray = [];

        window.location = that.Settings.PrevURL;
    },

    OnSaveData: function () {
        var that = this;

        if (that.Settings.View == "Approval") {
            that.OnBeginSaveData();
            var postData = that.SetMasterObject();
            that.SaveShiftApproval(postData);
        }
        else {
            var saveDetails = that.Settings.EmployeeJsonArray.length > 0;
            if (!that.CheckIsDirty()) {
                that.OnBeginSaveData();
                var postData = that.SetMasterObject();
                that.SaveShiftRequest(postData, saveDetails);
            }
        }
    },

    //OnRefreshData: function () {
    //    var that = this;
    //    var postData = that.SetMasterObject();
    //    that.RefreshShiftApproval(postData);
    //},

    DisableControls: function (disableAll) {
        var that = this;
        $(that.Settings.MonthControlSelector).prop('disabled', true);
        $(that.Settings.DepartmentControlSelector).prop('disabled', true);

        $(that.Settings.ImportControlSelector).hide();
        $(that.Settings.FileSelector).prop('disabled', true);

        if (disableAll) {
            that.Settings.ReadOnlyGrid = true;
            that.ToggleSaveButton(false);
        }
    },

    TryLoadInformation: function () {
        var that = this;

        // Load view MODE
        if (that.Settings.View == 'Edit') {
            that.DisableControls(false);
        }
        else if (that.Settings.View == 'View') {
            that.DisableControls(true);
        }
        // Load ID

        if (that.Settings.Id) {

            var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftRequestGet, location.host, that.Settings.Id);
            $.ajax({
                type: "GET",
                url: url,
                cache: false,
                async: false,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
            }).done(function (response) {
                if (that.Settings.View == "Approval") {
                    // Check current department + location -> Prevent 
                    if (_rbvhContext && _rbvhContext.EmployeeInfo && (_rbvhContext.EmployeeInfo.ADAccount.ID != response.ApprovedBy.ID)) {
                        var delegateUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.IsDelegated, location.host, response.ApprovedBy.ID, that.Settings.Id);
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
                        //window.location = window.location.protocol + '//' + location.host + '/_layouts/15/AccessDenied.aspx';
                    }
                }

                that.Settings.RequestDueDate = response.RequestDueDate;
                that.Settings.RequestExpired = response.RequestExpired;

                $(that.Settings.DepartmentControlSelector).prop('disabled', true);
                $(that.Settings.DepartmentControlSelector).val(response.Department.Id);

                $(that.Settings.MonthControlSelector).val(response.Month + '/' + response.Year);
                that.Settings.CurrentMonth = response.Month;
                that.Settings.CurrentYear = response.Year;

                var diffDays = 1;
                try {
                    var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidInputDate);
                    if (configVal) {
                        diffDays = parseInt(configVal);
                    }
                }
                catch (err) { diffDays = 1; }

                var today = new Date();
                var day = today.getDate() + diffDays; // 0 -> 11
                var month = today.getMonth() + 1; // 0 -> 11
                var year = today.getYear() + 1900;

                that.Settings.ReadOnlyGrid = false;
                if (that.Settings.View == 'View') {
                    that.Settings.ReadOnlyGrid = true;
                }
                else {
                    if (response.Year < year)
                        that.Settings.ReadOnlyGrid = true;
                    else if (response.Year == year) {
                        if (response.Month < month)
                            that.Settings.ReadOnlyGrid = true;
                        else if (response.Month == month) {
                            if (day > 20)
                                that.Settings.ReadOnlyGrid = true;
                        }
                    }
                }

                that.Settings.DaysInMonth = new Date(response.Year, response.Month - 1, 0).getDate();
                that.Settings.ShiftManagementId = response.Id;
                that.Settings.ShiftManagementDataSource = response.ShiftManagementDetailModelList;
                that.Settings.Requester.Id = response.Requester.LookupId;
                that.Settings.Requester.Name = response.Requester.LookupValue;
                that.Settings.LocationId = response.Location.LookupId;
                that.Settings.LocationName = response.Location.LookupValue;
                $(that.Settings.FactoryLocationControlSelector).html(response.Location.LookupValue);
                that.Settings.Requester.DepartmentId = response.Department.LookupId;
                that.Settings.DepartmentId = response.Department.Id;
                that.Settings.ApprovedBy = response.ApprovedBy;
                that.Settings.ModifiedBy = response.ModifiedBy;
                $(that.Settings.ApprovedByControlSelector).html(response.ApprovedBy.FullName);
                var additionalApprovers = response.AdditionalUser;
                if (additionalApprovers != null) {
                    $(that.Settings.LatestApprovedByControlSelector).html(additionalApprovers[0].FullName);
                }
                $(that.Settings.MonthControlSelector).prop('disabled', true);
            });
        }
    },

    PopulateData: function () {
        this.PopulateDepartment();
        this.TryLoadInformation();

        this.PopulatePersonalInformation();
        this.CalculateDateRange();
        this.GetNonWorkingDays();
        this.PopulateLeavesByDepartment();
        this.PopulateShiftTime();

        this.PopulateGrid();
    },

    PopulateDepartment: function () {
        var that = this;
        var lcid = SP.Res.lcid;
        var locationId = _rbvhContext.EmployeeInfo != null ? _rbvhContext.EmployeeInfo.FactoryLocation.LookupId : 2;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DepartmentList, location.host, lcid, locationId);
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
                that.Settings.DepartmentId = $(that.Settings.DepartmentControlSelector).val();
            }
        });
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
            success: function (result) {
                if (result) {
                    if (result.IsSystemAdmin == true) {
                        that.Settings.IsSystemAdmin = true;
                        that.PopulateDepartment();
                        that.PopulateAdminRequester();
                    }
                    else {
                        if (that.Settings.View != "Approval" && !!that.Settings.Id == false) {
                            that.Settings.Requester = {
                                Id: result.ID,
                                Name: result.FullName,
                                DepartmentId: result.Department.LookupId,
                                LocationId: result.Location.LookupId
                            };
                            that.Settings.DepartmentId = result.Department.LookupId;
                            that.Settings.LocationId = result.Location.LookupId;
                            that.Settings.LocationName = result.Location.LookupValue;
                            $(that.Settings.FactoryLocationControlSelector).html(result.Location.LookupValue);
                            // $(that.Settings.DepartmentControlSelector).empty();
                            // $(that.Settings.DepartmentControlSelector).append($("<option>").attr('value', result.Department.LookupId).text(result.Department.LookupValue));
                            $(that.Settings.DepartmentControlSelector).val(result.Department.LookupId).change();
                            $(that.Settings.DepartmentControlSelector).prop('disabled', true);
                        }
                        if (!!that.Settings.ApprovedBy.UserName == false) {
                            // Load Approved by:
                            that.PopulateApprovedBy(result.ID);
                        }
                    }
                }
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
                if (result && result.Approver2) {
                    $(that.Settings.ApprovedByControlSelector).html(result.Approver2.FullLoginName);
                    that.Settings.ApprovedBy.UserName = result.Approver2.LoginName;
                    that.Settings.ApprovedBy.FullName = result.Approver2.FullLoginName;
                }
            }
        });
    },

    PopulateLeavesByDepartment: function () {
        var that = this;

        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.LeavesByDepartment, location.host, that.Settings.LocationId, that.Settings.DepartmentId, that.Settings.StartDateRange, that.Settings.EndDateRange);

        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                if (result) {
                    that.Settings.LeavesByDepartmentArray = result;
                }
            }
        });
    },

    //PopulateLeavesByEmployee: function (employeeId, processLeaveFunc) {
    //    var that = this;

    //    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.LeavesByEmployee, location.host, employeeId, that.Settings.DepartmentId, that.Settings.LocationId, that.Settings.StartDateRange, that.Settings.EndDateRange);

    //    $.ajax({
    //        type: "GET",
    //        url: url,
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //        async: false,
    //        success: function (result) {
    //            processLeaveFunc(result);
    //        }
    //    });
    //},

    PopulateShiftTime: function () {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftTimeList, location.host);
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                //that.Settings.ShiftTimes = result;
                that.Settings.ShiftTimes = result.filter(function (item) {
                    return item.ShiftRequired == true;
                });
                that.Settings.ShiftTimeCodes = $.map(that.Settings.ShiftTimes, function (val, i) {
                    return val.Code;
                });
            }
        });

    },

    PopulateGrid: function () {
        var that = this;
        // Disable button:
        that.ToggleSaveButton(false);
        if (that.Settings.ShiftManagementId > 0 && that.Settings.ShiftManagementDataSource.length > 0) {
            if (_rbvhContext && _rbvhContext.EmployeeInfo && _rbvhContext.EmployeeInfo.DepartmentPermission === "Administrators"
                && _rbvhContext.EmployeeInfo.EmployeePosition.LookupValue === "Admin Dept") {
                that.ToggleExportButton(true);
            }
            else {
                that.ToggleExportButton(false);
            }
        }
        else {
            that.ToggleExportButton(false);
        }

        // Bind GID fields
        that.BindShiftTimeField();
        // Bind GRID columns
        that.BindGridColumns();
        if (that.Settings.View == "Approval") {
            //that.Settings.ReadOnlyGrid = false;
            //that.ToggleSaveButton(true);
            //that.ToggleApprovalButton();
        }
        $(that.Settings.GridEmployeeControlSelector).jsGrid({
            width: "100%",
            height: "400px",
            headercss: "jsGridEmployeeHeader",
            editing: !that.Settings.ReadOnlyGrid,
            autoload: true,
            noDataContent: '',

            //data: that.Settings.Data,
            onDataLoaded: function (args) {
                that.Settings.ShiftManagementDataSource = [];
                that.HighLightGrid();
                // Custom Grid event
                $(document).on('click', '.jsgrid-row , .jsgrid-alt-row', function () {
                    var currentRow = $(this);
                    var editedRow = $(this).prev();

                    var diffDays = 1;
                    try {
                        var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidInputDate);
                        if (configVal) {
                            diffDays = parseInt(configVal);
                        }
                    }
                    catch (err) { diffDays = 1; }

                    var today = new Date();
                    var day = today.getDate() + diffDays; // 0 -> 11
                    var month = today.getMonth() + 1; // 0 -> 11
                    var year = today.getYear() + 1900;

                    if (year == that.Settings.CurrentYear && month == that.Settings.CurrentMonth - 1 || (year == that.Settings.CurrentYear - 1 && month == 12 && that.Settings.CurrentMonth == 1)) {
                        // CURRENT: 25/6 
                        // Chon thang 7 ([21/6] -> [20/7]): Hide [21/6] -> [24/6]
                        if (day > 21) {
                            var left21 = 1 + (day - 21);
                            for (var i = 2; i <= left21; i++) {
                                editedRow.find("td:eq(" + i + ") input").attr('disabled', true);
                            }
                        }
                    }
                    else if (year == that.Settings.CurrentYear && month == that.Settings.CurrentMonth) {
                        // CURRENT: 1/6
                        // Chon thang 6 ([21/5] -> [20/6]): Hide [21/5] -> CURRENT
                        // [21/5]: Index = 2 _ CURRENT = DaysInMonth(5) - 21 + Index + day

                        var endLeft = that.Settings.DaysInMonth - 21 + 1 + day;
                        for (var j = 2; j <= endLeft; j++) {
                            editedRow.find("td:eq(" + j + ") input").attr('disabled', true);
                        }
                    }

                    // Feedback: Disable cells have leave info
                    currentRow.find('td.shift-time-valid, td.jsgrid-cell-half-day, td.jsgrid-cell-all-day').each(function () {
                        var index = $(this).index();
                        editedRow.find("td:eq(" + index + ") input").attr('disabled', true);

                    });
                    //if (that.Settings.View == "Approval") {
                    //    currentRow.find('td.shift-time-invalid').each(function () {
                    //        if ($(this).html() == '') {
                    //            var index = $(this).index();
                    //            editedRow.find("td:eq(" + index + ") input").attr('disabled', true);
                    //        }
                    //    });
                    //}

                    // Add class for non-working days EDIT
                    currentRow.find('td.highlight').each(function () {
                        var index = $(this).index();
                        editedRow.find("td:eq(" + index + ")").addClass('highlight-edit');
                    });

                    editedRow.find("td input:enabled:not([readonly]):first").focus();
                });

                $(that.Settings.GridEmployeeControlSelector).on("keyup", ".jsgrid-edit-row input", function (e) {
                    if (e.which == 13 || e.which == 40 || e.which == 38) { // Enter - Move down - Move up
                        $('.jsgrid-update-button').click();
                    }
                    else if (e.which == 27) {
                        $('.jsgrid-cancel-edit-button').click();
                    }
                });

                $(that.Settings.GridEmployeeControlSelector).on("paste", ".jsgrid-edit-row input", function (e) {
                    var hotKeyType = $("input:radio[name='grHotKey']:checked").val();
                    if (typeof hotKeyType != 'undefined') {
                        var inputInstance = this;
                        if ($(inputInstance).parent().hasClass("highlight-edit"))
                            return;
                        if (hotKeyType == 'Month') {
                            setTimeout(function () {
                                var value = $(inputInstance).val();
                                var $item = $(inputInstance).parent().next('td').not(".highlight-edit").find('input[type!=button]:enabled:not([readonly]):first');
                                var $nextItem = $(inputInstance).parent().next('td').find('input[type!=button]:enabled:not([readonly]):first');
                                var counter = 0;
                                while ($nextItem.length > 0) {

                                    if ($item.length == 0)
                                        $item = $nextItem.parent().next('td').not(".highlight-edit").find('input[type!=button]:enabled:not([readonly]):first');
                                    else {
                                        $item.val(value);
                                        $item = $item.parent().next('td').not(".highlight-edit").find('input[type!=button]:enabled:not([readonly]):first');
                                    }

                                    $nextItem = $nextItem.parent().next('td').find('input[type!=button]:enabled:not([readonly]):first');
                                    counter++;
                                }
                            }, 100);
                        }
                        if (hotKeyType == 'Week') {
                            setTimeout(function () {
                                var value = $(inputInstance).val();
                                var $item = $(inputInstance).parent().next('td').not(".highlight-edit").find('input[type!=button]:enabled:not([readonly]):first');
                                while ($item.length > 0) {
                                    $item.val(value);
                                    $item = $item.parent().next('td').not(".highlight-edit").find('input[type!=button]:enabled:not([readonly]):first');
                                }
                            }, 100);
                        }
                    }
                });

                $(window).keydown(function (event) {
                    if (event.which == 13) {
                        event.preventDefault();
                        return false;
                    }
                });

                $('.jsgrid-grid-body').scroll(function () {
                    that.UpdateColPos(2);
                });

                // LEAVE popup
                var delay = 1000, setTimeoutConst;
                $(document)
                    .on('mouseover', 'td.jsgrid-cell-all-day, td.jsgrid-cell-half-day', function (e) {
                        var currentRow = $(this);
                        setTimeoutConst = setTimeout(function () {
                            var leaveUrl = currentRow.attr("data-leave-url");
                            $("#leave-link").attr("href", leaveUrl);

                            $("#leave-dialog").dialog();

                            $('#leave-link').off('click').on('click', function () {
                                var itemURL = $(this).attr('href');
                                window.open(itemURL, "_blank");
                                setTimeout(function () {
                                    $("#leave-dialog").dialog('close');
                                }, 1000);

                                return false;
                            });

                            return false;
                        }, delay);
                    })
                    .on('mouseout', 'td.jsgrid-cell-all-day, td.jsgrid-cell-half-day', function (e) {
                        clearTimeout(setTimeoutConst);
                    });
            },

            rowClick: function (args) {
                if (this._editingRow) {
                    this.updateItem();
                }
                var $target = $(args.event.target);
                if (that.Settings.View == "Approval") {
                    if (args.item.IsExisted) {
                        this.editItem($target.closest("tr"));
                    }
                }
                else {
                    this.editItem($target.closest("tr"));
                }
            },

            rowRenderer: function (item) {
                var empLeaves = that.Settings.LeavesByDepartmentArray.filter(function (index) { return index.EmployeeId == item.Employee.Id; });
                var hasLeave = empLeaves.length > 0 && empLeaves[0].Leaves.length > 0;
                var $item;
                var $td1 = '<td class="jsgrid-cell jsgrid-align-center" style="width: 100px;">' + item.Employee.EmployeeId + '</td>';
                var $td2 = '<td class="jsgrid-cell" style="width: 150px;font-weight: bold;">' + item.Employee.FullName + '</td>';
                $item = $("<tr>").append($td1).append($td2);
                var $td;
                var shiftCellNotApprovedCSS = 'shift-time-invalid';
                var shiftCellApprovedCSS = 'shift-time-valid';

                var shiftCellCSS = shiftCellNotApprovedCSS;
                //var shiftCellHighlightCss = '';
                for (var d = that.Settings.BeginDate; d <= that.Settings.DaysInMonth; d++) {
                    shiftCellCSS = shiftCellNotApprovedCSS;

                    var dbShiftCell = item["ShiftTime" + d];
                    dbShiftCell = dbShiftCell || {};
                    var cellValue = dbShiftCell.Code == 'P' ? 'P' : that.GetShiftTimeCodeById(dbShiftCell.Value);

                    // LEAVE
                    var leaveInfo = null;
                    var leaveCSS = '';
                    var leaveUrl = '';
                    if (hasLeave) {
                        leaveInfo = $.grep(empLeaves[0].Leaves, function (e) { return (e.Day == d && $.inArray(d, that.Settings.NonWorkingDays) < 0); });
                        if (leaveInfo.length == 1) {
                            leaveCSS = ' jsgrid-cell-half-day ';
                            if (leaveInfo[0].AllDay == true) {
                                leaveCSS = ' jsgrid-cell-all-day ';
                                shiftCellCSS = shiftCellApprovedCSS;
                                cellValue = 'P'; // TODO: MUST load from SHIFT
                                //if (cellValue === 'P') {
                                //    leaveCSS = ' jsgrid-cell-all-day ';
                                //    shiftCellCSS = shiftCellApprovedCSS;
                                //}
                                //else {
                                //    leaveCSS = '';
                                //    shiftCellCSS = shiftCellNotApprovedCSS;
                                //}
                            }
                            else {
                                if (!cellValue)
                                    cellValue = 'HC';
                            }
                            leaveUrl = leaveInfo[0].ItemUrl;
                        }
                    }
                    shiftCellCSS = !!dbShiftCell.Approved ? shiftCellApprovedCSS : shiftCellCSS;
                    $td = '<td class="jsgrid-cell jsgrid-align-center ' + leaveCSS + shiftCellCSS + '" data-leave-url="' + leaveUrl + '">' + cellValue + '</td>';

                    $item.append($td);
                }
                for (var index = 1; index <= that.Settings.EndDate; index++) {
                    shiftCellCSS = shiftCellNotApprovedCSS;

                    var endDateShiftCell = item["ShiftTime" + index];
                    endDateShiftCell = endDateShiftCell || {};
                    var cellValue = endDateShiftCell.Code == 'P' ? 'P' : that.GetShiftTimeCodeById(endDateShiftCell.Value);

                    // LEAVE
                    var leaveInfo = null;
                    var leaveCSS = '';
                    var leaveUrl = '';
                    if (hasLeave) {
                        leaveInfo = $.grep(empLeaves[0].Leaves, function (e) { return (e.Day == index && $.inArray(index, that.Settings.NonWorkingDays) < 0); });
                        if (leaveInfo.length == 1) {
                            leaveCSS = ' jsgrid-cell-half-day ';
                            if (leaveInfo[0].AllDay == true) {
                                leaveCSS = ' jsgrid-cell-all-day ';
                                shiftCellCSS = shiftCellApprovedCSS;
                                cellValue = 'P'; // TODO: MUST load from SHIFT
                                //if (cellValue === 'P') {
                                //    leaveCSS = ' jsgrid-cell-all-day ';
                                //    shiftCellCSS = shiftCellApprovedCSS;
                                //}
                                //else {
                                //    leaveCSS = '';
                                //    shiftCellCSS = shiftCellNotApprovedCSS;
                                //}
                            }
                            else {
                                if (!cellValue)
                                    cellValue = 'HC';
                            }

                            leaveUrl = leaveInfo[0].ItemUrl;
                        }
                    }

                    shiftCellCSS = !!endDateShiftCell.Approved ? shiftCellApprovedCSS : shiftCellCSS;
                    $td = '<td class="jsgrid-cell jsgrid-align-center ' + leaveCSS + shiftCellCSS + '" data-leave-url="' + leaveUrl + '">' + cellValue + '</td>';

                    $item.append($td);
                }
                var $tdRight = '<td class="jsgrid-cell" style="width: 60px;"></td>';
                $item.append($tdRight);
                return $item;
            },
            controller: {
                loadData: function (filter) {

                    var postData = {};
                    var year;
                    var month;
                    var url;
                    var departmentId;
                    var d;
                    if (that.Settings.View == "Approval") {
                        if (that.Settings.Id > 0) {
                            if (that.Settings.ShiftManagementDataSource.length > 0) {
                                that.Settings.GridBeforeEdit = JSON.stringify(that.Settings.ShiftManagementDataSource);
                                that.Settings.EmployeeJsonArray = that.Settings.ShiftManagementDataSource;

                                that.ToggleApprovalButton();
                                return that.Settings.ShiftManagementDataSource;
                            }
                        }
                    }
                    else {
                        // Disable for first load
                        if (that.Settings.Id > 0) {
                            if (that.Settings.ShiftManagementDataSource.length > 0) {
                                that.Settings.GridBeforeEdit = JSON.stringify(that.Settings.ShiftManagementDataSource);
                                return that.Settings.ShiftManagementDataSource;
                            }
                        }
                        else {
                            d = $.Deferred();

                            url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftRequestList, location.host);
                            month = that.Settings.CurrentMonth;
                            year = that.Settings.CurrentYear;
                            departmentId = that.Settings.DepartmentId;//$(that.Settings.DepartmentControlSelector).val();
                            locationId = that.Settings.LocationId;//$(that.Settings.DepartmentControlSelector).val();
                            postData = {};
                            postData.Month = month.toString();
                            postData.Year = year.toString();
                            postData.DepartmentId = departmentId.toString();
                            postData.LocationId = locationId.toString();
                            $.ajax({
                                type: "POST",
                                url: url,
                                cache: false,
                                data: JSON.stringify(postData),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                            }).done(function (response) {
                                // Enable button
                                that.Settings.GridBeforeEdit = JSON.stringify(response.ShiftManagementDetailModelList);
                                that.Settings.ShiftManagementId = response.Id;
                                d.resolve(response.ShiftManagementDetailModelList);
                            });

                            return d.promise();
                        }
                    }
                    //return that.Settings.Data;
                },
                updateItem: function (item) {
                    // Check dirty:
                    if (that.Settings.ItemBeforeEdit != null) {
                        if (JSON.stringify(that.Settings.ItemBeforeEdit) != JSON.stringify(item)) {
                            // Remove existing item in JSON ARRAY
                            that.FilterEmployeeList(item.Employee.Id);
                            that.Settings.EmployeeJsonArray.push(item);
                        }
                    }
                },

                deleteItem: function (item) {
                }
            },
            onItemEditing: function (args) {
                that.ToggleSaveButton(false);
                that.Settings.ItemBeforeEdit = args.item;
                setTimeout(function () { that.UpdateColPos(2); }, 1);
            },
            onItemUpdated: function (args) {
                that.ToggleSaveButton();
                that.HighLightGrid();
                that.UpdateColPos(2);
            },
            onItemEditCancelling: function (args) {
                that.ToggleSaveButton();
            },
            onRefreshed: function (args) {
                that.UpdateColPos(2);
            },
            fields:
            that.Settings.Fields
        });
    },

    ToggleSaveButton: function (enable) {
        if (typeof enable != 'undefined') {
            if (enable) {
                $(this.Settings.SaveControlSelector).prop('disabled', false);
                $(this.Settings.SaveControlSelector).removeClass('disable');
            }
            else {
                $(this.Settings.SaveControlSelector).prop('disabled', true);
                $(this.Settings.SaveControlSelector).addClass('disable');
            }
        }
        else {
            if (this.Settings.View == "Approval") {
                this.ToggleApprovalButton();
            }
            else {
                if (this.CheckIsDirty() == true) {
                    $(this.Settings.SaveControlSelector).prop('disabled', true);
                    $(this.Settings.SaveControlSelector).addClass('disable');
                }
                else {
                    $(this.Settings.SaveControlSelector).prop('disabled', false);
                    $(this.Settings.SaveControlSelector).removeClass('disable');
                }
            }
        }
    },
    ToggleExportButton: function (enable) {
        if (typeof enable != 'undefined') {
            if (enable) {
                $(this.Settings.ExportControlSelector).prop('disabled', false);
                $(this.Settings.ExportControlSelector).removeClass('disable');
            }
            else {
                $(this.Settings.ExportControlSelector).prop('disabled', true);
                $(this.Settings.ExportControlSelector).addClass('disable');
            }
        }
        else {
            if (this.CheckIsDirty() == true) {
                $(this.Settings.ExportControlSelector).prop('disabled', true);
                $(this.Settings.ExportControlSelector).addClass('disable');
            }
            else {
                $(this.Settings.ExportControlSelector).prop('disabled', false);
                $(this.Settings.ExportControlSelector).removeClass('disable');
            }
        }
    },

    CheckIsDirty: function () {
        if (this.Settings.GridBeforeEdit != null) {
            var gridAfterEdit = JSON.stringify($(this.Settings.GridEmployeeControlSelector).jsGrid("option", "data"));
            if (this.Settings.GridBeforeEdit != gridAfterEdit)
                return false;
        }

        return true;
    },

    InitControls: function () {
        var that = this;
        $(that.Settings.MonthControlSelector).datepicker({
            viewMode: "months",
            minViewMode: "months",
            format: "mm/yyyy",
            autoclose: true
        });
        $(that.Settings.ImportControlSelector).prop('disabled', true);
        $(that.Settings.ImportControlSelector).addClass('disable');

        $("#ShiftFileUpload").val("");
    },

    SetDefaultValue: function () {
        var that = this;

        var diffDays = 1;
        try {
            var configVal = Functions.getConfigValue(that.Settings.Configurations, that.Settings.ConfigKey_ValidInputDate);
            if (configVal) {
                diffDays = parseInt(configVal);
            }
        }
        catch (err) { diffDays = 1; }

        var today = new Date();
        var day = today.getDate() + diffDays;
        var month = today.getMonth();
        var year = today.getYear() + 1900;
        //year = month == 12 ? year - 1 : year;
        that.Settings.DaysInMonth = new Date(year, month, 0).getDate();
        that.Settings.CurrentMonth = month + 1; // month: 0 -> 11
        that.Settings.CurrentYear = year;
        $(that.Settings.MonthControlSelector).val(month + 1 + '/' + year);

        if (day > 20)
            that.Settings.ReadOnlyGrid = true;
    },

    BindGridColumns: function () {
        var that = this;

        jsGrid.fields.custShiftTimeField = that.Settings.Grid.CustomFields.ShiftTimeField;

        that.Settings.Fields = [
            { name: "Employee.EmployeeId", title: that.Settings.Grid.Titles.GridColumn_EmployeeId, width: 100, validate: "required", readOnly: true, align: "center" },
            { name: "Employee.FullName", title: that.Settings.Grid.Titles.GridColumn_EmployeeName, width: 150, validate: "required" },
        ];

        for (var d = that.Settings.BeginDate; d <= that.Settings.DaysInMonth; d++) {
            var fieldName = "ShiftTime" + d + ".Value";
            that.Settings.Fields.push({ name: fieldName, title: d, type: "custShiftTimeField", align: "center", width: 50 })
        }
        for (var d = 1; d <= that.Settings.EndDate; d++) {
            var fieldName = "ShiftTime" + d + ".Value";
            that.Settings.Fields.push({ name: fieldName, title: d, type: "custShiftTimeField", align: "center", width: 50 })
        }
        that.Settings.Fields.push({ type: "control" });
    },

    BindShiftTimeField: function () {
        var that = this;
        that.Settings.Grid.CustomFields.ShiftTimeField = function (config) {
            jsGrid.Field.call(this, config);
        };

        that.Settings.Grid.CustomFields.ShiftTimeField.prototype = new jsGrid.Field({
            itemTemplate: function (value) {
                return value;
            },

            insertTemplate: function (value) {
                return this._insertPicker = $("<input>").autocomplete({
                    source: that.Settings.ShiftTimeCodes
                });
            },

            editTemplate: function (value) {
                return this._editPicker = $("<input>").autocomplete({
                    source: that.Settings.ShiftTimeCodes
                }).val(that.GetShiftTimeCodeById(value));
            },

            insertValue: function () {
                return this._insertPicker.val();
            },

            editValue: function () {
                return that.GetShiftTimeIdByCode(this._editPicker.val());
            }
        });
    },

    HighLightGrid: function () {
        var that = this;
        // Get days in month need to be highlighted:
        var exceptDays = that.Settings.NonWorkingDays;
        var index;
        for (var i = 0; i < exceptDays.length; i++) {
            if (exceptDays[i] >= that.Settings.BeginDate && exceptDays[i] <= that.Settings.MaxDate) {
                index = (parseInt(exceptDays[i]) - that.Settings.BeginDate) + that.Settings.StartIndex;

                $('.jsgrid-alt-row').find("td:eq(" + index + ")").addClass('highlight');

                $('.jsgrid-row').find("td:eq(" + index + ")").addClass('highlight');
            }
            else if (exceptDays[i] >= 1 && exceptDays[i] <= that.Settings.EndDate) {
                // 1 - 13
                index = parseInt(exceptDays[i]) + (that.Settings.Start1StDateIndex - (that.Settings.MaxDate - that.Settings.DaysInMonth)) - 1;
                $('.jsgrid-alt-row').find("td:eq(" + index + ")").addClass('highlight');

                $('.jsgrid-row').find("td:eq(" + index + ")").addClass('highlight');
            }
        }
    },

    GetHighlightCSSClass: function (empLocation, day) {
        if (empLocation) {
            if (empLocation == 'NM1') {
                if (this.Settings.NonWorkingDaysNM1.indexOf(day) >= 0) {
                    return 'highlight-location1';
                }
            }
            else if (empLocation == 'NM2') {
                if (this.Settings.NonWorkingDaysNM2.indexOf(day) >= 0) {
                    return 'highlight-location2';
                }
            }
        }
        return '';
    },

    /* Business */
    GetShiftTimeIdByCode: function (code) {
        if (code != '') {
            var id = '';
            $.each(this.Settings.ShiftTimes, function (i, v) {
                if (v.Code == code) {
                    id = v.Id;
                    return;
                }
            });
            return id + "";
        }

        return "";
    },

    GetShiftTimeCodeById: function (id) {
        if (id != '') {
            var code = '';
            $.each(this.Settings.ShiftTimes, function (i, v) {
                if (v.Id == id) {
                    code = v.Code;
                    return;
                }
            });
            return code;
        }

        return "";
    },

    FilterEmployeeList: function (employeeId) {
        var that = this;
        that.Settings.EmployeeJsonArray = that.Settings.EmployeeJsonArray.filter(function (item) {
            return item.Employee.Id != employeeId;
        });
    },

    GetNonWorkingDays: function () {
        var that = this;

        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.CalendarList, location.host, that.Settings.StartDateRange, that.Settings.EndDateRange);
        if (that.Settings.IsSystemAdmin) {
            url = url + '/""';
        }
        else {
            url = url + '/' + that.Settings.LocationId;
        }
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (result) {
                that.Settings.NonWorkingDays = [];

                that.Settings.NonWorkingDays = result.filter(function (item) {
                    return item.Location == that.Settings.LocationName;
                }).map(function (obj) { return obj.Day; });
            }
        });
    },

    // Load test:
    SetMasterObject: function () {
        var that = this;
        var postData = {};
        postData.Id = that.Settings.ShiftManagementId;
        postData.Month = that.Settings.CurrentMonth.toString();
        postData.Year = that.Settings.CurrentYear.toString();
        postData.Department = {};
        postData.Department.Id = that.Settings.DepartmentId.toString();
        postData.Location = {};
        postData.Location.LookupId = that.Settings.LocationId.toString();
        postData.ShiftManagementDetailModelList = [];
        postData.Requester = {};
        postData.Requester.LookupId = that.Settings.Requester.Id;
        postData.Requester.LookupValue = that.Settings.Requester.Name;
        postData.ApprovedBy = that.Settings.ApprovedBy || {};
        postData.ShiftManagementDetailModelList = [];//that.Settings.EmployeeJsonArray;
        postData.ApproverFullName = that.Settings.ApproverFullName != '' ? that.Settings.ApproverFullName : postData.ApprovedBy.FullName;

        return postData;
    },

    SaveShiftRequest: function (postData, saveDetails) {
        var that = this;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementMasterDetailInsert, location.host);
        postData.ShiftManagementDetailModelList = that.Settings.EmployeeJsonArray;
        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response && response.Code > 0) {
                console.log(response.Message);
            }

            that.OnEndSaveData();
            window.location = that.Settings.PrevURL;
        });
    },

    //SaveShiftRequest: function (postData, saveDetails) {
    //    var that = this;

    //    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementMasterInsert, location.host);
    //    $.ajax({
    //        type: "POST",
    //        url: url,
    //        data: JSON.stringify(postData),
    //        contentType: "application/json; charset=utf-8",
    //        dataType: "json",
    //    }).done(function (response) {
    //        var validResponse = false;
    //        if (!!response.ObjectId) {
    //            that.Settings.ShiftManagementId = response.ObjectId;
    //            if (!!saveDetails)
    //                that.SaveShiftDetail(response.ObjectId);
    //            else {
    //                that.OnEndSaveData();
    //                window.location = that.Settings.PrevURL;
    //            }
    //        }
    //        else {
    //            that.OnEndSaveData();
    //            window.location = that.Settings.PrevURL;
    //        }
    //    });
    //},

    //SaveShiftDetail: function (shiftManagementId) {
    //    var that = this;
    //    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementDetailInsert, location.host);
    //    var i = 0;
    //    //var employeeList = that.CreateEmployeeObject();
    //    var length = that.Settings.EmployeeJsonArray.length; //employeeList.length; //
    //    var counter = 0;
    //    var startTime = performance.now();
    //    for (i = 0; i < length; i++) {
    //        var shiftManagementDetail = that.Settings.EmployeeJsonArray[i]; //employeeList[i]; //
    //        shiftManagementDetail.ShiftManagementID = shiftManagementId;
    //        $.ajax({
    //            type: "POST",
    //            url: url,
    //            data: JSON.stringify(shiftManagementDetail),
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //        }).done(function (response) {
    //            if (!!response.ObjectId)
    //                counter++;

    //            if (counter == length) {
    //                var endTime = performance.now();
    //                //alert("Save " + length + " employees, takes " + (endTime - startTime) + " milliseconds.");
    //                that.OnEndSaveData();
    //                //window.location = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ShiftManagementListUrl, location.host);
    //                window.location = that.Settings.PrevURL;
    //            }
    //        });
    //    }
    //},

    SaveShiftApproval: function (postData) {
        var that = this;
        var approvalList = [];
        var adminApproval = {};
        adminApproval.Year = postData.Year;
        adminApproval.Month = postData.Month;
        adminApproval.ShiftManagementId = postData.Id;
        adminApproval.DepartmentId = postData.Department.Id;
        adminApproval.LocationId = postData.Location.LookupId;
        adminApproval.EmployeeNameList = [];
        adminApproval.ApproverFullName = postData.ApproverFullName;
        var modifiedBy = "";
        var modifiedByUser = that.Settings.ModifiedBy;
        if (!!modifiedByUser) {
            modifiedBy = modifiedByUser.UserName;
        }

        if (_rbvhContext && _rbvhContext.EmployeeInfo && _rbvhContext.EmployeeInfo.ADAccount && _rbvhContext.EmployeeInfo.ADAccount.UserName) {
            postData.AdditionalUser = [];
            var currentApprover = {
                UserName: _rbvhContext.EmployeeInfo.ADAccount.UserName,
                ID: _rbvhContext.EmployeeInfo.ADAccount.ID
            }
            postData.AdditionalUser.push(currentApprover);
        }

        adminApproval.ModifiedBy = modifiedBy;
        postData.ModifiedByString = modifiedBy;

        var startTime = performance.now();
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementDetailApprove, location.host);

        //var approveAll = true;
        for (var i = 0; i < that.Settings.EmployeeJsonArray.length; i++) {
            var model = that.Settings.EmployeeJsonArray[i];
            var approvalDays = [];
            if (model.IsExisted == true) {
                var existToBeApprovedShift = false;
                var j = 0;
                for (j = 1; j <= 31; j++) {
                    if (model["ShiftTime" + j].Value > 0 && model["ShiftTime" + j].Approved == false) {
                        existToBeApprovedShift = true;
                        model.NewApproval = true;
                        model["ShiftTime" + j].Approved = true;
                        if (that.Settings.NonWorkingDays.indexOf(j) >= 0) {
                            var dayInfo = {};
                            dayInfo.Day = j;
                            dayInfo.ShiftTimeId = model["ShiftTime" + j].Value;
                            approvalDays.push(dayInfo);
                            model["ShiftTime" + j].Day = j;
                        }
                    }
                    else if (model["ShiftTime" + j].Value <= 0 && model["ShiftTime" + j].Approved == false) {
                        // If exists at least one day not approved (working day): approveAll = false;
                    }
                }

                if (existToBeApprovedShift === true) {
                    adminApproval.EmployeeNameList.push(model.Employee.FullName);
                }
                model["ApprovalDays"] = approvalDays;
                approvalList.push(model);
            }
        }
        var endTime = performance.now();
        var length = approvalList.length;
        var counter = 0;
        startTime = performance.now();

        for (i = 0; i < length; i++) {
            postData.ShiftManagementDetailModelList.push(approvalList[i]);
        }

        $.ajax({
            type: "POST",
            url: url,
            data: JSON.stringify(postData),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (response) {
            if (response.Code == 0) {
                endTime = performance.now();
                startTime = performance.now();
                var adminEmailUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementSendAdminApprovalEmail, location.host);
                $.ajax({
                    type: "POST",
                    url: adminEmailUrl,
                    cache: false,
                    data: JSON.stringify(adminApproval),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                }).done(function (response) {
                    endTime = performance.now();
                    that.OnEndSaveData();
                });
            }
            else if (response && response.Code == 2) { //cannot approve
                alert(response.Message);
                window.location.reload();
            }
        });

        //for (i = 0; i < length; i++) {
        //    postData.ShiftManagementDetailModelList.push(approvalList[i]);
        //    $.ajax({
        //        type: "POST",
        //        url: url,
        //        data: JSON.stringify(postData),
        //        contentType: "application/json; charset=utf-8",
        //        dataType: "json",
        //    }).done(function (response) {
        //        if (response.Code == 0)
        //            counter++;

        //        if (counter == length) {
        //            endTime = performance.now();
        //            //console.log("2.End approve: " + (endTime - startTime) + " milliseconds.");
        //            startTime = performance.now();
        //            //console.log('3.Begin send email for Admin:');

        //            var adminEmailUrl = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementSendAdminApprovalEmail, location.host);

        //            $.ajax({
        //                type: "POST",
        //                url: adminEmailUrl,
        //                cache: false,
        //                data: JSON.stringify(adminApproval),
        //                contentType: "application/json; charset=utf-8",
        //                dataType: "json",
        //            }).done(function (response) {
        //                endTime = performance.now();
        //                //console.log("3.End send email for Admin: " + (endTime - startTime) + " milliseconds.");
        //                that.OnEndSaveData();
        //                //window.location = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ShiftManagementListUrl, location.host);
        //            });
        //        }
        //    });
        //    postData.ShiftManagementDetailModelList = [];
        //}
    },

    //RefreshShiftApproval: function (postData) {

    //    var that = this;
    //    var approvalList = [];
    //    // Admin
    //    var adminApproval = {};
    //    adminApproval.Year = postData.Year;
    //    adminApproval.Month = postData.Month;
    //    adminApproval.EmployeeShiftList = [];
    //    //console.log('1.Begin prepare data:');
    //    var startTime = performance.now();
    //    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ShiftManagementDetailRefresh, location.host);

    //    for (i = 0; i < that.Settings.EmployeeJsonArray.length; i++) {
    //        postData.ShiftManagementDetailModelList.push(that.Settings.EmployeeJsonArray[i]);
    //        $.ajax({
    //            type: "POST",
    //            url: url,
    //            data: JSON.stringify(postData),
    //            contentType: "application/json; charset=utf-8",
    //            dataType: "json",
    //        }).done(function (response) {
    //        });
    //        postData.ShiftManagementDetailModelList = [];
    //    }
    //},

    ToggleApprovalButton: function () {
        var that = this;
        var approvalList = that.Settings.EmployeeJsonArray.filter(function (item) {
            return item.IsExisted == true;
        });

        for (var i = 0; i < approvalList.length; i++) {
            var model = approvalList[i];
            var j = 0;
            for (j = 1; j <= 31; j++) {
                if (model["ShiftTime" + j].Value > 0 && model["ShiftTime" + j].Approved == false) {
                    if (that.Settings.RequestExpired === true) {
                        errMsg = decodeURI(that.Settings.RequestExpiredMsgFormat);
                        errMsg = RBVH.Stada.WebPages.Utilities.String.format(errMsg, that.Settings.RequestDueDate);
                        RBVH.Stada.WebPages.Utilities.GUI.showRequestExpired(that.Settings.ErrorMsgContainerSelector, that.Settings.ErrorMsgSelector, errMsg);
                        that.ToggleSaveButton(false);
                    }
                    else {
                        that.ToggleSaveButton(true);
                    }
                    return;
                }
            }
        }
    },

    GetApprovalDays: function (employeeDetail) {
        var result = [];
        for (j = 1; j <= 31; j++) {
            if (employeeDetail["ShiftTime" + j].Value > 0 && employeeDetail["ShiftTime" + j].Approved == false) {
                result.push(j);
            }
        }
    },

    CreateEmployeeObject: function () {
        var employeeList = [];
        for (var index = 0; index <= 5; index++) {
            var employeeObject = {};
            employeeObject.Employee = {};
            employeeObject.Employee.EmployeeId = "19024";
            employeeObject.Employee.FullName = "Chau Pham";
            employeeObject.Employee.Id = 157;
            for (var i = 1; i <= 31; i++) {
                employeeObject["ShiftTime" + i] = {};
                employeeObject["ShiftTime" + i].Value = "1";
                employeeObject["ShiftTime" + i].Approved = false;
                employeeObject["ShiftTime" + i].Day = 0;
            }
            employeeList.push(employeeObject);
        }

        return employeeList;
    },

    CalculateDateRange: function () {
        var that = this;
        var startMonth = RBVH.Stada.WebPages.Utilities.String.padDate(that.Settings.CurrentMonth - 1);

        var startYear = that.Settings.CurrentYear;
        if (that.Settings.CurrentMonth == 1) {
            startMonth = 12;
            startYear = that.Settings.CurrentYear - 1;
        }

        var startDate = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ISODateFormat, startMonth, '21', startYear);

        var nextYear = that.Settings.CurrentYear;
        var nextMonth = RBVH.Stada.WebPages.Utilities.String.padDate(that.Settings.CurrentMonth);// + 1;

        var endDate = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ISODateFormat, nextMonth, '20', nextYear);

        that.Settings.StartDateRange = startDate;
        that.Settings.EndDateRange = endDate;
    },

    // Freeze Grid columns
    UpdateColPos: function (cols) {
        var left = $('.jsgrid-grid-body').scrollLeft() < $('.jsgrid-grid-body .jsgrid-table').width() - $('.jsgrid-grid-body').width() + 16
            ? $('.jsgrid-grid-body').scrollLeft() : $('.jsgrid-grid-body .jsgrid-table').width() - $('.jsgrid-grid-body').width() + 16;
        $('.jsgrid-header-row th:nth-child(-n+' + cols + '), .jsgrid-filter-row td:nth-child(-n+' + cols + '), .jsgrid-insert-row td:nth-child(-n+' + cols + '), .jsgrid-grid-body tr td:nth-child(-n+' + cols + ')')
            .css({
                "position": "relative",
                "left": left
            });
    }
};
