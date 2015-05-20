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