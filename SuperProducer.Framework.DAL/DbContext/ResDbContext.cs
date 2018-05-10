using SuperProducer.Core.Config;
using SuperProducer.Framework.DAL;
using SuperProducer.Framework.Model.Res;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperProducer.Framework.DAL
{
    /// <summary>
    /// 支持多语言
    /// </summary>
    public class ResDbContext : DbContextBase
    {
        public ResDbContext() : base(CachedFileConfigContext.Current.DaoConfig.Res)
        {
            Database.SetInitializer(new NullDatabaseInitializer<ResDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region "Fluent API"

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            #endregion
        }



        public DbSet<ProgStringInfo> ProgStringInfo { get; set; }

        public DbSet<ProgWarningReceiver> ProgWarningReceiver { get; set; }

        public DbSet<SystemExtendTypeInfo> SystemExtendTypeInfo { get; set; }

        public DbSet<SystemExtendTypeData> SystemExtendTypeData { get; set; }
    }
}
