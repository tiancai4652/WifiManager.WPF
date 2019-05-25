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
    public class TaskHelper : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="runTaskDelayMs"></param>
        public TaskHelper(int runTaskDelayMs = 500)
        {
            RunTaskDelayMs = runTaskDelayMs;
            Task.Run(new Action(() => { EnabledCheckThread(); }));
            swatch.Start();
            ActionStack = new ConcurrentStack<Action>();
        }

        int RunTaskDelayMs = 1000;
        Stopwatch swatch = new Stopwatch();
        ConcurrentStack<Action> ActionStack { get; set; }



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
                source.Token.ThrowIfCancellationRequested();
                IsRunning = false;
                //if (IsFinished)
                //{
                //    ActionStack.Clear();
                //}
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
        public void SendAction(Action action, Action callBackAction = null)
        {
            source.Cancel();
            source = new CancellationTokenSource();
            source.Token.ThrowIfCancellationRequested();
            Thread.Sleep(50);
            CallBackAction = callBackAction;
            ActionStack.Push(action);
            swatch.Restart();
        }

        void EnabledCheckThread()
        {
            while (true)
            {
                Thread.Sleep(50);
                if (ActionStack.Count > 0)
                {
                    if (swatch.ElapsedMilliseconds > RunTaskDelayMs)
                    {
                        Action ac = null;
                        int count = 3;
                        while (count > 0)
                        {
                            if (!ActionStack.TryPop(out ac))
                            {
                                count--;
                            }
                            else
                            {
                                ActionStack.Clear();
                                break;
                            }
                        }
                        if (ac != null)
                        {
                            RunMethodWithCancelToken(ac);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            try
            {
                source.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
