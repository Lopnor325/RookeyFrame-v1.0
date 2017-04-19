/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading;

namespace Rookey.Frame.Cache.Factory.Provider
{
    /// <summary>
    /// 内存缓存提供器
    /// </summary>
    public class MemoryCacheProvider : ICacheProvider
    {
        /// <summary>
        /// 单个写线程锁与多个读线程的锁
        /// </summary>
        private static ReaderWriterLock rw = new ReaderWriterLock();

        #region 属性

        /// <summary>
        /// MemoryCacheCache
        /// </summary>
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
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
            rw.AcquireWriterLock(500);
            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;
            Cache.Set(new CacheItem(key, value), policy);
            rw.ReleaseWriterLock();
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add<T>(string key, T value)
        {
            Add(key, value as object);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            //读缓存锁
            rw.AcquireWriterLock(500);
            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.SlidingExpiration = slidingExpiration;
            Cache.Set(new CacheItem(key, value), policy);
            rw.ReleaseWriterLock();
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="slidingExpiration">活动过期时间。</param>
        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            Add(key, value, slidingExpiration);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = absoluteExpiration;
            Cache.Set(new CacheItem(key, value), policy);
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="value">需要缓存的对象。</param>
        /// <param name="absoluteExpiration">绝对过期时间。</param>
        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            Add(key, value, absoluteExpiration);
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
            Set(key, value as object);
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
            Set(key, value as object, slidingExpiration);
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
            Set(key, value as object, absoluteExpiration);
        }

        #endregion

        #region Get

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <returns>被缓存的对象。</returns>
        public T Get<T>(string key)
        {
            object obj = Cache.Get(key);
            return obj == null ? default(T) : (T)obj;
        }

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <returns>被缓存的对象。</returns>
        public object Get(string key)
        {
            rw.AcquireReaderLock(100);
            object obj = Cache.Get(key);
            rw.ReleaseReaderLock();
            return obj;
        }

        #endregion

        #region Other

        /// <summary>
        /// 从缓存中移除对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        public void Remove(string key)
        {
            rw.AcquireWriterLock(100);
            Cache.Remove(key);
            rw.ReleaseWriterLock();
        }

        /// <summary>
        /// 获取一个值，该值表示拥有指定键值的缓存是否存在。
        /// </summary>
        /// <param name="key">指定的键值。</param>
        /// <returns>如果缓存存在，则返回true，否则返回false。</returns>
        public bool Exists(string key)
        {
            return Cache.Contains(key);
        }

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        public void FlushAll()
        {
            rw.AcquireWriterLock(500);
            foreach (var item in Cache)
                Remove(item.Key);
            rw.ReleaseWriterLock();
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
            rw.AcquireWriterLock(500);
            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;

            Dictionary<string, object> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, object>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
        }

        /// <summary>
        /// 向缓存中添加一个对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <param name="value">需要缓存的对象。</param>
        public void Add<T>(string key, string valKey, T value)
        {
            rw.AcquireWriterLock(500);
            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.NotRemovable;

            Dictionary<string, T> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, T>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
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
            rw.AcquireWriterLock(500);
            Dictionary<string, object> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, object>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }

            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.SlidingExpiration = slidingExpiration;

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
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
            rw.AcquireWriterLock(500);
            Dictionary<string, T> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, T>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }

            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.SlidingExpiration = slidingExpiration;

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
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
            rw.AcquireWriterLock(500);
            Dictionary<string, object> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, object>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, object>();
                dict.Add(valKey, value);
            }

            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = absoluteExpiration;

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
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
            rw.AcquireWriterLock(500);
            Dictionary<string, T> dict = null;
            if (Cache.Contains(key))
            {
                dict = (Dictionary<string, T>)Cache[key];
                dict[valKey] = value;
            }
            else
            {
                dict = new Dictionary<string, T>();
                dict.Add(valKey, value);
            }

            var policy = new CacheItemPolicy();
            policy.Priority = CacheItemPriority.Default;
            policy.AbsoluteExpiration = absoluteExpiration;

            Cache.Add(new CacheItem(key, dict), policy);
            rw.ReleaseWriterLock();
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
        public T Get<T>(string key, string valKey)
        {
            rw.AcquireReaderLock(100);
            T t = default(T);
            if (Cache.Contains(key))
            {
                Dictionary<string, T> dict = (Dictionary<string, T>)Cache[key];
                if (dict != null && dict.ContainsKey(valKey))
                    t = dict[valKey];
            }
            rw.ReleaseReaderLock();
            return t;
        }

        /// <summary>
        /// 从缓存中读取对象。
        /// </summary>
        /// <param name="key">缓存的键值，该值通常是使用缓存机制的方法的名称。</param>
        /// <param name="valKey">缓存值的键值，该值通常是由使用缓存机制的方法的参数值所产生。</param>
        /// <returns>被缓存的对象。</returns>
        public object Get(string key, string valKey)
        {
            rw.AcquireReaderLock(100);
            object obj = null;
            if (Cache.Contains(key))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)Cache[key];
                if (dict != null && dict.ContainsKey(valKey))
                    obj = dict[valKey];
            }
            rw.ReleaseReaderLock();
            return obj;
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
            rw.AcquireWriterLock(500);
            if (Cache.Contains(key))
            {
                Dictionary<string, object> dict = (Dictionary<string, object>)Cache[key];
                if (dict != null && dict.ContainsKey(valKey))
                {
                    var policy = new CacheItemPolicy();
                    policy.Priority = CacheItemPriority.NotRemovable;
                    dict.Remove(valKey);
                    Cache.Add(new CacheItem(key, dict), policy);
                }
            }
            rw.ReleaseWriterLock();
        }

        /// <summary>
        /// 获取一个值，该值表示拥有指定键值和缓存值键的缓存是否存在。
        /// </summary>
        /// <param name="key">指定的键值。</param>
        /// <param name="valKey">缓存值键。</param>
        /// <returns>如果缓存存在，则返回true，否则返回false。</returns>
        public bool Exists(string key, string valKey)
        {
            return Cache.Contains(key) &&
                ((Dictionary<string, object>)Cache[key]).ContainsKey(valKey);
        }

        #endregion

        #endregion
    }
}
