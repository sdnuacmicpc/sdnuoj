/*!
 * SDNU Online Judge
 * https://github.com/sdnuacmicpc/sdnuoj
 *
 * Copyright 2012-2015 SDNU ACM-ICPC TEAM
 * Licensed under the MIT license
 */

var SDNUOJ = SDNUOJ || {};

SDNUOJ.namespace = function (ns_string) {
    var parts = ns_string.split('.'),
        parent = SDNUOJ;

    if (parts[0] === "SDNUOJ") {
        parts = parts.slice(1);
    }

    for (var i = 0; i < parts.length; i += 1) {
        if (typeof parent[parts[i]] === "undefined") {
            parent[parts[i]] = {};
        }

        parent = parent[parts[i]];
    }

    return parent;
};

SDNUOJ.PM = {
    SOURCEVIEW: 0x2,
    ADMINISTRATOR: 0x80,
    NEWSMANAGE: 0x100,
    RESOURCEMANAGE: 0x200,
    FORUMMANAGE: 0x400,
    PROBLEMMANAGE: 0x800,
    CONTESTMANAGE: 0x1000,
    SOLUTIONMANAGE: 0x2000,
    SUPERADMINISTRATOR: 0x3FFFFFFE
};
SDNUOJ.namespace("SDNUOJ.util.cookies");

SDNUOJ.util.cookies = (function () {
    return {
        set: function (key, value, timeout, path, secure) {
            var cookies = key + "=" + escape(value);

            if (timeout > 0) {
                var date = new Date();
                date.setTime(date.getTime() + timeout * 1000);
                cookies += ";expires=" + date.toGMTString();
            }

            if (path != null) {
                cookies += ";path=" + path;
            }

            if (secure) {
                cookies += ";secure";
            }

            document.cookie = cookies;
        },
        get: function (key) {
            var arr = document.cookie.split("; ");

            for (var i = 0; i < arr.length; i++) {
                var temp = arr[i].split("=");

                if (temp[0] == key) return unescape(temp[1]);
            }

            return "";
        },
        remove: function (key) {
            document.cookie = key + "=;expires=" + (new Date(0)).toGMTString();
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.storage");

SDNUOJ.util.storage = (function () {
    var cookies = SDNUOJ.util.cookies;

    return {
        set: function (key, value, timeout) {
            var now = new Date().getTime();

            if (window.sessionStorage) {
                window.sessionStorage[key] = value;
                window.sessionStorage[key + "_timeset"] = now;
                window.sessionStorage[key + "_timeout"] = timeout;
            }
            else {
                cookies.set(key, value, timeout, "/", true);
            }
        },
        get: function (key) {
            var now = new Date().getTime();
            var last, span, timeout, value;

            if (window.sessionStorage) {
                last = window.sessionStorage[key + "_timeset"];
                timeout = window.sessionStorage[key + "_timeout"];
                value = window.sessionStorage[key];

                if (last == null || timeout == null) {
                    value = null;
                }

                if (value != null && last != null && timeout != null) {
                    span = (now - last) / 1000;
                    if (span > timeout) value = null;
                }
            }
            else {
                value = cookies.get(key);
            }

            return (value != "" ? value : null);
        },
        clear: function () {
            if (window.sessionStorage) {
                window.sessionStorage.clear();
            }
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.date");

SDNUOJ.util.date = (function () {
    var WEEKDAY_SHORTNAME = new Array("日", "一", "二", "三", "四", "五", "六");
    var WEEKDAY_LONGNAME = new Array("星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六");

    return {
        format: function (date, format) {
            if (!date) {
                return;
            }

            if (!format) {
                format = "yyyy-MM-dd HH:mm:ss";
            }

            switch (typeof date) {
                case "string":
                    date = new Date(date.replace(/-/g, "/"));
                    break;
                case "number":
                    date = new Date(date);
                    break;
            }

            if (!date instanceof Date) {
                return;
            }

            var dict = {
                "yyyy": date.getFullYear(),
                "M": date.getMonth() + 1,
                "d": date.getDate(),
                "H": date.getHours(),
                "m": date.getMinutes(),
                "s": date.getSeconds(),
                "MM": ("" + (date.getMonth() + 101)).substr(1),
                "dd": ("" + (date.getDate() + 100)).substr(1),
                "HH": ("" + (date.getHours() + 100)).substr(1),
                "mm": ("" + (date.getMinutes() + 100)).substr(1),
                "ss": ("" + (date.getSeconds() + 100)).substr(1),
                "ddd": WEEKDAY_SHORTNAME[date.getDay()],
                "dddd": WEEKDAY_LONGNAME[date.getDay()]
            };

            return format.replace(/(yyyy|MM?|d{1,4}|HH?|ss?|mm?)/g, function () {
                return dict[arguments[0]];
            });
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.form");

SDNUOJ.util.form = (function () {
    function verifyForm(o) {
        var children = o.find("input,textarea");

        for (var i = 0; i < children.length; i++) {
            var child = $(children[i]);

            if (child.is(":hidden") && !child.is("textarea[data-editor='true']")) {
                continue;
            }

            if (child.attr("data-emptyinfo") && child.val() == "") {
                alert(child.attr("data-emptyinfo"));
                return false;
            }

            if (child.attr("data-comparedto") && child.attr("data-notequalinfo") && child.val() != $("#" + child.attr("data-comparedto")).val()) {
                alert(child.attr("data-notequalinfo"));
                return false;
            }

            if (child.attr("data-regex") && child.attr("data-invalidinfo") && !(new RegExp(child.attr("data-regex")).test(child.val()))) {
                alert(child.attr("data-invalidinfo"));
                return false;
            }
        }

        return true;
    }

    function quickSubmit(o, e) {
        var et = e || window.event;
        var k = et.keyCode || et.which || et.charCode;

        if (et.ctrlKey && k == 13) {
            o.parents("form").trigger("submit");
        }
    }

    return {
        verifyForm: verifyForm,
        setFormAutoVerify: function () {
            $("form[data-verify='true']").submit(function (e) {
                return verifyForm($(this));
            });
        },
        quickSubmit: quickSubmit,
        setTextboxQuickSubmit: function () {
            $("input[data-quicksubmit='true'],textarea[data-quicksubmit='true']").keydown(function (e) {
                return quickSubmit($(this), e);
            });
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.dom");

SDNUOJ.util.dom = (function () {
    return {
        getCheckboxValue: function (objs) {
            var ret = "";
            var count = 0;

            for (var i = 0; i < objs.length; i++) {
                if (objs[i].checked) {
                    if (count > 0) {
                        ret += ",";
                    }

                    ret += $(objs[i]).val();
                    count++;
                }
            }
            
            return (count > 0 ? ret : null);
        },
        getRadioValue: function (objs) {
            for (var i = 0; i < objs.length; i++) {
                if (objs[i].checked) {
                    return $(objs[i]).val();
                }
            }

            return null;
        },
        getValue: function (name) {
            var o = $("input[name='" + name + "'],select[name='" + name + "'],textarea[name='" + name + "']");

            if (o.length <= 0) {
                return null;
            }

            if (o.is("input[type='checkbox']")) {
                return this.getCheckboxValue(o);
            }
            else if (o.is("input[type='radio']")) {
                return this.getRadioValue(o);
            }
            else {
                return o.val();
            }
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.file");

SDNUOJ.util.file = (function () {
    return {
        getFileExtension: function (fileName) {
            return fileName.substring(fileName.lastIndexOf('.')).toLowerCase();
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.list");

SDNUOJ.util.list = (function () {
    function selectAll(o) {
        var children = $("input[name='" + o.attr("data-selectall-name") + "']");

        for (var i = 0; i < children.length; i++) {
            children[i].checked = o[0].checked;
        }
    }

    return {
        selectAll: selectAll,
        setCheckboxSelectAll: function () {
            $("input[data-selectall-name]").change(function (e) {
                return selectAll($(this));
            });
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.loader");

SDNUOJ.util.loader = (function () {
    return {
        load: function (url) {
            var urls = typeof url == "string" ? [url] : url;
            
            for (var i = 0; i < urls.length; i++) {
                var name = urls[i];
                var att = name.split('.');
                var ext = att[att.length - 1].toLowerCase();
                var isCSS = ext == "css";
                var tag = isCSS ? "link" : "script";
                var attr = isCSS ? ' type="text/css" rel="stylesheet" ' : ' language="javascript" type="text/javascript" ';
                var link = (isCSS ? "href" : "src") + '="' + name + '"';

                if ($(tag + "[" + link + "]").length == 0) {
                    document.write("<" + tag + attr + link + "></" + tag + ">");
                }
            }
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.util.browser");

SDNUOJ.util.browser = (function () {
    return {
        ltIE8: function () {
            var browser = navigator.appName
            var b_version = navigator.appVersion
            var version = b_version.split(";");
            var trim_Version = (version && version.length > 1 ? version[1].replace(/[ ]/g, "") : "");
            
            if (browser == "Microsoft Internet Explorer" && (trim_Version == "MSIE6.0" || trim_Version == "MSIE7.0")) {
                return true;
            }

            return false;
        }
    }
})();
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
SDNUOJ.namespace("SDNUOJ.admin.framework");

SDNUOJ.admin.framework = (function () {
    var user = SDNUOJ.user;

    var MENUS = [
        {
            id: 1,
            title: "常规管理",
            permission: SDNUOJ.PM.ADMINISTRATOR,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "系统综合信息", permission: SDNUOJ.PM.ADMINISTRATOR, url: "welcome" },
                { title: "系统信息检测", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/info" },
                { title: "系统公告管理", permission: SDNUOJ.PM.NEWSMANAGE, url: "news/list" },
                { title: "上传文件管理", permission: SDNUOJ.PM.ADMINISTRATOR, url: "upload/list" },
                { title: "下载资源管理", permission: SDNUOJ.PM.RESOURCEMANAGE, url: "resource/list" },
                { title: "专题页面管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "topicpage/list" },
                { title: "运行参数管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/config" },
                { title: "系统缓存管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/cachelist" }
            ]
        },
        {
            id: 2,
            title: "题目管理",
            permission: SDNUOJ.PM.PROBLEMMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "题目信息添加", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/add" },
                { title: "题目信息导入", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "problem/import" },
                { title: "题目综合管理", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/list" },
                { title: "题目分类管理", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/categorylist" }
            ]
        },
        {
            id: 3,
            title: "竞赛管理",
            permission: SDNUOJ.PM.CONTESTMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "竞赛信息添加", permission: SDNUOJ.PM.CONTESTMANAGE, url: "contest/add" },
                { title: "竞赛综合管理", permission: SDNUOJ.PM.CONTESTMANAGE, url: "contest/list" }
            ]
        },
        {
            id: 4,
            title: "用户管理",
            permission: SDNUOJ.PM.SUPERADMINISTRATOR,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "在线用户查看", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/online" },
                { title: "用户密码修改", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/changepassword" },
                { title: "用户信息管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/list" },
                { title: "用户权限管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/permissionlist" }
            ]
        },
        {
            id: 5,
            title: "论坛管理",
            permission: SDNUOJ.PM.FORUMMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "论坛主题管理", permission: SDNUOJ.PM.FORUMMANAGE, url: "forum/topiclist" },
                { title: "论坛帖子管理", permission: SDNUOJ.PM.FORUMMANAGE, url: "forum/postlist" }
            ]
        },
        {
            id: 6,
            title: "评测管理",
            permission: SDNUOJ.PM.SOLUTIONMANAGE,
            accessOnly: false,
            defaultSubID: (user.hasPermission(SDNUOJ.PM.SUPERADMINISTRATOR) ? 0 : 2),
            allmenus: [
                { title: "评测机管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "judger/list" },
                { title: "提交评测管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "solution/list" },
                { title: "重新评测提交", permission: SDNUOJ.PM.SOLUTIONMANAGE, url: "solution/rejudge" }
            ]
        },
        {
            id: 7,
            title: "数据库管理",
            permission: SDNUOJ.PM.SUPERADMINISTRATOR,
            accessOnly: true,
            defaultSubID: 0,
            allmenus: [
                { title: "系统数据库压缩", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/compact" },
                { title: "系统数据库备份", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/backup" },
                { title: "系统数据库还原", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/restore" },
                { title: "系统数据库上传", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/upload" },
                { title: "系统数据库管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/list" }
            ]
        }
    ];

    function createMenu(o) {
        if (!user.hasPermission(o.permission)) {
            return false;
        }

        if (o.accessOnly && $("#menus").attr("data-isaccessdb").toLowerCase() != "true") {
            return false;
        }
        
        var link = $("<a>").attr("href", "javascript:void(0);").attr("data-menuid", o.id)
            .addClass(o.id == 1 ? "menu-open" : "menu-close")
            .append($("<span>").html(o.title))
            .click(function (e) {
                $("#menus a.menu-open").removeClass("menu-open").addClass("menu-close");
                $(this).removeClass("menu-close").addClass("menu-open");
                showSubMenus($(this).attr("data-menuid"));
            });

        return link;
    }

    function createSubMenu(o, selected) {
        if (!user.hasPermission(o.permission)) {
            return false;
        }

        var link = $("<a>").attr("href", "/admin/" + o.url).attr("target", "admin-page").html(o.title)
            .addClass(selected ? "submenu-open" : "submenu-close")
            .click(function (e) {
                $("#submenus a.submenu-open").removeClass("submenu-open").addClass("submenu-close");
                $(this).removeClass("submenu-close").addClass("submenu-open");
            });

        return link;
    }

    function showMenus() {
        for (var i = 0; i < MENUS.length; i++) {
            var link = createMenu(MENUS[i]);
            
            if (link) {
                $("#menus").append(link);
            }
        }
    }

    function showSubMenus(id) {
        $("#submenus").html("");

        if (!user.hasAdministratorPermission()) {
            return false;
        }

        var submenus = null;

        for (var i = 0; i < MENUS.length; i++) {
            if (MENUS[i].id == id) {
                submenus = MENUS[i];
                break;
            }
        }

        if (submenus == null) {
            return false;
        }

        for (var i = 0; i < submenus.allmenus.length; i++) {
            var link = createSubMenu(submenus.allmenus[i], (i == submenus.defaultSubID));

            if (link) {
                $("#submenus").append(link);
            }
        }

        var defaultSubID = submenus.defaultSubID;
        window.parent.frames["admin-page"].document.location.href = "/admin/" + submenus.allmenus[defaultSubID].url;
    }

    return {
        showMenus: showMenus,
        showSubMenus: showSubMenus
    };
})();

(function () {
    SDNUOJ.admin.framework.showMenus();
    SDNUOJ.admin.framework.showSubMenus(1);

    if (SDNUOJ.util.browser.ltIE8()) {
        alert("您的浏览器版本小于Internet Explorer 8，部分功能将无法使用，请更新您的浏览器或使用其他浏览器！");
        window.close();
    }
})();
