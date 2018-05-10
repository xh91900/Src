using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Core.Utility
{
    public interface IExceptionInfo
    {
        string Class { get; set; }

        string Method { get; set; }

        string Arguments { get; set; }

        string Content { get; set; }
    }
}
