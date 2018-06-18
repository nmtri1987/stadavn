$(function () {
    setNavigation();
});

function setNavigation() {
    var path = window.location.pathname;
    path = path.replace(/\/$/, "");
    path = decodeURIComponent(path);

    $("ul.nav li").each(function () {
        var href = $(this).find('a').attr("item-data");
        if (path.indexOf(href)>=0) {
            $(this).closest('li').css("background", "rgb(226, 226, 226)");
            setParent($(this));
        }
    });
}


function setParent(element)
{
    var parent = element.parent().closest("li");
    if(parent == null)
    {
        return;
    }
    else
    {
        parent.closest('li').css("background", "rgb(226, 226, 226)");
        setParent(parent);
    }

}