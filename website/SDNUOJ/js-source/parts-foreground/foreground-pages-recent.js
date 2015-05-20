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