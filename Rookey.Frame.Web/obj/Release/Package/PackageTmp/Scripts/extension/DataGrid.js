
//网格对象
//gridDom:网格dom对象
function DataGrid(gridDom) {
    //私有函数 网格参数
    var dataGridParams = function () {
        this.dom = null;
        //属性定义
        this.width = null; //宽
        this.height = null; //高
        this.columns = null; //非冻结列
        this.frozenColumns = null; //冻结列
        this.fitColumns = false; //列自适应宽度
        this.resizeHandle = 'right'; //调整列的位置，可用的值有：'left','right','both'。在使用'right'的时候用户可以通过拖动右侧边缘的列标题调整列
        this.autoRowHeight = true; //定义设置行的高度，根据该行的内容。设置为false可以提高负载性能。
        this.toolbar = null; //网格工具栏
        this.striped = true; //是否显示斑马线效果。
        this.method = 'post'; //该方法类型请求远程数据
        this.nowrap = true; //如果为true，则在同一行中显示数据。设置为true可以提高加载性能。
        this.idField = "Id"; //指明哪一个字段是标识字段。
        this.url = null; //一个URL从远程站点请求数据。
        this.data = null; //
        this.loadMsg = '正在加载数据...'; //在从远程站点加载数据的时候显示提示消息。
        this.pagination = true; //如果为true，则在DataGrid控件底部显示分页工具栏。
        this.rownumbers = true; //如果为true，则显示一个行号列。
        this.singleSelect = true; //如果为true，则只允许选择一行。
        this.ctrlSelect = false; //在启用多行选择的时候允许使用Ctrl键+鼠标点击的方式进行多选操作
        this.checkOnSelect = true; //如果为true，当用户点击行的时候该复选框就会被选中或取消选中。如果为false，当用户仅在点击该复选框的时候才会呗选中或取消。
        this.selectOnCheck = true; //如果为true，单击复选框将永远选择行。如果为false，选择行将不选中复选框。
        this.pagePosition = 'bottom'; //定义分页工具栏的位置。可用的值有：'top','bottom','both'。
        this.pageNumber = 1; //在设置分页属性的时候初始化页码。
        this.pageSize = 15; //在设置分页属性的时候初始化页面大小。
        this.pageList = [15, 30, 50, 100]; //在设置分页属性的时候 初始化页面大小选择列表
        this.queryParams = null; //在请求远程数据的时候发送额外的参数。 
        this.sortName = "Id"; //定义哪些列可以进行排序。
        this.sortOrder = "desc"; //定义列的排序顺序，只能是'asc'或'desc'。
        this.multiSort = false; //定义是否允许多列排序
        this.remoteSort = true; //定义从服务器对数据进行排序。
        this.showHeader = true; //定义是否显示行头。
        this.showFooter = false; //定义是否显示行脚。
        this.scrollbarSize = 18; //滚动条的宽度(当滚动条是垂直的时候)或高度(当滚动条是水平的时候)。
        //行样式处理
        this.rowStyler = null;
        //返回过滤数据显示。该函数带一个参数'data'用来指向源数据（即：获取的数据源，比如Json对象）。您可以改变源数据的标准数据格式。这个函数必须返回包含'total'和'rows'属性的标准数据对象。
        this.loadFilter = null;
        this.editors = null; //定义在编辑行的时候使用的编辑器。
        //事件定义
        //在数据加载成功的时候触发。
        this.onLoadSuccess = null;
        //在载入远程数据产生错误的时候触发。
        this.onLoadError = null;
        //在载入请求数据数据之前触发，如果返回false可终止载入数据操作
        this.onBeforeLoad = null;
        //用户单击一行的时候触发，参数rowIndex：点击的行的索引值，该索引值从0开始。rowData：对应于点击行的记录。
        this.onClickRow = null;
        //在用户双击一行的时候触发，参数包括：rowIndex：点击的行的索引值，该索引值从0开始。rowData：对应于点击行的记录。
        this.onDblClickRow = null;
        //在用户点击一个单元格的时候触发。
        this.onClickCell = null;
        //在用户双击一个单元格的时候触发。 
        this.onDblClickCell = null;
        //在用户排序一个列之前触发，返回false可以取消排序
        this.onBeforeSortColumn = null;
        //在用户排序一列的时候触发，参数包括：sort：排序列字段名称。order：排序列的顺序(ASC或DESC)
        this.onSortColumn = null;
        //在用户调整列大小的时候触发。
        this.onResizeColumn = null;
        //在用户选择一行的时候触发，参数包括：
        //rowIndex：选择的行的索引值，索引从0开始。
        //rowData：对应于所选行的记录。
        this.onSelect = null;
        //在用户取消选择一行的时候触发
        this.onUnselect = null;
        //在用户选择所有行的时候触发。
        this.onSelectAll = null;
        //在用户取消选择所有行的时候触发。
        this.onUnselectAll = null;
        //在用户勾选一行的时候触发
        this.onCheck = null;
        //在用户取消勾选一行的时候触发
        this.onUncheck = null;
        //在用户勾选所有行的时候触发
        this.onCheckAll = null;
        //在用户取消勾选所有行的时候触发
        this.onUncheckAll = null;
        //在用户开始编辑一行的时候触发
        this.onBeforeEdit = null;
        //在一行进入编辑模式的时候触发
        this.onBeginEdit = null;
        //在完成编辑但编辑器还没有销毁之前触发
        this.onEndEdit = null;
        //在用户完成编辑一行的时候触发
        this.onAfterEdit = null;
        //在用户取消编辑一行的时候触发
        this.onCancelEdit = null;
        //在鼠标右击DataGrid表格头的时候触发
        this.onHeaderContextMenu = null;
        //在鼠标右击一行记录的时候触发。
        this.onRowContextMenu = null;
    }
    //当前网格对象
    var gridObj = this;
    //网格参数对象
    var gridParams = new dataGridParams();
    gridParams.dom = gridDom;
    //获取网格参数
    gridObj.GetDataGridParams = function () {
        return gridParams;
    };
    //获取网格dom对象
    gridObj.GetGridDom = function () {
        return gridParams.dom;
    };
    //初始化列表
    gridObj.Init = function () {
        gridDom.datagrid(gridParams);
    };
}