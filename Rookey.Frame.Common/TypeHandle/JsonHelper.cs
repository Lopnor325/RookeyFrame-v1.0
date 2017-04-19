/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Script.Serialization;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 序列化帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <param name="errMsg">异常信息</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json, out string errMsg) where T : class
        {
            errMsg = string.Empty;
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return default(T);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json">JSON数据</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="json">JSON数据</param>
        /// <param name="type">类型</param>
        /// <param name="errMsg">异常信息</param>
        /// <returns></returns>
        public static object Deserialize(string json, Type type, out string errMsg)
        {
            try
            {
                errMsg = string.Empty;
                return JsonConvert.DeserializeObject(json, type);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return null;
        }

        /// <summary>
        /// 序列化到json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Serialize<T>(T entity) where T : class
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(entity, Newtonsoft.Json.Formatting.Indented, timeFormat);
        }

        /// <summary>
        /// 将datatable转换为json  
        /// </summary>
        /// <param name="dt">Dt</param>
        /// <returns>JSON字符串</returns>
        public static string DataTableToJson(System.Data.DataTable dt)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            System.Collections.ArrayList dic = new System.Collections.ArrayList();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                System.Collections.Generic.Dictionary<string, object> drow = new System.Collections.Generic.Dictionary<string, object>();
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    drow.Add(dc.ColumnName, dr[dc.ColumnName]);
                }
                dic.Add(drow);

            }
            //序列化  
            return js.Serialize(dic);
        }

        /// <summary>    
        /// 将获取的Json数据转换为DataTable    
        /// </summary>    
        /// <param name="strJson">Json字符串</param>   
        /// <returns></returns>    
        public static System.Data.DataTable JsonToDataTable(string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            ArrayList dic = js.Deserialize<ArrayList>(json);
            System.Data.DataTable dt = new System.Data.DataTable();
            if (dic.Count > 0)
            {
                foreach (Dictionary<string, object> drow in dic)
                {
                    if (dt.Columns.Count == 0)
                    {
                        foreach (string key in drow.Keys)
                        {
                            dt.Columns.Add(key, drow[key].GetType());
                        }
                    }
                    System.Data.DataRow row = dt.NewRow();
                    foreach (string key in drow.Keys)
                    {
                        row[key] = drow[key];
                    }
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }
    }

    public class MyJsonHelper<T> where T : class
    {
        /// <summary>
        /// 反序列化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public T Deserialize(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 反序列化为对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<T> CollectDeserialize(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<T>>(json);
        }

        /// <summary>
        /// 序列化到json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Serialize(T entity)
        {
            IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
            timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            return JsonConvert.SerializeObject(entity, Newtonsoft.Json.Formatting.Indented, timeFormat);
        }
    }
}
