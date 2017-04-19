/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Cache.Memcached;
using System;
using System.Collections.Generic;
using Rookey.Frame.Common;

namespace Rookey.Frame.Cache.Factory.Provider
{
    /// <summary>
    /// Memcached缓存
    /// </summary>
    public class MemcachedCacheProvider : ICacheProvider
    {
        #region 构造函数

        private MemcachedClientCache cache = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MemcachedCacheProvider()
        {
            cache = new MemcachedClientCache();
            try
            {
                string path = WebHelper.GetConfigFilePath("memcachedcache.config");
                MemcachedCacheConfigInfo configInfo = (MemcachedCacheConfigInfo)XmlHelper.DeserializeFromXML(typeof(MemcachedCacheConfigInfo), path);
                if (configInfo != null)
                {
                    cache = new MemcachedClientCache(configInfo.ServerList, configInfo.MinPoolSize, configInfo.MaxPoolSize, configInfo.ConnectionTimeOut, configInfo.DeadTimeOut);
                }
                else
                {
                    cache = new MemcachedClientCache();
                }
            }
            catch
            {
                cache = new MemcachedClientCache();
            }
        }
        #endregion

        #region 单键值

        #region Add

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add(string key, object value)
        {
            cache.Set(key, value);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add<T>(string key, T value)
        {
            cache.Set<T>(key, value);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            cache.Set(key, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            cache.Set<T>(key, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            cache.Set(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            cache.Set<T>(key, value, absoluteExpiration);
        }

        #endregion

        #region Set

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Set(string key, object value)
        {
            Add(key, value);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Set<T>(string key, T value)
        {
            Add<T>(key, value);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            Add(key, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            Add<T>(key, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Set(string key, object value, DateTime absoluteExpiration)
        {
            Add(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Set<T>(string key, T value, DateTime absoluteExpiration)
        {
            Add<T>(key, value, absoluteExpiration);
        }

        #endregion

        #region Get

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <returns>被缓存的对象。</returns>
        public object Get(string key)
        {
            return cache.Get(key);
        }

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <returns>被缓存的对象。</returns>
        public T Get<T>(string key)
        {
            object tempObj = cache.Get(key);
            return tempObj == null ? default(T) : (T)tempObj;
        }

        #endregion

        #region Other

        /// <summary>
        /// 从缓存中移除对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        public void Remove(string key)
        {
            cache.Remove(key);
        }

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示拥有指定键值的缓存是否存在。
        /// </summary>
        /// <param name="key">指定的键值。</param>
        /// <returns>如果缓存存在，则返回true，否则返回false。</returns>
        public bool Exists(string key)
        {
            return cache.Get(key) != null;
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void FlushAll()
        {
            cache.FlushAll();
        }

        #endregion

        #endregion

        #region 双键值

        #region Add

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add(string key, string valKey, object value)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, object> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, object>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }
            cache.Set(key, dict);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add<T>(string key, string valKey, T value)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, T> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, T>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }
            cache.Set<Dictionary<string, T>>(key, dict);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add(string key, string valKey, object value, TimeSpan slidingExpiration)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, object> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, object>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }
            cache.Set(key, dict, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add<T>(string key, string valKey, T value, TimeSpan slidingExpiration)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, T> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, T>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }
            cache.Set<Dictionary<string, T>>(key, dict, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add(string key, string valKey, object value, DateTime absoluteExpiration)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, object> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, object>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }
            cache.Set(key, dict, absoluteExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add<T>(string key, string valKey, T value, DateTime absoluteExpiration)
        {
            object tempDict = cache.Get(key);
            Dictionary<string, T> dict = null;
            if (tempDict != null) //缓存存在
            {
                dict = (Dictionary<string, T>)tempDict;
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }
            cache.Set<Dictionary<string, T>>(key, dict, absoluteExpiration);
        }

        #endregion

        #region Set

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Set(string key, string valKey, object value)
        {
            Add(key, valKey, value);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Set<T>(string key, string valKey, T value)
        {
            Add<T>(key, valKey, value);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Set(string key, string valKey, object value, TimeSpan slidingExpiration)
        {
            Add(key, valKey, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Set<T>(string key, string valKey, T value, TimeSpan slidingExpiration)
        {
            Add<T>(key, valKey, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Set(string key, string valKey, object value, DateTime absoluteExpiration)
        {
            Add(key, valKey, value, absoluteExpiration);
        }

        /// <summary>
        /// 向缓存中更新一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Set<T>(string key, string valKey, T value, DateTime absoluteExpiration)
        {
            Add<T>(key, valKey, value, absoluteExpiration);
        }

        #endregion

        #region Get

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <returns>被缓存的对象。</returns>
        public object Get(string key, string valKey)
        {
            object tempDict = cache.Get(key);
            if (tempDict != null)
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)tempDict;
                if (dict != null && dict.ContainsKey(valKey))
                    return dict[valKey];
                else
                    return null;
            }
            return null;
        }

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <returns>被缓存的对象。</returns>
        public T Get<T>(string key, string valKey)
        {
            object tempDict = cache.Get(key);
            if (tempDict != null)
            {
                Dictionary<string, T> dict = (Dictionary<string, T>)tempDict;
                if (dict != null && dict.ContainsKey(valKey))
                    return dict[valKey];
                else
                    return default(T);
            }
            return default(T);
        }

        #endregion

        #region Other

        /// <summary>
        /// 从缓存中移除对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值键</param>
        public void Remove(string key, string valKey)
        {
            object tempDict = cache.Get(key);
            if (tempDict != null)
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)tempDict;
                if (dict != null && dict.ContainsKey(valKey))
                {
                    dict.Remove(valKey);
                    cache.Set(key, dict);
                }
            }
        }

        /// <summary>
        /// 获取一个<see cref="Boolean"/>值，该值表示拥有指定键值和缓存值键的缓存是否存在。
        /// </summary>
        /// <param name="key">指定的键值。</param>
        /// <param name="valKey">缓存值键。</param>
        /// <returns>如果缓存存在，则返回true，否则返回false。</returns>
        public bool Exists(string key, string valKey)
        {
            object tempDict = cache.Get(key);
            return tempDict != null && ((Dictionary<string, object>)tempDict).ContainsKey(valKey);
        }

        #endregion

        #endregion
    }
}
