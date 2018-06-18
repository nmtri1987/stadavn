if (typeof RBVH == 'undefined' || !RBVH) RBVH = {};
if (typeof RBVH.Stada == 'undefined' || !RBVH.Stada) RBVH.Stada = {};
if (typeof RBVH.Stada.javascript == 'undefined' || !RBVH.Stada.javascript) RBVH.Stada.javascript = {};
if (typeof RBVH.Stada.javascript.common == 'undefined' || !RBVH.Stada.javascript.common) RBVH.Stada.javascript.common = {};
RBVH.Stada.javascript.common.NamespaceManager = {
    register: function (namespace) {
        namespace = namespace.split('.');

        if (!window[namespace[0]])
            window[namespace[0]] = {};

        var strFullNamespace = namespace[0];
        for (var i = 1; i < namespace.length; i++) {
            strFullNamespace += "." + namespace[i];
            eval("if(typeof window." + strFullNamespace + "== 'undefined' || !window." + strFullNamespace + ") window." + strFullNamespace + "={};");
        }
    }
};

RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.Utilities");
RBVH.Stada.WebPages.Utilities = {
    GetValueByParam: function (variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (pair[0].toUpperCase() == variable.toUpperCase()) {
                return pair[1];
            }
        }
    },
    GetValueByParamURL: function getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    },
    UpdateQueryStringParameter: function (uri, key, value) {
        var re = new RegExp("([?&])" + key + "=.*?(&|$)", "i");
        var separator = uri.indexOf('?') !== -1 ? "&" : "?";
        if (uri.match(re)) {
            return uri.replace(re, '$1' + key + "=" + value + '$2');
        }
        else {
            return uri + separator + key + "=" + value;
        }
    }
    ,

    String:
    {
        format: function () {
            var s = arguments[0];
            for (var i = 0; i < arguments.length - 1; i++) {
                
                var reg = new RegExp("\\{" + i + "\\}", "gm");
                s = s.replace(reg, arguments[i + 1]);
            }
            return s;
        },
        toJSDate: function () {
            var value = arguments[0]; //"/Date(1337878800000+0700)/";
            if (value.substring(0, 6) == "/Date(") {
                var dt = new Date(parseInt(value.substring(6, value.length - 2)));
                return dt;
            }
            return null;
        },
        toISOString: function () {
            var value = arguments[0];
            if (value instanceof Date) {
                return value.getUTCFullYear() + '-' + '0' + (value.getUTCMonth() + 1) + value.getUTCDate();
            }
            return '';
        },
        toMomentDateTime: function () {
            var value = arguments[0];

            try {
                //var dateFormat = arguments[1];
                //var lcid = arguments[1];
                //if (lcid == "1066") // vi-VN
                //{
                //    return moment(value).toDate();
                //}
                //else if (lcid == "1033") // en-US
                //{
                //    return moment(value).toDate();
                //}
                return moment(value).toDate();
            }
            catch (err) {
                return value;
            }
        },
        parseISOLocal: function (s) {
            var b = s.split(/\D/);
            return new Date(b[0], b[1] - 1, b[2], b[3], b[4], b[5]);
        },
        padDate: function (str) {
            if (str !== undefined && str !== "") {
                return str <= 9 ? ("0" + str) : str;
            }
            return "";
        },
        toISOStringTZ: function (dateObject) {
            var tzoffset = (new Date()).getTimezoneOffset() * 60000; //offset in milliseconds
            var localISOTime = (new Date(dateObject.getTime() - tzoffset)).toISOString().slice(0, -1);
            return localISOTime;
        },
    },
    GUI: {
        generateItemStatus: function (status) {
            var className = 'label label-default';
            var statusText = status;

            if (status == RBVH.Stada.WebPages.Constants.ApprovalStatus.En_US.Approved || status == RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Approved) {
                className = 'label label-success';
                if (_spPageContextInfo.currentLanguage == '1066') // vi-VN
                    statusText = RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Approved
            }
            else if (status == RBVH.Stada.WebPages.Constants.ApprovalStatus.En_US.Completed || status == RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Completed) {
                className = 'label label-success';
                if (_spPageContextInfo.currentLanguage == '1066') // vi-VN
                    statusText = RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Completed
            }
            else if (status == RBVH.Stada.WebPages.Constants.ApprovalStatus.En_US.Cancelled || status == RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Cancelled) {
                className = 'label label-warning';
                if (_spPageContextInfo.currentLanguage == '1066') // vi-VN
                    statusText = RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Cancelled
            }
            else if (status == RBVH.Stada.WebPages.Constants.ApprovalStatus.En_US.Rejected || status == RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Rejected) {
                className = 'label label-danger';
                if (_spPageContextInfo.currentLanguage == '1066') // vi-VN
                    statusText = RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.Rejected
            }
            else if (status == RBVH.Stada.WebPages.Constants.ApprovalStatus.En_US.InProcess || status == RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.InProcess) {
                className = 'label label-primary';
                if (_spPageContextInfo.currentLanguage == '1066') // vi-VN
                    statusText = RBVH.Stada.WebPages.Constants.ApprovalStatus.Vi_VN.InProcess
            }
            return $('<span />').attr('class', className).html(statusText);
        },
        showRequestExpired: function (errCtrlContainer, errCtrlId, msg) {
            $(errCtrlId).html('<span class="label label-danger">' + msg + '</span>');
            $(errCtrlContainer).show();
        }
    }
};

function getQueryStringParameter(paramToRetrieve) {
    var params =
    document.URL.split("?")[1].split("&");
    var strParams = "";
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve)
            return singleParam[1];
    }
}

function openModalDialog(title, url, dialogReturnValueCallback, args) {
    var options = {
        title: title,
        url: url,
        allowMaximize: false,
        showMaximized: false,
        showClose: false,
        autoSize: true,
        dialogReturnValueCallback: dialogReturnValueCallback,
        args: args
    };

    SP.SOD.execute('sp.ui.dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
}