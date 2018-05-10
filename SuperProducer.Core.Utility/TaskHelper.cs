using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SuperProducer.Core.Utility
{
    public class TaskHelper
    {
        /// <summary>
        /// 开启新任务[单输出参数]
        /// </summary>
        public static T Start<T>(out bool execComplete, int waitSec, Func<T> fn)
        {
            execComplete = false;
            try
            {
                Task<T> asyncTask = new TaskFactory<T>().StartNew(fn);
                execComplete = asyncTask.Wait(waitSec * 1000);
                if (execComplete)
                    return asyncTask.Result;
            }
            catch { }
            return default(T);
        }

        /// <summary>
        /// 延迟X秒后继续执行[当前线程]
        /// </summary>
        public static void DelaySecond(int sec)
        {
            if (sec > 0)
            {
                int delaySec = sec;
                while (delaySec > 0)
                {
                    delaySec -= 1;
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// 计时执行
        /// </summary>
        public static TimeSpan TimeStart(Action<object> fn, object args)
        {
            if (fn != null)
            {
                var monitor = GetStopwatch(true);
                fn.Invoke(args);
                monitor.Stop();
                return monitor.Elapsed;
            }
            return TimeSpan.MinValue;
        }

        /// <summary>
        /// 计时执行
        /// </summary>
        public static TimeSpan TimeStartAsync(Action<object> fn, object args)
        {
            if (fn != null)
            {
                var monitor = GetStopwatch(true);
                var task = new TaskFactory().StartNew(fn, args);
                task.Wait();
                monitor.Stop();
                return monitor.Elapsed;
            }
            return TimeSpan.MinValue;
        }

        /// <summary>
        /// 获取一个时间测量对象
        /// </summary>
        public static Stopwatch GetStopwatch(bool isStart = false)
        {
            var monitor = new Stopwatch();
            if (isStart)
                monitor.Start();
            return monitor;
        }
    }
}
