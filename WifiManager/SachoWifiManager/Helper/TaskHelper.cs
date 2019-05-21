using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SachoWifiManager.Helper
{
    /// <summary>
    /// 执行任务并检测任务线程是否需要取消
    /// </summary>
    public class TaskHelper:IDisposable
    {
        public TaskHelper(int runTaskDelayMs = 500)
        {
            RunTaskDelayMs = runTaskDelayMs;
            Task.Run(new Action(()=> { Check(); }));
            swatch.Start();
            
        }

        int RunTaskDelayMs = 500;
        Stopwatch swatch = new Stopwatch();
        static object Locker = new object();

        Action _Action = null;
        Action Action
        {
            get
            {
                return _Action;
            }
            set
            {
                lock (Locker)
                {
                    _Action = value;
                }
            }
        }

        Action _CallBackAction = null;
        Action CallBackAction
        {
            get { return _CallBackAction; }
            set
            {
                _CallBackAction = value;
            }
        }

        CancellationTokenSource source = new CancellationTokenSource();

        bool IsRunning = false;


        /// <summary>
        /// 执行命令
        /// 1 如果执行命令时被取消则执行取消
        /// 2 如果重复执行则当前执行操作被取消，再次重新执行
        /// </summary>
        /// <param name="action"></param>
        void RunMethodWithCancelToken(Action action)
        {
            bool IsFinished = false;
            bool IsNeedReRun = IsRunning;
            try
            {
                CancellationToken token = source.Token;
                token.ThrowIfCancellationRequested();
                if (IsRunning)
                {
                    source.Cancel();
                    Thread.Sleep(100);
                }
                else
                {
                    IsRunning = true;

                    var tasks = new ConcurrentBag<Task>();

                    Action actionWithToken = new Action(() =>
                    {
                        action();
                        IsFinished = true;
                        source.Cancel();
                    });

                    Task task = Task.Factory.StartNew(actionWithToken, token);
                    tasks.Add(task);

                    Task taskToken = Task.Factory.StartNew(() =>
                    {
                        while (true)
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }
                            Thread.Sleep(10);
                        }
                    }, token);
                    tasks.Add(taskToken);
                    Task.WaitAll(tasks.ToArray());
                   
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                source = new CancellationTokenSource();
                IsRunning = false;
                if (IsFinished)
                {
                    Action = null;
                }
                if (CallBackAction != null)
                {
                    CallBackAction();
                }
                if (IsNeedReRun)
                {
                    Task.Run(new Action(() => { RunMethodWithCancelToken(action); }));
                }

            }
        }

        /// <summary>
        /// 发送执行
        /// </summary>
        /// <param name="action">要延迟执行的动作</param>
        /// <param name="callBackAction">执行完毕/执行取消后发生的动作</param>
        public void SendAction(Action action,Action callBackAction = null)
        {
            CallBackAction = callBackAction;
            Action = action;
            swatch.Restart();
        }

        void Check()
        {
            while (true)
            {
                Thread.Sleep(50);
                if ( Action != null)
                {
                    if (swatch.ElapsedMilliseconds > RunTaskDelayMs)
                    {
                        RunMethodWithCancelToken(Action);
                    }
                }
            }
        }

        public void Dispose()
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }
    }
}
