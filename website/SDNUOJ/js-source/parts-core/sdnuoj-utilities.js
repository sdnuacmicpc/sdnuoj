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