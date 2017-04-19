$(function () {
    AutoMergeAppInfoCell();
});

//自动合并审批信息单元格
function AutoMergeAppInfoCell() {
    $.getScript('/Scripts/easyui-extension/datagrid-autoMergeCells.js', function () {
        var gridObj = $('#tb_approvalList');
        gridObj.datagrid("autoMergeCells", ['NodeName']);
        $('#div_approvalListTitle td').css('vertical-align', 'middle');
    });
}
