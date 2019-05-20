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
        public static void RunMethodWithToken(Action action,ref CancellationTokenSource source)
        {
            try
            {


                CancellationToken token = source.Token;
                token.ThrowIfCancellationRequested();
                var tasks = new ConcurrentBag<Task>();

                //Action actionWithToken = new Action(() =>
                //{
                //    action();
                //    source.Cancel();
                //});

                Task task = Task.Factory.StartNew(new Action<object>(RunActionAndRunCancel), new CancelTaskPara { Action=action, CancellationTokenSource=source }as object, token);
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
                source = new CancellationTokenSource();
            }
        }

        static void RunActionAndRunCancel(object ctP)
        {
            CancelTaskPara ctp = (CancelTaskPara)ctP;
            Action action = ctp.Action;
            CancellationTokenSource source = ctp.CancellationTokenSource;
            action();
            source.Cancel();
        }


    }


    public class CancelTaskPara
    {
        public Action Action { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
    }

}
