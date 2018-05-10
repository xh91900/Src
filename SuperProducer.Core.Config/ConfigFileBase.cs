using System;

namespace SuperProducer.Core.Config
{
    public abstract class ConfigFileBase
    {
        public int Id { get; set; }

        public virtual bool ClusteredByIndex { get { return false; } }

        internal virtual void Save() { }
    }
}
