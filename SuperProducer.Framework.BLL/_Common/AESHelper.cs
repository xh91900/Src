using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperProducer.Core.Config;
using SuperProducer.Core.Utility.Encrypt;

namespace SuperProducer.Framework.BLL.Common
{
    public static class AESHelper
    {
        /// <summary>
        /// AES全局实例[用户密码加密]
        /// </summary>
        public static AES InstanceForUserPassword { get; set; }

        /// <summary>
        /// AES全局实例[普通性加密]
        /// </summary>
        public static AES InstanceForUniversal { get; set; }

        static AESHelper()
        {
            var aesKey = string.Empty;

            aesKey = Core.Utility.InternalConstant.AESKey;
            if (CachedDbConfigContext.Current != null && CachedDbConfigContext.Current.SystemConfig != null)
                aesKey = CachedDbConfigContext.Current.SystemConfig.AESKey_UserPassword;
            InstanceForUserPassword = new AES(aesKey, format: AES.OutputFormat.Base64);

            aesKey = Core.Utility.InternalConstant.AESKey;
            if (CachedDbConfigContext.Current != null && CachedDbConfigContext.Current.SystemConfig != null)
                aesKey = CachedDbConfigContext.Current.SystemConfig.AESKey_Universal;
            InstanceForUniversal = new AES(aesKey, format: AES.OutputFormat.Base64);
        }
    }
}
