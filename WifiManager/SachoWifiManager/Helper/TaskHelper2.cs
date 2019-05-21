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
    public class TaskHelper2
    {
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
                IsRunning = false;
                source = new CancellationTokenSource();
            }
        }


    }
}
