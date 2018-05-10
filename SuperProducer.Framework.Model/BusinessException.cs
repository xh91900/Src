using SuperProducer.Core.Log;
using SuperProducer.Core.Utility;
using System;

namespace SuperProducer.Framework.Model
{
    public class BusinessException : Exception, ILogService
    {
        public BusinessException(LoggerRank rank, BusinessExceptionLog exp)
            : base(string.IsNullOrEmpty(exp.Content) ? null : exp.Content)
        {
            this.Rank = rank;
            this.MessageObject = exp;
        }

        public LoggerRank Rank { get; set; }

        /// <summary>
        /// BusinessExceptionLog对象
        /// </summary>
        public object MessageObject { get; set; }
    }

    public class BusinessExceptionLog : ModelBase, IExceptionInfo
    {
        public byte Rank { get; set; }

        public string Class { get; set; }

        public string Method { get; set; }

        public string Arguments { get; set; }

        public string Content { get; set; }

        public bool IsNotify { get; set; }

        public byte NotifyType { get; set; }
    }
}
