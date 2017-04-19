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
using System.Linq;
using System.Reflection;
using System.Text;

namespace Rookey.Frame.Common
{
     /**//// <summary>
    /// 继承IComparer<T>接口，实现同一自定义类型　对象比较
    /// </summary>
    /// <typeparam name="T">T为泛用类型</typeparam>
    public class SortComparer<T> : IComparer<T>
    {
        private Type type = null;
        private ReverserInfo info;

        /**//// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">进行比较的类类型</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortComparer(Type type, string name, ReverserInfo.Direction direction)
        {
            this.type = type;
            this.info.name = name;
            if (direction != ReverserInfo.Direction.ASC)
                this.info.direction = direction;
        }

        /**//// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="className">进行比较的类名称</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortComparer(string className, string name, ReverserInfo.Direction direction) {
            try
            {
                this.type = Type.GetType(className, true);
                this.info.name = name;
                this.info.direction = direction;
            }
            catch (Exception e){
                throw new Exception(e.Message);
            }
        }

        /**//// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">进行比较的类型的实例</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortComparer(T t, string name, ReverserInfo.Direction direction)
        {
            this.type = t.GetType();
            this.info.name = name;
            this.info.direction = direction;
        }

        //必须！实现IComparer<T>的比较方法。
        int IComparer<T>.Compare(T t1, T t2)
        {
            object x = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t1, null);
            object y = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t2, null);
            if (this.info.direction != ReverserInfo.Direction.ASC)
                Swap(ref x, ref y);
            return (new CaseInsensitiveComparer()).Compare(x, y);
        }

        //交换操作数
        private void Swap(ref object x, ref object y)
        {
            object temp = null;
            temp = x;
            x = y;
            y = temp;
        }
    }

    /**//// <summary>
    /// 对象比较时使用的信息类
    /// </summary>
    public struct ReverserInfo
    {
        /**//// <summary>
        /// 比较的方向，如下：
        /// ASC：升序
        /// DESC：降序
        /// </summary>
        public enum Direction
        {
            ASC = 0,
            DESC,
        };

        public enum Target
        {
            CUSTOMER = 0,
            FORM,
            FIELD,
            SERVER,
        };

        public string name;
        public Direction direction;
        public Target target;
    }
}
