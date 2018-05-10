using SuperProducer.Core.Utility;
using System;

namespace SuperProducer.Core.Service
{
    internal class InternalExceptionInfo : IExceptionInfo
    {
        public string Class { get; set; }

        public string Method { get; set; }

        public string Arguments { get; set; }

        public string Content { get; set; }
    }
}
