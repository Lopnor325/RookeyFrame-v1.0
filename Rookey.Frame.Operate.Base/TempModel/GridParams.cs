using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 网格参数类
    /// </summary>
    public class GridParams
    {
        private string dataOrUrl = string.Empty;
        /// <summary>
        /// 数据或URL
        /// </summary>
        public string DataOrUrl 
        {
            get { return dataOrUrl; }
            set { dataOrUrl = value; }
        }

        private bool isTreeGrid = false;
        /// <summary>
        /// 是否树网格
        /// </summary>
        public bool IsTreeGrid
        {
            get { return isTreeGrid; }
            set { isTreeGrid = value; }
        }

        private string treeField;
        /// <summary>
        /// 树网格时的树字段
        /// </summary>
        public string TreeField
        {
            get { return treeField; }
            set { treeField = value; }
        }

        private bool isPaging = true;
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsPaging
        {
            get { return isPaging; }
            set { isPaging = value; }
        }

        private int pageSize = 15;
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        private string pageList = "[15,30,50,100]";
        /// <summary>
        /// 分页列表
        /// </summary>
        public string PageList
        {
            get { return pageList; }
            set { pageList = value; }
        }

        private bool isMutiSelect = false;
        /// <summary>
        /// 是否多选
        /// </summary>
        public bool IsMutiSelect
        {
            get { return isMutiSelect; }
            set { isMutiSelect = value; }
        }

        private List<Sys_GridField> gridFields;
        /// <summary>
        /// 网格字段
        /// </summary>
        public List<Sys_GridField> GridFields
        {
            get { return gridFields; }
            set { gridFields = value; }
        }

        private Sys_GridField groupField;
        /// <summary>
        /// 分组字段
        /// </summary>
        public Sys_GridField GroupField
        {
            get { return groupField; }
            set { groupField = value; }
        }

        private List<Sys_GridButton> gridButtons;
        /// <summary>
        /// 网格按钮
        /// </summary>
        public List<Sys_GridButton> GridButtons
        {
            get { return gridButtons; }
            set { gridButtons = value; }
        }

        /// <summary>
        /// 其他参数
        /// </summary>
        public string OtherParmas { get; set; }

        /// <summary>
        /// Dic参数集合
        /// </summary>
        public Dictionary<string, object> DicPramas { get; set; }
    }
}
