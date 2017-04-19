/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Operate.Base;
using System.Collections.Generic;
using System.Web.Mvc;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Controllers.Attr;
using System.Threading.Tasks;
using Rookey.Frame.Common;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 通用页面控制器
    /// </summary>
    public class PageController : BaseController
    {
        /// <summary>
        /// 测试页面
        /// </summary>
        /// <returns></returns>
        [Anonymous]
        public ActionResult Test()
        {
            return View();
        }

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        [Anonymous]
        public ActionResult Init()
        {
            string errMsg = ToolOperate.InitData();
            if (string.IsNullOrEmpty(errMsg))
            {
                return RedirectToAction("Login", "User");
            }
            ViewBag.ErrMsg = errMsg;
            return View();
        }

        #endregion

        #region 主页面

        /// <summary>
        /// 主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Main()
        {
            return View();
        }

        /// <summary>
        /// 个人设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonalSet()
        {
            return View();
        }

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePwd()
        {
            return View();
        }

        /// <summary>
        /// 添加快捷菜单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddQuckMenu()
        {
            return View();
        }

        #endregion

        #region 列表页面

        /// <summary>
        /// 通用列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Grid()
        {
            return View();
        }

        /// <summary>
        /// 通用高级搜索页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AdvanceSearch()
        {
            return View();
        }

        /// <summary>
        /// 快速添加、编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickEditView()
        {
            return View();
        }

        /// <summary>
        /// 列表视图设置
        /// </summary>
        /// <returns></returns>
        public ActionResult GridSet()
        {
            return View();
        }

        /// <summary>
        /// 附属模块设置
        /// </summary>
        /// <returns></returns>
        public ActionResult AttachModuleSet()
        {
            return View();
        }

        #endregion

        #region 表单页面

        /// <summary>
        /// 通用表单页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditForm()
        {
            return View();
        }

        /// <summary>
        /// 通用查看页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewForm()
        {
            return View();
        }

        /// <summary>
        /// 通用上传附件页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadForm()
        {
            return View();
        }

        /// <summary>
        /// 表单页面图片控件上传表单
        /// </summary>
        /// <returns></returns>
        public ActionResult ImgUploadForm()
        {
            return View();
        }

        /// <summary>
        /// 编辑字段页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditField()
        {
            return View();
        }

        /// <summary>
        /// 批量编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult BatchEdit()
        {
            return View();
        }

        /// <summary>
        /// 在线查看文档
        /// </summary>
        /// <returns></returns>
        public ActionResult DocView()
        {
            return View();
        }

        /// <summary>
        /// 通用导入实体页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportModel()
        {
            return View();
        }

        /// <summary>
        /// 通用导出设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportModel()
        {
            return View();
        }

        /// <summary>
        /// 下拉框数据源设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CombDataSourceSet()
        {
            return View();
        }

        #endregion

        #region 公共页面

        /// <summary>
        /// 图标选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IconSelect()
        {
            return View();
        }

        /// <summary>
        /// 通用弹出树
        /// </summary>
        /// <returns></returns>
        public ActionResult DialogTree()
        {
            return View();
        }

        #endregion

        #region 特殊页面

        /// <summary>
        /// 设置用户角色页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetUserRole()
        {
            return View();
        }

        /// <summary>
        /// 设置角色表单
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRoleForm()
        {
            return View();
        }

        /// <summary>
        /// 快速编辑表单
        /// </summary>
        /// <returns></returns>
        public ActionResult QuickEditForm()
        {
            return View();
        }

        /// <summary>
        /// 系统配置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WebConfig()
        {
            return View();
        }

        /// <summary>
        /// 添加通用按钮页面
        /// </summary>
        /// <returns></returns>
        public ActionResult AddCommonBtn()
        {
            return View();
        }

        #endregion

        #region 权限页面

        #region 角色权限

        /// <summary>
        /// 设置角色模块权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRoleModulePermission()
        {
            return View();
        }

        /// <summary>
        /// 设置角色权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRolePermission()
        {
            return View();
        }

        /// <summary>
        /// 角色数据权限设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleDataPermissionSet()
        {
            return View();
        }

        /// <summary>
        /// 角色字段权限设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleFieldPermissionSet()
        {
            return View();
        }

        #endregion

        #region 用户权限

        /// <summary>
        /// 设置用户权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult SetUserPermission()
        {
            return View();
        }

        /// <summary>
        /// 用户数据权限设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDataPermissionSet()
        {
            return View();
        }

        /// <summary>
        /// 用户字段权限设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UserFieldPermissionSet()
        {
            return View();
        }

        #endregion

        #endregion

        #region 我的桌面

        /// <summary>
        /// 我的桌面主页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DesktopIndex()
        {
            return View();
        }

        /// <summary>
        /// 获取桌面列表页面
        /// </summary>
        /// <returns></returns>
        public ActionResult DesktopGrid()
        {
            return View();
        }

        #endregion

        #region 邮件管理

        /// <summary>
        /// 邮件首页
        /// </summary>
        /// <returns></returns>
        public ActionResult EmailIndex()
        {
            return View();
        }

        #endregion
    }
}
