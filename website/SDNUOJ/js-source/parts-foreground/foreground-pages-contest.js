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