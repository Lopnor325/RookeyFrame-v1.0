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
using System.Threading.Tasks;
using System.Timers;

namespace Rookey.Frame.AutoProcess
{
    /// <summary>
    /// 委托（用于异步处理任务）
    /// </summary>
    /// <param name="parameter">任务参数</param>
    /// <returns>是否执行成功</returns>
    public delegate bool TaskHandle(object parameter);

    /// <summary>
    /// 自动处理中心任务参数
    /// </summary>
    public class BackgroundTask
    {
        #region 私有成员
        private bool _IsOnce = true; //是否执行一次
        private bool _IsExecuteNow = false; //是否立即执行，对于重复执行生效
        private int _Interval = 86400; //重复执行时的执行间隔，秒为单位，默认为一天，如果只执行一次并且想马上执行将该值设置为0
        private bool _IsAbortExcute = false; //终止执行（设置为true时，系统将不会执行Timer的事件）
        private TaskHandle _ExecuteMethod = null; //任务执行方法
        private object _Parameter = null; //任务执行方法参数
        private DateTime? _ExecuteDateTime = null;
        #endregion

        #region 属性

        /// <summary>
        /// 是否已经终止
        /// </summary>
        public bool IsAbortExcute
        {
            get { return this._IsAbortExcute; }
        }

        /// <summary>
        /// 执行的方法
        /// </summary>
        public TaskHandle ExecuteMethod
        {
            get { return this._ExecuteMethod; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 任务参数构造函数
        /// </summary>
        /// <param name="executeMethod">执行方法</param>
        /// <param name="parameter">方法参数</param>
        /// <param name="isOnce">是否执行一次</param>
        /// <param name="interval">执行间隔（秒），默认为24小时</param>
        /// <param name="isExecuteNow">是否立即执行</param>
        public BackgroundTask(TaskHandle executeMethod, object parameter = null, bool isOnce = true, int interval = 86400, bool isExecuteNow = false, DateTime? ExecuteDateTime = null)
        {
            this._ExecuteMethod = executeMethod;
            this._Parameter = parameter;
            this._IsOnce = isOnce;
            this._Interval = interval;
            if (interval < 0)
            {
                this._Interval = 1;
            }
            this._IsExecuteNow = isExecuteNow;
            this._ExecuteDateTime = ExecuteDateTime;
        }

        #endregion

        /// <summary>
        /// 开始执行任务
        /// </summary>
        /// <returns></returns>
        public void Execute()
        {
            if (!AutoProcessTask.IsStart) return;
            int interval = _Interval * 1000;
            if (interval == 0) interval = 1;
            /*
             Timer是提供以指定的时间间隔执行某方法的这样一种机制，
             * 即如果想要实现一个定时发送数据，比如每隔3s中发送一次心跳报文，或者执行某个指定的方法，
             * 都可以考虑用Timer类来实现，
             * 不过要提出的是Timer类一边用来做一些比较简单又不耗时间的操作。
             * 据说是因为它执行的任务仍然在主线程里面
             */
            if (this._ExecuteDateTime.HasValue) //按执行时间时每秒跑一次
                interval = 1000;
            Timer _timer = new Timer(interval);
            _timer.AutoReset = !_IsOnce;
            _timer.Enabled = true;
            if (_IsExecuteNow && !_IsOnce) //立即执行
            {
                _ExecuteMethod.BeginInvoke(_Parameter, null, null);
            }
            _timer.Elapsed += new ElapsedEventHandler(Start);
            if (_IsOnce && _timer != null)
            {
                _timer.Enabled = false;
                _timer.Elapsed -= new ElapsedEventHandler(Start);
                _timer = null;
            }
        }

        /// <summary>
        /// 开始执行Timer具体方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start(object sender, ElapsedEventArgs e)
        {
            if (this._ExecuteDateTime.HasValue)
            {
                DateTime now = DateTime.Now;
                DateTime executeTime = this._ExecuteDateTime.Value;
                if (executeTime.Hour == now.Hour && executeTime.Minute == now.Minute && executeTime.Second == now.Second)
                {
                    _ExecuteMethod.BeginInvoke(_Parameter, null, null);
                }
                else
                {
                    (sender as Timer).Interval = 1000;
                }
            }
            else
            {
                _ExecuteMethod.BeginInvoke(_Parameter, null, null);
            }
        }
    }
}
