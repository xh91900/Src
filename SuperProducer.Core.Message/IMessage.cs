using System;

namespace SuperProducer.Core.Message
{
    public interface ISmsMessage
    {
        bool Send();
    }

    public interface IEmailMessage
    {
        bool Send();
    }
}
