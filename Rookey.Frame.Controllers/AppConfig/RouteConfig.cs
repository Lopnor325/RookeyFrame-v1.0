﻿using Rookey.Frame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Rookey.Frame.Controllers.AppConfig
{
    /// <summary>
    /// 路由配置
    /// </summary>
    public class RouteConfig
    {
        private const string NAMESPACE = "Rookey.Frame.Controllers";

        /// <summary>
        /// 注册路由
        /// </summary>
        /// <param name="routes">路由集合</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            // 忽略对 IM 路径的路由
            routes.IgnoreRoute("FileManage/{webform}");
            routes.IgnoreRoute("IM/{webform}");

            string defaultController = WebConfigHelper.GetAppSettingValue("DefaultController");
            string defaultAction = WebConfigHelper.GetAppSettingValue("DefaultAction");
            if (string.IsNullOrEmpty(defaultController))
                defaultController = "Page";
            if (string.IsNullOrEmpty(defaultAction))
                defaultAction = "Main";

            routes.MapRoute(
                 "index", // Route name
                 "", // URL with parameters
                 new { controller = defaultController, action = defaultAction, id = UrlParameter.Optional } // Parameter defaults
                 , namespaces: new[] { NAMESPACE }
            );
            routes.MapRoute(
                "Default1",
                "{controller}/{action}.html",
                new { controller = defaultController, action = defaultAction }
                , namespaces: new[] { NAMESPACE }
            );
            routes.MapRoute(
                "Default2", // Route name
                "{controller}/{action}/{id}.html", // URL with parameters
                new { controller = defaultController, action = defaultAction, id = UrlParameter.Optional } // Parameter defaults
                 , namespaces: new[] { NAMESPACE }
            );
        }
    }
}