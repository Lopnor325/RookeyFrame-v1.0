using Rookey.Frame.Common;
using ServiceStack.Caching;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rookey.Frame.Cache.Factory.Provider
{
    /// <summary>
    /// Redis缓存提供器
    /// </summary>
    public class RedisCacheProvider : ICacheProvider
    {
        #region 构造函数
        private ICacheClient cacheClient;
        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisCacheProvider()
        {
            if (cacheClient != null)
                cacheClient.Dispose();
            cacheClient = new RedisClient();
            try
            {
                string path = WebHelper.GetConfigFilePath("rediscache.config");
                RedisConfigInfo configInfo = (RedisConfigInfo)XmlHelper.DeserializeFromXML(typeof(RedisConfigInfo), path);
                if (configInfo != null)
                {
                    string pwd = string.IsNullOrEmpty(configInfo.Pwd) ? null : configInfo.Pwd;
                    RedisEndpoint point = new RedisEndpoint(configInfo.Host, configInfo.Port, pwd, configInfo.InitalDB);
                    cacheClient = new RedisClient(point);
                }
            }
            catch { }
            cacheClient.FlushAll();
        }

        #endregion

        #region 临时类
        /// <summary>
        /// 临时类
        /// </summary>
        class TempRedisClass<T> where T : class
        {
            ICacheClient _client = null;
            public TempRedisClass(ICacheClient client)
            {
                _client = client;
            }
            public void Set(string key, object value)
            {
                try
                {
                    _client.Set<T>(key, value as T);
                }
                catch { }
            }
            public void SetTimeoutCache(string key, object value, TimeSpan slidingExpiration)
            {
                try
                {
                    _client.Set<T>(key, value as T, slidingExpiration);
                }
                catch { }
            }
            /// <summary>
            /// 设置缓存值
            /// </summary>
            /// <param name="key">key</param>
            /// <param name="value">value</param>
            /// <param name="absoluteExpiration">过期时间</param>
            public void SetDateTimeCache(string key, object value, DateTime absoluteExpiration)
            {
                try
                {
                    _client.Set<T>(key, value as T, absoluteExpiration);
                }
                catch { }
            }
        }
        #endregion

        #region 私有函数

        /// <summary>
        /// 执行反射方法
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        private object ExecuteReflectMethod(Type type, string methodName, object[] args)
        {
            Type tempType = typeof(TempRedisClass<>);
            Type relectType = tempType.MakeGenericType(new Type[] { type });
            //实例化对象
            object obj = Activator.CreateInstance(relectType, new object[] { cacheClient });
            MethodInfo method = relectType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
            //反射执行方法
            return method.Invoke(obj, args);
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        private void SetCache(string key, object value)
        {
            ExecuteReflectMethod(value.GetType(), "Set", new object[] { key, value });
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="slidingExpiration">过期时间长</param>
        private void SetTimeoutCache(string key, object value, TimeSpan slidingExpiration)
        {
            ExecuteReflectMethod(value.GetType(), "SetTimeoutCache", new object[] { key, value, slidingExpiration });
        }

        /// <summary>
        /// 设置缓存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="absoluteExpiration">过期时间</param>
        private void SetDateTimeCache(string key, object value, DateTime absoluteExpiration)
        {
            ExecuteReflectMethod(value.GetType(), "SetDateTimeCache", new object[] { key, value, absoluteExpiration });
        }

        #endregion

        #region 单键值
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add(string key, object value)
        {
            SetCache(key, value);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Add<T>(string key, T value)
        {
            cacheClient.Set<T>(key, value);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            SetTimeoutCache(key, value, slidingExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            cacheClient.Set<T>(key, value, slidingExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            SetDateTimeCache(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            cacheClient.Set<T>(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            SetCache(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set<T>(string key, T value)
        {
            cacheClient.Set<T>(key, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            SetTimeoutCache(key, value, slidingExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Set<T>(string key, T value, TimeSpan slidingExpiration)
        {
            cacheClient.Set<T>(key, value, slidingExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Set(string key, object value, DateTime absoluteExpiration)
        {
            SetDateTimeCache(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Set<T>(string key, T value, DateTime absoluteExpiration)
        {
            cacheClient.Set<T>(key, value, absoluteExpiration);
        }

        /// <summary>
        /// 取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            return cacheClient.Get<T>(key);
        }

        /// <summary>
        /// 取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public object Get(string key)
        {
            return cacheClient.Get<object>(key);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key">键</param>
        public void Remove(string key)
        {
            cacheClient.Remove(key);
        }

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            object obj = Get(key);
            return obj != null;
        }
        
        /// <summary>
        /// 刷新缓存
        /// </summary>
        public void FlushAll()
        {
            cacheClient.FlushAll();
        }
        #endregion

        #region 双键值
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        public void Add(string key, string valKey, object value)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, object>();
                tempDict.Add(valKey, value);
            }
            SetCache(key, tempDict);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        public void Add<T>(string key, string valKey, T value)
        {
            Dictionary<string, T> tempDict = cacheClient.Get<Dictionary<string, T>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, T>();
                tempDict.Add(valKey, value);
            }
            cacheClient.Set<Dictionary<string, T>>(key, tempDict);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Add(string key, string valKey, object value, TimeSpan slidingExpiration)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, object>();
                tempDict.Add(valKey, value);
            }
            cacheClient.Set<Dictionary<string, object>>(key, tempDict, slidingExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Add<T>(string key, string valKey, T value, TimeSpan slidingExpiration)
        {
            Dictionary<string, T> tempDict = cacheClient.Get<Dictionary<string, T>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, T>();
                tempDict.Add(valKey, value);
            }
            cacheClient.Set<Dictionary<string, T>>(key, tempDict, slidingExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add(string key, string valKey, object value, DateTime absoluteExpiration)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, object>();
                tempDict.Add(valKey, value);
            }
            cacheClient.Set<Dictionary<string, object>>(key, tempDict, absoluteExpiration);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Add<T>(string key, string valKey, T value, DateTime absoluteExpiration)
        {
            Dictionary<string, T> tempDict = cacheClient.Get<Dictionary<string, T>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict[valKey] = value;
            }
            else
            {
                tempDict = new Dictionary<string, T>();
                tempDict.Add(valKey, value);
            }
            cacheClient.Set<Dictionary<string, T>>(key, tempDict, absoluteExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        public void Set(string key, string valKey, object value)
        {
            Add(key, valKey, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        public void Set<T>(string key, string valKey, T value)
        {
            Add<T>(key, valKey, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Set(string key, string valKey, object value, TimeSpan slidingExpiration)
        {
            Add(key, valKey, value);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="slidingExpiration">过期时间</param>
        public void Set<T>(string key, string valKey, T value, TimeSpan slidingExpiration)
        {
            Add<T>(key, valKey, value, slidingExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Set(string key, string valKey, object value, DateTime absoluteExpiration)
        {
            Add(key, valKey, value, absoluteExpiration);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">过期时间</param>
        public void Set<T>(string key, string valKey, T value, DateTime absoluteExpiration)
        {
            Add<T>(key, valKey, value, absoluteExpiration);
        }

        /// <summary>
        /// 取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <returns></returns>
        public T Get<T>(string key, string valKey)
        {
            Dictionary<string, T> tempDict = cacheClient.Get<Dictionary<string, T>>(key);
            if (tempDict != null) //缓存存在
            {
                return tempDict[valKey];
            }
            return default(T);
        }

        /// <summary>
        /// 取缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <returns></returns>
        public object Get(string key, string valKey)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                return tempDict[valKey];
            }
            return null;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        public void Remove(string key, string valKey)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                tempDict.Remove(valKey);
            }
        }

        /// <summary>
        /// 缓存是否存在
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="valKey">键</param>
        /// <returns></returns>
        public bool Exists(string key, string valKey)
        {
            Dictionary<string, object> tempDict = cacheClient.Get<Dictionary<string, object>>(key);
            if (tempDict != null) //缓存存在
            {
                return tempDict.ContainsKey(valKey);
            }
            return false;
        }
        #endregion
    }
}
