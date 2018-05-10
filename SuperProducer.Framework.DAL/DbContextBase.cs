using SuperProducer.Framework.Model;
using SuperProducer.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SqlClient;
using SuperProducer.Framework.Model.Attributes;

namespace SuperProducer.Framework.DAL
{
    /// <summary>
    /// 实现Repository通用泛型数据访问模式
    /// </summary>
    public class DbContextBase : DbContext, IDataRepository, IDisposable
    {
        public bool IsDisposable { get; set; }

        public IModelChangeLog ModelChangeLogger { get; set; }

        public DbContextBase(string connectionString)
        {
            //var objectContext = (this as IObjectContextAdapter).ObjectContext;
            //objectContext.CommandTimeout = 500;

            this.Database.Connection.ConnectionString = connectionString;
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbContextBase(string connectionString, IModelChangeLog modelChangeLogger) : this(connectionString)
        {
            this.ModelChangeLogger = modelChangeLogger;
        }


        /// <summary>
        /// 增加实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响的行数</returns>
        public virtual T Insert<T>(T entity) where T : ModelBase
        {
            this.Set<T>().Add(entity);
            this.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 增加实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回新增的实体</returns>
        public virtual int Insert<T>(IEnumerable<T> entity) where T : ModelBase
        {
            foreach (var item in entity)
            {
                this.Set<T>().Add(item);
            }
            return this.SaveChanges();
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回是否成功删除</returns>
        public virtual bool Delete<T>(T entity) where T : ModelBase
        {
            this.Entry<T>(entity).State = EntityState.Deleted;
            return this.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响的行数</returns>
        public virtual int Delete<T>(IEnumerable<T> entity) where T : ModelBase
        {
            foreach (var item in entity)
            {
                this.Entry<T>(item).State = EntityState.Deleted;
            }
            return this.SaveChanges();
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回修改后的实体</returns>
        public virtual T Update<T>(T entity) where T : ModelBase
        {
            this.Set<T>().Attach(entity);
            this.Entry<T>(entity).State = EntityState.Modified;
            this.SaveChanges();
            return entity;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="entity">实体</param>
        /// <returns>返回受影响的行数</returns>
        public virtual int Update<T>(IEnumerable<T> entity) where T : ModelBase
        {
            foreach (var item in entity)
            {
                this.Set<T>().Attach(item);
                this.Entry<T>(item).State = EntityState.Modified;
            }
            return this.SaveChanges();
        }

        /// <summary>
        /// 查询数据[主键]
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="keyValues">主键值</param>
        /// <returns>找到则返回实体，未找到返回NULL</returns>
        public virtual T Find<T>(params object[] keyValues) where T : ModelBase
        {
            return this.Set<T>().Find(keyValues);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="conditions">查询条件</param>
        /// <returns></returns>
        public virtual List<T> FindAll<T>(Expression<Func<T, bool>> conditions = null) where T : ModelBase
        {
#if DEBUG
            var sql = conditions == null ? this.Set<T>().ToString() : this.Set<T>().Where(conditions).ToString();
#endif

            if (conditions == null)
                return this.Set<T>().ToList();
            else
                return this.Set<T>().Where(conditions).ToList();
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="conditions">查询条件</param>
        /// <param name="takeCount">取前几条记录</param>
        /// <returns></returns>
        public virtual List<T> FindAll<T>(Expression<Func<T, bool>> conditions = null, int takeCount = 0) where T : ModelBase
        {
#if DEBUG
            var sql = ConvertHelper.GetString(conditions != null && takeCount > 0
                ? this.Set<T>().Where(conditions).Take(takeCount)
                : conditions != null
                ? this.Set<T>().Where(conditions)
                : takeCount > 0
                ? this.Set<T>().Take(takeCount)
                : null);
#endif

            var queryableInfo = conditions == null
                ? this.Set<T>()
                : this.Set<T>().Where(conditions);

            queryableInfo = takeCount > 0
                ? queryableInfo.Take(takeCount)
                : queryableInfo;

            return queryableInfo.ToList();
        }

        /// <summary>
        /// 查询数据[仅支持单个属性倒序查找]
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">分页索引</param>
        /// <returns></returns>
        public virtual PagedList<T> FindAllByPage<T, S>(Expression<Func<T, bool>> conditions, Expression<Func<T, S>> orderBy, int pageIndex, int pageSize) where T : ModelBase
        {
            var queryableList = conditions == null ? this.Set<T>() : this.Set<T>().Where(conditions);
            return queryableList.OrderByDescending(orderBy).ToPagedList(pageIndex, pageSize);
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="T">继承ModelBase的实体类</typeparam>
        /// <param name="conditions">查询条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="takeCount">取前几条记录</param>
        /// <returns></returns>
        public virtual List<T> FindAll<T>(Expression<Func<T, bool>> conditions, Action<IOrderable<T>> orderBy = null, int takeCount = 0) where T : ModelBase
        {
            var queryableInfo = conditions == null
                ? this.Set<T>()
                : this.Set<T>().Where(conditions);

            var orderableInfo = new Orderable<T>(queryableInfo);

            orderBy(orderableInfo);

            if (takeCount > 0)
                queryableInfo = orderableInfo.Queryable.Take(takeCount);
            else
                queryableInfo = orderableInfo.Queryable;

            return queryableInfo.ToList();
        }

        public override int SaveChanges()
        {
            this.WriteAuditableLog();

            var result = base.SaveChanges();
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.ModelChangeLogger != null)
            {
                this.ModelChangeLogger.Dispose();
            }
            base.Dispose(disposing);

            this.IsDisposable = true;
        }


        internal void WriteAuditableLog()
        {
            if (this.ModelChangeLogger == null)
                return;

            foreach (var dbEntry in this.ChangeTracker.Entries<ModelBase>().Where(p => p.State == EntityState.Added || p.State == EntityState.Deleted || p.State == EntityState.Modified))
            {
                var modelChangeAttrs = dbEntry.Entity.GetType().GetCustomAttributes(typeof(ModelChangeLogAttribute), false).SingleOrDefault() as ModelChangeLogAttribute;
                if (modelChangeAttrs == null)
                    continue;

                var operaterName = new Operater().Name; //WCFContext.Current.Operater.Name

                TaskHelper.TimeStartAsync((args) =>
                {
                    var tableAttrs = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;
                    var tableName = tableAttrs != null ? tableAttrs.Name : dbEntry.Entity.GetType().Name;
                    var moduleName = dbEntry.Entity.GetType().FullName;

                    this.ModelChangeLogger.WriteLog(args.ToString(), moduleName, tableName, dbEntry.Entity.ID, dbEntry.Entity, dbEntry.State.ToString(), string.Empty);
                }, operaterName);
            }
        }
    }
}
