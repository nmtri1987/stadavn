RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");

RBVH.Stada.WebPages.pages.Overview = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        ServiceUrls:
        {
            DepartmentList: this.Protocol + '//{0}/_vti_bin/Services/Department/DepartmentService.svc/GetDepartmentsByLcid/{1}/{2}',
            ModuleList: this.Protocol + '//{0}/_vti_bin/Services/Common/CommonService.svc/GetModules/{1}',
            TaskOverview: this.Protocol + '//{0}/_vti_bin/Services/Common/CommonService.svc/GetTaskOverview/{1}/{2}/{3}',
            TaskList: this.Protocol + '//{0}/_vti_bin/Services/Common/CommonService.svc/GetTaskByCondition/{1}/{2}/{3}/{4}',
        },

        ApprovalStatus: [
            { Name: "", Id: 0 },
            { Name: "In-Progress", Id: 1 },
            { Name: "Rejected", Id: 2 },
            { Name: "Approved", Id: 3 },
            { Name: "In-Process", Id: 4 },
            { Name: "Completed", Id: 5 },
        ],
        ApprovalStatusInVN: [
            { Name: "", Id: 0 },
            { Name: "Đang chờ duyệt", Id: 1 },
            { Name: "Từ chối", Id: 2 },
            { Name: "Đã duyệt", Id: 3 },
            { Name: "Đang thực hiện", Id: 4 },
            { Name: "Hoàn thành", Id: 5 },
        ],
        Condition:
        {
            WaitingApproval: "WaitingApproval",
            WaitingApprovalToday: "WaitingApprovalToday",
            InProcess: "InProcess",
            ApprovedToday: "ApprovedToday",
        },

        Data: {
            LCID: 0,
            ApprovalStatus: [],
            Departments: [],
            Modules: [],
            UserInfoId: 0,
            UserADId: 0,
            LocationId: 2,
            FullName: 'System Account',
            Position: '',
            IsBOD: false,
            Condition: '',

        },
        
        Grid:
        {
            DataSource: [],
            ShaderCssClass: '.jsgrid-load-shader',
            LoadPanelCssClass: '.jsgrid-load-panel',
            LoadedData: false,
        }
    },

    $.extend(true, this.Settings, settings);

    this.Initialize();
};

RBVH.Stada.WebPages.pages.Overview.prototype =
{
    Initialize: function () {
        var that = this;

        $(document).ready(function () {
            that.InitDefaultValue();
            that.RegisterEvents();

            that.GetOverviewData().then(function (taskOverviewModel) {
                that.PopulateChart(taskOverviewModel);
            });

            that.PopulateGridFilter(that.PopulateGrid);
        });
    },

    RegisterEvents: function () {
        // Show/hide
        $('h2.conner .show-hide').click(function (e) {
            e.stopPropagation();
            $(this).parent().parent().find('.grid-content').toggle();
            var $span = $(this);//.parent().find('span.show-hide');
            if ($span.hasClass('s-expand')) {
                $span.removeClass('s-expand');
                $span.addClass('s-collapse');
                $(window).trigger('resize');
            }
            else {
                $span.removeClass('s-collapse');
                $span.addClass('s-expand');
                $(window).trigger('resize');
            }
        });
    },

    InitDefaultValue: function () {
        this.Settings.Data.LCID = _spPageContextInfo.currentLanguage;
        if (_rbvhContext.EmployeeInfo != null)
        {
            this.Settings.Data.UserInfoId = _rbvhContext.EmployeeInfo.ID;
            this.Settings.Data.UserADId = _rbvhContext.EmployeeInfo.ADAccount.ID;
            this.Settings.Data.LocationId = _rbvhContext.EmployeeInfo.FactoryLocation.LookupId;
            this.Settings.Data.FullName = _rbvhContext.EmployeeInfo.FullName;
            this.Settings.Data.Position = _rbvhContext.EmployeeInfo.EmployeePosition.LookupValue;
            this.Settings.Data.IsBOD = this.Settings.Data.Position == 'Board of Director';
        }
    },

    PopulateDepartments: function () {
        var that = this;
        var lcid = _spPageContextInfo.currentLanguage;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.DepartmentList, location.host, lcid, that.Settings.Data.LocationId);
        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            //async: false,
            //success: function (result) {
            //    that.Settings.Departments = result;
            //}
        });
    },
    PopulateModules: function () {
        var that = this;
        var lcid = _spPageContextInfo.currentLanguage;
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.ModuleList, location.host, lcid);
        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            //async: false,
            //success: function (result) {
            //    that.Settings.Departments = result;
            //}
        });
    },
    PopulateGrid: function (condition) {
        var that = this;

        function applyFilter(data, filter) {
            return $.grep(data, function (client) {
                return (!filter.RequesterName || client.RequesterName.indexOf(filter.RequesterName) > -1)
                    && (!filter.DepartmentId || client.DepartmentId == filter.DepartmentId)
                    && (!filter.ModuleId || client.ModuleId == filter.ModuleId)
                    && (!filter.Description || client.Description.indexOf(filter.Description) > -1)
                    && (!filter.CreatedDate || client.CreatedDate.indexOf(filter.CreatedDate) > -1)
                    && (!filter.DueDate || client.DueDate.indexOf(filter.DueDate) > -1)
                    && (!filter.ApprovalStatusId || client.ApprovalStatusId == filter.ApprovalStatusId);
            });
        };

        $(that.Settings.Grid.Selector).jsGrid({
            height: "100%",
            width: "100%",

            filtering: true,
            sorting: true,
            paging: true,
            autoload: true,

            pageSize: 20,
            pageButtonCount: 5,
            pagerFormat: that.Settings.Grid.PagerFormat,
            pagePrevText: "<",
            pageNextText: ">",
            pageFirstText: "<<",
            pageLastText: ">>",
            noDataContent: that.Settings.Grid.EmptyDataTitle, // Resource
            loadMessage: that.Settings.Grid.LoadMessageTitle, //Resource

            controller: {
                loadData: function (filter) {
                    if (that.Settings.Data.Condition == '')
                        return [];

                    if (that.Settings.Grid.LoadedData)
                        return applyFilter(that.Settings.Grid.DataSource, filter);

                    var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.TaskList, location.host, that.Settings.Data.Condition, that.Settings.Data.UserADId, that.Settings.Data.UserInfoId, that.Settings.Data.FullName);
                    return $.ajax({
                        type: "GET",
                        url: url,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        cache: false,
                        //async: false,
                        success: function (result) {
                            // Hide Indicator:
                            $(that.Settings.Grid.ShaderCssClass).hide();
                            $(that.Settings.Grid.LoadPanelCssClass).hide();
                            // Bind Datasource
                            that.Settings.Grid.DataSource = result;
                            that.Settings.Grid.LoadedData = true;

                            // Rebind chart column:
                            //that.RebindChartColumnData(that.Settings.Data.Condition, result.length);

                            return that.Settings.Grid.DataSource;
                        }
                    });
                },
            },

            fields: [
                {
                    name: "ItemApprovalUrl", type: "text", width: 50, filtering: false, align: "center", title: "#",
                    itemTemplate: function (value, item) {
                        var $link = $("<a>").attr("href", value).attr("target","_blank");
                        var $eye = $("<i>").attr("class", "fa fa-eye");
                        $link.append($eye);
                        return $("<div>").append($link);
                    }
                },
                { name: "ModuleId", type: "select", title: that.Settings.Grid.Columns.Title.Module, width: "12%", items: that.Settings.Data.Modules, valueField: "ID", textField: "Name", align: "left" },
                { name: "RequesterName", type: "text", title: that.Settings.Grid.Columns.Title.Requester, width: "12%", align: "left" },
                { name: "DepartmentId", type: "select", title: that.Settings.Grid.Columns.Title.Department, width: "12%", items: that.Settings.Data.Departments, valueField: "Id", textField: "DepartmentName", align: "left", filtering: that.Settings.Data.IsBOD },
                { name: "Description", type: "text", title: that.Settings.Grid.Columns.Title.Description, width: 'auto', align: "left" },
                { name: "ApprovalStatusId", type: "select", title: that.Settings.Grid.Columns.Title.ApprovalStatus, width: "12%", items: that.Settings.Data.ApprovalStatus, valueField: "Id", textField: "Name", align: "center" },
                { name: "CreatedDate", type: "text", title: that.Settings.Grid.Columns.Title.CreatedDate, width: "12%", align: "center" },
                { name: "DueDate", type: "text", title: that.Settings.Grid.Columns.Title.DueDate, width: "12%", align: "center" },
                //{ name: "ApprovedDate", type: "text", title: "Ngày duyệt", width: 100, align: "left" },
                { type: "control", modeSwitchButton: false, editButton: false, deleteButton: false }
            ],

            onDataLoaded: function () {
                $('tr.jsgrid-row > td:nth-child(6), tr.jsgrid-alt-row > td:nth-child(6)').each(function () {
                    var $status = $(this);
                    var $span = RBVH.Stada.WebPages.Utilities.GUI.generateItemStatus($status.html());
                    $status.html($span);
                });
            }
        });


    },
    PopulateChart: function (taskOverviewModel) {
        var that = this;

        var colors = ['#888d91', '#c1140b', '#db1ed2', '#5cb85c'];

        // Init Chart
        var chart = new Highcharts.chart('container', {
            chart: {
                type: 'column',
                events: {
                    load: function (event) {
                        // Hide loading...
                        $(that.Settings.Chart.ShaderSelector).hide();
                        $(that.Settings.Chart.LoaderSelector).hide();
                        //$(".se-pre-con").fadeOut(0);
                    }
                }
            },
            xAxis: {
                categories: [that.Settings.Chart.Columns.Title.WaitingApprovalTitle, that.Settings.Chart.Columns.Title.WaitingApprovalTodayTitle, that.Settings.Chart.Columns.Title.InProcessTitle, that.Settings.Chart.Columns.Title.ApprovedTodayTitle],
                labels: {
                    style: {
                        color: '#575c60',
                        font: '14px "Helvetica Neue",Helvetica,Arial,sans-serif'
                    }
                }
            },
            title: {
                text: '', // Resource
                style: {
                    'fontSize': '2.77em',
                    'fontFamily': '"Helvetica Neue",Helvetica,Arial,sans-serif',
                }
            },
            yAxis: {
                allowDecimals: false,
                title: {
                    text: that.Settings.Chart.Columns.Title.LeftColumnTitle
                },
                labels: {
                    style: {
                        color: '#575c60',
                        font: '14px "Helvetica Neue",Helvetica,Arial,sans-serif'
                    }
                }
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                        this.point.y + ' ' + this.point.name.toLowerCase();
                }
            },
            plotOptions: {
                series: {
                    //pointWidth: 60,
                    cursor: 'pointer',
                    point: {
                        events: {
                            click: function () {
                                // Show Indicator:
                                $(that.Settings.Grid.ShaderCssClass).show();
                                $(that.Settings.Grid.LoadPanelCssClass).show();
                                // Set Grid title
                                $(that.Settings.Grid.TaskListTitleSelector).html(' - ' + this.category);
                                // Clear all filter
                                //$(that.Settings.Grid.Selector).jsGrid("clearFilter");
                                var myGrid = $(that.Settings.Grid.Selector).data("JSGrid");
                                if (myGrid != null)
                                {
                                    var $filterRow = myGrid._createFilterRow();
                                    myGrid._filterRow.replaceWith($filterRow);
                                    myGrid._filterRow = $filterRow;
                                }
                                
                                if (this.category == that.Settings.Chart.Columns.Title.WaitingApprovalTitle) { // Waiting approval
                                    that.Settings.Data.Condition = that.Settings.Condition.WaitingApproval;
                                }
                                else if (this.category == that.Settings.Chart.Columns.Title.WaitingApprovalTodayTitle) { // Waiting approval TODAY
                                    that.Settings.Data.Condition = that.Settings.Condition.WaitingApprovalToday;
                                }
                                else if (this.category == that.Settings.Chart.Columns.Title.InProcessTitle) { // In-Process
                                    that.Settings.Data.Condition = that.Settings.Condition.InProcess;
                                }
                                else if (this.category == that.Settings.Chart.Columns.Title.ApprovedTodayTitle) { // Approved TODAY
                                    that.Settings.Data.Condition = that.Settings.Condition.ApprovedToday;
                                }

                                // Reload data:
                                that.Settings.Grid.LoadedData = false;
                                $(that.Settings.Grid.Selector).jsGrid("loadData");
                            }
                        }
                    },
                    dataLabels: {
                        enabled: true,
                        crop: false,
                        overflow: 'none',
                        style: {
                            fontSize: "13px"
                        }
                    }
                }
            },
            legend: {
                enabled: false
            },

            series: [{
                name: that.Settings.Chart.Label, // Resource
                //colorByPoint: true,
                data: [{
                    name: that.Settings.Chart.Columns.Title.WaitingApprovalTitle,
                    y: taskOverviewModel.TotalWaitingApproval,
                    color: colors[0]
                }, {
                    name: that.Settings.Chart.Columns.Title.WaitingApprovalTodayTitle,
                    y: taskOverviewModel.TotalWaitingApprovalToday,
                    color: colors[1]
                }, {
                    name: that.Settings.Chart.Columns.Title.InProcessTitle,
                    y: taskOverviewModel.TotalInProcess,
                    color: colors[2]
                }, {
                    name: that.Settings.Chart.Columns.Title.ApprovedTodayTitle,
                    y: taskOverviewModel.TotalApprovedToday,
                    color: colors[3]
                }]
            }],

            exporting: {
                enabled: false
            },
            credits: {
                enabled: false
            },
        });

        // Auto resize
        var resizeTimer;
        $(window).resize(function () {
            clearTimeout(resizeTimer);
            resizeTimer = setTimeout(function () {
                //height = 300;
                var width = $('#container').parent().width();
                var height = 301;
                chart.setSize(width, height, doAnimation = true);
            }, 100);
        });
    },
    RebindChartColumnData: function(condition, newValue) {
        var chart = $('#container').highcharts();
        var oldValues = chart.series[0].data;
        if (oldValues != null)
        {
            var newValues = oldValues;
            switch(condition) {
                case this.Settings.Condition.WaitingApproval:
                    newValues[0].y = newValue;
                    break;
                case this.Settings.Condition.WaitingApprovalToday:
                    newValues[1].y = newValue;
                    break;
                case this.Settings.Condition.InProcess:
                    newValues[2].y = newValue;
                    break;
                case this.Settings.Condition.ApprovedToday:
                    newValues[3].y = newValue;
                    break;
                default:
                    // do nothing
            }
            chart.series[0].setData(newValues);
        }
        
    },

    GetOverviewData: function()
    {
        var that = this;
        // Show loading...
        //$(".se-pre-con").fadeIn(0);
        $(that.Settings.Chart.ShaderSelector).show();
        //$(that.Settings.Chart.LoaderSelector).show();
        // Resize loader:
        var $loadPanel = $(that.Settings.Chart.LoaderSelector).show();
        var actualWidth = $loadPanel.outerWidth();
        var actualHeight = $loadPanel.outerHeight();
        $loadPanel.css({
            marginTop: -actualHeight / 2,
            marginLeft: -actualWidth / 2
        });

        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.TaskOverview, location.host, that.Settings.Data.UserADId, that.Settings.Data.UserInfoId, that.Settings.Data.FullName);
        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            //success: function (result) {
            //}
        });
    },
    PopulateGridFilter: function (populateGrid) {
        var that = this;
        if (that.Settings.Data.LCID == 1066) // Vietnamese
            that.Settings.Data.ApprovalStatus = that.Settings.ApprovalStatusInVN;
        else
            that.Settings.Data.ApprovalStatus = that.Settings.ApprovalStatus;

        // Bind (Departmemt + Modules) -> Bind Grid + Chart
        $.when(that.PopulateDepartments(), that.PopulateModules()).done(function (departmentList, moduleList) {
            that.Settings.Data.Departments = departmentList[0];
            that.Settings.Data.Departments.unshift({ "DepartmentName": "", Id: 0 });
            that.Settings.Data.Modules = moduleList[0];
            that.Settings.Data.Modules.unshift({ "Name": "", Id: 0 });

            if (typeof populateGrid == 'function') {
                var processNextStep = populateGrid.bind(that);
                processNextStep();
            }
        });
    },
};
