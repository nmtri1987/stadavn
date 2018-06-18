var currentLabel = "";
var webProperties;
var rootWeb;
var url;
function varLabelsList() {
    SP.SOD.executeFunc('SP.Runtime.js', 'SP.ClientContext',
      function () {
          SP.SOD.executeFunc('SP.js', 'SP.ClientContext',
          function () {
              var siteUrl = _spPageContextInfo.webAbsoluteUrl;
              var ctx = new SP.ClientContext(siteUrl);
              rootWeb = ctx.get_site().get_rootWeb();
              webProperties = rootWeb.get_allProperties();
              ctx.load(rootWeb);
              ctx.load(webProperties);

              ctx.executeQueryAsync(function () {

                  var varLabelsListId = webProperties.get_item('_VarLabelsListId');

                  var labelsList = rootWeb.get_lists().getById(varLabelsListId);
                  var labelItems = labelsList.getItems(SP.CamlQuery.createAllItemsQuery());
                  ctx.load(labelItems);
                  ctx.executeQueryAsync(
                      function () {

                          var html = "";
                          var currentUrl = window.location.href;
                          var e = labelItems.getEnumerator();

                          while (e.moveNext()) {
                              var labelItem = e.get_current();
                              var label = labelItem.get_item('Title');
                              var displayName = labelItem.get_item('Flag_x0020_Control_x0020_Display');
                              var isCreated = labelItem.get_item('Hierarchy_x0020_Is_x0020_Created');
                              if (isCreated) {
                                  if (currentUrl.indexOf(label) >= 0) {
                                      currentLabel = label;
                                      html += "<option value='" + label + "' selected='selected'>" + displayName + "</option>";
                                  } else {
                                      html += "<option value='" + label + "'>" + displayName + "</option>";
                                  }
                              }
                          }

                          $("#varLabelList").html(html);

                      }, onError);

              }, onError);
          });
      });
}
function onError(sender, args) {
    alert(args.get_message() + '\n' + args.get_stackTrace());
}
_spBodyOnLoadFunctionNames.push("varLabelsList");
$(function () {

    url = document.URL;
    var enUrl = updateUrlParameter(url, "lang", "en-US");
    var viUrl = updateUrlParameter(url, "lang", "vi-VN");

    $(".ms-cui-topBar2 #RibbonContainer-TabRowRight").append("<span id ='flag' class='ms-qatbutton'><a href='" + enUrl + "'><img src='/_layouts/15/rbvh.stada.intranet.branding/images/language_flag/en.png'/></a><a href='" + viUrl + "'><img src='/_layouts/15/rbvh.stada.intranet.branding/images/language_flag/vn.png'/></a></span>");
});







