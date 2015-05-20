SDNUOJ.namespace("SDNUOJ.pages.submitcode");

SDNUOJ.pages.submitcode = (function () {
    var user = SDNUOJ.user;
    var highlight = SDNUOJ.page.highlight;

    return {
        init: function () {
            var lastlang = user.getLastLanguage();
            var haslang = false;

            var options = $("#lang").children("option");

            for (var i = 0; i < options.length; i++) {
                if (options[i].value == lastlang) {
                    haslang = true;
                    break;
                }
            }

            if (haslang) {
                $("#lang").val(lastlang);
            }

            $("#lang").change(function () {
                var langID = $("#lang").val();

                user.setLastLanguage(langID);
                highlight.setAllLanguage(langID);
            });

            $("button[type='reset']").click(function () {
                highlight.clearAllText("");
            });
        }
    }
})();