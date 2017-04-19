/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rookey.Frame.Common
{
    public class UploadFileModelCollection : List<UploadFileModel>
    {
        /// <summary>
        /// 讲文件集信息输出为Xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            var el = new XElement("files");

            foreach (var item in this)
            {
                el.Add(new XElement("file",
                            new XElement("name", item.FileName),
                            new XElement("size", item.FileSize),
                            new XElement("url", item.FilePath)
                      )
                 );
            }
            return el.ToString();
        }

        /// <summary>
        /// 从Xml载入文件信息
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static UploadFileModelCollection ParseByXml(string xml)
        {
            if (string.IsNullOrWhiteSpace(xml))
            {
                return new UploadFileModelCollection();
            }
            var el = XElement.Parse(xml);
            var attachs = new UploadFileModelCollection();
            if (el != null)
            {
                foreach (var e in el.Elements())
                {
                    UploadFileModel attach = new UploadFileModel();
                    var name = e.Element("name").Value;
                    var url = e.Element("url").Value;
                    var size = e.Element("size").Value;
                    attach.FileName = name;
                    attach.FileSize = size.ObjToLong();
                    attach.FilePath = url;
                    attachs.Add(attach);
                }
            }
            return attachs;
        }
    }
}
