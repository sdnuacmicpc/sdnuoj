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