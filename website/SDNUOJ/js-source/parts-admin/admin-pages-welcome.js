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