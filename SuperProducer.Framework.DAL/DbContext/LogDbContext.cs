using System.Data.Entity;
using SuperProducer.Framework.Model;
using Newtonsoft.Json;
using SuperProducer.Core.Config;
using System.Data.Entity.ModelConfiguration.Conventions;
using SuperProducer.Framework.Model.Log;

namespace SuperProducer.Framework.DAL
{
    public class LogDbContext : DbContextBase, IModelChangeLog
    {
        public LogDbContext() : base(CachedFileConfigContext.Current.DaoConfig.Log)
        {
            Database.SetInitializer(new NullDatabaseInitializer<LogDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region "Fluent API"

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #endregion
        }

        public DbSet<ModelChangeLog> ModelChangeLog { get; set; }

        public void WriteLog(string userName, string moduleName, string tableName, long modelID, ModelBase modelValue, string eventType, string remark)
        {
            try
            {
                this.ModelChangeLog.Add(new ModelChangeLog()
                {
                    UserName = userName,
                    ModuleName = moduleName,
                    TableName = tableName,
                    ModelID = modelID,
                    ModelValue = JsonConvert.SerializeObject(modelValue, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                    EventType = eventType,
                    Remark = remark,
                });
                this.SaveChanges();
            }
            catch { }
        }
    }
}
