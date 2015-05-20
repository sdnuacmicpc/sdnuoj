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