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