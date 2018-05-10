using SuperProducer.Framework.DAL;
using SuperProducer.Framework.Model.Res;
using System.Linq;

namespace SuperProducer.Framework.BLL.ProgString
{
    public static class ProgStringCommonInfo
    {
        public static string GetProgStringValue(byte platformType, string stringKey)
        {
            var result = GetProgString(platformType, stringKey);
            if (result != null)
            {
                return result.StringValue;
            }
            return string.Empty;
        }

        public static string GetProgStringRemark(byte platformType, string stringKey)
        {
            var result = GetProgString(platformType, stringKey);
            if (result != null)
            {
                return result.Remark;
            }
            return string.Empty;
        }

        public static ProgStringInfo GetProgString(byte platformType, string stringKey)
        {
            using (var resContext = new ResDbContext())
            {
                return resContext.ProgStringInfo.Where(item => item.IsDel == false && item.PlatformType == platformType && item.StringKey == stringKey).FirstOrDefault();
            }
        }
    }
}
