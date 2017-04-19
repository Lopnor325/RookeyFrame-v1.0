/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Rookey.Frame.Operate.Base;
using Rookey.Frame.Operate.Base.OperateHandle;
using Rookey.Frame.Operate.Base.TempModel;
using Rookey.Frame.Model.Sys;
using Rookey.Frame.Base;
using System.Threading.Tasks;
using Rookey.Frame.Controllers.Other;

namespace Rookey.Frame.Controllers
{
    /// <summary>
    /// 通用附件控制器（异步）
    /// </summary>
    public class AnnexAsyncController : AsyncBaseController
    {
        #region UEditor上传

        /// <summary>
        /// 异步UEditor上传图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> ImageUpAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).ImageUp();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        ///  异步UEditor上传图片
        /// </summary>
        /// <param name="upfile">文件</param>
        /// <param name="pictitle">图片标题</param>
        /// <param name="dir">路径</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> ImageUpAsync(HttpPostedFileBase upfile, string pictitle, string dir)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).ImageUp(upfile, pictitle, dir);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        ///  异步UEditor上传文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Task<ActionResult> FileUpAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).FileUp();
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        ///  异步UEditor上传文件
        /// </summary>
        /// <param name="upfile">文件</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> FileUpAsync(HttpPostedFileBase upfile)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).FileUp(upfile);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        ///  异步UEditor获取video
        /// </summary>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="videoType">视频类型</param>
        /// <returns></returns>
        public Task<ActionResult> GetMovieAsync(string searchKey, string videoType)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).GetMovie(searchKey, videoType);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步图片在线管理
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> ImageManagerAsync(string action)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).ImageManager(action);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步涂鸦
        /// </summary>
        /// <param name="upfile">图片文件</param>
        /// <param name="content">涂鸦内容</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> ScrawlUpAsync(HttpPostedFileBase upfile, string content)
        {
            return Task.Factory.StartNew(() =>
             {
                 return new AnnexController(Request).ScrawlUp(upfile, content);
             }).ContinueWith<ActionResult>(task =>
             {
                 return task.Result;
             });
        }
        #endregion

        #region 表单或文档附件处理

        /// <summary>
        /// 异步上传附件，兼容非表单附件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="moduleId">模块Id,针对表单附件</param>
        /// <param name="id">记录Id,针对表单附件</param>
        /// <param name="isCreateSwf">是否创建SWF文件</param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UploadAttachmentAsync(HttpPostedFileBase[] file, Guid? moduleId, Guid? id, bool isCreateSwf = false)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).UploadAttachment(file, moduleId, id, isCreateSwf);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步保存表单附件
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="fileMsg">文件信息</param>
        /// <param name="isAdd">是否只是添加</param>
        /// <returns></returns>
        public Task<ActionResult> SaveFormAttachAsync(Guid moduleId, Guid id, string fileMsg, bool isAdd = false)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).SaveFormAttach(moduleId, id, fileMsg, isAdd);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步删除附件
        /// </summary>
        /// <param name="attachIds">附件Id集合，多个以逗号分隔</param>
        /// <returns></returns>
        public Task<ActionResult> DeleteAttachmentAsync(string attachIds)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).DeleteAttachment(attachIds);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步下载附件
        /// </summary>
        /// <param name="attachId">附件Id</param>
        /// <returns></returns>
        public Task<ActionResult> DownloadAttachmentAsync(Guid attachId)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).DownloadAttachment(attachId);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Task<ActionResult> DownloadFileAsync(string fileName)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).DownloadFile(fileName);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 图片控件临时上传

        /// <summary>
        /// 异步上传临时图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UploadTempImageAsync(HttpPostedFileBase file)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).UploadTempImage(file);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion

        #region 数据导入临时文件上传

        /// <summary>
        /// 异步上传临时导入模板文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ActionResult> UploadTempImportFileAsync(HttpPostedFileBase file)
        {
            return Task.Factory.StartNew(() =>
            {
                return new AnnexController(Request).UploadTempImportFile(file);
            }).ContinueWith<ActionResult>(task =>
            {
                return task.Result;
            });
        }

        #endregion
    }

    /// <summary>
    /// 通用附件控制器
    /// </summary>
    public class AnnexController : BaseController
    {
        #region 构造函数

        private HttpRequestBase _Request = null; //请求对象

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public AnnexController()
        {
            _Request = Request;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="request">请求对象</param>
        public AnnexController(HttpRequestBase request)
            : base(request)
        {
            _Request = request;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 获取文件路径,如果路径不存在就创建
        /// </summary>
        /// <param name="pdfSavePath"></param>
        /// <param name="swfSavePath"></param>
        private void GetFilePath(out string pdfSavePath, out string swfSavePath)
        {
            //yyyyMM
            string dateFolder = DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo);

            //pdf保存文件夹路径
            string pdfTempFolder = "~/Upload/PdfFile/";
            //pdf GUID
            string pdfGid = Guid.NewGuid().ToString("N");
            //生成pdf文件名
            string pdfName = pdfGid + ".pdf";
            //生成pdf相对路径
            string pdfTempPath = pdfTempFolder + pdfName;

            pdfSavePath = pdfTempPath;

            //swf GUID
            string swfGuid = Guid.NewGuid().ToString("N");
            //swf保存文件夹路径
            string swfFolder = "~/Upload/SwfFile/" + dateFolder + "/";
            //文件名
            string swfName = swfGuid + ".swf";
            //swf保存相对路径
            string swfPath = swfFolder + swfName;

            swfSavePath = swfPath;

            //保存路径
            if (!Directory.Exists(_Request.RequestContext.HttpContext.Server.MapPath(pdfTempFolder)))
            {
                string tempPath = _Request.RequestContext.HttpContext.Server.MapPath(pdfTempFolder);
                Directory.CreateDirectory(tempPath);
            }
            if (!Directory.Exists(_Request.RequestContext.HttpContext.Server.MapPath(swfFolder)))
            {
                string strpath = _Request.RequestContext.HttpContext.Server.MapPath(swfFolder);
                Directory.CreateDirectory(strpath);
            }
        }

        /// <summary>
        /// 生成SWF文件
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="isAsync">是否异步方式</param>
        /// <returns></returns>
        private bool CreateSwfFile(object obj, bool isAsync = true)
        {
            Func<bool> action = () =>
            {
                bool flag = false;
                try
                {
                    string[] aa = obj as string[];
                    string ext = aa[0];
                    string sourcePath = aa[1];
                    string pdfSavePath = aa[2];
                    string swfSavePath = aa[3];
                    string exePath = aa[4];
                    string binPath = aa[5];
                    if (ext.Equals(".doc") || ext.Equals(".docx"))
                    {
                        if (OfficeToPdfHelper.Doc2Pdf(sourcePath, pdfSavePath))
                        {
                            flag = SwfToolHelper.PDF2SWF(pdfSavePath, swfSavePath, exePath, binPath);
                        }
                    }
                    else if (ext.Equals(".xls") || ext.Equals(".xlsx"))
                    {
                        if (OfficeToPdfHelper.Xls2Pdf(sourcePath, pdfSavePath))
                        {
                            flag = SwfToolHelper.PDF2SWF(pdfSavePath, swfSavePath, exePath, binPath);
                        }
                    }
                    else if (ext.Equals(".ppt"))
                    {
                        if (OfficeToPdfHelper.PPt2Pdf(sourcePath, pdfSavePath))
                        {
                            flag = SwfToolHelper.PDF2SWF(pdfSavePath, swfSavePath, exePath, binPath);
                        }
                    }
                    else if (ext.Equals(".pdf"))
                    {
                        flag = SwfToolHelper.PDF2SWF(sourcePath, swfSavePath, exePath, binPath);
                    }
                }
                catch { }
                return flag;
            };
            if (isAsync) //异步方式
            {
                Task.Factory.StartNew(() =>
                {
                    action();
                });
                return true;
            }
            else //同步方式
            {
                return action();
            }
        }
        #endregion

        #region UEditor上传

        /// <summary>
        /// UEditor上传图片
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ImageUp()
        {
            if (_Request == null) _Request = Request;
            string result = string.Format(String.Format("updateSavePath([{0}]);", "'Image'"));
            return Content(result);
        }

        /// <summary>
        ///  UEditor上传图片
        /// </summary>
        /// <param name="upfile">文件</param>
        /// <param name="pictitle">图片标题</param>
        /// <param name="dir">路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImageUp(HttpPostedFileBase upfile, string pictitle, string dir)
        {
            if (_Request == null) _Request = Request;
            string path = string.Empty;
            string fileName = upfile.FileName;
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            int s = fileName.LastIndexOf(pathFlag);
            if (s >= 0)
            {
                fileName = fileName.Substring(s + 1);
            }
            try
            {
                UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
                path = UploadFile(upfile, "UEImage", dir);
            }
            catch (Exception ex)
            {
                return Json(new { state = ex.Message });
            }
            return Json(new { state = "SUCCESS", url = path, title = pictitle, original = fileName });
        }

        /// <summary>
        ///  UEditor上传文件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FileUp()
        {
            if (_Request == null) _Request = Request;
            string result = string.Format(String.Format("updateSavePath([{0}]);", "'File'"));
            return Content(result);
        }

        /// <summary>
        ///  UEditor上传文件
        /// </summary>
        /// <param name="upfile">文件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult FileUp(HttpPostedFileBase upfile)
        {
            if (_Request == null) _Request = Request;
            string path = string.Empty;
            string fileName = upfile.FileName;
            string pathFlag = "\\";
            if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                pathFlag = "/";
            int s = fileName.LastIndexOf(pathFlag);
            if (s >= 0)
            {
                fileName = fileName.Substring(s + 1);
            }
            string fileType = Path.GetExtension(upfile.FileName);
            try
            {
                UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
                path = UploadFile(upfile, "UEFile", "File");
            }
            catch (Exception ex)
            {
                return Json(new { state = ex.Message });
            }
            return Json(new { state = "SUCCESS", url = path, fileType = fileType, original = fileName });
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="upfile">上传的文件</param>
        /// <param name="configName">配置名称</param>
        /// <param name="dir">保存的文件夹</param>
        /// <returns>保存路径</returns>
        private string UploadFile(HttpPostedFileBase upfile, string configName, string dir)
        {
            if (_Request == null) _Request = Request;
            string filePath = string.IsNullOrWhiteSpace(dir) ? "Other" : dir;
            string path = UploadFileManager.SaveAs(upfile, configName, filePath);
            return path;
        }

        /// <summary>
        ///  UEditor获取video
        /// </summary>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="videoType">视频类型</param>
        /// <returns></returns>
        public ActionResult GetMovie(string searchKey, string videoType)
        {
            if (_Request == null) _Request = Request;
            Uri httpURL = new Uri("http://api.tudou.com/v3/gw?method=item.search&appKey=myKey&format=json&kw=" + searchKey + "&pageNo=1&pageSize=20&channelId=" + videoType + "&inDays=7&media=v&sort=s");
            System.Net.WebClient MyWebClient = new System.Net.WebClient();

            MyWebClient.Credentials = System.Net.CredentialCache.DefaultCredentials;           //获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(httpURL);

            return Content(Encoding.UTF8.GetString(pageData));
        }

        /// <summary>
        /// 图片在线管理
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ImageManager(string action)
        {
            string[] paths = { "Image", "Other" }; //需要遍历的目录列表，最好使用缩略图地址，否则当网速慢时可能会造成严重的延时
            string[] filetype = { ".gif", ".png", ".jpg", ".jpeg", ".bmp" };
            String str = String.Empty;
            if (_Request == null) _Request = Request;
            if (action == "get")
            {
                UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
                foreach (string path in paths)
                {
                    var cfg = UploadFileManager.GetUploadConfig("UEImage");
                    string basePath = _Request.RequestContext.HttpContext.Server.MapPath("~/") + cfg.Folder + "/" + path;
                    if (WebConfigHelper.GetAppSettingValue("IsLinux") != "true")
                        basePath = basePath.Replace("/", "\\");
                    DirectoryInfo info = new DirectoryInfo(basePath);
                    //目录验证
                    if (info.Exists)
                    {
                        DirectoryInfo[] infoArr = info.GetDirectories();
                        foreach (DirectoryInfo tmpInfo in infoArr)
                        {
                            foreach (FileInfo fi in tmpInfo.GetFiles())
                            {
                                if (Array.IndexOf(filetype, fi.Extension) != -1)
                                {
                                    str += cfg.Folder + "/" + path + "/" + tmpInfo.Name + "/" + fi.Name + "ue_separate_ue";
                                }
                            }
                        }
                    }
                }
            }
            return Content(str);
        }

        /// <summary>
        /// 涂鸦
        /// </summary>
        /// <param name="upfile">图片文件</param>
        /// <param name="content">涂鸦内容</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ScrawlUp(HttpPostedFileBase upfile, string content)
        {
            if (_Request == null) _Request = Request;
            UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
            var cfg = UploadFileManager.GetUploadConfig("UEImage");
            if (upfile != null)
            {
                string path = string.Empty;
                string state = "SUCCESS";
                //上传图片
                try
                {
                    path = UploadFile(upfile, "UEImage", "Temp");
                }
                catch (Exception ex)
                {
                    return Json(new { state = ex.Message });
                }
                return Content("<script>parent.ue_callback('" + path + "','" + state + "')</script>");//回调函数
            }
            else
            {
                //上传图片
                string url = string.Empty;
                string state = "SUCCESS";
                FileStream fs = null;
                string pathFlag = "\\";
                if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                    pathFlag = "/";
                try
                {
                    string dir = _Request.RequestContext.HttpContext.Server.MapPath("~/") + cfg.Folder + "/Other";
                    string path = DateTime.Now.ToString("yyyyMM");
                    dir = dir + "/" + path;
                    if (pathFlag == "\\")
                        dir = dir.Replace("/", "\\");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    string filename = System.Guid.NewGuid() + ".png";
                    fs = System.IO.File.Create(dir + pathFlag + filename);
                    byte[] bytes = Convert.FromBase64String(content);
                    fs.Write(bytes, 0, bytes.Length);
                    url = cfg.Folder + "/Other/" + path + "/" + filename;
                }
                catch (Exception e)
                {
                    state = "未知错误:" + e.Message;
                    url = "";
                }
                finally
                {
                    fs.Close();
                    string tempDir = _Request.RequestContext.HttpContext.Server.MapPath("~/") + cfg.Folder + "/Temp";
                    if (pathFlag == "\\")
                        tempDir = tempDir.Replace("/", "\\");
                    DirectoryInfo info = new DirectoryInfo(tempDir);
                    if (info.Exists)
                    {
                        DirectoryInfo[] infoArr = info.GetDirectories();
                        foreach (var item in infoArr)
                        {
                            string str = tempDir + pathFlag + item.Name;
                            Directory.Delete(str, true);
                        }
                    }
                }
                return Json(new { url = url, state = state });
            }
        }
        #endregion

        #region 表单或文档附件处理

        /// <summary>
        /// 上传附件，兼容非表单附件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="moduleId">模块Id,针对表单附件</param>
        /// <param name="id">记录Id,针对表单附件</param>
        /// <param name="isCreateSwf">是否创建SWF文件</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadAttachment(HttpPostedFileBase[] file, Guid? moduleId, Guid? id, bool isCreateSwf = false)
        {
            if (file == null || file.Where(o => o == null).Count() > 0)
            {
                return Json(new ReturnResult { Success = false, Message = "请选择上传文件！" });
            }
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string attachType = _Request["attachType"].ObjToStr();
            UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
            string message = string.Empty;
            StringBuilder msg = new StringBuilder();
            List<AttachFileInfo> fileMsg = new List<AttachFileInfo>();
            foreach (var item in file)
            {
                try
                {
                    string fileSize = FileOperateHelper.FileSize(item.ContentLength);
                    string fileType = Path.GetExtension(item.FileName);
                    string fileName = item.FileName;
                    string pathFlag = "\\";
                    if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                        pathFlag = "/";
                    int s = fileName.LastIndexOf(pathFlag);
                    if (s >= 0)
                    {
                        fileName = fileName.Substring(s + 1);
                    }
                    //保存文件
                    string filePath = string.Empty;
                    if (moduleId.HasValue) //表单附件
                    {
                        filePath = UploadFileManager.SaveAs(item, "Attachment", "Temp");
                    }
                    else
                    {
                        filePath = UploadFileManager.SaveAs(item, string.Empty);
                    }
                    filePath = filePath.StartsWith("~/") ? filePath : filePath.StartsWith("/") ? "~" + filePath : "~/" + filePath;
                    //swf保存路径
                    string swfPath = string.Empty;
                    //pdf保存路径
                    string pdfPath = string.Empty;
                    if (isCreateSwf && !moduleId.HasValue)
                    {
                        //exe路径
                        string exePath = _Request.RequestContext.HttpContext.Server.MapPath("~/bin/SWFTools/pdf2swf.exe");
                        //bin路径
                        string binPath = _Request.RequestContext.HttpContext.Server.MapPath("~/bin/");
                        if (fileType.Equals(".doc") || fileType.Equals(".docx") ||
                            fileType.Equals(".xls") || fileType.Equals(".xlsx") ||
                            fileType.Equals(".ppt") || fileType.Equals(".pptx") ||
                            fileType.Equals(".pdf"))
                        {
                            //取pdf和swf路径
                            GetFilePath(out pdfPath, out swfPath);
                            //参数
                            string[] obj = new string[] { fileType, _Request.RequestContext.HttpContext.Server.MapPath(filePath), _Request.RequestContext.HttpContext.Server.MapPath(pdfPath), _Request.RequestContext.HttpContext.Server.MapPath(swfPath), exePath, binPath };
                            CreateSwfFile(obj);
                        }
                    }
                    fileMsg.Add(new AttachFileInfo() { AttachFile = filePath, PdfFile = pdfPath, SwfFile = swfPath, FileName = fileName, FileType = fileType, FileSize = fileSize, AttachType = attachType });
                }
                catch (Exception ex)
                {
                    msg.AppendLine(item.FileName + "上传失败：" + ex.Message);
                    break;
                }
            }
            if (moduleId.HasValue && id.HasValue) //查看页面，直接保存附件
            {
                return SaveFormAttach(moduleId.Value, id.Value, JsonHelper.Serialize(fileMsg), true);
            }
            return Json(new
            {
                Success = string.IsNullOrEmpty(msg.ToString()),
                Message = string.IsNullOrEmpty(msg.ToString()) ? "上传成功" : msg.ToString(),
                FileMsg = fileMsg.Count > 0 ? JsonHelper.Serialize(fileMsg) : string.Empty
            }, "text/plain");
        }

        /// <summary>
        /// 保存表单附件
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <param name="id">记录Id</param>
        /// <param name="fileMsg">文件信息</param>
        /// <param name="isAdd">是否只是添加</param>
        /// <returns></returns>
        public ActionResult SaveFormAttach(Guid moduleId, Guid id, string fileMsg, bool isAdd = false)
        {
            if (string.IsNullOrEmpty(fileMsg))
            {
                return Json(new ReturnResult { Success = true, Message = string.Empty });
            }
            if (_Request == null) _Request = Request;
            SetRequest(_Request);
            string errMsg = string.Empty;
            List<AttachFileInfo> addAttachs = null;
            try
            {
                string pathFlag = "\\";
                if (WebConfigHelper.GetAppSettingValue("IsLinux") == "true")
                    pathFlag = "/";
                UserInfo currUser = GetCurrentUser(_Request);
                Guid? userId = currUser != null ? currUser.UserId : (Guid?)null;
                string userName = currUser != null ? currUser.UserName : string.Empty;
                Sys_Module module = SystemOperate.GetModuleById(moduleId);
                List<AttachFileInfo> attachInfo = JsonHelper.Deserialize<List<AttachFileInfo>>(HttpUtility.UrlDecode(fileMsg, Encoding.UTF8));
                #region 删除已经移除的附件
                if (!isAdd) //非新增状态
                {
                    List<Guid> existIds = new List<Guid>();
                    if (attachInfo != null && attachInfo.Count > 0)
                    {
                        existIds = attachInfo.Select(x => x.Id.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
                    }
                    //对已删除的附件进行处理
                    List<Sys_Attachment> tempAttachments = CommonOperate.GetEntities<Sys_Attachment>(out errMsg, x => x.Sys_ModuleId == moduleId && x.RecordId == id, null, false);
                    if (tempAttachments != null) tempAttachments = tempAttachments.Where(x => !existIds.Contains(x.Id)).ToList();
                    SystemOperate.DeleteAttachment(tempAttachments);
                }
                #endregion
                #region 添加附件
                if (attachInfo != null && attachInfo.Count > 0)
                {
                    addAttachs = new List<AttachFileInfo>();
                    //日期文件夹
                    string dateFolder = DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo);
                    //记录对应的titleKey值
                    string titleKeyValue = CommonOperate.GetModelTitleKeyValue(moduleId, id);
                    List<Sys_Attachment> list = new List<Sys_Attachment>();
                    foreach (AttachFileInfo info in attachInfo)
                    {
                        if (string.IsNullOrEmpty(info.AttachFile)) continue;
                        if (info.Id.ObjToGuid() != Guid.Empty) continue; //原来的附件
                        string oldAttchFile = _Request.RequestContext.HttpContext.Server.MapPath(info.AttachFile); //临时附件
                        string dir = string.Format("{0}Upload{3}Attachment{3}{1}{3}{2}", Globals.GetWebDir(), module.TableName, dateFolder, pathFlag);
                        if (!Directory.Exists(dir)) //目录不存在则创建
                            Directory.CreateDirectory(dir);
                        string newAttachFile = string.Format("{0}{4}{1}_{2}{3}", dir, Path.GetFileNameWithoutExtension(info.FileName), id, Path.GetExtension(info.FileName), pathFlag);
                        try
                        {
                            System.IO.File.Copy(oldAttchFile, newAttachFile, true); //复制文件
                        }
                        catch (Exception ex)
                        {
                            return Json(new ReturnResult { Success = false, Message = ex.Message }, "text/plain");
                        }
                        //文件复制完成后删除临时文件
                        try
                        {
                            System.IO.File.Delete(oldAttchFile);
                        }
                        catch { }
                        string newPdfFile = string.Empty; //pdf文件
                        string newSwfFile = string.Empty; //swf文件
                        //可以转换成swf的进行转换
                        if (info.FileType.Equals(".doc") || info.FileType.Equals(".docx") ||
                                info.FileType.Equals(".xls") || info.FileType.Equals(".xlsx") ||
                                info.FileType.Equals(".ppt") || info.FileType.Equals(".pptx") ||
                                info.FileType.Equals(".pdf"))
                        {
                            newPdfFile = string.Format("{0}{2}{1}.pdf", dir, Path.GetFileNameWithoutExtension(newAttachFile), pathFlag);
                            newSwfFile = string.Format("{0}{2}{1}.swf", dir, Path.GetFileNameWithoutExtension(newAttachFile), pathFlag);
                            string exePath = _Request.RequestContext.HttpContext.Server.MapPath("~/bin/SWFTools/pdf2swf.exe");
                            string binPath = _Request.RequestContext.HttpContext.Server.MapPath("~/bin/");
                            string[] obj = new string[] { info.FileType, newAttachFile, newPdfFile, newSwfFile, exePath, binPath };
                            CreateSwfFile(obj);
                        }
                        //构造文件URL，保存为相对URL地址
                        string fileUrl = string.Format("Upload/Attachment/{0}/{1}/{2}", module.TableName, dateFolder, Path.GetFileName(newAttachFile));
                        string pdfUrl = string.IsNullOrEmpty(newPdfFile) ? string.Empty : newPdfFile.Replace(Globals.GetWebDir(), string.Empty).Replace("\\", "/");
                        string swfUrl = string.IsNullOrEmpty(newSwfFile) ? string.Empty : newSwfFile.Replace(Globals.GetWebDir(), string.Empty).Replace("\\", "/");
                        Guid attachiId = Guid.NewGuid();
                        info.Id = attachiId.ToString();
                        list.Add(new Sys_Attachment()
                        {
                            Id = attachiId,
                            Sys_ModuleId = moduleId,
                            Sys_ModuleName = module.Name,
                            RecordId = id,
                            RecordTitleKeyValue = titleKeyValue,
                            FileName = info.FileName,
                            FileType = info.FileType,
                            FileSize = info.FileSize,
                            FileUrl = fileUrl,
                            PdfUrl = pdfUrl,
                            SwfUrl = swfUrl,
                            AttachType = info.AttachType,
                            CreateDate = DateTime.Now,
                            CreateUserId = userId,
                            CreateUserName = userName,
                            ModifyDate = DateTime.Now,
                            ModifyUserId = userId,
                            ModifyUserName = userName
                        });
                        string tempUrl = "/" + fileUrl;
                        if (!string.IsNullOrEmpty(swfUrl))
                        {
                            tempUrl = string.Format("/Page/DocView.html?fn={0}&swfUrl={1}", HttpUtility.UrlEncode(info.FileName).Replace("+", "%20"), HttpUtility.UrlEncode(swfUrl).Replace("+", "%20"));
                        }
                        info.AttachFile = tempUrl;
                        info.PdfFile = pdfUrl;
                        info.SwfFile = swfUrl;
                        addAttachs.Add(info);
                    }
                    if (list.Count > 0)
                    {
                        Guid attachModuleId = SystemOperate.GetModuleIdByName("附件信息");
                        bool rs = CommonOperate.OperateRecords(attachModuleId, list, ModelRecordOperateType.Add, out errMsg, false);
                        if (!rs)
                            addAttachs = null;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return Json(new { Success = string.IsNullOrEmpty(errMsg), Message = errMsg, AddAttachs = addAttachs }, "text/plain");
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="attachIds">附件Id集合，多个以逗号分隔</param>
        /// <returns></returns>
        public ActionResult DeleteAttachment(string attachIds)
        {
            if (string.IsNullOrEmpty(attachIds))
            {
                return Json(new ReturnResult { Success = false, Message = "附件Id为空！" });
            }
            if (_Request == null) _Request = Request;
            string[] token = attachIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            if (token.Length == 0)
            {
                return Json(new ReturnResult { Success = false, Message = "附件Id为空！" });
            }
            List<Guid> delIds = token.Select(x => x.ObjToGuid()).Where(x => x != Guid.Empty).ToList();
            if (delIds == null || delIds.Count == 0)
            {
                return Json(new ReturnResult { Success = false, Message = "附件Id为空！" });
            }
            string errMsg = string.Empty;
            List<Sys_Attachment> tempAttachments = CommonOperate.GetEntities<Sys_Attachment>(out errMsg, x => delIds.Contains(x.Id), null, false);
            errMsg = SystemOperate.DeleteAttachment(tempAttachments);
            return Json(new ReturnResult { Success = string.IsNullOrEmpty(errMsg), Message = errMsg });
        }

        /// <summary>
        /// 下载附件
        /// </summary>
        /// <param name="attachId">附件Id</param>
        /// <returns></returns>
        public ActionResult DownloadAttachment(Guid attachId)
        {
            if (_Request == null) _Request = Request;
            string errMsg = string.Empty;
            Sys_Attachment attachment = CommonOperate.GetEntityById<Sys_Attachment>(attachId, out errMsg);
            if (attachment != null)
            {
                try
                {
                    string tempFile = string.Format("{0}{1}", Globals.GetWebDir(), attachment.FileUrl.ObjToStr().Replace(Globals.GetBaseUrl(), string.Empty));
                    if (WebConfigHelper.GetAppSettingValue("IsLinux") != "true")
                        tempFile = tempFile.Replace("/", "\\");
                    string ext = FileOperateHelper.GetFileExt(tempFile);
                    var fs = new System.IO.FileStream(tempFile, FileMode.Open);
                    if (fs != null)
                    {
                        return File(fs, FileOperateHelper.GetHttpMIMEContentType(ext), Url.Encode(attachment.FileName));
                    }
                }
                catch (Exception ex)
                {
                    return Content("<script>alert('异常：" + ex.Message + "');</script>");
                }
            }
            return Content("<script>alert('找不到此文件！');</script>");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string fileName)
        {
            if (_Request == null) _Request = Request;
            try
            {
                var fs = new System.IO.FileStream(HttpUtility.UrlDecode(fileName, Encoding.UTF8), FileMode.Open);
                if (fs != null)
                {
                    string ext = FileOperateHelper.GetFileExt(fileName);
                    return File(fs, FileOperateHelper.GetHttpMIMEContentType(ext), Url.Encode(Path.GetFileName(fileName)));
                }
                else
                {
                    return Content("<script>alert('找不到此文件！');</script>");
                }
            }
            catch (Exception ex)
            {
                return Content("<script>alert('异常：" + ex.Message + "');</script>");
            }
        }

        #endregion

        #region 图片控件临时上传

        /// <summary>
        /// 上传临时图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadTempImage(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Json(new ReturnResult { Success = false, Message = "请选择上传文件！" });
            }
            if (_Request == null) _Request = Request;
            string message = string.Empty;
            string imgName = _Request["imgName"].ObjToStr();
            try
            {
                string fileSize = FileOperateHelper.FileSize(file.ContentLength);
                string fileType = Path.GetExtension(file.FileName);
                //保存文件
                UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
                string filePath = UploadFileManager.SaveAs(file, string.Empty, "Temp", imgName);
                if (!string.IsNullOrEmpty(filePath) && filePath.Substring(0, 1) != "/")
                {
                    filePath = "/" + filePath;
                }
                else
                {
                    message = "临时图片保存失败！";
                }
                return Json(new { Success = string.IsNullOrEmpty(message), Message = message, FilePath = filePath }, "text/plain");
            }
            catch (Exception ex)
            {
                message = string.Format("图片上传失败，原因：{0}", ex.Message);
                return Json(new ReturnResult { Success = false, Message = message }, "text/plain");
            }
        }

        #endregion

        #region 数据导入临时文件上传

        /// <summary>
        /// 上传临时导入模板文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadTempImportFile(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Json(new ReturnResult { Success = false, Message = "请选择上传文件！" });
            }
            if (_Request == null) _Request = Request;
            string message = string.Empty;
            try
            {
                string fileSize = FileOperateHelper.FileSize(file.ContentLength);
                string fileType = Path.GetExtension(file.FileName);
                //保存文件
                UploadFileManager.httpContext = _Request.RequestContext.HttpContext;
                string filePath = UploadFileManager.SaveAs(file, string.Empty, "Temp");
                if (string.IsNullOrWhiteSpace(filePath)) message = "上传失败！";
                return Json(new { Success = string.IsNullOrEmpty(message), Message = message, FilePath = filePath }, "text/plain");
            }
            catch (Exception ex)
            {
                message = string.Format("上传失败，异常：{0}", ex.Message);
                return Json(new ReturnResult { Success = false, Message = message }, "text/plain");
            }
        }

        #endregion
    }
}
