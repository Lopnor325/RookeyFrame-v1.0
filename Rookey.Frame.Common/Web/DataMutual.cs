/*----------------------------------------------------------------
        // Copyright (C) Rookey
        // 版权所有
        // 开发者：rookey
        // Email：rookey@yeah.net
        // 
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Rookey.Frame.Common
{
    /// <summary>
    /// 数据交互类，从后台获取数据或向后台传递数据
    /// </summary>
    public class DataMutual
    {
        #region 成员属性
        private string _Url;
        /// <summary>
        /// 请求或发送的Url
        /// </summary>
        public string Url
        {
            get { return _Url; }
        }

        private string _method;
        private bool _isAsync = false;
        private int _timeout = 10000; //10秒

        /// <summary>
        /// 数据接收事件
        /// </summary>
        public event EventHandler<DataMutualEventArgs> DataReceivedEvent;

        /// <summary>
        /// 异常事件
        /// </summary>
        public event EventHandler<DataMutualEventArgs> DataErrorEvent;

        #endregion 成员属性

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="method">请求方式</param>
        /// <param name="isAsync">是否异步</param>
        /// <param name="timeout">超时时间，默认30s</param>
        public DataMutual(string url, string method = "POST", bool isAsync = false, int timeout = 30000)
        {
            this._Url = url;
            this._isAsync = isAsync;
            this._method = method;
            if (timeout > 1000)
            {
                this._timeout = timeout;
            }
        }
        #endregion 构造函数

        #region 公共方法
        /// <summary>
        /// 向服务器发送数据或请求数据
        /// </summary>
        /// <param name="data">向服务器发送的数据</param>
        public string Start(byte[] data)
        {
            Uri endPoint = new Uri(this._Url, UriKind.Absolute);
            WebRequest request = WebRequest.Create(endPoint);
            request.Method = _method;
            request.Timeout = _timeout; //超时时间
            request.ContentType = "application/x-www-form-urlencoded";
            if (data != null && data.Length > 0)
            {
                request.ContentLength = data.Length;
            }

            if (this._isAsync) //异步方式
            {
                #region 向服务器端POST信息
                request.BeginGetRequestStream(new AsyncCallback((asyncResult) =>
                {
                    WebRequest webRequest = (WebRequest)asyncResult.AsyncState;
                    try
                    {
                        if (data != null && data.Length > 0)
                        {
                            Stream postStream = webRequest.EndGetRequestStream(asyncResult);
                            postStream.Write(data, 0, data.Length);
                            postStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        if (this.DataErrorEvent != null)
                        {
                            this.DataErrorEvent(null, new DataMutualEventArgs(ex.Message, null));//调用数据异常事件
                        }
                    }
                    #region 向服务器端请求返回信息
                    //WebRequest类的一个特性就是可以异步请求页面。由于在给主机发送请求到接收响应之间有很长的延迟，因此，异步请求页面就显得比较重要。
                    //像WebClient.DownloadData()和WebRequest.GetResponse()等方法，在响应没有从服务器回来之前，是不会返回的。
                    //如果不希望在那段时间中应用程序处于等待状态，可以使用BeginGetResponse() 方法和 EndGetResponse()方法，
                    //BeginGetResponse()方法可以异步工作，并立即返回。在底层，运行库会异步管理一个后台线程，从服务器上接收响应。
                    //BeginGetResponse() 方法不返回WebResponse对象，而是返回一个执行IAsyncResult接口的对象。使用这个接口可以选择或等待可用的响应，然后调用EndGetResponse()搜集结果。
                    request.BeginGetResponse(new AsyncCallback((ar) =>
                    {
                        try
                        {
                            WebRequest wrq = (WebRequest)ar.AsyncState;
                            //对应 BeginGetResponse()方法，在此处调用EndGetResponse()搜集结果。
                            //WebResponse类代表从服务器获取的数据。调用EndGetResponse方法，实际上是把请求发送给Web服务器，创建一个Response对象。
                            WebResponse wrs = wrq.EndGetResponse(ar);
                            // 读取WebResponse对象中内含的从服务器端返回的结果数据流
                            using (Stream responseStream = wrs.GetResponseStream())
                            {
                                //StreamReader reader = new StreamReader(responseStream);
                                //string retGet = reader.ReadToEnd();//服务器返回的数据
                                if (this.DataReceivedEvent != null)
                                {
                                    this.DataReceivedEvent(null, new DataMutualEventArgs(null, responseStream));//调用数据接收事件
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            if (this.DataErrorEvent != null)
                            {
                                this.DataErrorEvent(null, new DataMutualEventArgs(ex.Message, null));//调用数据异常事件
                            }
                        }

                    }), webRequest);
                    #endregion

                }), request);
                return string.Empty;
                #endregion
            }
            else  //同步方式
            {
                #region 同步方式
                if (data != null && data.Length > 0)
                {
                    // 取得发向服务器的流 
                    System.IO.Stream newStream = request.GetRequestStream();
                    // 使用 POST 方法请求的时候，实际的参数通过请求的 Body 部分以流的形式传送 
                    newStream.Write(data, 0, data.Length);
                    // 完成后，关闭请求流. 
                    newStream.Close();
                }
                // GetResponse 方法才真的发送请求，等待服务器返回 
                try
                {
                    System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    if (this.DataReceivedEvent != null)
                    {
                        this.DataReceivedEvent(null, new DataMutualEventArgs(null, stream));//调用数据接收事件
                    }
                    TextReader tr = new StreamReader(stream);
                    return tr.ReadToEnd();
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                #endregion
            }
        }
        #endregion

    }

    /// <summary>
    /// 数据交互事件参数类
    /// </summary>
    public class DataMutualEventArgs : EventArgs
    {
        private string _ErrMsg;
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrMsg
        {
            get { return _ErrMsg; }
        }

        private Stream _ReceivedDataStream;
        /// <summary>
        /// 接收到的数据流
        /// </summary>
        public Stream ReceivedDataStream
        {
            get { return _ReceivedDataStream; }
        }

        public DataMutualEventArgs(string errMsg, Stream stream)
        {
            this._ErrMsg = errMsg;
            this._ReceivedDataStream = stream;
        }
    }
}
