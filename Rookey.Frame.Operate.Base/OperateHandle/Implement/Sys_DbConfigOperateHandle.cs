using Rookey.Frame.Base;
using Rookey.Frame.Common.Model;
using Rookey.Frame.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Rookey.Frame.Operate.Base.OperateHandle.Implement
{
    /// <summary>
    /// 数据库配置操作处理类
    /// </summary>
    class Sys_DbConfigOperateHandle : IGridOperateHandle<Sys_DbConfig>, IFormOperateHandle<Sys_DbConfig>
    {
        public void GridParamsSet(EnumDef.DataGridType gridType, TempModel.GridParams gridParams, HttpRequestBase request = null)
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

        public void PageGridDataHandle(List<Sys_DbConfig> data, object[] otherParams = null, UserInfo currUser = null)
        {
            if (data != null && data.Count > 0)
            {
                foreach (Sys_DbConfig t in data)
                {
                    TbIndexInfo tbIndexInfo = SystemOperate.GetTableIndexInfo(t.ModuleName);
                    if (tbIndexInfo != null)
                        t.CurrPageDensity = tbIndexInfo.FragmentationPercent;
                }
            }
        }

        public System.Linq.Expressions.Expression<Func<Sys_DbConfig, bool>> GetGridFilterCondition(out string where, EnumDef.DataGridType gridType, Dictionary<string, string> condition = null, string initModule = null, string initField = null, Dictionary<string, string> otherParams = null, UserInfo currUser = null)
        {
            where = string.Empty;
            return null;
        }

        public string GridButtonOperateVerify(string buttonText, List<Guid> ids, object[] otherParams = null, UserInfo currUser = null)
        {
            return string.Empty;
        }

        public void FormDataHandle(Sys_DbConfig t, Model.EnumSpace.FormTypeEnum formType, UserInfo currUser = null)
        {
            if (t != null)
            {
                TbIndexInfo tbIndexInfo = SystemOperate.GetTableIndexInfo(t.ModuleName);
                if (tbIndexInfo != null)
                    t.CurrPageDensity = tbIndexInfo.FragmentationPercent;
            }
        }

        public List<TempModel.FormButton> GetFormButtons(Model.EnumSpace.FormTypeEnum formType, List<TempModel.FormButton> buttons, bool isAdd = false, bool isDraft = false, UserInfo currUser = null)
        {
            return buttons;
        }

        public List<TempModel.FormToolTag> GetFormToolTags(Model.EnumSpace.FormTypeEnum formType, List<TempModel.FormToolTag> tags, bool isAdd = false, UserInfo currUser = null)
        {
            return tags;
        }

        public string GetAutoCompleteDisplay(Sys_DbConfig t, string initModule, string initField, object[] otherParams = null, UserInfo currUser = null)
        {
            return string.Empty;
        }

        public bool OverSaveDetailData(TempModel.DetailFormObject detailObj, out string errMsg, UserInfo currUser = null)
        {
            errMsg = string.Empty;
            return false;
        }
    }
}
