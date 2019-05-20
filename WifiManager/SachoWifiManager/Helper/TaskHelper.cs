using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        CancellationTokenSource source = new CancellationTokenSource();

        bool IsRunning = false;

        public void RunMethodWithToken(Action action)
        {
            bool IsNeedReRun = IsRunning;
            try
            {
                CancellationToken token = source.Token;
                token.ThrowIfCancellationRequested();
                if (IsNeedReRun)
                {
                    source.Cancel();
                    Thread.Sleep(100);
                    return;
                }
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
    }
}
