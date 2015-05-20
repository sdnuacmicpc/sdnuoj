SDNUOJ.namespace("SDNUOJ.pages.userinfo");

SDNUOJ.pages.userinfo = (function () {
    var user = SDNUOJ.user;

    return {
        init: function () {
            if (user.getIsLogined()) {
                var username = user.getCurrentUserName().toLowerCase();
                var container = $("#downloadcode_" + username);

                if (container.length > 0) {
                    container.html("(<a href=\"/solution/sourcecodes\">Download</a>)");
                }
            }
        }
    }
})();