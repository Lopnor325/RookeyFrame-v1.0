/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Operate.Base.EnumDef;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Rookey.Frame.Model.EnumSpace;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Base;
using Rookey.Frame.EntityBase;
using System.Reflection;
using Rookey.Frame.Model.Desktop;
using Rookey.Frame.Operate.Base.OperateHandle;
using Rookey.Frame.Model.Bpm;
using Rookey.Frame.Model.OrgM;
using Rookey.Frame.Base.User;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Common.HttpHelper;

namespace Rookey.Frame.UIOperate
{
    /// <summary>
    /// EasyUI页面框架
    /// </summary>
    public class EasyUIFrame : UIFrameFactory
    {
        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            #region 分页关键字初始化

            //页码起始值
            PageInfo.pageIndexStartNo = 1;

            //页码关键字
            PageInfo.pageIndexKeyWord = "page";

            //页记录数关键字
            PageInfo.pageSizeKeyWord = "rows";

            //排序字段关键字
            PageInfo.sortFieldKeyWord = "sort";

            //排序方式关键字
            PageInfo.sortOrderKeyWord = "order";

            //总记录数关键字，分页返回JSON结构中用到
            PageInfo.totalRecordKeyWord = "total";

            //分页数据关键字，分页返回JSON结构中用到
            PageInfo.pageDataKeyWord = "rows";
            #endregion
        }

        #endregion

        #region 登录页面

        /// <summary>
        /// 登录表单
        /// </summary>
        /// <param name="isDialog">是否为弹出框</param>
        /// <returns></returns>
        private string GetLoginForm(bool isDialog = false)
        {
            StringBuilder sb = new StringBuilder();
            string divAttr = !isDialog ? " class=\"easyui-dialog\" title=\"登录窗口\" data-options=\"iconCls:'eu-icon-password',closable:false,modal:true,buttons:[{id:'btnLogin',text:'登录',iconCls:'eu-icon-ok',handler:function(){DoLogin();}},{text:'关闭',iconCls:'eu-icon-close',handler:function(){Close();}}]\" style=\"width:470px;height:300px;overflow:hidden;\"" : string.Empty;
            sb.AppendFormat("<div id=\"w\"{0}>", divAttr);
            sb.Append("<table style=\"width:100%;\">");
            sb.Append("<tr><td colspan=\"2\"><img src=\"/Scripts/login/login_head.png\" /></td></tr>");
            sb.Append("<tr style=\"height:20px;\"><td colspan=\"2\">&nbsp;</td></tr>");
            sb.Append("<tr style=\"height:35px;\">");
            sb.Append("<td style=\"width:150px;text-align:right\">用户名：</td>");
            sb.Append("<td><input id=\"txtUserName\" class=\"easyui-textbox\" data-options=\"iconAlign:'left',iconCls:'eu-icon-user'\" style=\"width:200px;height:26px;\"></td>");
            sb.Append("</tr>");
            sb.Append("<tr style=\"height:35px;\">");
            sb.Append("<td style=\"width:150px;text-align:right\">密&nbsp;&nbsp;&nbsp;码：</td>");
            sb.Append("<td><input id=\"txtPwd\" type=\"password\" class=\"easyui-textbox\" data-options=\"iconAlign:'left',iconCls:'eu-icon-password'\" style=\"width:200px;height:26px;\"></td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"tr_validcode\" style=\"height:35px;display:none;\">");
            sb.Append("<td style=\"width:150px;text-align:right\">验证码：</td>");
            sb.Append("<td><table><tr><td><input id=\"txtValidate\" class=\"easyui-textbox\" style=\"width:125px;height:26px;\"></td><td><img id=\"validate\" onclick=\"this.src=this.src+'?'\" src=\"/Security/ValidateCode.html\" style=\"cursor: pointer; border: 1px solid #ddd\" alt=\"看不清楚，换一张\" title=\"看不清楚，换一张\" /></td></tr></table></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取登录页面
        /// </summary>
        /// <returns></returns>
        public override string GetLoginHTML()
        {
            return GetLoginForm(false);
        }

        /// <summary>
        /// 获取弹出登录框页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetDialogLoginHTML()
        {
            return GetLoginForm(true);
        }

        #endregion

        #region 主页面

        /// <summary>
        /// 返回主页面
        /// </summary>
        /// <returns></returns>
        public override string GetMainPageHTML()
        {
            StringBuilder sb = new StringBuilder();
            if (CurrUser == null) return sb.ToString();
            bool permissionFilter = CurrUser.UserName != "admin"; //是否要权限过滤
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(permissionFilter, CurrUser);
            //顶部区域
            string northStyle = string.Format("style=\"height:{0}px\"", ConstDefine.TOP_NORTH_REGION_HEIGHT.ToString());
            sb.AppendFormat("<div id=\"regionNorth\" defaultTheme=\"{1}\" data-options=\"region:'north'\" class=\"header\" {0}>", northStyle, ConstDefine.DEFAULT_THEME);
            sb.Append("<span class=\"header-right\">");
            sb.Append("<a id=\"btnLogout\" href=\"javascript:void(0)\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'eu-icon-exit_16'\">安全退出</a>");
            sb.Append("</span>");
            //sb.Append("<span class=\"header-right\">");
            //sb.Append("<a id=\"btnDocument\" href=\"javascript:void(0)\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'eu-p2-icon-ext-doc'\">文档</a>");
            //sb.Append("</span>");
            if (CurrUser.UserName == "admin" || (CurrUser.ExtendUserObject != null && CurrUser.ExtendUserObject.RoleNames != null && CurrUser.ExtendUserObject.RoleNames.Contains("系统管理员")))
            {
                sb.Append("<span class=\"header-right\">");
                sb.Append("<a id=\"btnFileManage\" href=\"javascript:void(0)\" class=\"easyui-linkbutton\" onclick=\"window.open('/FileManage/main.html');\" data-options=\"plain:true,iconCls:'eu-p2-icon-computer'\">系统文件管理</a>");
                sb.Append("</span>");
            }
            sb.Append("<span class=\"header-right\">");
            sb.AppendFormat("<a href=\"javascript:void(0)\" class=\"easyui-menubutton\" data-options=\"menu:'#mm_user',iconCls:'eu-p2-icon-user'\">欢迎您，{0}</a>", UserInfo.GetUserAliasName(CurrUser));
            sb.Append("<div id=\"mm_user\" style=\"width:100px;\">");
            sb.Append("<div data-options=\"iconCls:'eu-icon-cog'\" id=\"btnPersonalSet\">个人设置</div>");
            sb.Append("<div data-options=\"iconCls:'eu-icon-password'\" id=\"btnChangePwd\">修改密码</div>");
            string canChangeUserList = WebConfigHelper.GetAppSettingValue("CanChangeOps").ToLower(); //可切换操作用户列表
            List<string> canChangeList = !string.IsNullOrEmpty(canChangeUserList) ? canChangeUserList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList() : null;
            if (UserInfo.IsSuperAdmin(CurrUser) || (canChangeList != null && canChangeList.Contains(CurrUser.UserName.ToLower())))
            {
                sb.Append("<div class=\"menu-sep\"></div>");
                sb.Append("<div data-options=\"iconCls:'eu-icon-user'\" id=\"btnChangeUser\">切换用户</div>");
                //sb.Append("<div data-options=\"iconCls:'eu-p2-icon-drive_web'\" id=\"btnWebConfig\">系统配置</div>");
            }
            sb.Append("</div>");
            sb.Append("</span>");
            if (!GlobalSet.IsHorizontalMenu)
            {
                sb.Append("<span class=\"header-right\">");
                sb.Append("<a href=\"javascript:void(0)\" class=\"easyui-linkbutton easyui-tooltip\" data-options=\"iconCls:'eu-icon-grid',plain:true,hideEvent:'none',content:function(){return $('#quickOpToolbar');},");
                sb.Append("onShow:function(){var t=$(this);t.tooltip('tip').focus().unbind().bind('blur',function(){t.tooltip('hide');});}\">快捷菜单</a>");
                sb.Append("</span>");
            }
            sb.AppendFormat("<span class=\"head-left\" style=\"background:url({0}) no-repeat left;\">", WebConfigHelper.GetCurrWebLogo());
            string webLogoName = WebConfigHelper.GetCurrLogoName();
            if (string.IsNullOrEmpty(webLogoName)) webLogoName = "&nbsp;";
            sb.AppendFormat("<span>{0}</span>", webLogoName);
            sb.Append("</span>");
            if (GlobalSet.IsHorizontalMenu)
            {
                sb.Append("<div id=\"wnav\">");
                #region 横向菜单
                foreach (Sys_Menu menu in topMenus)
                {
                    string menuIcon = string.IsNullOrEmpty(menu.Icon) ? "eu-icon-grid" : menu.Icon;
                    string menuDisplay = string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display;
                    sb.AppendFormat("<span class=\"menu-left\"><a href=\"javascript:void(0)\" class=\"easyui-menubutton\" data-options=\"menu:'#mm_menu_{0}',iconCls:'{1}'\">{2}</a></span>", menu.Id.ToString(), menuIcon, menuDisplay);
                    sb.AppendFormat("<div id=\"mm_menu_{0}\" class=\"easyui-menu\" style=\"min-width:100px;\">", menu.Id.ToString());
                    List<Sys_Menu> childMenus = SystemOperate.GetChildMenus(menu.Id, true, false, permissionFilter, CurrUser);
                    foreach (Sys_Menu child in childMenus)
                    {
                        string childMenuIcon = string.IsNullOrEmpty(child.Icon) ? "eu-icon-grid" : child.Icon;
                        string childDisplay = string.IsNullOrEmpty(child.Display) ? child.Name : child.Display;
                        string url = child.Url.ObjToStr();
                        if (!string.IsNullOrEmpty(url))
                        {
                            if (url.IndexOf("?") > -1)
                                url += "&";
                            else
                                url += "?";
                            url += string.Format("mId={0}", child.Id.ToString());
                        }
                        else if (child.Sys_ModuleId.HasValue)
                        {
                            url = string.Format("/Page/Grid.html?page=grid&moduleId={0}&moduleName={1}", child.Sys_ModuleId.Value.ToString(), SystemOperate.GetModuleNameById(child.Sys_ModuleId.Value));
                        }
                        string menuClickMethod = string.IsNullOrEmpty(url) ? string.Empty : (child.IsNewWinOpen ? string.Format("window.open('{0}');", url) : string.Format("AddTab(null,'{0}','{1}');", childDisplay, url));
                        sb.AppendFormat("<div data-options=\"iconCls:'{0}'\" onclick=\"{1}\">", childMenuIcon, menuClickMethod);
                        List<Sys_Menu> thirdMenus = SystemOperate.GetChildMenus(child.Id, true, false, permissionFilter, CurrUser);
                        if (thirdMenus.Count > 0)
                        {
                            sb.AppendFormat("<span>{0}</span>", childDisplay);
                            sb.Append("<div>");
                            foreach (Sys_Menu thirdMenu in thirdMenus)
                            {
                                string thirdMenuIcon = string.IsNullOrEmpty(thirdMenu.Icon) ? "eu-icon-grid" : thirdMenu.Icon;
                                string thirdDisplay = string.IsNullOrEmpty(thirdMenu.Display) ? thirdMenu.Name : thirdMenu.Display;
                                string thirdUrl = thirdMenu.Url.ObjToStr();
                                if (!string.IsNullOrEmpty(thirdUrl))
                                {
                                    if (thirdUrl.IndexOf("?") > -1)
                                        thirdUrl += "&";
                                    else
                                        thirdUrl += "?";
                                    thirdUrl += string.Format("mId={0}", thirdMenu.Id.ToString());
                                }
                                else if (thirdMenu.Sys_ModuleId.HasValue)
                                {
                                    thirdUrl = string.Format("/Page/Grid.html?page=grid&moduleId={0}&moduleName={1}", thirdMenu.Sys_ModuleId.Value.ToString(), SystemOperate.GetModuleNameById(thirdMenu.Sys_ModuleId.Value));
                                }
                                string thirdMenuClickMethod = string.IsNullOrEmpty(thirdUrl) ? string.Empty : (thirdMenu.IsNewWinOpen ? string.Format("window.open('{0}');", thirdUrl) : string.Format("AddTab(null,'{0}','{1}');", thirdDisplay, thirdUrl));
                                sb.AppendFormat("<div data-options=\"iconCls:'{0}'\" onclick=\"{1}\">{2}</div>", thirdMenuIcon, thirdMenuClickMethod, thirdDisplay);
                            }
                            sb.Append("</div>");
                        }
                        else
                        {
                            sb.Append(childDisplay);
                        }
                        sb.Append("</div>");
                    }
                    sb.Append("</div>");
                }
                #endregion
                sb.Append("</div>");
            }
            sb.Append("</div>");
            if (!GlobalSet.IsHorizontalMenu)
            {
                #region 纵向菜单
                //左边菜单区域
                sb.AppendFormat("<div data-options=\"region:'west',title:'功能菜单',split:true\" style=\"width:{0}px;height:100%;background:#eee;float:left;\">", ConstDefine.MAIN_LEFT_MENU_WIDTH.ToString());
                sb.Append("<div class=\"easyui-accordion\" data-options=\"fit:true,border:false\" id=\"leftMenu\">");
                foreach (Sys_Menu menu in topMenus)
                {
                    string menuDisplay = string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display;
                    sb.AppendFormat("<div title=\"{0}\" data-options=\"iconCls:'{1}'\">", menuDisplay, menu.Icon.ObjToStr());
                    sb.AppendFormat("<ul menuId=\"{0}\" url=\"{1}\" name=\"{2}\"></ul>", menu.Id.ToString(), menu.Url.ObjToStr(), menuDisplay);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
                sb.Append("</div>");
                #endregion
            }
            //中间内容区域
            sb.Append("<div id=\"center\" data-options=\"region:'center'\" style=\"background: #eee; overflow: hidden;\">");
            sb.Append("<div id=\"tabs\" class=\"easyui-tabs\" fit=\"true\" border=\"false\" th=\"" + ConstDefine.TAB_HEAD_HEIGHT + "\" data-options=\"tools:'#tabs_toolbar',onClose:function(title,index){tabHeightInit();},onSelect:function(title,index){tabHeightInit();if(typeof(OverOnSelect)=='function'){OverOnSelect(title,index);}},tabHeight:" + ConstDefine.TAB_HEAD_HEIGHT.ToString() + "\">");
            sb.Append("<div title=\"我的桌面\" style=\"overflow: hidden;\"><iframe id=\"deskIframe\" scrolling=\"auto\" frameborder=\"0\" url=\"/Page/DesktopIndex.html\" style=\"width: 100%;height: 100%;\"></iframe></div>");
            sb.Append("</div>");
            sb.Append("</div>");
            //底部状态栏
            sb.Append("<div id=\"footer\" data-options=\"region:'south'\" class=\"bottom-south\">");
            sb.Append("<div style=\"text-align:center;\">");
            sb.AppendFormat("<span>{0}</span>", WebConfigHelper.GetCurrWebCopyright());
            sb.Append("</div>");
            sb.Append("</div>");
            //tab工具栏
            sb.Append("<div id=\"tabs_toolbar\" style=\"padding-top:3px;\">");
            sb.Append("<a class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-reload',plain:true\" title=\"刷新\" onclick=\"RefreshTab()\"></a>");
            sb.Append("<a class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-arrow_out',plain:true\" title=\"最大\" onclick=\"maximizeTab()\" id=\"ttb_max\"></a>");
            sb.Append("<a class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-arrow_in',plain:true\" title=\"还原\" onclick=\"restoreTab()\" id=\"ttb_min\" style=\"display: none;\"></a>");
            sb.Append("<a class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-cancel',plain:true\" title=\"关闭\" onclick=\"CloseTab()\"></a>");
            sb.Append("</div>");
            //标签右键菜单
            sb.Append("<div id=\"mm\" class=\"easyui-menu\" style=\"width: 180px;\">");
            sb.Append("<div id=\"mm-tabupdate\" data-options=\"iconCls:'eu-icon-arrow_refresh'\">刷新</div>");
            sb.Append("<div class=\"menu-sep\"></div>");
            sb.Append("<div id=\"mm-tabclose\" data-options=\"iconCls:'eu-p2-icon-cancel'\">关闭</div>");
            sb.Append("<div id=\"mm-tabcloseall\" data-options=\"iconCls:'eu-p2-icon-cancel'\">全部关闭</div>");
            sb.Append("<div id=\"mm-tabcloseother\" data-options=\"iconCls:'eu-p2-icon-cancel'\">除此之外全部关闭</div>");
            sb.Append("<div class=\"menu-sep\"></div>");
            sb.Append("<div id=\"mm-tabcloseright\" data-options=\"iconCls:'eu-p2-icon-cancel'\">当前页右侧全部关闭</div>");
            sb.Append("<div id=\"mm-tabcloseleft\" data-options=\"iconCls:'eu-p2-icon-cancel'\">当前页左侧全部关闭</div>");
            sb.Append("</div>");
            //顶层弹出框
            sb.Append("<div id=\"page_dialog1\"></div>");
            sb.Append("<div id=\"page_dialog2\"></div>");
            sb.Append("<div id=\"page_dialog3\"></div>");
            sb.Append("<div id=\"page_dialog4\"></div>");
            sb.Append("<div id=\"page_dialog5\"></div>");
            sb.Append("<div id=\"page_dialog6\"></div>");
            sb.Append("<div id=\"page_dialog7\"></div>");
            sb.Append("<div id=\"page_dialog8\"></div>");
            sb.Append("<div id=\"page_dialog9\"></div>");
            sb.Append("<div id=\"page_dialog10\"></div>");
            //快捷菜单Tooltip
            if (!GlobalSet.IsHorizontalMenu)
            {
                #region 快捷菜单Tooltip
                sb.Append("<div style=\"display:none\">");
                sb.Append("<div id=\"quickOpToolbar\" style=\"background:#F4F4F4\">");
                sb.Append("<table style=\"width:400px;line-height:50px;\">");
                List<Sys_Menu> userQuckMenus = SystemOperate.GetUserQuckMenus(CurrUser.UserId);
                if (userQuckMenus.Count > 0)
                {
                    for (int i = 0; i < userQuckMenus.Count; i++)
                    {
                        Sys_Menu menu = userQuckMenus[i];
                        int r = i / 3; //当前所在行
                        int c = i % 3; //当前所在列
                        if (i == 0)
                        {
                            sb.Append("<tr>");
                        }
                        else if (c == 0)
                        {
                            sb.Append("</tr><tr>");
                        }
                        string nodeJson = "{text:'" + (string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display) + "',attribute:{url:'" + menu.Url.ObjToStr() + "',obj:{isNewWinOpen:" + menu.IsNewWinOpen.ToString().ToLower() + "";
                        if (menu.Sys_ModuleId.HasValue && menu.Sys_ModuleId.Value != Guid.Empty)
                        {
                            nodeJson += ",moduleId:'" + menu.Sys_ModuleId.Value.ToString() + "',moduleName:'" + menu.Sys_ModuleName + "'";
                        }
                        nodeJson += "}}}";
                        sb.AppendFormat("<td style=\"width:33%\"><a href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'{0}',plain:true\"  onclick=\"TreeNodeOnClick({2})\">{1}</a></td>", string.IsNullOrEmpty(menu.Icon) ? "eu-p2-icon-table" : menu.Icon, string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display, nodeJson);
                    }
                    sb.Append("</tr>");
                }
                else
                {
                    sb.Append("<tr><td><div style=\"height:50px;line-height:50px;text-align:center\">还没有添加快捷菜单，赶紧添加吧！</div></td></tr>");
                }
                sb.Append("<tr><td colspan=\"2\"><a href=\"#\" onclick=\"SetQuckMenu();\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-p2-icon-tag_blue_add',plain:true\">添加快捷菜单</a></td></tr>");
                sb.Append("</table>");
                sb.Append("</div>");
                sb.Append("</div>");
                #endregion
            }
            sb.AppendFormat("<input id=\"isShowStyleBtn\" type=\"hidden\" value=\"{0}\" />", GlobalSet.IsShowStyleBtn ? "1" : "0");
            sb.AppendFormat("<input id=\"userInfo\" type=\"hidden\" value=\"{0}\" />", HttpUtility.UrlEncode(JsonHelper.Serialize(CurrUser).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20"));
            return sb.ToString();
        }

        /// <summary>
        /// 获取个人设置页面
        /// </summary>
        /// <returns></returns>
        public override string GetPersonalSetHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding:10px\"><table style=\"line-height:30px;\">");
            sb.Append("<tr>");
            sb.Append("<td style=\"width:100px;text-align:center\">皮肤：</td>");
            sb.Append("<td><input id=\"cb-theme\" style=\"width:200px\" /></td>");
            sb.Append("</tr>");
            sb.Append("</table></div>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/common/PersonalSet.js\"></script>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取修改密码页面
        /// </summary>
        /// <returns></returns>
        public override string GetChangePwdHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<form id=\"changePwdForm\">");
            sb.Append("<table style=\"width:450px;height:100px;margin:20px;\">");
            sb.Append("<tr>");
            sb.Append("<td>当前密码：</td>");
            sb.Append("<td><input id=\"oldPwd\" name=\"oldPwd\" class=\"easyui-textbox\" type=\"password\" data-options=\"required:true,missingMessage:null,iconCls:'eu-icon-password',iconAlign:'left'\" style=\"width:300px\"></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>新设密码：</td>");
            sb.Append("<td><input id=\"newPwd1\" name=\"newPwd1\" class=\"easyui-textbox\" type=\"password\" data-options=\"required:true,missingMessage:null,iconCls:'eu-icon-changePwd',iconAlign:'left'\" style=\"width:300px\"></td>");
            sb.Append("</tr>");
            sb.Append("<td>再次输入：</td>");
            sb.Append("<td><input id=\"newPwd2\" name=\"newPwd2\" class=\"easyui-textbox\" type=\"password\" data-options=\"required:true,missingMessage:null,iconCls:'eu-icon-changePwd',iconAlign:'left'\" style=\"width:300px\"></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</form>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/common/ChangePwd.js\"></script>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取添加快捷菜单页面
        /// </summary>
        /// <returns></returns>
        public override string GetAddQuckMenuHTML()
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<Sys_Menu, List<Sys_Menu>> dic = new Dictionary<Sys_Menu, List<Sys_Menu>>();
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(true, CurrUser);
            if (topMenus.Count > 0)
            {
                foreach (Sys_Menu menu in topMenus)
                {
                    var childMenus = SystemOperate.GetChildMenus(menu.Id, false, false, true, CurrUser);
                    childMenus = childMenus.Where(x => x.IsLeaf).ToList();
                    if (childMenus.Count > 0)
                    {
                        dic.Add(menu, childMenus);
                    }
                }
            }
            if (dic.Count > 0)
            {
                int h = 520 - ConstDefine.TOP_NORTH_REGION_HEIGHT - ConstDefine.TAB_HEAD_HEIGHT * 2 - 40;
                sb.Append("<script type=\"text/javascript\" src=\"/Scripts/easyui-extension/datagrid-groupview.js\"></script>");
                sb.Append("<div style=\"padding: 10px;\">");
                sb.Append("<style type=\"text/css\">");
                sb.Append("li {");
                sb.Append("text-align: center;");
                sb.Append("margin-bottom: 3px;");
                sb.Append("}");
                sb.Append("li a {");
                sb.Append("width: 30px;");
                sb.Append("}");
                sb.Append("</style>");
                sb.Append("<table style=\"width: 100%; height: 100%; line-height: 30px;\">");
                sb.Append("<tr>");
                sb.Append("<td colspan=\"3\">");
                sb.Append("</td></tr>");
                sb.Append("<tr>");
                sb.Append("<td style=\"width: 180px;\">可选菜单：</td>");
                sb.Append("<td style=\"width: 60px;\"></td>");
                sb.Append("<td style=\"width: 350px;\">已选菜单：</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<table id=\"leftGrid\" class=\"easyui-datagrid\" style=\"width: 180px; height: 350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'MenuId',view:groupview,groupField:'BigModule',groupFormatter:function(value, rows){return value+ '(' + rows.length + ')';}\">");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th data-options=\"field:'MenuId',checkbox:true\">菜单Id</th>");
                sb.Append("<th data-options=\"field:'BigModule',hidden:true\">大模块</th>");
                sb.Append("<th data-options=\"field:'MenuName',align:'left',width:150\">功能菜单</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("<tbody>");
                foreach (Sys_Menu key in dic.Keys)
                {
                    var list = dic[key];
                    foreach (Sys_Menu child in list)
                    {
                        sb.Append("<tr>");
                        sb.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", child.Id.ToString(), string.IsNullOrEmpty(key.Display) ? key.Name : key.Display, string.IsNullOrEmpty(child.Display) ? child.Name : child.Display);
                        sb.Append("</tr>");
                    }
                }
                sb.Append("</tbody>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<ul>");
                sb.Append("<li><a href=\"#\" title=\"移入选中项\" class=\"easyui-linkbutton\" onclick=\"RightMove()\">></a></li>");
                sb.Append("<li><a href=\"#\" title=\"全部移入\" class=\"easyui-linkbutton\" onclick=\"RightMoveAll()\">>></a></li>");
                sb.Append("<li><a href=\"#\" title=\"移出选中项\" class=\"easyui-linkbutton\" onclick=\"LeftMove()\"><</a></li>");
                sb.Append("<li><a href=\"#\" title=\"全部移出\" class=\"easyui-linkbutton\" onclick=\"LeftMoveAll()\"><<</a></li>");
                sb.Append("</ul>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<table id=\"rightGrid\" class=\"easyui-datagrid\" style=\"width: 350px;height:350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'MenuId'\">");
                sb.Append("<thead>");
                sb.Append("<tr>");
                sb.Append("<th data-options=\"field:'MenuId',checkbox:true\">菜单Id</th>");
                sb.Append("<th data-options=\"field:'MenuName',align:'left',width:180\">功能菜单</th>");
                sb.Append("<th data-options=\"field:'Operate',width:170\">操作</th>");
                sb.Append("</tr>");
                sb.Append("</thead>");
                List<Sys_Menu> userQuckMenus = SystemOperate.GetUserQuckMenus(CurrUser.UserId);
                foreach (Sys_Menu menu in userQuckMenus)
                {
                    string mIdStr = menu.Id.ToString();
                    sb.Append("<tr>");
                    string operate = "<input id=\"btnUp_" + mIdStr + "\" rowId=\"" + mIdStr + "\" title=\"上移\" type=\"button\" style=\"width:30px;\" value=\"↑\" />";
                    operate += "<input id=\"btnDown_" + mIdStr + "\" rowId=\"" + mIdStr + "\" title=\"下移\" type=\"button\" style=\"width:30px;\" value=\"↓\" />";
                    operate += "<input id=\"btnTop_" + mIdStr + "\" rowId=\"" + mIdStr + "\" title=\"移至最顶部\" type=\"button\" style=\"width:30px;\" value=\"↑↑\" />";
                    operate += "<input id=\"btnBottom_" + mIdStr + "\" rowId=\"" + mIdStr + "\" title=\"移至最底部\" type=\"button\" style=\"width:30px;\" value=\"↓↓\" />";
                    sb.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>", mIdStr, string.IsNullOrEmpty(menu.Display) ? menu.Name : menu.Display, operate);
                    sb.Append("</tr>");
                }
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</div>");
                sb.Append("<script type=\"text/javascript\" src=\"/Scripts/common/AddQuckMenu.js\"></script>");
            }
            else
            {
                sb.Append("<div style=\"height:200px;line-height:200px;\">没有可用菜单！</div>");
            }
            return sb.ToString();
        }

        #endregion

        #region 列表页面

        /// <summary>
        /// 返回网格页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="condition">过滤条件</param>
        /// <param name="where">where过滤条件</param>
        /// <param name="viewId">视图Id</param>
        /// <param name="initModule">针对表单弹出外键选择框，表单原始模块</param>
        /// <param name="initField">针对表单外键弹出框，表单原始字段</param>
        /// <param name="otherParams">其他参数</param>
        /// <param name="detailCopy">明细复制</param>
        /// <param name="filterFields">过滤字段</param>
        /// <param name="menuId">菜单ID，从哪个菜单进来的</param>
        /// <param name="isGridLeftTree">是否网格左侧树，针对附属网格</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public override string GetGridHTML(Guid moduleId, DataGridType gridType = DataGridType.MainGrid, string condition = null, string where = null, Guid? viewId = null, string initModule = null, string initField = null, Dictionary<string, object> otherParams = null, bool detailCopy = false, List<string> filterFields = null, Guid? menuId = null, bool isGridLeftTree = false, HttpRequestBase request = null)
        {
            #region 页面重写
            string html = GetCustomerPageHTML(moduleId, "GetGridHTML", new object[] { gridType, condition, where, viewId, initModule, initField, otherParams, detailCopy, filterFields, menuId, isGridLeftTree, request });
            if (!string.IsNullOrEmpty(html)) return html;
            #endregion
            #region 准备参数
            UserInfo currUser = this.CurrUser;
            //用户视图
            Sys_Grid grid = gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid ? SystemOperate.GetDefaultGrid(moduleId) : (viewId.HasValue ? SystemOperate.GetGrid(viewId.Value) : SystemOperate.GetUserDefaultGrid(currUser.UserId, moduleId));
            if (grid == null) return string.Empty;
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(moduleId); //是否启用流程
            Sys_Form form = null; //表单对象
            string customerFormUrl = string.Empty;
            if (isEnabledFlow)
                customerFormUrl = BpmOperate.GetLaunchNodeCustomerFormUrl(moduleId);
            if (string.IsNullOrEmpty(customerFormUrl))
            {
                form = SystemOperate.GetUserFinalForm(currUser, moduleId);
            }
            #endregion
            #region 获取缓存key
            string cacheKey = string.Empty;
            if (GlobalSet.IsEnabledPageCache)
            {
                cacheKey = string.Format("{0}_{1}_{2}", moduleId.ToString(), grid.Id.ToString(), currUser.UserId.ToString());
                cacheKey += string.Format("_{0}_{1}", currUser.ClientBrowserWidth, currUser.ClientBrowserHeight);
                if (form != null) cacheKey += string.Format("_{0}", form.Id.ToString());
                if (!string.IsNullOrEmpty(customerFormUrl)) cacheKey += string.Format("_{0}", customerFormUrl);
                if (!string.IsNullOrEmpty(condition)) cacheKey += string.Format("_{0}", condition);
                if (!string.IsNullOrEmpty(where)) cacheKey += string.Format("_{0}", where);
                if (!string.IsNullOrEmpty(initModule)) cacheKey += string.Format("_{0}", initModule);
                if (!string.IsNullOrEmpty(initField)) cacheKey += string.Format("_{0}", initField);
                if (otherParams != null && otherParams.Keys.Count > 0) cacheKey += string.Format("_{0}", string.Join(",", otherParams.Keys));
                if (filterFields != null && filterFields.Count > 0) cacheKey += string.Format("_{0}", string.Join(",", filterFields));
                if (detailCopy) cacheKey += "_true";
            }
            #endregion
            #region 取页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                string cacheHtml = SystemOperate.GetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.GridPage);
                if (!string.IsNullOrEmpty(cacheHtml))
                {
                    return cacheHtml;
                }
            }
            #endregion
            #region 生成页面
            #region 变量定义
            string errMsg = string.Empty;
            Sys_Module module = SystemOperate.GetModuleById(moduleId); //模块对象
            Type modelType = CommonOperate.GetModelType(module.TableName); //实体类型
            int formWidth = 500; //表单宽度
            int formHeight = 300; //表单高度
            int editMode = form != null ? GetEditMode(module, form, out formWidth, out formHeight, currUser.UserId) : (int)ModuleEditModeEnum.TabFormEdit; //编辑模式
            string titleKey = string.IsNullOrEmpty(module.TitleKey) ? string.Empty : module.TitleKey;
            string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(module);
            //取用户默认视图
            bool isDetailView = false; //是否是明细视图
            bool isShowFullPath = UIOperate.IsJsShowFullPath(request);
            string domainPath = isShowFullPath ? Globals.GetBaseUrl() : "/";
            #region 视图字段处理
            //加载视图字段
            List<Sys_GridField> gridFields = grid.GridFields != null && grid.GridFields.Count > 0 ? grid.GridFields : SystemOperate.GetGridFields(grid, true, true);
            if (grid.GridFields == null && gridFields.Count > 0)
            {
                grid.GridFields = gridFields;
            }
            if (filterFields != null && filterFields.Count > 0)
            {
                gridFields = gridFields.Where(x => filterFields.Contains(x.Sys_FieldName)).ToList();
            }
            if (grid.CreateUserId != currUser.UserId) //非自己创建的视图需要字段权限过滤
            {
                List<string> canViewFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, grid.Sys_ModuleId.Value, FieldPermissionTypeEnum.ViewField);
                if (canViewFields != null && canViewFields.Count > 0 && !canViewFields.Contains("-1"))
                    gridFields = gridFields.Where(x => canViewFields.Contains(x.Sys_FieldName)).ToList();
            }
            //取分组字段
            Sys_GridField groupField = gridFields.Where(x => x.IsGroupField).FirstOrDefault(); //SystemOperate.GetGridGroupField(grid.Id);
            #region 编辑、查看明细网格字段特殊处理
            if (gridType == DataGridType.EditDetailGrid && form != null) //针对编辑页面网格特殊处理
            {
                //表单字段
                List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id);
                if (form.FormFields == null && formFields.Count > 0)
                {
                    form.FormFields = formFields;
                }
                List<string> fieldNames = formFields.Where(x => x.Sys_FieldId.HasValue).Select(x => x.Sys_FieldName).ToList();
                #region 外键字段处理
                //外键字段处理
                if (SystemOperate.IsDetailModule(moduleId)) //当前是明细模块
                {
                    Sys_Module parentModule = SystemOperate.GetParentModule(moduleId);
                    if (editMode == (int)ModuleEditModeEnum.GridRowEdit)
                    {
                        string parentForeginField = string.Format("{0}Id", parentModule.TableName);
                        gridFields = gridFields.Where(x => x.Sys_FieldName != parentForeginField).ToList();
                        formFields = formFields.Where(x => x.Sys_FieldName != parentForeginField).ToList();
                        fieldNames.Remove(parentForeginField);
                    }
                    List<Sys_Field> foreignFields = SystemOperate.GetAllSysFields(x => x.Sys_ModuleId == moduleId && fieldNames.Contains(x.Name) && x.ForeignModuleName != null && x.ForeignModuleName != string.Empty && x.ForeignModuleName != parentModule.Name);
                    if (foreignFields.Count > 0)
                    {
                        List<string> foreignFieldNames = foreignFields.Select(x => x.Name.EndsWith("Id") ? x.Name.Substring(0, x.Name.Length - 2) + "Name" : x.Name + "Name").ToList();
                        fieldNames.AddRange(foreignFieldNames);
                    }
                }
                #endregion
                List<string> tempGridFieldNames = gridFields.Where(x => x.Sys_FieldId.HasValue).Select(x => x.Sys_FieldName).ToList();
                //表单字段对应的网格字段
                gridFields = gridFields.Where(x => fieldNames.Contains(x.Sys_FieldName)).ToList();
                foreach (Sys_GridField gf in gridFields.Where(x => x.IsVisible))
                {
                    Sys_FormField ff = formFields.Where(y => y.Sys_FieldName == gf.Sys_FieldName).FirstOrDefault();
                    if (ff == null) continue;
                    if (editMode == (int)ModuleEditModeEnum.GridRowEdit)
                        gf.Width = ff.Width;
                    gf.IsVisible = editMode != (int)ModuleEditModeEnum.GridRowEdit || (editMode == (int)ModuleEditModeEnum.GridRowEdit && ff.RowEditRowNo == 0);
                }
                //将表单字段中存在网格字段中不存在的字段初始化到网格字段中
                List<Sys_FormField> tempFields = formFields.Where(x => !tempGridFieldNames.Contains(x.Sys_FieldName)).ToList();
                if (tempFields.Count > 0)
                {
                    gridFields.AddRange(tempFields.Select(x => new Sys_GridField()
                    {
                        Sys_FieldId = x.Sys_FieldId,
                        Sys_FieldName = x.Sys_FieldName,
                        Align = (int)AlignTypeEnum.Left,
                        Width = x.Width,
                        Display = x.Display,
                        IsVisible = editMode != (int)ModuleEditModeEnum.GridRowEdit || (editMode == (int)ModuleEditModeEnum.GridRowEdit && x.RowEditRowNo == 0)
                    }));
                }
                //取消字段的排序
                gridFields.ForEach(x => { x.IsAllowSort = false; });
            }
            if ((gridType == DataGridType.ViewDetailGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.InnerDetailGrid) && form != null)
            {
                if (editMode == (int)ModuleEditModeEnum.GridRowEdit)
                {
                    //表单字段
                    List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id);
                    if (form.FormFields == null && formFields.Count > 0)
                    {
                        form.FormFields = formFields;
                    }
                    formFields = formFields.Where(x => x.RowEditRowNo != 0).ToList();
                    List<string> fieldNames = formFields.Where(x => x.Sys_FieldId.HasValue).Select(x => x.Sys_FieldName).ToList();
                    gridFields.Where(x => fieldNames.Contains(x.Sys_FieldName)).ForEach(x =>
                    {
                        x.IsVisible = false;
                    });
                }
            }
            #endregion
            #endregion
            //网格操作按钮
            List<Sys_GridButton> gridButtons = SystemOperate.GetGridButtons(moduleId);
            List<Guid> btnIds = PermissionOperate.GetUserFunPermissions(currUser, FunctionTypeEnum.Button, moduleId);
            if (btnIds != null)
                gridButtons = gridButtons.Where(x => btnIds.Contains(x.Id)).ToList();
            if (gridType == DataGridType.EditDetailGrid && editMode == (int)ModuleEditModeEnum.GridRowEdit)
                gridButtons = gridButtons.Where(x => x.ButtonTagId != "btnEdit").ToList();
            if (gridType == DataGridType.MainGrid && isEnabledFlow && (gridButtons.Where(x => x.ButtonTagId == "btnAdd").Count() > 0 || gridButtons.Where(x => x.ButtonTagId == "btnEdit").Count() > 0)) //主网格并且有新增和修改权限
            {
                if (BpmOperate.IsAllowLaunchFlow(moduleId, currUser, null)) //当前人有流程发起权限
                {
                    Sys_GridButton startFlowBtn = gridButtons.Where(x => x.ButtonTagId == "btnStartFlow").FirstOrDefault();
                    if (startFlowBtn == null)
                    {
                        string btnSubmitFlowText = "提交";
                        startFlowBtn = new Sys_GridButton() { ButtonTagId = "btnStartFlow", ButtonIcon = "eu-icon-tosubmit", ButtonText = btnSubmitFlowText, GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar, ClickMethod = "MutiStartFlow(this)", IsSystem = true, IsValid = true, OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton };
                        gridButtons.Add(startFlowBtn);
                    }
                    List<Sys_Module> detailModules = SystemOperate.GetDetailModules(moduleId);
                    if (detailModules.Where(x => BpmOperate.IsEnabledWorkflow(x.Id)).Count() == 0) //明细模块没有启用流程
                    {
                        Sys_GridButton reStartFlowBtn = gridButtons.Where(x => x.ButtonTagId == "btnReStartFlow").FirstOrDefault();
                        if (reStartFlowBtn == null)
                        {
                            string btnSubmitFlowText = "重新发起";
                            reStartFlowBtn = new Sys_GridButton() { ButtonTagId = "btnReStartFlow", ButtonIcon = "eu-icon-tosubmit", ButtonText = btnSubmitFlowText, GridButtonLocationOfEnum = GridButtonLocationEnum.Toolbar, ClickMethod = "ReStartFlow(this)", IsSystem = true, IsValid = true, OperateButtonTypeOfEnum = OperateButtonTypeEnum.CommonButton };
                            gridButtons.Add(reStartFlowBtn);
                        }
                    }
                }
            }
            //网格参数
            GridParams gridParams = new GridParams();
            gridParams.DicPramas = otherParams;
            gridParams.GridFields = gridFields;
            gridParams.GroupField = groupField;
            //是否加载附属模块
            bool isLoadAttachModule = false;
            if (GlobalSet.IsAllowAttachModule && gridType == DataGridType.MainGrid && grid.GridTypeOfEnum != GridTypeEnum.ComprehensiveDetail) //非综合明细视图
            {
                //用户是否绑定了下方附属模块
                bool hasUserAttach = SystemOperate.HasUserAttachModule(currUser.UserId, moduleId, false);
                isLoadAttachModule = GlobalSet.IsAllowAttachModule && gridType == DataGridType.MainGrid && (hasUserAttach || (!module.DetailInGrid && SystemOperate.HasDetailModule(moduleId)));
            }
            //html装载器
            StringBuilder sb = new StringBuilder();
            #endregion
            #region 加载脚本
            if (gridType != DataGridType.FlowGrid)
            {
                //打印JS
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery.jqprint-0.3.js\"></script>", domainPath);
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery-migrate-1.2.1.min.js\"></script>", domainPath);
                //数据网格js
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/easyui-extension/datagrid-groupview.js\"></script>", domainPath);
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/easyui-extension/datagrid-detailview.js\"></script>", domainPath);
                string formatterJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/Formatter.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/Formatter.js?r={1}\"></script>", domainPath, formatterJsr);
            }
            #endregion
            #region 左边树
            if (gridType == DataGridType.MainGrid && !string.IsNullOrWhiteSpace(grid.TreeField) && !grid.IsTreeGrid)
            {
                StringBuilder treeSb = new StringBuilder();
                string treeField = grid.TreeField; //树字段
                string treeTitle = string.Empty; //树标题
                string url = string.Empty; //树加载url
                if (SystemOperate.IsForeignField(moduleId, treeField)) //外键字段
                {
                    bool isTree = false; //是否是树型
                    //外键模块对象
                    Sys_Module foreignModule = SystemOperate.GetForeignModule(moduleId, treeField);
                    Type foreignModelType = CommonOperate.GetModelType(foreignModule.Id); //外键模块实体类型
                    if (foreignModelType.GetProperty("ParentId") != null) //外键模块是树型模块
                    {
                        //只有一个根结点才以树型方式加载
                        long n = CommonOperate.Count(out errMsg, foreignModule.Id, false, null, "ParentId IS NULL OR ParentId='" + Guid.Empty.ToString() + "'");
                        if (n == 1) isTree = true;
                    }
                    if (isTree) //加载外键树
                    {
                        //外键字段的树标题为外键模块名称
                        treeTitle = foreignModule.Name;
                        //树url
                        url = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, foreignModule.Id.ToString());
                    }
                    else
                    {
                        //树标题为字段名称
                        treeTitle = SystemOperate.GetFieldDisplay(moduleId, treeField);
                        //树url
                        url = string.Format("/{0}/GetTreeByNode.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), treeField);
                    }
                }
                else if (SystemOperate.IsEnumField(modelType, treeField) ||
                    SystemOperate.GetFieldType(modelType, treeField) == typeof(String))
                {
                    //树标题为字段名称
                    treeTitle = SystemOperate.GetFieldDisplay(moduleId, treeField);
                    //树url
                    url = string.Format("/{0}/GetTreeByNode.html?moduleId={1}&fieldName={2}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString(), treeField);
                }
                else if (treeField.EndsWith("Id") && treeField.Length > 2) //从字段中判断模块
                {
                    string tableName = treeField.Substring(0, treeField.Length - 2);
                    Sys_Module treeModule = SystemOperate.GetModuleByTableName(tableName);
                    Type treeModelType = CommonOperate.GetModelType(treeModule.Id); //外键模块实体类型
                    if (treeModelType.GetProperty("ParentId") != null) //外键模块是树型模块
                    {
                        //只有一个根结点才以树型方式加载
                        long n = CommonOperate.Count(out errMsg, treeModule.Id, false, null, "ParentId IS NULL OR ParentId='" + Guid.Empty.ToString() + "'");
                        if (n == 1)
                        {
                            //外键字段的树标题为外键模块名称
                            treeTitle = treeModule.Name;
                            //树url
                            url = string.Format("/{0}/GetTreeByNode.html?moduleId={1}", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, treeModule.Id.ToString());
                        }
                    }
                }
                if (!string.IsNullOrEmpty(url)) //显示树
                {
                    treeSb.AppendFormat("<div id=\"region_west\" data-options=\"region:'west',title:'{0}',split:true\" style=\"width: {1}px;\">", treeTitle, ConstDefine.GRID_LEFT_MENU_WIDTH.ToString());
                    treeSb.AppendFormat("<ul id=\"gridTree\" treeField=\"{0}\" class=\"easyui-tree\" data-options=\"url:'{1}',onClick:GridTreeNodeClick,loadFilter:GridTreeLoadFilter,onLoadSuccess:GridTreeLoadSuccess\"></ul>", treeField, url);
                    treeSb.Append("</div>");
                    sb.Append(treeSb.ToString());
                }
            }
            #endregion
            #region 网格内容
            #region region处理
            string region = gridType == DataGridType.MainGrid || gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid ? "regon_main" : "regon_" + moduleId;
            string reginMainWstr = string.Empty;
            string gridDataOption = " data-options=\"region:'center',border:false\"";
            if (gridType == DataGridType.MainGrid)
            {
                if (isLoadAttachModule) //有附属模块
                {
                    int w = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - ConstDefine.SCROLL_WIDTH - 5;
                    if (isShowFullPath) w -= 200; //针对嵌入其他系统
                    if (!string.IsNullOrWhiteSpace(grid.TreeField) && !grid.IsTreeGrid) //左侧树存在
                    {
                        w -= ConstDefine.GRID_LEFT_MENU_WIDTH;
                    }
                    reginMainWstr = string.Format("width:{0}px", w.ToString());
                }
                else //没有附属模块
                {
                    if (!string.IsNullOrWhiteSpace(grid.TreeField) && !grid.IsTreeGrid) //左侧树存在
                    {
                        int w = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - ConstDefine.GRID_LEFT_MENU_WIDTH - 4;
                        reginMainWstr = string.Format("width:{0}px", w.ToString());
                    }
                }
            }
            if (gridType == DataGridType.DialogGrid || gridType == DataGridType.Other || gridType == DataGridType.FlowGrid)
            {
                gridDataOption = string.Empty;
                if (gridType == DataGridType.FlowGrid)
                {
                    int w = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - ConstDefine.SCROLL_WIDTH - 5;
                    if (isShowFullPath) w -= 200; //针对嵌入其他系统
                    if (isGridLeftTree) //左侧树存在
                        w -= ConstDefine.GRID_LEFT_MENU_WIDTH;
                    reginMainWstr = string.Format("width:{0}px", w.ToString());
                }
                else
                {
                    reginMainWstr = "width:100%;height:100%";
                }
            }
            string parentModuleName = module.ParentId.HasValue ? SystemOperate.GetModuleNameById(module.ParentId.Value) : string.Empty;
            string isflowFlag = isEnabledFlow ? "1" : string.Empty;
            sb.AppendFormat("<div id=\"{0}\" moduleId=\"{1}\" moduleName=\"{2}\" tableName=\"{3}\" titleKey=\"{4}\"{6} style=\"overflow-x:hidden;{5}\" parentModuleName=\"{7}\" isflow=\"{8}\">",
                            region, moduleId.ToString(), module.Name, module.TableName, module.TitleKey, reginMainWstr, gridDataOption, parentModuleName, isflowFlag);
            #endregion
            #region 网格参数
            bool isTreeGrid = false; //是否为树网格
            string tableClass = "easyui-datagrid";
            #region 树字段处理
            if (gridType == DataGridType.MainGrid || gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid)
            {
                //添加树网格所需参数
                if (grid.IsTreeGrid && !string.IsNullOrEmpty(grid.TreeField))
                {
                    Sys_GridField treeField = gridFields.Where(x => x.Sys_FieldName == grid.TreeField && x.IsVisible).FirstOrDefault();
                    isTreeGrid = treeField != null;
                    gridParams.IsTreeGrid = isTreeGrid;
                    gridParams.TreeField = grid.TreeField;
                }
                else if (!string.IsNullOrEmpty(grid.TreeField))
                {
                    gridParams.TreeField = grid.TreeField;
                }
            }
            #endregion
            #region URL拼装
            string singleSelect = module.IsMutiSelect && grid.MutiSelect.HasValue && grid.MutiSelect.Value ? "false" : "true";
            gridParams.IsMutiSelect = singleSelect == "false";
            //数据加载url
            string gridUrl = string.Format("/DataAsync/LoadGridData.html?moduleId={0}", moduleId.ToString());
            gridUrl += "&tgt=" + ((int)gridType).ToString();
            string whereParams = string.Empty;
            if (!string.IsNullOrWhiteSpace(where))
            {
                whereParams = string.Format("&where={0}", HttpUtility.UrlEncode(MySecurity.EncodeBase64(where)));
                gridUrl += whereParams;
            }
            string otherGridUrl = string.Empty; //其他参数
            if (grid.GridTypeOfEnum == GridTypeEnum.Comprehensive || grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
            {
                otherGridUrl += "&viewId=" + grid.Id.ToString();
                if (grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
                    otherGridUrl += "&dv=1"; //综合明细视图标识
            }
            else
            {
                otherGridUrl += "&gvId=" + grid.Id.ToString();
            }
            if (!string.IsNullOrEmpty(otherGridUrl))
                gridUrl += otherGridUrl;
            if (!string.IsNullOrWhiteSpace(condition))
            {
                gridUrl += string.Format("&condition={0}", HttpUtility.UrlEncode(condition));
            }
            if (!string.IsNullOrEmpty(initModule) && !string.IsNullOrEmpty(initField))
            {
                gridUrl += string.Format("&initModule={0}&initField={1}", HttpUtility.UrlEncode(initModule), initField);
            }
            if (request != null && request["rsf"].ObjToInt() == 1) //重新发起流程标识
            {
                gridUrl += "&rsf=1";
            }
            if (request != null)
            {
                if (request[PageInfo.sortFieldKeyWord].ObjToStr() != string.Empty)
                    gridUrl += string.Format("&{0}={1}", PageInfo.sortFieldKeyWord, request[PageInfo.sortFieldKeyWord].ObjToStr());
                if (request[PageInfo.sortOrderKeyWord].ObjToStr() != string.Empty)
                    gridUrl += string.Format("&{0}={1}", PageInfo.sortOrderKeyWord, request[PageInfo.sortOrderKeyWord].ObjToStr());
            }
            if (otherParams != null && otherParams.Count > 0) //添加网格参数
            {
                if (otherParams.ContainsKey("muti_select") && otherParams["muti_select"].ObjToBool())
                {
                    singleSelect = "false";
                    gridParams.IsMutiSelect = true;
                }
                foreach (string key in otherParams.Keys)
                {
                    if (!key.StartsWith("p_")) continue;
                    gridUrl += string.Format("&{0}={1}", key, otherParams[key].ObjToStr());
                }
            }
            gridParams.DataOrUrl = gridUrl;
            #endregion
            #region 分页设置
            string pageInfo = "pageSize:15,pageList:[15,30,50,100]";
            string paging = "true"; //是否分页
            if (gridType == DataGridType.DialogGrid || gridType == DataGridType.FlowGrid) //弹出框或主网格下方网格
            {
                pageInfo = "pageSize:10,pageList:[10,30,50,100]";
                gridParams.PageSize = 10;
                gridParams.PageList = "[10,30,50,100]";
            }
            else if (gridType == DataGridType.InnerDetailGrid) //网格内嵌入网格
            {
                pageInfo = "pageSize:5,pageList:[5]";
                gridParams.PageSize = 5;
                gridParams.PageList = "[5]";
            }
            else if (gridType == DataGridType.EditDetailGrid) //编辑页面网格
            {
                paging = "false";
                singleSelect = "false";
                if (detailCopy) //明细复制
                {
                    gridUrl += "&copy=1";
                    gridParams.DataOrUrl += "&copy=1";
                }
                gridParams.PageList = string.Empty;
                gridParams.PageSize = 100;
                gridParams.IsPaging = false;
                gridParams.IsMutiSelect = true;
            }
            else if (gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid)
            {
                singleSelect = "false";
                gridParams.IsMutiSelect = true;
            }
            #endregion
            #region 网格参数重置
            CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "GridParamsSet", new object[] { gridType, gridParams, request });
            if (gridParams != null)
            {
                if (gridParams.GridFields != null && gridParams.GridFields.Count > 0)
                {
                    gridFields = gridParams.GridFields;
                }
                groupField = gridParams.GroupField;
                isTreeGrid = gridParams.IsTreeGrid && !string.IsNullOrWhiteSpace(gridParams.TreeField);
                if (!string.IsNullOrEmpty(gridParams.DataOrUrl))
                {
                    gridUrl = gridParams.DataOrUrl;
                }
                paging = gridParams.IsPaging.ToString().ToLower();
                pageInfo = string.Format("pageSize:{0}", gridParams.PageSize.ToString());
                if (!string.IsNullOrEmpty(gridParams.PageList))
                    pageInfo += string.Format(",pageList:{0}", gridParams.PageList);
                singleSelect = (!gridParams.IsMutiSelect).ToString().ToLower();
            }
            #endregion
            #region 其他参数
            string gridData = string.Empty; //网格JSON数据
            if (gridUrl.StartsWith("[{") && gridUrl.EndsWith("}]"))
            {
                gridData = gridUrl;
                gridUrl = string.Empty;
            }
            //网格Id
            string gridId = gridType == DataGridType.MainGrid || gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid ? "mainGrid" : "grid_" + moduleId;
            //网格参数 nowrap:为false时网格内容自动换行
            string gridOptions = string.Format("url:'{0}',pagination:{1},idField:'Id',rownumbers:{5},{2},collapsible:true,fitColumns:false,selectOnCheck:true,checkOnSelect:true,singleSelect:{4},toolbar:'#grid_toolbar_{3}',method:'get',multiSort:{6}",
                gridUrl, paging, pageInfo, moduleId.ToString(), singleSelect, "true", gridFields.Where(x => x.Sys_FieldName != "Id" && x.IsAllowSort).ToList().Count > 1 ? "true" : "false");
            if (!string.IsNullOrEmpty(gridData))
            {
                gridOptions += ",data:" + gridData + "";
            }
            if (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit) //网格行展开表单编辑模式
            {
                isDetailView = true;
                gridOptions += ",view:detailview,detailFormatter:function(index,row){return GridFormDeailFormatter('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "',index,row);}";
            }
            else if (groupField != null && !isTreeGrid) //分组模式
            {
                string tempGroupFieldName = groupField.Sys_FieldName;
                bool gfIsForein = SystemOperate.IsForeignNameField(module.Id, tempGroupFieldName);
                if (gfIsForein) //分组字段为外键字段
                    tempGroupFieldName = tempGroupFieldName.Substring(0, tempGroupFieldName.Length - 4) + "Id";
                gridOptions += ",view:groupview,groupField:'" + tempGroupFieldName + "',groupFormatter:function(value, rows){if(typeof(OverGroupFormatter)=='function'){ return OverGroupFormatter(value, rows,'" + tempGroupFieldName + "');} else {return " + (gfIsForein ? string.Format("rows[0]['{0}']", groupField.Sys_FieldName) : "value") + "+ '(' + rows.length + ')';}}";
            }
            else if (gridType == DataGridType.MainGrid && (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit || (module.DetailInGrid && SystemOperate.HasDetailModule(moduleId)) || SystemOperate.HasUserAttachModule(currUser.UserId, moduleId, true)))
            {
                isDetailView = true;
                gridOptions += ",view:detailview,detailFormatter:function(index,row){return '';}";
            }
            else if (editMode == (int)ModuleEditModeEnum.GridRowEdit && (gridType == DataGridType.EditDetailGrid || gridType == DataGridType.ViewDetailGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.InnerDetailGrid))
            {
                List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id);
                if (form.FormFields == null && formFields.Count > 0)
                {
                    form.FormFields = formFields;
                }
                if (formFields.Select(x => x.RowEditRowNo).Where(x => x >= 0).Distinct().Count() > 1)
                {
                    isDetailView = true;
                    gridOptions += ",view:detailview,detailFormatter:function(index,row){return DetailGridRowFormatter('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "',index,row);}";
                }
            }
            if (isTreeGrid && !isDetailView)
            {
                otherGridUrl += "&tg=1";
                tableClass = "easyui-treegrid";
            }
            //表格样式
            string tableStyle = string.Empty;
            int gh = 0;
            if (gridType == DataGridType.MainGrid || gridType == DataGridType.RecycleGrid || gridType == DataGridType.MyDraftGrid)
            {
                int h = currUser.ClientBrowserHeight - ConstDefine.TOP_NORTH_REGION_HEIGHT - ConstDefine.TAB_HEAD_HEIGHT - ConstDefine.BOTTOM_STATUS_REGON_HEIGHT - ConstDefine.GRID_FIX_HEIGHT;
                tableStyle = "style=\"";
                if (gridType == DataGridType.MainGrid && isLoadAttachModule)
                {
                    tableStyle += string.Format("height:{0}px;min-height:200px;", h.ToString());
                    int w = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - ConstDefine.SCROLL_WIDTH - 4; //屏幕宽度减去滚动条宽
                    if (isShowFullPath) w -= 200; //针对嵌入其他系统
                    if (!string.IsNullOrWhiteSpace(grid.TreeField) && !grid.IsTreeGrid) //左侧树存在
                        w -= ConstDefine.GRID_LEFT_MENU_WIDTH;
                    tableStyle += string.Format("width:{0}px;\"", w.ToString());
                }
                else
                {
                    tableStyle += isShowFullPath ? "height:100%;" : string.Format("height:{0}px;", h.ToString());
                    tableStyle += "width:100%;\"";
                }
                gridOptions += ",border:false";
                //添加树网格所需参数
                if (isTreeGrid && !isDetailView)
                {
                    gridOptions += string.Format(",treeField:'{0}'", gridParams.TreeField);
                }
                gh = h;
            }
            else if (gridType == DataGridType.DialogGrid || gridType == DataGridType.FlowGrid)
            {
                if (gridType == DataGridType.FlowGrid)
                {
                    int w = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - ConstDefine.SCROLL_WIDTH - 4; //屏幕宽度减去滚动条宽
                    if (isShowFullPath) w -= 200; //针对嵌入其他系统
                    if (isGridLeftTree) //左侧树存在
                        w -= ConstDefine.GRID_LEFT_MENU_WIDTH;
                    tableStyle = string.Format("style=\"width:{0}px;min-height:150px;\"", w.ToString());
                }
                else
                {
                    tableStyle = "style=\"height:356px;\"";
                }
                gridOptions += ",border:false";
            }
            else if (gridType == DataGridType.InnerDetailGrid)
            {
                tableStyle = "style=\"height:240px;\"";
                gridOptions += ",border:false";
            }
            else if (gridType == DataGridType.Other || gridType == DataGridType.DesktopGrid)
            {
                tableStyle = "style=\"height:100%;\"";
                gridOptions += ",border:false";
            }
            //其他重写参数
            if (gridParams != null && !string.IsNullOrEmpty(gridParams.OtherParmas))
            {
                gridOptions += string.Format(",{0}", gridParams.OtherParmas);
            }
            #endregion
            #endregion
            //添加网格事件
            #region 网格事件
            //行样式设置
            gridOptions += ",rowStyler:function(index,row){if(typeof(OverRowStyler)=='function'){return OverRowStyler(index,row,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //数据过滤
            gridOptions += ",loadFilter:function(data){if(typeof(OnGridLoadFilter)=='function'){return OnGridLoadFilter(data,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');} else { " + (isTreeGrid && !isDetailView ? "return ToTreeData(data.rows, 'Id', 'ParentId', 'children');" : "return data;") + "}}";
            //列头右键菜单
            gridOptions += ",onHeaderContextMenu:function (e, field) {CreateColumnContextMenu(e,field,'colmenu_" + moduleId.ToString() + "','" + gridId + "');}";
            //行右键菜单
            gridOptions += ",onRowContextMenu:function(e, rowIndex, rowData){if(typeof(OverOnRowContextMenu)=='function'){OverOnRowContextMenu(e, rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //加载成功事件
            gridOptions += ",onLoadSuccess:function(data){OnLoadSuccess(data,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}";
            //在载入远程数据产生错误的时候触发
            gridOptions += ",onLoadError:function(){if(typeof(OverOnLoadError)=='function'){OverOnLoadError('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在载入请求数据数据之前触发，如果返回false可终止载入数据操作
            gridOptions += ",onBeforeLoad:function(param){if(typeof(OverOnBeforeLoad)=='function'){return OverOnBeforeLoad(param,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');} else{return true;}}";
            //在用户点击一行的时候触发
            gridOptions += ",onClickRow:function(rowIndex, rowData){if(typeof(OverOnClickRow)=='function'){OverOnClickRow(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户双击一行的时候触发
            gridOptions += ",onDblClickRow:function(rowIndex, rowData){if(typeof(OverOnDblClickRow)=='function'){OverOnDblClickRow(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户点击一个单元格的时候触发
            gridOptions += ",onClickCell:function(rowIndex, field, value){if(typeof(OverOnClickCell)=='function'){OverOnClickCell(rowIndex, field, value,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户双击一个单元格的时候触发
            gridOptions += ",onDblClickCell:function(rowIndex, field, value){if(typeof(OverOnDblClickCell)=='function'){OverOnDblClickCell(rowIndex, field, value,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户排序一个列之前触发，返回false可以取消排序
            gridOptions += ",onBeforeSortColumn:function(sort, order){if(typeof(OverOnBeforeSortColumn)=='function'){return OverOnBeforeSortColumn(sort, order,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');} else{return true;}}";
            //在用户排序一列的时候触发
            gridOptions += ",onSortColumn:function(sort, order){if(typeof(OverOnSortColumn)=='function'){OverOnSortColumn(sort, order,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户调整列大小的时候触发
            gridOptions += ",onResizeColumn:function(field, width){if(typeof(OverOnResizeColumn)=='function'){OverOnResizeColumn(field, width,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户选择一行的时候触发
            gridOptions += ",onSelect:function(rowIndex, rowData){if(typeof(OnSelect)=='function'){OnSelect(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户取消选择一行的时候触发
            gridOptions += ",onUnselect:function(rowIndex, rowData){if(typeof(OverOnUnselect)=='function'){OverOnUnselect(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户选择所有行的时候触发
            gridOptions += ",onSelectAll:function(rows){if(typeof(OverOnSelectAll)=='function'){OverOnSelectAll(rows,'" + module.Name + "');}}";
            //在用户取消选择所有行的时候触发
            gridOptions += ",onUnselectAll:function(rows){if(typeof(OverOnUnselectAll)=='function'){OverOnUnselectAll(rows,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户勾选一行的时候触发
            gridOptions += ",onCheck:function(rowIndex,rowData){if(typeof(OverOnCheck)=='function'){OverOnCheck(rowIndex,rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户取消勾选一行的时候触发
            gridOptions += ",onUncheck:function(rowIndex,rowData){if(typeof(OverOnUncheck)=='function'){OverOnUncheck(rowIndex,rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户勾选所有行的时候触发
            gridOptions += ",onCheckAll:function(rows){if(typeof(OverOnCheckAll)=='function'){OverOnCheckAll(rows,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户取消勾选所有行的时候触发
            gridOptions += ",onUncheckAll:function(rows){if(typeof(OverOnUncheckAll)=='function'){OverOnUncheckAll(rows,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户开始编辑一行的时候触发
            gridOptions += ",onBeforeEdit:function(rowIndex, rowData){if(typeof(OverOnBeforeEdit)=='function'){OverOnBeforeEdit(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在一行进入编辑模式的时候触发
            gridOptions += ",onBeginEdit:function(rowIndex, rowData){if(typeof(OverOnBeginEdit)=='function'){OverOnBeginEdit(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在完成编辑但编辑器还没有销毁之前触发
            gridOptions += ",onEndEdit:function(rowIndex, rowData, changes){if(typeof(OverOnEndEdit)=='function'){OverOnEndEdit(rowIndex, rowData, changes,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户完成编辑一行的时候触发
            gridOptions += ",onAfterEdit:function(rowIndex, rowData, changes){if(typeof(OverOnAfterEdit)=='function'){OverOnAfterEdit(rowIndex, rowData, changes,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            //在用户取消编辑一行的时候触发
            gridOptions += ",onCancelEdit:function(rowIndex, rowData){if(typeof(OverOnCancelEdit)=='function'){OverOnCancelEdit(rowIndex, rowData,'" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "');}}";
            if (isDetailView) //明细视图模式 行展开事件
            {
                if (gridType == DataGridType.MainGrid)
                {
                    string overExpandRowFun = string.Empty; //展开行事件函数名称
                    if (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit) //网格行展开表单编辑模式
                    {
                        string optionJson = "{titleKey:'" + titleKey + "',titleKeyDisplay:'" + titleKeyDisplay + "',editWidth:" + formWidth.ToString() + ",editHeight:" + formHeight.ToString() + "}";
                        overExpandRowFun = "ExpandGridRowForm('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "',row,index," + optionJson + ")";
                    }
                    else
                    {
                        overExpandRowFun = "ExpandGridRow('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "',row,index)";
                    }
                    gridOptions += ",onExpandRow: function(index,row){" + overExpandRowFun + ";}";
                }
                else if (gridType == DataGridType.EditDetailGrid || gridType == DataGridType.ViewDetailGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.InnerDetailGrid)
                {
                    List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id);
                    formFields = formFields.Where(x => x.RowEditRowNo > 0).ToList();
                    formFields.ForEach(x =>
                    {
                        if (x.Sys_FieldId.HasValue)
                        {
                            Sys_Field sysField = x.TempSysField != null ? x.TempSysField : SystemOperate.GetFieldById(x.Sys_FieldId.Value);
                            if (sysField != null)
                            {
                                if (x.TempSysField == null) x.TempSysField = sysField;
                                Sys_GridField field = SystemOperate.GetDefaultGridField(sysField);
                                string editorStr = string.Empty;
                                string formatter = GetGridFieldFormatter(module, field, gridType, gridId, editMode, string.Empty, out editorStr, currUser.UserId);
                                x.FieldFormatter = formatter;
                                x.EditorFormatter = editorStr.Replace(",editor:", string.Empty);
                            }
                        }
                    });
                    var dic = formFields.GroupBy(x => x.RowEditRowNo).ToDictionary(x => x.Key, y => y.Select(o => new { field = o.Sys_FieldName, title = o.Display, ControlType = o.ControlType, width = o.Width, formatter = o.FieldFormatter, editor = o.EditorFormatter }));
                    string dicJson = HttpUtility.UrlEncode(JsonHelper.Serialize(dic).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
                    int miniW = 600;
                    int maxW = currUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - 120;
                    string overExpandRowFun = "DetailExpandGridRow('" + gridId + "','" + moduleId.ToString() + "','" + module.Name + "',row,index,'" + dicJson + "'," + miniW.ToString() + "," + maxW.ToString() + ")";
                    gridOptions += ",onExpandRow: function(index,row){" + overExpandRowFun + ";}";
                }
            }
            #endregion
            string baseUrl = gridUrl;
            if (!string.IsNullOrEmpty(whereParams))
                baseUrl = baseUrl.Replace(whereParams, string.Empty);
            if (!string.IsNullOrEmpty(otherGridUrl))
                baseUrl = baseUrl.Replace(otherGridUrl, string.Empty);
            //不允许隐藏字段设置
            string noHideFieldsStr = string.Empty;
            List<string> noHideFields = gridFields.Where(x => !x.IsAllowHide).Select(x => x.Sys_FieldName).ToList();
            if (noHideFields != null)
                noHideFieldsStr = string.Join(",", noHideFields);
            string foreignNameFieldsStr = string.Join(",", gridFields.Where(x => SystemOperate.IsForeignNameField(moduleId, x.Sys_FieldName)).Select(x => x.Sys_FieldName).ToList());
            sb.AppendFormat("<table id=\"{0}\" class=\"{3}\" baseUrl=\"{4}\" noHideFields=\"{5}\" h=\"{6}\" editMode=\"{7}\" foreignNameFields=\"{8}\" data-options=\"{1}\" {2}>", gridId, gridOptions, tableStyle, tableClass, baseUrl, noHideFieldsStr, gh.ToString(), editMode.ToString(), foreignNameFieldsStr);
            #region 网格字段
            //行操作按钮
            List<Sys_GridButton> gridOperateBtns = gridButtons.Where(x => (x.ParentId == null || x.ParentId == Guid.Empty) && x.GridButtonLocation == (int)GridButtonLocationEnum.RowHead).ToList();
            //冻结字段
            List<Sys_GridField> lockFields = gridFields.Where(x => x.IsFrozen && x.Sys_FieldName != "Id").ToList();
            sb.Append("<thead data-options=\"frozen:true\">");
            sb.Append("<tr>");
            if (grid.ShowCheckBox == true)
            {
                sb.Append("<th data-options=\"title:'ID',field:'Id',checkbox:true\">ID</th>");
            }
            if ((gridType == DataGridType.MainGrid || gridType == DataGridType.EditDetailGrid ||
                gridType == DataGridType.FlowGrid || gridType == DataGridType.ViewDetailGrid ||
                gridType == DataGridType.MyDraftGrid) && gridOperateBtns.Count > 0) //有行操作按钮
            {
                int colBtnWidth = gridOperateBtns.Count > 1 ? 35 * gridOperateBtns.Count : 60;
                //行操作按钮JSON
                string operateBtnsJson = HttpUtility.UrlEncode(JsonHelper.Serialize(gridOperateBtns)).Replace("+", "%20");
                sb.Append("<th data-options=\"title:'操作',field:'RowOperateBtn',width:" + colBtnWidth.ToString() + ",align:'center',formatter:function(value, row, index){return RowOperateBtnFormat(value, row, index,'" + moduleId.ToString() + "','" + module.Name + "');}\">操作<input id=\"txtRowOperateBtn_" + moduleId.ToString() + "\" type=\"hidden\" titleKey=\"" + titleKey + "\" titleKeyDisplay=\"" + titleKeyDisplay + "\" editMode=\"" + editMode.ToString() + "\" editWidth=\"" + formWidth.ToString() + "\" editHeight=\"" + formHeight.ToString() + "\" value=\"" + operateBtnsJson + "\"></th>");
            }
            if (isEnabledFlow && (gridType == DataGridType.MainGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.ViewDetailGrid)) //添加流程图标列
            {
                string mIdStr = moduleId.ToString();
                string mNameStr = module.Name;
                if (gridType == DataGridType.MainGrid && grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
                {
                    Sys_Module detailModule = SystemOperate.GetDetailModules(moduleId).FirstOrDefault();
                    mIdStr = detailModule.Id.ToString();
                    mNameStr = detailModule.Name;
                }
                sb.Append("<th data-options=\"title:'',field:'flow_" + mIdStr + "',width:30,align:'center',formatter:function(value, row, index){return FlowIconFormatter(value, row, index,'" + mIdStr + "','" + mNameStr + "');}\"></th>");
                sb.Append("<th data-options=\"title:'状态',field:'FlowStatus',width:50,align:'center',formatter:function(value, row, index){return FlowStatusFormatter(value, row, index,'" + mIdStr + "','" + mNameStr + "');}\"></th>");
            }
            bool isAllowEditField = gridType == DataGridType.MainGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.InnerDetailGrid || gridType == DataGridType.ViewDetailGrid;
            //其他格式化参数
            string otherFormatParams = gridType != DataGridType.EditDetailGrid ? HttpUtility.UrlEncode("{recycle:" + (gridType == DataGridType.RecycleGrid ? "1" : "0") + ",moduleId:'" + module.Id.ToString() + "',moduleDisplay:'" + (string.IsNullOrEmpty(module.Display) ? module.Name : module.Display) + "',titleKey:'" + titleKey + "',titleKeyDisplay:'" + titleKeyDisplay + "',editMode:" + editMode.ToString() + ",editWidth:" + formWidth.ToString() + ",editHeight:" + formHeight.ToString() + ",gridId:'" + gridId + "'}", Encoding.UTF8).Replace("+", "%20") : string.Empty;
            if (lockFields.Count > 0)
            {
                foreach (Sys_GridField field in lockFields)
                {
                    string editorStr = string.Empty;
                    string formatStr = GetGridFieldFormatter(module, field, gridType, gridId, editMode, otherFormatParams, out editorStr, currUser.UserId);
                    int fieldWidth = field.Width.HasValue ? field.Width.Value : 120;
                    sb.AppendFormat("<th data-options=\"title:'{0}',field:'{1}',width:{2},align:'{3}',hidden:{4},formatter:{5},sortable:{6}{7}\">{0}</th>",
                        field.Display, field.Sys_FieldName, fieldWidth.ToString(), field.Align.ToString(), (!field.IsVisible).ToString().ToLower(),
                        string.IsNullOrEmpty(formatStr) ? "''" : formatStr, field.IsAllowSort.ToString().ToLower(), editorStr);
                }
            }
            sb.Append("</tr>");
            sb.Append("</thead>");
            //非冻结字段
            List<Sys_GridField> noLockFields = gridFields.Where(x => !x.IsFrozen && x.Sys_FieldName != "Id").ToList();
            if (noLockFields.Count > 0)
            {
                sb.Append("<thead>");
                sb.Append("<tr>");
                foreach (Sys_GridField field in noLockFields)
                {
                    string editorStr = string.Empty;
                    string formatStr = GetGridFieldFormatter(module, field, gridType, gridId, editMode, otherFormatParams, out editorStr, currUser.UserId);
                    int fieldWidth = field.Width.HasValue ? field.Width.Value : 120;
                    sb.AppendFormat("<th data-options=\"title:'{0}',field:'{1}',width:{2},align:'{3}',hidden:{4},formatter:{5},sortable:{6}{7}\">{0}</th>",
                        field.Display, field.Sys_FieldName, fieldWidth.ToString(), field.Align.ToString(), (!field.IsVisible).ToString().ToLower(),
                        string.IsNullOrEmpty(formatStr) ? "''" : formatStr, field.IsAllowSort.ToString().ToLower(), editorStr);
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
            }
            #endregion
            sb.Append("</table>");
            //网格工具栏和搜索框
            sb.AppendFormat("<div id=\"grid_toolbar_{0}\" class=\"toolbar datagrid-toolbar\" style=\"height:{1}px;\">", moduleId.ToString(), ConstDefine.GRID_TOOLBAR_HEIGHT.ToString());
            StringBuilder mm_html = new StringBuilder(); //文件菜单按钮的下拉按钮html
            #region 工具栏
            bool isContainsDelBtn = false; //是否包含删除按钮
            //工具栏
            sb.AppendFormat("<div name=\"btns\" style=\"margin-left:10px;margin-right:30px;height:{0}px;line-height:{0}px;margin-top:{1}px;float:left;\">", ConstDefine.GRID_TOOLBAR_HEIGHT.ToString(), (ConstDefine.GRID_TOOLBAR_HEIGHT - 26) / 2);
            if (gridType != DataGridType.DialogGrid && gridType != DataGridType.Other && gridType != DataGridType.InnerDetailGrid && gridType != DataGridType.RecycleGrid && gridType != DataGridType.MyDraftGrid)
            {
                List<Sys_GridButton> topButtons = gridButtons.Where(x => x.ParentId == null || x.ParentId == Guid.Empty).ToList();
                int no = 0;
                foreach (Sys_GridButton button in topButtons)
                {
                    if (button.GridButtonLocation != (int)GridButtonLocationEnum.Toolbar)
                        continue;
                    if (gridType == DataGridType.EditDetailGrid && editMode == (int)ModuleEditModeEnum.GridRowEdit && button.ClickMethod.Contains("ViewRecord"))
                        continue;
                    if (gridType == DataGridType.EditDetailGrid)
                    {
                        if (button.ClickMethod == "PrintModel(this)" || button.ClickMethod == "BatchEdit(this)")
                            continue;
                        if (otherParams != null && otherParams.ContainsKey("p_todoId") && button.ClickMethod == "Delete(this)") //子流程审批主从表单时不允许删除
                            continue;
                    }
                    if (!module.IsAllowAdd && button.ClickMethod == "Add(this)") continue;
                    if (!module.IsAllowEdit && button.ClickMethod == "Edit(this)") continue;
                    if ((!module.IsAllowCopy || !module.IsAllowAdd) && button.ClickMethod == "Copy(this)") continue;
                    if (!module.IsAllowDelete && button.ClickMethod == "Delete(this)") continue;
                    if ((!module.IsAllowAdd || !module.IsAllowEdit || !module.IsAllowImport) && button.ClickMethod == "ImportModel(this)") continue;
                    if (!module.IsAllowExport && button.ClickMethod == "ExportModel(this)") continue;
                    if ((!module.IsAllowEdit || !module.IsEnabledBatchEdit) && button.ClickMethod == "BatchEdit(this)") continue;
                    if (!module.IsEnabledPrint && button.ClickMethod == "PrintModel(this)") continue;
                    no++;
                    if (button.ClickMethod == "Delete(this)")
                    {
                        isContainsDelBtn = true;
                    }
                    string icon = "eu-p2-icon-add_other";
                    if (!string.IsNullOrEmpty(button.ButtonIcon)) icon = button.ButtonIcon;
                    string btnClass = "easyui-linkbutton";
                    string mm = string.Empty;
                    #region 文件菜单按钮处理
                    if (button.ButtonTagId != "btnStartFlow" && SystemOperate.IsFileMenuButton(moduleId, button.Id)) //是文件菜单按钮
                    {
                        btnClass = "easyui-menubutton";
                        mm = string.Format(",menu:'#mm_btn_{0}'", button.Id.ToString());
                        List<Sys_GridButton> childBtns = gridButtons.Where(x => x.ParentId == button.Id).ToList();
                        mm_html.AppendFormat("<div id=\"mm_btn_{0}\" style=\"width:70px;\">", button.Id.ToString());
                        foreach (Sys_GridButton btn in childBtns)
                        {
                            if (btn.GridButtonLocation != (int)GridButtonLocationEnum.Toolbar)
                                continue;
                            if (gridType == DataGridType.EditDetailGrid && editMode == (int)ModuleEditModeEnum.GridRowEdit && btn.ClickMethod.Contains("ViewRecord"))
                                continue;
                            if (gridType == DataGridType.EditDetailGrid)
                            {
                                if (btn.ClickMethod == "PrintModel(this)" || btn.ClickMethod == "BatchEdit(this)")
                                    continue;
                                if (otherParams != null && otherParams.ContainsKey("p_todoId") && btn.ClickMethod == "Delete(this)") //子流程审批主从表单时不允许删除
                                    continue;
                            }
                            if (!module.IsAllowAdd && btn.ClickMethod == "Add(this)") continue;
                            if (!module.IsAllowEdit && btn.ClickMethod == "Edit(this)") continue;
                            if ((!module.IsAllowCopy || !module.IsAllowAdd) && btn.ClickMethod == "Copy(this)") continue;
                            if (!module.IsAllowDelete && btn.ClickMethod == "Delete(this)") continue;
                            if ((!module.IsAllowAdd || !module.IsAllowEdit || !module.IsAllowImport) && btn.ClickMethod == "ImportModel(this)") continue;
                            if (!module.IsAllowExport && btn.ClickMethod == "ExportModel(this)") continue;
                            if ((!module.IsAllowEdit || !module.IsEnabledBatchEdit) && btn.ClickMethod == "BatchEdit(this)") continue;
                            if (!module.IsEnabledPrint && btn.ClickMethod == "PrintModel(this)") continue;
                            if (btn.ClickMethod == "Delete(this)")
                            {
                                isContainsDelBtn = true;
                            }
                            var childs = gridButtons.Where(x => x.ParentId == btn.Id).ToList();
                            string tempBtnAttr = GetGridButtonAttr(module, titleKeyDisplay, editMode, formWidth, formHeight, gridId, gridType, btn, customerFormUrl);
                            if (childs.Count > 0) //存在子按钮
                            {
                                mm_html.AppendFormat("<div onclick=\"{0}\" iconCls=\"{2}\" {3}><span>{1}</span><div>", btn.ClickMethod, btn.ButtonText, btn.ButtonIcon, tempBtnAttr);
                                foreach (Sys_GridButton b in childs)
                                {
                                    if (b.GridButtonLocation != (int)GridButtonLocationEnum.Toolbar)
                                        continue;
                                    if (gridType == DataGridType.EditDetailGrid && editMode == (int)ModuleEditModeEnum.GridRowEdit && b.ClickMethod.Contains("ViewRecord"))
                                        continue;
                                    if (gridType == DataGridType.EditDetailGrid)
                                    {
                                        if (b.ClickMethod == "PrintModel(this)" || b.ClickMethod == "BatchEdit(this)")
                                            continue;
                                        if (otherParams != null && otherParams.ContainsKey("p_todoId") && b.ClickMethod == "Delete(this)") //子流程审批主从表单时不允许删除
                                            continue;
                                    }
                                    if (!module.IsAllowAdd && b.ClickMethod == "Add(this)") continue;
                                    if (!module.IsAllowEdit && b.ClickMethod == "Edit(this)") continue;
                                    if ((!module.IsAllowCopy || !module.IsAllowAdd) && b.ClickMethod == "Copy(this)") continue;
                                    if (!module.IsAllowDelete && b.ClickMethod == "Delete(this)") continue;
                                    if ((!module.IsAllowAdd || !module.IsAllowEdit || !module.IsAllowImport) && b.ClickMethod == "ImportModel(this)") continue;
                                    if (!module.IsAllowExport && b.ClickMethod == "ExportModel(this)") continue;
                                    if ((!module.IsAllowEdit || !module.IsEnabledBatchEdit) && b.ClickMethod == "BatchEdit(this)") continue;
                                    if (!module.IsEnabledPrint && b.ClickMethod == "PrintModel(this)") continue;
                                    if (b.ClickMethod == "Delete(this)")
                                    {
                                        isContainsDelBtn = true;
                                    }
                                    string bId = string.IsNullOrWhiteSpace(b.ButtonTagId) ? "btn_" + b.Id.ToString() : b.ButtonTagId;
                                    string tempAttr = GetGridButtonAttr(module, titleKeyDisplay, editMode, formWidth, formHeight, gridId, gridType, b, customerFormUrl);
                                    mm_html.AppendFormat("<div id=\"{0}\" data-options=\"iconCls:'{1}'\" onclick=\"{3}\" {4}>{2}</div>",
                                        bId, b.ButtonIcon, b.ButtonText, b.ClickMethod, tempAttr);
                                    if (b.AfterSeparator) //后接分隔符
                                    {
                                        mm_html.Append("<div class=\"menu-sep\"></div>");
                                    }
                                }
                                mm_html.Append("</div></div>");
                            }
                            else //不存在子按钮
                            {
                                string btnIdStr = string.IsNullOrWhiteSpace(btn.ButtonTagId) ? "btn_" + btn.Id.ToString() : btn.ButtonTagId;
                                mm_html.AppendFormat("<div id=\"{0}\" data-options=\"iconCls:'{1}'\" onclick=\"{3}\" {4}>{2}</div>",
                                    btnIdStr, btn.ButtonIcon, btn.ButtonText, btn.ClickMethod, tempBtnAttr);
                            }
                            if (btn.AfterSeparator) //后接分隔符
                            {
                                mm_html.Append("<div class=\"menu-sep\"></div>");
                            }
                        }
                        mm_html.Append("</div>");
                    }
                    #endregion
                    string btnAttr = GetGridButtonAttr(module, titleKeyDisplay, editMode, formWidth, formHeight, gridId, gridType, button, customerFormUrl);
                    if (GlobalSet.IsShowStyleBtn) //是否显示样式按钮
                        btnClass += string.Format(" c{0}", (no % 9).ToString());
                    sb.AppendFormat("<a id=\"{0}\" href=\"#\" style=\"float:left;margin-left:5px;\" class=\"{1}\" data-options=\"plain:true,iconCls:'{2}'{6}\" onclick=\"{3}\" {5}>{4}</a>",
                        button.ButtonTagId, btnClass, icon, button.ClickMethod, button.ButtonText, btnAttr, mm);
                }
            }
            else if (gridType == DataGridType.RecycleGrid) //回收站列表
            {
                string btnAttr = string.Format("moduleId=\"{0}\" moduleName=\"{1}\" titleKey=\"{2}\" titleKeyDisplay=\"{3}\" editMode=\"{4}\" editWidth=\"{5}\" editHeight=\"{6}\" gridId=\"{7}\"",
                        module.Id.ToString(), module.Name, titleKey, titleKeyDisplay, editMode.ToString(), formWidth.ToString(), formHeight.ToString(), gridId);
                //添加还原、删除和查看按钮
                sb.AppendFormat("<a id=\"btn_restore\" style=\"float:left;\" href=\"#\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-redo\" plain=\"true\" onclick=\"Restore(this)\" {2}>还原</a>", moduleId.ToString(), module.Name, btnAttr);
                sb.AppendFormat("<a id=\"btn_del\" style=\"float:left;\" href=\"#\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-delete2\" plain=\"true\" onclick=\"Delete(this)\" recycle=1 {2}>删除</a>", moduleId.ToString(), module.Name, btnAttr);
            }
            else if (gridType == DataGridType.MyDraftGrid) //我的草稿列表
            {
                string btnAttr = string.Format("moduleId=\"{0}\" moduleName=\"{1}\" titleKey=\"{2}\" titleKeyDisplay=\"{3}\" editMode=\"{4}\" editWidth=\"{5}\" editHeight=\"{6}\" gridId=\"{7}\"",
                        module.Id.ToString(), module.Name, titleKey, titleKeyDisplay, editMode.ToString(), formWidth.ToString(), formHeight.ToString(), gridId);
                //草稿可发布、删除
                sb.AppendFormat("<a id=\"btn_release\" style=\"float:left;\" href=\"#\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-ok\" plain=\"true\" onclick=\"Release(this)\" {2}>发布</a>", moduleId.ToString(), module.Name, btnAttr);
                sb.AppendFormat("<a id=\"btn_del\" style=\"float:left;\" href=\"#\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-delete2\" plain=\"true\" onclick=\"Delete(this)\" isHardDel=1 {2}>删除</a>", moduleId.ToString(), module.Name, btnAttr);
            }
            sb.Append("</div>");
            #endregion
            #region 搜索框和视图
            if (gridType != DataGridType.EditDetailGrid)
            {
                //搜索框和视图
                List<Sys_GridField> searchFields = gridFields.Where(x => x.IsVisible && x.IsAllowSearch).ToList(); //搜索字段
                if (!string.IsNullOrEmpty(titleKey) && !searchFields.Select(x => x.Sys_FieldName).Contains(titleKey)) //默认添加titlekey字段
                {
                    searchFields.Insert(0, gridFields.Where(x => x.Sys_FieldName == titleKey).FirstOrDefault());
                }
                if (gridType == DataGridType.FlowGrid || gridType == DataGridType.ViewDetailGrid)
                {
                    if (!string.IsNullOrEmpty(condition))
                    {
                        try
                        {
                            string tempField = condition.Replace("{", string.Empty).Replace("}", string.Empty).Split(":".ToCharArray()).FirstOrDefault();
                            if (!string.IsNullOrEmpty(tempField))
                                searchFields = searchFields.Where(x => x.Sys_FieldName != tempField).ToList();
                        }
                        catch { }
                    }
                }
                bool hasSearchFields = searchFields != null && searchFields.Count > 0;
                string searchFloat = "right";
                //自定义搜索UI
                string simpleSearchHtml = GetCustomerPageHTML(moduleId, "GetSimpleSearchHTML", new object[] { searchFields, gridType, condition, where, viewId, initModule, initField });
                sb.AppendFormat("<div id=\"div_search{0}\" style=\"margin-right:15px;float:{1};margin-top:{2}px;\">", moduleId.ToString(), searchFloat, (ConstDefine.GRID_TOOLBAR_HEIGHT - 26) / 2);
                if (string.IsNullOrEmpty(simpleSearchHtml)) //不存在用户自定义搜索UI
                {
                    if (hasSearchFields) //有搜索字段
                    {
                        //是否显示复杂搜索
                        bool displayComplexSearch = (gridType == DataGridType.MainGrid || gridType == DataGridType.DialogGrid) && grid.MaxSearchNum > 1;
                        Dictionary<Sys_Field, string> fieldInputHtmls = new Dictionary<Sys_Field, string>();
                        //搜索控件domId
                        string searchTxtName = gridType == DataGridType.MainGrid ? "txtSearch" : string.Format("txtSearch{0}", moduleId);
                        //简单搜索框
                        sb.AppendFormat("<div id=\"div_sampleSearch\" style=\"float:left;display:{0}\">", displayComplexSearch && !GlobalSet.IsDefaultSimpleSearch ? "none" : "block");
                        sb.Append("<input id=\"" + searchTxtName + "\" moduleId=\"" + moduleId.ToString() + "\" gridId=\"" + gridId + "\" class=\"easyui-searchbox\" reget=\"1\" data-options=\"height:24,prompt:'请输入搜索值，空格隔开可搜索多个值',menu:'#search_mm" + moduleId.ToString() + "',searcher:function(value,name){if(typeof(SimpleSearch)=='function'){SimpleSearch(this,value,name);}}\" style=\"width:350px;\" />");
                        sb.AppendFormat("<div id=\"search_mm{0}\" searchTxtId=\"{1}\">", moduleId.ToString(), searchTxtName);
                        int no = 1;
                        int maxSearchNum = grid.MaxSearchNum;
                        if (gridType == DataGridType.DialogGrid)
                            maxSearchNum = 3;
                        foreach (Sys_GridField searchField in searchFields)
                        {
                            Sys_Field sysField = searchField.TempSysField != null ? searchField.TempSysField : SystemOperate.GetFieldById(searchField.Sys_FieldId.Value);
                            if (sysField == null) continue;
                            if (searchField.TempSysField == null) searchField.TempSysField = sysField;
                            PropertyInfo p = modelType.GetProperty(sysField.Name);
                            if (p == null) //非当前模块字段
                            {
                                Type tempType = CommonOperate.GetModelType(sysField.Sys_ModuleId.Value);
                                p = tempType.GetProperty(sysField.Name); //外键或明细模块字段
                                if (p == null) continue;
                            }
                            //外键字段和字符串字段支持简单搜索
                            bool isSupportSampleSearch = !string.IsNullOrWhiteSpace(sysField.ForeignModuleName) || p.PropertyType == typeof(String);
                            if (isSupportSampleSearch) //支持简单搜索
                            {
                                sb.AppendFormat("<div data-options=\"iconCls:'eu-p2-icon-table',name:'{0}'\">{1}</div>", sysField.Name, sysField.Display);
                            }
                            if (displayComplexSearch)
                            {
                                if (no > maxSearchNum) continue;
                                Sys_FormField formField = SystemOperate.GetNfDefaultFormSingleField(sysField);
                                if (formField == null)
                                {
                                    if (sysField.Name == "CreateDate" || sysField.Name == "ModifyDate")
                                        formField = new Sys_FormField() { ControlTypeOfEnum = ControlTypeEnum.DateTimeBox };
                                }
                                string inputHtml = string.Empty;
                                if (formField != null)
                                {
                                    if (formField.ControlTypeOfEnum == ControlTypeEnum.MutiCheckBox) continue;
                                    bool isSupportComplexSearch = isSupportSampleSearch || formField.ControlTypeOfEnum == ControlTypeEnum.ComboBox || formField.ControlTypeOfEnum == ControlTypeEnum.SingleCheckBox || formField.ControlTypeOfEnum == ControlTypeEnum.DateBox || formField.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox;
                                    if (!isSupportComplexSearch) continue;
                                    int controlWidth = 100; //默认搜索框宽100
                                    switch (formField.ControlTypeOfEnum)
                                    {
                                        case ControlTypeEnum.SingleCheckBox:
                                        case ControlTypeEnum.IntegerBox:
                                        case ControlTypeEnum.NumberBox:
                                            controlWidth = 60;
                                            break;
                                        case ControlTypeEnum.DateBox:
                                        case ControlTypeEnum.DateTimeBox:
                                            controlWidth = 175;
                                            break;
                                    }
                                    if (formField.ControlTypeOfEnum == ControlTypeEnum.DateBox || formField.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox)
                                        inputHtml = string.Format("<input id=\"{0}\" name=\"{0}\" type=\"text\" class=\"easyui-daterange\" style=\"width:{1}px\" />", sysField.Name, controlWidth.ToString());
                                    else
                                        inputHtml = GetFormFieldInputHTML(moduleId, formField, sysField, null, true, null, null, controlWidth, 20, true, false, currUser);
                                }
                                else if (p.PropertyType == typeof(String)) //表单字段不存在
                                {
                                    inputHtml = string.Format("<input id=\"{0}\" name=\"{0}\" class=\"easyui-textbox\" style=\"width:100px\" />", sysField.Name);
                                }
                                if (!string.IsNullOrEmpty(inputHtml))
                                {
                                    fieldInputHtmls.Add(sysField, inputHtml);
                                    no++;
                                }
                            }
                        }
                        sb.Append("</div>");
                        sb.Append("</div>");
                        //添加复杂搜索
                        if (displayComplexSearch && fieldInputHtmls.Count > 0)
                        {
                            sb.AppendFormat("<div id=\"div_complexSearch\" style=\"float:left;{0}\">", GlobalSet.IsDefaultSimpleSearch ? "display:none;" : string.Empty);
                            sb.Append("<form method=\"post\" id=\"searchform\">");
                            sb.Append("<div id=\"mainContent\">");
                            sb.Append("<table style=\"height:26px;line-height:26px;\"><tr>");
                            foreach (Sys_Field key in fieldInputHtmls.Keys)
                            {
                                sb.AppendFormat("<td><span style=\"margin-left:5px\">{0}：</span></td>", key.Display);
                                sb.Append("<td>");
                                sb.Append(fieldInputHtmls[key]);
                                sb.Append("</td>");
                            }
                            sb.Append("</tr></table>");
                            sb.Append("</div>");
                            sb.Append("</form>");
                            sb.Append("</div>");
                            sb.AppendFormat("<a id=\"btn_search\" moduleId=\"{0}\" moduleName=\"{1}\" gridId=\"{2}\" viewId=\"{3}\" href=\"#\" title=\"点击搜索\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-icon-search\" plain=\"true\" onclick=\"ComplexSearch(this)\" style=\"{4}\">查询</a>", moduleId.ToString(), module.Name, gridId, grid.Id.ToString(), GlobalSet.IsDefaultSimpleSearch ? "display:none;" : string.Empty);
                            //切换搜索按钮
                            sb.AppendFormat("<a id=\"btn_changeSearch\" href=\"#\" title=\"切换搜索方式\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-p2-icon-feed_magnify\" plain=\"true\" onclick=\"ChangeSearchStyle(this)\"></a>", moduleId.ToString(), module.Name, gridId, grid.Id.ToString());
                            sb.Append("<a id=\"btn_clear\" href=\"#\" title=\"清除搜索内容\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-icon-clear\" plain=\"true\" onclick=\"ClearSearch(this)\">清除</a>");
                        }
                        //高级搜索按钮
                        sb.AppendFormat("<a id=\"btn_advanceSearch{0}\" moduleId=\"{0}\" moduleName=\"{1}\" gridId=\"{2}\" viewId=\"{3}\" href=\"#\" title=\"高级搜索\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-icon-advance_search\" plain=\"true\" onclick=\"AdvanceSearch(this)\"></a>", moduleId.ToString(), module.Name, gridId, grid.Id.ToString());
                        if (gridType == DataGridType.MainGrid && module.Name != "待办任务")
                        {
                            int isrf = grid.AddFilterRow.HasValue && grid.AddFilterRow.Value ? 1 : 0;
                            string filterTitle = "启用行过滤搜索";
                            string filterIcon = "eu-icon-filter";
                            if (isrf == 1)
                            {
                                filterTitle = "关闭行过滤搜索";
                                filterIcon = "eu-icon-tip";
                            }
                            //启用过滤行搜索按钮
                            sb.AppendFormat("<a id=\"btn_filterSearch\" moduleId=\"{0}\" moduleName=\"{1}\" gridId=\"{2}\" viewId=\"{3}\" isrf=\"{4}\" href=\"#\" title=\"{5}\" class=\"easyui-linkbutton\" iconCls=\"{6}\" plain=\"true\" onclick=\"OpenOrCloseRowFilterSearch(this)\"></a>", moduleId.ToString(), module.Name, gridId, grid.Id.ToString(), isrf.ToString(), filterTitle, filterIcon);
                        }
                    }
                }
                else //存在用户自定义搜索UI
                {
                    sb.Append(simpleSearchHtml);
                }
                if (gridType == DataGridType.MainGrid)
                {
                    //视图设置按钮
                    string treeField = gridParams.TreeField;
                    if (treeField == null) treeField = string.Empty;
                    string tempGridUrl = gridUrl; //保存基本网格url,切换视图时用到
                    if (!string.IsNullOrEmpty(otherGridUrl))
                        tempGridUrl = gridUrl.Replace(otherGridUrl, string.Empty);
                    sb.AppendFormat("<a id=\"btn_gridSet{0}\" moduleId=\"{0}\" moduleName=\"{1}\" gridId=\"{2}\" viewId=\"{3}\" viewName=\"{4}\" treeField=\"{5}\" gridUrl=\"{6}\" href=\"#\" title=\"单击可切换列表视图--当前视图：{7}\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-grid\" plain=\"true\" onclick=\"GridSet(this)\"></a>",
                            moduleId.ToString(), module.Name, gridId, grid.Id.ToString(), grid.Name, treeField, tempGridUrl, grid.Name);
                    //附属模块及明细显示设置按钮
                    if (grid.GridTypeOfEnum != GridTypeEnum.ComprehensiveDetail && SystemOperate.GetAttachModules(moduleId).Count > 0)
                    {
                        sb.AppendFormat("<a id=\"btn_attach_set_{0}\" moduleId=\"{0}\" moduleName=\"{1}\" class=\"easyui-linkbutton easyui-tooltip\" data-options=\"iconCls:'eu-icon-cog',plain:true\" title=\"附属模块显示设置\" onclick=\"AttachModuleSet(this)\"></a>", moduleId.ToString(), module.Name);
                    }
                    //我的草稿图标
                    if (module.IsEnabledDraft)
                    {
                        Guid tempModuleId = moduleId;
                        string tempModuleName = module.Name;
                        if (grid.GridTypeOfEnum == GridTypeEnum.ComprehensiveDetail)
                        {
                            Sys_Module detailModule = SystemOperate.GetDetailModules(moduleId).FirstOrDefault();
                            tempModuleId = detailModule.Id;
                            tempModuleName = detailModule.Name;
                        }
                        sb.AppendFormat("<a id=\"btn_draft\" moduleId=\"{0}\" moduleName=\"{1}\" href=\"#\" title=\"点击进入我的草稿\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-icon-draft\" plain=\"true\" onclick=\"GoToDraft(this)\"></a>", tempModuleId.ToString(), tempModuleName);
                    }
                }
                if (gridType == DataGridType.MainGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.ViewDetailGrid)
                {
                    //回收站图标
                    if (module.IsEnabledRecycle && isContainsDelBtn)
                    {
                        Guid tempModuleId = moduleId;
                        string tempModuleName = module.Name;
                        string tempModuleDisplay = string.IsNullOrEmpty(module.Display) ? module.Name : module.Display;
                        sb.AppendFormat("<a id=\"btn_recycle\" moduleId=\"{0}\" moduleName=\"{1}\" moduleDisplay=\"{2}\" href=\"#\" title=\"点击进入回收站\" class=\"easyui-linkbutton easyui-tooltip\" iconCls=\"eu-icon-recycle\" plain=\"true\" onclick=\"GoToRecycle(this)\"></a>", tempModuleId.ToString(), tempModuleName, tempModuleDisplay);
                    }
                }
                sb.Append("</div>");
            }
            #endregion
            sb.Append(mm_html.ToString());
            sb.Append("</div>");
            //end 网格工具栏和搜索框
            #region 网格明细或附属模块
            if (isLoadAttachModule) //允许加载附属模块
            {
                List<Sys_Module> detailOrAttachModules = new List<Sys_Module>();
                if (!module.DetailInGrid) //添加明细模块
                    detailOrAttachModules.AddRange(SystemOperate.GetDetailModules(module.Id));
                //附属模块
                detailOrAttachModules.AddRange(SystemOperate.GetUserBindAttachModules(currUser.UserId, module.Id, false));
                sb.AppendFormat("<div id=\"region_south\" style=\"overflow:hidden;margin-top:5px;min-height:182px;\" moduleId=\"{0}\" moduleName=\"{1}\" gridId=\"{2}\" foreignField=\"{3}Id\" data-options=\"region:'south',border:false\">",
                        moduleId.ToString(), module.Name, gridId, module.TableName);
                sb.AppendFormat("<div id=\"detailTabs\" class=\"easyui-tabs\" data-options=\"fit:false,border:false,onSelect:AttachTabSelected,tabHeight:{0}\">", ConstDefine.TAB_HEAD_HEIGHT.ToString());
                foreach (Sys_Module attachModule in detailOrAttachModules)
                {
                    sb.AppendFormat("<div title=\"{0}\">", string.IsNullOrEmpty(attachModule.Display) ? attachModule.Name : attachModule.Display);
                    string currForeignField = string.Format("{0}Id", module.TableName);
                    string attachCondition = "{" + currForeignField + ":{Id}}";
                    string attachGridHtml = GetGridHTML(attachModule.Id, DataGridType.FlowGrid, attachCondition, null, null, null, null, null, false, null, null, !string.IsNullOrWhiteSpace(grid.TreeField) && !grid.IsTreeGrid, request);
                    sb.Append(attachGridHtml);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
                sb.Append("</div>");
            }
            #endregion
            sb.Append("</div>");
            #endregion
            #region 其他处理
            if (gridType == DataGridType.MainGrid)
            {
                #region 页面弹出框
                sb.Append("<div id=\"page_dialog1\"></div>");
                sb.Append("<div id=\"page_dialog2\"></div>");
                sb.Append("<div id=\"page_dialog3\"></div>");
                sb.Append("<div id=\"page_dialog4\"></div>");
                sb.Append("<div id=\"page_dialog5\"></div>");
                sb.Append("<div id=\"page_dialog6\"></div>");
                sb.Append("<div id=\"page_dialog7\"></div>");
                sb.Append("<div id=\"page_dialog8\"></div>");
                sb.Append("<div id=\"page_dialog9\"></div>");
                sb.Append("<div id=\"page_dialog10\"></div>");
                #endregion
            }
            string fixedSerializeJsr = WebHelper.GetJsModifyTimeStr("/Scripts/extension/FormFixedSerialize.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/extension/FormFixedSerialize.js?r={1}\"></script>", domainPath, fixedSerializeJsr);
            if (gridType != DataGridType.EditDetailGrid && gridType != DataGridType.ViewDetailGrid)
            {
                if (gridType != DataGridType.FlowGrid)
                {
                    //网格js
                    string gridJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/Grid.js");
                    sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/Grid.js?r={1}\"></script>", domainPath, gridJsr);
                }
                //网格扩展js
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/easyui-extension/datagrid-cellediting.js\"></script>", domainPath);
            }
            #region 行过滤处理
            if (gridType == DataGridType.MainGrid) //主网格启用过滤行  && grid.AddFilterRow.HasValue && grid.AddFilterRow.Value
            {
                //网格过滤行js
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/easyui-extension/datagrid-filter.js\"></script>", domainPath);
                //网格扩展js
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/Grid-Extension.js\"></script>", domainPath);
                //行过滤规则
                string tempStr = string.Empty;
                string tempNoFilterStr = string.Empty;
                if (grid.AddFilterRow.HasValue && grid.AddFilterRow.Value)
                {
                    List<string> noFilterFields = null;
                    if (isEnabledFlow)
                        gridFields.Insert(1, new Sys_GridField() { Sys_FieldName = "FlowStatus", Display = "状态", IsAllowSearch = false, Width = 86, Sort = 1, IsFrozen = true, FieldFormatter = "function(value, row, index){return FlowStatusFormatter(value, row, index,'" + moduleId.ToString() + "','" + module.Name + "');}" });
                    StringBuilder ruleFilters = SystemOperate.GetGridRowFilterRules(moduleId, gridFields, out noFilterFields, currUser, btnIds);
                    if (ruleFilters.Length > 0)
                    {
                        tempStr = string.Format("[{0}]", ruleFilters.ToString().Substring(0, ruleFilters.Length - 1));
                        tempNoFilterStr = noFilterFields.Count > 0 ? string.Join(",", noFilterFields) : string.Empty;
                        tempStr = HttpUtility.UrlEncode(tempStr, Encoding.UTF8);
                    }
                }
                sb.AppendFormat("<input id=\"ruleFilters\" type=\"hidden\" value=\"{0}\" noFilterFields=\"{1}\" />", tempStr, tempNoFilterStr);
            }
            #endregion
            //列头右键菜单字段显示框
            sb.AppendFormat("<div id=\"colmenu_{0}\" style=\"overflow:auto;\"></div>", moduleId.ToString());
            if (isShowFullPath)
            {
                sb.AppendFormat("<input id=\"userInfo\" type=\"hidden\" value=\"{0}\" />", HttpUtility.UrlEncode(JsonHelper.Serialize(currUser).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20"));
            }
            //模块自定义js
            sb.AppendLine(UIOperate.GetModelJsHTML(module, request));
            #endregion
            html = sb.ToString();
            #endregion
            #region 设置页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                SystemOperate.SetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.GridPage, html);
            }
            #endregion
            return html;
        }

        #region 其他

        /// <summary>
        /// 获取弹出编辑表单时网格按钮属性
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="titleKeyDisplay">titleKey显示名称</param>
        /// <param name="editMode">编辑模式</param>
        /// <param name="formWidth">表单宽度</param>
        /// <param name="formHeight">表单高度</param>
        /// <param name="gridId">网格tagId</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="button">按钮</param>
        /// <param name="customerFormUrl">编辑、查看表单的自定义表单URL</param>
        /// <param name="formId">表单ID</param>
        /// <returns></returns>
        private static string GetGridButtonAttr(Sys_Module module, string titleKeyDisplay, int editMode, int formWidth, int formHeight, string gridId, DataGridType gridType, Sys_GridButton button, string customerFormUrl = null, Guid? formId = null)
        {
            if (module == null || button == null) return string.Empty;
            string btnAttr = string.Format("moduleId=\"{0}\" moduleName=\"{1}\" titleKey=\"{2}\" titleKeyDisplay=\"{3}\" editMode=\"{4}\" editWidth=\"{5}\" editHeight=\"{6}\" gridId=\"{7}\" gt=\"{8}\" moduleDisplay=\"{9}\"",
                        module.Id.ToString(), module.Name, module.TitleKey, titleKeyDisplay, editMode.ToString(), formWidth.ToString(), formHeight.ToString(), gridId, ((int)gridType).ToString(), module.Display);
            #region 针对弹出框时表单按钮JSON
            if (button.ClickMethod == "Add(this)" || button.ClickMethod == "Copy(this)" || button.ClickMethod == "Edit(this)" || button.ClickMethod == "ViewRecord(this)")
            {
                if (!string.IsNullOrEmpty(customerFormUrl))
                {
                    btnAttr += string.Format(" formUrl=\"{0}\"", customerFormUrl);
                }
                else if (formId.HasValue && formId.Value != Guid.Empty)
                {
                    btnAttr += string.Format(" formId=\"{0}\"", formId.Value.ToString());
                }
            }
            else if (button.ClickMethod == "Delete(this)" && !module.IsEnabledRecycle)
            {
                btnAttr += " isHardDel=1";
            }
            #endregion
            return btnAttr;
        }

        /// <summary>
        /// 获取网格字段格式化参数，并返回编辑参数
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="field">字段</param>
        /// <param name="gridType">网格类型</param>
        /// <param name="gridId">网格DomId</param>
        /// <param name="editMode">编辑模式</param>
        /// <param name="otherFormatParams">其他参数</param>
        /// <param name="editorStr">返回编辑参数</param>
        /// <param name="userId">当前用户</param>
        /// <returns></returns>
        private static string GetGridFieldFormatter(Sys_Module module, Sys_GridField field, DataGridType gridType, string gridId, int editMode, string otherFormatParams, out string editorStr, Guid userId)
        {
            editorStr = field.EditorFormatter;
            if (!field.Sys_FieldId.HasValue) return string.Empty;
            bool isAllowEditField = gridType == DataGridType.MainGrid || gridType == DataGridType.FlowGrid || gridType == DataGridType.InnerDetailGrid || gridType == DataGridType.ViewDetailGrid;
            string formatStr = gridType != DataGridType.RecycleGrid ? field.FieldFormatter : string.Empty;
            string errMsg = string.Empty;
            if (string.IsNullOrEmpty(formatStr))
            {
                Sys_Field sysField = field.TempSysField != null ? field.TempSysField : SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                if (sysField == null || !sysField.Sys_ModuleId.HasValue) return string.Empty;
                if (field.TempSysField == null) field.TempSysField = sysField;
                string foreignFormatParams = string.Empty; //外键格式化参数
                if (!CommonDefine.BaseEntityFields.Contains(sysField.Name) && !string.IsNullOrWhiteSpace(sysField.ForeignModuleName) && gridType != DataGridType.RecycleGrid && gridType != DataGridType.MyDraftGrid && gridType != DataGridType.EditDetailGrid) //外键模块处理
                {
                    Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                    if (foreignModule != null)
                    {
                        Sys_Form foreginForm = SystemOperate.GetUserForm(userId, foreignModule.Id); //表单对象
                        int ew = 0; //外键表单宽度
                        int eh = 0; //外键表单高度
                        int em = GetEditMode(foreignModule, foreginForm, out ew, out eh, userId); //外键编辑模式
                        string foreignTitleKey = string.IsNullOrEmpty(foreignModule.TitleKey) ? string.Empty : foreignModule.TitleKey;
                        string foreignTitleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(foreignModule);
                        foreignFormatParams = HttpUtility.UrlEncode("{moduleId:'" + foreignModule.Id.ToString() + "',moduleDisplay:'" + (string.IsNullOrEmpty(foreignModule.Display) ? foreignModule.Name : foreignModule.Display) + "',titleKey:'" + foreignTitleKey + "',titleKeyDisplay:'" + foreignTitleKeyDisplay + "',editMode:" + em.ToString() + ",editWidth:" + ew.ToString() + ",editHeight:" + eh.ToString() + "}", Encoding.UTF8).Replace("+", "%20");
                    }
                }
                formatStr = SystemOperate.GetGridFormatFunction(sysField.Sys_ModuleId.Value, sysField, gridId, isAllowEditField, otherFormatParams, foreignFormatParams, field.Sys_FieldName);
                field.FieldFormatter = formatStr;
                if (!string.IsNullOrEmpty(formatStr) && gridType != DataGridType.RecycleGrid)
                {
                    CommonOperate.OperateRecord<Sys_GridField>(field, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "FieldFormatter" });
                }
            }
            if (string.IsNullOrEmpty(editorStr))
            {
                Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                if (sysField == null || !sysField.Sys_ModuleId.HasValue) return formatStr;
                string editor = string.Empty;
                if (editMode == (int)ModuleEditModeEnum.GridRowEdit)
                {
                    bool isCanEdtor = true; //是否允许打开编辑器
                    string parentModuleName = module.ParentId.HasValue ? SystemOperate.GetModuleNameById(module.ParentId.Value) : string.Empty;
                    if ((gridType == DataGridType.FlowGrid || gridType == DataGridType.ViewDetailGrid) && !string.IsNullOrEmpty(sysField.ForeignModuleName) && sysField.ForeignModuleName == parentModuleName)
                    {
                        isCanEdtor = false; //附属网格和明细查看网格时对应父模块外键字段不允许编辑
                    }
                    if (isCanEdtor) //允许打开编辑器
                    {
                        Sys_FormField formField = SystemOperate.GetNfDefaultFormSingleField(sysField);
                        if (formField != null && (formField.IsAllowAdd == true || formField.IsAllowEdit == true)) //允许编辑时
                        {
                            if (PermissionOperate.CanEditField(userId, module.Id, sysField.Name)) //有编辑权限
                                editor = SystemOperate.GetFieldEditor(SystemOperate.GetModuleById(sysField.Sys_ModuleId.Value), sysField);
                        }
                    }
                }
                editorStr = string.IsNullOrEmpty(editor) ? string.Empty : string.Format(",editor:{0}", editor);
                field.EditorFormatter = editorStr;
                if (!string.IsNullOrEmpty(editorStr))
                {
                    CommonOperate.OperateRecord<Sys_GridField>(field, ModelRecordOperateType.Edit, out errMsg, new List<string>() { "EditorFormatter" });
                }
            }
            return formatStr;
        }

        /// <summary>
        /// 加载高级搜索页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="viewId">视图Id</param>
        /// <returns></returns>
        public override string GetAdvanceSearchHTML(Guid moduleId, Guid? viewId)
        {
            Guid tempViewId = viewId.HasValue ? viewId.Value : SystemOperate.GetDefaultGrid(moduleId).Id;
            #region 取页面缓存
            string cacheKey = string.Format("{0}_{1}", moduleId.ToString(), tempViewId.ToString());
            if (GlobalSet.IsEnabledPageCache)
            {
                string cacheHtml = SystemOperate.GetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.OtherPage);
                if (!string.IsNullOrEmpty(cacheHtml))
                {
                    return cacheHtml;
                }
            }
            #endregion
            #region 生成页面
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            StringBuilder sb = new StringBuilder();
            sb.Append("<form method=\"post\" id=\"searchform\">");
            sb.Append("<div style=\"width:380px;height:100%;padding:10px;\">");
            sb.Append("<div id=\"mainContent\" class=\"content\">");
            sb.Append("<table border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:300px;line-height:30px;table-layout:fixed;\">");
            List<Sys_GridField> searchFields = SystemOperate.GetSearchGridFields(tempViewId); //搜索字段
            List<Sys_FormField> formFields = new List<Sys_FormField>();
            List<Sys_FormField> specialFormFields = new List<Sys_FormField>();
            #region 将创建人、修改人、创建时间、修改时间添加进去
            if (SystemOperate.GetModuleByName("员工管理") != null)
            {
                specialFormFields.Add(new Sys_FormField()
                {
                    Sys_FieldName = "CreateUserName",
                    Display = "创建人",
                    ControlType = (int)ControlTypeEnum.DialogTree
                });
                specialFormFields.Add(new Sys_FormField()
                {
                    Sys_FieldName = "ModifyUserName",
                    Display = "修改人",
                    ControlType = (int)ControlTypeEnum.DialogTree
                });
            }
            specialFormFields.Add(new Sys_FormField()
            {
                Sys_FieldName = "CreateDate",
                Display = "创建日期",
                ControlType = (int)ControlTypeEnum.DateTimeBox
            });
            specialFormFields.Add(new Sys_FormField()
            {
                Sys_FieldName = "ModifyDate",
                Display = "修改日期",
                ControlType = (int)ControlTypeEnum.DateTimeBox
            });
            if (BpmOperate.IsEnabledWorkflow(moduleId))
            {
                specialFormFields.Add(new Sys_FormField()
                {
                    Sys_FieldName = "FlowStatus",
                    Display = "状态",
                    ControlType = (int)ControlTypeEnum.ComboBox
                });
            }
            #endregion
            List<string> specialFields = specialFormFields.Select(x => x.Sys_FieldName).ToList();
            specialFields.Add("CreateUserId");
            specialFields.Add("ModifyUserId");
            foreach (Sys_GridField gridField in searchFields)
            {
                if (!gridField.Sys_FieldId.HasValue) continue;
                Sys_Field sysField = gridField.TempSysField != null ? gridField.TempSysField : SystemOperate.GetFieldById(gridField.Sys_FieldId.Value);
                if (sysField == null) continue;
                if (gridField.TempSysField == null) gridField.TempSysField = sysField;
                Sys_FormField field = SystemOperate.GetNfDefaultFormSingleField(sysField);
                if (field == null && !specialFields.Contains(sysField.Name))
                {
                    Type fieldType = sysField.Sys_ModuleId.HasValue ? SystemOperate.GetFieldType(sysField.Sys_ModuleId.Value, sysField.Name) : null;
                    ControlTypeEnum controlType = ControlTypeEnum.TextBox;
                    if (fieldType == typeof(DateTime) || fieldType == typeof(DateTime?))
                        controlType = ControlTypeEnum.DateBox;
                    else if (fieldType == typeof(Boolean) || fieldType == typeof(Boolean?))
                        controlType = ControlTypeEnum.SingleCheckBox;
                    else if (SystemOperate.IsEnumField(sysField.Sys_ModuleId.Value, sysField.Name) ||
                             SystemOperate.IsDictionaryBindField(sysField.Sys_ModuleId.Value, sysField.Name))
                        controlType = ControlTypeEnum.ComboBox;
                    field = new Sys_FormField() { Sys_FieldId = gridField.Sys_FieldId, Sys_FieldName = sysField.Name, Display = sysField.Display, ControlType = (int)controlType };
                }
                if (field != null)
                {
                    if (string.IsNullOrEmpty(field.Sys_FieldName))
                        field.Sys_FieldName = sysField.Name;
                    if (field.TempSysField == null)
                        field.TempSysField = sysField;
                    formFields.Add(field);
                }
            }
            formFields.AddRange(specialFormFields);
            List<object> fieldObjects = new List<object>(); //表单字段匿名对象集合
            foreach (Sys_FormField field in formFields)
            {
                int labelWidth = 120;
                int inputWidth = 180;
                sb.Append("<tr>");
                Sys_Field sysField = field.TempSysField != null ? field.TempSysField : (field.Sys_FieldId.HasValue ? SystemOperate.GetFieldById(field.Sys_FieldId.Value) : null);
                Type fieldType = sysField != null && sysField.Sys_ModuleId.HasValue ? SystemOperate.GetFieldType(sysField.Sys_ModuleId.Value, sysField.Name) : null;
                //组装标签
                string display = sysField != null ? sysField.Display : field.Display;
                bool dateBox = field.ControlTypeOfEnum == ControlTypeEnum.DateBox || field.ControlTypeOfEnum == ControlTypeEnum.DateTimeBox || fieldType == typeof(DateTime) || fieldType == typeof(DateTime?);
                sb.AppendFormat("<td style=\"width:{0}px;text-align:right;\">{1}{2}：</td>", labelWidth, display, dateBox ? "(开始)" : string.Empty); //标签
                #region 组装输入框
                string foreignModuleName = sysField != null ? sysField.ForeignModuleName : (field.Sys_FieldName == "CreateUserName" || field.Sys_FieldName == "ModifyUserName" ? "员工管理" : string.Empty);
                bool isTitleKey = sysField != null ? module.TitleKey == sysField.Name : false; //是否titlekey字段
                //外键模块
                Sys_Module foreignModule = !string.IsNullOrEmpty(foreignModuleName) ? SystemOperate.GetModuleByName(foreignModuleName) : null;
                string foreignTitleKey = foreignModule != null ? foreignModule.TitleKey : string.Empty; //外键模块titleKey
                string inputHtml = string.Empty;
                if (sysField == null)
                {
                    switch (field.Sys_FieldName)
                    {
                        case "CreateUserName":
                        case "ModifyUserName":
                            {
                                string fieldUrl = string.Format("/Page/DialogTree.html?moduleName={0}", HttpUtility.UrlEncode(foreignModuleName));
                                string valueField = "Id";
                                string textField = "Name";
                                string fieldAttr = string.Format("url=\"{0}\" valueField=\"{1}\" textField=\"{2}\" foreignModuleId=\"{3}\" foreignModuleName=\"{4}\"", fieldUrl, valueField, textField, foreignModule.Id.ToString(), foreignModuleName);
                                fieldAttr += " isTree=\"1\"";
                                string inputOptions = "data-options=\"icons: [{iconCls:'eu-icon-search',handler: function(e){SelectDialogData($(e.data.target))}}]\"";
                                inputHtml = string.Format("<input id=\"{0}\" name=\"{0}\" class=\"easyui-textbox\" style=\"width:100%;\" {1} {2}/>",
                                                           field.Sys_FieldName, fieldAttr, inputOptions);
                            }
                            break;
                        case "CreateDate":
                        case "ModifyDate":
                            {
                                inputHtml = string.Format("<input id=\"{0}\" name=\"{0}\" class=\"easyui-datetimebox\" style=\"width:100%;\" />", field.Sys_FieldName);
                            }
                            break;
                        case "FlowStatus":
                            {
                                string fieldUrl = string.Format("/{0}/BindEnumFieldData.html?moduleId={1}&fieldName=FlowStatus", GlobalConst.ASYNC_DATA_CONTROLLER_NAME, moduleId.ToString());
                                string dtFilter = "loadFilter:function(data){if(typeof (data)== 'string'){var tempData=eval('(' + data + ')');return tempData;} else{return data;}}";
                                inputHtml = string.Format("<input id=\"{0}\" name=\"{0}\" class=\"easyui-combobox\" data-options=\"editable:false,url:'{1}',valueField:'Id',textField:'Name',{2}\" style=\"width:100%;\" />", field.Sys_FieldName, fieldUrl, dtFilter);
                            }
                            break;
                    }
                }
                else
                {
                    inputHtml = GetFormFieldInputHTML(moduleId, field, sysField, null, true, null, null, inputWidth, null, true, false, CurrUser);
                }
                string title = string.Empty;
                if (dateBox)
                {
                    title = "开始时间";
                    if (field.ControlTypeOfEnum == ControlTypeEnum.DateBox) title = "开始日期";
                }
                sb.AppendFormat("<td style=\"width:{0}px;text-align:left;\" title=\"{1}\">{2}</td>", inputWidth, title, inputHtml);
                #endregion
                sb.Append("</tr>");
                #region 针对日期字段处理
                if (dateBox) //日期时间型
                {
                    sb.Append("<tr>");
                    sb.AppendFormat("<td style=\"width:{0}px;text-align:right;\">{1}(结束)：</td>", labelWidth, display);
                    string inputClass = "easyui-datetimebox";
                    string dateTitle = "结束时间";
                    if (field.ControlTypeOfEnum == ControlTypeEnum.DateBox)
                    {
                        inputClass = "easyui-datebox";
                        dateTitle = "结束日期";
                    }
                    sb.AppendFormat("<td style=\"width:{0}px;text-align:left;\" title=\"{1}\"><input id=\"{2}_End\" name=\"{2}_End\" class=\"{3}\" style=\"width:{0}px;\" /></td>", inputWidth, dateTitle, field.Sys_FieldName, inputClass);
                    sb.Append("</tr>");
                }
                #endregion
                //设置表单字段对象
                if (foreignModule != null)
                {
                    //添加外键name字段
                    fieldObjects.Add(new
                    {
                        Sys_FieldName = field.Sys_FieldName.Substring(0, field.Sys_FieldName.Length - 2) + "Name",
                        Display = display,
                        ControlType = field.ControlType,
                        ForeignModuleName = foreignModuleName,
                        IsTitleKey = isTitleKey,
                        ForignFieldName = module.TableName + "Id",
                        ForeignTitleKey = foreignTitleKey
                    });
                }
                fieldObjects.Add(new
                {
                    Sys_FieldName = field.Sys_FieldName,
                    Display = display,
                    ControlType = field.ControlType,
                    ForeignModuleName = foreignModuleName,
                    IsTitleKey = isTitleKey,
                    ForignFieldName = module.TableName + "Id",
                    ForeignTitleKey = foreignTitleKey
                });
            }
            sb.Append("</table>");
            sb.Append("</div></div></form>");
            //表单字段对象添加到隐藏域中
            string formFieldJson = HttpUtility.UrlEncode(JsonHelper.Serialize(fieldObjects).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
            sb.AppendFormat("<input id=\"hd_formFields\" type=\"hidden\" value=\"{0}\" />", formFieldJson);
            //表单扩展js
            string fixedSerializeJs = UIOperate.FormatJsPath("/Scripts/extension/FormFixedSerialize.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", fixedSerializeJs);
            string advanceSearchJs = UIOperate.FormatJsPath("/Scripts/common/AdvanceSearch.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", advanceSearchJs);
            string html = sb.ToString();
            #endregion
            #region 设置页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                SystemOperate.SetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.OtherPage, html);
            }
            #endregion
            return html;
        }

        /// <summary>
        /// 加载快速编辑视图页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="viewId">视图ID</param>
        /// <returns></returns>
        public override string GetQuickEditViewHTML(Guid moduleId, Guid? viewId)
        {
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            string titleKey = module.TitleKey;
            List<string> primaryFields = string.IsNullOrWhiteSpace(module.PrimaryKeyFields) ? new List<string>() : module.PrimaryKeyFields.Split(",".ToCharArray()).ToList();
            string titleKeyFieldId = string.Empty;
            UserInfo currUser = CurrUser;
            Sys_Grid defaultGrid = SystemOperate.GetDefaultGrid(moduleId); //默认视图
            List<Sys_GridField> leftGridFields = defaultGrid.GridFields != null && defaultGrid.GridFields.Count > 0 ? defaultGrid.GridFields : SystemOperate.GetDefaultGridFields(moduleId); //加载默认视图字段
            if (leftGridFields.Count > 0 && (defaultGrid.GridFields == null || defaultGrid.GridFields.Count == 0))
                defaultGrid.GridFields = leftGridFields;
            List<string> canViewFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.ViewField);
            if (canViewFields != null && canViewFields.Count > 0 && !canViewFields.Contains("-1")) //字段权限过滤
                leftGridFields = leftGridFields.Where(x => canViewFields.Contains(x.Sys_FieldName)).ToList();
            List<Sys_GridField> rightGridFields = viewId.HasValue ? SystemOperate.GetGridFields(viewId.Value, false) : new List<Sys_GridField>();
            Sys_Grid rightGrid = viewId.HasValue ? SystemOperate.GetGrid(viewId.Value) : null;
            Sys_GridField gField = rightGridFields.Where(x => x.IsGroupField).FirstOrDefault();
            string groupField = gField != null ? gField.Sys_FieldName : string.Empty;
            string treeField = rightGrid == null ? string.Empty : rightGrid.TreeField;
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/easyui-extension/datagrid-groupview.js\"></script>");
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<style type=\"text/css\">");
            sb.Append("li {");
            sb.Append("text-align: center;");
            sb.Append("margin-bottom: 3px;");
            sb.Append("}");
            sb.Append("li a {");
            sb.Append("width: 30px;");
            sb.Append("}");
            sb.Append("</style>");
            sb.Append("<table style=\"width: 100%; height: 100%; line-height: 30px;\">");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"3\">");
            sb.Append("<table style=\"width:100%;\">");
            sb.Append("<tr>");
            sb.Append("<td style=\"width:90px;\"><lable>视图名称：</lable></td>");
            sb.AppendFormat("<td style=\"width:170px;\"><input style=\"width:160px;\" id=\"GridName\" class=\"easyui-textbox\" data-options=\"required:true,value:'{0}'\" /></td>", rightGrid == null ? string.Format("[{0}]视图", module.Name) : rightGrid.Name);
            sb.Append("<td style=\"width:90px;\"><lable>分组字段：</lable></td>");
            sb.AppendFormat("<td style=\"width:180px;\"><input style=\"width:180px;\" id=\"GroupField\" class=\"easyui-combobox\" data-options=\"valueField:'Sys_FieldName',textField:'Display',value:'{0}',url:'/{2}/LoadViewFields.html?moduleId={1}',loadFilter:FilterGroupFields\" /></td>", groupField, moduleId.ToString(), GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME);
            sb.Append("</tr><tr>");
            sb.Append("<td style=\"width:90px;\"><lable>树显示字段：</lable></td>");
            sb.AppendFormat("<td style=\"width:170px;\"><input style=\"width:160px;\" id=\"TreeField\" class=\"easyui-combobox\" data-options=\"valueField:'FieldName',textField:'Display',value:'{0}',url:'/{2}/LoadViewFields.html?moduleId={1}&flag=3'\" /></td>", treeField, moduleId.ToString(), GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME);
            sb.Append("<td style=\"width:90px;\"><lable>是否默认：</lable></td>");
            string checkStr = string.Empty;
            if (rightGrid != null)
            {
                bool isDefault = SystemOperate.IsUserDefaultGridView(CurrUser.UserId, viewId.Value);
                if (isDefault) checkStr = "checked=\"checked\"";
            }
            string otherOptions = string.Empty;
            //需要排除的字段名，把主模块、明细模块、外键模块重名的字段移除，防止加载数据异常
            List<string> executeModuleFns = leftGridFields.Select(x => x.Sys_FieldName).ToList();
            //当前模块的所有外键模块
            List<Sys_Module> foreignModules = SystemOperate.GetNoRepeatForeignModules(moduleId);
            List<Sys_Module> detailModules = SystemOperate.GetDetailModules(moduleId);
            if (foreignModules.Count > 0 || detailModules.Count == 1) //只有一个明细模块
            {
                otherOptions = ",view:groupview,groupField:'ModuleName',groupFormatter:function(value, rows){return value+ '(' + rows.length + ')';}";
                //添加明细模块字段
                if (detailModules.Count == 1)
                {
                    Sys_Module tempDetailModule = detailModules.FirstOrDefault();
                    List<Sys_GridField> tempDetailGridFields = SystemOperate.GetDefaultGridFields(tempDetailModule.Id); //明细模块默认视图
                    List<string> canViewDetailFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, tempDetailModule.Id, FieldPermissionTypeEnum.ViewField);
                    if (canViewDetailFields != null && canViewDetailFields.Count > 0 && !canViewDetailFields.Contains("-1")) //字段权限过滤
                        tempDetailGridFields = tempDetailGridFields.Where(x => canViewDetailFields.Contains(x.Sys_FieldName)).ToList();
                    //排除基类字段、外键主模块字段、与主模块字段名称相同的字段
                    tempDetailGridFields = tempDetailGridFields.Where(x => !CommonDefine.BaseEntityFields.Contains(x.Sys_FieldName) && x.Sys_FieldName != module.TableName + "Id" && x.Sys_FieldName != module.TableName + "Name" && !executeModuleFns.Contains(x.Sys_FieldName)).ToList();
                    leftGridFields.AddRange(tempDetailGridFields);
                    //将明细字段添加到排除字段集合
                    executeModuleFns.AddRange(tempDetailGridFields.Select(x => x.Sys_FieldName));
                }
                //添加外键模块字段
                if (foreignModules.Count > 0)
                {
                    foreach (Sys_Module tempModule in foreignModules)
                    {
                        List<Sys_GridField> tempGridFields = SystemOperate.GetDefaultGridFields(tempModule.Id); //外键模块默认视图
                        List<string> canViewForeignFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, tempModule.Id, FieldPermissionTypeEnum.ViewField);
                        if (canViewForeignFields != null && canViewForeignFields.Count > 0 && !canViewForeignFields.Contains("-1")) //字段权限过滤
                            tempGridFields = tempGridFields.Where(x => canViewForeignFields.Contains(x.Sys_FieldName)).ToList();
                        //排除基类字段、标记字段、executeModuleFns里面的字段
                        tempGridFields = tempGridFields.Where(x => !CommonDefine.BaseEntityFields.Contains(x.Sys_FieldName) && x.Sys_FieldName != tempModule.TitleKey && !executeModuleFns.Contains(x.Sys_FieldName)).ToList();
                        leftGridFields.AddRange(tempGridFields);
                        executeModuleFns.AddRange(tempGridFields.Select(x => x.Sys_FieldName));
                    }
                }
            }
            sb.AppendFormat("<td><input id=\"IsDefault\" type=\"checkbox\" {0} /></td>", checkStr);
            sb.Append("</tr></table>");
            sb.Append("</td></tr>");
            sb.Append("<tr>");
            sb.Append("<td style=\"width: 180px;\"><lable>可选字段：</lable></td>");
            sb.Append("<td style=\"width: 60px;\"></td>");
            sb.Append("<td style=\"width: 350px;\"><lable>已选字段：</lable></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<table id=\"leftGrid\" class=\"easyui-datagrid\" style=\"width: 180px; height: 350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'Id'" + otherOptions + "\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'Id',checkbox:true\"></th>");
            sb.Append("<th data-options=\"field:'FieldName',hidden:true,width:0\">字段名称</th>");
            sb.Append("<th data-options=\"field:'ModuleName',hidden:true,width:0\">模块</th>");
            sb.Append("<th data-options=\"field:'Display',width:180\">字段</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            //加载字段数据
            if (leftGridFields != null && leftGridFields.Count > 0)
            {
                sb.Append("<tbody>");
                foreach (Sys_GridField field in leftGridFields)
                {
                    string fieldName = field.Sys_FieldName;
                    if (!field.Sys_FieldId.HasValue || !field.IsVisible || fieldName == "Id")
                        continue;
                    string currFieldModuleName = string.Empty;
                    if (!string.IsNullOrEmpty(otherOptions))
                    {
                        Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                        currFieldModuleName = sysField.Sys_ModuleId.HasValue ? SystemOperate.GetModuleNameById(sysField.Sys_ModuleId.Value) : string.Empty;
                    }
                    string tempFieldName = fieldName;
                    if (SystemOperate.IsForeignNameField(moduleId, fieldName))
                        tempFieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
                    if (rightGrid == null && ((titleKey != null && fieldName == titleKey) || primaryFields.Contains(tempFieldName)))
                    {
                        rightGridFields.Add(field);
                    }
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", field.Id.ToString());
                    sb.AppendFormat("<td>{0}</td>", fieldName);
                    sb.AppendFormat("<td>{0}</td>", currFieldModuleName);
                    sb.AppendFormat("<td>{0}</td>", field.Display);
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
            }
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<ul>");
            sb.Append("<li><a href=\"#\" title=\"移入选中项\" class=\"easyui-linkbutton\" onclick=\"RightMove()\">></a></li>");
            sb.Append("<li><a href=\"#\" title=\"全部移入\" class=\"easyui-linkbutton\" onclick=\"RightMoveAll()\">>></a></li>");
            sb.Append("<li><a href=\"#\" title=\"移出选中项\" class=\"easyui-linkbutton\" onclick=\"LeftMove()\"><</a></li>");
            sb.Append("<li><a href=\"#\" title=\"全部移出\" class=\"easyui-linkbutton\" onclick=\"LeftMoveAll()\"><<</a></li>");
            sb.Append("</ul>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<table id=\"rightGrid\" class=\"easyui-datagrid\" style=\"width: 350px; height: 350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'Id'" + otherOptions + "\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'Id',checkbox:true\"></th>");
            sb.Append("<th data-options=\"field:'FieldName',hidden:true,width:0\">字段名称</th>");
            sb.Append("<th data-options=\"field:'ModuleName',hidden:true,width:0\">模块</th>");
            sb.Append("<th data-options=\"field:'Display',width:180\">字段</th>");
            sb.Append("<th data-options=\"field:'Operate',width:170\">操作</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            if (rightGridFields.Count > 0)
            {
                sb.Append("<tbody>");
                foreach (Sys_GridField field in rightGridFields)
                {
                    if (!field.Sys_FieldId.HasValue || !field.IsVisible || field.Sys_FieldName == "Id")
                        continue;
                    string fieldIdStr = field.Id.ToString();
                    if (field.Sys_FieldName == titleKey && !string.IsNullOrWhiteSpace(titleKey))
                    {
                        titleKeyFieldId = fieldIdStr;
                    }
                    string currFieldModuleName = string.Empty;
                    if (!string.IsNullOrEmpty(otherOptions))
                    {
                        Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                        currFieldModuleName = sysField.Sys_ModuleId.HasValue ? SystemOperate.GetModuleNameById(sysField.Sys_ModuleId.Value) : string.Empty;
                    }
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", fieldIdStr);
                    sb.AppendFormat("<td>{0}</td>", field.Sys_FieldName);
                    sb.AppendFormat("<td>{0}</td>", currFieldModuleName);
                    sb.AppendFormat("<td>{0}</td>", field.Display);
                    string operate = "<input id=\"btnUp_" + fieldIdStr + "\" rowId=\"" + fieldIdStr + "\" title=\"上移\" type=\"button\" style=\"width:30px;\" value=\"↑\" />";
                    operate += "<input id=\"btnDown_" + fieldIdStr + "\" rowId=\"" + fieldIdStr + "\" title=\"下移\" type=\"button\" style=\"width:30px;\" value=\"↓\" />";
                    operate += "<input id=\"btnTop_" + fieldIdStr + "\" rowId=\"" + fieldIdStr + "\" title=\"移至最顶部\" type=\"button\" style=\"width:30px;\" value=\"↑↑\" />";
                    operate += "<input id=\"btnBottom_" + fieldIdStr + "\" rowId=\"" + fieldIdStr + "\" title=\"移至最底部\" type=\"button\" style=\"width:30px;\" value=\"↓↓\" />";
                    sb.AppendFormat("<td>{0}</td>", operate);
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
            }
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.AppendFormat("<input id=\"titleKeyFieldId\" type=\"hidden\" value=\"{0}\" />", titleKeyFieldId);
            string quickEditViewJs = UIOperate.FormatJsPath("/Scripts/common/QuickEditView.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", quickEditViewJs);
            return sb.ToString();
        }

        /// <summary>
        /// 加载列表视图设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public override string GetGridSetHTML(Guid moduleId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<form method=\"post\">");
            sb.Append("<div style=\"width:400px;height:100%;padding:10px;\">");
            sb.Append("<div class=\"content\">");
            sb.Append("<table border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:100%;table-layout:fixed;\">");
            sb.Append("<tr><td style=\"width:100px;\">");
            sb.Append("<lable>请选择视图：</lable>");
            sb.Append("</td><td>");
            sb.AppendFormat("<input id=\"txtView\" name=\"txtView\" style=\"width:180px;\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',url:'/{1}/LoadUserGridView.html?moduleId={0}',onLoadSuccess:ViewLoadSuccess,onSelect:ViewSelected,editable:false\" />&nbsp;&nbsp;", moduleId.ToString(), GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME);
            sb.AppendFormat("<a href=\"#\" id=\"btnAddView\" moduleId=\"{0}\" moduleName=\"{1}\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-add_other\" plain=\"true\" onclick=\"AddView(this)\"></a>", moduleId.ToString(), SystemOperate.GetModuleNameById(moduleId));
            sb.AppendFormat("<a href=\"#\" id=\"btnEditView\" moduleId=\"{0}\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-edit\" plain=\"true\" onclick=\"EditView(this)\"></a>", moduleId.ToString());
            sb.AppendFormat("<a href=\"#\" id=\"btnDelView\" moduleId=\"{0}\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-delete2\" plain=\"true\" onclick=\"DelView(this)\"></a>", moduleId.ToString());
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("</div></div></form>");
            string gridSetJs = UIOperate.FormatJsPath("/Scripts/common/GridSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", gridSetJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取附属模块设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public override string GetAttachModuleSetHTML(Guid moduleId)
        {
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table id=\"attachModuleTable\" moduleName=\"{0}\" style=\"margin-left:5px;margin-top:5px;line-height:25px;\">", module.Name);
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th style=\"width:60px;text-align:center;font-weight:bold;\">序号</th>");
            sb.Append("<th style=\"width:150px;font-weight:bold;\">附属模块</th>");
            sb.Append("<th style=\"width:120px;font-weight:bold;\">排序</th>");
            sb.Append("<th style=\"width:100px;text-align:center;font-weight:bold;\">是否启用</th>");
            sb.Append("<th style=\"width:100px;text-align:center;font-weight:bold;\">嵌入网格显示</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            List<Sys_Module> attachModules = SystemOperate.GetAttachModules(moduleId);
            if (attachModules.Count > 0)
            {
                List<Sys_AttachModuleBind> attachBinds = SystemOperate.GetAttachModuleBind(CurrUser.UserId, moduleId);
                int n = 1;
                foreach (Sys_Module attachModule in attachModules)
                {
                    Sys_AttachModuleBind attachBind = attachBinds.Where(x => x.Sys_ModuleId == attachModule.Id && x.IsValid).FirstOrDefault();
                    string sort = attachBind == null ? "1" : attachBind.Sort.ToString();
                    string checkedStr = attachBind != null && attachBind.IsValid ? "checked=\"checked\"" : string.Empty;
                    string inGridStr = attachBind != null && attachBind.AttachModuleInGrid ? "checked=\"checked\"" : string.Empty;
                    sb.Append("<tr>");
                    sb.AppendFormat("<td style=\"text-align:center;\"><span>{0}</span></td>", n.ToString());
                    sb.AppendFormat("<td><span id=\"Sys_ModuleId_{0}\" attachModuleId=\"{0}\">{1}</span></td>", attachModule.Id.ToString(), attachModule.Name);
                    sb.AppendFormat("<td><input id=\"Sort_{0}\" style=\"width:112px;\" class=\"easyui-numberbox\" type=\"text\" value=\"{1}\" data-options=\"min:1,precision:0\" /></td>", attachModule.Id.ToString(), sort);
                    sb.AppendFormat("<td style=\"text-align:center;\"><span><input id=\"IsValid_{0}\" type=\"checkbox\" {1}/></span></td>", attachModule.Id.ToString(), checkedStr);
                    sb.AppendFormat("<td style=\"text-align:center;\"><span><input id=\"InGrid_{0}\" type=\"checkbox\" {1}/></span></td>", attachModule.Id.ToString(), inGridStr);
                    sb.Append("</tr>");
                    n++;
                }
            }
            sb.Append("</tbody>");
            sb.Append("</table>");
            string attachModuleSetJs = UIOperate.FormatJsPath("/Scripts/common/AttachModuleSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", attachModuleSetJs);
            return sb.ToString();
        }

        #endregion

        #endregion

        #region 表单页面

        #region 编辑表单

        /// <summary>
        /// 基于表格式Group编辑表单Table
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="form">表单</param>
        /// <param name="rows">group内字段</param>
        /// <param name="formFields">表单字段</param>
        /// <param name="tableWidth">table宽</param>
        /// <param name="model">实体对象</param>
        /// <param name="copyModel">复制实体对象</param>
        /// <param name="fieldObjects">字段匿名对象集合</param>
        /// <param name="canViewFields">允许查看字段集合</param>
        /// <param name="canOpFields">允许操作（新增、编辑）字段集合</param>
        /// <param name="isCache">是否来自缓存</param>
        /// <param name="isRestartFlow">是否重新发起流程</param>
        /// <returns></returns>
        private string GetNewEditFormGroupTable(Sys_Module module, Sys_Form form, List<IGrouping<int, Sys_FormField>> rows, List<Sys_FormField> formFields, string tableWidth, object model, object copyModel, ref List<object> fieldObjects, List<string> canViewFields, List<string> canOpFields, bool? isCache = false, bool isRestartFlow = false)
        {
            if (fieldObjects == null)
                fieldObjects = new List<object>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class=\"thinBorder padding5\" border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:{0};table-layout:fixed;\">", tableWidth);
            int maxCol = 0;
            foreach (var row in rows)
            {
                int count = row.Count();
                if (maxCol < count)
                    maxCol = count;
            }
            foreach (var row in rows)
            {
                sb.AppendFormat("<tr style=\"height:{0}px;\">", row.Select(x => x.ControlType).Contains((int)ControlTypeEnum.TextAreaBox) ? ConstDefine.FORM_TEXTAREA_ROW_HEIGHT : ConstDefine.FORM_ROW_HEIGHT);
                int n = 0;
                Dictionary<int, int> dic = new Dictionary<int, int>(); //每个字段所占标准宽字段倍数
                foreach (Sys_FormField field in row)
                {
                    if (!field.Sys_FieldId.HasValue)
                        continue;
                    n++;
                    Sys_Field sysField = field.TempSysField == null ? SystemOperate.GetFieldById(field.Sys_FieldId.Value) : field.TempSysField;
                    if (sysField == null) continue;
                    if (field.TempSysField == null) field.TempSysField = sysField;
                    int labelWidth = form.LabelWidth > 0 ? form.LabelWidth : ConstDefine.STANDARD_LABEL;
                    int inputWidth = (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : ConstDefine.STANDARD_CONTROL));
                    int width = labelWidth + inputWidth;
                    string splitChar = string.IsNullOrEmpty(form.RightToken) ? "：" : form.RightToken;
                    string xinStr = field.IsRequired == true ? "<font color=\"red\">*</font>" : string.Empty;
                    sb.AppendFormat("<th style=\"padding:2px;width:{0}px;text-align:right\">{1}{2}{3}</th>", labelWidth.ToString(), xinStr, sysField.Display, splitChar);
                    //获取受当前字段关联的字段
                    List<string> linkFieldsList = formFields.Where(x => x.Sys_FieldId.HasValue).Where(x => x.DefaultValue.ObjToStr() == "{" + sysField.Name + "}").Select(x => SystemOperate.GetFieldById(x.Sys_FieldId.Value).Name).ToList();
                    string linkFields = linkFieldsList != null && linkFieldsList.Count > 0 ? string.Join(",", linkFieldsList) : string.Empty;
                    //取字段输入控件
                    bool canOp = canOpFields == null || canOpFields.Count == 0 || canOpFields.Contains(sysField.Name);
                    bool canView = canViewFields == null || canViewFields.Count == 0 || canViewFields.Contains(sysField.Name);
                    string inputHtml = canView ? GetFormFieldInputHTML(module.Id, field, sysField, model, false, linkFields, copyModel, null, null, canOp, isCache, CurrUser, isRestartFlow) :
                                       "<span title=\"无该字段查看权限\" style=\"width:100%\">******</span>";
                    bool canEditOp = (model != null ? field.IsAllowEdit != false : field.IsAllowAdd != false) && canOp;
                    string colSpan = string.Empty;
                    string ipWidthStr = string.Format("{0}px", inputWidth.ToString());
                    int residueCs = 0; //剩余colspan数
                    if (row.Count() < maxCol)
                    {
                        int rn = (int)Math.Ceiling(width * 1.0 / ConstDefine.STANDARD_INPUTWIDTH);
                        int yn = 0; //已占用的td数
                        foreach (int key in dic.Keys)
                        {
                            yn += dic[key];
                        }
                        int currCs = (maxCol - yn) * 2 - 1; //允许colspan最大td数
                        int cn = rn * 2 - 1; //需要colspan的td数
                        if (cn >= currCs)
                        {
                            cn = currCs;
                            ipWidthStr = "100%";
                        }
                        else if (n == row.Count())
                        {
                            residueCs = currCs - cn;
                            if (residueCs > 0)
                            {
                                ipWidthStr = string.Format("{0}px;border-right-color:#fff", inputWidth.ToString());
                            }
                        }
                        dic.Add(n, rn);
                        colSpan = string.Format(" colspan=\"{0}\"", cn.ToString());
                    }
                    sb.AppendFormat("<td style=\"padding:2px;width:{0}\"{1}>", ipWidthStr, colSpan);
                    sb.Append(inputHtml);
                    sb.Append("</td>");
                    if (residueCs > 0)
                        sb.AppendFormat("<td style=\"width:100%;border-left-color:#fff\" colspan=\"{0}\">&nbsp;</td>", residueCs.ToString());
                    Type fieldType = SystemOperate.GetFieldType(module.Id, sysField.Name);
                    bool isCanNull = fieldType != null && fieldType.ToString().Contains("System.Nullable`1");
                    string fieldTypeStr = fieldType != null ? fieldType.ToString() : string.Empty;
                    bool isEnum = SystemOperate.IsEnumField(module.Id, sysField.Name);
                    string foreignModuleName = string.Empty;
                    bool isTitleKey = module.TitleKey == sysField.Name; //是否titlekey字段
                    string foreignTitleKey = string.Empty; //外键模块titleKey
                    //外键模块
                    Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                    if (foreignModule != null)
                    {
                        foreignModuleName = sysField.ForeignModuleName;
                        foreignTitleKey = foreignModule.TitleKey;
                        //添加外键name字段
                        fieldObjects.Add(new
                        {
                            Sys_FieldName = sysField.Name.Substring(0, sysField.Name.Length - 2) + "Name",
                            Display = sysField.Display,
                            ControlType = field.ControlType,
                            ForeignModuleName = foreignModuleName,
                            IsTitleKey = isTitleKey,
                            ForignFieldName = module.TableName + "Id",
                            ForeignTitleKey = foreignTitleKey,
                            IsCanNull = true,
                            IsEnum = false,
                            RowNo = field.RowNo,
                            ColNo = field.ColNo,
                            CanEdit = canEditOp,
                            FieldType = fieldTypeStr
                        });
                    }
                    fieldObjects.Add(new
                    {
                        Sys_FieldName = sysField.Name,
                        Display = sysField.Display,
                        ControlType = field.ControlType,
                        ForeignModuleName = foreignModuleName,
                        IsTitleKey = isTitleKey,
                        ForignFieldName = module.TableName + "Id",
                        ForeignTitleKey = foreignTitleKey,
                        IsCanNull = isCanNull,
                        IsEnum = isEnum,
                        RowNo = field.RowNo,
                        ColNo = field.ColNo,
                        CanEdit = canEditOp,
                        FieldType = fieldTypeStr
                    });
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// 返回通用编辑表单页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单编辑模式的网格Id</param>
        /// <param name="copyId">复制时被复制的记录Id</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="todoTaskId">待办任务ID</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public override string GetEditFormHTML(Guid moduleId, Guid? id, string gridId = null, Guid? copyId = null, bool showTip = false, Guid? todoTaskId = null, Guid? formId = null, HttpRequestBase request = null)
        {
            #region 前置验证
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            if (module == null) return string.Empty;
            UserInfo currUser = this.CurrUser;
            if (currUser == null) return UIOperate.GetAccountExpiredTipHtml();
            #endregion
            #region 页面重写
            string html = GetCustomerPageHTML(moduleId, "GetEditFormHTML", new object[] { id, gridId, copyId, showTip, todoTaskId, formId, request });
            if (!string.IsNullOrEmpty(html)) return html;
            #endregion
            #region 前置参数
            #region 表单按钮
            bool isParentTodo = todoTaskId.HasValue && BpmOperate.IsChildFlowParentTodo(todoTaskId.Value); //是否为父待办ID
            Sys_Form form = formId.HasValue && formId.Value != Guid.Empty ? SystemOperate.GetForm(formId.Value) : SystemOperate.GetUserFinalForm(currUser, moduleId, todoTaskId);
            if (form == null) return string.Empty;
            bool isDraft = false;
            int formWidth = 0; //表单宽度
            int formHeight = 0; //表单高度
            int editMode = todoTaskId.HasValue && todoTaskId.Value != Guid.Empty ? (int)ModuleEditModeEnum.TabFormEdit : GetEditMode(module, form, out formWidth, out formHeight, currUser.UserId);
            List<FormButton> buttons = SystemOperate.GetFormButtons(module, FormTypeEnum.EditForm, !id.HasValue, isDraft, id, todoTaskId, currUser);
            string btnsHtml = GetFormButtonHTML(module, buttons, editMode, gridId, currUser);
            string topBtnsHtml = string.Empty; //顶部按钮
            string bottomBtnsHtml = string.Empty;
            if (editMode == (int)ModuleEditModeEnum.TabFormEdit || (editMode == (int)ModuleEditModeEnum.GridRowBottomFormEdit && id.HasValue))
            {
                //顶部按钮
                if (form.ButtonLocationOfEnum == ButtonLocationEnum.Top)
                {
                    topBtnsHtml = btnsHtml;
                }
                //底部按钮
                else if (form.ButtonLocationOfEnum == ButtonLocationEnum.Bottom)
                {
                    bottomBtnsHtml = btnsHtml;
                }
                else
                {
                    topBtnsHtml = btnsHtml;
                    bottomBtnsHtml = btnsHtml;
                }
            }
            #endregion
            #region 表单tag
            string tagsHtml = string.Empty; //tag的html
            if (id.HasValue && id.Value != Guid.Empty)
            {
                //表单快捷工具按钮
                List<FormToolTag> toolTags = showTip ? SystemOperate.GetFormToolTags(module, FormTypeEnum.ViewForm, false, currUser) : new List<FormToolTag>();
                tagsHtml = GetFormToolTagsHTML(module, id, toolTags);
            }
            #endregion
            #region 表单附件
            string attachHtml = string.Empty; //附件html
            string topAttachHtml = string.Empty;
            string bottomAttachHtml = string.Empty;
            if (module.IsEnableAttachment)
            {
                attachHtml = GetAttachmentListHTML(module, id, FormTypeEnum.EditForm);
            }
            #endregion
            #region 明细处理
            //自定义表单明细编辑网格页面
            bool detailTopDisplay = module.DetailTopDisplay; //明细是否顶部显示
            bool hasDetail = SystemOperate.HasDetailModule(moduleId);
            object[] args = new object[] { id, detailTopDisplay, copyId, request };
            string detailHtml = GetCustomerPageHTML(moduleId, "GetEditDetailHTML", args); //明细html
            if (!string.IsNullOrWhiteSpace(detailHtml))
            {
                detailTopDisplay = args[1].ObjToBool();
            }
            if (string.IsNullOrWhiteSpace(detailHtml)) //没有自定义明细编辑页面
            {
                //走通用明细编辑
                #region 通用明细编辑网格
                if (hasDetail)
                {
                    List<Sys_Module> detailModules = SystemOperate.GetDetailModules(moduleId);
                    string condition = "{Id:" + Guid.Empty.ToString() + "}";
                    if (detailModules != null && detailModules.Count > 0)
                    {
                        StringBuilder detailTab = new StringBuilder();
                        if (!detailTopDisplay)
                        {
                            detailTab.AppendFormat("<div id=\"detailTab\" class=\"easyui-tabs\" data-options=\"border:true,tabHeight:{0}\" style=\"width:100%;min-height:150px;\">", ConstDefine.TAB_HEAD_HEIGHT);
                        }
                        foreach (Sys_Module detailModule in detailModules)
                        {
                            Guid tempId = Guid.Empty;
                            bool detailCopy = false;
                            if (id.HasValue && id.Value != Guid.Empty) //编辑页面
                            {
                                tempId = id.Value;
                            }
                            else if (copyId.HasValue && copyId.Value != Guid.Empty) //新增并且复制Id存在
                            {
                                tempId = copyId.Value;
                                detailCopy = detailModule.IsAllowCopy;
                            }
                            if (tempId != Guid.Empty) //构造过滤条件
                            {
                                Sys_Field conditionField = null;
                                List<Sys_Field> detailFields = SystemOperate.GetFieldInfos(detailModule.Id);
                                if (detailFields != null)
                                    conditionField = detailFields.Where(x => x.ForeignModuleName == module.Name && x.Name.EndsWith("Id")).FirstOrDefault();
                                if (conditionField != null)
                                    condition = "{" + conditionField.Name + ":" + tempId + "}";
                            }
                            detailTab.AppendFormat("<div title=\"{0}\">", detailModule.Name);
                            Dictionary<string, object> otherParams = new Dictionary<string, object>();
                            if (isParentTodo && BpmOperate.IsEnabledWorkflow(detailModule.Id))
                            {
                                otherParams.Add("p_todoId", todoTaskId.Value);
                                otherParams.Add("muti_select", true);
                            }
                            string editGridHtml = GetGridHTML(detailModule.Id, DataGridType.EditDetailGrid, condition, null, null, null, null, otherParams, detailCopy, null, null, false, request);
                            detailTab.Append(editGridHtml);
                            detailTab.Append("</div>");
                        }
                        if (!detailTopDisplay)
                        {
                            detailTab.Append("</div>");
                        }
                        detailHtml = detailTab.ToString();
                    }
                }
                #endregion
            }
            #endregion
            #region 审批信息
            bool isShowFullPath = UIOperate.IsJsShowFullPath(request);
            string domainPath = isShowFullPath ? Globals.GetBaseUrl() : "/";
            string approvalInfoHtml = string.Empty; //审批信息html
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(moduleId); //是否启用流程
            if (isEnabledFlow)
            {
                approvalInfoHtml = GetApprovalInfoAndOpinionsHtml(moduleId, id, todoTaskId, currUser, isParentTodo, domainPath);
            }
            #endregion
            #region 实体对象
            object model = null; //实体对象
            object copyModel = null; //被复制的实体对象
            if (id.HasValue && id.Value != Guid.Empty) //编辑页面
            {
                model = CommonOperate.GetFormData(moduleId, id.Value, FormTypeEnum.EditForm, currUser);
                if (model != null)
                {
                    Type modelType = SystemOperate.GetModelType(module.Id);
                    PropertyInfo pIsDraft = modelType.GetProperty("IsDraft");
                    isDraft = pIsDraft != null && pIsDraft.GetValue2(model, null).ObjToBool();
                }
            }
            else //新增页面
            {
                if (copyId.HasValue && copyId.Value != Guid.Empty) //复制时
                {
                    copyModel = CommonOperate.GetFormData(moduleId, copyId.Value, FormTypeEnum.EditForm, currUser);
                }
            }
            #endregion
            #region 表单字段
            List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id, false);
            if (form.FormFields == null && formFields.Count > 0)
            {
                form.FormFields = formFields;
            }
            int hiddenType = (int)ControlTypeEnum.HiddenBox;
            formFields = formFields.Where(x => x.ControlType != hiddenType).ToList();
            //字段权限
            List<string> viewFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.ViewField);
            if (viewFields == null || viewFields.Contains("-1"))
                viewFields = new List<string>();
            List<string> opFields = null;
            if (isParentTodo)
            {
                opFields = new List<string>() { string.Empty }; //子流程待办时无主模块表单字段编辑权限
            }
            else
            {
                opFields = model != null ? PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.EditField) :
                                        PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.AddField);
                if (opFields == null || opFields.Contains("-1"))
                    opFields = new List<string>();
            }
            #endregion
            #region 替换控件
            Func<string, string> action = (string tempHtml) =>
            {
                tempHtml = tempHtml.Replace("{topBtnsHtml}", topBtnsHtml).Replace("{bottomBtnsHtml}", bottomBtnsHtml)
                        .Replace("{tagsHtml}", tagsHtml).Replace("{attachHtml}", attachHtml)
                        .Replace("{detailHtml}", detailHtml).Replace("{approvalInfoHtml}", approvalInfoHtml);
                #region 替换字段控件
                if (GlobalSet.IsEnabledPageCache)
                {
                    foreach (Sys_FormField field in formFields)
                    {
                        Sys_Field sysField = field.TempSysField == null ? SystemOperate.GetFieldById(field.Sys_FieldId.Value) : field.TempSysField;
                        if (sysField == null) continue;
                        if (field.TempSysField == null) field.TempSysField = sysField;
                        string textValue = string.Empty;
                        string value = GetFormFieldInputValue(out textValue, module, field, sysField, model, copyModel);
                        if (string.IsNullOrEmpty(textValue))
                            textValue = value;
                        tempHtml = tempHtml.Replace("{" + sysField.Name + "}", value).Replace("{" + sysField.Name + "_textValue}", textValue);
                    }
                }
                #endregion
                if (model != null)
                {
                    string modelJson = HttpUtility.UrlEncode(JsonHelper.Serialize(model).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
                    tempHtml += string.Format("<input id=\"formDataObj\" type=\"hidden\" value=\"{0}\" />", modelJson);
                }
                return tempHtml;
            };
            #endregion
            #endregion
            #region 获取缓存key
            string cacheKey = string.Format("{0}_{1}", moduleId.ToString(), form.Id.ToString());
            if (!string.IsNullOrEmpty(gridId)) cacheKey += string.Format("_{0}", gridId);
            if (todoTaskId.HasValue) cacheKey += string.Format("_{0}", todoTaskId.Value);
            #endregion
            #region 取页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                string cacheHtml = SystemOperate.GetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.EditForm);
                if (!string.IsNullOrEmpty(cacheHtml))
                {
                    cacheHtml = action(cacheHtml); //替换相关控件
                    return cacheHtml;
                }
            }
            #endregion
            #region 生成页面
            string errMsg = string.Empty;
            List<object> fieldObjects = new List<object>(); //表单字段匿名对象集合
            StringBuilder sb = new StringBuilder();
            string titleKey = string.IsNullOrEmpty(module.TitleKey) ? string.Empty : module.TitleKey;
            string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(moduleId);
            sb.Append("<div id=\"divEditForm\" class=\"content\" style=\"display:block\">");
            //表单按钮
            sb.Append("{topBtnsHtml}");
            //表单tag
            sb.Append("{tagsHtml}");
            #region 主表表单
            //表单字段
            sb.Append("<form method=\"post\" id=\"mainform\">");
            var tabs = formFields.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.TabName).ToList();
            string tabCls = string.Empty;
            string tabHeight = "100%";
            string tabHeightKey = "height";
            string thHeightStr = string.Empty;
            if ((tabs.Count > 1 && tabs.Where(x => !string.IsNullOrEmpty(x.Key)).ToList().Count > 1) || (tabs.Count == 1 && !string.IsNullOrEmpty(formFields.FirstOrDefault().TabName)))
            {
                tabCls = "easyui-tabs";
            }
            //存在明细并且在顶部显示
            if (string.IsNullOrEmpty(tabCls) && (SystemOperate.HasDetailModule(moduleId) || !string.IsNullOrWhiteSpace(detailHtml)) && detailTopDisplay)
            {
                tabCls = "easyui-tabs";
            }
            if (!string.IsNullOrEmpty(tabCls))
            {
                int tmpH = formHeight;
                tabHeight = string.Format("{0}px", tmpH.ToString());
                tabHeightKey = "min-height";
                thHeightStr = string.Format(",tabHeight:{0}", ConstDefine.TAB_HEAD_HEIGHT.ToString());
            }
            sb.AppendFormat("<div id=\"editFormTabs\" class=\"{0}\" style=\"width:100%;{2}:{1};\" data-options=\"onSelect:OnEditFormTabSelect{3}\">", tabCls, tabHeight, tabHeightKey, thHeightStr);
            bool isRestartFlow = request != null && request["rsf"].ObjToInt() == 1;
            foreach (var tab in tabs)
            {
                string tabTitle = string.IsNullOrEmpty(tab.FirstOrDefault().TabName) ? "主信息" : tab.FirstOrDefault().TabName;
                sb.AppendFormat("<div title=\"{0}\">", tabTitle);
                sb.Append("<div id=\"mainContent\" class=\"content\">");
                var groups = tab.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.GroupName).ToList();
                foreach (var group in groups)
                {
                    var rows = group.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
                    string panelCls = string.Empty;
                    int pw = GetPanelWidth(group, form);
                    string panelWidth = "100%";
                    string tableWidth = string.Format("{0}px", pw.ToString());
                    if (!string.IsNullOrEmpty(group.FirstOrDefault().GroupName))
                    {
                        panelCls = "easyui-panel";
                    }
                    else
                    {
                        panelWidth = string.Format("{0}px", (pw + 20).ToString());
                    }
                    sb.AppendFormat("<div class=\"{0}\" title=\"{1}\" style=\"width:{2};padding:{3}px;margin-bottom:3px;\" data-options=\"collapsible:true\">", panelCls, group.FirstOrDefault().GroupName, panelWidth, ConstDefine.FORM_PANEL_PADDING.ToString());
                    string tableHtml = GetNewEditFormGroupTable(module, form, rows, formFields, tableWidth, model, copyModel, ref fieldObjects, viewFields, opFields, GlobalSet.IsEnabledPageCache, isRestartFlow);
                    sb.Append(tableHtml);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
                sb.Append("</div>");
            }
            #endregion
            if (!detailTopDisplay)
            {
                sb.Append("</div>");
                sb.Append("{attachHtml}");
            }
            sb.Append("{detailHtml}"); //添加明细页面
            if (detailTopDisplay)
            {
                sb.Append("</div>");
                sb.Append("{attachHtml}");
            }
            sb.Append("{approvalInfoHtml}");
            //表单按钮
            sb.Append("{bottomBtnsHtml}");
            sb.Append("</form>");
            //表单字段对象添加到隐藏域中
            string formFieldJson = HttpUtility.UrlEncode(JsonHelper.Serialize(fieldObjects).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
            sb.AppendFormat("<input id=\"hd_formFields\" type=\"hidden\" value=\"{0}\" />", formFieldJson);
            sb.Append("</div>");
            #region Js加载
            //打印JS
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery.jqprint-0.3.js\"></script>", domainPath);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery-migrate-1.2.1.min.js\"></script>", domainPath);
            //相关JS
            if (hasDetail)
            {
                //网格js
                string gridJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/Grid.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/Grid.js?r={1}\"></script>", domainPath, gridJsr);
            }
            //编辑表单js
            string editFormJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/EditForm.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/EditForm.js?r={1}\"></script>", domainPath, editFormJsr);
            //表单扩展js
            string fixedSerializeJsr = WebHelper.GetJsModifyTimeStr("/Scripts/extension/FormFixedSerialize.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/extension/FormFixedSerialize.js?r={1}\"></script>", domainPath, fixedSerializeJsr);
            //富文本框js
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/ueditor/ueditor.config.js\"></script>", domainPath);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/ueditor/ueditor.all.js\"></script>", domainPath);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/ueditor/lang/zh-cn/zh-cn.js\"></script>", domainPath);
            if (string.IsNullOrEmpty(gridId))
            {
                sb.Append(UIOperate.GetModelJsHTML(module, request));
            }
            #endregion
            #region 页面弹出框
            sb.Append("<div id=\"page_dialog1\"></div>");
            sb.Append("<div id=\"page_dialog2\"></div>");
            sb.Append("<div id=\"page_dialog3\"></div>");
            sb.Append("<div id=\"page_dialog4\"></div>");
            sb.Append("<div id=\"page_dialog5\"></div>");
            sb.Append("<div id=\"page_dialog6\"></div>");
            sb.Append("<div id=\"page_dialog7\"></div>");
            sb.Append("<div id=\"page_dialog8\"></div>");
            sb.Append("<div id=\"page_dialog9\"></div>");
            sb.Append("<div id=\"page_dialog10\"></div>");
            #endregion
            if (isShowFullPath)
            {
                sb.AppendFormat("<input id=\"userInfo\" type=\"hidden\" value=\"{0}\" />", HttpUtility.UrlEncode(JsonHelper.Serialize(currUser).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20"));
            }
            html = sb.ToString();
            #endregion
            #region 设置页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                SystemOperate.SetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.EditForm, html);
            }
            #endregion
            html = action(html);
            return html;
        }

        #endregion

        #region 查看表单

        /// <summary>
        /// 基于表格式Group查看表单Table
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="form">表单</param>
        /// <param name="rows">group内字段</param>
        /// <param name="tableWidth">group的panel宽</param>
        /// <param name="id">记录id</param>
        /// <param name="model">实体对象</param>
        /// <param name="isRecycle">是否来自回收站</param>
        /// <param name="fieldObjects">字段匿名对象集合</param>
        /// <param name="canViewFields">允许查看字段集合</param>
        /// <param name="canEditFields">允许编辑字段集合</param>
        /// <param name="hasRecoredEdit">是否有记录的编辑权限</param>
        /// <param name="isCache">是否来自缓存</param>
        /// <returns></returns>
        private string GetNewViewFormGroupTable(Sys_Module module, Sys_Form form, List<IGrouping<int, Sys_FormField>> rows, string tableWidth, Guid id, object model, bool isRecycle, ref List<object> fieldObjects, List<string> canViewFields, List<string> canEditFields, bool hasRecoredEdit, bool? isCache = false)
        {
            if (fieldObjects == null)
                fieldObjects = new List<object>();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<table class=\"thinBorder padding5\" border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:{0};table-layout:fixed;\">", tableWidth);
            int maxCol = 0;
            foreach (var row in rows)
            {
                int count = row.Count();
                if (maxCol < count)
                    maxCol = count;
            }
            foreach (var row in rows)
            {
                sb.AppendFormat("<tr style=\"height:{0}px;\">", ConstDefine.FORM_ROW_HEIGHT.ToString());
                int n = 0;
                Dictionary<int, int> dic = new Dictionary<int, int>(); //每个字段所占标准宽字段倍数
                foreach (Sys_FormField field in row)
                {
                    if (!field.Sys_FieldId.HasValue)
                        continue;
                    n++;
                    Sys_Field sysField = field.TempSysField == null ? SystemOperate.GetFieldById(field.Sys_FieldId.Value) : field.TempSysField;
                    if (sysField == null) continue;
                    if (field.TempSysField == null) field.TempSysField = sysField;
                    int labelWidth = form.LabelWidth > 0 ? form.LabelWidth : ConstDefine.STANDARD_LABEL;
                    int inputWidth = (field.Width.HasValue ? field.Width.Value : (form.InputWidth > 0 ? form.InputWidth : ConstDefine.STANDARD_CONTROL));
                    int width = labelWidth + inputWidth;
                    //label
                    string splitChar = string.IsNullOrEmpty(form.RightToken) ? "：" : form.RightToken;
                    sb.AppendFormat("<th style=\"padding:2px;width:{0}px;text-align:right\">{1}{2}</th>", labelWidth.ToString(), sysField.Display, splitChar);
                    //data
                    bool canView = canViewFields == null || canViewFields.Count == 0 || canViewFields.Contains(sysField.Name);
                    bool canEdit = canEditFields == null || canEditFields.Count == 0 || canEditFields.Contains(sysField.Name);
                    string valueSpan = isCache == true ? "{" + sysField.Name + "}" : GetFormFieldViewValue(module, form, field, sysField, model, id, canView, canEdit, hasRecoredEdit);
                    string colSpan = string.Empty;
                    string ipWidthStr = string.Format("{0}px", inputWidth.ToString());
                    int residueCs = 0; //剩余colspan数
                    if (row.Count() < maxCol)
                    {
                        int rn = (int)Math.Ceiling(width * 1.0 / ConstDefine.STANDARD_INPUTWIDTH);
                        int yn = 0; //已占用的td数
                        foreach (int key in dic.Keys)
                        {
                            yn += dic[key];
                        }
                        int currCs = (maxCol - yn) * 2 - 1; //允许colspan最大td数
                        int cn = rn * 2 - 1; //需要colspan的td数
                        if (cn >= currCs)
                        {
                            cn = currCs;
                            ipWidthStr = "100%";
                        }
                        else if (n == row.Count())
                        {
                            residueCs = currCs - cn;
                            if (residueCs > 0)
                            {
                                ipWidthStr = string.Format("{0}px;border-right-color:#fff", inputWidth.ToString());
                            }
                        }
                        dic.Add(n, rn);
                        colSpan = string.Format(" colspan=\"{0}\"", cn.ToString());
                    }
                    sb.AppendFormat("<td style=\"padding:2px;width:{0}\"{1}>{2}</td>", ipWidthStr, colSpan, valueSpan);
                    if (residueCs > 0)
                        sb.AppendFormat("<td style=\"width:100%;border-left-color:#fff\" colspan=\"{0}\">&nbsp;</td>", residueCs.ToString());
                    Type fieldType = SystemOperate.GetFieldType(module.Id, sysField.Name);
                    string fieldTypeStr = fieldType != null ? fieldType.ToString() : string.Empty;
                    string foreignModuleName = string.Empty;
                    bool isTitleKey = module.TitleKey == sysField.Name; //是否titlekey字段
                    string foreignTitleKey = string.Empty; //外键模块titleKey
                    //外键模块
                    Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                    if (foreignModule != null)
                    {
                        foreignModuleName = sysField.ForeignModuleName;
                        foreignTitleKey = foreignModule.TitleKey;
                        //添加外键name字段
                        fieldObjects.Add(new
                        {
                            Sys_FieldName = sysField.Name.Substring(0, sysField.Name.Length - 2) + "Name",
                            Display = sysField.Display,
                            ControlType = field.ControlType,
                            ForeignModuleName = foreignModuleName,
                            IsTitleKey = isTitleKey,
                            ForignFieldName = module.TableName + "Id",
                            ForeignTitleKey = foreignTitleKey,
                            FieldType = fieldTypeStr
                        });
                    }
                    fieldObjects.Add(new
                    {
                        Sys_FieldName = sysField.Name,
                        Display = sysField.Display,
                        ControlType = field.ControlType,
                        ForeignModuleName = foreignModuleName,
                        IsTitleKey = isTitleKey,
                        ForignFieldName = module.TableName + "Id",
                        ForeignTitleKey = foreignTitleKey,
                        FieldType = fieldTypeStr
                    });
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            return sb.ToString();
        }

        /// <summary>
        /// 查看页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="gridId">为网格表单查看模式的网格Id</param>
        /// <param name="fromEditPageFlag">从编辑页面点击查看按钮标识</param>
        /// <param name="isRecycle">是否来自回收站</param>
        /// <param name="showTip">是否显示表单tip按钮</param>
        /// <param name="formId">指定表单ID</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public override string GetViewFormHTML(Guid moduleId, Guid id, string gridId = null, string fromEditPageFlag = null, bool isRecycle = false, bool showTip = false, Guid? formId = null, HttpRequestBase request = null)
        {
            #region 前置验证
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            if (module == null) return string.Empty;
            UserInfo currUser = this.CurrUser;
            if (currUser == null) return UIOperate.GetAccountExpiredTipHtml();
            #endregion
            #region 页面重写
            string html = GetCustomerPageHTML(moduleId, "GetViewFormHTML", new object[] { id, gridId, fromEditPageFlag, isRecycle, showTip, formId, request });
            if (!string.IsNullOrEmpty(html)) return html;
            #endregion
            #region 参数准备
            Sys_Form form = formId.HasValue && formId.Value != Guid.Empty ? SystemOperate.GetForm(formId.Value) : SystemOperate.GetDefaultForm(moduleId);
            if (form == null) return string.Empty;
            Type modelType = CommonOperate.GetModelType(moduleId);
            #region 实体对象
            string titleKey = module.TitleKey.ObjToStr();
            object model = fromEditPageFlag == "true" ? null : CommonOperate.GetFormData(moduleId, id, FormTypeEnum.ViewForm, currUser);
            string titleKeyValue = string.Empty; //titleKey字段值
            if (model != null && !string.IsNullOrEmpty(titleKey))
                titleKeyValue = CommonOperate.GetModelFieldValueByModel(moduleId, model, module.TitleKey).ObjToStr();
            #endregion
            #region 表单按钮
            string btnsHtml = string.Empty; //按钮html
            int formWidth = 0; //表单宽度
            int formHeight = 0; //表单高度
            int editMode = GetEditMode(module, form, out formWidth, out formHeight, currUser.UserId);
            List<FormButton> buttons = SystemOperate.GetFormButtons(module, FormTypeEnum.ViewForm, false, false, id, null, currUser);
            btnsHtml = GetFormButtonHTML(module, buttons, editMode, gridId, currUser, id, titleKeyValue);
            string topBtnsHtml = string.Empty; //顶部按钮
            string bottomBtnsHtml = string.Empty; //底部按钮
            if (editMode == (int)ModuleEditModeEnum.TabFormEdit)
            {
                //顶部按钮
                if (form.ButtonLocationOfEnum == ButtonLocationEnum.Top)
                {
                    topBtnsHtml = btnsHtml;
                }
                //底部按钮
                else if (form.ButtonLocationOfEnum == ButtonLocationEnum.Bottom)
                {
                    bottomBtnsHtml = btnsHtml;
                }
                else
                {
                    topBtnsHtml = btnsHtml;
                    bottomBtnsHtml = btnsHtml;
                }
            }
            #endregion
            #region 表单tag
            string tagsHtml = string.Empty; //tag的html
            //表单快捷工具按钮
            List<FormToolTag> toolTags = showTip ? SystemOperate.GetFormToolTags(module, FormTypeEnum.ViewForm, false, currUser) : new List<FormToolTag>();
            tagsHtml = GetFormToolTagsHTML(module, id, toolTags);
            #endregion
            #region 表单附件
            string attachHtml = string.Empty; //附件html
            if (fromEditPageFlag != "true" && module.IsEnableAttachment)
                attachHtml = GetAttachmentListHTML(module, id, FormTypeEnum.ViewForm);
            #endregion
            #region 明细处理
            List<Sys_Module> detailModules = null; //明细模块集合
            List<Sys_Module> attachModules = SystemOperate.GetUserBindAttachModules(currUser.UserId, moduleId);
            bool detailTopDisplay = module.DetailTopDisplay; //明细是否顶部显示
            bool hasDetail = fromEditPageFlag.ObjToInt() != 1 && (SystemOperate.HasDetailModule(moduleId) || attachModules.Count > 0);
            object[] args = new object[] { id, detailTopDisplay, request };
            string detailHtml = GetCustomerPageHTML(moduleId, "GetViewDetailHTML", args);
            if (!string.IsNullOrWhiteSpace(detailHtml))
            {
                detailTopDisplay = args[1].ObjToBool();
            }
            if (string.IsNullOrWhiteSpace(detailHtml)) //没有自定义明细编辑页面
            {
                #region 通用明细查查看网格页面
                if (hasDetail)
                {
                    detailModules = SystemOperate.GetDetailModules(moduleId);
                    detailModules.AddRange(attachModules);
                    string condition = "{Id:" + Guid.Empty.ToString() + "}";
                    if (detailModules.Count > 0)
                    {
                        StringBuilder detailTab = new StringBuilder();
                        if (!detailTopDisplay)
                        {
                            detailTab.AppendFormat("<div id=\"detailTab\" class=\"easyui-tabs\" data-options=\"border:true,tabHeight:{0}\" style=\"min-height:150px;\">", ConstDefine.TAB_HEAD_HEIGHT.ToString());
                        }
                        foreach (Sys_Module detailModule in detailModules)
                        {
                            Sys_Field conditionField = null;
                            List<Sys_Field> detailFields = SystemOperate.GetFieldInfos(detailModule.Id);
                            if (detailFields != null)
                                conditionField = detailFields.Where(x => x.ForeignModuleName == module.Name && x.Name.EndsWith("Id")).FirstOrDefault();
                            if (conditionField != null)
                                condition = "{" + conditionField.Name + ":" + id.ToString() + "}";
                            detailTab.AppendFormat("<div title=\"{0}\">", detailModule.Name);
                            string editGridHtml = GetGridHTML(detailModule.Id, DataGridType.ViewDetailGrid, condition, null, null, null, null, null, false, null, null, false, request);
                            detailTab.Append(editGridHtml);
                            detailTab.Append("</div>");
                        }
                        if (!detailTopDisplay)
                        {
                            detailTab.Append("</div>");
                        }
                        detailHtml = detailTab.ToString();
                    }
                }
                #endregion
            }
            #endregion
            #region 审批信息
            string approvalInfoHtml = string.Empty; //审批信息html
            bool isEnabledFlow = BpmOperate.IsEnabledWorkflow(moduleId); //是否启用流程
            if (isEnabledFlow)
            {
                bool isParentTodo = false;
                if (detailModules != null && detailModules.Count > 0)
                {
                    isParentTodo = detailModules.Where(x => BpmOperate.IsEnabledWorkflow(x.Id)).Count() > 0;
                }
                approvalInfoHtml = GetApprovalInfoAndOpinionsHtml(moduleId, id, null, currUser, isParentTodo);
            }
            #endregion
            #region 表单字段
            List<Sys_FormField> formFields = form.FormFields != null && form.FormFields.Count > 0 ? form.FormFields : SystemOperate.GetFormField(form.Id, false);
            if (form.FormFields == null && formFields.Count > 0)
            {
                form.FormFields = formFields;
            }
            int hiddenType = (int)ControlTypeEnum.HiddenBox;
            formFields = formFields.Where(x => x.ControlType != hiddenType).ToList();
            //字段权限
            List<string> viewFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.ViewField);
            List<string> opFields = new List<string>();
            if (viewFields == null || viewFields.Contains("-1"))
                viewFields = new List<string>();
            if (fromEditPageFlag.ObjToInt() != 1 && !ModelConfigHelper.ModelIsViewMode(modelType))
            {
                opFields = PermissionOperate.GetUserFieldsPermissions(currUser.UserId, moduleId, FieldPermissionTypeEnum.EditField);
                if (opFields == null || opFields.Contains("-1"))
                    opFields = new List<string>();
            }
            else
            {
                opFields = new List<string>() { "0" };
            }
            bool hasEditPower = model != null && !isRecycle ? PermissionOperate.UserHasOperateRecordPermission(currUser.UserId, module.Id, id, DataPermissionTypeEnum.EditData) : true;
            #endregion
            #region 替换字段值
            Func<string, string> action = (string tempHtml) =>
            {
                tempHtml = tempHtml.Replace("{topBtnsHtml}", topBtnsHtml).Replace("{bottomBtnsHtml}", bottomBtnsHtml)
                        .Replace("{tagsHtml}", tagsHtml).Replace("{attachHtml}", attachHtml)
                        .Replace("{detailHtml}", detailHtml).Replace("{approvalInfoHtml}", approvalInfoHtml);
                #region 替换字段控件
                if (GlobalSet.IsEnabledPageCache)
                {
                    foreach (Sys_FormField field in formFields)
                    {
                        Sys_Field sysField = field.TempSysField == null ? SystemOperate.GetFieldById(field.Sys_FieldId.Value) : field.TempSysField;
                        if (sysField == null) continue;
                        if (field.TempSysField == null) field.TempSysField = sysField;
                        bool canView = viewFields == null || viewFields.Count == 0 || viewFields.Contains(sysField.Name);
                        bool canEdit = opFields == null || opFields.Count == 0 || opFields.Contains(sysField.Name);
                        string value = GetFormFieldViewValue(module, form, field, sysField, model, id, canView, canEdit, hasEditPower);
                        tempHtml = tempHtml.Replace("{" + sysField.Name + "}", value);
                    }
                }
                #endregion
                if (model != null)
                {
                    string modelJson = HttpUtility.UrlEncode(JsonHelper.Serialize(model).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
                    tempHtml += string.Format("<input id=\"formDataObj\" type=\"hidden\" value=\"{0}\" />", modelJson);
                }
                return tempHtml;
            };
            #endregion
            #endregion
            #region 获取缓存key
            string cacheKey = string.Format("{0}_{1}", moduleId.ToString(), form.Id.ToString());
            if (!string.IsNullOrEmpty(gridId)) cacheKey += string.Format("_{0}", gridId);
            #endregion
            #region 取页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                string cacheHtml = SystemOperate.GetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.ViewForm);
                if (!string.IsNullOrEmpty(cacheHtml))
                {
                    cacheHtml = action(cacheHtml); //替换相关控件
                    return cacheHtml;
                }
            }
            #endregion
            #region 生成页面
            //通用处理
            string errMsg = string.Empty;
            List<object> fieldObjects = new List<object>(); //表单字段匿名对象集合
            StringBuilder sb = new StringBuilder();
            string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(moduleId);
            sb.Append("<div id=\"divEditForm\" class=\"content\" style=\"display:block\">");
            //顶部表单按钮
            sb.Append("{topBtnsHtml}");
            //表单tag
            sb.Append("{tagsHtml}");
            #region 主表表单
            var tabs = formFields.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.TabName).ToList();
            string tabCls = string.Empty;
            string tabHeight = "100%";
            string tabHeightKey = "height";
            string thHeightStr = string.Empty;
            if (tabs.Count > 1 || (tabs.Count == 1 && !string.IsNullOrEmpty(formFields.FirstOrDefault().TabName)))
            {
                tabCls = "easyui-tabs";
            }
            //存在明细并且在顶部显示
            if (string.IsNullOrEmpty(tabCls) && (SystemOperate.HasDetailModule(moduleId) || !string.IsNullOrWhiteSpace(detailHtml)) && detailTopDisplay)
            {
                tabCls = "easyui-tabs";
            }
            if (!string.IsNullOrEmpty(tabCls))
            {
                int tmpH = formHeight;
                tabHeight = string.Format("{0}px", tmpH.ToString());
                tabHeightKey = "min-height";
                thHeightStr = string.Format("data-options=\"tabHeight:{0}\"", ConstDefine.TAB_HEAD_HEIGHT.ToString());
            }
            sb.AppendFormat("<div class=\"{0}\" style=\"width:100%;{2}:{1};\" {3}>", tabCls, tabHeight, tabHeightKey, thHeightStr);
            foreach (var tab in tabs)
            {
                string tabTitle = string.IsNullOrEmpty(tab.FirstOrDefault().TabName) ? "主信息" : tab.FirstOrDefault().TabName;
                sb.AppendFormat("<div title=\"{0}\">", tabTitle);
                sb.Append("<div class=\"content\">");
                var groups = tab.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.GroupName).ToList();
                foreach (var group in groups)
                {
                    var rows = group.OrderBy(x => x.RowNo).ThenBy(x => x.ColNo).GroupBy(x => x.RowNo).ToList();
                    string panelCls = string.Empty;
                    int pw = GetPanelWidth(group, form);
                    string panelWidth = "100%";
                    string tableWidth = string.Format("{0}px", pw.ToString());
                    if (!string.IsNullOrEmpty(group.FirstOrDefault().GroupName))
                    {
                        panelCls = "easyui-panel";
                    }
                    else
                    {
                        panelWidth = string.Format("{0}px", (pw + 20).ToString());
                    }
                    sb.AppendFormat("<div class=\"{0}\" title=\"{1}\" style=\"width:{2};padding:{3}px;margin-bottom:3px;\" data-options=\"collapsible:true\">", panelCls, group.FirstOrDefault().GroupName, panelWidth, ConstDefine.FORM_PANEL_PADDING.ToString());
                    string tableHtml = GetNewViewFormGroupTable(module, form, rows, tableWidth, id, model, isRecycle, ref fieldObjects, viewFields, opFields, hasEditPower, GlobalSet.IsEnabledPageCache);
                    sb.Append(tableHtml);
                    sb.Append("</div>");
                }
                sb.Append("</div>");
                sb.Append("</div>");
            }
            #endregion
            if (!detailTopDisplay)
            {
                sb.Append("</div>");
                sb.Append("{attachHtml}"); //附件
            }
            sb.Append("{detailHtml}"); //明细网格
            if (detailTopDisplay)
            {
                sb.Append("</div>");
                sb.Append("{attachHtml}"); //附件
            }
            sb.Append("{approvalInfoHtml}"); //审批信息
            //底部表单按钮
            sb.Append("{bottomBtnsHtml}");
            //表单字段对象添加到隐藏域中
            string formFieldJson = HttpUtility.UrlEncode(JsonHelper.Serialize(fieldObjects).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
            sb.AppendFormat("<input id=\"hd_formFields\" type=\"hidden\" value=\"{0}\" />", formFieldJson);
            sb.Append("</div>");
            bool isShowFullPath = UIOperate.IsJsShowFullPath(request);
            string domainPath = isShowFullPath ? Globals.GetBaseUrl() : "/";
            //相关JS
            //打印JS
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery.jqprint-0.3.js\"></script>", domainPath);
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/jquery-plug/jquery-migrate-1.2.1.min.js\"></script>", domainPath);
            if (hasDetail)
            {
                //网格js
                string gridJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/Grid.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/Grid.js?r={1}\"></script>", domainPath, gridJsr);
            }
            //编辑表单js
            string viewFormJsr = WebHelper.GetJsModifyTimeStr("/Scripts/common/ViewForm.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}Scripts/common/ViewForm.js?r={0}\"></script>", domainPath, viewFormJsr);
            if (string.IsNullOrEmpty(gridId))
            {
                //模块自定义js
                sb.Append(UIOperate.GetModelJsHTML(module, request));
            }
            #region 页面弹出框
            sb.Append("<div id=\"page_dialog1\"></div>");
            sb.Append("<div id=\"page_dialog2\"></div>");
            sb.Append("<div id=\"page_dialog3\"></div>");
            sb.Append("<div id=\"page_dialog4\"></div>");
            sb.Append("<div id=\"page_dialog5\"></div>");
            sb.Append("<div id=\"page_dialog6\"></div>");
            sb.Append("<div id=\"page_dialog7\"></div>");
            sb.Append("<div id=\"page_dialog8\"></div>");
            sb.Append("<div id=\"page_dialog9\"></div>");
            sb.Append("<div id=\"page_dialog10\"></div>");
            #endregion
            html = sb.ToString();
            #endregion
            #region 设置页面缓存
            if (GlobalSet.IsEnabledPageCache)
            {
                SystemOperate.SetPageCacheHtml(moduleId, cacheKey, CachePageTypeEnum.ViewForm, html);
            }
            #endregion
            html = action(html);
            return html;
        }

        #endregion

        #region 其他

        /// <summary>
        /// 获取附件上传表单
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public override string GetUploadMitiFileHTML(Guid? moduleId)
        {
            StringBuilder sb = new StringBuilder();
            string attachUploadJs = UIOperate.FormatJsPath("/Scripts/common/AttachUpload.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", attachUploadJs);
            sb.Append("<input id=\"attachFile\" type=\"hidden\" />");
            sb.Append("<form method=\"post\" id=\"form_Upload\" enctype=\"multipart/form-data\">");
            sb.Append("<table style=\"width: 100%;margin-top:5px;\">");
            sb.Append("<tbody>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</form>");
            sb.Append("<div style=\"width:100%;height:30px;line-height:30px;text-align:center;margin-left:-3px;margin-top:8px;\">");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取编辑字段页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="recordId">记录Id</param>
        /// <returns></returns>
        public override string GetEditFieldHTML(Guid moduleId, string fieldName, Guid recordId)
        {
            UserInfo currUser = CurrUser;
            if (!PermissionOperate.CanEditField(currUser.UserId, moduleId, fieldName))
            {
                return "<div style=\"padding-top:20px;width:100%;text-align:center\"><font style=\"color:red;font-size:14px\">您没有该字段的编辑权限！</font>";
            }
            if (!PermissionOperate.UserHasOperateRecordPermission(currUser.UserId, moduleId, recordId, DataPermissionTypeEnum.EditData))
            {
                return "<div style=\"padding-top:20px;width:100%;text-align:center\"><font style=\"color:red;font-size:14px\">您没有该记录的编辑权限！</font>";
            }
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            StringBuilder sb = new StringBuilder();
            Sys_Field sysField = SystemOperate.GetFieldInfo(moduleId, fieldName);
            Sys_FormField field = SystemOperate.GetNfDefaultFormSingleField(sysField);
            if (field != null && field.IsAllowEdit.HasValue && field.IsAllowEdit.Value)
            {
                string errMsg = string.Empty;
                object model = CommonOperate.GetEntityById(moduleId, recordId, out errMsg);
                sb.Append("<form method=\"post\" id=\"editFieldForm\">");
                sb.Append("<div id=\"mainContent\">");
                sb.Append("<div style=\"width:100%;height:100%;padding:10px;\">");
                sb.Append("<div class=\"content\">");
                sb.Append("<table border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:100%;table-layout:fixed;\">");
                sb.Append("<tr><td style=\"height:150px;vertical-align:top;\">");
                int labelWidth = 90;
                if (field.Sys_FormId.HasValue)
                {
                    Sys_Form form = SystemOperate.GetForm(field.Sys_FormId.Value);
                    if (form != null && form.LabelWidth > 0)
                    {
                        labelWidth = form.LabelWidth;
                    }
                }
                int inputWidth = field.Width.HasValue && field.Width.Value > 0 ? field.Width.Value : 180;
                int width = labelWidth + inputWidth;
                sb.AppendFormat("<span style=\"padding:2px;width:{0}px;\">", width.ToString());
                //label
                sb.AppendFormat("<span id=\"lable_{0}\" style=\"float:left;width:{1}px;height:{2}px;line-height:{2}px;text-align:left;\">{3}:</span>",
                    sysField.Name, labelWidth.ToString(), ConstDefine.FORM_CONTROL_HEIGHT.ToString(), sysField.Display);
                //input span
                string inputMarginTop = "0px";
                if (field.ControlTypeOfEnum == ControlTypeEnum.SingleCheckBox ||
                    field.ControlTypeOfEnum == ControlTypeEnum.MutiCheckBox)
                {
                    inputMarginTop = "3px";
                }
                string title = string.Empty;
                sb.AppendFormat("<span style=\"float:left;width:{0}px;height:{1}px;line-height:{1}px;margin-top:{2};\" title=\"{3}\">", inputWidth.ToString(), ConstDefine.FORM_CONTROL_HEIGHT.ToString(), inputMarginTop, title);
                string inputHtml = GetFormFieldInputHTML(moduleId, field, sysField, model, false, null, null, null, null, true, false, currUser);
                sb.Append(inputHtml);
                sb.Append("</span>");
                sb.Append("</span>");
                sb.Append("</td></tr>");
                sb.Append("</table>");
                sb.Append("</div></div>");
                sb.Append("</div></form>");
                //添加表单字段JSON对象到隐藏域
                string foreignModuleName = string.Empty;
                bool isTitleKey = module.TitleKey == sysField.Name; //是否titlekey字段
                string foreignTitleKey = string.Empty; //外键模块titleKey
                //外键模块
                Sys_Module foreignModule = SystemOperate.GetModuleByName(sysField.ForeignModuleName);
                if (foreignModule != null)
                {
                    foreignModuleName = sysField.ForeignModuleName;
                    foreignTitleKey = foreignModule.TitleKey;
                }
                object fieldObject = new
                    {
                        Sys_FieldName = sysField.Name,
                        Display = sysField.Display,
                        ControlType = field.ControlType,
                        ForeignModuleName = foreignModuleName,
                        IsTitleKey = isTitleKey,
                        ForignFieldName = module.TableName + "Id",
                        ForeignTitleKey = foreignTitleKey
                    };
                //表单字段对象添加到隐藏域中
                string formFieldJson = HttpUtility.UrlEncode(JsonHelper.Serialize(fieldObject).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20");
                sb.AppendFormat("<input id=\"hd_formField\" type=\"hidden\" value=\"{0}\" />", formFieldJson);
                //表单扩展js
                string fixedSerializeJs = UIOperate.FormatJsPath("/Scripts/extension/FormFixedSerialize.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", fixedSerializeJs);
                string editFieldJs = UIOperate.FormatJsPath("/Scripts/common/EditField.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", editFieldJs);
                return sb.ToString();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取批量编辑页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="selectRecords">选择的记录数</param>
        /// <param name="pageRecords">当前页记录数</param>
        /// <returns></returns>
        public override string GetBatchEditHTML(Guid moduleId, int selectRecords, int pageRecords)
        {
            StringBuilder sb = new StringBuilder();
            UserInfo currUser = CurrUser;
            string errMsg = string.Empty;
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<div class=\"easyui-panel\" title=\"变更范围设置\" style=\"padding: 10px;\">");
            sb.Append("<table style=\"width: 100%;\">");
            sb.Append("<tr>");
            sb.AppendFormat("<td><input name=\"rdItem\" type=\"radio\" value=\"1\" checked=\"checked\" /><span>当前选中(<span id=\"spanSelected\" style=\"color: red\">{0}</span>)</span></td>", selectRecords.ToString());
            sb.AppendFormat("<td><input name=\"rdItem\" type=\"radio\" value=\"2\" /><span>当前页面(<span id=\"spanPage\" style=\"color: red\">{0}</span>)</span></td>", pageRecords.ToString());
            long totalCount = CommonOperate.Count(out errMsg, moduleId); //总记录数
            sb.AppendFormat("<td><input name=\"rdItem\" type=\"radio\" value=\"3\" /><span>所有记录(<span id=\"spanAll\" style=\"color: red\">{0}</span>)</span></td>", totalCount.ToString());
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<div class=\"easyui-panel\" title=\"变更字段选择\" style=\"padding: 10px;\">");
            sb.Append("<table style=\"width: 100%;\">");
            //允许批量编辑的字段
            List<Sys_FormField> batchEditFields = SystemOperate.GetDefaultBatchEditFields(moduleId);
            for (int i = 0; i < batchEditFields.Count; i++)
            {
                int r = i / 4; //当前所在行
                int c = i % 4; //当前所在列
                if (i == 0)
                {
                    sb.Append("<tr>");
                }
                else if (c == 0)
                {
                    sb.Append("</tr><tr>");
                }
                Sys_FormField field = batchEditFields[i];
                sb.AppendFormat("<td><input id=\"chk_{0}\" type=\"checkbox\" value=\"{0}\" /><span>{1}</span></td>", field.Sys_FieldName, field.Display);
            }
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<div class=\"easyui-panel\" title=\"编辑表单\" style=\"padding: 10px;\">");
            sb.Append("<form method=\"post\" id=\"batchEditForm\">");
            sb.Append("<div id=\"mainContent\">");
            sb.Append("<table style=\"width: 100%;line-height:30px;\">");
            foreach (Sys_FormField field in batchEditFields)
            {
                Sys_Field sysField = SystemOperate.GetFieldById(field.Sys_FieldId.Value);
                sb.AppendFormat("<tr id=\"tr_{0}\" style=\"display:none;\">", sysField.Name);
                sb.AppendFormat("<td style=\"width:100px;\">{0}：</td>", sysField.Display);
                sb.Append("<td style=\"width:420px;\">");
                string inputHtml = GetFormFieldInputHTML(moduleId, field, sysField, null, false, null, null, null, null, true, false, currUser);
                sb.Append(inputHtml);
                sb.Append("</td>");
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</form>");
            sb.Append("</div>");
            sb.Append("</div>");
            //表单扩展js
            string fixedSerializeJs = UIOperate.FormatJsPath("/Scripts/extension/FormFixedSerialize.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", fixedSerializeJs);
            string batchEditJs = UIOperate.FormatJsPath("/Scripts/common/BatchEdit.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", batchEditJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取实体导入页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public override string GetImportModelHTML(Guid moduleId)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<form method=\"post\" id=\"form_Import\" enctype=\"multipart/form-data\">");
            sb.Append("<table style=\"width: 100%;line-height:30px;\">");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<input type=\"file\" id=\"file\" name=\"file\" style=\"width:96%;margin-left:10px;\" />");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<img title=\"点击下载导入模板\" moduleId=\"{0}\" style=\"cursor:pointer;width:32px;height:32px;\" src=\"/CSS/icons/excel_32.png\" onclick=\"DownImportTemp(this)\" />", moduleId);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<span style=\"margin-left:10px;\">导入说明：</span></br>");
            sb.Append("<span style=\"color:red;margin-left:10px;\">");
            sb.Append("1、数据导入以模板文件为准，导入模板请点击右边excel图标下载。");
            sb.Append("</span></br>");
            sb.Append("<span style=\"color:red;margin-left:10px;\">");
            sb.Append("2、系统只能识别在模板中出现的列头字段，如果需要增加导入字段，请联系管理员开通相应的字段权限后重新下载导入模板。");
            sb.Append("</span></br>");
            sb.Append("<span style=\"color:red;margin-left:10px;\">");
            List<Sys_Field> sysFields = SystemOperate.GetPrimaryKeyFields(moduleId);
            string displayDes = "【" + string.Join("】,【", sysFields.Select(x => x.Display)) + "】";
            sb.AppendFormat("3、导入时以字段{0}作为唯一记录标识，在数据表中如果存在相应的记录则更新记录。", displayDes);
            sb.Append("</span>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</form>");
            sb.Append("</div>");
            string importModelJs = UIOperate.FormatJsPath("/Scripts/common/ImportModel.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", importModelJs);
            //模块自定义js
            sb.Append(UIOperate.GetModelJsHTML(moduleId, null));
            return sb.ToString();
        }

        /// <summary>
        /// 获取导出设置页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="cc">当前记录数</param>
        /// <returns></returns>
        public override string GetExportModelHTML(Guid moduleId, int cc)
        {
            StringBuilder sb = new StringBuilder();
            object methodObj = SystemOperate.GetEnumTypeList(typeof(ConditionMethodEnum));
            string json = JsonHelper.Serialize(methodObj).Replace("\"", "'");
            string errMsg = string.Empty;
            long count = CommonOperate.Count(out errMsg, moduleId, true, null, null, null, null, CurrUser);
            List<Sys_GridField> gridFields = SystemOperate.GetUserGridFields(CurrUser.UserId, moduleId);
            if (gridFields.Count > 0)
            {
                gridFields = gridFields.Where(x => x.Sys_FieldName != "Id" && x.IsVisible).ToList();
                List<object> fieldObjs = new List<object>();
                foreach (Sys_GridField field in gridFields)
                {
                    object obj = SystemOperate.GetFieldCommonInfo(moduleId, field.Sys_FieldName);
                    if (obj == null) continue;
                    fieldObjs.Add(obj);
                }
                string fieldJson = HttpUtility.UrlEncode(JsonHelper.Serialize(fieldObjs)).Replace("+", "%20"); ;
                sb.AppendFormat("<input id=\"fieldInfos\" type=\"hidden\" value=\"{0}\" />", fieldJson);
            }
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<table style=\"width:100%;height:100%;line-height:30px;\">");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<table style=\"width:420px;\"><tr>");
            sb.AppendFormat("<td><input id=\"rd1\" name=\"rd_select\" type=\"radio\" checked=\"checked\" value=\"0\" /><span style=\"cursor:pointer;\" onclick=\"$('#rd1').click();\" title=\"导出当前列表搜索的数据\">导出当前(<span style=\"color:red;\">{0}</span>)</span></td>", cc);
            sb.AppendFormat("<td><input id=\"rd2\" name=\"rd_select\" type=\"radio\" value=\"1\" /><span style=\"cursor:pointer;\" onclick=\"$('#rd2').click();\" title=\"导出所有数据\">导出全部(<span style=\"color:red;\">{0}</span>)</span></td>", count);
            sb.Append("<td><input id=\"rd3\" name=\"rd_select\" type=\"radio\" value=\"2\" /><span style=\"cursor:pointer;\" onclick=\"$('#rd3').click();\">按条件导出</span></td>");
            sb.Append("</tr></table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<div id=\"div_condition\" style=\"width:100%;\">");
            sb.Append("<table style=\"width:564px;height:160px;\" id=\"conditionGrid\" class=\"easyui-datagrid\" data-options=\"title:'条件设置',onSelect:function(rowIndex, rowData){$('#conditionGrid').datagrid('unselectAll');}\">");
            sb.Append("<thead><tr>");
            sb.Append("<th data-options=\"field:'Field',width:150,align:'center',editor:{type:'combobox',required:true,options:{valueField:'Sys_FieldName',textField:'Display',url:'/" + GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME + "/LoadViewFields.html?flag=1&moduleId=" + moduleId + "',onSelect:OnFieldSelected}}\">字段名</th>");
            sb.Append("<th data-options=\"field:'Method',width:120,align:'center',editor:{type:'combobox',required:true,options:{valueField:'Id',textField:'Name',data:" + json + "}}\">条件</th>");
            sb.Append("<th data-options=\"field:'Value',width:150,align:'center',editor:{type:'textbox',required:true}\">字段值</th>");
            sb.Append("<th data-options=\"field:'Group',width:60,align:'center',editor:{type:'combobox',options:{valueField:'Id',textField:'Name',data:[{Id:'And',Name:'并且'},{Id:'Or',Name:'或者'}]}}\">分组</th>");
            sb.Append("<th data-options=\"field:'Op',width:80,align:'center',formatter:function(value,row,index){return FormatOp(value,row,index);}\">操作</th>");
            sb.Append("</tr></thead>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            string exportModelJs = UIOperate.FormatJsPath("/Scripts/common/ExportModel.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", exportModelJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取下拉框数据源设置页面
        /// </summary>
        /// <returns></returns>
        public override string GetCombDataSourceSetHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<table style=\"width:100%;height:100%;line-height:50px;\">");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<form id=\"nvForm\">");
            sb.Append("<table style=\"width:100%;\"><tr>");
            sb.Append("<td>名称：</td>");
            sb.Append("<td><input id=\"name\" class=\"easyui-textbox\" data-options=\"required:true\" type=\"text\" style=\"width:150px;\" /></td>");
            sb.Append("<td>值：</td>");
            sb.Append("<td><input id=\"value\" class=\"easyui-textbox\" type=\"text\" style=\"width:150px;\" /></td>");
            sb.Append("<td><a href=\"#\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-add_other\" plain=\"true\" onclick=\"AddNameValue()\"></a></td>");
            sb.Append("</tr></table>");
            sb.Append("</form>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<div style=\"width:100%;\">");
            sb.Append("<table style=\"width:100%;height:205px;\" id=\"valueGrid\" class=\"easyui-datagrid\" data-options=\"title:'下拉数据',singleSelect:true,onSelect:OnSelectDataRow\">");
            sb.Append("<thead><tr>");
            sb.Append("<th data-options=\"field:'Op',width:60,align:'center',formatter:OpFormatter\">操作</th>");
            sb.Append("<th data-options=\"field:'Name',width:150,align:'center'\">名称</th>");
            sb.Append("<th data-options=\"field:'Value',width:150,align:'center'\">值</th>");
            sb.Append("</tr></thead>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            string combDataSourceSetJs = UIOperate.FormatJsPath("/Scripts/common/CombDataSourceSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", combDataSourceSetJs);
            return sb.ToString();
        }

        #endregion

        #endregion

        #region 公共页面

        /// <summary>
        /// 返回选择图标页面
        /// </summary>
        /// <returns></returns>
        public override string GetIconSelectHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div id=\"icon_tab\" class=\"easyui-tabs\" data-options=\"fit:true,border:false,tabHeight:{0}\" style=\"width:100%;height:100%;\">", ConstDefine.TAB_HEAD_HEIGHT);
            Dictionary<string, string> dic = EnumHelper.GetEnumDescValue(typeof(IconTypeEnum));
            foreach (string des in dic.Keys)
            {
                int value = dic[des].ObjToInt();
                if (value <= 0) continue;
                IconTypeEnum iconType = (IconTypeEnum)Enum.Parse(typeof(IconTypeEnum), dic[des]);
                long total = 0;
                int pageSize = 0;
                string html = SystemOperate.GetPageIconsHtml(out total, out pageSize, iconType);
                if (total == 0) continue;
                sb.AppendFormat("<div title=\"{0}\" style=\"padding:10px\">", des);
                //图标内容
                sb.AppendFormat("<div id=\"div_icon_{0}\" style=\"height:400px;\">", value.ToString());
                sb.Append(html);
                sb.Append("</div>");
                //分页控件
                sb.Append("<div class=\"easyui-panel\">");
                sb.Append("<div id=\"pp\" class=\"easyui-pagination\" data-options=\"total:" + total.ToString() + ",pageSize:" + pageSize.ToString() + ",layout:['first','prev','next','last'],onSelectPage:function(pageNumber,pageSize){IconPageSelected(pageNumber,pageSize," + value.ToString() + ");}\"></div>");
                sb.Append("</div>");
                sb.Append("</div>");
            }
            sb.Append("</div>");
            string iconSelectJs = UIOperate.FormatJsPath("/Scripts/common/IconSelect.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", iconSelectJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取弹出树HTML
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <param name="request">请求对象</param>
        /// <returns></returns>
        public override string GetDialogTreeHTML(Guid moduleId, HttpRequestBase request)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding:3px;\">");
            sb.Append("<div>");
            sb.Append("<input id=\"searchBox\" name=\"searchBox\" class=\"easyui-searchbox\" data-options=\"searcher:SearchNode,prompt:'请输入关键字',width:300\"></input>");
            sb.Append("</div>");
            sb.Append("<div style=\"height: 272px; overflow: auto;\">");
            sb.Append("<table style=\"line-height: 20px; margin: 5px 0 0 5px;\">");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<span id=\"msg\" style=\"color: red\"></span>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<ul id=\"tree\"></ul>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<fieldset class=\"fieldSet\">");
            sb.Append("<legend><span style=\"color: red;\">已选择项</span></legend>");
            sb.Append("<div id=\"selectedNodeList\" class=\"atta\" style=\"min-height:60px;\">");
            sb.Append("</div>");
            sb.Append("</fieldset>");
            sb.Append("</div>");
            sb.Append("<div>");
            sb.Append("</div>");
            string dialogTreeJs = UIOperate.FormatJsPath("/Scripts/common/DialogTree.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", dialogTreeJs);
            //模块自定义js
            sb.Append(UIOperate.GetModelJsHTML(moduleId, request));
            return sb.ToString();
        }

        #endregion

        #region 特殊页面

        /// <summary>
        /// 获取关联角色表单页面
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public override string GetSetRoleFormHTML(Guid roleId)
        {
            if (roleId == Guid.Empty) return string.Empty;
            string errMsg = string.Empty;
            Sys_Role role = CommonOperate.GetEntityById<Sys_Role>(roleId, out errMsg);
            StringBuilder sb = new StringBuilder();
            sb.Append("<form method=\"post\">");
            sb.Append("<div style=\"width:350px;height:100%;padding:10px;\">");
            sb.Append("<div class=\"content\">");
            sb.Append("<table border=\"0\" cellpadding=\"1\" cellspacing=\"2\" style=\"width:100%;table-layout:fixed;line-height:40px;\">");
            sb.Append("<tr><td style=\"width:75px;\">");
            sb.Append("<lable>请选择模块：</lable>");
            sb.Append("</td><td>");
            sb.AppendFormat("<input id=\"txtModule\" name=\"txtModule\" style=\"width:180px;\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',url:'/{0}/LoadModules.html',onSelect:ModuleSelected,filter:{1}\" />", GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME, "function(q, row){var opts = $(this).combobox('options');return row[opts.textField].indexOf(q)>-1;}");
            sb.Append("</td></tr>");
            sb.Append("<tr><td style=\"width:75px;\">");
            sb.Append("<lable>请选择表单：</lable>");
            sb.Append("</td><td>");
            sb.Append("<input id=\"txtForm\" name=\"txtForm\" style=\"width:180px;\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',url:'',onLoadSuccess:FormLoadSuccess\" />&nbsp;&nbsp;");
            sb.AppendFormat("<a href=\"#\" id=\"btnAddForm\" roleId=\"{0}\" roleName=\"{1}\" class=\"easyui-linkbutton\" iconCls=\"eu-p2-icon-add_other\" plain=\"true\" onclick=\"AddRoleForm(this)\"></a>", roleId.ToString(), role.Name);
            sb.AppendFormat("<a href=\"#\" id=\"btnEditForm\" roleId=\"{0}\" roleName=\"{1}\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-edit\" plain=\"true\" onclick=\"EditRoleForm(this)\"></a>", roleId.ToString(), role.Name);
            sb.AppendFormat("<a href=\"#\" id=\"btnDelForm\" roleId=\"{0}\" roleName=\"{1}\" class=\"easyui-linkbutton\" iconCls=\"eu-icon-close\" plain=\"true\" onclick=\"DelRoleForm(this)\"></a>", roleId.ToString(), role.Name);
            sb.Append("</td></tr>");
            sb.Append("</table>");
            sb.Append("</div></div></form>");
            string setRoleFormJs = UIOperate.FormatJsPath("/Scripts/common/SetRoleForm.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", setRoleFormJs);
            return sb.ToString();
        }

        /// <summary>
        /// 加载快速编辑角色表单页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="roleId">角色Id</param>
        /// <param name="formId">表单Id</param>
        /// <returns></returns>
        public override string GetQuickEditFormHTML(Guid moduleId, Guid roleId, Guid? formId)
        {
            string errMsg = string.Empty;
            Sys_Role role = CommonOperate.GetEntityById<Sys_Role>(roleId, out errMsg);
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            List<string> primaryFields = string.IsNullOrWhiteSpace(module.PrimaryKeyFields) ? new List<string>() : module.PrimaryKeyFields.Split(",".ToCharArray()).ToList();
            string titleKey = module.TitleKey;
            string titleKeyFieldId = string.Empty;
            List<Sys_FormField> leftFormFields = SystemOperate.GetDefaultFormField(moduleId);
            List<Sys_FormField> rightFormFields = formId.HasValue ? SystemOperate.GetFormField(formId.Value) : new List<Sys_FormField>();
            Sys_Form rightForm = formId.HasValue ? SystemOperate.GetForm(formId.Value) : null;
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<style type=\"text/css\">");
            sb.Append("li {");
            sb.Append("text-align: center;");
            sb.Append("margin-bottom: 3px;");
            sb.Append("}");
            sb.Append("li a {");
            sb.Append("width: 30px;");
            sb.Append("}");
            sb.Append("</style>");
            sb.Append("<table style=\"width: 100%; height: 100%; line-height: 30px;\">");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"3\">");
            sb.Append("<table style=\"width:100%;\">");
            sb.Append("<tr>");
            sb.Append("<td style=\"width:70px;\"><lable>角色名称：</lable></td>");
            sb.AppendFormat("<td style=\"width:170px;\"><input style=\"width:160px;\" id=\"RoleName\" class=\"easyui-textbox\" data-options=\"disabled:true,value:'{0}'\" /></td>", role.Name);
            sb.Append("<td style=\"width:75px;\"><lable>模块名称：</lable></td>");
            sb.AppendFormat("<td style=\"width:180px;\"><input style=\"width:180px;\" id=\"ModuleName\" class=\"easyui-textbox\" data-options=\"disabled:'true',value:'{0}'\" /></td>", module.Name);
            sb.Append("</tr><tr>");
            sb.Append("<td style=\"width:70px;\"><lable>表单名称：</lable></td>");
            sb.AppendFormat("<td style=\"width:170px;\"><input style=\"width:160px;\" id=\"FormName\" class=\"easyui-textbox\" data-options=\"required:true,value:'{0}'\" /></td>", rightForm == null ? string.Format("[{0}][{1}]表单", role.Name, module.Name) : rightForm.Name);
            sb.Append("<td style=\"width:75px;\"><lable>编辑模式：</lable></td>");
            int editMode = rightForm != null && rightForm.ModuleEditMode.HasValue ? rightForm.ModuleEditMode.Value : 0;
            Dictionary<string, string> dic = EnumHelper.GetEnumDescValue(typeof(ModuleEditModeEnum));
            string tempString = string.Empty;
            foreach (string key in dic.Keys)
            {
                tempString += "{Id:" + dic[key] + ",Name:'" + key + "'},";
            }
            tempString = "[" + tempString.Substring(0, tempString.Length - 1) + "]";
            sb.AppendFormat("<td style=\"width:180px;\"><input style=\"width:180px;\" id=\"EditMode\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',value:'{0}',data:{1}\" /></td>", editMode.ToString(), tempString);
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td></tr>");
            sb.Append("<tr>");
            sb.Append("<td style=\"width: 180px;\">可选字段：</td>");
            sb.Append("<td style=\"width: 60px;\"></td>");
            sb.Append("<td style=\"width: 380px;\">已选字段：</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<table id=\"leftGrid\" class=\"easyui-datagrid\" style=\"width: 180px; height: 350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'Id'\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'Id',checkbox:true\"></th>");
            sb.Append("<th data-options=\"field:'FieldName',hidden:true,width:0\">字段名称</th>");
            sb.Append("<th data-options=\"field:'Display',width:180\">字段</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            //加载字段数据
            if (leftFormFields != null && leftFormFields.Count > 0)
            {
                sb.Append("<tbody>");
                foreach (Sys_FormField field in leftFormFields)
                {
                    string fieldName = field.Sys_FieldName;
                    if (!field.Sys_FieldId.HasValue || field.IsDeleted || fieldName == "Id")
                        continue;
                    string tempFieldName = fieldName;
                    if (SystemOperate.IsForeignNameField(moduleId, fieldName))
                        tempFieldName = fieldName.Substring(0, fieldName.Length - 4) + "Id";
                    if (rightForm == null && ((titleKey != null && fieldName == titleKey) || primaryFields.Contains(tempFieldName)))
                    {
                        rightFormFields.Add(field);
                    }
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", field.Id.ToString());
                    sb.AppendFormat("<td>{0}</td>", fieldName);
                    sb.AppendFormat("<td>{0}</td>", field.Display);
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
            }
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<ul>");
            sb.Append("<li><a href=\"#\" title=\"移入选中项\" class=\"easyui-linkbutton\" onclick=\"RightMove()\">></a></li>");
            sb.Append("<li><a href=\"#\" title=\"全部移入\" class=\"easyui-linkbutton\" onclick=\"RightMoveAll()\">>></a></li>");
            sb.Append("<li><a href=\"#\" title=\"移出选中项\" class=\"easyui-linkbutton\" onclick=\"LeftMove()\"><</a></li>");
            sb.Append("<li><a href=\"#\" title=\"全部移出\" class=\"easyui-linkbutton\" onclick=\"LeftMoveAll()\"><<</a></li>");
            sb.Append("</ul>");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("<table id=\"rightGrid\" class=\"easyui-datagrid\" style=\"width: 380px; height: 350px;\" data-options=\"fitColumns:true,selectOnCheck:true,checkOnSelect:true,rownumbers:true,idField:'Id'\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'Id',checkbox:true\"></th>");
            sb.Append("<th data-options=\"field:'FieldName',hidden:true,width:0\">字段名称</th>");
            sb.Append("<th data-options=\"field:'Display',width:120\">字段</th>");
            sb.Append("<th data-options=\"field:'ControlWidth',width:80,editor:{type:'numberbox',options:{precision:0}}\">控件宽</th>");
            sb.Append("<th data-options=\"field:'RowNum',width:60,editor:{type:'numberbox',options:{precision:0,min:1,max:100}}\">行</th>");
            sb.Append("<th data-options=\"field:'ColNum',width:60,editor:{type:'numberbox',options:{precision:0,min:1,max:100}}\">列</th>");
            sb.Append("<th data-options=\"field:'CanEdit',width:80,editor:{type:'checkbox',options:{on:'是',off:'否'}}\">允许编辑</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            if (rightFormFields.Count > 0)
            {
                sb.Append("<tbody>");
                foreach (Sys_FormField field in rightFormFields)
                {
                    if (!field.Sys_FieldId.HasValue || field.IsDeleted || field.Sys_FieldName == "Id")
                        continue;
                    if (field.Sys_FieldName == titleKey && !string.IsNullOrWhiteSpace(titleKey))
                    {
                        titleKeyFieldId = field.Id.ToString();
                    }
                    sb.Append("<tr>");
                    sb.AppendFormat("<td>{0}</td>", field.Id.ToString());
                    sb.AppendFormat("<td>{0}</td>", field.Sys_FieldName);
                    sb.AppendFormat("<td>{0}</td>", field.Display);
                    sb.AppendFormat("<td>{0}</td>", field.Width.HasValue ? field.Width.Value : 180);
                    sb.AppendFormat("<td>{0}</td>", field.RowNo.ToString());
                    sb.AppendFormat("<td>{0}</td>", field.ColNo.ToString());
                    sb.AppendFormat("<td>{0}</td>", field.IsAllowEdit.HasValue && field.IsAllowEdit.Value ? "是" : "否");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody>");
            }
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.AppendFormat("<input id=\"titleKeyFieldId\" type=\"hidden\" value=\"{0}\" />", titleKeyFieldId);
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/easyui-extension/datagrid-cellediting.js\"></script>");
            string quickEditFormJs = UIOperate.FormatJsPath("/Scripts/common/QuickEditForm.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", quickEditFormJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取设置用户角色页面
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public override string GetSetUserRoleHTML(Guid userId)
        {
            string errMsg = string.Empty;
            StringBuilder sb = new StringBuilder();
            //获取所有角色
            List<Sys_Role> roles = CommonOperate.GetEntities<Sys_Role>(out errMsg, null, null, false);
            //构造HTML
            sb.Append("<div style=\"padding:10px\">");
            sb.Append("<table>");
            if (roles != null && roles.Count > 0)
            {
                for (int i = 0; i < roles.Count; i++)
                {
                    int r = i / 4; //当前所在行
                    int c = i % 4; //当前所在列
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }
                    else if (c == 0)
                    {
                        sb.Append("</tr><tr>");
                    }
                    Sys_Role role = roles[i];
                    string checkedStr = string.Empty;
                    Sys_UserRole userRole = CommonOperate.GetEntity<Sys_UserRole>(x => x.Sys_RoleId == role.Id && x.Sys_UserId == userId, null, out errMsg);
                    if (userRole != null)
                    {
                        checkedStr = "checked=\"checked\"";
                    }
                    sb.AppendFormat("<td style=\"text-align:left;width:160px;height:32px;\"><input id=\"chk_{0}\" value=\"{0}\" type=\"checkbox\" {2}/><span id=\"span_{0}\">{1}</span></td>", role.Id.ToString(), role.Name, checkedStr);
                }
                sb.Append("</tr>");
            }
            sb.Append("</table>");
            sb.Append("</div>");
            string setUserRoleJs = UIOperate.FormatJsPath("/Scripts/common/SetUserRole.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", setUserRoleJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取系统配置页面
        /// </summary>
        /// <returns></returns>
        public override string GetWebConfigHTML()
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }

        /// <summary>
        /// 添加通用按钮
        /// </summary>
        /// <param name="moduleId">模块id</param>
        /// <returns></returns>
        public override string GetAddCommonBtnHTML(Guid moduleId)
        {
            StringBuilder sb = new StringBuilder();
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            List<Sys_GridButton> commonBtns = SystemOperate.GetGridButtons(moduleId).Where(x => CommonDefine.GridCommonBtns.Contains(x.ButtonText)).ToList();
            sb.Append("<div style=\"padding:10px;\">");
            sb.Append("<table style=\"line-height:35px;\">");
            sb.Append("<tr>");
            sb.Append("<td>请选择模块：</td>");
            sb.Append("<td><input id=\"moduleId\" class=\"easyui-combobox\" data-options=\"required:true,missingMessage:null,valueField:'Id',textField:'Name',url:'/" + GlobalConst.ASYNC_STSTEM_CONTROLLER_NAME + "/LoadModules.html',value:'" + (moduleId != Guid.Empty ? moduleId.ToString() : string.Empty) + "',onSelect:function(record){location.href = '/Page/AddCommonBtn.html?moduleId='+record.Id;}\" /></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            for (int i = 0; i < CommonDefine.GridCommonBtns.Count; i++)
            {
                string chkStr = string.Empty;
                string text = CommonDefine.GridCommonBtns[i];
                if (module != null)
                {
                    if (!module.IsAllowAdd && text == "新增") continue;
                    if (!module.IsAllowEdit && text == "编辑") continue;
                    if ((!module.IsAllowCopy || !module.IsAllowAdd) && text == "复制") continue;
                    if (!module.IsAllowDelete && text == "删除") continue;
                    if ((!module.IsAllowAdd || !module.IsAllowEdit || !module.IsAllowImport) && text == "导入") continue;
                    if (!module.IsAllowExport && text == "导出") continue;
                    if ((!module.IsAllowEdit || !module.IsEnabledBatchEdit) && text == "批量编辑") continue;
                    if (!module.IsEnabledPrint && text == "打印") continue;
                }
                if (commonBtns.Where(x => x.ButtonText == text).FirstOrDefault() != null)
                {
                    chkStr = "checked=\"checked\"";
                }
                sb.AppendFormat("<input style=\"margin-left:10px;\" type=\"checkbox\" name=\"btnChk\" value=\"{0}\" {1} /><span>{2}</span>", i.ToString(), chkStr, text);
            }
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        #endregion

        #region 权限页面

        #region 角色权限

        /// <summary>
        /// 获取设置角色模块权限页面
        /// </summary>
        /// <returns></returns>
        public override string GetSetRoleModulePermissionHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/easyui-extension/datagrid-groupview.js\"></script>");
            sb.Append("<div data-options=\"region:'west',title:'角色',split:true,border:false\" style=\"width:180px;\">");
            sb.Append("<ul class=\"tree\" style=\"line-height:30px;\">");
            sb.Append("<li>");
            sb.Append("<label>&nbsp;检索：</label>");
            sb.Append("<input type=\"text\" id=\"roleTxt\" style=\"width: 90px;\" />");
            sb.Append("&nbsp;<a href=\"javascript:void(0);\" style=\"height:22px;margin-top:-1px;\" title=\"搜索\" onclick=\"SearchRole()\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-search'\"></a>");
            sb.Append("</li>");
            sb.Append("</ul>");
            sb.Append("<ul id=\"roleTree\"></ul>");
            sb.Append("</div>");
            sb.Append("<div data-options=\"region:'center',border:false\" style=\"background: #E0ECFF;\">");
            sb.AppendFormat("<div id=\"tabs_permission\" class=\"easyui-tabs\" data-options=\"fit:true,border:false,tabHeight:{0}\" style=\"height:100%;\">", ConstDefine.TAB_HEAD_HEIGHT);
            sb.Append("<div title=\"角色权限\">");
            sb.Append("<div class=\"easyui-layout\" style=\"width:100%;height:100%;\">");
            sb.Append("<div data-options=\"region:'west',title:'功能菜单',split:true,border:false\" style=\"width: 180px;\">");
            sb.Append("<ul id=\"menuTree\"></ul>");
            sb.Append("</div>");
            sb.Append("<div id=\"permisson_center\" data-options=\"region:'center',title:'',border:false\" style=\"padding: 2px;\">");

            sb.Append("<div class=\"easyui-panel\" title=\"操作权限\" style=\"width:98%;height:200px;margin-bottom:3px;\" data-options=\"collapsible:true\">");

            sb.Append("</div>");

            sb.Append("<div class=\"easyui-panel\" title=\"字段权限\" style=\"width:98%;height:200px;margin-bottom:3px;\" data-options=\"collapsible:true\">");

            sb.Append("</div>");

            sb.Append("<div class=\"easyui-panel\" title=\"数据权限\" style=\"width:98%;height:200px;margin-bottom:3px;\" data-options=\"collapsible:true\">");

            sb.Append("</div>");

            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<div title=\"包含用户\"></div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/permission/SetRoleModulePermission.js\"></script>");

            return sb.ToString();
        }

        /// <summary>
        /// 获取设置角色权限页面
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        public override string GetSetRolePermissionHTML(Guid? roleId)
        {
            StringBuilder sb = new StringBuilder();
            //列表高
            int h = CurrUser.ClientBrowserHeight - ConstDefine.TOP_NORTH_REGION_HEIGHT - ConstDefine.TAB_HEAD_HEIGHT * 2 - ConstDefine.BOTTOM_STATUS_REGON_HEIGHT - 32 - 12;
            if (!roleId.HasValue) //从角色权限设置菜单进入
            {
                sb.Append("<div data-options=\"region:'west',title:'角色',split:true,border:false\" style=\"width:180px;\">");
                sb.Append("<ul class=\"tree\" style=\"line-height:30px;\">");
                sb.Append("<li>");
                sb.Append("<label>&nbsp;检索：</label>");
                sb.Append("<input type=\"text\" id=\"roleTxt\" style=\"width: 90px;\" />");
                sb.Append("&nbsp;<a href=\"javascript:void(0);\" style=\"height:22px;margin-top:-1px;\" title=\"搜索\" onclick=\"SearchRole()\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-search'\"></a>");
                sb.Append("</li>");
                sb.Append("</ul>");
                sb.Append("<ul id=\"roleTree\"></ul>");
                sb.Append("</div>");
            }
            sb.Append("<div data-options=\"region:'center',border:false\" style=\"background: #E0ECFF;\">");
            sb.AppendFormat("<div id=\"tabs_permission\" class=\"easyui-tabs\" data-options=\"border:false,fit:true,tabHeight:{0}\" style=\"height:100%;\">", ConstDefine.TAB_HEAD_HEIGHT.ToString());
            sb.Append("<div title=\"权限设置\">");
            //权限设置内容--start
            sb.Append("<div class=\"content\">");
            if (roleId.HasValue) //从角色列表点击进入
                h = 520 - ConstDefine.PANEL_HEAD_HEIGHT - ConstDefine.TAB_HEAD_HEIGHT - ConstDefine.DIALOG_TOOLBAR_HEIGHT - 40 - ConstDefine.DIALOG_BORDER_WIDTH;
            sb.Append("<table style=\"width:100%;height:32px;line-height:32px;text-align:left;\"><tr>");
            sb.Append("<td style=\"width:300px;\">");
            sb.Append("<label>&nbsp;按大模块检索：</label>");
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(true, CurrUser);
            string selectDefaultStr = topMenus.Count > 0 ? string.Format(",value:'{0}'", topMenus.FirstOrDefault().Id.ToString()) : string.Empty;
            sb.AppendFormat("<select id=\"bigModuleItem\" class=\"easyui-combobox\" data-options=\"editable:false,onChange:BigModuleChanged{0}\" style=\"width:200px;\">", selectDefaultStr);
            if (topMenus.Count > 1)
            {
                sb.AppendFormat("<option value=\"{0}\">全部</option>", Guid.Empty.ToString());
            }
            foreach (Sys_Menu menu in topMenus)
            {
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", menu.Id.ToString(), menu.Display);
            }
            sb.Append("</select>");
            sb.Append("</td>");
            sb.Append("<td style=\"text-align:center;padding-right:300px;\">");
            if (!roleId.HasValue) //从角色权限设置菜单点击进入
            {
                sb.AppendFormat("<a id=\"btnSave\" style=\"margin-right:10px;color:#fff;\" href=\"#\" class=\"easyui-linkbutton{0}\" data-options=\"iconCls:'eu-icon-save'\" onclick=\"SaveRolePermission()\">保 存</a>", GlobalSet.IsShowStyleBtn ? " c1" : string.Empty);
                sb.AppendFormat("<a id=\"btnCancel\" style=\"margin-right:10px;color:#fff;\" href=\"#\" class=\"easyui-linkbutton{0}\" data-options=\"iconCls:'eu-icon-cancel'\" onclick=\"CloseTab()\">关 闭</a>", GlobalSet.IsShowStyleBtn ? " c2" : string.Empty);
            }
            sb.Append("</td>");
            sb.Append("</tr></table>");
            sb.AppendFormat("<table id=\"tb_permission\" h=\"{0}\" class=\"easyui-datagrid\" style=\"width:99%px;height:{0}px\"", h.ToString());
            sb.Append("data-options=\"singleSelect:true,selectOnCheck:false,checkOnSelect:false,url:'',idField:'MenuId',treeField:'MenuDisplay',method:'get'\">");
            sb.Append("<thead data-options=\"frozen:true\">");
            sb.Append("<tr>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'MenuId',checkbox:true\"></th>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'BigModule',hidden:true\"></th>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'MenuDisplay',align:'left',width:130\">功能模块</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th rowspan=\"2\" data-options=\"field:'OpPermisson',align:'conter',width:190\">操作权限</th>");
            sb.Append("<th colspan=\"3\">数据权限</th>");
            sb.Append("<th colspan=\"3\">字段权限</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'CanViewData',align:'center',width:180\">允许浏览数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanEditData',align:'center',width:180\">允许编辑数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanDelData',align:'center',width:180\">允许删除数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanViewFields',align:'center',width:200\">允许查看字段</th>");
            sb.Append("<th data-options=\"field:'CanAddFields',align:'center',width:200\">允许新增字段</th>");
            sb.Append("<th data-options=\"field:'CanEditFields',align:'center',width:200\">允许编辑字段</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("</table>");
            sb.Append("</div>");
            //权限设置内容--end
            sb.Append("</div>");
            sb.Append("<div title=\"包含用户\" fit=\"true\">");
            Guid userModuleId = SystemOperate.GetModuleIdByName("用户管理");
            string where = string.Format("Id IN(SELECT Sys_UserId FROM Sys_UserRole WHERE Sys_RoleId='{0}')", roleId.HasValue ? roleId.Value.ToString() : Guid.Empty.ToString());
            List<string> filterFields = new List<string>() { "UserName", "AliasName", "Sys_OrganizationId", "Sys_OrganizationName", "IsActivated", "IsValid" };
            string tempHtml = GetGridHTML(userModuleId, DataGridType.Other, null, where, null, null, null, null, false, filterFields);
            sb.Append(tempHtml);
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            string setRolePermissionJs = UIOperate.FormatJsPath("/Scripts/permission/SetRolePermission.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", setRolePermissionJs);

            return sb.ToString();
        }

        /// <summary>
        /// 获取角色数据权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="type">数据权限类型</param>
        /// <returns></returns>
        public override string GetRoleDataPermissionSetHTML(string moduleName, string roleName, int type)
        {
            StringBuilder sb = new StringBuilder();
            Sys_Module module = SystemOperate.GetModuleByName(moduleName);
            Sys_Role role = PermissionOperate.GetRole(roleName);
            if (module == null || role == null)
                return string.Empty;
            sb.Append("<div style=\"padding:3px;\">");
            sb.Append("<div>");
            sb.Append("<input id=\"searchBox\" name=\"searchBox\" class=\"easyui-searchbox\" data-options=\"searcher:SearchNode,prompt:'请输入关键字',width:300\"></input>");
            sb.Append("</div>");
            sb.Append("<div style=\"height: 272px; overflow: auto;\">");
            sb.Append("<table style=\"line-height: 20px; margin: 5px 0 0 5px;\">");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<span id=\"msg\" style=\"color: red\"></span>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<ul id=\"tree\"></ul>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<fieldset class=\"fieldSet\">");
            sb.Append("<legend><span style=\"color: red;\">已选择的组织</span></legend>");
            DataPermissionTypeEnum permissionType = (DataPermissionTypeEnum)Enum.Parse(typeof(DataPermissionTypeEnum), type.ToString());
            Dictionary<string, bool> dic = PermissionOperate.GetRoleDataPermissions(role.Id, module.Id, permissionType);
            sb.AppendFormat("<div id=\"selectedNodeList\" dic=\"{0}\" class=\"atta\" style=\"min-height:60px;\">", HttpUtility.UrlEncode(JsonHelper.Serialize(dic).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20"));
            sb.Append("</div>");
            sb.Append("</fieldset>");
            sb.Append("</div>");
            int appType1 = 1;
            int appType2 = 2;
            string typeName1 = "编辑";
            string typeName2 = "删除";
            if (type == 1)
            {
                appType1 = 0;
                typeName1 = "浏览";
            }
            else if (type == 2)
            {
                appType1 = 0;
                appType2 = 1;
                typeName1 = "浏览";
                typeName2 = "编辑";
            }
            sb.Append("<div>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.AppendFormat("<input type=\"checkbox\" appType=\"{0}\" id=\"chk_appToType1\" />", appType1);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<font color=\"blue\">应用到数据{0}权限</font>", typeName1);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input type=\"checkbox\" appType=\"{0}\" id=\"chk_appToType2\" style=\"margin-left:10px;\" />", appType2);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<font color=\"blue\">应用到数据{0}权限</font>", typeName2);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            string roleDataPermissionSetJs = UIOperate.FormatJsPath("/Scripts/permission/RoleDataPermissionSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", roleDataPermissionSetJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取角色字段权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="type">字段权限类型</param>
        /// <returns></returns>
        public override string GetRoleFieldPermissionSetHTML(string moduleName, string roleName, int type)
        {
            StringBuilder sb = new StringBuilder();
            Sys_Module module = SystemOperate.GetModuleByName(moduleName);
            Sys_Role role = PermissionOperate.GetRole(roleName);
            if (module == null || role == null)
                return string.Empty;
            List<Sys_Field> fields = new List<Sys_Field>();
            List<Sys_Field> sysFields = SystemOperate.GetFieldInfos(module);
            foreach (Sys_Field sysField in sysFields)
            {
                Sys_GridField gridField = SystemOperate.GetDefaultGridField(sysField);
                Sys_FormField formField = SystemOperate.GetRoleFormField(role.Id, module.Id, sysField);
                if (gridField == null && formField == null) continue;
                if (gridField != null && !gridField.IsVisible && formField == null) continue;
                fields.Add(sysField);
            }
            fields = fields.Where(x => !CommonDefine.BaseEntityFieldsContainId.Contains(x.Name)).ToList();
            sb.Append("<div style=\"padding:10px;width:765px;height:270px;\">");
            if (fields.Count > 0)
            {
                int appType1 = 1;
                int appType2 = 2;
                string typeName1 = "新增";
                string typeName2 = "编辑";
                if (type == 1)
                {
                    appType1 = 0;
                    typeName1 = "查看";
                }
                else if (type == 2)
                {
                    appType1 = 0;
                    appType2 = 1;
                    typeName1 = "查看";
                    typeName2 = "新增";
                }
                FieldPermissionTypeEnum permissionType = (FieldPermissionTypeEnum)Enum.Parse(typeof(FieldPermissionTypeEnum), type.ToString());
                Dictionary<string, bool> dic = PermissionOperate.GetRoleFieldPermissions(role.Id, module.Id, permissionType);
                sb.Append("<table style=\"width:100%;line-height:30px\">");
                sb.Append("<tr>");
                string selectAllCheckStr = string.Empty;
                string selectAllTitle = string.Empty;
                bool selectAllFromParentRole = dic.ContainsKey("-1") && dic["-1"]; //继承父角色的‘全部’字段权限
                if (selectAllFromParentRole)
                {
                    selectAllCheckStr = " checked=\"checked\" disabled=\"disabled\"";
                    selectAllTitle = "继承父角色权限";
                }
                else if (dic.ContainsKey("-1"))
                {
                    selectAllCheckStr += " checked=\"checked\"";
                    if (dic["-1"]) //继承父角色
                    {
                        selectAllCheckStr += " disabled=\"disabled\"";
                        selectAllTitle = "继承父角色权限";
                    }
                }
                sb.AppendFormat("<td style=\"width:420px;\"><table><tr><td><input id=\"chk_selectAll\" type=\"checkbox\" value=\"-1\" display=\"全部\" title=\"{0}\"{1} /></td><td>全部</td></tr></table></td>", selectAllTitle, selectAllCheckStr);
                sb.AppendFormat("<td><table><tr><td><input id=\"chk_appToType1\" type=\"checkbox\" value=\"{0}\" /></td><td>应用到字段{1}权限</td></tr></table></td>", appType1, typeName1);
                sb.AppendFormat("<td><table><tr><td><input id=\"chk_appToType2\" type=\"checkbox\" value=\"{1}\" /></td><td>应用到字段{1}权限</td></tr></table></td>", appType2, typeName2);
                sb.Append("<td></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("<div class=\"easyui-panel\" title=\"可选字段\" style=\"width:100%;height:100%;\">");
                sb.Append("<table id=\"tb_fields\" style=\"width:100%;line-height:30px\">");
                for (int i = 0; i < fields.Count; i++)
                {
                    int r = i / 4; //当前所在行
                    int c = i % 4; //当前所在列
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }
                    else if (c == 0)
                    {
                        sb.Append("</tr><tr>");
                    }
                    string title = "勾选赋于权限";
                    string checkedStr = string.Empty;
                    if (selectAllFromParentRole)
                    {
                        checkedStr += " checked=\"checked\" disabled=\"disabled\"";
                        title = "继承父角色权限";
                    }
                    else if (dic.ContainsKey(fields[i].Name) || dic.ContainsKey("-1"))
                    {
                        checkedStr += " checked=\"checked\"";
                        if (dic.ContainsKey(fields[i].Name) && dic[fields[i].Name]) //继承父角色
                        {
                            checkedStr += " disabled=\"disabled\"";
                            title = "继承父角色权限";
                        }
                    }
                    sb.AppendFormat("<td><table><tr><td style=\"width:110px;text-align:right;\">{0}</td><td><input title=\"{2}\" type=\"checkbox\" value=\"{1}\" display=\"{0}\"{3} /></td></tr></table></td>", fields[i].Display, fields[i].Name, title, checkedStr);
                }
                sb.Append("</tr></table>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append("<table style=\"width:100%;height:100%\"><tr><td style=\"text-align:center\"><span>无可用字段</span></td></tr></table>");
            }
            sb.Append("</div>");
            string roleFieldPermissionSetJs = UIOperate.FormatJsPath("/Scripts/permission/RoleFieldPermissionSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", roleFieldPermissionSetJs);
            return sb.ToString();
        }

        #endregion

        #region 用户权限

        /// <summary>
        /// 获取设置用户权限页面
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public override string GetSetUserPermissionHTML(Guid? userId)
        {
            StringBuilder sb = new StringBuilder();
            //列表高
            int h = CurrUser.ClientBrowserHeight - ConstDefine.TOP_NORTH_REGION_HEIGHT - ConstDefine.TAB_HEAD_HEIGHT - ConstDefine.BOTTOM_STATUS_REGON_HEIGHT - 32 - 8;
            if (!userId.HasValue) //从用户列表点击进入
            {
                sb.Append("<div data-options=\"region:'west',title:'用户',split:true,border:false\" style=\"width:180px;\">");
                sb.Append("<ul class=\"tree\" style=\"line-height:30px;\">");
                sb.Append("<li>");
                sb.Append("<label>&nbsp;检索：</label>");
                sb.Append("<input type=\"text\" id=\"userTxt\" style=\"width: 90px;\" />");
                sb.Append("&nbsp;<a href=\"javascript:void(0);\" style=\"height:22px;margin-top:-1px;\" title=\"搜索\" onclick=\"SearchUser()\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-search'\"></a>");
                sb.Append("</li>");
                sb.Append("</ul>");
                sb.Append("<ul id=\"userTree\"></ul>");
                sb.Append("</div>");
            }
            sb.Append("<div id=\"region_center\" data-options=\"region:'center',border:false\">");
            //权限设置内容--start
            sb.Append("<div class=\"content\">");
            if (userId.HasValue) //从用户列表点击进入
                h = 520 - ConstDefine.PANEL_HEAD_HEIGHT - ConstDefine.DIALOG_TOOLBAR_HEIGHT - ConstDefine.DIALOG_BORDER_WIDTH - 36;
            sb.Append("<table style=\"width:100%;height:32px;line-height:32px;text-align:left;\"><tr>");
            sb.Append("<td style=\"width:300px;\">");
            sb.Append("<label>&nbsp;按大模块检索：</label>");
            List<Sys_Menu> topMenus = SystemOperate.GetTopMenus(true, CurrUser);
            string defaultSelectStr = topMenus.Count > 1 ? string.Format(",value:'{0}'", topMenus.FirstOrDefault().Id) : string.Empty;
            sb.AppendFormat("<select id=\"bigModuleItem\" class=\"easyui-combobox\" data-options=\"editable:false,onChange:BigModuleChanged{0}\" style=\"width:200px;\">", defaultSelectStr);
            if (topMenus.Count > 1)
            {
                sb.AppendFormat("<option value=\"{0}\">全部</option>", Guid.Empty.ToString());
            }
            foreach (Sys_Menu menu in topMenus)
            {
                sb.AppendFormat("<option value=\"{0}\">{1}</option>", menu.Id.ToString(), menu.Display);
            }
            sb.Append("</select>");
            sb.Append("</td>");
            sb.Append("<td style=\"text-align:center;padding-right:300px;\">");
            if (!userId.HasValue) //从用户权限菜单中点击进入
            {
                sb.AppendFormat("<a id=\"btnSave\" style=\"margin-right:10px;color:#fff;\" href=\"#\" class=\"easyui-linkbutton{0}\" data-options=\"iconCls:'eu-icon-save'\" onclick=\"SaveUserPermission()\">保 存</a>", GlobalSet.IsShowStyleBtn ? " c1" : string.Empty);
                sb.AppendFormat("<a id=\"btnCancel\" style=\"margin-right:10px;color:#fff;\" href=\"#\" class=\"easyui-linkbutton{0}\" data-options=\"iconCls:'eu-icon-cancel'\" onclick=\"CloseTab()\">关 闭</a>", GlobalSet.IsShowStyleBtn ? " c2" : string.Empty);
                sb.AppendFormat("<a id=\"btnClear\" style=\"margin-right:10px;color:#fff;\" href=\"#\" class=\"easyui-linkbutton{0}\" data-options=\"iconCls:'eu-icon-clear'\" onclick=\"ClearUserPermission()\">清除权限</a>", GlobalSet.IsShowStyleBtn ? " c3" : string.Empty);
            }
            sb.Append("</td>");
            sb.Append("</tr></table>");
            sb.AppendFormat("<table id=\"tb_permission\" h=\"{0}\" class=\"easyui-datagrid\" style=\"width:99%px;height:{0}px\"", h.ToString());
            sb.Append("data-options=\"singleSelect:true,selectOnCheck:false,checkOnSelect:false,url:'',idField:'MenuId',treeField:'MenuDisplay',method:'get'\">");
            sb.Append("<thead data-options=\"frozen:true\">");
            sb.Append("<tr>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'MenuId',checkbox:true\"></th>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'BigModule',hidden:true\"></th>");
            sb.Append("<th rowspan=\"1\" data-options=\"field:'MenuDisplay',align:'left',width:130\">功能模块</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th rowspan=\"2\" data-options=\"field:'OpPermisson',align:'conter',width:190\">操作权限</th>");
            sb.Append("<th colspan=\"3\">数据权限</th>");
            sb.Append("<th colspan=\"3\">字段权限</th>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'CanViewData',align:'center',width:180\">允许浏览数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanEditData',align:'center',width:180\">允许编辑数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanDelData',align:'center',width:180\">允许删除数据的组织范围</th>");
            sb.Append("<th data-options=\"field:'CanViewFields',align:'center',width:200\">允许查看字段</th>");
            sb.Append("<th data-options=\"field:'CanAddFields',align:'center',width:200\">允许新增字段</th>");
            sb.Append("<th data-options=\"field:'CanEditFields',align:'center',width:200\">允许编辑字段</th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("</table>");
            sb.Append("</div>");
            //权限设置内容--end
            sb.Append("</div>");
            string setUserPermissionJs = UIOperate.FormatJsPath("/Scripts/permission/SetUserPermission.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", setUserPermissionJs);

            return sb.ToString();
        }

        /// <summary>
        /// 获取用户数据权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="userId">用户Id</param>
        /// <param name="type">数据权限类型</param>
        /// <returns></returns>
        public override string GetUserDataPermissionSetHTML(string moduleName, Guid userId, int type)
        {
            StringBuilder sb = new StringBuilder();
            Sys_Module module = SystemOperate.GetModuleByName(moduleName);
            Sys_User user = UserOperate.GetUser(userId);
            if (module == null || user == null)
                return string.Empty;
            sb.Append("<div style=\"padding:3px;\">");
            sb.Append("<div>");
            sb.Append("<input id=\"searchBox\" name=\"searchBox\" class=\"easyui-searchbox\" data-options=\"searcher:SearchNode,prompt:'请输入关键字',width:300\"></input>");
            sb.Append("</div>");
            sb.Append("<div style=\"height: 272px; overflow: auto;\">");
            sb.Append("<table style=\"line-height: 20px; margin: 5px 0 0 5px;\">");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<span id=\"msg\" style=\"color: red\"></span>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td colspan=\"2\">");
            sb.Append("<ul id=\"tree\"></ul>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<fieldset class=\"fieldSet\">");
            sb.Append("<legend><span style=\"color: red;\">已选择的组织</span></legend>");
            DataPermissionTypeEnum permissionType = (DataPermissionTypeEnum)Enum.Parse(typeof(DataPermissionTypeEnum), type.ToString());
            List<string> list = PermissionOperate.GetUserDataPermissions(user.Id, module.Id, permissionType);
            sb.AppendFormat("<div id=\"selectedNodeList\" dic=\"{0}\" class=\"atta\" style=\"min-height:60px;\">", HttpUtility.UrlEncode(JsonHelper.Serialize(list).Replace("\r\n", string.Empty), Encoding.UTF8).Replace("+", "%20"));
            sb.Append("</div>");
            sb.Append("</fieldset>");
            sb.Append("</div>");
            int appType1 = 1;
            int appType2 = 2;
            string typeName1 = "编辑";
            string typeName2 = "删除";
            if (type == 1)
            {
                appType1 = 0;
                typeName1 = "浏览";
            }
            else if (type == 2)
            {
                appType1 = 0;
                appType2 = 1;
                typeName1 = "浏览";
                typeName2 = "编辑";
            }
            sb.Append("<div>");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.AppendFormat("<input type=\"checkbox\" appType=\"{0}\" id=\"chk_appToType1\" />", appType1);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<font color=\"blue\">应用到数据{0}权限</font>", typeName1);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input type=\"checkbox\" appType=\"{0}\" id=\"chk_appToType2\" style=\"margin-left:10px;\" />", appType2);
            sb.Append("</td>");
            sb.Append("<td>");
            sb.AppendFormat("<font color=\"blue\">应用到数据{0}权限</font>", typeName2);
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            string userDataPermissionSetJs = UIOperate.FormatJsPath("/Scripts/permission/UserDataPermissionSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", userDataPermissionSetJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取用户字段权限设置页面
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="userId">用户Id</param>
        /// <param name="type">字段权限类型</param>
        /// <returns></returns>
        public override string GetUserFieldPermissionSetHTML(string moduleName, Guid userId, int type)
        {
            StringBuilder sb = new StringBuilder();
            Sys_Module module = SystemOperate.GetModuleByName(moduleName);
            Sys_User user = UserOperate.GetUser(userId);
            if (module == null || user == null)
                return string.Empty;
            List<Sys_Field> fields = new List<Sys_Field>();
            List<Sys_Field> sysFields = SystemOperate.GetFieldInfos(module);
            foreach (Sys_Field sysField in sysFields)
            {
                Sys_GridField gridField = SystemOperate.GetDefaultGridField(sysField);
                Sys_FormField formField = SystemOperate.GetUserFormField(user.Id, module.Id, sysField);
                if (gridField == null && formField == null) continue;
                if (gridField != null && !gridField.IsVisible && formField == null) continue;
                fields.Add(sysField);
            }
            fields = fields.Where(x => !CommonDefine.BaseEntityFieldsContainId.Contains(x.Name)).ToList();
            sb.Append("<div style=\"padding:10px;width:765px;height:270px;\">");
            if (fields.Count > 0)
            {
                int appType1 = 1;
                int appType2 = 2;
                string typeName1 = "新增";
                string typeName2 = "编辑";
                if (type == 1)
                {
                    appType1 = 0;
                    typeName1 = "查看";
                }
                else if (type == 2)
                {
                    appType1 = 0;
                    appType2 = 1;
                    typeName1 = "查看";
                    typeName2 = "新增";
                }
                FieldPermissionTypeEnum permissionType = (FieldPermissionTypeEnum)Enum.Parse(typeof(FieldPermissionTypeEnum), type.ToString());
                List<string> list = PermissionOperate.GetUserFieldsPermissions(user.Id, module.Id, permissionType);
                sb.Append("<table style=\"width:100%;line-height:30px\">");
                sb.Append("<tr>");
                string selectAllCheckStr = string.Empty;
                string selectAllTitle = string.Empty;
                if (list.Contains("-1"))
                {
                    selectAllCheckStr += " checked=\"checked\"";
                }
                sb.AppendFormat("<td style=\"width:420px;\"><table><tr><td><input id=\"chk_selectAll\" type=\"checkbox\" value=\"-1\" display=\"全部\" title=\"{0}\"{1} /></td><td>全部</td></tr></table></td>", selectAllTitle, selectAllCheckStr);
                sb.AppendFormat("<td><table><tr><td><input id=\"chk_appToType1\" type=\"checkbox\" value=\"{0}\" /></td><td>应用到字段{1}权限</td></tr></table></td>", appType1, typeName1);
                sb.AppendFormat("<td><table><tr><td><input id=\"chk_appToType2\" type=\"checkbox\" value=\"{1}\" /></td><td>应用到字段{1}权限</td></tr></table></td>", appType2, typeName2);
                sb.Append("<td></td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("<div class=\"easyui-panel\" title=\"可选字段\" style=\"width:100%;height:100%;\">");
                sb.Append("<table id=\"tb_fields\" style=\"width:100%;line-height:30px\">");
                for (int i = 0; i < fields.Count; i++)
                {
                    int r = i / 4; //当前所在行
                    int c = i % 4; //当前所在列
                    if (i == 0)
                    {
                        sb.Append("<tr>");
                    }
                    else if (c == 0)
                    {
                        sb.Append("</tr><tr>");
                    }
                    string title = "勾选赋于权限";
                    string checkedStr = string.Empty;
                    if (list.Contains(fields[i].Name) || list.Contains("-1"))
                    {
                        checkedStr += " checked=\"checked\"";
                    }
                    sb.AppendFormat("<td><table><tr><td style=\"width:110px;text-align:right;\">{0}</td><td><input title=\"{2}\" type=\"checkbox\" value=\"{1}\" display=\"{0}\"{3} /></td></tr></table></td>", fields[i].Display, fields[i].Name, title, checkedStr);
                }
                sb.Append("</tr></table>");
                sb.Append("</div>");
            }
            else
            {
                sb.Append("<table style=\"width:100%;height:100%\"><tr><td style=\"text-align:center\"><span>无可用字段</span></td></tr></table>");
            }
            sb.Append("</div>");
            string userFieldPermissionSetJs = UIOperate.FormatJsPath("/Scripts/permission/UserFieldPermissionSet.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", userFieldPermissionSetJs);
            return sb.ToString();
        }

        #endregion

        #endregion

        #region 桌面页面

        /// <summary>
        /// 获取我的桌面页面
        /// </summary>
        /// <returns></returns>
        public override string GetDesktopIndexHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"deskRegion\" region=\"center\" data-options=\"border:false\" style=\"overflow-x:hidden\">");
            List<Desktop_Item> desktopItems = SystemOperate.GetDesktopItems(CurrUser);
            if (desktopItems.Count > 0)
            {
                StringBuilder toolsSb = new StringBuilder();
                sb.AppendFormat("<div id=\"pp\" style=\"width:{0}px;overflow-x:hidden;\">", (CurrUser.ClientBrowserWidth - ConstDefine.MAIN_LEFT_MENU_WIDTH - 12).ToString());
                for (int j = 0; j < 2; j++) //桌面设置两列
                {
                    //--column--start
                    sb.Append("<div style=\"width:50%;\">");
                    for (int i = j; i < desktopItems.Count; i += 2)
                    {
                        if (desktopItems[i].Desktop_ItemTabs != null && desktopItems[i].Desktop_ItemTabs.Count > 0)
                        {
                            toolsSb.AppendFormat("<div id=\"tab-tools_{0}\">", i.ToString());
                            bool hasTabs = desktopItems[i].Desktop_ItemTabs.Count > 1;
                            if (hasTabs)
                            {
                                sb.Append("<div id=\"deskItem_" + i.ToString() + "\" class=\"easyui-tabs\" fit=\"false\" style=\"height:260px\" data-options=\"tabHeight:" + ConstDefine.TAB_HEAD_HEIGHT.ToString() + ",tools:'#tab-tools_" + i.ToString() + "',onSelect:function(title,index){OnSelectTab(title,index," + i.ToString() + ");}\">");
                                toolsSb.AppendFormat("<a name=\"refresh\" title=\"刷新\" href=\"javascript:void(0)\" class=\"easyui-linkbutton easyui-tooltip\" title=\"刷新\" data-options=\"plain:true,iconCls:'eu-icon-arrow_refresh'\" onclick=\"Refresh({0},this)\"></a>", i);
                                toolsSb.AppendFormat("<a name=\"more\" title=\"点击查看更多...\" href=\"javascript:void(0)\" class=\"easyui-linkbutton easyui-tooltip\" title=\"更多...\" data-options=\"plain:true,iconCls:'eu-icon-more-customer'\" onclick=\"LoadMore({0},this)\"{1}></a>", i.ToString(), string.IsNullOrEmpty(desktopItems[i].Desktop_ItemTabs[0].MoreUrl) ? " style=\"display:none\"" : string.Empty);
                            }
                            else
                            {
                                toolsSb.AppendFormat("<a name=\"refresh\" title=\"刷新\" href=\"javascript:void(0)\" style=\"width:30px;\" class=\"eu-icon-arrow_refresh\" onclick=\"Refresh({0},this)\"></a>", i.ToString());
                                if (!string.IsNullOrEmpty(desktopItems[i].Desktop_ItemTabs[0].MoreUrl))
                                    toolsSb.AppendFormat("<a name=\"more\" title=\"点击查看更多...\" href=\"javascript:void(0)\" class=\"eu-icon-more-customer\" onclick=\"LoadMore({0},this)\"></a>", i.ToString());
                            }
                            foreach (Desktop_ItemTab deskItemTab in desktopItems[i].Desktop_ItemTabs)
                            {
                                sb.AppendFormat("<div id=\"deskPanel_{0}_{1}\" flag=\"{6}\" title=\"{2}\"{4}><iframe scrolling=\"auto\" frameborder=\"0\" class=\"ifr\" url=\"{3}\" hastab=\"{5}\" style=\"width:100%;height:100%;\"></iframe>", i.ToString(), deskItemTab.Id.ToString(), deskItemTab.Title, deskItemTab.Url.ObjToStr(), hasTabs ? string.Format(" style=\"height:229px;\"") : string.Format(" style=\"height:260px\" data-options=\"tools:'#tab-tools_{0}'\"", i.ToString()), hasTabs ? "1" : "0", deskItemTab.Flag.ObjToStr());
                                sb.AppendFormat("<input type=\"hidden\" tabName=\"{0}\" moreUrl=\"{1}\" />", deskItemTab.Title, deskItemTab.MoreUrl.ObjToStr());
                                sb.Append("</div>");
                            }
                            if (hasTabs)
                            {
                                sb.Append("</div>");
                            }
                            toolsSb.Append("</div>");
                        }
                    }
                    sb.Append("</div>");
                    //--column--end
                }
                sb.Append("</div>");
                sb.Append(toolsSb.ToString());
            }
            sb.Append("</div>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/easyui-extension/jquery.portal.js\"></script>");
            string desktopIndexJs = UIOperate.FormatJsPath("/Scripts/desktop/DesktopIndex.js");
            sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", desktopIndexJs);
            return sb.ToString();
        }

        /// <summary>
        /// 获取通用桌面列表页面
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="top">top</param>
        /// <param name="sortName">排序字段</param>
        /// <param name="isDesc">是否降序</param>
        /// <param name="dic">其他参数</param>
        /// <returns></returns>
        public override string GetDesktopGridHTML(Guid moduleId, int top = 5, string sortName = "AutoIncrmId", bool isDesc = true, Dictionary<string, object> dic = null)
        {
            StringBuilder sb = new StringBuilder();
            List<Desktop_GridField> fields = SystemOperate.GetDesktopGridFields(moduleId);
            if (fields.Count > 0)
            {
                Sys_Module module = SystemOperate.GetModuleById(moduleId);
                string titleKey = string.IsNullOrEmpty(module.TitleKey) ? string.Empty : module.TitleKey;
                string titleKeyDisplay = SystemOperate.GetModuleTitleKeyDisplay(module);
                string url = string.Format("/DataAsync/LoadGridData.html?moduleId={0}&rows={1}&sort={2}&order={3}", moduleId, top > 0 ? top.ToString() : "8", string.IsNullOrEmpty(sortName) ? "Id" : sortName, isDesc ? "desc" : "asc");
                GridParams gridParams = new GridParams();
                gridParams.DataOrUrl = url;
                gridParams.DicPramas = dic != null && dic.Count > 0 ? dic : new Dictionary<string, object>();
                gridParams.DicPramas.Add("top", top);
                gridParams.DicPramas.Add("sortName", sortName);
                gridParams.DicPramas.Add("isDesc", isDesc);
                List<Sys_GridField> gridFields = fields.Select(x => new Sys_GridField() { Sys_FieldName = x.FieidName, Width = x.Width, Sort = x.Sort }).ToList();
                gridParams.GridFields = gridFields;
                CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "GridParamsSet", new object[] { DataGridType.DesktopGrid, gridParams, null });
                string otherParams = string.Format("rownumbers:true,singleSelect:true,border:false,url:'{0}',method:'get'", gridParams.DataOrUrl);
                otherParams += ",onSelect:function(rowIndex, rowData){$('#deskgrid_" + moduleId.ToString() + "').datagrid('unselectAll');}";
                otherParams += ",onLoadSuccess:function(data){if (typeof (OverOnDeskGridLoadSuccess)=='function') {OverOnDeskGridLoadSuccess(data, '" + string.Format("deskgrid_{0}", moduleId.ToString()) + "');}}";
                if (gridParams != null && !string.IsNullOrEmpty(gridParams.OtherParmas))
                {
                    otherParams = gridParams.OtherParmas;
                }
                if (gridParams != null && gridParams.GridFields != null && gridParams.GridFields.Count > 0)
                {
                    fields = gridParams.GridFields.Where(x => !string.IsNullOrEmpty(x.Sys_FieldName)).Select(x => new Desktop_GridField() { FieidName = x.Sys_FieldName, Width = x.Width.HasValue && x.Width.Value > 0 ? x.Width.Value : 120, Sort = x.Sort }).ToList();
                }
                sb.AppendFormat("<table id=\"deskgrid_{0}\" style=\"height:98%\" class=\"easyui-datagrid\" data-options=\"{1}\">", moduleId.ToString(), otherParams);
                sb.Append("<thead>");
                sb.Append("<tr>");
                int editMode = 1;
                int formWidth = 500; //表单宽度
                int formHeight = 300; //表单高度
                if (fields.Select(x => x.FieidName).ToList().Contains(titleKey))
                {
                    UserInfo currUser = CurrUser;
                    Sys_Form form = SystemOperate.GetUserFinalForm(currUser, moduleId);
                    editMode = form != null ? GetEditMode(module, form, out formWidth, out formHeight, currUser.UserId) : (int)ModuleEditModeEnum.TabFormEdit; //编辑模式
                }
                string otherFormatParams = HttpUtility.UrlEncode("{moduleId:'" + module.Id.ToString() + "',moduleDisplay:'" + (string.IsNullOrEmpty(module.Display) ? module.Name : module.Display) + "',titleKey:'" + titleKey + "',titleKeyDisplay:'" + titleKeyDisplay + "',editMode:" + editMode.ToString() + ",editWidth:" + formWidth.ToString() + ",editHeight:" + formHeight.ToString() + ",gridId:'deskgrid_" + moduleId.ToString() + "'}", Encoding.UTF8).Replace("+", "%20");
                foreach (Desktop_GridField field in fields)
                {
                    string display = SystemOperate.GetFieldDisplay(moduleId, field.FieidName);
                    string formatter = SystemOperate.GetGridFormatFunction(moduleId, field.FieidName, null, false, otherFormatParams);
                    string formatStr = string.Empty;
                    if (!string.IsNullOrEmpty(formatter)) formatStr = string.Format(",formatter:{0}", formatter);
                    sb.AppendFormat("<th data-options=\"field:'{0}',hidden:{4},width:{1}{3}\">{2}</th>", field.FieidName, field.Width.ToString(), display, formatStr, field.Width == 0 ? "true" : "false");
                }
                sb.Append("</tr>");
                sb.Append("</thead>");
                sb.Append("</table>");
                string formatterJs = UIOperate.FormatJsPath("/Scripts/common/Formatter.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", formatterJs);
                string gridJs = UIOperate.FormatJsPath("/Scripts/common/Grid.js");
                sb.AppendFormat("<script type=\"text/javascript\" src=\"{0}\"></script>", gridJs);
                //模块自定义js
                sb.Append(UIOperate.GetModelJsHTML(moduleId, null));
            }
            return sb.ToString();
        }

        #endregion

        #region 邮件管理

        /// <summary>
        /// 获取邮件首页
        /// </summary>
        /// <returns></returns>
        public override string GetEmailIndexHTML()
        {
            StringBuilder sb = new StringBuilder();

            return sb.ToString();
        }

        #endregion

        #region 流程页面

        /// <summary>
        /// 获取流程设计页面
        /// </summary>
        /// <returns></returns>
        public override string GetFlowDesignHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div data-options=\"region:'west',title:'流程分类',onCollapse:TreePanelChange,onExpand:TreePanelChange\" style=\"height: 100%; width: 200px\">");
            sb.Append("<div id=\"treePanel\" class=\"easyui-panel\" style=\"padding: 5px; height: 100%\" data-options=\"border:false\">");
            sb.Append("<ul id=\"categoryTree\" class=\"easyui-tree\">");
            sb.Append("</ul>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<div data-options=\"region:'center',title:'流程画布',noheader:true,border:false\" style=\"height: 99%; width: 80%; overflow: hidden\">");
            sb.AppendFormat("<div id=\"flowTabs\" class=\"easyui-tabs\" style=\"width: 100%; height: 100%\" data-options=\"fit:true,border:false,tabHeight:{0}\">", ConstDefine.TAB_HEAD_HEIGHT);
            sb.Append("<div title=\"操作指南\" style=\"overflow: hidden;\">");
            sb.Append("<iframe scrolling=\"auto\" frameborder=\"0\" src=\"/Bpm/FlowOpGuide.html\" style=\"width: 100%; height: 100%;\"></iframe>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("<!--右键菜单-->");
            sb.Append("<div id=\"rightClkMenu\" class=\"easyui-menu\" style=\"width: 120px; display: none\">");
            sb.Append("<!--增加右键打开流程功能-->");
            sb.Append("<div id=\"open\" data-options=\"iconCls:'eu-p2-icon-book_open'\" onclick=\"OpenFlow()\">打开</div>");
            sb.Append("<div id=\"new\" data-options=\"iconCls:'eu-p2-icon-add_other'\" onclick=\"AddNewFlow()\">新建流程</div>");
            sb.Append("<div id=\"add\" data-options=\"iconCls:'eu-icon-folder'\" onclick=\"AddFolder()\">添加分类</div>");
            sb.Append("<!--增加右键删除功能-->");
            sb.Append("<div id=\"del\" data-options=\"iconCls:'eu-p2-icon-delete2'\" onclick=\"DeleteNode()\">删除</div>");
            sb.Append("<!--增加属性与其他功能按钮之间的分割线-->");
            sb.Append("<div id=\"sep\" class=\"menu-sep\"></div>");
            sb.Append("<!--增加右键显示属性功能-->");
            sb.Append("<div id=\"property\" data-options=\"iconCls:'eu-icon-cog'\" onclick=\"LoadFlow()\">属性</div>");
            sb.Append("</div>");
            sb.Append("<!--Tab标签上的右键菜单-->");
            sb.Append("<div id=\"mm-flow\" class=\"easyui-menu\" style=\"width: 150px;\">");
            sb.Append("<div id=\"mm-tabupdate\">刷新</div>");
            sb.Append("<div id=\"mm-tabclose\">关闭</div>");
            sb.Append("<div id=\"mm-tabcloseother\">除此之外全部关闭</div>");
            sb.Append("</div>");
            sb.Append("<!--tree panel上的右键菜单-->");
            sb.Append("<div id=\"mm-pannel\" class=\"easyui-menu\" style=\"width: 120px;\">");
            sb.Append("<div id=\"mm-addclass\" data-options=\"iconCls:'eu-icon-folder'\">添加分类</div>");
            sb.Append("</div>");
            sb.Append("<input id=\"hd_flow\" type=\"hidden\" />");
            return sb.ToString();
        }

        /// <summary>
        /// 获取流程画布页面
        /// </summary>
        /// <returns></returns>
        public override string GetFlowCanvasHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div id=\"gooFlowDom\" style=\"width: 100%; height: 100%; border-width: 0px;\"></div>");
            sb.Append("<!--结点、连线右键菜单-->");
            sb.Append("<div id=\"mm-chart\" class=\"easyui-menu\" style=\"width: 150px;\">");
            sb.Append("<div id=\"mm-set\">");
            sb.Append("    参数设置");
            sb.Append("</div>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取流程节点参数设置页面
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">节点TagId</param>
        /// <returns></returns>
        public override string GetNodeParamSetHTML(Guid workflowId, string tagId)
        {
            Bpm_WorkNode workNode = BpmOperate.GetWorkNodeByTagId(workflowId, tagId);
            Bpm_WorkNode preNode = workNode != null ? BpmOperate.GetPrexNode(workflowId, workNode.Id) : null;
            //结点按钮配置
            List<Bpm_NodeBtnConfig> btnConfigs = workNode != null ? BpmOperate.GetAllApprovalBtnConfigs(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeId == workNode.Id) : new List<Bpm_NodeBtnConfig>();
            StringBuilder sb = new StringBuilder();
            sb.Append("<div name=\"divPanel\" class=\"easyui-panel\" title=\"基本配置\" data-options=\"collapsible:true\" style=\"padding: 15px;\">");
            sb.AppendFormat("<input id=\"Id\" type=\"hidden\" value=\"{0}\" />", workNode != null ? workNode.Id.ToString() : "0");
            sb.Append("<table style=\"width: 100%; line-height: 30px;\">");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">结点编码：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Code\" name=\"Code\" class=\"easyui-textbox\" value=\"{0}\" data-options=\"disabled:true\" style=\"width: 180px\"></td>", workNode != null ? workNode.Code.ObjToStr() : string.Empty);
            sb.Append("<td class=\"td_label\">结点名称：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Name\" name=\"Name\" class=\"easyui-textbox\" value=\"{0}\" style=\"width: 180px\"></td>", workNode != null ? workNode.Name.ObjToStr() : string.Empty);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">结点表单：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Sys_FormId\" name=\"Sys_FormId\" class=\"easyui-combobox\" data-options=\"value:'{0}',editable:false\" style=\"width: 180px\"></td>", workNode != null && workNode.Sys_FormId.HasValue ? workNode.Sys_FormId.Value.ObjToStr() : Guid.Empty.ToString());
            sb.Append("<td class=\"td_label\">表单URL：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"FormUrl\" name=\"FormUrl\" class=\"easyui-textbox\" value=\"{0}\" data-options=\"disabled:true\" style=\"width: 180px\"></td>", workNode != null ? workNode.FormUrl.ObjToStr() : string.Empty);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">处理者类型：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"HandlerType\" name=\"HandlerType\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',editable:false,url:'/DataAsync/BindEnumFieldData.html?moduleId={1}&fieldName=HandlerType',value:{0}\" style=\"width: 180px\"></td>", workNode != null ? workNode.HandlerType : 0, SystemOperate.GetModuleIdByName("流程结点"));
            sb.Append("<td class=\"td_label\">处理策略：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"HandleStrategy\" name=\"HandleStrategy\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',editable:false,url:'/DataAsync/BindEnumFieldData.html?moduleId={1}&fieldName=HandleStrategy',value:{0}\" style=\"width: 180px\"></td>", workNode != null ? workNode.HandleStrategy : 0, SystemOperate.GetModuleIdByName("流程结点"));
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">处理范围：</td>");
            sb.Append("<td colspan=\"3\">");
            sb.Append("<input id=\"HandleRange\" name=\"HandleRange\" class=\"easyui-textbox\" value=\"" + (workNode != null ? workNode.HandleRange : string.Empty) + "\" data-options=\"multiline:true,editable:false,icons:[{iconCls:'eu-icon-search'}]\" style=\"width: 472px;height:44px\"></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">找处理者字段：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"FormFieldName\" name=\"FormFieldName\" class=\"easyui-combobox\" value=\"{0}\" data-options=\"editable:false\" style=\"width: 180px\"></td>", workNode != null ? workNode.FormFieldName.ObjToStr() : string.Empty);
            sb.Append("<td class=\"td_label\">子流程：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Bpm_WorkFlowSubId\" name=\"Bpm_WorkFlowSubId\" class=\"easyui-combobox\" data-options=\"editable:false,valueField:'Id',textField:'DisplayName',url:'/DataAsync/BindModuleComboData.html?moduleId={2}&where={1}'\" value=\"{0}\" style=\"width: 180px\"></td>", workNode != null && workNode.Bpm_WorkFlowSubId.HasValue ? workNode.Bpm_WorkFlowSubId.Value.ToString() : string.Empty, MySecurity.EncodeBase64(string.Format("Id!='{0}'", workflowId)), SystemOperate.GetModuleIdByName("流程信息"));
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">显示名称：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"DisplayName\" name=\"DisplayName\" class=\"easyui-textbox\" value=\"{0}\" style=\"width: 180px\"></td>", workNode != null ? workNode.DisplayName.ObjToStr() : string.Empty);
            sb.Append("<td class=\"td_label\">子流程类型：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"SubFlowType\" name=\"SubFlowType\" class=\"easyui-combobox\" data-options=\"editable:false,valueField:'Id',textField:'Name',editable:false,url:'/DataAsync/BindEnumFieldData.html?moduleId={1}&fieldName=SubFlowType',value:'{0}'\" style=\"width: 180px\"></td>", workNode != null ? workNode.SubFlowType : 1, SystemOperate.GetModuleIdByName("流程结点"));
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            if (preNode == null || (preNode != null && preNode.WorkNodeTypeOfEnum != WorkNodeTypeEnum.Start))
            {
                sb.Append("<div name=\"divPanel\" class=\"easyui-panel\" title=\"审批功能按钮\" data-options=\"collapsible:true\" style=\"padding: 15px;\">");
                sb.Append("<table style=\"width: 100%; line-height: 30px\">");
                List<Bpm_FlowBtn> flowBtns = BpmOperate.GetAllWorkButtons();
                string backTypeDisplay = "display:none;";
                if (flowBtns.Count > 0)
                {
                    for (int i = 0; i < flowBtns.Count; i++)
                    {
                        if (i % 2 == 0) sb.Append("<tr>");
                        Bpm_FlowBtn btn = flowBtns[i];
                        Bpm_NodeBtnConfig btnConfig = btnConfigs.Where(x => x.Bpm_WorkFlowId == workflowId && x.Bpm_WorkNodeId == workNode.Id && x.Bpm_FlowBtnId == btn.Id).FirstOrDefault();
                        string domTagId = string.Empty;
                        string chkValue = btnConfig != null ? (btnConfig.IsEnabled ? "1" : "0") : (workNode != null ? "0" : "1");
                        string disableStr = string.Empty;
                        string attr = string.Format("FlowBtnId=\"{0}\" BtnConfigId=\"{1}\"", btn.Id, btnConfig != null ? btnConfig.Id.ToString() : string.Empty);
                        switch (btn.ButtonTypeOfEnum)
                        {
                            case FlowButtonTypeEnum.AgreeBtn:
                                domTagId = "AgreeBtn";
                                disableStr = "disabled=\"disabled\"";
                                chkValue = "1";
                                break;
                            case FlowButtonTypeEnum.RejectBtn:
                                domTagId = "RejectBtn";
                                break;
                            case FlowButtonTypeEnum.BackBtn:
                                domTagId = "BackBtn";
                                if (chkValue == "1")
                                    backTypeDisplay = string.Empty;
                                break;
                            case FlowButtonTypeEnum.AssignBtn:
                                domTagId = "AsignBtn";
                                break;
                            case FlowButtonTypeEnum.CustomerBtn:
                                domTagId = "CustomerBtn" + btn.Id;
                                break;
                        }
                        sb.AppendFormat("<td class=\"td_label\">{0}标签：</td>", btn.ButtonText);
                        sb.Append("<td class=\"td_btnInput\">");
                        sb.AppendFormat("<input id=\"{0}\" tag=\"btn\" class=\"easyui-textbox\" style=\"width: 100px\" value=\"{1}\" {2} \"></td>", domTagId, btn.ButtonText.ObjToStr(), attr);
                        sb.Append("<td>");
                        sb.Append("<table>");
                        sb.Append("<tr>");
                        sb.Append("<td>启用</td>");
                        sb.Append("<td>");
                        sb.AppendFormat("<input id=\"{0}Enabled\" tag=\"btnEnable\" type=\"checkbox\" value=\"{1}\" {2} {3} {4} /></td>", domTagId, chkValue, chkValue == "1" ? "checked=\"checked\"" : string.Empty, disableStr, attr);
                        sb.Append("</tr>");
                        sb.Append("</table>");
                        sb.Append("</td>");
                        if (i % 2 != 0) sb.Append("</tr>");
                    }
                }
                sb.AppendFormat("<tr id=\"trBackType\" style=\"{0}\">", backTypeDisplay);
                sb.Append("<td class=\"td_label\">回退类型：</td>");
                sb.Append("<td colspan=\"5\">");
                sb.AppendFormat("<input id=\"BackType\" name=\"BackType\" class=\"easyui-combobox\" data-options=\"valueField:'Id',textField:'Name',url:'/DataAsync/BindEnumFieldData.html?moduleName=流程结点&fieldName=BackType',value:{0}\" style=\"width: 180px\"></td>", workNode != null ? workNode.BackType : 0);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</div>");
                string autoJumpRule = workNode != null ? workNode.AutoJumpRule.ObjToStr() : string.Empty;
                string[] token = autoJumpRule.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (token == null || token.Length != 5)
                {
                    token = workNode == null ? new string[] { "1", "1", "1", "0", "0" } : new string[] { "0", "0", "0", "0", "0" };
                }
                sb.Append("<div name=\"divPanel\" class=\"easyui-panel\" title=\"自动跳转规则\" data-options=\"collapsible:true\" style=\"padding: 15px;\">");
                sb.Append("<table style=\"width: 100%; height: 100%; line-height: 30px;\">");
                sb.Append("<tr>");
                sb.Append("<td>");
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td>处理者是发起者</td>");
                sb.Append("<td>");
                sb.AppendFormat("<input id=\"AutoJumpRule1\" type=\"checkbox\" value=\"{0}\" {1} /></td>", token[0], token[0] == "1" ? "checked=\"checked\"" : string.Empty);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td>处理者已出现过</td>");
                sb.Append("<td>");
                sb.AppendFormat("<input id=\"AutoJumpRule2\" type=\"checkbox\" value=\"{0}\" {1} /></td>", token[1], token[1] == "1" ? "checked=\"checked\"" : string.Empty);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td>处理者与上一步相同</td>");
                sb.Append("<td>");
                sb.AppendFormat("<input id=\"AutoJumpRule3\" type=\"checkbox\" value=\"{0}\" {1} /></td>", token[2], token[2] == "1" ? "checked=\"checked\"" : string.Empty);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td>找不到处理者</td>");
                sb.Append("<td>");
                sb.AppendFormat("<input id=\"AutoJumpRule4\" type=\"checkbox\" value=\"{0}\" {1} /></td>", token[3], token[3] == "1" ? "checked=\"checked\"" : string.Empty);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("<td>");
                sb.Append("<table>");
                sb.Append("<tr>");
                sb.Append("<td>提示错误</td>");
                sb.Append("<td>");
                sb.AppendFormat("<input id=\"AutoJumpRule5\" type=\"checkbox\" value=\"{0}\" {1} /></td>", token[4], token[4] == "1" ? "checked=\"checked\"" : string.Empty);
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</td>");
                sb.Append("</tr>");
                sb.Append("</table>");
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取流程连接线参数设置页面
        /// </summary>
        /// <param name="workflowId">流程ID</param>
        /// <param name="tagId">连线TagId</param>
        /// <returns></returns>
        public override string GetLineParamSetHTML(Guid workflowId, string tagId)
        {
            Bpm_WorkLine workLine = BpmOperate.GetWorkLineByTagId(workflowId, tagId);
            string code = string.Empty;
            string preNode = string.Empty;
            string nextNode = string.Empty;
            string note = string.Empty;
            bool iscustomer = false;
            string formCondition = string.Empty;
            string dutyCondition = string.Empty;
            string deptCondition = string.Empty;
            string sqlCondition = string.Empty;
            if (workLine != null)
            {
                code = workLine.Code.ObjToStr();
                if (workLine.Bpm_WorkNodeStartId.HasValue)
                {
                    Bpm_WorkNode node = BpmOperate.GetWorkNode(workLine.Bpm_WorkNodeStartId.Value);
                    if (node != null) preNode = node.Name.ObjToStr();
                }
                if (workLine.Bpm_WorkNodeEndId.HasValue)
                {
                    Bpm_WorkNode node = BpmOperate.GetWorkNode(workLine.Bpm_WorkNodeEndId.Value);
                    if (node != null) nextNode = node.Name.ObjToStr();
                }
                note = workLine.Note.ObjToStr();
                iscustomer = workLine.IsCustomerCondition;
                formCondition = HttpUtility.UrlEncode(workLine.FormCondition.ObjToStr());
                dutyCondition = HttpUtility.UrlEncode(workLine.DutyCondition.ObjToStr());
                deptCondition = HttpUtility.UrlEncode(workLine.DeptCondition.ObjToStr());
                sqlCondition = HttpUtility.UrlEncode(workLine.SqlCondition.ObjToStr());
            }
            Guid moduleId = BpmOperate.GetWorkflowModuleId(workflowId);
            Sys_Module module = SystemOperate.GetModuleById(moduleId);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<div class=\"easyui-tabs\" style=\"width: 100%; height: 100%\" data-options=\"fit:true,border:false,tabHeight:{0}\">", ConstDefine.TAB_HEAD_HEIGHT);
            sb.Append("<div title=\"基本信息\" style=\"padding: 15px;\">");
            sb.Append("<table style=\"width: 100%; line-height: 30px;\">");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">连线编码：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Code\" type=\"text\" class=\"easyui-textbox\" data-options=\"disabled:true,value:'{0}'\" style=\"width: 250px\" /></td>", code);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">前一结点：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"PreNode\" type=\"text\" class=\"easyui-textbox\" data-options=\"disabled:true,value:'{0}'\" style=\"width: 250px\" /></td>", preNode);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">后一结点：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"NextNode\" type=\"text\" class=\"easyui-textbox\" data-options=\"disabled:true,value:'{0}'\" style=\"width: 250px\" /></td>", nextNode);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">连线标注：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"Note\" type=\"text\" class=\"easyui-textbox\" data-options=\"value:'{0}'\" style=\"width: 250px\" /></td>", note);
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_label\">是否自定义条件：</td>");
            sb.Append("<td>");
            sb.AppendFormat("<input id=\"IsCustomerCondition\" type=\"checkbox\" {0} /></td>", iscustomer ? "checked=\"checked\" value=\"1\"" : string.Empty);
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<div title=\"表单条件\" style=\"padding: 5px;\">");
            sb.AppendFormat("<div id=\"formCondition\" moduleName=\"{0}\" condition=\"{1}\"></div>", module != null ? module.Name : string.Empty, formCondition);
            sb.Append("</div>");
            sb.Append("<div title=\"职务条件\" style=\"padding: 5px;\">");
            sb.AppendFormat("<div id=\"dutyCondition\" condition=\"{0}\"></div>", dutyCondition);
            sb.Append("</div>");
            sb.Append("<div title=\"部门条件\" style=\"padding: 5px;\">");
            sb.AppendFormat("<div id=\"deptCondition\" condition=\"{0}\"></div>", deptCondition);
            sb.Append("</div>");
            sb.Append("<div title=\"SQL条件\" style=\"padding: 15px;\">");
            sb.AppendFormat("<input id=\"sql\" class=\"easyui-textbox\" data-options=\"multiline:true,prompt:'以AND开头的WHERE条件语句',value:'{0}'\" style=\"width:600px;height:300px\" /> ", sqlCondition);
            sb.Append("</div>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取选择回退结点页面
        /// </summary>
        /// <param name="workTodoId">待办ID</param>
        /// <returns></returns>
        public override string GetSelectBackNodeHTML(Guid workTodoId)
        {
            StringBuilder sb = new StringBuilder();
            Guid workNodeId = BpmOperate.GetWorkNodeIdByTodoId(workTodoId);
            Bpm_WorkNode workNode = BpmOperate.GetWorkNode(workNodeId);
            sb.Append("<div style=\"padding:10px\">");
            sb.Append("<table style=\"line-height:50px\"><tr>");
            sb.Append("<td style=\"width:150px\">请选择回退结点：</td>");
            sb.Append("<td><select id=\"backNode\" class=\"easyui-combobox\" style=\"width:200px;\">");
            if (workNode != null && workNode.Bpm_WorkFlowId.HasValue && workNode.Bpm_WorkFlowId.Value != Guid.Empty)
            {
                switch (workNode.BackTypeOfEnum)
                {
                    case NodeBackTypeEnum.BackToLast:
                        {
                            Bpm_WorkNode preNode = BpmOperate.GetPrexNode(workNode.Bpm_WorkFlowId.Value, workNodeId);
                            if (preNode != null)
                            {
                                sb.AppendFormat("<option value=\"{0}\">{1}</option>", preNode.Id, preNode.Name);
                            }
                        }
                        break;
                    case NodeBackTypeEnum.BackToFirst:
                        {
                            Bpm_WorkNode firstNode = BpmOperate.GetLaunchNode(workNode.Bpm_WorkFlowId.Value);
                            if (firstNode != null)
                            {
                                sb.AppendFormat("<option value=\"{0}\">{1}</option>", firstNode.Id, firstNode.Name);
                            }
                        }
                        break;
                    case NodeBackTypeEnum.BackToAll:
                        {

                        }
                        break;
                }
            }
            sb.Append("</select></td>");
            sb.Append("</tr></table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取流程tips页面HTML
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="recordId">记录ID</param>
        /// <returns></returns>
        public override string GetFlowTipsHTML(Guid moduleId, Guid recordId)
        {
            StringBuilder sb = new StringBuilder();
            Bpm_WorkFlowInstance workflowInst = BpmOperate.GetWorkflowInstance(moduleId, recordId);
            if (workflowInst == null)
            {
                string errMsg = string.Empty;
                List<Guid?> workflowIds = BpmOperate.GetAllWorkflows(x => x.Sys_ModuleId == moduleId).Select(x => (Guid?)x.Id).ToList();
                Bpm_WorkFlowInstanceHistory workflowInstHistory = workflowIds.Count > 0 ? CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => workflowIds.Contains(x.Bpm_WorkFlowId) && x.RecordId == recordId, null, out errMsg) :
                                                                 CommonOperate.GetEntity<Bpm_WorkFlowInstanceHistory>(x => x.Bpm_WorkFlowId == workflowIds.FirstOrDefault().Value && x.RecordId == recordId, null, out errMsg);
                if (workflowInstHistory != null)
                {
                    workflowInst = new Bpm_WorkFlowInstance();
                    ObjectHelper.CopyValue(workflowInstHistory, workflowInst);
                }
            }
            object objStr = CommonOperate.ExecuteCustomeOperateHandleMethod(moduleId, "GetFlowTipsHTML", new object[] { recordId, workflowInst != null ? workflowInst.StatusOfEnum : WorkFlowStatusEnum.NoStatus, CurrUser });
            if (objStr != null && objStr.ObjToStr() != string.Empty)
                return objStr.ObjToStr();
            if (workflowInst != null)
            {
                string flowStatusDes = string.Empty;
                string icon = "/Css/icons/";
                switch (workflowInst.StatusOfEnum)
                {
                    case WorkFlowStatusEnum.NoStatus:
                        flowStatusDes = "未提交";
                        icon += "tosubmit.png";
                        break;
                    case WorkFlowStatusEnum.Start:
                        flowStatusDes = "已发起";
                        icon += "toapproval.png";
                        break;
                    case WorkFlowStatusEnum.Approving:
                        flowStatusDes = "审批中";
                        icon += "inapproval.png";
                        break;
                    case WorkFlowStatusEnum.Return:
                        flowStatusDes = "被退回";
                        icon += "toreturn.png";
                        break;
                    case WorkFlowStatusEnum.Refused:
                        flowStatusDes = "被拒绝";
                        icon += "reject.png";
                        break;
                    case WorkFlowStatusEnum.Freezed:
                        icon = string.Empty;
                        break;
                    case WorkFlowStatusEnum.Over:
                        flowStatusDes = "已通过";
                        icon += "approvalok.png";
                        break;
                    case WorkFlowStatusEnum.Customer:
                        return string.Empty;
                }
                string flowImgStr = string.Empty;
                if (icon != string.Empty)
                {
                    flowImgStr = string.Format("<img src=\"{0}\" />", icon);
                }
                List<ApprovalInfo> approvalInfos = new List<ApprovalInfo>();
                if (workflowInst.StatusOfEnum == WorkFlowStatusEnum.Refused)
                {
                    //拒绝的判断最后一个拒绝
                    List<ApprovalInfo> tempAppInfo = BpmOperate.GetRecordApprovalInfos(workflowInst.Id, false);
                    tempAppInfo = tempAppInfo.Where(x => x.HandleResult == "拒绝").ToList();
                    if (tempAppInfo.Count > 0)
                    {
                        approvalInfos.Add(new ApprovalInfo()
                        {
                            NodeId = tempAppInfo.FirstOrDefault().NodeId,
                            NodeName = tempAppInfo.FirstOrDefault().NodeName,
                            Handler = string.Join(",", tempAppInfo.Select(x => x.Handler)),
                            HandleResult = tempAppInfo.FirstOrDefault().HandleResult,
                            HandleTime = tempAppInfo.FirstOrDefault().HandleTime,
                            ApprovalOpinions = string.Join(",", tempAppInfo.Where(x => !string.IsNullOrEmpty(x.ApprovalOpinions)).Select(x => string.Format("{0}：{1}", x.Handler, x.ApprovalOpinions))),
                            NextName = tempAppInfo.FirstOrDefault().NextName,
                            NextNodeName = tempAppInfo.FirstOrDefault().NextNodeName,
                            NextHandler = tempAppInfo.FirstOrDefault().NextHandler
                        });
                    }
                }
                else
                {
                    approvalInfos = BpmOperate.GetRecordApprovalInfos(workflowInst.Id, true);
                }
                if (approvalInfos.Count > 0)
                {
                    ApprovalInfo info = approvalInfos.FirstOrDefault();
                    workflowInst.OrgM_EmpName = workflowInst.OrgM_EmpId.HasValue ? OrgMOperate.GetEmpName(workflowInst.OrgM_EmpId.Value) : string.Empty;
                    sb.Append("<table style=\"width:100%\">");
                    sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">发起人：</th><td>{0}</td></tr>", workflowInst.OrgM_EmpName);
                    sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">状&nbsp;&nbsp;&nbsp;态：</th><td><table><tr><td>{0}</td><td>{1}</td></tr></table></td></tr>", flowImgStr, flowStatusDes);
                    if (workflowInst.StatusOfEnum != WorkFlowStatusEnum.Over && workflowInst.StatusOfEnum != WorkFlowStatusEnum.Refused)
                    {
                        sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">下一处理人：</th><td>{0}</td></tr>", info.NextHandler);
                    }
                    else if (workflowInst.StatusOfEnum == WorkFlowStatusEnum.Refused)
                    {
                        sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">原&nbsp;&nbsp;&nbsp;因：</th><td>{0}</td></tr>", info.ApprovalOpinions);
                    }
                    sb.Append("</table>");
                }
            }
            else
            {
                string errMsg = string.Empty;
                object obj = CommonOperate.GetEntityById(moduleId, recordId, out errMsg, new List<string>() { "CreateUserName", "FlowStatus" });
                BaseEntity model = obj as BaseEntity;
                string createUser = model != null ? model.CreateUserName : string.Empty;
                string statusDes = model != null && model.FlowStatus == (int)WorkFlowStatusEnum.Over ? "已通过" : "未提交";
                sb.Append("<table style=\"width:100%\">");
                sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">创建人：</th><td>{0}</td></tr>", createUser);
                sb.AppendFormat("<tr><th style=\"text-align:right;width:80px;\">流程状态：</th><td>{0}</td></tr>", statusDes);
                sb.Append("</table>");
            }
            return sb.ToString();
        }

        #endregion

        #region 调度页面

        /// <summary>
        /// 获取调度中心页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetQuartzCenterHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"content\">");
            sb.Append("<table class=\"easyui-datagrid\" data-options=\"title:'调度服务',fitColumns:false,singleSelect:true,toolbar:toolbar,onSelect:function(rowIndex, rowData){$(this).datagrid('unselectAll');}\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'aaa',width:150,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'bbb',width:300,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'ccc',width:150,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'ddd',width:300,align:'left'\"></th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td>服务状态：</td>");
            sb.Append("<td><span id=\"serviceStatus\" style=\"color: Blue\"></span></td>");
            sb.Append("<td>服务器主机名：</td>");
            sb.Append("<td><span id=\"remoteHostName\" style=\"color: Blue\"></span></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>客户机主机名：</td>");
            sb.Append("<td><span id=\"localHostName\" style=\"color: Blue\"></span></td>");
            sb.Append("<td>客户机IP地址：</td>");
            sb.Append("<td><span id=\"localIp\" style=\"color: Blue\"></span></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>操作信息：</td>");
            sb.Append("<td><span id=\"operateInfo\" style=\"color: Red\"></span></td>");
            sb.Append("<td>异常信息：</td>");
            sb.Append("<td><span id=\"exceptionMsg\" style=\"color: Red\"></span></td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("<br />");
            sb.Append("<table id=\"dg_schedule\" class=\"easyui-datagrid\" data-options=\"title:'作业信息',fitColumns:false,singleSelect:true,onSelect:function(rowIndex, rowData){$(this).datagrid('unselectAll');}\">");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<th data-options=\"field:'eee',width:150,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'fff',width:300,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'ggg',width:150,align:'left'\"></th>");
            sb.Append("<th data-options=\"field:'hhh',width:300,align:'left'\"></th>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td>作业名称：</td>");
            sb.Append("<td><span id=\"SchedulerName\"></span></td>");
            sb.Append("<td>实例编码：</td>");
            sb.Append("<td><span id=\"SchedulerInstanceId\"></span></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>是否远程调度：</td>");
            sb.Append("<td><span id=\"IsRemote\"></span></td>");
            sb.Append("<td>作业类型：</td>");
            sb.Append("<td><span id=\"SchedulerType\"></span></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>运行开始时间：</td>");
            sb.Append("<td><span id=\"StartRunningTime\"></span></td>");
            sb.Append("<td>运行状态：</td>");
            sb.Append("<td><span id=\"SchedulerStatus\"></span></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>总的任务数：</td>");
            sb.Append("<td><span id=\"TotalTaskNum\"></span></td>");
            sb.Append("<td>已执行任务数：</td>");
            sb.Append("<td><span id=\"ExcutedTaskNum\"></span></td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/Quartz/QuartzCenter.js\"></script>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取任务管理页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetJobManageHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"content\">");
            sb.Append("<table id=\"dg\"></table>");
            sb.Append("</div>");
            sb.Append("<script type=\"text/javascript\" src= \"/Scripts/easyui-extension/datagrid-detailview.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/Quartz/JobManage.js\"></script>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取添加任务页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetAddJobHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style=\"padding: 10px;\">");
            sb.Append("<table style=\"width:96%;margin-left: 2px;\">");
            sb.Append("<tr style=\"height: 50px; text-align: center\">");
            sb.Append("<td>");
            sb.Append("<a id=\"btnSave\" style=\"margin-right: 5px;\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-save'\">保 存</a>");
            sb.Append("<a id=\"btnCancel\" style=\"margin-left: 5px;\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-cancel'\">取 消</a>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<div class=\"easyui-panel\" title=\"\" data-options=\" collapsible:true\" style=\"padding: 10px; margin-bottom: 5px\">");
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
            sb.Append("<tbody id=\"jobTbody\">");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">任务组：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input id=\"JobGroup\" class=\"easyui-combobox\" name=\"dept\" data-options=\"required:true,valueField:'id',textField:'text',url:'/Quartz/GetJobGroupList.html'\" />");
            sb.Append("</td>");
            sb.Append("<td class=\"td_text\">任务名称：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input id=\"JobName\" class=\"easyui-validatebox\" data-options=\"required:true\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">任务类型：</td>");
            sb.Append("<td class=\"td_value\" colspan=\"3\">");
            sb.Append("<input style=\"width: 484px;\" id=\"JobType\" class=\"easyui-combobox\" name=\"JobType\" data-options=\"required:true,valueField:'id',textField:'text',editable:false,url:'/Quartz/GetJobTypeList.html'\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">任务说明：</td>");
            sb.Append("<td class=\"td_value\" colspan=\"3\">");
            sb.Append("<input style=\"width: 480px\" id=\"JobDes\" name=\"JobDes\" title=\"任务说明\" type=\"text\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td id=\"td_List\">");
            sb.Append("<div>");
            sb.Append("<table id=\"tb_JobPlan\"></table>");
            sb.Append("</div>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取添加任务计划页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetAddJobPlanHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div align=\"left\" style=\"width: 95%; margin: 10px 0px 0px 10px;\">");
            sb.Append("<table cellpadding=\"0\" cellspacing=\"0\">");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划名称：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 204px;\" id=\"PlanName\" class=\"easyui-validatebox\" data-options=\"required:true\" /></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">所属分组：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 208px;\" id=\"PlanGroup\" class=\"easyui-combobox\" name=\"PlanGroup\" data-options=\"required:true,valueField:'id',textField:'text',editable:true,url:'/Quartz/GetPlanGroupList.html'\" />");
            sb.Append("<input id=\"PlanGroupH\" type=\"hidden\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划类型：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<select id=\"IsRepeat\" class=\"easyui-combobox\" name=\"IsRepeat\" style=\"width: 208px;\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"1\">重复执行</option>");
            //sb.Append("<option value=\"0\">执行一次</option>");
            sb.Append("</select>");
            sb.Append("<input id=\"IsRepeatH\" type=\"hidden\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划开始时间：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 208px;\" id=\"PlanStartTime\" class=\"easyui-datetimebox\" name=\"PlanStartTime\" data-options=\"showSeconds:true\" />");
            sb.Append("<input id=\"PlanStartTimeH\" type=\"hidden\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划结束时间：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 208px;\" id=\"PlanEndTime\" class=\"easyui-datetimebox\" name=\"PlanEndTime\" data-options=\"showSeconds:true\" />");
            sb.Append("<input id=\"PlanEndTimeH\" type=\"hidden\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">执行状态：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<select id=\"PlanStatus\" class=\"easyui-combobox\" name=\"PlanStatus\" style=\"width: 208px;\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"1\">等待执行</option>");
            sb.Append("<option value=\"0\">立即执行</option>");
            sb.Append("</select>");
            sb.Append("<input id=\"PlanStatusH\" type=\"hidden\" />");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trJobRepeatIntervalType\">");
            sb.Append("<td class=\"td_text\">执行频率：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<select id=\"JobRepeatIntervalType\" class=\"easyui-combobox\" name=\"JobRepeatIntervalType\" style=\"width: 208px;\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"EveryDay\">每天</option>");
            sb.Append("<option value=\"EveryWeek\">每周</option>");
            sb.Append("<option value=\"EveryMonth\">每月</option>");
            sb.Append("</select>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trJobRepeatInterval\">");
            sb.Append("<td class=\"td_text\">执行间隔：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td><input style=\"width: 100px;\" class=\"easyui-numberbox\" value=\"1\" data-options=\"min:0,precision:0\" id=\"JobRepeatInterval\" name=\"JobRepeatInterval\" type=\"text\" /></td>");
            sb.Append("<td>&nbsp;&nbsp;<span id=\"spanRepeatInterval\"></span></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trIntervalWeek\" style=\"display: none\">");
            sb.Append("<td class=\"td_text\">周执行频率：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 200px;\" id=\"IntervalWeek\" name=\"IntervalWeek\" type=\"hidden\" />");
            sb.Append("<input id=\"chk2\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"2\" />周一");
            sb.Append("<input id=\"chk3\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"3\" />周二");
            sb.Append("<input id=\"chk4\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"4\" />周三");
            sb.Append("<input id=\"chk5\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"5\" />周四");
            sb.Append("<input id=\"chk6\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"6\" />周五");
            sb.Append("<input id=\"chk7\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"7\" />周六");
            sb.Append("<input id=\"chk1\" name=\"checkbox\" onclick=\"SetIntervalWeek();\" type=\"checkbox\" value=\"1\" />周日</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trIntervalMonthType\" style=\"display: none\">");
            sb.Append("<td class=\"td_text\">月执行频率：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<select id=\"IntervalMonthType\" class=\"easyui-combobox\" name=\"IntervalMonthType\" style=\"width: 208px;\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"1\">天</option>");
            sb.Append("<option value=\"2\">周</option>");
            sb.Append("</select>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trIntervalMonthDay\" style=\"display: none\">");
            sb.Append("<td class=\"td_text\">每月第：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<input style=\"width: 164px;\" value=\"1\" data-options=\"min:1,precision:0\" id=\"IntervalMonthDay\" name=\"IntervalMonthDay\" type=\"text\" />");
            sb.Append("天执行</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trIntervalMonthWeek\" style=\"display: none\">");
            sb.Append("<td class=\"td_text\">每月：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<select style=\"width: 120px;\" class=\"easyui-combobox\" id=\"NumWeekDay\" name=\"NumWeekDay\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"1\">第一个</option>");
            sb.Append("<option value=\"2\">第二个</option>");
            sb.Append("<option value=\"3\">第三个</option>");
            sb.Append("<option value=\"4\">第四个</option>");
            sb.Append("<option value=\"5\">最后一个</option>");
            sb.Append("</select>");
            sb.Append("<select id=\"IntervalMonthWeek\" class=\"easyui-combobox\" name=\"IntervalMonthWeek\" style=\"width: 80px\" title=\"月间隔时，按周执行时的周间隔频率\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"2\">周一</option>");
            sb.Append("<option value=\"3\">周二</option>");
            sb.Append("<option value=\"4\">周三</option>");
            sb.Append("<option value=\"5\">周四</option>");
            sb.Append("<option value=\"6\">周五</option>");
            sb.Append("<option value=\"7\">周六</option>");
            sb.Append("<option value=\"1\">周日</option>");
            sb.Append("</select></td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trEverydayIntervalType\">");
            sb.Append("<td class=\"td_text\">每天重复执行：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td style=\"width:30px;\">");
            sb.Append("<select id=\"EverydayIntervalType\" class=\"easyui-combobox\" name=\"EverydayIntervalType\" style=\"width: 44px;\" data-options=\"editable:false\">");
            sb.Append("<option value=\"1\">是</option>");
            sb.Append("<option selected=\"selected\" value=\"0\">否</option>");
            sb.Append("</select></td>");
            sb.Append("<td style=\"width:64px;text-align:center\"><span id=\"spanEverydayIntervalLabel\">&nbsp;&nbsp;时间&nbsp;&nbsp;</span></td>");
            sb.Append("<td style=\"width:90px\" id=\"tbEverydayIntervalFrmTime\">");
            sb.Append("<input id=\"EverydayIntervalFrmTime\" class=\"easyui-timespinner\" required=\"required\" value=\"00:00:00\" data-options=\"min:'00:00',showSeconds:true,required:true\" style=\"width: 90px;\" title=\"每天开始时间\" type=\"text\" /></td>");
            sb.Append("<td style=\"width:16px;\" id=\"tdEverydayIntervalSplit\"><span>&nbsp;-&nbsp;</span></td>");
            sb.Append("<td style=\"width:90px;\" id=\"tdEverydayIntervalEndTime\">");
            sb.Append("<input id=\"EverydayIntervalEndTime\" class=\"easyui-timespinner\" required=\"required\" value=\"23:59:59\" data-options=\"min:'00:30',showSeconds:true\" style=\"width: 90px\" title=\"每天结束时间\" type=\"text\" /></td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr id=\"trEverydayInterval\">");
            sb.Append("<td class=\"td_text\">每天间隔频率：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<input style=\"width: 120px;\" id=\"EverydayInterval\" class=\"easyui-numberbox\" value=\"1\" data-options=\"min:0,precision:0\" name=\"EverydayInterval\" type=\"text\" /></td>");
            sb.Append("<td>");
            sb.Append("&nbsp;&nbsp;<select id=\"EverydayIntervalUnit\" class=\"easyui-combobox\" name=\"EverydayIntervalUnit\" style=\"width: 77px;\" data-options=\"editable:false\">");
            sb.Append("<option selected=\"selected\" value=\"H\">小时</option>");
            sb.Append("<option value=\"M\">分</option>");
            sb.Append("<option value=\"S\">秒</option>");
            sb.Append("</select>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划说明：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<textarea cols=\"50\" id=\"PlanDes\" readonly=\"readonly\" rows=\"3\"></textarea>");
            sb.Append("<td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"td_text\">计划公式：</td>");
            sb.Append("<td class=\"td_value\">");
            sb.Append("<table>");
            sb.Append("<tr>");
            sb.Append("<td>");
            sb.Append("<input id=\"CronExp\" name=\"CronExp\" readonly=\"readonly\" style=\"width: 200px\" title=\"任务格式化字符串\" type=\"text\" />");
            sb.Append("</td>");
            sb.Append("<td>");
            sb.Append("&nbsp;&nbsp;<a id=\"btnCronExp\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"iconCls:'eu-icon-ok'\" onclick=\"CreateTaskPlan();\">生成计划</a>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</div>");
            return sb.ToString();
        }

        /// <summary>
        /// 获取查看执行日志页面HTML
        /// </summary>
        /// <returns></returns>
        public override string GetQuartzLogHTML()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"content\">");
            sb.Append("<table id=\"dg\"></table>");
            sb.Append("</div>");
            sb.Append("<script type=\"text/javascript\" src= \"/Scripts/easyui-extension/datagrid-groupview.js\"></script>");
            sb.Append("<script type=\"text/javascript\" src=\"/Scripts/Quartz/QuartzLog.js\"></script>");
            return sb.ToString();
        }

        #endregion
    }
}