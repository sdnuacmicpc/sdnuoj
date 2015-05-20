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