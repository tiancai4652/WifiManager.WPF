using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SachoWifiManager.Helper
{
    public class TaskHelper
    {
        public static void RunMethodWithToken(Action action, CancellationTokenSource source)
        {
            try
            {
                CancellationToken token = source.Token;
                token.ThrowIfCancellationRequested();
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
                        Thread.Sleep(100);
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

            }



        }
    }
}
