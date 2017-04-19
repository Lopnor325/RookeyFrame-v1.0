/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 对象操作帮助类
    /// </summary>
    public static class ObjectHelper
    {
        #region 实体复制

        /// <summary>
        /// 实现对象属性复制
        /// </summary>
        /// <param name="origin">原始对象</param>
        /// <param name="target">目标对象</param>
        public static void CopyValue(object origin, object target)
        {
            PropertyInfo[] properties = (target.GetType()).GetProperties();
            PropertyInfo[] fields = (origin.GetType()).GetProperties();
            for (int i = 0; i < fields.Length; i++)
            {
                for (int j = 0; j < properties.Length; j++)
                {
                    if (fields[i].Name == properties[j].Name && fields[i].PropertyType == properties[j].PropertyType && properties[j].CanWrite)
                    {
                        try
                        {
                            properties[j].SetValue2(target, fields[i].GetValue2(origin, null), null);
                        }
                        catch { }
                    }
                }
            }
        }

        /// <summary>
        /// 对象复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="origin">源对象</param>
        /// <param name="target">目标对象</param>
        /// <param name="copyFields">复制的字段集合</param>
        public static void CopyValue<T>(T origin, T target, List<string> copyFields = null) where T : class
        {
            if (origin == null || target == null) return;
            PropertyInfo[] properties = typeof(T).GetProperties().Where(x => x.CanWrite).ToArray();
            if (copyFields != null && copyFields.Count > 0)
                properties = properties.Where(x => copyFields.Contains(x.Name)).ToArray();
            foreach (PropertyInfo p in properties)
            {
                try
                {
                    p.SetValue2(target, p.GetValue2(origin, null), null);
                }
                catch { }
            }
        }

        #endregion

        #region DataTable转换成实体类

        /// <summary>
        /// 填充对象列表：用DataSet的第一个表填充实体类
        /// </summary>
        /// <param name="ds">DataSet</param>
        /// <returns></returns>
        public static List<T> FillModel<T>(DataSet ds) where T : class,new()
        {
            if (ds == null || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
            {
                return new List<T>();
            }
            else
            {
                return FillModel<T>(ds.Tables[0]);
            }
        }

        /// <summary>  
        /// 填充对象列表：用DataSet的第index个表填充实体类
        /// </summary>  
        public static List<T> FillModel<T>(DataSet ds, int index) where T : class,new()
        {
            if (ds == null || ds.Tables.Count <= index || ds.Tables[index].Rows.Count == 0)
            {
                return new List<T>();
            }
            else
            {
                return FillModel<T>(ds.Tables[index]);
            }
        }

        /// <summary>  
        /// 填充对象列表：用DataTable填充实体类
        /// </summary>  
        public static List<T> FillModel<T>(DataTable dt) where T : class,new()
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return new List<T>();
            }
            List<T> modelList = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T model = FillModel<T>(dr);
                modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// 填充对象列表，用DataTable填充实体集合
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static IEnumerable FillModel(Type modelType, DataTable dt)
        {
            if (modelType == null || dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            List<object> modelList = new List<object>();
            foreach (DataRow dr in dt.Rows)
            {
                object model = FillModel(modelType, dr);
                if (model != null)
                    modelList.Add(model);
            }
            return modelList;
        }

        /// <summary>
        /// 填充对象
        /// </summary>
        /// <param name="modelType">实体类型</param>
        /// <param name="dr">dr</param>
        /// <returns></returns>
        public static object FillModel(Type modelType, DataRow dr)
        {
            if (modelType == null || dr == null) return null;
            object model = Activator.CreateInstance(modelType);
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                try
                {
                    PropertyInfo propertyInfo = modelType.GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue2(model, TypeUtil.ChangeType(dr[i], propertyInfo.PropertyType), null);
                }
                catch { }
            }
            return model;
        }

        /// <summary>  
        /// 填充对象：用DataRow填充实体类
        /// </summary>  
        public static T FillModel<T>(DataRow dr) where T : class,new()
        {
            if (dr == null)
            {
                return default(T);
            }
            T model = new T();
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                try
                {
                    PropertyInfo propertyInfo = typeof(T).GetProperty(dr.Table.Columns[i].ColumnName);
                    if (propertyInfo != null && dr[i] != DBNull.Value)
                        propertyInfo.SetValue2(model, TypeUtil.ChangeType(dr[i], propertyInfo.PropertyType), null);
                }
                catch { }
            }
            return model;
        }

        #endregion

        #region 实体类转换成DataTable

        /// <summary>
        /// 实体类转换成DataSet
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataSet FillDataSet<T>(List<T> modelList) where T : class,new()
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            else
            {
                DataSet ds = new DataSet();
                ds.Tables.Add(FillDataTable(modelList));
                return ds;
            }
        }

        /// <summary>
        /// 实体类转换成DataTable
        /// </summary>
        /// <param name="modelList">实体类列表</param>
        /// <returns></returns>
        public static DataTable FillDataTable<T>(List<T> modelList) where T : class,new()
        {
            if (modelList == null || modelList.Count == 0)
            {
                return null;
            }
            DataTable dt = CreateData(modelList[0]);

            foreach (T model in modelList)
            {
                DataRow dataRow = dt.NewRow();
                foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
                {
                    dataRow[propertyInfo.Name] = propertyInfo.GetValue2(model, null);
                }
                dt.Rows.Add(dataRow);
            }
            return dt;
        }

        /// <summary>
        /// 根据实体类得到表结构
        /// </summary>
        /// <param name="model">实体类</param>
        /// <returns></returns>
        private static DataTable CreateData<T>(T model) where T : class,new()
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
            }
            return dataTable;
        }

        #endregion
    }
}
