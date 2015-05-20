SDNUOJ.namespace("SDNUOJ.admin.framework");

SDNUOJ.admin.framework = (function () {
    var user = SDNUOJ.user;

    var MENUS = [
        {
            id: 1,
            title: "常规管理",
            permission: SDNUOJ.PM.ADMINISTRATOR,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "系统综合信息", permission: SDNUOJ.PM.ADMINISTRATOR, url: "welcome" },
                { title: "系统信息检测", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/info" },
                { title: "系统公告管理", permission: SDNUOJ.PM.NEWSMANAGE, url: "news/list" },
                { title: "上传文件管理", permission: SDNUOJ.PM.ADMINISTRATOR, url: "upload/list" },
                { title: "下载资源管理", permission: SDNUOJ.PM.RESOURCEMANAGE, url: "resource/list" },
                { title: "专题页面管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "topicpage/list" },
                { title: "运行参数管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/config" },
                { title: "系统缓存管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "system/cachelist" }
            ]
        },
        {
            id: 2,
            title: "题目管理",
            permission: SDNUOJ.PM.PROBLEMMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "题目信息添加", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/add" },
                { title: "题目信息导入", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "problem/import" },
                { title: "题目综合管理", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/list" },
                { title: "题目分类管理", permission: SDNUOJ.PM.PROBLEMMANAGE, url: "problem/categorylist" }
            ]
        },
        {
            id: 3,
            title: "竞赛管理",
            permission: SDNUOJ.PM.CONTESTMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "竞赛信息添加", permission: SDNUOJ.PM.CONTESTMANAGE, url: "contest/add" },
                { title: "竞赛综合管理", permission: SDNUOJ.PM.CONTESTMANAGE, url: "contest/list" }
            ]
        },
        {
            id: 4,
            title: "用户管理",
            permission: SDNUOJ.PM.SUPERADMINISTRATOR,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "在线用户查看", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/online" },
                { title: "用户密码修改", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/changepassword" },
                { title: "用户信息管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/list" },
                { title: "用户权限管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "user/permissionlist" }
            ]
        },
        {
            id: 5,
            title: "论坛管理",
            permission: SDNUOJ.PM.FORUMMANAGE,
            accessOnly: false,
            defaultSubID: 0,
            allmenus: [
                { title: "论坛主题管理", permission: SDNUOJ.PM.FORUMMANAGE, url: "forum/topiclist" },
                { title: "论坛帖子管理", permission: SDNUOJ.PM.FORUMMANAGE, url: "forum/postlist" }
            ]
        },
        {
            id: 6,
            title: "评测管理",
            permission: SDNUOJ.PM.SOLUTIONMANAGE,
            accessOnly: false,
            defaultSubID: (user.hasPermission(SDNUOJ.PM.SUPERADMINISTRATOR) ? 0 : 2),
            allmenus: [
                { title: "评测机管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "judger/list" },
                { title: "提交评测管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "solution/list" },
                { title: "重新评测提交", permission: SDNUOJ.PM.SOLUTIONMANAGE, url: "solution/rejudge" }
            ]
        },
        {
            id: 7,
            title: "数据库管理",
            permission: SDNUOJ.PM.SUPERADMINISTRATOR,
            accessOnly: true,
            defaultSubID: 0,
            allmenus: [
                { title: "系统数据库压缩", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/compact" },
                { title: "系统数据库备份", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/backup" },
                { title: "系统数据库还原", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/restore" },
                { title: "系统数据库上传", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/upload" },
                { title: "系统数据库管理", permission: SDNUOJ.PM.SUPERADMINISTRATOR, url: "database/list" }
            ]
        }
    ];

    function createMenu(o) {
        if (!user.hasPermission(o.permission)) {
            return false;
        }

        if (o.accessOnly && $("#menus").attr("data-isaccessdb").toLowerCase() != "true") {
            return false;
        }
        
        var link = $("<a>").attr("href", "javascript:void(0);").attr("data-menuid", o.id)
            .addClass(o.id == 1 ? "menu-open" : "menu-close")
            .append($("<span>").html(o.title))
            .click(function (e) {
                $("#menus a.menu-open").removeClass("menu-open").addClass("menu-close");
                $(this).removeClass("menu-close").addClass("menu-open");
                showSubMenus($(this).attr("data-menuid"));
            });

        return link;
    }

    function createSubMenu(o, selected) {
        if (!user.hasPermission(o.permission)) {
            return false;
        }

        var link = $("<a>").attr("href", "/admin/" + o.url).attr("target", "admin-page").html(o.title)
            .addClass(selected ? "submenu-open" : "submenu-close")
            .click(function (e) {
                $("#submenus a.submenu-open").removeClass("submenu-open").addClass("submenu-close");
                $(this).removeClass("submenu-close").addClass("submenu-open");
            });

        return link;
    }

    function showMenus() {
        for (var i = 0; i < MENUS.length; i++) {
            var link = createMenu(MENUS[i]);
            
            if (link) {
                $("#menus").append(link);
            }
        }
    }

    function showSubMenus(id) {
        $("#submenus").html("");

        if (!user.hasAdministratorPermission()) {
            return false;
        }

        var submenus = null;

        for (var i = 0; i < MENUS.length; i++) {
            if (MENUS[i].id == id) {
                submenus = MENUS[i];
                break;
            }
        }

        if (submenus == null) {
            return false;
        }

        for (var i = 0; i < submenus.allmenus.length; i++) {
            var link = createSubMenu(submenus.allmenus[i], (i == submenus.defaultSubID));

            if (link) {
                $("#submenus").append(link);
            }
        }

        var defaultSubID = submenus.defaultSubID;
        window.parent.frames["admin-page"].document.location.href = "/admin/" + submenus.allmenus[defaultSubID].url;
    }

    return {
        showMenus: showMenus,
        showSubMenus: showSubMenus
    };
})();

(function () {
    SDNUOJ.admin.framework.showMenus();
    SDNUOJ.admin.framework.showSubMenus(1);

    if (SDNUOJ.util.browser.ltIE8()) {
        alert("您的浏览器版本小于Internet Explorer 8，部分功能将无法使用，请更新您的浏览器或使用其他浏览器！");
        window.close();
    }
})();