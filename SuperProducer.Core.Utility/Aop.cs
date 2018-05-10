using System;

namespace SuperProducer.Core.Utility
{
    /// <summary>
    /// 简单实现[慎用]
    /// </summary>
    public class Aop
    {
        public TimeSpan ExecElapsed { get; set; }

        public Action<object> begin { get; set; }

        public Action<object> end { get; set; }

        public Action<Exception, object> exception { get; set; }

        public Action<object, TimeSpan> complete { get; set; }

        public Aop() : this(null, null, null, null) { }

        public Aop(Action<object> _begin) : this(_begin, null, null, null) { }

        public Aop(Action<object> _begin, Action<object> _end) : this(_begin, _end, null, null) { }

        public Aop(Action<object> _begin, Action<object> _end, Action<object, TimeSpan> _complete) : this(_begin, _end, _complete, null) { }

        /// <summary>
        /// Aop实例化
        /// </summary>
        /// <param name="_begin">执行目标方法前执行的方法(参数为目标方法的传入参数)</param>
        /// <param name="_end">执行目标方法后执行的方法(参数为目标方法的返回值)</param>
        /// <param name="_complete">目标方法执行完成后执行的方法(参数1为目标方法的返回值,参数2为执行计时)</param>
        /// <param name="_exception">begin,fn,end执行异常时执行的方法</param>
        public Aop(Action<object> _begin, Action<object> _end, Action<object, TimeSpan> _complete, Action<Exception, object> _exception)
        {
            this.begin = _begin;
            this.end = _end;
            this.complete = _complete;
            this.exception = _exception;
        }

        public TimeSpan Intercept(Func<object, object> fn, object args)
        {
            if (fn != null)
            {
                var monitor = TaskHelper.GetStopwatch(true);

                object result = null;
                try
                {
                    if (begin != null) begin(args);
                    result = fn.Invoke(args);
                    if (end != null) end(result);
                }
                catch (Exception ex)
                {
                    if (exception != null) exception(ex, args);
                }
                finally
                {
                    monitor.Stop();

                    if (complete != null) complete(result, monitor.Elapsed);
                }
                return monitor.Elapsed;
            }
            return TimeSpan.MinValue;
        }
    }
}
