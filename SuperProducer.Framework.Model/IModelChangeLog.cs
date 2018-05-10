using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SuperProducer.Framework.Model
{
    public interface IModelChangeLog
    {
        void WriteLog(string userName, string moduleName, string tableName, long modelID, ModelBase modelValue, string eventType, string remark);

        void Dispose();
    }
}
