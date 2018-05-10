using System;
using System.Text;

namespace SuperProducer.Core.Config
{
    public interface IConfigService
    {
        string GetConfig(string name);

        void SaveConfig(string name, string content);
    }
}
