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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Concurrent;
using System.Reflection;

namespace Rookey.Frame.Common
{
    public static class EnumHelper
    {
        //系统枚举字典
        static ConcurrentDictionary<Type, Dictionary<object, DisplayAttribute>> values = new ConcurrentDictionary<Type, Dictionary<object, DisplayAttribute>>();
        static ConcurrentDictionary<Assembly, List<Type>> cacheAssemblyTypes = new ConcurrentDictionary<Assembly, List<Type>>();


        /// <summary>
        /// 根据枚举值获取枚举显示信息
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <param name="enumField">枚举成员或枚举值或枚举名称</param>
        /// <param name="defaultText">默认值</param>
        /// <returns>枚举显示信息</returns>
        public static string GetDisplayName(Type enumType, object enumField, string defaultText = "")
        {
            var displays = GetDisplays(enumType);
            try
            {
                var enumItem = Enum.Parse(enumType, enumField.ToString());
                var display = displays[enumItem];
                return display.Name ?? display.ShortName;
            }
            catch (Exception)
            {
                return defaultText;
            }
        }

        /// <summary>
        /// 根据枚举值获取枚举描述信息
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumField"></param>
        /// <returns></returns>
        public static string GetDescription(Type enumType, object enumField, string defaultText = "")
        {
            string Description = string.Empty;
            try
            {
                var enumObj = Enum.Parse(enumType, enumField.ToString());
                System.Reflection.FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());
                object[] attribArray = fieldInfo.GetCustomAttributes(false);
                if (attribArray.Length == 0)
                    return Description;
                else
                {
                    return (attribArray[0] as DescriptionAttribute).Description;
                }
            }
            catch (Exception)
            {
                return defaultText;
            }
        }

        /// <summary>
        /// 根据枚举值获取枚举显示信息
        /// </summary>
        /// <param name="enumField">枚举成员</param>
        /// <returns>枚举显示信息</returns>
        public static string GetDisplayName(object enumField)
        {
            return GetDisplayName(enumField.GetType(), enumField);
        }

        /// <summary>
        /// 根据枚举值获取枚举显示信息
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <param name="value">枚举成员值</param>
        /// <param name="defaultText">未找到枚举值时的默认显示</param>
        /// <returns>枚举显示信息</returns>
        public static string GetDisplayName<TEnum>(object value, string defaultText = "")
        {
            return GetDisplayName(typeof(TEnum), value, defaultText);
        }

        /// <summary>
        /// 获取枚举类型的显示属性
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns>枚举和显示属性字典</returns>
        public static Dictionary<object, string> GetDisplayNames<TEnum>()
        {
            return GetDisplayNames(typeof(TEnum));
        }

        /// <summary>
        /// 获取枚举类型的显示属性
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举和显示属性字典</returns>
        public static Dictionary<object, string> GetDisplayNames(Type enumType)
        {
            return GetDisplays(enumType)
                    .OrderBy(o => o.Value.GetOrder() ?? 0)
                    .ToDictionary(o => o.Key, o => o.Value.Name ?? o.Value.ShortName);
        }

        /// <summary>
        /// 在程序集中搜索枚举类型，并转换成字典返回
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <returns>返回信息</returns>
        public static Dictionary<Type, Dictionary<object, string>> GetDisplayNames(params Assembly[] assemblies)
        {
            var dirs = GetDisplays(assemblies);
            return dirs.ToDictionary(
                o => o.Key,
                o => o.Value.ToDictionary(
                    x => x.Key,
                    x => x.Value.Name ?? x.Value.ShortName
                )
            );
        }

        /// <summary>
        /// 获取枚举类型的显示属性
        /// </summary>
        /// <typeparam name="TEnum">枚举类型</typeparam>
        /// <returns>枚举和显示属性字典</returns>
        public static Dictionary<object, DisplayAttribute> GetDisplays<TEnum>()
        {
            return GetDisplays(typeof(TEnum));
        }

        /// <summary>
        /// 在程序集中搜索枚举类型，并转换成字典返回
        /// </summary>
        /// <param name="assemblies">程序集</param>
        /// <returns>返回信息</returns>
        public static Dictionary<Type, Dictionary<object, DisplayAttribute>> GetDisplays(params Assembly[] assemblies)
        {
            var enumDicts = new Dictionary<Type, Dictionary<object, DisplayAttribute>>();
            if (assemblies == null) return enumDicts;
            foreach (var arr in assemblies)
            {
                if (cacheAssemblyTypes.ContainsKey(arr))
                {
                    var types = cacheAssemblyTypes[arr];
                    foreach (var o in types)
                    {
                        enumDicts.Add(o, GetDisplays(o));
                    }
                }
                else
                {
                    var types = arr.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.IsEnum)
                        {
                            enumDicts.Add(type, GetDisplays(type));
                        }
                    }
                }
            }
            return enumDicts;
        }

        /// <summary>
        /// 获取枚举类型的显示属性
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>枚举和显示属性字典</returns>
        public static Dictionary<object, DisplayAttribute> GetDisplays(Type enumType)
        {
            if (!enumType.IsEnum) throw new Exception("非枚举类型");

            Dictionary<object, DisplayAttribute> result;
            if (values.TryGetValue(enumType, out result)) return result;
            result = new Dictionary<object, DisplayAttribute>();
            var names = Enum.GetNames(enumType);
            foreach (var name in names)
            {
                var enumItem = Enum.Parse(enumType, name);
                var displays = enumType.GetField(name).GetCustomAttributes(typeof(DisplayAttribute), true);
                if (displays == null || displays.FirstOrDefault() == null)
                {
                    result.AddOrUpdate(enumItem, new DisplayAttribute()
                    {
                        Name = name
                    });
                }
                else
                {
                    var display = (DisplayAttribute)displays.First();
                    result.AddOrUpdate(enumItem, display);
                }
            }
            values.AddOrUpdate(enumType, result);
            return result;
        }

        /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <returns>键值对</returns>
        public static Dictionary<string, string> GetEnumDescValue<T>()
        {
            return GetEnumDescValue(typeof(T));
        }

        /// <summary>
        /// /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        public static Dictionary<string, string> GetEnumDescValue(Type enumType)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strValue = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    keyValues.Add(strText, strValue);
                }
            }
            return keyValues;
        }

        /// <summary>
        /// /// <summary>
        /// 从枚举类型和它的特性读出并返回一个键值对
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <returns>键值对</returns>
        public static Dictionary<string, string> GetEnumDescName(Type enumType)
        {
            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            Type typeDescription = typeof(DescriptionAttribute);
            System.Reflection.FieldInfo[] fields = enumType.GetFields();
            string strText = string.Empty;
            string strName = string.Empty;
            foreach (FieldInfo field in fields)
            {
                if (field.FieldType.IsEnum)
                {
                    strName = field.Name;
                    object[] arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        DescriptionAttribute aa = (DescriptionAttribute)arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }
                    keyValues.Add(strText, strName);
                }
            }
            return keyValues;
        }

        /// <summary>
        /// 获取Enum的描述信息
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            while (((enumType.IsGenericType && (enumType.GetGenericTypeDefinition() == typeof(Nullable<>))) && ((enumType.GetGenericArguments() != null) && (enumType.GetGenericArguments().Length == 1))) && enumType.GetGenericArguments()[0].IsEnum)
            {
                enumType = enumType.GetGenericArguments()[0];
            }
            if (!enumType.IsEnum)
            {
                return string.Empty;
            }
            string name = enumValue.ToString();
            FieldInfo field = enumType.GetField(name);
            if (field == null)
            {
                return name;
            }
            object[] customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((customAttributes != null) && (customAttributes.Length > 0))
            {
                DescriptionAttribute attribute = customAttributes[0] as DescriptionAttribute;
                name = attribute.Description;
            }

            return (name ?? field.Name);
        }
    }
}
