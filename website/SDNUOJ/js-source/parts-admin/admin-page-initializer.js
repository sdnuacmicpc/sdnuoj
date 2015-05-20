(function () {
    if (SDNUOJ.util.browser.ltIE8()) {
        $("#browser-warning").show();
        window.close();
    }

    SDNUOJ.admin.page.setForm();
    SDNUOJ.admin.page.setPager();
    SDNUOJ.admin.page.setAdvanceLink();
    SDNUOJ.admin.page.setSwitchLink();
    SDNUOJ.admin.page.setCollapseLink();
    SDNUOJ.admin.page.setButtonLink();
    SDNUOJ.admin.page.setRadio();
    SDNUOJ.admin.page.setDatetimePicker();
    SDNUOJ.admin.page.generateSelectItems();
    SDNUOJ.admin.page.setSelect();
    SDNUOJ.admin.page.disableEnter();

    SDNUOJ.admin.editor.setAllEditors();

    SDNUOJ.util.list.setCheckboxSelectAll();

    SDNUOJ.admin.page.initPage();
})();