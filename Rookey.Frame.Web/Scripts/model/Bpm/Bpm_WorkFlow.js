$(function () {
    if (page == 'add') {
        var classId = GetLocalQueryString("flowClassId");
        if (classId) {
            $('#Bpm_FlowClassId').combotree('setValue', classId);
        }
    }
});