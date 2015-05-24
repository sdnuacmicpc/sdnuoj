(function () {
    if (SDNUOJ.util.browser.ltIE8()) {
        $("#browser-warning").show();
    }

    $("[placeholder]").placeholder();
    $(".img-checkcode").trigger("click");

    SDNUOJ.util.form.setFormAutoVerify();
    SDNUOJ.util.form.setTextboxQuickSubmit();
    SDNUOJ.util.list.setCheckboxSelectAll();

    SDNUOJ.page.highlight.setAllHighlightEditors();
    SDNUOJ.page.highlight.setAllHighlightViewers();

    SDNUOJ.page.tree.setImageCollapse();

    SDNUOJ.page.foreground.initPage();
    SDNUOJ.page.foreground.contest.initPage();
})();

var _hmt = _hmt || [];
(function () {
    var id = $("#main-container").attr("data-tjid");

    if (id != "") {
        var hm = document.createElement("script");
        hm.src = "//hm.baidu.com/hm.js?" + id;
        var s = document.getElementsByTagName("script")[0];
        s.parentNode.insertBefore(hm, s);
    }
})();