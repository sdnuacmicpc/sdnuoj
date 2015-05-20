SDNUOJ.namespace("SDNUOJ.pages.mailbox");

SDNUOJ.pages.mailbox = (function () {
    var user = SDNUOJ.user;

    return {
        init: function () {
            $("#form-mailbox-delete").submit(function (e) {
                if ($(this).serialize().indexOf("mailid") < 0) {
                    alert("Please select at least one mail!");
                    return false;
                }

                if (!window.confirm("Are you sure you want to delete selected mail(s)?")) {
                    return false;
                }

                return true;
            });
        }
    };
})();

SDNUOJ.namespace("SDNUOJ.pages.mail");

SDNUOJ.pages.mail = (function () {
    var user = SDNUOJ.user;

    return {
        init: function () {
            $("#form-mail-delete").submit(function (e) {
                if (!window.confirm("Are you sure you want to delete this mail?")) {
                    return false;
                }

                return true;
            });
        }
    };
})();