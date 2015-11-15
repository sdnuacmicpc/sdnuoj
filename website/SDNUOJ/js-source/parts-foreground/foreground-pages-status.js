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