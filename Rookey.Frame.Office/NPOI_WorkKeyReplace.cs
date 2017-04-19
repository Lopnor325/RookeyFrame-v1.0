/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Rookey.Frame.Office
{
    /// <summary>
    /// Word操作帮助
    /// </summary>
    public class NPOI_WordHelper
    {
        /// <summary>
        /// 将模板文件中关键字替换并生成新的word文件
        /// </summary>
        /// <param name="templateFileName">模板文件</param>
        /// <param name="model">实体对象</param>
        /// <param name="fileName">保存的路径</param>
        public static void WordKeyReplace(string templateFileName, object model, string fileName)
        {
            using (MemoryStream ms = Export(templateFileName, model))
            {
                if (ms != null)
                    SaveToFile(ms, fileName);
            }
        }

        /// <summary>
        /// 将模板文件中关键字替换并生成新的word文件
        /// </summary>
        /// <param name="templateFileName">模板文件</param>
        /// <param name="replaceKeyAndValue">key:替换关键字,value:将key替换成value</param>
        /// <param name="fileName">保存的路径</param>
        public static void WordKeyReplace(string templateFileName, Dictionary<string, string> replaceKeyAndValue, string fileName)
        {
            using (MemoryStream ms = Export(templateFileName, replaceKeyAndValue))
            {
                if (ms != null)
                    SaveToFile(ms, fileName);
            }
        }

        /// <summary>
        /// 导出word模板流
        /// </summary>
        /// <param name="filepath">模板文件</param>
        /// <param name="model">实体对象</param>
        /// <returns></returns>
        public static MemoryStream Export(string filepath, object model)
        {
            try
            {
                if (model == null) return null;
                using (FileStream stream = File.OpenRead(filepath))
                {
                    XWPFDocument doc = new XWPFDocument(stream);
                    //遍历段落
                    foreach (var para in doc.Paragraphs)
                    {
                        ReplaceKey(para, model);
                    }
                    //遍历表格
                    var tables = doc.Tables;
                    Type t = model.GetType();
                    PropertyInfo[] pi = t.GetProperties();
                    foreach (var table in tables)
                    {
                        foreach (var row in table.Rows)
                        {
                            foreach (var cell in row.GetTableCells())
                            {
                                bool handlePara = true;
                                foreach (PropertyInfo p in pi)
                                {
                                    string key = p.Name;
                                    if (cell.GetText() == key)
                                    {
                                        object obj = p.GetValue(model, null);
                                        string value = obj != null ? obj.ToString() : string.Empty;
                                        cell.SetText(value);
                                        for (int i = 0; i < cell.Paragraphs.Count; i++)
                                        {
                                            var para = cell.Paragraphs[i];
                                            var runs = para.Runs;
                                            for (int j = 0; j < runs.Count; j++)
                                            {
                                                runs[j].SetText(string.Empty, 0);
                                            }
                                        }
                                        handlePara = false;
                                    }
                                }
                                if (handlePara)
                                {
                                    foreach (var para in cell.Paragraphs)
                                    {
                                        ReplaceKey(para, model);
                                    }
                                }
                            }
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        doc.Write(ms);
                        return ms;
                    }
                }
            }
            catch (Exception ex)
            { }
            return null;
        }

        /// <summary>
        /// 导出word模板流
        /// </summary>
        /// <param name="filepath">模板文件</param>
        /// <param name="replaceKeyAndValue">key:替换关键字,value:将key替换成value</param>
        /// <returns></returns>
        public static MemoryStream Export(string filepath, Dictionary<string, string> replaceKeyAndValue)
        {
            try
            {
                if (replaceKeyAndValue == null || replaceKeyAndValue.Count == 0)
                    return null;
                using (FileStream stream = File.OpenRead(filepath))
                {
                    XWPFDocument doc = new XWPFDocument(stream);
                    //遍历段落
                    foreach (var para in doc.Paragraphs)
                    {
                        ReplaceKey(para, replaceKeyAndValue);
                    }
                    //遍历表格
                    var tables = doc.Tables;
                    foreach (var table in tables)
                    {
                        foreach (var row in table.Rows)
                        {
                            foreach (var cell in row.GetTableCells())
                            {
                                bool handlePara = true;
                                foreach (string key in replaceKeyAndValue.Keys)
                                {
                                    if (cell.GetText() == key)
                                    {
                                        string value = replaceKeyAndValue[key];
                                        if (value == null) value = string.Empty;
                                        cell.SetText(value);
                                        for (int i = 0; i < cell.Paragraphs.Count; i++)
                                        {
                                            var para = cell.Paragraphs[i];
                                            var runs = para.Runs;
                                            for (int j = 0; j < runs.Count; j++)
                                            {
                                                runs[j].SetText(string.Empty, 0);
                                            }
                                        }
                                        handlePara = false;
                                    }
                                }
                                if (handlePara)
                                {
                                    foreach (var para in cell.Paragraphs)
                                    {
                                        ReplaceKey(para, replaceKeyAndValue);
                                    }
                                }
                            }
                        }
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        doc.Write(ms);
                        return ms;
                    }
                }
            }
            catch (Exception ex)
            { }
            return null;
        }

        /// <summary>
        /// 替换关键字
        /// </summary>
        /// <param name="para"></param>
        /// <param name="model"></param>
        private static void ReplaceKey(XWPFParagraph para, object model)
        {
            if (model == null) return;
            string text = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                Type t = model.GetType();
                PropertyInfo[] pi = t.GetProperties();
                foreach (PropertyInfo p in pi)
                {
                    string key = p.Name;
                    if (text.Contains(key))
                    {
                        object obj = p.GetValue(model, null);
                        string value = obj != null ? obj.ToString() : string.Empty;
                        text = text.Replace(key, value);
                    }
                }
                runs[i].SetText(text, 0);
            }
        }

        /// <summary>
        /// 替换关键字
        /// </summary>
        /// <param name="para"></param>
        /// <param name="replaceKeyAndValue"></param>
        private static void ReplaceKey(XWPFParagraph para, Dictionary<string, string> replaceKeyAndValue)
        {
            if (replaceKeyAndValue == null || replaceKeyAndValue.Count == 0)
                return;
            string text = para.ParagraphText;
            var runs = para.Runs;
            string styleid = para.Style;
            for (int i = 0; i < runs.Count; i++)
            {
                var run = runs[i];
                text = run.ToString();
                foreach (string key in replaceKeyAndValue.Keys)
                {
                    if (text.Contains(key))
                    {
                        string value = replaceKeyAndValue[key];
                        if (value == null) value = string.Empty;
                        text = text.Replace(key, value);
                    }
                }
                runs[i].SetText(text, 0);
            }
        }

        /// <summary>
        /// 保存Word文档流到文件
        /// </summary>
        /// <param name="ms">word文档流</param>
        /// <param name="fileName">文件名</param>
        private static void SaveToFile(MemoryStream ms, string fileName)
        {
            try
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
                using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    byte[] data = ms.ToArray();

                    fs.Write(data, 0, data.Length);
                    fs.Flush();

                    data = null;
                }
            }
            catch (Exception ex)
            { }
        }
    }
}
