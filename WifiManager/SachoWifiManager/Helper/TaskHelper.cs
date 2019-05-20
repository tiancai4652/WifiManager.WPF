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
    public class TaskHelper
    {
        public TaskHelper()
        {
            Task.Run(new Action(()=> { Check(); }));
            swatch.Start();
        }

        int RunTaskDelayMs = 500;
        Stopwatch swatch = new Stopwatch();
        Action Action = null;

        CancellationTokenSource source = new CancellationTokenSource();

        bool IsRunning = false;

        void RunMethod(Action action)
        {
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
                if (IsNeedReRun)
                {
                    Task.Run(new Action(() => { RunMethodWithToken(action); }));
                }

            }
        }

        public void RunMethodWithToken(Action action)
        {
            Action = action;
            swatch.Restart();
        }

        void Check()
        {
            while (true)
            {
                Thread.Sleep(100);
                if ( Action != null)
                {
                    if (swatch.ElapsedMilliseconds > RunTaskDelayMs)
                    {
                        RunMethod(Action);
                    }
                }
            }
        }
    }
}
