using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rookey.Frame.Common;
using Rookey.Frame.Bridge;
using Rookey.Frame.Model.Sys;
using System.Web;

namespace Rookey.Frame.Operate.Base
{
    /// <summary>
    /// 初始化工厂类
    /// </summary>
    public abstract class InitFactory
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        /// <returns></returns>
        public static InitFactory GetInstance()
        {
            InitFactory factory = null;
            Type type = BridgeObject.GetCustomerOperateHandleTypes().Where(x => x.BaseType == typeof(InitFactory)).FirstOrDefault();
            if (type != null)
            {
                object obj = Activator.CreateInstance(type);
                return obj as InitFactory;
            }
            return factory;
        }

        /// <summary>
        /// 应用程序启动事件
        /// </summary>
        /// <param name="application">应用程序对象</param>
        public abstract void App_Start(HttpApplicationState application);

        /// <summary>
        /// 应用程序结束事件
        /// </summary>
        /// <param name="application">应用程序对象</param>
        public abstract void App_End(HttpApplication application);

        /// <summary>
        /// 自定义初始化，包括菜单、模块、字段、字典等数据初始化
        /// </summary>
        public abstract void CustomerInit();
    }
}
