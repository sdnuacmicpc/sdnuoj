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

SDNUOJ.namespace("SDNUOJ.status");
SDNUOJ.status = (function () {
    if (typeof queryStr != "undefined" && queryStr.length > 0) {
        setTimeout("SDNUOJ.status.checkPendingStatus()", 1000);
    }

    function updateStatus(data) {
        clearQueryStr();

        for (var i in data) {
            var item = data[i];

            if (item.Result == 0 || item.Result == 1) { //Pending
                addToQueryStr(item.SolutionID);
                continue;

            } else if (item.Result == 2) { //Judging
                addToQueryStr(item.SolutionID);
            }

            updateItemStatus(item);
        }

        if (queryStr.length > 0) {
            setTimeout("SDNUOJ.status.checkPendingStatus()", 1000);
        }
    }

    function updateItemStatus(item) {
        $("#time_" + item.SolutionID).text(item.TimeCost + "MS");
        $("#mem_" + item.SolutionID).text(item.MemoryCost + "KB");

        var reg = /\s/g;
        $("#res_" + item.SolutionID).attr("class", "label status_" + item.ResultString.replace(reg, ""));

        if (item.Result == 3 || item.Result == 4) { //Compile Error or Runtime Error
            item.ResultString = "<a href=\"/solution/info/" + item.SolutionID + "\">" + item.ResultString + "</a>";
        }
        else if (item.Result == 2) { //Judging
            item.ResultString += "<img src=\"/static/img/working.gif\" width=\"10px\" height=\"10px\">";
        }

        $("#res_" + item.SolutionID).html(item.ResultString);
    }

    function clearQueryStr() {
        queryStr = "";
    }

    function addToQueryStr(sid) {
        if (queryStr.length > 0) {
            queryStr += ",";
        }
        queryStr += sid;
    }

    return {
        checkPendingStatus: function () {
            $.get("/status/querystatus?sids=" + queryStr).done(function (data) {
                var jsonData = eval("(" + data + ")");
                updateStatus(jsonData);
            });
        }
    }
})();

/*! http://mths.be/placeholder v2.0.8 by @mathias */
;(function(window, document, $) {

	// Opera Mini v7 doesn’t support placeholder although its DOM seems to indicate so
	var isOperaMini = Object.prototype.toString.call(window.operamini) == '[object OperaMini]';
	var isInputSupported = 'placeholder' in document.createElement('input') && !isOperaMini;
	var isTextareaSupported = 'placeholder' in document.createElement('textarea') && !isOperaMini;
	var prototype = $.fn;
	var valHooks = $.valHooks;
	var propHooks = $.propHooks;
	var hooks;
	var placeholder;

	if (isInputSupported && isTextareaSupported) {

		placeholder = prototype.placeholder = function() {
			return this;
		};

		placeholder.input = placeholder.textarea = true;

	} else {

		placeholder = prototype.placeholder = function() {
			var $this = this;
			$this
				.filter((isInputSupported ? 'textarea' : ':input') + '[placeholder]')
				.not('.placeholder')
				.bind({
					'focus.placeholder': clearPlaceholder,
					'blur.placeholder': setPlaceholder
				})
				.data('placeholder-enabled', true)
				.trigger('blur.placeholder');
			return $this;
		};

		placeholder.input = isInputSupported;
		placeholder.textarea = isTextareaSupported;

		hooks = {
			'get': function(element) {
				var $element = $(element);

				var $passwordInput = $element.data('placeholder-password');
				if ($passwordInput) {
					return $passwordInput[0].value;
				}

				return $element.data('placeholder-enabled') && $element.hasClass('placeholder') ? '' : element.value;
			},
			'set': function(element, value) {
				var $element = $(element);

				var $passwordInput = $element.data('placeholder-password');
				if ($passwordInput) {
					return $passwordInput[0].value = value;
				}

				if (!$element.data('placeholder-enabled')) {
					return element.value = value;
				}
				if (value == '') {
					element.value = value;
					// Issue #56: Setting the placeholder causes problems if the element continues to have focus.
					if (element != safeActiveElement()) {
						// We can't use `triggerHandler` here because of dummy text/password inputs :(
						setPlaceholder.call(element);
					}
				} else if ($element.hasClass('placeholder')) {
					clearPlaceholder.call(element, true, value) || (element.value = value);
				} else {
					element.value = value;
				}
				// `set` can not return `undefined`; see http://jsapi.info/jquery/1.7.1/val#L2363
				return $element;
			}
		};

		if (!isInputSupported) {
			valHooks.input = hooks;
			propHooks.value = hooks;
		}
		if (!isTextareaSupported) {
			valHooks.textarea = hooks;
			propHooks.value = hooks;
		}

		$(function() {
			// Look for forms
			$(document).delegate('form', 'submit.placeholder', function() {
				// Clear the placeholder values so they don't get submitted
				var $inputs = $('.placeholder', this).each(clearPlaceholder);
				setTimeout(function() {
					$inputs.each(setPlaceholder);
				}, 10);
			});
		});

		// Clear placeholder values upon page reload
		$(window).bind('beforeunload.placeholder', function() {
			$('.placeholder').each(function() {
				this.value = '';
			});
		});

	}

	function args(elem) {
		// Return an object of element attributes
		var newAttrs = {};
		var rinlinejQuery = /^jQuery\d+$/;
		$.each(elem.attributes, function(i, attr) {
			if (attr.specified && !rinlinejQuery.test(attr.name)) {
				newAttrs[attr.name] = attr.value;
			}
		});
		return newAttrs;
	}

	function clearPlaceholder(event, value) {
		var input = this;
		var $input = $(input);
		if (input.value == $input.attr('placeholder') && $input.hasClass('placeholder')) {
			if ($input.data('placeholder-password')) {
				$input = $input.hide().next().show().attr('id', $input.removeAttr('id').data('placeholder-id'));
				// If `clearPlaceholder` was called from `$.valHooks.input.set`
				if (event === true) {
					return $input[0].value = value;
				}
				$input.focus();
			} else {
				input.value = '';
				$input.removeClass('placeholder');
				input == safeActiveElement() && input.select();
			}
		}
	}

	function setPlaceholder() {
		var $replacement;
		var input = this;
		var $input = $(input);
		var id = this.id;
		if (input.value == '') {
			if (input.type == 'password') {
				if (!$input.data('placeholder-textinput')) {
					try {
						$replacement = $input.clone().attr({ 'type': 'text' });
					} catch(e) {
						$replacement = $('<input>').attr($.extend(args(this), { 'type': 'text' }));
					}
					$replacement
						.removeAttr('name')
						.data({
							'placeholder-password': $input,
							'placeholder-id': id
						})
						.bind('focus.placeholder', clearPlaceholder);
					$input
						.data({
							'placeholder-textinput': $replacement,
							'placeholder-id': id
						})
						.before($replacement);
				}
				$input = $input.removeAttr('id').hide().prev().attr('id', id).show();
				// Note: `$input[0] != input` now!
			}
			$input.addClass('placeholder');
			$input[0].value = $input.attr('placeholder');
		} else {
			$input.removeClass('placeholder');
		}
	}

	function safeActiveElement() {
		// Avoid IE9 `document.activeElement` of death
		// https://github.com/mathiasbynens/jquery-placeholder/pull/99
		try {
			return document.activeElement;
		} catch (exception) {}
	}

}(this, document, jQuery));
SDNUOJ.namespace("SDNUOJ.page.foreground");

SDNUOJ.page.foreground = (function () {
    var user = SDNUOJ.user;

    function getPagePath() {
        var urlroot = window.location.protocol + "//" + window.location.host;

        if (!window.location.host.indexOf(":") && window.location.port != 80) {
            urlroot += ":" + window.location.port;
        }

        if (urlroot.charAt(urlroot.length - 1) != '/') {
            urlroot += "/";
        }

        var path = window.location.href.replace(urlroot, "");

        if (path.indexOf("?") > -1) {
            path = path.substring(0, path.indexOf("?"));
        }
        
        return path;
    }

    function isContestPage() {
        return $("#contest-submenus").length > 0;
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

    var pagePath = getPagePath();
    
    //设置导航选中状态
    if (!isContestPage()) {
        $("#navbar > .navbar-nav li a[href$='" + pagePath + "']").parent().addClass("active");
        $("#navbar > .navbar-nav > .dropdown > .dropdown-menu li a[href$='" + pagePath + "']").parent().parent().parent().addClass("active");
    }

    //设置用户登录状态
    if (user.getIsLogined()) {
        var userName = user.getCurrentUserName();
        var userUnReadMail = user.getUnreadMail();

        if (userUnReadMail > 0) {
            $("#btn-user").html(userName + (userUnReadMail > 0 ? "&nbsp;<span class=\"badge badge-danger\">" + userUnReadMail + "</span>" : ""))
                .click(function () { self.location = "/mail/box"; });

            $("#user-mail").html("Mail Box" + (userUnReadMail > 0 ? "&nbsp;<span class=\"badge badge-danger\">" + userUnReadMail + "</span>" : ""));
        }
        else {
            $("#btn-user").click(function () { self.location = "/user/info/" + userName; });
        }

        if (user.hasAdministratorPermission()) {
            var btnadmin = $("<li>").append($("<a>")
                .attr({
                    href: "/admin",
                    target: "_blank"
                })
                .html("Administration"));
            var divider = $("<li>").addClass("divider");
            
            btnadmin.insertBefore($("#user-logout").parent());
            divider.insertBefore($("#user-logout").parent());
        }
    }

    return {
        is: function (type) {
            var containertype = $("#main-container").attr("data-type");

            return type == containertype;
        },
        initPage: function () {
            var containertype = $("#main-container").attr("data-type");
            
            if (containertype == "") {
                return;
            }
            
            var funcs = containertype.split('.');
            var func = SDNUOJ.pages;

            for (var i = 0; i < funcs.length; i++) {
                var name = funcs[i];
                var func = func[name];

                callPageFunc(func);
            }
        },
        getPagePath: getPagePath,
        getIsContestPage: isContestPage
    }
})();

SDNUOJ.namespace("SDNUOJ.page.highlight");

SDNUOJ.page.highlight = (function () {
    var LANG_MODE_MAP = [
        "text/x-csrc",//0,C
        "text/x-c++src",//1,C++
        "text/x-java"//2,Java
        //"text/x-csharp",//C#
        //"text/x-vb",//VB.NET
        //"text/x-pascal",//Pascal
        //"text/x-python",//Python
        //"text/x-ruby",//Ruby
        //"text/x-objectivec",//Object-C
        //"text/x-scala",//Scala
    ];

    var _instances = new Array();
    
    function getEditorMode(langID) {
        return LANG_MODE_MAP[langID];
    }

    function addHighlightor(codemirror, textarea, language, readonly, simple) {
        if ("undefined" == typeof (codemirror)) {
            return;
        }

        if (!codemirror) {
            return;
        }

        config = {
            lineNumbers: true,
            styleActiveLine: !simple,
            matchBrackets: !simple,
            autofocus: !simple && !readonly,
            indentUnit: 4,
            mode: getEditorMode(language),
            extraKeys: {
                "F11": function (cm) {
                    cm.setOption("fullScreen", !cm.getOption("fullScreen"));

                    if (cm.getOption("fullScreen")) {
                        $("nav").hide();
                    }
                    else {
                        $("nav").show();
                    }
                },
                "Esc": function (cm) {
                    if (cm.getOption("fullScreen")) cm.setOption("fullScreen", false);

                    if (!cm.getOption("fullScreen")) {
                        $("nav").show();
                    }
                }
            }
        };

        if (readonly) {
            config.viewportMargin = Infinity;
            config.readOnly = true;
            config.cursorBlinkRate = -1;
        }

        var highlightor = codemirror.fromTextArea(textarea[0], config);

        _instances.push(highlightor);
    }

    return {
        addHighlightor: addHighlightor,
        setAllHighlightEditors: function () {
            if ($("textarea[data-codeeditor='true']").length > 0) {
                var user = SDNUOJ.user;
                var lastlang = user.getLastLanguage();

                $("textarea[data-codeeditor='true']").each(function () {
                    var o = $(this);
                    
                    addHighlightor(CodeMirror, o, lastlang, false, false);
                });
            }
        },
        setAllHighlightViewers: function() {
            if ($("textarea[data-codeview='true']").length > 0) {
                $("textarea[data-codeview='true']").each(function () {
                    var o = $(this);
                    var lang = o.attr("data-language");
                    var simple = o.attr("data-simpleview") == "true";

                    addHighlightor(CodeMirror, o, lang, true, simple);
                });
            }
        },
        getAll: function () {
            return _instances;
        },
        clearAllText: function () {
            for (var i = 0; i < _instances.length; i++) {
                _instances[i].setValue("");
            }
        },
        setAllLanguage: function (langID) {
            for (var i = 0; i < _instances.length; i++) {
                _instances[i].setOption("mode", getEditorMode(langID));
            }
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.page.tree");

SDNUOJ.page.tree = (function () {
    function collapseList(o) {
        if (o.hasClass("tree-item-expand")) {
            o.removeClass("tree-item-expand");
            o.addClass("tree-item-collapse");
            $("#" + o.attr("data-collapse-target")).show();
        }
        else if (o.hasClass("tree-item-collapse")) {
            o.removeClass("tree-item-collapse");
            o.addClass("tree-item-expand");
            $("#" + o.attr("data-collapse-target")).hide();
        }
    }

    return {
        collapseList: collapseList,
        setImageCollapse: function () {
            $("a[data-collapse='true']").attr("href", "javascript:void(0);").click(function (e) {
                return collapseList($(this));
            });
        }
    }
})();
SDNUOJ.namespace("SDNUOJ.page.foreground.contest");

SDNUOJ.page.foreground.contest = (function () {
    var user = SDNUOJ.user;

    return {
        initPage: function () {
            if ($("#contest-submenus").length > 0) {
                if (user.hasContestManagePermission()) {
                    var cid = $("#contest-submenus").attr("data-contestid");
                    var li = $("<li>").append($("<a>").attr("href", "/contest/" + cid + "/overall/statistics").html("Statistics"));
                    $("#contest-submenus").append(li);
                }

                var pagePath = SDNUOJ.page.foreground.getPagePath();
                $("#contest-submenus li a[href$='" + pagePath + "']").parent().addClass("active");
            }
        }
    }
})();

SDNUOJ.namespace("SDNUOJ.pages.contest.index");

SDNUOJ.pages.contest.index = (function () {
    function clockTick() {
        var now = new Date(new Date().getTime() + TIME_DIFF);
        $("#server-time").html(SDNUOJ.util.date.format(now, "yyyy-MM-dd HH:mm:ss"));
        setTimeout("SDNUOJ.pages.contest.index.serverClockTick()", 1000);
    }

    return {
        init: function () {
            if ($("#server-time").length > 0) {
                clockTick();
            }
        },
        serverClockTick: clockTick
    }
})();

SDNUOJ.namespace("SDNUOJ.pages.contest.statistics");

SDNUOJ.pages.contest.statistics = (function () {
    function calcStatisticsTotal() {
        var table = $("#table-contest-statistics")[0];
        var cid = $("#contest-submenus").attr("data-contestid");
        var titleRow = 0;//标题行索引
        var firstRow = titleRow + 1;//首行索引

        var emptyContent = (table.rows[firstRow].textContent ? table.rows[firstRow].textContent : table.rows[firstRow].innerText);

        if (table.rows.length <= 2 && emptyContent.indexOf("No") > -1) {
            return;
        }

        var tr = table.insertRow(table.rows.length);
        var tdTotal = tr.insertCell(0);
        tdTotal.style.textAlign = "center"
        tdTotal.innerHTML = '<strong><a href="/contest/' + cid + '/status/list">Total</a></strong>';

        for (var i = 1; i < table.rows[firstRow].cells.length; i++) {
            var td = tr.insertCell(i);
            var count = 0;
            td.align = "center"

            for (var j = firstRow; j < table.rows.length - 1; j++) {
                var cell = table.rows[j].cells[i];
                var num = (cell.textContent ? cell.textContent : cell.innerText);
                count += parseInt(num);
            }

            var cellInTitleRow = table.rows[titleRow].cells[i];
            var cellInFirstRow = table.rows[firstRow].cells[i];
            var className = $(cellInFirstRow).find("span").attr("class");

            td.innerHTML = '<a href=\"/contest/' + cid + "/status/list" + (cellInTitleRow.id != "" ? '?' + cellInTitleRow.id : "") + '">'
                + '<span' + (className != null ? ' class="' + className + '"' : '') + '>' + count + '</span></a>';
            td.style.textAlign = "center";
        }
    }

    return {
        init: function () {
            if ($("#table-contest-statistics").length > 0) {
                calcStatisticsTotal();
            }
        }
    }
})();
SDNUOJ.namespace("SDNUOJ.pages.problems");

SDNUOJ.pages.problems = (function () {
    var user = SDNUOJ.user;

    function setProblemStatus(pid, status) {
        var td = $("#S" + pid);

        if (td.length > 0) {
            td.addClass(status == 2 ? "problem-ac" : (status == 1 ? "problem-wa" : "problem-normal"));
            td.html(status == 2 ? "Y" : (status == 1 ? "N" : ""));
        }
    }

    return {
        init: function () {
            if (user.getIsLogined() && $("#SOpen").length > 0) {
                user.getSubmitList(function (data) {
                    var solved = data.solved;
                    var unsolved = data.unsolved;

                    if (unsolved) {
                        for (var i = 0; i < unsolved.length; i++) {
                            setProblemStatus(unsolved[i], 1);
                        }
                    }

                    if (solved) {
                        for (var i = 0; i < solved.length; i++) {
                            setProblemStatus(solved[i], 2);
                        }
                    }
                });
            }
        }
    }
})();
SDNUOJ.namespace("SDNUOJ.pages.recent");

SDNUOJ.pages.recent = (function () {
    return {
        init: function () {
            if ($("#table-recent").length > 0) {
                $.getJSON("/contests/recentinfo").done(function (data) {
                    $("#tr-recent-info").remove();

                    for (var i = data.length - 1; i >= 0; i--) {
                        if (data[i].access == "") {
                            data[i].access = "Public";
                        }

                        var cid = $("<td>").addClass("text-center").html(i + 1000);
                        var cname = $("<td>").html("<a href=\"" + data[i].link + "\" target=\"_blank\">" + data[i].name + "</a>");
                        var start = $("<td>").addClass("text-center").html(data[i].start_time);
                        var type = $("<td>").addClass("text-center").html("<span class=\"label contest_" + data[i].access + "\">" + data[i].access + "</span>");
                        var oj = $("<td>").addClass("text-center").html(data[i].oj);
                        var line = $("<tr>").append(cid).append(cname).append(start).append(type).append(oj);

                        $("#table-recent").append(line);
                    }
                }).fail(function () {
                    $("td-recent-info").html("No such contest");
                });
            }
        }
    }
})();
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

SDNUOJ.pages.problemstatistic = (function () {
    return {
        init: function () {
            SDNUOJ.pages.status.init();

            function isNoSubmit() {
                if (totalCount < 1) {
                    return true;
                }

                for (var i = 0; i < statisticData.length; i++) {
                    if (statisticData[i].data > 0) {
                        return false;
                    }
                }

                return true;
            }

            if (!isNoSubmit()) {
                $.getScript("/static/js/foundation/jquery.flot-0.8.3.min.js", function () {
                    $.getScript("/static/js/foundation/jquery.flot.pie-0.8.3.min.js", function () {
                        $.plot("#statistic-chart", statisticData, {
                            series: {
                                pie: {
                                    innerRadius: 0.6,
                                    show: true,
                                    label: {
                                        show: false
                                    }
                                }
                            },
                            legend: {
                                show: false
                            },
                            grid: {
                                hoverable: true
                            }
                        });

                        $('<span id="statistic-chart-tipcontainer"></span>')
                            .addClass("pieLabel").addClass("statistic-chart-tipcontainer")
                            .appendTo($("#statistic-chart"));

                        $("#statistic-chart").bind("plothover", function (event, pos, item) {
                            if (item) {
                                var percent = (totalCount > 0 ? item.datapoint[1][0][1] / totalCount * 100 : 0);
                                var tip = $("<div></div>").addClass("statistic-chart-tip")
                                    .css("background-color", item.series.color)
                                    .html(item.series.label + "<br/>" + percent.toFixed(2) + "%");
                                var top = 100 - 24 - 5;
                                var left = 125 - (percent >= 100 ? 28 : percent >= 10 ? 25 : 22) - 6;
                                $("#statistic-chart-tipcontainer").css({
                                    position: "absolute",
                                    top: top * $("#statistic-chart").height() / 200,
                                    left: left * $("#statistic-chart").width() / 250
                                }).empty().append(tip).fadeIn(200);
                            }
                            else {
                                $("#statistic-chart-tipcontainer").hide().empty();
                            }
                        });
                    });
                });
            } else {
                $("#statistic-chart").hide();
            }
        }
    };
})();
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

            $("button[data-loading-text]").click(function (e) {
                $(this).button("loading");
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

            $("button[data-loading-text]").click(function (e) {
                $(this).button("loading");
            });
        }
    };
})();
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
