/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Rookey.Frame.Common;
using System.Web;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Base;
using Rookey.Frame.Controllers.Attr;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Operate.Base.Extension;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using Rookey.Frame.Controllers.Other;
using System.Web.Security;
using Rookey.Frame.Base.Set;
using Rookey.Frame.Model.OrgM;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 用户控制器（异步）
    /// </summary>
    public class UserAsyncController : AsyncBaseController
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="userpwd">密码</param>
        /// <param name="valcode">验证码</param>
        /// <returns></returns>
        [Login]
        [Anonymous]
        public Task<ActionResult> UserLoginAsync(string username, string userpwd, string valcode)
        {
            return Task.Factory.StartNew(() =>
            {
                return new UserController(Request, Response, Session, TempData).UserLogin(username, userpwd, valcode);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 获取数据权限组织树
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> GetDataPermissionOrgTreeAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new UserController(Request).GetDataPermissionOrgTree();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public Task<ActionResult> ChangePwdAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new UserController(Request).ChangePwd();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }
    }

    /// <summary>
    /// 用户控制器
    /// </summary>
    public class UserController : BaseController
    {
        #region 构造函数

        private HttpSessionStateBase _Session = null; //Session
        private HttpRequestBase _Request = null; //请求对象
        private HttpResponseBase _Response = null; //响应对象
        private TempDataDictionary _TempData = null; //临时字典对象

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public UserController()
        {
            _Request = Request;
            _Session = Session;
            _Response = Response;
            _TempData = TempData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public UserController(HttpRequestBase request)
            : base(request)
        {
            _Request = request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        /// <param name="response">返回请求</param>
        /// <param name="session">session</param>
        /// <param name="tempData">临时数据</param>
        public UserController(HttpRequestBase request, HttpResponseBase response, HttpSessionStateBase session, TempDataDictionary tempData)
            : base(request)
        {
            _Request = request;
            _Session = session;
            _Response = response;
            _TempData = tempData;
        }

        #endregion

        #region 公共方法

        private const string LOGINERROR = "LoginError";

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [Anonymous]
        public ActionResult Login()
        {
            if (!ToolOperate.IsNeedInit()) //不需要初始化时
            {
                if (WebConfigHelper.GetAppSettingValue("NeedRepairTable") == "true") //需要修复数据表
                {
                    string tables = WebConfigHelper.GetAppSettingValue("RepairTables"); //要修复的数据表
                    List<string> token = new List<string>();
                    if (!string.IsNullOrEmpty(tables))
                    {
                        token = tables.Split(",".ToCharArray()).ToList();
                    }
                    ToolOperate.RepairTables(token);
                }
            }
            else //需要初始化
            {
                return RedirectToAction("Init", "Page");
            }
            ViewBag.IsShowValidateCode = (Session[LOGINERROR].ObjToInt() >= 2).ToString().ToLower();
            return View();
        }

        /// <summary>
        /// 弹出登录框
        /// </summary>
        /// <returns></returns>
        [Anonymous]
        public ActionResult DialogLogin()
        {
            ViewBag.IsShowValidateCode = (Convert.ToInt32(Session[LOGINERROR]) >= 2).ToString().ToLower();
            return View();
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="userpwd">密码</param>
        /// <param name="valcode">验证码</param>
        /// <returns></returns>
        [Login]
        [Anonymous]
        public ActionResult UserLogin(string username, string userpwd, string valcode)
        {
            if (_Request == null) _Request = Request;
            if (_Response == null) _Response = Response;
            if (_Session == null) _Session = Session;
            string errMsg = string.Empty;
            ViewBag.IsShowValidateCode = "false";
            bool isNoCode = _Request["isNoCode"].ObjToBool(); //是否不需要验证码
            if (!isNoCode && _Session[LOGINERROR].ObjToInt() >= 2)
            {
                bool validatecode = false;
                if (_TempData.ContainsKey(SecurityController.VALIDATECODE))
                {
                    string code = _TempData[SecurityController.VALIDATECODE].ToString();
                    validatecode = valcode.ToLower() == code.ToLower();
                }
                if (!validatecode)
                {
                    return Json(new LoginReturnResult() { Success = false, Message = "验证码错误！", IsShowCode = true });
                }
            }
            //获取用户信息
            string tempUserName = GetUserName(username);
            UserInfo userInfo = UserOperate.GetUserInfo(tempUserName, userpwd, out errMsg);
            if (!string.IsNullOrEmpty(errMsg))
            {
                var isShowCode = false;
                _Session[LOGINERROR] = _Session[LOGINERROR] == null ? 0 : _Session[LOGINERROR].ObjToInt() + 1;
                if (!isNoCode && _Session[LOGINERROR].ObjToInt() >= 2)
                    isShowCode = true;
                return Json(new LoginReturnResult() { Success = false, Message = errMsg, IsShowCode = isShowCode });
            }
            CacheUserData(userInfo); //缓存cookie
            //执行登录成功后的操作
            CommonOperate.ExecuteUserOperateHandleMethod("AfterLoginSuccess", new object[] { _Session, _Request, _Response, username, userpwd, UserInfo.ACCOUNT_EXPIRATION_TIME });

            return Json(new LoginReturnResult() { Success = true, Message = string.Empty, Url = HttpUtility.UrlEncode(string.Empty) });
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        /// <param name="username">用户名或工号或邮箱或手机号</param>
        /// <returns></returns>
        private string GetUserName(string username)
        {
            string tempUserName = username.Trim();
            string errMsg = string.Empty;
            if (GlobalSet.IsAllowOtherConfigRuleLogin) //允许其他方式登录
            {
                //先检测默认登录规则账号是否存在
                bool rs = UserOperate.UserIsValid(tempUserName, out errMsg);
                if (rs)
                    return tempUserName;
                //默认登录规则账号不存在时检测其他方式
                OrgM_Emp emp = null;
                switch (GlobalSet.EmpUserNameConfigRule)
                {
                    case UserNameAndEmpConfigRule.EmpCode:
                        {
                            emp = OrgMOperate.GetEmpByMobile(tempUserName); //根据手机号获取员工
                            if (emp == null)
                            {
                                emp = OrgMOperate.GetEmpByEmail(tempUserName); //根据邮箱获取员工
                                if (emp == null)
                                    emp = OrgMOperate.GetEmpByEmailPrex(tempUserName); //根据邮箱前缀获取员工
                            }
                        }
                        break;
                    case UserNameAndEmpConfigRule.Mobile:
                        {
                            emp = OrgMOperate.GetEmpByCode(tempUserName); //根据工号获取员工
                            if (emp == null)
                            {
                                emp = OrgMOperate.GetEmpByEmail(tempUserName); //根据邮箱获取员工
                                if (emp == null)
                                    emp = OrgMOperate.GetEmpByEmailPrex(tempUserName); //根据邮箱前缀获取员工
                            }
                        }
                        break;
                    case UserNameAndEmpConfigRule.Email:
                        {
                            emp = OrgMOperate.GetEmpByCode(tempUserName); //根据工号获取员工
                            if (emp == null)
                            {
                                emp = OrgMOperate.GetEmpByMobile(tempUserName); //根据手机号获取员工
                                if (emp == null)
                                    emp = OrgMOperate.GetEmpByEmailPrex(tempUserName); //根据邮箱前缀获取员工
                            }
                        }
                        break;
                    case UserNameAndEmpConfigRule.EmailPre:
                        {
                            emp = OrgMOperate.GetEmpByCode(tempUserName); //根据工号获取员工
                            if (emp == null)
                            {
                                emp = OrgMOperate.GetEmpByMobile(tempUserName); //根据手机号获取员工
                                if (emp == null)
                                    emp = OrgMOperate.GetEmpByEmail(tempUserName); //根据邮箱获取员工
                            }
                        }
                        break;
                }
                if (emp != null)
                    return OrgMOperate.GetUserNameByEmp(emp);
            }
            return tempUserName;
        }

        /// <summary>
        /// 切换用户
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangeUser()
        {
            if (_Request == null) _Request = Request;
            if (_Response == null) _Response = Response;
            if (_Session == null) _Session = Session;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
                return Json(new ReturnResult() { Success = false, Message = "非法操作" });
            string username = _Request["username"].ObjToStr();
            if (username == "admin")
                return Json(new ReturnResult() { Success = false, Message = "没有权限" });
            Guid userId = UserOperate.GetUserIdByUserName(username);
            UserInfo userInfo = UserOperate.GetUserInfo(userId);
            if (userInfo == null)
                return Json(new ReturnResult() { Success = false, Message = "用户不存在" });
            userInfo.ClientBrowserWidth = currUser.ClientBrowserWidth;
            userInfo.ClientBrowserHeight = currUser.ClientBrowserHeight;
            CacheUserData(userInfo); //缓存cookie
            return Json(new ReturnResult() { Success = true, Message = string.Empty });
        }

        /// <summary>
        /// 缓存用户Cookie数据
        /// </summary>
        /// <param name="userInfo">用户信息</param>
        /// <returns></returns>
        private void CacheUserData(UserInfo userInfo)
        {
            _Session[LOGINERROR] = null;
            //客户端浏览器参数
            int w = _Request["w"].ObjToInt();
            int h = _Request["h"].ObjToInt();
            if (w > 0)
                userInfo.ClientBrowserWidth = w;
            if (h > 0)
                userInfo.ClientBrowserHeight = h;
            //获取客户端IP
            userInfo.ClientIP = WebHelper.GetClientIP(_Request);
            //缓存用户扩展信息
            UserInfo.CacheUserExtendInfo(userInfo.UserName, userInfo.ExtendUserObject);
            //用户票据保存
            FormsPrincipal.Login(userInfo.UserName, userInfo, UserInfo.ACCOUNT_EXPIRATION_TIME, GetHttpContext(_Request));
            //登录成功写cookie，保存客户端用户名
            var userNameCookie = new HttpCookie("UserName", userInfo.UserName);
            userNameCookie.Expires = DateTime.Now.AddDays(365);
            _Response.Cookies.Add(userNameCookie);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (_Response == null) _Response = Response;
            if (_Session == null) _Session = Session;
            FormsPrincipal.Logout(_Response, _Session);
            return RedirectToAction("Login");
        }

        /// <summary>
        /// 获取数据权限组织树
        /// </summary>
        /// <returns></returns>
        public ActionResult GetDataPermissionOrgTree()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            bool isAsync = _Request["async"].ObjToInt() == 1; //是否异步
            Guid? tempId = _Request["id"].ObjToGuidNull();
            TreeNode tempNode = new TreeNode() { id = "-1", text = "全部", iconCls = "eu-icon-dept" };
            TreeNode currDeptNode = new TreeNode() { id = "0", text = "本部门", iconCls = "eu-icon-dept" };
            Expression<Func<Sys_Organization, bool>> expression = null;
            string q = _Request["q"].ObjToStr(); //查询字符
            if (!string.IsNullOrEmpty(q))
                expression = x => x.Name.Contains(q);
            TreeNode node = CommonOperate.GetTreeNode<Sys_Organization>(tempId, null, null, null, null, "eu-icon-dept", expression, null, null, isAsync, GetCurrentUser(_Request));
            if (isAsync)
            {
                node.state = "closed";
                if (node.children != null && node.children.Count() > 0)
                {
                    node.children.ForEach(x => { x.state = "closed"; });
                }
            }
            if (tempId == null || tempId == Guid.Empty)
            {
                List<TreeNode> list = new List<TreeNode> { tempNode, currDeptNode };
                if (node != null) list.Add(node);
                return Json(list.ToJson().Content);
            }
            else
            {
                return Json(node.children.ToJson().Content);
            }
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePwd()
        {
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            UserInfo currUser = GetCurrentUser(_Request);
            if (currUser == null)
            {
                return Json(new ReturnResult() { Success = false, Message = "您未登录系统或登录时间过长，请重新登录系统后再修改密码！" });
            }
            string errMsg = string.Empty;
            string oldPwd = _Request["oldPwd"].ObjToStr();
            string newPwd = _Request["newPwd"].ObjToStr();
            UserInfo tempUserInfo = UserOperate.GetUserInfo(currUser.UserName, oldPwd, out errMsg);
            if (tempUserInfo == null)
            {
                return Json(new ReturnResult() { Success = false, Message = "您当前登录密码输入不正确，请重新输入！" });
            }
            bool rs = UserOperate.ModifyPassword(currUser.UserId, newPwd, out errMsg);
            if (rs)
            {
                CommonOperate.ExecuteUserOperateHandleMethod("AfterChangePwd", new object[] { currUser.UserName, oldPwd, newPwd });
            }
            return Json(new ReturnResult() { Success = rs, Message = errMsg });
        }

        #endregion
    }

    /// <summary>
    /// 用户API控制器
    /// </summary>
    public class UserApiController : BaseApiController
    {
        #region 公共方法

        /// <summary>
        /// 获取数据权限组织树
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public dynamic GetDataPermissionOrgTree()
        {
            HttpRequestBase request = WebHelper.GetContextRequest(Request);
            JsonResult result = new UserController(request).GetDataPermissionOrgTree() as JsonResult;
            return result.Data;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        [System.Web.Http.HttpPost]
        public ReturnResult ChangePwd()
        {
            HttpRequestBase request = WebHelper.GetContextRequest(Request);
            JsonResult result = new UserController(request).ChangePwd() as JsonResult;
            return result.Data as ReturnResult;
        }

        #endregion
    }
}
