using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace Rookey.Frame.Controllers.FileManage
{
    /// <summary>
    /// 解压缩处理类
    /// </summary>
    public static class ZipClass
    {
        public static string BackUpPath;

        private static bool ZipFileDictory(string FolderToZip, ZipOutputStream s, string ParentFolderName, bool separate, string[] files, string[] dirs)
        {
            bool res = true;
            string[] folders, filenames;
            ZipEntry entry = null;
            FileStream fs = null;
            Crc32 crc = new Crc32();

            try
            {
                //创建当前文件夹
                entry = new ZipEntry(Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip) + "/")); //加上 “/” 才会当成是文件夹创建
                s.PutNextEntry(entry);
                s.Flush();

                //先压缩文件，再递归压缩文件夹 
                if (separate)
                {
                    filenames = files;
                }
                else
                {
                    filenames = Directory.GetFiles(FolderToZip);
                }

                foreach (string file in filenames)
                {
                    //打开压缩文件
                    fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string p = Path.GetFileName(FolderToZip);

                    if (p.Length < 1)
                    {
                        p = Path.GetFileName(file);
                    }
                    else
                    {
                        p += "/" + Path.GetFileName(file);
                    }

                    entry = new ZipEntry(Path.Combine(ParentFolderName, p));

                    entry.DateTime = DateTime.Now;
                    entry.Size = fs.Length;
                    fs.Close();

                    crc.Reset();
                    crc.Update(buffer);

                    entry.Crc = crc.Value;

                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }

                if (entry != null)
                {
                    entry = null;
                }

                GC.Collect();
                GC.Collect(1);
            }

            if (separate)
            {
                folders = dirs;
            }
            else
            {
                folders = Directory.GetDirectories(FolderToZip);
            }

            foreach (string folder in folders)
            {
                if (folder.Equals(BackUpPath, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }
                else if (!ZipFileDictory(folder, s, Path.Combine(ParentFolderName, Path.GetFileName(FolderToZip)), false, null, null))
                {
                    return false;
                }
            }

            return res;
        }

        private static bool ZipFileDictory(string FolderToZip, string ZipedFile, String Password, bool separate, string[] files, string[] dirs)
        {
            bool res;

            if (!Directory.Exists(FolderToZip))
            {
                return false;
            }

            ZipOutputStream s = new ZipOutputStream(File.Create(ZipedFile));
            s.SetLevel(6);
            s.Password = Password;

            res = ZipFileDictory(FolderToZip, s, "", separate, files, dirs);

            s.Finish();
            s.Close();

            return res;
        }

        private static bool ZipFile(string FileToZip, string ZipedFile, String Password)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + FileToZip + " 不存在!");
            }

            FileStream ZipFile = null;
            ZipOutputStream ZipStream = null;
            ZipEntry ZipEntry = null;

            bool res = true;

            try
            {
                ZipFile = File.OpenRead(FileToZip);
                byte[] buffer = new byte[ZipFile.Length];
                ZipFile.Read(buffer, 0, buffer.Length);
                ZipFile.Close();

                ZipFile = File.Create(ZipedFile);
                ZipStream = new ZipOutputStream(ZipFile);
                ZipStream.Password = Password;
                ZipEntry = new ZipEntry(Path.GetFileName(FileToZip));
                ZipStream.PutNextEntry(ZipEntry);
                ZipStream.SetLevel(6);

                ZipStream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                res = false;
            }
            finally
            {
                if (ZipEntry != null)
                {
                    ZipEntry = null;
                }

                if (ZipStream != null)
                {
                    ZipStream.Finish();
                    ZipStream.Close();
                }

                if (ZipFile != null)
                {
                    ZipFile.Close();
                    ZipFile = null;
                }

                GC.Collect();
                GC.Collect(1);
            }

            return res;
        }

        public static bool Zip(String FileToZip, String ZipedFile, String Password, bool separate, string[] files, string[] dirs)
        {
            BackUpPath = Path.GetDirectoryName(ZipedFile);

            if (Directory.Exists(FileToZip))
            {
                return ZipFileDictory(FileToZip, ZipedFile, Password, separate, files, dirs);
            }
            else if (File.Exists(FileToZip))
            {
                return ZipFile(FileToZip, ZipedFile, Password);
            }
            else
            {
                return false;
            }
        }

        public static bool Zip(String FileToZip, String ZipedFile, String Password)
        {
            return Zip(FileToZip, ZipedFile, Password, false, null, null);
        }

        public static void UnZip(string FileToUpZip, string ZipedFolder, string Password)
        {
            if (!File.Exists(FileToUpZip))
            {
                return;
            }

            if (!Directory.Exists(ZipedFolder))
            {
                Directory.CreateDirectory(ZipedFolder);
            }

            ZipInputStream s = null;
            ZipEntry theEntry = null;

            string fileName;
            FileStream streamWriter = null;

            try
            {
                s = new ZipInputStream(File.OpenRead(FileToUpZip));

                s.Password = Password;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    if (theEntry.Name != String.Empty)
                    {
                        fileName = Path.Combine(ZipedFolder, theEntry.Name);

                        if (fileName.EndsWith("/") || fileName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(fileName);
                            continue;
                        }

                        streamWriter = File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[2048];

                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter = null;
                }

                if (theEntry != null)
                {
                    theEntry = null;
                }

                if (s != null)
                {
                    s.Close();
                    s = null;
                }

                GC.Collect();
                GC.Collect(1);
            }
        }
    }
}
