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
SDNUOJ.namespace("SDNUOJ.admin.editor");

SDNUOJ.admin.editor = (function () {
    var _instances = new Array();

    function addEditor(tinyeditor, id, style) {
        if (!tinyeditor) {
            return;
        }

        if (!style) {
            style = {};
        }

        style.id = id;
        style.cssfile = ['/static/css/foundation/bootstrap-3.3.4.min.css', '/static/css/foreground.min.css'];
        style.css = "html { position: static; } body { margin:0; padding: 0; }";

        var editor = new tinyeditor.edit('editor' + _instances.length, style);

        _instances.push(editor);
    }

    return {
        addEditor: addEditor,
        setAllEditors: function () {
            if ($("textarea[data-editor='true']").length > 0) {
                $.getScript("/static/js/foundation/tinyeditor.min.js", function () {
                    var tinyeditor = null;

                    if ("undefined" != typeof (TINY)) {
                        tinyeditor = TINY.editor;
                    }

                    $("textarea[data-editor='true']").each(function () {
                        var o = $(this);
                        var style = {};

                        if (o.attr("data-width")) {
                            style.width = o.attr("data-width");
                        }

                        if (o.attr("data-height")) {
                            style.height = o.attr("data-height");
                        }
                        
                        addEditor(tinyeditor, o.attr("id"), style);
                    });
                });
            }
        },
        getAll: function () {
            return _instances;
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.admin.page");

SDNUOJ.admin.page = (function () {
    function showWaiting() {
        var overlay = $("<div>");
        var img = $("<div>").addClass("loading-img");
        var info = $("<p>").addClass("loading-info").html("正在操作，请稍后");
        var tip = $("<div>").addClass("loading-tip").append(img).append(info);

        $(overlay).addClass("loading-overlay").css("display", "none").append(tip).appendTo("body");

        overlay.delay(100).fadeIn("normal", function () {
            $("#container").addClass("loading-container");
        });
    }

    function showError(info) {
        self.location = "/admin/info?c=" + info;
    }

    function checkAndGetValue(name) {
        var value = SDNUOJ.util.dom.getValue(name);
        
        if (value == null) {
            alert("请至少选择一项内容进行操作！");
            return null;
        }

        return value;
    }

    function callPageFunc(pagefunc) {
        if ("undefined" == typeof (pagefunc)) {
            return;
        }

        var initfun = pagefunc.init;

        if ("undefined" == typeof (initfun)) {
            return;
        }

        initfun();
    }

    return {
        is: function (type) {
            var containertype = $("#page-bottominfo").attr("data-type");

            return type == containertype;
        },
        initPage: function () {
            var containertype = $("#page-bottominfo").attr("data-type");
            
            if (containertype == "") {
                return;
            }
            
            var funcs = containertype.split('.');
            var func = SDNUOJ.admin.pages;

            for (var i = 0; i < funcs.length; i++) {
                var name = funcs[i];
                var func = func[name];

                callPageFunc(func);
            }
        },
        showWaiting: showWaiting,
        showError: showError,
        setForm: function () {
            $("form").submit(function (e) {
                var o = $(this);
                var editors = SDNUOJ.admin.editor.getAll();

                for (var i = 0; i < editors.length; i++) {
                    if (editors[i] && editors[i].post) {
                        editors[i].post();
                    }
                }

                if (o.attr("data-verify") == "true" && !SDNUOJ.util.form.verifyForm(o)) {
                    return false;
                }

                if (o.attr("data-confirm") == "true" && !window.confirm("您确定要执行此操作？")) {
                    return false;
                }

                if (o.attr("data-waiting") == "true") {
                    showWaiting();
                }

                return true;
            });
        },
        setPager: function() {
            $("select[data-pager='true']").change(function () {
                var o = $(this);
                var select = o[0];
                var url = o.attr("data-urltemplate").replace("__page__", o.val());
                self.location = url;
            });
        },
        setAdvanceLink: function () {
            $("a[data-advancelink='true']").each(function () {
                var REGEX = /__[a-zA-Z0-9]+__/;

                var o = $(this);

                o.attr("data-url", o.attr("href"))
                    .attr("href", "javascript:void(0);")
                    .click(function (e) {
                        var url = o.attr("data-url");
                        var match = REGEX.exec(url);

                        while (match != null) {
                            var name = match.toString().replace(/__([a-zA-Z0-9]+)__/, "$1");
                            var value = checkAndGetValue(name);
                            if (value == null) {
                                return;
                            }

                            url = url.replace(match, value);
                            match = REGEX.exec(url);
                        }

                        if (o.attr("data-confirm") == "true" && !window.confirm("您确定要执行此操作？")) {
                            return false;
                        }

                        if (o.attr("data-async") == "true") {
                            showWaiting();

                            $.getJSON(url).done(function (data) {
                                if (data && data.status == "success") {
                                    location.reload();
                                }
                                else if (data && data.status == "fail") {
                                    showError(data.result);
                                }
                                else {
                                    showError("The operation failed for an unspecified or unknown reason.");
                                }
                            }).fail(function () {
                                showError("The operation failed.");
                            });
                        }
                        else {
                            self.location = url;
                        }
                    });
            });
        },
        setSwitchLink: function () {
            $("a[data-switch-name][data-switch-value]").each(function () {
                var o = $(this);
                var name = o.attr("data-switch-name");
                var value = o.attr("data-switch-value");
                var container = $("input[name='" + name + "']");

                if (container.length == 0) {
                    return;
                }

                var realvalue = container.val();
                var text = o.html();
                var span = $("<span>").addClass(value == realvalue ? "link-selected" : "link-normal").html(text);

                o.html("");
                o.append(span);

                o.attr("href", "javascript:void(0);").click(function () {
                    container.val(value);
                    o.parents("form").submit();
                });
            });
        },
        setCollapseLink: function () {
            $("a[data-collapse='true']").each(function () {
                var o = $(this);

                o.attr("href", "javascript:void(0);").click(function (e) {
                    var table = o.parents("table");
                    var length = table.find(".collapse-item").length;
                    var status = "unknown";

                    if (o.hasClass("button-expand")) {
                        o.removeClass("button-expand");
                        o.addClass("button-collapse");
                        status = "expanded";
                    }
                    else if (o.hasClass("button-collapse")) {
                        o.removeClass("button-collapse");
                        o.addClass("button-expand");
                        status = "collapsed";
                    }

                    table.find("tr[data-collapsed='true']").each(function (i) {
                        if (status == "expanded") {
                            $(this).removeClass("collapsed");
                        }
                        else if (status == "collapsed") {
                            $(this).addClass("collapsed");
                        }
                    });
                });

            });
        },
        setButtonLink: function() {
            $("input[type='button'][data-navigateurl],button[data-navigateurl]").click(function (e) {
                var o = $(this);

                if (o.attr("data-confirm") == "true") {
                    if (!window.confirm("您确定要执行此操作？")) {
                        return false;
                    }
                }

                self.location = o.attr("data-navigateurl");
            });
        },
        setRadio: function () {
            $("input[type='radio'][data-hidetarget],input[type='radio'][data-showtarget]").click(function (e) {
                var o = $(this);
                
                if (o.attr("data-hidetarget")) {
                    $("#" + o.attr("data-hidetarget")).hide();
                }

                if (o.attr("data-showtarget")) {
                    $("#" + o.attr("data-showtarget")).show();
                }
            });
        },
        setDatetimePicker: function () {
            if ($(".datetimepicker").length > 0) {
                SDNUOJ.util.loader.load("/static/css/foundation/jquery.datetimepicker-2.4.3-mod.min.css");

                $.getScript("/static/js/foundation/jquery.datetimepicker-2.4.3-mod.min.js", function () {
                    $(".datetimepicker").datetimepicker({
                        lang: 'zh',
                        format: 'Y-m-d',
                        allowBlank: true,
                        timepicker: false,
                        scrollMonth: false,
                        scrollInput: false,
                        closeOnDateSelect: true,
                        yearStart: 2012,
                        yearEnd: new Date().getFullYear() + 1
                    });
                });
            }
        },
        setSelect: function () {
            $("select[data-value]").each(function () {
                var o = $(this);
                var value = o.attr("data-value");

                for (var i = 0; i < o[0].options.length; i++) {
                    var option = o[0].options[i];
                    if (option.value == value) {
                        option.selected = true;
                        break;
                    }
                }
            });
        },
        generateSelectItems: function () {
            $("select[data-value-from][data-value-to]").each(function () {
                var o = $(this);
                var from = parseInt(o.attr("data-value-from"));
                var to = parseInt(o.attr("data-value-to"));
                var len = o.attr("data-value-from").length;

                for (var i = from; i <= to; i++) {
                    var s = i.toString();

                    if (s.length < len) {
                        s = new Array(len - s.length + 1).join("0") + s;
                    }

                    o.append($("<option>").html(s));
                }
            });
        },
        disableEnter: function () {
            $("input[data-disableenter='true'],textarea[data-disableenter='true']").keydown(function (e) {
                var et = e || window.event;
                var k = et.keyCode || et.which || et.charCode;

                if (k == 13) {
                    if (window.event) window.event.returnValue = false;
                    else et.preventDefault(); //ff
                }
            });
        }
    }
})();
SDNUOJ.namespace("SDNUOJ.admin.pages.welcome");

SDNUOJ.admin.pages.welcome = (function () {
    function showStatisticInfo(info) {
        $("#statistic-chart").css("height", "auto").addClass("text-center").html(info);
        $("#statistic-legend").empty();
    }
    
    return {
        init: function () {
            var now = new Date();
            var yearselect = $("#admin-year");
            var monthselect = $("#admin-month");

            for (var year = 2012; year <= now.getFullYear(); year++) {
                yearselect.append($("<option>").html(year).val(year));
            }

            yearselect.change(function () {
                var year = $(this).val();
                var lastMonth = 12;

                if (year == now.getFullYear()) {
                    lastMonth = now.getMonth() + 1;
                }

                monthselect.empty();

                for (var month = 1; month <= lastMonth; month++) {
                    monthselect.append($("<option>").html(month).val(month));
                }

                monthselect.trigger("change");
            });

            yearselect.val(now.getFullYear());
            yearselect.trigger("change");
            monthselect.val(now.getMonth() + 1);

            showStatisticInfo("正在加载统计信息...");

            $.getScript("/static/js/foundation/jquery.flot-0.8.3.min.js", function () {
                monthselect.change(function () {
                    showStatisticInfo("正在加载统计信息...");
                    $.getJSON("/admin/welcome/submitstatistic?year=" + $("#admin-year").val() + "&month=" + $("#admin-month").val()).done(function (data) {
                        if (data && data.status == "success") {
                            $("#statistic-chart").css("height", "330px").removeClass("text-center").html("");
                            var statisticData = [
                                {
                                    data: data.result.all,
                                    label: "Submit",
                                    lines: {
                                        show: true,
                                        lineWidth: 3
                                    },
                                    points: {
                                        show: true
                                    },
                                    color: "#b3d7fa"
                                },
                                {
                                    data: data.result.accepted,
                                    label: "Accepted",
                                    bars: {
                                        show: true,
                                        barWidth: 0.5,
                                        align: "center"
                                    },
                                    color: "#80d766"
                                }
                            ];

                            var xticks = [];

                            for (var i = 1; i <= data.result.all.length; i++) {
                                xticks.push(i);
                            }

                            $.plot("#statistic-chart", statisticData, {
                                xaxis: {
                                    font: {
                                        size: 12,
                                        lineHeight: 14,
                                        weight: "normal",
                                        family: "Arial",
                                        color: "#000"
                                    },
                                    color: "#000",
                                    ticks: xticks,
                                    tickColor: "#eaeaea",
                                    tickFormatter: function (d) {
                                        return parseInt(d).toString();
                                    }
                                },
                                yaxis: {
                                    font: {
                                        size: 12,
                                        lineHeight: 14,
                                        weight: "normal",
                                        family: "Arial",
                                        color: "#000"
                                    },
                                    color: "#000",
                                    min: 0,
                                    ticks: 10,
                                    tickColor: "#eaeaea"
                                },
                                legend: {
                                    container: "#statistic-legend",
                                    noColumns: 2
                                },
                                grid: {
                                    hoverable: true,
                                    borderWidth: 0,
                                    margin: 10,
                                    labelMargin: 10
                                }
                            });

                            $("#statistic-chart").bind("plothover", function (event, pos, item) {
                                if (item) {
                                    var tip = data.result.date + "-" + item.datapoint[0] + " " + item.series.label + ": " + item.datapoint[1];
                                    $("#statistic-chart-tooltip").html(tip)
                                        .css({ top: item.pageY + 8, left: item.pageX + 8 })
                                        .fadeIn(200);
                                }
                                else {
                                    $("#statistic-chart-tooltip").hide();
                                }
                            });
                        }
                        else {
                            showStatisticInfo("统计信息加载失败，请重试！");
                        }
                    }).fail(function () {
                        showStatisticInfo("统计信息加载失败，请重试！");
                    });
                }).trigger("change");
            });
        }
    }
})();
SDNUOJ.namespace("SDNUOJ.admin.pages.problemimport");

SDNUOJ.admin.pages.problemimport = (function () {
    return {
        init: function () {
            $("#import-filetype").change(function () {
                var value = $(this).val();

                if (value == "0") {
                    $("#tr-file").hide();
                    $("#tr-content").show();
                }
                else if (value == "1") {
                    $("#tr-content").hide();
                    $("#tr-file").show();
                }
            });

            $("#form-importproblem input[type='reset']").click(function () {
                $("#tr-file").hide();
                $("#tr-content").show();
            });
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.admin.pages.problemcategory");

SDNUOJ.admin.pages.problemcategory = (function () {
    function outputSelectValue() {
        var output = "";

        $("#select-choosed>option").each(function (i) {
            if (i > 0) {
                output += ",";
            }

            output += $(this).val();
        });

        $("#txt-target").val(output);
    }

    return {
        init: function () {
            $("input[type='button'][data-convert-type],button[data-convert-type]").click(function () {
                var type = parseInt($(this).attr("data-convert-type"));//大于0为添加，小于0为删除；1为对单个操作，大于1为对全部操作
                var convertAll = Math.abs(type) > 1;

                var source = $(type > 0 ? "#select-unchoosed" : "#select-choosed");
                var target = $(type > 0 ? "#select-choosed" : "#select-unchoosed");

                source.find("option").each(function (i) {
                    var o = $(this);

                    if (convertAll || o[0].selected) {
                        o[0].selected = false;

                        o.remove();
                        target.append(o.clone());
                    }
                });

                outputSelectValue();
            });
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.admin.pages.problemdata");

SDNUOJ.admin.pages.problemdata = (function () {
    var PROBLEMDATA_MAX = 10;

    var problemDataCount = 0;
    var problemDataTop = 0;

    function addProblemData() {
        if (problemDataCount >= PROBLEMDATA_MAX) {
            alert("一个题目最多只能包含" + PROBLEMDATA_MAX + "个数据！");
            return;
        }

        var newrow = createDataRow();
        $("#table-data").append(newrow);

        problemDataCount++;
    }

    function createDataRow() {
        var id = ++problemDataTop;
        var rowid = "datarow-" + id;
        var leftcolid = "datarow-left-" + id;
        var rightcolid = "datarow-right-" + id;

        var selectbox = $("<select>")
            .append($("<option>").html("从文件上传").val("1"))
            .append($("<option>").html("在线编辑数据").val("2"))
            .append($("<option>").html("删除该组数据").val("3"))
            .attr("data-value", "1")
            .val("1")
            .change(function () {
                var o = $(this);
                var value = o.val();

                var row = $("#" + rowid);
                var right = $("#" + rightcolid);

                if (value == "1") {
                    createFileinput(right, id);
                }
                else if (value == "2") {
                    createTextarea(right, id);
                }
                else if (value == "3") {
                    if (problemDataCount <= 1) {
                        o.val(o.attr("data-value"));
                        alert("一个题目最少需要包含1个数据！");
                        return;
                    }

                    row.remove();
                    return;
                }

                o.attr("data-value", value);
            });

        var newrow = $("<tr>").attr("id", rowid);
        var leftcol = $("<td>").attr("id", leftcolid).append("题目数据&nbsp;").append(selectbox);
        var rightrol = $("<td>").attr("id", rightcolid).css("vertical-align", "top");
        createFileinput(rightrol, id);

        newrow.append(leftcol).append(rightrol);

        return newrow;
    }

    function createFileinput(o, id) {
        var disableKeyDown = function () {
            return false;
        }

        var inbox = $("<input>").attr({
            id: "in" + id,
            name: "in" + id,
            type: "file"
        }).addClass("uploadbox-tiny").keydown(disableKeyDown);

        var outbox = $("<input>").attr({
            id: "out" + id,
            name: "out" + id,
            type: "file"
        }).addClass("uploadbox-tiny").keydown(disableKeyDown);

        o.empty();
        o.append($("<p>").append("输入数据&nbsp;").append(inbox).css("margin", "5px 0 5px 0"))
            .append($("<p>").append("输出数据&nbsp;").append(outbox).css("margin", "5px 0 5px 0"));
    }

    function createTextarea(o, id) {
        var inbox = $("<textarea>").attr({
            id: "in" + id,
            name: "in" + id
        }).addClass("textarea-tiny");

        var outbox = $("<textarea>").attr({
            id: "out" + id,
            name: "out" + id
        }).addClass("textarea-tiny");

        o.empty();
        o.append("输入数据&nbsp;").append(inbox)
            .append("<br/>")
            .append("输出数据&nbsp;").append(outbox);
    }

    function verifyForm() {
        for (var i = 1; i <= problemDataTop; i++) {
            var input = $("#in" + i);
            var output = $("#out" + i);
            
            if (input.is("input[type='file']")) {
                if (input.val() == "") {
                    alert("题目输入数据文件选择不全，请重新选择！");
                    return false;
                }
                
                var ext = SDNUOJ.util.file.getFileExtension(input.val());
                
                if (ext != ".in") {
                    alert("题目输入数据文件类型有误，请重新选择！");
                    return false;
                }
            }

            if (output.is("input[type='file']")) {
                if (output.val() == "") {
                    alert("题目输出数据文件选择不全，请重新选择！");
                    return false;
                }

                var ext = SDNUOJ.util.file.getFileExtension(output.val());

                if (ext != ".out" && ext != ".ans") {
                    alert("题目输出数据文件类型有误，请重新选择！");
                    return false;
                }
            }

            if (input.is("textarea")) {
                if (input.val() == "") {
                    alert("题目输入数据设置不全，请重新输入！");
                    return false;
                }
            }

            if (output.is("textarea")) {
                if (output.val() == "") {
                    alert("题目输出数据设置不全，请重新输入！");
                    return false;
                }
            }
        }

        return true;
    }

    return {
        init: function () {
            $("#btn-adddata").attr("href", "javascript:void(0);").click(function () {
                addProblemData();
            });

            $("#form-createproblemdata").submit(function (e) {
                var o = $(this);
                
                if (!SDNUOJ.util.form.verifyForm(o)) {
                    return false;
                }

                if (!verifyForm()) {
                    return false;
                }

                SDNUOJ.admin.page.showWaiting();
            });

            addProblemData();
        }
    }
})();
(function () {
    if (SDNUOJ.util.browser.ltIE8()) {
        $("#browser-warning").show();
        window.close();
    }

    SDNUOJ.admin.page.setForm();
    SDNUOJ.admin.page.setPager();
    SDNUOJ.admin.page.setAdvanceLink();
    SDNUOJ.admin.page.setSwitchLink();
    SDNUOJ.admin.page.setCollapseLink();
    SDNUOJ.admin.page.setButtonLink();
    SDNUOJ.admin.page.setRadio();
    SDNUOJ.admin.page.setDatetimePicker();
    SDNUOJ.admin.page.generateSelectItems();
    SDNUOJ.admin.page.setSelect();
    SDNUOJ.admin.page.disableEnter();

    SDNUOJ.admin.editor.setAllEditors();

    SDNUOJ.util.list.setCheckboxSelectAll();

    SDNUOJ.admin.page.initPage();
})();
