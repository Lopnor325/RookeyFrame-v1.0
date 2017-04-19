/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Rookey.Frame.Common
{
    public class UploadFileManager
    {
        public static HttpContextBase httpContext = new HttpContextWrapper(ApplicationObject.CurrentOneHttpContext);

        /// <summary>
        /// 将流转换为内存字节数组
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadBytes(Stream stream)
        {
            //是否允许查找流
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            if (stream.CanSeek) stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        public static string SaveThumbnail(string tempPath, string oldPath, string configName, int txt_width, int txt_height, int txt_top, int txt_left, int txt_DropWidth, int txt_DropHeight)
        {
            var stream = UploadFileManager.GetUploadFile(tempPath, configName);

            //检查临时文件是否存在
            if (stream != null)
            {
                var result = CutPhotoHelp.SaveCutPic(stream, 0, 0, txt_DropWidth, txt_DropHeight, txt_left, txt_top, txt_width, txt_height);


                stream.Close();
                stream.Dispose();

                string fileName = UploadFileManager.Save(result, oldPath, ".jpg", configName);

                UploadFileManager.RemoveUploadFile(tempPath, configName);

                return fileName;
            }
            throw new ApplicationException("上传临时文件已被清除，请重新上传！");
        }

        /// <summary>
        /// 读取上传文件
        /// </summary>
        /// <param name="url">上传文件地址</param>
        /// <param name="configName">上传配置名</param>
        /// <returns>文件流</returns>
        public static Stream GetUploadFile(string url, string configName)
        {
            string filePath = GetUploadFileDiskPath(url, configName);
            if (File.Exists(filePath))
                return new FileStream(filePath, FileMode.Open);
            else
                return null;
        }

        /// <summary>
        /// 删除上传文件
        /// </summary>
        /// <param name="url">上传文件地址</param>
        /// <param name="configName">上传配置名</param>
        public static void RemoveUploadFile(string url, string configName)
        {
            string filePath = GetUploadFileDiskPath(url, configName);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private static string GetUploadFileDiskPath(string url, string configName)
        {
            return GetUploadFileDiskPath(url, GetUploadConfig(configName));
        }

        private static string GetUploadFileDiskPath(string url, UploadConfig cfg)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;

            Uri uri = new Uri(url, UriKind.RelativeOrAbsolute);

            if (uri.IsAbsoluteUri)
            {
                url = uri.AbsolutePath.Substring(1);
            }

            var p = System.Web.VirtualPathUtility.IsAppRelative(url) ? System.Web.VirtualPathUtility.ToAbsolute(url) : url;
            var fullPath = string.Empty;
            if (System.IO.Path.IsPathRooted(p))
            {
                fullPath = httpContext.Server.MapPath(p);
            }
            else
            {
                string basePath = string.IsNullOrWhiteSpace(cfg.BasePath) ? ApplicationObject.CurrentOneHttpContext.Server.MapPath("~/") : cfg.BasePath;
                fullPath = System.IO.Path.Combine(basePath, url);
            }
            return fullPath;
        }

        public static string GetUploadFileUrlPath(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;

            url = System.Web.VirtualPathUtility.IsAppRelative(url) ? System.Web.VirtualPathUtility.ToAbsolute(url) : url;
            Uri u = null;
            if (System.IO.Path.IsPathRooted(url) || Uri.TryCreate(url, UriKind.Absolute, out u))
            {
                return url;
            }
            else
            {
                var imagePath = System.Configuration.ConfigurationManager.AppSettings["ImagePath"];
                imagePath = System.Web.VirtualPathUtility.IsAppRelative(imagePath) ? System.Web.VirtualPathUtility.ToAbsolute(imagePath) : imagePath;
                imagePath = System.Web.VirtualPathUtility.AppendTrailingSlash(imagePath);
                return System.IO.Path.Combine(imagePath, url);
            }
        }

        /// <summary>
        /// 创建一个指定类型带路径的文件名
        /// </summary>
        /// <param name="ext">文件类型</param>
        /// <param name="cfg">上传配置</param>
        /// <param name="customerFileName">自定义文件名</param>
        /// <returns></returns>
        private static string CreateFileName(string ext, UploadConfig cfg, string customerFileName = null)
        {
            string folder = System.Web.Mvc.UrlHelper.GenerateContentUrl(cfg.Folder, new HttpContextWrapper(ApplicationObject.CurrentOneHttpContext));
            folder = System.Web.VirtualPathUtility.AppendTrailingSlash(folder);
            folder = string.Concat(folder, DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo));
            folder = System.Web.VirtualPathUtility.AppendTrailingSlash(folder);
            string name = customerFileName.ObjToStr().Length > 0 ? customerFileName.ObjToStr() : Guid.NewGuid().ToString("N");
            var fileName = name.Contains(ext) ? string.Concat(folder, name, ext) : string.Concat(folder, name);
            return File.Exists(fileName) ? CreateFileName(ext, cfg) : fileName;
        }

        /// <summary>
        /// 创建一个指定类型带路径的临时文件名
        /// </summary>
        /// <param name="ext">文件类型</param>
        /// <returns></returns>
        private static string CreatTemporaryFileName(string ext, UploadConfig cfg)
        {
            string folder = System.Web.Mvc.UrlHelper.GenerateContentUrl(cfg.TempPath, new HttpContextWrapper(ApplicationObject.CurrentOneHttpContext));
            folder = System.Web.VirtualPathUtility.AppendTrailingSlash(folder);
            string name = Guid.NewGuid().ToString("N");
            return string.Concat(folder, name, ext);
        }

        /// <summary>
        /// 获取上传参数
        /// </summary>
        /// <returns>上传参数</returns>
        public static UploadConfig GetUploadConfig(string configName)
        {
            string configPath = "~/Config/upload.xml";
            string path = httpContext.Server.MapPath(configPath);
            var doc = XDocument.Load(path);
            UploadConfig cfg = new UploadConfig();
            if (doc != null)
            {
                var def = doc.Element("root").Element("default");
                GetConfigByXmlNote(cfg, def);
                if (!string.IsNullOrWhiteSpace(configName))
                {
                    var config = doc.Element("root").Element(configName);
                    if (config != null)
                    {
                        GetConfigByXmlNote(cfg, config);
                    }
                }
            }
            return cfg;
        }

        /// <summary>
        /// 获取上传参数
        /// </summary>
        /// <returns>上传参数</returns>
        public static UploadConfig GetUploadConfig(string configPath, string configName)
        {
            string path = httpContext.Server.MapPath(configPath);
            var doc = XDocument.Load(path);
            UploadConfig cfg = new UploadConfig();
            if (doc != null)
            {
                var def = doc.Element("root").Element("default");
                GetConfigByXmlNote(cfg, def);

                var config = doc.Element("root").Element(configName);
                if (config != null)
                {
                    GetConfigByXmlNote(cfg, config);
                }
            }
            return cfg;
        }

        /// <summary>
        /// 获取上传参数
        /// </summary>
        /// <returns>上传参数</returns>
        public static UploadConfig GetUploadConfig()
        {
            return GetUploadConfig(null);
        }

        /// <summary>
        /// 从XML载入上传配置
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="el"></param>
        private static void GetConfigByXmlNote(UploadConfig cfg, XElement el)
        {
            var allowType = el.Element("allowType");
            if (allowType != null && !string.IsNullOrWhiteSpace(allowType.Value))
            {
                var exts = allowType.Value.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);

                cfg.AllowType = exts.Select(o => System.IO.Path.GetExtension(o).ToUpper()).ToList();
            }

            var maxFileSize = el.Element("maxFileSize");
            if (maxFileSize != null && !string.IsNullOrWhiteSpace(maxFileSize.Value))
            {
                cfg.MaxFileSize = maxFileSize.Value.ObjToInt();
            }

            var tempPath = el.Element("tempPath");
            if (tempPath != null && !string.IsNullOrWhiteSpace(tempPath.Value))
            {
                cfg.TempPath = tempPath.Value;
            }

            var queueSizeLimit = el.Element("queueSizeLimit");
            if (queueSizeLimit != null && !string.IsNullOrWhiteSpace(queueSizeLimit.Value))
            {
                cfg.QueueSizeLimit = queueSizeLimit.Value.ObjToInt();
            }

            var folder = el.Element("folder");
            if (folder != null && !string.IsNullOrWhiteSpace(folder.Value))
            {
                cfg.Folder = folder.Value;
            }

            var basePath = el.Element("basePath");
            if (basePath != null && !string.IsNullOrWhiteSpace(basePath.Value))
            {
                cfg.BasePath = basePath.Value;
            }
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="fileName">保存文件名</param>
        /// <param name="configName">上传配置信息节点名称</param>
        /// <returns>返回地址</returns>
        public static string Save(HttpPostedFileBase file, string fileName, string configName)
        {
            var cfg = UploadFileManager.GetUploadConfig(configName);
            return Save(file, fileName, cfg);
        }

        /// <summary>
        /// 保存文件流
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <param name="ext"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static string Save(Stream stream, string fileName, string ext, UploadConfig cfg)
        {
            Uri uri = null;
            if (string.IsNullOrWhiteSpace(fileName) || Uri.TryCreate(fileName, UriKind.Absolute, out uri))
            {
                fileName = CreateFileName(ext, cfg);
            }
            else
            {
                fileName = CreateFileName(ext, cfg, fileName);
            }
            string diskFileName = GetUploadFileDiskPath(fileName, cfg);
            string dir = Path.GetDirectoryName(diskFileName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var bytes = ReadBytes(stream);
            stream.Close();
            stream.Dispose();

            File.WriteAllBytes(diskFileName, bytes);
            return fileName;
        }

        public static string Save(Stream stream, string fileName, string ext, string configName)
        {
            var cfg = UploadFileManager.GetUploadConfig(configName);
            return Save(stream, fileName, ext, cfg);
        }

        /// <summary>
        /// 上传文件检查
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="cfg">上传配置信息</param>
        public static void Check(HttpPostedFileBase file, string configName)
        {
            var cfg = UploadFileManager.GetUploadConfig(configName);
            Check(file, cfg);
        }

        /// <summary>
        /// 上传文件检查
        /// </summary>
        /// <param name="file">文件信息</param>
        /// <param name="cfg">上传配置信息</param>
        public static void Check(HttpPostedFileBase file, UploadConfig cfg)
        {
            if (file == null || file.ContentLength <= 0)
            {
                throw new Exception("上传文件不能为空！");
            }

            //获取当前请求HttpContext
            var context = ApplicationObject.CurrentOneHttpContext;

            //获取上传文件扩展名
            var ext = System.IO.Path.GetExtension(file.FileName);
            ext = string.IsNullOrWhiteSpace(ext) ? string.Empty : ext.Trim().ToUpper();

            //检查此扩展名文件是否允许上传
            if (!cfg.AllowType.Contains(ext))
            {
                throw new Exception(string.Format("不支持{0}文件类型上传", ext));
            }


            //检查文件大小是否超过限制
            if ((file.ContentLength / 1024) > cfg.MaxFileSize)
            {
                throw new Exception(string.Format("上传文件必须小于{0}", TypeUtil.FileSize(cfg.MaxFileSize * 1024)));
            }
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="fileName">保存文件名</param>
        /// <param name="cfg">上传配置信息</param>
        /// <returns>返回地址</returns>
        public static string Save(HttpPostedFileBase file, string fileName, UploadConfig cfg)
        {
            Check(file, cfg);

            //获取上传文件扩展名
            var ext = System.IO.Path.GetExtension(file.FileName);
            ext = string.IsNullOrWhiteSpace(ext) ? string.Empty : ext.Trim().ToUpper();

            return Save(file.InputStream, fileName, ext, cfg);
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="configName">上传配置信息节点名称</param>
        /// <returns>新保存文件路径信息</returns>
        public static string SaveAs(HttpPostedFileBase file, string configName)
        {
            var cfg = GetUploadConfig(configName);
            return SaveAs(file, cfg);
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="configName">上传配置信息节点名称</param>
        /// <param name="fileName">自定义保存文件夹</param>
        /// <param name="customerFileName">自定义文件名</param>
        /// <returns>新保存文件路径信息</returns>
        public static string SaveAs(HttpPostedFileBase file, string configName, string fileName, string customerFileName = null)
        {
            var cfg = GetUploadConfig(configName);
            cfg.Folder = cfg.Folder.EndsWith("/") ? cfg.Folder + fileName : cfg.Folder + "/" + fileName;
            return SaveAs(file, cfg, customerFileName);
        }

        /// <summary>
        /// 保存上传文件
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="cfg">上传配置信息</param>
        /// <param name="customerFileName">自定义文件名</param>
        /// <returns>新保存文件路径信息</returns>
        public static string SaveAs(HttpPostedFileBase file, UploadConfig cfg, string customerFileName = null)
        {
            string ext = Path.GetExtension(file.FileName);
            string fileName = !string.IsNullOrWhiteSpace(customerFileName) ? customerFileName : file.FileName;
            return Save(file, fileName, cfg);
        }

        /// <summary>
        /// 保存上传文件临时文件夹
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="configName">上传配置信息节点名称</param>
        /// <returns>新保存文件路径信息</returns>
        public static string SaveAsTemporary(HttpPostedFileBase file, string configName)
        {
            var cfg = GetUploadConfig(configName);
            return SaveAsTemporary(file, cfg);
        }

        /// <summary>
        /// 保存上传文件临时文件夹
        /// </summary>
        /// <param name="file">上传文件</param>
        /// <param name="cfg">上传配置信息</param>
        /// <returns>新保存文件路径信息</returns>
        public static string SaveAsTemporary(HttpPostedFileBase file, UploadConfig cfg)
        {
            string ext = Path.GetExtension(file.FileName);
            string fileName = CreatTemporaryFileName(ext, cfg);
            return Save(file, fileName, cfg);
        }

        public static UploadFileModelCollection SaveAs(string attachments, string configName)
        {
            return SaveAs(attachments, configName, ";", "||");
        }

        /// <summary>
        /// 将缓存目录中的文件保存到实际保存目录，并获取文件信息
        /// </summary>
        /// <param name="attachments">文件信息字符串</param>
        /// <param name="fileSplit">文件分隔符</param>
        /// <param name="itemSplit">文件名和文件路径分隔符</param>
        /// <param name="saveFolder">保存的文件夹</param>
        /// <returns>保存的文件Xml集合</returns>
        public static UploadFileModelCollection SaveAs(string attachments, string configName, string fileSplit, string itemSplit)
        {
            var collection = new UploadFileModelCollection();

            //将缓存中的上传文件另存
            if (!string.IsNullOrWhiteSpace(attachments))
            {
                var files = attachments.Split(new string[] { fileSplit }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var cfg = GetUploadConfig(configName);

                //如果实际上传文件个数大于配置中限制的个数，则只获取限制个数的文件
                if (cfg.QueueSizeLimit > 0 && files.Count() > cfg.QueueSizeLimit)
                {
                    files = files.Take(cfg.QueueSizeLimit).ToList();
                }

                foreach (var o in files)
                {
                    var items = o.Split(new string[] { itemSplit }, StringSplitOptions.None).ToList();
                    var file = items[0];
                    var name = items.Count == 2 ? items[1] : System.IO.Path.GetFileName(file);

                    file = System.Web.Mvc.UrlHelper.GenerateContentUrl(file, httpContext);

                    var tempDiskPath = string.Empty;

                    if (System.IO.Path.IsPathRooted(file))
                    {
                        tempDiskPath = httpContext.Server.MapPath(file);
                    }
                    else
                    {
                        tempDiskPath = System.IO.Path.Combine(cfg.BasePath, file);
                    }
                    if (System.IO.File.Exists(tempDiskPath))
                    {
                        string folder = System.Web.Mvc.UrlHelper.GenerateContentUrl(cfg.Folder, httpContext);
                        folder = System.Web.VirtualPathUtility.AppendTrailingSlash(folder);
                        folder = string.Concat(folder, DateTime.Now.ToString("yyyyMM", DateTimeFormatInfo.InvariantInfo));

                        //保存的磁盘目录
                        string diskFolder = string.Empty;

                        if (System.IO.Path.IsPathRooted(folder))
                        {
                            diskFolder = httpContext.Server.MapPath(folder);
                        }
                        else
                        {
                            diskFolder = System.IO.Path.Combine(cfg.BasePath, folder);
                        }

                        //保存磁盘目录检查
                        if (!System.IO.Directory.Exists(diskFolder))
                        {
                            System.IO.Directory.CreateDirectory(diskFolder);
                        }

                        string fileName = System.IO.Path.GetFileName(tempDiskPath);
                        string newFilePath = System.IO.Path.Combine(diskFolder, fileName);

                        //将文件移到实际保存路径中
                        System.IO.File.Move(tempDiskPath, newFilePath);

                        var fileInfo = new System.IO.FileInfo(newFilePath);



                        collection.Add(new UploadFileModel()
                        {
                            FileName = name,
                            FilePath = System.IO.Path.Combine(System.Web.VirtualPathUtility.AppendTrailingSlash(folder), fileName),
                            FileSize = fileInfo.Length
                        });
                    }
                }
            }

            return collection;
        }
    }
}
