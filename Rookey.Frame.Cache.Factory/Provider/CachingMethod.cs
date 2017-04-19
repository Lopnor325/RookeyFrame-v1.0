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

namespace Rookey.Frame.Cache.Factory.Provider
{
    /// <summary>
    /// 表示用于Caching特性的缓存方式。
    /// </summary>
    public enum CachingMethod
    {
        /// <summary>
        /// 表示需要从缓存中获取对象。如果缓存中不存在所需的对象，系统则会调用实际的方法获取对象，
        /// 然后将获得的结果添加到缓存中。
        /// </summary>
        Get,
        /// <summary>
        /// 表示需要将对象存入缓存。此方式会调用实际方法以获取对象，然后将获得的结果添加到缓存中，
        /// 并直接返回方法的调用结果。
        /// </summary>
        Set,
        /// <summary>
        /// 表示需要将对象从缓存中移除。
        /// </summary>
        Remove
    }
}
