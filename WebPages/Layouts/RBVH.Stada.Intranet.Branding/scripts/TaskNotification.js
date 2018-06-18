RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");

RBVH.Stada.WebPages.pages.TaskNotification = function (settings) {
    this.Protocol = window.location.protocol;
    this.Settings = {
        ServiceUrls:
        {
            TaskList: this.Protocol + '//{0}/_vti_bin/Services/Common/CommonService.svc/GetTaskByCondition/{1}/{2}/{3}/{4}',
        },
    },

    $.extend(true, this.Settings, settings);

    this.Initialize();
};

RBVH.Stada.WebPages.pages.TaskNotification.prototype =
{
    Initialize: function () {
        var that = this;

        $(document).ready(function () {

            
            that.PopulateTaskCounter();
        });
    },

    PopulateTaskCounter: function () {
        var that = this;
        var userInfoId = 0;
        var userADId = 0;
        var approverFullName = 'Administrator';
        if (_rbvhContext.EmployeeInfo != null)
        {
            userInfoId = _rbvhContext.EmployeeInfo.ID;
            userADId = _rbvhContext.EmployeeInfo.ADAccount.ID;
            approverFullName = _rbvhContext.EmployeeInfo.FullName;
        }
        
        var url = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.ServiceUrls.TaskList, location.host, "WaitingApprovalToday", userADId, userInfoId, approverFullName);
        return $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            success: function (result) {
                // Bind Datasource
                if (result != null)
                {
                    if (result.length > 0) {
                        var tooltipMessage = RBVH.Stada.WebPages.Utilities.String.format(that.Settings.Tooltip, result.length);
                        $(that.Settings.CounterContainerSelector).attr('title', tooltipMessage);
                        $(that.Settings.CounterContainerSelector).tooltip({
                            tooltipClass: "notification-tooltip",
                        });
                        $(that.Settings.CounterContainerSelector).css('display', 'table-cell');
                        $(that.Settings.CounterSelector).html(result.length);
                    }
                    else
                        $(that.Settings.CounterContainerSelector).css('display', 'none');
                    
                }
            }
        });
    },
};
