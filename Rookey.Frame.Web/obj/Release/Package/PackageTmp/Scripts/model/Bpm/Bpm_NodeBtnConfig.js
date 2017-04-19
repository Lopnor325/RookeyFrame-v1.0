
//分组格式化重写
function OverGroupFormatter(value, rows, groupField) {
    if (groupField == 'Bpm_WorkNodeId') {
        return rows[0]['Bpm_WorkFlowName'] + '－' + rows[0]['Bpm_WorkNodeName'] + '(' + rows.length + ')';
    }
    return value + '(' + rows.length + ')';
}