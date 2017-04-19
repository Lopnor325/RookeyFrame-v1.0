/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using Rookey.Frame.Cache.Factory.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rookey.Frame.Cache.Factory
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public static class CacheFactory
    {
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();

        /// <summary>
        /// 创建缓存实例
        /// </summary>
        /// <param name="providerType">缓存类型</param>
        /// <returns></returns>
        public static ICacheProvider GetCacheInstance(CacheProviderType providerType)
        {
            ICacheProvider cacheProvider = new MemoryCacheProvider();
            switch (providerType)
            {
                case CacheProviderType.LOCALMEMORYCACHE:
                    cacheProvider = new MemoryCacheProvider();
                    break;
                case CacheProviderType.MEMCACHEDCACHE:
                    cacheProvider = new MemcachedCacheProvider();
                    break;
                case CacheProviderType.ServiceStackREDIS:
                    cacheProvider = new RedisCacheProvider();
                    break;
            }
            return cacheProvider;
        }
    }
}
