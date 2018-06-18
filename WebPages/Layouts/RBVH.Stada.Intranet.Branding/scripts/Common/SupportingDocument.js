RBVH.Stada.javascript.common.NamespaceManager.register("RBVH.Stada.WebPages.pages");

RBVH.Stada.WebPages.pages.SupportingDocument = function (settings) {
    this.Settings = {
        DocumentCount: 0,
    }
    $.extend(true, this.Settings, settings);
    this.Initialize();
};

RBVH.Stada.WebPages.pages.SupportingDocument.prototype =
{
    Initialize: function () {
        var that = this;
        $(document).ready(function () {
            ExecuteOrDelayUntilScriptLoaded(function () {
                that.InitControls();
                that.EventsRegister();
            }, "sp.js");
        });
    },

    InitControls: function () {
        var that = this;


    },
    EventsRegister: function () {
        var that = this;

        $(document).on("click", that.Settings.Controls.AddMoreFileSelector, function () {
            that.AppendInputFile();
        })
        // debugger;
        // $(that.Settings.Controls.AddDocumentButtonSelector).on("click", function(){
        //     that.AppendInputFile();
        // });    

        $(document).on("click", ".span-remove", function () {
            var currentSpan = $(this);
            var inputParent = currentSpan.attr("spanName");
            $("input[name='" + inputParent + "']").remove();
            $("span[spanName='" + inputParent + "']").next().remove();
            $("span[spanName='" + inputParent + "']").next().remove();
            $("span[spanName='" + inputParent + "']").remove();
        })

    },
    AppendInputFile: function () {
        var that = this;
        var countNumer = that.Settings.DocumentCount++;
        $(that.Settings.Controls.GridSupportingDocumentSelector).append("<input type='file' name='supportingDocument" + countNumer + "' style='display:inline'>  <span spanName='supportingDocument" + countNumer + "' class='glyphicon glyphicon-trash span-remove'></span>  <span spanErrorName='supportingDocument" + countNumer + "' class='ms-formvalidation ms-csrformvalidation'></span><br/>")
    },
    ValidateAttachments: function () {
        var errorCount = 0;
        var that = this;
        var inputFileControls = $("input[name^='supportingDocument']");
        $("span[spanErrorName^='supportingDocument']").html("");
        if (inputFileControls && inputFileControls.length > 0) {
            for (var idx = 0; idx < inputFileControls.length; idx++) {
                var fileName = inputFileControls[idx].value;
                var currentfileInputControlName = $(inputFileControls[idx]).attr("name");

                if (!fileName || fileName == "") {
                    $("span[spanErrorName='" + currentfileInputControlName + "']").html(that.Settings.Message.CantLeaveTheBlank);
                    errorCount++;
                }
                else {
                    $("span[spanErrorName='" + currentfileInputControlName + "']").html("");
                    if (that.IsValidFileName(fileName) == false) {
                        if (currentfileInputControlName) {
                            $("span[spanErrorName='" + currentfileInputControlName + "']").html(that.Settings.Message.InvalidFileName);
                            errorCount++;
                        }
                    }
                    if (inputFileControls[idx] && inputFileControls[idx].files[0] && inputFileControls[idx].files[0].size == 0) {
                        $("span[spanErrorName='" + currentfileInputControlName + "']").html(that.Settings.Message.FileEmpty);
                        errorCount++;
                    }
                }
            }
        }
        return errorCount == 0;
    },
    IsValidFileName: function (fileName) {
        if (!fileName)
            return false;
        return !(fileName.indexOf("#") !== -1);
    },
}