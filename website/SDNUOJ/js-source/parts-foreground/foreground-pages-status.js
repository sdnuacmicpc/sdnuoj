SDNUOJ.namespace("SDNUOJ.pages.status");

SDNUOJ.pages.status = (function () {
    var user = SDNUOJ.user;

    return {
        init: function () {
            if (user.getIsLogined()) {
                function getSourceCodeLink(sid, name, lang, contest) {
                    if (user.hasSourceViewPermission() || user.getCurrentUserName() == name) {
                        return "<a href=\"/solution/view/" + sid + "\">" + lang + "</a>";
                    }
                    else {
                        return lang;
                    }
                }

                $(".status-language").each(function (i, e) {
                    var id = $(this).attr("id");

                    if (id.indexOf("lang_default_") == 0) {
                        var s = id.split('_');

                        $(this).html(getSourceCodeLink(s[2], s[3], $(this).html(), false));
                        $(this).attr("id", "lang_" + i);
                    }
                    else if (id.indexOf("lang_contest_") == 0) {
                        var s = id.split('_');

                        $(this).html(getSourceCodeLink(s[2], s[3], $(this).html(), true));
                        $(this).attr("id", "lang_" + i);
                    }
                });
            }
        }
    };
})();