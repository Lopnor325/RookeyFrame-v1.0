using Rookey.Frame.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Rookey.Frame.Controllers.FileManage
{
    /// <summary>
    /// WebExplorer
    /// </summary>
    public class WebExplorer : IHttpHandler
    {
        /// <summary>
        /// 接收请求
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Buffer = true;//互不影响
            context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(0);
            context.Response.Expires = 0;
            context.Response.AddHeader("Pragma", "No-Cache");
            UserInfo currUser = UserInfo.GetCurretnUser(context);
            bool rs = currUser != null && (currUser.UserName == "admin" || (currUser.ExtendUserObject != null && currUser.ExtendUserObject.RoleNames != null && currUser.ExtendUserObject.RoleNames.Contains("系统管理员")));
            if (rs) //用户已登录
            {
                string action = context.Request["action"];//获取操作类型
                switch (action)
                {
                    case "ISADMIN": context.Response.Write((currUser.UserName == "admin").ToString().ToLower()); break;//判断是否为admin
                    case "CHECKLOGIN": CheckLogin(context); break; //检查登录情况
                    case "LIST": ResponseList(context, currUser.UserName == "admin"); break;//获取文件列表
                    case "DOWNLOAD": DownFile(context); break;//下载文件
                    case "GETEDITFILE": GetEditFileContent(context); break;//从服务器读取文件内容
                    case "SAVEEDITFILE": SaveFile(context, false); break;//保存已经编辑的文件 
                    case "NEWDIR": CreateDirectory(context); break;//新建目录
                    case "NEWFILE": SaveFile(context, true); break;//新建文件
                    case "DELETE": Delete(context); break;//删除操作
                    case "COPY": CutCopy(context, "copy"); break;//复制操作
                    case "CUT": CutCopy(context, "cut"); break;//剪贴操作
                    case "UPLOAD": UpLoad(context); break;//上传操作
                    case "RENAME": ReName(context); break;//重名
                    case "ZIP": Zip(context); break;//压缩文件
                    case "UNZIP": UnZip(context); break;//解压缩
                    case "DOWNLOADS": DownLoads(context); break;//下载多个文件
                }
            }
            else
            {
                context.Response.Write("<script type='text/javascript'>window.location.href = '/User/Login.html';</script>");
            }
        }

        /// <summary>
        /// IsReusable
        /// </summary>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #region 具体的操作实现过程

        /// <summary>
        /// 检查用户是否登录
        /// </summary>
        /// <param name="context"></param>
        private void CheckLogin(HttpContext context)
        {
            UserInfo currUser = UserInfo.GetCurretnUser(context);
            bool rs = currUser != null && (currUser.UserName == "admin" || (currUser.ExtendUserObject != null && currUser.ExtendUserObject.RoleNames != null && currUser.ExtendUserObject.RoleNames.Contains("系统管理员")));
            context.Response.Write(rs);
        }

        /// <summary>
        /// 获取文件及文件夹路径列表
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isAdmin"></param>
        private void ResponseList(HttpContext context, bool isAdmin)
        {
            string value1 = context.Request["value1"];//获取参数    {key:value,}
            StringBuilder json = new StringBuilder("var GetList={\"Directory\":[", 500);
            string path = context.Server.MapPath(value1);//获取要列举的物理路径

            string[] dir = Directory.GetDirectories(path);//获取指定路径下面所有文件夹
            //if (!isAdmin)
            //{
            //    if (value1 == "~/")
            //        dir = dir.Where(x => x.EndsWith("\\Scripts")).ToArray();
            //    else if (value1 == "~/Scripts/")
            //        dir = dir.Where(x => x.EndsWith("\\Scripts\\model")).ToArray();
            //    else if (value1 == "~/Scripts/model/")
            //        dir = dir.Where(x => x.EndsWith("\\Scripts\\model\\TempModel")).ToArray();
            //}
            string[] files = Directory.GetFiles(path);//....
            foreach (string d in dir)
            {
                DirectoryInfo info = new DirectoryInfo(d);
                //{"Name":"Program","LastModify":"2012-08-08 12:12:49"}
                json.Append("{\"Name\":\"" + info.Name + "\",\"LastModify\":\"" + info.LastWriteTime + "\"},");
            }
            string tem = json.ToString();
            if (tem.EndsWith(","))//去掉最后一个尾巴 
            {
                tem = tem.Substring(0, tem.Length - 1);
            }
            json = new StringBuilder(tem);
            json.Append("],\"File\":[");//接着拼接文件 

            foreach (string f in files)
            {
                FileInfo info = new FileInfo(f);
                //if (!isAdmin && !info.Directory.FullName.EndsWith("\\Scripts\\model\\TempModel"))
                //    continue;
                string size = null;//换算单位 
                if (info.Length > 1024 * 1024)//M
                    size = ((double)info.Length / 1024 / 1024).ToString("F2") + "MB";
                else if (info.Length > 1024)
                    size = ((double)info.Length / 1024).ToString("F2") + "KB";
                else
                    size = info.Length.ToString() + "B";

                json.Append("{\"Name\":\"" + info.Name + "\",\"Size\":\"" + size + "\",\"LastModify\":\"" + info.LastWriteTime + "\"},");
            }
            tem = json.ToString();
            if (tem.EndsWith(","))//去掉最后一个尾巴 
            {
                tem = tem.Substring(0, tem.Length - 1);
            }
            json = new StringBuilder(tem);
            json.Append("]}");
            //输出JSON
            context.Response.Write(json.ToString());
        }

        /// <summary>
        /// 下载文件操作
        /// </summary>
        /// <param name="context"></param>
        private void DownFile(HttpContext context)
        {
            string value1 = context.Request["value1"];
            string[] files = value1.Split('|');
            foreach (string item in files)
            {
                string path = context.Server.MapPath(item);//获取绝对路径
                if (File.Exists(path))
                    DownloadFile.ResponseFile(path, context, false);
            }
        }

        /// <summary>
        /// 从服务器读取文件内容
        /// </summary>
        /// <param name="context"></param>
        private void GetEditFileContent(HttpContext context)
        {
            string path = context.Server.MapPath(context.Request["value1"]);
            context.Response.Write(File.ReadAllText(path, Encoding.UTF8));
        }

        /// <summary>
        /// 保存已经编辑的文件 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isNew">是否新建</param>
        private void SaveFile(HttpContext context, bool isNew)
        {
            //isNew-true:表示是新文件    false表示是修改操作
            string path = context.Server.MapPath(context.Request["value1"]);
            if (isNew & File.Exists(path))
                return;
            string content = context.Request["content"];
            StreamWriter sw = File.CreateText(path);
            sw.Write(content);
            sw.Close();
            context.Response.Write("OK");
        }

        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="context"></param>
        private void CreateDirectory(HttpContext context)
        {
            string path = context.Request["value1"];
            Directory.CreateDirectory(context.Server.MapPath(path));
            context.Response.Write("OK");
        }

        /// <summary>
        /// 删除文件/目录操作
        /// </summary>
        /// <param name="context"></param>
        private void Delete(HttpContext context)
        {
            UserInfo currUser = UserInfo.GetCurretnUser(context);
            if (currUser == null || currUser.UserName != "admin")
            {
                context.Response.Write("没有操作权限，请联系管理员");
                return;
            }
            string[] files = context.Request["value1"].Split('|');// 分割
            foreach (string item in files)
            {
                string path = context.Server.MapPath(item);
                if (File.Exists(path))
                    File.Delete(path);
                else if (Directory.Exists(path))
                    Directory.Delete(path, false);
            }
            context.Response.Write("OK");
        }

        /// <summary>
        /// 执行剪贴 复制 操作
        /// </summary>
        /// <param name="context"></param>
        /// <param name="flag"></param>
        private void CutCopy(HttpContext context, string flag)
        {
            if (flag == "cut")
            {
                UserInfo currUser = UserInfo.GetCurretnUser(context);
                if (currUser == null || currUser.UserName != "admin")
                {
                    context.Response.Write("没有操作权限，请联系管理员");
                    return;
                }
            }
            string path = context.Server.MapPath(context.Request["value1"]);//请求的路径
            string[] files = context.Request["value2"].Split('|');
            foreach (string item in files)
            {
                string p = context.Server.MapPath(item);
                string fileName = Path.GetFileName(p);//获取文件名
                if (File.Exists(p))//如果是文件 
                    if (flag == "cut")
                        File.Move(p, path + fileName);
                    else
                        File.Copy(p, path + fileName);
                else if (Directory.Exists(p))
                    if (flag == "cut")
                        Directory.Move(p, path + fileName);
                    else
                        Direactory(p, path + fileName);
            }
            context.Response.Write("OK");
        }

        /// <summary>
        /// 复制目录操作
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        private void Direactory(string source, string target)
        {
            if (!target.StartsWith(source, StringComparison.CurrentCultureIgnoreCase))
            {
                DirectoryInfo s = new DirectoryInfo(source);//源
                DirectoryInfo t = new DirectoryInfo(target);//目的地
                t.Create();// 目录创建完毕 
                //连同文件也复制过去
                FileInfo[] sFiles = s.GetFiles();
                for (int i = 0; i < sFiles.Length; i++)
                {
                    File.Copy(sFiles[i].FullName, t.FullName + "\\" + sFiles[i].Name, true);
                }
                //连同子目录也复制过去
                DirectoryInfo[] ds = t.GetDirectories();
                for (int i = 0; i < ds.Length; i++)
                {
                    Direactory(ds[i].FullName, t.FullName + "\\" + ds[i].Name);
                }
            }

        }

        /// <summary>
        /// 压缩文件 
        /// </summary>
        /// <param name="context"></param>
        private void Zip(HttpContext context)
        {
            string zipFile = context.Server.MapPath(context.Request["value1"]);
            string[] fd = context.Request["value2"].Split('|');
            List<string> files = new List<string>();
            List<string> dirs = new List<string>();
            //将要压缩的文件或者文件夹全部存储到集合中
            foreach (string item in fd)
            {
                string p = context.Server.MapPath(item);
                if (File.Exists(p))
                    files.Add(p);
                else if (Directory.Exists(p))
                    dirs.Add(p);
            }
            ZipClass.Zip(Path.GetDirectoryName(zipFile) + "\\", zipFile, "", true, files.ToArray(), dirs.ToArray());
            context.Response.Write("OK");
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="context"></param>
        private void UnZip(HttpContext context)
        {
            string unZipDir = context.Server.MapPath(context.Request["value1"]);
            string[] zipFiles = context.Request["value2"].Split('|');
            foreach (string item in zipFiles)
            {
                ZipClass.UnZip(context.Server.MapPath(item), unZipDir, "");
            }
            context.Response.Write("OK");
        }

        /// <summary>
        /// 上传 
        /// </summary>
        /// <param name="context"></param>
        private void UpLoad(HttpContext context)
        {
            string path = context.Server.MapPath(context.Request["value1"]);
            HttpFileCollection files = context.Request.Files;
            long allSize = 0;
            for (int i = 0; i < files.Count; i++)
            {
                allSize += files[i].ContentLength;
            }
            if (allSize > 20 * 1024 * 1024)
                context.Response.Write("文件大小超过限制");
            for (int i = 0; i < files.Count; i++)
            {
                files[i].SaveAs(path + Path.GetFileName(files[i].FileName));
            }
            context.Response.Write("OK");
        }

        /// <summary>
        /// 下载多个文件
        /// </summary>
        /// <param name="context"></param>
        private void DownLoads(HttpContext context)
        {
            string zipFile = context.Server.MapPath("#download.zip");
            string[] fd = context.Request["value1"].Split('|');
            List<string> files = new List<string>();
            List<string> dirs = new List<string>();
            foreach (string item in fd)
            {
                string p = context.Server.MapPath(item);
                if (File.Exists(p))
                    files.Add(p);
                else if (Directory.Exists(p))
                    dirs.Add(p);
            }
            ZipClass.Zip(Path.GetDirectoryName(zipFile) + "\\", zipFile, "", true, files.ToArray(), dirs.ToArray());
            DownloadFile.ResponseFile(zipFile, context, true);
        }

        /// <summary>
        /// 重命名操作
        /// </summary>
        /// <param name="context"></param>
        private void ReName(HttpContext context)
        {
            string oldName = context.Server.MapPath(context.Request["value1"]);
            string newName = context.Server.MapPath(context.Request["value2"]);
            File.Move(oldName, newName);
            context.Response.Write("OK");
        }

        #endregion
    }
}
