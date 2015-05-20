SDNUOJ.namespace("SDNUOJ.user");

SDNUOJ.user = (function () {
    var cookies = SDNUOJ.util.cookies;
    var storage = SDNUOJ.util.storage;

    var FORMSNAME = "_oj_";
    var BROWSERSTATUSKEY = FORMSNAME + "bs";
    var USERLASTLANGUAGEKEY = FORMSNAME + "la";
    var SUBMITLISTKEY = FORMSNAME + "sl";

    var _browserStatus = cookies.get(BROWSERSTATUSKEY);
    var _browserStatusList = (_browserStatus != "" ? _browserStatus.split('|') : null);
    var _userName = (_browserStatusList != null ? _browserStatusList[0] : "");
    var _userPermission = (_browserStatusList != null ? _browserStatusList[1] : "");
    var _userUnReadMail = (_browserStatusList != null ? _browserStatusList[2] : "");

    if (_userUnReadMail == "") {
        _userUnReadMail = "0";
    }

    var _isLogined = (function () {
        var logined = false;

        if (_userName == "") {
            cookies.remove(BROWSERSTATUSKEY);
        }
        else {
            logined = true;
        }

        if (!logined) {
            storage.clear();
        }

        return logined;
    })();

    function hasPermission(p) {
        return ((_userPermission & p) == p);
    };

    return {
        getIsLogined: function () {
            return _isLogined;
        },
        getCurrentUserName: function () {
            return _userName;
        },
        getCurrentPermission: function () {
            return _userPermission;
        },
        getUnreadMail: function () {
            return _userUnReadMail;
        },
        getLastLanguage: function() {
            var value = cookies.get(USERLASTLANGUAGEKEY);

            return (!value || value == "") ? 0 : parseInt(value);
        },
        setLastLanguage: function(value) {
            cookies.set(USERLASTLANGUAGEKEY, value, 365 * 24 * 60 * 60, "/");
        },
        hasPermission: function (p) {
            return hasPermission(p);
        },
        hasSourceViewPermission: function () {
            return hasPermission(SDNUOJ.PM.SOURCEVIEW);
        },
        hasAdministratorPermission: function () {
            return hasPermission(SDNUOJ.PM.ADMINISTRATOR);
        },
        hasContestManagePermission: function () {
            return hasPermission(SDNUOJ.PM.CONTESTMANAGE);
        },
        getSubmitList: function (done) {
            var data = storage.get(SUBMITLISTKEY);

            if (data != null) {
                done($.parseJSON(data));
                return;
            }

            $.getJSON("/solution/submitlist").done(function (data) {
                storage.set(SUBMITLISTKEY, JSON.stringify(data), 30);

                done(data);
            });
        }
    }
})();