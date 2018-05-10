using SuperProducer.Core.Utility;
using System.IO;
using System.Text;

namespace SuperProducer.Core.Config
{
    public class FileConfigService : IConfigService
    {
        private readonly string configFolder = Path.Combine(AssemblyHelper.GetBaseDirectory(), "Config");
        private readonly string configExtension = ".config";

        public Encoding DefaultEncode
        {
            get { return InternalConstant.DefaultEncode; }
        }

        public string GetConfig(string fileName)
        {
            if (!Directory.Exists(configFolder))
                Directory.CreateDirectory(configFolder);

            var configPath = GetFilePath(fileName);
            if (!File.Exists(configPath))
                return null;
            else
                return File.ReadAllText(configPath, this.DefaultEncode);
        }

        public void SaveConfig(string fileName, string content)
        {
            var configPath = GetFilePath(fileName);
            File.WriteAllText(configPath, content, this.DefaultEncode);
        }

        public string GetFilePath(string fileName)
        {
            return string.Format(@"{0}\{1}{2}", configFolder, fileName, configExtension);
        }
    }
}
