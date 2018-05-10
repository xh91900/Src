using SuperProducer.Framework.DAL;
using SuperProducer.Framework.Model.Res;
using System.Collections.Generic;
using System.Linq;

namespace SuperProducer.Framework.BLL.ExtendType
{
    public static class ExtendTypeCommonInfo
    {
        public static string GetSystemExtendTypeDataKeyByRemark(long typeID, string dataRemark)
        {
            var dataList = GetSystemExtendTypeData(typeID, dataRemark: dataRemark);
            if (dataList != null && dataList.Count == 1)
            {
                return dataList.FirstOrDefault().DataKey;
            }
            return string.Empty;
        }

        public static string GetSystemExtendTypeDataKeyByValue(long typeID, string dataValue)
        {
            var dataList = GetSystemExtendTypeData(typeID, dataValue: dataValue);
            if (dataList != null && dataList.Count == 1)
            {
                return dataList.FirstOrDefault().DataKey;
            }
            return string.Empty;
        }

        public static string GetSystemExtendTypeDataValueByKey(long typeID, string dataKey)
        {
            var dataList = GetSystemExtendTypeData(typeID, dataKey: dataKey);
            if (dataList != null && dataList.Count == 1)
            {
                return dataList.FirstOrDefault().DataValue;
            }
            return string.Empty;
        }

        public static string GetSystemExtendTypeDataRemarkByKey(long typeID, string dataKey)
        {
            var dataList = GetSystemExtendTypeData(typeID, dataKey: dataKey);
            if (dataList != null && dataList.Count == 1)
            {
                return dataList.FirstOrDefault().DataRemark;
            }
            return string.Empty;
        }

        public static List<SystemExtendTypeData> GetSystemExtendTypeData(long typeID, string dataKey = null, string dataValue = null, string dataRemark = null, byte platformType = 0)
        {
            using (var resContext = new ResDbContext())
            {
                var query = resContext.SystemExtendTypeData.Where(item => item.IsDel == false && item.PlatformType == platformType && item.TypeID == typeID);

                if (!string.IsNullOrEmpty(dataKey))
                    query = query.Where(item => item.DataKey == dataKey);

                if (!string.IsNullOrEmpty(dataValue))
                    query = query.Where(item => item.DataValue == dataValue);

                if (!string.IsNullOrEmpty(dataRemark))
                    query = query.Where(item => item.DataRemark == dataRemark);

                if (platformType > 0)
                    query = query.Where(item => item.PlatformType == platformType);

                return query.ToList();
            }
        }

        public static List<SystemExtendTypeInfo> GetSystemExtendTypeInfo(long typeID = 0, string typeCode = null, byte platformType = 0)
        {
            using (var resContext = new ResDbContext())
            {
                var query = resContext.SystemExtendTypeInfo.Where(item => item.IsDel == false);

                if (typeID > 0)
                    query = query.Where(item => item.ID == typeID);

                if (!string.IsNullOrEmpty(typeCode))
                    query = query.Where(item => item.TypeCode == typeCode);

                if (platformType > 0)
                    query = query.Where(item => item.PlatformType == platformType);

                return query.ToList();
            }
        }
    }
}
