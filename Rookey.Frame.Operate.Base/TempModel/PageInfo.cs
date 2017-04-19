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
using System.Web;
using Rookey.Frame.Common;

namespace Rookey.Frame.Operate.Base.TempModel
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {
        #region 构造函数

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public PageInfo()
        {
            totalCount = 0;
            sortname = "AutoIncrmId";
            sortorder = "desc";
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pageIndex">页编号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="orderFields">排序字段</param>
        /// <param name="isdescs">是否降序排列</param>
        public PageInfo(int pageIndex, int pageSize, List<string> orderFields, List<bool> isdescs)
            : this()
        {
            page = pageIndex < 1 ? 1 : pageIndex;
            pagesize = pageSize < 1 ? 10 : pageSize;
            sortname = orderFields != null && orderFields.Count > 0 ? string.Join(",", orderFields) : null;
            sortorder = isdescs != null && isdescs.Count > 0 ? string.Join(",", isdescs.Select(x => x ? "desc" : "asc")) : null;
        }
        #endregion

        #region 分页数据

        /// <summary>
        /// 页号
        /// </summary>
        public int page { get; set; }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int pagesize { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string sortname { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string sortorder { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public long totalCount { get; set; }

        #endregion

        #region 关键字

        /// <summary>
        /// 页码起始值，默认为1
        /// </summary>
        public static int pageIndexStartNo = 1;

        /// <summary>
        /// 页码关键字
        /// </summary>
        public static string pageIndexKeyWord = "page";

        /// <summary>
        /// 页记录数关键字
        /// </summary>
        public static string pageSizeKeyWord = "rows";

        /// <summary>
        /// 排序字段关键字
        /// </summary>
        public static string sortFieldKeyWord = "sort";

        /// <summary>
        /// 排序方式关键字
        /// </summary>
        public static string sortOrderKeyWord = "order";

        /// <summary>
        /// 总记录数关键字，分页返回JSON结构中用到
        /// </summary>
        public static string totalRecordKeyWord = "total";

        /// <summary>
        /// 分页数据关键字，分页返回JSON结构中用到
        /// </summary>
        public static string pageDataKeyWord = "rows";

        #endregion

        #region 静态方法

        /// <summary>
        /// 获取默认分页信息
        /// </summary>
        /// <returns></returns>
        public static PageInfo GetDefaultPageInfo()
        {
            return new PageInfo()
            {
                //分页数据初始化
                page = 1,
                pagesize = 10
            };
        }

        /// <summary>
        /// 根据request获取分页信息
        /// </summary>
        /// <param name="request">request对象</param>
        /// <param name="defaultSortField">默认排序字段</param>
        /// <returns></returns>
        public static PageInfo GetPageInfo(HttpRequestBase request, string defaultSortField = "AutoIncrmId")
        {
            int _page = request[PageInfo.pageIndexKeyWord].ObjToInt();
            if (PageInfo.pageIndexStartNo == 0)
            {
                _page += 1;
            }
            else
            {
                if (_page < 1) _page = 1;
            }
            int _pagesize = request[PageInfo.pageSizeKeyWord].ObjToInt();
            if (_pagesize <= 0) _pagesize = 10;
            string sortname = request[PageInfo.sortFieldKeyWord].ObjToStr();
            string sortorder = request[PageInfo.sortOrderKeyWord].ObjToStr();
            PageInfo pageInfo = new PageInfo()
            {
                page = _page,
                pagesize = _pagesize
            };
            if (!string.IsNullOrEmpty(defaultSortField))
                pageInfo.sortname = defaultSortField;
            if (!string.IsNullOrEmpty(sortname))
                pageInfo.sortname = sortname;
            if (!string.IsNullOrEmpty(sortorder))
                pageInfo.sortorder = sortorder;
            return pageInfo;
        }
        #endregion
    }
}
