/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Base;
using Rookey.Frame.Common;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 菜单操作事件
    /// </summary>
    class Sys_MenuOperateHandle : IGridOperateHandle<Sys_Menu>
    {
        /// <summary>
        /// 网格参数设置
        /// </summary>
        /// <param name="gridType"></param>
        /// <param name="gridParams"></param>
        /// <param name="request"></param>
        public void GridParamsSet(DataGridType gridType, TempModel.GridParams gridParams, HttpRequestBase request = null)
        {
        }

        /// <summary>
        /// 网格数据加载参数设置
        /// </summary>
        /// <param name="module"></param>
        /// <param name="gridDataParams"></param>
        /// <param name="request"></param>
        public void GridLoadDataParamsSet(Sys_Module module, TempModel.GridDataParmas gridDataParams, HttpRequestBase request = null)
        {
        }

        /// <summary>
        /// 网格数据处理
        /// </summary>
        /// <param name="data"></param>
        /// <param name="otherParams"></param>
        /// <param name="currUser"></param>
        public void PageGridDataHandle(List<Sys_Menu> data, object[] otherParams = null, UserInfo currUser = null)
        {
            if (data != null)
            {
                List<Sys_Menu> list = new List<Sys_Menu>();
                foreach (Sys_Menu menu in data)
                {
                    Sys_Menu temp = new Sys_Menu();
                    ObjectHelper.CopyValue<Sys_Menu>(menu, temp);
                    string url = SystemOperate.GetIconUrl(menu.Icon);
                    if (!string.IsNullOrEmpty(url))
                    {
                        temp.Icon = string.Format("<img src=\"{0}\" />", url);
                    }
                    list.Add(temp);
                }
                data = list;
            }
        }

        public Expression<Func<Sys_Menu, bool>> GetGridFilterCondition(out string where, DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null)
        {
            where = string.Empty;
            return null;
        }

        public string GridButtonOperateVerify(string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null)
        {
            return string.Empty;
        }
    }
}
