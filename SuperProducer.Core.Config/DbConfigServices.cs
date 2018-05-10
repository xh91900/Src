using SuperProducer.Core.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Linq;
using System;

namespace SuperProducer.Core.Config
{
    public class DbConfigServices : IConfigService
    {
        private enum SQLStringKey
        {
            HasConfig,
            SelectConfig,
            InsertConfig,
            UpdateConfig
        }

        private readonly string configTableName = "[dbo].[SystemConfigInfo]";

        private readonly Dictionary<string, string> configSqlString = new Dictionary<string, string>()
        {
            { SQLStringKey.HasConfig.ToString(), "SELECT ConfigKey, ConfigValue, ConfigRemark FROM {0} WHERE ConfigKey = @ConfigKey AND IsDel = 0" },
            { SQLStringKey.SelectConfig.ToString(), "SELECT TOP 1 COUNT(ID) FROM {0} WHERE ConfigKey = @ConfigKey AND IsDel = 0" },
            { SQLStringKey.InsertConfig.ToString(), "INSERT INTO {0}(ConfigName, ConfigKey, ConfigValue, ConfigRemark) VALUES(@ConfigName, @ConfigKey, @ConfigValue, @ConfigRemark)" },
            { SQLStringKey.UpdateConfig.ToString(), "UPDATE {0} SET ConfigValue=@ConfigValue WHERE ConfigKey=@ConfigKey" },
        };

        public DbConfigServices()
        {
            this.InitConfigSqlString();
        }

        private void InitConfigSqlString()
        {
            if (configSqlString == null)
            {
                foreach (var item in configSqlString)
                {
                    configSqlString[item.Key] = string.Format(item.Value, configTableName);
                }
            }
        }

        public string GetConfig(string name)
        {
            var retVal = string.Empty;

            try
            {
                var sqlString = configSqlString.GetValue(SQLStringKey.SelectConfig.ToString());
                using (DataTable dtlInfo = new DataTable())
                {
                    using (SqlCommand command = GetSqlCommand(sqlString))
                    {
                        if (command.Connection.State != ConnectionState.Open)
                        {
                            command.Connection.Open();
                        }

                        command.Parameters.Add(new SqlParameter("@ConfigKey", name));

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dtlInfo);
                        }
                    }

                    if (dtlInfo != null && dtlInfo.Rows.Count > 0)
                    {
                        retVal = ConvertHelper.GetString(dtlInfo.Rows[0]["ConfigValue"]);
                    }
                }
            }
            catch { }
            return retVal;
        }

        public void SaveConfig(string name, string content)
        {
            try
            {
                var nameDescription = this.GetTypeDescription(name);
                if (!string.IsNullOrEmpty(nameDescription))
                {
                    var sqlString = configSqlString.GetValue(SQLStringKey.HasConfig.ToString());

                    using (SqlCommand command = this.GetSqlCommand(sqlString))
                    {
                        if (command.Connection.State != ConnectionState.Open)
                        {
                            command.Connection.Open();
                        }

                        command.Parameters.Add(new SqlParameter("@ConfigKey", name));

                        sqlString = configSqlString.GetValue(SQLStringKey.InsertConfig.ToString());

                        if (ConvertHelper.GetLong(command.ExecuteScalar(), 0) > 0)
                            sqlString = configSqlString.GetValue(SQLStringKey.UpdateConfig.ToString());

                        command.CommandText = sqlString;

                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@ConfigName", nameDescription));
                        command.Parameters.Add(new SqlParameter("@ConfigKey", name));
                        command.Parameters.Add(new SqlParameter("@ConfigValue", content));
                        command.Parameters.Add(new SqlParameter("@ConfigRemark", DBNull.Value));

                        if (command.ExecuteNonQuery() > 0)
                        {

                        }
                    }
                }
            }
            catch { }
        }

        private string GetTypeDescription(string targetTypeName)
        {
            var allTypes = this.GetType().Assembly.GetLoadableTypes();
            if (allTypes != null)
            {
                var targetType = allTypes.Where(item => item.Name == targetTypeName).FirstOrDefault();
                if (targetType != null)
                {
                    var allAttrs = targetType.GetCustomAttributes<DescriptionAttribute>(false);
                    if (allAttrs != null && allAttrs.Count() > 0)
                    {
                        return allAttrs.FirstOrDefault().Description;
                    }
                }
            }
            return null;
        }

        public SqlCommand GetSqlCommand(string sqlString)
        {
            if (string.IsNullOrEmpty(sqlString))
            {
                sqlString = configSqlString.GetValue(SQLStringKey.SelectConfig.ToString());
            }

            using (SqlConnection connection = new SqlConnection(CachedFileConfigContext.Current.DaoConfig.Main))
            {
                using (SqlCommand command = new SqlCommand(sqlString, connection))
                {
                    return command;
                }
            }
        }
    }
}
