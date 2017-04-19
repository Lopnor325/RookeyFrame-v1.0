/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using Rookey.Frame.Common.HttpHelper;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Operate.Base.EnumDef;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using System;
using Rookey.Frame.Base;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Base.Set;
using System.IO;

namespace Rookey.Frame.UIOperate
{
    /// <summary>
    /// UI操作
    /// </summary>
    public static class UIOperate
    {
        /// <summary>
        /// 从request中取moduleId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Guid GetModuleIdByRequest(HttpRequestBase request)
        {
            Sys_Module module = SystemOperate.GetModuleByRequest(request);
            if (module != null)
                return module.Id;
            return Guid.Empty;
        }

        /// <summary>
        /// 获取网格HTML
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static string GetGridHTML(HttpRequestBase request)
        {
            UserInfo currUser = UserInfo.GetCurretnUser(ApplicationObject.GetHttpContext(request));
            if (currUser == null)
                return GetAccountExpiredTipHtml();
            #region 参数初始化
            Guid moduleId = UIOperate.GetModuleIdByRequest(request); //模块Id
            Guid? menuId = request["mId"].ObjToGuidNull(); //菜单ID
            UIFrameFactory frameFactory = UIFrameFactory.GetInstance(request);
            string condition = string.Empty; //条件参数
            Guid? viewId = null; //视图Id
            DataGridType gridType = DataGridType.MainGrid;
            string initModule = string.Empty;
            string initField = string.Empty;
            Dictionary<string, object> dic = null; //网格其他参数
            string page = request["page"].ObjToStr(); //页面类型
            condition = HttpUtility.UrlDecode(request["condition"].ObjToStr()); //条件参数
            viewId = request["viewId"].ObjToGuidNull(); //视图Id
            bool recycle = request["recycle"].ObjToInt() == 1; //是否回收站
            bool draft = request["draft"].ObjToInt() == 1; //我的草稿
            gridType = DataGridType.MainGrid;
            initModule = string.Empty;
            initField = string.Empty;
            if (page == "fdGrid") //弹出网格
            {
                gridType = DataGridType.DialogGrid;
            }
            else if (page == "fwGrid") //列表页面明细或附属模块网格
            {
                gridType = DataGridType.FlowGrid;
            }
            else if (page == "inGrid") //网格内嵌入网格
            {
                gridType = DataGridType.InnerDetailGrid;
            }
            else if (page == "otGrid") //其他网格
            {
                gridType = DataGridType.Other;
            }
            else if (recycle) //回收站网格
            {
                gridType = DataGridType.RecycleGrid;
            }
            else if (draft) //我的草稿网格
            {
                gridType = DataGridType.MyDraftGrid;
            }
            if (gridType == DataGridType.DialogGrid)
            {
                initModule = HttpUtility.UrlDecode(request["initModule"].ObjToStr());
                initField = request["initField"].ObjToStr();
            }
            //where条件语句
            string where = request["where"].ObjToStr();
            if (!string.IsNullOrWhiteSpace(where))
            {
                try
                {
                    where = MySecurity.DecodeBase64(HttpUtility.UrlDecode(where));
                }
                catch
                {
                    where = string.Empty;
                }
            }
            //过滤字段
            List<string> filterFieldsList = null;
            string filterFields = request["filterFields"].ObjToStr();
            if (!string.IsNullOrWhiteSpace(filterFields))
            {
                filterFieldsList = filterFields.Split(",".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            //提出网格参数
            if (request.Params.AllKeys.Where(x => x.StartsWith("p_")).Count() > 0)
            {
                dic = new Dictionary<string, object>();
                List<string> keys = request.Params.AllKeys.Where(x => x.StartsWith("p_")).Distinct().ToList();
                foreach (string key in keys)
                {
                    dic.Add(key, request.Params[key]);
                }
            }
            string mutiSelect = request["ms"].ObjToStr(); //启用多选
            if (mutiSelect == "1")
            {
                if (dic == null) dic = new Dictionary<string, object>();
                dic.Add("muti_select", true);
            }
            #endregion
            #region 权限判断
            if (gridType == DataGridType.MainGrid)
            {
                bool noVeryfyMenuPower = request != null && request["nvm"].ObjToInt() == 1; //包含nvm=1时不验证菜单权限
                if (!noVeryfyMenuPower) //需要验证菜单权限
                {
                    bool hasPermission = menuId.HasValue && menuId.Value != Guid.Empty ? PermissionOperate.HasMenuPermission(currUser, menuId.Value) : PermissionOperate.HasModuleBrowerPermission(currUser, moduleId);
                    if (!hasPermission)
                    {
                        return "<div style=\"padding-top:20px;width:100%;text-align:center\"><font style=\"color:red;font-size:16px;font-weight:bold;\">您没有该模块数据的浏览权限！</font>";
                    }
                }
            }
            #endregion
            return frameFactory.GetGridHTML(moduleId, gridType, condition, where, viewId, initModule, initField, dic, false, filterFieldsList, menuId, false, request);
        }

        /// <summary>
        /// 获取皮肤名称
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetThemeName(HttpRequestBase request)
        {
            string cookieTheme = HttpCookieHelper.GetCookie("easyuitheme");
            if (string.IsNullOrEmpty(cookieTheme) && request["easyuitheme"].ObjToStr() != string.Empty)
            {
                cookieTheme = request["easyuitheme"].ObjToStr();
            }
            string themeName = string.IsNullOrWhiteSpace(cookieTheme) ? ConstDefine.DEFAULT_THEME : cookieTheme;
            return themeName;
        }

        /// <summary>
        /// 获取modeljs的HTML
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static string GetModelJsHTML(Guid moduleId, HttpRequestBase request)
        {
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            return GetModelJsHTML(module, request);
        }

        /// <summary>
        /// 获取modeljs的HTML
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static string GetModelJsHTML(Sys_Module module, HttpRequestBase request)
        {
            string html = string.Empty;
            bool isFullPath = request != null ? request[CommonDefine.NoFrameFlag].ObjToInt() == 1 : false; //JS显示全路径
            string modelJs = SystemOperate.GetModuleJsFilePath(module, isFullPath);
            if (!string.IsNullOrEmpty(modelJs))
            {
                //模块自定义js
                html = string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", modelJs);
            }
            return html;
        }

        /// <summary>
        /// 获取账号过期提示html
        /// </summary>
        /// <returns></returns>
        public static string GetAccountExpiredTipHtml()
        {
            return "<div style=\"padding-top:20px;width:100%;text-align:center\"><font style=\"color:red;font-size:16px;font-weight:bold;\">账号已过期请<a href=\"/User/Login.html\">重新登录</a>！</font>";
        }

        /// <summary>
        /// js是否显示全路径
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public static bool IsJsShowFullPath(HttpRequestBase request)
        {
            if (request != null)
            {
                bool isFullPath = request[CommonDefine.NoFrameFlag].ObjToInt() == 1;
                return isFullPath;
            }
            return false;
        }

        /// <summary>
        /// 格式化JS格式，根据修改日期添加标识
        /// </summary>
        /// <param name="jsPath">JS的URL路径</param>
        /// <returns></returns>
        internal static string FormatJsPath(string jsPath)
        {
            if (string.IsNullOrEmpty(jsPath))
                return string.Empty;
            string r = WebHelper.GetJsModifyTimeStr(jsPath);
            jsPath += string.Format("?r={0}", r);
            return jsPath;
        }
    }
}
